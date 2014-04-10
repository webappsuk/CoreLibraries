#region © Copyright Web Applications (UK) Ltd, 2013.  All rights reserved.
// Copyright (c) 2013, Web Applications UK Ltd
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
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WebApplications.Utilities.Threading;

namespace WebApplications.Utilities
{
    /// <summary>
    ///   Extension methods for <see cref="Task"/> and <see cref="TaskCompletionSource&lt;TResult&gt;"/>.
    /// </summary>
    public static partial class TaskExtensions
    {
        /// <summary>
        ///   Sets the result of a <see cref="TaskCompletionSource&lt;TResult&gt;.Task"/> via a
        ///   different <see cref="Task"/>.
        /// </summary>
        /// <param name="tcs">The task completion source.</param>
        /// <param name="task">
        ///   The task used to set the result of the <paramref name="tcs"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   <para><paramref name="tcs"/> was a <see langword="null"/>.</para>
        ///   <para>-or-</para>
        ///   <para><paramref name="task"/> was a <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///   <para>The <see cref="System.Threading.Tasks.TaskStatus"/> was in a faulted state and
        ///   was completed due to an unhandled exception. The exception couldn't be retrieved.</para>
        ///   <para>-or-</para>
        ///   <para>The task was not completed.</para>
        /// </exception>
        [UsedImplicitly]
        public static void SetFromTask(
            [NotNull] this TaskCompletionSource<bool> tcs,
            [NotNull] Task task)
        {
            if (tcs == null) throw new ArgumentNullException("tcs");
            if (task == null) throw new ArgumentNullException("task");

            switch (task.Status)
            {
                case TaskStatus.RanToCompletion:
                    tcs.SetResult(true);
                    break;
                case TaskStatus.Faulted:
                    if ((task.Exception == null) || (task.Exception.InnerExceptions == null))
                        tcs.SetException(
                            new InvalidOperationException(
                                Resources.TaskExtensions_SetFromTask_TaskStateFaulted));
                    else
                        tcs.SetException(task.Exception.InnerExceptions);
                    break;
                case TaskStatus.Canceled:
                    tcs.SetCanceled();
                    break;
                default:
                    throw new InvalidOperationException(Resources.TaskExtensions_SetFromTask_TaskWasNotCompleted);
            }
        }

        /// <summary>
        ///   Sets the result of a <see cref="TaskCompletionSource&lt;TResult&gt;.Task"/> with a
        ///   <see cref="Task&lt;TResult&gt;"/>.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="tcs">The task completion source.</param>
        /// <param name="task">
        ///   The task used to set the result of the <paramref name="tcs"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   <para><paramref name="tcs"/> was a <see langword="null"/>.</para>
        ///   <para>-or-</para>
        ///   <para><paramref name="task"/> was a <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///   <para>The <see cref="System.Threading.Tasks.TaskStatus"/> was in a faulted state
        ///   and was completed due to an unhandled exception. The exception couldn't be retrieved.</para>
        ///   <para>-or-</para>
        ///   <para>The task was not completed.</para>
        /// </exception>
        [UsedImplicitly]
        public static void SetFromTask<TResult>(
            [NotNull] this TaskCompletionSource<TResult> tcs,
            [NotNull] Task<TResult> task)
        {
            if (tcs == null) throw new ArgumentNullException("tcs");
            if (task == null) throw new ArgumentNullException("task");

            switch (task.Status)
            {
                case TaskStatus.RanToCompletion:
                    tcs.SetResult(task.Result);
                    break;
                case TaskStatus.Faulted:
                    if ((task.Exception == null) || (task.Exception.InnerExceptions == null))
                        tcs.SetException(
                            new InvalidOperationException(
                                Resources.TaskExtensions_SetFromTask_TaskStateFaulted));
                    else
                        tcs.SetException(task.Exception.InnerExceptions);
                    break;
                case TaskStatus.Canceled:
                    tcs.SetCanceled();
                    break;
                default:
                    throw new InvalidOperationException(Resources.TaskExtensions_SetFromTask_TaskWasNotCompleted);
            }
        }

        /// <summary>
        ///   Performs an <see cref="AsyncCallback">async callback</see> once a task is complete
        ///   (preserving the state).
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="callback">The method to be called once the task is complete.</param>
        /// <param name="state">
        ///   The object to use as the underlying <see cref="Task"/>'s state.
        /// </param>
        /// <returns>The passed in <paramref name="task"/>.</returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="task"/> is a <see langword="null"/>.
        /// </exception>
        [UsedImplicitly]
        public static Task WithAsyncCallback(
            [NotNull] this Task task,
            [CanBeNull] AsyncCallback callback,
            [CanBeNull] object state)
        {
            if (task == null) throw new ArgumentNullException("task");

            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>(state);
            task.ContinueWith(_ =>
                {
                    tcs.SetFromTask(task);
                    if (callback != null) callback(tcs.Task);
                });

            return tcs.Task;
        }

        /// <summary>
        ///   Performs an <see cref="AsyncCallback">async callback</see> once a task is complete
        ///   (preserving the task's state).
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="task">The task.</param>
        /// <param name="callback">The method to be called once the task is complete.</param>
        /// <param name="state">
        ///   The object to use as the underlying <see cref="Task&lt;TResult&gt;"/>'s state.
        /// </param>
        /// <returns>The passed in <paramref name="task"/>.</returns>
        [UsedImplicitly]
        public static Task<TResult> WithAsyncCallback<TResult>(
            [NotNull] this Task<TResult> task,
            [CanBeNull] AsyncCallback callback,
            [CanBeNull] object state)
        {
            TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>(state);
            task.ContinueWith(_ =>
                {
                    tcs.SetFromTask(task);
                    if (callback != null) callback(tcs.Task);
                });

            return tcs.Task;
        }

        /// <summary>
        ///   Takes a function that creates a <see cref="Task"/> and wraps it so that it will always return a <see cref="Task"/>.
        ///   If the passed in function throws an exception then this is turned into a <see cref="Task"/> that throws the exception.
        /// </summary>
        /// <param name="taskCreator">A function that creates a task.</param>
        /// <returns>
        ///   The created <see cref="Task"/>.
        ///   If the <paramref name="taskCreator"/> threw an exception then the returned task is a task
        ///   that will throw that exception.
        /// </returns>
        /// <remarks>
        ///   This is particularly vital for APM where the exception needs to propagate to the end call.
        /// </remarks>
        [UsedImplicitly]
        public static Task Safe([NotNull] this Func<Task> taskCreator)
        {
            try
            {
                return taskCreator();
            }
            catch (Exception e)
            {
                return new Task(
                    () => { throw e; });
            }
        }

        /// <summary>
        ///   Takes a function that creates a <see cref="System.Threading.Tasks.Task&lt;TResult&gt;">Task</see>
        ///   and wraps it so that it will always return a <see cref="System.Threading.Tasks.Task&lt;TResult&gt;">Task</see>.
        ///   If the passed in function throws an exception then this is turned into a
        ///   <see cref="System.Threading.Tasks.Task&lt;TResult&gt;">Task</see> that throws the exception.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="taskCreator">A function that creates a task.</param>
        /// <returns>
        ///   <para>The created <see cref="System.Threading.Tasks.Task&lt;TResult&gt;">Task</see>.</para>
        ///   <para>If the <paramref name="taskCreator"/> threw an exception then the returned task is a task
        ///   that will throw that exception.</para>
        /// </returns>
        /// <remarks>
        ///   This is particularly vital for APM where the exception needs to propagate to the end call.
        /// </remarks>
        [UsedImplicitly]
        [NotNull]
        public static Task<TResult> Safe<TResult>([NotNull] this Func<Task<TResult>> taskCreator)
        {
            Contract.Requires(taskCreator != null);
            try
            {
                return taskCreator();
            }
            catch (Exception e)
            {
                return new Task<TResult>(
                    () => { throw e; });
            }
        }

        /// <summary>
        ///   Creates a task continuation if necessary but supports a null antecedent.
        ///   Also handles errors and cancellation of the original task.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TNewResult">The type of the new result.</typeparam>
        /// <param name="task">The task.</param>
        /// <param name="continuation">The continuation.</param>
        /// <returns>The continuation task.</returns>
        /// <exception cref="TaskCanceledException">Antecedent task was cancelled.</exception>
        /// <exception cref="InvalidOperationException">
        ///   <para>The <see cref="System.Threading.Tasks.TaskStatus"/> was in a faulted state.
        ///   No exception could be retrieved.</para>
        ///   <para>-or-</para>
        ///   <para>The antecedent task was in an invalid state.</para>
        /// </exception>
        public static Task<TNewResult> After<TResult, TNewResult>([CanBeNull] this Task<TResult> task,
                                                                  [NotNull] Func<Task<TResult>, TNewResult> continuation)
        {
            return After(task, continuation, Task<TNewResult>.Factory.CreationOptions);
        }

        /// <summary>
        ///   Creates a task continuation if necessary but supports a null antecedent.
        ///   Also handles errors and cancellation of the original task.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TNewResult">The type of the new result.</typeparam>
        /// <param name="task">The task.</param>
        /// <param name="continuation">The continuation.</param>
        /// <param name="creationOptions">
        ///   The creation options, which can be used to control the behaviour of the task.
        /// </param>
        /// <returns>The continuation task.</returns>
        /// <exception cref="TaskCanceledException">Antecedent task was cancelled.</exception>
        /// <exception cref="InvalidOperationException">
        ///   <para>The <see cref="System.Threading.Tasks.TaskStatus"/> was in a faulted state.
        ///   No exception could be retrieved.</para>
        ///   <para>-or-</para>
        ///   <para>The antecedent task was in an invalid state.</para>
        /// </exception>
        /// <remarks>
        ///   An exception will be thrown if <paramref name="task"/> is ended prematurely by an exception.
        /// </remarks>
        [UsedImplicitly]
        public static Task<TNewResult> After<TResult, TNewResult>([CanBeNull] this Task<TResult> task,
                                                                  [NotNull] Func<Task<TResult>, TNewResult> continuation,
                                                                  TaskCreationOptions creationOptions)
        {
            // If the antecedent task is null just start the continuation (but pass in null).
            if ((task == null))
                return Task.Factory.StartNew(() => continuation(null), creationOptions);

            // Create a continuation.
            return task.ContinueWith(
                t =>
                    {
                        if (t == null)
                            return continuation(null);

                        if (t.Exception != null)
                            throw t.Exception;

                        if (t.IsCanceled)
                            throw new TaskCanceledException(Resources.TaskExtensions_After_AntecedentTaskCancelled);

                        switch (t.Status)
                        {
                            case TaskStatus.RanToCompletion:
                                // We're OK to continue
                                return continuation(t);
                            case TaskStatus.Canceled:
                                throw new TaskCanceledException(Resources.TaskExtensions_After_AntecedentTaskCancelled);
                            case TaskStatus.Faulted:
                                throw new InvalidOperationException(
                                    Resources.TaskExtensions_After_AntecedentTaskInFaultedState);
                            default:
                                throw new InvalidOperationException(
                                    Resources.TaskExtensions_After_AntecedentTaskInvalidState);
                        }
                    }, creationOptions.GetEquivalentContinuationOptions());
        }

        /// <summary>
        ///   Creates a task continuation if necessary but supports zero antecedents.
        ///   Also handles errors and cancellation of the original task.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TNewResult">The type of the new result.</typeparam>
        /// <param name="tasks">The tasks.</param>
        /// <param name="continuation">The continuation.</param>
        /// <param name="creationOptions">
        ///   The creation options, which can be used to control the behaviour of the task.
        /// </param>
        /// <returns>The continuation task.</returns>
        /// <exception cref="TaskCanceledException">An antecedent task was cancelled.</exception>
        /// <exception cref="InvalidOperationException">
        ///   <para>The <see cref="System.Threading.Tasks.TaskStatus"/> was in a faulted state.
        ///   No exception could be retrieved.</para>
        ///   <para>-or-</para>
        ///   <para>The antecedent task was in an invalid state.</para>
        /// </exception>
        public static Task<TNewResult> AfterAll<TResult, TNewResult>([CanBeNull] this IEnumerable<Task<TResult>> tasks,
                                                                     [NotNull] Func<IEnumerable<Task<TResult>>, TNewResult>
                                                                         continuation,
                                                                     TaskCreationOptions creationOptions)
        {
            if (tasks == null) tasks = Enumerable.Empty<Task<TResult>>();
            Task<TResult>[] taskArray = tasks.ToArray();

            // If there are no tasks to wait for start a new task.
            if (taskArray.Length < 1)
                return Task.Factory.StartNew(() => continuation(tasks), creationOptions);

            return Task<TNewResult>.Factory.ContinueWhenAll(
                taskArray,
                antecedents =>
                    {
                        if ((antecedents == null) || (antecedents.Length < 1))
                            return continuation(null);

                        List<Task<TResult>> taskList = new List<Task<TResult>>();
                        List<Exception> exceptions = new List<Exception>();
                        foreach (Task<TResult> antecedent in antecedents)
                        {
                            if (antecedent == null)
                                continue;

                            if (antecedent.Exception != null)
                            {
                                if (antecedent.Exception.InnerExceptions != null)
                                    exceptions.AddRange(antecedent.Exception.InnerExceptions);
                                else
                                    exceptions.Add(antecedent.Exception);
                                continue;
                            }

                            if (antecedent.IsCanceled)
                                throw new TaskCanceledException(
                                    Resources.TaskExtensions_AfterAll_AntecedentTaskCancelled);

                            switch (antecedent.Status)
                            {
                                case TaskStatus.RanToCompletion:
                                    // We're OK to continue
                                    taskList.Add(antecedent);
                                    break;
                                case TaskStatus.Canceled:
                                    throw new TaskCanceledException(
                                        Resources.TaskExtensions_AfterAll_AntecedentTaskCancelled);
                                case TaskStatus.Faulted:
                                    exceptions.Add(
                                        new InvalidOperationException(
                                            Resources.TaskExtensions_AfterAll_AntecedentTaskInFaultedState));
                                    break;
                                default:
                                    exceptions.Add(
                                        new InvalidOperationException(
                                            Resources.TaskExtensions_AfterAll_AntecedentTaskInvalidState));
                                    break;
                            }
                        }
                        if (exceptions.Count > 0)
                            throw new AggregateException(exceptions);

                        // No exceptions or cancellations
                        return continuation(taskList);
                    }, creationOptions.GetEquivalentContinuationOptions());
        }

        /// <summary>
        ///   Gets the equivalent <see cref="System.Threading.Tasks.TaskContinuationOptions"/> from the specified
        ///   <see cref="System.Threading.Tasks.TaskCreationOptions"/>.
        /// </summary>
        /// <param name="creationOptions">
        ///   The creation options, which can be used to control the behaviour of the task.
        /// </param>
        /// <returns>
        ///   The equivalent <see cref="System.Threading.Tasks.TaskContinuationOptions"/> from the
        ///   <paramref name="creationOptions"/> specified.
        /// </returns>
        /// <remarks>
        ///   Useful for quickly passing a <see cref="Task"/>'s
        ///   <see cref="System.Threading.Tasks.TaskCreationOptions">creation options</see> to a continuation.
        /// </remarks>
        [UsedImplicitly]
        public static TaskContinuationOptions GetEquivalentContinuationOptions(this TaskCreationOptions creationOptions)
        {
            return (TaskContinuationOptions) creationOptions;
        }

        /// <summary>
        ///   Combines all wait handles into a single, <see cref="System.Threading.WaitHandle"/>
        ///   that is signalled when any handle has been signalled.
        /// </summary>
        /// <param name="handle">The wait handle.</param>
        /// <param name="handles">The handles.</param>
        /// <returns>
        ///   The combined wait <paramref name="handles"/> as a single <see cref="System.Threading.WaitHandle"/> object.
        /// </returns>
        /// <exception cref="System.Security.SecurityException">
        ///   The caller does not have the required permission.
        /// </exception>
        [UsedImplicitly]
        public static WaitHandle WaitAny([NotNull] this WaitHandle handle, [NotNull] params WaitHandle[] handles)
        {
            ManualResetEvent newHandle = new ManualResetEvent(false);
            int handleCount = handles.Length;
            RegisteredWaitHandle[] waitHandles = new RegisteredWaitHandle[handleCount + 1];

            // Register waits for each handle on the thread pool.
            for (int h = 0; h <= handleCount; h++)
            {
                WaitHandle currentHandle = (h == handleCount) ? handle : handles[h];
                waitHandles[h] = ThreadPool
                    .UnsafeRegisterWaitForSingleObject(
                        currentHandle,
                        delegate
                            {
                                // Signal the manual reset event.
                                newHandle.Set();
                            },
                        null, -1, true);
            }

            // Cancel all waits when we finally get the signal.
            ThreadPool
                .UnsafeRegisterWaitForSingleObject(
                    newHandle,
                    delegate
                        {
                            foreach (RegisteredWaitHandle waiter in waitHandles)
                                waiter.Unregister(null);
                        },
                    null, -1, true);

            return newHandle;
        }

        /// <summary>
        ///   Combines all wait handles into a single, <see cref="System.Threading.WaitHandle"/>
        ///   that is signalled when all handles have been signalled.
        /// </summary>
        /// <param name="handle">The wait handle.</param>
        /// <param name="handles">The handles to combine.</param>
        /// <returns>
        ///   The combined wait <paramref name="handles"/> as a single <see cref="System.Threading.WaitHandle"/> object.
        /// </returns>
        /// <exception cref="System.Security.SecurityException">
        ///   The caller does not have the required permission.
        /// </exception>
        [UsedImplicitly]
        public static WaitHandle WaitAll([NotNull] this WaitHandle handle, [NotNull] params WaitHandle[] handles)
        {
            ManualResetEvent newHandle = new ManualResetEvent(false);

            // Create a queue from the handles.
            Queue<WaitHandle> handleQueue = new Queue<WaitHandle>(handles);
            // Register initial wait.
            ThreadPool
                .UnsafeRegisterWaitForSingleObject(
                    handle,
                    (s, t) => WaitAllCallback(newHandle, handleQueue),
                    null, -1, true);

            // Return the new handle.
            return newHandle;
        }

        /// <summary>
        ///   The callback for <see cref="WebApplications.Utilities.TaskExtensions.WaitAll"/>.
        /// </summary>
        /// <param name="newHandle">The new combined handle.</param>
        /// <param name="handleQueue">A FIFO (first-in, first-out) collection of handle objects.</param>
        /// <exception cref="System.Security.SecurityException">
        ///   The caller does not have the required permission.
        /// </exception>
        private static void WaitAllCallback([NotNull] ManualResetEvent newHandle,
                                            [NotNull] Queue<WaitHandle> handleQueue)
        {
            // If the queue is empty we're done.
            if (handleQueue.Count < 1)
                newHandle.Set();

            WaitHandle nextHandle = handleQueue.Dequeue();

            // Register next wait.
            ThreadPool
                .UnsafeRegisterWaitForSingleObject(
                    nextHandle,
                    (s, t) => WaitAllCallback(newHandle, handleQueue),
                    null, -1, true);
        }

        /// <summary>
        ///   Converts APM to TPL, supports task cancellation.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="asyncResult">The async result.</param>
        /// <param name="endMethod">The method to execute once the async method completes.</param>
        /// <param name="cancellationMethod">The method to run if cancellation is requested.</param>
        /// <param name="creationOptions">
        ///   The creation options, which can be used to control the task behaviour.
        /// </param>
        /// <param name="scheduler">
        ///   <para>The scheduler, handles all the scheduling logic.</para>
        ///   <para>By default this uses <see cref="TaskScheduler.Default"/>.</para>
        /// </param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        ///   The <see cref="System.Threading.Tasks.Task&lt;TResult&gt;">Task&lt;TResult&gt;</see> created by
        ///   <see cref="System.Threading.Tasks.TaskCompletionSource&lt;TResult&gt;">TaskCompletionSource&lt;TResult&gt;</see>.
        /// </returns>
        [UsedImplicitly]
        public static Task<TResult> FromAsync<TResult>([NotNull] this IAsyncResult asyncResult,
                                                       [NotNull] Func<IAsyncResult, TResult> endMethod,
                                                       [CanBeNull] Action<IAsyncResult> cancellationMethod = null,
                                                       TaskCreationOptions creationOptions = TaskCreationOptions.None,
                                                       [CanBeNull] TaskScheduler scheduler = null,
                                                       CancellationToken cancellationToken = default(CancellationToken))
        {
            bool cancellable = cancellationToken != CancellationToken.None;

            if (scheduler == null)
                scheduler = TaskScheduler.Default;

            // Create a task completion source.
            TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>();

            // Create the continuation task that will run once the APM method completes.
            Task t = new Task(
                () =>
                    {
                        Exception exception = null;
                        OperationCanceledException canceledException = null;
                        TResult result = default(TResult);
                        try
                        {
                            result = endMethod(asyncResult);
                        }
                        catch (OperationCanceledException ex)
                        {
                            canceledException = ex;
                        }
                        catch (Exception ex)
                        {
                            exception = ex;
                        }
                        finally
                        {
                            if ((canceledException != null) || cancellationToken.IsCancellationRequested)
                                // The task was cancelled
                                // note we don't call the cancellation method as the APM call is already finished.
                                tcs.TrySetCanceled();
                            else if (exception != null)
                                tcs.TrySetException(exception);
                            else
                                tcs.TrySetResult(result);
                        }
                    },
                cancellationToken,
                creationOptions);

            // If we're already complete run the continuation task.
            if (asyncResult.IsCompleted)
            {
                try
                {
                    t.RunSynchronously(scheduler);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            }
            else
            {
                // We're not complete so get the wait handle.
                WaitHandle waitHandle = asyncResult.AsyncWaitHandle;

                // If we have a cancellation token combine wait handles.
                if (cancellable)
                    waitHandle = waitHandle.WaitAny(cancellationToken.WaitHandle);

                // Register wait for result.
                RegisteredWaitHandle endTaskWaiter = ThreadPool.RegisterWaitForSingleObject(
                    waitHandle,
                    (state, timeout) =>
                        {
                            if (cancellationToken.IsCancellationRequested)
                            {
                                // We cancelled before APM completed so we need to notify
                                // Cancellation method (if any)
                                if (cancellationMethod != null)
                                    try
                                    {
                                        cancellationMethod(asyncResult);
                                    }
                                    catch (Exception e)
                                    {
                                        // If cancellation failed set the exception.
                                        tcs.TrySetException(e);
                                        return;
                                    }

                                // Set the completion source to cancelled
                                tcs.TrySetCanceled();
                                return;
                            }

                            // We have a result from the APM call, run the continuation task.
                            try
                            {
                                t.RunSynchronously(scheduler);
                            }
                            catch (Exception ex)
                            {
                                tcs.TrySetException(ex);
                            }
                        }, null, -1, true);
            }

            // Return the task completion source's task.
            return tcs.Task;
        }

        /// <summary>
        /// Provides await functionality for ordinary <see cref="WaitHandle"/>s.
        /// </summary>
        /// <param name="handle">The handle to wait on.</param>
        /// <returns>The awaiter.</returns>
        [UsedImplicitly]
        public static TaskAwaiter GetAwaiter(this WaitHandle handle)
        {
            return handle.ToTask().GetAwaiter();
        }

        /// <summary>
        /// Creates a TPL Task that is marked as completed when a <see cref="WaitHandle"/> is signaled.
        /// </summary>
        /// <param name="handle">The handle whose signal triggers the task to be completed.</param>
        /// <returns>A Task that is completed after the handle is signaled.</returns>
        /// <remarks>
        /// There is a (brief) time delay between when the handle is signaled and when the task is marked as completed.
        /// </remarks>
        [UsedImplicitly]
        public static Task ToTask(this WaitHandle handle)
        {
            Contract.Ensures(Contract.Result<Task>() != null);

            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
            object localVariableInitLock = new object();
            lock (localVariableInitLock)
            {
                RegisteredWaitHandle callbackHandle = null;
                callbackHandle = ThreadPool.RegisterWaitForSingleObject(
                    handle,
                    (state, timedOut) =>
                        {
                            tcs.SetResult(null);

                            // We take a lock here to make sure the outer method has completed setting the local variable callbackHandle.
                            lock (localVariableInitLock)
                            {
                                callbackHandle.Unregister(null);
                            }
                        },
                    null,
                    Timeout.Infinite,
                    executeOnlyOnce: true);
            }

            return tcs.Task;
        }

        /// <summary>
        /// Creates a Task from an exception
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>A task.</returns>
        [PublicAPI]
        public static Task ToTask([NotNull] this Exception exception)
        {
            TaskCompletionSource<Exception> source = new TaskCompletionSource<Exception>();
            source.SetException(exception);
            return source.Task;
        }

        /// <summary>
        /// Creates a Task from an exception
        /// </summary>
        /// <typeparam name="TResult">The type of the T result.</typeparam>
        /// <param name="exception">The exception.</param>
        /// <returns>A task.</returns>
        [PublicAPI]
        public static Task<TResult> ToTask<TResult>([NotNull] this Exception exception)
        {
            TaskCompletionSource<TResult> source = new TaskCompletionSource<TResult>();
            source.SetException(exception);
            return source.Task;
        }

        /// <summary>
        /// Adds cancellation support to a task that is otherwise not cancellable.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An awaitable task</returns>
        /// <remarks><para>This should be used with caution, as it doesn't stop the underlying task, which continues to execute, instead it stops you waiting for it.
        /// This can be desirable behaviour when used properly, but it must be understood that the underlying task is still running and so care should be
        /// taken to not make use of any shared resources, etc.</para>
        /// <para>See http://blogs.msdn.com/b/pfxteam/archive/2012/10/05/how-do-i-cancel-non-cancelable-async-operations.aspx for more information.</para></remarks>
        [NotNull]
        public static Task WithCancellation([NotNull] this Task task, CancellationToken cancellationToken)
        {
            Contract.Requires(task != null);
            if (task.IsCompleted || !cancellationToken.CanBeCanceled)
                return task;

            return cancellationToken.IsCancellationRequested
                ? new Task(() => {}, cancellationToken)
                : WithCancellationInternal(task, cancellationToken);
        }

        /// <summary>
        /// Adds cancellation support to a task that is otherwise not cancellable.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.OperationCanceledException"></exception>
        [NotNull]
        private static async Task WithCancellationInternal([NotNull] Task task, CancellationToken cancellationToken)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            using (cancellationToken.Register(s => ((TaskCompletionSource<bool>) s).TrySetResult(true), tcs))
                if (task != await Task.WhenAny(task, tcs.Task))
                    throw new OperationCanceledException(cancellationToken);
            await task;
        }

        /// <summary>
        /// Adds cancellation support to a task that is otherwise not cancellable.
        /// </summary>
        /// <typeparam name="T">The task result.</typeparam>
        /// <param name="task">The task.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An awaitable task</returns>
        /// <remarks>
        /// <para>This should be used with caution, as it doesn't stop the underlying task, which continues to execute, instead it stops you waiting for it.
        /// This can be desirable behaviour when used properly, but it must be understood that the underlying task is still running and so care should be
        /// taken to not make use of any shared resources, etc.</para>
        /// <para>See http://blogs.msdn.com/b/pfxteam/archive/2012/10/05/how-do-i-cancel-non-cancelable-async-operations.aspx for more information.</para>
        /// </remarks>
        [NotNull]
        public static Task<T> WithCancellation<T>([NotNull] this Task<T> task, CancellationToken cancellationToken)
        {
            Contract.Requires(task != null);
            if (task.IsCompleted || !cancellationToken.CanBeCanceled)
                return task;

            return cancellationToken.IsCancellationRequested
                ? new Task<T>(() => default(T), cancellationToken)
                : WithCancellationInternal(task, cancellationToken);
        }

        /// <summary>
        /// Adds cancellation support to a task that is otherwise not cancellable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task">The task.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="System.OperationCanceledException"></exception>
        [NotNull]
        private static async Task<T> WithCancellationInternal<T>([NotNull] Task<T> task, CancellationToken cancellationToken)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            using (cancellationToken.Register(s => ((TaskCompletionSource<bool>) s).TrySetResult(true), tcs))
                if (task != await Task.WhenAny(task, tcs.Task))
                    throw new OperationCanceledException(cancellationToken);
            return await task;
        }

        /// <summary>
        /// Gets the awaiter for a <see cref="SynchronizationContext"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>SynchronizationContextAwaiter.</returns>
        [PublicAPI]
        [NotNull]
        public static SynchronizationContextAwaiter GetAwaiter([NotNull]this SynchronizationContext context)
        {
            Contract.Requires(context != null);
            return new SynchronizationContextAwaiter(context);
        }

        /// <summary>
        /// Invokes the action on the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="callback">The callback.</param>
        [PublicAPI]
        public static void Invoke([NotNull] this SynchronizationContext context, [NotNull] Action callback)
        {
            Contract.Requires(context != null);
            Contract.Requires(callback != null);
            context.Send(_ => callback(), null);
        }

        /// <summary>
        /// Invokes the function on the specified context.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">The context.</param>
        /// <param name="callback">The callback.</param>
        /// <returns>T.</returns>
        [PublicAPI]
        public static T Invoke<T>([NotNull] this SynchronizationContext context, [NotNull] Func<T> callback)
        {
            Contract.Requires(context != null);
            Contract.Requires(callback != null);
            T result = default(T);
            context.Send(_ => result = callback(), null);

            return result;
        }
    }
}