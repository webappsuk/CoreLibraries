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
        /// The completed result.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly CancelableTask Completed = new CancelableTask(TaskResult.Completed, null);

        /// <summary>
        /// A cancelable task that returns a <see langword="true"/>.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly CancelableTask<bool> True = new CancelableTask<bool>(TaskResult.True, null);

        /// <summary>
        /// A cancelable task that returns a <see langword="false"/>.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly CancelableTask<bool> False = new CancelableTask<bool>(TaskResult.False, null);

        /// <summary>
        /// A cancelable task that returns a <c>0</c>.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly CancelableTask<int> Zero = new CancelableTask<int>(TaskResult.Zero, null);

        /// <summary>
        /// A cancelable task that returns <see cref="System.Int32.MinValue"/>.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly CancelableTask<int> MinInt = new CancelableTask<int>(TaskResult.MinInt, null);

        /// <summary>
        /// A cancelable task that returns <see cref="System.Int32.MaxValue"/>.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly CancelableTask<int> MaxInt = new CancelableTask<int>(TaskResult.MaxInt, null);

        /// <summary>
        /// The cancelled task.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly CancelableTask Cancelled = new CancelableTask(TaskResult.Cancelled, null);

        /// <summary>
        /// Creates a <see cref="CancelableTask{TResult}" /> that's completed successfully with the specified result.
        /// </summary>
        /// <typeparam name="TResult">The type of the result returned by the task.</typeparam>
        /// <param name="result">The result to store into the completed task.</param>
        /// <returns>
        /// The successfully completed task.
        /// </returns>
        [PublicAPI]
        [NotNull]
        public static CancelableTask<TResult> FromResult<TResult>(TResult result)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            return new CancelableTask<TResult>(Task.FromResult(result), new CancelableTokenSource());
        }

        /// <summary>
        /// Creates a <see cref="CancelableTask" /> that's completed exceptionally with the specified exception.
        /// </summary>
        /// <param name="exception">The exception with which to complete the task.</param>
        /// <returns>
        /// The faulted task.
        /// </returns>
        [PublicAPI]
        [NotNull]
        public static CancelableTask FromException(Exception exception)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            return new CancelableTask(TaskResult.FromException(exception), null);
        }

        /// <summary>
        /// Creates a <see cref="T:Task{TResult}" /> that's completed exceptionally with the specified exception.
        /// </summary>
        /// <typeparam name="TResult">The type of the result returned by the task.</typeparam>
        /// <param name="exception">The exception with which to complete the task.</param>
        /// <returns>
        /// The faulted task.
        /// </returns>
        [PublicAPI]
        [NotNull]
        public static CancelableTask<TResult> FromException<TResult>(Exception exception)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            return new CancelableTask<TResult>(
                TaskResult<TResult>.FromException(exception),
                new CancelableTokenSource());
        }

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
        /// Gets whether this <see cref="CancelableTask"/> instance has completed execution due to being canceled.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the task has completed due to being canceled; otherwise <see langword="false" />.
        /// </value>
        public bool IsCanceled
        {
            get { return _task.IsCanceled; }
        }

        /// <summary>
        /// Gets whether this <see cref="CancelableTask"/> has completed.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the task has completed; otherwise <see langword="false" />.
        /// </value>
        public bool IsCompleted
        {
            get { return _task.IsCompleted; }
        }

        /// <summary>
        /// Gets whether the <see cref="CancelableTask"/> completed due to an unhandled exception.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the task has thrown an unhandled exception; otherwise <see langword="false" />.
        /// </value>
        public bool IsFaulted
        {
            get { return _task.IsFaulted; }
        }

        /// <summary>
        /// Gets the <see cref="TaskStatus"/> of this task.
        /// </summary>
        /// <value>
        /// The current <see cref="TaskStatus"/> of this task instance.
        /// </value>
        public TaskStatus Status
        {
            get { return _task.Status; }
        }

        /// <summary>
        /// Gets the <see cref="Exception"/> that caused the <see cref="CancelableTask"/> to end prematurely. 
        /// If the <see cref="CancelableTask"/> completed successfully or has not yet thrown any exceptions, this will return <see langword="null"/>.
        /// </summary>
        /// <value>
        /// The <see cref="Exception"/> that caused the <see cref="CancelableTask"/> to end prematurely.
        /// </value>
        public Exception Exception
        {
            get { return _task.Exception.Unwrap(); }
        }

        /// <summary>
        /// Queues the specified work to run on the ThreadPool and returns a task handle for that work.
        /// </summary>
        /// <param name="action">The work to execute asynchronously.</param>
        /// <param name="token">A cancellation token that should be used to cancel the work.</param>
        /// <returns>A <see cref="CancelableTask"/> that represents the work queued to execute in the ThreadPool.</returns>
        [PublicAPI]
        [NotNull]
        public static CancelableTask Run(
            [NotNull] Action<CancellationToken> action,
            CancellationToken token = default(CancellationToken))
        {
            return new CancelableTask(action, token, TaskCreationOptions.None, true);
        }

        /// <summary>
        /// Queues the specified work to run on the ThreadPool and returns a task handle for that work.
        /// </summary>
        /// <param name="action">The work to execute asynchronously.</param>
        /// <param name="token">A cancellation token that should be used to cancel the work.</param>
        /// <returns>A <see cref="CancelableTask"/> that represents the work queued to execute in the ThreadPool.</returns>
        [PublicAPI]
        [NotNull]
        public static CancelableTask Run(
            [NotNull] Func<CancellationToken, Task> action,
            CancellationToken token = default(CancellationToken))
        {
            return new CancelableTask(action, token, TaskCreationOptions.None, true);
        }

        /// <summary>
        /// Queues the specified work to run on the ThreadPool and returns a task handle for that work.
        /// </summary>
        /// <param name="function">The work to execute asynchronously.</param>
        /// <param name="token">A cancellation token that should be used to cancel the work.</param>
        /// <returns>A <see cref="CancelableTask{TResult}"/> that represents the work queued to execute in the ThreadPool.</returns>
        [PublicAPI]
        [NotNull]
        public static CancelableTask<TResult> Run<TResult>(
            [NotNull] Func<CancellationToken, TResult> function,
            CancellationToken token = default(CancellationToken))
        {
            return new CancelableTask<TResult>(function, token, TaskCreationOptions.None, true);
        }

        /// <summary>
        /// Queues the specified work to run on the ThreadPool and returns a task handle for that work.
        /// </summary>
        /// <param name="function">The work to execute asynchronously.</param>
        /// <param name="token">A cancellation token that should be used to cancel the work.</param>
        /// <returns>A <see cref="CancelableTask{TResult}"/> that represents the work queued to execute in the ThreadPool.</returns>
        [PublicAPI]
        [NotNull]
        public static CancelableTask<TResult> Run<TResult>(
            [NotNull] Func<CancellationToken, Task<TResult>> function,
            CancellationToken token = default(CancellationToken))
        {
            return new CancelableTask<TResult>(function, token, TaskCreationOptions.None, true);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelableTask"/> class.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="cts">The cancelable token source.</param>
        internal CancelableTask([NotNull] Task task, [CanBeNull] ICancelableTokenSource cts)
        {
            _task = task;
            _cts = cts;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelableTask"/> class.
        /// </summary>
        /// <param name="action">The delegate that represents the code to execute in the task.</param>
        /// <param name="token">The <see cref="CancellationToken"/> that the new task will observe.</param>
        /// <param name="creationOptions">The <see cref="TaskCreationOptions"/> used to customize the task's behavior.</param>
        /// <param name="run">if set to <see langword="true" /> the task will be started.</param>
        private CancelableTask(
            [NotNull] Action<CancellationToken> action,
            CancellationToken token,
            TaskCreationOptions creationOptions,
            bool run)
        {
            Contract.Requires(action != null);

            _cts = token.ToCancelable();
            token = _cts.Token;

            Action wrapped = () =>
            {
                action(token);

                ICancelableTokenSource cts = Interlocked.Exchange(ref _cts, null);
                if (cts != null)
                    cts.Dispose();
            };

            Task task = run
                ? Task.Run(wrapped, token)
                : new Task(wrapped, token, creationOptions);
            Contract.Assert(task != null);

            _task = task;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelableTask"/> class.
        /// </summary>
        /// <param name="action">The delegate that represents the code to execute in the task.</param>
        /// <param name="token">The <see cref="CancellationToken"/> that the new task will observe.</param>
        /// <param name="creationOptions">The <see cref="TaskCreationOptions"/> used to customize the task's behavior.</param>
        /// <param name="run">if set to <see langword="true" /> the task will be started.</param>
        private CancelableTask(
            [NotNull] Func<CancellationToken, Task> action,
            CancellationToken token,
            TaskCreationOptions creationOptions,
            bool run)
        {
            Contract.Requires(action != null);

            _cts = token.ToCancelable();
            token = _cts.Token;

            Func<Task> wrapped = () =>
            {
                Task tmp = action(token) ??
                           TaskResult.FromException(
                               new ArgumentException(Resources.CancelableTask_CancelableTask_NullTask, "action"));
                return tmp.ContinueWith(
                    t =>
                    {
                        ICancelableTokenSource cts = Interlocked.Exchange(ref _cts, null);
                        if (cts != null)
                            cts.Dispose();
                        return t;
                    }).Unwrap();
            };

            Task task = run
                ? Task.Run(wrapped, token)
                : new Task<Task>(wrapped, token, creationOptions).Unwrap();
            Contract.Assert(task != null);

            _task = task;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelableTask{T}" /> class.
        /// </summary>
        /// <param name="action">The delegate that represents the code to execute in the task.</param>
        /// <param name="token">The <see cref="CancellationToken"/> that the new task will observe.</param>
        /// <param name="creationOptions">The <see cref="TaskCreationOptions"/> used to customize the task's behavior.</param>
        public CancelableTask(
            [NotNull] Action<CancellationToken> action,
            CancellationToken token = default(CancellationToken),
            TaskCreationOptions creationOptions = TaskCreationOptions.None)
            : this(action, token, creationOptions, false)
        {
            Contract.Requires(action != null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelableTask{T}" /> class.
        /// </summary>
        /// <param name="action">The delegate that represents the code to execute in the task.</param>
        /// <param name="token">The <see cref="CancellationToken"/> that the new task will observe.</param>
        /// <param name="creationOptions">The <see cref="TaskCreationOptions"/> used to customize the task's behavior.</param>
        public CancelableTask(
            [NotNull] Func<CancellationToken, Task> action,
            CancellationToken token = default(CancellationToken),
            TaskCreationOptions creationOptions = TaskCreationOptions.None)
            : this(action, token, creationOptions, false)
        {
            Contract.Requires(action != null);
        }

        /// <summary>
        /// Communicates a request for cancellation.
        /// </summary>
        public void Cancel()
        {
            ICancelableTokenSource cts = _cts;
            if (cts != null)
                cts.Cancel();
        }

        /// <summary>
        /// Schedules a cancel operation on this <see cref="CancelableTask"/> after the specified number of milliseconds.
        /// </summary>
        /// <param name="millisecondsDelay">The time span to wait before canceling this <see cref="T:System.Threading.CancellationTokenSource"/>.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CancelAfter(int millisecondsDelay)
        {
            ICancelableTokenSource cts = _cts;
            if (cts != null) cts.CancelAfter(millisecondsDelay);
        }

        /// <summary>
        /// Schedules a cancel operation on this <see cref="ICancelableTokenSource"/> after the specified time span.
        /// </summary>
        /// <param name="delay">The time span to wait before canceling this <see cref="ICancelableTokenSource"/>.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CancelAfter(TimeSpan delay)
        {
            ICancelableTokenSource cts = _cts;
            if (cts != null) cts.CancelAfter(delay);
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
        /// Configures an awaiter used to await this <see cref="CancelableTask"/>.
        /// </summary>
        /// <param name="continueOnCapturedContext"><see langword="true" /> to attempt to marshal the continuation back to the original context captured; otherwise, <see langword="false" />.</param>
        /// <returns></returns>
        [PublicAPI]
        public ConfiguredTaskAwaitable ConfigureAwait(bool continueOnCapturedContext)
        {
            return _task.ConfigureAwait(continueOnCapturedContext);
        }

        /// <summary>
        /// Creates a continuation that executes when the target <see cref="CancelableTask"/> completes.
        /// </summary>
        /// <param name="continuationAction">An action to run when the Task completes. When run, the delegate will be passed the completed task as an argument.</param>
        /// <param name="token">The <see cref="CancellationToken"/> that will be assigned to the new continuation task.</param>
        /// <param name="options">Options for when the continuation is scheduled and how it behaves.</param>
        /// <param name="scheduler">The <see cref="TaskScheduler"/> to associate with the continuation task and to use for its execution.</param>
        /// <returns>A new continuation <see cref="CancelableTask"/>.</returns>
        [PublicAPI]
        public CancelableTask ContinueWith(
            [NotNull] Action<CancelableTask, CancellationToken> continuationAction,
            CancellationToken token = default(CancellationToken),
            TaskContinuationOptions options = TaskContinuationOptions.None,
            TaskScheduler scheduler = null)
        {
            Contract.Requires<ArgumentNullException>(continuationAction != null);

            if (scheduler == null)
                scheduler = TaskScheduler.Current;
            Contract.Assert(scheduler != null);

            ICancelableTokenSource cts = token.ToCancelable();
            Task task = _task.ContinueWith(t => continuationAction(this, cts.Token), cts.Token, options, scheduler);

            return new CancelableTask(task, cts);
        }

        /// <summary>
        /// Creates a continuation that executes when the target <see cref="CancelableTask"/> completes.
        /// </summary>
        /// <param name="continuationAction">An action to run when the Task completes. When run, the delegate will be passed the completed task as an argument.</param>
        /// <param name="token">The <see cref="CancellationToken"/> that will be assigned to the new continuation task.</param>
        /// <param name="options">Options for when the continuation is scheduled and how it behaves.</param>
        /// <param name="scheduler">The <see cref="TaskScheduler"/> to associate with the continuation task and to use for its execution.</param>
        /// <returns>A new continuation <see cref="CancelableTask"/>.</returns>
        [PublicAPI]
        public CancelableTask<TResult> ContinueWith<TResult>(
            [NotNull] Func<CancelableTask, CancellationToken, TResult> continuationAction,
            CancellationToken token = default(CancellationToken),
            TaskContinuationOptions options = TaskContinuationOptions.None,
            TaskScheduler scheduler = null)
        {
            Contract.Requires<ArgumentNullException>(continuationAction != null);

            if (scheduler == null)
                scheduler = TaskScheduler.Current;
            Contract.Assert(scheduler != null);

            ICancelableTokenSource cts = token.ToCancelable();
            Task<TResult> task = _task.ContinueWith(
                t => continuationAction(this, cts.Token),
                cts.Token,
                options,
                scheduler);

            return new CancelableTask<TResult>(task, cts);
        }

        /// <summary>
        /// Creates a continuation that executes when the target <see cref="CancelableTask"/> completes.
        /// </summary>
        /// <param name="continuationAction">An action to run when the Task completes. When run, the delegate will be passed the completed task as an argument.</param>
        /// <param name="token">The <see cref="CancellationToken"/> that will be assigned to the new continuation task.</param>
        /// <param name="options">Options for when the continuation is scheduled and how it behaves.</param>
        /// <param name="scheduler">The <see cref="TaskScheduler"/> to associate with the continuation task and to use for its execution.</param>
        /// <returns>A new continuation <see cref="CancelableTask"/>.</returns>
        [PublicAPI]
        public CancelableTask ContinueWith(
            [NotNull] Func<CancelableTask, CancellationToken, Task> continuationAction,
            CancellationToken token = default(CancellationToken),
            TaskContinuationOptions options = TaskContinuationOptions.None,
            TaskScheduler scheduler = null)
        {
            Contract.Requires<ArgumentNullException>(continuationAction != null);

            if (scheduler == null)
                scheduler = TaskScheduler.Current;
            Contract.Assert(scheduler != null);

            ICancelableTokenSource cts = token.ToCancelable();
            Task task =
                _task.ContinueWith(t => continuationAction(this, cts.Token), cts.Token, options, scheduler).Unwrap();
            Contract.Assert(task != null);

            return new CancelableTask(task, cts);
        }

        /// <summary>
        /// Creates a continuation that executes when the target <see cref="CancelableTask"/> completes.
        /// </summary>
        /// <param name="continuationAction">An action to run when the Task completes. When run, the delegate will be passed the completed task as an argument.</param>
        /// <param name="token">The <see cref="CancellationToken"/> that will be assigned to the new continuation task.</param>
        /// <param name="options">Options for when the continuation is scheduled and how it behaves.</param>
        /// <param name="scheduler">The <see cref="TaskScheduler"/> to associate with the continuation task and to use for its execution.</param>
        /// <returns>A new continuation <see cref="CancelableTask"/>.</returns>
        [PublicAPI]
        public CancelableTask<TResult> ContinueWith<TResult>(
            [NotNull] Func<CancelableTask, CancellationToken, Task<TResult>> continuationAction,
            CancellationToken token = default(CancellationToken),
            TaskContinuationOptions options = TaskContinuationOptions.None,
            TaskScheduler scheduler = null)
        {
            Contract.Requires<ArgumentNullException>(continuationAction != null);

            if (scheduler == null)
                scheduler = TaskScheduler.Current;
            Contract.Assert(scheduler != null);

            ICancelableTokenSource cts = token.ToCancelable();
            Task<TResult> task =
                _task.ContinueWith(t => continuationAction(this, cts.Token), cts.Token, options, scheduler).Unwrap();
            Contract.Assert(task != null);

            return new CancelableTask<TResult>(task, cts);
        }

        /// <summary>
        /// Waits for the <see cref="CancelableTask"/> to complete execution.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Wait(CancellationToken cancellationToken = default(CancellationToken))
        {
            _task.Wait(Timeout.Infinite, cancellationToken);
        }

        /// <summary>
        /// Waits for the <see cref="CancelableTask"/> to complete execution within a specified number of milliseconds.
        /// </summary>
        /// <param name="millisecondsTimeout">The number of milliseconds to wait, or <see cref="Timeout.Infinite"/> (-1) to wait indefinitely.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Wait(int millisecondsTimeout, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _task.Wait(millisecondsTimeout, cancellationToken);
        }

        /// <summary>
        /// Waits for the <see cref="CancelableTask"/> to complete execution within a specified time interval.
        /// </summary>
        /// <param name="timeout">A <see cref="TimeSpan"/> that represents the number of milliseconds to wait, or a <see cref="TimeSpan"/> that represents -1 milliseconds to wait indefinitely.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Wait(TimeSpan timeout, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _task.Wait((int)timeout.TotalMilliseconds, cancellationToken);
        }

        /// <summary>
        /// Starts the <see cref="CancelableTask"/>, scheduling it for execution to the specified <see cref="TaskScheduler"/>.
        /// </summary>
        /// <param name="scheduler">The <see cref="TaskScheduler"/> with which to associate and execute this task.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Start(TaskScheduler scheduler = null)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            _task.Start(scheduler ?? TaskScheduler.Current);
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
    /// <typeparam name="TResult">The type the task returns.</typeparam>
    [PublicAPI]
    public class CancelableTask<TResult> : ICancelableTask
    {
        /// <summary>
        /// A cancelable task that returns the <see langword="default"/> value for the type <typeparamref name="TResult"/>.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly CancelableTask<TResult> Default = new CancelableTask<TResult>(
            TaskResult<TResult>.Default,
            null);

        /// <summary>
        /// The cancelled task.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly CancelableTask<TResult> Cancelled =
            new CancelableTask<TResult>(TaskResult<TResult>.Cancelled, null);

        /// <summary>
        /// Creates a <see cref="CancelableTask{TResult}" /> that's completed successfully with the specified result.
        /// </summary>
        /// <typeparam name="TResult">The type of the result returned by the task.</typeparam>
        /// <param name="result">The result to store into the completed task.</param>
        /// <returns>
        /// The successfully completed task.
        /// </returns>
        [PublicAPI]
        [NotNull]
        public static CancelableTask<TResult> FromResult(TResult result)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            return new CancelableTask<TResult>(
                System.Threading.Tasks.Task.FromResult(result),
                new CancelableTokenSource());
        }

        /// <summary>
        /// Creates a <see cref="Task{TResult}" /> that's completed exceptionally with the specified exception.
        /// </summary>
        /// <param name="exception">The exception with which to complete the task.</param>
        /// <returns>
        /// The faulted task.
        /// </returns>
        [PublicAPI]
        [NotNull]
        public static CancelableTask<TResult> FromException(Exception exception)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            return new CancelableTask<TResult>(
                TaskResult<TResult>.FromException(exception),
                new CancelableTokenSource());
        }

        /// <summary>
        /// The task
        /// </summary>
        [NotNull]
        private readonly Task<TResult> _task;

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
        public Task<TResult> Task
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
        /// Gets whether this <see cref="CancelableTask"/> instance has completed execution due to being canceled.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the task has completed due to being canceled; otherwise <see langword="false" />.
        /// </value>
        public bool IsCanceled
        {
            get { return _task.IsCanceled; }
        }

        /// <summary>
        /// Gets whether this <see cref="CancelableTask"/> has completed.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the task has completed; otherwise <see langword="false" />.
        /// </value>
        public bool IsCompleted
        {
            get { return _task.IsCompleted; }
        }

        /// <summary>
        /// Gets whether the <see cref="CancelableTask"/> completed due to an unhandled exception.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the task has thrown an unhandled exception; otherwise <see langword="false" />.
        /// </value>
        public bool IsFaulted
        {
            get { return _task.IsFaulted; }
        }

        /// <summary>
        /// Gets the <see cref="TaskStatus"/> of this task.
        /// </summary>
        /// <value>
        /// The current <see cref="TaskStatus"/> of this task instance.
        /// </value>
        public TaskStatus Status
        {
            get { return _task.Status; }
        }

        /// <summary>
        /// Gets the <see cref="Exception"/> that caused the <see cref="CancelableTask"/> to end prematurely. 
        /// If the <see cref="CancelableTask"/> completed successfully or has not yet thrown any exceptions, this will return <see langword="null"/>.
        /// </summary>
        /// <value>
        /// The <see cref="Exception"/> that caused the <see cref="CancelableTask"/> to end prematurely.
        /// </value>
        public Exception Exception
        {
            get { return _task.Exception.Unwrap(); }
        }

        /// <summary>
        /// Gets the result value of this <see cref="CancelableTask{TResult}"/>
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        [PublicAPI]
        public TResult Result
        {
            get { return _task.Result; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelableTask"/> class.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="cts">The cancelable token source.</param>
        internal CancelableTask([NotNull] Task<TResult> task, [CanBeNull] ICancelableTokenSource cts)
        {
            _task = task;
            _cts = cts;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelableTask"/> class.
        /// </summary>
        /// <param name="function">The delegate that represents the code to execute in the task.</param>
        /// <param name="token">The <see cref="CancellationToken"/> that the new task will observe.</param>
        /// <param name="creationOptions">The <see cref="TaskCreationOptions"/> used to customize the task's behavior.</param>
        /// <param name="run">if set to <see langword="true" /> the task will be started.</param>
        internal CancelableTask(
            [NotNull] Func<CancellationToken, TResult> function,
            CancellationToken token,
            TaskCreationOptions creationOptions,
            bool run)
        {
            Contract.Requires(function != null);

            _cts = token.ToCancelable();
            token = _cts.Token;

            Func<TResult> wrapped = () =>
            {
                TResult result = function(token);

                ICancelableTokenSource cts = Interlocked.Exchange(ref _cts, null);
                if (cts != null)
                    cts.Dispose();

                return result;
            };

            Task<TResult> task = run
                ? System.Threading.Tasks.Task.Run(wrapped, token)
                : new Task<TResult>(wrapped, token, creationOptions);
            Contract.Assert(task != null);

            _task = task;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelableTask"/> class.
        /// </summary>
        /// <param name="function">The delegate that represents the code to execute in the task.</param>
        /// <param name="token">The <see cref="CancellationToken"/> that the new task will observe.</param>
        /// <param name="creationOptions">The <see cref="TaskCreationOptions"/> used to customize the task's behavior.</param>
        /// <param name="run">if set to <see langword="true" /> the task will be started.</param>
        internal CancelableTask(
            [NotNull] Func<CancellationToken, Task<TResult>> function,
            CancellationToken token,
            TaskCreationOptions creationOptions,
            bool run)
        {
            Contract.Requires(function != null);

            _cts = token.ToCancelable();
            token = _cts.Token;

            Func<Task<TResult>> wrapped = () =>
            {
                Task<TResult> tmp = function(token) ??
                                    TaskResult<TResult>.FromException(
                                        new ArgumentException(
                                            Resources.CancelableTask_CancelableTask_NullTask,
                                            "function"));
                return tmp.ContinueWith(
                    t =>
                    {
                        ICancelableTokenSource cts = Interlocked.Exchange(ref _cts, null);
                        if (cts != null)
                            cts.Dispose();
                        return t;
                    }).Unwrap();
            };

            Task<TResult> task = run
                ? System.Threading.Tasks.Task.Run(wrapped, token)
                : new Task<Task<TResult>>(wrapped, token, creationOptions).Unwrap();
            Contract.Assert(task != null);

            _task = task;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelableTask{T}" /> class.
        /// </summary>
        /// <param name="function">The delegate that represents the code to execute in the task.
        /// When the function has completed, the task's Result property will be set to return the result value of the function.</param>
        /// <param name="token">The <see cref="CancellationToken"/> that the new task will observe.</param>
        /// <param name="creationOptions">The <see cref="TaskCreationOptions"/> used to customize the task's behavior.</param>
        public CancelableTask(
            [NotNull] Func<CancellationToken, TResult> function,
            CancellationToken token = default(CancellationToken),
            TaskCreationOptions creationOptions = TaskCreationOptions.None)
            : this(function, token, creationOptions, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelableTask{T}" /> class.
        /// </summary>
        /// <param name="function">The delegate that represents the code to execute in the task.
        /// When the function has completed, the task's Result property will be set to return the result value of the function.</param>
        /// <param name="token">The <see cref="CancellationToken" /> that the new task will observe.</param>
        /// <param name="creationOptions">The <see cref="TaskCreationOptions" /> used to customize the task's behavior.</param>
        public CancelableTask(
            [NotNull] Func<CancellationToken, Task<TResult>> function,
            CancellationToken token = default(CancellationToken),
            TaskCreationOptions creationOptions = TaskCreationOptions.None)
            : this(function, token, creationOptions, false)
        {
            Contract.Requires(function != null);
        }

        /// <summary>
        /// Communicates a request for cancellation.
        /// </summary>
        public void Cancel()
        {
            ICancelableTokenSource cts = _cts;
            if (cts != null)
                cts.Cancel();
        }

        /// <summary>
        /// Schedules a cancel operation on this <see cref="CancelableTask"/> after the specified number of milliseconds.
        /// </summary>
        /// <param name="millisecondsDelay">The time span to wait before canceling this <see cref="T:System.Threading.CancellationTokenSource"/>.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CancelAfter(int millisecondsDelay)
        {
            ICancelableTokenSource cts = _cts;
            if (cts != null) cts.CancelAfter(millisecondsDelay);
        }

        /// <summary>
        /// Schedules a cancel operation on this <see cref="ICancelableTokenSource"/> after the specified time span.
        /// </summary>
        /// <param name="delay">The time span to wait before canceling this <see cref="ICancelableTokenSource"/>.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CancelAfter(TimeSpan delay)
        {
            ICancelableTokenSource cts = _cts;
            if (cts != null) cts.CancelAfter(delay);
        }

        /// <summary>
        /// Gets an awaiter used to await this <see cref="CancelableTask{T}"/>.
        /// </summary>
        [PublicAPI]
        public TaskAwaiter<TResult> GetAwaiter()
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
        /// Configures an awaiter used to await this <see cref="CancelableTask"/>.
        /// </summary>
        /// <param name="continueOnCapturedContext"><see langword="true" /> to attempt to marshal the continuation back to the original context captured; otherwise, <see langword="false" />.</param>
        /// <returns></returns>
        [PublicAPI]
        public ConfiguredTaskAwaitable<TResult> ConfigureAwait(bool continueOnCapturedContext)
        {
            return _task.ConfigureAwait(continueOnCapturedContext);
        }

        /// <summary>
        /// Creates a continuation that executes when the target <see cref="CancelableTask"/> completes.
        /// </summary>
        /// <param name="continuationAction">An action to run when the Task completes. When run, the delegate will be passed the completed task as an argument.</param>
        /// <param name="token">The <see cref="CancellationToken"/> that will be assigned to the new continuation task.</param>
        /// <param name="options">Options for when the continuation is scheduled and how it behaves.</param>
        /// <param name="scheduler">The <see cref="TaskScheduler"/> to associate with the continuation task and to use for its execution.</param>
        /// <returns>A new continuation <see cref="CancelableTask"/>.</returns>
        [PublicAPI]
        public CancelableTask ContinueWith(
            [NotNull] Action<CancelableTask<TResult>, CancellationToken> continuationAction,
            CancellationToken token = default(CancellationToken),
            TaskContinuationOptions options = TaskContinuationOptions.None,
            TaskScheduler scheduler = null)
        {
            Contract.Requires<ArgumentNullException>(continuationAction != null);

            if (scheduler == null)
                scheduler = TaskScheduler.Current;
            Contract.Assert(scheduler != null);

            ICancelableTokenSource cts = token.ToCancelable();
            Task task = _task.ContinueWith(t => continuationAction(this, cts.Token), cts.Token, options, scheduler);

            return new CancelableTask(task, cts);
        }

        /// <summary>
        /// Creates a continuation that executes when the target <see cref="CancelableTask"/> completes.
        /// </summary>
        /// <param name="continuationAction">An action to run when the Task completes. When run, the delegate will be passed the completed task as an argument.</param>
        /// <param name="token">The <see cref="CancellationToken"/> that will be assigned to the new continuation task.</param>
        /// <param name="options">Options for when the continuation is scheduled and how it behaves.</param>
        /// <param name="scheduler">The <see cref="TaskScheduler"/> to associate with the continuation task and to use for its execution.</param>
        /// <returns>A new continuation <see cref="CancelableTask"/>.</returns>
        [PublicAPI]
        public CancelableTask<TNewResult> ContinueWith<TNewResult>(
            [NotNull] Func<CancelableTask<TResult>, CancellationToken, TNewResult> continuationAction,
            CancellationToken token = default(CancellationToken),
            TaskContinuationOptions options = TaskContinuationOptions.None,
            TaskScheduler scheduler = null)
        {
            Contract.Requires<ArgumentNullException>(continuationAction != null);

            if (scheduler == null)
                scheduler = TaskScheduler.Current;
            Contract.Assert(scheduler != null);

            ICancelableTokenSource cts = token.ToCancelable();
            Task<TNewResult> task = _task.ContinueWith(
                t => continuationAction(this, cts.Token),
                cts.Token,
                options,
                scheduler);

            return new CancelableTask<TNewResult>(task, cts);
        }

        /// <summary>
        /// Creates a continuation that executes when the target <see cref="CancelableTask"/> completes.
        /// </summary>
        /// <param name="continuationAction">An action to run when the Task completes. When run, the delegate will be passed the completed task as an argument.</param>
        /// <param name="token">The <see cref="CancellationToken"/> that will be assigned to the new continuation task.</param>
        /// <param name="options">Options for when the continuation is scheduled and how it behaves.</param>
        /// <param name="scheduler">The <see cref="TaskScheduler"/> to associate with the continuation task and to use for its execution.</param>
        /// <returns>A new continuation <see cref="CancelableTask"/>.</returns>
        [PublicAPI]
        public CancelableTask ContinueWith(
            [NotNull] Func<CancelableTask<TResult>, CancellationToken, Task> continuationAction,
            CancellationToken token = default(CancellationToken),
            TaskContinuationOptions options = TaskContinuationOptions.None,
            TaskScheduler scheduler = null)
        {
            Contract.Requires<ArgumentNullException>(continuationAction != null);

            if (scheduler == null)
                scheduler = TaskScheduler.Current;
            Contract.Assert(scheduler != null);

            ICancelableTokenSource cts = token.ToCancelable();
            Task task =
                _task.ContinueWith(t => continuationAction(this, cts.Token), cts.Token, options, scheduler).Unwrap();
            Contract.Assert(task != null);

            return new CancelableTask(task, cts);
        }

        /// <summary>
        /// Creates a continuation that executes when the target <see cref="CancelableTask"/> completes.
        /// </summary>
        /// <param name="continuationAction">An action to run when the Task completes. When run, the delegate will be passed the completed task as an argument.</param>
        /// <param name="token">The <see cref="CancellationToken"/> that will be assigned to the new continuation task.</param>
        /// <param name="options">Options for when the continuation is scheduled and how it behaves.</param>
        /// <param name="scheduler">The <see cref="TaskScheduler"/> to associate with the continuation task and to use for its execution.</param>
        /// <returns>A new continuation <see cref="CancelableTask"/>.</returns>
        [PublicAPI]
        public CancelableTask<TNewResult> ContinueWith<TNewResult>(
            [NotNull] Func<CancelableTask<TResult>, CancellationToken, Task<TNewResult>> continuationAction,
            CancellationToken token = default(CancellationToken),
            TaskContinuationOptions options = TaskContinuationOptions.None,
            TaskScheduler scheduler = null)
        {
            Contract.Requires<ArgumentNullException>(continuationAction != null);

            if (scheduler == null)
                scheduler = TaskScheduler.Current;
            Contract.Assert(scheduler != null);

            ICancelableTokenSource cts = token.ToCancelable();
            Task<TNewResult> task =
                _task.ContinueWith(t => continuationAction(this, cts.Token), cts.Token, options, scheduler).Unwrap();
            Contract.Assert(task != null);

            return new CancelableTask<TNewResult>(task, cts);
        }

        /// <summary>
        /// Waits for the <see cref="CancelableTask"/> to complete execution.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Wait(CancellationToken cancellationToken = default(CancellationToken))
        {
            _task.Wait(Timeout.Infinite, cancellationToken);
        }

        /// <summary>
        /// Waits for the <see cref="CancelableTask"/> to complete execution within a specified number of milliseconds.
        /// </summary>
        /// <param name="millisecondsTimeout">The number of milliseconds to wait, or <see cref="Timeout.Infinite"/> (-1) to wait indefinitely.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Wait(int millisecondsTimeout, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _task.Wait(millisecondsTimeout, cancellationToken);
        }

        /// <summary>
        /// Waits for the <see cref="CancelableTask"/> to complete execution within a specified time interval.
        /// </summary>
        /// <param name="timeout">A <see cref="TimeSpan"/> that represents the number of milliseconds to wait, or a <see cref="TimeSpan"/> that represents -1 milliseconds to wait indefinitely.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Wait(TimeSpan timeout, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _task.Wait((int)timeout.TotalMilliseconds, cancellationToken);
        }

        /// <summary>
        /// Starts the <see cref="CancelableTask"/>, scheduling it for execution to the specified <see cref="TaskScheduler"/>.
        /// </summary>
        /// <param name="scheduler">The <see cref="TaskScheduler"/> with which to associate and execute this task.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Start(TaskScheduler scheduler = null)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            _task.Start(scheduler ?? TaskScheduler.Current);
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