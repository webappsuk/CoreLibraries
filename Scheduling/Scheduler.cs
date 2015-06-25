#region © Copyright Web Applications (UK) Ltd, 2015.  All rights reserved.
// Copyright (c) 2015, Web Applications UK Ltd
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NodaTime;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Scheduling.Configuration;
using WebApplications.Utilities.Scheduling.Scheduled;

namespace WebApplications.Utilities.Scheduling
{
    /// <summary>
    /// Implements a scheduler
    /// </summary>
    /// <remarks></remarks>
    [PublicAPI]
    public static class Scheduler
    {
        /// <summary>
        /// The maximum ticks.
        /// </summary>
        internal static readonly long MaxTicks = Instant.MaxValue.Ticks;

        /// <summary>
        /// The maximum time span supported by a timer (in milliseconds).
        /// </summary>
        private const long MaxTimerMs = 0xfffffffe;

        /// <summary>
        /// The named schedules.
        /// </summary>
        [NotNull]
        private static readonly ConcurrentDictionary<string, ScheduleInfo> _schedules =
            new ConcurrentDictionary<string, ScheduleInfo>();

        private struct ScheduleInfo : IEquatable<ScheduleInfo>
        {
            /// <summary>
            /// Whether the schedule was loaded from the configuration.
            /// </summary>
            public readonly bool IsFromConfiguration;

            [NotNull]
            private readonly ISchedule _schedule;

            /// <summary>
            /// Gets the schedule.
            /// </summary>
            [NotNull]
            public ISchedule Schedule
            {
                get
                {
                    Debug.Assert(_schedule != null);
                    return _schedule;
                }
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ScheduleInfo"/> struct.
            /// </summary>
            /// <param name="isFromConfiguration">if set to <see langword="true" /> [is from configuration].</param>
            /// <param name="schedule">The schedule.</param>
            /// <exception cref="System.ArgumentNullException">schedule</exception>
            public ScheduleInfo(bool isFromConfiguration, [NotNull] ISchedule schedule)
            {
                IsFromConfiguration = isFromConfiguration;
                _schedule = schedule;
            }

            /// <summary>
            /// Indicates whether the current object is equal to another object of the same type.
            /// </summary>
            /// <param name="other">An object to compare with this object.</param>
            /// <returns>
            /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
            /// </returns>
            public bool Equals(ScheduleInfo other)
            {
                return IsFromConfiguration == other.IsFromConfiguration && _schedule.Equals(other._schedule);
            }

            /// <summary>
            /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
            /// </summary>
            /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
            /// <returns>
            ///   <see langword="true" /> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <see langword="false" />.
            /// </returns>
            public override bool Equals(object obj)
            {
                return obj is ScheduleInfo && Equals((ScheduleInfo)obj);
            }

            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <returns>
            /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
            /// </returns>
            public override int GetHashCode()
            {
                unchecked
                {
                    return (IsFromConfiguration.GetHashCode() * 397) ^ _schedule.GetHashCode();
                }
            }
        }

        /// <summary>
        /// Holds all scheduled actions.
        /// </summary>
        [NotNull]
        private static readonly ConcurrentDictionary<CombGuid, ScheduledAction> _actions =
            new ConcurrentDictionary<CombGuid, ScheduledAction>();

        /// <summary>
        /// The tick state.
        /// 0 = Inactive
        /// 1 = Running
        /// 2 = 
        /// </summary>
        private static int _tickState;

        [NotNull]
        private static readonly Timer _ticker;

        /// <summary>
        /// Initializes the <see cref="Scheduler"/> class.
        /// </summary>
        static Scheduler()
        {
            SchedulerConfiguration schedulerConfiguration = SchedulerConfiguration.Active;
            Debug.Assert(schedulerConfiguration != null);

            _ticker = new Timer(CheckSchedule, null, Timeout.Infinite, Timeout.Infinite);

            LoadConfiguration();
            SchedulerConfiguration.Changed += (s, e) => LoadConfiguration();
        }

        /// <summary>
        /// Loads the configuration.
        /// </summary>
        private static void LoadConfiguration()
        {
            SchedulerConfiguration schedulerConfiguration = SchedulerConfiguration.Active;
            Debug.Assert(schedulerConfiguration != null);

            DefaultMaximumHistory = schedulerConfiguration.DefautlMaximumHistory;
            DefaultMaximumDuration = schedulerConfiguration.DefaultMaximumDuration;

            // Get the names of the schedules that were last loaded from the configuration
            HashSet<string> current = new HashSet<string>(
                _schedules.Values.Where(s => s.IsFromConfiguration).Select(s => s.Schedule.Name));

            foreach (ScheduleElement scheduleElement in schedulerConfiguration.Schedules)
            {
                ISchedule schedule = AddSchedule(scheduleElement.GetInstance<ISchedule>(), true);

                // Remove the names of schedules that are in the config
                current.Remove(schedule.Name);
            }

            // Any schedules left should be removed.
            foreach (string name in current)
            {
                Debug.Assert(name != null);
                RemoveSchedule(name);
            }

            Enabled = schedulerConfiguration.Enabled;
        }

        #region Named Schedules
        /// <summary>
        /// Gets the schedule with the specific name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>ISchedule.</returns>
        [NotNull]
        public static ISchedule GetSchedule([NotNull] string name)
        {
            if (name == null) throw new ArgumentNullException("name");

            ScheduleInfo schedule;
            if (!_schedules.TryGetValue(name, out schedule))
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                throw new ArgumentOutOfRangeException(
                    "name",
                    string.Format(Resource.Scheduler_GetSchedule_NotFound, name));
            }

            Debug.Assert(Equals(schedule.Schedule.Name, name));
            return schedule.Schedule;
        }

        /// <summary>
        /// Tries to get the schedule with the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="schedule">The schedule.</param>
        /// <returns><see langword="true" /> if found, <see langword="false" /> otherwise.</returns>
        [ContractAnnotation("=>true,schedule:notnull;=>false,schedule:null")]
        public static bool TryGetSchedule([NotNull] string name, out ISchedule schedule)
        {
            if (name == null) throw new ArgumentNullException("name");

            ScheduleInfo scheduleInfo;
            if (_schedules.TryGetValue(name, out scheduleInfo))
            {
                schedule = scheduleInfo.Schedule;
                return true;
            }
            schedule = null;
            return false;
        }

        /// <summary>
        /// Adds the <see cref="Schedule"/> with the specified <see cref="ISchedule.Name"/> - which must not be <see langword="null"/>.
        /// </summary>
        /// <param name="schedule">The schedule.</param>
        /// <returns>The <see cref="ISchedule" /> removed, if any; otherwise <see langword="null" />.</returns>
        [NotNull]
        public static ISchedule AddSchedule([NotNull] ISchedule schedule)
        {
            return AddSchedule(schedule, false);
        }

        /// <summary>
        /// Adds the <see cref="ISchedule"/> with the specified <see cref="ISchedule.Name"/> - which must not be <see langword="null"/>.
        /// </summary>
        /// <param name="schedule">The schedule.</param>
        /// <param name="isFromConfiguration"></param>
        /// <returns>The <see cref="ISchedule" /> removed, if any; otherwise <see langword="null" />.</returns>
        [NotNull]
        private static ISchedule AddSchedule([NotNull] ISchedule schedule, bool isFromConfiguration)
        {
            if (schedule == null) throw new ArgumentNullException("schedule");
            if (schedule.Name == null)
                throw new ArgumentNullException("schedule", Resource.Scheduler_AddSchedule_SchenduleNameNull);

            ScheduleInfo info = new ScheduleInfo(isFromConfiguration, schedule);

            // ReSharper disable AssignNullToNotNullAttribute
            return _schedules.AddOrUpdate(schedule.Name, info, (k, v) => Equals(v, info) ? v : info).Schedule;
            // ReSharper restore AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Removes the schedule with the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The <see cref="ISchedule"/> removed, if any; otherwise <see langword="null"/>.</returns>
        [CanBeNull]
        public static ISchedule RemoveSchedule([NotNull] string name)
        {
            if (name == null) throw new ArgumentNullException("name");

            ScheduleInfo schedule;
            return _schedules.TryRemove(name, out schedule)
                // ReSharper disable once PossibleNullReferenceException
                ? schedule.Schedule
                : null;
        }
        #endregion

        #region Add Actions Overloads

        #region Add using named schedule
        /// <summary>
        /// Schedules the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="scheduleName">Name of the schedule.</param>
        /// <param name="maximumHistory">The maximum history (default to <see cref="DefaultMaximumHistory"/>).</param>
        /// <param name="maximumDuration">The maximum duration (default to <see cref="DefaultMaximumDuration"/>).</param>
        /// <returns>A <see cref="ScheduledAction" />.</returns>
        [NotNull]
        public static ScheduledAction Add(
            [NotNull] ScheduledAction.SchedulableAction action,
            [NotNull] string scheduleName,
            int maximumHistory = -1,
            Duration maximumDuration = default(Duration))
        {
            if (action == null) throw new ArgumentNullException("action");
            if (scheduleName == null) throw new ArgumentNullException("scheduleName");

            // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute
            return Add(
                false,
                (d, t) =>
                {
                    action();
                    return TaskResult.True;
                },
                GetSchedule(scheduleName),
                maximumHistory,
                maximumDuration);
            // ReSharper restore AssignNullToNotNullAttribute, PossibleNullReferenceException
        }

        /// <summary>
        /// Schedules the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="scheduleName">Name of the schedule.</param>
        /// <param name="maximumHistory">The maximum history (default to <see cref="DefaultMaximumHistory"/>).</param>
        /// <param name="maximumDuration">The maximum duration (default to <see cref="DefaultMaximumDuration"/>).</param>
        /// <returns>A <see cref="ScheduledAction"/>.</returns>
        [NotNull]
        public static ScheduledAction Add(
            [NotNull] ScheduledAction.SchedulableDueAction action,
            [NotNull] string scheduleName,
            int maximumHistory = -1,
            Duration maximumDuration = default(Duration))
        {
            if (action == null) throw new ArgumentNullException("action");
            if (scheduleName == null) throw new ArgumentNullException("scheduleName");

            // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute
            return Add(
                false,
                (d, t) =>
                {
                    action(d);
                    return TaskResult.True;
                },
                GetSchedule(scheduleName),
                maximumHistory,
                maximumDuration);
            // ReSharper restore AssignNullToNotNullAttribute, PossibleNullReferenceException
        }

        /// <summary>
        /// Schedules the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="scheduleName">Name of the schedule.</param>
        /// <param name="maximumHistory">The maximum history (default to <see cref="DefaultMaximumHistory"/>).</param>
        /// <param name="maximumDuration">The maximum duration (default to <see cref="DefaultMaximumDuration"/>).</param>
        /// <returns>A <see cref="ScheduledAction" />.</returns>
        [NotNull]
        public static ScheduledAction Add(
            [NotNull] ScheduledAction.SchedulableCancellableActionAsync action,
            [NotNull] string scheduleName,
            int maximumHistory = -1,
            Duration maximumDuration = default(Duration))
        {
            if (action == null) throw new ArgumentNullException("action");
            if (scheduleName == null) throw new ArgumentNullException("scheduleName");

            // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute
            return Add(
                false,
                (d, t) =>
                    action(t)
                    .ContinueWith(
                        _ => true,
                        t,
                        TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion,
                        TaskScheduler.Current),
                GetSchedule(scheduleName),
                maximumHistory,
                maximumDuration);
            // ReSharper restore AssignNullToNotNullAttribute, PossibleNullReferenceException
        }

        /// <summary>
        /// Schedules the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="scheduleName">Name of the schedule.</param>
        /// <param name="maximumHistory">The maximum history (default to <see cref="DefaultMaximumHistory"/>).</param>
        /// <param name="maximumDuration">The maximum duration (default to <see cref="DefaultMaximumDuration"/>).</param>
        /// <returns>A <see cref="ScheduledAction" />.</returns>
        [NotNull]
        public static ScheduledAction Add(
            [NotNull] ScheduledAction.SchedulableDueCancellableActionAsync action,
            [NotNull] string scheduleName,
            int maximumHistory = -1,
            Duration maximumDuration = default(Duration))
        {
            if (action == null) throw new ArgumentNullException("action");
            if (scheduleName == null) throw new ArgumentNullException("scheduleName");

            // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute
            return Add(
                false,
                (d, t) =>
                    action(d, t)
                    .ContinueWith(
                        _ => true,
                        t,
                        TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion,
                        TaskScheduler.Current),
                GetSchedule(scheduleName),
                maximumHistory,
                maximumDuration);
            // ReSharper restore AssignNullToNotNullAttribute, PossibleNullReferenceException
        }

        /// <summary>
        /// Schedules the specified function.
        /// </summary>
        /// <typeparam name="T">The return type</typeparam>
        /// <param name="function">The function.</param>
        /// <param name="scheduleName">Name of the schedule.</param>
        /// <param name="maximumHistory">The maximum history (default to <see cref="DefaultMaximumHistory"/>).</param>
        /// <param name="maximumDuration">The maximum duration (default to <see cref="DefaultMaximumDuration"/>).</param>
        /// <returns>A <see cref="ScheduledFunction{T}" />.</returns>
        [NotNull]
        public static ScheduledFunction<T> Add<T>(
            [NotNull] ScheduledFunction<T>.SchedulableFunction function,
            [NotNull] string scheduleName,
            int maximumHistory = -1,
            Duration maximumDuration = default(Duration))
        {
            if (function == null) throw new ArgumentNullException("function");
            if (scheduleName == null) throw new ArgumentNullException("scheduleName");

            return Add(
                true,
                (d, t) => Task.FromResult(function()),
                GetSchedule(scheduleName),
                maximumHistory,
                maximumDuration);
        }

        /// <summary>
        /// Schedules the specified function.
        /// </summary>
        /// <typeparam name="T">The return type</typeparam>
        /// <param name="function">The function.</param>
        /// <param name="scheduleName">Name of the schedule.</param>
        /// <param name="maximumHistory">The maximum history (default to <see cref="DefaultMaximumHistory"/>).</param>
        /// <param name="maximumDuration">The maximum duration (default to <see cref="DefaultMaximumDuration"/>).</param>
        /// <returns>A <see cref="ScheduledFunction{T}" />.</returns>
        [NotNull]
        public static ScheduledFunction<T> Add<T>(
            [NotNull] ScheduledFunction<T>.SchedulableDueFunction function,
            [NotNull] string scheduleName,
            int maximumHistory = -1,
            Duration maximumDuration = default(Duration))
        {
            if (function == null) throw new ArgumentNullException("function");
            if (scheduleName == null) throw new ArgumentNullException("scheduleName");

            return Add(
                true,
                (d, t) => Task.FromResult(function(d)),
                GetSchedule(scheduleName),
                maximumHistory,
                maximumDuration);
        }

        /// <summary>
        /// Schedules the specified function.
        /// </summary>
        /// <typeparam name="T">The return type</typeparam>
        /// <param name="function">The function.</param>
        /// <param name="scheduleName">Name of the schedule.</param>
        /// <param name="maximumHistory">The maximum history (default to <see cref="DefaultMaximumHistory"/>).</param>
        /// <param name="maximumDuration">The maximum duration (default to <see cref="DefaultMaximumDuration"/>).</param>
        /// <returns>A <see cref="ScheduledFunction{T}" />.</returns>
        [NotNull]
        public static ScheduledFunction<T> Add<T>(
            [NotNull] ScheduledFunction<T>.SchedulableCancellableFunctionAsync function,
            [NotNull] string scheduleName,
            int maximumHistory = -1,
            Duration maximumDuration = default(Duration))
        {
            if (function == null) throw new ArgumentNullException("function");
            if (scheduleName == null) throw new ArgumentNullException("scheduleName");

            return Add(true, (d, t) => function(t), GetSchedule(scheduleName), maximumHistory, maximumDuration);
        }

        /// <summary>
        /// Schedules the specified function.
        /// </summary>
        /// <typeparam name="T">The return type</typeparam>
        /// <param name="function">The function.</param>
        /// <param name="scheduleName">Name of the schedule.</param>
        /// <param name="maximumHistory">The maximum history (default to <see cref="DefaultMaximumHistory"/>).</param>
        /// <param name="maximumDuration">The maximum duration (default to <see cref="DefaultMaximumDuration"/>).</param>
        /// <returns>A <see cref="ScheduledFunction{T}" />.</returns>
        [NotNull]
        public static ScheduledFunction<T> Add<T>(
            [NotNull] ScheduledFunction<T>.SchedulableDueCancellableFunctionAsync function,
            [NotNull] string scheduleName,
            int maximumHistory = -1,
            Duration maximumDuration = default(Duration))
        {
            if (function == null) throw new ArgumentNullException("function");
            if (scheduleName == null) throw new ArgumentNullException("scheduleName");

            return Add(true, function, GetSchedule(scheduleName), maximumHistory, maximumDuration);
        }
        #endregion

        #region Add using ISchedule
        /// <summary>
        /// Schedules the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="schedule">The schedule.</param>
        /// <param name="maximumHistory">The maximum history (default to <see cref="DefaultMaximumHistory"/>).</param>
        /// <param name="maximumDuration">The maximum duration (default to <see cref="DefaultMaximumDuration"/>).</param>
        /// <returns>A <see cref="ScheduledAction" />.</returns>
        [NotNull]
        public static ScheduledAction Add(
            [NotNull] ScheduledAction.SchedulableAction action,
            [NotNull] ISchedule schedule,
            int maximumHistory = -1,
            Duration maximumDuration = default(Duration))
        {
            if (action == null) throw new ArgumentNullException("action");
            if (schedule == null) throw new ArgumentNullException("schedule");

            // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute
            return Add(
                false,
                (d, t) =>
                {
                    action();
                    return TaskResult.True;
                },
                schedule,
                maximumHistory,
                maximumDuration);
            // ReSharper restore AssignNullToNotNullAttribute, PossibleNullReferenceException
        }

        /// <summary>
        /// Schedules the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="schedule">The schedule.</param>
        /// <param name="maximumHistory">The maximum history (default to <see cref="DefaultMaximumHistory"/>).</param>
        /// <param name="maximumDuration">The maximum duration (default to <see cref="DefaultMaximumDuration"/>).</param>
        /// <returns>A <see cref="ScheduledAction" />.</returns>
        [NotNull]
        public static ScheduledAction Add(
            [NotNull] ScheduledAction.SchedulableDueAction action,
            [NotNull] ISchedule schedule,
            int maximumHistory = -1,
            Duration maximumDuration = default(Duration))
        {
            if (action == null) throw new ArgumentNullException("action");
            if (schedule == null) throw new ArgumentNullException("schedule");

            // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute
            return Add(
                false,
                (d, t) =>
                {
                    action(d);
                    return TaskResult.True;
                },
                schedule,
                maximumHistory,
                maximumDuration);
            // ReSharper restore AssignNullToNotNullAttribute, PossibleNullReferenceException
        }

        /// <summary>
        /// Schedules the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="schedule">The schedule.</param>
        /// <param name="maximumHistory">The maximum history (default to <see cref="DefaultMaximumHistory"/>).</param>
        /// <param name="maximumDuration">The maximum duration (default to <see cref="DefaultMaximumDuration"/>).</param>
        /// <returns>A <see cref="ScheduledAction" />.</returns>
        [NotNull]
        public static ScheduledAction Add(
            [NotNull] ScheduledAction.SchedulableCancellableActionAsync action,
            [NotNull] ISchedule schedule,
            int maximumHistory = -1,
            Duration maximumDuration = default(Duration))
        {
            if (action == null) throw new ArgumentNullException("action");
            if (schedule == null) throw new ArgumentNullException("schedule");

            // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute
            return Add(
                false,
                (d, t) =>
                    action(t)
                    .ContinueWith(
                        _ => true,
                        t,
                        TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion,
                        TaskScheduler.Current),
                schedule,
                maximumHistory,
                maximumDuration);
            // ReSharper restore AssignNullToNotNullAttribute, PossibleNullReferenceException
        }

        /// <summary>
        /// Schedules the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="schedule">The schedule.</param>
        /// <param name="maximumHistory">The maximum history (default to <see cref="DefaultMaximumHistory"/>).</param>
        /// <param name="maximumDuration">The maximum duration (default to <see cref="DefaultMaximumDuration"/>).</param>
        /// <returns>A <see cref="ScheduledAction" />.</returns>
        [NotNull]
        public static ScheduledAction Add(
            [NotNull] ScheduledAction.SchedulableDueCancellableActionAsync action,
            [NotNull] ISchedule schedule,
            int maximumHistory = -1,
            Duration maximumDuration = default(Duration))
        {
            if (action == null) throw new ArgumentNullException("action");
            if (schedule == null) throw new ArgumentNullException("schedule");

            // ReSharper disable PossibleNullReferenceException, AssignNullToNotNullAttribute
            return Add(
                false,
                (d, t) =>
                    action(d, t)
                    .ContinueWith(
                        _ => true,
                        t,
                        TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion,
                        TaskScheduler.Current),
                schedule,
                maximumHistory,
                maximumDuration);
            // ReSharper restore AssignNullToNotNullAttribute, PossibleNullReferenceException
        }

        /// <summary>
        /// Schedules the specified function.
        /// </summary>
        /// <typeparam name="T">The return type</typeparam>
        /// <param name="function">The function.</param>
        /// <param name="schedule">The schedule.</param>
        /// <param name="maximumHistory">The maximum history (default to <see cref="DefaultMaximumHistory"/>).</param>
        /// <param name="maximumDuration">The maximum duration (default to <see cref="DefaultMaximumDuration"/>).</param>
        /// <returns>A <see cref="ScheduledFunction{T}"/>.</returns>
        [NotNull]
        public static ScheduledFunction<T> Add<T>(
            [NotNull] ScheduledFunction<T>.SchedulableFunction function,
            [NotNull] ISchedule schedule,
            int maximumHistory = -1,
            Duration maximumDuration = default(Duration))
        {
            if (function == null) throw new ArgumentNullException("function");
            if (schedule == null) throw new ArgumentNullException("schedule");

            return Add(true, (d, t) => Task.FromResult(function()), schedule, maximumHistory, maximumDuration);
        }

        /// <summary>
        /// Schedules the specified function.
        /// </summary>
        /// <typeparam name="T">The return type</typeparam>
        /// <param name="function">The function.</param>
        /// <param name="schedule">The schedule.</param>
        /// <param name="maximumHistory">The maximum history (default to <see cref="DefaultMaximumHistory"/>).</param>
        /// <param name="maximumDuration">The maximum duration (default to <see cref="DefaultMaximumDuration"/>).</param>
        /// <returns>A <see cref="ScheduledFunction{T}"/>.</returns>
        [NotNull]
        public static ScheduledFunction<T> Add<T>(
            [NotNull] ScheduledFunction<T>.SchedulableDueFunction function,
            [NotNull] ISchedule schedule,
            int maximumHistory = -1,
            Duration maximumDuration = default(Duration))
        {
            if (function == null) throw new ArgumentNullException("function");
            if (schedule == null) throw new ArgumentNullException("schedule");

            return Add(true, (d, t) => Task.FromResult(function(d)), schedule, maximumHistory, maximumDuration);
        }

        /// <summary>
        /// Schedules the specified function.
        /// </summary>
        /// <typeparam name="T">The return type</typeparam>
        /// <param name="function">The function.</param>
        /// <param name="schedule">The schedule.</param>
        /// <param name="maximumHistory">The maximum history (default to <see cref="DefaultMaximumHistory"/>).</param>
        /// <param name="maximumDuration">The maximum duration (default to <see cref="DefaultMaximumDuration"/>).</param>
        /// <returns>A <see cref="ScheduledFunction{T}"/>.</returns>
        [NotNull]
        public static ScheduledFunction<T> Add<T>(
            [NotNull] ScheduledFunction<T>.SchedulableCancellableFunctionAsync function,
            [NotNull] ISchedule schedule,
            int maximumHistory = -1,
            Duration maximumDuration = default(Duration))
        {
            if (function == null) throw new ArgumentNullException("function");
            if (schedule == null) throw new ArgumentNullException("schedule");

            return Add(true, (d, t) => function(t), schedule, maximumHistory, maximumDuration);
        }

        /// <summary>
        /// Schedules the specified function.
        /// </summary>
        /// <typeparam name="T">The return type</typeparam>
        /// <param name="function">The function.</param>
        /// <param name="schedule">The schedule.</param>
        /// <param name="maximumHistory">The maximum history (default to <see cref="DefaultMaximumHistory"/>).</param>
        /// <param name="maximumDuration">The maximum duration (default to <see cref="DefaultMaximumDuration"/>).</param>
        /// <returns>A <see cref="ScheduledFunction{T}"/>.</returns>
        [NotNull]
        public static ScheduledFunction<T> Add<T>(
            [NotNull] ScheduledFunction<T>.SchedulableDueCancellableFunctionAsync function,
            [NotNull] ISchedule schedule,
            int maximumHistory = -1,
            Duration maximumDuration = default(Duration))
        {
            if (function == null) throw new ArgumentNullException("function");
            if (schedule == null) throw new ArgumentNullException("schedule");

            return Add(true, function, schedule, maximumHistory, maximumDuration);
        }

        /// <summary>
        /// Schedules the specified function.
        /// </summary>
        /// <typeparam name="T">The return type</typeparam>
        /// <param name="isFunction">if set to <see langword="true" /> [is function].</param>
        /// <param name="function">The function.</param>
        /// <param name="schedule">The schedule.</param>
        /// <param name="maximumHistory">The maximum history (default to <see cref="DefaultMaximumHistory"/>).</param>
        /// <param name="maximumDuration">The maximum duration (default to <see cref="DefaultMaximumDuration"/>).</param>
        /// <returns>A <see cref="ScheduledFunction{T}" />.</returns>
        [NotNull]
        private static ScheduledFunction<T> Add<T>(
            bool isFunction,
            [NotNull] ScheduledFunction<T>.SchedulableDueCancellableFunctionAsync function,
            [NotNull] ISchedule schedule,
            int maximumHistory = -1,
            Duration maximumDuration = default(Duration))
        {
            if (function == null) throw new ArgumentNullException("function");
            if (schedule == null) throw new ArgumentNullException("schedule");

            ScheduledFunction<T> sf = new ScheduledFunction<T>(
                isFunction,
                function,
                schedule,
                maximumHistory < 1 ? _defaultMaximumHistory : maximumHistory,
                maximumDuration <= Duration.Zero ? _defaultMaximumDuration : maximumDuration);

            // Update actions dictionary
            _actions.AddOrUpdate(sf.ID, sf, (i, a) => sf);

            // Calculate next due
            sf.RecalculateNextDue(Instant.MinValue);
            return sf;
        }
        #endregion

        #endregion

        #region Remove actions Overloads
        /// <summary>
        /// Removes the specified scheduled action.
        /// </summary>
        /// <param name="scheduledAction">The scheduled action.</param>
        /// <returns><see langword="true" /> if removed, <see langword="false" /> otherwise.</returns>
        public static bool Remove([NotNull] ScheduledAction scheduledAction)
        {
            if (scheduledAction == null) throw new ArgumentNullException("scheduledAction");

            ScheduledAction a;
            bool result = _actions.TryRemove(scheduledAction.ID, out a);

            Debug.Assert(!result || (a != null && a.ID == scheduledAction.ID));
            return result;
        }

        /// <summary>
        /// Removes the specified scheduled function.
        /// </summary>
        /// <param name="scheduledFunction">The scheduled action.</param>
        /// <returns><see langword="true" /> if removed, <see langword="false" /> otherwise.</returns>
        public static bool Remove<T>([NotNull] ScheduledFunction<T> scheduledFunction)
        {
            if (scheduledFunction == null) throw new ArgumentNullException("scheduledFunction");

            ScheduledAction a;
            bool result = _actions.TryRemove(scheduledFunction.ID, out a);

            Debug.Assert(!result || (a != null && a.ID == scheduledFunction.ID));
            return result;
        }
        #endregion

        /// <summary>
        /// Gets or sets the maximum duration of an action.
        /// </summary>
        /// <value>The maximum duration of any action.</value>
        public static Duration DefaultMaximumDuration
        {
            get { return _defaultMaximumDuration; }
            set
            {
                if (value.TotalMilliseconds() < 10.0) throw new ArgumentOutOfRangeException("value");
                if (value.TotalStandardDays() > 1.0) throw new ArgumentOutOfRangeException("value");

                _defaultMaximumDuration = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Scheduler"/> is enabled.
        /// </summary>
        /// <value><see langword="true" /> if enabled; otherwise, <see langword="false" />.</value>
        public static bool Enabled
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
                CheckSchedule();
            }
        }

        private static int _defaultMaximumHistory;

        /// <summary>
        /// Gets or sets the default maximum history.
        /// </summary>
        /// <value>The default maximum history.</value>
        /// <exception cref="System.ArgumentOutOfRangeException">The default maximum history cannot be negative.</exception>
        public static int DefaultMaximumHistory
        {
            get { return _defaultMaximumHistory; }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("value", Resource.Scheduler_DefaultMaximumHistory_Negative);
                _defaultMaximumHistory = value;
            }
        }

        private static ScheduledAction _nextScheduledAction;

        /// <summary>
        /// Gets the next scheduled action (if any).
        /// </summary>
        /// <value>The next scheduled action.</value>
        [CanBeNull]
        public static ScheduledAction NextScheduledAction
        {
            get { return _nextScheduledAction; }
        }

        /// <summary>
        /// The next time the action is due.
        /// </summary>
        private static long _nextDueTicks;

        private static Duration _defaultMaximumDuration;
        private static int _enabled;

        /// <summary>
        /// Gets the next due date and time (UTC).
        /// </summary>
        /// <value>The next due date and time.</value>
        public static Instant NextDue
        {
            get
            {
                long ndt = Interlocked.Read(ref _nextDueTicks);
                return new Instant(ndt);
            }
        }

        /// <summary>
        /// Called when the scheduling timer is retrieved.
        /// </summary>
        /// <param name="state">The state.</param>
        internal static void CheckSchedule([CanBeNull] object state = null)
        {
            // Only allow one check to run at a time, namely the check that caused the tick state to move from 0 to 1.
            // The increment will force any currently executing check to recheck.
            if (Interlocked.Increment(ref _tickState) > 1) return;

            do
            {
                long wait;
                do
                {
                    // Check if we're disabled.
                    if (_enabled < 1)
                    {
                        Interlocked.Exchange(ref _tickState, 0);
                        return;
                    }

                    // Ensure ticker is stopped.
                    _ticker.Change(Timeout.Infinite, Timeout.Infinite);

                    long ndt = MaxTicks;

                    // Set our tick state to 1, we're about to do a complete check of the actions if any other tick calls
                    // come in from this point onwards we will need to recheck.
                    ScheduledAction nextScheduledAction = null;
                    Interlocked.Exchange(ref _tickState, 1);
                    foreach (ScheduledAction action in _actions.Values)
                    {
                        Debug.Assert(action != null);
                        if (!action.Enabled) continue;

                        long andt = action.NextDueTicks;

                        if (andt < TimeHelpers.Clock.Now.Ticks)
                            // Due now so kick of an execution (shouldn't block).
                            action.ExecuteAsync();
                        else if (andt < ndt)
                        {
                            // Update the next due time.
                            nextScheduledAction = action;
                            ndt = andt;
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
                    if (ndt >= MaxTicks)
                    {
                        // Set to infinite wait.
                        wait = long.MaxValue;

                        // Update properties.
                        Interlocked.Exchange(ref _nextDueTicks, MaxTicks);
                        Interlocked.Exchange(ref _nextScheduledAction, null);
                        break;
                    }

                    // Update properties.
                    Interlocked.Exchange(ref _nextDueTicks, ndt);
                    Interlocked.Exchange(ref _nextScheduledAction, nextScheduledAction);

                    long now = TimeHelpers.Clock.Now.Ticks;

                    // If we're due in the future calculate how long to wait.
                    if (ndt > now)
                    {
                        long wt = ndt - now;
                        if (wt > 0)
                        {
                            // Check we're waiting at least a millisecond.
                            if (wt > NodaConstants.TicksPerMillisecond)
                            {
                                // Calculate number of milliseconds to wait.
                                wait = (ndt - now) / NodaConstants.TicksPerMillisecond;
                                // Ensure the wait duration doesn't exceed the maximum supported by Timer.
                                if (wait > MaxTimerMs)
                                    wait = MaxTimerMs;
                                break;
                            }

                            // Use a spin wait instead.
                            SpinWait s = new SpinWait();
                            while (wt > 0)
                            {
                                s.SpinOnce();
                                wt = ndt - TimeHelpers.Clock.Now.Ticks;
                            }
                        }
                    }

                    // We're due in the past so try again.
                    Thread.Yield();
                } while (true);

                // Set the ticker to run after the wait period.
                _ticker.Change(wait <= MaxTimerMs ? wait : Timeout.Infinite, Timeout.Infinite);

                // Try to set the tick state back to 0, from 1 and finish
                if (Interlocked.CompareExchange(ref _tickState, 0, 1) == 1)
                    return;

                Trace.WriteLine("Cancel wait.");
                // The tick state managed to increase from 1 before we could exit, so we need to clear the ticker and recheck.
                _ticker.Change(Timeout.Infinite, Timeout.Infinite);
            } while (true);
        }
    }
}