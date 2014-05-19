﻿#region © Copyright Web Applications (UK) Ltd, 2014.  All rights reserved.
// Copyright (c) 2014, Web Applications UK Ltd
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
using System.Diagnostics.Contracts;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using JetBrains.Annotations;
using WebApplications.Utilities.Logging;
using WebApplications.Utilities.Service.PipeProtocol;

namespace WebApplications.Utilities.Service
{
    /// <summary>
    /// Creates a server for a bi-directional pipe.
    /// </summary>
    public partial class NamedPipeServer : IDisposable
    {
        /// <summary>
        /// The connection lock.
        /// </summary>
        [NotNull]
        private readonly object _connectionLock = new object();

        /// <summary>
        /// The named pipe connections.
        /// </summary>
        [NotNull]
        private readonly List<NamedPipeConnection> _namedPipeConnections;

        /// <summary>
        /// The service.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public readonly BaseService Service;

        /// <summary>
        /// The pipe name.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public readonly string Name;

        /// <summary>
        /// Gets the maximum number of connections.
        /// </summary>
        /// <value>The maximum connections.</value>
        [PublicAPI]
        public readonly int MaximumConnections;

        /// <summary>
        /// Gets the connection count.
        /// </summary>
        /// <value>The connection count.</value>
        [PublicAPI]
        public int ConnectionCount
        {
            get
            {
                lock (_connectionLock)
                    // ReSharper disable once PossibleNullReferenceException
                    return _namedPipeConnections.Count(c => c.State == PipeState.Connected);
            }
        }

        /// <summary>
        /// The input buffer size
        /// </summary>
        [PublicAPI]
        public const int InBufferSize = 16384;

        /// <summary>
        /// The output buffer size
        /// </summary>
        [PublicAPI]
        public const int OutBufferSize = 32768;

        /// <summary>
        /// The pipe security.
        /// </summary>
        [NotNull]
        private readonly PipeSecurity _pipeSecurity;

        private Timer _connectionCheckTimer;

        private TimeSpan _heartbeat;

        private NamedPipeServerLogger _logger;

        #region Constructor overloads
        /// <summary>
        /// Initializes a new instance of the <see cref="NamedPipeServer" /> class.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="name">The name.</param>
        /// <param name="sddlForm">SDDL string for the SID used to create the <see cref="SecurityIdentifier"/> object to 
        /// identify clients that can access the pipe.</param>
        /// <param name="maximumConnections">The maximum number of connections.</param>
        /// <param name="heartbeat">The heartbeat timespan, ensures a connection is always available, defaults to once every 5 seconds.</param>
        public NamedPipeServer(
            [NotNull] BaseService service,
            [CanBeNull] string name,
            [NotNull] string sddlForm,
            int maximumConnections = 1,
            TimeSpan heartbeat = default(TimeSpan))
            : this(service, name, new SecurityIdentifier(sddlForm), maximumConnections, heartbeat)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedPipeServer" /> class.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="name">The name.</param>
        /// <param name="sidType">One of the enumeration of well known sid types, the value must not be 
        /// <see cref="WellKnownSidType.LogonIdsSid" />.  This defines
        /// who can connect to the pipe.</param>
        /// <param name="domainSid"><para>The domain SID. This value is required for the following <see cref="WellKnownSidType" /> values.
        /// This parameter is ignored for any other <see cref="WellKnownSidType" /> values.</para>
        /// <list type="bullet">
        ///   <item>
        ///     <description>AccountAdministratorSid</description>
        ///   </item>
        ///   <item>
        ///     <description>AccountGuestSid</description>
        ///   </item>
        ///   <item>
        ///     <description>AccountKrbtgtSid</description>
        ///   </item>
        ///   <item>
        ///     <description>AccountDomainAdminsSid</description>
        ///   </item>
        ///   <item>
        ///     <description>AccountDomainUsersSid</description>
        ///   </item>
        ///   <item>
        ///     <description>AccountDomainGuestsSid</description>
        ///   </item>
        ///   <item>
        ///     <description>AccountComputersSid</description>
        ///   </item>
        ///   <item>
        ///     <description>AccountControllersSid</description>
        ///   </item>
        ///   <item>
        ///     <description>AccountCertAdminsSid</description>
        ///   </item>
        ///   <item>
        ///     <description>AccountSchemaAdminsSid</description>
        ///   </item>
        ///   <item>
        ///     <description>AccountEnterpriseAdminsSid</description>
        ///   </item>
        ///   <item>
        ///     <description>AccountPolicyAdminsSid</description>
        ///   </item>
        ///   <item>
        ///     <description>AccountRasAndIasServersSid</description>
        ///   </item>
        /// </list></param>
        /// <param name="maximumConnections">The maximum number of connections.</param>
        /// <param name="heartbeat">The heartbeat timespan, ensures a connection is always available, defaults to once every 5 seconds.</param>
        public NamedPipeServer(
            [NotNull] BaseService service,
            [CanBeNull] string name,
            WellKnownSidType sidType,
            SecurityIdentifier domainSid = null,
            int maximumConnections = 1,
            TimeSpan heartbeat = default(TimeSpan))
            : this(service, name, new SecurityIdentifier(sidType, domainSid), maximumConnections, heartbeat)
        {
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedPipeServer" /> class.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="name">The pipe name.</param>
        /// <param name="identity">The identity of clients that can access the pipe (defaults to BuiltinUsers).</param>
        /// <param name="maximumConnections">The maximum number of connections.</param>
        /// <param name="heartbeat">The heartbeat timespan, ensures a connection is always available, defaults to once every 5 seconds.</param>
        /// <exception cref="ServiceException">
        /// </exception>
        public NamedPipeServer(
            [NotNull] BaseService service,
            [CanBeNull] string name = null,
            IdentityReference identity = null,
            int maximumConnections = 1,
            TimeSpan heartbeat = default(TimeSpan))
        {
            Contract.Requires(maximumConnections > 0);
            Service = service;
            MaximumConnections = maximumConnections;
            StringBuilder builder = new StringBuilder();
            builder.Append(Guid.NewGuid().ToString("D"))
                .Append('_');
            if (string.IsNullOrWhiteSpace(name))
                name = service.ServiceName;
            foreach (char c in name)
                builder.Append(char.IsLetterOrDigit(c) ? c : '_');
            builder.Append(Common.NameSuffix);
            Name = builder.ToString();

            // Create security context
            try
            {
                if (identity == null)
                    identity = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);

                WindowsIdentity currentIdentity = WindowsIdentity.GetCurrent();
                if (currentIdentity == null ||
                    currentIdentity.Owner == null)
                    throw new ServiceException(() => ServiceResources.Err_NamedPipeServer_CannotGetCurrentOwner);

                _pipeSecurity = new PipeSecurity();
                _pipeSecurity.AddAccessRule(
                    new PipeAccessRule(
                        identity,
                        PipeAccessRights.ReadWrite,
                        AccessControlType.Allow));
                _pipeSecurity.AddAccessRule(
                    new PipeAccessRule(
                        currentIdentity.Owner,
                        PipeAccessRights.FullControl,
                        AccessControlType.Allow));
            }
            catch (Exception exception)
            {
                throw new ServiceException(exception, () => ServiceResources.Err_NamedPipeServer_Fatal_Error_Securing);
            }

            // Check no one has tried to create the pipe before us, combined with the GUID name this makes the most common
            // form of pipe attack (pre-registration) impossible.
            if (File.Exists(@"\\.\pipe\" + Name))
                throw new ServiceException(() => ServiceResources.Err_NamedPipeServer_PipeAlreadyExists);

            // Create a connection, before adding it to the list and starting.
            NamedPipeConnection connection = new NamedPipeConnection(this);
            _namedPipeConnections = new List<NamedPipeConnection>(MaximumConnections) {connection};
            connection.Start();
            _logger = new NamedPipeServerLogger(this);
            Log.AddLogger(_logger);

            if (heartbeat < TimeSpan.Zero)
            {
                _heartbeat = TimeSpan.MinValue;
                return;
            }

            _heartbeat = heartbeat == default(TimeSpan) ? TimeSpan.FromSeconds(5) : heartbeat;
            _connectionCheckTimer = new Timer(CheckConnections);
            _connectionCheckTimer.Change(_heartbeat, Timeout.InfiniteTimeSpan);
        }

        /// <summary>
        /// Checks the connections to ensure we have at least one open, this should never happen but services are long running, and so
        /// if something truly fatal happens this should restore connectivity.
        /// </summary>
        /// <param name="state">The state.</param>
        private void CheckConnections([CanBeNull] object state)
        {
            lock (_connectionLock)
            {
                if (_connectionCheckTimer == null) return;

                if (_namedPipeConnections.Count < MaximumConnections)
                {
                    _connectionCheckTimer.Change(Timeout.Infinite, Timeout.Infinite);

                    if (!_namedPipeConnections.Any(c => c.State == PipeState.Open))
                        Add();
                }

                // Kick off timer again
                _connectionCheckTimer.Change(_heartbeat, Timeout.InfiniteTimeSpan);
            }
        }

        /// <summary>
        /// Removes the specified connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        private void Remove(NamedPipeConnection connection)
        {
            lock (_namedPipeConnections)
                _namedPipeConnections.Remove(connection);
            Add();
        }

        /// <summary>
        /// Starts a new listening connection, if there is capactiy.
        /// </summary>
        private void Add()
        {
            lock (_namedPipeConnections)
            {
                // Sanity check, this will remove any connections that were terminated before they could tell us!
                // ReSharper disable once PossibleNullReferenceException
                _namedPipeConnections.RemoveAll(c => c.State == PipeState.Closed);

                if (_namedPipeConnections.Count >= MaximumConnections) return;

                // Create a new connection.
                NamedPipeConnection connection = new NamedPipeConnection(this);
                _namedPipeConnections.Add(connection);
                // Note we never start the connection until we've added it to the list, to avoid a race condition where
                // it is removed before it is started.
                connection.Start();
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Timer timer = Interlocked.Exchange(ref _connectionCheckTimer, null);
            if (timer != null)
            {
                timer.Change(Timeout.Infinite, Timeout.Infinite);
                timer.Dispose();
            }

            NamedPipeServerLogger logger = Interlocked.Exchange(ref _logger, null);
            if (logger != null)
            {
                Log.RemoveLogger(logger);
                logger.Dispose();
            }

            lock (_namedPipeConnections)
            {
                foreach (NamedPipeConnection connection in _namedPipeConnections.ToArray())
                    // ReSharper disable once PossibleNullReferenceException
                    connection.Dispose();

                // Should already be clear, but do anyway.
                _namedPipeConnections.Clear();
            }
        }
    }
}