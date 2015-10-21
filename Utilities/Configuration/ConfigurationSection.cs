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
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Xml.Linq;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Configuration
{
    /// <summary>
    /// Provides extended functionality for <see cref="System.Configuration.ConfigurationSection" />.
    /// Allows easy configuration retrieval.
    /// </summary>
    [PublicAPI]
    public abstract partial class ConfigurationSection<T> : ConfigurationSection, IInternalConfigurationElement
        where T : ConfigurationSection<T>, IConfigurationElement, new()
    {
        #region Delegates
        /// <summary>
        /// Handles changes in configuration.
        /// </summary>
        /// <param name="sender">The sender (the original configuration).</param>
        /// <param name="e">The <see cref="ConfigurationChangedEventArgs" /> instance containing the event data.</param>
        public delegate void ConfigurationChangedEventHandler(
            [NotNull] object sender,
            [NotNull] ConfigurationChangedEventArgs e);
        #endregion

        /// <summary>
        /// Calculates the section name.
        /// </summary>
        [NotNull]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static readonly Lazy<string> _sectionName =
            new Lazy<string>(
                () =>
                {
                    // Try to find attribute
                    ConfigurationSectionAttribute attribute =
                        (ConfigurationSectionAttribute)
                            typeof(T).GetCustomAttributes(typeof(ConfigurationSectionAttribute), false).
                                FirstOrDefault();

                    string sectionName = attribute?.Name;

                    if (!string.IsNullOrEmpty(sectionName))
                        return sectionName;

                    // Get type name (after last '.')
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
                    return sectionName;
                },
                LazyThreadSafetyMode.PublicationOnly);


        /// <summary>
        /// Flag to indicate that this configuration needs to be reloaded.
        /// </summary>
        private bool _forceReload;

        /// <summary>
        ///   Holds the currently active configuration section.
        /// </summary>
        [CanBeNull]
        private static T _active;

        [CanBeNull]
        private FileSystemWatcher _configWatcher;

        /// <summary>
        /// Handles the <see cref="FileSystemWatcher.Changed">configuration file changed event</see>.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.IO.FileSystemEventArgs" /> instance containing the event data.</param>
        private void OnConfigFileChanged(object sender, FileSystemEventArgs e)
        {
            // Only detect the first change, after which we have to be reloaded anyway.
            FileSystemWatcher configWatcher = Interlocked.Exchange(ref _configWatcher, null);
            configWatcher?.Dispose();

            _forceReload = true;

            ConfigurationManager.RefreshSection(SectionName);
            T newConfiguration = CurrentConfiguration?.GetSection(SectionName) as T;
            if (newConfiguration != null)
                Changed?.Invoke(this, new ConfigurationChangedEventArgs((T)this, newConfiguration));
        }

        /// <summary>
        /// Handles the <see cref="E:ActiveChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="WebApplications.Utilities.Configuration.ConfigurationSection{T}.ConfigurationChangedEventArgs" /> instance containing the event data.</param>
        private static void OnActiveChanged(object sender, ConfigurationChangedEventArgs e)
        {
            _active = e.NewConfiguration;
            ActiveChanged?.Invoke(_active, e);
        }

        /// <summary>
        /// Creates an instance of this section.
        /// </summary>
        /// <returns>A <see cref="ConfigurationSection{T}"/> of the correct type.</returns>
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
        public static string SectionName => _sectionName.Value;

        /// <summary>
        ///   Gets or sets the active configuration.
        /// </summary>
        /// <remarks>
        ///   <para>Once set as active a configuration is marked as read only.</para>
        ///   <para>Setting the active configuration to <see langword="null"/> will load the default configuration.</para>
        /// </remarks>
        [NotNull]
        public static T Active
        {
            get
            {
                T active = _active;
                if (active == null || active._forceReload)
                {
                    if (active != null)
                        active.Changed -= OnActiveChanged;

                    _active = active = GetConfiguration();
                    active.Changed += OnActiveChanged;
                }
                return active;
            }
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
        ///   Gets the configuration.
        /// </summary>
        [NotNull]
        private static T GetConfiguration()
        {
            do
            {
                System.Configuration.Configuration configuration = HttpContext.Current == null
                    ? ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None)
                    : WebConfigurationManager.OpenWebConfiguration(null);

                // We get the configuration from different places, depending on whether we are
                // running as a website or an application.
                T section = configuration.GetSection(SectionName) as T;
                if (section != null) return section;

                // Create new configuration and set it as the active one.
                section = Create();

                // Add to existing configuration and save
                configuration.Sections.Add(SectionName, section);
                configuration.Save();

                // We need to reload after a save!
                section._forceReload = true;
            } while (true);
        }

        /// <summary>
        /// Saves the section.
        /// </summary>
        /// <param name="saveMode">The save mode.</param>
        /// <param name="forceSaveAll"><see langword="true"/> to save even if the configuration was not modified.</param>
        /// <exception cref="ConfigurationErrorsException">The current section is not associated with a configuration.</exception>
        /// <remarks>After saving a configuration section you should always retrieve it using <see cref="Active" />
        /// before using again.</remarks>
        public void Save(ConfigurationSaveMode saveMode = ConfigurationSaveMode.Modified, bool forceSaveAll = false)
        {
            System.Configuration.Configuration configuration = CurrentConfiguration;
            if (configuration == null)
                throw new ConfigurationErrorsException(Resources.ConfigurationSection_Save_No_Configuration);

            configuration.Save(saveMode, forceSaveAll);
            ConfigurationManager.RefreshSection(SectionName);

            // Force configuration to reload after a save.
            _forceReload = true;
        }

        /// <summary>
        /// Saves as.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="saveMode">The save mode.</param>
        /// <param name="forceSaveAll">
        ///   <see langword="true" /> to save even if the configuration was not modified.</param>
        /// <exception cref="ArgumentNullException"><paramref name="filename"/> is <see langword="null" />.</exception>
        /// <exception cref="ConfigurationErrorsException">The current section is not associated with a configuration.</exception>
        public void SaveAs([NotNull] string filename, ConfigurationSaveMode saveMode = ConfigurationSaveMode.Modified, bool forceSaveAll = false)
        {
            if (filename == null)
                throw new ArgumentNullException(nameof(filename));

            System.Configuration.Configuration configuration = CurrentConfiguration;
            if (configuration == null)
                throw new ConfigurationErrorsException(Resources.ConfigurationSection_Save_No_Configuration);

            configuration.SaveAs(filename, saveMode, forceSaveAll);
            ConfigurationManager.RefreshSection(SectionName);

            // Force configuration to reload after a save.
            _forceReload = true;
        }

        /// <inheritdoc />
        public IConfigurationElement Section => this;

        #region Nested type: ConfigurationChangedEventArgs
        /// <summary>
        ///   Information about the configuration changed event.
        /// </summary>
        [PublicAPI]
        public class ConfigurationChangedEventArgs : EventArgs
        {
            /// <summary>
            ///   The new Configuration
            /// </summary>
            [NotNull]
            public readonly T NewConfiguration;

            /// <summary>
            ///   The old configuration.
            /// </summary>
            [NotNull]
            public readonly T OldConfiguration;

            /// <summary>
            /// Initializes a new instance of the <see cref="ConfigurationSection&lt;T&gt;.ConfigurationChangedEventArgs" /> class.
            /// </summary>
            /// <param name="oldConfiguration">The old configuration.</param>
            /// <param name="newConfiguration">The new configuration.</param>
            public ConfigurationChangedEventArgs([NotNull] T oldConfiguration, [NotNull] T newConfiguration)
            {
                OldConfiguration = oldConfiguration;
                NewConfiguration = newConfiguration;
            }
        }
        #endregion
    }
}