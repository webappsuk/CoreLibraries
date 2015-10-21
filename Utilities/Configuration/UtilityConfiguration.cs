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

using System.Configuration;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Globalization;

namespace WebApplications.Utilities.Configuration
{
    /// <summary>
    /// The configuration section for the Utility library.
    /// </summary>
    [PublicAPI]
    public sealed class UtilityConfiguration : ConfigurationSection<UtilityConfiguration>
    {
        static UtilityConfiguration()
        {
            // NOTE: Handlers assigned here to ensure they will always be the first ones invoked
            ActiveChanged += CurrencyInfoProvider.OnActiveUtilityConfigurationChanged;
            ActiveChanged += TimeHelpers.OnActiveUtilityConfigurationChanged;
        }
        
        /// <summary>
        /// Gets or sets the file path for the TimeZone DB.
        /// </summary>
        [ConfigurationProperty("timeZoneDB", DefaultValue = null, IsRequired = false)]
        [CanBeNull]
        public string TimeZoneDB
        {
            get { return GetProperty<string>("timeZoneDB"); }
            set { SetProperty("timeZoneDB", value); }
        }

        /// <summary>
        /// Gets or sets the file path for the ISO 4217 currency information file.
        /// </summary>
        [ConfigurationProperty("iso4217", DefaultValue = null, IsRequired = false)]
        [CanBeNull]
        public string ISO4217
        {
            get { return GetProperty<string>("iso4217"); }
            set { SetProperty("iso4217", value); }
        }
    }
}