using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Scheduling.Schedulable
{
    /// <summary>
    /// Creates an asyncrhonous schedulable action from an <see cref="Action"/>.
    /// </summary>
    /// <remarks></remarks>
    public class SchedulableActionAsync : SchedulableActionAsyncBase
    {
        /// <summary>
        /// Whether the action needs the task parameter.
        /// </summary>
        private readonly bool _needsTask = true;

        /// <summary>
        /// The action.
        /// </summary>
        [NotNull]
        private readonly Action<object> _action;

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
        /// Initializes a new instance of the <see cref="SchedulableActionAsync"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <remarks></remarks>
        public SchedulableActionAsync([NotNull]Action action)
            : this(o => action(), null, TaskCreationOptions.None)
        {
            _needsTask = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulableActionAsync"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="creationOptions">The creation options.</param>
        /// <remarks></remarks>
        public SchedulableActionAsync([NotNull]Action action, TaskCreationOptions creationOptions)
            :this(o => action(), null, creationOptions)
        {
            _needsTask = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulableActionAsync"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="state">The state.</param>
        /// <param name="creationOptions">The creation options.</param>
        /// <remarks></remarks>
        public SchedulableActionAsync([NotNull]Action<object> action, [CanBeNull]object state, TaskCreationOptions creationOptions)
        {
            _action = action;
            _state = state;
            _creationOptions = creationOptions;
        }

        /// <inheritdoc/>
        public override Task ExecuteAsync()
        {
            return new Task(_action, _state, _creationOptions);
        }

        /// <inheritdoc/>
        public override void Execute()
        {
            if (!_needsTask)
                _action(null);
            else
                base.Execute();
        }
    }
}