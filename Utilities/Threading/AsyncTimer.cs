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
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Threading
{
    /// <summary>
    /// Delegate for a
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public delegate Task AsyncTimerCallback(CancellationToken cancellationToken);

    /// <summary>
    /// Provides a mechanism for executing an asynchronous method at specified intervals.
    /// </summary>
    public sealed class AsyncTimer : IDisposable
    {
        /// <summary>
        /// Factor to use for converting Stopwatch ticks to miliseconds.
        /// </summary>
        private static readonly double _ticksToMs = 1000.0 / Stopwatch.Frequency;

        /// <summary>
        /// Factor to use for converting miliseconds to Stopwatch ticks.
        /// </summary>
        private static readonly double _msToTicks = Stopwatch.Frequency / 1000.0;

        /// <summary>
        /// Holds together timeout information in an immutable object for thread safety.
        /// </summary>
        private class TimeOuts
        {
            /// <summary>
            /// The due time (in milliseconds) between the last time the timeouts were changed (see <see cref="TimeStamp"/>) and the start of the task invocation.
            /// </summary>
            public readonly int DueTimeMs;

            /// <summary>
            /// The minimum gap (in milliseconds) between the start of the task invocation and the end of the previous task invocation.
            /// </summary>
            public readonly int MinimumGapMs;

            /// <summary>
            /// The minimum gap (in milliseconds) between the start of the task invocation and the start of the previous task invocation.
            /// </summary>
            public readonly int PeriodMs;

            /// <summary>
            /// The time stamp (in <see cref="Stopwatch.Frequency">stopwatch ticks</see>), for when these timeouts were set.
            /// </summary>
            public readonly long TimeStamp;

            /// <summary>
            /// The time stamp (in <see cref="Stopwatch.Frequency">stopwatch ticks</see>), for <see cref="DueTimeMs"/>.
            /// </summary>
            /// <remarks>If <see cref="Change"/> was called and the DueTime was not updated, this field will have the 
            /// same value as in the old TimeOut. Otherwise, this will be equal to the <see cref="TimeStamp"/> + <see cref="DueTimeMs"/> (converted to stopwatch ticks)</remarks>
            public readonly long DueTimeStamp;

            /// <summary>
            /// Initializes a new instance of the <see cref="TimeOuts" /> class.
            /// </summary>
            /// <param name="dueTime">The due time.</param>
            /// <param name="minimumGap">The minimum gap.</param>
            /// <param name="period">The period.</param>
            /// <param name="timeStamp">The time stamp.</param>
            /// <param name="dueTimeStamp">The due time stamp.</param>
            public TimeOuts(
                TimeSpan dueTime,
                TimeSpan minimumGap,
                TimeSpan period,
                long timeStamp,
                long? dueTimeStamp = null)
            {
                TimeStamp = timeStamp;
                if (dueTime < TimeSpan.Zero)
                {
                    DueTimeMs = -1;
                    MinimumGapMs = -1;
                    PeriodMs = -1;
                    DueTimeStamp = long.MaxValue;
                }
                else
                {
                    DueTimeMs = dueTime < TimeSpan.Zero ? -1 : (int)dueTime.TotalMilliseconds;
                    MinimumGapMs = minimumGap < TimeSpan.Zero ? -1 : (int)minimumGap.TotalMilliseconds;
                    PeriodMs = period < TimeSpan.Zero ? -1 : (int)period.TotalMilliseconds;
                    DueTimeStamp = dueTimeStamp ?? (long)(TimeStamp + (DueTimeMs * _msToTicks));
                }
            }

            /// <summary>
            /// The due time between the last time the timeouts were changed (see <see cref="TimeStamp"/>) and the start of the task invocation.
            /// </summary>
            public TimeSpan DueTime
            {
                get { return TimeSpan.FromMilliseconds(DueTimeMs); }
            }

            /// <summary>
            /// The minimum gap  between the start of the task invocation and the end of the previous task invocation.
            /// </summary>
            public TimeSpan MinimumGap
            {
                get { return TimeSpan.FromMilliseconds(MinimumGapMs); }
            }

            /// <summary>
            /// The minimum gap between the start of the task invocation and the start of the previous task invocation.
            /// </summary>
            public TimeSpan Period
            {
                get { return TimeSpan.FromMilliseconds(PeriodMs); }
            }
        }

        [NotNull]
        private readonly AsyncTimerCallback _callback;

        private readonly PauseToken _pauseToken;

        private CancellationTokenSource _cancellationTokenSource;
        private CancellationTokenSource _timeOutsChanged;

        private TimeOuts _timeOuts;

        private int _rumImmediate;
        private TaskCompletionSource<bool> _callbackCompletionSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncTimer"/> class.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="dueTime">The due time (in milliseconds) between the last time the timeouts were changed and the start of the task invocation.</param>
        /// <param name="minimumGap">The minimum gap (in milliseconds) between the start of the task invocation and the end of the previous task invocation.</param>
        /// <param name="period">The minimum gap (in milliseconds) between the start of the task invocation and the start of the previous task invocation.</param>
        /// <param name="pauseToken">The pause token for pasuing the timer.</param>
        public AsyncTimer(
            [NotNull] AsyncTimerCallback callback,
            int dueTime = 0,
            int minimumGap = 0,
            int period = 0,
            PauseToken pauseToken = default(PauseToken))
            : this(callback,
                   TimeSpan.FromMilliseconds(dueTime),
                   TimeSpan.FromMilliseconds(minimumGap),
                   TimeSpan.FromMilliseconds(period),
                   pauseToken)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncTimer" /> class.
        /// </summary>
        /// <param name="callback">The asynchronous method to be executed.</param>
        /// <param name="dueTime">The due time between the last time the timeouts were changed and the start of the task invocation.</param>
        /// <param name="minimumGap">The minimum gap between the start of the task invocation and the end of the previous task invocation.</param>
        /// <param name="period">The minimum gap between the start of the task invocation and the start of the previous task invocation.</param>
        /// <param name="pauseToken">The pause token for pasuing the timer.</param>
        public AsyncTimer(
            [NotNull] AsyncTimerCallback callback,
            TimeSpan dueTime = default(TimeSpan),
            TimeSpan minimumGap = default(TimeSpan),
            TimeSpan period = default(TimeSpan),
            PauseToken pauseToken = default(PauseToken))
        {
            Contract.Requires<ArgumentNullException>(callback != null);
            long timeStamp = Stopwatch.GetTimestamp();
            _callback = callback;
            _pauseToken = pauseToken;
            _timeOuts = new TimeOuts(dueTime, minimumGap, period, timeStamp);

            _cancellationTokenSource = new CancellationTokenSource();
            _timeOutsChanged = new CancellationTokenSource();
            _callbackCompletionSource = null;

            Task.Run(() => TimerTask(_cancellationTokenSource.Token), _cancellationTokenSource.Token);
        }

        /// <summary>
        /// The timer task executes the callback asynchronously after set delays.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        private async Task TimerTask(CancellationToken cancellationToken)
        {
            long startTicks = long.MinValue;
            long endTicks = long.MinValue;
            while (!cancellationToken.IsCancellationRequested)
            {
                // Check for pausing.
                await _pauseToken.WaitWhilePausedAsync(cancellationToken).ConfigureAwait(false);
                if (cancellationToken.IsCancellationRequested) return;

                CancellationTokenSource timeoutsChanged;
                do
                {
                    // Create new cancellation token source and set _timeOutsChanged to it in a thread-safe none-locking way.
                    timeoutsChanged = new CancellationTokenSource();
                    CancellationTokenSource toc = Interlocked.Exchange(ref _timeOutsChanged, timeoutsChanged);
                    if (ReferenceEquals(toc, null))
                    {
                        toc = Interlocked.CompareExchange(ref _timeOutsChanged, null, timeoutsChanged);
                        if (!ReferenceEquals(toc, null))
                            toc.Dispose();
                        return;
                    }

                    using (ITokenSource tokenSource = cancellationToken.CreateLinked(timeoutsChanged.Token))
                    {
                        // Get timeouts
                        TimeOuts timeOuts = _timeOuts;
                        if (ReferenceEquals(timeOuts, null)) return;

                        if (timeOuts.DueTimeMs < 0 ||
                            (startTicks > -1 && (timeOuts.MinimumGapMs < 0 || timeOuts.PeriodMs < 0)))
                        {
                            // If we have infinite waits then we are effectively awaiting cancellation
                            // ReSharper disable once PossibleNullReferenceException
                            await tokenSource.ConfigureAwait(false);

                            if (cancellationToken.IsCancellationRequested) return;
                            continue;
                        }

                        // If we need to run immediately, we dont check for cancellation indicating value change
                        if (Interlocked.CompareExchange(ref _rumImmediate, 0, 1) == 1)
                            break;

                        // If all timeouts are zero we effectively run again immediately (after checking we didn't get a cancellation
                        // indicating the value have changed again).
                        if (timeOuts.DueTimeMs == 0 &&
                            timeOuts.MinimumGapMs == 0 &&
                            timeOuts.PeriodMs == 0)
                            continue;

                        int wait;

                        if (startTicks > -1)
                        {
                            // Calculate the wait time based on the minimum gap and the period.
                            long now = Stopwatch.GetTimestamp();
                            int a = timeOuts.PeriodMs - (int)(_ticksToMs * (now - startTicks));
                            int b = timeOuts.MinimumGapMs - (int)(_ticksToMs * (now - endTicks));
                            int c = (int)(_ticksToMs * (timeOuts.DueTimeStamp - now));

                            wait = Math.Max(a, Math.Max(b, c));
                        }
                        else
                        // Wait the initial due time
                            wait = (int)(_ticksToMs * (timeOuts.DueTimeStamp - Stopwatch.GetTimestamp()));

                        // If we don't need to wait run again immediately (after checking values haven't changed).
                        if (wait < 1) continue;

                        try
                        {
                            // Wait for set milliseconds
                            // ReSharper disable PossibleNullReferenceException
                            await Task.Delay(wait, tokenSource.Token).ConfigureAwait(false);
                            // ReSharper restore PossibleNullReferenceException
                        }
                        catch (OperationCanceledException)
                        {
                        }
                    }

                    // Recalculate wait time if 'cancelled' due to signal
                } while (timeoutsChanged.IsCancellationRequested &&
                         !cancellationToken.IsCancellationRequested);

                if (cancellationToken.IsCancellationRequested) return;

                try
                {
                    // Check for pausing.
                    await _pauseToken.WaitWhilePausedAsync(cancellationToken).ConfigureAwait(false);
                    if (cancellationToken.IsCancellationRequested) return;

                    Interlocked.CompareExchange(ref _callbackCompletionSource, new TaskCompletionSource<bool>(), null);

                    startTicks = Stopwatch.GetTimestamp();

                    // ReSharper disable once PossibleNullReferenceException
                    await _callback(cancellationToken).ConfigureAwait(false);

                    if (cancellationToken.IsCancellationRequested) return;
                }
                catch (OperationCanceledException)
                {
                    // Just finish as we're cancelled

                    TaskCompletionSource<bool> callbackCompletionSource =
                        Interlocked.Exchange(ref _callbackCompletionSource, null);

                    // If the completion source is not null, then someone is awaiting last execution, so complete the task
                    if (!ReferenceEquals(callbackCompletionSource, null))
                        callbackCompletionSource.TrySetCanceled();

                    return;
                }
                    // ReSharper disable once EmptyGeneralCatchClause
                catch (Exception exception)
                {
                    // Supress errors thrown by callback, unless someone is awaiting it.

                    TaskCompletionSource<bool> callbackCompletionSource =
                        Interlocked.Exchange(ref _callbackCompletionSource, null);

                    // If the completion source is not null, then someone is awaiting last execution, so complete the task
                    if (!ReferenceEquals(callbackCompletionSource, null))
                        callbackCompletionSource.TrySetException(exception);
                }
                finally
                {
                    endTicks = Stopwatch.GetTimestamp();

                    TaskCompletionSource<bool> callbackCompletionSource =
                        Interlocked.Exchange(ref _callbackCompletionSource, null);

                    // If the completion source is not null, then someone is awaiting last execution, so complete the task
                    if (!ReferenceEquals(callbackCompletionSource, null))
                        callbackCompletionSource.TrySetResult(true);
                }
            }
        }

        /// <summary>
        /// Executes the timer's callback immediately, if it's not currently executing, and allows you to wait for it to finish.  Otheriwse,
        /// it waits for the current execution to finish.  The next execution will then be calculated based on the start/end of this execution.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An awaitable task.
        /// </returns>
        /// <remarks>
        /// This makes it easy to programmatically trigger the execution of a task that normally runs on a timer.
        /// </remarks>#
        [NotNull]
        [PublicAPI]
        public Task Execute(CancellationToken cancellationToken = default(CancellationToken))
        {
            CancellationTokenSource timeOutsChanged = _timeOutsChanged;
            // If we don't have a cancellation token we're disposed
            if (ReferenceEquals(timeOutsChanged, null))
                return TaskResult.Cancelled;

            TaskCompletionSource<bool> callbackCompletionSource = new TaskCompletionSource<bool>();
            TaskCompletionSource<bool> oldTcs = Interlocked.CompareExchange(
                ref _callbackCompletionSource,
                callbackCompletionSource,
                null);
            if (ReferenceEquals(oldTcs, null))
            {
                Interlocked.Exchange(ref _rumImmediate, 1);

                timeOutsChanged.Cancel();
            }
            else
                callbackCompletionSource = oldTcs;

            // NOTE: Can't just cancel the TCS with the cancellation token as it's Task may be returned in further calls to this method
            // ReSharper disable once PossibleNullReferenceException, AssignNullToNotNullAttribute
            return callbackCompletionSource.Task.WithCancellation(cancellationToken);
        }

        /// <summary>
        /// Changes the specified due time and period.
        /// </summary>
        /// <param name="dueTime">The optional due time (in milliseconds) between the last time the timeouts were changed and the start of the task invocation; use <see langword="null"/> to leave the value unchaged.</param>
        /// <param name="minimumGap">The optional minimum gap (in milliseconds) between the start of the task invocation and the end of the previous task invocation; use <see langword="null"/> to leave the value unchaged.</param>
        /// <param name="period">The optional minimum gap (in milliseconds) between the start of the task invocation and the start of the previous task invocation; use <see langword="null"/> to leave the value unchaged.</param>
        [PublicAPI]
        public void Change(int? dueTime = null, int? minimumGap = null, int? period = null)
        {
            Change(
                dueTime.HasValue ? TimeSpan.FromMilliseconds(dueTime.Value) : (TimeSpan?)null,
                minimumGap.HasValue ? TimeSpan.FromMilliseconds(minimumGap.Value) : (TimeSpan?)null,
                period.HasValue ? TimeSpan.FromMilliseconds(period.Value) : (TimeSpan?)null);
        }

        /// <summary>
        /// Changes the specified due time and period.
        /// </summary>
        /// <param name="dueTime">The optional due time between the last time the timeouts were changed and the start of the task invocation; use <see langword="null"/> to leave the value unchaged.</param>
        /// <param name="minimumGap">The optional minimum gap between the start of the task invocation and the end of the previous task invocation; use <see langword="null"/> to leave the value unchaged.</param>
        /// <param name="period">The optional minimum gap between the start of the task invocation and the start of the previous task invocation; use <see langword="null"/> to leave the value unchaged.</param>
        [PublicAPI]
        public void Change(TimeSpan? dueTime = null, TimeSpan? minimumGap = null, TimeSpan? period = null)
        {
            long timeStamp = Stopwatch.GetTimestamp();
            bool dueTimeChanged = !ReferenceEquals(dueTime, null);
            bool minimumGapChanged = !ReferenceEquals(minimumGap, null);
            bool periodChanged = !ReferenceEquals(period, null);
            if (dueTimeChanged &&
                minimumGapChanged &&
                periodChanged)
            {
                // Changing everything so we can just go ahead and change

                CancellationTokenSource timeOutsChanged = _timeOutsChanged;
                // If we don't have a cancellation token we're disposed
                if (ReferenceEquals(timeOutsChanged, null)) return;

                // Update the timeOuts and cancel timeOutsChanged, as timeOuts includes a timestamp it always changes.
                _timeOuts = new TimeOuts(dueTime.Value, minimumGap.Value, period.Value, timeStamp);
                timeOutsChanged.Cancel();
            }
            else if (dueTimeChanged ||
                     minimumGapChanged ||
                     periodChanged)
            {
                TimeOuts newTimeOuts, oldTimeOuts;
                // We're changing at least one thing
                do
                {
                    oldTimeOuts = _timeOuts;
                    // Check we have timeouts (might be disposed)
                    if (ReferenceEquals(oldTimeOuts, null)) return;

                    // If the current timestamp is newer than this ignore
                    if (oldTimeOuts.TimeStamp >= timeStamp) return;

                    newTimeOuts = new TimeOuts(
                        dueTime ?? oldTimeOuts.DueTime,
                        minimumGap ?? oldTimeOuts.MinimumGap,
                        period ?? oldTimeOuts.Period,
                        timeStamp,
                        dueTime.HasValue ? (long?)null : oldTimeOuts.DueTimeStamp);
                } while (Interlocked.CompareExchange(ref _timeOuts, newTimeOuts, oldTimeOuts) != oldTimeOuts);

                CancellationTokenSource timeOutsChanged = _timeOutsChanged;
                // If we don't have a cancellation token we're disposed
                if (ReferenceEquals(timeOutsChanged, null)) return;

                timeOutsChanged.Cancel();
            }
        }

        /// <summary>
        /// The due time between the last time the timeouts were changed and the start of the task invocation.
        /// </summary>
        /// <value>
        /// The due time.
        /// </value>
        [PublicAPI]
        public TimeSpan DueTime
        {
            get
            {
                TimeOuts timeouts = _timeOuts;
                return timeouts != null ? timeouts.DueTime : Timeout.InfiniteTimeSpan;
            }
        }

        /// <summary>
        /// The minimum gap  between the start of the task invocation and the end of the previous task invocation.
        /// </summary>
        /// <value>
        /// The minimum gap.
        /// </value>
        [PublicAPI]
        public TimeSpan MinimumGap
        {
            get
            {
                TimeOuts timeouts = _timeOuts;
                return timeouts != null ? timeouts.MinimumGap : Timeout.InfiniteTimeSpan;
            }
        }

        /// <summary>
        /// The minimum gap between the start of the task invocation and the start of the previous task invocation.
        /// </summary>
        /// <value>
        /// The period.
        /// </value>
        [PublicAPI]
        public TimeSpan Period
        {
            get
            {
                TimeOuts timeouts = _timeOuts;
                return timeouts != null ? timeouts.Period : Timeout.InfiniteTimeSpan;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            CancellationTokenSource cts = Interlocked.Exchange(ref _cancellationTokenSource, null);
            if (!ReferenceEquals(cts, null))
            {
                cts.Cancel();
                cts.Dispose();
            }

            CancellationTokenSource pdc = Interlocked.Exchange(ref _timeOutsChanged, null);
            if (!ReferenceEquals(pdc, null))
            {
                pdc.Cancel();
                pdc.Dispose();
            }

            TaskCompletionSource<bool> tcs = Interlocked.Exchange(ref _callbackCompletionSource, null);
            if (!ReferenceEquals(tcs, null))
                tcs.TrySetCanceled();

            _timeOuts = null;
        }
    }
}