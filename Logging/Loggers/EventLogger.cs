#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities.Logging 
// Project: WebApplications.Utilities.Logging
// File: EventLogger.cs
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

using System.Diagnostics;
using System.Diagnostics.Contracts;
using JetBrains.Annotations;
using WebApplications.Utilities.Logging.Configuration;

namespace WebApplications.Utilities.Logging.Loggers
{
    /// <summary>
    ///   Allows logging to Windows <see cref="EventLog">event logs</see>.
    /// </summary>
    [UsedImplicitly]
    public class EventLogger : LoggerBase
    {
        /// <summary>
        ///   The <see cref="System.Diagnostics.EventLog.Log">name</see> of the <see cref="EventLog">event log</see>.
        /// </summary>
        [UsedImplicitly] [NotNull] public readonly string EventLog;

        /// <summary>
        ///   Initializes a new instance of the <see cref="EventLogger"/> class.
        /// </summary>
        /// <param name="name">The name of the logger.</param>
        /// <param name="eventLog">
        ///   <para>The <see cref="System.Diagnostics.EventLog.Log">name</see> of the <see cref="EventLog">event log</see> to read/write to.</para>
        ///   <para>By default this is set to "Application".</para>
        /// </param>
        /// <param name="validLevels">
        ///   <para>The valid log levels.</para>
        ///   <para>By default this is set to <see cref="LogLevels">LogLevels.AtLeastInformation</see>.</para>
        /// </param>
        /// <remarks>
        ///   <para>There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> on this method requiring the
        ///   <paramref name="name"/> to not be a null value.</para>
        ///   <para>There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> on this method requiring the
        ///   <paramref name="eventLog"/> to not be a null value.</para>
        /// </remarks>
        public EventLogger(
            [NotNull] string name,
            [NotNull] string eventLog = "Application",
            LogLevels validLevels = LogLevels.AtLeastInformation)
            : base(name, false, validLevels)
        {
            Contract.Requires(name != null, Resources.EventLogger_NameCannotBeNull);
            Contract.Requires(eventLog != null, Resources.EventLogger_EventLogCannotBeNull);
            EventLog = eventLog;
        }

        /// <summary>
        ///   Adds the specified log to the <see cref="EventLog">event log</see>.
        /// </summary>
        /// <param name="log">The log to write to the <see cref="EventLog">event log</see>.</param>
        /// <remarks>
        ///   There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> on this method requiring the
        ///   <paramref name="log"/> to not be a null value.
        /// </remarks>
        /// <exception cref="System.InvalidOperationException">
        ///   The registry key for the event log couldn't be opened.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        ///   The <see cref="Log.ToString()">log message</see> is larger than 32766 bytes.
        /// </exception>
        public override void Add(Log log)
        {
            Contract.Requires(log != null, Resources.EventLogger_LogCannotBeNull);

            EventLog eventLog = new EventLog {Source = LoggingConfiguration.Active.ApplicationName, Log = EventLog};
            EventLogEntryType entryType;
            switch (log.Level)
            {
                case LogLevel.Emergency:
                case LogLevel.Critical:
                case LogLevel.Error:
                    entryType = EventLogEntryType.Error;
                    break;
                case LogLevel.Warning:
                    entryType = EventLogEntryType.Warning;
                    break;
                case LogLevel.SystemNotification:
                case LogLevel.Notification:
                case LogLevel.Information:
                    entryType = EventLogEntryType.Information;
                    break;
                default:
                    // Ignore debug information.
                    return;
            }
            eventLog.WriteEntry(log.ToString(), entryType);
        }
    }
}