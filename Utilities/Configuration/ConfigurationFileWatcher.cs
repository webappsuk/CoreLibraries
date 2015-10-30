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
using System.Collections.Generic;
using System.IO;
using System.Threading;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Threading;

namespace WebApplications.Utilities.Configuration
{
    /// <summary>
    /// Watches for changes to the configuration file for the application.
    /// </summary>
    internal sealed class ConfigurationFileWatcher : IDisposable
    {
        /// <summary>
        /// The file system watchers.
        /// </summary>
        [NotNull]
        private static readonly Dictionary<string, ConfigurationFileWatcher> _watchers
            = new Dictionary<string, ConfigurationFileWatcher>();

        /// <summary>
        /// The sections interested in the file.
        /// </summary>
        [NotNull, ItemNotNull]
        private readonly HashSet<IInternalConfigurationSection> _sections =
            new HashCollection<IInternalConfigurationSection>();

        /// <summary>
        /// The file system watcher.
        /// </summary>
        private FileSystemWatcher _watcher;

        /// <summary>
        /// The event action that buffers events.
        /// </summary>
        private BufferedAction _eventAction;

        /// <summary>
        /// The path being watched.
        /// </summary>
        [NotNull]
        private readonly string _path;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationFileWatcher"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        private ConfigurationFileWatcher([NotNull] string path)
        {
            _eventAction = new BufferedAction(WatcherOnChanged, 100);
            _path = path;
            // ReSharper disable once AssignNullToNotNullAttribute
            _watcher = new FileSystemWatcher(Path.GetDirectoryName(path), Path.GetFileName(path));
            _watcher.Changed += (s, e) => _eventAction.Run();
            _watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size;
            _watcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// Fired when a watcher is changed.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        private void WatcherOnChanged(object[][] arguments)
        {
            string name = $"->{_path}";
            lock (_sections)
                foreach (IInternalConfigurationSection section in _sections)
                    section.OnChanged(section.GetFullPath(name));
        }
        
        /// <summary>
        /// Watches the specified <paramref name="section"/>.
        /// </summary>
        /// <param name="section">The section.</param>
        public static void Watch([NotNull] IInternalConfigurationSection section)
        {
            string path = section.FilePath;
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path)) return;
            lock (_watchers)
            {
                ConfigurationFileWatcher watcher;
                if (!_watchers.TryGetValue(path, out watcher))
                    watcher = _watchers[path] = new ConfigurationFileWatcher(path);
                // ReSharper disable once PossibleNullReferenceException
                lock (watcher._sections)
                    watcher._sections.Add(section);
            }
        }

        /// <summary>
        /// Removes any watch on the specified <paramref name="section"/>.
        /// </summary>
        /// <param name="section">The section.</param>
        public static void UnWatch([NotNull] IInternalConfigurationSection section)
        {
            string path = section.FilePath;
            if (string.IsNullOrWhiteSpace(path)) return;
            lock (_watchers)
            {
                ConfigurationFileWatcher watcher;
                if (!_watchers.TryGetValue(path, out watcher)) return;

                // ReSharper disable once PossibleNullReferenceException
                lock (watcher._sections)
                {
                    watcher._sections.Remove(section);
                    if (watcher._sections.Count > 0) return;
                    watcher.Dispose();
                }
            }
        }

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        public void Dispose()
        {
            // ReSharper disable ExceptionNotDocumented
            FileSystemWatcher watcher = Interlocked.Exchange(ref _watcher, null);
            watcher?.Dispose();
            BufferedAction action = Interlocked.Exchange(ref _eventAction, null);
            action?.Dispose();
            // ReSharper restore ExceptionNotDocumented
        }
    }
}