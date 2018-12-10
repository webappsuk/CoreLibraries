﻿#region © Copyright Web Applications (UK) Ltd, 2015.  All rights reserved.
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

using NodaTime;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Caching;
using WebApplications.Utilities.Configuration;
using WebApplications.Utilities.Formatting;
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
    public sealed partial class Log
    {
        /// <summary>
        /// The new log item performance counter.
        /// </summary>
        [NotNull]
        [NonSerialized]
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private static readonly PerfCounter _perfCounterNewItem;

        /// <summary>
        /// The exception performance counter.
        /// </summary>
        [NotNull]
        [NonSerialized]
        internal static readonly PerfCounter PerfCounterException;

        /// <summary>
        /// Calculates the most likely candidate for an assembly that serves as an entry point.
        /// </summary>
        [NotNull]
        private static readonly Lazy<Assembly> _entryAssembly = new Lazy<Assembly>(
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
                        typeof(int).Assembly.GetName().GetPublicKey(),
                        typeof(UtilityExtensions).Assembly.GetName().GetPublicKey(),
                        typeof(PerformanceCounterExtensions).Assembly.GetName().GetPublicKey()
                    };
                    // Get the assembly name from the lowest point on the stack that does not have a
                    // 'known' public key.
                    StackFrame[] frames = new StackTrace().GetFrames();
                    if (!ReferenceEquals(frames, null))
                        // ReSharper disable PossibleNullReferenceException
                        assembly = frames
                            .Select(f => f.GetMethod())
                            .Where(m => m != null)
                            .Select(m => m.DeclaringType)
                            .Where(t => t != null)
                            .Select(t => new { t.Assembly, Name = t.Assembly.GetName() })
                            // ReSharper disable once AssignNullToNotNullAttribute
                            .Where(n => (n.Name != null) && !publicKeys.Contains(n.Name.GetPublicKey()))
                            .Select(n => n.Assembly)
                            .FirstOrDefault();
                    // ReSharper restore PossibleNullReferenceException
                }
                catch
                {
                    assembly = null;
                }

                // If we failed to get an assembly the fancy way, try a less subtle approach.
                return assembly ?? Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
            },
            LazyThreadSafetyMode.PublicationOnly);

        #region Format Tags
        /// <summary>
        /// The header control format tag.
        /// </summary>
        [NotNull]
        public const string FormatTagHeader = "!header";

        /// <summary>
        /// The message format tag.
        /// </summary>
        [NotNull]
        public const string FormatTagMessage = "message";

        /// <summary>
        /// The resource format tag.
        /// </summary>
        [NotNull]
        public const string FormatTagResource = "resource";

        /// <summary>
        /// The culture format tag.
        /// </summary>
        [NotNull]
        public const string FormatTagCulture = "culture";

        /// <summary>
        /// The time format tag.
        /// </summary>
        [NotNull]
        public const string FormatTagTimeStamp = "time";

        /// <summary>
        /// The local time format tag.
        /// </summary>
        public const string FormatTagTimeStampLocal = "localtime";

        /// <summary>
        /// The level format tag.
        /// </summary>
        [NotNull]
        public const string FormatTagLevel = "level";

        /// <summary>
        /// The guid format tag.
        /// </summary>
        [NotNull]
        public const string FormatTagGuid = "guid";

        /// <summary>
        /// The exception format tag.
        /// </summary>
        [NotNull]
        public const string FormatTagException = "exception";

        /// <summary>
        /// The stack format tag.
        /// </summary>
        [NotNull]
        public const string FormatTagStackTrace = "stack";

        /// <summary>
        /// The thread ID format tag.
        /// </summary>
        [NotNull]
        public const string FormatTagThreadID = "threadid";

        /// <summary>
        /// The thread format tag.
        /// </summary>
        [NotNull]
        public const string FormatTagThreadName = "thread";

        /// <summary>
        /// The application name format tag.
        /// </summary>
        [NotNull]
        public const string FormatTagApplicationName = "application";

        /// <summary>
        /// The application guid format tag.
        /// </summary>
        [NotNull]
        public const string FormatTagApplicationGuid = "applicationguid";

        /// <summary>
        /// The sproc format tag.
        /// </summary>
        [NotNull]
        public const string FormatTagStoredProcedure = "sproc";

        /// <summary>
        /// The innter exception format tag.
        /// </summary>
        [NotNull]
        public const string FormatTagInnerException = "innerexception";

        /// <summary>
        /// The context format tag.
        /// </summary>
        [NotNull]
        public const string FormatTagContext = "context";
        #endregion

        #region Context Reservations
        /// <summary>
        /// The log reservation.
        /// </summary>
        [NonSerialized]
        private static readonly Guid _logReservation = System.Guid.NewGuid();

        /// <summary>
        /// The parameter key prefix.
        /// </summary>
        [NotNull]
        [NonSerialized]
        public static readonly string LogKeyPrefix = LogContext.ReservePrefix(
            "Log ",
            _logReservation);

        /// <summary>
        /// The parameter key prefix.
        /// </summary>
        [NotNull]
        [NonSerialized]
        public static readonly string ParameterPrefix =
            LogContext.ReservePrefix("Log Parameter ", _logReservation);

        /// <summary>
        /// Reserved context key.
        /// </summary>
        [NotNull]
        [NonSerialized]
        public static readonly string GuidKey = LogContext.ReserveKey(
            "Log GUID",
            _logReservation);

        /// <summary>
        /// Reserved context key.
        /// </summary>
        [NotNull]
        [NonSerialized]
        public static readonly string LevelKey = LogContext.ReserveKey(
            "Log Level",
            _logReservation);

        /// <summary>
        /// Reserved context key.
        /// </summary>
        [NotNull]
        [NonSerialized]
        public static readonly string MessageFormatKey =
            LogContext.ReserveKey("Log Message Format", _logReservation);

        /// <summary>
        /// Reserved context key.
        /// </summary>
        [NotNull]
        [NonSerialized]
        public static readonly string ResourcePropertyKey =
            LogContext.ReserveKey("Log Resource Property", _logReservation);

        /// <summary>
        /// Reserved context key.
        /// </summary>
        [NotNull]
        [NonSerialized]
        public static readonly string ExceptionTypeFullNameKey =
            LogContext.ReserveKey("Log Exception Type", _logReservation);

        /// <summary>
        /// Reserved context key.
        /// </summary>
        [NotNull]
        [NonSerialized]
        public static readonly string InnerExceptionGuidsPrefix =
            LogContext.ReservePrefix("Log Inner Exception ", _logReservation);

        /// <summary>
        /// Reserved context key.
        /// </summary>
        [NotNull]
        [NonSerialized]
        public static readonly string StackTraceKey = LogContext.ReserveKey(
            "Log Stack Trace",
            _logReservation);

        /// <summary>
        /// Reserved context key.
        /// </summary>
        [NotNull]
        [NonSerialized]
        public static readonly string ThreadIDKey = LogContext.ReserveKey(
            "Log Thread ID",
            _logReservation);

        /// <summary>
        /// Reserved context key.
        /// </summary>
        [NotNull]
        [NonSerialized]
        public static readonly string ThreadNameKey = LogContext.ReserveKey(
            "Log Thread Name",
            _logReservation);

        /// <summary>
        /// Reserved context key.
        /// </summary>
        [NotNull]
        [NonSerialized]
        public static readonly string StoredProcedureKey =
            LogContext.ReserveKey("Log Stored Procedure", _logReservation);

        /// <summary>
        /// Reserved context key.
        /// </summary>
        [NotNull]
        [NonSerialized]
        public static readonly string StoredProcedureLineKey =
            LogContext.ReserveKey("Log Stored Procedure Line", _logReservation);

        /// <summary>
        /// Reserved context key.
        /// </summary>
        [NotNull]
        [NonSerialized]
        public static readonly string IsTerminatingKey =
            LogContext.ReserveKey("Is Terminating", _logReservation);
        #endregion

        /// <summary>
        /// The logging assembly
        /// </summary>
        [NotNull]
        [NonSerialized]
        internal static readonly Assembly LoggingAssembly = typeof(Log).Assembly;

        /// <summary>
        /// Loggers collection.
        /// </summary>
        [NonSerialized]
        [NotNull]
        private static readonly Dictionary<ILogger, LoggerInfo> _loggers;

        /// <summary>
        /// The default memory logger always exists and ensures we're always capturing at least the last minutes worth of logs.
        /// </summary>
        [NonSerialized]
        [NotNull]
        private static readonly MemoryLogger _defaultMemoryLogger;

        /// <summary>
        /// The tick subscription.
        /// </summary>
        [NonSerialized]
        [NotNull]
        private static readonly AsyncTimer _tickAction;

        /// <summary>
        /// The queue lock.
        /// </summary>
        [NotNull]
        private static readonly AsyncLock _queueLock = new AsyncLock();

        /// <summary>
        /// The global logging queue.
        /// </summary>
        [NotNull]
        private static readonly ConcurrentBag<Log> _buffer = new ConcurrentBag<Log>();

        /// <summary>
        ///   Initializes static members of the <see cref="Log" /> class.
        /// </summary>
        // ReSharper disable once NotNullMemberIsNotInitialized
        static Log()
        {
            // Set logging to all
            ValidLevels = LoggingLevels.All;

            Color.Red.SetName(LogLevelColorName);

            // Initialize performance counters
            PerfCounterException =
                PerfCategory.GetOrAdd<PerfCounter>("Logged exception", "Tracks every time an exception is logged.");
            _perfCounterNewItem =
                PerfCategory.GetOrAdd<PerfCounter>("Logged new item", "Tracks every time a log entry is logged.");

            // Create tick action
            _tickAction = new AsyncTimer(DoFlush, dueTime: TimeHelpers.InfiniteDuration);

            // Create loggers and add default memory logger.
            _loggers = new Dictionary<ILogger, LoggerInfo>();
            _defaultMemoryLogger = new MemoryLogger("Default memory logger", TimeSpan.FromMinutes(1));
            _loggers[_defaultMemoryLogger] = new LoggerInfo(false, typeof(MemoryLogger));

            ConfigurationSection<LoggingConfiguration>.ActiveChanged += (o, e) => LoadConfiguration();

            // Flush logs on domain unload and unhandled exceptions.
            AppDomain.CurrentDomain.DomainUnload += (s, e) => CleanUp();
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            Add(LoggingLevel.Notification, () => Resources.Log_PerfCategory_GUID, PerfCategory.InstanceGuid);
        }

        /// <summary>
        /// Gets the most likely entry assembly.
        /// </summary>
        /// <value>The entry assembly.</value>
        [NotNull]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public static Assembly EntryAssembly
        {
            get
            {
                Debug.Assert(_entryAssembly.Value != null);
                return _entryAssembly.Value;
            }
        }

        /// <summary>
        ///   Gets the valid <see cref="LoggingLevel">log levels</see>.
        /// </summary>
        public static LoggingLevels ValidLevels { get; set; }

        /// <summary>
        ///   Gets the loggers.
        /// </summary>
        [NotNull]
        public static IEnumerable<ILogger> Loggers
        {
            get
            {
                lock (_loggers)
                    return _loggers.Keys;
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
        /// Clears all cached logs.
        /// </summary>
        public static void ClearCache()
        {
            _defaultMemoryLogger.Clear();
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
        /// The default tick period.
        /// </summary>
        private static readonly Duration _defaultPeriod = Duration.FromSeconds(1);

        /// <summary>
        ///   Loads the configuration whenever it is changed.
        /// </summary>
        /// <remarks>
        ///   As this is called from ModuleInitializer then this code is called whenever the Assembly
        ///   is referenced. In turn this activates the static constructor, which attaches this
        ///   method as the response to configuration changes. However, the initial configuration load does
        ///   not trigger that event, so it's only hit once on start up and then on every subsequent change.
        /// </remarks>
        internal static void LoadConfiguration()
        {
            _tickAction.Change(dueTime: TimeHelpers.InfiniteDuration);

            // Ensure we only run one load at a time.
            lock (_loggers)
            {
                // Get the active configuration
                LoggingConfiguration configuration = ConfigurationSection<LoggingConfiguration>.Active;
                Debug.Assert(configuration != null);

                ApplicationName = configuration.ApplicationName;
                ApplicationGuid = configuration.ApplicationGuid;

                // Grab all loggers that came from the configuration.
                // ReSharper disable once PossibleNullReferenceException
                KeyValuePair<ILogger, LoggerInfo>[] loggers =
                    _loggers.Where(kvp => kvp.Value.IsFromConfiguration).ToArray();
                foreach (ILogger l in loggers.Select(l => l.Key))
                {
                    Debug.Assert(l != null);
                    _loggers.Remove(l);
                }

                // Dispose existing configuration loggers
                Parallel.ForEach(
                    loggers,
                    kvp =>
                    {
                        Debug.Assert(kvp.Key != null);
                        Debug.Assert(kvp.Value != null);
                        using (kvp.Value.Lock.LockAsync().Result)
                            kvp.Key.Dispose();
                    });

                // Update default memory logger values
                if (configuration.LogCacheExpiry > TimeSpan.Zero)
                    _defaultMemoryLogger.CacheExpiry = configuration.LogCacheExpiry;
                if (configuration.LogCacheMaximumEntries > 0)
                    _defaultMemoryLogger.MaximumLogEntries = configuration.LogCacheMaximumEntries;

                // If we're enabled add specified loggers that are also enabled.
                if (configuration.Enabled)
                {
                    ValidLevels = configuration.ValidLevels;
                    foreach (LoggerElement loggerElement in configuration.Loggers.Where(l => l.Enabled))
                    {
                        Debug.Assert(loggerElement != null);
                        try
                        {
                            AddLogger(loggerElement.GetInstance<ILogger>(), _defaultMemoryLogger, int.MaxValue, true);
                        }
                        catch (Exception exception)
                        {
                            // ReSharper disable once ObjectCreationAsStatement
                            new LoggingException(
                                exception,
                                LoggingLevel.Critical,
                                () => Resources.LogStatic_LoadConfiguration_ErrorCreatingLogger,
                                loggerElement.Name);
                        }
                    }

                    Add(LoggingLevel.Notification, () => Resources.Log_Configured, ApplicationName, ApplicationGuid);

                    // Get the tick period, and re-enabled the tick.
                    Duration configPeriod = configuration.Period;
                    Duration period = configPeriod < Duration.Zero
                        ? _defaultPeriod
                        : configPeriod;

                    _tickAction.Change(period, Duration.Zero);
                }
                else
                // Disable logging.
                    ValidLevels = LoggingLevels.None;
            }
        }

        /// <summary>
        /// Sets a trace logger.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="validLevels">The valid levels.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetTrace([CanBeNull] string format, LoggingLevels validLevels = LoggingLevels.All)
        {
            SetTrace(format != null ? new FormatBuilder().Append(format) : null, validLevels);
        }

        /// <summary>
        /// Sets a trace logger.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="validLevels">The valid levels.</param>
        public static void SetTrace(
            [CanBeNull] FormatBuilder format = null,
            LoggingLevels validLevels = LoggingLevels.All)
        {
            TraceLogger traceLogger;
            lock (_loggers)
                traceLogger = _loggers.Keys.OfType<TraceLogger>().FirstOrDefault();

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
        public static void SetConsole([CanBeNull] string format, LoggingLevels validLevels = LoggingLevels.All)
        {
            if (!ConsoleHelper.IsConsole)
                return;

            SetConsole(format != null ? new FormatBuilder().Append(format) : null, validLevels);
        }

        /// <summary>
        /// Sets a console logger (if running in a console).
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="validLevels">The valid levels.</param>
        public static void SetConsole(
            [CanBeNull] FormatBuilder format = null,
            LoggingLevels validLevels = LoggingLevels.All)
        {
            if (!ConsoleHelper.IsConsole)
                return;

            ConsoleLogger consoleLogger;
            lock (_loggers)
                consoleLogger = _loggers.OfType<ConsoleLogger>().FirstOrDefault();
            if (consoleLogger != null)
            {
                consoleLogger.Format = format;
                consoleLogger.ValidLevels = validLevels;
                return;
            }

            AddLogger(new ConsoleLogger("Console logger", format, validLevels), _defaultMemoryLogger);
        }

        /// <summary>
        /// Adds a new logger and copies over log items from the source logger.
        /// </summary>
        /// <param name="logger">The logger to add.</param>
        /// <param name="sourceLogger">The source logger to copy log items from defaults to the cache.</param>
        /// <param name="limit"><para>The maximum number of logs to copy.</para>
        /// <para>By default this is set to <see cref="int.MaxValue" />.</para></param>
        /// <returns>The existing logger if duplicates not allowed; otherwise the supplied logger.</returns>
        [NotNull]
        public static ILogger AddLogger(
            [NotNull] ILogger logger,
            [CanBeNull] ILogger sourceLogger = null,
            int limit = Int32.MaxValue)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            return AddLogger(logger, sourceLogger, limit, false);
        }

        /// <summary>
        /// Adds the logger.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="sourceLogger">The source logger.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="isFromConfiguration">if set to <see langword="true" /> the logger was added from the configuration.</param>
        /// <returns>ILogger.</returns>
        [NotNull]
        private static ILogger AddLogger(
            [NotNull] ILogger logger,
            [CanBeNull] ILogger sourceLogger,
            int limit,
            bool isFromConfiguration)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            if (sourceLogger == null)
                sourceLogger = _defaultMemoryLogger;

            IDisposable loggerLock = null;
            try
            {
                lock (_loggers)
                {
                    Type loggerType = logger.GetType();
                    if (!logger.AllowMultiple)
                    {
                        // Check for existing logger of same type.
                        KeyValuePair<ILogger, LoggerInfo> existing =
                            // ReSharper disable once PossibleNullReferenceException
                            _loggers.FirstOrDefault(i => loggerType == i.Value.Type);
                        if (existing.Key != null)
                            return existing.Key;
                    }

                    // Create the logger info
                    LoggerInfo info = new LoggerInfo(false, loggerType);

                    // Grab the logger lock, this ensures we get to add log entries first!
                    loggerLock = info.Lock.LockAsync().Result;

                    // We can add logger and release _loggers lock.
                    _loggers.Add(logger, info);
                }

                // Add logs, we already have the lock, so we go first.
                IQueryable<Log> logs = sourceLogger.All;
                if (logs != null)
                    logger.Add(
                        limit < int.MaxValue ? (IEnumerable<Log>)new CyclicConcurrentQueue<Log>(logs, limit) : logs);
            }
            finally
            {
                if (loggerLock != null)
                    loggerLock.Dispose();
            }
            return logger;
        }

        /// <summary>
        /// Adds a new logger and copies over log items from the source logger.
        /// </summary>
        /// <typeparam name="T">The logger type.</typeparam>
        /// <param name="logger">The logger to add.</param>
        /// <param name="sourceLogger">The source logger to copy log items from defaults to the cache.</param>
        /// <param name="limit"><para>The maximum number of logs to copy.</para>
        /// <para>By default this is set to <see cref="int.MaxValue" />.</para></param>
        /// <returns>The existing logger if duplicates not allowed; otherwise the supplied logger.</returns>
        [NotNull]
        public static T AddLogger<T>(
            [NotNull] T logger,
            [CanBeNull] ILogger sourceLogger = null,
            int limit = Int32.MaxValue)
            where T : ILogger
        {
            if (logger == null) throw new ArgumentNullException("logger");
            return AddLogger(() => logger, sourceLogger, limit, false);
        }

        /// <summary>
        /// Adds a new logger and copies over log items from the source logger.
        /// </summary>
        /// <typeparam name="T">The logger type.</typeparam>
        /// <param name="loggerCreator">The logger creator.</param>
        /// <param name="sourceLogger">The source logger to copy log items from defaults to the cache.</param>
        /// <param name="limit"><para>The maximum number of logs to copy.</para>
        /// <para>By default this is set to <see cref="int.MaxValue" />.</para></param>
        /// <returns>The existing logger if duplicates not allowed; otherwise the supplied logger.</returns>
        [NotNull]
        public static T AddLogger<T>(
            [NotNull] [InstantHandle] Func<T> loggerCreator,
            [CanBeNull] ILogger sourceLogger = null,
            int limit = Int32.MaxValue)
            where T : ILogger
        {
            if (loggerCreator == null) throw new ArgumentNullException("loggerCreator");
            return AddLogger(loggerCreator, sourceLogger, limit, false);
        }

        /// <summary>
        /// Adds a new logger and copies over log items from the source logger.
        /// </summary>
        /// <typeparam name="T">The logger type.</typeparam>
        /// <param name="loggerCreator">The logger creator.</param>
        /// <param name="sourceLogger">The source logger.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="isFromConfiguration">if set to <see langword="true" /> [is from configuration].</param>
        /// <returns>The existing logger if duplicates not allowed; otherwise the supplied logger.</returns>
        [NotNull]
        private static T AddLogger<T>(
            [NotNull] [InstantHandle] Func<T> loggerCreator,
            [CanBeNull] ILogger sourceLogger,
            int limit,
            bool isFromConfiguration)
            where T : ILogger
        {
            if (loggerCreator == null) throw new ArgumentNullException("loggerCreator");
            if (sourceLogger == null)
                sourceLogger = _defaultMemoryLogger;

            T logger;
            IDisposable loggerLock = null;
            try
            {
                lock (_loggers)
                {
                    // Check for existing logger of same type.
                    KeyValuePair<ILogger, LoggerInfo> existing =
                        // ReSharper disable once PossibleNullReferenceException
                        _loggers.FirstOrDefault(i => i.Value.Type == typeof(T));
                    if ((existing.Key != null) &&
                        !existing.Key.AllowMultiple)
                        return (T)existing.Key;

                    logger = loggerCreator();
                    if (ReferenceEquals(logger, null))
                        // ReSharper disable once AssignNullToNotNullAttribute
                        return default(T);

                    // Create the logger info
                    LoggerInfo info = new LoggerInfo(false, typeof(T));

                    // Grab the logger lock, this ensures we get to add log entries first!
                    loggerLock = info.Lock.LockAsync().Result;

                    // We can add logger and release _loggers lock.
                    _loggers.Add(logger, info);
                }

                // Add logs, we already have the lock, so we go first.
                IQueryable<Log> logs = sourceLogger.All;
                if (logs != null)
                    logger.Add(
                        limit < int.MaxValue ? (IEnumerable<Log>)new CyclicConcurrentQueue<Log>(logs, limit) : logs);
            }
            finally
            {
                if (loggerLock != null)
                    loggerLock.Dispose();
            }
            return logger;
        }

        /// <summary>
        /// Tries to remove the logger with the name specified.
        /// </summary>
        /// <param name="logger">The retrieved logger.</param>
        /// <returns>Returns <see langword="true" /> if the logger was retrieved successfully; otherwise returns <see langword="false" />.</returns>
        public static bool RemoveLogger([NotNull] ILogger logger)
        {
            if (logger == null) throw new ArgumentNullException("logger");

            // Sanity check - should be impossible to get reference to _defaultMemoryLogger anyway.
            if (ReferenceEquals(logger, _defaultMemoryLogger))
                return false;

            lock (_loggers)
                return _loggers.Remove(logger);
        }

        /// <summary>
        /// Gets the loggers of the <typeparamref name="T">specified type</typeparamref>.
        /// </summary>
        /// <typeparam name="T">The logger type</typeparam>
        /// <returns>All loggers matching the type</returns>
        [NotNull]
        public static IEnumerable<T> GetLoggers<T>()
            where T : ILogger
        {
            lock (_loggers)
                return _loggers.Keys.OfType<T>();
        }

        /// <summary>
        ///   Flushes all outstanding logs asynchronously.
        /// </summary>
        /// <remarks>
        ///   Note: If more logs are added whilst the system is flushing this will continue to block until there are no outstanding logs.
        /// </remarks>
        [NotNull]
        public static Task Flush(CancellationToken token = new CancellationToken())
        {
            return _tickAction.ExecuteAsync(token);
        }

        /// <summary>
        ///   Flushes all outstanding logs asynchronously.
        /// </summary>
        /// <remarks>
        ///   Note: If more logs are added whilst the system is flushing this will continue to block until there are no outstanding logs.
        /// </remarks>
        [NotNull]
        private static async Task DoFlush(CancellationToken token = new CancellationToken())
        {
            DateTime requested = DateTime.UtcNow;

            List<Log> ready;
            using (await _queueLock.LockAsync(token).ConfigureAwait(false))
            {
                ready = new List<Log>(_buffer.Count);
                List<Log> notReady = new List<Log>(_buffer.Count);

                // Grab all the elements in the buffer, as bags are not ordered in anyway we have to scan all available logs.
                while (!_buffer.IsEmpty)
                {
                    // We stop scanning if the token is cancelled.
                    if (token.IsCancellationRequested)
                    {
                        // Put everything back first!
                        foreach (Log rLog in ready)
                            _buffer.Add(rLog);
                        foreach (Log nrLog in notReady)
                            _buffer.Add(nrLog);

                        return;
                    }

                    Log log;
                    if (!_buffer.TryTake(out log)) break;
                    Debug.Assert(log != null);

                    if (log.TimeStamp > requested)
                        notReady.Add(log);
                    else
                        ready.Add(log);
                }

                // Anything that isn't ready yet we stick back in the bag.
                foreach (Log log in notReady)
                    _buffer.Add(log);
            }

            // Check if we have anything ready to log.
            if (ready.Count < 1)
                return;

            // Grab valid loggers
            List<KeyValuePair<ILogger, LoggerInfo>> loggers;
            lock (_loggers)
                loggers = _loggers
                    // ReSharper disable once PossibleNullReferenceException
                    .Where(kvp => (((byte)kvp.Key.ValidLevels) & ((byte)ValidLevels)) > 0)
                    .ToList();

            // Order the logs
            // ReSharper disable once PossibleNullReferenceException
            Log[] orderedLogs = ready.OrderBy(l => l.TimeStamp).ToArray();
            ready.Clear();

            // Last chance to cancel
            if (token.IsCancellationRequested)
            {
                // Put the logs back first
                foreach (Log log in orderedLogs)
                    _buffer.Add(log);

                return;
            }

            // Create tasks
            // Note we don't use the supplied cancellation token as we always write out logs once they're removed from the buffer.
            // ReSharper disable once AssignNullToNotNullAttribute
            await Task.WhenAll(
                loggers.Select(
                    // ReSharper disable once PossibleNullReferenceException
                    kvp => kvp.Value.Lock
                        .LockAsync(CancellationToken.None)
                        .ContinueWith(
                            t =>
                            {
                                Debug.Assert(t != null);
                                try
                                {
                                    Debug.Assert(kvp.Key != null);
                                    kvp.Key.Add(
                                        // ReSharper disable PossibleNullReferenceException
                                        orderedLogs.Where(log => log.Level.IsValid(kvp.Key.ValidLevels)),
                                        // ReSharper restore PossibleNullReferenceException
                                        CancellationToken.None);
                                }
                                finally
                                {
                                    if (t.Result != null)
                                        t.Result.Dispose();
                                }
                            },
                            CancellationToken.None,
                            TaskContinuationOptions.LongRunning,
                            // ReSharper disable AssignNullToNotNullAttribute
                            TaskScheduler.Default)))
                // ReSharper restore AssignNullToNotNullAttribute
                // We do support cancelling the wait though, this doesn't stop the actual writes occurring.
                .WithCancellation(token)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Called when an unhandled exception occurs..
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="UnhandledExceptionEventArgs"/> instance containing the event data.</param>
        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        private static void OnUnhandledException([CanBeNull] object sender, [CanBeNull] UnhandledExceptionEventArgs e)
        {
            Exception exception = e != null ? e.ExceptionObject as Exception : null;
            bool isTerminating = e != null && e.IsTerminating;
            Add(
                new LogContext().Set(_logReservation, IsTerminatingKey, isTerminating),
                exception,
                isTerminating ? LoggingLevel.Critical : LoggingLevel.Error,
                () => Resources.Log_OnUnhandledException);

            if (isTerminating)
                CleanUp();
            else
                _tickAction.ExecuteAsync().Wait();
        }

        /// <summary>
        /// Attempts to clean up the logs on unloading of an app domain.
        /// </summary>
        private static void CleanUp()
        {
            Thread.BeginCriticalRegion();
            Add(LoggingLevel.Notification, () => Resources.Log_Application_Exiting, ApplicationName, ApplicationGuid);

            _tickAction.Change(dueTime: TimeHelpers.InfiniteDuration);
            _tickAction.ExecuteAsync().Wait();
            _tickAction.Dispose();
            ILogger[] loggers = _loggers.Keys.ToArray();
            _loggers.Clear();
            // ReSharper disable once PossibleNullReferenceException
            Parallel.ForEach(loggers, l => l.Dispose());
            Thread.EndCriticalRegion();
            // ReSharper disable once AssignNullToNotNullAttribute
            TraceTextWriter.Default.WriteLine(Resources.Log_Cleanup_Finished);
        }

        #region Add Overloads
        // ReSharper disable ObjectCreationAsStatement

        /// <summary>
        ///   Logs a message at the information <see cref="LoggingLevel">log level</see>.
        /// </summary>
        /// <param name="format">The log message.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        /// <remarks>
        ///   If the information <see cref="LoggingLevel">log level</see> is invalid then the log won't be added.
        /// </remarks>
        [StringFormatMethod("format")]
        public static void Add(
            [LocalizationRequired] [CanBeNull] string format,
            [CanBeNull] params object[] parameters)
        {
            // Add to queue for logging if we are a valid level.
            if (LoggingLevel.Information.IsValid(ValidLevels))
                _buffer.Add(
                    new Log(
                        Translation.DefaultCulture,
                        null,
                        null,
                        LoggingLevel.Information,
                        null,
                        format,
                        null,
                        parameters));
        }

        /// <summary>
        ///   Logs a message at the information <see cref="LoggingLevel">log level</see>.
        /// </summary>
        /// <param name="context">The log context.</param>
        /// <param name="format">The log message.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        /// <remarks>
        ///   If the information <see cref="LoggingLevel">log level</see> is invalid then the log won't be added.
        /// </remarks>
        [StringFormatMethod("format")]
        public static void Add(
            [CanBeNull] LogContext context,
            [LocalizationRequired] [CanBeNull] string format,
            [CanBeNull] params object[] parameters)
        {
            // Add to queue for logging if we are a valid level.
            if (LoggingLevel.Information.IsValid(ValidLevels))
                _buffer.Add(
                    new Log(
                        Translation.DefaultCulture,
                        context,
                        null,
                        LoggingLevel.Information,
                        null,
                        format,
                        null,
                        parameters));
        }

        /// <summary>
        ///   Logs a message at the specified <see cref="LoggingLevel"/>.
        /// </summary>
        /// <param name="format">The log message.</param>
        /// <param name="level">The log level.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        /// <remarks>
        ///   If the log <paramref name="level"/> is invalid then the log won't be added.
        /// </remarks>
        [StringFormatMethod("format")]
        public static void Add(
            LoggingLevel level,
            [LocalizationRequired] [CanBeNull] string format,
            [CanBeNull] params object[] parameters)
        {
            // Add to queue for logging if we are a valid level.
            if (level.IsValid(ValidLevels))
                _buffer.Add(new Log(Translation.DefaultCulture, null, null, level, null, format, null, parameters));
        }

        /// <summary>
        /// Logs a message at the specified <see cref="LoggingLevel" />.
        /// </summary>
        /// <param name="context">The log context.</param>
        /// <param name="level">The log level.</param>
        /// <param name="format">The log message.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        /// <remarks>If the log <paramref name="level" /> is invalid then the log won't be added.</remarks>
        [StringFormatMethod("format")]
        public static void Add(
            [CanBeNull] LogContext context,
            LoggingLevel level,
            [LocalizationRequired] [CanBeNull] string format,
            [CanBeNull] params object[] parameters)
        {
            // Add to queue for logging if we are a valid level.
            if (level.IsValid(ValidLevels))
                _buffer.Add(new Log(Translation.DefaultCulture, context, null, level, null, format, null, parameters));
        }

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="exception"><para>The exception to log.</para>
        /// <para>
        ///   <see cref="LoggingException" />'s add themselves and so this method ignores them.</para></param>
        /// <param name="level"><para>The log level.</para>
        /// <para>By default this uses the error log level.</para></param>
        /// <remarks>If the log <paramref name="level" /> is invalid then the log won't be added.</remarks>
        public static void Add([CanBeNull] Exception exception, LoggingLevel level = LoggingLevel.Error)
        {
            if (level.IsValid(ValidLevels))
                _buffer.Add(new Log(Translation.DefaultCulture, null, exception, level, null, null, null, null));
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
        public static void Add(
            [CanBeNull] LogContext context,
            [CanBeNull] Exception exception,
            LoggingLevel level = LoggingLevel.Error)
        {
            if (level.IsValid(ValidLevels))
                _buffer.Add(new Log(Translation.DefaultCulture, context, exception, level, null, null, null, null));
        }

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="exception"><para>The exception to log.</para>
        ///   <para><see cref="LoggingException" />'s add themselves and so this method ignores them.</para></param>
        /// <param name="level"><para>The log level.</para>
        ///   <para>By default this uses the error log level.</para></param>
        /// <param name="format">The format.</param>
        /// <param name="parameters">The parameters.</param>
        /// <remarks>
        /// If the log <paramref name="level" /> is invalid then the log won't be added.
        /// </remarks>
        [StringFormatMethod("format")]
        public static void Add(
            [CanBeNull] Exception exception,
            LoggingLevel level,
            [LocalizationRequired] [CanBeNull] string format,
            [CanBeNull] params object[] parameters)
        {
            if (level.IsValid(ValidLevels))
                _buffer.Add(new Log(Translation.DefaultCulture, null, exception, level, null, format, null, parameters));
        }

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="context">The log context.</param>
        /// <param name="exception"><para>The exception to log.</para>
        ///   <para><see cref="LoggingException" />'s add themselves and so this method ignores them.</para></param>
        /// <param name="level"><para>The log level.</para>
        ///   <para>By default this uses the error log level.</para></param>
        /// <param name="format">The format.</param>
        /// <param name="parameters">The parameters.</param>
        /// <remarks>
        /// If the log <paramref name="level" /> is invalid then the log won't be added.
        /// </remarks>
        [StringFormatMethod("format")]
        public static void Add(
            [CanBeNull] LogContext context,
            [CanBeNull] Exception exception,
            LoggingLevel level,
            [LocalizationRequired] [CanBeNull] string format,
            [CanBeNull] params object[] parameters)
        {
            if (level.IsValid(ValidLevels))
                _buffer.Add(
                    new Log(Translation.DefaultCulture, context, exception, level, null, format, null, parameters));
        }

        /// <summary>
        /// Logs a message at the information <see cref="LoggingLevel">log level</see>.
        /// </summary>
        /// <param name="resource">The resource expression, e.g. ()=> Resources.Log_Message.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        /// <remarks>If the information <see cref="LoggingLevel">log level</see> is invalid then the log won't be added.</remarks>
        public static void Add(
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
        {
            // Add to queue for logging if we are a valid level.
            if (LoggingLevel.Information.IsValid(ValidLevels))
                _buffer.Add(
                    new Log(
                        Translation.DefaultCulture,
                        null,
                        null,
                        LoggingLevel.Information,
                        null,
                        null,
                        resource,
                        parameters));
        }

        /// <summary>
        ///   Logs a message at the information <see cref="LoggingLevel">log level</see>.
        /// </summary>
        /// <param name="context">The log context.</param>
        /// <param name="resource">The resource expression, e.g. ()=> Resources.Log_Message.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        /// <remarks>
        ///   If the information <see cref="LoggingLevel">log level</see> is invalid then the log won't be added.
        /// </remarks>
        public static void Add(
            [CanBeNull] LogContext context,
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
        {
            // Add to queue for logging if we are a valid level.
            if (LoggingLevel.Information.IsValid(ValidLevels))
                _buffer.Add(
                    new Log(
                        Translation.DefaultCulture,
                        context,
                        null,
                        LoggingLevel.Information,
                        null,
                        null,
                        resource,
                        parameters));
        }

        /// <summary>
        ///   Logs a message at the specified <see cref="LoggingLevel"/>.
        /// </summary>
        /// <param name="resource">The resource expression, e.g. ()=> Resources.Log_Message.</param>
        /// <param name="level">The log level.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        /// <remarks>
        ///   If the log <paramref name="level"/> is invalid then the log won't be added.
        /// </remarks>
        public static void Add(
            LoggingLevel level,
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
        {
            // Add to queue for logging if we are a valid level.
            if (level.IsValid(ValidLevels))
                _buffer.Add(new Log(Translation.DefaultCulture, null, null, level, null, null, resource, parameters));
        }

        /// <summary>
        /// Logs a message at the specified <see cref="LoggingLevel" />.
        /// </summary>
        /// <param name="context">The log context.</param>
        /// <param name="level">The log level.</param>
        /// <param name="resource">The resource expression, e.g. ()=> Resources.Log_Message.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        /// <remarks>If the log <paramref name="level" /> is invalid then the log won't be added.</remarks>
        public static void Add(
            [CanBeNull] LogContext context,
            LoggingLevel level,
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
        {
            // Add to queue for logging if we are a valid level.
            if (level.IsValid(ValidLevels))
                _buffer.Add(new Log(Translation.DefaultCulture, context, null, level, null, null, resource, parameters));
        }

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="exception"><para>The exception to log.</para>
        ///   <para><see cref="LoggingException" />'s add themselves and so this method ignores them.</para></param>
        /// <param name="level"><para>The log level.</para>
        ///   <para>By default this uses the error log level.</para></param>
        /// <param name="resource">The resource expression, e.g. ()=> Resources.Log_Message.</param>
        /// <param name="parameters">The parameters.</param>
        /// <remarks>
        /// If the log <paramref name="level" /> is invalid then the log won't be added.
        /// </remarks>
        public static void Add(
            [CanBeNull] Exception exception,
            LoggingLevel level,
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
        {
            if (level.IsValid(ValidLevels))
                _buffer.Add(
                    new Log(Translation.DefaultCulture, null, exception, level, null, null, resource, parameters));
        }

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="context">The log context.</param>
        /// <param name="exception"><para>The exception to log.</para>
        ///   <para><see cref="LoggingException" />'s add themselves and so this method ignores them.</para></param>
        /// <param name="level"><para>The log level.</para>
        ///   <para>By default this uses the error log level.</para></param>
        /// <param name="resource">The resource expression, e.g. ()=> Resources.Log_Message.</param>
        /// <param name="parameters">The parameters.</param>
        /// <remarks>
        /// If the log <paramref name="level" /> is invalid then the log won't be added.
        /// </remarks>
        public static void Add(
            [CanBeNull] LogContext context,
            [CanBeNull] Exception exception,
            LoggingLevel level,
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
        {
            if (level.IsValid(ValidLevels))
                _buffer.Add(
                    new Log(Translation.DefaultCulture, context, exception, level, null, null, resource, parameters));
        }

        /// <summary>
        /// Logs a message at the information <see cref="LoggingLevel">log level</see>.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="format">The log message.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        /// <remarks>
        /// If the information <see cref="LoggingLevel">log level</see> is invalid then the log won't be added.
        /// </remarks>
        [StringFormatMethod("format")]
        public static void Add(
            [CanBeNull] CultureInfo culture,
            [LocalizationRequired] [CanBeNull] string format,
            [CanBeNull] params object[] parameters)
        {
            // Add to queue for logging if we are a valid level.
            if (LoggingLevel.Information.IsValid(ValidLevels))
                _buffer.Add(new Log(culture, null, null, LoggingLevel.Information, null, format, null, parameters));
        }

        /// <summary>
        /// Logs a message at the information <see cref="LoggingLevel">log level</see>.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="context">The log context.</param>
        /// <param name="format">The log message.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        /// <remarks>
        /// If the information <see cref="LoggingLevel">log level</see> is invalid then the log won't be added.
        /// </remarks>
        [StringFormatMethod("format")]
        public static void Add(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] LogContext context,
            [LocalizationRequired] [CanBeNull] string format,
            [CanBeNull] params object[] parameters)
        {
            // Add to queue for logging if we are a valid level.
            if (LoggingLevel.Information.IsValid(ValidLevels))
                _buffer.Add(new Log(culture, context, null, LoggingLevel.Information, null, format, null, parameters));
        }

        /// <summary>
        /// Logs a message at the specified <see cref="LoggingLevel" />.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="level">The log level.</param>
        /// <param name="format">The log message.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        /// <remarks>
        /// If the log <paramref name="level" /> is invalid then the log won't be added.
        /// </remarks>
        [StringFormatMethod("format")]
        public static void Add(
            [CanBeNull] CultureInfo culture,
            LoggingLevel level,
            [LocalizationRequired] [CanBeNull] string format,
            [CanBeNull] params object[] parameters)
        {
            // Add to queue for logging if we are a valid level.
            if (level.IsValid(ValidLevels))
                _buffer.Add(new Log(culture, null, null, level, null, format, null, parameters));
        }

        /// <summary>
        /// Logs a message at the specified <see cref="LoggingLevel" />.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="context">The log context.</param>
        /// <param name="level">The log level.</param>
        /// <param name="format">The log message.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        /// <remarks>
        /// If the log <paramref name="level" /> is invalid then the log won't be added.
        /// </remarks>
        [StringFormatMethod("format")]
        public static void Add(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] LogContext context,
            LoggingLevel level,
            [LocalizationRequired] [CanBeNull] string format,
            [CanBeNull] params object[] parameters)
        {
            // Add to queue for logging if we are a valid level.
            if (level.IsValid(ValidLevels))
                _buffer.Add(new Log(culture, context, null, level, null, format, null, parameters));
        }

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="exception"><para>The exception to log.</para>
        ///   <para>
        ///   <see cref="LoggingException" />'s add themselves and so this method ignores them.</para></param>
        /// <param name="level"><para>The log level.</para>
        ///   <para>By default this uses the error log level.</para></param>
        /// <remarks>
        /// If the log <paramref name="level" /> is invalid then the log won't be added.
        /// </remarks>
        public static void Add(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] Exception exception,
            LoggingLevel level = LoggingLevel.Error)
        {
            if (level.IsValid(ValidLevels))
                _buffer.Add(new Log(culture, null, exception, level, null, null, null, null));
        }

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="context">The log context.</param>
        /// <param name="exception"><para>The exception to log.</para>
        ///   <para><see cref="LoggingException" />'s add themselves and so this method ignores them.</para></param>
        /// <param name="level"><para>The log level.</para>
        ///   <para>By default this uses the error log level.</para></param>
        /// <remarks>
        /// If the log <paramref name="level" /> is invalid then the log won't be added.
        /// </remarks>
        public static void Add(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] LogContext context,
            [CanBeNull] Exception exception,
            LoggingLevel level = LoggingLevel.Error)
        {
            if (level.IsValid(ValidLevels))
                _buffer.Add(new Log(culture, context, exception, level, null, null, null, null));
        }

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="exception"><para>The exception to log.</para>
        ///   <para><see cref="LoggingException" />'s add themselves and so this method ignores them.</para></param>
        /// <param name="level"><para>The log level.</para>
        ///   <para>By default this uses the error log level.</para></param>
        /// <param name="format">The format.</param>
        /// <param name="parameters">The parameters.</param>
        /// <remarks>
        /// If the log <paramref name="level" /> is invalid then the log won't be added.
        /// </remarks>
        [StringFormatMethod("format")]
        public static void Add(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] Exception exception,
            LoggingLevel level,
            [LocalizationRequired] [CanBeNull] string format,
            [CanBeNull] params object[] parameters)
        {
            if (level.IsValid(ValidLevels))
                _buffer.Add(new Log(culture, null, exception, level, null, format, null, parameters));
        }

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="context">The log context.</param>
        /// <param name="exception"><para>The exception to log.</para>
        ///   <para><see cref="LoggingException" />'s add themselves and so this method ignores them.</para></param>
        /// <param name="level"><para>The log level.</para>
        ///   <para>By default this uses the error log level.</para></param>
        /// <param name="format">The format.</param>
        /// <param name="parameters">The parameters.</param>
        /// <remarks>
        /// If the log <paramref name="level" /> is invalid then the log won't be added.
        /// </remarks>
        [StringFormatMethod("format")]
        public static void Add(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] LogContext context,
            [CanBeNull] Exception exception,
            LoggingLevel level,
            [LocalizationRequired] [CanBeNull] string format,
            [CanBeNull] params object[] parameters)
        {
            if (level.IsValid(ValidLevels))
                _buffer.Add(new Log(culture, context, exception, level, null, format, null, parameters));
        }

        /// <summary>
        /// Logs a message at the information <see cref="LoggingLevel">log level</see>.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="resource">The resource expression, e.g. ()=&gt; Resources.Log_Message.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        /// <remarks>
        /// If the information <see cref="LoggingLevel">log level</see> is invalid then the log won't be added.
        /// </remarks>
        public static void Add(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
        {
            // Add to queue for logging if we are a valid level.
            if (LoggingLevel.Information.IsValid(ValidLevels))
                _buffer.Add(new Log(culture, null, null, LoggingLevel.Information, null, null, resource, parameters));
        }

        /// <summary>
        /// Logs a message at the information <see cref="LoggingLevel">log level</see>.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="context">The log context.</param>
        /// <param name="resource">The resource expression, e.g. ()=&gt; Resources.Log_Message.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        /// <remarks>
        /// If the information <see cref="LoggingLevel">log level</see> is invalid then the log won't be added.
        /// </remarks>
        public static void Add(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] LogContext context,
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
        {
            // Add to queue for logging if we are a valid level.
            if (LoggingLevel.Information.IsValid(ValidLevels))
                _buffer.Add(new Log(culture, context, null, LoggingLevel.Information, null, null, resource, parameters));
        }

        /// <summary>
        /// Logs a message at the specified <see cref="LoggingLevel" />.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="level">The log level.</param>
        /// <param name="resource">The resource expression, e.g. ()=&gt; Resources.Log_Message.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        /// <remarks>
        /// If the log <paramref name="level" /> is invalid then the log won't be added.
        /// </remarks>
        public static void Add(
            [CanBeNull] CultureInfo culture,
            LoggingLevel level,
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
        {
            // Add to queue for logging if we are a valid level.
            if (level.IsValid(ValidLevels))
                _buffer.Add(new Log(culture, null, null, level, null, null, resource, parameters));
        }

        /// <summary>
        /// Logs a message at the specified <see cref="LoggingLevel" />.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="context">The log context.</param>
        /// <param name="level">The log level.</param>
        /// <param name="resource">The resource expression, e.g. ()=&gt; Resources.Log_Message.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        /// <remarks>
        /// If the log <paramref name="level" /> is invalid then the log won't be added.
        /// </remarks>
        public static void Add(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] LogContext context,
            LoggingLevel level,
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
        {
            // Add to queue for logging if we are a valid level.
            if (level.IsValid(ValidLevels))
                _buffer.Add(new Log(culture, context, null, level, null, null, resource, parameters));
        }

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="exception"><para>The exception to log.</para>
        ///   <para><see cref="LoggingException" />'s add themselves and so this method ignores them.</para></param>
        /// <param name="level"><para>The log level.</para>
        ///   <para>By default this uses the error log level.</para></param>
        /// <param name="resource">The resource expression, e.g. ()=&gt; Resources.Log_Message.</param>
        /// <param name="parameters">The parameters.</param>
        /// <remarks>
        /// If the log <paramref name="level" /> is invalid then the log won't be added.
        /// </remarks>
        public static void Add(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] Exception exception,
            LoggingLevel level,
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
        {
            if (level.IsValid(ValidLevels))
                _buffer.Add(new Log(culture, null, exception, level, null, null, resource, parameters));
        }

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="context">The log context.</param>
        /// <param name="exception"><para>The exception to log.</para>
        ///   <para><see cref="LoggingException" />'s add themselves and so this method ignores them.</para></param>
        /// <param name="level"><para>The log level.</para>
        ///   <para>By default this uses the error log level.</para></param>
        /// <param name="resource">The resource expression, e.g. ()=&gt; Resources.Log_Message.</param>
        /// <param name="parameters">The parameters.</param>
        /// <remarks>
        /// If the log <paramref name="level" /> is invalid then the log won't be added.
        /// </remarks>
        public static void Add(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] LogContext context,
            [CanBeNull] Exception exception,
            LoggingLevel level,
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
        {
            if (level.IsValid(ValidLevels))
                _buffer.Add(new Log(culture, context, exception, level, null, null, resource, parameters));
        }

        /// <summary>
        /// Logs a message at the information <see cref="LoggingLevel">log level</see>.
        /// </summary>
        /// <param name="resourceType">The resource class type.</param>
        /// <param name="resourceProperty">The name of the resource property in <paramref name="resourceType"/>.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        /// <remarks>If the information <see cref="LoggingLevel">log level</see> is invalid then the log won't be added.</remarks>
        public static void Add(
            [CanBeNull] Type resourceType,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
        {
            // Add to queue for logging if we are a valid level.
            if (LoggingLevel.Information.IsValid(ValidLevels))
                _buffer.Add(
                    new Log(
                        Translation.DefaultCulture,
                        null,
                        null,
                        LoggingLevel.Information,
                        resourceType,
                        resourceProperty,
                        null,
                        parameters));
        }

        /// <summary>
        ///   Logs a message at the information <see cref="LoggingLevel">log level</see>.
        /// </summary>
        /// <param name="context">The log context.</param>
        /// <param name="resourceType">The resource class type.</param>
        /// <param name="resourceProperty">The name of the resource property in <paramref name="resourceType"/>.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        /// <remarks>
        ///   If the information <see cref="LoggingLevel">log level</see> is invalid then the log won't be added.
        /// </remarks>
        public static void Add(
            [CanBeNull] LogContext context,
            [CanBeNull] Type resourceType,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
        {
            // Add to queue for logging if we are a valid level.
            if (LoggingLevel.Information.IsValid(ValidLevels))
                _buffer.Add(
                    new Log(
                        Translation.DefaultCulture,
                        context,
                        null,
                        LoggingLevel.Information,
                        resourceType,
                        resourceProperty,
                        null,
                        parameters));
        }

        /// <summary>
        ///   Logs a message at the specified <see cref="LoggingLevel"/>.
        /// </summary>
        /// <param name="resourceType">The resource class type.</param>
        /// <param name="resourceProperty">The name of the resource property in <paramref name="resourceType"/>.</param>
        /// <param name="level">The log level.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        /// <remarks>
        ///   If the log <paramref name="level"/> is invalid then the log won't be added.
        /// </remarks>
        public static void Add(
            LoggingLevel level,
            [CanBeNull] Type resourceType,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
        {
            // Add to queue for logging if we are a valid level.
            if (level.IsValid(ValidLevels))
                _buffer.Add(
                    new Log(
                        Translation.DefaultCulture,
                        null,
                        null,
                        level,
                        resourceType,
                        resourceProperty,
                        null,
                        parameters));
        }

        /// <summary>
        /// Logs a message at the specified <see cref="LoggingLevel" />.
        /// </summary>
        /// <param name="context">The log context.</param>
        /// <param name="level">The log level.</param>
        /// <param name="resourceType">The resource class type.</param>
        /// <param name="resourceProperty">The name of the resource property in <paramref name="resourceType" />.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        /// <remarks>
        /// If the log <paramref name="level" /> is invalid then the log won't be added.
        /// </remarks>
        public static void Add(
            [CanBeNull] LogContext context,
            LoggingLevel level,
            [CanBeNull] Type resourceType,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
        {
            // Add to queue for logging if we are a valid level.
            if (level.IsValid(ValidLevels))
                _buffer.Add(
                    new Log(
                        Translation.DefaultCulture,
                        context,
                        null,
                        level,
                        resourceType,
                        resourceProperty,
                        null,
                        parameters));
        }

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="exception"><para>The exception to log.</para>
        ///   <para><see cref="LoggingException" />'s add themselves and so this method ignores them.</para></param>
        /// <param name="level"><para>The log level.</para>
        ///   <para>By default this uses the error log level.</para></param>
        /// <param name="resourceType">The resource class type.</param>
        /// <param name="resourceProperty">The name of the resource property in <paramref name="resourceType"/>.</param>
        /// <param name="parameters">The parameters.</param>
        /// <remarks>
        /// If the log <paramref name="level" /> is invalid then the log won't be added.
        /// </remarks>
        public static void Add(
            [CanBeNull] Exception exception,
            LoggingLevel level,
            [CanBeNull] Type resourceType,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
        {
            if (level.IsValid(ValidLevels))
                _buffer.Add(
                    new Log(
                        Translation.DefaultCulture,
                        null,
                        exception,
                        level,
                        resourceType,
                        resourceProperty,
                        null,
                        parameters));
        }

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="context">The log context.</param>
        /// <param name="exception"><para>The exception to log.</para>
        ///   <para><see cref="LoggingException" />'s add themselves and so this method ignores them.</para></param>
        /// <param name="level"><para>The log level.</para>
        ///   <para>By default this uses the error log level.</para></param>
        /// <param name="resourceType">The resource class type.</param>
        /// <param name="resourceProperty">The name of the resource property in <paramref name="resourceType"/>.</param>
        /// <param name="parameters">The parameters.</param>
        /// <remarks>
        /// If the log <paramref name="level" /> is invalid then the log won't be added.
        /// </remarks>
        public static void Add(
            [CanBeNull] LogContext context,
            [CanBeNull] Exception exception,
            LoggingLevel level,
            [CanBeNull] Type resourceType,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
        {
            if (level.IsValid(ValidLevels))
                _buffer.Add(
                    new Log(
                        Translation.DefaultCulture,
                        context,
                        exception,
                        level,
                        resourceType,
                        resourceProperty,
                        null,
                        parameters));
        }

        /// <summary>
        /// Logs a message at the information <see cref="LoggingLevel">log level</see>.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="resourceType">The resource class type.</param>
        /// <param name="resourceProperty">The name of the resource property in <paramref name="resourceType"/>.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        /// <remarks>
        /// If the information <see cref="LoggingLevel">log level</see> is invalid then the log won't be added.
        /// </remarks>
        public static void Add(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] Type resourceType,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
        {
            // Add to queue for logging if we are a valid level.
            if (LoggingLevel.Information.IsValid(ValidLevels))
                _buffer.Add(
                    new Log(
                        culture,
                        null,
                        null,
                        LoggingLevel.Information,
                        resourceType,
                        resourceProperty,
                        null,
                        parameters));
        }

        /// <summary>
        /// Logs a message at the information <see cref="LoggingLevel">log level</see>.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="context">The log context.</param>
        /// <param name="resourceType">The resource class type.</param>
        /// <param name="resourceProperty">The name of the resource property in <paramref name="resourceType"/>.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        /// <remarks>
        /// If the information <see cref="LoggingLevel">log level</see> is invalid then the log won't be added.
        /// </remarks>
        public static void Add(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] LogContext context,
            [CanBeNull] Type resourceType,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
        {
            // Add to queue for logging if we are a valid level.
            if (LoggingLevel.Information.IsValid(ValidLevels))
                _buffer.Add(
                    new Log(
                        culture,
                        context,
                        null,
                        LoggingLevel.Information,
                        resourceType,
                        resourceProperty,
                        null,
                        parameters));
        }

        /// <summary>
        /// Logs a message at the specified <see cref="LoggingLevel" />.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="level">The log level.</param>
        /// <param name="resourceType">The resource class type.</param>
        /// <param name="resourceProperty">The name of the resource property in <paramref name="resourceType"/>.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        /// <remarks>
        /// If the log <paramref name="level" /> is invalid then the log won't be added.
        /// </remarks>
        public static void Add(
            [CanBeNull] CultureInfo culture,
            LoggingLevel level,
            [CanBeNull] Type resourceType,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
        {
            // Add to queue for logging if we are a valid level.
            if (level.IsValid(ValidLevels))
                _buffer.Add(new Log(culture, null, null, level, resourceType, resourceProperty, null, parameters));
        }

        /// <summary>
        /// Logs a message at the specified <see cref="LoggingLevel" />.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="context">The log context.</param>
        /// <param name="level">The log level.</param>
        /// <param name="resourceType">The resource class type.</param>
        /// <param name="resourceProperty">The name of the resource property in <paramref name="resourceType"/>.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        /// <remarks>
        /// If the log <paramref name="level" /> is invalid then the log won't be added.
        /// </remarks>
        public static void Add(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] LogContext context,
            LoggingLevel level,
            [CanBeNull] Type resourceType,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
        {
            // Add to queue for logging if we are a valid level.
            if (level.IsValid(ValidLevels))
                _buffer.Add(new Log(culture, context, null, level, resourceType, resourceProperty, null, parameters));
        }

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="exception"><para>The exception to log.</para>
        ///   <para><see cref="LoggingException" />'s add themselves and so this method ignores them.</para></param>
        /// <param name="level"><para>The log level.</para>
        ///   <para>By default this uses the error log level.</para></param>
        /// <param name="resourceType">The resource class type.</param>
        /// <param name="resourceProperty">The name of the resource property in <paramref name="resourceType"/>.</param>
        /// <param name="parameters">The parameters.</param>
        /// <remarks>
        /// If the log <paramref name="level" /> is invalid then the log won't be added.
        /// </remarks>
        public static void Add(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] Exception exception,
            LoggingLevel level,
            [CanBeNull] Type resourceType,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
        {
            if (level.IsValid(ValidLevels))
                _buffer.Add(new Log(culture, null, exception, level, resourceType, resourceProperty, null, parameters));
        }

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="context">The log context.</param>
        /// <param name="exception"><para>The exception to log.</para>
        ///   <para><see cref="LoggingException" />'s add themselves and so this method ignores them.</para></param>
        /// <param name="level"><para>The log level.</para>
        ///   <para>By default this uses the error log level.</para></param>
        /// <param name="resourceType">The resource class type.</param>
        /// <param name="resourceProperty">The name of the resource property in <paramref name="resourceType"/>.</param>
        /// <param name="parameters">The parameters.</param>
        /// <remarks>
        /// If the log <paramref name="level" /> is invalid then the log won't be added.
        /// </remarks>
        public static void Add(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] LogContext context,
            [CanBeNull] Exception exception,
            LoggingLevel level,
            [CanBeNull] Type resourceType,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
        {
            if (level.IsValid(ValidLevels))
                _buffer.Add(
                    new Log(culture, context, exception, level, resourceType, resourceProperty, null, parameters));
        }

        /// <summary>
        /// Logs a message at the information <see cref="LoggingLevel">log level</see>.
        /// </summary>
        /// <typeparam name="TResource">The type of the resource.</typeparam>
        /// <param name="resourceProperty">The name of the resource property in <typeparamref name="TResource" />.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        /// <remarks>
        /// If the information <see cref="LoggingLevel">log level</see> is invalid then the log won't be added.
        /// </remarks>
        public static void Add<TResource>(
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
            where TResource : class
        {
            // Add to queue for logging if we are a valid level.
            if (LoggingLevel.Information.IsValid(ValidLevels))
                _buffer.Add(
                    new Log(
                        Translation.DefaultCulture,
                        null,
                        null,
                        LoggingLevel.Information,
                        typeof(TResource),
                        resourceProperty,
                        null,
                        parameters));
        }

        /// <summary>
        /// Logs a message at the information <see cref="LoggingLevel">log level</see>.
        /// </summary>
        /// <typeparam name="TResource">The type of the resource.</typeparam>
        /// <param name="context">The log context.</param>
        /// <param name="resourceProperty">The name of the resource property in <typeparamref name="TResource" />.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        /// <remarks>
        /// If the information <see cref="LoggingLevel">log level</see> is invalid then the log won't be added.
        /// </remarks>
        public static void Add<TResource>(
            [CanBeNull] LogContext context,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
            where TResource : class
        {
            // Add to queue for logging if we are a valid level.
            if (LoggingLevel.Information.IsValid(ValidLevels))
                _buffer.Add(
                    new Log(
                        Translation.DefaultCulture,
                        context,
                        null,
                        LoggingLevel.Information,
                        typeof(TResource),
                        resourceProperty,
                        null,
                        parameters));
        }

        /// <summary>
        /// Logs a message at the specified <see cref="LoggingLevel" />.
        /// </summary>
        /// <typeparam name="TResource">The type of the resource.</typeparam>
        /// <param name="level">The log level.</param>
        /// <param name="resourceProperty">The name of the resource property in <typeparamref name="TResource" />.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        /// <remarks>
        /// If the log <paramref name="level" /> is invalid then the log won't be added.
        /// </remarks>
        public static void Add<TResource>(
            LoggingLevel level,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
            where TResource : class
        {
            // Add to queue for logging if we are a valid level.
            if (level.IsValid(ValidLevels))
                _buffer.Add(
                    new Log(
                        Translation.DefaultCulture,
                        null,
                        null,
                        level,
                        typeof(TResource),
                        resourceProperty,
                        null,
                        parameters));
        }

        /// <summary>
        /// Logs a message at the specified <see cref="LoggingLevel" />.
        /// </summary>
        /// <typeparam name="TResource">The type of the resource.</typeparam>
        /// <param name="context">The log context.</param>
        /// <param name="level">The log level.</param>
        /// <param name="resourceProperty">The name of the resource property in <typeparamref name="TResource" />.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        /// <remarks>
        /// If the log <paramref name="level" /> is invalid then the log won't be added.
        /// </remarks>
        public static void Add<TResource>(
            [CanBeNull] LogContext context,
            LoggingLevel level,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
            where TResource : class
        {
            // Add to queue for logging if we are a valid level.
            if (level.IsValid(ValidLevels))
                _buffer.Add(
                    new Log(
                        Translation.DefaultCulture,
                        context,
                        null,
                        level,
                        typeof(TResource),
                        resourceProperty,
                        null,
                        parameters));
        }

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <typeparam name="TResource">The type of the resource.</typeparam>
        /// <param name="exception"><para>The exception to log.</para>
        ///   <para><see cref="LoggingException" />'s add themselves and so this method ignores them.</para></param>
        /// <param name="level"><para>The log level.</para>
        ///   <para>By default this uses the error log level.</para></param>
        /// <param name="resourceProperty">The name of the resource property in <typeparamref name="TResource" />.</param>
        /// <param name="parameters">The parameters.</param>
        /// <remarks>
        /// If the log <paramref name="level" /> is invalid then the log won't be added.
        /// </remarks>
        public static void Add<TResource>(
            [CanBeNull] Exception exception,
            LoggingLevel level,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
            where TResource : class
        {
            if (level.IsValid(ValidLevels))
                _buffer.Add(
                    new Log(
                        Translation.DefaultCulture,
                        null,
                        exception,
                        level,
                        typeof(TResource),
                        resourceProperty,
                        null,
                        parameters));
        }

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <typeparam name="TResource">The type of the resource.</typeparam>
        /// <param name="context">The log context.</param>
        /// <param name="exception"><para>The exception to log.</para>
        ///   <para><see cref="LoggingException" />'s add themselves and so this method ignores them.</para></param>
        /// <param name="level"><para>The log level.</para>
        ///   <para>By default this uses the error log level.</para></param>
        /// <param name="resourceProperty">The name of the resource property in <typeparamref name="TResource" />.</param>
        /// <param name="parameters">The parameters.</param>
        /// <remarks>
        /// If the log <paramref name="level" /> is invalid then the log won't be added.
        /// </remarks>
        public static void Add<TResource>(
            [CanBeNull] LogContext context,
            [CanBeNull] Exception exception,
            LoggingLevel level,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
            where TResource : class
        {
            if (level.IsValid(ValidLevels))
                _buffer.Add(
                    new Log(
                        Translation.DefaultCulture,
                        context,
                        exception,
                        level,
                        typeof(TResource),
                        resourceProperty,
                        null,
                        parameters));
        }

        /// <summary>
        /// Logs a message at the information <see cref="LoggingLevel">log level</see>.
        /// </summary>
        /// <typeparam name="TResource">The type of the resource.</typeparam>
        /// <param name="culture">The culture.</param>
        /// <param name="resourceProperty">The name of the resource property in <typeparamref name="TResource" />.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        /// <remarks>
        /// If the information <see cref="LoggingLevel">log level</see> is invalid then the log won't be added.
        /// </remarks>
        public static void Add<TResource>(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
            where TResource : class
        {
            // Add to queue for logging if we are a valid level.
            if (LoggingLevel.Information.IsValid(ValidLevels))
                _buffer.Add(
                    new Log(
                        culture,
                        null,
                        null,
                        LoggingLevel.Information,
                        typeof(TResource),
                        resourceProperty,
                        null,
                        parameters));
        }

        /// <summary>
        /// Logs a message at the information <see cref="LoggingLevel">log level</see>.
        /// </summary>
        /// <typeparam name="TResource">The type of the resource.</typeparam>
        /// <param name="culture">The culture.</param>
        /// <param name="context">The log context.</param>
        /// <param name="resourceProperty">The name of the resource property in <typeparamref name="TResource" />.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        /// <remarks>
        /// If the information <see cref="LoggingLevel">log level</see> is invalid then the log won't be added.
        /// </remarks>
        public static void Add<TResource>(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] LogContext context,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
            where TResource : class
        {
            // Add to queue for logging if we are a valid level.
            if (LoggingLevel.Information.IsValid(ValidLevels))
                _buffer.Add(
                    new Log(
                        culture,
                        context,
                        null,
                        LoggingLevel.Information,
                        typeof(TResource),
                        resourceProperty,
                        null,
                        parameters));
        }

        /// <summary>
        /// Logs a message at the specified <see cref="LoggingLevel" />.
        /// </summary>
        /// <typeparam name="TResource">The type of the resource.</typeparam>
        /// <param name="culture">The culture.</param>
        /// <param name="level">The log level.</param>
        /// <param name="resourceProperty">The name of the resource property in <typeparamref name="TResource" />.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        /// <remarks>
        /// If the log <paramref name="level" /> is invalid then the log won't be added.
        /// </remarks>
        public static void Add<TResource>(
            [CanBeNull] CultureInfo culture,
            LoggingLevel level,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
            where TResource : class
        {
            // Add to queue for logging if we are a valid level.
            if (level.IsValid(ValidLevels))
                _buffer.Add(new Log(culture, null, null, level, typeof(TResource), resourceProperty, null, parameters));
        }

        /// <summary>
        /// Logs a message at the specified <see cref="LoggingLevel" />.
        /// </summary>
        /// <typeparam name="TResource">The type of the resource.</typeparam>
        /// <param name="culture">The culture.</param>
        /// <param name="context">The log context.</param>
        /// <param name="level">The log level.</param>
        /// <param name="resourceProperty">The name of the resource property in <typeparamref name="TResource" />.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        /// <remarks>
        /// If the log <paramref name="level" /> is invalid then the log won't be added.
        /// </remarks>
        public static void Add<TResource>(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] LogContext context,
            LoggingLevel level,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
            where TResource : class
        {
            // Add to queue for logging if we are a valid level.
            if (level.IsValid(ValidLevels))
                _buffer.Add(
                    new Log(culture, context, null, level, typeof(TResource), resourceProperty, null, parameters));
        }

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <typeparam name="TResource">The type of the resource.</typeparam>
        /// <param name="culture">The culture.</param>
        /// <param name="exception"><para>The exception to log.</para>
        ///   <para><see cref="LoggingException" />'s add themselves and so this method ignores them.</para></param>
        /// <param name="level"><para>The log level.</para>
        ///   <para>By default this uses the error log level.</para></param>
        /// <param name="resourceProperty">The name of the resource property in <typeparamref name="TResource" />.</param>
        /// <param name="parameters">The parameters.</param>
        /// <remarks>
        /// If the log <paramref name="level" /> is invalid then the log won't be added.
        /// </remarks>
        public static void Add<TResource>(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] Exception exception,
            LoggingLevel level,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
            where TResource : class
        {
            if (level.IsValid(ValidLevels))
                _buffer.Add(
                    new Log(culture, null, exception, level, typeof(TResource), resourceProperty, null, parameters));
        }

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <typeparam name="TResource">The type of the resource class.</typeparam>
        /// <param name="culture">The culture.</param>
        /// <param name="context">The log context.</param>
        /// <param name="exception"><para>The exception to log.</para>
        ///   <para><see cref="LoggingException" />'s add themselves and so this method ignores them.</para></param>
        /// <param name="level"><para>The log level.</para>
        ///   <para>By default this uses the error log level.</para></param>
        /// <param name="resourceProperty">The name of the resource property in <typeparamref name="TResource" />.</param>
        /// <param name="parameters">The parameters.</param>
        /// <remarks>
        /// If the log <paramref name="level" /> is invalid then the log won't be added.
        /// </remarks>
        public static void Add<TResource>(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] LogContext context,
            [CanBeNull] Exception exception,
            LoggingLevel level,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
            where TResource : class
        {
            if (level.IsValid(ValidLevels))
                _buffer.Add(
                    new Log(culture, context, exception, level, typeof(TResource), resourceProperty, null, parameters));
        }

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="exception"><para>The exception to log.</para>
        ///   <para><see cref="LoggingException" />'s add themselves and so this method ignores them.</para></param>
        /// <param name="format">The format.</param>
        /// <param name="parameters">The parameters.</param>
        /// <remarks>
        /// If <see cref="LoggingLevel.Error"/> is an invalid level then the log won't be added.
        /// </remarks>
        [StringFormatMethod("format")]
        public static void Add(
            [CanBeNull] Exception exception,
            [LocalizationRequired] [CanBeNull] string format,
            [CanBeNull] params object[] parameters)
        {
            if (LoggingLevel.Error.IsValid(ValidLevels))
                _buffer.Add(
                    new Log(
                        Translation.DefaultCulture,
                        null,
                        exception,
                        LoggingLevel.Error,
                        null,
                        format,
                        null,
                        parameters));
        }

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="context">The log context.</param>
        /// <param name="exception"><para>The exception to log.</para>
        ///   <para><see cref="LoggingException" />'s add themselves and so this method ignores them.</para></param>
        /// <param name="format">The format.</param>
        /// <param name="parameters">The parameters.</param>
        /// <remarks>
        /// If <see cref="LoggingLevel.Error"/> is an invalid level then the log won't be added.
        /// </remarks>
        [StringFormatMethod("format")]
        public static void Add(
            [CanBeNull] LogContext context,
            [CanBeNull] Exception exception,
            [LocalizationRequired] [CanBeNull] string format,
            [CanBeNull] params object[] parameters)
        {
            if (LoggingLevel.Error.IsValid(ValidLevels))
                _buffer.Add(
                    new Log(
                        Translation.DefaultCulture,
                        context,
                        exception,
                        LoggingLevel.Error,
                        null,
                        format,
                        null,
                        parameters));
        }

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="exception"><para>The exception to log.</para>
        ///   <para><see cref="LoggingException" />'s add themselves and so this method ignores them.</para></param>
        /// <param name="resource">The resource expression, e.g. ()=> Resources.Log_Message.</param>
        /// <param name="parameters">The parameters.</param>
        /// <remarks>
        /// If <see cref="LoggingLevel.Error"/> is an invalid level then the log won't be added.
        /// </remarks>
        public static void Add(
            [CanBeNull] Exception exception,
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
        {
            if (LoggingLevel.Error.IsValid(ValidLevels))
                _buffer.Add(
                    new Log(
                        Translation.DefaultCulture,
                        null,
                        exception,
                        LoggingLevel.Error,
                        null,
                        null,
                        resource,
                        parameters));
        }

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="context">The log context.</param>
        /// <param name="exception"><para>The exception to log.</para>
        ///   <para><see cref="LoggingException" />'s add themselves and so this method ignores them.</para></param>
        /// <param name="resource">The resource expression, e.g. ()=> Resources.Log_Message.</param>
        /// <param name="parameters">The parameters.</param>
        /// <remarks>
        /// If <see cref="LoggingLevel.Error"/> is an invalid level then the log won't be added.
        /// </remarks>
        public static void Add(
            [CanBeNull] LogContext context,
            [CanBeNull] Exception exception,
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
        {
            if (LoggingLevel.Error.IsValid(ValidLevels))
                _buffer.Add(
                    new Log(
                        Translation.DefaultCulture,
                        context,
                        exception,
                        LoggingLevel.Error,
                        null,
                        null,
                        resource,
                        parameters));
        }

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="exception"><para>The exception to log.</para>
        ///   <para><see cref="LoggingException" />'s add themselves and so this method ignores them.</para></param>
        /// <param name="format">The format.</param>
        /// <param name="parameters">The parameters.</param>
        /// <remarks>
        /// If <see cref="LoggingLevel.Error"/> is an invalid level then the log won't be added.
        /// </remarks>
        [StringFormatMethod("format")]
        public static void Add(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] Exception exception,
            [LocalizationRequired] [CanBeNull] string format,
            [CanBeNull] params object[] parameters)
        {
            if (LoggingLevel.Error.IsValid(ValidLevels))
                _buffer.Add(new Log(culture, null, exception, LoggingLevel.Error, null, format, null, parameters));
        }

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="context">The log context.</param>
        /// <param name="exception"><para>The exception to log.</para>
        ///   <para><see cref="LoggingException" />'s add themselves and so this method ignores them.</para></param>
        /// <param name="format">The format.</param>
        /// <param name="parameters">The parameters.</param>
        /// <remarks>
        /// If <see cref="LoggingLevel.Error"/> is an invalid level then the log won't be added.
        /// </remarks>
        [StringFormatMethod("format")]
        public static void Add(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] LogContext context,
            [CanBeNull] Exception exception,
            [LocalizationRequired] [CanBeNull] string format,
            [CanBeNull] params object[] parameters)
        {
            if (LoggingLevel.Error.IsValid(ValidLevels))
                _buffer.Add(new Log(culture, context, exception, LoggingLevel.Error, null, format, null, parameters));
        }

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="exception"><para>The exception to log.</para>
        ///   <para><see cref="LoggingException" />'s add themselves and so this method ignores them.</para></param>
        /// <param name="resource">The resource expression, e.g. ()=&gt; Resources.Log_Message.</param>
        /// <param name="parameters">The parameters.</param>
        /// <remarks>
        /// If <see cref="LoggingLevel.Error"/> is an invalid level then the log won't be added.
        /// </remarks>
        public static void Add(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] Exception exception,
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
        {
            if (LoggingLevel.Error.IsValid(ValidLevels))
                _buffer.Add(new Log(culture, null, exception, LoggingLevel.Error, null, null, resource, parameters));
        }

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="context">The log context.</param>
        /// <param name="exception"><para>The exception to log.</para>
        ///   <para><see cref="LoggingException" />'s add themselves and so this method ignores them.</para></param>
        /// <param name="resource">The resource expression, e.g. ()=&gt; Resources.Log_Message.</param>
        /// <param name="parameters">The parameters.</param>
        /// <remarks>
        /// If <see cref="LoggingLevel.Error"/> is an invalid level then the log won't be added.
        /// </remarks>
        public static void Add(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] LogContext context,
            [CanBeNull] Exception exception,
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
        {
            if (LoggingLevel.Error.IsValid(ValidLevels))
                _buffer.Add(new Log(culture, context, exception, LoggingLevel.Error, null, null, resource, parameters));
        }

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="exception"><para>The exception to log.</para>
        ///   <para><see cref="LoggingException" />'s add themselves and so this method ignores them.</para></param>
        /// <param name="resourceType">The resource class type.</param>
        /// <param name="resourceProperty">The name of the resource property in <paramref name="resourceType"/>.</param>
        /// <param name="parameters">The parameters.</param>
        /// <remarks>
        /// If <see cref="LoggingLevel.Error"/> is an invalid level then the log won't be added.
        /// </remarks>
        public static void Add(
            [CanBeNull] Exception exception,
            [CanBeNull] Type resourceType,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
        {
            if (LoggingLevel.Error.IsValid(ValidLevels))
                _buffer.Add(
                    new Log(
                        Translation.DefaultCulture,
                        null,
                        exception,
                        LoggingLevel.Error,
                        resourceType,
                        resourceProperty,
                        null,
                        parameters));
        }

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="context">The log context.</param>
        /// <param name="exception"><para>The exception to log.</para>
        ///   <para><see cref="LoggingException" />'s add themselves and so this method ignores them.</para></param>
        /// <param name="resourceType">The resource class type.</param>
        /// <param name="resourceProperty">The name of the resource property in <paramref name="resourceType"/>.</param>
        /// <param name="parameters">The parameters.</param>
        /// <remarks>
        /// If <see cref="LoggingLevel.Error"/> is an invalid level then the log won't be added.
        /// </remarks>
        public static void Add(
            [CanBeNull] LogContext context,
            [CanBeNull] Exception exception,
            [CanBeNull] Type resourceType,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
        {
            if (LoggingLevel.Error.IsValid(ValidLevels))
                _buffer.Add(
                    new Log(
                        Translation.DefaultCulture,
                        context,
                        exception,
                        LoggingLevel.Error,
                        resourceType,
                        resourceProperty,
                        null,
                        parameters));
        }

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="exception"><para>The exception to log.</para>
        ///   <para><see cref="LoggingException" />'s add themselves and so this method ignores them.</para></param>
        /// <param name="resourceType">The resource class type.</param>
        /// <param name="resourceProperty">The name of the resource property in <paramref name="resourceType"/>.</param>
        /// <param name="parameters">The parameters.</param>
        /// <remarks>
        /// If <see cref="LoggingLevel.Error"/> is an invalid level then the log won't be added.
        /// </remarks>
        public static void Add(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] Exception exception,
            [CanBeNull] Type resourceType,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
        {
            if (LoggingLevel.Error.IsValid(ValidLevels))
                _buffer.Add(
                    new Log(
                        culture,
                        null,
                        exception,
                        LoggingLevel.Error,
                        resourceType,
                        resourceProperty,
                        null,
                        parameters));
        }

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="context">The log context.</param>
        /// <param name="exception"><para>The exception to log.</para>
        ///   <para><see cref="LoggingException" />'s add themselves and so this method ignores them.</para></param>
        /// <param name="resourceType">The resource class type.</param>
        /// <param name="resourceProperty">The name of the resource property in <paramref name="resourceType"/>.</param>
        /// <param name="parameters">The parameters.</param>
        /// <remarks>
        /// If <see cref="LoggingLevel.Error"/> is an invalid level then the log won't be added.
        /// </remarks>
        public static void Add(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] LogContext context,
            [CanBeNull] Exception exception,
            [CanBeNull] Type resourceType,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
        {
            if (LoggingLevel.Error.IsValid(ValidLevels))
                _buffer.Add(
                    new Log(
                        culture,
                        context,
                        exception,
                        LoggingLevel.Error,
                        resourceType,
                        resourceProperty,
                        null,
                        parameters));
        }

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <typeparam name="TResource">The type of the resource.</typeparam>
        /// <param name="exception"><para>The exception to log.</para>
        ///   <para><see cref="LoggingException" />'s add themselves and so this method ignores them.</para></param>
        /// <param name="resourceProperty">The name of the resource property in <typeparamref name="TResource" />.</param>
        /// <param name="parameters">The parameters.</param>
        /// <remarks>
        /// If <see cref="LoggingLevel.Error"/> is an invalid level then the log won't be added.
        /// </remarks>
        public static void Add<TResource>(
            [CanBeNull] Exception exception,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
            where TResource : class
        {
            if (LoggingLevel.Error.IsValid(ValidLevels))
                _buffer.Add(
                    new Log(
                        Translation.DefaultCulture,
                        null,
                        exception,
                        LoggingLevel.Error,
                        typeof(TResource),
                        resourceProperty,
                        null,
                        parameters));
        }

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <typeparam name="TResource">The type of the resource.</typeparam>
        /// <param name="context">The log context.</param>
        /// <param name="exception"><para>The exception to log.</para>
        ///   <para><see cref="LoggingException" />'s add themselves and so this method ignores them.</para></param>
        /// <param name="resourceProperty">The name of the resource property in <typeparamref name="TResource" />.</param>
        /// <param name="parameters">The parameters.</param>
        /// <remarks>
        /// If <see cref="LoggingLevel.Error"/> is an invalid level then the log won't be added.
        /// </remarks>
        public static void Add<TResource>(
            [CanBeNull] LogContext context,
            [CanBeNull] Exception exception,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
            where TResource : class
        {
            if (LoggingLevel.Error.IsValid(ValidLevels))
                _buffer.Add(
                    new Log(
                        Translation.DefaultCulture,
                        context,
                        exception,
                        LoggingLevel.Error,
                        typeof(TResource),
                        resourceProperty,
                        null,
                        parameters));
        }

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <typeparam name="TResource">The type of the resource.</typeparam>
        /// <param name="culture">The culture.</param>
        /// <param name="exception"><para>The exception to log.</para>
        ///   <para><see cref="LoggingException" />'s add themselves and so this method ignores them.</para></param>
        /// <param name="resourceProperty">The name of the resource property in <typeparamref name="TResource" />.</param>
        /// <param name="parameters">The parameters.</param>
        /// <remarks>
        /// If <see cref="LoggingLevel.Error"/> is an invalid level then the log won't be added.
        /// </remarks>
        public static void Add<TResource>(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] Exception exception,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
            where TResource : class
        {
            if (LoggingLevel.Error.IsValid(ValidLevels))
                _buffer.Add(
                    new Log(
                        culture,
                        null,
                        exception,
                        LoggingLevel.Error,
                        typeof(TResource),
                        resourceProperty,
                        null,
                        parameters));
        }

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <typeparam name="TResource">The type of the resource class.</typeparam>
        /// <param name="culture">The culture.</param>
        /// <param name="context">The log context.</param>
        /// <param name="exception"><para>The exception to log.</para>
        ///   <para><see cref="LoggingException" />'s add themselves and so this method ignores them.</para></param>
        /// <param name="resourceProperty">The name of the resource property in <typeparamref name="TResource" />.</param>
        /// <param name="parameters">The parameters.</param>
        /// <remarks>
        /// If <see cref="LoggingLevel.Error"/> is an invalid level then the log won't be added.
        /// </remarks>
        public static void Add<TResource>(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] LogContext context,
            [CanBeNull] Exception exception,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
            where TResource : class
        {
            if (LoggingLevel.Error.IsValid(ValidLevels))
                _buffer.Add(
                    new Log(
                        culture,
                        context,
                        exception,
                        LoggingLevel.Error,
                        typeof(TResource),
                        resourceProperty,
                        null,
                        parameters));
        }

        // ReSharper restore ObjectCreationAsStatement
        #endregion
    }
}