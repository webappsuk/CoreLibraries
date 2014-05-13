using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Service.Client
{
    public class ViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private ICommand _toggleShowCommands;
        /// <summary>
        /// Gets the toggle show commands command.
        /// </summary>
        /// <value>The toggle show commands command.</value>
        public ICommand ToggleShowCommands
        {
            get
            {
                return _toggleShowCommands ??
                       (_toggleShowCommands = new CommandHandler(() => this.ShowConmmands = !_showConmmands, true));
            }
        }

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool _showConmmands = true;

        [NotNull]
        private readonly ObservableCollection<string> _history = new ObservableCollection<string>();
        /// <summary>
        /// Gets the command history.
        /// </summary>
        /// <value>The history.</value>
        [NotNull]
        public ObservableCollection<string> History { get { return _history; } }

        /// <summary>
        /// Gets or sets a value indicating whether to show commands.
        /// </summary>
        /// <value><see langword="true" /> to show commands; otherwise, <see langword="false" />.</value>
        public bool ShowConmmands
        {
            get { return _showConmmands; }
            set
            {
                if (value.Equals(_showConmmands)) return;
                _showConmmands = value;
                OnPropertyChanged();
            }
        }
    }
}
