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
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Configuration
{
    /// <summary>
    ///   Provides extended functionality for <see cref="System.Configuration.ConfigurationSection"/>.
    ///   Allows easy configuration retrieval.
    /// </summary>
    /// <typeparam name="T">The section type.</typeparam>
    [PublicAPI]
    public abstract class ConfigurationSection<T> : ConfigurationSection
        where T : ConfigurationSection<T>
    {
        #region Delegates
        /// <summary>
        ///   Handles changes in configuration.
        /// </summary>
        /// <param name="sender">The sender (the original configuration).</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public delegate void ConfigurationChangedEventHandler(
            [NotNull] object sender,
            [NotNull] ConfigurationChangedEventArgs e);
        #endregion

        /// <summary>
        ///   Holds the constructor function.
        /// </summary>
        [NotNull]
        private static readonly Func<T> _constructor = typeof(T).ConstructorFunc<T>();

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

                    string sectionName = attribute != null ? attribute.Name : null;

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
        ///   Holds the currently active configuration section.
        /// </summary>
        [CanBeNull]
        private static T _active;

        /// <summary>
        /// Initializes the <see cref="ConfigurationSection{T}"/> class.
        /// </summary>
        static ConfigurationSection()
        {
            ConfigurationFileWatcher.Changed += OnConfigurationFileChanged;
        }

        /// <summary>
        /// Called when the configuration file changes.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private static void OnConfigurationFileChanged(object sender, EventArgs e)
        {
            ConfigurationManager.RefreshSection(SectionName);
            T oldConfiguration = _active;
            _active = GetConfiguration();
            _active.SetReadOnly();

            ConfigurationChangedEventHandler onChanged = Changed;
            if ((oldConfiguration != null) &&
                (onChanged != null))
                onChanged(_active, new ConfigurationChangedEventArgs(oldConfiguration, _active));
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
        public static string SectionName
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            get { return _sectionName.Value; }
        }

        /// <summary>
        ///   Gets or sets the active configuration.
        /// </summary>
        /// <remarks>
        ///   <para>Once set as active a configuration is marked as readonly.</para>
        ///   <para>Setting the active configuration to <see langword="null"/> will load the default configuration.</para>
        /// </remarks>
        [NotNull]
        public static T Active
        {
            get
            {
                if (_active == null)
                {
                    _active = GetConfiguration();
                    _active.SetReadOnly();
                }
                return _active;
            }
            set
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse, HeuristicUnreachableCode
                if (value == null)
                    // ReSharper disable HeuristicUnreachableCode
                    value = GetConfiguration();
                // ReSharper restore HeuristicUnreachableCode
                else if (Equals(_active, value))
                    return;

                T oldConfiguration = _active;
                _active = value;
                _active.SetReadOnly();

                ConfigurationChangedEventHandler onChanged = Changed;
                if ((oldConfiguration != null) &&
                    (onChanged != null))
                    onChanged(_active, new ConfigurationChangedEventArgs(oldConfiguration, _active));
            }
        }

        /// <summary>
        ///   Gets a <see cref="bool"/> value indicating whether this instance is the active configuration.
        /// </summary>
        public bool IsActive
        {
            get { return ReferenceEquals(this, _active); }
        }

        /// <summary>
        ///   Gets the XMLNS.
        /// </summary>
        /// <remarks>
        ///   This allows a configuration section to reference a specific namespace for Visual Studio intellisense support.
        ///   Without this property, specifying a namespace on a configuration section will cause the configuration section
        ///   to fail to load at runtime.
        /// </remarks>
        /// <exception cref="ConfigurationErrorsException">The property is read-only or locked.</exception>
        [ConfigurationProperty("xmlns", IsRequired = false)]
        public string Xmlns
        {
            get { return (string)base["xmlns"]; }
            set { base["xmlns"] = value; }
        }

        /// <summary>
        ///   Occurs when the <see cref="Active"/> ConfigurationSection is changed.
        /// </summary>
        public static event ConfigurationChangedEventHandler Changed;

        /// <summary>
        ///   Gets a <see cref="bool"/> value indicating whether the configuration section is read-only.
        /// </summary>
        /// <returns>
        ///   Returns <see langword="true"/> if the configuration section is read-only; otherwise returns <see langword="false"/>.
        /// </returns>
        public override bool IsReadOnly()
        {
            return IsActive;
        }

        /// <summary>
        ///   Gets the configuration name and stores it in the <see cref="ConfigurationSection&lt;T&gt;.SectionName"/> property.
        /// </summary>
        [NotNull]
        private static T GetConfiguration()
        {
            // We get the configuration from different places, depending on whether we are
            // running as a website or an application.
            T configuration = HttpContext.Current == null
                ? ConfigurationManager.GetSection(SectionName) as T
                : WebConfigurationManager.GetSection(SectionName) as T;

            if (configuration == null)
            {
                configuration = _constructor();
                // ReSharper disable once PossibleNullReferenceException
                configuration.InitializeDefault();
            }
            return configuration;
        }

        /// <summary>
        ///   Gets the configuration property.
        /// </summary>
        /// <typeparam name="TProp">The property type.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The specified property.</returns>
        /// <exception cref="ConfigurationErrorsException">The property is read-only or locked.</exception>
        protected TProp GetProperty<TProp>(string propertyName)
        {
            return (TProp)base[propertyName];
        }

        /// <summary>
        ///   Sets the configuration property.
        /// </summary>
        /// <typeparam name="TProp">The property type.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="value">The value to set the property.</param>
        /// <exception cref="ConfigurationErrorsException">The property is read-only or locked.</exception>
        protected void SetProperty<TProp>(string propertyName, TProp value)
        {
            base[propertyName] = value;
        }

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
            ///   The old configuration (if any).
            /// </summary>
            [NotNull]
            public readonly T OldConfiguration;

            /// <summary>
            ///   Initializes a new instance of the <see cref="ConfigurationSection&lt;T&gt;.ConfigurationChangedEventArgs"/> class.
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