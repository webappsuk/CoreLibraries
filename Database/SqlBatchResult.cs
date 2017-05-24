#region © Copyright Web Applications (UK) Ltd, 2017.  All rights reserved.
// Copyright (c) 2017, Web Applications UK Ltd
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

// TODO Remove when migrated to .net standard project format
#define NET452

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Threading;

namespace WebApplications.Utilities.Database
{
    /// <summary>
    /// Represents the result of a batched <see cref="SqlProgram"/>
    /// </summary>
    public abstract class SqlBatchResult
    {
        /// <summary>
        /// The <see cref="TaskCreationOptions"/> to use for the results <see cref="TaskCompletionSource" />
        /// </summary>
#if NET452
        protected static readonly TaskCreationOptions CompletionSourceOptions;
        static SqlBatchResult()
        {
            FieldInfo field = typeof(TaskCreationOptions).GetField("RunContinuationsAsynchronously");
            if (field == null)
            {
                CompletionSourceOptions = TaskCreationOptions.None;
                return;
            }

            CompletionSourceOptions = (TaskCreationOptions)field.GetValue(null);
        }
#else
        protected const TaskCreationOptions CompletionSourceOptions = TaskCreationOptions.RunContinuationsAsynchronously;
#endif

        /// <summary>
        /// The command that this is the result of.
        /// </summary>
        [CanBeNull]
        private SqlBatchCommand _command;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlBatchResult"/> class.
        /// </summary>
        internal SqlBatchResult()
        {
        }

        /// <summary>
        /// The command that this is the result of.
        /// </summary>
        [NotNull]
        protected internal SqlBatchCommand Command
        {
            get
            {
                Debug.Assert(_command != null);
                return _command;
            }
            internal set
            {
                Debug.Assert(_command == null);
                _command = value;
            }
        }

        /// <summary>
        /// Determines whether this instance is completed.
        /// </summary>
        /// <returns>
        ///   <see langword="true" /> if this instance is completed; otherwise, <see langword="false" />.
        /// </returns>
        internal abstract bool IsCompleted();

        /// <summary>
        /// Determines whether the specified index is completed.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>
        ///   <see langword="true" /> if the specified index is completed; otherwise, <see langword="false" />.
        /// </returns>
        internal abstract bool IsCompleted(int index);

        /// <summary>
        /// Sets the result count.
        /// </summary>
        /// <param name="count">The count.</param>
        internal abstract void SetResultCount(int count);

        /// <summary>
        /// Sets the result to completed.
        /// </summary>
        /// <param name="index">The index.</param>
        internal abstract void SetCompleted(int index);

        /// <summary>
        /// Sets the exception(s) that occurred.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="exceptions">The exceptions.</param>
        internal abstract void SetException(int index, [NotNull] IEnumerable<Exception> exceptions);

        /// <summary>
        /// Sets the exception(s) that occurred.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="exceptions">The exceptions.</param>
        internal void SetException(int index, [NotNull] params Exception[] exceptions)
            => SetException(index, (IEnumerable<Exception>)exceptions);

        /// <summary>
        /// Sets the result to canceled.
        /// </summary>
        /// <param name="index">The index.</param>
        internal abstract void SetCanceled(int index);

        /// <summary>
        /// Sets the result to canceled if it is not yet completed.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token that caused the cancellation.</param>
        internal abstract void SetCanceledIfNotComplete(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Sets the result to faulted if it is not yet completed.
        /// </summary>
        /// <param name="exceptionFactory">A function which returns the exception for this result.</param>
        internal abstract void SetExceptionIfNotComplete([NotNull] Func<Exception> exceptionFactory);

        /// <summary>
        /// Gets the result asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token which can be used to cancel the operation. The batch will continue running.</param>
        /// <returns>An awaitable task.</returns>
        [NotNull]
        public Task GetResultAsync(CancellationToken cancellationToken = default(CancellationToken))
            => GetResultInternalAsync(false, cancellationToken);

        /// <summary>
        /// Gets the result asynchronously.
        /// </summary>
        /// <param name="startExecuting">If set to <see langword="true" /> the batch will start executing if it hasn't already.</param>
        /// <param name="cancellationToken">A cancellation token which can be used to cancel the operation. The batch will continue running.</param>
        /// <returns>An awaitable task.</returns>
        [NotNull]
        public Task GetResultAsync(bool startExecuting, CancellationToken cancellationToken = default(CancellationToken))
            => GetResultInternalAsync(startExecuting, cancellationToken);

        /// <summary>
        /// When overridden, gets the result asynchronously.
        /// </summary>
        /// <param name="startExecuting">If set to <see langword="true" /> the batch will start executing if it hasn't already.</param>
        /// <param name="cancellationToken">A cancellation token which can be used to cancel the operation. The batch will continue running.</param>
        /// <returns>
        /// An awaitable task.
        /// </returns>
        [NotNull]
        protected abstract Task GetResultInternalAsync(bool startExecuting, CancellationToken cancellationToken);
    }

    /// <summary>
    /// Represents the result of a batched <see cref="SqlProgram"/>
    /// </summary>
    /// <typeparam name="T">The type of the value returned by the program.</typeparam>
    public sealed class SqlBatchResult<T> : SqlBatchResult
    {
        private (T result, bool completed, Exception[] exceptions)[] _results;
        private bool _cancelled;

        [NotNull]
        private readonly TaskCompletionSource _completionSource =
            new TaskCompletionSource(CompletionSourceOptions);

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlBatchResult"/> class.
        /// </summary>
        internal SqlBatchResult()
        {
        }

        /// <summary>
        /// Determines whether this instance is completed.
        /// </summary>
        /// <returns>
        ///   <see langword="true" /> if this instance is completed; otherwise, <see langword="false" />.
        /// </returns>
        internal override bool IsCompleted() => _completionSource.Task.IsCompleted;

        /// <summary>
        /// Determines whether the specified index is completed.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>
        ///   <see langword="true" /> if the specified index is completed; otherwise, <see langword="false" />.
        /// </returns>
        internal override bool IsCompleted(int index) => _results != null && _results[index].completed;

        /// <summary>
        /// Sets the result count.
        /// </summary>
        /// <param name="count">The count.</param>
        internal override void SetResultCount(int count)
        {
            _results = new(T, bool, Exception[])[count];
        }

        /// <summary>
        /// Sets the result for the index given.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        internal void SetResult(int index, T value)
        {
            Debug.Assert(_results != null, "_results != null");

            _results[index].result = value;
        }

        /// <summary>
        /// Sets the result to completed.
        /// </summary>
        /// <param name="index">The index.</param>
        internal override void SetCompleted(int index)
        {
            Debug.Assert(_results != null, "_results != null");

            // Set the result to completed
            _results[index].completed = true;

            if (_completionSource.Task.IsCompleted) return;

            // If all the results are completed, we need to complete the task
            foreach ((_, bool completed, _) in _results)
                if (!completed)
                    return;

            // If there are any exceptions, set the result to faulted
            if (_results.Any(r => r.exceptions != null && r.exceptions.Length > 0))
            {
                Exception GetException((T, bool, Exception[] exceptions) result, int ind)
                {
                    if (result.exceptions == null || result.exceptions.Length < 1) return null;
                    if (result.exceptions.Length < 2) return result.exceptions[0];
                    return new AggregateException(
                        $"Multiple exceptions occurred for the program on connection #{ind}",
                        result.exceptions);
                }

                _completionSource.TrySetException(_results.Select(GetException).Where(e => e != null));
                return;
            }

            // If any connection was cancelled, cancel the result
            if (_cancelled)
            {
                _completionSource.TrySetCanceled();
                return;
            }

            // Otherwise its completed
            _completionSource.TrySetCompleted();
        }

        /// <summary>
        /// Sets the exception that occurred.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="exceptions"></param>
        internal override void SetException(int index, IEnumerable<Exception> exceptions)
        {
            Debug.Assert(_results != null, "_results != null");
            Debug.Assert(exceptions != null);

            // Combine the exceptions for the index
            AddExceptions(ref _results[index].exceptions, exceptions);

            void AddExceptions(ref Exception[] origEx, IEnumerable<Exception> newEx)
            {
                origEx = origEx?.Union(newEx).ToArray() ?? newEx as Exception[] ?? newEx.ToArray();
            }
        }

        /// <summary>
        /// Sets the result to canceled.
        /// </summary>
        /// <param name="index">The index.</param>
        internal override void SetCanceled(int index) => _cancelled = true;

        /// <summary>
        /// Sets the result to canceled if it is not yet completed.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token that caused the cancellation.</param>
        internal override void SetCanceledIfNotComplete(CancellationToken cancellationToken = default(CancellationToken))
#if NET452
            => _completionSource.TrySetCanceled();
#else
            => _completionSource.TrySetCanceled(cancellationToken);
#endif

        /// <summary>
        /// Sets the result to faulted if it is not yet completed.
        /// </summary>
        /// <param name="exceptionFactory">A function which returns the exception for this result.</param>
        internal override void SetExceptionIfNotComplete(Func<Exception> exceptionFactory)
        {
            if (!_completionSource.Task.IsCompleted)
                _completionSource.TrySetException(exceptionFactory());
        }

        /// <summary>
        /// Gets the result asynchronously.
        /// </summary>
        /// <param name="startExecuting">If set to <see langword="true" /> the batch will start executing if it hasn't already.</param>
        /// <param name="cancellationToken">A cancellation token which can be used to cancel the operation. The batch will continue running.</param>
        /// <returns>An awaitable task.</returns>
        protected override Task GetResultInternalAsync(bool startExecuting, CancellationToken cancellationToken) 
            => GetResultAsync(startExecuting, cancellationToken);

        /// <summary>
        /// Gets the result asynchronously.
        /// </summary>
        /// <remarks>If the batch was executed against all connections, this will return the result of a single connection.</remarks>
        /// <param name="cancellationToken">A cancellation token which can be used to cancel the operation. The batch will continue running.</param>
        /// <returns>An awaitable task which returns the result.</returns>
        [NotNull]
        public new Task<T> GetResultAsync(CancellationToken cancellationToken = default(CancellationToken)) 
            => GetResultAsync(false, cancellationToken);

        /// <summary>
        /// Gets the result asynchronously.
        /// </summary>
        /// <remarks>If the batch was executed against all connections, this will return the result of a single connection.</remarks>
        /// <param name="startExecuting">If set to <see langword="true" /> the batch will start executing if it hasn't already.</param>
        /// <param name="cancellationToken">A cancellation token which can be used to cancel the operation. The batch will continue running.</param>
        /// <returns>An awaitable task which returns the result.</returns>
        [NotNull]
        public new async Task<T> GetResultAsync(bool startExecuting, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Execute the command if its not already running, if requested
            if (startExecuting)
                Command.Owner.BeginExecute(false);

            await _completionSource.Task.WithCancellation(cancellationToken).ConfigureAwait(false);

#if NET452
            // If TCO.RunContinuationsAsynchronously isnt supported, need to yield after waiting for the task
            if (CompletionSourceOptions == TaskCreationOptions.None)
                await Task.Yield();
#endif

            Debug.Assert(_results != null, "_results != null");
            return _results[0].result;
        }

        /// <summary>
        /// Gets the results for all connections asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token which can be used to cancel the operation. The batch will continue running.</param>
        /// <returns>An awaitable task which returns the results for all connections.</returns>
        [NotNull]
        public Task<IEnumerable<T>> GetResultsAsync(CancellationToken cancellationToken = default(CancellationToken))
            => GetResultsAsync(false, cancellationToken);

        /// <summary>
        /// Gets the results for all connections asynchronously.
        /// </summary>
        /// <param name="startExecuting">If set to <see langword="true" /> the batch will start executing if it hasn't already.</param>
        /// <param name="cancellationToken">A cancellation token which can be used to cancel the operation. The batch will continue running.</param>
        /// <returns>An awaitable task which returns the results for all connections.</returns>
        [NotNull]
        public async Task<IEnumerable<T>> GetResultsAsync(bool startExecuting, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Execute the command for all connections if its not already running
            if (startExecuting)
                Command.Owner.BeginExecute(true);

            await _completionSource.Task.WithCancellation(cancellationToken).ConfigureAwait(false);

#if NET452
            // If TCO.RunContinuationsAsynchronously isnt supported, need to yield after waiting for the task
            if (CompletionSourceOptions == TaskCreationOptions.None)
                await Task.Yield();
#endif

            Debug.Assert(_results != null, "_results != null");
            return _results.Select(t => t.result);
        }
    }
}