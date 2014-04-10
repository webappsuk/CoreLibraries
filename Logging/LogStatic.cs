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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Security;
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
    public sealed partial class Log
    {
        /// <summary>
        /// The Header/Footer string
        /// </summary>
        [NotNull]
        private const string Header =
            "====================================================================================================";

        /// <summary>
        /// The new log item performance counter.
        /// </summary>
        [NotNull]
        [NonSerialized]
        private static readonly PerfCounter _perfCounterNewItem;

        /// <summary>
        /// Holds all resource types seen by the logger.
        /// </summary>
        [NotNull]
        [NonSerialized]
        private static readonly ConcurrentDictionary<string, ResourceManager> _resourceManagers =
            new ConcurrentDictionary<string, ResourceManager>();

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
                        typeof (int).Assembly.GetName().GetPublicKey(),
                        typeof (Observable).Assembly.GetName().GetPublicKey(),
                        typeof (UtilityExtensions).Assembly.GetName().GetPublicKey()
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

        /// <summary>
        /// An empty string array.
        /// </summary>
        [NotNull]
        [NonSerialized]
        private static readonly string[] _emptyStringArray = new string[0];

        /// <summary>
        /// An empty <see cref="CombGuid"/> array.
        /// </summary>
        [NotNull]
        [NonSerialized]
        private static readonly CombGuid[] _emptyCombGuidArray = new CombGuid[0];

        /// <summary>
        /// Holds lookup for formats.
        /// </summary>
        [NotNull]
        private static readonly Dictionary<string, LogFormat> _formats =
            ExtendedEnum<LogFormat>.ValueDetails
                .SelectMany(
                    vd => vd.Select(v => new KeyValuePair<string, LogFormat>(v.ToLower(), vd.Value)))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value, StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// The log reservation.
        /// </summary>
        [NonSerialized]
        private static readonly Guid _logReservation = System.Guid.NewGuid();

        /// <summary>
        /// Gets or sets the default culture information.
        /// </summary>
        /// <value>
        /// The default culture information.
        /// </value>
        /// <remarks>
        /// <para>
        /// This is the culture that logs will use when added to the system.
        /// </para></remarks>
        [NotNull]
        [PublicAPI]
        public static CultureInfo DefaultCulture
        {
            get { return _defaultCulture; }
            set { _defaultCulture = value ?? CultureInfo.CurrentCulture; }
        }

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

        /// <summary>
        /// The logging assembly
        /// </summary>
        [NotNull]
        [NonSerialized]
        internal static readonly Assembly LoggingAssembly = typeof(Log).Assembly;

        /// <summary>
        /// The global tick, ticks once a second and is used for batching, etc.
        /// </summary>
        [NonSerialized]
        [NotNull]
        public static readonly IObservable<long> Tick;

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
        private static readonly IDisposable _tickSubscription;

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
        /// The default culture information.
        /// </summary>
        [NotNull]
        [NonSerialized]
        private static CultureInfo _defaultCulture = CultureInfo.CurrentCulture;

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

            // Create loggers and add default memory logger.
            _loggers = new Dictionary<ILogger, LoggerInfo>();
            _defaultMemoryLogger = new MemoryLogger("Default memory logger", TimeSpan.FromMinutes(1));
            _loggers[_defaultMemoryLogger] = new LoggerInfo(false);

            // Flush on tick.
            _tickSubscription = Tick.Subscribe(l => Flush());

            ConfigurationSection<LoggingConfiguration>.Changed += (o, e) => LoadConfiguration();

            // Flush logs on domain unload and unhandled exceptions.
            AppDomain.CurrentDomain.DomainUnload += (s, e) => Cleanup();
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            if (PerfCategory.HasAccess)
            {
                Add(LoggingLevel.Notification, () => Resources.Log_PerfCategory_Enabled, PerfCategory.InstanceGuid);

                foreach (PerfCategory pc in PerfCategory.All)
                    if (pc.IsValid)
                        Add(LoggingLevel.Debugging, () => Resources.Log_PerfCategory_Initialized, pc.CategoryName);
                    else
                        Add(LoggingLevel.Warning, () => Resources.Log_PerfCategory_Missing, pc.CategoryName);
            }
            else
                Add(LoggingLevel.Warning, () => Resources.Log_PerfCategory_Access_Denied);
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
        /// Registers the type for resource retrieval.
        /// </summary>
        /// <param name="type">The type.</param>
        [PublicAPI]
        [CanBeNull]
        public static ResourceManager GetResourceManager([NotNull] Type type)
        {
            Contract.Requires(type != null);
            Contract.Requires(type.FullName != null);
            return _resourceManagers.GetOrAdd(
                type.FullName,
                t =>
                {
                    try
                    {
                        PropertyInfo resourceInfo = type.GetProperty(
                            "ResourceManager",
                            BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);

                        return resourceInfo == null
                            ? null
                            : resourceInfo.GetValue(null) as ResourceManager;
                    }
                    catch (Exception e)
                    {
                        Add(e, LoggingLevel.Error, () => Resources.Log_GetResourceManager_FatalError, t);
                        return null;
                    }
                });
        }

        /// <summary>
        /// Gets the resource manager, by the type's full name.
        /// </summary>
        /// <param name="typeFullName">Full name of the type.</param>
        /// <returns>A resource manager, if the type has already been registered.</returns>
        /// <remarks>This will only retrieve <see cref="ResourceManager">resource managers</see> for previously registered types.</remarks>
        [PublicAPI]
        [CanBeNull]
        public static ResourceManager GetResourceManager([NotNull] string typeFullName)
        {
            Contract.Requires(typeFullName != null);
            ResourceManager resourceManager;
            return _resourceManagers.TryGetValue(typeFullName, out resourceManager) ? resourceManager : null;
        }

        /// <summary>
        ///   Gets the valid <see cref="LoggingLevel">log levels</see>.
        /// </summary>
        [PublicAPI]
        public static LoggingLevels ValidLevels { get; set; }

        /// <summary>
        ///   Gets the loggers.
        /// </summary>
        [PublicAPI]
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
            // Ensure we only run one load at a time.
            lock (_loggers)
            {
                // Get the active configuration
                LoggingConfiguration configuration = ConfigurationSection<LoggingConfiguration>.Active;
                Contract.Assert(configuration != null);

                ApplicationName = configuration.ApplicationName;
                ApplicationGuid = configuration.ApplicationGuid;

                // Grab all loggers that came from the configuration.
                // ReSharper disable once PossibleNullReferenceException
                KeyValuePair<ILogger, LoggerInfo>[] loggers =
                    _loggers.Where(kvp => kvp.Value.IsFromConfiguration).ToArray();
                foreach (ILogger l in loggers.Select(l => l.Key))
                {
                    Contract.Assert(l != null);
                    _loggers.Remove(l);
                }

                // Dispose existing configuration loggers
                Parallel.ForEach(
                    loggers,
                    kvp =>
                    {
                        Contract.Assert(kvp.Key != null);
                        Contract.Assert(kvp.Value != null);
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
                    foreach (LoggerElement loggerElement in configuration.Loggers.Where(l => l != null && l.Enabled))
                    {
                        Contract.Assert(loggerElement != null);
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
        /// <exception cref="LoggingException"><see cref="LoggerBase.Queryable">retrieval</see> is not supported.</exception>
        [NotNull]
        [PublicAPI]
        public static ILogger AddLogger(
            [NotNull] ILogger logger,
            [CanBeNull] ILogger sourceLogger = null,
            int limit = Int32.MaxValue)
        {
            Contract.Requires(logger != null);
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
        /// <exception cref="LoggingException"></exception>
        [NotNull]
        [PublicAPI]
        private static ILogger AddLogger(
            [NotNull] ILogger logger,
            [CanBeNull] ILogger sourceLogger,
            int limit,
            bool isFromConfiguration)
        {
            Contract.Requires(logger != null);
            if (sourceLogger == null)
                sourceLogger = _defaultMemoryLogger;

            // Check for source logger can retrieve logs
            if (!sourceLogger.Queryable)
                throw new LoggingException(
                    () => Resources.LogStatic_AddOrUpdateLogger_RetrievalNotSupported,
                    sourceLogger.Name);

            IDisposable loggerLock = null;
            try
            {
                lock (_loggers)
                {
                    if (!logger.AllowMultiple)
                    {
                        Type loggerType = logger.GetType();
                        // Check for existing logger of same type.
                        KeyValuePair<ILogger, LoggerInfo> existing =
                            _loggers.FirstOrDefault(i => loggerType.IsInstanceOfType(i.Key));
                        if (existing.Key != null)
                            return existing.Key;
                    }

                    // Create the logger info
                    LoggerInfo info = new LoggerInfo(false);

                    // Grab the logger lock, this ensures we get to add log entries first!
                    loggerLock = info.Lock.LockAsync().Result;

                    // We can add logger and release _loggers lock.
                    _loggers.Add(logger, info);
                }

                // Add logs, we already have the lock, so we go first.
                logger.Add(sourceLogger.Qbserve.TakeLast(limit).ToEnumerable());
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
        /// <exception cref="LoggingException"><see cref="LoggerBase.Queryable">retrieval</see> is not supported.</exception>
        [NotNull]
        [PublicAPI]
        public static T AddLogger<T>(
            [NotNull] T logger,
            [CanBeNull] ILogger sourceLogger = null,
            int limit = Int32.MaxValue)
            where T : ILogger
        {
            Contract.Requires(logger != null);
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
        /// <exception cref="LoggingException"><see cref="LoggerBase.Queryable">retrieval</see> is not supported.</exception>
        [NotNull]
        [PublicAPI]
        public static T AddLogger<T>(
            [NotNull] Func<T> loggerCreator,
            [CanBeNull] ILogger sourceLogger = null,
            int limit = Int32.MaxValue)
            where T : ILogger
        {
            Contract.Requires(loggerCreator != null);
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
        /// <exception cref="LoggingException">
        /// </exception>
        [NotNull]
        [PublicAPI]
        private static T AddLogger<T>(
            [NotNull] Func<T> loggerCreator,
            [CanBeNull] ILogger sourceLogger,
            int limit,
            bool isFromConfiguration)
            where T : ILogger
        {
            Contract.Requires(loggerCreator != null);
            if (sourceLogger == null)
                sourceLogger = _defaultMemoryLogger;

            // Check for source logger can retrieve logs
            if (!sourceLogger.Queryable)
                throw new LoggingException(
                    () => Resources.LogStatic_AddOrUpdateLogger_RetrievalNotSupported,
                    sourceLogger.Name);

            T logger;
            IDisposable loggerLock = null;
            try
            {
                lock (_loggers)
                {
                    // Check for existing logger of same type.
                    KeyValuePair<ILogger, LoggerInfo> existing =
                        _loggers.FirstOrDefault(i => i.Key is T);
                    if ((existing.Key != null) &&
                        !existing.Key.AllowMultiple)
                        return (T)existing.Key;

                    logger = loggerCreator();
                    if (ReferenceEquals(logger, null))
                        // ReSharper disable once AssignNullToNotNullAttribute
                        return default(T);

                    // Create the logger info
                    LoggerInfo info = new LoggerInfo(false);

                    // Grab the logger lock, this ensures we get to add log entries first!
                    loggerLock = info.Lock.LockAsync().Result;

                    // We can add logger and release _loggers lock.
                    _loggers.Add(logger, info);
                }

                // Add logs, we already have the lock, so we go first.
                logger.Add(sourceLogger.Qbserve.TakeLast(limit).ToEnumerable());
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
        [PublicAPI]
        public static bool RemoveLogger([NotNull] ILogger logger)
        {
            Contract.Requires(logger != null);
            // Sanity check - should be impossible to get reference to _defaultMemoryLoger anyway.
            if (ReferenceEquals(logger, _defaultMemoryLogger))
                return false;

            lock (_loggers)
                return _loggers.Remove(logger);
        }

        /// <summary>
        /// Gets the loggers of the <see typeref="T">specified type</see>.
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
        [PublicAPI]
        [NotNull]
        public static async Task Flush(CancellationToken token = new CancellationToken())
        {
            DateTime requested = DateTime.UtcNow;

            List<Log> ready;
            using (await _queueLock.LockAsync(token))
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
                    .Where(kvp => (((byte)kvp.Key.ValidLevels) & ((byte)ValidLevels)) > 0)
                    .ToList();

            // Order the logs
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
            await Task.WhenAll(
                loggers.Select(
                // ReSharper disable once PossibleNullReferenceException
                    kvp => kvp.Value.Lock
                        .LockAsync(CancellationToken.None)
                        .ContinueWith(
                            t =>
                            {
                                try
                                {
                                    kvp.Key.Add(
                                        orderedLogs.Where(log => log.Level.IsValid(kvp.Key.ValidLevels)),
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
                        // ReSharper disable once AssignNullToNotNullAttribute
                            TaskScheduler.Default)))
                // We do support cancelling the wait though, this doesn't stop the actual writes occurring.
                .WithCancellation(token);
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
            Exception exception = e.ExceptionObject as Exception;
            Add(
                new LogContext().Set(_logReservation, IsTerminatingKey, e.IsTerminating),
                exception,
                e.IsTerminating ? LoggingLevel.Critical : LoggingLevel.Error,
                () => Resources.Log_OnUnhandledException);

            if (e.IsTerminating)
                Cleanup();
            else
                Flush().Wait();
        }

        /// <summary>
        /// Attempts to clean up the logs on unloading of an app domain.
        /// </summary>
        private static void Cleanup()
        {
            Thread.BeginCriticalRegion();
            _tickSubscription.Dispose();
            Add(LoggingLevel.Notification, () => Resources.Log_Application_Exiting, ApplicationName, ApplicationGuid);

            Flush().Wait();
            ILogger[] loggers = _loggers.Keys.ToArray();
            _loggers.Clear();
            Parallel.ForEach(loggers, l => l.Dispose());
            Thread.EndCriticalRegion();
            Trace.WriteLine(Resources.Log_Cleanup_Finished);
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
        [PublicAPI]
        public static void Add(
            [LocalizationRequired] [CanBeNull] string format,
            [CanBeNull] params object[] parameters)
        {
            // Add to queue for logging if we are a valid level.
            if (LoggingLevel.Information.IsValid(ValidLevels))
                new Log(DefaultCulture, null, null, LoggingLevel.Information, format, null, parameters);
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
        [PublicAPI]
        public static void Add(
            [CanBeNull] LogContext context,
            [LocalizationRequired] [CanBeNull] string format,
            [CanBeNull] params object[] parameters)
        {
            // Add to queue for logging if we are a valid level.
            if (LoggingLevel.Information.IsValid(ValidLevels))
                new Log(DefaultCulture, context, null, LoggingLevel.Information, format, null, parameters);
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
        [PublicAPI]
        public static void Add(
            LoggingLevel level,
            [LocalizationRequired] [CanBeNull] string format,
            [CanBeNull] params object[] parameters)
        {
            // Add to queue for logging if we are a valid level.
            if (level.IsValid(ValidLevels))
                new Log(DefaultCulture, null, null, level, format, null, parameters);
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
        [PublicAPI]
        public static void Add(
            [CanBeNull] LogContext context,
            LoggingLevel level,
            [LocalizationRequired] [CanBeNull] string format,
            [CanBeNull] params object[] parameters)
        {
            // Add to queue for logging if we are a valid level.
            if (level.IsValid(ValidLevels))
                new Log(DefaultCulture, context, null, level, format, null, parameters);
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
        [PublicAPI]
        public static void Add([CanBeNull] Exception exception, LoggingLevel level = LoggingLevel.Error)
        {
            if (level.IsValid(ValidLevels))
                new Log(DefaultCulture, null, exception, level, null, null, null);
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
        [PublicAPI]
        public static void Add(
            [CanBeNull] LogContext context,
            [CanBeNull] Exception exception,
            LoggingLevel level = LoggingLevel.Error)
        {
            if (level.IsValid(ValidLevels))
                new Log(DefaultCulture, context, exception, level, null, null, null);
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
        [PublicAPI]
        [StringFormatMethod("format")]
        public static void Add(
            [CanBeNull] Exception exception,
            LoggingLevel level,
            [LocalizationRequired] [CanBeNull] string format,
            [CanBeNull] params object[] parameters)
        {
            if (level.IsValid(ValidLevels))
                new Log(DefaultCulture, null, exception, level, format, null, parameters);
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
        [PublicAPI]
        [StringFormatMethod("format")]
        public static void Add(
            [CanBeNull] LogContext context,
            [CanBeNull] Exception exception,
            LoggingLevel level,
            [LocalizationRequired] [CanBeNull] string format,
            [CanBeNull] params object[] parameters)
        {
            if (level.IsValid(ValidLevels))
                new Log(DefaultCulture, context, exception, level, format, null, parameters);
        }

        /// <summary>
        /// Logs a message at the information <see cref="LoggingLevel">log level</see>.
        /// </summary>
        /// <param name="resource">The resource expression, e.g. ()=> Resources.Log_Message.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        /// <remarks>If the information <see cref="LoggingLevel">log level</see> is invalid then the log won't be added.</remarks>
        [PublicAPI]
        public static void Add(
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
        {
            // Add to queue for logging if we are a valid level.
            if (LoggingLevel.Information.IsValid(ValidLevels))
                new Log(DefaultCulture, null, null, LoggingLevel.Information, null, resource, parameters);
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
        [PublicAPI]
        public static void Add(
            [CanBeNull] LogContext context,
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
        {
            // Add to queue for logging if we are a valid level.
            if (LoggingLevel.Information.IsValid(ValidLevels))
                new Log(DefaultCulture, context, null, LoggingLevel.Information, null, resource, parameters);
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
        [PublicAPI]
        public static void Add(
            LoggingLevel level,
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
        {
            // Add to queue for logging if we are a valid level.
            if (level.IsValid(ValidLevels))
                new Log(DefaultCulture, null, null, level, null, resource, parameters);
        }

        /// <summary>
        /// Logs a message at the specified <see cref="LoggingLevel" />.
        /// </summary>
        /// <param name="context">The log context.</param>
        /// <param name="level">The log level.</param>
        /// <param name="resource">The resource expression, e.g. ()=> Resources.Log_Message.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        /// <remarks>If the log <paramref name="level" /> is invalid then the log won't be added.</remarks>
        [PublicAPI]
        public static void Add(
            [CanBeNull] LogContext context,
            LoggingLevel level,
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
        {
            // Add to queue for logging if we are a valid level.
            if (level.IsValid(ValidLevels))
                new Log(DefaultCulture, context, null, level, null, resource, parameters);
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
        [PublicAPI]
        public static void Add(
            [CanBeNull] Exception exception,
            LoggingLevel level,
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
        {
            if (level.IsValid(ValidLevels))
                new Log(DefaultCulture, null, exception, level, null, resource, parameters);
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
        [PublicAPI]
        public static void Add(
            [CanBeNull] LogContext context,
            [CanBeNull] Exception exception,
            LoggingLevel level,
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
        {
            if (level.IsValid(ValidLevels))
                new Log(DefaultCulture, context, exception, level, null, resource, parameters);
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
        [PublicAPI]
        public static void Add(
            [CanBeNull] CultureInfo culture,
            [LocalizationRequired] [CanBeNull] string format,
            [CanBeNull] params object[] parameters)
        {
            // Add to queue for logging if we are a valid level.
            if (LoggingLevel.Information.IsValid(ValidLevels))
                new Log(culture, null, null, LoggingLevel.Information, format, null, parameters);
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
        [PublicAPI]
        public static void Add(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] LogContext context,
            [LocalizationRequired] [CanBeNull] string format,
            [CanBeNull] params object[] parameters)
        {
            // Add to queue for logging if we are a valid level.
            if (LoggingLevel.Information.IsValid(ValidLevels))
                new Log(culture, context, null, LoggingLevel.Information, format, null, parameters);
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
        [PublicAPI]
        public static void Add(
            [CanBeNull] CultureInfo culture,
            LoggingLevel level,
            [LocalizationRequired] [CanBeNull] string format,
            [CanBeNull] params object[] parameters)
        {
            // Add to queue for logging if we are a valid level.
            if (level.IsValid(ValidLevels))
                new Log(culture, null, null, level, format, null, parameters);
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
        [PublicAPI]
        public static void Add(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] LogContext context,
            LoggingLevel level,
            [LocalizationRequired] [CanBeNull] string format,
            [CanBeNull] params object[] parameters)
        {
            // Add to queue for logging if we are a valid level.
            if (level.IsValid(ValidLevels))
                new Log(culture, context, null, level, format, null, parameters);
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
        [PublicAPI]
        public static void Add([CanBeNull] CultureInfo culture, [CanBeNull] Exception exception, LoggingLevel level = LoggingLevel.Error)
        {
            if (level.IsValid(ValidLevels))
                new Log(culture, null, exception, level, null, null, null);
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
        [PublicAPI]
        public static void Add(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] LogContext context,
            [CanBeNull] Exception exception,
            LoggingLevel level = LoggingLevel.Error)
        {
            if (level.IsValid(ValidLevels))
                new Log(culture, context, exception, level, null, null, null);
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
        [PublicAPI]
        [StringFormatMethod("format")]
        public static void Add(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] Exception exception,
            LoggingLevel level,
            [LocalizationRequired] [CanBeNull] string format,
            [CanBeNull] params object[] parameters)
        {
            if (level.IsValid(ValidLevels))
                new Log(culture, null, exception, level, format, null, parameters);
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
        [PublicAPI]
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
                new Log(culture, context, exception, level, format, null, parameters);
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
        [PublicAPI]
        public static void Add(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
        {
            // Add to queue for logging if we are a valid level.
            if (LoggingLevel.Information.IsValid(ValidLevels))
                new Log(culture, null, null, LoggingLevel.Information, null, resource, parameters);
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
        [PublicAPI]
        public static void Add(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] LogContext context,
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
        {
            // Add to queue for logging if we are a valid level.
            if (LoggingLevel.Information.IsValid(ValidLevels))
                new Log(culture, context, null, LoggingLevel.Information, null, resource, parameters);
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
        [PublicAPI]
        public static void Add(
            [CanBeNull] CultureInfo culture,
            LoggingLevel level,
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
        {
            // Add to queue for logging if we are a valid level.
            if (level.IsValid(ValidLevels))
                new Log(culture, null, null, level, null, resource, parameters);
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
        [PublicAPI]
        public static void Add(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] LogContext context,
            LoggingLevel level,
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
        {
            // Add to queue for logging if we are a valid level.
            if (level.IsValid(ValidLevels))
                new Log(culture, context, null, level, null, resource, parameters);
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
        [PublicAPI]
        public static void Add(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] Exception exception,
            LoggingLevel level,
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
        {
            if (level.IsValid(ValidLevels))
                new Log(culture, null, exception, level, null, resource, parameters);
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
        [PublicAPI]
        public static void Add(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] LogContext context,
            [CanBeNull] Exception exception,
            LoggingLevel level,
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
        {
            if (level.IsValid(ValidLevels))
                new Log(culture, context, exception, level, null, resource, parameters);
        }

        // ReSharper restore ObjectCreationAsStatement
        #endregion
    }
}