using System;

namespace WebApplications.Utilities.Scheduling.Scheduled
{
    /// <summary>
    /// The result of a scheduled action.
    /// </summary>
    internal class ScheduledActionResult : IScheduledActionResult
    {
        /// <summary>
        /// When the execution was due.
        /// </summary>
        private readonly DateTime _due;

        /// <summary>
        /// When the execution duration.
        /// </summary>
        private TimeSpan _duration;

        /// <summary>
        /// Any exception that was thrown by the function.
        /// </summary>
        private Exception _exception;

        /// <summary>
        /// When the execution actually started.
        /// </summary>
        private DateTime _started;

        /// <summary>
        /// Whether this action was cancelled.
        /// </summary>
        private bool _cancelled;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduledActionResult"/> class.
        /// </summary>
        /// <param name="due">The due.</param>
        /// <param name="started">The started.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="cancelled">if set to <see langword="true"/> the action was cancelled.</param>
        /// <remarks></remarks>
        internal ScheduledActionResult(DateTime due, DateTime started, TimeSpan duration, Exception exception, bool cancelled)
        {
            _due = due;
            _started = started;
            _duration = duration;
            _exception = exception;
            _cancelled = cancelled;
        }

        /// <inheritdoc/>
        public DateTime Due
        {
            get { return _due; }
        }

        /// <inheritdoc/>
        public TimeSpan Duration
        {
            get { return _duration; }
        }

        /// <inheritdoc/>
        public Exception Exception
        {
            get { return _exception; }
        }

        /// <inheritdoc/>
        public DateTime Started
        {
            get { return _started; }
        }

        /// <inheritdoc/>
        public bool Cancelled
        {
            get { return _cancelled; }
        }
    }
}