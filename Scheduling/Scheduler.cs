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

using System.Collections.Concurrent;
using System.Diagnostics.Contracts;
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
    public class Scheduler
    {
        /// <summary>
        /// Holds all scheduled actions.
        /// </summary>
        [NotNull]
        private readonly ConcurrentDictionary<CombGuid, ScheduledAction> _actions =
            new ConcurrentDictionary<CombGuid, ScheduledAction>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Scheduler"/> class.
        /// </summary>
        /// <param name="defaultMaximumHistory">The default maximum history.</param>
        /// <remarks></remarks>
        public Scheduler(int defaultMaximumHistory = 100)
        {
            DefaultMaximumHistory = defaultMaximumHistory;
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
            return Add((d, t) => Task.FromResult(function()), schedule, maximumHistory);
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
            return Add((d, t) => Task.FromResult(function(d)), schedule, maximumHistory);
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
            return Add((d, t) => function(), schedule, maximumHistory);
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
            return Add((d, t) => function(d), schedule, maximumHistory);
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
            return Add((d, t) => function(t), schedule, maximumHistory);
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
            ScheduledFunction<T> sf = new ScheduledFunction<T>(
                function,
                this,
                schedule,
                maximumHistory < 0 ? DefaultMaximumHistory : maximumHistory);
            // Update actions dictionary
            _actions.AddOrUpdate(sf.ID, sf, (i, a) => sf);
            return sf;
        }
        #endregion

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

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Scheduler"/> is enabled.
        /// </summary>
        /// <value><see langword="true" /> if enabled; otherwise, <see langword="false" />.</value>
        [PublicAPI]
        public bool Enabled { get; set; }

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
    }
}