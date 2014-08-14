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
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Globalization;
using JetBrains.Annotations;
using NodaTime;
using NodaTime.Text;

namespace WebApplications.Utilities.Scheduling.Configuration
{
    /// <summary>
    /// Converts to/from <see cref="Duration"/>.
    /// </summary>
    public class DurationConverter : TypeConverter
    {
        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
        /// <param name="sourceType">A <see cref="T:System.Type" /> that represents the type you want to convert from.</param>
        /// <returns>true if this converter can perform the conversion; otherwise, false.</returns>
        // ReSharper disable once CodeAnnotationAnalyzer
        public override bool CanConvertFrom([NotNull] ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string)) return true;
            if (sourceType == typeof(TimeSpan)) return true;
            if (sourceType == typeof(Period)) return true;

            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Returns whether this converter can convert the object to the specified type, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
        /// <param name="destinationType">A <see cref="T:System.Type" /> that represents the type you want to convert to.</param>
        /// <returns>true if this converter can perform the conversion; otherwise, false.</returns>
        // ReSharper disable once CodeAnnotationAnalyzer
        public override bool CanConvertTo([NotNull] ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string)) return true;
            if (destinationType == typeof(TimeSpan)) return true;
            if (destinationType == typeof(Period)) return true;

            return base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        /// Converts the given object to the type of this converter, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo" /> to use as the current culture.</param>
        /// <param name="value">The <see cref="T:System.Object" /> to convert.</param>
        /// <returns>An <see cref="T:System.Object" /> that represents the converted value.</returns>
        // ReSharper disable once CodeAnnotationAnalyzer
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is TimeSpan)
                return Duration.FromTimeSpan((TimeSpan)value);

            Period p = value as Period;
            if (p != null)
                return p.ToDuration();

            string str = value as string;
            if (str != null)
            {
                // ReSharper disable PossibleNullReferenceException
                ParseResult<Duration> result = DurationPattern.RoundtripPattern.Parse(str);
                // ReSharper restore PossibleNullReferenceException
                Contract.Assert(result != null);
                if (result.Success)
                    return result.Value;

                TimeSpan timeSpan;
                if (TimeSpan.TryParse(str, culture, out timeSpan))
                    return Duration.FromTimeSpan(timeSpan);
            }

            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Converts the given value object to the specified type, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
        /// <param name="culture">A <see cref="T:System.Globalization.CultureInfo" />. If null is passed, the current culture is assumed.</param>
        /// <param name="value">The <see cref="T:System.Object" /> to convert.</param>
        /// <param name="destinationType">The <see cref="T:System.Type" /> to convert the <paramref name="value" /> parameter to.</param>
        /// <returns>An <see cref="T:System.Object" /> that represents the converted value.</returns>
        // ReSharper disable once CodeAnnotationAnalyzer
        public override object ConvertTo(
            [CanBeNull] ITypeDescriptorContext context,
            [CanBeNull] CultureInfo culture,
            [NotNull] object value,
            Type destinationType)
        {
            Duration d = (Duration)value;
            if (destinationType == typeof(string))
                // ReSharper disable PossibleNullReferenceException
                return DurationPattern.RoundtripPattern.Format(d);
            // ReSharper restore PossibleNullReferenceException

            if (destinationType == typeof(TimeSpan))
                return d.ToTimeSpan();

            if (destinationType == typeof(Period))
                return Period.FromTicks(d.Ticks);

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}