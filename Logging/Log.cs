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
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using System.Xml.Serialization;
using JetBrains.Annotations;
using WebApplications.Utilities.Performance;

namespace WebApplications.Utilities.Logging
{
    /// <summary>
    /// Holds information about a single log item.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("{Message} @ {TimeStamp}")]
    public partial class Log : IEnumerable<KeyValuePair<string, string>>, IFormattable
    {
        /// <summary>
        /// The context (holds all log data).
        /// </summary>
        [NotNull]
        private readonly Dictionary<string, string> _context;

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
        ///   A Guid used to group log items together.
        /// </summary>
        /// <remarks>
        /// <para>If the log item is not part of a group this always return <see cref="Guid"/>.</para>
        /// </remarks>
        [UsedImplicitly]
        public CombGuid Group
        {
            get
            {
                string gStr = Get(GroupKey);
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
            get
            {
                string lStr = Get(LevelKey);
                if (lStr == null) return LoggingLevel.Information;
                LoggingLevel l;
                return _levels.TryGetValue(lStr, out l) ? l : LoggingLevel.Information;
            }
        }

        /// <summary>
        ///   The formatted log message.
        /// </summary>
        [UsedImplicitly]
        [NotNull]
        public virtual string Message { get { return MessageFormat.SafeFormat(Parameters.Cast<object>().ToArray()); } }

        /// <summary>
        ///   Gets a <see cref="bool"/> value indicating whether this instance was generated from an exception.
        /// </summary>
        /// <value>
        ///   Returns <see langword="true"/> if this instance was an exception; otherwise <see langword="false"/>.
        /// </value>
        [UsedImplicitly]
        public bool IsException { get { return _context.ContainsKey(ExceptionTypeFullNameKey); } }

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
        public IEnumerable<string> Parameters { get { return _context.Where(kvp => kvp.Key.StartsWith(ParameterKeyPrefix)).Select(kvp => kvp.Value); } }

        /// <summary>
        /// Gets the stack trace.
        /// </summary>
        /// <value>The stack trace.</value>
        public string StackTrace { get { return Get(StackTraceKey); } }

        /// <summary>
        /// Gets the message format.
        /// </summary>
        /// <value>The message format.</value>
        [NotNull]
        public string MessageFormat { get { return Get(MessageFormatKey) ?? string.Empty; } }

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
        public string ThreadName { get { return Get(ThreadNameKey); } }
        
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
        public Log(IEnumerable<KeyValuePair<string, string>> context)
        {
            _context = context.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            string s;
            CombGuid guid;
            if (!_context.TryGetValue(GuidKey, out s) || 
                !CombGuid.TryParse(s, out guid))
                throw new LoggingException("The log deserialization did not supply a valid GUID.");

            if (_context.TryGetValue(GroupKey, out s) &&
                !CombGuid.TryParse(s, out guid))
                throw new LoggingException("The log deserialization supplied an invalid Group.");

            LoggingLevel level;
            if (_context.TryGetValue(LevelKey, out s) &&
                !_levels.ContainsKey(s.ToLower()))
                throw new LoggingException("The log deserialization supplied an invalid Level.");

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
        /// <param name="logGroup">The unique ID that groups log items together.</param>
        /// <param name="context">The context information.</param>
        /// <param name="exception">The exception. If none then pass <see langword="null" />.</param>
        /// <param name="level">The log level.</param>
        /// <param name="format">The format.</param>
        /// <param name="parameters">The parameters.</param>
        [StringFormatMethod("message")]
        private Log(
            CombGuid logGroup,
            [CanBeNull] LogContext context,
            [CanBeNull] Exception exception,
            LoggingLevel level,
            [NotNull] string format,
            [NotNull] params object[] parameters)
        {
            var guid = CombGuid.NewCombGuid();

            // Dictionary size
            int size = parameters.Length + 9 + (context == null ? 0 : context.Count);

            // Build context.
            _context = new Dictionary<string, string>(size);

            if (context != null)
            {
                // Copy supplied context into our context.
                foreach (var kvp in context)
                    _context.Add(kvp.Key, kvp.Value);
            }

            // Get the current thread information
            Thread currentThread = Thread.CurrentThread;
            int threadId = currentThread.ManagedThreadId;

            // We can safely add our data due to the way LogContext protects reservations.
            _context.Add(LevelKey, level.ToString());
            _context.Add(GuidKey, CombGuid.NewCombGuid().ToString());
            // Only add group if specified.
            if (!logGroup.Equals(CombGuid.Empty))
                _context.Add(GroupKey, logGroup.ToString());
            _context.Add(ThreadIDKey, threadId.ToString());
            if (!string.IsNullOrWhiteSpace(currentThread.Name))
                _context.Add(ThreadNameKey, currentThread.Name);
            _context.Add(MessageFormatKey, format);

            for (int p = 0; p < parameters.Length; p++)
            {
                object v = parameters[p];
                _context.Add(ParameterKeyPrefix + " " + p, v == null ? null : v.ToString());
            }

            string stackTrace = null;
            if (exception != null)
            {
                _context.Add(ExceptionTypeFullNameKey, exception.GetType().ToString());

                // If we're a logging exception we're being called from the constructor, so we
                // use our own stack trace formatter which is superior.
                if (!(exception is LoggingException))
                {
                    // Use exception stack trace if available
                    stackTrace = exception.StackTrace;

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

            // Add stack trace.
            _context.Add(StackTraceKey,
                         String.IsNullOrWhiteSpace(stackTrace)
                             ? FormatStackTrace(new StackTrace(2, true))
                             : stackTrace);

            // Post log onto queue.
            ReLog();

            // Increment performance counter.
            _perfCounterNewItem.Increment();
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
        public void ReLog()
        {
            // Post the log if the level is valid
            // We check here as exceptions always create a log (even if the level isn't valid).
            // It also reduces the race when the ValidLevels is changed.
            if (Level.IsValid(ValidLevels))
                lock (_queue)
                    _queue.Enqueue(this);
        }

        /// <summary>
        /// Formats the stack trace, skipping stack frames that form part of the exception construction.
        /// </summary>
        /// <param name="trace">The stack trace to format.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>The formatted stack <paramref name="trace" />.</returns>
        private static String FormatStackTrace(StackTrace trace)
        {
            // Check for stack trace frames.
            if (trace == null)
                return "No stack trace available.";

            StackFrame[] frames = trace.GetFrames();
            if ((frames == null) || (frames.Length < 1))
                return "No stack trace frames available.";

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
        /// Gets the enumerator.
        /// </summary>
        /// <returns>IEnumerator{KeyValuePair{System.StringSystem.String}}.</returns>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _context.GetEnumerator();
        }

        /// <summary>
        /// Gets the value of the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        [CanBeNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string Get(string key)
        {
            string value;
            return _context.TryGetValue(key, out value) ? value : null;
        }

        /// <summary>
        /// Gets all keys and their values that match the prefix.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <returns>IEnumerable{KeyValuePair{System.StringSystem.String}}.</returns>
        [NotNull]
        public IEnumerable<KeyValuePair<string, string>> GetPrefixed(string prefix)
        {
            return _context.Where(kvp => kvp.Key.StartsWith(prefix));
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The value associated with the specified key. If the specified key is not found, throws a <see cref="T:System.Collections.Generic.KeyNotFoundException" />.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key" /> is null.</exception>
        /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">The property is retrieved and <paramref name="key" /> does not exist in the collection.</exception>
        [CanBeNull]
        public string this[[NotNull]string key]
        {
            get
            {
                return _context[key];
            }
        }

        #region ToString overloads
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
        /// <param name="format">The format.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        [NotNull]
        public string ToString(string format)
        {
            return ToString(format, null);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        [NotNull]
        public string ToString(IFormatProvider formatProvider)
        {
            return ToString(null, formatProvider);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="format">The format to use.-or- A null reference (Nothing in Visual Basic) to use the default format defined for the type of the <see cref="T:System.IFormattable" /> implementation.</param>
        /// <param name="formatProvider">The provider to use to format the value.-or- A null reference (Nothing in Visual Basic) to obtain the numeric format information from the current locale setting of the operating system.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == null) format = LogFormat.General.ToString();

            if (formatProvider != null)
            {
                ICustomFormatter formatter = formatProvider.GetFormat(typeof(Log)) as ICustomFormatter;

                if (formatter != null)
                    return formatter.Format(format, this, formatProvider);
            }

            // Try to get the actual format if specified as an enum.
            LogFormat logFormat;
            if (_formats.TryGetValue(format.ToLower(), out logFormat) || Enum.TryParse(format, true, out logFormat))
                return ToString(logFormat);

            // Parse format
            StringBuilder builder = new StringBuilder(format.Length);
            int i = 0;
            bool inTag = false;
            bool escape = false;
            StringBuilder tag = new StringBuilder(16);
            while (i < format.Length)
            {
                char c = format[i++];

                switch (c)
                {
                    case '\\':
                        if (!escape)
                        {
                            escape = true;
                            continue;
                        }
                        break;
                    case '{':
                        if (!escape)
                        {
                            // If we're already in a tag start a new tag, and dump out previous one as not a tag.
                            if (inTag)
                            {
                                builder.Append('{');
                                builder.Append(tag);
                                tag.Clear();
                            }
                            inTag = true;
                            continue;
                        }
                        break;
                    case '}':
                        if (!escape && inTag)
                        {
                            // Finished a tag.
                            string t = tag.ToString();
                            tag.Clear();

                            string cv;
                            if (_context.TryGetValue(t, out cv))
                                // We have a context key.
                                builder.Append(cv);
                            else if (_formats.TryGetValue(t.ToLower(), out logFormat) ||
                                     Enum.TryParse(t, true, out logFormat))
                                // We have a standard formatter.
                                builder.Append(ToString(logFormat));
                            else
                            {
                                // We didn't match anything
                                builder.Append('{');
                                builder.Append(t);
                                builder.Append('}');
                            }
                            inTag = false;
                            continue;
                        }
                        break;
                }

                // Output the character to the current tag or the builder.
                if (inTag)
                    tag.Append(c);
                else
                    builder.Append(c);
                escape = false;
            }

            // Output our formatted string.
            return builder.ToString();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        /// <exception cref="System.FormatException"></exception>
        [NotNull]
        public string ToString(LogFormat format)
        {
            const int headerSize = 120;
            if (format == LogFormat.None)
                return String.Empty;

            // Get option flags
            bool includeMissing = format.HasFlag(LogFormat.IncludeMissing);
            bool includeHeader = format.HasFlag(LogFormat.Header);
            // Remove option flags
            format = ((LogFormat) (((int) format) & 0x9FFFFFFF));

            // If we are a combination of keys then include keys.
            bool includeKey = format.IsCombinationFlag(true);

            // Otherwise always inclue value.
            if (!includeKey)
                includeMissing = true;

            StringBuilder builder = new StringBuilder();
            if (includeHeader)
                builder.AppendLine(Header);
            foreach (LogFormat flag in format.SplitFlags(true))
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
                        value = TimeStamp.ToString("o");
                        break;
                    case LogFormat.Level:
                        key = "Level";
                        value = Level.ToString();
                        break;
                    case LogFormat.Guid:
                        key = "Guid";
                        var guid = Guid;
                        value = guid == CombGuid.Empty ? null : guid.ToString();
                        break;
                    case LogFormat.Group:
                        key = "Group";
                        var group = Group;
                        value = group == CombGuid.Empty ? null : group.ToString();
                        break;
                    case LogFormat.Context:
                        key = "Context";
                        value = GetContext();
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
                            if (!string.IsNullOrWhiteSpace(sline) &&
                                (int.TryParse(sline, out sl)))
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
                        value = threadID < 0 ? null : threadID.ToString();
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
                        value = ApplicationGuid.ToString();
                        break;

                    default:
                        throw new FormatException(String.Format("Unexpected singular format '{0}'.", flag));
                }
                if (value == null)
                {
                    if (includeMissing)
                        value = string.Empty;
                    else continue;
                }
                if (includeKey)
                {
                    builder.Append(key);
                    builder.Append(' ', 17 - key.Length);
                    builder.Append(": ");
                    builder.AppendLine(value);
                }
                else
                {
                    builder.Append(value);
                }
            }
            if (includeHeader)
                builder.AppendLine(Header);
            return builder.ToString();
        }
        #endregion

        /// <summary>
        /// Appends the additional context.
        /// </summary>
        /// <returns>Context value</returns>
        private string GetContext()
        {
            KeyValuePair<string, string>[] remainingContext =
                this.Where(kvp => !kvp.Key.StartsWith(LogKeyPrefix)).ToArray();
            if (remainingContext.Length < 1) return string.Empty;

            StringBuilder builder = new StringBuilder();
            foreach (KeyValuePair<string, string> kvp in remainingContext)
            {
                builder.AppendLine();
                builder.Append("   ");
                if (kvp.Key == null)
                    builder.Append("null");
                else
                {
                    builder.Append("'");
                    builder.Append(kvp.Key);
                    builder.Append("'");
                }
                builder.Append(" = ");
                if (kvp.Value == null)
                    builder.Append("null");
                else
                {
                    builder.Append("'");
                    builder.Append(kvp.Value);
                    builder.Append("'");
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}