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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using JetBrains.Annotations;
using WebApplications.Utilities.Threading;

namespace WebApplications.Utilities
{
    /// <summary>
    /// Helpful functions for initializing and using a console safely.
    /// </summary>
    /// <remarks>
    /// <para>The <see cref="ConsoleHelper"/> provides a <see cref="SynchronizationContext"/> that ensures all write methods occur in a synchronized fashion.
    /// This prevents colour changes, etc. from being interleaved, so long as you only access the Console through the Helper methods.</para>
    /// <para>The overloads for <see cref="Write(string)"/> and <see cref="WriteLine()"/>, as well as being synchronized, also fallback to their equivalents
    /// in the <see cref="Trace"/> class, if a Console is not actually available.</para>
    /// </remarks>
    [PublicAPI]
    public static class ConsoleHelper
    {
        /// <summary>
        /// Calculates whether we have a console available.
        /// </summary>
        [NotNull]
        private static readonly Lazy<bool> _isConsole = new Lazy<bool>(
            () =>
            {
                if (!Environment.UserInteractive) return false;
                try
                {
                    return Console.CursorLeft >= int.MinValue;
                }
                catch (IOException)
                {
                    // Try to attach to parent process's console window
                    return AttachConsole(0xFFFFFFFF);
                }
            }, LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// The console synchronization context is respected by all write methods in the helper, and should be used anywhere you wish to synchronize writes.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly SynchronizationContext SynchronizationContext =
            new SerializingSynchronizationContext();

        /// <summary>
        /// An empty objects array.
        /// </summary>
        [NotNull]
        private static readonly object[] _emptyArgs = new object[0];

        /// <summary>
        /// The custom colour names.
        /// </summary>
        [NotNull]
        private static readonly Dictionary<string, ConsoleColor> _customColors =
            new Dictionary<string, ConsoleColor>();

        /// <summary>
        /// Whether the current application is running in a console.
        /// </summary>
        [PublicAPI]
        public static bool IsConsole
        {
            get { return _isConsole.Value; }
        }

        /// <summary>
        /// Attaches to a parent console.
        /// </summary>
        /// <param name="dwProcessId">The dw process identifier.</param>
        /// <returns><see langword="true" /> if succeeds, <see langword="false" /> otherwise.</returns>
        [DllImport("kernel32", SetLastError = true)]
        private static extern bool AttachConsole(uint dwProcessId);

        /// <summary>
        /// Shows the window.
        /// </summary>
        /// <param name="hWnd">The window handle.</param>
        /// <param name="cmdShow">The command id.</param>
        /// <returns><see langword="true" /> if successfull, <see langword="false" /> otherwise.</returns>
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int cmdShow);

        /// <summary>
        /// Maximises the console window.
        /// </summary>
        [PublicAPI]
        public static void Maximise()
        {
            if (!IsConsole) return;

            Process p = Process.GetCurrentProcess();
            ShowWindow(p.MainWindowHandle, 3); //SW_MAXIMIZE = 3
        }

        /// <summary>
        /// Sets the custom name of the <see cref="ConsoleColor"/>.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="colour">The colour.</param>
        [PublicAPI]
        public static void SetCustomColourName([NotNull] string name, ConsoleColor colour)
        {
            Contract.Requires(name != null);
            if (!IsConsole) return;
            SynchronizationContext.Invoke(() => _customColors[name] = colour);
        }

        /// <summary>
        /// Tries to get the colour with the custom name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="colour">The colour.</param>
        /// <returns><see langword="true" /> if found, <see langword="false" /> otherwise.</returns>
        [PublicAPI]
        public static bool TryGetCustomColour([NotNull] string name, out ConsoleColor colour)
        {
            Contract.Requires(name != null);
            if (!IsConsole)
            {
                colour = default(ConsoleColor);
                return false;
            }

            ConsoleColor c = default(ConsoleColor);

            bool result = SynchronizationContext.Invoke(() => _customColors.TryGetValue(name, out c));
            colour = c;
            return result;
        }

        /// <summary>
        /// Tries to remove the custom name for the colour.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><see langword="true" /> if removed, <see langword="false" /> otherwise.</returns>
        [PublicAPI]
        public static bool RemoveCustomColour([NotNull] string name)
        {
            Contract.Requires(name != null);
            return IsConsole && SynchronizationContext.Invoke(() => _customColors.Remove(name));
        }

        /// <summary>
        /// Writes a line break, and preventing failure if the console is absent.
        /// </summary>
        [PublicAPI]
        public static void WriteLine()
        {
            Write(string.Empty, true, _emptyArgs);
        }

        /// <summary>
        /// Writes the specified buffer followed by a line break, respecting colouration tags, and preventing failure if the console is absent.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        [PublicAPI]
        public static void WriteLine([CanBeNull] char[] buffer)
        {
            Write(buffer != null ? new string(buffer) : string.Empty, true, _emptyArgs);
        }

        /// <summary>
        /// Writes the specified buffer followed by a line break, respecting colouration tags, and preventing failure if the console is absent.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        [PublicAPI]
        public static void WriteLine([CanBeNull] char[] buffer, int index, int count)
        {
            Write(buffer != null ? new string(buffer, index, count) : string.Empty, true, _emptyArgs);
        }

        /// <summary>
        /// Writes the specified value followed by a line break, respecting colouration tags, and preventing failure if the console is absent.
        /// </summary>
        /// <param name="value">The value.</param>
        [PublicAPI]
        public static void WriteLine([CanBeNull] object value)
        {
            Write(value != null ? value.ToString() : string.Empty, true, _emptyArgs);
        }

        /// <summary>
        /// Writes the specified string followed by a line break, respecting colouration tags, and preventing failure if the console is absent.
        /// </summary>
        /// <param name="value">The value.</param>
        [PublicAPI]
        public static void WriteLine([CanBeNull] string value)
        {
            Write(value ?? string.Empty, true, _emptyArgs);
        }

        /// <summary>
        /// Writes the formatted string followed by a line break, respecting colouration tags, and preventing failure if the console is absent.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="args">The arguments.</param>
        [PublicAPI]
        public static void WriteLine([CanBeNull] string str, [CanBeNull] params object[] args)
        {
            Write(str ?? string.Empty, true, args);
        }

        /// <summary>
        /// Writes the specified buffer, respecting colouration tags, and preventing failure if the console is absent.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        [PublicAPI]
        public static void Write([CanBeNull] char[] buffer)
        {
            if (buffer == null) return;
            Write(new string(buffer), false, _emptyArgs);
        }


        /// <summary>
        /// Writes the specified buffer, respecting colouration tags, and preventing failure if the console is absent.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        [PublicAPI]
        public static void Write([CanBeNull] char[] buffer, int index, int count)
        {
            if (buffer == null) return;
            Write(new string(buffer, index, count), false, _emptyArgs);
        }


        /// <summary>
        /// Writes the specified value, respecting colouration tags, and preventing failure if the console is absent.
        /// </summary>
        /// <param name="value">The value.</param>
        [PublicAPI]
        public static void Write([CanBeNull] object value)
        {
            if (value == null) return;
            Write(value.ToString(), false, _emptyArgs);
        }

        /// <summary>
        /// Writes the specified string, respecting colouration tags, and preventing failure if the console is absent.
        /// </summary>
        /// <param name="value">The value.</param>
        [PublicAPI]
        public static void Write([CanBeNull] string value)
        {
            if (string.IsNullOrEmpty(value)) return;
            Write(value, false, _emptyArgs);
        }

        /// <summary>
        /// Writes the formatted string, respecting colouration tags, and preventing failure if the console is absent.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="args">The arguments.</param>
        /// <returns><see langword="true" /> if colouration tags were found, <see langword="false" /> otherwise.</returns>
        [PublicAPI]
        public static void Write([CanBeNull] string str, [CanBeNull] params object[] args)
        {
            if (string.IsNullOrEmpty(str)) return;
            Write(str, false, args);
        }

        /// <summary>
        /// Writes the specified string on the synchronization context.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="addNewLine">if set to <see langword="true" /> [add new line].</param>
        /// <param name="args">The arguments.</param>
        private static void Write([CanBeNull] string str, bool addNewLine, [CanBeNull] params object[] args)
        {
            if (string.IsNullOrEmpty(str) && !addNewLine) return;
            if (args == null) args = _emptyArgs;

            // Invoke on the synchronization context
            SynchronizationContext.Invoke(() =>
            {
                ConsoleColor currentFore = Console.ForegroundColor;
                ConsoleColor currentBack = Console.BackgroundColor;
                TextWriter writer = IsConsole ? Console.Out : new StringWriter();
                Contract.Assert(writer != null);

                foreach (Tuple<string, string, string> tuple in str.FormatChunks())
                {
                    Contract.Assert(tuple != null);

                    if (string.IsNullOrEmpty(tuple.Item1))
                    {
                        writer.Write(tuple.Item3);
                        continue;
                    }

                    if (args.Length > 0)
                    {
                        int arg;
                        if (int.TryParse(tuple.Item1, out arg))
                        {
                            if ((arg < 0) || (arg >= args.Length))
                            {
                                writer.Write(tuple.Item3);
                                continue;
                            }
                            if (string.IsNullOrEmpty(tuple.Item2))
                            {
                                writer.Write(args[arg]);
                                continue;
                            }

                            Contract.Assert(!string.IsNullOrEmpty(tuple.Item3));
                            try
                            {
                                // ReSharper disable once AssignNullToNotNullAttribute
                                writer.Write(tuple.Item3, args);
                            }
                            catch
                            {
                                writer.Write(tuple.Item3);
                            }
                            continue;
                        }
                    }

                    // ReSharper disable once PossibleNullReferenceException
                    bool back = tuple.Item1[0] == '-';
                    if (!back && (tuple.Item1[0] != '+'))
                    {
                        writer.Write(tuple.Item3);
                        continue;
                    }

                    string cStr = tuple.Item1.Substring(1);
                    ConsoleColor colour;
                    if (cStr == "_")
                    {
                        colour = back ? currentBack : currentFore;
                    }
                    else if (!_customColors.TryGetValue(cStr, out colour) &&
                             !Enum.TryParse(cStr, true, out colour))
                    {
                        writer.Write(tuple.Item3);
                        continue;
                    }

                    // Set the relevant console colour, if we're running in a console.
                    if (!IsConsole) continue;

                    if (back)
                        Console.BackgroundColor = colour;
                    else
                        Console.ForegroundColor = colour;
                }

                // Restore colours
                if (IsConsole)
                {
                    Console.BackgroundColor = currentBack;
                    Console.ForegroundColor = currentFore;
                }

                if (addNewLine)
                    writer.WriteLine();

                if (IsConsole) return;

                // Write the StringWriter out to trace.
                Trace.Write(writer.ToString());
                writer.Dispose();
            });
        }
    }
}