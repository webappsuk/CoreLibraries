using JetBrains.Annotations;
using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace WebApplications.Utilities.Threading
{
    /// <summary>
    /// Debounces an asynchronous function.
    /// </summary>
    public class AsyncDebouncedFunction
    {
        /// <summary>
        /// The semaphore controls concurrent access to function execution.
        /// </summary>
        [NotNull] private readonly AsyncLock _lock = new AsyncLock();

        /// <summary>
        /// The asynchronous function to run.
        /// </summary>
        [NotNull] private readonly Func<CancellationToken, Task> _function;

        /// <summary>
        /// The gap ticks is the number of ticks to leave after a successfully completed function invocation.
        /// </summary>
        private readonly long _gapTicks;

        /// <summary>
        /// The next run is the earliest tick that the function can be run again.
        /// </summary>
        private long _nextRun;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncDebouncedFunction"/> class.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <param name="minimumGap">The minimum gap, is the time left after a successful execution before the function can be run again.</param>
        [PublicAPI]
        public AsyncDebouncedFunction(
            [NotNull] Func<Task> function,
            TimeSpan minimumGap = default(TimeSpan))
            : this(token => function(), minimumGap)
        {
            Contract.Requires(function != null);
            Contract.Requires(minimumGap >= TimeSpan.Zero);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncDebouncedFunction" /> class.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <param name="minimumGap">The minimum gap, is the time left after a successful execution before the function can be run again.</param>
        [PublicAPI]
        public AsyncDebouncedFunction(
            [NotNull] Func<CancellationToken, Task> function,
            TimeSpan minimumGap = default(TimeSpan))
        {
            Contract.Requires(function != null);
            Contract.Requires(minimumGap >= TimeSpan.Zero);

            // Calculate the gap ticks, based on stopwatch frequency.
            _gapTicks = minimumGap.Ticks;
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
        public async Task Run(CancellationToken token = default (CancellationToken))
        {
            // Record when the request was made.
            long requested = DateTime.UtcNow.Ticks;

            // Wait until the semaphore says we can go.
            using (await _lock.LockAsync(token))
            {
                // If we're cancelled, or we were requested before the next run date we're done.
                token.ThrowIfCancellationRequested();

                if (requested <= _nextRun)
                    return;

                // Await on a task.
                await _function(token);

                // If we're cancelled we don't update our next run as we were unsuccessful.
                token.ThrowIfCancellationRequested();

                // Set the next time you can run the function.
                _nextRun = DateTime.UtcNow.Ticks + _gapTicks;
            }
        }
    }

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
        /// The gap ticks is the number of ticks to leave after a successfully completed function invocation.
        /// </summary>
        private readonly long _gapTicks;

        /// <summary>
        /// The next run is the earliest tick that the function can be run again.
        /// </summary>
        private long _nextRun;

        /// <summary>
        /// The last result.
        /// </summary>
        private TResult _lastresult;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncDebouncedFunction{T}"/> class.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <param name="minimumGap">The minimum gap, is the time left after a successful execution before the function can be run again.</param>
        [PublicAPI]
        public AsyncDebouncedFunction(
            [NotNull] Func<Task<TResult>> function,
            TimeSpan minimumGap = default(TimeSpan))
            : this(token => function(), minimumGap)
        {
            Contract.Requires(function != null);
            Contract.Requires(minimumGap >= TimeSpan.Zero);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncDebouncedFunction{T}"/> class.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <param name="minimumGap">The minimum gap, is the time left after a successful execution before the function can be run again.</param>
        [PublicAPI]
        public AsyncDebouncedFunction(
            [NotNull]Func<CancellationToken, Task<TResult>> function,
            TimeSpan minimumGap = default(TimeSpan))
        {
            Contract.Requires(function != null);
            Contract.Requires(minimumGap >= TimeSpan.Zero);

            // Calculate the gap ticks, based on stopwatch frequency.
            _gapTicks = minimumGap.Ticks;
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
        public async Task<TResult> Run(CancellationToken token = default (CancellationToken))
        {
            // Record when the request was made.
            long requested = DateTime.UtcNow.Ticks;

            // Wait until the semaphore says we can go.
            using (await _lock.LockAsync(token))
            {
                // If we're cancelled, or we were requested before the next run date we're done.
                token.ThrowIfCancellationRequested();

                if (requested <= _nextRun)
                    return _lastresult;

                // Await on a task.
                TResult lastResult = await _function(token);

                // If we're cancelled we don't update our next run as we were unsuccessful.
                token.ThrowIfCancellationRequested();

                // Set the next time you can run the function.
                _lastresult = lastResult;
                _nextRun = DateTime.UtcNow.Ticks + _gapTicks;
                return lastResult;
            }
        }
    }
}
