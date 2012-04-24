using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebApplications.Testing
{
    /// <summary>
    /// Holds a data range.
    /// </summary>
    /// <remarks></remarks>
    public struct DateTimeRange
    {
        /// <summary>
        /// The start of the range.
        /// </summary>
        public readonly DateTime Start;

        /// <summary>
        /// The inclusive end of the range.
        /// </summary>
        public readonly DateTime End;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeRange" /> struct.
        /// </summary>
        /// <param name="start">The start (inclusive).</param>
        /// <param name="end">The end (inclusive).</param>
        /// <remarks></remarks>
        public DateTimeRange(DateTime start, DateTime end) : this()
        {
            Start = start;
            End = end;
        }

        /// <summary>
        /// Binds the specified value so that it cannot fall outside the values of the range. 
        /// </summary>
        /// <param name="value">The value to bind.</param>
        /// <returns>
        /// The bound value.
        /// </returns>
        public DateTime Bind(DateTime value)
        {
            return value < Start
                       ? this.Start
                       : (End < value ? this.End : value);
        }
    }
}
