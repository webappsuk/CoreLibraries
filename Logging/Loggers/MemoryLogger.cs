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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Joins;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WebApplications.Utilities.Caching;
using WebApplications.Utilities.Logging.Interfaces;

namespace WebApplications.Utilities.Logging.Loggers
{
    /// <summary>
    /// A logger that stores logs solely in memory.
    /// Is used by the core logger to hold log items temporarily, and for caching.
    /// </summary>
    [UsedImplicitly]
    internal class MemoryLogger : LoggerBase
    {
        /// <summary>
        ///   The length of time log items are cached for.
        ///   <see cref="TimeSpan.Zero"/> is used for infinity.
        /// </summary>
        public TimeSpan CacheExpiry
        {
            get { return _cacheExpiry; }
            set
            {
                if (_cacheExpiry == value) return;

                if (value == default(TimeSpan))
                    value = TimeSpan.MaxValue;
                else if (value < TimeSpan.FromSeconds(10))
                    throw new LoggingException(Resources.MemoryLogger_CacheExpiryLessThanTenSeconds,
                        LoggingLevel.Critical, value);

                _cacheExpiry = value;
                Clean();
            }
        }

        /// <summary>
        /// The oldest queue item.
        /// </summary>
        private DateTime _oldest = DateTime.MaxValue;

        /// <summary>
        ///   The maximum number of log entries to store.
        /// </summary>
        public int MaximumLogEntries
        {
            get { return _maximumLogEntries; }
            set
            {
                if (_maximumLogEntries == value) return;

                if (value < 1)
                    throw new LoggingException(Resources.MemoryLogger_MaximumLogsLessThanOne,
                        LoggingLevel.Critical, value);
                _maximumLogEntries = value;
                Clean();
            }
        }

        /// <summary>
        ///   The cache that stores the logs.
        /// </summary>
        [NotNull]
        private readonly Queue<Log> _queue = new Queue<Log>();

        /// <summary>
        /// The cleaner subscription.
        /// </summary>
        private readonly IDisposable _cleaner;

        private int _maximumLogEntries;
        private TimeSpan _cacheExpiry;

        /// <summary>
        ///   Initializes a new instance of the <see cref="MemoryLogger"/> class.
        /// </summary>
        /// <param name="name">The logger name.</param>
        /// <param name="cacheExpiry">
        ///   <para>The length of time that log items are cached for.</para>
        ///   <para>Use <see cref="TimeSpan.Zero"/> for infinity.</para>
        ///   <para>By default the expiry will be set to 10 minutes.</para>
        /// </param>
        /// <param name="maximumLogEntries">
        ///   <para>The maximum number of log entries to store.</para>
        ///   <para>By default this is set to 10,000.</para>
        /// </param>
        /// <param name="validLevels">
        ///   <para>The valid log levels.</para>
        ///   <para>By default this is set allow all <see cref="LoggingLevels">all log levels</see>.</para>
        /// </param>
        /// <exception cref="LoggingException">
        ///   <para><paramref name="maximumLogEntries"/> was less than 1.</para>
        ///   <para>-or-</para>
        ///   <para><paramref name="cacheExpiry"/> cannot be less than 10 seconds.</para>
        /// </exception>
        public MemoryLogger(
            [NotNull] string name,
            TimeSpan cacheExpiry = default(TimeSpan),
            int maximumLogEntries = 10000,
            LoggingLevels validLevels = LoggingLevels.All)
            : base(name, true, false, validLevels)
        {
            MaximumLogEntries = maximumLogEntries;
            CacheExpiry = cacheExpiry;
            _cleaner = Log.Tick.Subscribe(t => Clean());
        }

        /// <summary>
        /// Adds the specified logs to storage in batches.
        /// </summary>
        /// <param name="logs">The logs to add to storage.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task.</returns>
        public override Task Add(IEnumerable<Log> logs, CancellationToken token)
        {
            lock (_lock)
            {
                foreach (Log log in logs
                    .Where(log => log.Level.IsValid(ValidLevels) &&
                                  CacheExpiry > (DateTime.UtcNow - log.TimeStamp)))
                {
                    token.ThrowIfCancellationRequested();
                    _queue.Enqueue(log);
                }
            }

            // We always complete synchronously.
            return Task.FromResult(true);
        }

        /// <summary>
        /// Gets all cached logs.
        /// </summary>
        /// <value>All.</value>
        [NotNull]
        public IEnumerable<Log> All
        {
            get
            {
                Log[] snapshot;
                lock (_lock)
                    snapshot = _queue.ToArray();

                // Combine cache with any added in future.
                return snapshot
                    .Skip(snapshot.Length > MaximumLogEntries ? snapshot.Length - MaximumLogEntries : 0)
                    .Where(log => CacheExpiry > (DateTime.UtcNow - log.TimeStamp));
            }
        }

        /// <summary>
        /// Gets the Qbservable allowing asynchronous querying of log data.
        /// </summary>
        /// <value>The query.</value>
        [NotNull]
        public override IQbservable<IEnumerable<KeyValuePair<string, string>>> Qbserve
        {
            get
            {
                return All
                    .ToObservable()
                    .AsQbservable();
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            _cleaner.Dispose();
            lock (_lock)
                _queue.Clear();
        }

        private object _lock = new object();

        /// <summary>
        /// Cleans this instance.
        /// </summary>
        private void Clean()
        {
            // only one cleaner should run at a time.
            lock (_lock)
            {
                while ((_queue.Count > 0) &&
                       ((_queue.Count > MaximumLogEntries) ||
                        (CacheExpiry > (DateTime.UtcNow - _queue.Peek().TimeStamp))))
                    _queue.Dequeue();
            }
        }
    }
}