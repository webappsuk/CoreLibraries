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
        public static void Add(Level level, string format, params object[] parameters)
        {
            if (format == null) return;
            string message = null;
            if (parameters.Length < 1)
                message = format;
            else
                try
                {
                    message = string.Format(format, parameters);
                }
                catch (FormatException ex)
                {
                    message = string.Format("{0}. {1}", message, string.Join(", ", parameters));
                }

            foreach (var logger in _loggers)
                logger(message, level);
        }
    }
}