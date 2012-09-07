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
using System.Configuration;
using JetBrains.Annotations;
using WebApplications.Utilities.Configuration;
using WebApplications.Utilities.Logging.Loggers;

namespace WebApplications.Utilities.Logging.Configuration
{
    /// <summary>
    ///   The configuration section for logging configurations.
    /// </summary>
    public class LoggingConfiguration : ConfigurationSection<LoggingConfiguration>
    {
        /// <summary>
        ///   Gets a value indicating whether logging is enabled.
        /// </summary>
        [ConfigurationProperty("enabled", DefaultValue = true, IsRequired = false)]
        [UsedImplicitly]
        public bool Enabled
        {
            get { return GetProperty<bool>("enabled"); }
            set { SetProperty("enabled", value); }
        }

        /// <summary>
        ///   Gets or sets the name of the application.
        /// </summary>
        [ConfigurationProperty("applicationName", DefaultValue = "", IsRequired = false)]
        [UsedImplicitly]
        public string ApplicationName
        {
            get { return GetProperty<string>("applicationName"); }
            set { SetProperty("applicationName", value); }
        }

        /// <summary>
        ///   Gets or sets the application <see cref="Guid"/>.
        /// </summary>
        [ConfigurationProperty("applicationGuid", IsRequired = false)]
        [UsedImplicitly]
        public Guid ApplicationGuid
        {
            get { return GetProperty<Guid>("applicationGuid"); }
            set { SetProperty("applicationGuid", value); }
        }

        /// <summary>
        ///   Gets the valid <see cref="LogLevels">logging levels</see>.
        /// </summary>
        [ConfigurationProperty("validLevels", DefaultValue = LogLevels.All, IsRequired = false)]
        [UsedImplicitly]
        public LogLevels ValidLevels
        {
            get { return GetProperty<LogLevels>("validLevels"); }
            set { SetProperty("validLevels", value); }
        }

        /// <summary>
        ///   Gets minimum batch size.
        /// </summary>
        [ConfigurationProperty("minBatchSize", DefaultValue = 10, IsRequired = false)]
        [IntegerValidator(MinValue = 1, MaxValue = 5000)]
        [UsedImplicitly]
        public int MinBatchSize
        {
            get { return GetProperty<int>("minBatchSize"); }
            set { SetProperty("minBatchSize", value); }
        }

        /// <summary>
        ///   Gets the maximum batch size.
        /// </summary>
        [ConfigurationProperty("maxBatchSize", DefaultValue = 1000, IsRequired = false)]
        [IntegerValidator(MinValue = 1, MaxValue = 10000)]
        [UsedImplicitly]
        public int MaxBatchSize
        {
            get { return GetProperty<int>("maxBatchSize"); }
            set { SetProperty("maxBatchSize", value); }
        }

        /// <summary>
        ///   Gets the batch wait time.
        /// </summary>
        [ConfigurationProperty("batchWait", DefaultValue = "00:00:02", IsRequired = false)]
        [TimeSpanValidator(MinValueString = "00:00:00.5", MaxValueString = "1:00:00")]
        [UsedImplicitly]
        public TimeSpan BatchWait
        {
            get { return GetProperty<TimeSpan>("batchWait"); }
            set { SetProperty("batchWait", value); }
        }

        /// <summary>
        ///   Gets the log cache's expiry.
        /// </summary>
        [ConfigurationProperty("logCacheExpiry", DefaultValue = "00:10:00", IsRequired = false)]
        [TimeSpanValidator(MinValueString = "00:00:00", MaxValueString = "1.00:00:00")]
        [UsedImplicitly]
        public TimeSpan LogCacheExpiry
        {
            get { return GetProperty<TimeSpan>("logCacheExpiry"); }
            set { SetProperty("logCacheExpiry", value); }
        }

        /// <summary>
        ///   Gets maximum size (number of log entries) for the cache.
        /// </summary>
        [ConfigurationProperty("logCacheMaximumEntries", DefaultValue = "10000", IsRequired = false)]
        [IntegerValidator(MinValue = 0, MaxValue = int.MaxValue)]
        [UsedImplicitly]
        public int LogCacheMaximumEntries
        {
            get { return GetProperty<int>("logCacheMaximumEntries"); }
            set { SetProperty("logCacheMaximumEntries", value); }
        }

        /// <summary>
        ///   Gets all of the <see cref="LoggersCollection">loggers</see>.
        /// </summary>
        [ConfigurationProperty("loggers", IsRequired = false, IsDefaultCollection = false)]
        [ConfigurationCollection(typeof (LoggersCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        [NotNull]
        [UsedImplicitly]
        public LoggersCollection Loggers
        {
            get { return GetProperty<LoggersCollection>("loggers"); }
            set { SetProperty("loggers", value); }
        }

        /// <summary>
        ///   Sets the <see cref="LoggingConfiguration"/> to its initial state.
        /// </summary>
        protected override void InitializeDefault()
        {
            // Ensure loggers is initialised.
            // ReSharper disable ConstantNullCoalescingCondition
            Loggers = Loggers ?? new LoggersCollection();
            // ReSharper restore ConstantNullCoalescingCondition

            // Add a trace logger.
            Loggers.Add(new LoggerElement
                            {
                                Name = "Trace Logger",
                                Type = typeof (TraceLogger),
                                Enabled = true,
                                ValidLevels = LogLevels.All
                            });

            ApplicationGuid = Guid.NewGuid();
            base.InitializeDefault();
        }
    }
}