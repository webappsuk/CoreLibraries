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
using System.Linq;
using System.Net;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace WebApplications.Utilities.BuildTasks
{
    /// <summary>
    /// Downloads a file from the internet to a specified location.
    /// </summary>
    public class DownloadTZDB : Task
    {
        /// <summary>
        /// Gets or sets the download URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        public string TZDBUri { get; set; }

        /// <summary>
        /// Gets or sets the output folder path.
        /// </summary>
        /// <value>The output file path.</value>
        /// <remarks></remarks>
        public string OutputFolderPath { get; set; }

        /// <summary>
        /// Gets or sets the output file name.
        /// </summary>
        /// <value>The output file path.</value>
        /// <remarks></remarks>
        public string OutputFileName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to overwite the TZDB if already present.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if overwrite; otherwise, <see langword="false" />.
        /// </value>
        public bool Overwrite { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use default credentials.
        /// </summary>
        /// <value>
        /// <see langword="true" /> to use default credentials; otherwise, <see langword="false" />.
        /// </value>
        public bool UseDefaultCredentials { get; set; }

        /// <summary>
        /// Gets or sets the username used to authenticate against the remote web server.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password used to authenticate against the remote web server. A value for <see cref="Username"/> must also be provided.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the domain of the user being used to authenticate against the remote web server. A value for <see cref="Username"/> must also be provided.
        /// </summary>
        /// <value>
        /// The domain.
        /// </value>
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

            string path = Path.GetFullPath(string.IsNullOrWhiteSpace(OutputFolderPath) ? "Resources" : OutputFolderPath);

            // Validate uri.
            Uri uri;
            if (string.IsNullOrWhiteSpace(TZDBUri))
                uri = new Uri("http://nodatime.org/tzdb/latest.txt");
            else if (!Uri.TryCreate(TZDBUri, UriKind.Absolute, out uri) ||
                     ReferenceEquals(uri, null))
            {
                Log.LogError("Invalid URL provided - '{0}'.", TZDBUri);
                return false;
            }

            if (string.IsNullOrWhiteSpace(path))
            {
                Log.LogError("The output directory was not specified");
                return false;
            }

            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                using (WebClient webClient = new WebClient())
                {
                    // Set credentials
                    if (UseDefaultCredentials)
                        webClient.Credentials = CredentialCache.DefaultCredentials;
                    else if (!string.IsNullOrWhiteSpace(Username))
                        webClient.Credentials = new NetworkCredential(Username, Password, Domain);

                    // First we go and download the Uri from the first Uri
                    Log.LogMessage("Downloading redirect URI from '{0}'.", uri);
                    string tzdb = webClient.DownloadString(uri);
                    string fileName = OutputFileName;
                    if (!Uri.TryCreate(tzdb, UriKind.Absolute, out uri) ||
                        ReferenceEquals(uri, null) ||
                        (string.IsNullOrWhiteSpace(fileName) &&
                         string.IsNullOrWhiteSpace(fileName = uri.Segments.LastOrDefault())))
                    {
                        Log.LogError("Invalid redirect URL provided - '{0}'.", tzdb);
                        return false;
                    }

                    // Create the download file path.
                    path = Path.Combine(path, fileName);
                    if (File.Exists(path))
                    {
                        if (!Overwrite)
                        {
                            Log.LogMessage(
                                MessageImportance.Normal,
                                string.Format(
                                    "The output file path '{0}' exists and we are not overwriting so skipping download from '{1}'.",
                                    path,
                                    uri));
                            return true;
                        }

                        // Delete the existing.
                        File.Delete(path);
                    }

                    Log.LogMessage("Downloading TZDB from '{0}' to '{1}'.", uri, path);
                    webClient.DownloadFile(uri, path);
                    Log.LogMessage("Successfully downloaded TZDB '{0}' from '{1}'.", path, uri);
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