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

#if false // TODO
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Logging.Loggers
{
    /// <summary>
    /// The <see cref="GlobalLogger"/> gives access to all logs, from all processes, on the current machine.
    /// </summary>
    [PublicAPI]
    public sealed class GlobalLogger : LoggerBase
    {
        /// <summary>
        /// The log file path.
        /// </summary>
        [CanBeNull]
        public static readonly string LogFilePath;

        /// <summary>
        /// The log file name.
        /// </summary>
        [NotNull]
        public const string LogFileName = @"WebApplications.Utilities.Logging.GlobalLog.log";

        static GlobalLogger()
        {
            try
            {
                // Get the file path to the shared log file, which should be in the common application data folder.
                LogFilePath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    LogFileName);

                if (!File.Exists(LogFilePath))
                {
                }
            }
            catch
            {
                LogFilePath = null;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalLogger" /> class.
        /// </summary>
        /// <param name="validLevels">The valid levels.</param>
        public GlobalLogger(LoggingLevels validLevels = LoggingLevels.All)
            :
                base("Global Logger", false, validLevels)
        {
        }

        /// <summary>
        /// Adds the specified logs to storage in batches.
        /// </summary>
        /// <param name="logs">The logs to add to storage.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override Task Add(IEnumerable<Log> logs, CancellationToken token = new CancellationToken())
        {
            if (logs == null) throw new ArgumentNullException("logs");
            throw new NotImplementedException();
        }
    }
}
#endif