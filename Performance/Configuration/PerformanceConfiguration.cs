using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodaTime;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Configuration;

namespace WebApplications.Utilities.Performance.Configuration
{
    /// <summary>
    /// The configuration section for the Perfomance library.
    /// </summary>
    [PublicAPI]
    public class PerformanceConfiguration : ConfigurationSection<PerformanceConfiguration>
    {
        /// <summary>
        /// Gets or sets whether counters are enabled.
        /// </summary>
        [ConfigurationProperty("enabled", DefaultValue = true, IsRequired = false)]
        [PublicAPI]
        public bool Enabled
        {
            get { return GetProperty<bool>("enabled"); }
            set { SetProperty("enabled", value); }
        }

        /// <summary>
        /// Gets or sets the default samples.
        /// </summary>
        [ConfigurationProperty("defaultSamples", DefaultValue = 10, IsRequired = false)]
        [IntegerValidator(MinValue = 2, MaxValue = int.MaxValue)]
        [PublicAPI]
        public int DefaultSamples
        {
            get { return GetProperty<int>("defaultSamples"); }
            set { SetProperty("defaultSamples", value); }
        }

        /// <summary>
        /// Gets or sets the default warning duration.
        /// </summary>
        [ConfigurationProperty("defaultWarning", DefaultValue = "00:00:02", IsRequired = false)]
        [TimeSpanValidator(MinValueString = "00:00:00.001", MaxValueString = "1.00:00:00")]
        [PublicAPI]
        public TimeSpan DefaultWarning
        {
            get { return GetProperty<TimeSpan>("defaultWarning"); }
            set { SetProperty("defaultWarning", value); }
        }

        /// <summary>
        /// Gets or sets the default critical duration.
        /// </summary>
        [ConfigurationProperty("defaultCritical", DefaultValue = "00:00:05", IsRequired = false)]
        [TimeSpanValidator(MinValueString = "00:00:00.001", MaxValueString = "1.00:00:00")]
        [PublicAPI]
        public TimeSpan DefaultCritical
        {
            get { return GetProperty<TimeSpan>("defaultCritical"); }
            set { SetProperty("defaultCritical", value); }
        }

        /// <summary>
        /// Whether the counters are enabled.
        /// </summary>
        internal static bool IsEnabled;

        /// <summary>
        /// The default maximum samples.
        /// </summary>
        internal static int DefaultMaximumSamples;

        /// <summary>
        /// The default warning duration.
        /// </summary>
        internal static Duration DefaultWarningDuration;

        /// <summary>
        /// The default critical duration.
        /// </summary>
        internal static Duration DefaultCriticalDuration;

        /// <summary>
        /// Initializes static members of the <see cref="PerformanceConfiguration"/> class.
        /// </summary>
        static PerformanceConfiguration()
        {
            Update(Active);
            Changed += (s, e) => Update(e.NewConfiguration);
        }

        /// <summary>
        /// Handles the <see cref="E:Changed" /> event.
        /// </summary>
        /// <param name="config">The configuration.</param>
        private static void Update([NotNull] PerformanceConfiguration config)
        {
            IsEnabled = config.GetProperty<bool>("enabled");
            DefaultMaximumSamples = config.GetProperty<int>("defaultSamples");
            DefaultWarningDuration = Duration.FromTimeSpan(config.GetProperty<TimeSpan>("defaultWarning"));
            DefaultCriticalDuration = Duration.FromTimeSpan(config.GetProperty<TimeSpan>("defaultCritical"));
        }
    }
}
