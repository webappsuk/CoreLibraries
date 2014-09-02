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
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WebApplications.Utilities.Database.Schema;
using WebApplications.Utilities.Logging;
using ConfigurationElement = WebApplications.Utilities.Configuration.ConfigurationElement;

namespace WebApplications.Utilities.Database.Configuration
{
    /// <summary>
    ///   An element that represents a database.
    /// </summary>
    public partial class DatabaseElement : ConfigurationElement
    {
        /// <summary>
        ///   Gets or sets the identifier for a database.
        /// </summary>
        /// <value>
        ///   The identifier for this database.
        /// </value>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">
        ///   The property is read-only or locked.
        /// </exception>
        [ConfigurationProperty("id", IsRequired = true, IsKey = true)]
        [NotNull]
        public string Id
        {
            get { return GetProperty<string>("id"); }
            set { SetProperty("id", value); }
        }

        /// <summary>
        ///   Gets or sets a <see cref="bool"/> value indicating whether the database is enabled.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if the database is enabled; otherwise <see langword="false"/>.
        /// </value>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">
        ///   The property is read-only or locked.
        /// </exception>
        [ConfigurationProperty("enabled", DefaultValue = true, IsRequired = false)]
        public bool Enabled
        {
            get { return GetProperty<bool>("enabled"); }
            set { SetProperty("enabled", value); }
        }

        /// <summary>
        ///   Gets or sets the connections for this database.
        /// </summary>
        /// <value>The connections.</value>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">
        ///   The property is read-only or locked.
        /// </exception>
        [ConfigurationProperty("connections", IsRequired = true, IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(LoadBalancedConnectionCollection))]
        [NotNull]
        public LoadBalancedConnectionCollection Connections
        {
            get { return GetProperty<LoadBalancedConnectionCollection>("connections"); }
            set { SetProperty("connections", value); }
        }

        /// <summary>
        ///   Gets or sets the <see cref="WebApplications.Utilities.Database.SqlProgram">programs</see> for this database.
        /// </summary>
        /// <value>
        ///   The stored procedures and functions for this database.
        /// </value>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">
        ///   The property is read-only or locked.
        /// </exception>
        [ConfigurationProperty("programs", IsRequired = false, IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(ProgramCollection),
            CollectionType = ConfigurationElementCollectionType.BasicMapAlternate)]
        [NotNull]
        public ProgramCollection Programs
        {
            get { return GetProperty<ProgramCollection>("programs"); }
            set { SetProperty("programs", value); }
        }

        /// <summary>
        ///   Used to initialize a default set of values for the <see cref="System.Configuration.ConfigurationElement"/> object.
        /// </summary>
        /// <remarks>
        ///   Called to set the internal state to appropriate default values.
        /// </remarks>
        protected override void InitializeDefault()
        {
            // ReSharper disable ConstantNullCoalescingCondition
            Connections = Connections ?? new LoadBalancedConnectionCollection();
            Programs = Programs ?? new ProgramCollection();
            // ReSharper restore ConstantNullCoalescingCondition

            base.InitializeDefault();
        }

        /// <summary>
        /// Gets the <see cref="WebApplications.Utilities.Database.SqlProgram" /> with the specified name and parameters,
        /// respecting configured options.
        /// </summary>
        /// <param name="name">The name of the stored procedure or function.</param>
        /// <param name="parameters">The program parameters.</param>
        /// <param name="ignoreValidationErrors">If set to <see langword="true" /> will ignore validation errors regardless of configuration.</param>
        /// <param name="checkOrder">If set to <see langword="true" /> will check parameter order matches regardless of configuration.</param>
        /// <param name="defaultCommandTimeout"><para>The default command timeout.</para>
        /// <para>If set will override the configuration from <see cref="ProgramElement.DefaultCommandTimeout" />.</para></param>
        /// <param name="constraintMode"><para>The constraint mode</para>
        /// <para>If set will override the configuration from <see cref="ProgramElement.ConstraintMode" />.</para></param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The retrieved <see cref="WebApplications.Utilities.Database.SqlProgram" />.</returns>
        /// <exception cref="WebApplications.Utilities.Logging.LoggingException">
        /// </exception>
        /// <exception cref="LoggingException"><para>Could not find a default load balanced connection for the database with this <see cref="Id" />.</para>
        /// <para>-or-</para>
        /// <para>A parameter with no name map was found.</para></exception>
        [NotNull]
        public async Task<SqlProgram> GetSqlProgram(
            [NotNull] string name,
            [CanBeNull] IEnumerable<KeyValuePair<string, Type>> parameters = null,
            bool ignoreValidationErrors = false,
            bool checkOrder = false,
            TimeSpan? defaultCommandTimeout = null,
            TypeConstraintMode? constraintMode = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Contract.Requires(name != null);

            // Grab the default load balanced connection for the database.
            // ReSharper disable once PossibleNullReferenceException
            LoadBalancedConnectionElement connectionElement = Connections.FirstOrDefault(c => c.Enabled);

            if (connectionElement == null)
                throw new LoggingException(
                    () => Resources.DatabaseElement_GetSqlProgram_DefaultLoadBalanceConnectionNotFound,
                    Id);

            // Look for program mapping information
            ProgramElement prog = Programs[name];
            if (prog != null)
            {
                // Check for name mapping
                if (!String.IsNullOrWhiteSpace(prog.MapTo))
                    // ReSharper disable once AssignNullToNotNullAttribute
                    name = prog.MapTo;

                // Set options if not already set.
                ignoreValidationErrors = ignoreValidationErrors && prog.IgnoreValidationErrors;
                checkOrder = checkOrder && prog.CheckOrder;
                defaultCommandTimeout = defaultCommandTimeout ?? prog.DefaultCommandTimeout;
                constraintMode = constraintMode ?? prog.ConstraintMode;

                if (!String.IsNullOrEmpty(prog.Connection))
                {
                    // ReSharper disable once AssignNullToNotNullAttribute
                    connectionElement = Connections[prog.Connection];
                    if ((connectionElement == null) ||
                        (!connectionElement.Enabled))
                        throw new LoggingException(
                            () => Resources.DatabaseElement_GetSqlProgram_LoadBalanceConnectionNotFound,
                            prog.Connection,
                            Id,
                            name);
                }

                // Check for parameter mappings
                if ((parameters != null) &&
                    prog.Parameters.Any())
                    parameters = parameters
                        .Select(
                            kvp =>
                            {
                                // ReSharper disable once AssignNullToNotNullAttribute
                                ParameterElement param = prog.Parameters[kvp.Key];
                                if (param == null) return kvp;
                                if (String.IsNullOrWhiteSpace(param.MapTo))
                                    throw new LoggingException(
                                        () => Resources.DatabaseElement_GetSqlProgram_MappingNotSpecified,
                                        kvp.Key,
                                        prog.Name);

                                return new KeyValuePair<string, Type>(param.MapTo, kvp.Value);
                            }).ToList();
            }

            if (constraintMode == null) constraintMode = TypeConstraintMode.Warn;


            if (connectionElement == null)
                throw new LoggingException(
                    () => Resources.DatabaseElement_GetSchemas_No_Connection, Id);

            LoadBalancedConnection connection = await connectionElement.GetLoadBalancedConnection(cancellationToken: cancellationToken).ConfigureAwait(false);

            Contract.Assert(connection != null);
            Contract.Assert(name != null);
            return await SqlProgram.Create(
                connection,
                name,
                parameters,
                ignoreValidationErrors,
                checkOrder,
                defaultCommandTimeout,
                (TypeConstraintMode)constraintMode,
                cancellationToken);
        }

        /// <summary>
        /// Gets the schema for the named database (and ensures all connections have identical schemas).
        /// </summary>
        /// <param name="connectionName">Name of the connection.</param>
        /// <param name="forceReload">If set to <see langword="true" /> forces the schema to <see cref="DatabaseSchema.Load">reload</see>.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;IEnumerable&lt;DatabaseSchema&gt;&gt;.</returns>
        [PublicAPI]
        [NotNull]
        public Task<DatabaseSchema> GetSchema(
            [CanBeNull] string connectionName = null,
            bool forceReload = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            LoadBalancedConnectionElement connectionElement = string.IsNullOrWhiteSpace(connectionName)
                ? Connections.FirstOrDefault(c => c.Enabled)
                : Connections[connectionName];

            if (connectionElement == null)
                throw new LoggingException(
                    () => Resources.DatabaseElement_GetSchemas_No_Connection, Id);

            return connectionElement.GetSchema(forceReload, cancellationToken);
        }
        
        /// <summary>
        /// Gets the <see cref="LoadBalancedConnection">load balanced connection</see> from the configuration with the optional
        /// connection ID.
        /// </summary>
        /// <param name="connectionName">Name of the connection (defaults to first connection).</param>
        /// <param name="ensureIdentical">if set to <see langword="true" /> ensures schemas are identical; if <see langword="null" /> then falls back to
        /// the <see cref="LoadBalancedConnectionElement.EnsureSchemasIdentical">configured value</see>; otherwise does not check the schemas.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;DatabaseSchema&gt;.</returns>
        /// <exception cref="WebApplications.Utilities.Logging.LoggingException"></exception>
        [PublicAPI]
        [NotNull]
        public Task<LoadBalancedConnection> GetConnection(
            [CanBeNull] string connectionName = null,
            [CanBeNull] bool? ensureIdentical = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            LoadBalancedConnectionElement connectionElement = string.IsNullOrWhiteSpace(connectionName)
                ? Connections.FirstOrDefault(c => c.Enabled)
                : Connections[connectionName];
            if (connectionElement == null)
                throw new LoggingException(
                    () => Resources.DatabaseElement_GetSchemas_No_Connection, Id);

            return connectionElement.GetLoadBalancedConnection(ensureIdentical, cancellationToken);
        }

        /// <summary>
        /// Gets the <see cref="LoadBalancedConnection">load balanced connections</see> from the configuration.
        /// </summary>
        /// <param name="ensureIdentical">if set to <see langword="true" /> ensures schemas are identical; if <see langword="null" /> then falls back to
        /// the <see cref="LoadBalancedConnectionElement.EnsureSchemasIdentical">configured value</see>; otherwise does not check the schemas.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;DatabaseSchema&gt;.</returns>
        /// <exception cref="WebApplications.Utilities.Logging.LoggingException"></exception>
        [PublicAPI]
        [NotNull]
        public Task<IEnumerable<LoadBalancedConnection>> GetConnections(
            [CanBeNull] bool? ensureIdentical = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute
            return Task.WhenAll(
                Connections.Select(c => c.GetLoadBalancedConnection(ensureIdentical, cancellationToken)))
                .ContinueWith(
                    t => (IEnumerable<LoadBalancedConnection>)t.Result,
                    cancellationToken,
                    TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion,
                    TaskScheduler.Current);
            // ReSharper restore PossibleNullReferenceException, AssignNullToNotNullAttribute
        }
    }
}