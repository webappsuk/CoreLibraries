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
using System.Configuration;
using System.Diagnostics.Contracts;
using JetBrains.Annotations;
using NodaTime;
using WebApplications.Utilities.Configuration;

namespace WebApplications.Utilities.Scheduling.Configuration
{
    /// <summary>
    ///   The configuration section for the default <see cref="Scheduler"/>.
    /// </summary>
    [PublicAPI]
    public class SchedulerConfiguration : ConfigurationSection<SchedulerConfiguration>
    {
        /// <summary>
        ///   Gets a value indicating whether the scheduler is enabled is enabled.
        /// </summary>
        [ConfigurationProperty("enabled", DefaultValue = true, IsRequired = false)]
        [PublicAPI]
        public bool Enabled
        {
            get { return GetProperty<bool>("enabled"); }
            set { SetProperty("enabled", value); }
        }

        /// <summary>
        ///   Gets the default for the maximum history in a scheduled function/action.
        /// </summary>
        [ConfigurationProperty("defautlMaximumHistory", DefaultValue = 100, IsRequired = false)]
        [IntegerValidator(MinValue = 0, MaxValue = int.MaxValue)]
        [PublicAPI]
        public int DefautlMaximumHistory
        {
            get { return GetProperty<int>("defautlMaximumHistory"); }
            set { SetProperty("defautlMaximumHistory", value); }
        }

        /// <summary>
        ///   Gets the default maximum duration of a scheduled action/function.
        /// </summary>
        [ConfigurationProperty("defaultMaximumDuration", DefaultValue = "00:10:00", IsRequired = false)]
        [TimeSpanValidator(MinValueString = "00:00:00.01", MaxValueString = "1.00:00:00")] // TODO Duration
        [PublicAPI]
        public Duration DefaultMaximumDuration
        {
            get { return GetProperty<Duration>("defaultMaximumDuration"); }
            set { SetProperty("defaultMaximumDuration", value); }
        }

        /// <summary>
        /// Gets or sets the  named schedules.
        /// </summary>
        /// <value>The schedules.</value>
        [ConfigurationProperty("schedules", IsRequired = false, IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(ScheduleCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        [NotNull]
        [PublicAPI]
        public ScheduleCollection Schedules
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            get { return (ScheduleCollection)this["schedules"]; }
            set
            {
                Contract.Requires(value != null);
                this["schedules"] = value;
            }
        }

        /// <summary>
        ///   Sets the <see cref="SchedulerConfiguration"/> to its initial state.
        /// </summary>
        protected override void InitializeDefault()
        {
            // Ensure loggers is initialised.
            // ReSharper disable ConstantNullCoalescingCondition
            Schedules = Schedules ?? new ScheduleCollection();
            // ReSharper restore ConstantNullCoalescingCondition
            
            base.InitializeDefault();
        }
    }
}