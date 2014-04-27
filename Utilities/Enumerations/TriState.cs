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
using JetBrains.Annotations;

namespace WebApplications.Utilities.Enumerations
{
    /// <summary>
    ///   Handy tristate, useful where bools are just not enough!
    /// </summary>
    public struct TriState : IFormattable
    {
        #region Style enum
        /// <summary>
        ///   Sets the style for the <see cref="TriState"/>'s formatting.
        /// </summary>
        public enum Style
        {
            /// <summary>
            ///   States are displayed as Yes/Unknown/No.
            /// </summary>
            YesUnknownNo,

            /// <summary>
            ///   States are displayed as True/Undefined/False.
            /// </summary>
            TrueUndefinedFalse,

            /// <summary>
            ///   States are displayed as Negative/Equal/Positive.
            /// </summary>
            NegativeEqualPositive
        }
        #endregion

        /// <summary>
        ///   State: No.
        /// </summary>
        [UsedImplicitly]
        public static readonly TriState No = new TriState(255);

        /// <summary>
        ///   State: False.
        /// </summary>
        [UsedImplicitly]
        public static readonly TriState False = No;

        /// <summary>
        ///   State: Negative.
        /// </summary>
        [UsedImplicitly]
        public static readonly TriState Negative = No;

        /// <summary>
        ///   State: Unknown.
        /// </summary>
        [UsedImplicitly]
        public static readonly TriState Unknown = new TriState(0);

        /// <summary>
        ///   State: Undefined.
        /// </summary>
        [UsedImplicitly]
        public static readonly TriState Undefined = Unknown;

        /// <summary>
        ///   State: Equal.
        /// </summary>
        [UsedImplicitly]
        public static readonly TriState Equal = Unknown;

        /// <summary>
        ///   State: Yes.
        /// </summary>
        [UsedImplicitly]
        public static readonly TriState Yes = new TriState(1);

        /// <summary>
        ///   State: True.
        /// </summary>
        [UsedImplicitly]
        public static readonly TriState True = Yes;

        /// <summary>
        ///   State: Positive.
        /// </summary>
        [UsedImplicitly]
        public static readonly TriState Positive = Yes;

        /// <summary>
        ///   Stores the underlying state.
        /// </summary>
        private readonly byte _state;

        /// <summary>
        ///   Used internally to create the different states.
        /// </summary>
        /// <param name="state">The underlying state.</param>
        private TriState(byte state)
        {
            _state = state;
        }

        #region IFormattable Members
        /// <summary>
        ///   Formats the value of the current instance using the specified format.
        /// </summary>
        /// <param name="format">
        ///   <para>The format to use.</para>
        ///   <list type="bullet">
        ///     <item><description>G - Yes/Unknown/No. If the format string is null or empty then G is used.</description></item>
        ///     <item><description>Y - Yes/Unknown/No.</description></item>
        ///     <item><description>T - True/Undefined/False.</description></item>
        ///     <item><description>N - Positive/Equal/Negative.</description></item>
        ///   </list>
        ///   <para>If the format is not one of the above it will try and parse it to a <see cref="Style"/>.</para>
        /// </param>
        /// <param name="formatProvider">
        ///   The provider to use to format the value. Specify <see langword="null"/> to obtain the numeric format information
        ///   from the current locale setting of the operating system.
        /// </param>
        /// <returns>The value of the current instance in the format specified.</returns>
        /// <exception cref="FormatException">
        ///   The format must be one of the following:
        ///   <list type="bullet">
        ///     <item><description>G</description></item>
        ///     <item><description>Y</description></item>
        ///     <item><description>T</description></item>
        ///     <item><description>N</description></item>
        ///   </list>
        /// </exception>
        /// <filterpriority>2</filterpriority>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (String.IsNullOrEmpty(format)) format = "G";

            switch (format.ToUpperInvariant())
            {
                case "G":
                case "Y":
                    return Equals(Yes) ? "Yes" : Equals(No) ? "No" : "Unknown";
                case "T":
                    return Equals(Yes) ? "True" : Equals(No) ? "False" : "Undefined";
                case "N":
                    return this == Yes ? "Positive" : Equals(No) ? "Negative" : "Equal";
                default:
                    Style style;
                    if (!Enum.TryParse(format, true, out style))
                        throw new FormatException(String.Format(Resources.TriState_ToString_FormatException, format));
                    return ToString(style);
            }
        }
        #endregion

        /// <summary>
        ///   Returns a <see cref="string"/> that represents this instance.
        ///   Uses the default format of Style.YesUnknownNo.
        /// </summary>
        /// <returns>
        ///   A <see cref="string"/> representation of this instance in the default Yes/Unknown/No format.
        /// </returns>
        public override string ToString()
        {
            return ToString(Style.YesUnknownNo);
        }

        /// <summary>
        ///   Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <param name="style">The formatting style.</param>
        /// <returns>
        ///   A <see cref="string"/> representation of this instance in the chosen <paramref name="style"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   The <paramref name="style"/> specified is invalid.
        /// </exception>
        [NotNull]
        [UsedImplicitly]
        public string ToString(Style style)
        {
            switch (style)
            {
                case Style.YesUnknownNo:
                    return Equals(Yes) ? "Yes" : Equals(No) ? "No" : "Unknown";
                case Style.TrueUndefinedFalse:
                    return Equals(Yes) ? "True" : Equals(No) ? "False" : "Undefined";
                case Style.NegativeEqualPositive:
                    return this == Yes ? "Positive" : Equals(No) ? "Negative" : "Equal";
                default:
                    throw new ArgumentOutOfRangeException("style");
            }
        }

        /// <summary>
        ///   Performs an explicit conversion from a tristate to a <see cref="bool"/>.
        /// </summary>
        /// <param name="state">The tristate to convert.</param>
        /// <returns>The result of the conversion.</returns>
        /// <exception cref="InvalidCastException">
        ///   Cannot cast the <paramref name="state"/> if it's TriState.Unknown.
        /// </exception>
        public static explicit operator Boolean(TriState state)
        {
            if (state == Unknown)
                throw new InvalidCastException(Resources.TriState_ExplicitBoolConversion_CannotCastUnknownState);
            return state == Yes;
        }

        /// <summary>
        ///   Performs an implicit conversion from a <see cref="bool"/> to a tristate.
        /// </summary>
        /// <param name="boolean">The bool to convert.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator TriState(Boolean boolean)
        {
            return boolean ? Yes : No;
        }

        /// <summary>
        ///   Performs an implicit conversion from a tristate to a <see cref="byte"/>.
        /// </summary>
        /// <param name="state">The tristate to convert.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator byte(TriState state)
        {
            return state._state;
        }

        /// <summary>
        ///   Performs an explicit conversion from a <see cref="byte"/> to a tristate.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator TriState(byte value)
        {
            if ((value > 2) &&
                (value < 255))
                throw new InvalidCastException(Resources.TriState_ExplicitByteConversion_ValueGreaterThanThree, value);
            return value.Equals(1) ? Yes : value.Equals(255) ? No : Unknown;
        }

        /// <summary>
        ///   Performs an implicit conversion from a tristate to an <see cref="int"/>.
        /// </summary>
        /// <param name="state">The tristate to convert.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator int(TriState state)
        {
            return state.Equals(Yes) ? 1 : state.Equals(No) ? -1 : 0;
        }

        /// <summary>
        ///   Performs an implicit conversion from an <see cref="int"/> to a tristate.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator TriState(int value)
        {
            return value > 0 ? Yes : value < 0 ? No : Unknown;
        }
    }
}