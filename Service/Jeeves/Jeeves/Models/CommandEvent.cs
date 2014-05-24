using System;

namespace Jeeves.Models
{
    public sealed class CommandEvent : Event
    {
        public override EventType Type
        {
            get { return EventType.Command; }
        }

        public override DateTime StartDate
        {
            get { throw new NotImplementedException(); }
        }

        public override DateTime EndDate
        {
            get { throw new NotImplementedException(); }
        }
    }
}