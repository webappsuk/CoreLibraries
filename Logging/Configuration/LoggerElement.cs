#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities.Logging 
// Project: WebApplications.Utilities.Logging
// File: LoggerElement.cs
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
using System.ComponentModel;
using System.Configuration;
using JetBrains.Annotations;
using WebApplications.Utilities.Configuration;
using WebApplications.Utilities.Logging.Interfaces;

namespace WebApplications.Utilities.Logging.Configuration
{
    /// <summary>
    ///   A logger element from a configuration file.
    /// </summary>
    public class LoggerElement : ConstructorConfigurationElement
    {
        /// <summary>
        ///   Gets or sets the logger type.
        /// </summary>
        /// <value>The logger type.</value>
        [ConfigurationProperty("type")]
        [TypeConverter(typeof (TypeNameConverter))]
        [SubclassTypeValidator(typeof (ILogger))]
        public override Type Type
        {
            get { return GetProperty<Type>("type"); }
            set { SetProperty("type", value); }
        }

        /// <summary>
        ///   Gets or sets the logger name.
        /// </summary>
        /// <value>The logger name.</value>
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        [NotNull]
        public string Name
        {
            get { return GetProperty<string>("name"); }
            set { SetProperty("name", value); }
        }

        /// <summary>
        ///   Gets a value indicating whether logging is enabled.
        /// </summary>
        [ConfigurationProperty("enabled", DefaultValue = true, IsRequired = false)]
        public bool Enabled
        {
            get { return GetProperty<bool>("enabled"); }
            set { SetProperty("enabled", value); }
        }

        /// <summary>
        ///   Gets the valid <see cref="LogLevels">logging levels</see>.
        /// </summary>
        [ConfigurationProperty("validLevels", DefaultValue = LogLevels.All, IsRequired = false)]
        public LogLevels ValidLevels
        {
            get { return GetProperty<LogLevels>("validLevels"); }
            set { SetProperty("validLevels", value); }
        }
    }
}