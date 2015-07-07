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
using System.Threading;
using System.Threading.Tasks;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Threading
{
    /// <summary>
    /// Asynchronous lock making it easy to lock a region of code using the async/await syntax.
    /// </summary>
    /// <remarks>
    /// http://blogs.msdn.com/b/pfxteam/archive/2012/02/12/10266988.aspx
    /// </remarks>
    [PublicAPI]
    public class AsyncReaderWriterLock
    {
        [NotNull]
        private readonly Task<IDisposable> _readerReleaser;

        [NotNull]
        private readonly Queue<TaskCompletionSource<IDisposable>> _waitingWriters =
            new Queue<TaskCompletionSource<IDisposable>>();

        [NotNull]
        private readonly Task<IDisposable> _writerReleaser;

        private int _readersWaiting;
        private int _status;

        [NotNull]
        private TaskCompletionSource<IDisposable> _waitingReader = new TaskCompletionSource<IDisposable>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncReaderWriterLock" /> class.
        /// </summary>
        public AsyncReaderWriterLock()
        {
            // ReSharper disable AssignNullToNotNullAttribute
            _readerReleaser = Task.FromResult((IDisposable)new Releaser(this, false));
            _writerReleaser = Task.FromResult((IDisposable)new Releaser(this, true));
            // ReSharper restore AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Creates a read lock region that can be completed by disposing the returned disposable.
        /// </summary>
        /// <param name="token">The optional cancellation token.</param>
        /// <returns>Task{IDisposable}.</returns>
        /// <remarks><para>This is best used with a <see langword="using"/> statement.</para></remarks>
        [NotNull]
        public Task<IDisposable> ReaderLockAsync(CancellationToken token)
        {
            lock (_waitingWriters)
            {
                if (_status >= 0 &&
                    _waitingWriters.Count == 0)
                {
                    ++_status;
                    return _readerReleaser;
                }
                ++_readersWaiting;

                // ReSharper disable PossibleNullReferenceException
                return _waitingReader.Task.ContinueWith(t => t.Result, token);
                // ReSharper restore PossibleNullReferenceException
            }
        }

        /// <summary>
        /// Creates a write lock region that can be completed by disposing the returned disposable.
        /// </summary>
        /// <param name="token">The optional cancellation token.</param>
        /// <returns>Task{IDisposable}.</returns>
        /// <remarks><para>This is best used with a <see langword="using"/> statement.</para></remarks>
        [NotNull]
        public Task<IDisposable> WriterLockAsync(CancellationToken token)
        {
            lock (_waitingWriters)
            {
                if (_status == 0)
                {
                    _status = -1;
                    return _writerReleaser;
                }
                TaskCompletionSource<IDisposable> waiter = new TaskCompletionSource<IDisposable>(token);
                _waitingWriters.Enqueue(waiter);

                // ReSharper disable once AssignNullToNotNullAttribute
                return waiter.Task;
            }
        }

        private void ReaderRelease()
        {
            TaskCompletionSource<IDisposable> toWake = null;

            lock (_waitingWriters)
            {
                --_status;
                if (_status == 0 &&
                    _waitingWriters.Count > 0)
                {
                    _status = -1;
                    toWake = _waitingWriters.Dequeue();
                }
            }

            if (toWake != null)
                toWake.SetResult(new Releaser(this, true));
        }

        private void WriterRelease()
        {
            TaskCompletionSource<IDisposable> toWake = null;
            bool toWakeIsWriter = false;

            lock (_waitingWriters)
            {
                if (_waitingWriters.Count > 0)
                {
                    toWake = _waitingWriters.Dequeue();
                    toWakeIsWriter = true;
                }
                else if (_readersWaiting > 0)
                {
                    toWake = _waitingReader;
                    _status = _readersWaiting;
                    _readersWaiting = 0;
                    _waitingReader = new TaskCompletionSource<IDisposable>();
                }
                else _status = 0;
            }

            if (toWake != null)
                toWake.SetResult(new Releaser(this, toWakeIsWriter));
        }

        #region Nested type: Releaser
        /// <summary>
        /// Releaser struct, used as disposable to allow releasing of lock on disposal.
        /// </summary>
        private struct Releaser : IDisposable
        {
            [NotNull]
            private readonly AsyncReaderWriterLock _toRelease;

            private readonly bool _writer;

            internal Releaser([NotNull] AsyncReaderWriterLock toRelease, bool writer)
            {
                _toRelease = toRelease;
                _writer = writer;
            }

            #region IDisposable Members
            public void Dispose()
            {
                if (_writer) _toRelease.WriterRelease();
                else _toRelease.ReaderRelease();
            }
            #endregion
        }
        #endregion
    }
}