using System;

namespace Jeeves.Models
{
    public abstract class Event
    {
        public abstract EventType Type { get; }
        public abstract DateTime StartDate { get; }
        public abstract DateTime EndDate { get; }
        public TimeSpan Duration { get { return EndDate - StartDate; }}
    }
}