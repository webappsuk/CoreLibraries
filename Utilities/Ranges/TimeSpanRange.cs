using System;

namespace WebApplications.Utilities.Ranges
{
    /// <summary>
    /// A range of <see cref="TimeSpan">s</see> values with the time component ignored.
    /// </summary>
    public class TimeSpanRange : Range<TimeSpan>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeSpanRange"/> class.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        public TimeSpanRange(TimeSpan start, TimeSpan end) 
            : base(start, end, TimeSpan.FromDays(1.0))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeSpanRange"/> class.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="step">The step in days.</param>
        public TimeSpanRange(TimeSpan start, TimeSpan end, double step) 
            : base(start, end, TimeSpan.FromDays(step))
        { }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format("{0:dd/MM/yyyy} - {1:dd/MM/yyyy}", Start, End);
        }
    }
}
