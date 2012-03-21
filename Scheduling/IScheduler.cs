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
        IScheduledAction Add([NotNull]ISchedule schedule, [NotNull]ISchedulableAction action, int maximumHistory = -1);

        /// <summary>
        /// Adds the function to be run on the specified schedule.
        /// </summary>
        /// <typeparam name="T">The functions return type.</typeparam>
        /// <param name="schedule">The schedule.</param>
        /// <param name="function">The function.</param>
        /// <param name="maximumHistory">The optional maximum history for this action (use a negative value to indicate <see cref="DefaultMaximumHistory"/> should be used).</param>
        /// <returns></returns>
        /// <remarks>This is a utility overload that ensures consistency of return type with functions.</remarks>
        IScheduledFunction<T> Add<T>([NotNull]ISchedule schedule, [NotNull]ISchedulableFunction<T> function, int maximumHistory = -1);

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
        bool Execute([NotNull]IScheduledAction scheduledAction);

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
        Task<bool> ExecuteAsync([NotNull]IScheduledAction scheduledAction);

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