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
using System.IO.Compression;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.PowerShell
{
    /// <summary>
    /// Provide zip utilities.
    /// </summary>
    [UsedImplicitly]
    public static class Zip
    {
        /// <summary>
        /// Compresses the specified directory.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="zipFile">The zip file.</param>
        /// <param name="force">if set to <see langword="true"/> overwrites zip if present.</param>
        [UsedImplicitly]
        public static void Compress([NotNull] string directory, [NotNull] string zipFile, bool force)
        {
            if (!Directory.Exists(directory))
                throw new DirectoryNotFoundException(
                    String.Format(
                        Resources.Zip_Compress_DirectoryNotFound,
                        directory));

            if (File.Exists(zipFile))
            {
                if (!force)
                    throw new InvalidOperationException(
                        String.Format(
                            Resources.Zip_Compress_CannotOverwriteExistingFile,
                            zipFile));

                File.Delete(zipFile);
            }

            ZipFile.CreateFromDirectory(directory, zipFile);
        }

        /// <summary>
        /// Compresses the specified directory.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="zipFile">The zip file.</param>
        /// <param name="force">if set to <see langword="true"/> overwrites files in the directory if present.</param>
        [UsedImplicitly]
        public static void Decompress([NotNull] string zipFile, [NotNull] string directory, bool force)
        {
            if (!File.Exists(zipFile))
                throw new FileNotFoundException(string.Format(Resources.Zip_Decompress_FileNotFound, zipFile));

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            using (ZipArchive archive = ZipFile.Open(zipFile, ZipArchiveMode.Read))
            {
                Contract.Assert(archive != null);

                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    Contract.Assert(entry != null);

                    //Identifies the destination file name and path
                    string fileName = Path.Combine(directory, entry.FullName);
                    string fileDirectory = Path.GetDirectoryName(fileName);

                    if (!Directory.Exists(fileDirectory))
                        Directory.CreateDirectory(fileDirectory);

                    entry.ExtractToFile(fileName, force);
                }
            }
        }
    }
}