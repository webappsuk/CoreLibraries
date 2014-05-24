using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jeeves.Models;
using Telerik.Windows.Controls;

namespace Jeeves.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private readonly ObservableCollection<ConnectionViewModel> _connections =
            new ObservableCollection<ConnectionViewModel>();

        public ObservableCollection<ConnectionViewModel> Connections
        {
            get { return _connections; }
        }

        public MainWindowViewModel()
        {
            if (IsInDesignMode)
            {
                _connections.Add(new ConnectionViewModel());
            }
            else
            {
                _connections.Add(new ConnectionViewModel(new Connection("Test Service", @"\\.\pipe\Test_Service")));
            }
        }
    }
}
