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
using System.Linq;
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
        [PublicAPI]
        public readonly string Path;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationFileWatcher"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        private ConfigurationFileWatcher([NotNull] string path)
        {
            _eventAction = new BufferedAction(WatcherOnChanged, 100);
            Path = path;
            if (!File.Exists(path)) return;

            // ReSharper disable once AssignNullToNotNullAttribute
            _watcher = new FileSystemWatcher(System.IO.Path.GetDirectoryName(path), System.IO.Path.GetFileName(path));
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
            IInternalConfigurationSection[] sections;
            lock (_sections)
                sections = _sections.ToArray();
            
            foreach (IInternalConfigurationSection section in sections)
                section.OnFileChanged(Path);
        }

        /// <summary>
        /// Watches the specified <paramref name="section"/>.
        /// </summary>
        /// <param name="section">The section.</param>
        public static void Watch([NotNull] IInternalConfigurationSection section)
        {
            string filePath = section.FilePath;
            if (string.IsNullOrWhiteSpace(filePath)) return;

            lock (_watchers)
            {
                ConfigurationFileWatcher watcher;
                // ReSharper disable AssignNullToNotNullAttribute, PossibleNullReferenceException
                if (!_watchers.TryGetValue(filePath, out watcher))
                    watcher = _watchers[filePath] = new ConfigurationFileWatcher(filePath);
                lock (watcher._sections)
                    watcher._sections.Add(section);
                // ReSharper restore AssignNullToNotNullAttribute, PossibleNullReferenceException
            }
        }

        /// <summary>
        /// Removes any watch on the specified <paramref name="section"/>.
        /// </summary>
        /// <param name="section">The section.</param>
        public static void UnWatch([NotNull] IInternalConfigurationSection section)
        {
            string filePath = section.FilePath;
            if (string.IsNullOrWhiteSpace(filePath)) return;

            lock (_watchers)
            {
                ConfigurationFileWatcher watcher;
                // ReSharper disable AssignNullToNotNullAttribute, PossibleNullReferenceException
                if (!_watchers.TryGetValue(filePath, out watcher)) return;

                lock (watcher._sections)
                {
                    watcher._sections.Remove(section);
                    if (watcher._sections.Count > 0) return;
                    watcher.Dispose();
                }
                // ReSharper restore AssignNullToNotNullAttribute, PossibleNullReferenceException
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

        /// <inheritdoc />
        public override string ToString() => Path;
    }
}