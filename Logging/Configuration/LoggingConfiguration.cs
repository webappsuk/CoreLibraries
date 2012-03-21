#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities.Logging 
// Project: WebApplications.Utilities.Logging
// File: LoggingConfiguration.cs
// 
// This software, its object code and source code and all modifications made to
// the same (the “Software”) are, and shall at all times remain, the proprietary
// information and intellectual property rights of Web Applications (UK) Limited. 
// You are only entitled to use the Software as expressly permitted by Web
// Applications (UK) Limited within the Software Customisation and
// Licence Agreement (the “Agreement”).  Any copying, modification, decompiling,
// distribution, licensing, sale, transfer or other use of the Software other than
// as expressly permitted in the Agreement is expressly forbidden.  Web
// Applications (UK) Limited reserves its rights to take action against you and
// your employer in accordance with its contractual and common law rights
// (including injunctive relief) should you breach the terms of the Agreement or
// otherwise infringe its copyright or other intellectual property rights in the
// Software.
// 
// © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
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
        [ConfigurationCollection(typeof(LoggersCollection),
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
                                Type = typeof(TraceLogger),
                                Enabled = true,
                                ValidLevels = LogLevels.All
                            });

            ApplicationGuid = Guid.NewGuid();
            base.InitializeDefault();
        }
    }
}