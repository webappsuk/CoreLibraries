#region © Copyright Web Applications (UK) Ltd, 2015.  All rights reserved.
// Copyright (c) 2015, Web Applications UK Ltd
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of Web Applications UK Ltd nor the
//       names of its contributors may be used to endorse or promote products
//       derived from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL WEB APPLICATIONS UK LTD BE LIABLE FOR ANY
// DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Configuration;
using WebApplications.Utilities.Database.Configuration;
using WebApplications.Utilities.Threading;

namespace WebApplications.Utilities.Database
{
    internal static class ConcurrencyController
    {
        /// <summary>
        /// The lock used to ensure only a single config update will be performed at a time.
        /// </summary>
        [NotNull]
        private static readonly object _updatedLock = new object();

        [NotNull]
        private static readonly ConcurrentDictionary<string, AsyncSemaphore> _databaseSemaphores =
            new ConcurrentDictionary<string, AsyncSemaphore>();

        [NotNull]
        private static readonly ConcurrentDictionary<Id, AsyncSemaphore> _loadBalancedConnectionSemaphores =
            new ConcurrentDictionary<Id, AsyncSemaphore>();

        [NotNull]
        private static readonly ConcurrentDictionary<ConnectionId, AsyncSemaphore> _connectionSemaphores =
            new ConcurrentDictionary<ConnectionId, AsyncSemaphore>();

        [NotNull]
        private static readonly ConcurrentDictionary<Id, AsyncSemaphore> _programSemaphores =
            new ConcurrentDictionary<Id, AsyncSemaphore>();

        /// <summary>
        /// Called when the databse configuartion changes.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="ConfigurationSection{T}.ConfigurationChangedEventArgs"/> instance containing the event data.</param>
        public static void OnActiveConfigChanged(
            [NotNull] DatabasesConfiguration sender,
            [NotNull] ConfigurationSection<DatabasesConfiguration>.ConfigurationChangedEventArgs args)
        {
            UpdateSemaphores(sender);
        }

        /// <summary>
        /// Updates the semaphores.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public static void UpdateSemaphores(DatabasesConfiguration config)
        {
            HashSet<string> databases = new HashSet<string>();
            HashSet<Id> loadBalancedConns = new HashSet<Id>();
            HashSet<ConnectionId> connections = new HashSet<ConnectionId>();
            HashSet<Id> programs = new HashSet<Id>();

            lock (_updatedLock)
            {
                // Add or update the semaphores for the values in the config
                foreach (DatabaseElement db in config.Databases)
                {
                    Debug.Assert(db != null);

                    string databaseId = db.Id;

                    Debug.Assert(!databases.Contains(databaseId));
                    databases.Add(databaseId);

                    UpdateSemaphore(db.MaximumConcurrency, databaseId, _databaseSemaphores);

                    foreach (LoadBalancedConnectionElement lbConnection in db.Connections)
                    {
                        Debug.Assert(lbConnection != null);

                        Id lbcId = new Id(databaseId, lbConnection.Id);

                        Debug.Assert(!loadBalancedConns.Contains(lbcId));
                        loadBalancedConns.Add(lbcId);

                        UpdateSemaphore(lbConnection.MaximumConcurrency, lbcId, _loadBalancedConnectionSemaphores);

                        foreach (IGrouping<ConnectionId, KeyValuePair<ConnectionId, int>> conn in lbConnection.Connections
                            .Select(
                                c => new KeyValuePair<ConnectionId, int>(
                                    // ReSharper disable once PossibleNullReferenceException
                                    new ConnectionId(lbcId, c.ConnectionString),
                                    c.MaximumConcurrency))
                            .GroupBy(c => c.Key))
                        {
                            Debug.Assert(conn != null);

                            ConnectionId connectionId = conn.Key;

                            int maxConcurrency = conn.Aggregate(
                                (int?)null,
                                (mc, c) =>
                                {
                                    if (mc < 1) return mc;
                                    if (c.Value < 1) return -1;
                                    return (mc ?? 0) + c.Value;
                                }) ?? -1;

                            connections.Add(connectionId);

                            UpdateSemaphore(maxConcurrency, connectionId, _connectionSemaphores);
                        }
                    }

                    foreach (ProgramElement program in db.Programs)
                    {
                        Debug.Assert(program != null);

                        Id programId = new Id(databaseId, program.Name);

                        Debug.Assert(!programs.Contains(programId));
                        programs.Add(programId);

                        UpdateSemaphore(program.MaximumConcurrency, programId, _programSemaphores);
                    }
                }

                // Remove any semaphores that are no longer in the configuration
                foreach (string id in _databaseSemaphores.Keys)
                {
                    Debug.Assert(id != null);
                    
                    if (databases.Contains(id) || !_databaseSemaphores.TryRemove(id, out AsyncSemaphore semaphore))
                        continue;

                    Debug.Assert(semaphore != null);
                    semaphore.MaxCount = int.MaxValue;
                }
                foreach (Id id in _loadBalancedConnectionSemaphores.Keys)
                {
                    if (loadBalancedConns.Contains(id) || !_loadBalancedConnectionSemaphores.TryRemove(id, out AsyncSemaphore semaphore))
                        continue;

                    Debug.Assert(semaphore != null);
                    semaphore.MaxCount = int.MaxValue;
                }
                foreach (ConnectionId id in _connectionSemaphores.Keys)
                {
                    if (connections.Contains(id) || !_connectionSemaphores.TryRemove(id, out AsyncSemaphore semaphore))
                        continue;

                    Debug.Assert(semaphore != null);
                    semaphore.MaxCount = int.MaxValue;
                }
                foreach (Id id in _programSemaphores.Keys)
                {
                    if (programs.Contains(id) || !_programSemaphores.TryRemove(id, out AsyncSemaphore semaphore))
                        continue;

                    Debug.Assert(semaphore != null);
                    semaphore.MaxCount = int.MaxValue;
                }
            }
        }

        /// <summary>
        /// Updates the maximum count for the semaphore with the given ID.
        /// </summary>
        /// <typeparam name="TId">The type of the identifier.</typeparam>
        /// <param name="maximumConcurrency">The maximum concurrency.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="semaphores">The semaphores.</param>
        private static void UpdateSemaphore<TId>(
            int maximumConcurrency,
            [NotNull] TId id,
            [NotNull] ConcurrentDictionary<TId, AsyncSemaphore> semaphores)
        {
            if (maximumConcurrency < 1)
            {
                if (semaphores.TryRemove(id, out AsyncSemaphore semaphore))
                {
                    Debug.Assert(semaphore != null);
                    semaphore.MaxCount = int.MaxValue;
                }
            }
            else
                semaphores.AddOrUpdate(
                    id,
                    _ => new AsyncSemaphore(maximumConcurrency),
                    (_, s) =>
                    {
                        Debug.Assert(s != null);
                        s.MaxCount = maximumConcurrency;
                        return s;
                    });
        }

        /// <summary>
        /// Gets the semaphore for the specified database.
        /// </summary>
        /// <param name="databaseId">The database identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="databaseId"/> was null.</exception>
        [CanBeNull]
        public static AsyncSemaphore GetDatabaseSemaphore([NotNull] string databaseId)
        {
            if (databaseId == null) throw new ArgumentNullException(nameof(databaseId));
            
            _databaseSemaphores.TryGetValue(databaseId, out AsyncSemaphore semaphore);
            return semaphore;
        }

        /// <summary>
        /// Gets the semaphore for the specified load balanced connection.
        /// </summary>
        /// <param name="databaseId">The database identifier.</param>
        /// <param name="connectionId">The connection identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="databaseId"/> or <paramref name="connectionId"/> was null.</exception>
        [CanBeNull]
        public static AsyncSemaphore GetLoadBalancedConnectionSemaphore(
            [NotNull] string databaseId,
            [NotNull] string connectionId)
        {
            if (databaseId == null) throw new ArgumentNullException(nameof(databaseId));
            if (connectionId == null) throw new ArgumentNullException(nameof(connectionId));

            _loadBalancedConnectionSemaphores.TryGetValue(
                new Id(databaseId, connectionId),
                out AsyncSemaphore semaphore);
            return semaphore;
        }

        /// <summary>
        /// Gets the semaphore for the specified connection.
        /// </summary>
        /// <param name="databaseId">The database identifier.</param>
        /// <param name="connectionId">The connection identifier.</param>
        /// <param name="connection">The connection.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="databaseId"/> or <paramref name="connectionId"/> or <paramref name="connection"/> was null.</exception>
        [CanBeNull]
        public static AsyncSemaphore GetConnectionSemaphore(
            [NotNull] string databaseId,
            [NotNull] string connectionId,
            [NotNull] Connection connection)
        {
            if (databaseId == null) throw new ArgumentNullException(nameof(databaseId));
            if (connectionId == null) throw new ArgumentNullException(nameof(connectionId));
            if (connection == null) throw new ArgumentNullException(nameof(connection));

            _connectionSemaphores.TryGetValue(
                new ConnectionId(databaseId, connectionId, connection),
                out AsyncSemaphore semaphore);
            return semaphore;
        }

        /// <summary>
        /// Gets the semaphore for the specified program.
        /// </summary>
        /// <param name="databaseId">The database identifier.</param>
        /// <param name="programName">Name of the program.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="databaseId"/> or <paramref name="programName"/> was null.</exception>
        [CanBeNull]
        public static AsyncSemaphore GetProgramSemaphore(
            [NotNull] string databaseId,
            [NotNull] string programName)
        {
            if (databaseId == null) throw new ArgumentNullException(nameof(databaseId));
            if (programName == null) throw new ArgumentNullException(nameof(programName));

            _programSemaphores.TryGetValue(new Id(databaseId, programName), out AsyncSemaphore semaphore);
            return semaphore;
        }

        private struct Id : IEquatable<Id>
        {
            /// <summary>
            /// The database identifier
            /// </summary>
            public readonly string DatabaseId;

            /// <summary>
            /// The object identifier
            /// </summary>
            public readonly string ObjectId;

            /// <summary>
            /// Initializes a new instance of the <see cref="Id"/> struct.
            /// </summary>
            /// <param name="databaseId">The database identifier.</param>
            /// <param name="objectId">The object identifier.</param>
            public Id(string databaseId, string objectId)
            {
                DatabaseId = databaseId;
                ObjectId = objectId;
            }

            /// <summary>
            /// Equalses the specified other.
            /// </summary>
            /// <param name="other">The other.</param>
            /// <returns></returns>
            public bool Equals(Id other)
            {
                return string.Equals(DatabaseId, other.DatabaseId) && string.Equals(ObjectId, other.ObjectId);
            }

            /// <summary>
            /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
            /// </summary>
            /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
            /// <returns>
            ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
            /// </returns>
            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is Id && Equals((Id)obj);
            }

            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <returns>
            /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
            /// </returns>
            public override int GetHashCode()
            {
                unchecked
                {
                    return ((DatabaseId != null ? DatabaseId.GetHashCode() : 0) * 397) ^
                           (ObjectId != null ? ObjectId.GetHashCode() : 0);
                }
            }
        }

        private struct ConnectionId : IEquatable<ConnectionId>
        {
            /// <summary>
            /// The load balanced connection identifier
            /// </summary>
            public readonly Id LoadBalancedConnectionId;

            /// <summary>
            /// The connection string
            /// </summary>
            public readonly string ConnectionString;

            /// <summary>
            /// Initializes a new instance of the <see cref="ConnectionId" /> struct.
            /// </summary>
            /// <param name="databaseId">The database identifier.</param>
            /// <param name="loadBalancedConnectionId">The load balanced connection identifier.</param>
            /// <param name="connection">The connection.</param>
            public ConnectionId(string databaseId, string loadBalancedConnectionId, [NotNull] Connection connection)
            {
                LoadBalancedConnectionId = new Id(databaseId, loadBalancedConnectionId);
                ConnectionString = connection.ConnectionString;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ConnectionId"/> struct.
            /// </summary>
            /// <param name="loadBalancedConnectionId">The load balanced connection identifier.</param>
            /// <param name="connectionString">The connection string.</param>
            public ConnectionId(Id loadBalancedConnectionId, string connectionString)
            {
                LoadBalancedConnectionId = loadBalancedConnectionId;
                ConnectionString = Connection.NormalizeConnectionString(connectionString);
            }

            /// <summary>
            /// Equalses the specified other.
            /// </summary>
            /// <param name="other">The other.</param>
            /// <returns></returns>
            public bool Equals(ConnectionId other)
            {
                return LoadBalancedConnectionId.Equals(other.LoadBalancedConnectionId) &&
                       string.Equals(ConnectionString, other.ConnectionString);
            }

            /// <summary>
            /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
            /// </summary>
            /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
            /// <returns>
            ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
            /// </returns>
            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is ConnectionId && Equals((ConnectionId)obj);
            }

            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <returns>
            /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
            /// </returns>
            public override int GetHashCode()
            {
                unchecked
                {
                    return (LoadBalancedConnectionId.GetHashCode() * 397) ^
                           (ConnectionString != null ? ConnectionString.GetHashCode() : 0);
                }
            }
        }
    }
}