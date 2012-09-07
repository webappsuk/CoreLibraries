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
        ///   A <see cref="bool"/> value indicating whether the logger can retrieve historic logs.
        /// </summary>
        /// <value>
        ///   Returns <see langword="true"/> if this instance can retrieve historic logs; otherwise returns <see langword="false"/>.
        /// </value>
        bool CanRetrieve { get; }

        /// <summary>
        ///   The valid <see cref="LogLevels">log levels</see> for this log level.
        /// </summary>
        LogLevels ValidLevels { get; set; }

        /// <summary>
        ///   Gets the logger name.
        /// </summary>
        /// <value>The logger name.</value>
        [NotNull]
        string Name { get; }

        /// <summary>
        ///   Adds the specified log to storage in time order.
        /// </summary>
        /// <param name="log">The log to add to storage.</param>
        void Add(Log log);

        /// <summary>
        ///   Adds the specified logs to storage in time order.
        ///   This can be overridden in the default class to bulk store items in one call (e.g. to a database),
        ///   the inbuilt logger will always use this method where possible for efficiency.
        ///   By default it calls the standard Add method repeatedly.
        /// </summary>
        /// <param name="logs">The logs to add to storage.</param>
        void Add(IEnumerable<Log> logs);

        /// <summary>
        ///   Gets all of the logs from the end date backwards up to the specified limit.
        /// </summary>
        /// <param name="endDate">The last date to get logs up to (exclusive).</param>
        /// <param name="limit">The maximum number of logs to retrieve.</param>
        /// <returns>
        ///   The retrieved logs in reverse date order, the first being the newest log before the <paramref name="endDate"/>.
        /// </returns>
        [NotNull]
        IEnumerable<Log> Get(DateTime endDate, int limit);

        /// <summary>
        ///   Gets all of the logs from the end date backwards up to the start date.
        /// </summary>
        /// <param name="endDate">The last date to get logs from (exclusive).</param>
        /// <param name="startDate">The start date to get logs up to (inclusive).</param>
        /// <returns>
        ///   The retrieved logs in reverse date order, the first being the newest log before the <paramref name="endDate"/>
        ///   and the last being the first log after the <paramref name="startDate"/>.
        /// </returns>
        [NotNull]
        IEnumerable<Log> Get(DateTime endDate, DateTime startDate);

        /// <summary>
        ///   Gets all of the logs from the start date forwards up to the specified limit.
        /// </summary>
        /// <param name="startDate">The start date to get logs from (inclusive).</param>
        /// <param name="limit">The maximum number of logs to retrieve.</param>
        /// <returns>
        ///   The retrieved logs starting from the <paramref name="startDate"/> up to the specified <paramref name="limit"/>.
        /// </returns>
        [NotNull]
        IEnumerable<Log> GetForward(DateTime startDate, int limit);

        /// <summary>
        ///   Gets all of the logs from the start date forwards up to the end date.
        ///   If not overridden, and <see cref="ILogger.CanRetrieve">retrieval</see> is supported, then
        ///   it will reverse the results of the normal get method.
        /// </summary>
        /// <param name="startDate">The start date to get logs from (inclusive).</param>
        /// <param name="endDate">The last date to get logs to (exclusive).</param>
        /// <returns>
        ///   All of the retrieved logs from the <paramref name="startDate"/> to the <paramref name="endDate"/>.
        /// </returns>
        [NotNull]
        IEnumerable<Log> GetForward(DateTime startDate, DateTime endDate);

        /// <summary>
        ///   Flushes the logger.
        /// </summary>
        void Flush();
    }
}