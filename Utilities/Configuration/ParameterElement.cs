#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities
// File: ParameterElement.cs
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

namespace WebApplications.Utilities.Configuration
{
    /// <summary>
    ///   Holds a parameter used for object construction.
    /// </summary>
    public class ParameterElement : ConfigurationElement
    {
        /// <summary>
        ///   Gets or sets the parameter's name.
        /// </summary>
        /// <exception cref="ConfigurationErrorsException">
        ///   The configuration property is read-only or locked.
        /// </exception>
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        [NotNull]
        public string Name
        {
            get { return (string) this["name"]; }
            set { this["name"] = value; }
        }

        /// <summary>
        ///   Gets or sets the parameter's value.
        /// </summary>
        /// <exception cref="ConfigurationErrorsException">
        ///   The configuration property is read-only or locked.
        /// </exception>
        [ConfigurationProperty("value", IsRequired = false)]
        [CanBeNull]
        public string Value
        {
            get { return (string) this["value"]; }
            set { this["value"] = value; }
        }

        /// <summary>
        ///   Gets or sets a <see cref="bool"/> value indicating whether this parameter is required.
        /// </summary>
        /// <exception cref="ConfigurationErrorsException">
        ///   The configuration property is read-only or locked.
        /// </exception>
        [ConfigurationProperty("required", DefaultValue = true, IsRequired = false)]
        public bool IsRequired
        {
            get { return (bool) this["required"]; }
            set { this["required"] = value; }
        }

        /// <summary>
        ///   Gets or sets the type of the parameter.
        /// </summary>
        /// <exception cref="ConfigurationErrorsException">
        ///   The configuration property is read-only or locked.
        /// </exception>
        [ConfigurationProperty("type", IsRequired = false)]
        [TypeConverter(typeof (TypeNameConverter))]
        [CanBeNull]
        public Type Type
        {
            get { return (Type) this["type"]; }
            set { this["type"] = value; }
        }

        /// <summary>
        ///   Gets or sets the type convertor, which is used to convert a textual
        ///   representation of the <see cref="object"/> to the actual desired type.
        /// </summary>
        /// <exception cref="ConfigurationErrorsException">
        ///   The configuration property is read-only or locked.
        /// </exception>
        [ConfigurationProperty("typeConverter", IsRequired = false)]
        [TypeConverter(typeof (TypeNameConverter))]
        [SubclassTypeValidator(typeof (TypeConverter))]
        [CanBeNull]
        public Type TypeConverter
        {
            get { return (Type) this["typeConverter"]; }
            set { this["typeConverter"] = value; }
        }
    }
}