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
using System.IO;
using System.Linq;
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
    [PublicAPI]
    public abstract partial class ConfigurationSection<T> : ConfigurationSection, IInternalConfigurationSection
        where T : ConfigurationSection<T>, IConfigurationElement, new()
    {
        #region Delegates
        /// <summary>
        /// Handles changes in configuration.
        /// </summary>
        /// <param name="sender">The sender (the original configuration).</param>
        /// <param name="e">The <see cref="ConfigurationChangedEventArgs" /> instance containing the event data.</param>
        public delegate void ConfigurationChangedEventHandler(
            [NotNull] T sender,
            [NotNull] ConfigurationChangedEventArgs e);
        #endregion

        // ReSharper restore AssignNullToNotNullAttribute, PossibleNullReferenceException

        /// <summary>
        /// The change action buffers all changes so that configuration changes don't fire too frequently.
        /// </summary>
        private BufferedAction<IInternalConfigurationElement, string> _changeAction;

        /// <summary>
        /// Initializes static members of the <see cref="ConfigurationSection{T}" /> class.
        /// </summary>
        static ConfigurationSection()
        {

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
        protected ConfigurationSection()
        {
            // Set up change buffer
            _changeAction =
                new BufferedAction<IInternalConfigurationElement, string>(
                    // ReSharper disable once EventExceptionNotDocumented, AssignNullToNotNullAttribute
                    changes => Changed?.Invoke((T)this, new ConfigurationChangedEventArgs(changes)),
                    400);

            // This will get set during initialization
            FilePaths = Array<string>.Empty;
        }

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
        public static readonly string SectionName;

        /// <summary>
        ///   Gets or sets the active configuration.
        /// </summary>
        /// <remarks>
        ///   <para>Once set as active a configuration is marked as read only.</para>
        ///   <para>Setting the active configuration to <see langword="null"/> will load the default configuration.</para>
        /// </remarks>
        /// <exception cref="ObjectDisposedException" accessor="set">You cannot set the active configuration section to
        /// a <see cref="IsDisposed">disposed</see> section.</exception>
        [NotNull]
        public static T Active
        {
            get
            {
                T active = _active;
                if (active != null)
                    active.Changed -= OnActiveChanged;

                if (active == null || active.IsDisposed)
                {
                    _active = active = GetActiveConfiguration();
                    active.Changed += OnActiveChanged;
                }
                return active;
            }
            set
            {
                T active = _active;
                if (Equals(active, value)) return;
                if (value.IsDisposed) throw new ObjectDisposedException(typeof(T).ToString());
                if (active != null)
                    active.Changed -= OnActiveChanged;
                active = value;
                active.Changed += OnActiveChanged;
            }
        }

        /// <summary>
        /// Handles the <see cref="E:ActiveChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ConfigurationSection{T}.ConfigurationChangedEventArgs" /> instance containing the event data.</param>
        private static void OnActiveChanged([NotNull] T sender, [NotNull] ConfigurationChangedEventArgs e)
        {
            ActiveChanged?.Invoke(sender, e);
        }

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
        ///   Occurs when the <see cref="Active"/> ConfigurationSection is changed on disk.
        /// </summary>
        public event ConfigurationChangedEventHandler Changed;

        /// <summary>
        ///   Gets the active configuration.
        /// </summary>
        [NotNull]
        private static T GetActiveConfiguration() => GetOrAdd(
            HttpContext.Current == null
                ? ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None)
                : WebConfigurationManager.OpenWebConfiguration(null));

        /// <summary>
        /// Gets the section from the specified configuration, or adds it.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>T.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="configuration"/> is <see langword="null" />.</exception>
        /// <exception cref="ConfigurationErrorsException">Error during initialization.</exception>
        [NotNull]
        public static T GetOrAdd([NotNull] System.Configuration.Configuration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            // Get the section.
            T section = configuration.GetSection(SectionName) as T;
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

        /// <summary>
        /// Loads the configuration section from the specified file.
        /// </summary>
        /// <param name="filename">The configuration file path.</param>
        /// <returns>A new configuration section.</returns>
        /// <remarks><para>If the file does not exist, it will try to create one.</para></remarks>
        /// <exception cref="ConfigurationErrorsException">Error during initialization.</exception>
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

            if (string.Equals(filename, configuration.FilePath))
                configuration.Save(saveMode, forceSaveAll);
            else
                configuration.SaveAs(filename, saveMode, forceSaveAll);
            ConfigurationManager.RefreshSection(SectionName);
            Dispose();

            return LoadOrCreate(filename);
        }

        /// <inheritdoc />
        IInternalConfigurationSection IInternalConfigurationElement.Section => this;

        /// <inheritdoc />
        void IInternalConfigurationElement.OnChanged(IInternalConfigurationElement sender, string propertyName)
        {
            // Propagate to parent.
            ((IInternalConfigurationElement)this).Parent?.OnChanged(
                sender,
                $"{PropertyName}.{propertyName}");

            _isModified = true;
            _changeAction?.Run(sender, propertyName);
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
                    ConfigurationElement.GetConfigFilePaths(CurrentConfiguration)
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
        protected virtual void Dispose(bool disposing)
        {
            IsDisposed = true;
            if (!disposing) return;
            ConfigurationFileWatcher.UnWatch(this);
            // ReSharper disable once ExceptionNotDocumented
            BufferedAction<IInternalConfigurationElement, string> action = Interlocked.Exchange(ref _changeAction, null);
            action?.Dispose();
        }

        #region Nested type: ConfigurationChangedEventArgs
        /// <summary>
        /// Information about the configuration changed event.
        /// </summary>
        [PublicAPI]
        public class ConfigurationChangedEventArgs : EventArgs, ILookup<IConfigurationElement, string>
        {
            /// <summary>
            /// The changes.
            /// </summary>
            [NotNull]
            private ILookup<IConfigurationElement, string> _changes;

            /// <summary>
            /// Initializes a new instance of the <see cref="ConfigurationSection&lt;T&gt;.ConfigurationChangedEventArgs" /> class.
            /// </summary>
            /// <param name="changes">The changes.</param>
            internal ConfigurationChangedEventArgs(
                [NotNull] [ItemNotNull] IEnumerable<IInternalConfigurationElement, string> changes)
            {
                _changes = changes.ToLookup(t => (IConfigurationElement)t.Item1, t => t.Item2);
            }

            /// <inheritdoc />
            public IEnumerator<IGrouping<IConfigurationElement, string>> GetEnumerator() => _changes.GetEnumerator();

            /// <inheritdoc />
            IEnumerator IEnumerable.GetEnumerator() => _changes.GetEnumerator();

            /// <inheritdoc />
            public bool Contains(IConfigurationElement key) => _changes.Contains(key);

            /// <inheritdoc />
            public int Count => _changes.Count;

            /// <inheritdoc />
            public IEnumerable<string> this[IConfigurationElement key] => _changes[key];
        }
        #endregion
    }
}