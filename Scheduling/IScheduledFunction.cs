using System.Collections.Generic;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Scheduling
{
    /// <summary>
    /// A schedulable function and it's associated schedule.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <remarks></remarks>
    public interface IScheduledFunction<out T> : IScheduledAction
    {
        /// <summary>
        /// Gets the history.
        /// </summary>
        /// <remarks></remarks>
        [NotNull]
        new IEnumerable<IScheduledFunctionResult<T>> History { get; }

        /// <summary>
        /// Gets the function.
        /// </summary>
        /// <remarks></remarks>
        [NotNull]
        ISchedulableFunction<T> Function { get; }
    }
}