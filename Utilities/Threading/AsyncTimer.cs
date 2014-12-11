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
            /// Initializes a new instance of the <see cref="TimeOuts"/> class.
            /// </summary>
            /// <param name="dueTime">The due time.</param>
            /// <param name="minimumGap">The minimum gap.</param>
            /// <param name="period">The period.</param>
            public TimeOuts(TimeSpan dueTime, TimeSpan minimumGap, TimeSpan period, long timeStamp)
            {
                TimeStamp = timeStamp;
                if (dueTime < TimeSpan.Zero)
                {
                    DueTimeMs = -1;
                    MinimumGapMs = -1;
                    PeriodMs = -1;
                }
                else
                {
                    DueTimeMs = dueTime < TimeSpan.Zero ? -1 : (int)dueTime.TotalMilliseconds;
                    MinimumGapMs = minimumGap < TimeSpan.Zero ? -1 : (int)minimumGap.TotalMilliseconds;
                    PeriodMs = period < TimeSpan.Zero ? -1 : (int)period.TotalMilliseconds;
                }
            }

            /// <summary>
            /// The due time between the last time the timeouts were changed (see <see cref="TimeStamp"/>) and the start of the task invocation.
            /// </summary>
            public TimeSpan DueTime { get { return TimeSpan.FromMilliseconds(DueTimeMs); } }

            /// <summary>
            /// The minimum gap  between the start of the task invocation and the end of the previous task invocation.
            /// </summary>
            public TimeSpan MinimumGap { get { return TimeSpan.FromMilliseconds(MinimumGapMs); } }

            /// <summary>
            /// The minimum gap between the start of the task invocation and the start of the previous task invocation.
            /// </summary>
            public TimeSpan Period { get { return TimeSpan.FromMilliseconds(PeriodMs); } }
        }

        [NotNull]
        private readonly AsyncTimerCallback _callback;

        private readonly PauseToken _pauseToken;

        private CancellationTokenSource _cancellationTokenSource;
        private CancellationTokenSource _timeOutsChanged;

        private TimeOuts _timeOuts;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncTimer" /> class.
        /// </summary>
        /// <param name="callback">The asynchronous method to be executed.</param>
        /// <param name="dueTime">The due time (in milliseconds) between the last time the timeouts were changed and the start of the task invocation.</param>
        /// <param name="minimumGap">The minimum gap (in milliseconds) between the start of the task invocation and the end of the previous task invocation.</param>
        /// <param name="period">The minimum gap (in milliseconds) between the start of the task invocation and the start of the previous task invocation.</param>
        /// <param name="pauseToken">The pause token.</param>
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
                await _pauseToken.WaitWhilePausedAsync(cancellationToken);
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
                            // ReSharper disable PossibleNullReferenceException
                            await tokenSource.Token.WaitHandle;
                            // ReSharper restore PossibleNullReferenceException

                            if (cancellationToken.IsCancellationRequested) return;
                            continue;
                        }

                        // If all timeouts are zero we effectively run again immediately (after checking we didn't get a cancellation
                        // indicating the value have changed again).
                        if (timeOuts.DueTimeMs == 0 &&
                            timeOuts.MinimumGapMs == 0 &&
                            timeOuts.PeriodMs == 0)
                            continue;

                        // Calculate the wait time based on the minimum gap and the period.
                        long now = Stopwatch.GetTimestamp();
                        int a = timeOuts.PeriodMs - (int)(1000.0 * (now - startTicks) / Stopwatch.Frequency);
                        int b = timeOuts.MinimumGapMs - (int)(1000.0 * (now - endTicks) / Stopwatch.Frequency);
                        int c = timeOuts.DueTimeMs - (int)(1000.0 * (now - timeOuts.TimeStamp) / Stopwatch.Frequency);

                        int wait = Math.Max(a, Math.Max(b, c));

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
                    startTicks = Stopwatch.GetTimestamp();

                    // ReSharper disable PossibleNullReferenceException
                    await _callback(cancellationToken);
                    // ReSharper restore PossibleNullReferenceException

                    if (cancellationToken.IsCancellationRequested) return;
                }
                catch (OperationCanceledException)
                {
                    // Just finish as we're cancelled
                    return;
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch
                {
                    // Supress errors thrown by callback.
                }
                finally
                {
                    endTicks = Stopwatch.GetTimestamp();
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
        /// (e.g. a Log flush)
        /// </remarks>
        public Task Execute(CancellationToken cancellationToken)
        {
            // TODO Effectively we need to tell the timer task to cancel the current wait and proceed immediately to execution
            // then we need to wait on a signal that the current execution has completed.
            // Probably easiest to use a ManualResetEventSlim and await the WaitHandle?
            throw new NotImplementedException();
        }

        /// <summary>
        /// Changes the specified due time and period.
        /// </summary>
        /// <param name="dueTime">The optional due time (in milliseconds) between the last time the timeouts were changed and the start of the task invocation; use <see langword="null"/> to leave the value unchaged.</param>
        /// <param name="minimumGap">The optional minimum gap (in milliseconds) between the start of the task invocation and the end of the previous task invocation; use <see langword="null"/> to leave the value unchaged.</param>
        /// <param name="period">The optional minimum gap (in milliseconds) between the start of the task invocation and the start of the previous task invocation; use <see langword="null"/> to leave the value unchaged.</param>
        [PublicAPI]
        public void Change(TimeSpan? dueTime, TimeSpan? minimumGap, TimeSpan? period)
        {
            long timeStamp = Stopwatch.GetTimestamp();
            bool dueTimeUnchanged = !ReferenceEquals(dueTime, null);
            bool minimumGapUnchanged = !ReferenceEquals(minimumGap, null);
            bool periodUnchanged = !ReferenceEquals(period, null);
            if (dueTimeUnchanged &&
                minimumGapUnchanged &&
                periodUnchanged)
            {
                // Changing everything so we can just go ahea an change

                CancellationTokenSource timeOutsChanged = _timeOutsChanged;
                // If we don't have a cancellation token we're disposed
                if (ReferenceEquals(timeOutsChanged, null)) return;

                // Update the timeOuts and cancel timeOutsChanged, as timeOuts includes a timestamp it always changes.
                _timeOuts = new TimeOuts(dueTime.Value, minimumGap.Value, period.Value, timeStamp);
                timeOutsChanged.Cancel();
            } else if (dueTimeUnchanged ||
                       minimumGapUnchanged ||
                       periodUnchanged)
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
                        timeStamp);

                } while (Interlocked.CompareExchange(ref _timeOuts, newTimeOuts, oldTimeOuts) != oldTimeOuts);
            }
        }

        /// <summary>
        /// The due time between the last time the timeouts were changed (see <see cref="TimeStamp" />) and the start of the task invocation.
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

            _timeOuts = null;
        }
    }
}