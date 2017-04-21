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
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Threading
{
    /// <summary>
    /// Asynchronous semaphore makes it easy to synchronize/throttle tasks so that no more than a fixed
    /// number can enter a critical region at the same time
    /// </summary>
    /// <remarks>
    /// Originally based on http://blogs.msdn.com/b/pfxteam/archive/2012/02/12/10266983.aspx
    /// </remarks>
    [PublicAPI]
    public class AsyncSemaphore
    {
        [NotNull]
        private readonly Queue<TaskCompletionSource<IDisposable>> _waiters = new Queue<TaskCompletionSource<IDisposable>>();

        private int _currentCount;

        private readonly IDisposable _releaser;

        [NotNull]
        private readonly Task<IDisposable> _releaserTask;

        /// <summary>
        /// Gets the current count.
        /// </summary>
        /// <value>
        /// The current count.
        /// </value>
        public int CurrentCount => _currentCount;

        private int _maxCount;

        /// <summary>
        /// Gets or sets the maximum count.
        /// </summary>
        /// <value>
        /// The maximum count.
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">value</exception>
        public int MaxCount
        {
            get { return _maxCount; }
            set
            {
                if (value < 1) throw new ArgumentOutOfRangeException(nameof(value));
                lock (_waiters)
                {
                    _maxCount = value;

                    if (_currentCount >= _maxCount) return;
                    if (_waiters.Count < 1) return;

                    // If the max count was increased and there were waiters, let them continue

                    while (_currentCount < _maxCount &&
                           _waiters.Count > 0)
                    {
                        // ReSharper disable once PossibleNullReferenceException
                        while (!_waiters.Dequeue().TrySetResult(_releaser)) { }
                        _currentCount++;
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncSemaphore" /> class.
        /// </summary>
        /// <param name="initialCount">The initial count.</param>
        /// <exception cref="ArgumentOutOfRangeException">The count is less than one.</exception>
        public AsyncSemaphore(int initialCount = 1)
        {
            if (initialCount < 1) throw new ArgumentOutOfRangeException(nameof(initialCount));
            _maxCount = initialCount;
            _currentCount = 0;
            _releaser = new Releaser(this);
            // ReSharper disable once AssignNullToNotNullAttribute
            _releaserTask = Task.FromResult(_releaser);
        }

        /// <summary>
        /// Waits on the semaphore.
        /// </summary>
        /// <param name="token">The optional cancellation token.</param>
        /// <returns>Task.</returns>
        /// <remarks><para>This is best used with a <see langword="using"/> statement.</para></remarks>
        [NotNull]
        public Task<IDisposable> WaitAsync(CancellationToken token = default(CancellationToken))
        {
            lock (_waiters)
            {
                if (_currentCount < _maxCount)
                {
                    ++_currentCount;
                    return _releaserTask;
                }

                TaskCompletionSource<IDisposable> waiter = new TaskCompletionSource<IDisposable>();
                if (token.IsCancellationRequested)
                {
                    waiter.TrySetCanceled();
                    return waiter.Task;
                }

                // If the token gets canceled, need to cancel the TCS
                if (token.CanBeCanceled)
                    token.Register(() => waiter.TrySetCanceled());

                _waiters.Enqueue(waiter);
                return waiter.Task;
            }
        }

        /// <summary>
        /// Releases any waiters waiting on the semaphore.
        /// </summary>
        public void Release()
        {
            if (_currentCount == 0) return;

            TaskCompletionSource<IDisposable> toRelease;
            do
            {
                toRelease = null;
                lock (_waiters)
                {
                    if (_currentCount <= _maxCount &&
                        _waiters.Count > 0)
                        toRelease = _waiters.Dequeue();
                    else
                        --_currentCount;
                }
            }
            while (toRelease != null &&
                   !toRelease.TrySetResult(_releaser));
        }

        /// <summary>
        /// Waits on all the given semaphores.
        /// </summary>
        /// <param name="semaphores">The semaphores to wait on. Can contain null elements.</param>
        /// <returns></returns>
        /// <remarks><para>This is best used with a <see langword="using"/> statement.</para></remarks>
        [NotNull]
        public static Task<IDisposable> WaitAllAsync([NotNull] params AsyncSemaphore[] semaphores)
        {
            return WaitAllAsync(default(CancellationToken), semaphores);
        }

        /// <summary>
        /// Waits on all the given semaphores.
        /// </summary>
        /// <param name="token">The optional cancellation token.</param>
        /// <param name="semaphores">The semaphores to wait on. Can contain null elements.</param>
        /// <returns></returns>
        /// <remarks><para>This is best used with a <see langword="using"/> statement.</para></remarks>
        [NotNull]
        public static Task<IDisposable> WaitAllAsync(
            CancellationToken token,
            [NotNull] params AsyncSemaphore[] semaphores)
        {
            if (semaphores == null) throw new ArgumentNullException(nameof(semaphores));
            token.ThrowIfCancellationRequested();

            List<AsyncSemaphore> sems = new List<AsyncSemaphore>(semaphores.Length);
            sems.AddRange(semaphores.Where(semaphore => semaphore != null));

            if (sems.Count < 1) return AllReleaser.DefaultTask;
            // ReSharper disable once PossibleNullReferenceException
            if (sems.Count < 2) return sems[0].WaitAsync(token);

            return WaitAllInternal(token, sems);
        }

        [NotNull]
        private static async Task<IDisposable> WaitAllInternal(
            CancellationToken token,
            [NotNull] List<AsyncSemaphore> semaphores)
        {
            AllReleaser releaser = new AllReleaser(semaphores.Count);
            try
            {
                // If the same semaphore was passed multiple times, it could cause a deadlock
                HashSet<AsyncSemaphore> seen = new HashSet<AsyncSemaphore>();

                for (int i = 0; i < semaphores.Count; i++)
                {
                    AsyncSemaphore sem = semaphores[i];
                    if (sem == null || !seen.Add(sem)) continue;

                    await sem.WaitAsync(token).ConfigureAwait(false);
                    releaser.Semaphores[i] = sem;
                }
            }
            catch
            {
                releaser.Dispose();
                throw;
            }

            return releaser.IsDefault ? AllReleaser.Default : releaser;
        }

        /// <summary>
        /// Used to release a single semaphore.
        /// </summary>
        private struct Releaser : IDisposable
        {
            private readonly AsyncSemaphore _semaphore;

            /// <summary>
            /// Initializes a new instance of the <see cref="Releaser"/> struct.
            /// </summary>
            /// <param name="semaphore">The semaphore.</param>
            public Releaser(AsyncSemaphore semaphore)
            {
                _semaphore = semaphore;
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                _semaphore?.Release();
            }
        }

        /// <summary>
        /// Used to release multiple semaphores that have been waited on by WaitAllAsync.
        /// </summary>
        private struct AllReleaser : IDisposable
        {
            /// <summary>
            /// A default instance of the releaser that does nothing.
            /// </summary>
            public static readonly IDisposable Default = new AllReleaser();

            /// <summary>
            /// A task with the value <see cref="Default"/>.
            /// </summary>
            [NotNull]
            // ReSharper disable once AssignNullToNotNullAttribute
            public static readonly Task<IDisposable> DefaultTask = Task.FromResult(Default);

            private AsyncSemaphore[] _semaphores;

            /// <summary>
            /// Initializes a new instance of the <see cref="AllReleaser"/> struct.
            /// </summary>
            /// <param name="count">The total possible number of semaphores.</param>
            public AllReleaser(int count)
            {
                _semaphores = new AsyncSemaphore[count];
            }

            /// <summary>
            /// Gets a value indicating whether this instance is default.
            /// </summary>
            /// <value>
            /// <see langword="true" /> if this instance is default; otherwise, <see langword="false" />.
            /// </value>
            public bool IsDefault => _semaphores == null;

            /// <summary>
            /// Gets the semaphores to release.
            /// </summary>
            [NotNull]
            public AsyncSemaphore[] Semaphores
            {
                get
                {
                    Debug.Assert(_semaphores != null);
                    return _semaphores;
                }
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                AsyncSemaphore[] semaphores = Interlocked.Exchange(ref _semaphores, null);
                if (semaphores == null) return;

                List<Exception> ex = null;

                for (int i = 0; i < semaphores.Length; i++)
                    try
                    {
                        Interlocked.Exchange(ref semaphores[i], null)?.Release();
                    }
                    catch (Exception e)
                    {
                        if (ex == null) ex = new List<Exception>();
                        ex.Add(e);
                    }

                if (ex == null || ex.Count < 1)
                    return;

                // ReSharper disable once AssignNullToNotNullAttribute
                if (ex.Count < 2) ex[0].ReThrow();
                throw new AggregateException(ex);
            }
        }
    }
}