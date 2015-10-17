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
using System.ComponentModel;
using System.Configuration;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Converters;

namespace WebApplications.Utilities.Configuration
{
    /// <summary>
    ///   Holds a parameter used for object construction.
    /// </summary>
    [PublicAPI]
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
            // ReSharper disable once AssignNullToNotNullAttribute
            get { return (string)this["name"]; }
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
            get { return (string)this["value"]; }
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
            // ReSharper disable once PossibleNullReferenceException
            get { return (bool)this["required"]; }
            set { this["required"] = value; }
        }

        /// <summary>
        ///   Gets or sets the type of the parameter.
        /// </summary>
        /// <exception cref="ConfigurationErrorsException">
        ///   The configuration property is read-only or locked.
        /// </exception>
        [ConfigurationProperty("type", IsRequired = false)]
        [TypeConverter(typeof(SimplifiedTypeNameConverter))]
        [CanBeNull]
        public Type Type
        {
            get { return (Type)this["type"]; }
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
        [TypeConverter(typeof(SimplifiedTypeNameConverter))]
        [SubclassTypeValidator(typeof(TypeConverter))]
        [CanBeNull]
        public Type TypeConverter
        {
            get { return (Type)this["typeConverter"]; }
            set { this["typeConverter"] = value; }
        }
    }
}