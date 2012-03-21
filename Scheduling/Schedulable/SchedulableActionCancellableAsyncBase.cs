using System.Threading;
using System.Threading.Tasks;

namespace WebApplications.Utilities.Scheduling.Schedulable
{
    /// <summary>
    /// Base class for cancellable scheduled asynchronous actions.
    /// </summary>
    /// <remarks></remarks>
    public abstract class SchedulableActionCancellableAsyncBase : SchedulableActionAsyncBase,
                                                                ISchedulableActionCancellableAsync
    {
        /// <inheritdoc/>
        public abstract Task ExecuteAsync(CancellationToken cancellationToken);

        /// <inheritdoc/>
        public override Task ExecuteAsync()
        {
            return ExecuteAsync(CancellationToken.None);
        }
    }
}