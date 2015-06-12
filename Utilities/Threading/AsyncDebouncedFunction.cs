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
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Threading
{
    /// <summary>
    /// Debounces an asynchronous function.
    /// </summary>
    public class AsyncDebouncedFunction<TResult>
    {
        /// <summary>
        /// The semaphore controls concurrent access to function execution.
        /// </summary>
        [NotNull]
        private readonly AsyncLock _lock = new AsyncLock();

        /// <summary>
        /// The asynchronous function to run.
        /// </summary>
        [NotNull]
        private readonly Func<CancellationToken, Task<TResult>> _function;

        /// <summary>
        /// The duration ticks is the number of ticks to leave after a successfully completed function invocation, from the start of the last successful invocation.
        /// </summary>
        private readonly long _durationTicks;

        /// <summary>
        /// The gap ticks is the number of ticks to leave after a successfully completed function invocation, from the end of the last successful invocation.
        /// </summary>
        private readonly long _minimumGapTicks;

        /// <summary>
        /// The next run is the earliest tick that the function can be run again.
        /// </summary>
        private long _nextRun;

        /// <summary>
        /// The last result.
        /// </summary>
        private TResult _lastResult;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncDebouncedFunction{T}" /> class.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <param name="minimumGap">The minimum gap, is the time left after a successful execution before the function can be run again.</param>
        [PublicAPI]
        public AsyncDebouncedFunction(
            [NotNull] Func<Task<TResult>> function,
            TimeSpan duration = default(TimeSpan),
            TimeSpan minimumGap = default(TimeSpan))
            : this(token => function(), duration, minimumGap)
        {
            if (function == null) throw new ArgumentNullException("function");
            if (duration < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException("duration", Resources.AsyncDebounced_DurationNegative);
            if (minimumGap < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException("minimumGap", Resources.AsyncDebounced_MinimumGapNegative);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncDebouncedFunction{T}"/> class.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <param name="minimumGap">The minimum gap, is the time left after a successful execution before the function can be run again.</param>
        [PublicAPI]
        public AsyncDebouncedFunction(
            [NotNull] Func<CancellationToken, Task<TResult>> function,
            TimeSpan duration = default(TimeSpan),
            TimeSpan minimumGap = default(TimeSpan))
        {
            if (function == null) throw new ArgumentNullException("function");
            if (duration < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException("duration", Resources.AsyncDebounced_DurationNegative);
            if (minimumGap < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException("minimumGap", Resources.AsyncDebounced_MinimumGapNegative);

            // Calculate the gap ticks, based on stopwatch frequency.
            _durationTicks = duration.Ticks;
            _minimumGapTicks = minimumGap.Ticks;
            _function = function;
        }

        /// <summary>
        /// Runs the function asynchronously.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>An awaitable task, that completes with the debounced result.</returns>
        /// <remarks><para>If the function is currently running, this will wait until it completes (or
        /// <paramref name="token"/> has been cancelled),
        /// but won't run the function again.  However, if the original function is cancelled, or errors,
        /// then it will run the function again immediately.</para>
        /// </remarks>
        [NotNull]
        [PublicAPI]
        public async Task<TResult> Run(CancellationToken token = default(CancellationToken))
        {
            // Record when the request was made.
            long requested = DateTime.UtcNow.Ticks;

            // Wait until the semaphore says we can go.
            using (await _lock.LockAsync(token).ConfigureAwait(false))
            {
                // If we're cancelled, or we were requested before the next run date we're done.
                token.ThrowIfCancellationRequested();

                if (requested <= _nextRun)
                    return _lastResult;

                // Await on a task.
                // ReSharper disable once PossibleNullReferenceException - Let it throw
                TResult lastResult = await _function(token).ConfigureAwait(false);

                // If we're cancelled we don't update our next run as we were unsuccessful.
                token.ThrowIfCancellationRequested();

                // Set the next time you can run the function.
                _lastResult = lastResult;
                _nextRun = Math.Max(requested + _durationTicks, DateTime.UtcNow.Ticks + _minimumGapTicks);
                return lastResult;
            }
        }
    }
}