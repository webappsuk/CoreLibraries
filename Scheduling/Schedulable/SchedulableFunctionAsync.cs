using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Scheduling.Schedulable
{
    /// <summary>
    /// Creates an asyncrhonous schedulable function from a <see cref="Func{T}"/>.
    /// </summary>
    /// <remarks></remarks>
    public class SchedulableFunctionAsync<T> : SchedulableFunctionAsyncBase<T>
    {
        /// <summary>
        /// Whether the function needs the task parameter.
        /// </summary>
        private readonly bool _needsTask = true;

        /// <summary>
        /// The function.
        /// </summary>
        [NotNull]
        private readonly Func<object, T> _function;

        /// <summary>
        /// The task state.
        /// </summary>
        [CanBeNull]
        private readonly object _state;

        /// <summary>
        /// The task creation options.
        /// </summary>
        private readonly TaskCreationOptions _creationOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebApplications.Utilities.Scheduling.Schedulable.SchedulableFunctionAsync{T}"/> class.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <remarks></remarks>
        public SchedulableFunctionAsync([NotNull]Func<T> function)
            : this(o => function(), null, TaskCreationOptions.None)
        {
            _needsTask = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebApplications.Utilities.Scheduling.Schedulable.SchedulableFunctionAsync{T}"/> class.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <param name="creationOptions">The creation options.</param>
        /// <remarks></remarks>
        public SchedulableFunctionAsync([NotNull]Func<T> function, TaskCreationOptions creationOptions)
            : this(o => function(), null, creationOptions)
        {
            _needsTask = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebApplications.Utilities.Scheduling.Schedulable.SchedulableFunctionAsync{T}"/> class.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <param name="state">The state.</param>
        /// <param name="creationOptions">The creation options.</param>
        /// <remarks></remarks>
        public SchedulableFunctionAsync([NotNull]Func<object,T> function, [CanBeNull]object state, TaskCreationOptions creationOptions)
        {
            _function = function;
            _state = state;
            _creationOptions = creationOptions;
        }

        /// <inheritdoc/>
        public override Task<T> ExecuteAsync()
        {
            return new Task<T>(_function, _state, _creationOptions);
        }

        /// <inheritdoc/>
        public override T Execute()
        {
            return !_needsTask ? _function(null) : base.Execute();
        }
    }
}