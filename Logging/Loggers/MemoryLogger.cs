#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities.Logging 
// Project: WebApplications.Utilities.Logging
// File: MemoryLogger.cs
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
using System.Linq;
using JetBrains.Annotations;
using WebApplications.Utilities.Caching;

namespace WebApplications.Utilities.Logging.Loggers
{
    /// <summary>
    ///   A logger that stores logs solely in memory.
    ///   Is used by the core logger to hold log items temporarily, and for caching.
    /// </summary>
    [UsedImplicitly]
    public class MemoryLogger : LoggerBase
    {
        /// <summary>
        ///   The length of time log items are cached for.
        ///   <see cref="TimeSpan.Zero"/> is used for infinity.
        /// </summary>
        public readonly TimeSpan CacheExpiry;

        /// <summary>
        ///   The maximum number of log entries to store.
        /// </summary>
        public readonly int MaximumLogEntries;

        /// <summary>
        ///   The cache that stores the logs.
        /// </summary>
        private readonly CachingQueue<Log> _logs;

        /// <summary>
        ///   Initializes a new instance of the <see cref="MemoryLogger"/> class with the logs from an older logger.
        /// </summary>
        /// <param name="oldLogger">The old logger.</param>
        /// <param name="name">The logger name.</param>
        /// <param name="cacheExpiry">
        ///   <para>The length of time that log items are cached for.</para>
        ///   <para>Use <see cref="TimeSpan.Zero"/> for infinity.</para>
        ///   <para>By default the expiry will be set to 10 minutes.</para>
        /// </param>
        /// <param name="maximumLogEntries">
        ///   <para>The maximum number of log entries to store.</para>
        ///   <para>By default this is set to 10,000.</para>
        /// </param>
        /// <param name="validLevels">
        ///   <para>The valid log levels.</para>
        ///   <para>By default this is set allow all <see cref="LogLevels">all log levels</see>.</para>
        /// </param>
        /// <exception cref="LoggingException">
        ///   <para><paramref name="maximumLogEntries"/> was less than 1.</para>
        ///   <para>-or-</para>
        ///   <para><paramref name="cacheExpiry"/> cannot be less than 10 seconds.</para>
        /// </exception>
        internal MemoryLogger(
            [NotNull]MemoryLogger oldLogger,
            [NotNull] string name,
            TimeSpan cacheExpiry = default(TimeSpan),
            int maximumLogEntries = 10000,
            LogLevels validLevels = LogLevels.All)
            : base(name, true, validLevels)
        {
            if (maximumLogEntries <= 1)
                throw new LoggingException(
                    Resources.MemoryLogger_MaximumLogsLessThanOne, LogLevel.Critical,
                    maximumLogEntries);

            if (cacheExpiry == default(TimeSpan))
                cacheExpiry = TimeSpan.FromMinutes(10);
            else if (cacheExpiry < TimeSpan.FromSeconds(10))
                throw new LoggingException(
                    Resources.MemoryLogger_CacheExpiryLessThanTenSeconds, LogLevel.Critical,
                    cacheExpiry);

            MaximumLogEntries = maximumLogEntries;
            CacheExpiry = cacheExpiry;

            _logs = oldLogger._logs;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="MemoryLogger"/> class.
        /// </summary>
        /// <param name="name">The logger name.</param>
        /// <param name="cacheExpiry">
        ///   <para>The length of time that log items are cached for.</para>
        ///   <para>Use <see cref="TimeSpan.Zero"/> for infinity.</para>
        ///   <para>By default the expiry will be set to 10 minutes.</para>
        /// </param>
        /// <param name="maximumLogEntries">
        ///   <para>The maximum number of log entries to store.</para>
        ///   <para>By default this is set to 10,000.</para>
        /// </param>
        /// <param name="validLevels">
        ///   <para>The valid log levels.</para>
        ///   <para>By default this is set allow all <see cref="LogLevels">all log levels</see>.</para>
        /// </param>
        /// <exception cref="LoggingException">
        ///   <para><paramref name="maximumLogEntries"/> was less than 1.</para>
        ///   <para>-or-</para>
        ///   <para><paramref name="cacheExpiry"/> cannot be less than 10 seconds.</para>
        /// </exception>
        public MemoryLogger(
            [NotNull] string name,
            TimeSpan cacheExpiry = default(TimeSpan),
            int maximumLogEntries = 10000,
            LogLevels validLevels = LogLevels.All)
            : base(name, true, validLevels)
        {
            if (maximumLogEntries <= 1)
                throw new LoggingException(
                    Resources.MemoryLogger_MaximumLogsLessThanOne, LogLevel.Critical,
                    maximumLogEntries);

            if (cacheExpiry == default(TimeSpan))
                cacheExpiry = TimeSpan.FromMinutes(10);
            else if (cacheExpiry < TimeSpan.FromSeconds(10))
                throw new LoggingException(
                    Resources.MemoryLogger_CacheExpiryLessThanTenSeconds, LogLevel.Critical,
                    cacheExpiry);

            MaximumLogEntries = maximumLogEntries;
            CacheExpiry = cacheExpiry;

            _logs = new CachingQueue<Log>(cacheExpiry, maximumLogEntries);
        }

        /// <summary>
        ///   Gets the time which marks when the log is caching from.
        /// </summary>
        /// <value>The time the log is currently caching from.</value>
        public DateTime CachingFrom
        {
            get { return DateTime.Now.Subtract(CacheExpiry); }
        }

        /// <summary>
        ///   Adds the specified log to the cache.
        /// </summary>
        /// <param name="log">The log to add.</param>
        public override void Add(Log log)
        {
            _logs.Enqueue(log);
        }

        /// <summary>
        ///   Gets all of the logs from the end date backwards up to the specified limit.
        /// </summary>
        /// <param name="endDate">The last date to get logs up to (exclusive).</param>
        /// <param name="limit">The maximum number of logs to retrieve.</param>
        /// <returns>
        ///   The retrieved logs in reverse date order, the first being the newest log before the <paramref name="endDate"/>.
        /// </returns>
        public override IEnumerable<Log> Get(DateTime endDate, int limit)
        {
            DateTime e = CachingFrom;
            return (from l in _logs
                    where l.TimeStamp < endDate && l.TimeStamp > e
                    orderby l.TimeStamp descending
                    select l).Take(limit);
        }

        /// <summary>
        ///   Gets all of the logs from the end date backwards up to the start date.
        /// </summary>
        /// <param name="endDate">The last date to get logs from (exclusive).</param>
        /// <param name="startDate">The start date to get logs up to (inclusive).</param>
        /// <returns>
        ///   The retrieved logs in reverse date order, the first being the newest log before the <paramref name="endDate"/>
        ///   and the last being the first log after the <paramref name="startDate"/>.
        /// </returns>
        public override IEnumerable<Log> Get(DateTime endDate, DateTime startDate)
        {
            DateTime e = CachingFrom;
            return from l in _logs
                   where l.TimeStamp >= startDate && l.TimeStamp < endDate && l.TimeStamp > e
                   orderby l.TimeStamp descending
                   select l;
        }

        /// <summary>
        ///   Gets all of the logs from the start date forwards up to the specified limit.
        /// </summary>
        /// <param name="startDate">The start date to get logs from (inclusive).</param>
        /// <param name="limit">The maximum number of logs to retrieve.</param>
        /// <returns>
        ///   The retrieved logs starting from the <paramref name="startDate"/> up to the specified <paramref name="limit"/>.
        /// </returns>
        public override IEnumerable<Log> GetForward(DateTime startDate, int limit)
        {
            DateTime e = CachingFrom;

            // We ToList() before we exit otherwise the deferred execution object will be passed round instead.
            return (from l in _logs
                    where l.TimeStamp >= startDate && l.TimeStamp > e
                    select l).Take(limit).ToList();
        }

        /// <summary>
        ///   Gets all of the logs from the start date forwards up to the end date.
        ///   By default this calls the Get method and reverses, override in classes where reversal can be done when retrieving from storage.
        /// </summary>
        /// <param name="startDate">The start date to get logs from (inclusive).</param>
        /// <param name="endDate">The last date to get logs to (exclusive).</param>
        /// <returns>
        ///   All of the retrieved logs from the <paramref name="startDate"/> to the <paramref name="endDate"/>.
        /// </returns>
        public override IEnumerable<Log> GetForward(DateTime startDate, DateTime endDate)
        {
            DateTime e = CachingFrom;
            return from l in _logs
                   where l.TimeStamp >= startDate && l.TimeStamp < endDate && l.TimeStamp > e
                   orderby l.TimeStamp
                   select l;
        }
    }
}