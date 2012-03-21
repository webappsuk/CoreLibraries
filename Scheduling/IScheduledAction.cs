using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Scheduling
{
    /// <summary>
    /// A schedulable action and it's associated schedule.
    /// </summary>
    /// <remarks></remarks>
    public interface IScheduledAction
    {
        /// <summary>
        /// Gets the scheduler.
        /// </summary>
        /// <remarks></remarks>
        [NotNull]
        IScheduler Scheduler { get; }

        /// <summary>
        /// Gets the history.
        /// </summary>
        /// <remarks></remarks>
        [NotNull]
        IEnumerable<IScheduledActionResult> History { get; }
        
        /// <summary>
        /// Gets the schedule.
        /// </summary>
        [NotNull]
        ISchedule Schedule { get; set; }

        /// <summary>
        /// Gets the action.
        /// </summary>
        /// <remarks></remarks>
        [NotNull]
        ISchedulableAction Action { get; }

        /// <summary>
        /// Whether the current scheduled action is enabled.
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the maximum number of history items stored for this <see cref="IScheduledAction"/>.
        /// </summary>
        /// <value>The maximum number of history items.</value>
        /// <remarks>Setting this value to a negative number will mean the action uses the <see cref="IScheduler.DefaultMaximumHistory"/> value.</remarks>
        int MaximumHistory { get; }

        /// <summary>
        /// Whether the action is actually a function.
        /// </summary>
        bool IsFunction { get; }

        /// <summary>
        /// Holds the function's return type if the action is a function.
        /// </summary>
        [CanBeNull]
        Type FunctionReturnType { get; }

        /// <summary>
        /// Whether the action is asynchronous.
        /// </summary>
        bool IsAsynchronous { get; }

        /// <summary>
        /// Whether the action is cancellable.
        /// </summary>
        bool IsCancellable { get; }

        /// <summary>
        /// Executes the scheduled action (or function) synchronously (that is it will block until the action is complete).
        /// </summary>
        /// <returns><see langword="true"/> if the action was executed; otherwise <see langword="false"/>.</returns>
        /// <remarks>
        /// <para>If the scheduled actions <see cref="IScheduledAction.Schedule">schedule</see> <see cref="ScheduleOptions.AllowConcurrent">allows for
        /// concurrent execution</see> then this will always return <see langword="true"/>; otherwise this will only return <see langword="true"/>
        /// in the case the action is not currently running (and therefore can run immediately).</para>
        /// <para>In the default non-concurrent case, then executing the action immediately can effect the next time the action will run (depending on
        /// the <see cref="ISchedule">schedule</see>.</para>
        /// </remarks>
        bool Execute();

        /// <summary>
        /// Executes the scheduled action (or function) asynchronously with cancellation support (if available).
        /// </summary>
        /// <returns>A <see cref="Task"/> that when complete will indicate whether the action was executed.</returns>
        /// <remarks>
        /// <para>If the scheduled actions <see cref="IScheduledAction.Schedule">schedule</see> <see cref="ScheduleOptions.AllowConcurrent">allows for
        /// concurrent execution</see> then this will always return <see langword="true"/>; otherwise this will only return <see langword="true"/>
        /// in the case the action is not currently running (and therefore can run immediately).</para>
        /// <para>In the default non-concurrent case, then executing the action immediately can effect the next time the action will run (depending on
        /// the <see cref="ISchedule">schedule</see>.</para>
        /// </remarks>
        [NotNull]
        Task<bool> ExecuteAsync();

        /// <summary>
        /// Executes the scheduled action (or function) asynchronously with cancellation support (if available).
        /// </summary>
        /// <returns>A <see cref="Task"/> that when complete will indicate whether the action was executed.</returns>
        /// <remarks>
        /// <para>If the scheduled actions <see cref="IScheduledAction.Schedule">schedule</see> <see cref="ScheduleOptions.AllowConcurrent">allows for
        /// concurrent execution</see> then this will always return <see langword="true"/>; otherwise this will only return <see langword="true"/>
        /// in the case the action is not currently running (and therefore can run immediately).</para>
        /// <para>In the default non-concurrent case, then executing the action immediately can effect the next time the action will run (depending on
        /// the <see cref="ISchedule">schedule</see>.</para>
        /// </remarks>
        [NotNull]
        Task<bool> ExecuteAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Gets the next time the action is scheduled to run.
        /// </summary>
        /// <remarks></remarks>
        DateTime NextDue { get; }

        /// <summary>
        /// Gets the time the last execution finished.
        /// </summary>
        /// <remarks>Will return <see cref="DateTime.MinValue"/> if never executed.</remarks>
        DateTime LastExecutionFinished { get; }

        /// <summary>
        /// Gets the execution count.
        /// </summary>
        /// <remarks>Returns count of completed executions (not executions in progress or started).</remarks>
        long ExecutionCount { get; }
    }
}