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
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security;
using System.Text;
using System.Xml.Linq;
using JetBrains.Annotations;
using WebApplications.Utilities.Logging.Performance;

namespace WebApplications.Utilities.Logging
{
    /// <summary>
    ///   Implements a specialised Exception handler used throughout Babel, automatically logs errors
    ///   and the context in which they occurred. Also makes use of Babel's late-binding translation.
    ///   BabelException should always be used where exceptions are thrown.
    /// </summary>
    [Serializable]
    public class LoggingException : ApplicationException, ISerializable
    {
        [NonSerialized]
        private const string AttributeType = "type";
        [NonSerialized]
        private const string AttributeLevel = "level";
        [NonSerialized]
        private const string AttributeLogGroup = "logGroup";
        [NonSerialized]
        private const string NodeError = "Error";
        [NonSerialized]
        private const string NodeMessage = "Message";
        [NonSerialized]
        private const string NodeStackTrace = "StackTrace";
        [NonSerialized]
        private const string NodeInnerException = "InnerException";

        /// <summary>
        ///   The <see cref="LogContext"/> for the exception.
        /// </summary>
        [UsedImplicitly]
        public readonly LogContext Context;

        /// <summary>
        ///   The logging level.
        /// </summary>
        public readonly LogLevel Level;

        /// <summary>
        ///   The logging group <see cref="Guid"/>.
        /// </summary>
        public readonly Guid LogGroup;

        /// <summary>
        ///   Holds the parameters as an <see cref="Array"/>.
        /// </summary>
        [NotNull]
        private readonly string[] _parameters;

        /// <summary>
        ///   Caches the XML version of the exception so that it doesn't need to be reconstructed.
        /// </summary>
        [NonSerialized]
        [CanBeNull]
        private XElement _xml;

        /// <summary>
        ///   Initializes a new instance of the <see cref="LoggingException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="parameters">The parameters.</param>
        /// <remarks>
        ///   <para>There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> on this method requiring the
        ///   <paramref name="message"/> to not be a null value.</para>
        ///   <para>There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> on this method requiring the
        ///   <paramref name="parameters"/> to not be a null value.</para>
        /// </remarks>
        [StringFormatMethod("message")]
        [UsedImplicitly]
        public LoggingException([NotNull] string message, [NotNull] params object[] parameters)
            : this(null, null, message, LogLevel.Error, parameters)
        {
            Contract.Requires(message != null, Resources.LoggingException_MessageCannotBeNull);
            Contract.Requires(parameters != null, Resources.LoggingException_ParametersCannotBeNull);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="LoggingException"/> class.
        /// </summary>
        /// <param name="context">The log context.</param>
        /// <param name="message">The exception message.</param>
        /// <param name="parameters">The parameters.</param>
        /// <remarks>
        ///   <para>There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> on this method requiring the
        ///   <paramref name="message"/> to not be a null value.</para>
        ///   <para>There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> on this method requiring the
        ///   <paramref name="parameters"/> to not be a null value.</para>
        /// </remarks>
        [StringFormatMethod("message")]
        [UsedImplicitly]
        public LoggingException([NotNull] LogContext context, [NotNull] string message,
                                [NotNull] params object[] parameters)
            : this(context, null, message, LogLevel.Error, parameters)
        {
            Contract.Requires(context != null, Resources.LoggingException_ContextCannotBeNull);
            Contract.Requires(message != null, Resources.LoggingException_MessageCannotBeNull);
            Contract.Requires(parameters != null, Resources.LoggingException_ParametersCannotBeNull);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="LoggingException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="level">The log level.</param>
        /// <param name="parameters">The parameters.</param>
        /// <remarks>
        ///   <para>There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> on this method requiring the
        ///   <paramref name="message"/> to not be a null value.</para>
        ///   <para>There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> on this method requiring the
        ///   <paramref name="parameters"/> to not be a null value.</para>
        /// </remarks>
        [StringFormatMethod("message")]
        public LoggingException([NotNull] string message, LogLevel level, [NotNull] params object[] parameters)
            : this(null, null, message, level, parameters)
        {
            Contract.Requires(message != null, Resources.LoggingException_MessageCannotBeNull);
            Contract.Requires(parameters != null, Resources.LoggingException_ParametersCannotBeNull);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="LoggingException"/> class.
        /// </summary>
        /// <param name="context">The log context.</param>
        /// <param name="message">The exception message.</param>
        /// <param name="level">The log level.</param>
        /// <param name="parameters">The parameters.</param>
        /// <remarks>
        ///   <para>There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> on this method requiring the
        ///   <paramref name="message"/> to not be a null value.</para>
        ///   <para>There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> on this method requiring the
        ///   <paramref name="parameters"/> to not be a null value.</para>
        ///   <para>There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> on this method requiring the
        ///   <paramref name="context"/> to not be a null value.</para>
        /// </remarks>
        [StringFormatMethod("message")]
        public LoggingException([NotNull] LogContext context, [NotNull] string message, LogLevel level,
                                [NotNull] params object[] parameters)
            : this(context, null, message, level, parameters)
        {
            Contract.Requires(context != null, Resources.LoggingException_ContextCannotBeNull);
            Contract.Requires(message != null, Resources.LoggingException_MessageCannotBeNull);
            Contract.Requires(parameters != null, Resources.LoggingException_ParametersCannotBeNull);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="LoggingException"/> class.
        /// </summary>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="level">The log level.</param>
        /// <remarks>
        ///   <para>There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> on this method requiring the
        ///   <paramref name="innerException"/> to not be a null value.</para>
        /// </remarks>
        public LoggingException([NotNull] Exception innerException, LogLevel level = LogLevel.Error)
            : this(innerException, innerException.Message, level)
        {
            Contract.Requires(innerException != null, Resources.LoggingException_InnerExceptionCannotBeNull);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="LoggingException"/> class.
        /// </summary>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="message">The exception message.</param>
        /// <param name="parameters">The parameters.</param>
        /// <remarks>
        ///   <para>There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> on this method requiring the
        ///   <paramref name="message"/> to not be a null value.</para>
        ///   <para>There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> on this method requiring the
        ///   <paramref name="parameters"/> to not be a null value.</para>
        /// </remarks>
        [StringFormatMethod("message")]
        public LoggingException(
            [CanBeNull] Exception innerException,
            [NotNull] string message,
            [NotNull] params object[] parameters)
            : this(null, innerException, message, LogLevel.Error, parameters)
        {
            Contract.Requires(message != null, Resources.LoggingException_MessageCannotBeNull);
            Contract.Requires(parameters != null, Resources.LoggingException_ParametersCannotBeNull);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="LoggingException"/> class.
        /// </summary>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="message">The exception message.</param>
        /// <param name="level">The log level.</param>
        /// <param name="parameters">The parameters.</param>
        /// <remarks>
        ///   <para>There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> on this method requiring the
        ///   <paramref name="message"/> to not be a null value.</para>
        ///   <para>There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> on this method requiring the
        ///   <paramref name="parameters"/> to not be a null value.</para>
        /// </remarks>
        [StringFormatMethod("message")]
        public LoggingException(
            [CanBeNull] Exception innerException,
            [NotNull] string message,
            LogLevel level,
            [NotNull] params object[] parameters)
            : this(null, innerException, message, level, parameters)
        {
            Contract.Requires(message != null, Resources.LoggingException_MessageCannotBeNull);
            Contract.Requires(parameters != null, Resources.LoggingException_ParametersCannotBeNull);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="LoggingException"/> class.
        /// </summary>
        /// <param name="context">The log context.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="message">The exception message.</param>
        /// <param name="level">The log level.</param>
        /// <param name="parameters">The parameters.</param>
        /// <remarks>
        ///   <para>There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> on this method requiring the
        ///   <paramref name="message"/> to not be a null value.</para>
        ///   <para>There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> on this method requiring the
        ///   <paramref name="parameters"/> to not be a null value.</para>
        ///   <para>There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> on this method requiring the
        ///   <paramref name="context"/> to not be a null value.</para>
        /// </remarks>
        [StringFormatMethod("message")]
        public LoggingException(
            [NotNull] LogContext context,
            [CanBeNull] Exception innerException,
            [NotNull] string message,
            LogLevel level,
            [NotNull] params object[] parameters)
            : base(message.SafeFormat(parameters), innerException)
        {
            Contract.Requires(context != null, Resources.LoggingException_ContextCannotBeNull);
            Contract.Requires(message != null, Resources.LoggingException_MessageCannotBeNull);
            Contract.Requires(parameters != null, Resources.LoggingException_ParametersCannotBeNull);

            LogGroup = Guid.Empty;
            Level = level;
            Context = new LogContext(context, parameters);
            _parameters = parameters
                .Select(o =>
                    o == null
                        ? null
                        : o.ToString())
                .ToArray();

            // Create an inner exception stack of all inner exceptions until the last logging exception
            Stack<Exception> innerExceptions = new Stack<Exception>();
            if (innerException != null)
            {
                Stack<Exception> currentStack = new Stack<Exception>();
                currentStack.Push(innerException);
                while (currentStack.Count > 0)
                {
                    Exception e = currentStack.Pop();
                    LoggingException le = e as LoggingException;

                    // If the inner exception is a logging exception stop, as it will have logged itself
                    // and it's inner exceptions, already.
                    if (le != null)
                    {
                        // Reuse the log group from the inner exception
                        // Very Cool - this groups future exceptions
                        // into the same group, if they correctly
                        // pass the inner exception...
                        LogGroup = le.LogGroup;
                        break;
                    }

                    // Push the exception onto our stack
                    innerExceptions.Push(e);

                    // Check to see if we are an aggregate exception
                    AggregateException ae = e as AggregateException;
                    if (ae != null)
                    {
                        // Push all inner exceptions
                        foreach (Exception aee in ae.InnerExceptions)
                            currentStack.Push(aee);
                    }
                    else if (e.InnerException != null)
                        // Push the inner exception
                        currentStack.Push(e.InnerException);
                }
            }

            // If we haven't got a logGroup create a new one
            if (LogGroup == Guid.Empty)
                LogGroup = Guid.NewGuid();

            // We now take items back off stack (starting with deepest first, hence maintaining
            // order based on when exception occurred) and log each independently.
            while (innerExceptions.Count > 0)
            {
                Exception e = innerExceptions.Pop();
                Log.Add(LogGroup, e, LogLevel.Information);
            }

            // Get stack trace now - note that normally the stack trace is only calculated at the point
            // an exception is caught (so that it can stop the trace at the catching stack frame).
            // We need to get a stack trace skipping 2 frames + the number of base types constructors.
            StackTrace = FormatStackTrace(new StackTrace(2, true));

            // Now we can create the associated log item for this exception.
            PerformanceCounterHelper.PerfCounterException.Increment();
            Log.Add(LogGroup, this, Context);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="LoggingException"/> class.
        ///   Used during de-serialization only.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="ArgumentNullException"><paramref name="info"/> is a <see langword="null"/>.</exception>
        private LoggingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            LogGroup = (Guid)info.GetValue("LG", typeof(Guid));
            Level = (LogLevel)info.GetValue("LL", typeof(LogLevel));
        }

        /// <summary>
        ///   Gets the parameters as an <see cref="Array"/>.
        /// </summary>
        [NotNull]
        public IEnumerable<string> Parameters
        {
            get { return _parameters; }
        }

        /// <summary>
        ///   Gets the stack trace as a <see cref="string"/>.
        ///   Occasionally the stack trace becomes unavailable, so we capture to string on construction.
        /// </summary>
        /// <value>The safe stack trace.</value>
        public new string StackTrace { get; private set; }

        #region ISerializable Members
        /// <summary>
        ///   When overridden in a derived class, sets the <see cref="SerializationInfo" /> with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext" /> that contains contextual information about the source or destination.</param>
        /// <exception cref="ArgumentNullException">
        ///   The <paramref name="info"/> parameter is a <see langword="null"/>.
        /// </exception>
        /// <PermissionSet>
        ///   <Permission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version = "1" Read = "*AllFiles*" PathDiscovery = "*AllFiles*" />
        ///   <Permission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version = "1" Flags = "SerializationFormatter" />
        /// </PermissionSet>
        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("LG", LogGroup, typeof(Guid));
            info.AddValue("LL", Level, typeof(LogLevel));
        }
        #endregion

        /// <summary>
        ///   Gets the XML representation of an exception.
        ///   This is cached to avoid reconstructing the XML each time.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="deep">If <see langword="true"/> then returns the inner exceptions.</param>
        /// <param name="includeStackTrace">If set to <see langword="true"/> includes stack traces and full logs (where available).</param>
        /// <returns>The XML representation of the exception.</returns>
        public static XElement GetXml(Exception exception, bool deep = true, bool includeStackTrace = true)
        {
            XElement error = new XElement(
                NodeError,
                new XAttribute(AttributeType, exception.GetType()),
                new XElement(NodeMessage, exception.Message.XmlEscape()));

            LoggingException l = exception as LoggingException;
            if (l != null)
            {
                error.Add(new XAttribute(AttributeLogGroup, l.LogGroup));
                error.Add(new XAttribute(AttributeLevel, l.Level));
            }
            if (includeStackTrace)
                error.Add(new XElement(NodeStackTrace, exception.StackTrace.XmlEscape()));

            if (deep && (exception.InnerException != null))
                error.Add(new XElement(NodeInnerException, GetXml(exception.InnerException, true, includeStackTrace)));

            return error;
        }

        /// <summary>
        ///   Gets the XML representation of an exception.
        ///   This is cached to avoid reconstructing the XML each time.
        /// </summary>
        /// <returns>The XML representation of the exception.</returns>
        public XElement GetXml()
        {
            return _xml ?? (_xml = GetXml(this));
        }

        /// <summary>
        ///   Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> representation this instance.</returns>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" PathDiscovery="*AllFiles*"/>
        /// </PermissionSet>
        [UsedImplicitly]
        public override string ToString()
        {
            return base.ToString();
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
            Type baseType = null;
            bool displayFilenames = true; // we'll try, but demand may fail
            String word_At = "at";
            String inFileLineNum = "in {0}:line {1}";

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
                    // We only skip constructors.
                    if (mb is ConstructorInfo)
                    {
                        Type declaringType = mb.DeclaringType;
                        // If we've already seen this as a base type we can skip this frame.
                        if (declaringType == baseType)
                            continue;

                        // Look for inheritance from logging exception.
                        baseType = declaringType;
                        while ((baseType != typeof(object)) &&
                               (baseType != typeof(LoggingException)))
                            baseType = baseType.BaseType;

                        if (baseType == typeof(LoggingException))
                        {
                            // We are descended from LoggingException so skip frame.
                            baseType = declaringType;
                            continue;
                        }
                    }

                    // We are not part of the exception construction, so no longer check for frame skipping.
                    checkSkip = false;
                    baseType = null;
                }

                // We want a newline at the end of every line except for the last
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
    }
}