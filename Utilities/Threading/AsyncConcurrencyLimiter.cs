using JetBrains.Annotations;
using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace WebApplications.Utilities.Threading
{
    /// <summary>
    /// Allows an asynchronous task to be run a maximum number of times at the same time.
    /// </summary>
    public class AsyncConcurrencyLimiter
    {
        /// <summary>
        /// The semaphore controls concurrent access to action execution.
        /// </summary>
        [NotNull]
        private readonly AsyncSemaphore _semaphore;

        /// <summary>
        /// The asynchronous action to run.
        /// </summary>
        [NotNull]
        private readonly Func<CancellationToken, Task> _action;

        /// <summary>
        /// The gap ticks is the number of ticks to leave after a successfully completed action invocation.
        /// </summary>
        private readonly long _gapTicks;

        /// <summary>
        /// The next run is the earliest tick that the action can be run again.
        /// </summary>
        private long _nextRun;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncConcurrencyLimiter" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="minimumGap">The minimum gap.</param>
        [PublicAPI]
        public AsyncConcurrencyLimiter(
            [NotNull] Action action,
            TimeSpan minimumGap = default(TimeSpan))
            : this(token => Task.Factory.StartNew(action, token), minimumGap)
        {
            Contract.Requires(action != null);
            Contract.Requires(minimumGap >= TimeSpan.Zero);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncConcurrencyLimiter" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="minimumGap">The minimum gap.</param>
        [PublicAPI]
        public AsyncConcurrencyLimiter(
            [NotNull] Action<CancellationToken> action,
            TimeSpan minimumGap = default(TimeSpan))
            : this(token => Task.Factory.StartNew(() => action(token), token), minimumGap)
        {
            Contract.Requires(action != null);
            Contract.Requires(minimumGap >= TimeSpan.Zero);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncConcurrencyLimiter" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="minimumGap">The minimum gap.</param>
        [PublicAPI]
        public AsyncConcurrencyLimiter(
            [NotNull] Func<Task> action,
            TimeSpan minimumGap = default(TimeSpan))
            : this(token => action(), minimumGap)
        {
            Contract.Requires(action != null);
            Contract.Requires(minimumGap >= TimeSpan.Zero);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncConcurrencyLimiter"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="minimumGap">The minimum gap, is the time left after a successful execution before the action can be run again.</param>
        [PublicAPI]
        public AsyncConcurrencyLimiter(
            [NotNull]Func<CancellationToken, Task> action,
            TimeSpan minimumGap = default(TimeSpan))
        {
            Contract.Requires(action != null);
            Contract.Requires(minimumGap >= TimeSpan.Zero);

            // Calculate the gap ticks, based on stopwatch frequency.
            _gapTicks = minimumGap.Ticks;

            _semaphore = new AsyncSemaphore();
            _action = action;
        }

        /// <summary>
        /// Runs the action asynchronously.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>An awaitable task.</returns>
        /// <remarks><para>If the action is currently running, this will wait until it completes (or <paramref name="token"/> has been cancelled),
        /// but won't run the action again.  However, if the original action is cancelled, or errors, then it will run the action again immediately.</para>
        /// </remarks>
        [NotNull]
        [PublicAPI]
        public async Task Run(CancellationToken token = default (CancellationToken))
        {
            // Record when the request was made.
            long requested = DateTime.UtcNow.Ticks;

            // Wait until the semaphore says we can go.
            await _semaphore.WaitAsync(token);

            try
            {
                // If we're cancelled, or we were requested before the next run date we're done.
                if (token.IsCancellationRequested || (requested <= _nextRun))
                    return;

                // Await on a task.
                await _action(token);

                // If we're cancelled we don't update our next run as we were unsuccessful.
                if (token.IsCancellationRequested) return;

                // Set the next time you can run the action.
                _nextRun = DateTime.UtcNow.Ticks + _gapTicks;
            }
            finally
            {
                // Release the semaphore.
                _semaphore.Release();
            }
        }
    }
}
