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
using NodaTime.Text;

namespace WebApplications.Utilities.Scheduling.Configuration
{
    /// <summary>
    /// Describes validation rules for a duration. This class cannot be inherited.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DurationValidatorAttribute : ConfigurationValidatorAttribute
    {
        private Duration _min = Duration.FromTimeSpan(TimeSpan.MinValue);
        private Duration _max = Duration.FromTimeSpan(TimeSpan.MaxValue);
        private bool _excludeRange;

        /// <summary>
        /// The duration minimum value as a string.
        /// </summary>
        public const string DurationMinValue = "-10675199.02:48:05.4775808";

        /// <summary>
        /// The duration maximum value as a string.
        /// </summary>
        public const string DurationMaxValue = "10675199.02:48:05.4775807";

        /// <summary>
        /// Initializes a new instance of the <see cref="DurationValidatorAttribute"/> class.
        /// </summary>
        [PublicAPI]
        public DurationValidatorAttribute()
        {
        }

        /// <summary>
        /// Gets the validator attribute instance.
        /// </summary>
        /// <value>The validator instance.</value>
        [NotNull]
        [PublicAPI]
        public override ConfigurationValidatorBase ValidatorInstance
        {
            get { return new DurationValidator(_min, _max, _excludeRange); }
        }

        /// <summary>
        /// Gets the minimum value.
        /// </summary>
        /// <value>The minimum value.</value>
        [PublicAPI]
        public Duration MinValue
        {
            get { return _min; }
        }

        /// <summary>
        /// Gets the maximum value.
        /// </summary>
        /// <value>The maximum value.</value>
        [PublicAPI]
        public Duration MaxValue
        {
            get { return _max; }
        }

        /// <summary>
        /// Gets or sets the minimum value string.
        /// </summary>
        /// <value>The minimum value string.</value>
        /// <exception cref="System.ArgumentOutOfRangeException">value</exception>
        [NotNull]
        [PublicAPI]
        public string MinValueString
        {
            get { return _min.ToString(); }
            set
            {
                Contract.Requires(value != null);
                // ReSharper disable PossibleNullReferenceException
                ParseResult<Duration> parseResult = DurationPattern.RoundtripPattern.Parse(value);
                Duration duration;
                if (parseResult.Success)
                    duration = parseResult.Value;
                else
                {
                    TimeSpan t = TimeSpan.Parse(value);
                    duration = Duration.FromTimeSpan(t);
                }
                // ReSharper restore PossibleNullReferenceException

                if (duration > MaxValue)
                {
                    throw new ArgumentOutOfRangeException(
                        "value",
                        string.Format(
                            // ReSharper disable once AssignNullToNotNullAttribute
                            Resource.DurationValidator_Validate_InclusiveMaximum,
                            duration,
                            MaxValue));
                }

                _min = duration;
            }
        }

        /// <summary>
        /// Gets or sets the maximum value string.
        /// </summary>
        /// <value>The maximum value string.</value>
        /// <exception cref="System.ArgumentOutOfRangeException">value</exception>
        [NotNull]
        [PublicAPI]
        public string MaxValueString
        {
            get { return _max.ToString(); }
            set
            {
                Contract.Requires(value != null);
                // ReSharper disable PossibleNullReferenceException
                ParseResult<Duration> parseResult = DurationPattern.RoundtripPattern.Parse(value);
                Duration duration;
                if (parseResult.Success)
                    duration = parseResult.Value;
                else
                {
                    TimeSpan t = TimeSpan.Parse(value);
                    duration = Duration.FromTimeSpan(t);
                }
                // ReSharper restore PossibleNullReferenceException

                if (duration < MinValue)
                {
                    throw new ArgumentOutOfRangeException(
                        "value",
                        string.Format(
                            // ReSharper disable once AssignNullToNotNullAttribute
                            Resource.DurationValidator_Validate_InclusiveMinimum,
                            duration,
                            MinValue));
                }

                _max = duration;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the range is exclusive.
        /// </summary>
        /// <value><see langword="true" /> if exclusive; otherwise, <see langword="false" />.</value>
        [PublicAPI]
        public bool ExcludeRange
        {
            get { return _excludeRange; }
            set { _excludeRange = value; }
        }
    }
}