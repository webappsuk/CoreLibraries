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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Logging.Interfaces;

namespace WebApplications.Utilities.Logging.Loggers
{
    /// <summary>
    ///   A logger base class that implements log storage and retrieval.
    /// </summary>
    public abstract class LoggerBase : ILogger
    {
        private readonly bool _allowMultiple;

        [NotNull]
        private readonly string _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerBase" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="allowMultiple">if set to <see langword="true" /> the logger supports multiple instances.</param>
        /// <param name="validLevels">The valid levels.</param>
        protected LoggerBase(
            [NotNull] string name,
            bool allowMultiple = true,
            LoggingLevels validLevels = LoggingLevels.All)
        {
            if (name == null) throw new ArgumentNullException("name");

            _name = name;
            _allowMultiple = allowMultiple;
            ValidLevels = validLevels;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual void Dispose()
        {
        }

        /// <summary>
        /// A <see cref="bool" /> value indicating whether the logger supports multiple instances.
        /// </summary>
        /// <value>Returns <see langword="true" /> if the logger supports multiple instances; otherwise returns <see langword="false" />.</value>
        public bool AllowMultiple
        {
            get { return _allowMultiple; }
        }

        /// <summary>
        /// The valid <see cref="LoggingLevels">log levels</see> for this log level.
        /// </summary>
        /// <value>The valid levels.</value>
        public LoggingLevels ValidLevels { get; set; }

        /// <summary>
        /// Gets the logger name.
        /// </summary>
        /// <value>The logger name.</value>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Adds the specified logs to storage in batches.
        /// </summary>
        /// <param name="logs">The logs to add to storage.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task.</returns>
        public abstract Task Add(IEnumerable<Log> logs, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Gets all logs (if available).
        /// </summary>
        /// <value>The query.</value>
        public virtual IQueryable<Log> All
        {
            get { return null; }
        }

        /// <summary>
        /// Force a flush of this logger.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>Task.</returns>
        public virtual Task Flush(CancellationToken token = default(CancellationToken))
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            return TaskResult.Completed;
        }
    }
}