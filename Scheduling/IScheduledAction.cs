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