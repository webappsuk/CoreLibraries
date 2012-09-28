#region © Copyright Web Applications (UK) Ltd, 2012.  All rights reserved.
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
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Threading
{
    /// <summary>
    /// Asynchronous lock making it easy to lock a region of code using the async/await syntax.
    /// </summary>
    /// <remarks>
    /// http://blogs.msdn.com/b/pfxteam/archive/2012/02/12/10266988.aspx
    /// </remarks>
    public class AsyncLock
    {
        [NotNull] private readonly Task<IDisposable> _releaser;
        [NotNull] private readonly AsyncSemaphore _semaphore;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncLock" /> class.
        /// </summary>
        public AsyncLock()
        {
            _semaphore = new AsyncSemaphore();
            _releaser = Task.FromResult((IDisposable) new Releaser(this));
        }

        /// <summary>
        /// Creates a lock region that can be completed by disposing the returned disposable.
        /// </summary>
        /// <param name="token">The optional cancellation token.</param>
        /// <returns>Task{IDisposable}.</returns>
        /// <remarks><para>This is best used with a <see langword="using"/> statement.</para></remarks>
        [NotNull]
        public Task<IDisposable> LockAsync(CancellationToken token = default(CancellationToken))
        {
            Task wait = _semaphore.WaitAsync(token);
            return wait.IsCompleted
                       ? _releaser
                       : wait.ContinueWith((_, state) => (IDisposable) new Releaser((AsyncLock) state),
                                           this, token,
                                           TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }

        #region Nested type: Releaser
        /// <summary>
        /// Releaser struct, used as disposable to allow releasing of lock on disposal.
        /// </summary>
        private struct Releaser : IDisposable
        {
            private readonly AsyncLock _toRelease;

            internal Releaser(AsyncLock toRelease)
            {
                _toRelease = toRelease;
            }

            #region IDisposable Members
            public void Dispose()
            {
                if (_toRelease != null)
                    _toRelease._semaphore.Release();
            }
            #endregion
        }
        #endregion
    }
}