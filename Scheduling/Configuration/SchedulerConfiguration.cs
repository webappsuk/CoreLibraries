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
using WebApplications.Utilities.Configuration.Validators;

namespace WebApplications.Utilities.Scheduling.Configuration
{
    /// <summary>
    ///   The configuration section for the default <see cref="Scheduler"/>.
    /// </summary>
    [PublicAPI]
    public class SchedulerConfiguration : ConfigurationSection<SchedulerConfiguration>
    {
        static SchedulerConfiguration()
        {
            // NOTE: Handler assigned here to ensure it will always be the first one invoked
            ActiveChanged += (s, e) => Scheduler.LoadConfiguration();
        }

        /// <summary>
        ///   Gets a value indicating whether the scheduler is enabled is enabled.
        /// </summary>
        [ConfigurationProperty("enabled", DefaultValue = true, IsRequired = false)]
        public bool Enabled
        {
            get { return GetProperty<bool>("enabled"); }
            set { SetProperty("enabled", value); }
        }

        /// <summary>
        ///   Gets the default for the maximum history in a scheduled function/action.
        /// </summary>
        [ConfigurationProperty("defautlMaximumHistory", DefaultValue = 100, IsRequired = false)]
        [IntegerValidator(MinValue = 1, MaxValue = int.MaxValue)]
        public int DefautlMaximumHistory
        {
            get { return GetProperty<int>("defautlMaximumHistory"); }
            set { SetProperty("defautlMaximumHistory", value); }
        }

        /// <summary>
        ///   Gets the default maximum duration of a scheduled action/function.
        /// </summary>
        [ConfigurationProperty("defaultMaximumDuration", DefaultValue = "00:10:00", IsRequired = false)]
        [DurationValidator(MinValueString = "00:00:00.01", MaxValueString = "1.00:00:00")]
        public Duration DefaultMaximumDuration
        {
            get { return GetProperty<Duration>("defaultMaximumDuration"); }
            set { SetProperty("defaultMaximumDuration", value); }
        }

        /// <summary>
        /// Gets or sets the named schedules.
        /// </summary>
        /// <value>The schedules.</value>
        [ConfigurationProperty("schedules", IsRequired = false, IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(ScheduleCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        [NotNull]
        [ItemNotNull]
        public ScheduleCollection Schedules
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            get { return (ScheduleCollection)this["schedules"]; }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                this["schedules"] = value;
            }
        }
    }
}