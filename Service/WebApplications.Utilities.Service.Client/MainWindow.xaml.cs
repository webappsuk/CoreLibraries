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

namespace WebApplications.Utilities.Service.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public LinkedList<string> Commands = new LinkedList<string>();

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

        /// <summary>
        /// Handles the OnMouseDown event of the MainWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void MainWindow_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        /// <summary>
        /// Handles the Click event of the Execute control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Execute_Click(object sender, RoutedEventArgs e)
        {
            string command = CommandLine.Text;
            CommandLine.Text = string.Empty;
            if (string.IsNullOrWhiteSpace(command))
                return;
            Commands.AddLast(command);

            while (Commands.Count > HistorySize)
                Commands.RemoveFirst();

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
