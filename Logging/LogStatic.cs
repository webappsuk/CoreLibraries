#region © Copyright Web Applications (UK) Ltd, 2014.  All rights reserved.
// Copyright (c) 2014, Web Applications UK Ltd
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
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WebApplications.Utilities.Configuration;
using WebApplications.Utilities.Logging.Configuration;
using WebApplications.Utilities.Logging.Interfaces;
using WebApplications.Utilities.Logging.Loggers;
using WebApplications.Utilities.Performance;
using WebApplications.Utilities.Threading;

namespace WebApplications.Utilities.Logging
{
    /// <summary>
    ///   Defines <see langword="static"/> members of the <see cref="Log"/> class.
    /// </summary>
    public partial class Log
    {
        /// <summary>
        /// The Header/Footer string
        /// </summary>
        [NotNull] private const string Header =
            "====================================================================================================";

        /// <summary>
        /// The new log item performance counter.
        /// </summary>
        [NotNull] [NonSerialized] private static readonly PerfCounter _perfCounterNewItem;

        /// <summary>
        /// The exception performance counter.
        /// </summary>
        [NotNull] [NonSerialized] internal static readonly PerfCounter PerfCounterException;

        /// <summary>
        /// Calculates the most likely candidate for an assembly that serves as an entry point.
        /// </summary>
        [NotNull] private static readonly Lazy<Assembly> _entryAssembly = new Lazy<Assembly>(
            () =>
            {
                Assembly assembly = null;
                /*
                 * Scan call stack for assemblies that we can query
                 */
                try
                {
                    // Create a hash set of known public keys that we don't want to match.
                    HashSet<HashedByteArray> publicKeys = new HashSet<HashedByteArray>
                    {
                        typeof (int).Assembly.GetName().GetPublicKey(),
                        typeof (Observable).Assembly.GetName().GetPublicKey(),
                        typeof (UtilityExtensions).Assembly.GetName().GetPublicKey()
                    };
                    // Get the assembly name from the lowest point on the stack that does not have a
                    // 'known' public key.
                    StackFrame[] frames = new StackTrace().GetFrames();
                    if (!ReferenceEquals(frames, null))
                    {
                        // ReSharper disable PossibleNullReferenceException
                        assembly = frames
                            .Select(f => f.GetMethod())
                            .Where(m => m != null)
                            .Select(m => m.DeclaringType)
                            .Where(t => t != null)
                            .Select(t => new {t.Assembly, Name = t.Assembly.GetName()})
                            // ReSharper disable once AssignNullToNotNullAttribute
                            .Where(n => (n.Name != null) && !publicKeys.Contains(n.Name.GetPublicKey()))
                            .Select(n => n.Assembly)
                            .FirstOrDefault();
                        // ReSharper restore PossibleNullReferenceException
                    }
                }
                catch
                {
                    assembly = null;
                }

                // If we failed to get an assembly the fancy way, try a less subtle approach.
                return assembly ?? Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
            }, LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// Holds lookup for logging levels.
        /// </summary>
        [NotNull] private static readonly Dictionary<string, LoggingLevel> _levels =
            ExtendedEnum<LoggingLevel>.ValueDetails
                .SelectMany(
                    vd => vd.Select(v => new KeyValuePair<string, LoggingLevel>(v.ToLower(), vd.Value)))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value, StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// Holds lookup for formats.
        /// </summary>
        [NotNull] private static readonly Dictionary<string, LogFormat> _formats =
            ExtendedEnum<LogFormat>.ValueDetails
                .SelectMany(
                    vd => vd.Select(v => new KeyValuePair<string, LogFormat>(v.ToLower(), vd.Value)))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value, StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// The log reservation.
        /// </summary>
        [NonSerialized] private static readonly Guid _logReservation = System.Guid.NewGuid();

        /// <summary>
        /// The parameter key prefix.
        /// </summary>
        [NotNull] [NonSerialized] public static readonly string LogKeyPrefix = LogContext.ReservePrefix("Log ",
            _logReservation);

        /// <summary>
        /// The parameter key prefix.
        /// </summary>
        [NotNull] [NonSerialized] public static readonly string ParameterKeyPrefix =
            LogContext.ReservePrefix("Log Parameter ", _logReservation);

        /// <summary>
        /// Reserved context key.
        /// </summary>
        [NotNull] [NonSerialized] public static readonly string GuidKey = LogContext.ReserveKey("Log GUID",
            _logReservation);

        /// <summary>
        /// Reserved context key.
        /// </summary>
        [NotNull] [NonSerialized] public static readonly string GroupKey = LogContext.ReserveKey("Log Group",
            _logReservation);

        /// <summary>
        /// Reserved context key.
        /// </summary>
        [NotNull] [NonSerialized] public static readonly string LevelKey = LogContext.ReserveKey("Log Level",
            _logReservation);

        /// <summary>
        /// Reserved context key.
        /// </summary>
        [NotNull] [NonSerialized] public static readonly string MessageFormatKey =
            LogContext.ReserveKey("Log Message Format", _logReservation);

        /// <summary>
        /// Reserved context key.
        /// </summary>
        [NotNull] [NonSerialized] public static readonly string ExceptionTypeFullNameKey =
            LogContext.ReserveKey("Log Exception Type", _logReservation);

        /// <summary>
        /// Reserved context key.
        /// </summary>
        [NotNull] [NonSerialized] public static readonly string StackTraceKey = LogContext.ReserveKey(
            "Log Stack Trace", _logReservation);

        /// <summary>
        /// Reserved context key.
        /// </summary>
        [NotNull] [NonSerialized] public static readonly string ThreadIDKey = LogContext.ReserveKey("Log Thread ID",
            _logReservation);

        /// <summary>
        /// Reserved context key.
        /// </summary>
        [NotNull] [NonSerialized] public static readonly string ThreadNameKey = LogContext.ReserveKey(
            "Log Thread Name", _logReservation);

        /// <summary>
        /// Reserved context key.
        /// </summary>
        [NotNull] [NonSerialized] public static readonly string StoredProcedureKey =
            LogContext.ReserveKey("Log Stored Procedure", _logReservation);

        /// <summary>
        /// Reserved context key.
        /// </summary>
        [NotNull] [NonSerialized] public static readonly string StoredProcedureLineKey =
            LogContext.ReserveKey("Log Stored Procedure Line", _logReservation);

        /// <summary>
        /// The logging assembly
        /// </summary>
        [NotNull] [NonSerialized] internal static readonly Assembly LoggingAssembly = typeof (Log).Assembly;

        /// <summary>
        /// The global tick, ticks once a second and is used for batching, etc.
        /// </summary>
        [NonSerialized] [NotNull] public static readonly IObservable<long> Tick;

        /// <summary>
        /// Loggers collection.
        /// </summary>
        [NonSerialized] [NotNull] private static readonly List<ILogger> _loggers;

        /// <summary>
        /// The default memory logger always exists and ensures we're always capturing at least the last minutes worth of logs.
        /// </summary>
        [NonSerialized] [NotNull] private static readonly MemoryLogger _defaultMemoryLogger;

        /// <summary>
        /// The tick subscription.
        /// </summary>
        [NonSerialized] [NotNull] private static readonly IDisposable _tickSubscription;

        /// <summary>
        /// Flushes the queues asynchronously, note that only one instance is run at a time.
        /// </summary>
        [NotNull] private static readonly AsyncDebouncedAction _doFlush = new AsyncDebouncedAction(
            token =>
            {
                // Grab batch.
                Log[] batch;
                int batchCount = 0;
                lock (_queue)
                {
                    batch = new Log[_queue.Count];
                    while (_queue.Count > 0 &&
                           !token.IsCancellationRequested)
                    {
                        Log l = _queue.Dequeue();
                        if (l.Level.IsValid(ValidLevels))
                            batch[batchCount++] = l;
                    }
                }

                if ((ValidLevels == LoggingLevels.None) ||
                    (batch.Length < 1))
                    return TaskResult.Completed;

                // Grab valid loggers
                List<ILogger> loggers;
                lock (_loggers)
                    loggers = _loggers
                        .Where(l => (((byte) l.ValidLevels) & ((byte) ValidLevels)) > 0)
                        .ToList();

                // Add memory logger.
                loggers.Add(_defaultMemoryLogger);

                // Next bit we can run in parallel.
                Parallel.ForEach(loggers, l =>
                {
                    try
                    {
                        l.Add(batch.Where(log => log.Level.IsValid(l.ValidLevels)));
                    }
                    catch (Exception e)
                    {
                        // Disable this logger as it threw an exception
                        // ReSharper disable once ObjectCreationAsStatement
                        new LoggingException(
                            e,
                            LoggingLevel.Critical,
                            Resources.LogStatic_LogBatch_FatalErrorOccured,
                            l.Name);

                        // Stop logger running again.
                        l.ValidLevels = LoggingLevels.None;
                    }
                });
                return TaskResult.Completed;
            });

        /// <summary>
        /// The global logging queue.
        /// </summary>
        [NotNull] private static readonly Queue<Log> _queue = new Queue<Log>();

        /// <summary>
        ///   Initializes static members of the <see cref="Log" /> class.
        /// </summary>
        static Log()
        {
            // Set logging to all
            ValidLevels = LoggingLevels.All;

            // Initialize performance counters
            PerfCounterException =
                PerfCategory.GetOrAdd<PerfCounter>("Logged exception", "Tracks every time an exception is logged.");
            _perfCounterNewItem =
                PerfCategory.GetOrAdd<PerfCounter>("Logged new item", "Tracks every time a log entry is logged.");

            // Create tick
            Tick = Observable.Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));

            _loggers = new List<ILogger>();
            _defaultMemoryLogger = new MemoryLogger("Default memory logger", TimeSpan.FromMinutes(1));

            // Flush on tick.
            _tickSubscription = Tick.Subscribe(l => Flush());

            ConfigurationSection<LoggingConfiguration>.Changed += (o, e) => LoadConfiguration();

            // Flush logs on domain unload.
            AppDomain.CurrentDomain.DomainUnload += (s, e) => Cleanup();

            if (PerfCategory.HasAccess)
            {
                Add(LoggingLevel.Notification, Resources.Log_PerfCategory_Enabled, PerfCategory.InstanceGuid);

                foreach (PerfCategory pc in PerfCategory.All)
                {
                    if (pc.IsValid)
                        Add(LoggingLevel.Debugging, Resources.Log_PerfCategory_Initialized, pc.CategoryName);
                    else
                        Add(LoggingLevel.Warning, Resources.Log_PerfCategory_Missing, pc.CategoryName);
                }
            }
            else
                Add(LoggingLevel.Warning, Resources.Log_PerfCategory_Access_Denied);
        }

        /// <summary>
        /// Gets the most likely entry assembly.
        /// </summary>
        /// <value>The entry assembly.</value>
        [NotNull]
        [PublicAPI]
        public static Assembly EntryAssembly
        {
            get
            {
                Contract.Assert(_entryAssembly.Value != null);
                return _entryAssembly.Value;
            }
        }

        /// <summary>
        ///   Gets the valid <see cref="LoggingLevel">log levels</see>.
        /// </summary>
        [UsedImplicitly]
        public static LoggingLevels ValidLevels { get; set; }

        /// <summary>
        ///   Gets the loggers.
        /// </summary>
        [UsedImplicitly]
        [NotNull]
        public static IEnumerable<ILogger> Loggers
        {
            get
            {
                ILogger[] copy;
                lock (_loggers)
                    copy = _loggers.ToArray();
                return copy;
            }
        }

        /// <summary>
        /// Gets the name of the application.
        /// </summary>
        /// <value>The name of the application.</value>
        [NotNull]
        public static string ApplicationName { get; private set; }

        /// <summary>
        /// Gets the application GUID.
        /// </summary>
        /// <value>The name of the application.</value>
        public static Guid ApplicationGuid { get; private set; }

        /// <summary>
        ///   The maximum number of log entries to store in the memory cache.
        /// </summary>
        public static int CacheMaximum
        {
            get { return _defaultMemoryLogger.MaximumLogEntries; }
            set { _defaultMemoryLogger.MaximumLogEntries = value; }
        }

        /// <summary>
        ///   The maximum length of time to hold items in the memory cache.
        /// </summary>
        public static TimeSpan CacheExpiry
        {
            get { return _defaultMemoryLogger.CacheExpiry; }
            set { _defaultMemoryLogger.CacheExpiry = value; }
        }

        /// <summary>
        /// Gets all cached logs.
        /// </summary>
        /// <value>The query.</value>
        [NotNull]
        public static IEnumerable<Log> AllCached
        {
            get { return _defaultMemoryLogger.All; }
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
            lock (_loggers)
            {
                // Get the active configuration
                LoggingConfiguration configuration = ConfigurationSection<LoggingConfiguration>.Active;

                ApplicationName = configuration.ApplicationName;
                ApplicationGuid = configuration.ApplicationGuid;

                // Get all loggers
                ILogger[] loggers = _loggers.ToArray();
                // Remove all loggers
                _loggers.Clear();

                // Dispose loggers
                foreach (ILogger logger in loggers.Where(logger => logger != null))
                    logger.Dispose();

                // Update default memory logger values
                if (configuration.LogCacheExpiry > TimeSpan.Zero)
                    _defaultMemoryLogger.CacheExpiry = configuration.LogCacheExpiry;
                if (configuration.LogCacheMaximumEntries > 0)
                    _defaultMemoryLogger.MaximumLogEntries = configuration.LogCacheMaximumEntries;


                // If we're enabled add specified loggers that are also enabled.
                if (configuration.Enabled)
                {
                    ValidLevels = configuration.ValidLevels;
                    foreach (LoggerElement loggerElement in configuration.Loggers.Where(l => l != null && l.Enabled))
                    {
                        try
                        {
                            AddLogger(loggerElement.GetInstance<ILogger>(), _defaultMemoryLogger);
                        }
                        catch (Exception exception)
                        {
                            new LoggingException(exception,
                                LoggingLevel.Critical,
                                Resources.LogStatic_LoadConfiguration_ErrorCreatingLogger,
                                loggerElement.Name);
                        }
                    }

                    Add(LoggingLevel.Notification, Resources.Log_Configured, ApplicationName, ApplicationGuid);
                }
                else
                {
                    // Disable logging.
                    ValidLevels = LoggingLevels.None;
                }
            }
        }

        /// <summary>
        /// Sets a trace logger.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="validLevels">The valid levels.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [PublicAPI]
        public static void SetTrace(LogFormat format, LoggingLevels validLevels = LoggingLevels.All)
        {
            SetTrace(format.ToString(), validLevels);
        }

        /// <summary>
        /// Sets a trace logger.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="validLevels">The valid levels.</param>
        [PublicAPI]
        public static void SetTrace([CanBeNull] string format = null, LoggingLevels validLevels = LoggingLevels.All)
        {
            if (format == null)
                format = TraceLogger.DefaultFormat;
            TraceLogger traceLogger = _loggers.OfType<TraceLogger>().SingleOrDefault();
            if (traceLogger != null)
            {
                traceLogger.Format = format;
                traceLogger.ValidLevels = validLevels;
                return;
            }

            AddLogger(new TraceLogger("Trace logger", format, validLevels), _defaultMemoryLogger);
        }

        /// <summary>
        /// Sets a console logger (if running in a console).
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="validLevels">The valid levels.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [PublicAPI]
        public static void SetConsole(LogFormat format, LoggingLevels validLevels = LoggingLevels.All)
        {
            SetConsole(format.ToString(), validLevels);
        }

        /// <summary>
        /// Sets a console logger (if running in a console).
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="validLevels">The valid levels.</param>
        [PublicAPI]
        public static void SetConsole([CanBeNull] string format = null, LoggingLevels validLevels = LoggingLevels.All)
        {
            if (format == null)
                format = ConsoleLogger.DefaultFormat;

            ConsoleLogger consoleLogger = _loggers.OfType<ConsoleLogger>().SingleOrDefault();
            if (consoleLogger != null)
            {
                consoleLogger.Format = format;
                consoleLogger.ValidLevels = validLevels;
                return;
            }

            AddLogger(new ConsoleLogger("Console logger", format, validLevels), _defaultMemoryLogger);
        }

        /// <summary>
        ///   Adds a new logger and copies over log items from the source logger.
        /// </summary>
        /// <param name="logger">The logger to add.</param>
        /// <param name="sourceLogger">
        ///   <para>The source logger to copy log items from defaults to the cache.</para>
        /// </param>
        /// <param name="limit">
        ///   <para>The maximum number of logs to copy.</para>
        ///   <para>By default this is set to <see cref="int.MaxValue"/>.</para>
        /// </param>
        /// <returns>The logger with the specified logger name.</returns>
        /// <exception cref="LoggingException">
        ///   <see cref="LoggerBase.Queryable">retrieval</see> is not supported.
        /// </exception>
        [UsedImplicitly]
        public static ILogger AddLogger(
            [NotNull] ILogger logger,
            [CanBeNull] ILogger sourceLogger = null,
            int limit = Int32.MaxValue)
        {
            if (sourceLogger == null)
                sourceLogger = _defaultMemoryLogger;

            // Check for source logger can retrieve logs
            if (!sourceLogger.Queryable)
            {
                throw new LoggingException(
                    Resources.LogStatic_AddOrUpdateLogger_RetrievalNotSupported,
                    logger.Name);
            }

            // Add logs
            logger.Add(sourceLogger.Qbserve.TakeLast(limit).ToEnumerable().Select(l => new Log(l)));

            lock (_loggers)
            {
                _loggers.Add(logger);

                // Check for collisions on singletons.
                Type lType = _loggers.GetType();
                if (_loggers.Any(l => !l.AllowMultiple && l.GetType() == lType))
                {
                    _loggers.Remove(logger);
                    throw new LoggingException(
                        Resources.Log_Logger_Multiple_Instances,
                        logger.Name);
                }
            }

            return logger;
        }

        /// <summary>
        /// Tries to remove the logger with the name specified.
        /// </summary>
        /// <param name="logger">The retrieved logger.</param>
        /// <returns>Returns <see langword="true" /> if the logger was retrieved successfully; otherwise returns <see langword="false" />.</returns>
        [UsedImplicitly]
        public static bool RemoveLogger([NotNull] ILogger logger)
        {
            lock (_loggers)
                return _loggers.Remove(logger);
        }

        /// <summary>
        ///   Flushes all outstanding logs asynchronously.
        /// </summary>
        /// <remarks>
        ///   Note: If more logs are added whilst the system is flushing this will continue to block until there are no outstanding logs.
        /// </remarks>
        [PublicAPI]
        [NotNull]
        public static Task Flush(CancellationToken token = default(CancellationToken))
        {
            return _doFlush.Run(token);
        }

        /// <summary>
        /// Cleanups this instance.
        /// </summary>
        private static void Cleanup()
        {
            Thread.BeginCriticalRegion();
            _tickSubscription.Dispose();
            Add(LoggingLevel.Notification, Resources.Log_Application_Exiting, ApplicationName, ApplicationGuid);

            Flush().Wait();
            ILogger[] loggers = _loggers.ToArray();

            _loggers.Clear();
            foreach (ILogger logger in _loggers)
                logger.Dispose();

            _defaultMemoryLogger.Dispose();
            Thread.EndCriticalRegion();
            Trace.WriteLine(Resources.Log_Cleanup_Finished);
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
            if (LoggingLevel.Information.IsValid(ValidLevels))
                new Log(CombGuid.Empty, null, null, LoggingLevel.Information, message, parameters);
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
            if (LoggingLevel.Information.IsValid(ValidLevels))
                new Log(CombGuid.Empty, context, null, LoggingLevel.Information, message, parameters);
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
        public static void Add(CombGuid logGroup, [NotNull] string message, [NotNull] params object[] parameters)
        {
            // Add to queue for logging if we are a valid level.
            if (LoggingLevel.Information.IsValid(ValidLevels))
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
        public static void Add(CombGuid logGroup, [NotNull] LogContext context, [NotNull] string message,
            [NotNull] params object[] parameters)
        {
            // Add to queue for logging if we are a valid level.
            if (LoggingLevel.Information.IsValid(ValidLevels))
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
            if (level.IsValid(ValidLevels))
                new Log(CombGuid.Empty, null, null, level, message, parameters);
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
            if (level.IsValid(ValidLevels))
                new Log(CombGuid.Empty, context, null, level, message, parameters);
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
        public static void Add(CombGuid logGroup, LoggingLevel level, [NotNull] string message,
            [NotNull] params object[] parameters)
        {
            // Add to queue for logging if we are a valid level.
            if (level.IsValid(ValidLevels))
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
        public static void Add(CombGuid logGroup, [NotNull] LogContext context, LoggingLevel level,
            [NotNull] string message,
            [NotNull] params object[] parameters)
        {
            // Add to queue for logging if we are a valid level.
            if (level.IsValid(ValidLevels))
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
            if (level.IsValid(ValidLevels))
                new Log(CombGuid.Empty, null, exception, level, exception.Message);
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
            if (level.IsValid(ValidLevels))
                new Log(CombGuid.Empty, context, exception, level, exception.Message);
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
        public static void Add(CombGuid logGroup, [NotNull] Exception exception, LoggingLevel level = LoggingLevel.Error)
        {
            // Don't re-add logging exception, as they add themselves.
            if (exception is LoggingException)
                return;

            // Add to queue for logging if we are a valid level.
            if (level.IsValid(ValidLevels))
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
        public static void Add(CombGuid logGroup, [NotNull] LogContext context, [NotNull] Exception exception,
            LoggingLevel level = LoggingLevel.Error)
        {
            // Don't re-add logging exception, as they add themselves.
            if (exception is LoggingException)
                return;

            // Add to queue for logging if we are a valid level.
            if (level.IsValid(ValidLevels))
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
        internal static Log Add(CombGuid logGroup, [CanBeNull] LogContext context, [NotNull] LoggingException exception,
            LoggingLevel level, [NotNull] string message,
            [NotNull] params object[] parameters)
        {
            return new Log(logGroup, context, exception, level, message, parameters);
        }
        #endregion
    }
}