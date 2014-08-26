#region © Copyright Web Applications (UK) Ltd, 2012.  All rights reserved.
// Copyright (c) 2012, Web Applications UK Ltd
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
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WebApplications.Utilities.Database.Schema;
using WebApplications.Utilities.Enumerations;
using WebApplications.Utilities.Logging;
using WebApplications.Utilities.Threading;

namespace WebApplications.Utilities.Database
{
    /// <summary>
    ///   Allows the specification of a set of connection strings to be selected at random based on a weighting.
    /// </summary>
    public class LoadBalancedConnection : IEnumerable<Connection>
    {
        /// <summary>
        ///   The random number generator, used for choosing the connection.
        /// </summary>
        [NotNull]
        private static readonly Random _random = new Random();

        /// <summary>
        ///   Holds <see cref="Connection"/>s and their <see cref="Connection.Weight">weighting</see>.
        /// </summary>
        [NotNull]
        private readonly List<Connection> _connections = new List<Connection>();

        /// <summary>
        ///   The total weight of all the <see cref="Connection"/>s.
        /// </summary>
        private readonly double _totalWeight;

        /// <summary>
        /// The lock for checking schemas match.
        /// </summary>
        [NotNull]
        private readonly AsyncLock _lock = new AsyncLock();

        /// <summary>
        ///   Whether schemas are identical.
        /// </summary>
        private TriState _identicalSchemas = TriState.Unknown;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadBalancedConnection" /> class.
        /// This creates a load balanced connection with only one connection string.
        /// </summary>
        /// <param name="connectionString"><para>The connection string.</para>
        /// <para>This is given a weighting of 1.0.</para></param>
        /// <param name="weight">The weight.</param>
        /// <param name="ensureSchemasIdentical"><para>If set to <see langword="true" /> then the schemas must be identical.</para>
        /// <para>By default this is <see langword="false" />.</para></param>
        /// <exception cref="LoggingException"><paramref name="connectionString" /> is <see langword="null" />.</exception>
        /// <remarks>There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> specifying that
        /// <paramref name="connectionString" /> cannot be <see langword="null" />.</remarks>
        [PublicAPI]
        public LoadBalancedConnection([NotNull] string connectionString, double weight = 1D, bool ensureSchemasIdentical = false)
            : this(new[] { new Connection(connectionString, weight) }, ensureSchemasIdentical)
        {
            Contract.Requires(connectionString != null);
            Contract.Requires(weight > 0D);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="LoadBalancedConnection"/> class.
        ///   This creates an evenly balanced set of connections.
        /// </summary>
        /// <param name="connectionStrings">
        ///   The connection strings, which all have a weighting of 1.0.
        /// </param>
        /// <remarks>
        ///   There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> specifying that
        ///   <paramref name="connectionStrings"/> cannot be <see langword="null"/>.
        /// </remarks>
        /// <exception cref="LoggingException">
        ///   No connection strings specified in <paramref name="connectionStrings"/> or all strings were <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="connectionStrings"/> was <see langword="null"/>.
        /// </exception>
        [PublicAPI]
        public LoadBalancedConnection([NotNull] params string[] connectionStrings)
            : this(connectionStrings
                       .Where(cs => !string.IsNullOrWhiteSpace(cs))
                // ReSharper disable once AssignNullToNotNullAttribute
                       .Select(cs => new Connection(cs)))
        {
            Contract.Requires(connectionStrings != null);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="LoadBalancedConnection"/> class.
        ///   This creates an evenly balanced set of connections.
        /// </summary>
        /// <param name="connectionStrings">The connection strings.</param>
        /// <param name="ensureSchemasIdentical">
        ///   <para>If set to <see langword="true"/> then the schemas must be identical.</para>
        ///   <para>By default this is <see langword="false"/>.</para>
        /// </param>
        /// <remarks>
        ///   There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> specifying that
        ///   <paramref name="connectionStrings"/> cannot be <see langword="null"/>.
        /// </remarks>
        /// <exception cref="LoggingException">
        ///   <para>No connection strings specified in <paramref name="connectionStrings"/>
        ///   or all strings were <see langword="null"/>.</para>
        ///   <para>-or-</para>
        ///   <para>Connections do not have identical schemas
        ///   and <paramref name="ensureSchemasIdentical"/> is <see langword="true"/>.</para>
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="connectionStrings"/> was <see langword="null"/>.
        /// </exception>
        [PublicAPI]
        public LoadBalancedConnection(
            [NotNull] IEnumerable<string> connectionStrings,
            bool ensureSchemasIdentical = false)
            : this(connectionStrings
                       .Where(cs => !string.IsNullOrWhiteSpace(cs))
                // ReSharper disable once AssignNullToNotNullAttribute
                       .Select(cs => new Connection(cs)),
                   ensureSchemasIdentical)
        {
            Contract.Requires(connectionStrings != null);
        }

        /// <summary>
        ///   <para>Initializes a new instance of the <see cref="LoadBalancedConnection"/> class.
        ///   This is allows a weighting to be set for each connection string.</para>
        ///   <para>The higher the weighting the more likely a connection string will be chosen when
        ///   requesting a new connection.</para>
        ///   <para>The connection can be given a unique id for lookup.</para>
        /// </summary>
        /// <param name="connectionStrings">The connection strings.</param>
        /// <param name="ensureSchemasIdentical">
        ///   <para>If set to <see langword="true"/> then the schemas must be identical.</para>
        ///   <para>By default this is <see langword="false"/>.</para>
        /// </param>
        /// <remarks>
        ///   There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> specifying that
        ///   <paramref name="connectionStrings"/> cannot be <see langword="null"/>.
        /// </remarks>
        /// <exception cref="LoggingException">
        ///   <para><paramref name="connectionStrings"/> is <see langword="null"/>.</para>
        ///   <para>-or-</para>
        ///   <para>No connection string specified.</para>
        ///   <para>-or-</para>
        ///   <para>The weight of a connection string was less than zero.</para>
        ///   <para>-or-</para>
        ///   <para>All the <paramref name="connectionStrings"/> cannot be zero weighted.</para>
        ///   <para>-or-</para>
        ///   <para>Connections do not have identical schemas
        ///   and <paramref name="ensureSchemasIdentical"/> is <see langword="true"/>.</para>
        /// </exception>
        [PublicAPI]
        public LoadBalancedConnection(
            [NotNull] IEnumerable<KeyValuePair<string, double>> connectionStrings,
            bool ensureSchemasIdentical = false)
            // ReSharper disable once AssignNullToNotNullAttribute
            : this(connectionStrings
                       .Where(kvp => !string.IsNullOrWhiteSpace(kvp.Key))
                       // ReSharper disable AssignNullToNotNullAttribute
                       .Select(kvp => new Connection(kvp.Key, kvp.Value)),
                   // ReSharper restore AssignNullToNotNullAttribute
                   ensureSchemasIdentical)
        {
            Contract.Requires(connectionStrings != null);
        }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="LoadBalancedConnection" /> class.
        /// This is allows a weighting to be set for each connection string.</para>
        /// <para>The higher the weighting the more likely a connection string will be chosen when
        /// requesting a new connection.</para>
        /// <para>The connection can be given a unique id for lookup.</para>
        /// </summary>
        /// <param name="connections">The connections.</param>
        /// <param name="ensureSchemasIdentical"><para>If set to <see langword="true" /> then the schemas must be identical.</para>
        /// <para>By default this is <see langword="false" />.</para></param>
        [PublicAPI]
        public LoadBalancedConnection(
            [NotNull] IEnumerable<Connection> connections,
            bool ensureSchemasIdentical = false)
        {
            Contract.Requires(connections != null);

            // TODO Remove ensure schemas identical as should be async!

            // Combine identical connection strings and their weighting.
            foreach (Connection connection in connections)
            {
                if (connection == null) continue;
                // ReSharper disable once PossibleNullReferenceException
                Connection existing = _connections.FirstOrDefault(c => c.EquivalentTo(connection));
                if (existing != null)
                    existing.Weight += connection.Weight;
                else
                    _connections.Add(connection);
                _totalWeight += connection.Weight;
            }

            if (!_connections.Any())
                throw new LoggingException(
                    () =>
                        Resources.LoadBalancedConnection_NoConnectionStrings);

            // Throw an exception if we have any connection strings with a negative weighting.
            // ReSharper disable once PossibleNullReferenceException
            if (_connections.Any(c => c.Weight < 0))
                throw new LoggingException(
                    () => Resources.LoadBalancedConnection_WeightLessThanZero);

            if (_totalWeight <= 0.0D)
                throw new LoggingException(
                    () => Resources.LoadBalancedConnection_AllStringsZeroWeighted);

            // Finally validate the schemas are identical if required. TODO REMOVE!
            if ((ensureSchemasIdentical) &&
                (!IdenticalSchemas))
                throw new LoggingException(
                    () => Resources.LoadBalancedConnection_SchemasNotIdentical);
        }

        /// <summary>
        ///   Gets a <see cref="bool"/> value indicating whether schemas are identical.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if the schemas are identical; otherwise returns <see langword="false"/>.
        /// </value>
        public Task<bool> CheckIdentical(CancellationToken token = default(CancellationToken))
        {
            // TODO Make Async!!!!
            if (_identicalSchemas != TriState.Unknown) return (bool) _identicalSchemas;

            lock (_connections)
            {
                if (_identicalSchemas != TriState.Unknown) return (bool) _identicalSchemas;

                DatabaseSchema schema = null;
                _identicalSchemas = TriState.Yes;
                foreach (Connection connection in _connections)
                {
                    Contract.Assert(connection != null);
                    if (schema == null)
                        schema = DatabaseSchema.GetOrAdd(connection.ConnectionString);
                    else if (!schema.Equals(DatabaseSchema.GetOrAdd(connection.ConnectionString)))
                    {
                        _identicalSchemas = TriState.No;
                        break;
                    }
                }
            }

            return (bool) _identicalSchemas;
        }

        #region IEnumerable<string> Members
        /// <summary>
        ///   Returns an enumerator that allows iteration through the connection strings.
        /// </summary>
        /// <returns>
        ///   A <see cref = "T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<Connection> GetEnumerator()
        {
            return _connections.GetEnumerator();
        }

        /// <summary>
        ///   Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///   An <see cref = "T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        /// <summary>
        /// Gets a new <see cref="SqlConnection" /> using a random
        /// <see cref="Connection.ConnectionString">connection string</see>.
        /// </summary>
        /// <returns>A <see cref="SqlConnection" /> object.</returns>
        /// <exception cref="WebApplications.Utilities.Logging.LoggingException"></exception>
        /// <exception cref="LoggingException">Failed to select a valid connection string from the <see cref="LoadBalancedConnection" />.</exception>
        /// <exception cref="InvalidOperationException">The connection did not specify a source or server.</exception>
        /// <exception cref="SqlException">A connection-level error occurred whilst opening the connection.
        /// See <see cref="SqlConnection.Open" /> for more details.</exception>
        /// <remarks>This should be used in a <c>using</c> statement to ensure it is disposed
        /// as a connection is not closed when it goes out of scope.</remarks>
        [NotNull]
        public SqlConnection GetConnection()
        {
            // Create connection using a random connection string
            return new SqlConnection(GetConnectionString());
        }

        /// <summary>
        /// Gets a random
        /// <see cref="Connection.ConnectionString">connection string</see>.
        /// </summary>
        /// <returns>A <see cref="SqlConnection" /> object.</returns>
        /// <exception cref="WebApplications.Utilities.Logging.LoggingException"></exception>
        /// <exception cref="LoggingException">Failed to select a valid connection string from the <see cref="LoadBalancedConnection" />.</exception>
        /// <exception cref="InvalidOperationException">The connection did not specify a source or server.</exception>
        /// <exception cref="SqlException">A connection-level error occurred whilst opening the connection.
        /// See <see cref="SqlConnection.Open" /> for more details.</exception>
        /// <remarks>This should be used in a <c>using</c> statement to ensure it is disposed
        /// as a connection is not closed when it goes out of scope.</remarks>
        [NotNull]
        public string GetConnectionString()
        {
            // Calculate a random value between 0 and the total weight.
            double next = _random.NextDouble() * _totalWeight;

            // Pick a connection string
            string connectionString = null;
            foreach (Connection c in _connections)
            {
                Contract.Assert(c != null);
                next -= c.Weight;
                if (next > 0)
                    continue;

                connectionString = c.ConnectionString;
                break;
            }

            if (connectionString == null)
                throw new LoggingException(LoggingLevel.Critical,
                    () => Resources.LoadBalancedConnection_CreateConnection_NoValidConnectionString);

            // Create connection using connection string
            return connectionString;
        }

        /// <summary>
        ///   Reloads the schemas and returns a <see cref="bool"/> value indicating whether any schemas have changed.
        /// </summary>
        /// <param name="ensureSchemasIdentical">
        ///   <para>If set to <see langword="true"/> then the schemas must be identical.</para>
        ///   <para>By default this is <see langword="false"/>.</para>
        /// </param>
        /// <returns>
        ///   Returns <see langword="true"/> if the schemas have changed; otherwise returns <see langword="false"/>.
        /// </returns>
        /// <exception cref="LoggingException">
        ///   Connections strings do not have identical schemas
        ///   and <paramref name="ensureSchemasIdentical"/> is <see langword="true"/>.
        /// </exception>
        [UsedImplicitly]
        public bool ReloadSchemas(bool ensureSchemasIdentical = false)
        {
            bool hasChanges = false;
            // ReSharper disable once PossibleNullReferenceException
            foreach (string connectionString in _connections.Select(c => c.ConnectionString))
            {
                bool hc;
                // ReSharper disable once AssignNullToNotNullAttribute
                DatabaseSchema.GetOrAdd(connectionString, true, out hc);
                hasChanges |= hc;
            }

            // Finally validate the schemas are identical
            if ((ensureSchemasIdentical) &&
                (!IdenticalSchemas))
            {
                throw new LoggingException(() => Resources.LoadBalancedConnection_ReloadSchemas_SchemasNotIdentical);
            }

            return hasChanges;
        }

        /// <summary>
        ///   Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>
        ///   A <see cref="string"/> representation of this instance. The format string can be changed in the 
        ///   Resources.resx resource file at the key 'LoadBalancedConnectionToString'.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///   The format string was a <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.FormatException">
        ///   An index from the format string is either less than zero or greater than or equal to the number of arguments.
        /// </exception>
        public override string ToString()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            return String.Format(Resources.LoadBalancedConnection_ToString, _connections.Count);
        }
    }
}