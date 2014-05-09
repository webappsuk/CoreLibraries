using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using JetBrains.Annotations;
using WebApplications.Utilities.Formatting;

namespace WebApplications.Utilities.Test.Formatting
{
    /// <summary>
    /// Colored text writer that appends special tags to the output when the color is changed.
    /// </summary>
    public class TestColoredTextWriter : TextWriter, IColoredTextWriter
    {
        private readonly bool _writeToTrace;

        [NotNull]
        private readonly StringBuilder _builder = new StringBuilder();

        public TestColoredTextWriter(bool writeToTrace = false)
        {
            _writeToTrace = writeToTrace;
        }

        public override void Write(char value)
        {
            _builder.Append(value);
            Trace.Write(value);
        }

        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }

        public void ResetColors()
        {
            _builder.Append("{reset}");
            if (_writeToTrace)
                Trace.Write("{reset}");
        }

        public void ResetForegroundColor()
        {
            _builder.Append("{/fg}");
            if (_writeToTrace)
                Trace.Write("{/fg}");
        }

        public void SetForegroundColor(Color color)
        {
            string c = color.IsNamedColor ? color.Name : string.Format("#{0:X8}", color.ToArgb());

            _builder.AppendFormat("{{fg:{0}}}", c);
            if (_writeToTrace)
                Trace.Write(string.Format("{{fg:{0}}}", c));
        }

        public void ResetBackgroundColor()
        {
            _builder.Append("{/bg}");
            if (_writeToTrace)
                Trace.Write("{/bg}");
        }

        public void SetBackgroundColor(Color color)
        {
            string c = color.IsNamedColor ? color.Name : string.Format("#{0:X8}", color.ToArgb());

            _builder.AppendFormat("{{bg:{0}}}", c);
            if (_writeToTrace)
                Trace.Write(string.Format("{{bg:{0}}}", c));
        }

        public override string ToString()
        {
            return _builder.ToString();
        }
    }
}