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
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApplications.Utilities.Annotations;
using NodaTime;
using WebApplications.Utilities.Caching;

namespace WebApplications.Utilities.Scheduling.Scheduled
{
    /// <summary>
    /// Holds a scheduled action.
    /// </summary>
    /// <remarks></remarks>
    public abstract class ScheduledAction : IEquatable<ScheduledAction>
    {
        #region Delegates
        /// <summary>
        /// Delegate describing a schedulable action.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        public delegate void SchedulableAction();

        /// <summary>
        /// Delegate describing a schedulable action which accepts a due date and time.
        /// </summary>
        /// <param name="due">The due date and time which indicates when the action was scheduled to run.</param>
        /// <returns>An awaitable task.</returns>
        public delegate void SchedulableDueAction(Instant due);

        /// <summary>
        /// Delegate describing an asynchronous schedulable action which supports cancellation.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>An awaitable task.</returns>
        public delegate Task SchedulableCancellableActionAsync(CancellationToken token);

        /// <summary>
        /// Delegate describing an asynchronous schedulable action which accepts a due date and time and supports cancellation.
        /// </summary>
        /// <param name="due">The due date and time which indicates when the action was scheduled to run.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>An awaitable task.</returns>
        public delegate Task SchedulableDueCancellableActionAsync(Instant due, CancellationToken token);
        #endregion

        /// <summary>
        /// Unique identifier for the action.
        /// </summary>
        internal readonly CombGuid ID = CombGuid.NewCombGuid();

        /// <summary>
        /// The maximum duration in milliseconds.
        /// </summary>
        protected int MaximumDurationMs;

        /// <summary>
        /// Internal list of results.
        /// </summary>
        [NotNull]
        protected readonly CyclicConcurrentQueue<ScheduledActionResult> HistoryQueue;

        /// <summary>
        /// The maximum history.
        /// </summary>
        [PublicAPI]
        public readonly int MaximumHistory;

        /// <summary>
        /// The return type (if a function).
        /// </summary>
        [PublicAPI]
        [CanBeNull]
        public readonly Type ReturnType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduledAction" /> class.
        /// </summary>
        /// <param name="schedule">The schedule.</param>
        /// <param name="maximumHistory">The maximum history.</param>
        /// <param name="maximumDuration">The maximum duration.</param>
        /// <param name="returnType">Type of the return (if a function).</param>
        protected ScheduledAction(
            [NotNull] ISchedule schedule,
            int maximumHistory,
            Duration maximumDuration,
            [CanBeNull] Type returnType)
        {
            Contract.Requires(schedule != null);
            Contract.Requires(maximumHistory > 0);
            _enabled = 1;
            LastExecutionFinished = Instant.MinValue;
            _schedule = schedule;
            MaximumHistory = maximumHistory;
            ReturnType = returnType;
            HistoryQueue = new CyclicConcurrentQueue<ScheduledActionResult>(MaximumHistory);
            int md;
            unchecked
            {
                md = (int)(maximumDuration.Ticks / NodaConstants.TicksPerMillisecond);
            }
            MaximumDurationMs = md < 0 ? 0 : md;
        }

        /// <summary>
        /// Gets or sets the maximum duration.
        /// </summary>
        /// <value>The maximum duration.</value>
        [PublicAPI]
        public Duration MaximumDuration
        {
            get { return Duration.FromMilliseconds(MaximumDurationMs); }
            set
            {
                if (value <= Duration.Zero) value = Scheduler.DefaultMaximumDuration;
                else if (value > TimeHelpers.OneStandardDay) value = TimeHelpers.OneStandardDay;

                unchecked
                {
                    MaximumDurationMs = (int)(value.Ticks / NodaConstants.TicksPerMillisecond);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is a function.
        /// </summary>
        /// <value><see langword="true" /> if this instance is function; otherwise, <see langword="false" />.</value>
        [PublicAPI]
        public bool IsFunction
        {
            get { return !ReferenceEquals(ReturnType, null); }
        }

        /// <summary>
        /// Gets the execution history.
        /// </summary>
        /// <value>The history.</value>
        [PublicAPI]
        [NotNull]
        public IEnumerable<ScheduledActionResult> History
        {
            get { return HistoryQueue; }
        }

        /// <summary>
        /// Gets the last result (if any).
        /// </summary>
        /// <value>The history.</value>
        [PublicAPI]
        [CanBeNull]
        public ScheduledActionResult LastResult
        {
            get { return HistoryQueue.LastOrDefault(); }
        }

        /// <summary>
        /// Holds the schedule.
        /// </summary>
        [NotNull]
        private ISchedule _schedule;

        /// <summary>
        /// Gets or sets the schedule.
        /// </summary>
        /// <value>The schedule.</value>
        [PublicAPI]
        [NotNull]
        public ISchedule Schedule
        {
            get { return _schedule; }
            set
            {
                Contract.Requires(value != null);
                if (ReferenceEquals(_schedule, value))
                    return;
                _schedule = value;

                RecalculateNextDue(Instant.MinValue);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ScheduledAction"/> is enabled.
        /// </summary>
        /// <value><see langword="true" /> if enabled; otherwise, <see langword="false" />.</value>
        [PublicAPI]
        public bool Enabled
        {
            get { return _enabled > 0; }
            set
            {
                if (!value)
                {
                    Interlocked.CompareExchange(ref _enabled, 0, 1);
                    return;
                }

                if (Interlocked.CompareExchange(ref _enabled, 1, 0) > 0) return;

                // We have been enabled, recheck schedule.
                Scheduler.CheckSchedule();
            }
        }

        /// <summary>
        /// Executes the action asynchronously, de-bouncing if necessary.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An awaitable task containing the result.</returns>
        [NotNull]
        public Task<ScheduledActionResult> ExecuteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return DoExecuteAsync(cancellationToken);
        }

        /// <summary>
        /// Executes the action asynchronously, de-bouncing if necessary.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An awaitable task containing the result.</returns>
        [NotNull]
        protected abstract Task<ScheduledActionResult> DoExecuteAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// The next time the action is due.
        /// </summary>
        protected long NextDueTicksInternal = Scheduler.MaxTicks;

        /// <summary>
        /// Gets the next due date and time (UTC).
        /// </summary>
        /// <value>The next due date and time.</value>
        [PublicAPI]
        public Instant NextDue
        {
            get
            {
                long ndt = Interlocked.Read(ref NextDueTicksInternal);
                return new Instant(ndt);
            }
        }

        /// <summary>
        /// Gets the next due date and time (UTC) in ticks.
        /// </summary>
        /// <value>The next due date and time.</value>
        [PublicAPI]
        public long NextDueTicks
        {
            get { return Interlocked.Read(ref NextDueTicksInternal); }
        }
        
        /// <summary>
        /// Gets the date and time (UTC) that the last execution finished.
        /// </summary>
        /// <value>The last execution finished date and time.</value>
        [PublicAPI]
        public Instant LastExecutionFinished { get; protected set; }

        protected long ExecutionCountInternal;

        /// <summary>
        /// Gets the execution count.
        /// </summary>
        /// <value>The execution count.</value>
        [PublicAPI]
        public long ExecutionCount
        {
            get { return ExecutionCountInternal; }
        }

        /// <summary>
        /// Lock used in the case optimistic setting fails.
        /// </summary>
        private int _calculatorCount;

        private int _enabled;

        /// <summary>
        /// Recalculates the next due date.
        /// </summary>
        /// <param name="due">The instant the schedule was last due to run, or completed.</param>
        internal void RecalculateNextDue(Instant due)
        {
            // Increment recalculate counter, only let first increment continue.
            if (Interlocked.Increment(ref _calculatorCount) > 1)
                return;

            do
            {
                try
                {
                    long ndt = Interlocked.Read(ref NextDueTicksInternal);
                    Instant now = Scheduler.Clock.Now;
                    long nt = now.Ticks;

                    // If next due is in future, ask schedule when we're next due.
                    if (ndt > nt)
                    {
                        ScheduleOptions options = Schedule.Options;
                        Instant last;
                        if (!options.HasFlag(ScheduleOptions.FromDue))
                        {
                            Instant lef = LastExecutionFinished;
                            last = lef > Instant.MinValue
                                ? lef
                                : Scheduler.Clock.Now;
                        }
                        else
                            last = due > Instant.MinValue
                                ? due
                                : Scheduler.Clock.Now;

                        ndt = Schedule.Next(last).Ticks;

                        // If options >= 4 means one of the alignment flags is set.
                        if ((byte)options >= 4)
                        {
                            // Align the ticks as per flags.
                            if (options.HasFlag(ScheduleOptions.AlignHours))
                            {
                                ndt = ((ndt + NodaConstants.TicksPerHour - 1) / NodaConstants.TicksPerHour) *
                                      NodaConstants.TicksPerHour;
                            }
                            else if (options.HasFlag(ScheduleOptions.AlignMinutes))
                            {
                                ndt = ((ndt + NodaConstants.TicksPerMinute - 1) / NodaConstants.TicksPerMinute) *
                                      NodaConstants.TicksPerMinute;
                            }
                            else if (options.HasFlag(ScheduleOptions.AlignSeconds))
                            {
                                ndt = ((ndt + NodaConstants.TicksPerSecond - 1) / NodaConstants.TicksPerSecond) *
                                      NodaConstants.TicksPerSecond;
                            }
                        }
                    }

                    // If it's more than the max clamp to max.
                    if (ndt > Scheduler.MaxTicks) ndt = Scheduler.MaxTicks;

                    // Update next due
                    Interlocked.Exchange(ref NextDueTicksInternal, ndt);
                }
                finally
                {
                    // Mark update as done.
                    Interlocked.Decrement(ref _calculatorCount);
                }

                // Keep going if we need to recalculate.
            } while (_calculatorCount < 0);

            // Notify the scheduler that we've changed our due date.
            Scheduler.CheckSchedule();
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><see langword="true" /> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <see langword="false" />.</returns>
        public override bool Equals([CanBeNull] object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            ScheduledAction a = obj as ScheduledAction;
            return !ReferenceEquals(a, null) && Equals(a);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals([CanBeNull] ScheduledAction other)
        {
            if (ReferenceEquals(null, other)) return false;
            return ReferenceEquals(this, other) || ID.Equals(other.ID);
        }

        /// <summary>
        /// Indicates whether the left object is equal to the right object of the same type.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>true if the <paramref name="left" /> parameter is equal to the <paramref name="right" /> parameter; otherwise, false.</returns>
        [PublicAPI]
        public static bool Equals([CanBeNull] ScheduledAction left, [CanBeNull] ScheduledAction right)
        {
            if (ReferenceEquals(null, left)) return ReferenceEquals(null, right);
            if (ReferenceEquals(null, right)) return false;
            return ReferenceEquals(left, right) || left.ID.Equals(right.ID);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        /// <summary>
        /// Implements the ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==([CanBeNull] ScheduledAction left, [CanBeNull] ScheduledAction right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Implements the !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(ScheduledAction left, ScheduledAction right)
        {
            return !Equals(left, right);
        }
    }
}