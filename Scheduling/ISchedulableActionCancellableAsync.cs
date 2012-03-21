using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Scheduling
{
    /// <summary>
    /// Defines an action that is executed asynchronously on a schedule, and support cancellation.
    /// </summary>
    /// <remarks></remarks>
    public interface ISchedulableActionCancellableAsync : ISchedulableActionAsync
    {
        /// <summary>
        /// Gets a task that will complete the scheduled action asychronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        [NotNull]
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}