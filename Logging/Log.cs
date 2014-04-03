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
using System.Data.SqlClient;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Logging
{
    /// <summary>
    /// Holds information about a single log item.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("{Message} @ {TimeStamp}")]
    [PublicAPI]
    public partial class Log : IEnumerable<KeyValuePair<string, string>>, IFormattable
    {
        /// <summary>
        /// The context (holds all log data).
        /// </summary>
        [NotNull]
        private readonly Dictionary<string, string> _context;

        /// <summary>
        /// Cached level as used so frequently
        /// </summary>
        [NonSerialized]
        private LoggingLevel _level;

        /// <summary>
        /// Called when deserializing.
        /// </summary>
        /// <exception cref="LoggingException">
        /// The log deserialization did not supply a valid GUID.
        /// or
        /// The log deserialization supplied an invalid Group.
        /// or
        /// The log deserialization supplied an invalid Level.
        /// or
        /// The log deserialization supplied an invalid Thread ID.
        /// or
        /// The log deserialization supplied an invalid Stored Procedure line number.
        /// </exception>
        [OnDeserialized]
        private void OnDeserialize(StreamingContext context)
        {
            string s;
            CombGuid guid;
            if (!_context.TryGetValue(GuidKey, out s) ||
                !CombGuid.TryParse(s, out guid))
                throw new LoggingException("The log deserialization did not supply a valid GUID.");

            LoggingLevel level = LoggingLevel.Information;
            if (_context.TryGetValue(LevelKey, out s) &&
                !_levels.TryGetValue(s, out level))
                throw new LoggingException("The log deserialization supplied an invalid Level.");

            // We cache the level as it is checked so frequently.
            _level = level;

            int i;
            if (_context.TryGetValue(ThreadIDKey, out s) &&
                !Int32.TryParse(s, out i))
                throw new LoggingException("The log deserialization supplied an invalid Thread ID.");

            if (_context.TryGetValue(StoredProcedureLineKey, out s) &&
                !Int32.TryParse(s, out i))
                throw new LoggingException("The log deserialization supplied an invalid Stored Procedure line number.");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Log" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <remarks>
        /// <para>This is used for deserializing Log entries - it does not result in logs being added!</para>
        /// <para>To add logs use <see cref="Log.Add(string, object[])" /> instead.</para>
        /// <para>You can create partial logs, however the context must contain at least the 
        /// <see cref="GuidKey">Guid key</see>, and be a valid <see cref="CombGuid"/>.</para>
        /// <para>Typed keys must also be valid if supplied otherwise an exception will be thrown.</para>
        /// </remarks>
        public Log([NotNull] IEnumerable<KeyValuePair<string, string>> context)
        {
            Contract.Requires(context != null);
            _context = context.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            OnDeserialize(default(StreamingContext));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Log" /> class.
        /// </summary>
        /// <param name="context">The context information.</param>
        /// <param name="exception">The exception. If none then pass <see langword="null" />.</param>
        /// <param name="level">The log level.</param>
        /// <param name="format">The format.</param>
        /// <param name="parameters">The parameters.</param>
        /// <remarks>
        /// <para>
        /// If you don't need the <see cref="Log"/> you should use <see cref="Log.Add(LogContext, Exception, LoggingLevel, string, object[])"/> instead
        /// as it won't create the <see cref="Log"/> object if the <see paramref="level"/> isn't a <see cref="ValidLevels">valid level</see>.
        /// </para></remarks>
        [StringFormatMethod("format")]
        public Log(
            [CanBeNull] LogContext context,
            [CanBeNull] Exception exception,
            LoggingLevel level,
            [CanBeNull] string format,
            [CanBeNull] params object[] parameters)
        {
            CombGuid guid = CombGuid.NewCombGuid();

            // Estimate dictionary size
            int size = 9 +
                (parameters == null ? 0 : parameters.Length) +
                (context == null ? 0 : context.Count);

            // Build context.
            _context = new Dictionary<string, string>(size);
            if (context != null)
            {
                // Lock the context to prevent changes after it is locked.
                context.Lock();

                // Copy supplied context into our context.
                foreach (KeyValuePair<string, string> kvp in context)
                    _context.Add(kvp.Key, kvp.Value);
            }

            // We can safely add our data due to the way LogContext protects reservations.
            _context.Add(LevelKey, level.ToString());
            _level = level;

            _context.Add(GuidKey, guid.ToString());

            // Get the current thread information
            Thread currentThread = Thread.CurrentThread;
            int threadId = currentThread.ManagedThreadId;
            _context.Add(ThreadIDKey, threadId.ToString());
            if (!string.IsNullOrEmpty(currentThread.Name))
                _context.Add(ThreadNameKey, currentThread.Name);

            string stackTrace = null;
            bool hasMessage = false;

            // If we have a formatted message add it now
            if (!string.IsNullOrEmpty(format))
            {
                hasMessage = true;
                _context.Add(MessageFormatKey, format);

                if ((parameters == null) || (parameters.Length < 1))
                    _context.Add(ParameterCountKey, "0");
                else
                {
                    _context.Add(ParameterCountKey, parameters.Length.ToString());
                    for (int p = 0; p < parameters.Length; p++)
                    {
                        object v = parameters[p];
                        if (v == null) continue;
                        _context.Add(ParameterPrefix + " " + p, v.ToString());
                    }
                }

                // Grab the current stack trace
                stackTrace = FormatStackTrace(new StackTrace(2, true));
            }

            Exception[] innerExceptions = null;
            if (exception != null)
            {
                if (hasMessage)
                {
                    innerExceptions = new[] { exception };
                }
                else
                {
                    // Add the exception type.
                    _context.Add(ExceptionTypeFullNameKey, exception.GetType().ToString());
                    _context.Add(MessageFormatKey, exception.Message);
                    _context.Add(ParameterCountKey, "0");

                    stackTrace = exception.StackTrace;

                    // Check for aggregate exception
                    AggregateException aggregateException = exception as AggregateException;
                    if (aggregateException != null)
                    {
                        innerExceptions = aggregateException.InnerExceptions != null
                            ? aggregateException.InnerExceptions.ToArray()
                            : null;
                    }
                    else
                    {
                        if (exception.InnerException != null)
                            innerExceptions = new[] { exception.InnerException };

                        // If this is a SQL exception, then log the stored proc.
                        SqlException sqlException = exception as SqlException;
                        if (sqlException != null)
                        {
                            _context.Add(StoredProcedureKey,
                                String.IsNullOrEmpty(sqlException.Procedure) ? "<Unknown>" : sqlException.Procedure);
                            _context.Add(StoredProcedureLineKey, sqlException.LineNumber.ToString());
                        }
                    }
                }
            }

            if (innerExceptions != null)
            {
                // Link to inner exceptions
                foreach (var innerException in innerExceptions)
                {
                    LoggingException le = innerException as LoggingException;

                    _context.Add(InnerExceptionGuidKey,
                        le != null
                            ? le.Guid.ToString()
                            : new Log(new LogContext().Set(_logReservation, InnerExceptionGuidKey, guid),
                                innerException, LoggingLevel.Error, null).Guid.ToString());
                }
            }

            if (!String.IsNullOrWhiteSpace(stackTrace))
                _context.Add(StackTraceKey, stackTrace);

            // Increment performance counter.
            _perfCounterNewItem.Increment();

            // Post log onto queue
            ReLog();
        }

        /// <summary>
        ///   A Guid used to uniquely identify a log item.
        /// </summary>
        [UsedImplicitly]
        public CombGuid Guid
        {
            get
            {
                string gStr = Get(GuidKey);
                if (gStr == null) return CombGuid.Empty;
                CombGuid g;
                return CombGuid.TryParse(gStr, out g) ? g : CombGuid.Empty;
            }
        }

        /// <summary>
        ///   The <see cref="LoggingLevel">log level</see>.
        /// </summary>
        [UsedImplicitly]
        public LoggingLevel Level
        {
            get { return _level; }
        }

        /// <summary>
        ///   The formatted log message.
        /// </summary>
        [PublicAPI]
        [CanBeNull]
        public virtual string Message
        {
            get { return MessageFormat == null ? null : MessageFormat.SafeFormat(Parameters.Cast<object>().ToArray()); }
        }

        /// <summary>
        ///   Gets a <see cref="bool"/> value indicating whether this instance was generated from an exception.
        /// </summary>
        /// <value>
        ///   Returns <see langword="true"/> if this instance was an exception; otherwise <see langword="false"/>.
        /// </value>
        [UsedImplicitly]
        public bool IsException
        {
            get { return _context.ContainsKey(ExceptionTypeFullNameKey); }
        }

        /// <summary>
        ///   The time stamp of when the log item was created.
        /// </summary>
        [UsedImplicitly]
        public DateTime TimeStamp
        {
            get { return Guid.Created; }
        }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        [NotNull]
        public IEnumerable<string> Parameters
        {
            get
            {
                return _context
                    .Where(kvp => kvp.Key.StartsWith(ParameterPrefix))
                    // TODO ORDERING!
                    .Select(kvp => kvp.Value);
            }
        }

        /// <summary>
        /// Gets the stack trace.
        /// </summary>
        /// <value>The stack trace.</value>
        [CanBeNull]
        public string StackTrace
        {
            get { return Get(StackTraceKey); }
        }

        /// <summary>
        /// Gets the message format.
        /// </summary>
        /// <value>The message format.</value>
        [CanBeNull]
        public string MessageFormat
        {
            get { return Get(MessageFormatKey); }
        }

        /// <summary>
        /// Gets the thread ID (or -1 if not known).
        /// </summary>
        /// <value>The thread ID.</value>
        public int ThreadID
        {
            get
            {
                string iStr = Get(ThreadIDKey);
                if (iStr == null) return -1;
                int i;
                return Int32.TryParse(iStr, out i) ? i : -1;
            }
        }

        /// <summary>
        /// Gets the name of the thread.
        /// </summary>
        /// <value>The name of the thread.</value>
        [CanBeNull]
        public string ThreadName
        {
            get { return Get(ThreadNameKey); }
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The value associated with the specified key. If the specified key is not found, throws a <see cref="T:System.Collections.Generic.KeyNotFoundException" />.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key" /> is null.</exception>
        /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">The property is retrieved and <paramref name="key" /> does not exist in the collection.</exception>
        [CanBeNull]
        public string this[[NotNull] string key]
        {
            get { return _context[key]; }
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
            if (format == null) format = LogFormat.General.ToString();

            if (formatProvider != null)
            {
                ICustomFormatter formatter = formatProvider.GetFormat(typeof(Log)) as ICustomFormatter;

                if (formatter != null)
                    return formatter.Format(format, this, formatProvider) ?? String.Empty;
            }

            // Try to get the actual format if specified as an enum.
            LogFormat logFormat;
            if (_formats.TryGetValue(format, out logFormat) || Enum.TryParse(format, true, out logFormat))
                return ToString(logFormat);

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
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        [NotNull]
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
        public string ToString(LogFormat format, [CanBeNull] string options = null)
        {
            if (format == LogFormat.None)
                return String.Empty;
            StringBuilder builder = new StringBuilder();
            AppendFormatted(builder, format, options);
            return builder.ToString();
        }

        /// <summary>
        /// Appends the the formatted information.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="format">The format.</param>
        /// <param name="options">The options.</param>
        /// <exception cref="System.FormatException">
        /// </exception>
        private void AppendFormatted([NotNull] StringBuilder builder, LogFormat format,
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
            {
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

                // This is a single value format, just output the value directly
                switch (flag)
                {
                    case LogFormat.Message:
                        key = "Message";
                        value = Message;
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
                        value = Get(ExceptionTypeFullNameKey) ?? null;
                        break;
                    case LogFormat.SQLException:
                        key = "Stored Procedure";
                        value = Get(StoredProcedureKey);
                        if (value != null)
                        {
                            string sline = Get(StoredProcedureLineKey);
                            int sl;
                            if (!String.IsNullOrWhiteSpace(sline) &&
                                (Int32.TryParse(sline, out sl)))
                                value += " at line " + sl.ToString();
                        }
                        break;
                    case LogFormat.StackTrace:
                        key = "Stack Trace";
                        value = StackTrace;
                        break;
                    case LogFormat.ThreadID:
                        key = "Thread ID";
                        int threadID = ThreadID;
                        value = threadID < 0 ? null : threadID.ToString(options ?? "G");
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
                        KeyValuePair<string, string>[] remainingContext =
                            this.Where(kvp => !kvp.Key.StartsWith(LogKeyPrefix)).ToArray();
                        key = "Context";

                        if (remainingContext.Length < 1)
                            value = null;
                        else
                        {
                            StringBuilder cv = new StringBuilder();
                            cv.AppendLine(masterFormat == MasterFormat.JSON ? "{" : String.Empty);

                            string i = indent + "   ";
                            bool cvf = true;
                            foreach (KeyValuePair<string, string> kvp in remainingContext)
                            {
                                if (!cvf)
                                    cv.AppendLine(masterFormat == MasterFormat.JSON ? "," : String.Empty);
                                cvf = false;
                                AddKVP(cv, masterFormat, i, kvp.Key, kvp.Value);
                            }
                            switch (masterFormat)
                            {
                                case MasterFormat.Xml:
                                    cv.AppendLine();
                                    cv.Append(indent);
                                    break;
                                case MasterFormat.JSON:
                                    cv.Append("}");
                                    break;
                            }
                            value = cv.ToString();
                        }
                        break;

                    default:
                        throw new FormatException(String.Format(Resources.Log_Invalid_Format_Singular, flag));
                }
                if (value == null && !includeMissing)
                    continue;

                if (includeKey)
                {
                    if (!first)
                        builder.AppendLine(masterFormat == MasterFormat.JSON ? "," : String.Empty);

                    AddKVP(builder, masterFormat, indent, key, value, flag == LogFormat.Context);

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
                        builder.AppendLine("}");
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
        /// <returns>IEnumerator{KeyValuePair{System.StringSystem.String}}.</returns>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _context.GetEnumerator();
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
        private static String FormatStackTrace(StackTrace trace)
        {
            // Check for stack trace frames.
            if (trace == null)
                return null;

            StackFrame[] frames = trace.GetFrames();
            if ((frames == null) || (frames.Length < 1))
                return null;

            bool checkSkip = true;
            bool displayFilenames = true; // we'll try, but demand may fail
            const string word_At = "at";
            const string inFileLineNum = "in {0}:line {1}";
            Type baseType = null;

            StringBuilder sb = new StringBuilder(255);
            foreach (StackFrame sf in frames)
            {
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
                            baseType = baseType.BaseType;

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

                sb.AppendFormat("   {0} ", word_At);

                Type t = mb.DeclaringType;
                // if there is a type (non global method) print it
                if (t != null)
                {
                    sb.Append(t.FullName.Replace('+', '.'));
                    sb.Append(".");
                }
                sb.Append(mb.Name);

                // deal with the generic portion of the method 
                if (mb is MethodInfo && mb.IsGenericMethod)
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
                    if (fFirstParam == false)
                        sb.Append(", ");
                    else
                        fFirstParam = false;

                    String typeName = "<UnknownType>";
                    if (t1.ParameterType != null)
                        typeName = t1.ParameterType.Name;
                    sb.Append(typeName + " " + t1.Name);
                }
                sb.Append(")");

                // source location printing
                if (!displayFilenames || (sf.GetILOffset() == -1)) continue;

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
            string value;
            return _context.TryGetValue(key, out value) ? value : null;
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
            return _context.Where(kvp => kvp.Key.StartsWith(prefix));
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
        private void AddKVP([NotNull] StringBuilder builder, MasterFormat masterFormat, [NotNull] string indent,
            [NotNull] string key, [CanBeNull] string value, bool escaped = false)
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
                    int len = key.Length;
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