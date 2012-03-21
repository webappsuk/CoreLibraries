using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Scheduling
{
    /// <summary>
    /// Defines a function that is executed asynchronously on a schedule, and support cancellation.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <remarks></remarks>
    public interface ISchedulableFunctionCancellableAsync<T> : ISchedulableFunctionAsync<T>
    {
        /// <summary>
        /// Gets a task that will complete the scheduled action asychronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        [NotNull]
        Task<T> ExecuteAsync(CancellationToken cancellationToken);
    }
}