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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities
{
    /// <summary>
    /// Helpful functions for initializing and using a console safely.
    /// </summary>
    [PublicAPI]
    public static class ConsoleHelper
    {
        /// <summary>
        /// Whether the current application is running in a console.
        /// </summary>
        [PublicAPI]
        public static readonly bool IsConsole;

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
        /// <returns><see langword="true" /> if successful, <see langword="false" /> otherwise.</returns>
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int cmdShow);

        /// <summary>
        /// Gets the console screen buffer information.
        /// </summary>
        /// <param name="hConsoleOutput">The h console output.</param>
        /// <param name="csbe">The console screen buffer info.</param>
        /// <returns><see langword="true" /> if successful, <see langword="false" /> otherwise.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetConsoleScreenBufferInfoEx(
            IntPtr hConsoleOutput,
            ref ConsoleScreenBufferInfoEx csbe);

        /// <summary>
        /// The handle for the standard output (see WinBase.h).
        /// </summary>
        private const int StdOutputHandle = -11;

        /// <summary>
        /// An invalid handle (see WinBase.h).
        /// </summary>
        private static readonly IntPtr _invalidHandleValue = new IntPtr(-1);

        /// <summary>
        /// Gets the standard handle.
        /// </summary>
        /// <param name="stdHandle">The standard handle.</param>
        /// <returns>IntPtr.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int stdHandle);

        /// <summary>
        /// Invalid characters when reading a password line.
        /// </summary>
        [NotNull]
        private static readonly HashSet<char> _passwordFilter = new HashSet<char>(new[] { '\0', '\x1b', '\t', '\n' });

        /// <summary>
        /// Represents a coordinate
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct Coord
        {
            [PublicAPI]
            public short X;

            [PublicAPI]
            public short Y;
        }

        /// <summary>
        /// Represents a small rectangle.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct SmallRect
        {
            [PublicAPI]
            public short Left;

            [PublicAPI]
            public short Top;

            [PublicAPI]
            public short Right;

            [PublicAPI]
            public short Bottom;
        }

        /// <summary>
        /// Represents an RGB color
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct ColorRef
        {
            [PublicAPI]
            public uint ColorDWORD;

            [PublicAPI]
            public ColorRef(Color color)
            {
                ColorDWORD = color.R + (((uint) color.G) << 8) + (((uint) color.B) << 16);
            }

            [PublicAPI]
            public ColorRef(uint r, uint g, uint b)
            {
                ColorDWORD = r + (g << 8) + (b << 16);
            }

            [PublicAPI]
            public Color GetColor()
            {
                return Color.FromArgb(
                    (int) (0x000000FFU & ColorDWORD),
                    (int) (0x0000FF00U & ColorDWORD) >> 8,
                    (int) (0x00FF0000U & ColorDWORD) >> 16);
            }

            /* For future use, we can technically set the colors and update the console window.
            [PublicAPI]
            public void SetColor(Color color)
            {
                ColorDWORD = color.R + (((uint)color.G) << 8) + (((uint)color.B) << 16);
            }
             */
        }

        /// <summary>
        /// Holds information about the Console's screen buffer.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct ConsoleScreenBufferInfoEx
        {
            [PublicAPI]
            public int BufferSize;

            [PublicAPI]
            public Coord WindowSize;

            [PublicAPI]
            public Coord CursorPosition;

            [PublicAPI]
            public ushort Attributes;

            [PublicAPI]
            public SmallRect Window;

            [PublicAPI]
            public Coord MaximumWindowSize;

            [PublicAPI]
            public ushort PopupAttributes;

            [PublicAPI]
            public bool FullscreenSupported;

            /*
             * Actual color values
             */
            public ColorRef Black;
            public ColorRef DarkBlue;
            public ColorRef DarkGreen;
            public ColorRef DarkCyan;
            public ColorRef DarkRed;
            public ColorRef DarkMagenta;
            public ColorRef DarkYellow;
            public ColorRef Gray;
            public ColorRef DarkGray;
            public ColorRef Blue;
            public ColorRef Green;
            public ColorRef Cyan;
            public ColorRef Red;
            public ColorRef Magenta;
            public ColorRef Yellow;
            public ColorRef White;
        }

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
        /// Initializes static members of the <see cref="ConsoleHelper"/> class.
        /// </summary>
        static ConsoleHelper()
        {
            if (Environment.UserInteractive)
                try
                {
                    IsConsole = Console.CursorLeft >= Int32.MinValue;
                }
                catch (IOException)
                {
                    // Try to attach to parent process's console window
                    IsConsole = AttachConsole(0xFFFFFFFF);
                }
            else
                IsConsole = false;

            if (IsConsole)
                try
                {
                    ConsoleScreenBufferInfoEx csbe = new ConsoleScreenBufferInfoEx();
                    csbe.BufferSize = Marshal.SizeOf(csbe); // 96 = 0x60
                    IntPtr hConsoleOutput = GetStdHandle(StdOutputHandle); // 7
                    if (hConsoleOutput == _invalidHandleValue)
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                    bool brc = GetConsoleScreenBufferInfoEx(hConsoleOutput, ref csbe);
                    if (!brc)
                        throw new Win32Exception(Marshal.GetLastWin32Error());

                    _consoleColors = new Dictionary<ConsoleColor, Color>
                    {
                        {ConsoleColor.Black, csbe.Black.GetColor()},
                        {ConsoleColor.DarkBlue, csbe.DarkBlue.GetColor()},
                        {ConsoleColor.DarkGreen, csbe.DarkGreen.GetColor()},
                        {ConsoleColor.DarkCyan, csbe.DarkCyan.GetColor()},
                        {ConsoleColor.DarkRed, csbe.DarkRed.GetColor()},
                        {ConsoleColor.DarkMagenta, csbe.DarkMagenta.GetColor()},
                        {ConsoleColor.DarkYellow, csbe.DarkYellow.GetColor()},
                        {ConsoleColor.Gray, csbe.Gray.GetColor()},
                        {ConsoleColor.DarkGray, csbe.DarkGray.GetColor()},
                        {ConsoleColor.Blue, csbe.Blue.GetColor()},
                        {ConsoleColor.Green, csbe.Green.GetColor()},
                        {ConsoleColor.Cyan, csbe.Cyan.GetColor()},
                        {ConsoleColor.Red, csbe.Red.GetColor()},
                        {ConsoleColor.Magenta, csbe.Magenta.GetColor()},
                        {ConsoleColor.Yellow, csbe.Yellow.GetColor()},
                        {ConsoleColor.White, csbe.White.GetColor()}
                    };
                }
                catch
                {
                    _consoleColors = _defaultConsoleColors;
                }
            else
                _consoleColors = _defaultConsoleColors;

            // Create reverse lookup
            _consoleColorsReverse = _consoleColors.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
            _consoleColorReverseCache = new ConcurrentDictionary<Color, ConsoleColor>(_consoleColorsReverse);
        }

        /// <summary>
        /// The default console colors
        /// </summary>
        [NotNull]
        private static readonly IReadOnlyDictionary<ConsoleColor, Color> _defaultConsoleColors =
            new Dictionary<ConsoleColor, Color>
            {
                {ConsoleColor.Black, Color.Black},
                {ConsoleColor.DarkBlue, Color.DarkBlue},
                {ConsoleColor.DarkGreen, Color.DarkGreen},
                {ConsoleColor.DarkCyan, Color.DarkCyan},
                {ConsoleColor.DarkRed, Color.DarkRed},
                {ConsoleColor.DarkMagenta, Color.DarkMagenta},
                {ConsoleColor.DarkYellow, Color.DarkGoldenrod},
                {ConsoleColor.Gray, Color.Gray},
                {ConsoleColor.DarkGray, Color.DarkGray},
                {ConsoleColor.Blue, Color.Blue},
                {ConsoleColor.Green, Color.Green},
                {ConsoleColor.Cyan, Color.Cyan},
                {ConsoleColor.Red, Color.Red},
                {ConsoleColor.Magenta, Color.Magenta},
                {ConsoleColor.Yellow, Color.Yellow},
                {ConsoleColor.White, Color.White}
            };

        /// <summary>
        /// The console's actual colours.
        /// </summary>
        [NotNull]
        private static readonly IReadOnlyDictionary<ConsoleColor, Color> _consoleColors;

        /// <summary>
        /// Maps <see cref="Color"/> to <see cref="ConsoleColor"/> for exact matches.
        /// </summary>
        [NotNull]
        private static readonly IReadOnlyDictionary<Color, ConsoleColor> _consoleColorsReverse;

        /// <summary>
        /// The console color cache.
        /// </summary>
        [NotNull]
        private static readonly ConcurrentDictionary<Color, ConsoleColor> _consoleColorReverseCache;

        /// <summary>
        /// Get's the <see cref="Color"/> of a <see cref="ConsoleColor"/>.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>The equivalent <see cref="Color"/>.</returns>
        public static Color ToColor(ConsoleColor color)
        {
            return _consoleColors[color];
        }

        /// <summary>
        /// Converts a <see cref="Color"/> to the nearest <see cref="ConsoleColor"/>.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>The closest <see cref="ConsoleColor"/>.</returns>
        public static ConsoleColor ToConsoleColor(Color color)
        {
            return _consoleColorReverseCache.GetOrAdd(
                color,
                c => _consoleColorsReverse
                    .Select(kvp => new KeyValuePair<ConsoleColor, double>(kvp.Value, Distance(kvp.Key, color)))
                    .MinBy(kvp => kvp.Value)
                    .Key);
        }

        /// <summary>
        /// Gets the square of the euclidean distance between two <see cref="Color">Colors</see>.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>System.Double.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double Distance(Color a, Color b)
        {
            int rd = a.R - b.R;
            int gd = a.G - b.G;
            int bd = a.B - b.B;
            int ad = a.A - b.A;
            return rd * rd +
                   gd * gd +
                   bd * bd +
                   ad * ad;
        }

        /// <summary>
        /// Like System.Console.ReadLine(), only with a mask.
        /// </summary>
        /// <param name="mask">a <c>char</c> representing your choice of console mask</param>
        /// <returns>the string the user typed in</returns>
        [NotNull]
        [PublicAPI]
        public static string ReadPassword(char mask = '*')
        {
            LinkedList<char> pass = new LinkedList<char>();
            char chr;

            while ((chr = Console.ReadKey(true).KeyChar) != '\r')
                switch (chr)
                {
                    case '\b':
                        if (pass.Count > 0)
                        {
                            Console.Write("\b \b");
                            pass.RemoveLast();
                        }
                        break;
                    case '\x7f':
                        int c = pass.Count;
                        pass.Clear();
                        string cb = new string('\b', c);
                        string cs = new string(' ', c);
                        Console.Write(cb + cs + cb);
                        break;
                    default:
                        if (!_passwordFilter.Contains(chr))
                        {
                            pass.AddLast(chr);
                            Console.Write(mask);
                        }
                        break;
                }

            Console.WriteLine();
            return new string(pass.ToArray());
        }
    }
}