#region © Copyright Web Applications (UK) Ltd, 2014.  All rights reserved.
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
using System.Diagnostics.Contracts;
using System.IO;
using System.Security.Principal;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Service
{
    /// <summary>
    /// Configuration for the NamedPipeServer.
    /// </summary>
    public class ServerConfig
    {
        /// <summary>
        /// The default configuration.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly ServerConfig Default = new ServerConfig();

        /// <summary>
        /// The disabled configuration.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly ServerConfig Disabled = new ServerConfig(
            maximumConnections: 0,
            heartbeat: TimeSpan.MinValue);

        /// <summary>
        /// The maximum number of remote connections (defaults to 1)
        /// </summary>
        [PublicAPI]
        public readonly int MaximumConnections;

        /// <summary>
        /// The pipe name
        /// </summary>
        [CanBeNull]
        [PublicAPI]
        public readonly string Name;

        /// <summary>
        /// The identity of clients that can access the pipe (defaults to BuiltinUsers).
        /// </summary>
        [NotNull]
        [PublicAPI]
        public readonly IdentityReference Identity;

        /// <summary>
        /// The heartbeat timespan, ensures a connection is always available, defaults to once every 5 seconds.
        /// </summary>
        [PublicAPI]
        public readonly TimeSpan Heartbeat;

        #region Constructor overloads
        /// <summary>
        /// Initializes a new instance of the <see cref="NamedPipeServer" /> class.
        /// </summary>
        /// <param name="sddlForm">SDDL string for the SID used to create the
        /// <see cref="SecurityIdentifier" /> object to
        /// identify clients that can access the pipe.</param>
        /// <param name="maximumConnections">The maximum number of connections.</param>
        /// <param name="name">The pipe name.</param>
        /// <param name="heartbeat">The heartbeat timespan, ensures a connection is always available, defaults to once every 5 seconds, specify a negative value to disable.</param>
        public ServerConfig(
            [NotNull] string sddlForm,
            int maximumConnections = 1,
            [CanBeNull] string name = null,
            TimeSpan heartbeat = default(TimeSpan))
            : this(new SecurityIdentifier(sddlForm), name, maximumConnections, heartbeat)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedPipeServer" /> class.
        /// </summary>
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
        /// <param name="name">The pipe name.</param>
        /// <param name="maximumConnections">The maximum number of connections.</param>
        /// <param name="heartbeat">The heartbeat timespan, ensures a connection is always available, defaults to once every 5 seconds, specify a negative value to disable.</param>
        public ServerConfig(
            WellKnownSidType sidType,
            [CanBeNull] SecurityIdentifier domainSid = null,
            [CanBeNull] string name = null,
            int maximumConnections = 1,
            TimeSpan heartbeat = default(TimeSpan))
            : this(new SecurityIdentifier(sidType, domainSid), name, maximumConnections, heartbeat)
        {
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedPipeServer" /> class.
        /// </summary>
        /// <param name="identity">The identity of clients that can access the pipe (defaults to BuiltinUsers).</param>
        /// <param name="name">The pipe name.</param>
        /// <param name="maximumConnections">The maximum number of connections.</param>
        /// <param name="heartbeat">The heartbeat timespan, ensures a connection is always available, defaults to once every 5 seconds, specify a negative value to disable.</param>
        /// <exception cref="System.ArgumentException">
        /// Invalid pipe name.;name
        /// or
        /// Invalid pipe name.;name
        /// </exception>
        /// <exception cref="ServiceException"></exception>
        public ServerConfig(
            [CanBeNull] IdentityReference identity = null,
            [CanBeNull] string name = null,
            int maximumConnections = 1,
            TimeSpan heartbeat = default(TimeSpan))
        {
            Contract.Requires<RequiredContractException>(maximumConnections >= 0, "NamedPipeServer_MaxConnections");
            MaximumConnections = maximumConnections;
            Identity = identity ?? new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);
            Heartbeat = heartbeat < TimeSpan.Zero
                ? TimeSpan.MinValue
                : (heartbeat == default(TimeSpan)
                    ? TimeSpan.FromSeconds(5)
                    : heartbeat);

            if (string.IsNullOrWhiteSpace(name)) return;

            string directory = Path.GetDirectoryName(name);
            if (string.IsNullOrWhiteSpace(directory))
                throw new ArgumentException("Invalid pipe name.", "name");
            string[] rootParts = name.Split(new[] {'\\'}, StringSplitOptions.RemoveEmptyEntries);
            if (rootParts.Length != 2 ||
                !string.Equals(rootParts[1], "pipe", StringComparison.CurrentCultureIgnoreCase))
                throw new ArgumentException("Invalid pipe name.", "name");
            Name = name;
        }
    }
}