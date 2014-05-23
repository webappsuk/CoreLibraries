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
using WebApplications.Utilities.Formatting;
using WebApplications.Utilities.Logging.Interfaces;
using AsyncLock = WebApplications.Utilities.Threading.AsyncLock;

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
        private FormatBuilder _fileNameFormat;

        [NotNull]
        private FormatBuilder _format;

        /// <summary>
        ///   The current log file being written to.
        /// </summary>
        private LogFile _logFile;

        private TimeSpan _maxDuration;
        private long _maxLog;

        [NotNull]
        private FormatBuilder _pathFormat;

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
        /// The default file name format
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static FormatBuilder DefaultFileNameFormat =
            new FormatBuilder(
                "{" + Log.FormatTagApplicationName + "}-{" + Log.FormatTagTimeStamp + ":yyMMddHHmmssffff}",
                true);

        /*
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
            [CanBeNull] string format = null,
            [CanBeNull] string fileNameFormat = null,
            [CanBeNull] string extension = null,
            uint buffer = 65536,
            bool autoFlush = false)
            : this(name, directory, maxLog, maxDuration, validLevels, (FormatBuilder)format, (FormatBuilder)fileNameFormat, extension, buffer, autoFlush)
        {
            Contract.Requires(name != null);
        }
         */

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
            [CanBeNull] FormatBuilder format = null,
            [CanBeNull] FormatBuilder fileNameFormat = null,
            [CanBeNull] string extension = null,
            uint buffer = 65536,
            bool autoFlush = false)
            : base(name, false, true, validLevels)
        {
            Contract.Requires(name != null);
            MaxLog = maxLog;
            MaxDuration = maxDuration == default(TimeSpan) ? TimeSpan.FromDays(1) : maxDuration;
            _directory = directory ?? DefaultDirectory;
            _fileNameFormat = fileNameFormat ?? DefaultFileNameFormat;
            _extension = extension ?? string.Empty;
            if (format != null)
            {
                // If there is a single chunk with no format, children or alignment,
                // Check if it is one of the built in formats.
                FormatChunk[] formatChunks = format.RootChunk.Children.Take(2).ToArray();

                FormatChunk chunk = formatChunks.Length == 1
                    ? formatChunks[0]
                    : null;
                if ((chunk != null) &&
                    !chunk.IsResolved &&
                    !chunk.IsControl &&
                    (chunk.Tag != null) &&
                    (chunk.Alignment == 0) &&
                    (chunk.Format == null) &&
                    (!chunk.Children.Any()))
                {
                    switch (chunk.Tag.ToLowerInvariant())
                    {
                        case "all":
                            format = Log.AllFormat;
                            break;
                        case "verbose":
                            format = Log.VerboseFormat;
                            break;
                        case "xml":
                            format = Log.XMLFormat;
                            break;
                        case "json":
                            format = Log.JSONFormat;
                            break;
                        case "short":
                            format = Log.ShortFormat;
                            break;
                    }
                }
            }
            _format = format ?? Log.VerboseFormat;
            _pathFormat = ValidatePathFormat(_directory, _fileNameFormat, ref _extension, _format);
            Buffer = buffer;
            AutoFlush = autoFlush;
        }

        /// <summary>
        ///   The directory being logged to.
        /// </summary>
        [NotNull]
        [PublicAPI]
        // ReSharper disable once CodeAnnotationAnalyzer
        public string Directory
        {
            get { return _directory; }
            set
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                // ReSharper disable once HeuristicUnreachableCode
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
        // ReSharper disable once CodeAnnotationAnalyzer
        public FormatBuilder FileNameFormat
        {
            get { return _fileNameFormat; }
            set
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                // ReSharper disable once HeuristicUnreachableCode
                if (value == null) value = DefaultFileNameFormat;
                if (_fileNameFormat.ToString() == value) return;
                _pathFormat = ValidatePathFormat(_directory, value, ref _extension, _format);
                _fileNameFormat = value;
                CloseFile();
            }
        }

        /// <summary>
        /// Gets or sets the log format.
        /// </summary>
        /// <value>The format.</value>
        [NotNull]
        [PublicAPI]
        // ReSharper disable once CodeAnnotationAnalyzer
        public FormatBuilder Format
        {
            get { return _format; }
            set
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                // ReSharper disable once HeuristicUnreachableCode
                if (value == null) value = Log.VerboseFormat;
                if (_format.ToString() == value) return;
                _format = value;
                _pathFormat = ValidatePathFormat(_directory, _fileNameFormat, ref _extension, _format);
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
        /// The format tag used to dedupe a file.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public const string FormatTagDedupe = "dedupe";

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
        private static FormatBuilder ValidatePathFormat(
            [NotNull] string directory,
            [NotNull] FormatBuilder fileNameFormat,
            [NotNull] ref string extension,
            [NotNull] FormatBuilder format)
        {
            Contract.Requires(directory != null);
            Contract.Requires(fileNameFormat != null);
            Contract.Requires(extension != null);
            Contract.Requires(format != null);

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

            if (fileNameFormat.IsEmpty)
                // ReSharper disable once AssignNullToNotNullAttribute
                throw new LoggingException(LoggingLevel.Critical, () => Resources.FileLogger_FileNameFormatNotSpecified);


            if (string.IsNullOrWhiteSpace(extension))
                if (format == Log.XMLFormat)
                    extension = ".xml";
                else if (format == Log.JSONFormat)
                    extension = ".json";
                else
                    extension = ".log";
            else
            {
                extension = extension.Trim();
                if (extension.StartsWith("."))
                    extension = extension.Substring(1);
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

            FormatBuilder pathBuilder = new FormatBuilder()
                .Append(directory)
                .Append('\\')
                .AppendFormat(fileNameFormat);

            // Add a dedupe tag if not already present
            // ReSharper disable once PossibleNullReferenceException
            Stack<FormatChunk> formatStack = new Stack<FormatChunk>();
            formatStack.Push(fileNameFormat.RootChunk);
            bool found = false;
            while (formatStack.Count > 0)
            {
                FormatChunk chunk = formatStack.Pop();
                // ReSharper disable once PossibleNullReferenceException
                if (string.Equals(chunk.Tag, FormatTagDedupe, StringComparison.CurrentCultureIgnoreCase))
                {
                    found = true;
                    break;
                }

                foreach (var child in chunk.Children)
                    formatStack.Push(child);
            }

            if (!found)
                pathBuilder.AppendFormat("{dedupe: ({dedupe:D})}");

            return pathBuilder
                .Append(extension)
                .MakeReadOnly();
        }

        /// <summary>
        ///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            CloseFile();
        }

        /// <summary>
        /// The file creation lock prevents multiple threads trying to create the same file.
        /// </summary>
        [NotNull]
        private static readonly AsyncLock _fileCreationLock = new AsyncLock();

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
                    // ReSharper disable once PossibleNullReferenceException
                    (log.TimeStamp - logFile.Start >= MaxDuration))
                {
                    // Close out the last logfile.
#pragma warning disable 4014
                    LogFile lf = logFile;
                    if (lf != null)
                        Task.Run(() => lf.Dispose(), token);
#pragma warning restore 4014

                    int dedupe = 1;
                    using (await _fileCreationLock.LockAsync(token).ConfigureAwait(false))
                    {
                        string fileName;
                        do
                        {
                            fileName = _pathFormat.ToString(
                                (writer, chunk) =>
                                {
                                    // ReSharper disable PossibleNullReferenceException
                                    switch (chunk.Tag.ToLower())
                                    // ReSharper restore PossibleNullReferenceException
                                    {
                                        case Log.FormatTagApplicationName:
                                            return new Resolution(Log.ApplicationName);
                                        case Log.FormatTagApplicationGuid:
                                            return new Resolution(Log.ApplicationGuid);
                                        case Log.FormatTagTimeStamp:
                                            return new Resolution(log.TimeStamp);
                                        case FormatTagDedupe:
                                            return dedupe < 2
                                                ? Resolution.Null
                                                : new Resolution(dedupe);
                                        default:
                                            return Resolution.Unknown;
                                    }
                                });
                            dedupe++;
                            if (File.Exists(fileName)) continue;

                            try
                            {
                                logFile = new LogFile(
                                    File.Create(
                                        fileName,
                                        (int) Buffer,
                                        FileOptions.Asynchronous | FileOptions.SequentialScan),
                                    fileName,
                                    Format,
                                    log.TimeStamp);

                                // ReSharper disable once AssignNullToNotNullAttribute
                                dedupe = -1;
                                TraceTextWriter.Default.WriteLine(Resources.FileLogger_Started_File, fileName, Buffer.ToMemorySize());
                                break;
                            }
                            catch (Exception e)
                            {
                                throw new LoggingException(e, () => Resources.FileLogger_File_Creation_Failed, fileName);
                            }
                        } while (dedupe < 99);
                        if (dedupe > 0)
                            throw new LoggingException(()=>Resources.FileLogger_File_Creation_Retry_Failed);

                        Interlocked.CompareExchange(ref _logFile, logFile, null);
                    }
                }
                await logFile.Write(log, token).ConfigureAwait(false);
            }

            if (AutoFlush && (logFile != null))
                await logFile.FlushAsync(token).ConfigureAwait(false);
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
        /// The log file style.
        /// </summary>
        private enum LogFileStyle
        {
            XML,
            JSON,
            Text
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
            public readonly FormatBuilder Format;

            [PublicAPI]
            public readonly LogFileStyle Style;

            public readonly DateTime Start;

            [NotNull]
            private FileStream _fileStream;

            /// <summary>
            /// Initializes a new instance of the <see cref="LogFile" /> class.
            /// </summary>
            /// <param name="fileStream">The file stream.</param>
            /// <param name="fileName">Name of the file.</param>
            /// <param name="format">The format.</param>
            /// <param name="start">The start.</param>
            public LogFile([NotNull]FileStream fileStream, [NotNull] string fileName, [NotNull] FormatBuilder format, DateTime start)
            {
                Contract.Requires(fileName != null);
                Contract.Requires(format != null);
                Format = format;
                Start = start;

                // Calculate style.
                Style = Format == Log.XMLFormat
                    ? LogFileStyle.XML
                    : (Format == Log.JSONFormat
                        ? LogFileStyle.JSON
                        : LogFileStyle.Text);

                FileName = fileName;
                _fileStream = fileStream;

                switch (Style)
                {
                    case LogFileStyle.XML:
                        WriteLine("<Logs>" + Environment.NewLine + "</Logs>").Wait();
                        break;
                    case LogFileStyle.JSON:
                        WriteLine("[" + Environment.NewLine + "]").Wait();
                        break;
                    default:
                        Write(Format.ToString("F") + Environment.NewLine + Environment.NewLine)
                            .Wait();
                        break;
                }
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
                TraceTextWriter.Default.WriteLine(Resources.FileLogger_Ended_File, FileName);
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

                switch (Style)
                {
                    case LogFileStyle.XML:
                        _fileStream.Seek(_xmlSeekOffset, SeekOrigin.End);
                        await Write(logStr, token).ConfigureAwait(false);
                        await WriteLine("</Logs>", token).ConfigureAwait(false);
                        return;
                    case LogFileStyle.JSON:
                        _fileStream.Seek(_jsonSeekOffset, SeekOrigin.End);
                        if (Logs > 1)
                            await WriteLine(",", token).ConfigureAwait(false);

                        await Write(logStr, token).ConfigureAwait(false);
                        await WriteLine("]", token).ConfigureAwait(false);
                        return;
                    default:
                        await WriteLine(logStr, token).ConfigureAwait(false);
                        return;
                }
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