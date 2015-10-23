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

using NodaTime;
using System;
using System.Configuration;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Configuration;

namespace WebApplications.Utilities.Performance.Configuration
{
    /// <summary>
    /// The configuration section for the Performance library.
    /// </summary>
    [PublicAPI]
    public class PerformanceConfiguration : ConfigurationSection<PerformanceConfiguration>
    {
        /// <summary>
        /// Gets or sets whether counters are enabled.
        /// </summary>
        [ConfigurationProperty("enabled", DefaultValue = true, IsRequired = false)]
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
            // ReSharper disable once PossibleNullReferenceException
            ActiveChanged += (s, e) => Update(s);
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