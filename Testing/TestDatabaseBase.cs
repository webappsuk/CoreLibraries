#region © Copyright Web Applications (UK) Ltd, 2012.  All rights reserved.
// Copyright (c) 2012, Web Applications UK Ltd
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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace WebApplications.Testing
{
    /// <summary>
    /// Base class for unit tests on Databases.
    /// </summary>
    public abstract class TestDatabaseBase : TestBase
    {
        /// <summary>
        /// Static constructor of the <see cref="T:System.Object" /> class, used to initialize the locatoin of the data directory for all tests.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Thrown if the data directory cannot be found.</exception>
        /// <remarks></remarks>
        [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
        static TestDatabaseBase()
        {
            // Find the data directory
            string path = Path.GetDirectoryName(typeof (TestBase).Assembly.Location);
            string root = Path.GetPathRoot(path);
            string dataDirectory;
            do
            {
                // Look recursively for directory called Data containing mdf files.
                dataDirectory = Directory.GetDirectories(path, "Data", SearchOption.AllDirectories)
                    .SingleOrDefault(d => Directory.GetFiles(d, "*.mdf", SearchOption.TopDirectoryOnly).Any());

                // Move up a directory
                path = Path.GetDirectoryName(path);
            } while ((dataDirectory == null) &&
                     !String.IsNullOrWhiteSpace(path) &&
                     !path.Equals(root, StringComparison.CurrentCultureIgnoreCase));

            if (dataDirectory == null)
                throw new InvalidOperationException("Could not find the data directory.");

            // Set the DataDirectory data in the current AppDomain for use in connection strings.
            AppDomain.CurrentDomain.SetData("DataDirectory", dataDirectory);
        }

        /// <summary>
        /// Creates the connection string.
        /// </summary>
        /// <param name="databaseName">Name of the database.</param>
        /// <param name="isAsync">if set to <see langword="true"/> then set connection to asynchronous.</param>
        /// <returns></returns>
        protected static string CreateConnectionString(string databaseName, bool isAsync = false)
        {
            return
                String.Format(
                    @"Data Source=.\SQLEXPRESS;AttachDbFilename=|DataDirectory|\{0}.mdf;Integrated Security=True;Connect Timeout=30;User Instance=True;{1}",
                    databaseName,
                    isAsync ? "Asynchronous Processing=true" : String.Empty);
        }
    }
}