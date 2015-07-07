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
using System.Diagnostics;
using System.IO;
using System.Net;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.BuildTasks
{
    /// <summary>
    /// Downloads a file from the internet to a specified location.
    /// </summary>
    [UsedImplicitly]
    public class DownloadFile : Task
    {
        /// <summary>
        /// Gets or sets the download URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        [Required]
        [UsedImplicitly]
        public string Uri { get; set; }

        /// <summary>
        /// Gets or sets the output file path.
        /// </summary>
        /// <value>The output file path.</value>
        /// <remarks></remarks>
        [Required]
        [UsedImplicitly]
        public string OutputFilePath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to overwite the <see cref="OutputFilePath"/> if already present.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if overwrite; otherwise, <see langword="false" />.
        /// </value>
        [UsedImplicitly]
        public bool Overwrite { get; set; }

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
            Debug.Assert(Log != null);
            string path = Path.GetFullPath(OutputFilePath);

            // Validate uri.
            Uri uri;
            if (!System.Uri.TryCreate(Uri, UriKind.Absolute, out uri) ||
                ReferenceEquals(uri, null))
            {
                Log.LogError("Invalid URL provided - '{0}'.", Uri);
                return false;
            }

            string directoryName = Path.GetDirectoryName(path);
            if (string.IsNullOrWhiteSpace(directoryName) ||
                !Directory.Exists(directoryName))
            {
                Log.LogError("The output directory does not exist '{0}'.", directoryName);
                return false;
            }

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

            try
            {
                Log.LogMessage("Downloading file '{0}' from '{1}'.", path, uri);
                using (WebClient webClient = new WebClient())
                {
                    if (UseDefaultCredentials)
                        webClient.Credentials = CredentialCache.DefaultCredentials;
                    else if (!string.IsNullOrWhiteSpace(Username))
                        webClient.Credentials = new NetworkCredential(Username, Password, Domain);

                    webClient.DownloadFile(uri, path);
                }
                Log.LogMessage("Successfully downloaded file '{0}' from '{1}'.", path, uri);
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