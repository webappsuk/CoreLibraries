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
using WebApplications.Utilities.Converters;

namespace WebApplications.Utilities.Configuration.Validators
{
    /// <summary>
    /// Declaratively instructs the .NET Framework to perform duration validation on a configuration property. This class cannot be inherited.
    /// </summary>
    [PublicAPI]
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DurationValidatorAttribute : ConfigurationValidatorAttribute
    {
        [NotNull]
        private static readonly DurationConverter _converter = new DurationConverter();

        private Duration _minValue = Duration.FromTicks(long.MinValue);
        private Duration _maxValue = Duration.FromTicks(long.MaxValue);

        /// <summary>
        /// The duration minimum value as a string.
        /// </summary>
        public const string DurationMinValue = "-10675199.02:48:05.4775808";

        /// <summary>
        /// The duration maximum value as a string.
        /// </summary>
        public const string DurationMaxValue = "10675199.02:48:05.4775807";

        /// <summary>
        /// Gets the minimum value.
        /// </summary>
        /// <value>
        /// The minimum value.
        /// </value>
        public Duration MinValue => _minValue;

        /// <summary>
        /// Gets the maximum value.
        /// </summary>
        /// <value>
        /// The maximum value.
        /// </value>
        public Duration MaxValue => _maxValue;

        /// <summary>
        /// Gets or sets the minimum value.
        /// </summary>
        /// <value>
        /// The minimum value.
        /// </value>
        public string MinValueString
        {
            get { return _minValue.ToString(); }
            set
            {
                Duration minValue = Parse(value);
                if (_maxValue < minValue)
                    throw new ArgumentOutOfRangeException(Resources.Validator_MinGreaterThanMax);
                _minValue = minValue;
            }
        }

        /// <summary>
        /// Gets or sets the maximum value.
        /// </summary>
        /// <value>
        /// The maximum value.
        /// </value>
        public string MaxValueString
        {
            get { return _maxValue.ToString(); }
            set
            {
                Duration maxValue = Parse(value);
                if (maxValue < _minValue)
                    throw new ArgumentOutOfRangeException(Resources.Validator_MinGreaterThanMax);
                _maxValue = maxValue;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether to include or exclude the values in the range as defined by <see cref="MinValueString"/> and <see cref="MaxValueString"/>.
        /// </summary>
        /// <value>
        ///   <see langword="true" /> if the value must be excluded; otherwise, <see langword="false" />. The default is <see langword="false" />.
        /// </value>
        public bool ExcludeRange { get; set; }

        /// <summary>
        /// Parses the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        // ReSharper disable once PossibleNullReferenceException
        private static Duration Parse(string value) => (Duration)_converter.ConvertFromString(value);

        /// <summary>
        /// Gets the validator attribute instance.
        /// </summary>
        /// <returns>
        /// The current <see cref="ConfigurationValidatorBase"/>.
        /// </returns>
        public override ConfigurationValidatorBase ValidatorInstance
            => new DurationValidator(_minValue, _maxValue, ExcludeRange);
    }
}