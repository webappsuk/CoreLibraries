using System.Threading.Tasks;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Scheduling
{
    /// <summary>
    /// Defines a function that is executed asynchronously on a schedule.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <remarks></remarks>
    public interface ISchedulableFunctionAsync<T> : ISchedulableFunction<T>
    {
        /// <summary>
        /// Gets a task that will complete the scheduled action asychronously.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        [NotNull]
        Task<T> ExecuteAsync();
    }
}