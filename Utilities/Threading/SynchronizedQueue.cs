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
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Threading
{
    /// <summary>
    /// Allows queuing of work to run exclusively, or concurrently.
    /// </summary>
    public class SynchronizedQueue
    {
        /// <summary>
        /// The schedule pair.
        /// </summary>
        [NotNull]
        private readonly ConcurrentExclusiveSchedulerPair _pair;

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizedQueue" /> class.
        /// </summary>
        /// <param name="taskScheduler">The target scheduler on which this pair should execute.</param>
        /// <param name="maxConcurrencyLevel">The maximum number of tasks to run concurrently.</param>
        /// <param name="maxItemsPerTask">The maximum number of tasks to process for each underlying scheduled task used by the pair.</param>
        public SynchronizedQueue(
            [CanBeNull] TaskScheduler taskScheduler = null,
            int maxConcurrencyLevel = -1,
            int maxItemsPerTask = -1)
        {
            _pair = new ConcurrentExclusiveSchedulerPair(
                taskScheduler ?? TaskScheduler.Default,
                maxConcurrencyLevel < 1 ? Environment.ProcessorCount : maxConcurrencyLevel,
                maxItemsPerTask);
        }

        /// <summary>
        /// Schedules the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        [NotNull]
        [PublicAPI]
        public Task Do([CanBeNull] Action action, CancellationToken token = default(CancellationToken))
        {
            return action == null
                ? TaskResult.Completed
                : Task.Factory.StartNew(action, token, TaskCreationOptions.PreferFairness, _pair.ExclusiveScheduler);
        }

        /// <summary>
        /// Schedules the specified action to run concurrently.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        [NotNull]
        [PublicAPI]
        public Task DoConcurrent([CanBeNull] Action action, CancellationToken token = default(CancellationToken))
        {
            return action == null
                ? TaskResult.Completed
                : Task.Factory.StartNew(action, token, TaskCreationOptions.PreferFairness, _pair.ExclusiveScheduler);
        }

        /// <summary>
        /// Schedules the specified function.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="function">The function.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        [NotNull]
        [PublicAPI]
        public Task<TResult> Do<TResult>(
            [CanBeNull] Func<TResult> function,
            CancellationToken token = default(CancellationToken))
        {
            return function == null
                ? TaskResult<TResult>.Default
                : Task<TResult>.Factory.StartNew(
                    function,
                    token,
                    TaskCreationOptions.PreferFairness,
                    _pair.ConcurrentScheduler);
        }

        /// <summary>
        /// Schedules the specified action to run concurrently.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="function">The function.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        [NotNull]
        [PublicAPI]
        public Task<TResult> DoConcurrent<TResult>(
            [CanBeNull] Func<TResult> function,
            CancellationToken token = default(CancellationToken))
        {
            return function == null
                ? TaskResult<TResult>.Default
                : Task<TResult>.Factory.StartNew(
                    function,
                    token,
                    TaskCreationOptions.PreferFairness,
                    _pair.ConcurrentScheduler);
        }
    }
}