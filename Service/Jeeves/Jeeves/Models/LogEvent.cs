using System;
using WebApplications.Utilities.Logging;

namespace Jeeves.Models
{
    public sealed class LogEvent : Event
    {
        private readonly bool _local;
        private readonly Log _log;

        public LogEvent(bool local, Log log)
        {
            _local = local;
            _log = log;
        }

        public Log Log { get { return _log; } }

        public override EventType Type
        {
            get { return _local ? EventType.LocalLog : EventType.RemoteLog; }
        }

        public override DateTime StartDate
        {
            get { return _log.TimeStamp.ToLocalTime(); }
        }

        public override DateTime EndDate
        {
            get { return _log.TimeStamp.ToLocalTime(); }
        }
    }
}