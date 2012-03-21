namespace WebApplications.Utilities.Scheduling.ScheduledFunctions
{
    /// <summary>
    /// Base class for scheduled actions.
    /// </summary>
    /// <remarks></remarks>
    public abstract class SchedulableActionBase : ISchedulableAction
    {
        /// <inheritdoc/>
        public abstract void Execute();
    }
}