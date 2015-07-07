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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NodaTime;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Threading;

namespace WebApplications.Utilities.Scheduling.Scheduled
{
    /// <summary>
    /// Base class for scheduled functions.
    /// </summary>
    /// <typeparam name="T">The return type.</typeparam>
    /// <remarks></remarks>
    [PublicAPI]
    public sealed class ScheduledFunction<T> : ScheduledAction
    {
        #region Delegates
        /// <summary>
        /// Delegate describing a schedulable function.
        /// </summary>
        /// <returns>An awaitable task that contains the result.</returns>
        public delegate T SchedulableFunction();

        /// <summary>
        /// Delegate describing a schedulable function which accepts a due date and time.
        /// </summary>
        /// <param name="due">The due date and time which indicates when the action was scheduled to run.</param>
        /// <returns>An awaitable task that contains the result.</returns>
        public delegate T SchedulableDueFunction(Instant due);

        /// <summary>
        /// Delegate describing an asynchronous schedulable function which supports cancellation.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>An awaitable task that contains the result.</returns>
        public delegate Task<T> SchedulableCancellableFunctionAsync(CancellationToken token);

        /// <summary>
        /// Delegate describing an asynchronous schedulable function which accepts a due date and time and supports cancellation.
        /// </summary>
        /// <param name="due">The due date and time which indicates when the action was scheduled to run.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>An awaitable task that contains the result.</returns>
        public delegate Task<T> SchedulableDueCancellableFunctionAsync(Instant due, CancellationToken token);
        #endregion

        [NotNull]
        private readonly SchedulableDueCancellableFunctionAsync _function;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduledAction" /> class.
        /// </summary>
        /// <param name="isFunction">if set to <see langword="true" /> then this instance is a function.</param>
        /// <param name="function">The function.</param>
        /// <param name="schedule">The schedule.</param>
        /// <param name="maximumHistory">The maximum history.</param>
        /// <param name="maximumDuration">The maximum duration.</param>
        internal ScheduledFunction(
            bool isFunction,
            [NotNull] SchedulableDueCancellableFunctionAsync function,
            [NotNull] ISchedule schedule,
            int maximumHistory,
            Duration maximumDuration)
            : base(schedule, maximumHistory, maximumDuration, isFunction ? typeof(T) : null)
        {
            if (function == null) throw new ArgumentNullException("function");
            if (schedule == null) throw new ArgumentNullException("schedule");
            _function = function;
        }

        /// <summary>
        /// Gets the execution history.
        /// </summary>
        /// <value>The history.</value>
        [NotNull]
        public new IEnumerable<ScheduledFunctionResult<T>> History
        {
            get
            {
                if (!IsFunction) throw new InvalidOperationException();
                return HistoryQueue.Cast<ScheduledFunctionResult<T>>();
            }
        }

        /// <summary>
        /// Gets the last result (if any).
        /// </summary>
        /// <value>The history.</value>
        [CanBeNull]
        public new ScheduledFunctionResult<T> LastResult
        {
            get
            {
                if (!IsFunction) throw new InvalidOperationException();
                return HistoryQueue.LastOrDefault() as ScheduledFunctionResult<T>;
            }
        }

        /// <summary>
        /// The semaphore controls concurrent access to function execution.
        /// </summary>
        [NotNull]
        private readonly AsyncLock _lock = new AsyncLock();

        /// <summary>
        /// Executes the function asynchronously, so long as it is enabled and not already running.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An awaitable task containing the result.</returns>
        [NotNull]
        public new Task<ScheduledFunctionResult<T>> ExecuteAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!IsFunction) throw new InvalidOperationException();
            return DoExecuteAsync(cancellationToken)
                .ContinueWith(
                    // ReSharper disable once PossibleNullReferenceException
                    t => t.Result as ScheduledFunctionResult<T>,
                    cancellationToken,
                    TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.ExecuteSynchronously,
                    // ReSharper disable once AssignNullToNotNullAttribute
                    TaskScheduler.Current);
        }

        /// <summary>
        /// Executes the action asynchronously, de-bouncing if necessary.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An awaitable task containing the result.</returns>
        protected override async Task<ScheduledActionResult> DoExecuteAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Start stopwatch, and mark time the request was made.
            Stopwatch stopwatch = Stopwatch.StartNew();
            Instant started = TimeHelpers.Clock.Now;

            // Combine cancellation token with Timeout to ensure no action runs beyond the Scheduler's limit.
            using (ITokenSource tokenSource = cancellationToken.WithTimeout(MaximumDurationMs))
            {
                // Quick cancellation check.
                // ReSharper disable once PossibleNullReferenceException
                if (tokenSource.IsCancellationRequested)
                {
                    // ReSharper disable once AssignNullToNotNullAttribute
                    return new ScheduledFunctionResult<T>(
                        NextDue,
                        TimeHelpers.Clock.Now,
                        Duration.Zero,
                        null,
                        true,
                        default(T));
                }

                // Always yield to ensure tasks run asynchronously.
                await Task.Yield();

                // Wait until the semaphore says we can go.
                using (await _lock.LockAsync(tokenSource.Token).ConfigureAwait(false))
                {
                    // Quick cancellation check.
                    // ReSharper disable once PossibleNullReferenceException
                    if (tokenSource.IsCancellationRequested)
                    {
                        // ReSharper disable once AssignNullToNotNullAttribute
                        return new ScheduledFunctionResult<T>(
                            NextDue,
                            TimeHelpers.Clock.Now,
                            Duration.Zero,
                            null,
                            true,
                            default(T));
                    }

                    ScheduledFunctionResult<T> result;
                    // De-bounce - we started before the last execution finished, so return last result, so long as it wasn't cancelled.
                    if (started <= LastExecutionFinished)
                    {
                        result = HistoryQueue.LastOrDefault() as ScheduledFunctionResult<T>;
                        if (result != null &&
                            !result.Cancelled) return result;
                    }

                    // Get the due date, and set to not due.
                    long ndt = Interlocked.Exchange(ref NextDueTicksInternal, Scheduler.MaxTicks);
                    Instant due = new Instant(ndt);

                    try
                    {
                        // Execute function (ensuring cancellation).
                        // ReSharper disable once AssignNullToNotNullAttribute
                        T r = await _function(due, tokenSource.Token)
                            .WithCancellation(tokenSource.Token)
                            .ConfigureAwait(false);
                        stopwatch.Stop();

                        result = new ScheduledFunctionResult<T>(
                            due,
                            started,
                            Duration.FromTimeSpan(stopwatch.Elapsed),
                            null,
                            tokenSource.IsCancellationRequested,
                            r);
                    }
                    catch (Exception e)
                    {
                        stopwatch.Stop();

                        bool cancelled = tokenSource.IsCancellationRequested ||
                                         e is TaskCanceledException ||
                                         e is OperationCanceledException;

                        // ReSharper disable once AssignNullToNotNullAttribute
                        result = new ScheduledFunctionResult<T>(
                            due,
                            started,
                            Duration.FromTimeSpan(stopwatch.Elapsed),
                            !cancelled ? e : null,
                            cancelled,
                            default(T));
                    }
                    // Increment the execution count.
                    Interlocked.Increment(ref ExecutionCountInternal);

                    // Mark execution finished
                    LastExecutionFinished = TimeHelpers.Clock.Now;

                    // Enqueue history item.
                    HistoryQueue.Enqueue(result);

                    // Recalculate when we're next due.
                    RecalculateNextDue(due);
                    return result;
                }
            }
        }
    }
}