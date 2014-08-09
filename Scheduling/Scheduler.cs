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

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WebApplications.Utilities.Logging;
using WebApplications.Utilities.Scheduling.Scheduled;

namespace WebApplications.Utilities.Scheduling
{
    /// <summary>
    /// Implements a scheduler
    /// </summary>
    /// <remarks></remarks>
    public sealed class Scheduler : IDisposable
    {
        /// <summary>
        /// The default <see cref="Scheduler"/>.
        /// </summary>
        [NotNull]
        public static readonly Scheduler Default = new Scheduler();

        /// <summary>
        /// The maximum ticks.
        /// </summary>
        internal static readonly long MaxTicks = DateTime.MaxValue.Ticks;

        /// <summary>
        /// The maximum time span supported by a timer.
        /// </summary>
        internal static readonly TimeSpan MaxTimeSpan = TimeSpan.FromMilliseconds(0xfffffffe);

        /// <summary>
        /// Holds all scheduled actions.
        /// </summary>
        [NotNull]
        private readonly ConcurrentDictionary<CombGuid, ScheduledAction> _actions =
            new ConcurrentDictionary<CombGuid, ScheduledAction>();

        /// <summary>
        /// The tick state.
        /// 0 = Inactive
        /// 1 = Running
        /// 2 = 
        /// </summary>
        private int _tickState;
        private Timer _ticker;

        /// <summary>
        /// Initializes a new instance of the <see cref="Scheduler" /> class.
        /// </summary>
        private Scheduler()
        {
            // TODO Get from configuration.
            DefaultMaximumHistory = 100;
            MaximumActionDuration = TimeSpan.FromMinutes(15);
            _ticker = new Timer(CheckSchedule, null, Timeout.Infinite, Timeout.Infinite);
        }

        #region Add Overloads
        /// <summary>
        /// Schedules the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="schedule">The schedule.</param>
        /// <param name="maximumHistory">The maximum history.</param>
        /// <returns>A <see cref="ScheduledAction" />.</returns>
        [NotNull]
        [PublicAPI]
        public ScheduledAction Add(
            [NotNull] ScheduledAction.SchedulableAction action,
            [NotNull] ISchedule schedule,
            int maximumHistory = -1)
        {
            Contract.Requires(action != null);
            Contract.Requires(schedule != null);
            // ReSharper disable PossibleNullReferenceException
            // ReSharper disable AssignNullToNotNullAttribute
            return Add(
                false,
                (d, t) =>
                {
                    action();
                    return TaskResult.True;
                },
                schedule,
                maximumHistory);
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore PossibleNullReferenceException
        }

        /// <summary>
        /// Schedules the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="schedule">The schedule.</param>
        /// <param name="maximumHistory">The maximum history.</param>
        /// <returns>A <see cref="ScheduledAction" />.</returns>
        [NotNull]
        [PublicAPI]
        public ScheduledAction Add(
            [NotNull] ScheduledAction.SchedulableDueAction action,
            [NotNull] ISchedule schedule,
            int maximumHistory = -1)
        {
            Contract.Requires(action != null);
            Contract.Requires(schedule != null);
            // ReSharper disable PossibleNullReferenceException
            // ReSharper disable AssignNullToNotNullAttribute
            return Add(
                false,
                (d, t) =>
                {
                    action(d);
                    return TaskResult.True;
                },
                schedule,
                maximumHistory);
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore PossibleNullReferenceException
        }

        /// <summary>
        /// Schedules the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="schedule">The schedule.</param>
        /// <param name="maximumHistory">The maximum history.</param>
        /// <returns>A <see cref="ScheduledAction" />.</returns>
        [NotNull]
        [PublicAPI]
        public ScheduledAction Add(
            [NotNull] ScheduledAction.SchedulableActionAsync action,
            [NotNull] ISchedule schedule,
            int maximumHistory = -1)
        {
            Contract.Requires(action != null);
            Contract.Requires(schedule != null);
            // ReSharper disable PossibleNullReferenceException
            // ReSharper disable AssignNullToNotNullAttribute
            return Add(
                false,
                (d, t) => action().ContinueWith(_ => true, t, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Current),
                schedule,
                maximumHistory);
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore PossibleNullReferenceException
        }

        /// <summary>
        /// Schedules the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="schedule">The schedule.</param>
        /// <param name="maximumHistory">The maximum history.</param>
        /// <returns>A <see cref="ScheduledAction" />.</returns>
        [NotNull]
        [PublicAPI]
        public ScheduledAction Add(
            [NotNull] ScheduledAction.SchedulableDueActionAsync action,
            [NotNull] ISchedule schedule,
            int maximumHistory = -1)
        {
            Contract.Requires(action != null);
            Contract.Requires(schedule != null);
            // ReSharper disable PossibleNullReferenceException
            // ReSharper disable AssignNullToNotNullAttribute
            return Add(
                false,
                (d, t) => action(d).ContinueWith(_ => true, t, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Current),
                schedule,
                maximumHistory);
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore PossibleNullReferenceException
        }

        /// <summary>
        /// Schedules the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="schedule">The schedule.</param>
        /// <param name="maximumHistory">The maximum history.</param>
        /// <returns>A <see cref="ScheduledAction" />.</returns>
        [NotNull]
        [PublicAPI]
        public ScheduledAction Add(
            [NotNull] ScheduledAction.SchedulableCancellableActionAsync action,
            [NotNull] ISchedule schedule,
            int maximumHistory = -1)
        {
            Contract.Requires(action != null);
            Contract.Requires(schedule != null);
            // ReSharper disable PossibleNullReferenceException
            // ReSharper disable AssignNullToNotNullAttribute
            return Add(
                false,
                (d, t) => action(t).ContinueWith(_ => true, t, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Current),
                schedule,
                maximumHistory);
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore PossibleNullReferenceException
        }

        /// <summary>
        /// Schedules the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="schedule">The schedule.</param>
        /// <param name="maximumHistory">The maximum history.</param>
        /// <returns>A <see cref="ScheduledAction" />.</returns>
        [NotNull]
        [PublicAPI]
        public ScheduledAction Add(
            [NotNull] ScheduledAction.SchedulableDueCancellableActionAsync action,
            [NotNull] ISchedule schedule,
            int maximumHistory = -1)
        {
            Contract.Requires(action != null);
            Contract.Requires(schedule != null);
            // ReSharper disable PossibleNullReferenceException
            // ReSharper disable AssignNullToNotNullAttribute
            return Add(
                false,
                (d, t) => action(d, t).ContinueWith(_ => true, t, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Current),
                schedule,
                maximumHistory);
            // ReSharper restore AssignNullToNotNullAttribute
            // ReSharper restore PossibleNullReferenceException
        }

        /// <summary>
        /// Schedules the specified function.
        /// </summary>
        /// <typeparam name="T">The return type</typeparam>
        /// <param name="function">The function.</param>
        /// <param name="schedule">The schedule.</param>
        /// <param name="maximumHistory">The maximum history.</param>
        /// <returns>A <see cref="ScheduledFunction{T}"/>.</returns>
        [NotNull]
        [PublicAPI]
        public ScheduledFunction<T> Add<T>(
            [NotNull] ScheduledFunction<T>.SchedulableFunction function,
            [NotNull] ISchedule schedule,
            int maximumHistory = -1)
        {
            Contract.Requires(function != null);
            Contract.Requires(schedule != null);
            return Add(true, (d, t) => Task.FromResult(function()), schedule, maximumHistory);
        }

        /// <summary>
        /// Schedules the specified function.
        /// </summary>
        /// <typeparam name="T">The return type</typeparam>
        /// <param name="function">The function.</param>
        /// <param name="schedule">The schedule.</param>
        /// <param name="maximumHistory">The maximum history.</param>
        /// <returns>A <see cref="ScheduledFunction{T}"/>.</returns>
        [NotNull]
        [PublicAPI]
        public ScheduledFunction<T> Add<T>(
            [NotNull] ScheduledFunction<T>.SchedulableDueFunction function,
            [NotNull] ISchedule schedule,
            int maximumHistory = -1)
        {
            Contract.Requires(function != null);
            Contract.Requires(schedule != null);
            return Add(true, (d, t) => Task.FromResult(function(d)), schedule, maximumHistory);
        }

        /// <summary>
        /// Schedules the specified function.
        /// </summary>
        /// <typeparam name="T">The return type</typeparam>
        /// <param name="function">The function.</param>
        /// <param name="schedule">The schedule.</param>
        /// <param name="maximumHistory">The maximum history.</param>
        /// <returns>A <see cref="ScheduledFunction{T}"/>.</returns>
        [NotNull]
        [PublicAPI]
        public ScheduledFunction<T> Add<T>(
            [NotNull] ScheduledFunction<T>.SchedulableFunctionAsync function,
            [NotNull] ISchedule schedule,
            int maximumHistory = -1)
        {
            Contract.Requires(function != null);
            Contract.Requires(schedule != null);
            return Add(true, (d, t) => function(), schedule, maximumHistory);
        }

        /// <summary>
        /// Schedules the specified function.
        /// </summary>
        /// <typeparam name="T">The return type</typeparam>
        /// <param name="function">The function.</param>
        /// <param name="schedule">The schedule.</param>
        /// <param name="maximumHistory">The maximum history.</param>
        /// <returns>A <see cref="ScheduledFunction{T}"/>.</returns>
        [NotNull]
        [PublicAPI]
        public ScheduledFunction<T> Add<T>(
            [NotNull] ScheduledFunction<T>.SchedulableDueFunctionAsync function,
            [NotNull] ISchedule schedule,
            int maximumHistory = -1)
        {
            Contract.Requires(function != null);
            Contract.Requires(schedule != null);
            return Add(true, (d, t) => function(d), schedule, maximumHistory);
        }

        /// <summary>
        /// Schedules the specified function.
        /// </summary>
        /// <typeparam name="T">The return type</typeparam>
        /// <param name="function">The function.</param>
        /// <param name="schedule">The schedule.</param>
        /// <param name="maximumHistory">The maximum history.</param>
        /// <returns>A <see cref="ScheduledFunction{T}"/>.</returns>
        [NotNull]
        [PublicAPI]
        public ScheduledFunction<T> Add<T>(
            [NotNull] ScheduledFunction<T>.SchedulableCancellableFunctionAsync function,
            [NotNull] ISchedule schedule,
            int maximumHistory = -1)
        {
            Contract.Requires(function != null);
            Contract.Requires(schedule != null);
            return Add(true, (d, t) => function(t), schedule, maximumHistory);
        }

        /// <summary>
        /// Schedules the specified function.
        /// </summary>
        /// <typeparam name="T">The return type</typeparam>
        /// <param name="function">The function.</param>
        /// <param name="schedule">The schedule.</param>
        /// <param name="maximumHistory">The maximum history.</param>
        /// <returns>A <see cref="ScheduledFunction{T}"/>.</returns>
        [NotNull]
        [PublicAPI]
        public ScheduledFunction<T> Add<T>(
            [NotNull] ScheduledFunction<T>.SchedulableDueCancellableFunctionAsync function,
            [NotNull] ISchedule schedule,
            int maximumHistory = -1)
        {
            Contract.Requires(function != null);
            Contract.Requires(schedule != null);
            return Add(true, function, schedule, maximumHistory);
        }

        /// <summary>
        /// Schedules the specified function.
        /// </summary>
        /// <typeparam name="T">The return type</typeparam>
        /// <param name="isFunction">if set to <see langword="true" /> [is function].</param>
        /// <param name="function">The function.</param>
        /// <param name="schedule">The schedule.</param>
        /// <param name="maximumHistory">The maximum history.</param>
        /// <returns>A <see cref="ScheduledFunction{T}" />.</returns>
        [NotNull]
        [PublicAPI]
        private ScheduledFunction<T> Add<T>(
            bool isFunction,
            [NotNull] ScheduledFunction<T>.SchedulableDueCancellableFunctionAsync function,
            [NotNull] ISchedule schedule,
            int maximumHistory = -1)
        {
            Contract.Requires(function != null);
            Contract.Requires(schedule != null);
            ScheduledFunction<T> sf = new ScheduledFunction<T>(
                isFunction,
                function,
                this,
                schedule,
                maximumHistory < 0 ? DefaultMaximumHistory : maximumHistory);
            // Update actions dictionary
            _actions.AddOrUpdate(sf.ID, sf, (i, a) => sf);
            return sf;
        }
        #endregion

        #region Remove Overloads
        /// <summary>
        /// Removes the specified scheduled action.
        /// </summary>
        /// <param name="scheduledAction">The scheduled action.</param>
        /// <returns><see langword="true" /> if removed, <see langword="false" /> otherwise.</returns>
        [PublicAPI]
        public bool Remove([NotNull]ScheduledAction scheduledAction)
        {
            Contract.Requires(scheduledAction != null);
            ScheduledAction a;
            bool result = _actions.TryRemove(scheduledAction.ID, out a);
            Contract.Assert(!result || (a != null && a.ID == scheduledAction.ID));
            return result;
        }

        /// <summary>
        /// Removes the specified scheduled function.
        /// </summary>
        /// <param name="scheduledFunction">The scheduled action.</param>
        /// <returns><see langword="true" /> if removed, <see langword="false" /> otherwise.</returns>
        [PublicAPI]
        public bool Remove<T>([NotNull]ScheduledFunction<T> scheduledFunction)
        {
            Contract.Requires(scheduledFunction != null);
            ScheduledAction a;
            bool result = _actions.TryRemove(scheduledFunction.ID, out a);
            Contract.Assert(!result || (a != null && a.ID == scheduledFunction.ID));
            return result;
        }
        #endregion

        /// <summary>
        /// Gets or sets the maximum duration of an action.
        /// </summary>
        /// <value>The maximum duration of any action.</value>
        [PublicAPI]
        public TimeSpan MaximumActionDuration
        {
            get { return _maximumActionDuration; }
            set
            {
                if (value <= TimeSpan.FromMilliseconds(10))
                    value = TimeSpan.FromMinutes(15);
                _maximumActionDuration = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Scheduler"/> is enabled.
        /// </summary>
        /// <value><see langword="true" /> if enabled; otherwise, <see langword="false" />.</value>
        [PublicAPI]
        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (_enabled == value) return;
                _enabled = value;
                CheckSchedule();
            }
        }

        private int _defaultMaximumHistory;
        /// <summary>
        /// Gets or sets the default maximum history.
        /// </summary>
        /// <value>The default maximum history.</value>
        /// <exception cref="WebApplications.Utilities.Logging.LoggingException"></exception>
        [PublicAPI]
        public int DefaultMaximumHistory
        {
            get { return _defaultMaximumHistory; }
            set
            {
                if (value < 0)
                    throw new LoggingException(() => Resource.Scheduler_DefaultMaximumHistory_Negative);
                _defaultMaximumHistory = value;
            }
        }

        private ScheduledAction _nextScheduledAction;
        /// <summary>
        /// Gets the next scheduled action (if any).
        /// </summary>
        /// <value>The next scheduled action.</value>
        [PublicAPI]
        [CanBeNull]
        public ScheduledAction NextScheduledAction { get { return _nextScheduledAction; } }

        /// <summary>
        /// The next time the action is due.
        /// </summary>
        private long _nextDueTicks;

        private TimeSpan _maximumActionDuration;
        private bool _enabled;

        /// <summary>
        /// Gets the next due date and time (UTC).
        /// </summary>
        /// <value>The next due date and time.</value>
        [PublicAPI]
        public DateTime NextDue
        {
            get
            {
                long ndt = Interlocked.Read(ref _nextDueTicks);
                return new DateTime(ndt, DateTimeKind.Utc);
            }
        }

        /// <summary>
        /// Called when the scheduling timer is retrieved.
        /// </summary>
        /// <param name="state">The state.</param>
        internal void CheckSchedule([CanBeNull] object state = null)
        {
            // Only allow one check to run at a time, namely the check that caused the tick state to move from 0 to 1.
            // The increment will force any currently executing check to recheck.
            if (Interlocked.Increment(ref _tickState) > 1) return;

            do
            {
                Timer ticker;
                TimeSpan wait;
                do
                {
                    ticker = _ticker;
                    // Check if we're disposed or disabled.
                    if (ticker == null ||
                        !_enabled)
                    {
                        Interlocked.Exchange(ref _tickState, 0);
                        return;
                    }

                    // Ensure ticker is stopped.
                    ticker.Change(Timeout.Infinite, Timeout.Infinite);

                    DateTime ndt = DateTime.MaxValue;

                    // Set our tick state to 1, we're about to do a complete check of the actions if any other tick calls
                    // come in from this point onwards we will need to recheck.
                    ScheduledAction nextScheduledAction = null;
                    Interlocked.Exchange(ref _tickState, 1);
                    foreach (ScheduledAction action in _actions.Values)
                    {
                        Contract.Assert(action != null);
                        if (!action.Enabled) continue;
                        
                        if (action.NextDue < DateTime.UtcNow)
                            // Due now so kick of an execution (shouldn't block).
                            action.ExecuteAsync();
                        else if (action.NextDue < ndt)
                        {
                            // Update the next due time.
                            nextScheduledAction = action;
                            ndt = action.NextDue;
                        }
                    }

                    // If the tick state has increased, check again.
                    if (_tickState > 1)
                    {
                        // Yield to allow the current actions some chance to run.
                        Thread.Yield();
                        continue;
                    }

                    // If the next due time is max value, we're never due
                    if (ndt == DateTime.MaxValue)
                    {
                        // Set to infinite wait.
                        wait = Timeout.InfiniteTimeSpan;

                        // Update properties.
                        _nextDueTicks = MaxTicks;
                        _nextScheduledAction = null;
                        break;
                    }

                    DateTime now = DateTime.UtcNow;

                    // If we're due in past continue.
                    if (ndt <= now) continue;

                    // Next due is in future
                    wait = ndt - now;

                    // Update properties.
                    _nextDueTicks = ndt.Ticks;
                    _nextScheduledAction = nextScheduledAction;

                    // Ensure the wait duration doesn't exceed the maximum supported by Timer.
                    if (wait > MaxTimeSpan)
                        wait = MaxTimeSpan;
                    break;
                } while (true);

                // Set the ticker to run after the wait period.
                ticker = _ticker;
                if (ticker == null)
                {
                    Interlocked.Exchange(ref _tickState, 0);
                    return;
                }
                ticker.Change(wait, Timeout.InfiniteTimeSpan);

                // Try to set the tick state back to 0, from 1 and finish
                if (Interlocked.CompareExchange(ref _tickState, 0, 1) == 1)
                    return;

                // The tick state managed to increase from 1 before we could exit, so we need to clear the ticker and recheck.
                ticker = _ticker;
                if (ticker == null)
                {
                    Interlocked.Exchange(ref _tickState, 0);
                    return;
                }
                ticker.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            } while (true);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Timer ticker = Interlocked.Exchange(ref _ticker, null);
            if (ticker != null)
                ticker.Dispose();
        }
    }
}