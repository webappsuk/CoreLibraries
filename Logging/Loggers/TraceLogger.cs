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

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.SqlServer.Server;
using WebApplications.Utilities.Logging.Interfaces;

namespace WebApplications.Utilities.Logging.Loggers
{
    /// <summary>
    ///   Allows logging to the debug window.
    /// </summary>
    [UsedImplicitly]
    internal class TraceLogger : LoggerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TraceLogger" /> class.
        /// </summary>
        /// <param name="name">The logger name.</param>
        /// <param name="validLevels">The valid log levels.</param>
        /// <param name="format">The format.</param>
        public TraceLogger([NotNull] string name, LoggingLevels validLevels = LoggingLevels.All, string format = "V")
            :base(name, false, validLevels)
        {
            Format = format;
        }

        /// <summary>
        /// Adds the specified logs to storage in batches.
        /// </summary>
        /// <param name="logs">The logs to add to storage.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task.</returns>
        public override Task Add(IEnumerable<Log> logs, CancellationToken token)
        {
            foreach (Log log in logs.Where(log => log.Level.IsValid(ValidLevels)))
            {
                token.ThrowIfCancellationRequested();
                Trace.WriteLine(log.ToString(Format));
            }

            // We always complete synchronously.
            return Task.FromResult(true);
        }

        /// <summary>
        /// Gets or sets the format for trace messages.
        /// </summary>
        /// <value>The format.</value>
        public string Format { get; set; }
    }
}