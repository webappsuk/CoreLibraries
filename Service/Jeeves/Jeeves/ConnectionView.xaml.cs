using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Timeline;

namespace Jeeves
{
    /// <summary>
    /// Interaction logic for ConnectionView.xaml
    /// </summary>
    public partial class ConnectionView : UserControl
    {
        public ConnectionView()
        {
            InitializeComponent();
        }

        private void TimeLine_OnLoaded(object sender, RoutedEventArgs e)
        {
            TimeLine.FindChildByType<TimelineAnnotationsPanel>()
                .ChildrenOfType<Border>()
                .First()
                .Visibility = Visibility.Collapsed;
        }
    }
}
