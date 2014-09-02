#region © Copyright Web Applications (UK) Ltd, 2014.  All rights reserved.
// Copyright (c) 2014, Web Applications UK Ltd
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

using System.Configuration;
using JetBrains.Annotations;
using ConfigurationElement = WebApplications.Utilities.Configuration.ConfigurationElement;

namespace WebApplications.Utilities.Database.Configuration
{
    /// <summary>
    ///   A configuration element that represents a <see cref="WebApplications.Utilities.Database.SqlProgram"/>'s parameter.
    /// </summary>
    public class ParameterElement : ConfigurationElement
    {
        /// <summary>
        ///   Gets or sets the identifier for the parameter.
        /// </summary>
        /// <value>The name of the element.</value>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">
        ///   The property is read-only or locked.
        /// </exception>
        [ConfigurationProperty("name", IsRequired = true)]
        [NotNull]
        public string Name
        {
            get { return GetProperty<string>("name"); }
            set { SetProperty("name", value); }
        }

        /// <summary>
        ///   Gets or sets the mapTo property, which maps the element to the
        ///   <see cref="WebApplications.Utilities.Database.SqlProgram"/> parameter it represents.
        /// </summary>
        /// <value>The <see cref="string"/> specifying the parameter to map to.</value>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">
        ///   The property is read-only or locked.
        /// </exception>
        [ConfigurationProperty("mapTo", IsRequired = true)]
        [NotNull]
        public string MapTo
        {
            get { return GetProperty<string>("mapTo"); }
            set { SetProperty("mapTo", value); }
        }
    }
}