#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities.Logging 
// Project: WebApplications.Utilities.Logging
// File: Log.cs
// 
// This software, its object code and source code and all modifications made to
// the same (the “Software”) are, and shall at all times remain, the proprietary
// information and intellectual property rights of Web Applications (UK) Limited. 
// You are only entitled to use the Software as expressly permitted by Web
// Applications (UK) Limited within the Software Customisation and
// Licence Agreement (the “Agreement”).  Any copying, modification, decompiling,
// distribution, licensing, sale, transfer or other use of the Software other than
// as expressly permitted in the Agreement is expressly forbidden.  Web
// Applications (UK) Limited reserves its rights to take action against you and
// your employer in accordance with its contractual and common law rights
// (including injunctive relief) should you breach the terms of the Agreement or
// otherwise infringe its copyright or other intellectual property rights in the
// Software.
// 
// © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
#endregion

using System;
using System.Data.SqlClient;
using System.Threading;
using System.Xml.Linq;
using JetBrains.Annotations;
using WebApplications.Utilities.Logging.Performance;

namespace WebApplications.Utilities.Logging
{
    /// <summary>
    ///   Holds information about a single log item.
    /// </summary>
    [Serializable]
    public sealed partial class Log : IEquatable<Log>
    {
        /// <summary>
        ///   The exception type or a <see langword="null"/> if the log item isn't an exception.
        /// </summary>
        [CanBeNull] [UsedImplicitly] public readonly string ExceptionType;

        /// <summary>
        ///   A Guid used to group log items together.
        /// </summary>
        [UsedImplicitly] public readonly CombGuid Group;

        /// <summary>
        ///   A Guid used to uniquely identify a log item.
        /// </summary>
        [UsedImplicitly] public readonly CombGuid Guid;

        /// <summary>
        ///   The <see cref="WebApplications.Utilities.Logging.LogLevel">log level</see>.
        /// </summary>
        [UsedImplicitly] public readonly LogLevel Level;

        /// <summary>
        ///   The log message.
        /// </summary>
        [UsedImplicitly] [NotNull] public readonly string Message;

        /// <summary>
        ///   The stack trace, if this was an exception.
        /// </summary>
        [UsedImplicitly] public readonly string StackTrace;

        /// <summary>
        ///   The ID of the thread that the log item was created on.
        /// </summary>
        [UsedImplicitly] public readonly int ThreadId;

        /// <summary>
        ///   The <see cref="Thread.Name">name</see> (or the ID if no name is set) of the thread that the log item was created on.
        /// </summary>
        [UsedImplicitly] [NotNull] public readonly string ThreadName;

        /// <summary>
        ///   The time stamp of when the log item was created.
        /// </summary>
        [UsedImplicitly] public DateTime TimeStamp { get { return Guid.Created; }}

        /// <summary>
        ///   The <see cref="LogContext">context</see> information for the log item.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        private readonly LogContext _context;

        /// <summary>
        ///   Gets the <see cref="LogContext">context</see> information for the log item.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public LogContext Context { get { return _context; } }

        /// <summary>
        ///   The currently executing command (if any).
        /// </summary>
        private readonly Operation _operation;

        /// <summary>
        ///   The Guid of the log item's operation
        /// </summary>
        private readonly CombGuid _operationGuid;

        /// <summary>
        ///   Caches the <see cref="string"/> representation of the log item, to prevent regeneration.
        /// </summary>
        [NonSerialized] private string _string;

        /// <summary>
        ///   The cached XML representation.
        /// </summary>
        [NonSerialized] private XElement _xml;

        /// <summary>
        ///   Prevents a default instance of the <see cref="Log"/> class from being created.
        /// </summary>
        /// <param name="guid">The log item's Guid.</param>
        private Log(CombGuid guid)
        {
            Guid = guid;

            // Store a weak reference to the log item within the store of log items.
            _logItems.GetOrAdd(Guid, this);
        }

#if false
        /// <summary>
        ///   Initializes a new instance of the <see cref="Log"/> class.
        /// </summary>
        /// <param name="logRow">The log row.</param>
        /// <param name="operation">The operation.</param>
        private Log([NotNull] LogRow logRow, [CanBeNull] Operation operation)
            : this(logRow.ID)
        {
            Group = logRow.LogGroup;
            Message = logRow.Message;
            StackTrace = logRow.StackTrace;
            TimeStamp = logRow.TimeStamp;
            Level = (LogLevel) logRow.Level;
            _context = logRow.Context;

            ThreadId = logRow.ThreadID;
            ThreadName = logRow.ThreadName;

            // Set the operation and the operation guid.
            _operation = operation;
            if (operation != null)
                _operationGuid = operation.Guid;
        }
#endif

        /// <summary>
        ///   Initializes a new instance of the <see cref="Log"/> class.
        /// </summary>
        /// <param name="logGroup">The unique ID that groups log items together.</param>
        /// <param name="context">The context information.</param>
        /// <param name="exception">The exception. If none then pass <see langword="null"/>.</param>
        /// <param name="message">The log message.</param>
        /// <param name="level">The log level.</param>
        /// <param name="parameters">The parameters.</param>
        [StringFormatMethod("message")]
        private Log(Guid logGroup, [NotNull] LogContext context, [CanBeNull] Exception exception,
                    [NotNull] string message, LogLevel level, [NotNull] params object[] parameters)
            : this(CombGuid.NewCombGuid(DateTime.Now))
        {
            using (new PerformanceCounter("Creating Log"))
            {
                // Create a context, adding the parameters.
                _context = new LogContext(context, parameters);

                if (exception != null)
                {
                    ExceptionType = exception.GetType().ToString();

                    // Logging exception has a more robust stack trace retrieval algorithm
                    LoggingException loggingException = exception as LoggingException;
                    StackTrace = (loggingException != null ? loggingException.StackTrace : exception.StackTrace);

                    if (String.IsNullOrWhiteSpace(StackTrace))
                        StackTrace = "No stack trace found.";

                    // If this is a SQL exception, then log the stored proc.
                    SqlException sqlException;
                    if ((sqlException = exception as SqlException) != null)
                    {
                        message = string.Format(
                            Resources.Log_SqlException,
                            Environment.NewLine,
                            message,
                            string.IsNullOrEmpty(sqlException.Procedure) ? "<Unknown>" : sqlException.Procedure,
                            sqlException.LineNumber);
                    }
                }
                else
                {
                    ExceptionType = null;
                    StackTrace = null;
                }

                // Get operation from thread context
                _operation = Operation.Current;

                _operationGuid = _operation == null ? CombGuid.Empty : _operation.Guid;

                // Get the current thread information
                ThreadId = Thread.CurrentThread.ManagedThreadId;
                ThreadName = string.IsNullOrWhiteSpace(Thread.CurrentThread.Name)
                                 ? ThreadId.ToString()
                                 : Thread.CurrentThread.Name;

                // Set provided properties
                Group = logGroup.Equals(System.Guid.Empty) ? CombGuid.NewCombGuid(TimeStamp) : (CombGuid) logGroup;

                // Attempt to format string safely.
                // ReSharper disable ConditionIsAlwaysTrueOrFalse
                Message = message == null ? string.Empty : message.SafeFormat(parameters);
                // ReSharper restore ConditionIsAlwaysTrueOrFalse
                Level = level;

                // Queue the log entry.
                _logQueue.Enqueue(this);

                // Signal monitor thread of new arrival.
                _logSignal.Set();
            }
        }

        /// <summary>
        ///   Gets the <see cref="Operation">operation</see>.
        /// </summary>
        /// <value>The <see cref="Operation"/> or a <see langword="null"/> if there is none.</value>
        /// <exception cref="LoggingException">Operation was not re-linked correctly.</exception>
        public Operation Operation
        {
            get
            {
                if (_operationGuid == CombGuid.Empty)
                    return null;
                if (_operation == null)
                    throw new LoggingException(Resources.Log_Operation_OperationNotReLinked, LogLevel.Critical);

                return _operation;
            }
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
            get { return ExceptionType != null; }
        }

        /// <summary>
        ///   Gets the XML version of the operation.
        /// </summary>
        /// <value>The XML version of the operation.</value>
        /// <remarks>The XML version is cached to avoid regeneration.</remarks>
        [UsedImplicitly]
        public XElement Xml
        {
            get
            {
                if (_xml == null)
                {
                    _xml = new XElement(
                        NodeLog,
                        new XAttribute(AttributeId, Guid.Guid),
                        new XAttribute(AttributeTimestamp, TimeStamp),
                        new XAttribute(AttributeLoggroup, Group),
                        new XElement(
                            NodeThread, new XAttribute(AttributeId, ThreadId), ThreadName.XmlEscape()),
                        new XElement(NodeLevel, Level),
                        new XElement(NodeMessage, Message.XmlEscape()));

                    if (_operation != null)
                        _xml.Add(_operation.Xml);

                    _xml.Add(_context.Xml);

                    if (IsException)
                    {
                        _xml.Add(
                            new XElement(NodeExceptionType, ExceptionType.XmlEscape()),
                            new XElement(NodeStackTrace, StackTrace.XmlEscape()));
                    }
                }

                // Return copy of cached XElement
                return new XElement(_xml);
            }
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
            // Build the string only once, then cache.
            if (_string == null)
            {
                string exception = IsException
                                       ? string.Format(
                                           Resources.Log_ToString_LogException,
                                           Environment.NewLine,
                                           '\t',
                                           ExceptionType,
                                           StackTrace)
                                       : string.Empty;

                _string =
                    string.Format(
                        Resources.Log_ToString,
                        Environment.NewLine,
                        '\t',
                        Guid,
                        ThreadId,
                        ThreadName,
                        Group,
                        TimeStamp,
                        Level,
                        Message,
                        _operation == null ? "No operation." : _operation.ToString(),
                        _context,
                        exception);
            }
            return _string;
        }
    }
}