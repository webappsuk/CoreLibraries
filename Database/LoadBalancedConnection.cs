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
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Database.Schema;
using WebApplications.Utilities.Logging;
using WebApplications.Utilities.Threading;

namespace WebApplications.Utilities.Database
{
    /// <summary>
    ///   Allows the specification of a set of connection strings to be selected at random based on a weighting.
    /// </summary>
    [PublicAPI]
    public class LoadBalancedConnection : IReadOnlyCollection<Connection>
    {
        /// <summary>
        ///   Holds <see cref="Connection"/>s and their <see cref="Connection.Weight">weighting</see>.
        /// </summary>
        [NotNull]
        private readonly IReadOnlyCollection<Connection> _connections;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadBalancedConnection" /> class.
        /// This creates a load balanced connection with only one connection string.
        /// </summary>
        /// <param name="connectionString"><para>The connection string.</para>
        /// <para>This is given a weighting of 1.0.</para></param>
        /// <param name="weight">The weight.</param>
        /// <exception cref="LoggingException"><paramref name="connectionString" /> is <see langword="null" />.</exception>
        public LoadBalancedConnection([NotNull] string connectionString, double weight = 1D)
            : this(new Connection(connectionString, weight).Yield())
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="LoadBalancedConnection"/> class.
        ///   This creates an evenly balanced set of connections.
        /// </summary>
        /// <param name="connectionStrings">
        ///   The connection strings, which all have a weighting of 1.0.
        /// </param>
        /// <exception cref="LoggingException">
        ///   No connection strings specified in <paramref name="connectionStrings"/> or all strings were <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="connectionStrings"/> was <see langword="null"/>.
        /// </exception>
        public LoadBalancedConnection([NotNull] params string[] connectionStrings)
            : this(connectionStrings
                       .Where(cs => !string.IsNullOrWhiteSpace(cs))
                       // ReSharper disable once AssignNullToNotNullAttribute
                       .Select(cs => new Connection(cs)))
        {
            if (connectionStrings == null) throw new ArgumentNullException("connectionStrings");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadBalancedConnection" /> class.
        /// This creates an evenly balanced set of connections.
        /// </summary>
        public LoadBalancedConnection([NotNull] IEnumerable<string> connectionStrings)
            : this(connectionStrings
                       .Where(cs => !string.IsNullOrWhiteSpace(cs))
                       // ReSharper disable once AssignNullToNotNullAttribute
                       .Select(cs => new Connection(cs)))
        {
            if (connectionStrings == null) throw new ArgumentNullException("connectionStrings");
        }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="LoadBalancedConnection" /> class.
        /// This is allows a weighting to be set for each connection string.</para>
        /// <para>The higher the weighting the more likely a connection string will be chosen when
        /// requesting a new connection.</para>
        /// <para>The connection can be given a unique id for lookup.</para>
        /// </summary>
        public LoadBalancedConnection([NotNull] IEnumerable<KeyValuePair<string, double>> connectionStrings)
            // ReSharper disable once AssignNullToNotNullAttribute
            : this(connectionStrings
                       .Where(kvp => !string.IsNullOrWhiteSpace(kvp.Key))
                       // ReSharper disable AssignNullToNotNullAttribute
                       .Select(kvp => new Connection(kvp.Key, kvp.Value)))
            // ReSharper restore AssignNullToNotNullAttribute
        {
            if (connectionStrings == null) throw new ArgumentNullException("connectionStrings");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadBalancedConnection"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public LoadBalancedConnection([NotNull] Connection connection)
            : this(connection.Yield())
        {
        }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="LoadBalancedConnection" /> class.
        /// This is allows a weighting to be set for each connection string.</para>
        /// <para>The higher the weighting the more likely a connection string will be chosen when
        /// requesting a new connection.</para>
        /// <para>The connection can be given a unique id for lookup.</para>
        /// </summary>
        /// <param name="connections">The connections.</param>
        /// <exception cref="WebApplications.Utilities.Logging.LoggingException">
        /// </exception>
        public LoadBalancedConnection([NotNull] IEnumerable<Connection> connections)
        {
            if (connections == null) throw new ArgumentNullException("connections");

            Dictionary<string, Connection> dictionary = new Dictionary<string, Connection>();

            // Combine identical connection strings and their weighting.
            foreach (Connection connection in connections)
            {
                if (connection == null) continue;

                Connection c;
                if (dictionary.TryGetValue(connection.ConnectionString, out c))
                {
                    Debug.Assert(c != null);
                    c = connection.AddWeight(c.Weight);
                }
                else
                    c = connection;

                dictionary[c.ConnectionString] = c;
            }

            _connections = dictionary.Values.ToArray();

            if (!_connections.Any())
                throw new LoggingException(
                    () =>
                        Resources.LoadBalancedConnection_NoConnectionStrings);

            // Throw an exception if we have any connection strings with a negative weighting.
            // ReSharper disable once PossibleNullReferenceException
            if (_connections.Any(c => c.Weight < 0))
                throw new LoggingException(
                    () => Resources.LoadBalancedConnection_WeightLessThanZero);

            // Create a de-bounced function to check if schemas are identical.
            _checkIdenticalFunction = new AsyncDebouncedFunction<bool>(
                async t =>
                {
                    Guid guid = Guid.Empty;
                    // ReSharper disable PossibleNullReferenceException
                    foreach (DatabaseSchema schema in await
                        Task.WhenAll(_connections.Select(c => DatabaseSchema.GetOrAdd(c, false, t)))
                            .ConfigureAwait(false))
                        // ReSharper restore PossibleNullReferenceException
                    {
                        Debug.Assert(schema != null);
                        if (guid.Equals(Guid.Empty)) guid = schema.Guid;
                        else if (!guid.Equals(schema.Guid)) return false;
                    }
                    return true;
                },
                TimeSpan.FromMinutes(1),
                TimeSpan.FromMinutes(1));

            // Create reload schemas de-bounced function.
            _reloadSchemasAction = new AsyncDebouncedAction(
                // ReSharper disable once AssignNullToNotNullAttribute
                t => Task.WhenAll(_connections.Select(c => DatabaseSchema.GetOrAdd(c, true, t))));
        }

        [NotNull]
        private readonly AsyncDebouncedFunction<bool> _checkIdenticalFunction;

        /// <summary>
        ///   Loads all <see cref="DatabaseSchema">schemas</see> and returns a value indicating if they're identical.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if the schemas are identical; otherwise returns <see langword="false"/>.
        /// </value>
        [NotNull]
        public Task<bool> CheckIdentical(CancellationToken token = default(CancellationToken))
        {
            return _checkIdenticalFunction.Run(token);
        }

        #region IReadOnlyCollection<Connection> Members
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

        /// <summary>
        /// Gets the number of connections in this connection.
        /// </summary>
        public int Count
        {
            get { return _connections.Count; }
        }
        #endregion

        /// <summary>
        /// Gets a random  <see cref="Connection.ConnectionString">connection string</see>.
        /// </summary>
        [NotNull]
        public string ChooseConnectionString()
        {
            return ChooseConnection(this).ConnectionString;
        }

        /// <summary>
        /// Chooses a random <see cref="Connection"/> from an enumeration of <see cref="Connection">connections</see>.
        /// </summary>
        /// <returns>A <see cref="SqlConnection" /> object.</returns>
        [NotNull]
        public static Connection ChooseConnection([NotNull] IEnumerable<Connection> connections)
        {
            if (connections == null) throw new ArgumentNullException("connections");
            // Choose a random connection.

            // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute
            return connections.Choose(c => c.Weight);
            // ReSharper restore AssignNullToNotNullAttribute, PossibleNullReferenceException
        }

        [NotNull]
        private readonly AsyncDebouncedAction _reloadSchemasAction;

        /// <summary>
        /// Reloads the schemas and returns a <see cref="bool"/> value indicating whether any schemas have changed.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns <see langword="true"/> if the schemas have changed; otherwise returns <see langword="false"/>.</returns>
        [NotNull]
        public Task ReloadSchemas(CancellationToken cancellationToken = default(CancellationToken))
        {
            return _reloadSchemasAction.Run(cancellationToken);
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
            return String.Format(Resources.LoadBalancedConnection_ToString, _connections.Count());
        }
    }
}