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
using System.Diagnostics;
using System.Text;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Configuration;

namespace WebApplications.Utilities.Cryptography.Configuration
{
    /// <summary>
    /// Configuration section for cryptography
    /// </summary>
    public class CryptographyConfiguration : ConfigurationSection<CryptographyConfiguration>
    {
        /// <summary>
        /// Gets or sets the <see cref="ProviderCollection">providers</see>.
        /// </summary>
        [ConfigurationProperty("providers", IsRequired = true, IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(ProviderCollection),
            CollectionType = ConfigurationElementCollectionType.BasicMapAlternate)]
        [NotNull]
        [ItemNotNull]
        public ProviderCollection Providers
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            get { return GetProperty<ProviderCollection>("providers"); }
            set { SetProperty("providers", value); }
        }

        /// <summary>
        /// Gets the raw XML representation of this section.
        /// </summary>
        internal string RawXml
        {
            get
            {
                StringBuilder xml = new StringBuilder("<cryptography><providers>");

                foreach (ProviderElement provider in Providers)
                {
                    Debug.Assert(provider != null);
                    Debug.Assert(provider.Type != null);

                    xml.Append("<add name=\"")
                        .Append(provider.Name)
                        .Append("\" type=\"")
                        .Append(provider.Type.SimplifiedTypeFullName())
                        .Append("\" enabled=\"")
                        .Append(provider.IsEnabled)
                        .Append("\" id=\"")
                        .Append(provider.Id)
                        .Append("\" keyLifeInDays=\"")
                        .Append(provider.KeyLifeInDays)
                        .Append("\"");
                    if (provider.Keys.Count > 0)
                    {
                        xml.Append("><keys>");
                        foreach (KeyElement key in provider.Keys)
                        {
                            Debug.Assert(key != null);

                            xml.Append("<add value=\"")
                                .Append(key.Value)
                                .Append("\" expiry=\"")
                                .Append(key.Expiry.ToString("yyyy-MM-dd HH:mm:ss"))
                                .Append("\"/>");
                        }

                        xml.Append("</keys></add>");
                    }
                    else
                        xml.Append("/>");
                }

                xml.Append("</providers></cryptography>");

                return xml.ToString();
            }
        }

        /// <summary>
        /// Used to initialize a default set of values for the <see cref="CryptographyConfiguration"/> object.
        /// </summary>
        protected override void InitializeDefault()
        {
            // ReSharper disable ConstantNullCoalescingCondition
            Providers = Providers ?? new ProviderCollection();
            // ReSharper restore ConstantNullCoalescingCondition
            base.InitializeDefault();
        }
    }
}