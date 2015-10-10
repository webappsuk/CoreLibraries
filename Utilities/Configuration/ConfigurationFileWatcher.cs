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
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Threading;

namespace WebApplications.Utilities.Configuration
{
    /// <summary>
    /// Watches for changes to the configuration file for the application.
    /// </summary>
    public static class ConfigurationFileWatcher
    {
        private static readonly FileSystemWatcher _configWatcher;

        [NotNull]
        private static readonly AsyncDebouncedAction _onChangedAction =
            new AsyncDebouncedAction(ConfigFileChangedHandler);

        /// <summary>
        /// Occurs when the configuration file changes on disk.
        /// </summary>
        public static event EventHandler Changed;

        static ConfigurationFileWatcher()
        {
            // TODO Watch machine/user configs

            string configPath = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            if (!File.Exists(configPath))
                return;

            string dir = Path.GetDirectoryName(configPath);
            string file = Path.GetFileName(configPath);

            Debug.Assert(dir != null);
            Debug.Assert(file != null);

            _configWatcher = new FileSystemWatcher(dir, file);
            _configWatcher.Changed += OnConfigFileChanged;
            _configWatcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// Called when the configuration file changes.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="FileSystemEventArgs"/> instance containing the event data.</param>
        private static void OnConfigFileChanged(object sender, FileSystemEventArgs e)
        {
#pragma warning disable 4014
            _onChangedAction.Run();
#pragma warning restore 4014
        }

        /// <summary>
        /// Called when the configuration file changes.
        /// </summary>
        /// <remarks>The event could be fired multiple times for a single change. To prevent unnessacary refreshes, we wait for 100ms after a change before we fire the handler.</remarks>
        private static async Task ConfigFileChangedHandler()
        {
            Debug.Assert(_configWatcher != null);

            // ReSharper disable once PossibleNullReferenceException
            await Task.Delay(100).ConfigureAwait(false);

            EventHandler handler = Changed;
            handler?.Invoke(_configWatcher, EventArgs.Empty);
        }
    }
}