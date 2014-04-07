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
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Logging.Loggers
{
    /// <summary>
    ///   Allows coloured logging to the console window, using an extended <see cref="Format"/> syntax.
    /// </summary>
    [PublicAPI]
    public sealed class ConsoleLogger : LoggerBase
    {
        /// <summary>
        /// The default format.
        /// </summary>
        [NotNull]
        public const string DefaultFormat = "{+Cyan}[{TimeStamp}] {+?}{Level}{+White}\t{Message}{+Gray}{StackTrace}\r\n";

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleLogger"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="format">The format (see <see cref="Format"/> for more information on usage).</param>
        /// <param name="validLevels">The valid levels.</param>
        public ConsoleLogger(
            [NotNull] string name,
            [NotNull] string format = DefaultFormat,
            LoggingLevels validLevels = LoggingLevels.All)
            : base(name, false, false, validLevels)
        {
            Contract.Requires(name != null);
            Contract.Requires(format != null);
            Format = format;
        }

        /// <summary>
        /// Gets or sets the format for trace messages.
        /// </summary>
        /// <value>The format.</value>
        /// <remarks>
        /// <para>Along with the <see cref="Log.ToString(string)">standard formats supported by the logger</see>, also supports colouration using colour formats.</para>
        /// <para>To change colour use a '+' or '-' followed by a <see cref="ConsoleColor"/> e.g. <code>{+White}</code>. '+' indicates a foreground colour change, whilst
        /// '-' will change the background colour.</para>
        /// <para>The '?' colour will pick a colour based on the current log level, e.g. <code>{+?}</code> for an <see cref="LoggingLevel.Error">error</see> will
        /// change the foreground colour to red.</para>
        /// <para>The '_' colour will use the current consoles default foreground or background colour (depending on whether it is preceeded with '+' or '-').</para>
        /// </remarks>
        [PublicAPI]
        [NotNull]
        public string Format { get; set; }

        /// <summary>
        /// Adds the specified logs to storage in batches.
        /// </summary>
        /// <param name="logs">The logs to add to storage.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task.</returns>
        public override async Task Add(IEnumerable<Log> logs, CancellationToken token = default(CancellationToken))
        {
            Contract.Requires(logs != null);
            // Check we're actually in a console!
            if (!ConsoleHelper.IsConsole) return;

            // Write out the logs on the synchronization context which will prevent 'interleaving'.
            await ConsoleHelper.SynchronizationContext;

            string format = Format;
            // ReSharper disable once PossibleNullReferenceException
            foreach (Log log in logs.Where(log => log.Level.IsValid(ValidLevels)))
            {
                token.ThrowIfCancellationRequested();
                // ReSharper disable once PossibleNullReferenceException
                ConsoleHelper.SetCustomColourName("?", LevelColour(log.Level));
                ConsoleHelper.WriteLine(log.ToString(format));
            }
            ConsoleHelper.RemoveCustomColour("?");
        }

        /// <summary>
        /// Gets the default colour for a given logging level.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns>ConsoleColor.</returns>
        private static ConsoleColor LevelColour(LoggingLevel level)
        {
            switch (level)
            {
                case LoggingLevel.Emergency:
                case LoggingLevel.Critical:
                case LoggingLevel.Error:
                    return ConsoleColor.Red;
                case LoggingLevel.Warning:
                case LoggingLevel.SystemNotification:
                case LoggingLevel.Notification:
                    return ConsoleColor.Yellow;
                case LoggingLevel.Debugging:
                    return ConsoleColor.Gray;
                default:
                    return ConsoleColor.White;
            }
        }
    }
}