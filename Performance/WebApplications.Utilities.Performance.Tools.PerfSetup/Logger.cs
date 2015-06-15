#region © Copyright Web Applications (UK) Ltd, 2015.  All rights reserved.
// Copyright (c) 2015, Web Applications UK Ltd
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
using JetBrains.Annotations;

namespace WebApplications.Utilities.Performance.Tools.PerfSetup
{
    /// <summary>
    /// Support logging and output of information.
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// The loggers.
        /// </summary>
        [NotNull]
        private static readonly List<Action<string, Level>> _loggers = new List<Action<string, Level>>();

        /// <summary>
        /// Initializes static members of the <see cref="Logger" /> class.
        /// </summary>
        static Logger()
        {
            // Add trace logger
            _loggers.Add((s, l) => Trace.WriteLine(string.Format("{0}: {1}", l.ToString(), s)));
        }

        /// <summary>
        /// Adds a logger.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public static void AddLogger(Action<string, Level> logger)
        {
            _loggers.Add(logger);
        }

        /// <summary>
        /// Adds the specified log message.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="format">The format.</param>
        /// <param name="parameters">The parameters.</param>
        public static void Add(Level level, string format, [NotNull] params object[] parameters)
        {
            if (format == null) return;
            if (parameters == null) throw new ArgumentNullException("parameters");

            string message = null;
            if (parameters.Length < 1)
                message = format;
            else
                try
                {
                    message = string.Format(format, parameters);
                }
                catch (FormatException)
                {
                    message = string.Format("{0}. {1}", message, string.Join(", ", parameters));
                }

            foreach (Action<string, Level> logger in _loggers)
            {
                Debug.Assert(logger != null);
                logger(message, level);
            }
        }
    }
}