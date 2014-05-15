using System;
using System.Windows.Input;

namespace WebApplications.Utilities.Service.Servicer
{
    /// <summary>
    /// Class CommandHandler.
    /// </summary>
    public class CommandHandler<T> : ICommand
    {
        private readonly Action<T> _action;
        private bool _canExecute;

        /// <summary>
        /// Gets a value indicating whether this instance can execute.
        /// </summary>
        /// <value><see langword="true" /> if this instance can execute; otherwise, <see langword="false" />.</value>
        public bool CanExecuteProperty
        {
            get { return _canExecute; }
            private set
            {
                if (_canExecute == value)
                    return;
                _canExecute = value;
                if (CanExecuteChanged != null)
                    CanExecuteChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandler{T}"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="canExecute">if set to <see langword="true" /> the command can execute.</param>
        public CommandHandler(Action<T> action, bool canExecute = true)
        {
            _action = action;
            CanExecuteProperty = canExecute;
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns>true if this command can be executed; otherwise, false.</returns>
        public bool CanExecute(object parameter)
        {
            return CanExecuteProperty;
        }

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        public void Execute(object parameter)
        {
            T p = parameter is T ? (T) parameter : default(T);
            _action(p);
        }
    }
}