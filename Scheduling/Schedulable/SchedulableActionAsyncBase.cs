using System.Threading.Tasks;
using WebApplications.Utilities.Scheduling.ScheduledFunctions;

namespace WebApplications.Utilities.Scheduling.Schedulable
{
    /// <summary>
    /// Base class for scheduled asynchronous actions.
    /// </summary>
    /// <remarks></remarks>
    public abstract class SchedulableActionAsyncBase : SchedulableActionBase, ISchedulableActionAsync
    {
        /// <inheritdoc/>
        public abstract Task ExecuteAsync();

        /// <inheritdoc/>
        public override void Execute()
        {
            ExecuteAsync().Wait();
        }
    }
}