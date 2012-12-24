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
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using JetBrains.Annotations;
using WebApplications.Utilities.Performance;

namespace WebApplications.Utilities.Logging
{
    /// <summary>
    ///   Implements a specialised Exception handler used throughout Babel, automatically logs errors
    ///   and the context in which they occurred. Also makes use of Babel's late-binding translation.
    ///   BabelException should always be used where exceptions are thrown.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("[{ExceptionTypeFullName}] {Message} @ {TimeStamp}")]
    public class LoggingException : ApplicationException, IEnumerable<KeyValuePair<string, string>>, IFormattable
    {
        /// <summary>
        /// The associated log item.
        /// </summary>
        [NotNull]
        private readonly Log _log;

        /// <summary>
        /// Gets the GUID.
        /// </summary>
        /// <value>The GUID.</value>
        public CombGuid Guid { get { return _log.Guid; } }

        /// <summary>
        /// Gets the log group.
        /// </summary>
        /// <value>The log group.</value>
        public CombGuid Group { get { return _log.Group; } }

        /// <summary>
        /// Gets the log group.
        /// </summary>
        /// <value>The log group.</value>
        public LoggingLevel Level { get { return _log.Level; } }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        [NotNull]
        public IEnumerable<string> Parameters { get { return _log.Parameters; } }

        /// <summary>
        /// Gets the time stamp.
        /// </summary>
        /// <value>The time stamp.</value>
        public DateTime TimeStamp { get { return _log.TimeStamp; } }

        /// <summary>
        /// Gets the message format.
        /// </summary>
        /// <value>The message format.</value>
        [NotNull]
        public string MessageFormat { get { return _log.MessageFormat; } }

        /// <summary>
        /// Gets the thread ID.
        /// </summary>
        /// <value>The thread ID.</value>
        public int ThreadID { get { return _log.ThreadID; } }

        /// <summary>
        /// Gets the name of the thread.
        /// </summary>
        /// <value>The name of the thread.</value>
        [NotNull]
        public string ThreadName { get { return _log.ThreadName; } }

        /// <summary>
        /// Gets the full name of the type of the exception.
        /// </summary>
        /// <value>The full name of the type of the exception.</value>
        [NotNull]
        public string ExceptionTypeFullName { get { return _log.Get(Log.ExceptionTypeFullNameKey); } }

        /// <summary>
        /// Gets the stored procedure name (if a SQL exception - otherwise null).
        /// </summary>
        /// <value>The stored procedure.</value>
        [NotNull]
        public string StoredProcedure { get { return _log.Get(Log.StoredProcedureKey); } }

        /// <summary>
        /// Gets the stored procedure line number (if a SQL exception - otherwise -1).
        /// </summary>
        /// <value>The stored procedure line.</value>
        public int StoredProcedureLine
        {
            get
            {
                string line = _log.Get(Log.StoredProcedureLineKey);
                if (string.IsNullOrWhiteSpace(line))
                    return -1;
                int l;
                return int.TryParse(line, out l) ? l : -1;
            }
        }

        /// <summary>
        /// Gets the value of the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        [CanBeNull]
        public string Get(string key)
        {
            return _log.Get(key);
        }

        /// <summary>
        /// Gets all keys and their values that match the prefix.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <returns>IEnumerable{KeyValuePair{System.StringSystem.String}}.</returns>
        [NotNull]
        public IEnumerable<KeyValuePair<string, string>> GetPrefixed(string prefix)
        {
            return _log.GetPrefixed(prefix);
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
                return _log[key];
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="parameters">The parameters.</param>
        /// <remarks><para>There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> on this method requiring the
        ///   <paramref name="message" /> to not be a null value.</para>
        ///   <para>There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> on this method requiring the
        ///   <paramref name="parameters" /> to not be a null value.</para></remarks>
        [StringFormatMethod("message")]
        [UsedImplicitly]
        public LoggingException([NotNull] string message, [NotNull] params object[] parameters)
            : this(null, null, LoggingLevel.Error, message, parameters)
        {
            Contract.Requires(message != null, Resources.LoggingException_MessageCannotBeNull);
            Contract.Requires(parameters != null, Resources.LoggingException_ParametersCannotBeNull);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="context">The log context.</param>
        /// <param name="message">The exception message.</param>
        /// <param name="parameters">The parameters.</param>
        /// <remarks><para>There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> on this method requiring the
        ///   <paramref name="message" /> to not be a null value.</para>
        ///   <para>There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> on this method requiring the
        ///   <paramref name="parameters" /> to not be a null value.</para></remarks>
        [StringFormatMethod("message")]
        [UsedImplicitly]
        public LoggingException([CanBeNull] LogContext context, [NotNull] string message,
                                [NotNull] params object[] parameters)
            : this(context, null, LoggingLevel.Error, message, parameters)
        {
            Contract.Requires(message != null, Resources.LoggingException_MessageCannotBeNull);
            Contract.Requires(parameters != null, Resources.LoggingException_ParametersCannotBeNull);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="level">The log level.</param>
        /// <param name="message">The exception message.</param>
        /// <param name="parameters">The parameters.</param>
        /// <remarks><para>There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> on this method requiring the
        ///   <paramref name="message" /> to not be a null value.</para>
        ///   <para>There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> on this method requiring the
        ///   <paramref name="parameters" /> to not be a null value.</para></remarks>
        [StringFormatMethod("message")]
        public LoggingException(LoggingLevel level, [NotNull] string message, [NotNull] params object[] parameters)
            : this(null, null, level, message, parameters)
        {
            Contract.Requires(message != null, Resources.LoggingException_MessageCannotBeNull);
            Contract.Requires(parameters != null, Resources.LoggingException_ParametersCannotBeNull);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="context">The log context.</param>
        /// <param name="level">The log level.</param>
        /// <param name="message">The exception message.</param>
        /// <param name="parameters">The parameters.</param>
        /// <remarks><para>There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> on this method requiring the
        ///   <paramref name="message" /> to not be a null value.</para>
        ///   <para>There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> on this method requiring the
        ///   <paramref name="parameters" /> to not be a null value.</para>
        ///   <para>There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> on this method requiring the
        ///   <paramref name="context" /> to not be a null value.</para></remarks>
        [StringFormatMethod("message")]
        public LoggingException([CanBeNull] LogContext context, LoggingLevel level, [NotNull] string message, 
                                [NotNull] params object[] parameters)
            : this(context, null, level, message, parameters)
        {
            Contract.Requires(message != null, Resources.LoggingException_MessageCannotBeNull);
            Contract.Requires(parameters != null, Resources.LoggingException_ParametersCannotBeNull);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="level">The log level.</param>
        /// <remarks>There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> on this method requiring the
        /// <paramref name="innerException" /> to not be a null value.</remarks>
        public LoggingException([NotNull] Exception innerException, LoggingLevel level = LoggingLevel.Error)
            : this(null, innerException, level, innerException.Message)
        {
            Contract.Requires(innerException != null, Resources.LoggingException_InnerExceptionCannotBeNull);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="message">The exception message.</param>
        /// <param name="parameters">The parameters.</param>
        /// <remarks><para>There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> on this method requiring the
        ///   <paramref name="message" /> to not be a null value.</para>
        ///   <para>There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> on this method requiring the
        ///   <paramref name="parameters" /> to not be a null value.</para></remarks>
        [StringFormatMethod("message")]
        public LoggingException(
            [CanBeNull] Exception innerException,
            [NotNull] string message,
            [NotNull] params object[] parameters)
            : this(null, innerException, LoggingLevel.Error, message, parameters)
        {
            Contract.Requires(message != null, Resources.LoggingException_MessageCannotBeNull);
            Contract.Requires(parameters != null, Resources.LoggingException_ParametersCannotBeNull);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="level">The log level.</param>
        /// <param name="message">The exception message.</param>
        /// <param name="parameters">The parameters.</param>
        /// <remarks><para>There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> on this method requiring the
        ///   <paramref name="message" /> to not be a null value.</para>
        ///   <para>There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> on this method requiring the
        ///   <paramref name="parameters" /> to not be a null value.</para></remarks>
        [StringFormatMethod("message")]
        public LoggingException(
            [CanBeNull] Exception innerException,
            LoggingLevel level,
            [NotNull] string message,
            [NotNull] params object[] parameters)
            : this(null, innerException, level, message, parameters)
        {
            Contract.Requires(message != null, Resources.LoggingException_MessageCannotBeNull);
            Contract.Requires(parameters != null, Resources.LoggingException_ParametersCannotBeNull);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException" /> class.
        /// </summary>
        /// <param name="context">The log context.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="level">The log level.</param>
        /// <param name="message">The exception message.</param>
        /// <param name="parameters">The parameters.</param>
        /// <remarks><para>There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> on this method requiring the
        ///   <paramref name="message" /> to not be a null value.</para>
        ///   <para>There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> on this method requiring the
        ///   <paramref name="parameters" /> to not be a null value.</para>
        ///   <para>There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> on this method requiring the
        ///   <paramref name="context" /> to not be a null value.</para></remarks>
        [StringFormatMethod("message")]
        public LoggingException(
            [CanBeNull] LogContext context,
            [CanBeNull] Exception innerException,
            LoggingLevel level,
            [NotNull] string message,
            [NotNull] params object[] parameters)
            : base(message.SafeFormat(Thread.CurrentThread.CurrentUICulture, parameters), innerException)
        {
            Contract.Requires(message != null, Resources.LoggingException_MessageCannotBeNull);
            Contract.Requires(parameters != null, Resources.LoggingException_ParametersCannotBeNull);
            
            // Create an inner exception stack of all inner exceptions until the last logging exception
            Stack<Exception> innerExceptions = new Stack<Exception>();
            CombGuid group = CombGuid.Empty;
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
                        group = le.Group;
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

            if (innerExceptions.Count > 0)
            {
                // If we haven't found a base logging exception, we need to create our own group.
                if (group == CombGuid.Empty)
                    group = CombGuid.NewCombGuid();

                // We now take items back off stack (starting with deepest first, hence maintaining
                // order based on when exception occurred) and log each independently.
                while (innerExceptions.Count > 0)
                {
                    Exception e = innerExceptions.Pop();
                    Log.Add(group, e, LoggingLevel.Information);
                }
            }

            // Now we can create the associated log item for this exception.
            _log = Log.Add(group, context, this, level, message, parameters);

            // Finally increment performance counter.
            Log.PerfCounterException.Increment();
        }

        /// <summary>
        /// Gets a message that describes the current exception.
        /// </summary>
        /// <value>The message.</value>
        /// <returns>The error message that explains the reason for the exception, or an empty string("").</returns>
        public override string Message
        {
            get { return _log.Message; }
        }

        /// <summary>
        ///   Gets the stack trace as a <see cref="string"/>.
        ///   Occasionally the stack trace becomes unavailable, so we capture to string on construction.
        /// </summary>
        /// <value>The safe stack trace.</value>
        [NotNull]
        [UsedImplicitly]
        public new string StackTrace { get { return _log.StackTrace; } }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>IEnumerator{KeyValuePair{System.StringSystem.String}}.</returns>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _log.GetEnumerator();
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
            return _log.ToString(LogFormat.General);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        [NotNull]
        public string ToString(string format)
        {
            return _log.ToString(format, null);
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
                ICustomFormatter formatter = formatProvider.GetFormat(this.GetType()) as ICustomFormatter;

                if (formatter != null)
                    return formatter.Format(format, this, formatProvider);
            }
            return _log.ToString(format, formatProvider);
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
            return _log.ToString(format);
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