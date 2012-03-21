#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities.Logging 
// Project: WebApplications.Utilities.Logging
// File: ILogger.cs
// 
// This software, its object code and source code and all modifications made to
// the same (the “Software”) are, and shall at all times remain, the proprietary
// information and intellectual property rights of Web Applications (UK) Limited. 
// You are only entitled to use the Software as expressly permitted by Web
// Applications (UK) Limited within the Software Customisation and
// Licence Agreement (the “Agreement”).  Any copying, modification, decompiling,
// distribution, licensing, sale, transfer or other use of the Software other than
// as expressly permitted in the Agreement is expressly forbidden.  Web
// Applications (UK) Limited reserves its rights to take action against you and
// your employer in accordance with its contractual and common law rights
// (including injunctive relief) should you breach the terms of the Agreement or
// otherwise infringe its copyright or other intellectual property rights in the
// Software.
// 
// © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
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