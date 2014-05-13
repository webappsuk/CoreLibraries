using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shell;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Service.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private Point _startPoint = default(Point);
        
        [NotNull]
        private readonly ViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            // This technically breaks encapsulation, but there's too many useful methods on the textboxes directly.
            _viewModel = (ViewModel)DataContext;
            _viewModel.LogView = LogView;
            _viewModel.CommandsView = CommandView;
        }

        #region PInvoke GetCursorPos
        /*
         * We have to use GetCursorPos rather than Mouse.GetPosition(null); as the later is inaccurate during drag
         * (see http://msdn.microsoft.com/en-us/library/system.windows.input.mouse.getposition(v=vs.110).aspx)
         * Which causes unwanted interaction when trying to mimic drag to restore functionality
         */
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out POINT lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }

            public POINT(Point pt) : this((int)pt.X, (int)pt.Y) { }

            public static implicit operator Point(POINT p)
            {
                return new Point(p.X, p.Y);
            }
        }
        #endregion

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            POINT p;
            _startPoint = GetCursorPos(out p) ? p : default(Point);
            this.DragMove();
        }

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _startPoint = default(Point);
        }

        /// <summary>
        /// Handles the MouseMove event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if ((WindowState != WindowState.Maximized) ||
                (e.LeftButton != MouseButtonState.Pressed) ||
                (_startPoint == default(Point)))
                return;
            // Get the current mouse position
            POINT p;
            Point mousePos = GetCursorPos(out p) ? p : default(Point);
            Vector diff = _startPoint - mousePos;

            if ((Math.Abs(diff.X) < SystemParameters.MinimumHorizontalDragDistance) &&
                (Math.Abs(diff.Y) < SystemParameters.MinimumVerticalDragDistance)) 
                return;
            
            using (Dispatcher.DisableProcessing())
            {
                WindowState = WindowState.Normal;
                Top = mousePos.Y + 15;
                Left = mousePos.X - (Width / 2);
            }

            DragMove();
        }

        /// <summary>
        /// Handles the MouseDoubleClick event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            _startPoint = default(Point);
            switch (WindowState)
            {
                case WindowState.Normal:
                    WindowState = WindowState.Maximized;
                    break;
                case WindowState.Maximized:
                    WindowState = WindowState.Normal;
                    break;
            }
        }

        /// <summary>
        /// Handles the Click event of the MinimizeButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Handles the Click event of the RestoreButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void RestoreButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Normal
                ? WindowState.Maximized
                : WindowState.Normal;
        }

        /// <summary>
        /// Handles the Click event of the Execute control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Execute_Click(object sender, RoutedEventArgs e)
        {
            DoExecute();
        }

        /// <summary>
        /// Handles the KeyUp event of the CommandLine control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        private void CommandLine_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return || e.Key == Key.Enter)
                DoExecute();
        }

        /// <summary>
        /// Executes the current command.
        /// </summary>
        private void DoExecute()
        {
            string command = CommandLine.Text;
            CommandLine.Text = string.Empty;
            if (string.IsNullOrWhiteSpace(command))
                return;

            _viewModel.Execute(command);
        }

        public event PropertyChangedEventHandler PropertyChanged;

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

        /// <summary>
        /// Handles the <see cref="E:CloseCanExecute" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void OnCloseCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// Handles the <see cref="E:CloseExecuted" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void OnCloseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
