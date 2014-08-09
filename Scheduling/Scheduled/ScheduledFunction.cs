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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WebApplications.Utilities.Scheduling.Schedulable;

namespace WebApplications.Utilities.Scheduling.Scheduled
{
    /// <summary>
    /// Base class for scheduled functions.
    /// </summary>
    /// <typeparam name="T">The return type.</typeparam>
    /// <remarks></remarks>
    internal sealed class ScheduledFunction<T> : ScheduledAction, IScheduledFunction<T>
    {
        /// <summary>
        /// Holds constructor for creating a result for this type of function.
        /// </summary>
        [NotNull]
        private static readonly Func<DateTime, DateTime, TimeSpan, Exception, bool, object, ScheduledActionResult>
            _resultCreator =
                (due, started, duration, exception, cancelled, result) =>
                    new ScheduledFunctionResult<T>(due, started, duration, exception, cancelled, (T)result);

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduledAction"/> class.
        /// </summary>
        /// <param name="scheduler">The scheduler.</param>
        /// <param name="schedule">The schedule.</param>
        /// <param name="action">The action.</param>
        /// <param name="actionInfo">The action info.</param>
        /// <remarks></remarks>
        internal ScheduledFunction(
            [NotNull] IScheduler scheduler,
            [NotNull] ISchedule schedule,
            [NotNull] ISchedulableAction action,
            [NotNull] SchedulableActionInfo actionInfo,
            int maximumHistory = -1)
            : base(scheduler, schedule, action, actionInfo, maximumHistory, _resultCreator)
        {
            Debug.Assert(FunctionReturnType == typeof(T));
        }

        /// <inheritdoc/>
        public new IEnumerable<IScheduledFunctionResult<T>> History
        {
            get { return base.History.Cast<IScheduledFunctionResult<T>>(); }
        }

        /// <inheritdoc/>
        public ISchedulableFunction<T> Function
        {
            get { return (ISchedulableFunction<T>)Action; }
        }

        /// <inheritdoc/>
        protected override ScheduledActionResult DoExecute(DateTime due, DateTime started)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Exception exception = null;
            T result;
            try
            {
                // Execute the action!
                result = Function.Execute();
            }
            catch (Exception e)
            {
                exception = e;
                result = default(T);
            }
            stopwatch.Stop();

            return new ScheduledFunctionResult<T>(due, started, stopwatch.Elapsed, exception, false, result);
        }

        /// <inheritdoc/>
        protected override Task<ScheduledActionResult> DoExecuteAsync(
            DateTime due,
            DateTime started,
            CancellationToken cancellationToken)
        {
            // Quick cancellation check.
            if (cancellationToken.IsCancellationRequested)
            {
                return
                    Task.FromResult(
                        (ScheduledActionResult)
                            new ScheduledFunctionResult<T>(due, started, TimeSpan.Zero, null, true, default(T)));
            }

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            ISchedulableFunctionCancellableAsync<T> cancellableFunction =
                Function as ISchedulableFunctionCancellableAsync<T>;
            Task<T> functionTask;
            if (cancellableFunction != null)
                functionTask = cancellableFunction.ExecuteAsync(cancellationToken);
            else
            {
                ISchedulableFunctionAsync<T> asyncFunction = Function as ISchedulableFunctionAsync<T>;

                // Functions should be picked up in override of this method, so this should always be true.
                Debug.Assert(asyncFunction != null);
                functionTask = asyncFunction.ExecuteAsync();
            }

            // Add task continuation.
            return functionTask
                .ContinueWith(
                    t =>
                    {
                        Debug.Assert(t != null);
                        stopwatch.Stop();
                        return
                            (ScheduledActionResult)
                                new ScheduledFunctionResult<T>(
                                    due,
                                    started,
                                    stopwatch.Elapsed,
                                    t.Exception,
                                    cancellationToken.IsCancellationRequested,
                                    t.Status == TaskStatus.RanToCompletion
                                        ? t.Result
                                        : default(T));
                    },
                    TaskContinuationOptions.ExecuteSynchronously);
        }
    }
}