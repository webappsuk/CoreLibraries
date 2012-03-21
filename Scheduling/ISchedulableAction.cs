using JetBrains.Annotations;

namespace WebApplications.Utilities.Scheduling
{
    /// <summary>
    /// Defines an action that is executed on a schedule.
    /// </summary>
    /// <remarks></remarks>
    public interface ISchedulableAction
    {
        /// <summary>
        /// Executes the action synchronously.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        void Execute();
    }
}