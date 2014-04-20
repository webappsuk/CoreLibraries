#region © Copyright Web Applications (UK) Ltd, 2014.  All rights reserved.
// Copyright (c) 2014, Web Applications UK Ltd
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of Web Applications UK Ltd nor the
//       names of its contributors may be used to endorse or promote products
//       derived from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL WEB APPLICATIONS UK LTD BE LIABLE FOR ANY
// DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

using System.Diagnostics;
using System.IO;
using System.Text;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Formatting
{
    /// <summary>
    /// Implements a <see cref="TextWriter"/> for tracing.
    /// </summary>
    [PublicAPI]
    public class TraceTextWriter : TextWriter
    {
        /// <summary>
        /// The default
        /// </summary>
        [NotNull]
        public static readonly TraceTextWriter Default = new TraceTextWriter();

        /// <summary>
        /// Prevents a default instance of the <see cref="TraceTextWriter"/> class from being created.
        /// </summary>
        private TraceTextWriter()
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

        /// <summary>
        /// Flushes this instance.
        /// </summary>
        public override void Flush()
        {
            Trace.Flush();
            base.Flush();
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(bool value)
        {
            Trace.Write(value);
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(char value)
        {
            Trace.Write(value);
        }

        /// <summary>
        /// Writes the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        public override void Write(char[] buffer)
        {
            Trace.Write(buffer);
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(decimal value)
        {
            Trace.Write(value);
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(double value)
        {
            Trace.Write(value);
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(float value)
        {
            Trace.Write(value);
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(int value)
        {
            Trace.Write(value);
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(long value)
        {
            Trace.Write(value);
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(object value)
        {
            Trace.Write(value);
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(string value)
        {
            Trace.Write(value);
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(uint value)
        {
            Trace.Write(value);
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Write(ulong value)
        {
            Trace.Write(value);
        }

        /// <summary>
        /// Writes the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        public override void Write(string format, object arg0)
        {
            Trace.Write(string.Format(format, arg0));
        }

        /// <summary>
        /// Writes the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg">The argument.</param>
        public override void Write(string format, params object[] arg)
        {
            Trace.Write(string.Format(format, arg));
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
            Trace.Write(x);
        }

        /// <summary>
        /// Writes the specified format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        /// <param name="arg1">The arg1.</param>
        public override void Write(string format, object arg0, object arg1)
        {
            Trace.Write(string.Format(format, arg0, arg1));
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
            Trace.Write(string.Format(format, arg0, arg1, arg2));
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        public override void WriteLine()
        {
            Trace.WriteLine(string.Empty);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(bool value)
        {
            Trace.WriteLine(value);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(char value)
        {
            Trace.WriteLine(value);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        public override void WriteLine(char[] buffer)
        {
            Trace.WriteLine(buffer);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(decimal value)
        {
            Trace.WriteLine(value);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(double value)
        {
            Trace.WriteLine(value);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(float value)
        {
            Trace.WriteLine(value);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(int value)
        {
            Trace.WriteLine(value);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(long value)
        {
            Trace.WriteLine(value);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(object value)
        {
            Trace.WriteLine(value);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(string value)
        {
            Trace.WriteLine(value);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(uint value)
        {
            Trace.WriteLine(value);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteLine(ulong value)
        {
            Trace.WriteLine(value);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        public override void WriteLine(string format, object arg0)
        {
            Trace.WriteLine(string.Format(format, arg0));
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg">The argument.</param>
        public override void WriteLine(string format, params object[] arg)
        {
            Trace.WriteLine(string.Format(format, arg));
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
            Trace.WriteLine(x);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="arg0">The arg0.</param>
        /// <param name="arg1">The arg1.</param>
        public override void WriteLine(string format, object arg0, object arg1)
        {
            Trace.WriteLine(string.Format(format, arg0, arg1));
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
            Trace.WriteLine(string.Format(format, arg0, arg1, arg2));
        }
    }
}