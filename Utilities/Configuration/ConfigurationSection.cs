﻿#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities
// File: ConfigurationSection.cs
// 
// This software, its object code and source code and all modifications made to
// the same (the “Software”) are, and shall at all times remain, the proprietary
// information and intellectual property rights of Web Applications (UK) Limited. 
// You are only entitled to use the Software as expressly permitted by Web
// Applications (UK) Limited within the Software Customisation and
// Licence Agreement (the “Agreement”).  Any copying, modification, decompiling,
// distribution, licensing, sale, transfer or other use of the Software other than
// as expressly permitted in the Agreement is expressly forbidden.  Web
// Applications (UK) Limited reserves its rights to take action against you and
// your employer in accordance with its contractual and common law rights
// (including injunctive relief) should you breach the terms of the Agreement or
// otherwise infringe its copyright or other intellectual property rights in the
// Software.
// 
// © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
#endregion

using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Configuration
{
    /// <summary>
    ///   Provides extended functionality for <see cref="System.Configuration.ConfigurationSection"/>.
    ///   Allows easy configuration retrieval.
    /// </summary>
    /// <typeparam name="T">The section type.</typeparam>
    [UsedImplicitly]
    public abstract class ConfigurationSection<T> : ConfigurationSection where T : ConfigurationSection<T>
    {
        #region Delegates
        /// <summary>
        ///   Handles changes in configuration.
        /// </summary>
        /// <param name="sender">The sender (the original configuration).</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public delegate void ConfigurationChangedEventHandler(
            [NotNull] object sender, [NotNull] ConfigurationChangedEventArgs e);
        #endregion

        // ReSharper disable StaticFieldInGenericType
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

                    if (!String.IsNullOrEmpty(sectionName))
                        return sectionName;

                    // Get type name (after last '.')
                    sectionName = typeof(T).Name;

                    int len = sectionName.Length;

                    // If it ends with 'configuration' strip it.
                    if (len > 20 &&
                        sectionName.EndsWith("configurationsection",
                                             StringComparison.CurrentCultureIgnoreCase))
                        sectionName = sectionName.Substring(0, len -= 20);
                    else if (len > 13 &&
                             sectionName.EndsWith("configuration",
                                                  StringComparison.CurrentCultureIgnoreCase))
                        sectionName = sectionName.Substring(0, len -= 13);
                    else if (len > 7 &&
                             sectionName.EndsWith("section",
                                                  StringComparison.CurrentCultureIgnoreCase))
                        sectionName = sectionName.Substring(0, len -= 7);
                    else if (len > 6 &&
                             sectionName.EndsWith("config",
                                                  StringComparison.CurrentCultureIgnoreCase))
                        sectionName = sectionName.Substring(0, len -= 6);

                    // Convert to lower camel case
                    sectionName = sectionName.Substring(0, 1).ToLower() + (len > 1
                                                                               ? sectionName.
                                                                                     Substring(1)
                                                                               : string.Empty);
                    return sectionName;
                }, LazyThreadSafetyMode.PublicationOnly);

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
        [UsedImplicitly]
        public static string SectionName { get { return _sectionName.Value; } }


        /// <summary>
        ///   Holds the currently active configuration section.
        /// </summary>
        [CanBeNull]
        private static T _active;

        /// <summary>
        ///   Gets or sets the active configuration.
        /// </summary>
        /// <remarks>
        ///   <para>Once set as active a configuration is marked as readonly.</para>
        ///   <para>Setting the active configuration to <see langword="null"/> will load the default configuration.</para>
        /// </remarks>
        public static T Active
        {
            [NotNull]
            get { return _active ?? GetConfiguration(); }
            [CanBeNull]
            set
            {
                if (value == null)
                    value = GetConfiguration();

                if (_active == value)
                    return;

                T oldConfiguration = _active;
                _active = value;
                _active.SetReadOnly();

                if ((oldConfiguration != null) &&
                    (Changed != null))
                    Changed(_active, new ConfigurationChangedEventArgs(oldConfiguration, _active));
            }
        }

        /// <summary>
        ///   Gets a <see cref="bool"/> value indicating whether this instance is the active configuration.
        /// </summary>
        [UsedImplicitly]
        public bool IsActive
        {
            get { return this == _active; }
        }

        /// <summary>
        ///   Occurs when the <see cref="Active"/> ConfigurationSection is changed.
        /// </summary>
        [UsedImplicitly]
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
                configuration.InitializeDefault();
            }
            return configuration;
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
        [UsedImplicitly]
        public string Xmlns
        {
            get { return (string)base["xmlns"]; }
            set { base["xmlns"] = value; }
        }

        #region Nested type: ConfigurationChangedEventArgs
        /// <summary>
        ///   Information about the configuration changed event.
        /// </summary>
        public class ConfigurationChangedEventArgs : EventArgs
        {
            /// <summary>
            ///   The new Configuration
            /// </summary>
            [NotNull]
            [UsedImplicitly]
            public T NewConfiguration;

            /// <summary>
            ///   The old configuration (if any).
            /// </summary>
            [NotNull]
            [UsedImplicitly]
            public T OldConfiguration;

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

        /// <summary>
        ///   Gets the configuration property.
        /// </summary>
        /// <typeparam name="TProp">The property type.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The specified property.</returns>
        /// <exception cref="ConfigurationErrorsException">The property is read-only or locked.</exception>
        [UsedImplicitly]
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
        [UsedImplicitly]
        protected void SetProperty<TProp>(string propertyName, TProp value)
        {
            base[propertyName] = value;
        }
    }
}