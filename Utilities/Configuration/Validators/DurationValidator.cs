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

using System;
using System.Configuration;
using NodaTime;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Configuration.Validators
{
    /// <summary>
    /// Provides validation of a <see cref="Duration"/> object.
    /// </summary>
    [PublicAPI]
    public sealed class DurationValidator : ConfigurationValidatorBase
    {
        private readonly Duration _minValue;
        private readonly Duration _maxValue;
        private readonly bool _exclusive;

        /// <summary>
        /// Initializes a new instance of the <see cref="DurationValidator" /> class.
        /// </summary>
        /// <param name="minValue">The minimum duration allowed to pass validation (inclusive).</param>
        /// <param name="maxValue">The maximum duration allowed to pass validation (exclusive).</param>
        /// <param name="rangeIsExclusive">If set to <see langword="true" /> the value must be outside the given range.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="minValue" /> is greater than <paramref name="maxValue" />.</exception>
        public DurationValidator(Duration minValue, Duration maxValue, bool rangeIsExclusive = false)
        {
            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException(nameof(minValue), Resources.Validator_MinGreaterThanMax);
            _minValue = minValue;
            _maxValue = maxValue;
            _exclusive = rangeIsExclusive;
        }

        /// <summary>
        /// Determines whether this instance can validate the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public override bool CanValidate(Type type)
        {
            return type == typeof(Duration);
        }

        /// <summary>
        /// Determines whether the value of an object is valid. 
        /// </summary>
        /// <param name="value">The object value.</param>
        public override void Validate(object value)
        {
            if (!(value is Duration))
                throw new ArgumentException(Resources.Validator_Validate_WrongType);

            Duration duration = (Duration)value;

            bool inRange = (_minValue <= duration && duration < _maxValue);

            if (_exclusive == inRange)
                throw new ArgumentException(
                    string.Format(
                        // ReSharper disable once AssignNullToNotNullAttribute
                        _exclusive
                            ? Resources.Validator_Validate_ValueNotOutsideRange
                            : Resources.Validator_Validate_ValueNotInRange,
                        duration,
                        _minValue,
                        _maxValue));
        }
    }
}