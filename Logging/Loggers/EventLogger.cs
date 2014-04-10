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

using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Logging.Loggers
{
    /// <summary>
    ///   Allows logging to Windows <see cref="EventLog">event logs</see>.
    /// </summary>
    [UsedImplicitly]
    public class EventLogger : LoggerBase
    {
        /// <summary>
        /// The default format.
        /// </summary>
        [PublicAPI]
        [NotNull]
        public const string DefaultFormat = "Verbose";

        [NotNull]
        private string _eventLog;

        [NotNull]
        private string _machineName;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventLogger" /> class.
        /// </summary>
        /// <param name="name">The name of the logger.</param>
        /// <param name="eventLog"><para>The <see cref="System.Diagnostics.EventLog.Log">name</see> of the <see cref="EventLog">event log</see> to read/write to.</para>
        /// <para>By default this is set to "Application".</para></param>
        /// <param name="validLevels"><para>The valid log levels.</para>
        /// <para>By default this is set to <see cref="LoggingLevels">LogLevels.AtLeastInformation</see>.</para></param>
        /// <param name="format">The format.</param>
        /// <param name="machineName">Name of the machine.</param>
        /// <remarks><para>There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> on this method requiring the
        /// <paramref name="name" /> to not be a null value.</para>
        /// <para>There is a <see cref="System.Diagnostics.Contracts.Contract">contract</see> on this method requiring the
        /// <paramref name="eventLog" /> to not be a null value.</para></remarks>
        public EventLogger(
            [NotNull] string name,
            [NotNull] string eventLog = "Application",
            LoggingLevels validLevels = LoggingLevels.AtLeastInformation,
            [NotNull] string format = DefaultFormat,
            [NotNull] string machineName = "."
            )
            : base(name, false, false, validLevels)
        {
            Contract.Requires(name != null);
            Contract.Requires(eventLog != null);
            Contract.Requires(format != null);
            Contract.Requires(machineName != null);
            Contract.Requires(name != null, Resources.EventLogger_NameCannotBeNull);
            EventLog = eventLog;
            MachineName = machineName;
            Format = format;
        }

        /// <summary>
        ///   The <see cref="System.Diagnostics.EventLog.Log">name</see> of the <see cref="EventLog">event log</see>.
        /// </summary>
        [UsedImplicitly]
        [NotNull]
        public string EventLog
        {
            get { return _eventLog; }
            set
            {
                Contract.Requires(value != null);
                if (_eventLog == value) return;
                if (string.IsNullOrWhiteSpace(value))
                    // ReSharper disable once AssignNullToNotNullAttribute
                    throw new LoggingException(() => Resources.EventLogger_EventLogCannotBeNull);
                _eventLog = value;
            }
        }

        /// <summary>
        /// Gets or sets the format for trace messages.
        /// </summary>
        /// <value>The format.</value>
        [PublicAPI]
        [NotNull]
        public string Format { get; set; }

        /// <summary>
        /// Gets or sets the format for trace messages.
        /// </summary>
        /// <value>The format.</value>
        [NotNull]
        [PublicAPI]
        public string MachineName
        {
            get { return _machineName; }
            set
            {
                Contract.Requires(value != null);
                if (_machineName == value) return;
                if (string.IsNullOrWhiteSpace(value))
                    // ReSharper disable once AssignNullToNotNullAttribute
                    throw new LoggingException(() => Resources.EventLogger_MachineNameCannotBeNull);
                _machineName = value;
            }
        }

        /// <summary>
        /// Adds the specified logs to storage in batches.
        /// </summary>
        /// <param name="logs">The logs to add to storage.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task.</returns>
        public override Task Add(IEnumerable<Log> logs, CancellationToken token = default(CancellationToken))
        {
            Contract.Requires(logs != null);
            string source = Log.ApplicationName;
            if (string.IsNullOrWhiteSpace(source))
                source = "Application";
            else if (source.Length > 254)
                source = source.Substring(0, 254);

            EventLog eventLog = new EventLog {Source = source, MachineName = MachineName, Log = EventLog};
            string format = Format;
            foreach (Log log in logs)
            {
                Contract.Assert(log != null);
                token.ThrowIfCancellationRequested();
                string logStr = log.ToString(format);
                StringBuilder builder = new StringBuilder(logStr.Length);
                int a = 0;

                // Maximum event log size is 31839
                bool percChar = false;
                while ((builder.Length < 31839) &&
                       (a < logStr.Length))
                {
                    char c = logStr[a++];

                    // We cannot have a digit after a '%'.
                    if (percChar && char.IsDigit(c))
                        continue;

                    percChar = c == '%';

                    builder.Append(c);
                }

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
                    default:
                        entryType = EventLogEntryType.Information;
                        break;
                }

                // Create an id based on time of day.
                int id = (int) (log.TimeStamp.TimeOfDay.TotalSeconds / 1.32);

                eventLog.WriteEntry(builder.ToString(), entryType, id, (short) log.Level, log.Guid.ToByteArray());
            }
            // ReSharper disable once AssignNullToNotNullAttribute
            return TaskResult.Completed;
        }
    }
}