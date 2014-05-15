using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Service.Servicer
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
                       (_toggleShowCommands = new CommandHandler<object>(_ => this.ShowCommands = !_showCommands, true));
            }
        }

        /// <summary>
        /// Gets or sets the color of the highlight.
        /// </summary>
        /// <value>The color of the highlight.</value>
        public SolidColorBrush HighlightColor
        {
            get { return _highlightColor; }
            set
            {
                if (Equals(value, _highlightColor)) return;
                _highlightColor = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is connected.
        /// </summary>
        /// <value><see langword="true" /> if this instance is connected; otherwise, <see langword="false" />.</value>
        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                if (value.Equals(_isConnected)) return;
                _isConnected = value;
                OnPropertyChanged();
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

        private bool _showCommands = true;

        [NotNull]
        private readonly ObservableCollection<string> _commandHistory = new ObservableCollection<string>();

        private int _bufferSize = 500;
        private int _historySize = 50;
        public readonly SolidColorBrush DefaultHighlight;
        public readonly SolidColorBrush LogsHighlight;
        public readonly SolidColorBrush CommandsHighlight;
        private SolidColorBrush _highlightColor;
        private bool _isConnected;


        public ViewModel()
        {
            DefaultHighlight = (SolidColorBrush)App.Current.Resources["DefaultHighlight"];
            LogsHighlight = (SolidColorBrush)App.Current.Resources["LogsHighlight"];
            CommandsHighlight = (SolidColorBrush)App.Current.Resources["CommandsHighlight"];
            _highlightColor = DefaultHighlight;
        }

        /// <summary>
        /// Gets the command history.
        /// </summary>
        /// <value>The history.</value>
        [NotNull]
        public ObservableCollection<string> CommandHistory { get { return _commandHistory; } }

        /// <summary>
        /// Gets or sets the log view.
        /// </summary>
        /// <value>The log view.</value>
        [NotNull]
        public RichTextBox LogView { get; set; }

        /// <summary>
        /// Gets or sets the commands view.
        /// </summary>
        /// <value>The commands view.</value>
        [NotNull]
        public RichTextBox CommandsView { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show commands.
        /// </summary>
        /// <value><see langword="true" /> to show commands; otherwise, <see langword="false" />.</value>
        public bool ShowCommands
        {
            get { return _showCommands; }
            set
            {
                if (value.Equals(_showCommands)) return;
                _showCommands = value;
                OnPropertyChanged();
            }
        }

        public int BufferSize
        {
            get { return _bufferSize; }
            set
            {
                if (value == _bufferSize) return;
                _bufferSize = value;
                OnPropertyChanged();
            }
        }

        public int HistorySize
        {
            get { return _historySize; }
            set
            {
                if (value == _historySize) return;
                _historySize = value;
                OnPropertyChanged();
            }
        }

        public void Execute(string command)
        {
            int c = 0;
            // Remove duplicate
            while (c < _commandHistory.Count)
            {
                if (string.Equals(_commandHistory[c], command))
                {
                    _commandHistory.RemoveAt(c);
                    break;
                }
                c++;
            }
            _commandHistory.Insert(0, command);
            while (_commandHistory.Count > _historySize)
                _commandHistory.RemoveAt(_commandHistory.Count - 1);

            AppendCommand(command);
            // TODO Execute the command
        }

        #region
        /// <summary>
        /// Adds the paragraph.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        public void AppendCommand(string command)
        {
            AppendLine(CommandsView);
            AppendText(CommandsView, "Executed: ", Colors.Yellow, FontStyles.Italic);
            AppendText(CommandsView, command, Colors.White, FontStyles.Italic);
            AppendLine(CommandsView);
        }

        /// <summary>
        /// Appends the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="color">The color.</param>
        public void AppendText(RichTextBox richTextBox, string text)
        {
            richTextBox.AppendText(text);
        }

        /// <summary>
        /// Appends the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="color">The color.</param>
        public void AppendText(RichTextBox richTextBox, string text, Color color)
        {
            TextRange range = new TextRange(richTextBox.Document.ContentEnd, richTextBox.Document.ContentEnd) { Text = text };
            range.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(color));
        }

        /// <summary>
        /// Appends the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="color">The color.</param>
        /// <param name="fontStyle">The font style.</param>
        public void AppendText(RichTextBox richTextBox, string text, Color color, FontStyle fontStyle)
        {
            TextRange range = new TextRange(richTextBox.Document.ContentEnd, richTextBox.Document.ContentEnd) { Text = text };
            range.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(color));
            range.ApplyPropertyValue(TextElement.FontStyleProperty, fontStyle);
        }

        /// <summary>
        /// Appends the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="fontStyle">The font style.</param>
        public void AppendText(RichTextBox richTextBox, string text, FontStyle fontStyle)
        {
            TextRange range = new TextRange(richTextBox.Document.ContentEnd, richTextBox.Document.ContentEnd) { Text = text };
            range.ApplyPropertyValue(TextElement.FontStyleProperty, fontStyle);
        }

        /// <summary>
        /// Appends the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="color">The color.</param>
        /// <param name="fontWeight">The font weight.</param>
        public void AppendText(RichTextBox richTextBox, string text, Color color, FontWeight fontWeight)
        {
            TextRange range = new TextRange(richTextBox.Document.ContentEnd, richTextBox.Document.ContentEnd) { Text = text };
            range.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(color));
            range.ApplyPropertyValue(TextElement.FontWeightProperty, fontWeight);
        }

        /// <summary>
        /// Appends the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="fontWeight">The font weight.</param>
        public void AppendText(RichTextBox richTextBox, string text, FontWeight fontWeight)
        {
            TextRange range = new TextRange(richTextBox.Document.ContentEnd, richTextBox.Document.ContentEnd) { Text = text };
            range.ApplyPropertyValue(TextElement.FontWeightProperty, fontWeight);
        }

        /// <summary>
        /// Appends the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="color">The color.</param>
        /// <param name="fontStyle">The font style.</param>
        /// <param name="fontWeight">The font weight.</param>
        public void AppendText(RichTextBox richTextBox, string text, Color color, FontStyle fontStyle, FontWeight fontWeight)
        {
            TextRange range = new TextRange(richTextBox.Document.ContentEnd, richTextBox.Document.ContentEnd) { Text = text };
            range.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(color));
            range.ApplyPropertyValue(TextElement.FontStyleProperty, fontStyle);
            range.ApplyPropertyValue(TextElement.FontWeightProperty, fontWeight);
        }

        /// <summary>
        /// Appends the line.
        /// </summary>
        public void AppendLine(RichTextBox richTextBox)
        {
            richTextBox.AppendText(Environment.NewLine);
            BlockCollection blocks = LogView.Document.Blocks;
            while (blocks.Count > BufferSize)
                blocks.Remove(blocks.FirstBlock);
        }
        #endregion
    }
}
