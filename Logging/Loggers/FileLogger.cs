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
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WebApplications.Utilities.Logging.Interfaces;

namespace WebApplications.Utilities.Logging.Loggers
{
    /// <summary>
    ///   A logger that implements logging to files.
    /// </summary>
    /// <remarks>
    ///   TODO It should be possible to turn this into a logger that sets <see cref="ILogger.Queryable"/> to <see langword="true"/>.
    ///   That is that supports log retrieval.
    /// </remarks>
    [PublicAPI]
    public class FileLogger : LoggerBase
    {
        /// <summary>
        /// The default logging directory.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly string DefaultDirectory;

        private uint _buffer;

        [NotNull]
        private string _directory;

        [NotNull]
        private string _extension;

        [NotNull]
        private string _fileNameFormat;

        [NotNull]
        private string _format;

        /// <summary>
        ///   The current log file being written to.
        /// </summary>
        private LogFile _logFile;

        private TimeSpan _maxDuration;
        private long _maxLog;

        [NotNull]
        private string _pathFormat;

        /// <summary>
        /// Initializes static members of the <see cref="FileLogger" /> class.
        /// </summary>
        static FileLogger()
        {
            // Grab the entry point assembly, or this assembly if the process is unmanaged.
            Assembly a = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            DefaultDirectory = Path.GetDirectoryName(a.Location) ?? Path.GetTempPath();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileLogger" /> class.
        /// </summary>
        /// <param name="name">The filename.</param>
        /// <param name="directory">The directory to log to (default to entry assembly directory).</param>
        /// <param name="maxLog"><para>The maximum number of log items in a single log file.</para>
        /// <para>By default this is set to 1,000.</para></param>
        /// <param name="maxDuration">The maximum time period that a single log file can cover.</param>
        /// <param name="validLevels"><para>The valid log levels.</para>
        /// <para>By default allows <see cref="LoggingLevels">all log levels</see>.</para></param>
        /// <param name="format">The log format (default to "Verbose,Xml").</param>
        /// <param name="fileNameFormat"><para>The filename format - where {DateTime} is the creation date time.</para>
        /// <para>By default the format is "{ApplicationName}-{DateTime:yyMMddHHmmssffff}".</para></param>
        /// <param name="extension"><para>The file extension.</para>
        /// <para>By default this is set to use "log".</para></param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="autoFlush">if set to <see langword="true" /> [auto flush].</param>
        /// <exception cref="LoggingException"><para>
        ///   <paramref name="maxLog" /> was less than 10, which would result in too many log files to be created.</para>
        /// <para>-or-</para>
        /// <para>
        ///   <paramref name="maxDuration" /> was less than 10 seconds, which would result in too many log files to be created.</para>
        /// <para>-or-</para>
        /// <para>
        ///   <paramref name="directory" /> was either <see cref="string.IsNullOrWhiteSpace">null or whitespace</see>.</para>
        /// <para>-or-</para>
        /// <para>The <paramref name="fileNameFormat" /> string was either <see cref="string.IsNullOrWhiteSpace">null or whitespace</see>.</para>
        /// <para>-or-</para>
        /// <para>An error occurred trying to access the <paramref name="directory" />.</para>
        /// <para>-or-</para>
        /// <para>
        ///   <paramref name="extension" /> was more than 5 characters long.</para>
        /// <para>-or-</para>
        /// <para>The <paramref name="fileNameFormat" /> led to an invalid path or created a path that references the wrong <paramref name="directory" />.</para>
        /// <para>-or-</para>
        /// <para>File path contained <see cref="Path.GetInvalidPathChars">invalid characters</see>.</para></exception>
        public FileLogger(
            [NotNull] string name,
            [CanBeNull] string directory = null,
            Int64 maxLog = 1000,
            TimeSpan maxDuration = default(TimeSpan),
            LoggingLevels validLevels = LoggingLevels.All,
            [NotNull] string format = "Verbose,Xml",
            [NotNull] string fileNameFormat = "{ApplicationName}-{DateTime:yyMMddHHmmssffff}",
            [CanBeNull] string extension = null,
            uint buffer = 65536,
            bool autoFlush = false)
            : base(name, false, true, validLevels)
        {
            Contract.Requires(name != null);
            Contract.Requires(format != null);
            Contract.Requires(fileNameFormat != null);
            MaxLog = maxLog;
            MaxDuration = maxDuration == default(TimeSpan) ? TimeSpan.FromDays(1) : maxDuration;
            _directory = directory ?? DefaultDirectory;
            _fileNameFormat = fileNameFormat;
            _extension = extension ?? string.Empty;
            _format = format;
            _pathFormat = ValidatePathFormat(_directory, _fileNameFormat, ref _extension, _format);
            Buffer = buffer;
            AutoFlush = autoFlush;
        }


        /// <summary>
        ///   The directory being logged to.
        /// </summary>
        [CanBeNull]
        [PublicAPI]
        public string Directory
        {
            get { return _directory; }
            set
            {
                if (value == null) value = DefaultDirectory;
                if (_directory == value) return;
                _pathFormat = ValidatePathFormat(value, _fileNameFormat, ref _extension, _format);
                _directory = value;
                CloseFile();
            }
        }

        /// <summary>
        ///   The format string used for naming log files.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public string FileNameFormat
        {
            get { return _fileNameFormat; }
            set
            {
                Contract.Requires(value != null);
                if (_fileNameFormat == value) return;
                _pathFormat = ValidatePathFormat(_directory, value, ref _extension, _format);
                _fileNameFormat = value;
                CloseFile();
            }
        }

        /// <summary>
        /// Gets or sets the extension.
        /// </summary>
        /// <value>The extension.</value>
        [PublicAPI]
        [CanBeNull]
        public string Extension
        {
            get { return _extension; }
            set
            {
                if (value == null) value = string.Empty;
                if (_extension == value) return;
                _pathFormat = ValidatePathFormat(_directory, _fileNameFormat, ref value, _format);
                _extension = value;
                CloseFile();
            }
        }

        /// <summary>
        ///   The maximum time period that a single log file can cover.
        /// </summary>
        [PublicAPI]
        public TimeSpan MaxDuration
        {
            get { return _maxDuration; }
            set
            {
                if (_maxDuration == value) return;
                if (value < TimeSpan.FromSeconds(10))
                    throw new LoggingException(
                        LoggingLevel.Critical,
                        // ReSharper disable once AssignNullToNotNullAttribute
                        () => Resources.FileLogger_FileDurationLessThanTenSeconds,
                        value);
                _maxDuration = value;
            }
        }

        /// <summary>
        ///   The maximum number of log items in a single log file.
        /// </summary>
        [PublicAPI]
        public Int64 MaxLog
        {
            get { return _maxLog; }
            set
            {
                if (_maxLog == value)
                    return;

                if (value < 10)
                    throw new LoggingException(
                        LoggingLevel.Critical,
                        // ReSharper disable once AssignNullToNotNullAttribute
                        () => Resources.FileLogger_MaximumLogsLessThanTen,
                        value);
                _maxLog = value;
            }
        }

        /// <summary>
        /// Gets or sets the log format.
        /// </summary>
        /// <value>The format.</value>
        [NotNull]
        [PublicAPI]
        public string Format
        {
            get { return _format; }
            set
            {
                Contract.Requires(value != null);
                if (_format == value) return;
                _format = value;
                _pathFormat = ValidatePathFormat(_directory, _fileNameFormat, ref _extension, _format);
                CloseFile();
            }
        }

        /// <summary>
        /// Gets or sets the buffer size in bytes.
        /// </summary>
        /// <value>The buffer.</value>
        [PublicAPI]
        public uint Buffer
        {
            get { return _buffer; }
            set
            {
                if (_buffer == value) return;
                if (value < 128)
                    throw new LoggingException(
                        LoggingLevel.Critical,
                        // ReSharper disable once AssignNullToNotNullAttribute
                        () => Resources.FileLogger_BufferSize_Too_Small,
                        value.ToMemorySize());
                if (value > int.MaxValue)
                    throw new LoggingException(
                        LoggingLevel.Critical,
// ReSharper disable once AssignNullToNotNullAttribute
                        () => Resources.FileLogger_BufferSize_Too_Big,
                        value.ToMemorySize());

                _buffer = value;
                CloseFile();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to auto flush the file.
        /// </summary>
        /// <value><see langword="true" /> if auto flush; otherwise, <see langword="false" />.</value>
        [PublicAPI]
        public bool AutoFlush { get; set; }

        /// <summary>
        /// Validates the path format.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="fileNameFormat">The format.</param>
        /// <param name="extension">The extension.</param>
        /// <param name="format"></param>
        /// <returns>System.String.</returns>
        /// <exception cref="LoggingException"></exception>
        [NotNull]
        private static string ValidatePathFormat(
            [NotNull] string directory,
            [NotNull] string fileNameFormat,
            [NotNull] ref string extension,
            [NotNull] string format)
        {
            Contract.Requires(directory != null);
            Contract.Requires(fileNameFormat != null);
            Contract.Requires(extension != null);

            if (string.IsNullOrWhiteSpace(directory))
                directory = DefaultDirectory;

            try
            {
                directory = Path.GetFullPath(directory.Trim());
                if (!System.IO.Directory.Exists(directory))
                    System.IO.Directory.CreateDirectory(directory);
            }
            catch (Exception e)
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                throw new LoggingException(
                    e,
                    LoggingLevel.Critical,
                    () => Resources.FileLogger_DirectoryAccessOrCreationError,
                    directory);
            }

            if (string.IsNullOrWhiteSpace(fileNameFormat))
                // ReSharper disable once AssignNullToNotNullAttribute
                throw new LoggingException(LoggingLevel.Critical, () => Resources.FileLogger_FileNameFormatNotSpecified);


            if (string.IsNullOrWhiteSpace(extension))
            {
                bool isXml = false, isJson = false;
                LogFormat logFormat;
                if (Enum.TryParse(format, true, out logFormat))
                    if (logFormat.HasFlag(LogFormat.Xml))
                        isXml = true;
                    else if (logFormat.HasFlag(LogFormat.Json))
                        isJson = true;
                Contract.Assert(!(isXml && isJson));

                if (isXml)
                    extension = ".xml";
                else if (isJson)
                    extension = ".json";
                else
                    extension = ".log";
            }
            else
            {
                extension = extension.Trim();
                if (extension.Any(c => !Char.IsLetterOrDigit(c)))
                    throw new LoggingException(
                        LoggingLevel.Critical,
                        // ReSharper disable once AssignNullToNotNullAttribute
                        () => Resources.FileLogger_Extension_Invalid_Char,
                        extension);
                if (extension.Length > 5)
                    throw new LoggingException(
                        LoggingLevel.Critical,
                        // ReSharper disable once AssignNullToNotNullAttribute
                        () => Resources.FileLogger_ExtensionLongerThanFiveCharacters,
                        extension);
                extension = "." + extension;
            }

            string fullFormat = directory + @"\" +
                                fileNameFormat.Replace("DateTime", "0")
                                    .Replace("ApplicationName", "2")
                                    .Replace("ApplicationGuid", "3")
                                + "{1}" + extension;

            // Test the format string
            string testFormat = string.Format(
                fullFormat,
                DateTime.Now,
                Int32.MaxValue,
                Log.ApplicationName,
                Log.ApplicationGuid);

            if (testFormat.IndexOfAny(Path.GetInvalidPathChars()) > -1)
                // ReSharper disable once AssignNullToNotNullAttribute
                throw new LoggingException(
                    LoggingLevel.Critical,
                    () => Resources.FileLogger_FileNameFormatInvalid,
                    fileNameFormat);

            try
            {
                testFormat = Path.GetFullPath(testFormat);
            }
            catch (Exception e)
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                throw new LoggingException(
                    e,
                    LoggingLevel.Critical,
                    () => Resources.FileLogger_InvalidPathCreation,
                    fileNameFormat);
            }

            if (!testFormat.StartsWith(directory))
                // ReSharper disable once AssignNullToNotNullAttribute
                throw new LoggingException(
                    LoggingLevel.Critical,
                    () => Resources.FileLogger_PathCreatedOutsideDirectory,
                    fileNameFormat);

            return fullFormat;
        }

        /// <summary>
        ///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            CloseFile();
        }

        /// <summary>
        /// Adds the specified logs to storage in batches.
        /// </summary>
        /// <param name="logs">The logs to add to storage.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task.</returns>
        public override async Task Add(
            [InstantHandle] IEnumerable<Log> logs,
            CancellationToken token = default(CancellationToken))
        {
            Contract.Requires(logs != null);
            LogFile logFile = null;
            foreach (Log log in logs)
            {
                Contract.Assert(log != null);

                logFile = _logFile;

                // Check if the current file is OK.
                if ((logFile == null) ||
                    (logFile.Logs > MaxLog) ||
                    (log.TimeStamp - logFile.Start >= MaxDuration))
                {
                    // Close out the last logfile.
#pragma warning disable 4014
                    LogFile lf = logFile;
                    if (lf != null)
                        Task.Run(() => lf.Dispose(), token);
#pragma warning restore 4014

                    int dedupe = 1;
                    string fileName;
                    do
                    {
                        fileName = string.Format(
                            _pathFormat,
                            log.TimeStamp,
                            dedupe > 1
                                ? string.Format(" ({0})", dedupe)
                                : string.Empty,
                            Log.ApplicationName,
                            Log.ApplicationGuid);
                        dedupe++;
                    } while (File.Exists(fileName));

                    logFile = new LogFile(fileName, Format, Buffer, log.TimeStamp);
                    Interlocked.CompareExchange(ref _logFile, logFile, null);
                }
                await logFile.Write(log, token);
            }

            if (AutoFlush && (logFile != null))
                await logFile.FlushAsync(token);
        }

        /// <summary>
        /// Force a flush of this logger.
        /// </summary>
        /// <returns>Task.</returns>
        public override Task Flush(CancellationToken token = default(CancellationToken))
        {
            LogFile lf = _logFile;
            return lf == null ? TaskResult.Completed : lf.FlushAsync(token);
        }

        /// <summary>
        /// Closes the current log file.
        /// </summary>
        [PublicAPI]
        public void CloseFile()
        {
            LogFile logFile = Interlocked.Exchange(ref _logFile, null);
            if (logFile == null) return;
            logFile.Dispose();
        }

        /// <summary>
        /// A log file.
        /// </summary>
        private class LogFile : IDisposable
        {
            /// <summary>
            /// The XML seek offset
            /// </summary>
            private static readonly int _xmlSeekOffset;

            /// <summary>
            /// The JSON seek offset
            /// </summary>
            private static readonly int _jsonSeekOffset;

            static LogFile()
            {
                // Sets how far from the end of the stream to seek when writing out a new log
                _xmlSeekOffset = -Encoding.Unicode.GetByteCount("</Logs>" + Environment.NewLine);
                _jsonSeekOffset = -Encoding.Unicode.GetByteCount("]" + Environment.NewLine);
            }

            /// <summary>
            /// The file name
            /// </summary>
            [NotNull]
            [PublicAPI]
            public readonly string FileName;

            [NotNull]
            [PublicAPI]
            public readonly string Format;

            [PublicAPI]
            public readonly bool IsJson;

            [PublicAPI]
            public readonly bool IsXml;

            public readonly DateTime Start;

            [NotNull]
            private FileStream _fileStream;

            /// <summary>
            /// Initializes a new instance of the <see cref="LogFile" /> class.
            /// </summary>
            /// <param name="fileName">Name of the file.</param>
            /// <param name="format">The format.</param>
            /// <param name="buffer">The buffer.</param>
            /// <param name="start">The start.</param>
            public LogFile([NotNull] string fileName, [NotNull] string format, uint buffer, DateTime start)
            {
                Contract.Requires(fileName != null);
                Contract.Requires(format != null);
                Format = format;
                Start = start;
                int b = (int) buffer;

                // Parse format to see if we're outputting XML / JSON.  Note - this only works for LogFormat formats.
                LogFormat logFormat;
                if (Enum.TryParse(format, true, out logFormat))
                    if (logFormat.HasFlag(LogFormat.Xml))
                        IsXml = true;
                    else if (logFormat.HasFlag(LogFormat.Json))
                        IsJson = true;

                FileName = fileName;
                _fileStream = File.Create(fileName, b, FileOptions.Asynchronous | FileOptions.SequentialScan);

                if (IsXml)
                    WriteLine("<Logs>" + Environment.NewLine + "</Logs>").Wait();
                else if (IsJson)
                    WriteLine("[" + Environment.NewLine + "]").Wait();
                else
                    Write(Format + Environment.NewLine + Environment.NewLine).Wait();

                // ReSharper disable once AssignNullToNotNullAttribute
                Trace.WriteLine(string.Format(Resources.FileLogger_Started_File, FileName, b.ToMemorySize()));
            }

            public int Logs { get; private set; }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                FileStream fileStream = Interlocked.Exchange(ref _fileStream, null);
                if (fileStream == null) return;

                fileStream.Flush();
                fileStream.Dispose();
                // ReSharper disable once AssignNullToNotNullAttribute
                Trace.WriteLine(string.Format(Resources.FileLogger_Ended_File, FileName));
            }

            /// <summary>
            /// Writes the specified log.
            /// </summary>
            /// <param name="log">The log.</param>
            /// <param name="token">The token.</param>
            /// <returns>Task.</returns>
            [NotNull]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public async Task Write([NotNull] Log log, CancellationToken token = default(CancellationToken))
            {
                Contract.Requires(log != null);
                string logStr = log.ToString(Format);
                Logs++;

                if (IsXml)
                {
                    _fileStream.Seek(_xmlSeekOffset, SeekOrigin.End);
                    await Write(logStr, token);
                    await WriteLine("</Logs>", token);
                    return;
                }

                if (IsJson)
                {
                    _fileStream.Seek(_jsonSeekOffset, SeekOrigin.End);
                    if (Logs > 1)
                        await WriteLine(",", token);

                    await Write(logStr, token);
                    await WriteLine("]", token);
                    return;
                }

                await WriteLine(logStr, token);
            }

            /// <summary>
            /// Writes the line.
            /// </summary>
            /// <param name="text">The text.</param>
            /// <param name="token">The token.</param>
            /// <returns>Task.</returns>
            [NotNull]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [PublicAPI]
            public Task WriteLine([NotNull] string text, CancellationToken token = default(CancellationToken))
            {
                Contract.Requires(text != null);
                return Write(text + Environment.NewLine, token);
            }

            /// <summary>
            /// Writes the specified text.
            /// </summary>
            /// <param name="text">The text.</param>
            /// <param name="token">The token.</param>
            /// <returns>Task.</returns>
            [NotNull]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [PublicAPI]
            public Task Write([CanBeNull] string text, CancellationToken token = default(CancellationToken))
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                if (text == null) return TaskResult.Completed;
                byte[] bytes = Encoding.Unicode.GetBytes(text);
                // ReSharper disable once AssignNullToNotNullAttribute
                return _fileStream.WriteAsync(bytes, 0, bytes.Length, token);
            }

            /// <summary>
            /// Flushes the log file.
            /// </summary>
            /// <param name="token">The token.</param>
            /// <returns>Task.</returns>
            [NotNull]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Task FlushAsync(CancellationToken token = default(CancellationToken))
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                return _fileStream.FlushAsync(token);
            }
        }
    }
}