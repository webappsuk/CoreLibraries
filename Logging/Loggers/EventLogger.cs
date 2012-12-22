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
        ///   <para>By default this is set to <see cref="LoggingLevels">LogLevels.AtLeastInformation</see>.</para>
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
            LoggingLevels validLevels = LoggingLevels.AtLeastInformation)
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
                case LoggingLevel.Emergency:
                case LoggingLevel.Critical:
                case LoggingLevel.Error:
                    entryType = EventLogEntryType.Error;
                    break;
                case LoggingLevel.Warning:
                    entryType = EventLogEntryType.Warning;
                    break;
                case LoggingLevel.SystemNotification:
                case LoggingLevel.Notification:
                case LoggingLevel.Information:
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