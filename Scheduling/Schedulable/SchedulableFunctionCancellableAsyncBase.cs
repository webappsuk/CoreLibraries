using System.Threading;
using System.Threading.Tasks;
using WebApplications.Utilities.Scheduling.ScheduledFunctions;

namespace WebApplications.Utilities.Scheduling.Schedulable
{
    /// <summary>
    /// Base class for cancellable scheduled asynchronous functions.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <remarks></remarks>
    public abstract class SchedulableFunctionCancellableAsyncBase<T> : SchedulableFunctionAsyncBase<T>,
                                                                     ISchedulableFunctionCancellableAsync<T>
    {
        /// <inheritdoc/>
        public abstract Task<T> ExecuteAsync(CancellationToken cancellationToken);

        /// <inheritdoc/>
        public override Task<T> ExecuteAsync()
        {
            return ExecuteAsync(CancellationToken.None);
        }
    }
}