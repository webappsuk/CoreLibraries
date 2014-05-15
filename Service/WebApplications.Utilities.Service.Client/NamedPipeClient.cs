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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WebApplications.Utilities.Logging;
using WebApplications.Utilities.Service.PipeProtocol;
using WebApplications.Utilities.Threading;

namespace WebApplications.Utilities.Service.Client
{
    public class NamedPipeClient : IDisposable
    {
        private NamedPipeClient()
        {
        }

        public static NamedPipeClient Connect(string pipe)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #region Find files kernal methods
        [StructLayout(LayoutKind.Sequential)]
        private struct FILETIME
        {
            public uint dwLowDateTime;
            public uint dwHighDateTime;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct WIN32_FIND_DATA
        {
            public uint dwFileAttributes;
            public FILETIME ftCreationTime;
            public FILETIME ftLastAccessTime;
            public FILETIME ftLastWriteTime;
            public uint nFileSizeHigh;
            public uint nFileSizeLow;
            public uint dwReserved0;
            public uint dwReserved1;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string cFileName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
            public string cAlternateFileName;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr FindFirstFile(string lpFileName, out WIN32_FIND_DATA lpFindFileData);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern int FindNextFile(IntPtr hFindFile, out WIN32_FIND_DATA lpFindFileData);
        [DllImport("kernel32.dll")]
        private static extern bool FindClose(IntPtr hFindFile);
        #endregion

        /// <summary>
        /// Gets the server pipes.
        /// </summary>
        /// <returns>An enumeration of pipes with the correct suffix.</returns>
        [NotNull]
        public static IEnumerable<NamedPipeServerInfo> GetServers()
        {
            // Note: Directory.GetFiles() can fail if there are pipes on the system with invalid characters,
            // to be safe we use the underlying kernal methods instead.
            IntPtr invalid = new IntPtr(-1);
            IntPtr handle = IntPtr.Zero;
            try
            {
                WIN32_FIND_DATA data;
                handle = FindFirstFile(@"\\.\pipe\*", out data);
                if (handle == invalid) yield break;

                do
                {
                    NamedPipeServerInfo nps = new NamedPipeServerInfo(@"\\.\pipe\" + data.cFileName);
                    if (nps.IsValid)
                        yield return nps;
                } while (FindNextFile(handle, out data) != 0);
                FindClose(handle);
                handle = invalid;
            }
            finally
            {
                if (handle != invalid)
                    FindClose(handle);
            }
        }

        /// <summary>
        /// Finds the server that matches the name or pipe specified.
        /// </summary>
        /// <param name="serverName">Name (or pipe) of the server.</param>
        /// <returns>The <see cref="NamedPipeServerInfo"/> if found; otherwise <see langword="null"/>.</returns>
        [CanBeNull]
        public static NamedPipeServerInfo FindServer([CanBeNull] string serverName)
        {
            if (string.IsNullOrWhiteSpace(serverName))
                return null;

            return
                GetServers()
                    .FirstOrDefault(
                        n => string.Equals(serverName, n.Name, StringComparison.CurrentCultureIgnoreCase) ||
                             string.Equals(serverName, n.Pipe, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}