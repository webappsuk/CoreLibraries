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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Formatting;

namespace WebApplications.Utilities.Logging.Loggers
{
    /// <summary>
    /// Allows logging to a <see cref="TextWriter"/>.
    /// </summary>
    public class TextWriterLogger : LoggerBase
    {
        /// <summary>
        /// The text writer.
        /// </summary>
        [NotNull]
        private readonly TextWriter _writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceLogger" /> class.
        /// </summary>
        /// <param name="name">The logger name.</param>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format.</param>
        /// <param name="validLevels">The valid log levels.</param>
        public TextWriterLogger(
            [NotNull] string name,
            [NotNull] TextWriter writer,
            [CanBeNull] FormatBuilder format = null,
            LoggingLevels validLevels = LoggingLevels.All)
            : this(name, writer, format, true, validLevels)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextWriterLogger" /> class.
        /// </summary>
        /// <param name="name">The logger name.</param>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format.</param>
        /// <param name="allowMultiple">if set to <see langword="true" /> the logger supports multiple instances.</param>
        /// <param name="validLevels">The valid log levels.</param>
        protected TextWriterLogger(
            [NotNull] string name,
            [NotNull] TextWriter writer,
            [CanBeNull] FormatBuilder format,
            bool allowMultiple,
            LoggingLevels validLevels)
            : base(name, allowMultiple, validLevels)
        {
            if (writer == null) throw new ArgumentNullException("writer");
            Format = format;
            _writer = writer;
        }

        /// <summary>
        /// Gets or sets the format for trace messages.
        /// </summary>
        /// <value>The format.</value>
        [PublicAPI]
        [CanBeNull]
        public FormatBuilder Format { get; set; }

        /// <summary>
        /// Adds the specified logs to storage in batches.
        /// </summary>
        /// <param name="logs">The logs to add to storage.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task.</returns>
        public override Task Add(IEnumerable<Log> logs, CancellationToken token = default(CancellationToken))
        {
            if (logs == null) throw new ArgumentNullException("logs");

            FormatBuilder format = Format ?? Log.VerboseFormat;
            // ReSharper disable once PossibleNullReferenceException
            foreach (Log log in logs.Where(log => log.Level.IsValid(ValidLevels)))
            {
                token.ThrowIfCancellationRequested();
                log.WriteTo(_writer, format);
            }
            return TaskResult.Completed;
        }
    }
}