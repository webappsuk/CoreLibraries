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
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Threading
{
    /// <summary>
    /// Allows cancellation of a task.
    /// </summary>
    [PublicAPI]
    public class CancelableTask : ICancelableTask
    {
        /// <summary>
        /// The task
        /// </summary>
        [NotNull]
        private readonly Task _task;

        /// <summary>
        /// The cancelable token source for canceling the task,
        /// </summary>
        [CanBeNull]
        private ICancelableTokenSource _cts;

        /// <summary>
        /// Gets the underlaying task that can be cancelled.
        /// </summary>
        public Task Task
        {
            get { return _task; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelableTask{T}"/> class.
        /// </summary>
        /// <param name="func">The function.</param>
        /// <exception cref="System.InvalidOperationException">
        /// The task returned by func was null
        /// or
        /// The task returned by func is not running
        /// </exception>
        public CancelableTask([NotNull] Action<CancellationToken> func)
        {
            Contract.Requires(func != null);

            _cts = new CancelableTokenSource();
            Task task = Task.Run(() => func(_cts.Token), _cts.Token);
            Contract.Assert(task != null);
            Contract.Assert(task.Status != TaskStatus.Created);
            _task = task;

            task.ContinueWith(
                t =>
                {
                    ICancelableTokenSource cts = Interlocked.Exchange(ref _cts, null);
                    if (cts != null)
                        cts.Dispose();
                });
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelableTask{T}"/> class.
        /// </summary>
        /// <param name="func">The function.</param>
        public CancelableTask([NotNull] Func<CancellationToken, Task> func)
        {
            Contract.Requires(func != null);

            _cts = new CancelableTokenSource();
            Task task = func(_cts.Token);
            if (task == null)
            {
                _cts.Dispose();
                _cts = null;
                throw new InvalidOperationException("The task returned by func was null");
            }
            if (task.Status == TaskStatus.Created)
            {
                _cts.Dispose();
                _cts = null;
                throw new InvalidOperationException("The task returned by func is not running");
            }
            _task = task;

            task.ContinueWith(
                t =>
                {
                    ICancelableTokenSource cts = Interlocked.Exchange(ref _cts, null);
                    if (cts != null)
                        cts.Dispose();
                });
        }

        /// <summary>
        /// Cancels the task.
        /// </summary>
        public void Cancel()
        {
            ICancelableTokenSource cts = _cts;
            if (cts != null)
                cts.Cancel();
        }

        /// <summary>
        /// Gets an awaiter used to await this <see cref="CancelableTask"/>.
        /// </summary>
        [PublicAPI]
        public TaskAwaiter GetAwaiter()
        {
            return _task.GetAwaiter();
        }

        /// <summary>
        /// Gets an awaiter used to await this <see cref="ICancelableTask"/>.
        /// </summary>
        INotifyCompletion ICancelableTask.GetAwaiter()
        {
            return _task.GetAwaiter();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            ICancelableTokenSource cts = Interlocked.Exchange(ref _cts, null);
            if (cts != null)
            {
                cts.Cancel();
                cts.Dispose();
            }
            _task.Dispose();
        }
    }

    /// <summary>
    /// Allows cancellation of a task.
    /// </summary>
    /// <typeparam name="T">The type the task returns.</typeparam>
    [PublicAPI]
    public class CancelableTask<T> : ICancelableTask
    {
        /// <summary>
        /// The task
        /// </summary>
        [NotNull]
        private readonly Task<T> _task;

        /// <summary>
        /// The cancelable token source for canceling the task,
        /// </summary>
        [CanBeNull]
        private ICancelableTokenSource _cts;

        /// <summary>
        /// Gets the underlaying task that can be cancelled.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public Task<T> Task
        {
            get { return _task; }
        }

        /// <summary>
        /// Gets the underlaying task that can be cancelled.
        /// </summary>
        Task ICancelableTask.Task
        {
            get { return _task; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelableTask{T}"/> class.
        /// </summary>
        /// <param name="func">The function.</param>
        /// <exception cref="System.InvalidOperationException">
        /// The task returned by func was null
        /// or
        /// The task returned by func is not running
        /// </exception>
        public CancelableTask([NotNull] Func<CancellationToken, T> func)
        {
            Contract.Requires(func != null);

            _cts = new CancelableTokenSource();
            Task<T> task = System.Threading.Tasks.Task.Run(() => func(_cts.Token), _cts.Token);
            Contract.Assert(task != null);
            Contract.Assert(task.Status != TaskStatus.Created);
            _task = task;

            task.ContinueWith(
                t =>
                {
                    ICancelableTokenSource cts = Interlocked.Exchange(ref _cts, null);
                    if (cts != null)
                        cts.Dispose();
                });
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelableTask{T}"/> class.
        /// </summary>
        /// <param name="func">The function.</param>
        public CancelableTask([NotNull] Func<CancellationToken, Task<T>> func)
        {
            Contract.Requires(func != null);

            _cts = new CancelableTokenSource();
            Task<T> task = func(_cts.Token);
            if (task == null)
            {
                _cts.Dispose();
                _cts = null;
                throw new InvalidOperationException("The task returned by func was null");
            }
            if (task.Status == TaskStatus.Created)
            {
                _cts.Dispose();
                _cts = null;
                throw new InvalidOperationException("The task returned by func is not running");
            }
            _task = task;

            task.ContinueWith(
                t =>
                {
                    ICancelableTokenSource cts = Interlocked.Exchange(ref _cts, null);
                    if (cts != null)
                        cts.Dispose();
                });
        }

        /// <summary>
        /// Cancels the task.
        /// </summary>
        [PublicAPI]
        public void Cancel()
        {
            ICancelableTokenSource cts = _cts;
            if (cts != null)
                cts.Cancel();
        }

        /// <summary>
        /// Gets an awaiter used to await this <see cref="CancelableTask{T}"/>.
        /// </summary>
        [PublicAPI]
        public TaskAwaiter<T> GetAwaiter()
        {
            return _task.GetAwaiter();
        }

        /// <summary>
        /// Gets an awaiter used to await this <see cref="ICancelableTask"/>.
        /// </summary>
        INotifyCompletion ICancelableTask.GetAwaiter()
        {
            return _task.GetAwaiter();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            ICancelableTokenSource cts = Interlocked.Exchange(ref _cts, null);
            if (cts != null)
            {
                cts.Cancel();
                cts.Dispose();
            }
            _task.Dispose();
        }
    }
}