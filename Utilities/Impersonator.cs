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
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Principal;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities
{
    /// <summary>
    /// Allows code to be executed under the security context of a specified user account.
    /// </summary>
    /// <remarks>
    /// See http://platinumdogs.me/2008/10/30/net-c-impersonation-with-network-credentials/
    /// </remarks>
    [PublicAPI]
    public class Impersonator : IDisposable
    {
        #region PInvoke
        /// <summary>
        /// The types of logon operation that can be performed.
        /// </summary>
        [PublicAPI]
        public enum LogonType
        {
            /// <summary>
            /// This logon type is intended for users who will be interactively using the computer, such as a user being
            ///  logged on by a terminal server, remote shell, or similar process. This logon type has the additional 
            /// expense of caching logon information for disconnected operations; therefore, it is inappropriate for 
            /// some client/server applications, such as a mail server.
            /// </summary>
            [PublicAPI]
            Interactive = 2,

            /// <summary>
            /// This logon type is intended for high performance servers to authenticate plaintext passwords. 
            /// Credentials are not cached for this logon type.
            /// </summary>
            [PublicAPI]
            Network = 3,

            /// <summary>
            /// This logon type is intended for batch servers, where processes may be executing on behalf of a user 
            /// without their direct intervention. This type is also for higher performance servers that process many 
            /// plaintext authentication attempts at a time, such as mail or web servers.
            /// </summary>
            [PublicAPI]
            Batch = 4,

            /// <summary>
            /// Indicates a service-type logon. The account provided must have the service privilege enabled.
            /// </summary>
            [PublicAPI]
            Service = 5,

            /// <summary>
            /// <para>GINAs are no longer supported.</para>
            /// <para><b>Windows Server 2003 and Windows XP:</b> This logon type is for GINA DLLs that log on users who will be 
            /// interactively using the computer. This logon type can generate a unique audit record that shows when 
            /// the workstation was unlocked.</para>
            /// </summary>
            [Obsolete("GINAs are no longer supported.")]
            [PublicAPI]
            Unlock = 7,

            /// <summary>
            /// This logon type preserves the name and password in the authentication package, which allows the server to make 
            /// connections to other network servers while impersonating the client. A server can accept plaintext credentials 
            /// from a client, create an <see cref="Impersonator"/>, verify that the user can access the system across the network, and still 
            /// communicate with other servers.
            /// </summary>
            [PublicAPI]
            NetworkCleartext = 8,

            /// <summary>
            /// This logon type allows the caller to clone its current token and specify new credentials for outbound connections. 
            /// The new logon session has the same local identifier but uses different credentials for other network connections.
            /// This logon type is supported only by the <see cref="LogonProvider.WinNT50"/> logon provider.
            /// </summary>
            [PublicAPI]
            NewCredentials = 9
        }

        /// <summary>
        /// The logon providers.
        /// </summary>
        [PublicAPI]
        public enum LogonProvider
        {
            /// <summary>
            /// Use the standard logon provider for the system. The default security provider is negotiate, unless you pass 
            /// null for the domain name and the user name is not in UPN format. In this case, the default provider is NTLM.
            /// </summary>
            [PublicAPI]
            Default = 0,

            /// <summary>
            /// Use the NT 3.51 logon provider.
            /// </summary>
            [PublicAPI]
            WinNT35 = 1,

            /// <summary>
            /// Use the NTLM logon provider.
            /// </summary>
            [PublicAPI]
            WinNT40 = 2,

            /// <summary>
            /// Use the negotiate logon provider.
            /// </summary>
            [PublicAPI]
            WinNT50 = 3
        }

        private enum ImpersonationLevel
        {
            // ReSharper disable UnusedMember.Local
            SecurityAnonymous = 0,
            SecurityIdentification = 1,
            SecurityImpersonation = 2,
            SecurityDelegation = 3
            // ReSharper restore UnusedMember.Local
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern int LogonUser(
            string lpszUserName,
            string lpszDomain,
            string lpszPassword,
            LogonType dwLogonType,
            LogonProvider dwLogonProvider,
            ref IntPtr phToken);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int DuplicateToken(
            IntPtr hToken,
            ImpersonationLevel impersonationLevel,
            ref IntPtr hNewToken);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool CloseHandle(IntPtr handle);
        #endregion

        private WindowsImpersonationContext _wic;

        /// <summary>
        /// Begins impersonation with the given credentials, Logon type and Logon provider.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <param name="logonType">Type of the logon.</param>
        /// <param name="logonProvider">The logon provider.</param>
        /// <exception cref="System.ComponentModel.Win32Exception">
        /// </exception>
        public Impersonator(
            [NotNull] string userName,
            [NotNull] string password,
            LogonType logonType = LogonType.Interactive,
            LogonProvider logonProvider = LogonProvider.Default)
        {
            if (userName == null) throw new ArgumentNullException("userName");
            if (password == null) throw new ArgumentNullException("password");

            string[] up = userName.Split('\\');
            string domainName;
            if (up.Length > 2)
                throw new ArgumentException(userName, Resources.Impersonator_Impersonator_InvalidUsername);
            if (up.Length < 2)
                domainName = ".";
            else
            {
                domainName = up[0];
                userName = up[1];
            }

            Init(userName, password, domainName, logonType, logonProvider);
        }

        /// <summary>
        /// Begins impersonation with the given credentials, Logon type and Logon provider.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <param name="domainName">Name of the domain.</param>
        /// <param name="logonType">Type of the logon.</param>
        /// <param name="logonProvider">The logon provider.</param>
        /// <exception cref="System.ComponentModel.Win32Exception">
        /// </exception>
        public Impersonator(
            [NotNull] string userName,
            [NotNull] string password,
            [NotNull] string domainName,
            LogonType logonType = LogonType.Interactive,
            LogonProvider logonProvider = LogonProvider.Default)
        {
            if (userName == null) throw new ArgumentNullException("userName");
            if (password == null) throw new ArgumentNullException("password");
            if (domainName == null) throw new ArgumentNullException("domainName");

            Init(userName, password, domainName, logonType, logonProvider);
        }

        /// <summary>
        /// Initializes the impersonator.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <param name="domainName">Name of the domain.</param>
        /// <param name="logonType">Type of the logon.</param>
        /// <param name="logonProvider">The logon provider.</param>
        /// <exception cref="System.ComponentModel.Win32Exception">
        /// </exception>
        private void Init(
            string userName,
            string password,
            string domainName,
            LogonType logonType,
            LogonProvider logonProvider)
        {
            IntPtr logonToken = IntPtr.Zero;
            IntPtr logonTokenDuplicate = IntPtr.Zero;
            try
            {
                // revert to the application pool identity, saving the identity of the current requestor
                _wic = WindowsIdentity.Impersonate(IntPtr.Zero);

                // do logon & impersonate
                if (LogonUser(
                    userName,
                    domainName,
                    password,
                    logonType,
                    logonProvider,
                    ref logonToken) != 0)
                    if (DuplicateToken(logonToken, ImpersonationLevel.SecurityImpersonation, ref logonTokenDuplicate) !=
                        0)
                    {
                        WindowsIdentity wi = new WindowsIdentity(logonTokenDuplicate);
                        wi.Impersonate();
                        // discard the returned identity context (which is the context of the application pool)
                    }
                    else
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                else
                    throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            finally
            {
                if (logonToken != IntPtr.Zero)
                    CloseHandle(logonToken);

                if (logonTokenDuplicate != IntPtr.Zero)
                    CloseHandle(logonTokenDuplicate);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // restore saved requestor identity
            if (_wic != null)
                _wic.Undo();
            _wic = null;
        }
    }
}