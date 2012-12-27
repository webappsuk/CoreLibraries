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
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WebApplications.Utilities.Logging.Loggers;

namespace WebApplications.Utilities.Logging.Interfaces
{
    /// <summary>
    /// Provides an interface for loggers.
    /// </summary>
    /// <remarks>
    ///   For a head start use <see cref="LoggerBase"/>, which already implements a number of these methods.
    /// </remarks>
    public interface ILogger : IDisposable
    {
        /// <summary>
        /// A <see cref="bool" /> value indicating whether the logger supports multiple instances.
        /// </summary>
        /// <value>Returns <see langword="true" /> if the logger supports multiple instances; otherwise returns <see langword="false" />.</value>
        bool AllowMultiple { get; }

        /// <summary>
        ///   A <see cref="bool"/> value indicating whether the logger is queryable.
        /// </summary>
        /// <value>
        ///   Returns <see langword="true"/> if this instance can retrieve historic logs; otherwise returns <see langword="false"/>.
        /// </value>
        bool Queryable { get; }

        /// <summary>
        ///   The valid <see cref="LoggingLevels">log levels</see> for this log level.
        /// </summary>
        LoggingLevels ValidLevels { get; set; }

        /// <summary>
        ///   Gets the logger name.
        /// </summary>
        /// <value>The logger name.</value>
        [NotNull]
        string Name { get; }

        /// <summary>
        /// Adds the specified logs to storage in batches.
        /// </summary>
        /// <param name="logs">The logs to add to storage.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task.</returns>
        [NotNull]
        Task Add(IEnumerable<Log> logs, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Gets the Qbservable allowing asynchronous querying of log data.
        /// </summary>
        /// <value>The query.</value>
        [NotNull]
        IQbservable<IEnumerable<KeyValuePair<string, string>>> Qbserve { get; }
        
        /// <summary>
        /// Force a flush of this logger.
        /// </summary>
        [NotNull]
        Task Flush(CancellationToken token = default(CancellationToken));
    }
}