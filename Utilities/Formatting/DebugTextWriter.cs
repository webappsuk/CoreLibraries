using System.Diagnostics;
using System.IO;
using System.Text;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Formatting
{
    /// <summary>
    /// Implements a <see cref="TextWriter"/> for tracing.
    /// </summary>
    public class DebugTextWriter : TextWriter
    {
        /// <summary>
        /// The default
        /// </summary>
        [NotNull]
        public static readonly DebugTextWriter Default = new DebugTextWriter();

        /// <summary>
        /// Prevents a default instance of the <see cref="DebugTextWriter"/> class from being created.
        /// </summary>
        private DebugTextWriter()
        {
        }

        /// <summary>
        /// Gets the encoding.
        /// </summary>
        /// <value>The encoding.</value>
        public override Encoding Encoding
        {
            get { return Encoding.Default; }
        }

#if DEBUG

        /// <summary>
        /// Flushes this instance.
        /// </summary>
        public override void Flush()
        {
            Debug.Flush();
            base.Flush();
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(bool value)
        {
            Debug.Write(value);
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(char value)
        {
            Debug.Write(value);
        }

        /// <summary>
        /// Writes the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        public override void Write(char[] buffer)
        {
            Debug.Write(buffer);
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(decimal value)
        {
            Debug.Write(value);
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(double value)
        {
            Debug.Write(value);
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(float value)
        {
            Debug.Write(value);
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(int value)
        {
            Debug.Write(value);
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(long value)
        {
            Debug.Write(value);
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(object value)
        {
            Debug.Write(value);
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(string value)
        {
            Debug.Write(value);
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(uint value)
        {
            Debug.Write(value);
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(ulong value)
        {
            Debug.Write(value);
        }

        /// <summary>
        /// Writes the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        public override void Write(string format, object arg0)
        {
            Debug.Write(string.Format(format, arg0));
        }

        /// <summary>
        /// Writes the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg">The argument.</param>
        public override void Write(string format, params object[] arg)
        {
            Debug.Write(string.Format(format, arg));
        }

        /// <summary>
        /// Writes the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        public override void Write(char[] buffer, int index, int count)
        {
            string x = new string(buffer, index, count);
            Debug.Write(x);
        }

        /// <summary>
        /// Writes the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        /// <param name="arg1">The arg1.</param>
        public override void Write(string format, object arg0, object arg1)
        {
            Debug.Write(string.Format(format, arg0, arg1));
        }

        /// <summary>
        /// Writes the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        /// <param name="arg1">The arg1.</param>
        /// <param name="arg2">The arg2.</param>
        public override void Write(string format, object arg0, object arg1, object arg2)
        {
            Debug.Write(string.Format(format, arg0, arg1, arg2));
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        public override void WriteLine()
        {
            Debug.WriteLine(string.Empty);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(bool value)
        {
            Debug.WriteLine(value);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(char value)
        {
            Debug.WriteLine(value);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        public override void WriteLine(char[] buffer)
        {
            Debug.WriteLine(buffer);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(decimal value)
        {
            Debug.WriteLine(value);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(double value)
        {
            Debug.WriteLine(value);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(float value)
        {
            Debug.WriteLine(value);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(int value)
        {
            Debug.WriteLine(value);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(long value)
        {
            Debug.WriteLine(value);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(object value)
        {
            Debug.WriteLine(value);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(string value)
        {
            Debug.WriteLine(value);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(uint value)
        {
            Debug.WriteLine(value);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(ulong value)
        {
            Debug.WriteLine(value);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        public override void WriteLine(string format, object arg0)
        {
            Debug.WriteLine(string.Format(format, arg0));
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg">The argument.</param>
        public override void WriteLine(string format, params object[] arg)
        {
            Debug.WriteLine(string.Format(format, arg));
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        public override void WriteLine(char[] buffer, int index, int count)
        {
            string x = new string(buffer, index, count);
            Debug.WriteLine(x);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        /// <param name="arg1">The arg1.</param>
        public override void WriteLine(string format, object arg0, object arg1)
        {
            Debug.WriteLine(string.Format(format, arg0, arg1));
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        /// <param name="arg1">The arg1.</param>
        /// <param name="arg2">The arg2.</param>
        public override void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            Debug.WriteLine(string.Format(format, arg0, arg1, arg2));
        }
#endif
    }
}