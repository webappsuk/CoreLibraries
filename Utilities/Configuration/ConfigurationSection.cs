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
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Xml.Linq;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Threading;

namespace WebApplications.Utilities.Configuration
{
    /// <summary>
    /// Provides extended functionality for <see cref="System.Configuration.ConfigurationSection" />.
    /// Allows easy configuration retrieval.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [PublicAPI]
    public abstract partial class ConfigurationSection<T> : ConfigurationSection, IInternalConfigurationSection
        where T : ConfigurationSection<T>, IConfigurationElement, new()
    {
        /// <summary>
        /// The time in milliseconds that events are buffered for.
        /// </summary>
        public const int EventBufferMs = 250;

        #region Delegates
        /// <summary>
        /// Handles changes in configuration.
        /// </summary>
        /// <param name="sender">The sender (the configuration section).</param>
        /// <param name="e">The <see cref="ConfigurationChangedEventArgs" /> instance containing the event data.</param>
        public delegate void ConfigurationChangedEventHandler(
            [NotNull] T sender,
            [NotNull] ConfigurationChangedEventArgs e);

        /// <summary>
        /// Handles load errors.
        /// </summary>
        /// <param name="e">The <see cref="ConfigurationLoadEventArgs" /> instance containing the event data.</param>
        public delegate void ConfigurationLoadErrorEventHandler([NotNull] ConfigurationLoadEventArgs e);
        #endregion

        // ReSharper restore AssignNullToNotNullAttribute, PossibleNullReferenceException

        /// <summary>
        /// The change action buffers all changes so that configuration changes don't fire too frequently.
        /// </summary>
        [NotNull]
        // ReSharper disable once StaticMemberInGenericType
        private static BufferedAction<string> _activeChangeAction;

        /// <summary>
        /// The change action buffers all changes so that configuration changes don't fire too frequently.
        /// </summary>
        private BufferedAction<string> _changeAction;

        /// <summary>
        /// Initializes static members of the <see cref="ConfigurationSection{T}" /> class.
        /// </summary>
        static ConfigurationSection()
        {
            _activeChangeAction =
                new BufferedAction<string>(
                    // ReSharper disable EventExceptionNotDocumented, AssignNullToNotNullAttribute
                    changes =>
                    {
                        try
                        {
                            T active = Active;
                            ActiveChanged?.Invoke(active, new ConfigurationChangedEventArgs(active, changes));
                        }
                        // ReSharper disable once CatchAllClause
                        catch
                        {
                            // ignored
                        }
                    },
                    // ReSharper restore EventExceptionNotDocumented, AssignNullToNotNullAttribute
                    EventBufferMs);

            // Try to find attribute
            ConfigurationSectionAttribute attribute =
                (ConfigurationSectionAttribute)
                    typeof(T).GetCustomAttributes(typeof(ConfigurationSectionAttribute), false).
                        FirstOrDefault();

            string sectionName = attribute?.Name;

            if (string.IsNullOrEmpty(sectionName))
            {
                sectionName = typeof(T).Name;

                int len = sectionName.Length;

                // If it ends with 'configuration' strip it.
                if (len > 20 &&
                    sectionName.EndsWith(
                        "configurationsection",
                        StringComparison.CurrentCultureIgnoreCase))
                    sectionName = sectionName.Substring(0, len -= 20);
                else if (len > 13 &&
                         sectionName.EndsWith(
                             "configuration",
                             StringComparison.CurrentCultureIgnoreCase))
                    sectionName = sectionName.Substring(0, len -= 13);
                else if (len > 7 &&
                         sectionName.EndsWith(
                             "section",
                             StringComparison.CurrentCultureIgnoreCase))
                    sectionName = sectionName.Substring(0, len -= 7);
                else if (len > 6 &&
                         sectionName.EndsWith(
                             "config",
                             StringComparison.CurrentCultureIgnoreCase))
                    sectionName = sectionName.Substring(0, len -= 6);

                // Convert to lower camel case
                sectionName = sectionName.Substring(0, 1).ToLower() + (len > 1
                    ? sectionName.
                        Substring(1)
                    : string.Empty);
            }

            SectionName = sectionName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationSection{T}" /> class.
        /// </summary>
        /// <exception cref="ConfigurationErrorsException">The selected value conflicts with a value that is already defined.</exception>
        protected ConfigurationSection()
        {
            // Set up change buffer
            _changeAction =
                new BufferedAction<string>(
                    // ReSharper disable once EventExceptionNotDocumented, AssignNullToNotNullAttribute
                    changes => Changed?.Invoke((T)this, new ConfigurationChangedEventArgs((T)this, changes)),
                    EventBufferMs);

            // This will get set during initialization
            FilePaths = Array<string>.Empty;
            ((IInternalConfigurationElement)this).ConfigurationElementName = $"<{SectionName}>";

            // As our system supports change notification, we can default the restart on external changes to false.
            SectionInformation.RestartOnExternalChanges = false;
        }

        /// <summary>
        /// Lock for controlling access to the active configuration.
        /// </summary>
        [NotNull]
        // ReSharper disable once StaticMemberInGenericType
        private static object _activeLock = new object();

        /// <summary>
        ///   Holds the currently active configuration section.
        /// </summary>
        [CanBeNull]
        private static T _active;

        /// <summary>
        /// Creates an instance of this section, and initializes it properly.
        /// </summary>
        /// <returns>A <see cref="ConfigurationSection{T}"/> of the correct type.</returns>
        /// <exception cref="ConfigurationErrorsException">Error during initialization.</exception>
        [NotNull]
        public static T Create()
        {
            T section = new T();
            section.Init();
            return section;
        }

        /// <summary>
        ///   Gets the name of the configuration section.
        /// </summary>
        /// <remarks>
        ///   <para>If you do not explicitly define the section name using the <see cref="ConfigurationSectionAttribute"/>
        ///   then  the name is calculated automatically from the type name using the following algorithm:</para>
        ///   <list type="number">
        ///     <item><description>Take the type name.</description></item>
        ///     <item><description>Remove one of the following suffixes if present -
        ///     'ConfigurationSection', 'Configuration', 'Section', 'Config'.</description></item>
        ///     <item><description>Make the first character lower case (camel case).</description></item>
        ///   </list>
        ///   <para>For example: 'MyClassConfigurationSection' will have a name of 'myClass'.</para>
        /// </remarks>
        [NotNull]
        // ReSharper disable once StaticMemberInGenericType
        public static readonly string SectionName;

        // ReSharper disable ExceptionNotThrown
        /// <summary>
        ///   Gets or sets the active configuration.
        /// </summary>
        /// <remarks>
        ///   <para>Once set as active a configuration is marked as read only.</para>
        ///   <para>Setting the active configuration to <see langword="null"/> will load the default configuration.</para>
        /// </remarks>
        /// <exception cref="ObjectDisposedException" accessor="set">You cannot set the active configuration section to
        /// a <see cref="IsDisposed">disposed</see> section.</exception>
        /// <exception cref="ConfigurationErrorsException" accessor="get">Error during initialization.</exception>
        /// <exception cref="UnauthorizedAccessException" accessor="get">The caller does not have the required permission. </exception>
        /// <exception cref="IOException" accessor="get">An I/O error occurs. </exception>
        [NotNull]
        public static T Active
        {
            get
            {
                // Get without lock
                T active = _active;
                if (active != null && !active.IsDisposed) return active;

                lock (_activeLock)
                {
                    // Check again
                    active = _active;
                    if (active != null && !active.IsDisposed) return active;

                    // ReSharper disable ExceptionNotDocumented
                    return _active = GetOrAdd(
                        HttpContext.Current == null
                            ? ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None)
                            : WebConfigurationManager.OpenWebConfiguration(null));
                    // ReSharper restore ExceptionNotDocumented
                }
            }
            set
            {
                T active = _active;
                if (Equals(active, value)) return;

                lock (_activeLock)
                {
                    active = _active;
                    if (Equals(active, value)) return;
                    if (value.IsDisposed) throw new ObjectDisposedException(typeof(T).ToString());
                    _active = value;
                }
            }
        }
        // ReSharper restore ExceptionNotThrown

        /// <summary>
        ///   Gets a <see cref="bool"/> value indicating whether this instance is the active configuration.
        /// </summary>
        public bool IsActive => ReferenceEquals(this, _active);

        /// <summary>
        ///   Gets the XMLNS.
        /// </summary>
        /// <remarks>
        ///   This allows a configuration section to reference a specific namespace for Visual Studio intellisense support.
        ///   Without this property, specifying a namespace on a configuration section will cause the configuration section
        ///   to fail to load at runtime.
        /// </remarks>
        [ConfigurationProperty("xmlns", IsRequired = false)]
        public XNamespace Xmlns
        {
            get { return (XNamespace)this["xmlns"]; }
            set { this["xmlns"] = value; }
        }

        /// <summary>
        ///   Occurs when the <see cref="Active"/> ConfigurationSection is changed on disk.
        /// </summary>
        public static event ConfigurationChangedEventHandler ActiveChanged;

        /// <summary>
        /// Occurs when an error is thrown loading a configuration.
        /// </summary>
        public static event ConfigurationLoadErrorEventHandler ConfigurationLoadError;

        /// <summary>
        ///   Occurs when the <see cref="Active"/> ConfigurationSection is changed on disk.
        /// </summary>
        public event ConfigurationChangedEventHandler Changed;

        /// <summary>
        /// Gets the section from the specified configuration, or adds it.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>T.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="configuration"/> is <see langword="null" />.</exception>
        /// <exception cref="ConfigurationErrorsException">Error during initialization.</exception>
        /// <exception cref="ObjectDisposedException">The current section <see cref="IsDisposed">is disposed</see>.</exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive). </exception>
        /// <exception cref="UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="IOException">An I/O error occurs. </exception>
        [NotNull]
        public static T GetOrAdd([NotNull] System.Configuration.Configuration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            // Get the section.
            T section;
            try
            {
                section = configuration.GetSection(SectionName) as T;
            }
            // ReSharper disable once CatchAllClause
            catch (Exception e)
            {
                // Intercept exception to throw
                ExceptionDispatchInfo edi = ExceptionDispatchInfo.Capture(e);
                // ReSharper disable once AssignNullToNotNullAttribute, EventExceptionNotDocumented
                ConfigurationLoadError?.Invoke(new ConfigurationLoadEventArgs(SectionName, edi));

                // ReSharper disable once ExceptionNotDocumented
                // ReSharper disable once ThrowingSystemException
                throw;
            }

            // Return if found.
            if (section != null) return section;

            // Create new configuration and set it as the active one.
            section = Create();

            // Add to existing configuration and save
            configuration.Sections.Add(SectionName, section);

            // If the section has an associated file, save and reload.
            if (section.HasFile)
                section = section.Save();
            return section;
        }

#pragma warning disable 618
        /// <summary>
        /// Loads the configuration section from the specified file.
        /// </summary>
        /// <param name="filename">The configuration file path.</param>
        /// <returns>A new configuration section.</returns>
        /// <remarks><para>If the file does not exist, it will try to create one.</para></remarks>
        /// <exception cref="ConfigurationErrorsException">Error during initialization.</exception>
        /// <exception cref="IOException">An I/O error occurs. </exception>
        /// <exception cref="UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive). </exception>
        /// <exception cref="ArgumentException"><paramref name="filename" /> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by <see cref="F:System.IO.Path.InvalidPathChars" />. </exception>
        [NotNull]
        public static T LoadOrCreate([NotNull] string filename)
        {
            // Create a blank configuration file if the file doesn't exist.
            if (!File.Exists(filename))
                using (StreamWriter writer = File.CreateText(filename))
                {
                    writer.WriteLine("<?xml version=\"1.0\"?>");
                    writer.WriteLine("<configuration>");
                    writer.WriteLine("</configuration>");
                }

            return GetOrAdd(ConfigurationManager.OpenExeConfiguration(filename));
        }

        /// <summary>
        /// Saves the section.
        /// </summary>
        /// <param name="saveMode">The save mode.</param>
        /// <param name="forceSaveAll">
        ///   <see langword="true" /> to save even if the configuration was not modified.</param>
        /// <returns>A new saved configuration section.</returns>
        /// <exception cref="ConfigurationErrorsException">The current section is not associated with a configuration.</exception>
        /// <remarks>After saving a configuration section it is disposed, so you must use the returned section.</remarks>
        /// <exception cref="ObjectDisposedException">The current section <see cref="IsDisposed">is disposed</see>.</exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive). </exception>
        /// <exception cref="UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="IOException">An I/O error occurs. </exception>
        [NotNull]
        public T Save(ConfigurationSaveMode saveMode = ConfigurationSaveMode.Modified, bool forceSaveAll = false)
            // ReSharper disable once AssignNullToNotNullAttribute
            => SaveAs(FilePath, saveMode, forceSaveAll);

        /// <summary>
        /// Saves as.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="saveMode">The save mode.</param>
        /// <param name="forceSaveAll">
        ///   <see langword="true" /> to save even if the configuration was not modified.</param>
        /// <returns>A new saved configuration section.</returns>
        /// <exception cref="ConfigurationErrorsException">The current section is not associated with a configuration.</exception>
        /// <remarks>After saving a configuration section it is disposed, so you must use the returned section.</remarks>
        /// <exception cref="ObjectDisposedException">The current section <see cref="IsDisposed">is disposed</see>.</exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive). </exception>
        /// <exception cref="UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="IOException">An I/O error occurs. </exception>
        /// <exception cref="ArgumentException"><paramref name="filename" /> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by <see cref="F:System.IO.Path.InvalidPathChars" />.</exception>
        [NotNull]
        public T SaveAs(
            [NotNull] string filename,
            ConfigurationSaveMode saveMode = ConfigurationSaveMode.Modified,
            bool forceSaveAll = false)
        {
            if (IsDisposed) throw new ObjectDisposedException(ToString());

            System.Configuration.Configuration configuration = CurrentConfiguration;
            if (string.IsNullOrWhiteSpace(filename) || configuration == null)
                throw new ConfigurationErrorsException(Resources.ConfigurationSection_Save_No_Configuration);

            // If this is the active configuration, ensure it is unloaded, this will make it impossible the file
            // save event will be raised against the active configuration.
            // ReSharper disable once ExceptionNotDocumented
            bool wasActive = ReferenceEquals(Interlocked.CompareExchange(ref _active, null, (T)this), this);

            if (string.Equals(filename, configuration.FilePath))
                configuration.Save(saveMode, forceSaveAll);
            else
                configuration.SaveAs(filename, saveMode, forceSaveAll);

            // Tell the configuration manager to refresh the section.
            ConfigurationManager.RefreshSection(SectionName);

            // We dispose as any saved configuration must be reloaded before being used again.
            Dispose();

            T config = LoadOrCreate(filename);
            if (wasActive)
                Active = config;

            return config;
        }
#pragma warning restore 618

        /// <inheritdoc />
        IInternalConfigurationSection IInternalConfigurationElement.Section => this;

        /// <inheritdoc />
        void IInternalConfigurationSection.OnFileChanged(string fullPath)
        {
            if (IsDisposed)
            {
                Trace.WriteLine($"#{InstanceNumber} - Config file '{fullPath}' changed for disposed {SectionName} section.");
                return;
            }

            // Raise change event.
            ((IInternalConfigurationElement)this).OnChanged(FullPath);

            // If this is the active configuration, ensure it is unloaded, this will make it impossible the file
            // save event will be raised against the active configuration.
            // ReSharper disable once ExceptionNotDocumented
            bool wasActive = ReferenceEquals(Interlocked.CompareExchange(ref _active, null, (T)this), this);

            // Tell the configuration manager to refresh the section.
            ConfigurationManager.RefreshSection(SectionName);

            // We need to dispose this section, as it's no longer active.
            Dispose();

            if (wasActive)
            {
                // ReSharper disable once UnusedVariable
                T active = Active;
                Trace.WriteLine($"#{InstanceNumber} - Config file '{fullPath}' changed for active {SectionName} section. - new Active instance #{active.InstanceNumber}");
            }
            else
                Trace.WriteLine($"#{InstanceNumber} - Config file '{fullPath}' changed for inactive {SectionName} section.");
        }

        /// <summary>
        /// Called when the OnChange event is called.
        /// </summary>
        /// <param name="fullPath">The full path.</param>
        partial void DoChanged(string fullPath)
        {
            _changeAction?.Run(fullPath);

            // Check to see if we're the active configuration.
            if (ReferenceEquals(this, _active))
                _activeChangeAction.Run(fullPath);
        }

        /// <inheritdoc />
        public bool HasFile => !string.IsNullOrWhiteSpace(FilePath);

        /// <inheritdoc />
        public string FilePath => FilePaths.FirstOrDefault();

        /// <inheritdoc />
        public IReadOnlyCollection<string> FilePaths { get; private set; }

        /// <inheritdoc />
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Called when initializing.
        /// </summary>
        partial void OnInit()
        {
            // We set the file paths once and don't change again.
            System.Configuration.Configuration configuration = CurrentConfiguration;
            if (configuration != null &&
                configuration.HasFile)
            {
                // Sanitize paths
                HashSet<string> set = new HashSet<string>(
                    // ReSharper disable once AssignNullToNotNullAttribute
                    CurrentConfiguration.GetConfigFilePaths()
                        .Where(p => p != null)
                        .Select(Path.GetFullPath)
                        .Where(File.Exists));

                string root = Path.GetFullPath(configuration.FilePath);
                int count = set.Count;
                if (count > 0)
                {
                    if (count < 2 || !set.Contains(root))
                        FilePaths = set.ToArray();
                    else
                    {
                        // Ensure root is always first
                        set.Remove(root);
                        string[] fps = new string[count];
                        fps[0] = root;
                        set.CopyTo(fps, 1);
                        FilePaths = fps;
                    }
                }
            }

            if (FilePaths.Count > 0)
                ConfigurationFileWatcher.Watch(this);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the specified instance.
        /// </summary>
        /// <param name="disposing">Whether this is disposing or finalizing.</param>
        /// <remarks>
        /// <para><paramref name="disposing"/> indicates whether the method was invoked from the 
        /// <see cref="IDisposable.Dispose"/> implementation or from the finalizer. The implementation should check the
        /// parameter before  accessing other reference objects. Such objects should  only be accessed when the method 
        /// is called from the <see cref="IDisposable.Dispose"/> implementation (when the <paramref name="disposing"/> 
        /// parameter is equal to <see langword="true"/>). If the method is invoked from the finalizer
        /// (disposing is false), other objects should not be accessed. The reason is that objects are finalized in an 
        /// unpredictable order and so they, or any of their dependencies, might already have been finalized.</para>
        /// </remarks>
        // ReSharper disable once VirtualMemberNeverOverriden.Global
        protected virtual void Dispose(bool disposing)
        {
            IsDisposed = true;
            // ReSharper disable once ExceptionNotDocumented
            // If we're disposing the active configuration section, we want to reload the active configuration.
            Interlocked.CompareExchange(ref _active, null, (T)this);
            if (!disposing) return;
            Trace.WriteLine($"Disposing {InstanceNumber}");
            ConfigurationFileWatcher.UnWatch(this);
            // ReSharper disable once ExceptionNotDocumented
            BufferedAction<string> action = Interlocked.Exchange(ref _changeAction, null);
            action?.Dispose();
        }

#if DEBUG
        public static int InstanceCount;
        public readonly int InstanceNumber = InstanceCount++;
#endif

        /// <summary>
        /// Information about the configuration changed event.
        /// </summary>
        [PublicAPI]
        public class ConfigurationChangedEventArgs : EventArgs, IReadOnlyCollection<string>
        {
            /// <summary>
            /// The section.
            /// </summary>
            [NotNull]
            public readonly T Section;

            /// <summary>
            /// The changes.
            /// </summary>
            [NotNull]
            private IReadOnlyCollection<string> _changes;

            /// <summary>
            /// Initializes a new instance of the <see cref="ConfigurationSection&lt;T&gt;.ConfigurationChangedEventArgs" /> class.
            /// </summary>
            /// <param name="section">The section.</param>
            /// <param name="changes">The changes.</param>
            internal ConfigurationChangedEventArgs(
                [NotNull] T section,
                [NotNull] [ItemNotNull] IEnumerable<string> changes)
            {
                Section = section;
                _changes = changes.ToArray();
            }

            /// <inheritdoc />
            public IEnumerator<string> GetEnumerator() => _changes.GetEnumerator();

            /// <inheritdoc />
            IEnumerator IEnumerable.GetEnumerator() => _changes.GetEnumerator();

            /// <inheritdoc />
            public bool Contains(string fullPath) => _changes.Contains(fullPath);

            /// <inheritdoc />
            public int Count => _changes.Count;

            /// <inheritdoc />
            public override string ToString()
                => "Changes: " + string.Join(", ", _changes);

            /// <summary>
            /// Checks whether the <paramref name="path"/> was affected by a change.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns><see langword="true"/> if the <paramref name="path"/> starts with any of the changed paths; otherwise <see langword="false"/>.</returns>
            public bool WasChanged([NotNull] string path) => !string.IsNullOrWhiteSpace(path) || _changes.Any(path.StartsWith);
        }

        /// <summary>
        /// Information about exceptions raised when loading a configuration.
        /// </summary>
        public class ConfigurationLoadEventArgs : EventArgs
        {
            /// <summary>
            /// The section name.
            /// </summary>
            [NotNull]
            public readonly string SectionName;

            /// <summary>
            /// The exception dispatch information.
            /// </summary>
            [NotNull]
            private readonly ExceptionDispatchInfo _exceptionDispatchInfo;

            /// <summary>
            /// Initializes a new instance of the <see cref="ConfigurationSection{T}.ConfigurationLoadEventArgs" /> class.
            /// </summary>
            /// <param name="sectionName">Name of the section.</param>
            /// <param name="exceptionDispatchInfo">The exception dispatch information.</param>
            internal ConfigurationLoadEventArgs(
                            [NotNull]string sectionName,
                            [NotNull]ExceptionDispatchInfo exceptionDispatchInfo)
            {
                SectionName = sectionName;
                _exceptionDispatchInfo = exceptionDispatchInfo;
            }

            /// <summary>
            /// Gets the exception.
            /// </summary>
            /// <value>The exception.</value>
            public Exception Exception => _exceptionDispatchInfo.SourceException;

            /// <summary>
            /// Throws the exception again.
            /// </summary>
            public void Throw() => _exceptionDispatchInfo.Throw();
        }
    }
}