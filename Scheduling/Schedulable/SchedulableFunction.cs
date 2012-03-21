using System;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Scheduling.Schedulable
{
    /// <summary>
    /// Creates a schedulable action from a <see cref="Func{T}"/>.
    /// </summary>
    /// <remarks></remarks>
    public class SchedulableFunction<T> : SchedulableFunctionBase<T>
    {
        /// <summary>
        /// The function.
        /// </summary>
        [NotNull]
        private readonly Func<T> _function;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebApplications.Utilities.Scheduling.Schedulable.SchedulableAction"/> class.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <remarks></remarks>
        public SchedulableFunction([NotNull]Func<T> function)
        {
            _function = function;
        }

        /// <inheritdoc/>
        public override T Execute()
        {
            return _function();
        }
    }
}