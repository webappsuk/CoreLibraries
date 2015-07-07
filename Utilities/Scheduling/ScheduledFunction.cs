using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace WebApplications.Utilities.Scheduling
{
    /// <summary>
    /// Base abstract class.
    /// </summary>
    public abstract class ScheduledFunction
    {
        /// <summary>
        /// The default maximum history count.
        /// </summary>
        public const int DefaultMaximumHistory = 100;

        /// <summary>
        /// Lock object, which is held during execution of the scheduled function.
        /// </summary>
        public readonly object ExecutionLock = new object();

        /// <summary>
        /// The schedule.
        /// </summary>
        public readonly Schedule Schedule;

        /// <summary>
        /// Gets the execution count (which may me more than the history count which is limited).
        /// </summary>
        /// <value>The execution count.</value>
        public ulong ExecutionCount { get; protected set; }

        /// <summary>
        /// The next time the function is due to be executed.
        /// </summary>
        public DateTime LastExecutionComplete { get; protected set; }

        /// <summary>
        /// The next time the function is due to be executed.
        /// </summary>
        public DateTime NextExecutionDue { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduledFunction"/> class.
        /// </summary>
        /// <param name="schedule">The schedule.</param>
        /// <param name="startAfter">The date time to start after.</param>
        protected ScheduledFunction(Schedule schedule, DateTime? startAfter = null)
        {
            if (schedule == null)
                throw new ArgumentNullException("schedule");
            Schedule = schedule;
            LastExecutionComplete = DateTime.MinValue;

            NextExecutionDue = Schedule.Next(startAfter ?? DateTime.Now);
        }

        /// <summary>
        /// Executes the scheduled function now, unless it is already executing, in which case it waits for the result of the
        /// current execution.
        /// </summary>
        /// <returns>The result of the function, and information about the execution.</returns>
        public abstract ScheduledFunctionResult ExecuteNow();
    }

    /// <summary>
    /// Holds information about a scheduled action in the <see cref="Scheduler"/>.
    /// </summary>
    public class ScheduledFunction<TResult> : ScheduledFunction, IEnumerable<ScheduledFunctionResult<TResult>>
    {

        /// <summary>
        /// The action to run (actually a function)
        /// </summary>
        private readonly Func<ScheduledFunction<TResult>, TResult> _function;

        /// <summary>
        /// Holds the history of scheduled actions.
        /// </summary>
        private readonly Queue<ScheduledFunctionResult<TResult>> _history;

        /// <summary>
        /// Gets the history of scheduled actions.
        /// </summary>
        /// <value>The history.</value>
        public IEnumerable<ScheduledFunctionResult<TResult>> History { get { return _history; } }

        private int _maxHistory;
        /// <summary>
        /// Gets or sets the max history count.
        /// </summary>
        /// <value>The max history count.</value>
        public int MaxHistory
        {
            get { return this._maxHistory; }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("value", "The maximum history count cannot be less than 1");

                if (this._maxHistory == value)
                    return;
                lock (ExecutionLock)
                {
                    if (this._maxHistory == value)
                        return;

                    // If we're decreasing the maximum history we need to trim out the history.
                    if (_maxHistory > MaxHistory)
                    {
                        while (_history.Count > _maxHistory)
                            _history.Dequeue();
                        _history.TrimExcess();
                    }
                    this._maxHistory = MaxHistory;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduledFunction&lt;TResult&gt;"/> class.
        /// </summary>
        /// <param name="schedule">The schedule.</param>
        /// <param name="function">The function.</param>
        /// <param name="startAfter">The date time to start after.</param>
        /// <param name="maximumHistory">The maximum history count.</param>
        internal ScheduledFunction(Schedule schedule, Func<ScheduledFunction<TResult>, TResult> function,
            DateTime? startAfter = null,
            int maximumHistory = DefaultMaximumHistory)
            : base(schedule, startAfter)
        {
            if (function == null)
                throw new ArgumentNullException("function");
            _function = function;

            if (maximumHistory < 1)
                throw new ArgumentOutOfRangeException("maximumHistory", "The maximum history count cannot be less than 1");

            _maxHistory = maximumHistory;
            _history = new Queue<ScheduledFunctionResult<TResult>>(maximumHistory);
        }

        /// <summary>
        /// Executes the scheduled function now, unless it is already executing, in which case it waits for the result of the
        /// current execution.
        /// </summary>
        /// <returns>The result of the function, and information about the execution.</returns>
        public override ScheduledFunctionResult ExecuteNow()
        {
            // Get the history count immediately.
            UInt64 executionCount = ExecutionCount;
            lock (ExecutionLock)
            {
                // Check to see if an execution has occurred whilst retrieving the lock.
                if (executionCount < ExecutionCount)
                    return _history.Last();

                DateTime started = DateTime.Now;
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                TResult result;
                Exception exception;
                try
                {
                    result = _function(this);
                    exception = null;
                }
                catch (Exception e)
                {
                    result = default(TResult);
                    exception = e;
                }
                stopwatch.Stop();
                LastExecutionComplete = DateTime.Now;

                ScheduledFunctionResult<TResult> sresult = new ScheduledFunctionResult<TResult>(
                        this.NextExecutionDue, started, stopwatch.Elapsed, result, exception);

                // Calculate next execution due date.
                NextExecutionDue = Schedule.Next(LastExecutionComplete);

                // Add result to history
                _history.Enqueue(sresult);

                // Trim history
                while (_history.Count > _maxHistory)
                    _history.Dequeue();

                // Increment the execution count.
                ExecutionCount++;

                return sresult;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<ScheduledFunctionResult<TResult>> GetEnumerator()
        {
            return _history.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}