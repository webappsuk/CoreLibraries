#region © Copyright Web Applications (UK) Ltd, 2012.  All rights reserved.
// Solution: WebApplications.Utilities.Scheduling 
// Project: WebApplications.Utilities.Scheduling
// File: ScheduledAction.cs
// 
// This software, its object code and source code and all modifications made to
// the same (the “Software”) are, and shall at all times remain, the proprietary
// information and intellectual property rights of Web Applications (UK) Limited. 
// You are only entitled to use the Software as expressly permitted by Web
// Applications (UK) Limited within the Software Customisation and
// Licence Agreement (the “Agreement”).  Any copying, modification, decompiling,
// distribution, licensing, sale, transfer or other use of the Software other than
// as expressly permitted in the Agreement is expressly forbidden.  Web
// Applications (UK) Limited reserves its rights to take action against you and
// your employer in accordance with its contractual and common law rights
// (including injunctive relief) should you breach the terms of the Agreement or
// otherwise infringe its copyright or other intellectual property rights in the
// Software.
// 
// © Copyright Web Applications (UK) Ltd, 2012.  All rights reserved.
#endregion

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WebApplications.Utilities.Caching;
using WebApplications.Utilities.Logging;
using WebApplications.Utilities.Scheduling.Schedulable;

namespace WebApplications.Utilities.Scheduling.Scheduled
{
    /// <summary>
    /// Holds a scheduled action.
    /// </summary>
    /// <remarks></remarks>
    internal class ScheduledAction : IScheduledAction, IEquatable<IScheduledAction>
    {
        /// <summary>
        /// Holds constructor for creating a result for an action.
        /// </summary>
        [NotNull]
        private static readonly Func<DateTime, DateTime, TimeSpan, Exception, bool, object, ScheduledActionResult>
            _actionResultCreator =
                (due, started, duration, exception, cancelled, result) =>
                new ScheduledActionResult(due, started, duration, exception, cancelled);

        /// <summary>
        /// Unique identifier for the action.
        /// </summary>
        internal readonly CombGuid ID = CombGuid.NewCombGuid();

        /// <summary>
        /// Holds information about the schedulable action.
        /// </summary>
        [NotNull]
        private readonly SchedulableActionInfo _actionInfo;

        /// <summary>
        /// Holds the scheduler.
        /// </summary>
        [NotNull]
        private readonly IScheduler _scheduler;

        /// <summary>
        /// Holds the action.
        /// </summary>
        [NotNull]
        private readonly ISchedulableAction _action;

        /// <summary>
        /// Internal list of results.
        /// </summary>
        [CanBeNull]
        private readonly CyclicConcurrentQueue<ScheduledActionResult> _history;

        /// <summary>
        /// Holds constructor for creating a result for an action.
        /// </summary>
        [NotNull]
        private readonly Func<DateTime, DateTime, TimeSpan, Exception, bool, object, ScheduledActionResult> _resultCreator;

        /// <summary>
        /// The maximum history.
        /// </summary>
        private readonly int _maximumHistory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduledAction"/> class.
        /// </summary>
        /// <param name="scheduler">The scheduler.</param>
        /// <param name="schedule">The schedule.</param>
        /// <param name="action">The action.</param>
        /// <param name="actionInfo">The action info.</param>
        /// <param name="maximumHistory">The maximum history.</param>
        /// <remarks></remarks>
        internal ScheduledAction([NotNull]IScheduler scheduler, [NotNull]ISchedule schedule, [NotNull]ISchedulableAction action, [NotNull]SchedulableActionInfo actionInfo, int maximumHistory = -1)
            : this(scheduler, schedule, action, actionInfo, maximumHistory, _actionResultCreator)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduledAction"/> class.
        /// </summary>
        /// <param name="scheduler">The scheduler.</param>
        /// <param name="schedule">The schedule.</param>
        /// <param name="action">The action.</param>
        /// <param name="actionInfo">The action info.</param>
        /// <param name="maximumHistory">The maximum history.</param>
        /// <param name="functionReturnType">Type of the function return.</param>
        /// <param name="resultCreator">The result creator.</param>
        /// <remarks></remarks>
        protected ScheduledAction([NotNull]IScheduler scheduler, [NotNull]ISchedule schedule, [NotNull]ISchedulableAction action, [NotNull]SchedulableActionInfo actionInfo, int maximumHistory, [NotNull]Func<DateTime, DateTime, TimeSpan, Exception, bool, object, ScheduledActionResult> resultCreator)
        {
            _scheduler = scheduler;
            _lastExecutionFinished = DateTime.MinValue;
            _schedule = schedule;
            _action = action;
            _actionInfo = actionInfo;
            _resultCreator = resultCreator;
            _maximumHistory = maximumHistory;
            _history = _maximumHistory > 0 ? new CyclicConcurrentQueue<ScheduledActionResult>(_maximumHistory) : null;
            RecalculateNextDue();
        }

        /// <inheritdoc/>
        public Type FunctionReturnType
        {
            get { return _actionInfo.FunctionReturnType; }
        }

        /// <inheritdoc/>
        public IScheduler Scheduler
        {
            get { return _scheduler; }
        }

        /// <inheritdoc/>
        public IEnumerable<IScheduledActionResult> History
        {
            get { return _history ?? Enumerable.Empty<IScheduledActionResult>(); }
        }

        /// <summary>
        /// Holds the schedule.
        /// </summary>
        [NotNull]
        private ISchedule _schedule;

        /// <inheritdoc/>
        public ISchedule Schedule
        {
            get { return _schedule; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value", "Cannot set a schedule to null.");
                if (_schedule == value)
                    return;
                _schedule = value;
                RecalculateNextDue();
            }
        }

        /// <inheritdoc/>
        public ISchedulableAction Action { get { return _action; } }

        /// <inheritdoc/>
        public bool Enabled { get; set; }

        /// <inheritdoc/>
        public int MaximumHistory { get { return _maximumHistory; } }

        /// <inheritdoc/>
        public bool IsFunction
        {
            get { return _actionInfo.IsFunction; }
        }

        /// <inheritdoc/>
        public bool IsAsynchronous
        {
            get { return _actionInfo.IsAsynchronous; }
        }

        /// <inheritdoc/>
        public bool IsCancellable
        {
            get { return _actionInfo.IsCancellable; }
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            ScheduledAction o = obj as ScheduledAction;
            return !ReferenceEquals(null, o) && o.ID.Equals(ID);
        }

        /// <inheritdoc/>
        public bool Equals(IScheduledAction other)
        {
            if (ReferenceEquals(null, other)) return false;
            ScheduledAction o = other as ScheduledAction;
            if (ReferenceEquals(null, o)) return false;
            return ReferenceEquals(this, other) || o.ID.Equals(ID);
        }

        /// <inheritdoc/>
        public bool Equals(ScheduledAction other)
        {
            if (ReferenceEquals(null, other)) return false;
            return ReferenceEquals(this, other) || other.ID.Equals(ID);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        /// <summary>
        /// Execution counter indicates how many concurrent executions are occurring.
        /// </summary>
        private int _executing;

        /// <inheritdoc/>
        public bool Execute()
        {
            if (!Enabled || !Scheduler.Enabled)
                return false;

            // If the action is asynchronous and cancellable, then run it asynchronously
            // so that cancellation support is still possible.
            if (IsAsynchronous && IsCancellable)
            {
                Task<bool> task = ExecuteAsync();
                task.Wait();
                return task.Result;
            }
            
            DateTime started = DateTime.Now;
            DateTime due = NextDue;

            // Grab schedule as the property can be changed.
            ISchedule schedule = Schedule;

            // Only execute if we allow concurrency or we're not executing already.
            int executing = Interlocked.Increment(ref _executing);
            bool executed;
            if (schedule.Options.HasFlag(ScheduleOptions.AllowConcurrent) ||
                (executing <= 1))
            {
                // Execute and add result to history
                if (_history != null)
                    _history.Enqueue(DoExecute(due, started));
                executed = true;

                // Increment the execution count.
                Interlocked.Increment(ref _executionCount);

                // Mark execution finish.
                LastExecutionFinished = DateTime.Now;
            }
            else 
                executed = false;

            // Decrement the execution counter.
            Interlocked.Decrement(ref _executing);
            return executed;
        }

        /// <inheritdoc/>
        public Task<bool> ExecuteAsync()
        {
            return ExecuteAsync(CancellationToken.None);
        }

        /// <inheritdoc/>
        public Task<bool> ExecuteAsync(CancellationToken cancellationToken)
        {
            if (!Enabled || !Scheduler.Enabled)
                return Scheduling.Scheduler.FalseResult;

            DateTime started = DateTime.Now;
            DateTime due = NextDue;

            // Mark this action as executing.
            int executing = Interlocked.Increment(ref _executing);

            // Grab schedule as the property can be changed.
            ISchedule schedule = Schedule;

            // Only execute if we allow concurrency or we're not executing already.
            if (!schedule.Options.HasFlag(ScheduleOptions.AllowConcurrent) && (executing >= 1))
            {
                Interlocked.Decrement(ref _executing);
                return Scheduling.Scheduler.FalseResult;
            }

            // Wrap non-asychronous tasks.
            Task<ScheduledActionResult> executeTask =
                IsAsynchronous
                    ? DoExecuteAsync(due, started, cancellationToken)
                    : Task<ScheduledActionResult>.Factory.StartNew(() => DoExecute(due, started));

            // Add continuation task and return
            return executeTask
                .ContinueWith(
                    t =>
                        {
                            Debug.Assert(t != null);

                            // Decrement the execution counter.
                            Interlocked.Decrement(ref _executing);

                            // Increment the execution count.
                            Interlocked.Increment(ref _executionCount);

                            // Mark execution finish.
                            LastExecutionFinished = DateTime.Now;

                            ScheduledActionResult result;
                            switch (t.Status)
                            {
                                case TaskStatus.RanToCompletion:
                                    result = t.Result;
                                    break;
                                case TaskStatus.Canceled:
                                    result = _resultCreator(due, started, DateTime.Now - started, null, true,
                                                            FunctionReturnType != null
                                                                ? FunctionReturnType.Default()
                                                                : null);
                                    break;
                                case TaskStatus.Faulted:
                                    result = _resultCreator(due, started, DateTime.Now - started, t.Exception, false,
                                                            FunctionReturnType != null
                                                                ? FunctionReturnType.Default()
                                                                : null);
                                    break;
                                default:
                                    result = _resultCreator(due, started, DateTime.Now - started,
                                                            new ArgumentOutOfRangeException(
                                                                String.Format(
                                                                    "Invalid task status '{0}' in scheduled action continuation.",
                                                                    t.Status)), false,
                                                            FunctionReturnType != null
                                                                ? FunctionReturnType.Default()
                                                                : null);
                                    break;
                            }

                            // Enqueue history item.
                            if (_history != null)
                                _history.Enqueue(result);

                            return true;
                        }, TaskContinuationOptions.ExecuteSynchronously);
        }

        /// <summary>
        /// Executes the action/function synchronously.
        /// </summary>
        /// <param name="due">When the action was due.</param>
        /// <param name="started">When the execution started.</param>
        /// <returns>The result.</returns>
        /// <remarks></remarks>
        protected virtual ScheduledActionResult DoExecute(DateTime due, DateTime started)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Exception exception = null;
            try
            {
                // Execute the action!
                Action.Execute();
            }
            catch (Exception e)
            {
                exception = e;
            }
            stopwatch.Stop();

            return new ScheduledActionResult(due, started, stopwatch.Elapsed, exception, false);
        }

        /// <summary>
        /// Executes the action/function asynchronously.
        /// </summary>
        /// <param name="due">When the action was due.</param>
        /// <param name="started">When the execution started.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The result.</returns>
        /// <remarks></remarks>
        [NotNull]
        protected virtual Task<ScheduledActionResult> DoExecuteAsync(DateTime due, DateTime started, CancellationToken cancellationToken)
        {
            // Quick cancellation check.
            if (cancellationToken.IsCancellationRequested)
                return TaskEx.FromResult(new ScheduledActionResult(due, started, TimeSpan.Zero, null, true));

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            ISchedulableActionCancellableAsync cancellableAction = Action as ISchedulableActionCancellableAsync;
            Task actionTask;
            if (cancellableAction != null)
                actionTask = cancellableAction.ExecuteAsync(cancellationToken);
            else
            {
                ISchedulableActionAsync asyncAction = Action as ISchedulableActionAsync;

                // Functions should be picked up in override of this method, so this should always be true.
                Debug.Assert(asyncAction != null);
                actionTask = asyncAction.ExecuteAsync();
            }

            // Add task continuation.
            return actionTask
                .ContinueWith(t =>
                                  {
                                      Debug.Assert(t != null);
                                      stopwatch.Stop();
                                      return new ScheduledActionResult(due, started, stopwatch.Elapsed, t.Exception,
                                                                       cancellationToken.IsCancellationRequested);
                                  }, TaskContinuationOptions.ExecuteSynchronously);
        }

        /// <summary>
        /// The next time the action is due.
        /// </summary>
        private long _nextDueTicks;

        /// <inheritdoc/>
        public DateTime NextDue
        {
            get { return new DateTime(_nextDueTicks, DateTimeKind.Utc); }
        }

        private DateTime _lastExecutionFinished;

        /// <inheritdoc/>
        public DateTime LastExecutionFinished
        {
            get { return _lastExecutionFinished; }
            private set
            {
                if (_lastExecutionFinished >= value) return;
                _lastExecutionFinished = value;
                RecalculateNextDue();
            }
        }

        private long _executionCount = 0;
        /// <inheritdoc/>
        public long ExecutionCount
        {
            get { return _executionCount; }
        }

        /// <summary>
        /// Lock used in the case optimistic setting fails.
        /// </summary>
        private SpinLock _calculatorLock = new SpinLock();

        /// <summary>
        /// Recalculates the next due date.
        /// </summary>
        /// <param name="withLock">if set to <see langword="true"/> uses a lock.</param>
        /// <remarks></remarks>
        private void RecalculateNextDue(bool withLock = false)
        {
            bool hasLock = false;
            if (withLock)
                _calculatorLock.Enter(ref hasLock);

            DateTime now = DateTime.Now;
            // Optimistic update strategy, to avoid locks.

            // Grab current value for nextDue.
            long ndt = _nextDueTicks;
            DateTime nd = new DateTime(ndt, DateTimeKind.Utc);

            // If we're due now, we're done.
            if (nd == now) return;

            // Calculate new value.
            DateTime newNextDue = Schedule.Next(now > _lastExecutionFinished ? now : _lastExecutionFinished);

            Debug.Assert(newNextDue >= now);

            // If the new due date is the existing one we're done.
            if (newNextDue == nd)
                return;

            long nndt = newNextDue.Ticks;

            // If we successfully update next ticks we're done.
            if (Interlocked.CompareExchange(ref _nextDueTicks, nndt, ndt) != ndt)
            {
                if (!withLock)
                    // Try again with a spin lock.
                    RecalculateNextDue(true);
                else
                    // We've failed to update - should never happen.
                    Log.Add("Failed to recalculate next due date for schedule.", LogLevel.Critical);
            }

            if (hasLock)
                _calculatorLock.Exit();
        }
    }
}