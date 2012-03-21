using System.Threading.Tasks;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Scheduling
{
    /// <summary>
    /// Defines an action that is executed asynchronoulsy on a schedule.
    /// </summary>
    /// <remarks></remarks>
    public interface ISchedulableActionAsync : ISchedulableAction
    {
        /// <summary>
        /// Gets a task that will complete the scheduled action asychronously.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        [NotNull]
        Task ExecuteAsync();
    }


}