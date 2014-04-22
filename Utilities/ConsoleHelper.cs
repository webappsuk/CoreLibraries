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
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using JetBrains.Annotations;
using WebApplications.Utilities.Formatting;
using WebApplications.Utilities.Threading;

namespace WebApplications.Utilities
{
    /// <summary>
    /// Helpful functions for initializing and using a console safely.
    /// </summary>
    [PublicAPI]
    public static class ConsoleHelper
    {
        /// <summary>
        /// Calculates whether we have a console available.
        /// </summary>
        [NotNull]
        private static readonly Lazy<bool> _isConsole = new Lazy<bool>(
            () =>
            {
                if (!Environment.UserInteractive) return false;
                try
                {
                    return Console.CursorLeft >= int.MinValue;
                }
                catch (IOException)
                {
                    // Try to attach to parent process's console window
                    return AttachConsole(0xFFFFFFFF);
                }
            },
            LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// Whether the current application is running in a console.
        /// </summary>
        [PublicAPI]
        public static bool IsConsole
        {
            get { return _isConsole.Value; }
        }

        /// <summary>
        /// Attaches to a parent console.
        /// </summary>
        /// <param name="dwProcessId">The dw process identifier.</param>
        /// <returns><see langword="true" /> if succeeds, <see langword="false" /> otherwise.</returns>
        [DllImport("kernel32", SetLastError = true)]
        private static extern bool AttachConsole(uint dwProcessId);

        /// <summary>
        /// Shows the window.
        /// </summary>
        /// <param name="hWnd">The window handle.</param>
        /// <param name="cmdShow">The command id.</param>
        /// <returns><see langword="true" /> if successfull, <see langword="false" /> otherwise.</returns>
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int cmdShow);

        /// <summary>
        /// Maximises the console window.
        /// </summary>
        [PublicAPI]
        public static void Maximise()
        {
            if (!IsConsole) return;

            Process p = Process.GetCurrentProcess();
            ShowWindow(p.MainWindowHandle, 3); //SW_MAXIMIZE = 3
        }
    }
}