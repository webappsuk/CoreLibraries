#region � Copyright Web Applications (UK) Ltd, 2012.  All rights reserved.
// Copyright (c) 2012, Web Applications UK Ltd
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
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Threading
{
    /// <summary>
    /// Asynchronous semaphore makes it easy to synchronize/throttle tasks so that no more than a fixed
    /// number can enter a critical region at the same time
    /// </summary>
    /// <remarks>
    /// http://blogs.msdn.com/b/pfxteam/archive/2012/02/12/10266983.aspx
    /// </remarks>
    public class AsyncSemaphore
    {
        [NotNull] private static readonly Task _completed = Task.FromResult(true);

        [NotNull] private readonly Queue<TaskCompletionSource<bool>> _waiters = new Queue<TaskCompletionSource<bool>>();
        private int _currentCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncSemaphore" /> class.
        /// </summary>
        /// <param name="initialCount">The initial count.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">The count is less than one.</exception>
        public AsyncSemaphore(int initialCount = 1)
        {
            if (initialCount < 1) throw new ArgumentOutOfRangeException("initialCount");
            _currentCount = initialCount;
        }

        /// <summary>
        /// Waits on the semaphire.
        /// </summary>
        /// <param name="token">The optional cancellation token.</param>
        /// <returns>Task.</returns>
        /// <remarks><para>This is best used with a <see langword="using"/> statement.</para></remarks>
        [NotNull]
        public Task WaitAsync(CancellationToken token = default(CancellationToken))
        {
            lock (_waiters)
            {
                if (_currentCount > 0)
                {
                    --_currentCount;
                    return _completed;
                }

                TaskCompletionSource<bool> waiter = new TaskCompletionSource<bool>(token);
                _waiters.Enqueue(waiter);
                return waiter.Task;
            }
        }

        /// <summary>
        /// Releases any waiters waiting on the semaphore.
        /// </summary>
        public void Release()
        {
            TaskCompletionSource<bool> toRelease = null;
            lock (_waiters)
            {
                if (_waiters.Count > 0)
                    toRelease = _waiters.Dequeue();
                else
                    ++_currentCount;
            }
            if (toRelease != null)
                toRelease.SetResult(true);
        }
    }
}