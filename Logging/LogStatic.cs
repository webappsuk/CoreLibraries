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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WebApplications.Utilities.Caching;
using WebApplications.Utilities.Configuration;
using WebApplications.Utilities.Logging.Configuration;
using WebApplications.Utilities.Logging.Interfaces;
using WebApplications.Utilities.Logging.Loggers;

namespace WebApplications.Utilities.Logging
{
    /// <summary>
    ///   Defines <see langword="static"/> members of the <see cref="Log"/> class.
    /// </summary>
    public sealed partial class Log
    {
        /// <summary>
        ///   The currently logged levels, which is all of the log levels that the registered loggers are
        ///   interested in ANDed with the current <see cref="ValidLevels"/>.
        /// </summary>
        [NonSerialized] private static LoggingLevels _loggedLevels = LoggingLevels.None;

        /// <summary>
        ///   Dictionary of all available loggers.
        /// </summary>
        [NonSerialized] [NotNull] private static readonly ConcurrentDictionary<string, ILogger> _loggers =
            new ConcurrentDictionary<string, ILogger>();

        /// <summary>
        ///   Holds a pointer to the default memory logger, used for caching.
        /// </summary>
        [UsedImplicitly] [NonSerialized] [CanBeNull] public static MemoryLogger DefaultMemoryLogger;

        /// <summary>
        ///   Used to signal the arrival of a new log item
        /// </summary>
        [NonSerialized] private static readonly ManualResetEventSlim _logSignal = new ManualResetEventSlim(false);

        /// <summary>
        ///   Used to signal when a batch has emptied the queue.
        /// </summary>
        [NonSerialized] private static readonly ManualResetEventSlim _batchComplete = new ManualResetEventSlim(true);

        /// <summary>
        ///   Used to signal when the logging thread should pause.
        /// </summary>
        [NonSerialized] private static readonly ManualResetEventSlim _continue = new ManualResetEventSlim(true);

        /// <summary>
        ///   Used to signal when the logging thread is pausing.
        /// </summary>
        [NonSerialized] private static readonly ManualResetEventSlim _pausing = new ManualResetEventSlim(false);

        /// <summary>
        ///   The <see cref="Thread"/> that handles the logging.
        /// </summary>
        [NonSerialized] [CanBeNull] private static Thread _loggerThread;

        /// <summary>
        ///   A queue of log items waiting to be dumped to loggers.
        /// </summary>
        [NonSerialized] 
        [NotNull]
        private static readonly ConcurrentQueue<Log> _logQueue = new ConcurrentQueue<Log>();


        /// <summary>
        ///   Used to synchronise methods.
        /// </summary>
        [NonSerialized] [NotNull] private static readonly object _lock = new object();

        /// <summary>
        ///   Initializes static members of the <see cref="Log" /> class.
        /// </summary>
        static Log()
        {
            ConfigurationSection<LoggingConfiguration>.Changed += (o, e) => LoadConfiguration();

            // Flush logs on domain unload.
            AppDomain.CurrentDomain.DomainUnload += (s, e) => Flush();
        }

        /// <summary>
        ///   Gets the valid <see cref="LoggingLevel">log levels</see>.
        /// </summary>
        [UsedImplicitly]
        public static LoggingLevels ValidLevels
        {
            get { return ConfigurationSection<LoggingConfiguration>.Active.ValidLevels; }
        }

        /// <summary>
        ///   Gets the loggers.
        /// </summary>
        [UsedImplicitly]
        [NotNull]
        public static IEnumerable<ILogger> Loggers
        {
            get { return _loggers.Values; }
        }

        /// <summary>
        ///   Loads the configuration whenever it is changed.
        /// </summary>
        /// <remarks>
        ///   As this uses AssemblyInitializeAttribute then this code is called whenever the Assembly
        ///   is referenced. In turn this activates the static constructor, which attaches this
        ///   method as the response to configuration changes. However, the initial configuration load does
        ///   not trigger that event, so it's only hit once on startup and then on every subsequent change.
        /// </remarks>
        internal static void LoadConfiguration()
        {
            lock (_lock)
            {
                // Reset the continue flag, which will cause logging thread to block
                _continue.Reset();

                // If we have a logging thread wait for it to pause.
                if (_loggerThread != null)
                {
                    _pausing.Wait();
                }

                // Get the active configuration
                LoggingConfiguration configuration = ConfigurationSection<LoggingConfiguration>.Active;

                // Remove loggers
                foreach (string loggerName in _loggers.Keys)
                {
                    if (loggerName == null) continue;
                    ILogger logger;
                    if ((_loggers.TryRemove(loggerName, out logger)) &&
                        (logger != DefaultMemoryLogger))
                        // Dispose loggers (except the default memory logger)
                        logger.Dispose();
                }

                // Create a memory logger for caching and add to loggers
                if ((configuration.Enabled) &&
                    (configuration.LogCacheExpiry > TimeSpan.Zero) &&
                    (configuration.LogCacheMaximumEntries > 0))
                {
                    if (DefaultMemoryLogger == null)
                    {
                        // No memory logger - create one.
                        DefaultMemoryLogger = new MemoryLogger(System.Guid.NewGuid().ToString(),
                                                               configuration.LogCacheExpiry);
                    }
                    else if ((DefaultMemoryLogger.CacheExpiry != configuration.LogCacheExpiry) ||
                             (DefaultMemoryLogger.MaximumLogEntries != configuration.LogCacheMaximumEntries))
                    {
                        // Changed the settings so create a new memory logger.
                        MemoryLogger oldLogger = DefaultMemoryLogger;
                        DefaultMemoryLogger = new MemoryLogger(oldLogger, System.Guid.NewGuid().ToString(),
                                                               configuration.LogCacheExpiry,
                                                               configuration.LogCacheMaximumEntries);
                        oldLogger.Dispose();
                    }
                    _loggers.AddOrUpdate(DefaultMemoryLogger.Name, DefaultMemoryLogger,
                                         (k, d) => DefaultMemoryLogger);
                }
                else
                {
                    if (DefaultMemoryLogger != null)
                        DefaultMemoryLogger.Dispose();

                    // Disable the memory logger.
                    DefaultMemoryLogger = null;
                }

                // If we're enabled add specified loggers that are also enabled.
                if (configuration.Enabled)
                {
                    foreach (LoggerElement loggerElement in configuration.Loggers.Where(l => l != null && l.Enabled))
                    {
                        try
                        {
                            AddOrUpdateLogger(loggerElement.GetInstance<ILogger>(), DefaultMemoryLogger);
                        }
                        catch (Exception exception)
                        {
                            new LoggingException(exception,
                                                 Resources.LogStatic_LoadConfiguration_ErrorCreatingLogger,
                                                 LoggingLevel.Critical,
                                                 loggerElement.Name,
                                                 exception.Message);
                        }
                    }

                    // If we don't have a logging thread create it.
                    if (_loggerThread == null)
                    {
                        _loggerThread = new Thread(LoggerThread) {Priority = ThreadPriority.BelowNormal, IsBackground = true};
                        _loggerThread.Start();
                    }
                }
                else
                {
                    // If we have a logging thread, abort it.
                    if (_loggerThread != null)
                    {
                        _loggerThread.Abort();
                        _loggerThread = null;
                    }
                }

                // Set the continue thread, any blocked logging thread will now continue with new configuration settings.
                _continue.Set();
            }
        }

        /// <summary>
        ///   Adds a new logger and copies over log items from the source logger.
        /// </summary>
        /// <param name="logger">The logger to add.</param>
        /// <param name="sourceLogger">
        ///   <para>The source logger to copy log items from.</para>
        ///   <para>Set to null if you don't want to copy the logs from the source.</para>
        /// </param>
        /// <param name="limit">
        ///   <para>The maximum number of logs to copy.</para>
        ///   <para>By default this is set to <see cref="int.MaxValue"/>.</para>
        /// </param>
        /// <returns>The logger with the specified logger name.</returns>
        /// <exception cref="LoggingException">
        ///   <see cref="WebApplications.Utilities.Logging.Loggers.LoggerBase.CanRetrieve">retrieval</see> is not supported.
        /// </exception>
        [UsedImplicitly]
        private static ILogger AddOrUpdateLogger(
            [NotNull] ILogger logger,
            [CanBeNull] ILogger sourceLogger = null,
            int limit = Int32.MaxValue)
        {
            if (sourceLogger != null)
            {
                // Check for source logger can retrieve logs
                if (!sourceLogger.CanRetrieve)
                {
                    throw new LoggingException(
                        Resources.LogStatic_AddOrUpdateLogger_RetrievalNotSupported,
                        LoggingLevel.Error,
                        logger.Name);
                }

                // Add logs
                logger.Add(sourceLogger.Get(DateTime.Now, limit).Reverse());
            }

            _loggedLevels = _loggedLevels | logger.ValidLevels & ValidLevels;
            return _loggers.AddOrUpdate(logger.Name, logger, (k, l) => logger);
        }

        /// <summary>
        ///   Tries to retrieve the logger with the name specified.
        /// </summary>
        /// <param name="loggerName">The name of the logger to retrieve.</param>
        /// <param name="logger">The retrieved logger.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the logger was retrieved successfully; otherwise returns <see langword="false"/>.
        /// </returns>
        [UsedImplicitly]
        public static bool TryGetLogger([NotNull] string loggerName, out ILogger logger)
        {
            return _loggers.TryGetValue(loggerName, out logger);
        }

        /// <summary>
        ///   Tries to remove a logger with the name specified.
        /// </summary>
        /// <param name="name">The name of the logger to remove.</param>
        /// <param name="logger">The logger removed.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the logger was removed successfully; otherwise returns <see langword="false"/>.
        /// </returns>
        [UsedImplicitly]
        private static bool TryRemoveLogger([NotNull] string name, out ILogger logger)
        {
            if (_loggers.TryRemove(name, out logger))
            {
                RecalculateLoggedLevels();
                return true;
            }
            return false;
        }

        /// <summary>
        ///   Recalculates the logged log levels if a logger is removed.
        /// </summary>
        internal static void RecalculateLoggedLevels()
        {
            _loggedLevels = _loggers.Aggregate(LoggingLevels.None, (c, n) => c | n.Value.ValidLevels) & ValidLevels;
        }

        /// <summary>
        ///   Gets all of the logs from the end date backwards up to the specified limit.
        /// </summary>
        /// <param name="endDate">The last date to get logs up to (exclusive).</param>
        /// <param name="limit">The maximum number of logs to retrieve.</param>
        /// <returns>
        ///   The retrieved logs in reverse date order, the first being the newest log before the <paramref name="endDate"/>.
        /// </returns>
        /// <exception cref="LoggingException">
        ///   <para>There is no log cache available.</para>
        ///   <para>-or-</para>
        ///   <para><paramref name="limit"/> is less than 1.</para>
        /// </exception>
        [UsedImplicitly]
        [NotNull]
        public static IEnumerable<Log> Get(DateTime endDate, int limit)
        {
            if (DefaultMemoryLogger == null)
                throw new LoggingException(Resources.LogStatic_Get_NoLogCacheAvailable, LoggingLevel.Critical);
            return Get(DefaultMemoryLogger, endDate, limit);
        }

        /// <summary>
        ///   Gets all of the logs from the end date backwards up to the specified limit.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="endDate">The last date to get logs up to. (exclusive)</param>
        /// <param name="limit">The maximum number of logs to retrieve.</param>
        /// <returns>
        ///   The retrieved logs in reverse date order, the first being the newest log before the <paramref name="endDate"/>.
        /// </returns>
        /// <exception cref="LoggingException">
        ///   <para><paramref name="logger"/> doesn't support <see cref="WebApplications.Utilities.Logging.Loggers.LoggerBase.CanRetrieve">retrieval</see>.</para>
        ///   <para>-or-</para>
        ///   <para><paramref name="limit"/> is less than 1.</para>
        /// </exception>
        [UsedImplicitly]
        [NotNull]
        public static IEnumerable<Log> Get([NotNull] LoggerBase logger, DateTime endDate, int limit)
        {
            if (!logger.CanRetrieve)
                throw new LoggingException(
                    Resources.LogStatic_Get_RetrievalNotSupported, LoggingLevel.Critical, logger.Name);

            if (limit <= 0)
                throw new LoggingException(Resources.LogStatic_Get_LimitLessThanZero, LoggingLevel.Error);

            // First get results from cache
            IEnumerable<Log> results = DefaultMemoryLogger == null
                                           ? Enumerable.Empty<Log>()
                                           : DefaultMemoryLogger.Get(endDate, limit);

            // If the logger is the memory logger, or we have enough results return
            if ((logger == DefaultMemoryLogger) ||
                (results.Count() >= limit))
                return results;

            return logger.Get(endDate, limit);
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
        /// <exception cref="LoggingException">
        ///   <para>There is no log cache available.</para>
        ///   <para>-or-</para>
        ///   <para>The logger doesn't support <see cref="WebApplications.Utilities.Logging.Loggers.LoggerBase.CanRetrieve">retrieval</see>.</para>
        ///   <para>-or-</para>
        ///   <para><paramref name="startDate"/> is is greater than the <see cref="System.DateTime.Now">current date</see>.</para>
        ///   <para>-or-</para>
        ///   <para><paramref name="startDate"/> is greater than <paramref name="endDate"/>.</para>
        /// </exception>
        [UsedImplicitly]
        [NotNull]
        public static IEnumerable<Log> Get(DateTime endDate, DateTime startDate)
        {
            if (DefaultMemoryLogger == null)
                throw new LoggingException(Resources.LogStatic_Get_NoLogCacheAvailable, LoggingLevel.Critical);
            return Get(DefaultMemoryLogger, endDate, startDate);
        }

        /// <summary>
        ///   Gets all logs from the end date backwards to start date
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="endDate">The last date to get logs from (exclusive).</param>
        /// <param name="startDate">The start date to get logs up to (inclusive).</param>
        /// <returns>
        ///   The retrieved logs in reverse date order, the first being the newest log before the <paramref name="endDate"/>
        ///   and the last being the first log after the <paramref name="startDate"/>.
        /// </returns>
        /// <exception cref="LoggingException">
        ///   <para>The <paramref name="logger"/> doesn't  support <see cref="WebApplications.Utilities.Logging.Loggers.LoggerBase.CanRetrieve">retrieval</see>.</para>
        ///   <para>-or-</para>
        ///   <para><paramref name="startDate"/> is greater than the <see cref="System.DateTime.Now">current date</see>.</para>
        ///   <para>-or-</para>
        ///   <para><paramref name="startDate"/> is greater than <paramref name="endDate"/>.</para>
        /// </exception>
        [UsedImplicitly]
        [NotNull]
        public static IEnumerable<Log> Get([NotNull] LoggerBase logger, DateTime endDate, DateTime startDate)
        {
            if (!logger.CanRetrieve)
                throw new LoggingException(
                    Resources.LogStatic_Get_RetrievalNotSupported, LoggingLevel.Critical, logger.Name);

            if (startDate >= DateTime.Now)
                throw new LoggingException(
                    Resources.LogStatic_Get_StartGreaterThanOrEqualToCurrent, LoggingLevel.Error);

            if (startDate >= endDate)
                throw new LoggingException(
                    Resources.LogStatic_Get_StartGreaterThanOrEqualToEnd, LoggingLevel.Error);

            // If we can get the logs from cache, do so.
            if ((DefaultMemoryLogger != null) && (startDate >= DefaultMemoryLogger.CachingFrom))
                logger = DefaultMemoryLogger;

            return logger.Get(endDate, startDate);
        }

        /// <summary>
        ///   Gets all of the logs from the start date forwards up to the specified limit.
        /// </summary>
        /// <param name="startDate">The start date to get logs from (inclusive).</param>
        /// <param name="limit">The maximum number of logs to retrieve.</param>
        /// <returns>
        ///   The retrieved logs starting from the <paramref name="startDate"/> up to the specified <paramref name="limit"/>.
        /// </returns>
        /// <exception cref="LoggingException">
        ///   <para>There is no log cache available.</para>
        ///   <para>-or-</para>
        ///   <para>The logger doesn't  support <see cref="WebApplications.Utilities.Logging.Loggers.LoggerBase.CanRetrieve">retrieval</see>.</para>
        ///   <para>-or-</para>
        ///   <para><paramref name="limit"/> is less than 1.</para>
        ///   <para>-or-</para>
        ///   <para><paramref name="startDate"/> is greater than the <see cref="System.DateTime.Now">current date</see>.</para>
        /// </exception>
        [UsedImplicitly]
        [NotNull]
        public static IEnumerable<Log> GetForward(DateTime startDate, int limit)
        {
            if (DefaultMemoryLogger == null)
                throw new LoggingException(Resources.LogStatic_Get_NoLogCacheAvailable, LoggingLevel.Critical);
            return GetForward(DefaultMemoryLogger, startDate, limit);
        }

        /// <summary>
        ///   Gets all of the logs from the start date forwards up to the specified limit.
        /// </summary>
        /// <param name = "logger">The logger to use.</param>
        /// <param name="startDate">The start date to get logs from (inclusive).</param>
        /// <param name="limit">The maximum number of logs to retrieve.</param>
        /// <returns>
        ///   The retrieved logs starting from the <paramref name="startDate"/> up to the specified <paramref name="limit"/>.
        /// </returns>
        /// <exception cref="LoggingException">
        ///   <para>The <paramref name="logger"/> doesn't implement Get.</para>
        ///   <para>-or-</para>
        ///   <para>The <paramref name="logger"/> doesn't  support <see cref="WebApplications.Utilities.Logging.Loggers.LoggerBase.CanRetrieve">retrieval</see>.</para>
        ///   <para>-or-</para>
        ///   <para><paramref name="limit"/> is less than 1.</para>
        ///   <para>-or-</para>
        ///   <para><paramref name="startDate"/> is greater than the <see cref="System.DateTime.Now">current date</see>.</para>
        /// </exception>
        [UsedImplicitly]
        [NotNull]
        public static IEnumerable<Log> GetForward([NotNull] LoggerBase logger, DateTime startDate, int limit)
        {
            if (!logger.CanRetrieve)
                throw new LoggingException(
                    Resources.LogStatic_Get_RetrievalNotSupported, LoggingLevel.Critical, logger.Name);

            if (limit <= 0)
                throw new LoggingException(Resources.LogStatic_Get_LimitLessThanZero, LoggingLevel.Error);

            if (startDate >= DateTime.Now)
                throw new LoggingException(
                    Resources.LogStatic_Get_StartGreaterThanOrEqualToCurrent, LoggingLevel.Error);

            // If we can get the logs from cache, do so.
            if ((DefaultMemoryLogger != null) && (startDate >= DefaultMemoryLogger.CachingFrom))
                logger = DefaultMemoryLogger;

            return logger.GetForward(startDate, limit);
        }

        /// <summary>
        ///   Gets all of the logs from the start date forwards up to the end date.
        ///   This calls the Get method and reverses.
        /// </summary>
        /// <param name="startDate">The start date to get logs from (inclusive).</param>
        /// <param name="endDate">The last date to get logs to (exclusive).</param>
        /// <returns>
        ///   All of the retrieved logs from the <paramref name="startDate"/> to the <paramref name="endDate"/>.
        /// </returns>
        /// <exception cref="LoggingException">
        ///   <para>The logger doesn't  support <see cref="WebApplications.Utilities.Logging.Loggers.LoggerBase.CanRetrieve">retrieval</see>.</para>
        ///   <para>-or-</para>
        ///   <para><paramref name="startDate"/> is greater than the <see cref="System.DateTime.Now">current date</see>.</para>
        ///   <para>-or-</para>
        ///   <para><paramref name="startDate"/> is greater than the <paramref name="endDate"/>.</para>
        /// </exception>
        [UsedImplicitly]
        [NotNull]
        public static IEnumerable<Log> GetForward(DateTime startDate, DateTime endDate)
        {
            if (DefaultMemoryLogger == null)
                throw new LoggingException(Resources.LogStatic_Get_NoLogCacheAvailable, LoggingLevel.Critical);
            return GetForward(DefaultMemoryLogger, startDate, endDate);
        }

        /// <summary>
        ///   Gets all of the logs from the start date forwards up to the end date.
        ///   This calls the Get method and reverses.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="startDate">The start date to get logs from (inclusive).</param>
        /// <param name="endDate">The last date to get logs to (exclusive).</param>
        /// <returns>
        ///   All of the retrieved logs from the <paramref name="startDate"/> to the <paramref name="endDate"/>.
        /// </returns>
        /// <exception cref="LoggingException">
        ///   <para>The <paramref name="logger"/> doesn't implement Get.</para>
        ///   <para>-or-</para>
        ///   <para>The <paramref name="logger"/> doesn't  support <see cref="WebApplications.Utilities.Logging.Loggers.LoggerBase.CanRetrieve">retrieval</see>.</para>
        ///   <para>-or-</para>
        ///   <para><paramref name="startDate"/> is greater than the <see cref="System.DateTime.Now">current date</see>.</para>
        ///   <para>-or-</para>
        ///   <para><paramref name="startDate"/> is greater than the <paramref name="endDate"/>.</para>
        /// </exception>
        [UsedImplicitly]
        [NotNull]
        public static IEnumerable<Log> GetForward([NotNull] LoggerBase logger, DateTime startDate, DateTime endDate)
        {
            if (!logger.CanRetrieve)
                throw new LoggingException(
                    Resources.LogStatic_Get_RetrievalNotSupported, LoggingLevel.Critical, logger.Name);

            if (startDate >= DateTime.Now)
                throw new LoggingException(
                    Resources.LogStatic_Get_StartGreaterThanOrEqualToCurrent, LoggingLevel.Error);

            if (startDate >= endDate)
                throw new LoggingException(
                    Resources.LogStatic_Get_StartGreaterThanOrEqualToEnd, LoggingLevel.Error);

            // If we can get the logs from cache, do so.
            if ((DefaultMemoryLogger != null) && (startDate >= DefaultMemoryLogger.CachingFrom))
                logger = DefaultMemoryLogger;

            return logger.GetForward(startDate, endDate);
        }

        /// <summary>
        ///   Flushes all outstanding logs.
        /// </summary>
        /// <remarks>
        ///   Note: If more logs are added whilst the system is flushing this will continue to block until there are no outstanding logs.
        /// </remarks>
        [UsedImplicitly]
        public static void Flush()
        {
            lock (_lock)
            {
                if ((_loggerThread != null) && (_loggerThread.IsAlive))
                {
                    // Firstly we reset the batch complete flag
                    // this ensures that we will block until we next clear down the logs completely.
                    _batchComplete.Reset();

                    // Set the log signal, to ensure we release the batch thread (if it is blocked).
                    _logSignal.Set();

                    // Block until we receive a batch complete signal.
                    _batchComplete.Wait();
                }

                // Flush loggers themselves.
                foreach (ILogger logger in _loggers.Values)
                    logger.Flush();
            }
        }

        /// <summary>
        ///   Thread entry point for logging items to loggers.
        /// </summary>
        private static void LoggerThread()
        {
            try
            {
                // Loop infinitely, logging
                do
                {
                    // Signal we are pausing
                    _pausing.Set();

                    // Wait if the pause flag is set.
                    _continue.Wait();

                    // Signal pause has completed.
                    _pausing.Reset();

                    // Check for an empty queue
                    if (_logQueue.IsEmpty)
                        // Block for 30s, or until we get a new log item
                        // We don't block permanently as there is a rare
                        // race condition where log items may still be in
                        // the queue, and we can't lock the wait!
                        _logSignal.Wait(30000);

                    // Reset the log signal
                    _logSignal.Reset();

                    // Once signalled we wait 2s, in many cases this allows
                    // our queue to build up to a nice size before storing
                    // It does add the risk that the last 2s of logs can
                    // be permanently lost, but the performance benefit
                    // far exceeds the disadvantage.
                    Thread.Sleep((int) ConfigurationSection<LoggingConfiguration>.Active.BatchWait.TotalMilliseconds);

                    // Always dump the first batch of logs here.
                    do
                    {
                        LogBatch();

                        // Do more batches if we have more than the minimum batch size left.
                    } while (_logQueue.Count >= ConfigurationSection<LoggingConfiguration>.Active.MinBatchSize);


                    // Set the batch complete flag if we have an empty queue.
                    if (_logQueue.Count < 1)
                        _batchComplete.Set();
                } while (true);
            }
            catch (ThreadAbortException)
            {
                Add(Resources.LogStatic_LoggerThread_LoggingThreadWasAborted);
            }
            catch (Exception e)
            {
                // Log exception, don't throw as can't catch anyway.
                new LoggingException(e, Resources.LogStatic_LoggerThread_FatalErrorWhilstLogging,
                                     LoggingLevel.Critical);
            }
            finally
            {
                // Count logs
                int remaining = _logQueue.Count;

                // Get rid of the remaining logs in batches.
                // Note it is possible for extra logs that are added to be stored - but this is not guaranteed
                // as it will ultimately stop trying when the count goes below zero.  This stops infinite log loops
                // from preventing clean termination.
                while (remaining > 0)
                    remaining -= LogBatch();

                // Explicitly dispose loggers.
                foreach (KeyValuePair<string, ILogger> kvp in _loggers)
                {
                    kvp.Value.Dispose();
                    ILogger l;
                    _loggers.TryRemove(kvp.Key, out l);
                }
                _loggerThread = null;
            }
        }

        /// <summary>
        ///   Dumps a batch of logs.
        /// </summary>
        private static int LogBatch()
        {
            int bsize = 0;
            List<Log> batch = new List<Log>(bsize);

            try
            {
                // Prevent thread abortion during log write outs.
                Thread.BeginCriticalRegion();

                // Build up batch from queue
                Log log;
                while ((_logQueue.TryDequeue(out log)) &&
                       (bsize++ < ConfigurationSection<LoggingConfiguration>.Active.MaxBatchSize))
                    batch.Add(log);

                // Run each logger in parallel.
                Parallel.ForEach(
                    _loggers.Values.Where(l => l != null && l.ValidLevels != LoggingLevels.None),
                    l =>
                        {
                            try
                            {
                                LoggingLevels validLoggingLevels = ValidLevels & l.ValidLevels;
                                // Only add log items that are valid for this logger.
                                IEnumerable<Log> loggerLogs = validLoggingLevels != LoggingLevels.All
                                                                  ? batch.Where(
                                                                      logItem =>
                                                                      logItem != null &&
                                                                      logItem.Level.IsValid(validLoggingLevels))
                                                                  : batch;

                                if (loggerLogs.Any())
                                    l.Add(loggerLogs);
                            }
                            catch (Exception e)
                            {
                                // Disable this logger as it threw an exception
                                new LoggingException(
                                    e,
                                    Resources.LogStatic_LogBatch_FatalErrorOccured,
                                    LoggingLevel.Critical,
                                    l.Name,
                                    e.Message);
                                TryRemoveLogger(l.Name, out l);
                            }
                        });
            }
            finally
            {
                Thread.EndCriticalRegion();
            }
            return bsize;
        }

        #region Add Overloads
        /// <summary>
        ///   Logs a message at the information <see cref="LoggingLevel">log level</see>.
        /// </summary>
        /// <param name="message">The log message.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        /// <remarks>
        ///   If the information <see cref="LoggingLevel">log level</see> is invalid then the log won't be added.
        /// </remarks>
        [StringFormatMethod("message")]
        [UsedImplicitly]
        public static void Add([NotNull] string message, [NotNull] params object[] parameters)
        {
            // Add to queue for logging if we are a valid level.
            if (LoggingLevel.Information.IsValid(_loggedLevels))
                new Log(System.Guid.Empty, null, null, LoggingLevel.Information, message, parameters);
        }

        /// <summary>
        ///   Logs a message at the information <see cref="LoggingLevel">log level</see>.
        /// </summary>
        /// <param name="context">The log context.</param>
        /// <param name="message">The log message.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        /// <remarks>
        ///   If the information <see cref="LoggingLevel">log level</see> is invalid then the log won't be added.
        /// </remarks>
        [StringFormatMethod("message")]
        [UsedImplicitly]
        public static void Add([NotNull] LogContext context, [NotNull] string message,
                               [NotNull] params object[] parameters)
        {
            // Add to queue for logging if we are a valid level.
            if (LoggingLevel.Information.IsValid(_loggedLevels))
                new Log(System.Guid.Empty, context, null, LoggingLevel.Information, message, parameters);
        }

        /// <summary>
        ///   Logs a message at the information <see cref="LoggingLevel">log level</see>.
        /// </summary>
        /// <param name="logGroup">The unique ID that groups log items together.</param>
        /// <param name="message">The log message.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        /// <remarks>
        ///   If the information <see cref="LoggingLevel">log level</see> is invalid then the log won't be added.
        /// </remarks>
        [StringFormatMethod("message")]
        [UsedImplicitly]
        public static void Add(Guid logGroup, [NotNull] string message, [NotNull] params object[] parameters)
        {
            // Add to queue for logging if we are a valid level.
            if (LoggingLevel.Information.IsValid(_loggedLevels))
                new Log(logGroup, null, null, LoggingLevel.Information, message, parameters);
        }

        /// <summary>
        ///   Logs a message at the information <see cref="LoggingLevel">log level</see>.
        /// </summary>
        /// <param name="logGroup">The unique ID that groups log items together.</param>
        /// <param name="context">The log context.</param>
        /// <param name="message">The log message.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        /// <remarks>
        ///   If the information <see cref="LoggingLevel">log level</see> is invalid then the log won't be added.
        /// </remarks>
        [StringFormatMethod("message")]
        [UsedImplicitly]
        public static void Add(Guid logGroup, [NotNull] LogContext context, [NotNull] string message,
                               [NotNull] params object[] parameters)
        {
            // Add to queue for logging if we are a valid level.
            if (LoggingLevel.Information.IsValid(_loggedLevels))
                new Log(logGroup, context, null, LoggingLevel.Information, message, parameters);
        }

        /// <summary>
        ///   Logs a message at the specified <see cref="LoggingLevel"/>.
        /// </summary>
        /// <param name="message">The log message.</param>
        /// <param name="level">The log level.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        /// <remarks>
        ///   If the log <paramref name="level"/> is invalid then the log won't be added.
        /// </remarks>
        [StringFormatMethod("message")]
        [UsedImplicitly]
        public static void Add(LoggingLevel level, [NotNull] string message, [NotNull] params object[] parameters)
        {
            // Add to queue for logging if we are a valid level.
            if (level.IsValid(_loggedLevels))
                new Log(System.Guid.Empty, null, null, level, message, parameters);
        }

        /// <summary>
        /// Logs a message at the specified <see cref="LoggingLevel" />.
        /// </summary>
        /// <param name="context">The log context.</param>
        /// <param name="level">The log level.</param>
        /// <param name="message">The log message.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        /// <remarks>If the log <paramref name="level" /> is invalid then the log won't be added.</remarks>
        [StringFormatMethod("message")]
        [UsedImplicitly]
        public static void Add([NotNull] LogContext context, LoggingLevel level, [NotNull] string message,
                               [NotNull] params object[] parameters)
        {
            // Add to queue for logging if we are a valid level.
            if (level.IsValid(_loggedLevels))
                new Log(System.Guid.Empty, context, null, level, message, parameters);
        }

        /// <summary>
        /// Logs a message at the specified <see cref="LoggingLevel" />.
        /// </summary>
        /// <param name="logGroup">The unique ID that groups log items together.</param>
        /// <param name="level">The log level.</param>
        /// <param name="message">The log message.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        /// <remarks>If the log <paramref name="level" /> is invalid then the log won't be added.</remarks>
        [StringFormatMethod("message")]
        [UsedImplicitly]
        public static void Add(Guid logGroup, LoggingLevel level, [NotNull] string message,
                               [NotNull] params object[] parameters)
        {
            // Add to queue for logging if we are a valid level.
            if (level.IsValid(_loggedLevels))
                new Log(logGroup, null, null, level, message, parameters);
        }

        /// <summary>
        /// Logs a message at the specified <see cref="LoggingLevel" />.
        /// </summary>
        /// <param name="logGroup">The unique ID that groups log items together.</param>
        /// <param name="context">The log context.</param>
        /// <param name="level">The log level.</param>
        /// <param name="message">The log message.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        /// <remarks>If the log <paramref name="level" /> is invalid then the log won't be added.</remarks>
        [StringFormatMethod("message")]
        [UsedImplicitly]
        public static void Add(Guid logGroup, [NotNull] LogContext context, LoggingLevel level, [NotNull] string message,
                               [NotNull] params object[] parameters)
        {
            // Add to queue for logging if we are a valid level.
            if (level.IsValid(_loggedLevels))
                new Log(logGroup, context, null, level, message, parameters);
        }

        /// <summary>
        ///   Logs an exception.
        /// </summary>
        /// <param name="exception">
        ///   <para>The exception to log.</para>
        ///   <para><see cref="LoggingException"/>'s add themselves and so this method ignores them.</para>
        /// </param>
        /// <param name="level">
        ///   <para>The log level.</para>
        ///   <para>By default this uses the error log level.</para>
        /// </param>
        /// <remarks>
        ///   If the log <paramref name="level"/> is invalid then the log won't be added.
        /// </remarks>
        [UsedImplicitly]
        public static void Add([NotNull] Exception exception, LoggingLevel level = LoggingLevel.Error)
        {
            // Don't re-add logging exception, as they add themselves.
            if (exception is LoggingException)
                return;

            // Add to queue for logging if we are a valid level.
            if (level.IsValid(_loggedLevels))
                new Log(System.Guid.Empty, null, exception, level, exception.Message);
        }

        /// <summary>
        ///   Logs an exception.
        /// </summary>
        /// <param name="context">The log context.</param>
        /// <param name="exception">
        ///   <para>The exception to log.</para>
        ///   <para><see cref="LoggingException"/>'s add themselves and so this method ignores them.</para>
        /// </param>
        /// <param name="level">
        ///   <para>The log level.</para>
        ///   <para>By default this uses the error log level.</para>
        /// </param>
        /// <remarks>
        ///   If the log <paramref name="level"/> is invalid then the log won't be added.
        /// </remarks>
        [UsedImplicitly]
        public static void Add([NotNull] LogContext context, [NotNull] Exception exception,
                               LoggingLevel level = LoggingLevel.Error)
        {
            // Don't re-add logging exception, as they add themselves.
            if (exception is LoggingException)
                return;

            // Add to queue for logging if we are a valid level.
            if (level.IsValid(_loggedLevels))
                new Log(System.Guid.Empty, context, exception, level, exception.Message);
        }

        /// <summary>
        ///   Logs an exception.
        /// </summary>
        /// <param name="logGroup">The unique ID that groups log items together.</param>
        /// <param name="exception">
        ///   <para>The exception to log.</para>
        ///   <para><see cref="LoggingException"/>'s add themselves and so this method ignores them.</para>
        /// </param>
        /// <param name="level">
        ///   <para>The log level.</para>
        ///   <para>By default this uses the error log level.</para>
        /// </param>
        /// <remarks>
        ///   If the log <paramref name="level"/> is invalid then the log won't be added.
        /// </remarks>
        [UsedImplicitly]
        public static void Add(Guid logGroup, [NotNull] Exception exception, LoggingLevel level = LoggingLevel.Error)
        {
            // Don't re-add logging exception, as they add themselves.
            if (exception is LoggingException)
                return;

            // Add to queue for logging if we are a valid level.
            if (level.IsValid(_loggedLevels))
                new Log(logGroup, null, exception, level, exception.Message);
        }

        /// <summary>
        ///   Logs an exception.
        /// </summary>
        /// <param name="logGroup">The unique ID that groups log items together.</param>
        /// <param name="context">The log context.</param>
        /// <param name="exception">
        ///   <para>The exception to log.</para>
        ///   <para><see cref="LoggingException"/>'s add themselves and so this method ignores them.</para>
        /// </param>
        /// <param name="level">
        ///   <para>The log level.</para>
        ///   <para>By default this uses the error log level.</para>
        /// </param>
        /// <remarks>
        ///   If the log <paramref name="level"/> is invalid then the log won't be added.
        /// </remarks>
        [UsedImplicitly]
        public static void Add(Guid logGroup, [NotNull] LogContext context, [NotNull] Exception exception,
                               LoggingLevel level = LoggingLevel.Error)
        {
            // Don't re-add logging exception, as they add themselves.
            if (exception is LoggingException)
                return;

            // Add to queue for logging if we are a valid level.
            if (level.IsValid(_loggedLevels))
                new Log(logGroup, context, exception, level, exception.Message);
        }


        /// <summary>
        /// Logs a logging exception.
        /// </summary>
        /// <param name="logGroup">The unique ID that groups log items together.</param>
        /// <param name="context">The log context.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="level">The level.</param>
        /// <param name="message">The message.</param>
        /// <param name="parameters">The parameters.</param>
        /// <remarks>If the error <see cref="LoggingLevel">log level</see> is invalid then the log won't be added.</remarks>
        [UsedImplicitly]
        [NotNull]
        internal static Log Add(Guid logGroup, [CanBeNull] LogContext context, [NotNull] LoggingException exception, LoggingLevel level, [NotNull] string message,
                               [NotNull] params object[] parameters)
        {
            return new Log(logGroup, context, exception, level, message, parameters);
        }
        #endregion
    }
}