using System;

namespace WebApplications.Utilities.Scheduling
{
    /// <summary>
    /// The result of a scheduled action.
    /// </summary>
    public class ScheduledFunctionResult
    {
        /// <summary>
        /// When the execution was due.
        /// </summary>
        public readonly DateTime Due;

        /// <summary>
        /// When the execution actually started.
        /// </summary>
        public readonly DateTime Started;

        /// <summary>
        /// When the execution duration.
        /// </summary>
        public readonly TimeSpan Duration;

        /// <summary>
        /// Any exception that was thrown by the function.
        /// </summary>
        public readonly Exception Exception;

        /// <summary>
        /// The result
        /// </summary>
        public readonly object Result;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduledFunctionResult"/> class.
        /// </summary>
        /// <param name="due">The execution due.</param>
        /// <param name="started">The execution started.</param>
        /// <param name="duration">The execution duration.</param>
        /// <param name="result">The result.</param>
        /// <param name="exception">The exception.</param>
        protected ScheduledFunctionResult(DateTime due, DateTime started, TimeSpan duration, object result, Exception exception)
        {
            this.Due = due;
            this.Result = result;
            this.Started = started;
            this.Duration = duration;
            this.Exception = exception;
        }
    }

    /// <summary>
    /// The result of a scheduled action.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public class ScheduledFunctionResult<TResult> : ScheduledFunctionResult
    {

        /// <summary>
        /// The result of the scheduled function.
        /// </summary>
        public new TResult Result { get { return (TResult)base.Result; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduledFunctionResult&lt;TResult&gt;"/> class.
        /// </summary>
        /// <param name="due">The execution due.</param>
        /// <param name="started">The execution started.</param>
        /// <param name="duration">The execution duration.</param>
        /// <param name="result">The result.</param>
        /// <param name="exception">The exception.</param>
        public ScheduledFunctionResult(DateTime due, DateTime started, TimeSpan duration, TResult result, Exception exception)
            : base(due, started, duration, result, exception)
        {
        }
    }
}