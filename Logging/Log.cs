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
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using JetBrains.Annotations;
using WebApplications.Utilities.Performance;

namespace WebApplications.Utilities.Logging
{
    /// <summary>
    /// Holds information about a single log item.
    /// </summary>
    [Serializable]
    public sealed partial class Log : IEquatable<Log>
    {
        /// <summary>
        /// The new log item performance counter.
        /// </summary>
        [NotNull]
        [NonSerialized]
        private static readonly PerfCounter _perfCounterNewItem =
            PerfCategory.GetOrAdd<PerfCounter>("Logged new item", "Tracks every time a log entry is logged.");

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
        public static readonly string ParameterKeyPrefix = LogContext.ReservePrefix("Parameter ", _logReservation);

        /// <summary>
        /// Reserved context key.
        /// </summary>
        [NotNull]
        [NonSerialized]
        public static readonly string MessageFormatKey = LogContext.ReserveKey("Message Format", _logReservation);

        /// <summary>
        /// Reserved context key.
        /// </summary>
        [NotNull]
        [NonSerialized]
        public static readonly string ExceptionTypeKey = LogContext.ReserveKey("Exception Type", _logReservation);

        /// <summary>
        /// Reserved context key.
        /// </summary>
        [NotNull]
        [NonSerialized]
        public static readonly string StackTraceKey = LogContext.ReserveKey("Stack Trace", _logReservation);

        /// <summary>
        /// Reserved context key.
        /// </summary>
        [NotNull]
        [NonSerialized]
        public static readonly string ThreadIDKey = LogContext.ReserveKey("Thread ID", _logReservation);

        /// <summary>
        /// Reserved context key.
        /// </summary>
        [NotNull]
        [NonSerialized]
        public static readonly string ThreadNameKey = LogContext.ReserveKey("Thread Name", _logReservation);

        /// <summary>
        /// Reserved context key.
        /// </summary>
        [NotNull]
        [NonSerialized]
        public static readonly string StoredProcedureKey = LogContext.ReserveKey("Stored Procedure", _logReservation);

        /// <summary>
        /// Reserved context key.
        /// </summary>
        [NotNull]
        [NonSerialized]
        public static readonly string StoredProcedureLineKey = LogContext.ReserveKey("Stored Procedure Line", _logReservation);

        /// <summary>
        /// The logging assembly
        /// </summary>
        [NotNull]
        [NonSerialized]
        internal static readonly Assembly LoggingAssembly = typeof(Log).Assembly;

        /// <summary>
        ///   A Guid used to group log items together.
        /// </summary>
        [UsedImplicitly]
        public readonly CombGuid Group;

        /// <summary>
        ///   A Guid used to uniquely identify a log item.
        /// </summary>
        [UsedImplicitly]
        public readonly CombGuid Guid;

        /// <summary>
        ///   The <see cref="LoggingLevel">log level</see>.
        /// </summary>
        [UsedImplicitly]
        public readonly LoggingLevel Level;

        /// <summary>
        ///   The log message.
        /// </summary>
        [UsedImplicitly]
        [NotNull]
        public readonly string Message;

        /// <summary>
        ///   Gets a <see cref="bool"/> value indicating whether this instance was generated from an exception.
        /// </summary>
        /// <value>
        ///   Returns <see langword="true"/> if this instance was an exception; otherwise <see langword="false"/>.
        /// </value>
        [UsedImplicitly]
        public readonly bool IsException;

        /// <summary>
        ///   The time stamp of when the log item was created.
        /// </summary>
        [UsedImplicitly]
        public DateTime TimeStamp
        {
            get { return Guid.Created; }
        }

        /// <summary>
        ///   The <see cref="LogContext">context</see> information for the log item.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        private readonly LogContext Context;

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        [NotNull]
        public IEnumerable<string> Parameters { get { return Context.GetPrefixed(ParameterKeyPrefix).Select(kvp => kvp.Value); } }

        /// <summary>
        /// Gets the stack trace.
        /// </summary>
        /// <value>The stack trace.</value>
        [NotNull]
        public string StackTrace { get { return Context.Get(StackTraceKey); } }

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
            Guid = CombGuid.NewCombGuid();

            // Create a context, based on the original context, adding the parameters.
            Context = new LogContext(context, _logReservation, ParameterKeyPrefix, parameters);
            Context.Set(_logReservation, MessageFormatKey, format);

            // Add stack trace
            Context.Set(_logReservation, StackTraceKey, FormatStackTrace(new StackTrace(2, true)));

            if (exception != null)
            {
                IsException = true;
                Context.Set(_logReservation, ExceptionTypeKey, exception.GetType().ToString());

                // If this is a SQL exception, then log the stored proc.
                SqlException sqlException = exception as SqlException;
                if (sqlException != null)
                {
                    Context.Set(_logReservation, StoredProcedureKey, String.IsNullOrEmpty(sqlException.Procedure) ? "<Unknown>" : sqlException.Procedure);
                    Context.Set(_logReservation, StoredProcedureLineKey, sqlException.LineNumber);
                }
            }

            // Get the current thread information
            Thread currentThread = Thread.CurrentThread;
            int threadId = currentThread.ManagedThreadId;
            Context.Set(_logReservation, ThreadIDKey, threadId);
            Context.Set(_logReservation, ThreadNameKey,
                         String.IsNullOrWhiteSpace(currentThread.Name)
                             ? threadId.ToString()
                             : currentThread.Name);

            // Set provided properties
            Group = logGroup.Equals(CombGuid.Empty)
                        ? CombGuid.NewCombGuid()
                        : logGroup;

            // Attempt to format string safely.
            // ReSharper disable ConditionIsAlwaysTrueOrFalse
            Message = format == null ? String.Empty : format.SafeFormat(parameters);
            // ReSharper restore ConditionIsAlwaysTrueOrFalse

            // Set the level.
            Level = level;

            // Queue the log entry.
            _logQueue.Enqueue(this);

            // Increment performance counter.
            _perfCounterNewItem.Increment();

            // Signal monitor thread of new arrival.
            _logSignal.Set();
        }

        /// <summary>
        ///   Formats the stack trace, skipping stack frames that form part of the exception construction.
        /// </summary>
        /// <param name="trace">The stack trace to format.</param>
        /// <returns>The formatted stack <paramref name="trace"/>.</returns>
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

            StringBuilder sb = new StringBuilder(255);
            foreach (StackFrame sf in frames)
            {
                MethodBase mb = sf.GetMethod();
                if (mb == null)
                {
                    sb.AppendFormat("Could not retrieve method from stack frame.");
                    continue;
                }

                // We only check for frame skipping until we stop skipping.
                if (checkSkip)
                {
                    // We skip everything in this assembly.
                    if ((mb.DeclaringType != null) &&
                        (mb.DeclaringType.Assembly == LoggingAssembly))
                        continue;

                    // We are not part of this assembly, so no longer check for frame skipping.
                    checkSkip = false;
                }

                // Add newline if this isn't the first new line.
                sb.Append(Environment.NewLine);

                sb.AppendFormat(CultureInfo.InvariantCulture, "   {0} ", word_At);

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
                sb.AppendFormat(CultureInfo.InvariantCulture, inFileLineNum, fileName, sf.GetFileLineNumber());
            }
            return sb.ToString();
        }

        #region IEquatable<Log> Members
        /// <summary>
        ///   Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">The log item to compare the instance with.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the current log item is equal to the <paramref name="other"/> log item specified;
        ///   otherwise returns <see langword="false"/>.
        /// </returns>
        public bool Equals(Log other)
        {
            return other != null && other.Guid == Guid;
        }
        #endregion

        /// <summary>
        ///   Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>
        ///   A <see cref="string"/> representation of this instance. The format strings can be changed in the 
        ///   Resources.resx resource file at the key 'LogToString' and 'LogException'
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   The format string was a <see langword="null"/>.
        /// </exception>
        /// <exception cref="FormatException">
        ///   An index from the format string is either less than zero or greater than or equal to the number of arguments.
        /// </exception>
        [UsedImplicitly]
        public override string ToString()
        {
            string s = new string('=', 80);
            const string indent = "\t";

            StringBuilder builder = new StringBuilder();

            builder.AppendLine(s);
            builder.Append("\"");
            builder.Append(Message);
            builder.AppendLine("\"");

            DateTime timestamp = TimeStamp;
            builder.AppendFormat(IsException
                               ? Resources.Log_ToString_Exception_Header
                               : Resources.Log_ToString_Log_Header,
                               timestamp.Date,
                               timestamp.TimeOfDay);
            builder.AppendLine();

            builder.Append(indent);
            builder.Append("Level: ");
            builder.AppendLine(Level.ToString());

            builder.Append(indent);
            builder.Append("ID: ");
            builder.AppendLine(Guid.ToString());

            builder.Append(indent);
            builder.Append("Group: ");
            builder.AppendLine(Group.ToString());

            builder.Append(indent);
            builder.Append("Context: ");
            builder.AppendLine(Context.ToString());

            builder.AppendLine(s);

            return builder.ToString();
        }
    }
}