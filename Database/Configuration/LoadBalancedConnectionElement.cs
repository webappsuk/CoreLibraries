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
using WebApplications.Utilities.Logging;
using ConfigurationElement = WebApplications.Utilities.Configuration.ConfigurationElement;

namespace WebApplications.Utilities.Database.Configuration
{
    /// <summary>
    ///   A LoadBalanced connection.
    /// </summary>
    public class LoadBalancedConnectionElement : ConfigurationElement
    {
        /// <summary>
        ///   Gets or sets the identifier for a load balanced connection.
        /// </summary>
        /// <value>The connection element.</value>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">
        ///   The property is read-only or locked.
        /// </exception>
        [ConfigurationProperty("id", IsRequired = false, IsKey = true)]
        [NotNull]
        public string Id
        {
            get { return GetProperty<string>("id"); }
            set { SetProperty("id", value); }
        }

        /// <summary>
        ///   Gets or sets a <see cref="bool"/> value indicating whether schemas must be identical.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if schemas must be identical; otherwise <see langword="false"/>.
        /// </value>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">
        ///   The property is read-only or locked.
        /// </exception>
        [ConfigurationProperty("ensureSchemasIdentical", DefaultValue = false, IsRequired = false)]
        public bool EnsureSchemasIdentical
        {
            get { return GetProperty<bool>("ensureSchemasIdentical"); }
            set { SetProperty("ensureSchemasIdentical", value); }
        }

        /// <summary>
        ///   Gets or sets a <see cref="bool"/> value indicating whether the load balanced collection is enabled.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if the load balanced collection is enabled; otherwise <see langword="false"/>.
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
        ///   Gets or sets the connections.
        /// </summary>
        /// <value>The connections.</value>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">
        ///   The property is read-only or locked.
        /// </exception>
        [ConfigurationProperty("", IsRequired = true, IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(ConnectionCollection),
            CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
        [NotNull]
        public ConnectionCollection Connections
        {
            get { return GetProperty<ConnectionCollection>(""); }
            set { SetProperty("", value); }
        }

        /// <summary>
        ///   Used to initialize a default set of values for the <see cref="ConnectionCollection"/> object.
        /// </summary>
        /// <remarks>
        ///   Called to set the internal state to appropriate default values.
        /// </remarks>
        protected override void InitializeDefault()
        {
            // ReSharper disable ConstantNullCoalescingCondition
            Connections = Connections ?? new ConnectionCollection();
            // ReSharper restore ConstantNullCoalescingCondition

            base.InitializeDefault();
        }

        /// <summary>
        /// Gets the load balanced connection based on this element; otherwise <see langword="null" /> if disabled..
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;LoadBalancedConnection&gt;.</returns>
        [NotNull]
        [PublicAPI]
        public Task<LoadBalancedConnection> GetLoadBalancedConnection(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetLoadBalancedConnection(null, cancellationToken);
        }

        /// <summary>
        /// Gets the load balanced connection based on this element; otherwise <see langword="null" /> if disabled..
        /// </summary>
        /// <param name="ensureIdentical">if set to <see langword="true" /> ensures schemas are identical.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;LoadBalancedConnection&gt;.</returns>
        [NotNull]
        [PublicAPI]
        public Task<LoadBalancedConnection> GetLoadBalancedConnection(bool? ensureIdentical = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!Enabled) return TaskResult<LoadBalancedConnection>.Default;

            KeyValuePair<string, double>[] connections = Connections
                .Where(lbc => lbc != null && lbc.Enabled)
                .Select(
                    lbc => new KeyValuePair<string, double>(lbc.ConnectionString, lbc.Weight))
                .ToArray();

            if (connections.Length < 1) return TaskResult<LoadBalancedConnection>.Default;

            LoadBalancedConnection connection = new LoadBalancedConnection(connections);

            // ReSharper disable once AssignNullToNotNullAttribute
            if (ensureIdentical == false ||
                (!ensureIdentical.HasValue && !EnsureSchemasIdentical)) return Task.FromResult(connection);

            return connection.CheckIdentical(cancellationToken)
                .ContinueWith(
                    t =>
                    {
                        Contract.Assert(t != null);
                        if (!t.Result)
                            throw new LoggingException(
                                () =>
                                    Resources
                                    .LoadBalancedConnectionElement_GetLoadBalancedConnection_SchemasNotIdentical);
                        return connection;
                    },
                    cancellationToken,
                    TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion,
                    // ReSharper disable once AssignNullToNotNullAttribute
                    TaskScheduler.Current);
        }
    }
}