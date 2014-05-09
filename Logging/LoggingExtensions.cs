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
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Microsoft.SqlServer.Server;
using WebApplications.Utilities.Formatting;

namespace WebApplications.Utilities.Logging
{
    /// <summary>
    ///   Extension methods for logging.
    /// </summary>
    public static class LoggingExtensions
    {
        /// <summary>
        ///   Returns a <see cref="bool"/> value indicating whether the specified
        ///   <see cref="LoggingLevel">log level</see> is within the valid
        ///   <see cref="LoggingLevels">levels</see>.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="validLevels">The valid levels.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the specified <paramref name="level"/> is within the <paramref name="validLevels"/>;
        ///   provided; otherwise returns <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [PublicAPI]
        public static bool IsValid(this LoggingLevel level, LoggingLevels validLevels)
        {
            LoggingLevels l = (LoggingLevels)level;
            return l == (l & validLevels);
        }

        /// <summary>
        /// Returns a <see cref="bool" /> value indicating whether the specified
        /// <see cref="LoggingLevel">log level</see> is within the valid
        /// <see cref="Log.ValidLevels">levels</see>.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns>
        /// Returns <see langword="true" /> if the specified <paramref name="level" /> is within the <see cref="Log.ValidLevels" />;
        /// provided; otherwise returns <see langword="false" />.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [PublicAPI]
        public static bool IsValid(this LoggingLevel level)
        {
            LoggingLevels l = (LoggingLevels)level;
            return l == (l & Log.ValidLevels);
        }

        /* 
         * Console 'safe' colors (actually depends on the configuration of the console)
         * Gray = ffc0c0c0 [Silver]
         * Black = ff000000 [Black]
         * White = ffffffff [White]
         * DarkGray = ff808080 [Gray]
         * Cyan = ff00ffff [Aqua]
         * DarkCyan = ff008080 [Teal]
         * Blue = ff0000ff [Blue]
         * DarkMagenta = ff800080 [Purple]
         * DarkRed = ff800000 [Maroon]
         * Green = ff00ff00 [Lime]
         * DarkYellow = ff808000 [Olive]
         * Red = ffff0000 [Red]
         * DarkBlue = ff000080 [Navy]
         * DarkGreen = ff008000 [Green]
         * Yellow = ffffff00 [Yellow]
         * Magenta = ffff00ff [Fuchsia]
         */

        /// <summary>
        /// The level colors
        /// </summary>
        [NotNull]
        private static readonly ConcurrentDictionary<LoggingLevel, Color> _levelColors =
            new ConcurrentDictionary<LoggingLevel, Color>(
                new Dictionary<LoggingLevel, Color>
                {
                    {LoggingLevel.Debugging, Color.Gray},
                    {LoggingLevel.Information, Color.Silver},
                    {LoggingLevel.Notification, Color.White},
                    {LoggingLevel.SystemNotification, Color.Olive},
                    {LoggingLevel.Warning, Color.Yellow},
                    {LoggingLevel.Error, Color.Red},
                    {LoggingLevel.Critical, Color.Purple},
                    {LoggingLevel.Emergency, Color.Fuchsia}
                });

        /// <summary>
        /// Gets the color of a log level.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns>Color.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [PublicAPI]
        public static Color ToColor(this LoggingLevel level)
        {
            return _levelColors[level];
        }

        /// <summary>
        /// Gets the color of a log level.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns>Color.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [PublicAPI]
        public static void SetColor(this LoggingLevel level, Color color)
        {
            _levelColors[level] = color;
        }
    }
}