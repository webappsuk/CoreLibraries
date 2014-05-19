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
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace WebApplications.Utilities.Service
{
    /// <summary>
    /// Allows code to be executed under the security context of a specified user account.
    /// </summary>
    /// <remarks>
    /// TODO Move to Utilities
    /// See http://platinumdogs.me/2008/10/30/net-c-impersonation-with-network-credentials/
    /// </remarks>
    public class Impersonator : IDisposable
    {
        #region PInvoke
        public enum LogonType
        {
            Interactive = 2,
            Network = 3,
            Batch = 4,
            Service = 5,
            Unlock = 7,
            NetworkCleartext = 8, // Win2K or higher
            NewCredentials = 9 // Win2K or higher
        };

        public enum LogonProvider
        {
            Default = 0,
            WinNT35 = 1,
            WinNT40 = 2,
            WinNT50 = 3
        };

        private enum ImpersonationLevel
        {
            SecurityAnonymous = 0,
            SecurityIdentification = 1,
            SecurityImpersonation = 2,
            SecurityDelegation = 3
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern int LogonUser(
            string lpszUserName,
            string lpszDomain,
            string lpszPassword,
            int dwLogonType,
            int dwLogonProvider,
            ref IntPtr phToken);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int DuplicateToken(
            IntPtr hToken,
            ImpersonationLevel impersonationLevel,
            ref IntPtr hNewToken);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool RevertToSelf();

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
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
            string userName,
            string password,
            LogonType logonType = LogonType.Interactive,
            LogonProvider logonProvider = LogonProvider.Default)
        {
            string[] up = userName.Split('\\');
            string domainName;
            if (up.Length > 2)
                throw new ArgumentException(userName, "Invalid username.");
            if (up.Length < 2)
                domainName = ".";
            else
            {
                domainName = up[0];
                userName = up[1];
            }

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
                    (int) logonType,
                    (int) logonProvider,
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