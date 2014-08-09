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

using System.Threading.Tasks;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Scheduling
{
    /// <summary>
    /// Defines a scheduler.
    /// </summary>
    /// <remarks></remarks>
    public interface IScheduler
    {
        /// <summary>
        /// Adds the action to be run on the specified schedule.
        /// </summary>
        /// <param name="schedule">The schedule.</param>
        /// <param name="action">The action.</param>
        /// <param name="maximumHistory">The optional maximum history for this action (use a negative value to indicate <see cref="DefaultMaximumHistory"/> should be used).</param>
        /// <returns></returns>
        /// <remarks></remarks>
        [NotNull]
        IScheduledAction Add([NotNull] ISchedule schedule, [NotNull] ISchedulableAction action, int maximumHistory = -1);

        /// <summary>
        /// Adds the function to be run on the specified schedule.
        /// </summary>
        /// <typeparam name="T">The functions return type.</typeparam>
        /// <param name="schedule">The schedule.</param>
        /// <param name="function">The function.</param>
        /// <param name="maximumHistory">The optional maximum history for this action (use a negative value to indicate <see cref="DefaultMaximumHistory"/> should be used).</param>
        /// <returns></returns>
        /// <remarks>This is a utility overload that ensures consistency of return type with functions.</remarks>
        [NotNull]
        IScheduledFunction<T> Add<T>(
            [NotNull] ISchedule schedule,
            [NotNull] ISchedulableFunction<T> function,
            int maximumHistory = -1);

        /// <summary>
        /// Removes the scheduled action (or function).
        /// </summary>
        /// <param name="scheduledAction">The scheduled action.</param>
        /// <returns><see langword="true"/> if the action was found and successfully removed; otherwise <see langword="false"/>.</returns>
        /// <remarks></remarks>
        bool Remove(IScheduledAction scheduledAction);

        /// <summary>
        /// Executes the specified scheduled action (or function) synchronously (that is it will block until the action is complete).
        /// </summary>
        /// <param name="scheduledAction">The scheduled action.</param>
        /// <returns><see langword="true"/> if the action was executed; otherwise <see langword="false"/>.</returns>
        /// <remarks>
        /// <para>If the scheduled actions <see cref="IScheduledAction.Schedule">schedule</see> <see cref="ScheduleOptions.AllowConcurrent">allows for
        /// concurrent execution</see> then this will always return <see langword="true"/>; otherwise this will only return <see langword="true"/>
        /// in the case the action is not currently running (and therefore can run immediately).</para>
        /// <para>In the default non-concurrent case, then executing the action immediately can effect the next time the action will run (depending on
        /// the <see cref="ISchedule">schedule</see>.</para>
        /// </remarks>
        bool Execute([NotNull] IScheduledAction scheduledAction);

        /// <summary>
        /// Executes the specified scheduled action (or function) asynchronously.
        /// </summary>
        /// <param name="scheduledAction">The scheduled action.</param>
        /// <returns>A <see cref="Task"/> that when complete will indicate whether the action was executed.</returns>
        /// <remarks>
        /// <para>If the scheduled actions <see cref="IScheduledAction.Schedule">schedule</see> <see cref="ScheduleOptions.AllowConcurrent">allows for
        /// concurrent execution</see> then this will always return <see langword="true"/>; otherwise this will only return <see langword="true"/>
        /// in the case the action is not currently running (and therefore can run immediately).</para>
        /// <para>In the default non-concurrent case, then executing the action immediately can effect the next time the action will run (depending on
        /// the <see cref="ISchedule">schedule</see>.</para>
        /// </remarks>
        [NotNull]
        Task<bool> ExecuteAsync([NotNull] IScheduledAction scheduledAction);

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IScheduler"/> is enabled.
        /// </summary>
        /// <value><see langword="true"/> if enabled; otherwise, <see langword="false"/>.</value>
        /// <remarks></remarks>
        bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the default maximum number of history items stored for <see cref="IScheduledAction">Scheduled Actions</see>.
        /// </summary>
        /// <value>The default maximum number of history items.</value>
        /// <remarks>
        /// When adding scheduled items this value will be used by default, modifying this value will not alter schedules that have already been added
        /// as the history length is fixed once created.  The only way to change existing maximum histories is to remove and re-add a schedule.
        /// </remarks>
        int DefaultMaximumHistory { get; set; }
    }
}