using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
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
using JetBrains.Annotations;

namespace WebApplications.Utilities.Service.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [NotNull]
        private readonly ObservableCollection<string> _history = new ObservableCollection<string>();

        private Point _startPoint = default(Point);

        /// <summary>
        /// Gets the command history.
        /// </summary>
        /// <value>The history.</value>
        [NotNull]
        public ObservableCollection<string> History { get { return _history; } }

        public const int BufferSize = 500;
        public const int HistorySize = 50;

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Click event of the Close control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
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
            int c = 0;
            // Remove duplicate
            while (c < _history.Count)
            {
                if (string.Equals(_history[c], command))
                {
                    _history.RemoveAt(c);
                    break;
                }
                c++;
            }
            _history.Add(command);
            while (_history.Count > HistorySize)
                _history.RemoveAt(0);

            AppendCommand(command);
            // TODO Execute the command
        }

        /// <summary>
        /// Adds the paragraph.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        public void AppendCommand(string command)
        {
            AppendLine();
            AppendText("Executed: ", Colors.Yellow, FontStyles.Italic);
            AppendText(command, Colors.White, FontStyles.Italic);
            AppendLine();
        }

        /// <summary>
        /// Appends the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="color">The color.</param>
        public void AppendText(string text)
        {
            LogView.AppendText(text);
        }

        /// <summary>
        /// Appends the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="color">The color.</param>
        public void AppendText(string text, Color color)
        {
            TextRange range = new TextRange(LogView.Document.ContentEnd, LogView.Document.ContentEnd) { Text = text };
            range.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(color));
        }

        /// <summary>
        /// Appends the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="color">The color.</param>
        /// <param name="fontStyle">The font style.</param>
        public void AppendText(string text, Color color, FontStyle fontStyle)
        {
            TextRange range = new TextRange(LogView.Document.ContentEnd, LogView.Document.ContentEnd) { Text = text };
            range.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(color));
            range.ApplyPropertyValue(TextElement.FontStyleProperty, fontStyle);
        }

        /// <summary>
        /// Appends the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="fontStyle">The font style.</param>
        public void AppendText(string text, FontStyle fontStyle)
        {
            TextRange range = new TextRange(LogView.Document.ContentEnd, LogView.Document.ContentEnd) { Text = text };
            range.ApplyPropertyValue(TextElement.FontStyleProperty, fontStyle);
        }

        /// <summary>
        /// Appends the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="color">The color.</param>
        /// <param name="fontWeight">The font weight.</param>
        public void AppendText(string text, Color color, FontWeight fontWeight)
        {
            TextRange range = new TextRange(LogView.Document.ContentEnd, LogView.Document.ContentEnd) { Text = text };
            range.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(color));
            range.ApplyPropertyValue(TextElement.FontWeightProperty, fontWeight);
        }

        /// <summary>
        /// Appends the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="fontWeight">The font weight.</param>
        public void AppendText(string text, FontWeight fontWeight)
        {
            TextRange range = new TextRange(LogView.Document.ContentEnd, LogView.Document.ContentEnd) { Text = text };
            range.ApplyPropertyValue(TextElement.FontWeightProperty, fontWeight);
        }

        /// <summary>
        /// Appends the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="color">The color.</param>
        /// <param name="fontStyle">The font style.</param>
        /// <param name="fontWeight">The font weight.</param>
        public void AppendText(string text, Color color, FontStyle fontStyle, FontWeight fontWeight)
        {
            TextRange range = new TextRange(LogView.Document.ContentEnd, LogView.Document.ContentEnd) {Text = text};
            range.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(color));
            range.ApplyPropertyValue(TextElement.FontStyleProperty, fontStyle);
            range.ApplyPropertyValue(TextElement.FontWeightProperty, fontWeight);
        }

        /// <summary>
        /// Appends the line.
        /// </summary>
        public void AppendLine()
        {
            LogView.AppendText(Environment.NewLine);
            BlockCollection blocks = LogView.Document.Blocks;
            while (blocks.Count > BufferSize)
                blocks.Remove(blocks.FirstBlock);
        }
    }
}
