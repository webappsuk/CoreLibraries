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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security;
using System.Text;
using System.Threading;
using JetBrains.Annotations;
using ProtoBuf;
using ProtoBuf.Meta;

namespace WebApplications.Utilities.Logging
{
    /// <summary>
    /// Holds information about a single log item.
    /// </summary>
    [ProtoContract(UseProtoMembersOnly = true, IgnoreListHandling = true)]
    [Serializable]
    [DebuggerDisplay("{Message} @ {TimeStamp}")]
    [PublicAPI]
    public sealed partial class Log : IEnumerable<KeyValuePair<string, string>>, IFormattable
    {
        #region Serialized Members
        [CanBeNull]
        [ProtoMember(13)]
        private readonly LogContext _context;

        [CanBeNull]
        [ProtoMember(7)]
        private readonly string _exceptionType;

        [ProtoMember(1, IsRequired = true)] // TODO Allow direct serialization of CombGuid.
        private readonly Guid _guid;

        [CanBeNull]
        [ProtoMember(12)]
        private readonly CombGuid[] _innerExceptionGuids;

        [ProtoMember(2)]
        [DefaultValue(LoggingLevel.Information)]
        private LoggingLevel _level;

        [CanBeNull]
        [ProtoMember(3)]
        private readonly string _messageFormat;

        [CanBeNull]
        [ProtoMember(11)]
        private readonly string[] _parameters;

        [CanBeNull]
        [ProtoMember(6)]
        private readonly string _stackTrace;

        [CanBeNull]
        [ProtoMember(8)]
        private readonly string _storedProcedure;

        [ProtoMember(9)]
        [DefaultValue(0)]
        private readonly int _storedProcedureLine;

        [CanBeNull]
        [ProtoMember(10)]
        private readonly string _resourceProperty;

        [ProtoMember(4)]
        [DefaultValue(0)]
        private readonly int _threadID;

        [CanBeNull]
        [ProtoMember(5)]
        private readonly string _threadName;
        #endregion

        #region Message production
        [NotNull]
        [NonSerialized]
        private object _lock = new object();

        [CanBeNull]
        [NonSerialized]
        private ResourceManager _resourceManager;

        [CanBeNull]
        [NonSerialized]
        private string _tag;

        [CanBeNull]
        [NonSerialized]
        private CultureInfo _latestCultureInfo;

        [CanBeNull]
        [NonSerialized]
        private string _latestMessageFormat;

        [CanBeNull]
        [NonSerialized]
        private string _latestMessage;
        #endregion

        /// <summary>
        /// Called when deserializing.
        /// </summary>
        /// <param name="context">The context.</param>
        [OnDeserializing]
        public void OnDeserializing(StreamingContext context)
        {
            _level = LoggingLevel.Information;
            _lock = new object();
        }

        /// <summary>
        /// Gets the protobuf schema on request.
        /// </summary>
        [NotNull]
        [NonSerialized]
        private static readonly Lazy<string> _protobufSchema = new Lazy<string>(
            // ReSharper disable once PossibleNullReferenceException
            () => RuntimeTypeModel.Default.GetSchema(typeof(Log)),
            LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// Initializes a new instance of the <see cref="Log"/> class.  Used during deserialization.
        /// </summary>
        private Log()
        {
            OnDeserializing(default(StreamingContext));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Log"/> class.
        /// </summary>
        /// <param name="dictionary">The key value pairs.</param>
        /// <remarks><para>This is used for deserializing Log entries - it does not result in logs being added!</para>
        /// <para>To add logs use <see cref="Log.Add(string, object[])"/> instead.</para>
        /// <para>You can create partial logs, however the context must contain at least the 
        /// <see cref="GuidKey">Guid key</see>, and be a valid <see cref="CombGuid"/>.</para></remarks>
        public Log([NotNull] [InstantHandle] IEnumerable<KeyValuePair<string, string>> dictionary)
            : this()
        {
            Contract.Requires(dictionary != null);
            Dictionary<string, string> context = new Dictionary<string, string>();
            SortedDictionary<int, string> parameters = new SortedDictionary<int, string>();
            SortedDictionary<int, CombGuid> ieGuids = new SortedDictionary<int, CombGuid>();
            int maxParamIndex = 0;
            int maxIeIndex = 0;

            foreach (KeyValuePair<string, string> kvp in dictionary)
            {
                string key = kvp.Key;
                string value = kvp.Value;
                if (string.IsNullOrEmpty(key)) continue;

                if (string.Equals(key, GuidKey))
                {
                    CombGuid guid;
                    _guid = CombGuid.TryParse(value, out guid) ? guid : CombGuid.Empty;
                    continue;
                }
                if (string.Equals(key, LevelKey))
                {
                    LoggingLevel level;
                    _level = Enum.TryParse(value, out level) ? level : LoggingLevel.Information;
                    continue;
                }
                if (string.Equals(key, MessageFormatKey))
                {
                    _messageFormat = value;
                    continue;
                }
                if (string.Equals(key, ResourcePropertyKey))
                {
                    _resourceProperty = value;
                    continue;
                }
                if (string.Equals(key, ExceptionTypeFullNameKey))
                {
                    _exceptionType = value;
                    continue;
                }
                if (string.Equals(key, StackTraceKey))
                {
                    _stackTrace = value;
                    continue;
                }
                if (string.Equals(key, ThreadIDKey))
                {
                    int threadId;
                    _threadID = int.TryParse(value, out threadId) ? threadId : -1;
                    continue;
                }
                if (string.Equals(key, ThreadNameKey))
                {
                    _threadName = value;
                    continue;
                }
                if (string.Equals(key, StoredProcedureKey))
                {
                    _storedProcedure = value;
                    continue;
                }
                if (string.Equals(key, StoredProcedureLineKey))
                {
                    int spline;
                    _storedProcedureLine = int.TryParse(value, out spline) ? spline : 0;
                    continue;
                }
                if (key.StartsWith(ParameterPrefix))
                {
                    int index;
                    if (!int.TryParse(key.Substring(ParameterPrefix.Length), out index))
                        index = ++maxParamIndex;
                    else if (index > maxParamIndex)
                        maxParamIndex = index;

                    parameters.Add(index, value);
                    continue;
                }
                if (key.StartsWith(InnerExceptionGuidsPrefix))
                {
                    int index;
                    if (!int.TryParse(key.Substring(InnerExceptionGuidsPrefix.Length), out index))
                        index = ++maxIeIndex;
                    else if (index > maxIeIndex)
                        maxIeIndex = index;

                    CombGuid guid;
                    ieGuids.Add(index, CombGuid.TryParse(value, out guid) ? guid : CombGuid.Empty);
                    continue;
                }

                // Add to the context.
                context[key] = kvp.Value;
            }

            if (context.Count > 0)
                _context = new LogContext(context);
            if (parameters.Count > 0)
                _parameters = parameters.Values.ToArray();
            if (ieGuids.Count > 0)
                _innerExceptionGuids = ieGuids.Values.ToArray();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Log" /> class.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="context">The context information.</param>
        /// <param name="exception">The exception. If none then pass <see langword="null" />.</param>
        /// <param name="level">The log level.</param>
        /// <param name="resourceType">Type of the resource.</param>
        /// <param name="format">The format.</param>
        /// <param name="resource">The resource.</param>
        /// <param name="parameters">The parameters.</param>
        private Log(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] LogContext context,
            [CanBeNull] Exception exception, 
            LoggingLevel level,
            [CanBeNull] Type resourceType, 
            [LocalizationRequired, CanBeNull] string format, 
            [CanBeNull] Expression<Func<string>> resource, 
            [CanBeNull] params object[] parameters)
            : this()
        {
            Contract.Requires(format == null || resource == null);
            Contract.Requires(resourceType == null || format != null);

            _guid = CombGuid.NewCombGuid();
            _level = level;

            // Lock the context to prevent changes after it is locked.
            if (context != null)
            {
                context.Lock();
                _context = context;
            }

            // Get the current thread information
            Thread currentThread = Thread.CurrentThread;
            _threadID = currentThread.ManagedThreadId;
            _threadName = currentThread.Name;

            _latestCultureInfo = culture;

            bool hasMessage = false;

            // If we have a formatted message add it now
            if (!string.IsNullOrEmpty(format))
            {
                if (resourceType != null)
                {
                    try
                    {
                        _resourceManager = Translation.GetResourceManager(resourceType);

                        if (_resourceManager != null)
                        {
                            _tag = format;
                            _resourceProperty = string.Join(".", resourceType.FullName, _tag);

                            _messageFormat = _resourceManager.GetString(_tag, culture);
                            hasMessage = true;
                        }
                    }
                    catch
                    {
                        _messageFormat = null;
                        hasMessage = false;
                    }
                }
                else
                {
                    hasMessage = true;
                    _messageFormat = format;
                }
            }
            else if (resource != null)
                // Try to evaluate the resource
                try
                {
                    Func<string> func = resource.Compile();

                    // See if we can get the resource property.
                    MemberExpression me = resource.Body as MemberExpression;
                    if (me != null)
                    {
                        PropertyInfo pi = me.Member as PropertyInfo;
                        if (pi != null)
                        {
                            MethodInfo mi = pi.GetMethod;
                            if (mi != null)
                            {
                                Type dt = mi.DeclaringType;
                                Contract.Assert(dt != null);
                                _resourceManager = Translation.GetResourceManager(dt);

                                if (_resourceManager != null)
                                {
                                    _tag = pi.Name;
                                    _resourceProperty = string.Join(".", dt.FullName, pi.Name);

                                    _messageFormat = _resourceManager.GetString(_tag, culture);
                                }
                            }
                        }
                    }

                    // Grab the original message format, this is used in future if we fail to get the resource again.
                    if (_messageFormat == null)
                        _messageFormat = func();

                    // If we have a resource property, then we have a message even if it's null for this culture
                    // as it may not be null with other cultures.
                    hasMessage = _resourceProperty != null || _messageFormat != null;
                }
                catch
                {
                    _messageFormat = null;
                    hasMessage = false;
                }

            // Add parameters
            if (hasMessage &&
                (parameters != null) &&
                (parameters.Length >= 1))
                _parameters = parameters.Select(p => p.ToString()).ToArray();

            Exception[] innerExceptions = null;
            if (exception != null)
            {
                LoggingException logException = exception as LoggingException;

                // NOTE: If this is a logging exception and Log has not been set yet, then this has been called from the LoggingException constructor
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                bool isLogException = (logException != null && logException.Log == null);

                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (!isLogException && hasMessage)
                    innerExceptions = new[] { exception };
                else
                {
                    // Add the exception type.
                    _exceptionType = exception.GetType().ToString();

                    // ReSharper disable ConditionIsAlwaysTrueOrFalse
                    if (!isLogException)
                        // ReSharper restore ConditionIsAlwaysTrueOrFalse
                        _messageFormat = exception.Message;

                    _stackTrace = exception.StackTrace;

                    // Check for aggregate exception
                    AggregateException aggregateException = exception as AggregateException;
                    if (aggregateException != null)
                        innerExceptions = aggregateException.InnerExceptions != null
                            ? aggregateException.InnerExceptions.ToArray()
                            : null;
                    else
                    {
                        if (exception.InnerException != null)
                            innerExceptions = new[] { exception.InnerException };

                        // If this is a SQL exception, then log the stored proc.
                        SqlException sqlException = exception as SqlException;
                        if (sqlException != null)
                        {
                            _storedProcedure = string.IsNullOrEmpty(sqlException.Procedure)
                                ? "<Unknown>"
                                : sqlException.Procedure;
                            _storedProcedureLine = sqlException.LineNumber;
                        }
                    }
                }
            }

            if (innerExceptions != null &&
                innerExceptions.Length > 0)
                _innerExceptionGuids = innerExceptions
                    .Select(
                        e =>
                        {
                            LoggingException le = e as LoggingException;
                            return le == null || ReferenceEquals(le, exception)
                                ? new Log(culture, null, e, LoggingLevel.Error, resourceType, null, null, null).Guid
                                : le.Guid;
                        }).ToArray();

            // If there was a message and no stack trace was added for an exception, get the current stack trace
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (hasMessage && _stackTrace == null)
                // Not that the first two stack frames are either Log.Add, or new Log() followed by new Log().
                _stackTrace = FormatStackTrace(new StackTrace(2, true));

            // Increment performance counter.
            _perfCounterNewItem.Increment();

            // Post log onto queue
            ReLog();
        }

        #region Constructor Overloads
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
        public Log([LocalizationRequired] [CanBeNull] string format, [CanBeNull] params object[] parameters)
            : this(Translation.DefaultCulture, null, null, LoggingLevel.Information, null, format, null, parameters)
        {
        }

        /// <summary>
        ///   Logs a message at the information <see cref="LoggingLevel">log level</see>.
        /// </summary>
        /// <param name="context">The log context.</param>
        /// <param name="format">The log message.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        [StringFormatMethod("format")]
        [PublicAPI]
        public Log(
            [CanBeNull] LogContext context,
            [LocalizationRequired] [CanBeNull] string format,
            [CanBeNull] params object[] parameters)
            : this(Translation.DefaultCulture, context, null, LoggingLevel.Information, null, format, null, parameters)
        {
        }

        /// <summary>
        ///   Logs a message at the specified <see cref="LoggingLevel"/>.
        /// </summary>
        /// <param name="format">The log message.</param>
        /// <param name="level">The log level.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        [StringFormatMethod("format")]
        [PublicAPI]
        public Log(
            LoggingLevel level,
            [LocalizationRequired] [CanBeNull] string format,
            [CanBeNull] params object[] parameters)
            : this(Translation.DefaultCulture, null, null, level, null, format, null, parameters)
        {
        }

        /// <summary>
        /// Logs a message at the specified <see cref="LoggingLevel" />.
        /// </summary>
        /// <param name="context">The log context.</param>
        /// <param name="level">The log level.</param>
        /// <param name="format">The log message.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        [StringFormatMethod("format")]
        [PublicAPI]
        public Log(
            [CanBeNull] LogContext context,
            LoggingLevel level,
            [LocalizationRequired] [CanBeNull] string format,
            [CanBeNull] params object[] parameters)
            : this(Translation.DefaultCulture, context, null, level, null, format, null, parameters)
        {
        }

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="exception"><para>The exception to log.</para>
        /// <para>
        ///   <see cref="LoggingException" />'s add themselves and so this method ignores them.</para></param>
        /// <param name="level"><para>The log level.</para>
        /// <para>By default this uses the error log level.</para></param>
        [PublicAPI]
        public Log([CanBeNull] Exception exception, LoggingLevel level = LoggingLevel.Error)
            : this(Translation.DefaultCulture, null, exception, level, null, null, null, null)
        {
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
        [PublicAPI]
        public Log(
            [CanBeNull] LogContext context,
            [CanBeNull] Exception exception,
            LoggingLevel level = LoggingLevel.Error)
            : this(Translation.DefaultCulture, context, exception, level, null, null, null, null)
        {
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
        [PublicAPI]
        [StringFormatMethod("format")]
        public Log(
            [CanBeNull] Exception exception,
            LoggingLevel level,
            [LocalizationRequired] [CanBeNull] string format,
            [CanBeNull] params object[] parameters)
            : this(Translation.DefaultCulture, null, exception, level, null, format, null, parameters)
        {
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
        [PublicAPI]
        [StringFormatMethod("format")]
        public Log(
            [CanBeNull] LogContext context,
            [CanBeNull] Exception exception,
            LoggingLevel level,
            [LocalizationRequired] [CanBeNull] string format,
            [CanBeNull] params object[] parameters)
            : this(Translation.DefaultCulture, context, exception, level, null, format, null, parameters)
        {
        }

        /// <summary>
        /// Logs a message at the information <see cref="LoggingLevel">log level</see>.
        /// </summary>
        /// <param name="resource">The resource expression, e.g. ()=> Resources.Log_Message.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        [PublicAPI]
        public Log([CanBeNull] Expression<Func<string>> resource, [CanBeNull] params object[] parameters)
            : this(Translation.DefaultCulture, null, null, LoggingLevel.Information, null, null, resource, parameters)
        {
        }

        /// <summary>
        ///   Logs a message at the information <see cref="LoggingLevel">log level</see>.
        /// </summary>
        /// <param name="context">The log context.</param>
        /// <param name="resource">The resource expression, e.g. ()=> Resources.Log_Message.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        [PublicAPI]
        public Log(
            [CanBeNull] LogContext context,
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
            : this(Translation.DefaultCulture, context, null, LoggingLevel.Information, null, null, resource, parameters)
        {
        }

        /// <summary>
        ///   Logs a message at the specified <see cref="LoggingLevel"/>.
        /// </summary>
        /// <param name="resource">The resource expression, e.g. ()=> Resources.Log_Message.</param>
        /// <param name="level">The log level.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        [PublicAPI]
        public Log(
            LoggingLevel level,
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
            : this(Translation.DefaultCulture, null, null, level, null, null, resource, parameters)
        {
        }

        /// <summary>
        /// Logs a message at the specified <see cref="LoggingLevel" />.
        /// </summary>
        /// <param name="context">The log context.</param>
        /// <param name="level">The log level.</param>
        /// <param name="resource">The resource expression, e.g. ()=> Resources.Log_Message.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        [PublicAPI]
        public Log(
            [CanBeNull] LogContext context,
            LoggingLevel level,
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
            : this(Translation.DefaultCulture, context, null, level, null, null, resource, parameters)
        {
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
        [PublicAPI]
        public Log(
            [CanBeNull] Exception exception,
            LoggingLevel level,
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
            : this(Translation.DefaultCulture, null, exception, level, null, null, resource, parameters)
        {
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
        [PublicAPI]
        public Log(
            [CanBeNull] LogContext context,
            [CanBeNull] Exception exception,
            LoggingLevel level,
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
            : this(Translation.DefaultCulture, context, exception, level, null, null, resource, parameters)
        {
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
        public Log(
            [CanBeNull] CultureInfo culture,
            [LocalizationRequired] [CanBeNull] string format,
            [CanBeNull] params object[] parameters)
            : this(culture, null, null, LoggingLevel.Information, null, format, null, parameters)
        {
        }

        /// <summary>
        /// Logs a message at the information <see cref="LoggingLevel">log level</see>.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="context">The log context.</param>
        /// <param name="format">The log message.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        [StringFormatMethod("format")]
        [PublicAPI]
        public Log(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] LogContext context,
            [LocalizationRequired] [CanBeNull] string format,
            [CanBeNull] params object[] parameters)
            : this(culture, context, null, LoggingLevel.Information, null, format, null, parameters)
        {
        }

        /// <summary>
        /// Logs a message at the specified <see cref="LoggingLevel" />.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="level">The log level.</param>
        /// <param name="format">The log message.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        [StringFormatMethod("format")]
        [PublicAPI]
        public Log(
            [CanBeNull] CultureInfo culture,
            LoggingLevel level,
            [LocalizationRequired] [CanBeNull] string format,
            [CanBeNull] params object[] parameters)
            : this(culture, null, null, level, null, format, null, parameters)
        {
        }

        /// <summary>
        /// Logs a message at the specified <see cref="LoggingLevel" />.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="context">The log context.</param>
        /// <param name="level">The log level.</param>
        /// <param name="format">The log message.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        [StringFormatMethod("format")]
        [PublicAPI]
        public Log(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] LogContext context,
            LoggingLevel level,
            [LocalizationRequired] [CanBeNull] string format,
            [CanBeNull] params object[] parameters)
            : this(culture, context, null, level, null, format, null, parameters)
        {
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
        [PublicAPI]
        public Log(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] Exception exception,
            LoggingLevel level = LoggingLevel.Error)
            : this(culture, null, exception, level, null, null, null, null)
        {
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
        [PublicAPI]
        public Log(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] LogContext context,
            [CanBeNull] Exception exception,
            LoggingLevel level = LoggingLevel.Error)
            : this(culture, context, exception, level, null, null, null, null)
        {
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
        [PublicAPI]
        [StringFormatMethod("format")]
        public Log(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] Exception exception,
            LoggingLevel level,
            [LocalizationRequired] [CanBeNull] string format,
            [CanBeNull] params object[] parameters)
            : this(culture, null, exception, level, null, format, null, parameters)
        {
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
        [PublicAPI]
        [StringFormatMethod("format")]
        public Log(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] LogContext context,
            [CanBeNull] Exception exception,
            LoggingLevel level,
            [LocalizationRequired] [CanBeNull] string format,
            [CanBeNull] params object[] parameters)
            : this(culture, context, exception, level, null, format, null, parameters)
        {
        }

        /// <summary>
        /// Logs a message at the information <see cref="LoggingLevel">log level</see>.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="resource">The resource expression, e.g. ()=&gt; Resources.Log_Message.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        [PublicAPI]
        public Log(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
            : this(culture, null, null, LoggingLevel.Information, null, null, resource, parameters)
        {
        }

        /// <summary>
        /// Logs a message at the information <see cref="LoggingLevel">log level</see>.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="context">The log context.</param>
        /// <param name="resource">The resource expression, e.g. ()=&gt; Resources.Log_Message.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        [PublicAPI]
        public Log(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] LogContext context,
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
            : this(culture, context, null, LoggingLevel.Information, null, null, resource, parameters)
        {
        }

        /// <summary>
        /// Logs a message at the specified <see cref="LoggingLevel" />.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="level">The log level.</param>
        /// <param name="resource">The resource expression, e.g. ()=&gt; Resources.Log_Message.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        [PublicAPI]
        public Log(
            [CanBeNull] CultureInfo culture,
            LoggingLevel level,
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
            : this(culture, null, null, level, null, null, resource, parameters)
        {
        }

        /// <summary>
        /// Logs a message at the specified <see cref="LoggingLevel" />.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="context">The log context.</param>
        /// <param name="level">The log level.</param>
        /// <param name="resource">The resource expression, e.g. ()=&gt; Resources.Log_Message.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        [PublicAPI]
        public Log(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] LogContext context,
            LoggingLevel level,
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
            : this(culture, context, null, level, null, null, resource, parameters)
        {
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
        [PublicAPI]
        public Log(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] Exception exception,
            LoggingLevel level,
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
            : this(culture, null, exception, level, null, null, resource, parameters)
        {
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
        [PublicAPI]
        public Log(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] LogContext context,
            [CanBeNull] Exception exception,
            LoggingLevel level,
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
            : this(culture, context, exception, level, null, null, resource, parameters)
        {
        }

        /// <summary>
        /// Logs a message at the information <see cref="LoggingLevel">log level</see>.
        /// </summary>
        /// <param name="resourceType">The resource class type.</param>
        /// <param name="resourceProperty">The name of the resource property in <paramref name="resourceType"/>.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        [PublicAPI]
        public Log(
            [CanBeNull] Type resourceType, 
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
            : this(Translation.DefaultCulture, null, null, LoggingLevel.Information, resourceType, resourceProperty, null, parameters)
        {
        }

        /// <summary>
        ///   Logs a message at the information <see cref="LoggingLevel">log level</see>.
        /// </summary>
        /// <param name="context">The log context.</param>
        /// <param name="resourceType">The resource class type.</param>
        /// <param name="resourceProperty">The name of the resource property in <paramref name="resourceType"/>.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        [PublicAPI]
        public Log(
            [CanBeNull] LogContext context,
            [CanBeNull] Type resourceType,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
            : this(Translation.DefaultCulture, context, null, LoggingLevel.Information, resourceType, resourceProperty, null, parameters)
        {
        }

        /// <summary>
        ///   Logs a message at the specified <see cref="LoggingLevel"/>.
        /// </summary>
        /// <param name="resourceType">The resource class type.</param>
        /// <param name="resourceProperty">The name of the resource property in <paramref name="resourceType"/>.</param>
        /// <param name="level">The log level.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        [PublicAPI]
        public Log(
            LoggingLevel level,
            [CanBeNull] Type resourceType,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
            : this(Translation.DefaultCulture, null, null, level, resourceType, resourceProperty, null, parameters)
        {
        }

        /// <summary>
        /// Logs a message at the specified <see cref="LoggingLevel" />.
        /// </summary>
        /// <param name="context">The log context.</param>
        /// <param name="level">The log level.</param>
        /// <param name="resourceType">The resource class type.</param>
        /// <param name="resourceProperty">The name of the resource property in <paramref name="resourceType"/>.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        [PublicAPI]
        public Log(
            [CanBeNull] LogContext context,
            LoggingLevel level,
            [CanBeNull] Type resourceType,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
            : this(Translation.DefaultCulture, context, null, level, resourceType, resourceProperty, null, parameters)
        {
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
        [PublicAPI]
        public Log(
            [CanBeNull] Exception exception,
            LoggingLevel level,
            [CanBeNull] Type resourceType,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
            : this(Translation.DefaultCulture, null, exception, level, resourceType, resourceProperty, null, parameters)
        {
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
        [PublicAPI]
        public Log(
            [CanBeNull] LogContext context,
            [CanBeNull] Exception exception,
            LoggingLevel level,
            [CanBeNull] Type resourceType,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
            : this(Translation.DefaultCulture, context, exception, level, resourceType, resourceProperty, null, parameters)
        {
        }

        /// <summary>
        /// Logs a message at the information <see cref="LoggingLevel">log level</see>.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="resourceType">The resource class type.</param>
        /// <param name="resourceProperty">The name of the resource property in <paramref name="resourceType"/>.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        [PublicAPI]
        public Log(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] Type resourceType,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
            : this(culture, null, null, LoggingLevel.Information, resourceType, resourceProperty, null, parameters)
        {
        }

        /// <summary>
        /// Logs a message at the information <see cref="LoggingLevel">log level</see>.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="context">The log context.</param>
        /// <param name="resourceType">The resource class type.</param>
        /// <param name="resourceProperty">The name of the resource property in <paramref name="resourceType"/>.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        [PublicAPI]
        public Log(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] LogContext context,
            [CanBeNull] Type resourceType,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
            : this(culture, context, null, LoggingLevel.Information, resourceType, resourceProperty, null, parameters)
        {
        }

        /// <summary>
        /// Logs a message at the specified <see cref="LoggingLevel" />.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="level">The log level.</param>
        /// <param name="resourceType">The resource class type.</param>
        /// <param name="resourceProperty">The name of the resource property in <paramref name="resourceType"/>.</param>
        /// <param name="parameters">The optional parameters, for formatting the message.</param>
        [PublicAPI]
        public Log(
            [CanBeNull] CultureInfo culture,
            LoggingLevel level,
            [CanBeNull] Type resourceType,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
            : this(culture, null, null, level, resourceType, resourceProperty, null, parameters)
        {
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
        [PublicAPI]
        public Log(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] LogContext context,
            LoggingLevel level,
            [CanBeNull] Type resourceType,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
            : this(culture, context, null, level, resourceType, resourceProperty, null, parameters)
        {
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
        [PublicAPI]
        public Log(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] Exception exception,
            LoggingLevel level,
            [CanBeNull] Type resourceType,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
            : this(culture, null, exception, level, resourceType, resourceProperty, null, parameters)
        {
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
        [PublicAPI]
        public Log(
            [CanBeNull] CultureInfo culture,
            [CanBeNull] LogContext context,
            [CanBeNull] Exception exception,
            LoggingLevel level,
            [CanBeNull] Type resourceType,
            [CanBeNull] string resourceProperty,
            [CanBeNull] params object[] parameters)
            : this(culture, context, exception, level, resourceType, resourceProperty, null, parameters)
        {
        }
        #endregion

        /// <summary>
        /// Gets the Protobuf schema (.proto) used for Protobuf serialization.
        /// </summary>
        /// <value>The protobuf schema.</value>
        /// <remarks><para>See https://code.google.com/p/protobuf/.
        /// </para></remarks>
        [PublicAPI]
        [NotNull]
        public static string ProtobufSchema
        {
            get
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                return _protobufSchema.Value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the log can be translated.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if <see cref="MessageFormat"/> is translatable; otherwise, <see langword="false" />.
        /// </value>
        [PublicAPI]
        public bool IsTranslatable
        {
            get { return _resourceProperty != null; }
        }

        /// <summary>
        ///   A Guid used to uniquely identify a log item.
        /// </summary>
        [PublicAPI]
        public CombGuid Guid
        {
            get { return _guid; }
        }

        /// <summary>
        ///   The <see cref="LoggingLevel">log level</see>.
        /// </summary>
        [PublicAPI]
        public LoggingLevel Level
        {
            get { return _level; }
        }

        /// <summary>
        ///   The formatted log message.
        /// </summary>
        [PublicAPI]
        [CanBeNull]
        public string Message
        {
            get { return GetMessage(Translation.DefaultCulture); }
        }

        /// <summary>
        /// Gets the message, for a particular culture.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        [CanBeNull]
        [PublicAPI]
        public string GetMessage([CanBeNull] CultureInfo culture)
        {
            lock (_lock)
            {
                CalculateMessage(culture ?? Translation.DefaultCulture);
                return _latestMessage;
            }
        }

        /// <summary>
        /// Gets the message format for the <see cref="Translation.DefaultCulture"/>.
        /// </summary>
        /// <value>The message format.</value>
        [CanBeNull]
        [PublicAPI]
        public string MessageFormat
        {
            get { return GetMessageFormat(Translation.DefaultCulture); }
        }

        /// <summary>
        /// Gets the message format, for a particular culture.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        [CanBeNull]
        [PublicAPI]
        public string GetMessageFormat([CanBeNull] CultureInfo culture)
        {
            lock (_lock)
            {
                CalculateMessage(culture ?? Translation.DefaultCulture);
                return _latestMessageFormat;
            }
        }

        /// <summary>
        ///   Gets a <see cref="bool"/> value indicating whether this instance was generated from an exception.
        /// </summary>
        /// <value>
        ///   Returns <see langword="true"/> if this instance was an exception; otherwise <see langword="false"/>.
        /// </value>
        [PublicAPI]
        public bool IsException
        {
            get { return !string.IsNullOrWhiteSpace(_exceptionType); }
        }

        /// <summary>
        /// Gets the type of the exception (if any).
        /// </summary>
        /// <value>The type of the exception.</value>
        [PublicAPI]
        [CanBeNull]
        public string ExceptionType
        {
            get { return _exceptionType; }
        }

        /// <summary>
        /// Gets the inner exceptions unique identifiers, if any.
        /// </summary>
        /// <value>The inner exceptions unique identifiers.</value>
        [PublicAPI]
        [NotNull]
        public IEnumerable<CombGuid> InnerExceptionGuids
        {
            get { return _innerExceptionGuids ?? _emptyCombGuidArray; }
        }

        /// <summary>
        ///   The time stamp of when the log item was created.
        /// </summary>
        [PublicAPI]
        public DateTime TimeStamp
        {
            get { return Guid.Created; }
        }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        [PublicAPI]
        [NotNull]
        public IEnumerable<string> Parameters
        {
            get { return _parameters ?? _emptyStringArray; }
        }

        /// <summary>
        /// Gets the stack trace.
        /// </summary>
        /// <value>The stack trace.</value>
        [CanBeNull]
        [PublicAPI]
        public string StackTrace
        {
            get { return _stackTrace; }
        }

        /// <summary>
        /// Gets the resource property full name, if the message is a translatable resource; otherwise <see langword="null"/>.
        /// </summary>
        /// <value>The resource property full name.</value>
        [CanBeNull]
        [PublicAPI]
        public string ResourceProperty
        {
            get { return _resourceProperty; }
        }

        /// <summary>
        /// Gets the tag resource tag, if the message is a translatable resource; otherwise <see langword="null"/>.
        /// </summary>
        /// <value>
        /// The tag.
        /// </value>
        [CanBeNull]
        [PublicAPI]
        public string Tag
        {
            get { return _tag; }
        }

        /// <summary>
        /// Gets the thread ID (or -1 if not known).
        /// </summary>
        /// <value>The thread ID.</value>
        [PublicAPI]
        public int ThreadID
        {
            get { return _threadID; }
        }

        /// <summary>
        /// Gets the name of the thread.
        /// </summary>
        /// <value>The name of the thread.</value>
        [CanBeNull]
        [PublicAPI]
        public string ThreadName
        {
            get { return _threadName; }
        }


        /// <summary>
        /// Gets the stored procedure (if any).
        /// </summary>
        /// <value>The stored procedure.</value>
        [CanBeNull]
        [PublicAPI]
        public string StoredProcedure
        {
            get { return _storedProcedure; }
        }

        /// <summary>
        /// Gets the stored procedure line (if any); otherwise 0.
        /// </summary>
        /// <value>The stored procedure line.</value>
        [PublicAPI]
        public int StoredProcedureLine
        {
            get { return _storedProcedureLine; }
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The value associated with the specified key; otherwise null.</returns>
        [CanBeNull]
        [PublicAPI]
        public string this[[NotNull] string key]
        {
            get { return this.FirstOrDefault(kvp => string.Equals(kvp.Key, key)).Value; }
        }

        /// <summary>
        /// Calculates the message given the current culture.
        /// </summary>
        private void CalculateMessage([NotNull] CultureInfo culture)
        {
            Contract.Requires(culture != null);

            string messageFormat = _messageFormat;
            // Check if we have a resource property.
            if (_resourceProperty == null)
            {
                if (_latestMessage != null)
                    return;
            }
            else
                try
                {
                    if (Equals(_latestCultureInfo, culture) &&
                        _latestMessage != null)
                        return;
                    _latestCultureInfo = culture;

                    // Check if we have the resource manager yet (will be blank after de-serialization).
                    if (_resourceManager == null)
                    {
                        int lastDot = _resourceProperty.LastIndexOf('.');
                        _resourceManager = Translation.GetResourceManager(
                            _resourceProperty.Substring(0, lastDot));

                        if (_resourceManager != null)
                            _tag = _resourceProperty.Substring(lastDot + 1);
                    }
                    else
                        Contract.Assert(_tag != null);

                    // Grab the format
                    if (_resourceManager != null)
                        messageFormat = _resourceManager.GetString(_tag, culture);
                }
                catch
                {
                    // Use the original one stored with the Log.
                    messageFormat = _messageFormat;
                }

            // See if it's changed.
            if (string.Equals(messageFormat, _latestMessageFormat)) return;

            _latestMessageFormat = messageFormat;
            // Recalculate the message.
            if (messageFormat == null)
                _latestMessage = null;
            else if (_parameters != null)
                _latestMessage = messageFormat.SafeFormat(_parameters.Cast<object>().ToArray());
            else
                _latestMessage = messageFormat;
        }

        #region ToString overloads
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="format">The format to use.-or- A null reference (Nothing in Visual Basic) to use the default format defined for the type of the <see cref="T:System.IFormattable" /> implementation.</param>
        /// <param name="formatProvider">The provider to use to format the value.-or- A null reference (Nothing in Visual Basic) to obtain the numeric format information from the current locale setting of the operating system.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public string ToString([CanBeNull] string format, [CanBeNull] IFormatProvider formatProvider = null)
        {
            return ToString(format, CultureInfo.CurrentCulture, formatProvider);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="format">The format to use.-or- A null reference (Nothing in Visual Basic) to use the default format defined for the type of the <see cref="T:System.IFormattable" /> implementation.</param>
        /// <param name="culture"></param>
        /// <param name="formatProvider">The provider to use to format the value.-or- A null reference (Nothing in Visual Basic) to obtain the numeric format information from the current locale setting of the operating system.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        [NotNull]
        [PublicAPI]
        public string ToString(
            [CanBeNull] string format,
            [NotNull] CultureInfo culture,
            [CanBeNull] IFormatProvider formatProvider = null)
        {
            if (format == null) format = LogFormat.General.ToString();

            if (formatProvider != null)
            {
                ICustomFormatter formatter = formatProvider.GetFormat(typeof(Log)) as ICustomFormatter;

                if (formatter != null)
                    return formatter.Format(format, this, formatProvider) ?? String.Empty;
            }

            // Try to get the actual format if specified as an enum.
            LogFormat logFormat;
            if (_formats.TryGetValue(format, out logFormat) ||
                Enum.TryParse(format, true, out logFormat))
                return ToString(logFormat); // TODO

            // Get format chunks
            StringBuilder builder = new StringBuilder(format.Length * 2);
            foreach (Tuple<string, string, string> tuple in format.FormatChunks())
            {
                Contract.Assert(tuple != null);
                if (String.IsNullOrEmpty(tuple.Item1) ||
                    (!_formats.TryGetValue(tuple.Item1, out logFormat) &&
                     !Enum.TryParse(tuple.Item1, true, out logFormat)))
                {
                    // We didn't recognise the format string (if there was one), so we just output the chunk un-escaoed.
                    builder.AddUnescaped(tuple.Item3);
                    continue;
                }

                // Append the formatted options.
                AppendFormatted(
                    builder,
                    logFormat,
                    culture,
                    tuple.Item2 != null
                        ? tuple.Item2.Unescape()
                        : null);
            }

            // Output our formatted string.
            return builder.ToString();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return ToString(LogFormat.General);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public string ToString([NotNull] CultureInfo culture)
        {
            return ToString(LogFormat.General, culture);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        [NotNull]
        [PublicAPI]
        public string ToString([CanBeNull] IFormatProvider formatProvider)
        {
            return ToString(null, formatProvider);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="options">The optional options for the format.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        /// <exception cref="System.FormatException">
        /// </exception>
        [NotNull]
        [PublicAPI]
        public string ToString(LogFormat format, [CanBeNull] string options = null)
        {
            return ToString(format, CultureInfo.CurrentCulture, options);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="options">The optional options for the format.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        /// <exception cref="System.FormatException"></exception>
        [NotNull]
        [PublicAPI]
        public string ToString(LogFormat format, [NotNull] CultureInfo culture, [CanBeNull] string options = null)
        {
            if (format == LogFormat.None)
                return String.Empty;
            StringBuilder builder = new StringBuilder();
            AppendFormatted(builder, format, culture, options);
            return builder.ToString();
        }

        /// <summary>
        /// Appends the the formatted information.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="format">The format.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="options">The options.</param>
        /// <exception cref="System.FormatException">
        /// </exception>
        private void AppendFormatted(
            [NotNull] StringBuilder builder,
            LogFormat format,
            [CanBeNull] CultureInfo culture,
            [CanBeNull] string options = null)
        {
            Contract.Requires(builder != null);
            if (format == LogFormat.None)
                return;

            // Get option flags
            bool includeMissing = format.HasFlag(LogFormat.IncludeMissing);
            bool includeHeader = format.HasFlag(LogFormat.Header);
            bool asXml = format.HasFlag(LogFormat.Xml);
            bool asJson = format.HasFlag(LogFormat.Json);

            if (culture == null)
                culture = Translation.DefaultCulture;

            // Remove option flags
            format = ((LogFormat)(((int)format) & 0x0FFFFFFF));

            if (asXml && asJson)
                throw new FormatException(Resources.Log_Invalid_Format_XML_JSON);

            MasterFormat masterFormat;
            bool includeKey;
            string indent;
            if (asXml)
            {
                masterFormat = MasterFormat.Xml;
                includeKey = true;
                indent = "   ";
            }
            else if (asJson)
            {
                masterFormat = MasterFormat.JSON;
                includeKey = true;
                indent = "   ";
            }
            else
            {
                masterFormat = MasterFormat.Text;

                // Only include the key if we're a combination of keys.
                includeKey = format.IsCombinationFlag(true);
                indent = String.Empty;
            }

            // Otherwise always include value.
            if (!includeKey)
                includeMissing = true;

            if (includeHeader)
                switch (masterFormat)
                {
                    case MasterFormat.Xml:
                        builder.AppendLine("<Log>");
                        break;
                    case MasterFormat.JSON:
                        builder.AppendLine("{");
                        break;
                    default:
                        builder.AppendLine(Header);
                        break;
                }

            bool first = true;
            LogFormat[] flags = format.SplitFlags(true).ToArray();
            if (flags.Length < 1) return;

            // Ignore options if we have multiple flags
            if (flags.Length > 1) options = null;
            foreach (LogFormat flag in flags)
            {
                string key;
                string value;
                bool escaped = false;

                // This is a single value format, just output the value directly
                switch (flag)
                {
                    case LogFormat.Message:
                        key = "Message";
                        value = GetMessage(culture);
                        break;
                    case LogFormat.ResourceProperty:
                        key = "Resource";
                        value = ResourceProperty;
                        break;
                    case LogFormat.Culture:
                        key = "Culture";
                        value = culture.Name;
                        break;
                    case LogFormat.TimeStamp:
                        key = "TimeStamp";
                        value = TimeStamp.ToString(options ?? "o");
                        break;
                    case LogFormat.Level:
                        key = "Level";
                        value = Level.ToString();
                        break;
                    case LogFormat.Guid:
                        key = "Guid";
                        CombGuid guid = Guid;
                        value = guid == CombGuid.Empty ? null : guid.ToString(options ?? "D");
                        break;
                    case LogFormat.Exception:
                        key = "Exception Type";
                        value = _exceptionType;
                        break;
                    case LogFormat.SQLException:
                        key = "Stored Procedure";
                        value = _storedProcedure;
                        if (value != null)
                            value += " at line " + _storedProcedureLine.ToString(options ?? "D");
                        break;
                    case LogFormat.InnerException:
                        key = "Inner Exception";

                        if ((_innerExceptionGuids == null) ||
                            (_innerExceptionGuids.Length < 1))
                            value = null;
                        else if (_innerExceptionGuids.Length == 1)
                            value = _innerExceptionGuids[0].ToString(options ?? "D");
                        else
                        {
                            key = "Inner Exceptions";

                            StringBuilder cv = new StringBuilder();

                            cv.AppendLine(masterFormat == MasterFormat.JSON ? "[" : String.Empty);

                            string i = indent + "   ";
                            bool cvf = true;
                            foreach (CombGuid ieg in _innerExceptionGuids)
                            {
                                if (!cvf)
                                    cv.AppendLine(masterFormat == MasterFormat.JSON ? "," : String.Empty);
                                cvf = false;

                                // ReSharper disable once AssignNullToNotNullAttribute
                                if (masterFormat == MasterFormat.JSON)
                                    // Write out as array
                                    cv.Append(i)
                                        .Append('"')
                                        .Append(ieg.ToString(options ?? "D"))
                                        .Append('"');
                                else
                                    AddKVP(
                                        cv,
                                        masterFormat,
                                        i,
                                        "Inner Exception",
                                        ieg.ToString(options ?? "D"));
                            }

                            switch (masterFormat)
                            {
                                case MasterFormat.Xml:
                                    cv.AppendLine()
                                        .Append(indent);
                                    break;
                                case MasterFormat.JSON:
                                    cv.AppendLine()
                                        .Append(indent)
                                        .Append("]");
                                    break;
                            }

                            value = cv.ToString();
                            escaped = true;
                        }
                        break;
                    case LogFormat.StackTrace:
                        key = "Stack Trace";
                        value = StackTrace;
                        break;
                    case LogFormat.ThreadID:
                        key = "Thread ID";
                        value = _threadID < 0 ? null : _threadID.ToString(options ?? "G");
                        break;
                    case LogFormat.ThreadName:
                        key = "Thread Name";
                        value = ThreadName;
                        break;
                    case LogFormat.ApplicationName:
                        key = "Application Name";
                        value = ApplicationName;
                        break;
                    case LogFormat.ApplicationGuid:
                        key = "Application Guid";
                        value = ApplicationGuid.ToString(options ?? "D");
                        break;
                    case LogFormat.Context:
                        key = "Context";

                        if ((_context == null) ||
                            (_context.Count < 1))
                            value = null;
                        else
                        {
                            StringBuilder cv = new StringBuilder();

                            cv.AppendLine(masterFormat == MasterFormat.JSON ? "{" : String.Empty);

                            string i = indent + "   ";
                            bool cvf = true;
                            foreach (
                                KeyValuePair<string, string> kvp in (IEnumerable<KeyValuePair<string, string>>)_context
                                )
                            {
                                Contract.Assert(kvp.Key != null);
                                if (!cvf)
                                    cv.AppendLine(masterFormat == MasterFormat.JSON ? "," : String.Empty);
                                cvf = false;

                                // ReSharper disable once AssignNullToNotNullAttribute
                                AddKVP(
                                    cv,
                                    masterFormat,
                                    i,
                                    kvp.Key,
                                    kvp.Value);
                            }

                            switch (masterFormat)
                            {
                                case MasterFormat.Xml:
                                    cv.AppendLine()
                                        .Append(indent);
                                    break;
                                case MasterFormat.JSON:
                                    cv.AppendLine()
                                        .Append(indent)
                                        .Append("]");
                                    break;
                            }

                            value = cv.ToString();
                            escaped = true;
                        }
                        break;

                    default:
                        throw new FormatException(String.Format(Resources.Log_Invalid_Format_Singular, flag));
                }
                if (value == null &&
                    !includeMissing)
                    continue;

                if (includeKey)
                {
                    if (!first)
                        builder.AppendLine(masterFormat == MasterFormat.JSON ? "," : String.Empty);

                    AddKVP(builder, masterFormat, indent, key, value, escaped);

                    first = false;
                }
                else
                    builder.Append(value);
            }

            if (includeHeader)
            {
                builder.AppendLine();
                switch (masterFormat)
                {
                    case MasterFormat.Xml:
                        builder.AppendLine("</Log>");
                        break;
                    case MasterFormat.JSON:
                        builder.Append("}");
                        break;
                    default:
                        builder.AppendLine(Header);
                        break;
                }
            }
        }
        #endregion

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>A set of key value pairs.</returns>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            // We have to manually yield our fields, as we don't hold everything in the context for performance.
            yield return new KeyValuePair<string, string>(GuidKey, _guid.ToString());
            yield return new KeyValuePair<string, string>(LevelKey, _level.ToString());
            if (_messageFormat != null)
                yield return new KeyValuePair<string, string>(MessageFormatKey, _messageFormat);
            if (_resourceProperty != null)
                yield return new KeyValuePair<string, string>(ResourcePropertyKey, _resourceProperty);
            yield return new KeyValuePair<string, string>(ThreadIDKey, _threadID.ToString(CultureInfo.InvariantCulture))
                ;
            if (_threadName != null)
                yield return new KeyValuePair<string, string>(ThreadNameKey, _threadName);
            if (_stackTrace != null)
                yield return new KeyValuePair<string, string>(StackTraceKey, _stackTrace);
            if (_exceptionType != null)
                yield return new KeyValuePair<string, string>(ExceptionTypeFullNameKey, _exceptionType);
            if (_storedProcedure != null)
            {
                yield return new KeyValuePair<string, string>(StoredProcedureKey, _storedProcedure);
                yield return
                    new KeyValuePair<string, string>(
                        StoredProcedureLineKey,
                        _storedProcedureLine.ToString(CultureInfo.InvariantCulture));
            }

            int count = 0;
            if (_parameters != null)
                foreach (string parameter in _parameters)
                    yield return new KeyValuePair<string, string>(ParameterPrefix + count++, parameter);

            count = 0;
            if (_innerExceptionGuids != null)
                foreach (CombGuid ieg in _innerExceptionGuids)
                    yield return new KeyValuePair<string, string>(InnerExceptionGuidsPrefix + count++, ieg.ToString());

            if (_context != null)
                foreach (KeyValuePair<string, string> kvp in _context)
                    yield return kvp;
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Reposts a log to the logging queue.
        /// </summary>
        /// <remarks>
        /// <para>Logs are added to the queue automatically, this allows a log to be 'relogged',
        /// i.e. reposted to the queue.  This should rarely be necessary but is useful for transferring logs between
        /// stores, etc.</para>
        /// <para>That said, this method should be used with extreme caution to avoid duplicate logging.</para>
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReLog()
        {
            // Post the log if the level is valid
            // We check here as exceptions always create a log (even if the level isn't valid).
            // It also reduces the race when the ValidLevels is changed.
            if (Level.IsValid(ValidLevels))
                _buffer.Add(this);
        }

        /// <summary>
        /// Formats the stack trace, skipping stack frames that form part of the exception construction.
        /// </summary>
        /// <param name="trace">The stack trace to format.</param>
        /// <returns>The formatted stack <paramref name="trace" />.</returns>
        [CanBeNull]
        private static String FormatStackTrace([CanBeNull] StackTrace trace)
        {
            // Check for stack trace frames.
            if (trace == null)
                return null;

            StackFrame[] frames = trace.GetFrames();
            if ((frames == null) ||
                (frames.Length < 1))
                return null;

            bool checkSkip = true;
            bool displayFilenames = true; // we'll try, but demand may fail
            const string wordAt = "at";
            const string inFileLineNum = "in {0}:line {1}";
            Type baseType = null;

            StringBuilder sb = new StringBuilder(255);
            foreach (StackFrame sf in frames)
            {
                Contract.Assert(sf != null);

                MethodBase mb = sf.GetMethod();
                if (mb == null)
                {
                    sb.Append("Could not retrieve method from stack frame.");
                    continue;
                }

                // We only check for frame skipping until we stop skipping.
                if (checkSkip)
                {
                    // We skip everything in this assembly.
                    if ((mb.DeclaringType != null) &&
                        (mb.DeclaringType.Assembly == LoggingAssembly))
                        continue;

                    // We only skip constructors.
                    if (mb is ConstructorInfo)
                    {
                        Type declaringType = mb.DeclaringType;
                        // If we've already seen this as a base type we can skip this frame.
                        if (declaringType == baseType)
                            continue;

                        // Look for inheritance from log or logging exception.
                        baseType = declaringType;
                        while ((baseType != typeof(object)) &&
                               (baseType != typeof(LoggingException)) &&
                               (baseType != typeof(Log)))
                        {
                            Contract.Assert(baseType != null);
                            baseType = baseType.BaseType;
                        }

                        if ((baseType == typeof(LoggingException)) ||
                            (baseType == typeof(Log)))
                        {
                            // We are descended from LoggingException or Log so skip frame.
                            baseType = declaringType;
                            continue;
                        }
                    }

                    // We are not part of the exception construction, so no longer check for frame skipping.
                    checkSkip = false;
                    baseType = null;
                }

                // Add newline if this isn't the first new line.
                sb.Append(Environment.NewLine);

                sb.AppendFormat("   {0} ", wordAt);

                Type t = mb.DeclaringType;
                // if there is a type (non global method) print it
                if (t != null)
                {
                    // ReSharper disable once PossibleNullReferenceException
                    sb.Append(t.FullName.Replace('+', '.'));
                    sb.Append(".");
                }
                sb.Append(mb.Name);

                // deal with the generic portion of the method 
                if (mb is MethodInfo &&
                    mb.IsGenericMethod)
                {
                    Type[] typars = mb.GetGenericArguments();
                    sb.Append("[");
                    int k = 0;
                    bool fFirstTyParam = true;
                    while (k < typars.Length)
                    {
                        if (fFirstTyParam == false)
                            sb.Append(",");
                        else
                            fFirstTyParam = false;

                        Contract.Assert(typars[k] != null);
                        sb.Append(typars[k].Name);
                        k++;
                    }
                    sb.Append("]");
                }

                // arguments printing
                sb.Append("(");
                ParameterInfo[] pi = mb.GetParameters();
                bool fFirstParam = true;
                foreach (ParameterInfo t1 in pi)
                {
                    Contract.Assert(t1 != null);

                    if (fFirstParam == false)
                        sb.Append(", ");
                    else
                        fFirstParam = false;

                    String typeName = "<UnknownType>";
                    // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                    if (t1.ParameterType != null)
                        typeName = t1.ParameterType.Name;
                    sb.Append(typeName + " " + t1.Name);
                }
                sb.Append(")");

                // source location printing
                if (!displayFilenames ||
                    (sf.GetILOffset() == -1)) continue;

                // If we don't have a PDB or PDB-reading is disabled for the module,
                // then the file name will be null. 
                String fileName = null;

                // Getting the filename from a StackFrame is a privileged operation - we won't want 
                // to disclose full path names to arbitrarily untrusted code.  Rather than just omit
                // this we could probably trim to just the filename so it's still mostly useful.
                try
                {
                    fileName = sf.GetFileName();
                }
                catch (NotSupportedException)
                {
                    // Having a deprecated stack modifier on the callstack (such as Deny) will cause
                    // a NotSupportedException to be thrown.  Since we don't know if the app can
                    // access the file names, we'll conservatively hide them.
                    displayFilenames = false;
                }
                catch (SecurityException)
                {
                    // If the demand for displaying filenames fails, then it won't 
                    // succeed later in the loop.  Avoid repeated exceptions by not trying again.
                    displayFilenames = false;
                }

                if (fileName == null) continue;

                // tack on e.g. " in c:\tmp\MyFile.cs:line 5" 
                sb.Append(' ');
                sb.AppendFormat(inFileLineNum, fileName, sf.GetFileLineNumber());
            }
            return sb.ToString();
        }

        /// <summary>
        /// Gets the value of the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        [CanBeNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string Get([NotNull] string key)
        {
            Contract.Requires(key != null);
            return this.FirstOrDefault(kvp => string.Equals(kvp.Key, key)).Value;
        }

        /// <summary>
        /// Gets all keys and their values that match the prefix.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <returns>IEnumerable{KeyValuePair{System.StringSystem.String}}.</returns>
        [NotNull]
        public IEnumerable<KeyValuePair<string, string>> GetPrefixed([NotNull] string prefix)
        {
            Contract.Requires(prefix != null);
            return this.Where(kvp => kvp.Key.StartsWith(prefix));
        }

        /// <summary>
        /// Adds the KVP.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="masterFormat">The master format.</param>
        /// <param name="indent">The indent.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="escaped">if set to <see langword="true" /> [escaped].</param>
        private void AddKVP(
            [NotNull] StringBuilder builder,
            MasterFormat masterFormat,
            [NotNull] string indent,
            [NotNull] string key,
            [CanBeNull] string value,
            bool escaped = false)
        {
            Contract.Requires(builder != null);
            Contract.Requires(indent != null);
            Contract.Requires(key != null);
            builder.Append(indent);
            switch (masterFormat)
            {
                case MasterFormat.Xml:
                    builder.Append('<');
                    key = key.Replace(' ', '_');
                    builder.Append(key);
                    if (value == null)
                    {
                        builder.Append(" />");
                        return;
                    }
                    builder.Append('>');
                    builder.Append(escaped ? value : value.XmlEscape());
                    builder.Append("</");
                    builder.Append(key);
                    builder.Append(">");
                    return;
                case MasterFormat.JSON:
                    builder.Append(key.ToJSON());
                    builder.Append(": ");
                    builder.Append(escaped ? (value ?? "null") : value.ToJSON());
                    return;
                default:
                    builder.Append(key);
                    int len = key.Length + indent.Length;
                    if (len < 17)
                        builder.Append(' ', 17 - len);
                    builder.Append(": ");
                    builder.Append(value);
                    return;
            }
        }

        /// <summary>
        /// The overall output format.
        /// </summary>
        private enum MasterFormat
        {
            Text,
            Xml,
            JSON
        }
    }
}