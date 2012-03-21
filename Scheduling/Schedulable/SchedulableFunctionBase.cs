namespace WebApplications.Utilities.Scheduling.Schedulable
{
    /// <summary>
    /// Base class for scheduled functions.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <remarks></remarks>
    public abstract class SchedulableFunctionBase<T> : ISchedulableFunction<T>
    {
        /// <inheritdoc/>
        public virtual ISchedule Schedule { get; set; }

        /// <inheritdoc/>
        public abstract T Execute();

        /// <inheritdoc/>
        void ISchedulableAction.Execute()
        {
            Execute();
        }
    }
}