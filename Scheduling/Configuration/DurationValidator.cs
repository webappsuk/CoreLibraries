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
using WebApplications.Utilities.Annotations;
using NodaTime;

namespace WebApplications.Utilities.Scheduling.Configuration
{
    /// <summary>
    /// Validates a <see cref="Duration"/>.
    /// </summary>
    public class DurationValidator : ConfigurationValidatorBase
    {
        /// <summary>
        /// The minimum value.
        /// </summary>
        [PublicAPI]
        public readonly Duration MinValue = Duration.FromTimeSpan(TimeSpan.MinValue);

        /// <summary>
        /// The maximum value.
        /// </summary>
        [PublicAPI]
        public readonly Duration MaxValue = Duration.FromTimeSpan(TimeSpan.MaxValue);

        /// <summary>
        /// The range is exclusive.
        /// </summary>
        [PublicAPI]
        public readonly bool RangeIsExclusive;

        /// <summary>
        /// Initializes a new instance of the <see cref="DurationValidator"/> class.
        /// </summary>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="maxValue">The maximum value.</param>
        /// <param name="rangeIsExclusive">if set to <see langword="true" /> [range is exclusive].</param>
        public DurationValidator(Duration minValue, Duration maxValue, bool rangeIsExclusive)
        {
            if (minValue > maxValue)
            {
                throw new ArgumentOutOfRangeException(
                    "minValue",
                    string.Format(
                        "The specified minimum duration '{0}' must be less than, or equal to, the specified maximum duration '{1}'.",
                        minValue,
                        maxValue));
            }
            MinValue = minValue;
            MaxValue = maxValue;
            RangeIsExclusive = rangeIsExclusive;
        }

        /// <summary>
        /// Determines whether an object can be validated based on type.
        /// </summary>
        /// <param name="type">The object type.</param>
        /// <returns>true if the <paramref name="type" /> parameter value matches the expected type; otherwise, false.</returns>
        // ReSharper disable once CodeAnnotationAnalyzer
        public override bool CanValidate(Type type)
        {
            return (type == typeof(Duration));
        }

        /// <summary>
        /// Determines whether the value of an object is valid.
        /// </summary>
        /// <param name="value">The object value.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// value;The specified value must be a Duration.
        /// or
        /// value
        /// or
        /// value
        /// or
        /// value
        /// or
        /// value
        /// </exception>
        // ReSharper disable once CodeAnnotationAnalyzer
        public override void Validate(object value)
        {
            if (!(value is Duration))
                throw new ArgumentOutOfRangeException("value", Resource.DurationValidator_Validate_NotDuration);
            Duration d = (Duration)value;
            if (RangeIsExclusive)
            {
                if (d <= MinValue)
                {
                    throw new ArgumentOutOfRangeException(
                        "value",
                        // ReSharper disable once AssignNullToNotNullAttribute
                        string.Format(Resource.DurationValidator_Validate_ExclusiveMinimum, d, MinValue));
                }
                if (d >= MaxValue)
                {
                    throw new ArgumentOutOfRangeException(
                        "value",
                        // ReSharper disable once AssignNullToNotNullAttribute
                        string.Format(Resource.DurationValidator_Validate_ExclusiveMaximum, d, MaxValue));
                }
            }
            else
            {
                if (d < MinValue)
                {
                    throw new ArgumentOutOfRangeException(
                        "value",
                        string.Format(
                            // ReSharper disable once AssignNullToNotNullAttribute
                            Resource.DurationValidator_Validate_InclusiveMinimum,
                            d,
                            MinValue));
                }
                if (d > MaxValue)
                {
                    throw new ArgumentOutOfRangeException(
                        "value",
                        string.Format(
                            // ReSharper disable once AssignNullToNotNullAttribute
                            Resource.DurationValidator_Validate_InclusiveMaximum,
                            d,
                            MaxValue));
                }
            }
        }
    }
}