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
        /// Sets the exception that occurred.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="exception">The exception.</param>
        internal abstract void SetException(int index, Exception exception);

        /// <summary>
        /// Sets the result to canceled.
        /// </summary>
        /// <param name="index">The index.</param>
        internal abstract void SetCanceled(int index);

        /// <summary>
        /// Gets the result asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token which can be used to cancel the operation. The batch will continue running.</param>
        /// <returns>An awaitable task.</returns>
        [NotNull]
        public Task GetResultAsync(CancellationToken cancellationToken = default(CancellationToken))
            => GetResultInternalAsync(cancellationToken);

        /// <summary>
        /// When overridden, gets the result asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token which can be used to cancel the operation. The batch will continue running.</param>
        /// <returns>An awaitable task.</returns>
        [NotNull]
        protected abstract Task GetResultInternalAsync(CancellationToken cancellationToken);
    }

    /// <summary>
    /// Represents the result of a batched <see cref="SqlProgram"/>
    /// </summary>
    /// <typeparam name="T">The type of the value returned by the program.</typeparam>
    public sealed class SqlBatchResult<T> : SqlBatchResult
    {
        private (T result, bool completed, Exception exception)[] _results;
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
        /// Sets the result count.
        /// </summary>
        /// <param name="count">The count.</param>
        internal override void SetResultCount(int count)
        {
            _results = new(T, bool, Exception)[count];
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
            // Set the result to completed
            _results[index].completed = true;

            if (_completionSource.Task.IsCompleted) return;

            // If all the results are completed, we need to complete the task
            foreach ((_, bool completed, _) in _results)
                if (!completed) return;

            // If there are any exceptions, set the result to faulted
            if (_results.Any(r => r.exception != null))
            {
                _completionSource.TrySetException(_results.Select(t => t.exception).Where(e => e != null));
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
        /// <param name="exception">The exception.</param>
        internal override void SetException(int index, Exception exception)
        {
            Debug.Assert(_results != null, "_results != null");

            // Combine the exceptions for the index
            AddException(ref _results[index].exception, exception, index);

            SetCompleted(index);

            void AddException(ref Exception origEx, Exception newEx, int i)
            {
                switch (origEx)
                {
                    case null:
                        origEx = newEx;
                        break;
                    case AggregateException aggEx:
                        origEx = new AggregateException(aggEx.Message, aggEx.InnerExceptions.Append(newEx));
                        break;
                    default:
                        origEx = new AggregateException(
                            $"Multiple exceptions occurred for the command on connection #{i}",
                            origEx,
                            newEx);
                        break;
                }
            }
        }

        /// <summary>
        /// Sets the result to canceled.
        /// </summary>
        /// <param name="index">The index.</param>
        internal override void SetCanceled(int index)
        {
            _cancelled = true;
            SetCompleted(index);
        }

        /// <summary>
        /// Gets the result asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token which can be used to cancel the operation. The batch will continue running.</param>
        /// <returns>An awaitable task.</returns>
        protected override Task GetResultInternalAsync(CancellationToken cancellationToken) 
            => GetResultAsync(cancellationToken);

        /// <summary>
        /// Gets the result asynchronously.
        /// </summary>
        /// <remarks>If the batch was executed against all connections, this will return the result of a single connection.</remarks>
        /// <param name="cancellationToken">A cancellation token which can be used to cancel the operation. The batch will continue running.</param>
        /// <returns>An awaitable task which returns the result.</returns>
        public new async Task<T> GetResultAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            // Execute the command if its not already running
            Command.Batch.BeginExecute(false);

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
        public async Task<IEnumerable<T>> GetResultsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            // Execute the command for all connections if its not already running
            Command.Batch.BeginExecute(true);

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