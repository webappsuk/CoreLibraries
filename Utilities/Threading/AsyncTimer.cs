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
using System.Threading;
using System.Threading.Tasks;
using NodaTime;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Threading
{
    /// <summary>
    /// Represents the method that handles calls from an <see cref="AsyncTimer"/>.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public delegate Task AsyncTimerCallback(CancellationToken cancellationToken);

    /// <summary>
    /// Provides a mechanism for executing an asynchronous method at specified intervals.
    /// </summary>
    [PublicAPI]
    public sealed class AsyncTimer : IDisposable
    {
        /// <summary>
        /// The minimum period (in milliseconds), prevents the timer thrashing too quickly.
        /// </summary>
        public const int MinimumPeriodMs = 50;

        /// <summary>
        /// The minimum period, prevents the timer thrashing too quickly.
        /// </summary>
        public static readonly Duration MinimumPeriod = Duration.FromMilliseconds(MinimumPeriodMs);

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
            /// The time stamp (in <see cref="Duration">duration ticks</see>), for when these timeouts were set.
            /// </summary>
            public readonly long TimeStamp;

            /// <summary>
            /// The time stamp (in <see cref="Duration">duration ticks</see>), for <see cref="DueTimeMs"/>.
            /// </summary>
            /// <remarks>If <see cref="M:Change"/> was called and the DueTime was not updated, this field will have the 
            /// same value as in the old TimeOut. Otherwise, this will be equal to the <see cref="TimeStamp"/> + <see cref="DueTimeMs"/> (converted to ticks)</remarks>
            public readonly long DueTimeStamp;

            /// <summary>
            /// Initializes a new instance of the <see cref="TimeOuts" /> class.
            /// </summary>
            /// <param name="period">The period.</param>
            /// <param name="dueTime">The due time.</param>
            /// <param name="minimumGap">The minimum gap.</param>
            /// <param name="timeStamp">The time stamp.</param>
            /// <param name="dueTimeStamp">The due time stamp.</param>
            public TimeOuts(
                Duration period,
                Duration dueTime,
                Duration minimumGap,
                long timeStamp,
                long? dueTimeStamp = null)
            {
                TimeStamp = timeStamp;
                DueTimeMs = dueTime < Duration.Zero ? -1 : (int)dueTime.TotalMilliseconds();
                MinimumGapMs = minimumGap < Duration.Zero ? -1 : (int)minimumGap.TotalMilliseconds();
                PeriodMs = period < Duration.Zero
                    ? -1
                    : (period < MinimumPeriod ? MinimumPeriodMs : (int)period.TotalMilliseconds());
                DueTimeStamp = dueTime < Duration.Zero
                    ? long.MaxValue
                    : (dueTimeStamp ?? TimeStamp + (DueTimeMs * NodaConstants.TicksPerMillisecond));
            }

            /// <summary>
            /// The due time between the last time the timeouts were changed (see <see cref="TimeStamp"/>) and the start of the task invocation.
            /// </summary>
            public Duration DueTime => Duration.FromMilliseconds(DueTimeMs);

            /// <summary>
            /// The minimum gap  between the start of the task invocation and the end of the previous task invocation.
            /// </summary>
            public Duration MinimumGap => Duration.FromMilliseconds(MinimumGapMs);

            /// <summary>
            /// The minimum gap between the start of the task invocation and the start of the previous task invocation.
            /// </summary>
            public Duration Period => Duration.FromMilliseconds(PeriodMs);
        }

        [NotNull]
        private readonly AsyncTimerCallback _callback;

        private readonly PauseToken _pauseToken;

        private CancellationTokenSource _cancellationTokenSource;
        private CancellationTokenSource _timeOutsChanged;

        private TimeOuts _timeOuts;

        private int _runImmediate;
        private TaskCompletionSource<bool> _callbackCompletionSource;

        [CanBeNull]
        private readonly Action<Exception> _errorHandler;


        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncTimer" /> class.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="period">The minimum gap (in milliseconds) between the start of the task invocation and the start of the previous task invocation (defaults to <see cref="Timeout.Infinite"/>).</param>
        /// <param name="dueTime">The due time (in milliseconds) between the last time the timeouts were changed and the start of the task invocation (defaults to 0ms).</param>
        /// <param name="minimumGap">The minimum gap (in milliseconds) between the start of the task invocation and the end of the previous task invocation (defaults to 0ms).</param>
        /// <param name="pauseToken">The pause token for pasuing the timer.</param>
        /// <param name="errorHandler">The optional error handler.</param>
        public AsyncTimer(
            [NotNull] Action callback,
            int period = Timeout.Infinite,
            int dueTime = 0,
            int minimumGap = 0,
            PauseToken pauseToken = default(PauseToken),
            Action<Exception> errorHandler = null)
            : this(
                t =>
                {
                    callback();
                    return TaskResult.Completed;
                },
                Duration.FromMilliseconds(period),
                Duration.FromMilliseconds(dueTime),
                Duration.FromMilliseconds(minimumGap),
                pauseToken,
                errorHandler)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncTimer" /> class.
        /// </summary>
        /// <param name="callback">The asynchronous method to be executed.</param>
        /// <param name="period">The minimum gap between the start of the task invocation and the start of the previous task invocation (defautls to <see langword="null" /> which is equivalent to <see cref="TimeHelpers.InfiniteDuration" />).</param>
        /// <param name="dueTime">The due time between the last time the timeouts were changed and the start of the task invocation (defautls to <see langword="null" /> which is equivalent to <see cref="Duration.Zero" />).</param>
        /// <param name="minimumGap">The minimum gap between the start of the task invocation and the end of the previous task invocation (defautls to <see langword="null" /> which is equivalent to <see cref="Duration.Zero" />).</param>
        /// <param name="pauseToken">The pause token for pasuing the timer.</param>
        /// <param name="errorHandler">The optional error handler.</param>
        public AsyncTimer(
            [NotNull] Action callback,
            Duration? period = null,
            Duration? dueTime = null,
            Duration? minimumGap = null,
            PauseToken pauseToken = default(PauseToken),
            Action<Exception> errorHandler = null)
            : this(
                t =>
                {
                    callback();
                    return TaskResult.Completed;
                },
                period,
                dueTime,
                minimumGap,
                pauseToken,
                errorHandler)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncTimer" /> class.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="period">The minimum gap (in milliseconds) between the start of the task invocation and the start of the previous task invocation (defaults to <see cref="Timeout.Infinite"/>).</param>
        /// <param name="dueTime">The due time (in milliseconds) between the last time the timeouts were changed and the start of the task invocation (defaults to 0ms).</param>
        /// <param name="minimumGap">The minimum gap (in milliseconds) between the start of the task invocation and the end of the previous task invocation (defaults to 0ms).</param>
        /// <param name="pauseToken">The pause token for pasuing the timer.</param>
        /// <param name="errorHandler">The optional error handler.</param>
        public AsyncTimer(
            [NotNull] Action<CancellationToken> callback,
            int period = Timeout.Infinite,
            int dueTime = 0,
            int minimumGap = 0,
            PauseToken pauseToken = default(PauseToken),
            Action<Exception> errorHandler = null)
            : this(
                t =>
                {
                    callback(t);
                    return TaskResult.Completed;
                },
                Duration.FromMilliseconds(period),
                Duration.FromMilliseconds(dueTime),
                Duration.FromMilliseconds(minimumGap),
                pauseToken,
                errorHandler)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncTimer" /> class.
        /// </summary>
        /// <param name="callback">The asynchronous method to be executed.</param>
        /// <param name="period">The minimum gap between the start of the task invocation and the start of the previous task invocation (defautls to <see langword="null" /> which is equivalent to <see cref="TimeHelpers.InfiniteDuration" />).</param>
        /// <param name="dueTime">The due time between the last time the timeouts were changed and the start of the task invocation (defautls to <see langword="null" /> which is equivalent to <see cref="Duration.Zero" />).</param>
        /// <param name="minimumGap">The minimum gap between the start of the task invocation and the end of the previous task invocation (defautls to <see langword="null" /> which is equivalent to <see cref="Duration.Zero" />).</param>
        /// <param name="pauseToken">The pause token for pasuing the timer.</param>
        /// <param name="errorHandler">The optional error handler.</param>
        public AsyncTimer(
            [NotNull] Action<CancellationToken> callback,
            Duration? period = null,
            Duration? dueTime = null,
            Duration? minimumGap = null,
            PauseToken pauseToken = default(PauseToken),
            Action<Exception> errorHandler = null)
            : this(
                t =>
                {
                    callback(t);
                    return TaskResult.Completed;
                },
                period,
                dueTime,
                minimumGap,
                pauseToken,
                errorHandler)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncTimer" /> class.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="period">The minimum gap (in milliseconds) between the start of the task invocation and the start of the previous task invocation (defaults to <see cref="Timeout.Infinite"/>).</param>
        /// <param name="dueTime">The due time (in milliseconds) between the last time the timeouts were changed and the start of the task invocation (defaults to 0ms).</param>
        /// <param name="minimumGap">The minimum gap (in milliseconds) between the start of the task invocation and the end of the previous task invocation (defaults to 0ms).</param>
        /// <param name="pauseToken">The pause token for pasuing the timer.</param>
        /// <param name="errorHandler">The optional error handler.</param>
        public AsyncTimer(
            [NotNull] AsyncTimerCallback callback,
            int period = Timeout.Infinite,
            int dueTime = 0,
            int minimumGap = 0,
            PauseToken pauseToken = default(PauseToken),
            Action<Exception> errorHandler = null)
            : this(callback,
                   Duration.FromMilliseconds(period),
                   Duration.FromMilliseconds(dueTime),
                   Duration.FromMilliseconds(minimumGap),
                   pauseToken,
                   errorHandler)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncTimer" /> class.
        /// </summary>
        /// <param name="callback">The asynchronous method to be executed.</param>
        /// <param name="period">The minimum gap between the start of the task invocation and the start of the previous task invocation (defautls to <see langword="null" /> which is equivalent to <see cref="TimeHelpers.InfiniteDuration" />).</param>
        /// <param name="dueTime">The due time between the last time the timeouts were changed and the start of the task invocation (defautls to <see langword="null" /> which is equivalent to <see cref="Duration.Zero" />).</param>
        /// <param name="minimumGap">The minimum gap between the start of the task invocation and the end of the previous task invocation (defautls to <see langword="null" /> which is equivalent to <see cref="Duration.Zero" />).</param>
        /// <param name="pauseToken">The pause token for pasuing the timer.</param>
        /// <param name="errorHandler">The optional error handler.</param>
        public AsyncTimer(
            [NotNull] AsyncTimerCallback callback,
            Duration? period = null,
            Duration? dueTime = null,
            Duration? minimumGap = null,
            PauseToken pauseToken = default(PauseToken),
            Action<Exception> errorHandler = null)
        {
            if (callback == null) throw new ArgumentNullException(nameof(callback));

            long timeStamp = HighPrecisionClock.Instance.NowTicks;
            _callback = callback;
            _pauseToken = pauseToken;
            _timeOuts = new TimeOuts(
                period ?? TimeHelpers.InfiniteDuration,
                dueTime ?? Duration.Zero,
                minimumGap ?? Duration.Zero,
                timeStamp);

            _cancellationTokenSource = new CancellationTokenSource();
            _timeOutsChanged = new CancellationTokenSource();
            _callbackCompletionSource = null;

            _errorHandler = errorHandler;

            Task.Run(() => TimerTask(_cancellationTokenSource.Token), _cancellationTokenSource.Token);
        }

        /// <summary>
        /// The timer task executes the callback asynchronously after set delays.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        // ReSharper disable once FunctionComplexityOverflow
        private async Task TimerTask(CancellationToken cancellationToken)
        {
            long startTicks = long.MinValue;
            long endTicks = long.MinValue;
            while (!cancellationToken.IsCancellationRequested)
                try
                {
                    CancellationTokenSource timeoutsChanged;

                    // Check we're not set to run immediately
                    if (Interlocked.Exchange(ref _runImmediate, 0) == 0)
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

                            // If we have run immediate set at this point, we can't rely on the correct _timeOutsChanged cts being cancelled.
                            if (Interlocked.Exchange(ref _runImmediate, 0) > 0) break;

                            using (ITokenSource tokenSource = cancellationToken.CreateLinked(timeoutsChanged.Token))
                            {
                                // Check for pausing.
                                try
                                {
                                    await _pauseToken.WaitWhilePausedAsync(tokenSource.Token).ConfigureAwait(false);
                                }
                                catch (OperationCanceledException)
                                {
                                }
                                catch (Exception exception)
                                {
                                    if (!ReferenceEquals(_errorHandler, null))
                                        _errorHandler(exception);
                                }

                                if (cancellationToken.IsCancellationRequested) return;

                                // Get timeouts
                                TimeOuts timeOuts = _timeOuts;
                                if (ReferenceEquals(timeOuts, null)) return;

                                if (timeOuts.DueTimeMs < 0 ||
                                    (startTicks > timeOuts.DueTimeStamp && (timeOuts.MinimumGapMs < 0 || timeOuts.PeriodMs < 0)))
                                {
                                    // If we have infinite waits then we are effectively awaiting cancellation
                                    // ReSharper disable once PossibleNullReferenceException
                                    await tokenSource.ConfigureAwait(false);

                                    if (cancellationToken.IsCancellationRequested) return;
                                    continue;
                                }

                                // If all timeouts are zero we effectively run again immediately (after checking we didn't get a cancellation
                                // indicating the value have changed again).
                                if (timeOuts.DueTimeMs == 0 &&
                                    timeOuts.MinimumGapMs == 0 &&
                                    timeOuts.PeriodMs == 0)
                                    continue;

                                int wait;

                                if (startTicks > long.MinValue)
                                {
                                    // Calculate the wait time based on the minimum gap and the period.
                                    long now = HighPrecisionClock.Instance.NowTicks;
                                    int a = timeOuts.PeriodMs -
                                            (int)((now - startTicks) / NodaConstants.TicksPerMillisecond);
                                    int b = timeOuts.MinimumGapMs -
                                            (int)((now - endTicks) / NodaConstants.TicksPerMillisecond);
                                    int c = (int)((timeOuts.DueTimeStamp - now) / NodaConstants.TicksPerMillisecond);

                                    wait = Math.Max(a, Math.Max(b, c));
                                }
                                else
                                    // Wait the initial due time
                                    wait =
                                        (int)
                                            ((timeOuts.DueTimeStamp - HighPrecisionClock.Instance.NowTicks) /
                                             NodaConstants.TicksPerMillisecond);

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
                                catch (Exception exception)
                                {
                                    if (!ReferenceEquals(_errorHandler, null))
                                        _errorHandler(exception);
                                }
                            }

                            // Recalculate wait time if 'cancelled' due to signal, and not set to run immediately; or if we're currently paused.
                        } while (
                            _pauseToken.IsPaused ||
                            (timeoutsChanged.IsCancellationRequested &&
                             !cancellationToken.IsCancellationRequested &&
                             Interlocked.Exchange(ref _runImmediate, 0) < 1));

                    if (cancellationToken.IsCancellationRequested) return;

                    try
                    {
                        Interlocked.CompareExchange(
                            ref _callbackCompletionSource,
                            new TaskCompletionSource<bool>(),
                            null);

                        startTicks = HighPrecisionClock.Instance.NowTicks;

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

                        if (!ReferenceEquals(_errorHandler, null))
                            _errorHandler(exception);
                    }
                    finally
                    {
                        endTicks = HighPrecisionClock.Instance.NowTicks;

                        // If run immediately was set whilst we were running, we can clear it.
                        Interlocked.Exchange(ref _runImmediate, 0);

                        TaskCompletionSource<bool> callbackCompletionSource =
                            Interlocked.Exchange(ref _callbackCompletionSource, null);

                        // If the completion source is not null, then someone is awaiting last execution, so complete the task
                        if (!ReferenceEquals(callbackCompletionSource, null))
                            callbackCompletionSource.TrySetResult(true);
                    }
                }
                catch (Exception exception)
                {
                    if (!ReferenceEquals(_errorHandler, null))
                        _errorHandler(exception);
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
        /// <para>This makes it easy to programmatically trigger the execution of a task that normally runs on a timer.</para>
        /// </remarks>
        [NotNull]
        public async Task ExecuteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            CancellationTokenSource timeOutsChanged = _timeOutsChanged;
            // If we don't have a cancellation token we're disposed
            if (ReferenceEquals(timeOutsChanged, null))
                throw new ObjectDisposedException("AsyncTimer");

            TaskCompletionSource<bool> callbackCompletionSource = new TaskCompletionSource<bool>();
            TaskCompletionSource<bool> oldTcs = Interlocked.CompareExchange(
                ref _callbackCompletionSource,
                callbackCompletionSource,
                null);
            if (ReferenceEquals(oldTcs, null))
            {
                Interlocked.Exchange(ref _runImmediate, 1);

                timeOutsChanged.Cancel();
            }
            else
                callbackCompletionSource = oldTcs;

            // NOTE: Can't just cancel the TCS with the cancellation token as it's Task may be returned in further calls to this method
            // ReSharper disable once PossibleNullReferenceException, AssignNullToNotNullAttribute
            await callbackCompletionSource.Task.WithCancellation(cancellationToken).ConfigureAwait(false);

            // By yielding, we prevent the awaiter from blocking the timer task.
            await Task.Yield();
        }

        /// <summary>
        /// Changes the specified due time and period.
        /// </summary>
        /// <param name="period">The optional minimum gap (in milliseconds) between the start of the task invocation and the start of the previous task invocation; use <see langword="null"/> to leave the value unchanged.</param>
        /// <param name="dueTime">The optional due time (in milliseconds) between the last time the timeouts were changed and the start of the task invocation; use <see langword="null"/> to leave the value unchanged.</param>
        /// <param name="minimumGap">The optional minimum gap (in milliseconds) between the start of the task invocation and the end of the previous task invocation; use <see langword="null"/> to leave the value unchanged.</param>
        public void Change(int? period = null, int? dueTime = null, int? minimumGap = null)
        {
            Change(
                period.HasValue ? Duration.FromMilliseconds(period.Value) : (Duration?)null,
                dueTime.HasValue ? Duration.FromMilliseconds(dueTime.Value) : (Duration?)null,
                minimumGap.HasValue ? Duration.FromMilliseconds(minimumGap.Value) : (Duration?)null);
        }

        /// <summary>
        /// Changes the specified due time and period.
        /// </summary>
        /// <param name="period">The optional minimum gap between the start of the task invocation and the start of the previous task invocation; use <see langword="null"/> to leave the value unchanged.</param>
        /// <param name="dueTime">The optional due time between the last time the timeouts were changed and the start of the task invocation; use <see langword="null"/> to leave the value unchanged.</param>
        /// <param name="minimumGap">The optional minimum gap between the start of the task invocation and the end of the previous task invocation; use <see langword="null"/> to leave the value unchanged.</param>
        public void Change(Duration? period = null, Duration? dueTime = null, Duration? minimumGap = null)
        {
            long timeStamp = HighPrecisionClock.Instance.NowTicks;
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
                if (ReferenceEquals(timeOutsChanged, null))
                    throw new ObjectDisposedException("AsyncTimer");

                // Update the timeOuts and cancel timeOutsChanged, as timeOuts includes a timestamp it always changes.
                _timeOuts = new TimeOuts(period.Value, dueTime.Value, minimumGap.Value, timeStamp);
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
                        period ?? oldTimeOuts.Period,
                        dueTime ?? oldTimeOuts.DueTime,
                        minimumGap ?? oldTimeOuts.MinimumGap,
                        timeStamp,
                        dueTime.HasValue ? (long?)null : oldTimeOuts.DueTimeStamp);
                } while (Interlocked.CompareExchange(ref _timeOuts, newTimeOuts, oldTimeOuts) != oldTimeOuts);

                CancellationTokenSource timeOutsChanged = _timeOutsChanged;
                // If we don't have a cancellation token we're disposed
                if (ReferenceEquals(timeOutsChanged, null))
                    throw new ObjectDisposedException("AsyncTimer");

                timeOutsChanged.Cancel();
            }
        }

        /// <summary>
        /// The due time between the last time the timeouts were changed and the start of the task invocation.
        /// </summary>
        /// <value>
        /// The due time.
        /// </value>
        public Duration DueTime
        {
            get
            {
                TimeOuts timeouts = _timeOuts;
                if (ReferenceEquals(timeouts, null))
                    throw new ObjectDisposedException("AsyncTimer");
                return timeouts.DueTime;
            }
        }

        /// <summary>
        /// The minimum gap  between the start of the task invocation and the end of the previous task invocation.
        /// </summary>
        /// <value>
        /// The minimum gap.
        /// </value>
        public Duration MinimumGap
        {
            get
            {
                TimeOuts timeouts = _timeOuts;
                if (ReferenceEquals(timeouts, null))
                    throw new ObjectDisposedException("AsyncTimer");
                return timeouts.MinimumGap;
            }
        }

        /// <summary>
        /// The minimum gap between the start of the task invocation and the start of the previous task invocation.
        /// </summary>
        /// <value>
        /// The period.
        /// </value>
        public Duration Period
        {
            get
            {
                TimeOuts timeouts = _timeOuts;
                if (ReferenceEquals(timeouts, null))
                    throw new ObjectDisposedException("AsyncTimer");
                return timeouts.Period;
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