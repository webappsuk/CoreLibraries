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
                new ScheduledFunctionResult<T>(due, started, duration, exception, cancelled, (T) result);
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduledAction"/> class.
        /// </summary>
        /// <param name="scheduler">The scheduler.</param>
        /// <param name="schedule">The schedule.</param>
        /// <param name="action">The action.</param>
        /// <param name="actionInfo">The action info.</param>
        /// <remarks></remarks>
        internal ScheduledFunction([NotNull]IScheduler scheduler, [NotNull]ISchedule schedule, [NotNull]ISchedulableAction action, [NotNull]SchedulableActionInfo actionInfo, int maximumHistory = -1)
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
        public ISchedulableFunction<T> Function { get { return (ISchedulableFunction<T>)Action; } }

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
        protected override Task<ScheduledActionResult> DoExecuteAsync(DateTime due, DateTime started, CancellationToken cancellationToken)
        {
            // Quick cancellation check.
            if (cancellationToken.IsCancellationRequested)
                return
                    Task.FromResult(
                        (ScheduledActionResult)
                        new ScheduledFunctionResult<T>(due, started, TimeSpan.Zero, null, true, default(T)));

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            ISchedulableFunctionCancellableAsync<T> cancellableFunction = Function as ISchedulableFunctionCancellableAsync<T>;
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
                .ContinueWith(t =>
                                  {
                                      Debug.Assert(t != null);
                                      stopwatch.Stop();
                                      return
                                          (ScheduledActionResult)
                                          new ScheduledFunctionResult<T>(due, started, stopwatch.Elapsed, t.Exception,
                                                                         cancellationToken.IsCancellationRequested,
                                                                         t.Status == TaskStatus.RanToCompletion
                                                                             ? t.Result
                                                                             : default(T));
                                  }, TaskContinuationOptions.ExecuteSynchronously);
        }
    }
}