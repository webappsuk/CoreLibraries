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
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using NodaTime;
using NodaTime.Text;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Converters
{
    /// <summary>
    ///  Providers methods for converting types to/from the <see cref="Instant"/> type.
    /// </summary>
    public class InstantConverter : TypeConverter
    {
        [NotNull]
        [ItemNotNull]
        private static readonly InstantPattern[] _patterns =
        {
            InstantPattern.ExtendedIsoPattern,
            InstantPattern.GeneralPattern,
        };

        [NotNull]
        [ItemNotNull]
        private static IEnumerable<InstantPattern> Patterns
            => _patterns.Concat(_patterns.Select(p => p.WithCulture(CultureInfo.CurrentCulture)));

        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
        /// </summary>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context. </param>
        /// <param name="sourceType">A <see cref="T:System.Type"/> that represents the type you want to convert from. </param>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string) ||
                sourceType == typeof(DateTime) ||
                sourceType == typeof(DateTimeOffset) ||
                sourceType == typeof(OffsetDateTime) ||
                sourceType == typeof(ZonedDateTime))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Converts the given object to the type of this converter, using the specified context and culture information.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents the converted value.
        /// </returns>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context. </param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo"/> to use as the current culture. </param>
        /// <param name="value">The <see cref="T:System.Object"/> to convert. </param>
        /// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string @string = value as string;
            if (@string != null)
            {
                Instant instant;
                if (Patterns.TryParseAny(@string.Trim(), out instant))
                    return instant;

                DateTimeOffset dateTimeOffset;
                if (DateTimeOffset.TryParse(@string, out dateTimeOffset))
                    return Instant.FromDateTimeOffset(dateTimeOffset);

                throw new NotSupportedException(
                    // ReSharper disable once AssignNullToNotNullAttribute
                    string.Format(Resources.InstantConverter_ConvertFrom_CannotParse, @string));
            }
            if (value is DateTime)
            {
                DateTime dateTime = (DateTime)value;
                return Instant.FromDateTimeUtc(dateTime.ToUniversalTime());
            }
            if (value is DateTimeOffset)
            {
                DateTimeOffset dateTimeOffset = (DateTimeOffset)value;
                return Instant.FromDateTimeOffset(dateTimeOffset);
            }
            if (value is OffsetDateTime)
            {
                OffsetDateTime offsetDateTime = (OffsetDateTime)value;
                return offsetDateTime.ToInstant();
            }
            if (value is ZonedDateTime)
            {
                ZonedDateTime zonedDateTime = (ZonedDateTime)value;
                return zonedDateTime.ToInstant();
            }

            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Returns whether this converter can convert the object to the specified type, using the specified context.
        /// </summary>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context. </param>
        /// <param name="destinationType">A <see cref="T:System.Type"/> that represents the type you want to convert to. </param>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string) ||
                destinationType == typeof(DateTime) ||
                destinationType == typeof(DateTimeOffset) ||
                destinationType == typeof(ZonedDateTime) ||
                destinationType == typeof(OffsetDateTime))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        /// Converts the given value object to the specified type, using the specified context and culture information.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents the converted value.
        /// </returns>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context. </param>
        /// <param name="culture">A <see cref="T:System.Globalization.CultureInfo"/>. If null is passed, the current culture is assumed. </param>
        /// <param name="value">The <see cref="T:System.Object"/> to convert. </param>
        /// <param name="destinationType">The <see cref="T:System.Type"/> to convert the <paramref name="value"/> parameter to. </param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="destinationType"/> parameter is null. </exception>
        /// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
        public override object ConvertTo(
            ITypeDescriptorContext context,
            CultureInfo culture,
            [NotNull] object value,
            Type destinationType)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (destinationType == null) throw new ArgumentNullException(nameof(destinationType));

            Instant instant = (Instant)value;

            if (destinationType == typeof(string))
                // ReSharper disable once PossibleNullReferenceException
                return InstantPattern.ExtendedIsoPattern.Format(instant);

            if (destinationType == typeof(DateTime))
                return instant.ToDateTimeUtc();

            if (destinationType == typeof(DateTimeOffset))
                return instant.ToDateTimeOffset();

            if (destinationType == typeof(ZonedDateTime))
                return instant.InUtc();

            if (destinationType == typeof(OffsetDateTime))
                return OffsetDateTime.FromDateTimeOffset(instant.ToDateTimeOffset());

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}