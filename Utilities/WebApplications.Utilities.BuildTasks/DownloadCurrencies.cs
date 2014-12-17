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

using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Globalization;

namespace WebApplications.Utilities.BuildTasks
{
    /// <summary>
    /// Downloads the ISO 4217 Currencies and creates a binary representation.
    /// </summary>
    [UsedImplicitly]
    public class DownloadCurrencies : Task
    {
        /// <summary>
        /// Gets or sets the download URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        [UsedImplicitly]
        public string ISO4217Uri { get; set; }

        /// <summary>
        /// Gets or sets the output file name.
        /// </summary>
        /// <value>The output file path.</value>
        /// <remarks></remarks>
        [UsedImplicitly]
        public string OutputFilePath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to overwite the currency file if already present.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if overwrite; otherwise, <see langword="false" />.
        /// </value>
        [UsedImplicitly]
        public bool Overwrite { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to merge the contents when overwriting.
        /// </summary>
        /// <value>
        /// <see langword="true" /> to merge the contents; otherwise, <see langword="false" />.
        /// </value>
        [UsedImplicitly]
        public bool Merge { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use default credentials.
        /// </summary>
        /// <value>
        /// <see langword="true" /> to use default credentials; otherwise, <see langword="false" />.
        /// </value>
        [UsedImplicitly]
        public bool UseDefaultCredentials { get; set; }

        /// <summary>
        /// Gets or sets the username used to authenticate against the remote web server.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        [UsedImplicitly]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password used to authenticate against the remote web server. A value for <see cref="Username"/> must also be provided.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        [UsedImplicitly]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the domain of the user being used to authenticate against the remote web server. A value for <see cref="Username"/> must also be provided.
        /// </summary>
        /// <value>
        /// The domain.
        /// </value>
        [UsedImplicitly]
        public string Domain { get; set; }

        /// <summary>
        /// When overridden in a derived class, executes the task.
        /// </summary>
        /// <returns>
        /// true if the task successfully executed; otherwise, false.
        /// </returns>
        public override bool Execute()
        {
            Contract.Assert(Log != null);

            // Validate uri.
            Uri uri;
            if (!Uri.TryCreate(ISO4217Uri, UriKind.Absolute, out uri) ||
                ReferenceEquals(uri, null))
            {
                Log.LogError("Invalid URL provided - '{0}'.", ISO4217Uri);
                return false;

            }

            if (string.IsNullOrWhiteSpace(OutputFilePath))
            {
                Log.LogError("The output path was not specified");
                return false;
            }

            string path = Path.GetFullPath(OutputFilePath);
            string directoryName = Path.GetDirectoryName(path);

            if (string.IsNullOrWhiteSpace(directoryName))
            {
                Log.LogError("The output path was not specified");
                return false;
            }

            bool saveBinary = !string.Equals(Path.GetExtension(path), ".xml", StringComparison.InvariantCultureIgnoreCase);
            Log.LogMessage(saveBinary ? "Data will be saved in binary." : "Data will be saved in XML.");

            try
            {
                CurrencyInfoProvider existing = null;

                if (!Directory.Exists(directoryName))
                    Directory.CreateDirectory(path);
                else if (File.Exists(path))
                {
                    if (Merge)
                        existing = CurrencyInfoProvider.LoadFromFile(path);
                    else if (!Overwrite)
                    {
                        Log.LogMessage(
                            MessageImportance.Normal,
                            string.Format(
                                "The output file path '{0}' exists and we are not overwriting so skipping download from '{1}'.",
                                path,
                                uri));
                        return true;
                    }
                }

                using (WebClient webClient = new WebClient())
                {
                    if (UseDefaultCredentials)
                        webClient.Credentials = CredentialCache.DefaultCredentials;
                    else if (!string.IsNullOrWhiteSpace(Username))
                        webClient.Credentials = new NetworkCredential(Username, Password, Domain);

                    Log.LogMessage("Downloading XML from '{0}'.", uri);
                    string xml = webClient.DownloadString(uri);
                    Log.LogMessage("Successfully downloaded XML from '{0}'.", uri);

                    ICurrencyInfoProvider downloaded = CurrencyInfoProvider.LoadFromXml(xml);
                    if (downloaded == null)
                    {
                        Log.LogError("Could not parse the XML downloaded from '{0}'.", uri);
                        return false;
                    }

                    if (existing != null)
                    {
                        Log.LogMessage("Merging downloaded file with the existing file.");
                        downloaded = downloaded.Merge(existing);
                    }

                    if (saveBinary)
                    {
                        Log.LogMessage("Converting XML to binary and saving to '{0}'.", path);
                        using (Stream file = File.Create(path))
                            downloaded.ToBinary(file);
                        Log.LogMessage("Successfully converted XML and saved to '{0}'.", path);
                    }
                    else
                    {
                        Log.LogMessage("Saving XML to '{0}'.", path);
                        File.WriteAllText(path, downloaded.ToXml());
                        Log.LogMessage("Successfully saved XML to '{0}'.", path);
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Log.LogErrorFromException(e);
                return false;
            }
        }
    }
}