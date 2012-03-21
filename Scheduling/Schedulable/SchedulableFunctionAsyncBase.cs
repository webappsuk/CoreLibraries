using System.Threading.Tasks;

namespace WebApplications.Utilities.Scheduling.Schedulable
{
    /// <summary>
    /// Base class for scheduled asynchronous functions.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <remarks></remarks>
    public abstract class SchedulableFunctionAsyncBase<T> : SchedulableFunctionBase<T>, ISchedulableFunctionAsync<T>
    {
        /// <inheritdoc/>
        public abstract Task<T> ExecuteAsync();

        /// <inheritdoc/>
        public override T Execute()
        {
            Task<T> task = ExecuteAsync();
            task.Wait();
            return task.Result;
        }
    }
}