#region © Copyright Web Applications (UK) Ltd, 2012.  All rights reserved.
// Copyright (c) 2012, Web Applications UK Ltd
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
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using WebApplications.Utilities.Logging.Configuration;
using WebApplications.Utilities.Logging.Interfaces;

namespace WebApplications.Utilities.Logging.Loggers
{
    /// <summary>
    ///   A logger that implements logging to files.
    /// </summary>
    /// <remarks>
    ///   TODO It should be possible to turn this into a logger that sets <see cref="ILogger.CanRetrieve"/> to <see langword="true"/>.
    ///   That is that supports log retrieval.
    /// </remarks>
    [UsedImplicitly]
    public class FileLogger : LoggerBase
    {
        /// <summary>
        ///   The directory being logged to.
        /// </summary>
        [UsedImplicitly] public readonly string Directory;

        /// <summary>
        ///   The maximum time period that a single log file can cover.
        /// </summary>
        [UsedImplicitly] public readonly TimeSpan MaxDuration;

        /// <summary>
        ///   The maximum number of log items in a single log file.
        /// </summary>
        [UsedImplicitly] public readonly Int64 MaxLog;

        /// <summary>
        ///   The format string used for naming log files.
        /// </summary>
        [NotNull] private readonly string _format;

        /// <summary>
        ///   The number of log entries in the current file
        /// </summary>
        private Int64 _logCount;

        /// <summary>
        ///   The current log file being written to.
        /// </summary>
        private StreamWriter _logFile;

        /// <summary>
        ///   The current log file's filename.
        /// </summary>
        private string _logFileName;

        /// <summary>
        ///   The time that the current log file was created.
        /// </summary>
        private DateTime _startCurrentLog = DateTime.MinValue;

        /// <summary>
        ///   Initializes a new instance of the <see cref="FileLogger"/> class.
        /// </summary>
        /// <param name="name">The filename.</param>
        /// <param name="directory">The directory to log to.</param>
        /// <param name="maxLog">
        ///   <para>The maximum number of log items in a single log file.</para>
        ///   <para>By default this is set to 1,000.</para>
        /// </param>
        /// <param name="maxDuration">The maximum time period that a single log file can cover.</param>
        /// <param name="validLevels">
        ///   <para>The valid log levels.</para>
        ///   <para>By default allows <see cref="LogLevels">all log levels</see>.</para>
        /// </param>
        /// <param name="format">
        ///   <para>The filename format - where {DateTime} is the creation date time.</para>
        ///   <para>By default the format is "{ApplicationName}-{DateTime:yyMMddHHmmssffff}".</para>
        /// </param>
        /// <param name="extension">
        ///   <para>The file extension.</para>
        ///   <para>By default this is set to use "log".</para>
        /// </param>
        /// <exception cref="LoggingException">
        ///   <para><paramref name="maxLog"/> was less than 10, which would result in too many log files to be created.</para>
        ///   <para>-or-</para>
        ///   <para><paramref name="maxDuration"/> was less than 10 seconds, which would result in too many log files to be created.</para>
        ///   <para>-or-</para>
        ///   <para><paramref name="directory"/> was either <see cref="string.IsNullOrWhiteSpace">null or whitespace</see>.</para>
        ///   <para>-or-</para>
        ///   <para>The <paramref name="format"/> string was either <see cref="string.IsNullOrWhiteSpace">null or whitespace</see>.</para>
        ///   <para>-or-</para>
        ///   <para>An error occurred trying to access the <paramref name="directory"/>.</para>
        ///   <para>-or-</para>
        ///   <para><paramref name="extension"/> was more than 5 characters long.</para>
        ///   <para>-or-</para>
        ///   <para>The <paramref name="format"/> led to an invalid path or created a path that references the wrong <paramref name="directory"/>.</para>
        ///   <para>-or-</para>
        ///   <para>File path contained <see cref="Path.GetInvalidPathChars">invalid characters</see>.</para>
        /// </exception>
        public FileLogger(
            [NotNull] string name,
            [NotNull] string directory,
            Int64 maxLog = 1000,
            TimeSpan maxDuration = default(TimeSpan),
            LogLevels validLevels = LogLevels.All,
            string format = "{ApplicationName}-{DateTime:yyMMddHHmmssffff}",
            string extension = "log")
            : base(name, false, validLevels)
        {
            if (maxLog < 10)
                throw new LoggingException(
                    Resources.FileLogger_MaximumLogsLessThanTen, LogLevel.Critical, maxLog);

            if (maxDuration == default(TimeSpan))
                maxDuration = TimeSpan.FromDays(1);
            else if (maxDuration < TimeSpan.FromSeconds(10))
                throw new LoggingException(Resources.FileLogger_FileDurationLessThanTenSeconds, LogLevel.Critical, maxLog);

            MaxLog = maxLog;
            MaxDuration = maxDuration;

            if (string.IsNullOrWhiteSpace(directory))
            {
                throw new LoggingException(Resources.FileLogger_DirectoryNotSpecified, LogLevel.Critical);
            }
            try
            {
                directory = directory.Trim();
                Directory = Path.GetFullPath(directory);
                if (!System.IO.Directory.Exists(Directory))
                    System.IO.Directory.CreateDirectory(Directory);
            }
            catch (Exception e)
            {
                throw new LoggingException(e, Resources.FileLogger_DirectoryAccessOrCreationError,
                    LogLevel.Critical, directory, e.Message);
            }

            if (string.IsNullOrWhiteSpace(format))
                throw new LoggingException(Resources.FileLogger_FileNameFormatNotSpecified, LogLevel.Critical);


            if (string.IsNullOrWhiteSpace(extension))
            {
                extension = string.Empty;
            }
            else
            {
                extension = extension.Trim();
                if (extension.Length > 5)
                    throw new LoggingException(
                        Resources.FileLogger_ExtensionLongerThanFiveCharacters, LogLevel.Critical, extension);
                extension = "." + extension;
            }

            _format = Directory + @"\" +
                        format.Replace("DateTime", "0")
                            .Replace("ApplicationName", "2")
                                .Replace("ApplicationGuid", "3")
                        + "{1}" + extension;

            // Test the format string
            LoggingConfiguration configuration = LoggingConfiguration.Active;
            string testFormat = string.Format(_format, DateTime.Now, Int32.MaxValue, configuration.ApplicationName, configuration.ApplicationGuid);
            if (testFormat.IndexOfAny(Path.GetInvalidPathChars()) > -1)
                throw new LoggingException(Resources.FileLogger_FileNameFormatInvalid, LogLevel.Critical, format);

            try
            {
                testFormat = Path.GetFullPath(testFormat);
            }
            catch (Exception e)
            {
                throw new LoggingException(e, Resources.FileLogger_InvalidPathCreation, LogLevel.Critical, format, e.Message);
            }

            if (!testFormat.StartsWith(directory))
                throw new LoggingException(Resources.FileLogger_PathCreatedOutsideDirectory, LogLevel.Critical, format);
        }

        /// <summary>
        ///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            Flush();
            CloseFile();
        }

        /// <summary>
        ///   Adds the specified log to the log file in time order.
        /// </summary>
        /// <param name="log">The log to write to the file.</param>
        public override void Add(Log log)
        {
            Add(new List<Log> {log});
        }

        /// <summary>
        ///   Adds the specified logs to storage in time order.
        ///   This can be overridden in the default class to bulk store items in one call (e.g. to a database),
        ///   the inbuilt logger will always use this method where possible for efficiency.
        ///   By default it calls the standard Add method repeatedly.
        /// </summary>
        /// <param name="logs">The logs to write the log file.</param>
        public override void Add(IEnumerable<Log> logs)
        {
            // ReSharper disable PossibleMultipleEnumeration
            Log firstLog = logs.FirstOrDefault();
            if (firstLog == null)
                return;

            SetFile(firstLog.TimeStamp);

            foreach (Log log in logs)
                _logFile.WriteLine(log.ToString());

            // ReSharper restore PossibleMultipleEnumeration

            // Flush out the batch.
            _logFile.Flush();
        }

        /// <summary>
        ///   Checks the current log file is still OK to use.
        ///   If OK then the log count for the file is incremented; otherwise a new log file is created and set.
        /// </summary>
        /// <param name="logTimestamp">The timestamp of the last log written to the log file.</param>
        private void SetFile(DateTime logTimestamp)
        {
            // Check if the current file is OK.
            if ((_logFile != null) && (_logCount <= MaxLog) &&
                (logTimestamp - _startCurrentLog < MaxDuration))
            {
                _logCount++;
                return;
            }

            _logCount = 0;
            _startCurrentLog = logTimestamp;
            CloseFile();

            LoggingConfiguration configuration = LoggingConfiguration.Active;
            int dedupe = 1;
            do
            {
                _logFileName = string.Format(_format, DateTime.Now,
                    dedupe > 1
                        ? string.Format(" ({0})", dedupe)
                        : string.Empty,
                    configuration.ApplicationName,
                    configuration.ApplicationGuid);
                dedupe++;
            } while (File.Exists(_logFileName));
            _logFile = new StreamWriter(_logFileName, true);
        }

        /// <summary>
        ///   Flushes the logs to disk.
        /// </summary>
        public override void Flush()
        {
            if (_logFile == null)
                return;
            _logFile.Flush();
        }

        /// <summary>
        ///   Closes the log file.
        /// </summary>
        private void CloseFile()
        {
            if (_logFile == null)
                return;
            _logFile.Flush();
            _logFile.Close();
            _logFile.Dispose();
            _logFile = null;
        }
    }
}