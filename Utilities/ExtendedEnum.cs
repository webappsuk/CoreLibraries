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
using System.Collections.Generic;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities
{
    /// <summary>
    ///   Extension methods for <see cref="Enum"/>s.
    /// </summary>
    /// <seealso cref="T:WebApplications.Utilities.ExtendedEnum`1"/>
    public static class ExtendedEnum
    {
        #region ToEnum overloads
        /// <summary>
        ///   Gets the enum that corresponds to the specified <see cref="sbyte"/> value.
        /// </summary>
        /// <typeparam name="TEnum">The enum type.</typeparam>
        /// <param name="value">The numerical value.</param>
        /// <param name="includeImplicit">
        ///   <para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        ///   <para>By default this is set to <see langword="false"/>.</para>
        /// </param>
        /// <returns>
        ///   The enum of type <typeparamref name="TEnum"/> that corresponds to the <paramref name="value"/> specified.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   The enum of type <typeparamref name="TEnum"/> doesn't contain the raw <paramref name="value"/> specified.
        /// </exception>
        [UsedImplicitly]
        public static TEnum ToEnum<TEnum>(this sbyte value, bool includeImplicit = false)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return ExtendedEnum<TEnum>.GetValue(value, includeImplicit);
        }

        /// <summary>
        ///   Gets the enum that corresponds to the specified <see cref="byte"/> value.
        /// </summary>
        /// <typeparam name="TEnum">The enum type.</typeparam>
        /// <param name="value">The numerical value.</param>
        /// <param name="includeImplicit">
        ///   <para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        ///   <para>By default this is set to <see langword="false"/>.</para>
        /// </param>
        /// <returns>
        ///   The enum of type <typeparamref name="TEnum"/> that corresponds to the <paramref name="value"/> specified.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   The enum of type <typeparamref name="TEnum"/> doesn't contain the raw <paramref name="value"/> specified.
        /// </exception>
        [UsedImplicitly]
        public static TEnum ToEnum<TEnum>(this byte value, bool includeImplicit = false)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return ExtendedEnum<TEnum>.GetValue(value, includeImplicit);
        }

        /// <summary>
        ///   Gets the enum that corresponds to the specified <see cref="ushort"/> value.
        /// </summary>
        /// <typeparam name="TEnum">The enum type.</typeparam>
        /// <param name="value">The numerical value.</param>
        /// <param name="includeImplicit">
        ///   <para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        ///   <para>By default this is set to <see langword="false"/>.</para>
        /// </param>
        /// <returns>
        /// The enum of type <typeparamref name="TEnum"/> that corresponds to the <paramref name="value"/> specified.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   The enum of type <typeparamref name="TEnum"/> doesn't contain the raw <paramref name="value"/> specified.
        /// </exception>
        [UsedImplicitly]
        public static TEnum ToEnum<TEnum>(this ushort value, bool includeImplicit = false)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return ExtendedEnum<TEnum>.GetValue(value, includeImplicit);
        }

        /// <summary>
        ///   Gets the enum that corresponds to the specified <see cref="short"/> value.
        /// </summary>
        /// <typeparam name="TEnum">The enum type.</typeparam>
        /// <param name="value">The numerical value.</param>
        /// <param name="includeImplicit">
        ///   <para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        ///   <para>By default this is set to <see langword="false"/>.</para>
        /// </param>
        /// <returns>
        ///   The enum of type <typeparamref name="TEnum"/> that corresponds to the <paramref name="value"/> specified.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   The enum of type <typeparamref name="TEnum"/> doesn't contain the raw <paramref name="value"/> specified.
        /// </exception>
        [UsedImplicitly]
        public static TEnum ToEnum<TEnum>(this short value, bool includeImplicit = false)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return ExtendedEnum<TEnum>.GetValue(value, includeImplicit);
        }

        /// <summary>
        ///   Gets the enum that corresponds to the specified <see cref="uint"/> value.
        /// </summary>
        /// <typeparam name="TEnum">The enum type.</typeparam>
        /// <param name="value">The numerical value.</param>
        /// <param name="includeImplicit">
        ///   <para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        ///   <para>By default this is set to <see langword="false"/>.</para>
        /// </param>
        /// <returns>
        ///   The enum of type <typeparamref name="TEnum"/> that corresponds to the <paramref name="value"/> specified.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   The enum of type <typeparamref name="TEnum"/> doesn't contain the raw <paramref name="value"/> specified.
        /// </exception>
        [UsedImplicitly]
        public static TEnum ToEnum<TEnum>(this uint value, bool includeImplicit = false)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return ExtendedEnum<TEnum>.GetValue(value, includeImplicit);
        }

        /// <summary>
        ///   Gets the enum that corresponds to the specified <see cref="int"/> value.
        /// </summary>
        /// <typeparam name="TEnum">The enum type.</typeparam>
        /// <param name="value">The numerical value.</param>
        /// <param name="includeImplicit">
        ///   <para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        ///   <para>By default this is set to <see langword="false"/>.</para>
        /// </param>
        /// <returns>
        ///   The enum of type <typeparamref name="TEnum"/> that corresponds to the <paramref name="value"/> specified.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   The enum of type <typeparamref name="TEnum"/> doesn't contain the raw <paramref name="value"/> specified.
        /// </exception>
        [UsedImplicitly]
        public static TEnum ToEnum<TEnum>(this int value, bool includeImplicit = false)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return ExtendedEnum<TEnum>.GetValue(value, includeImplicit);
        }

        /// <summary>
        ///   Gets the enum that corresponds to the specified <see cref="ulong"/> value.
        /// </summary>
        /// <typeparam name="TEnum">The enum type.</typeparam>
        /// <param name="value">The numerical value.</param>
        /// <param name="includeImplicit">
        ///   <para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        ///   <para>By default this is set to <see langword="false"/>.</para>
        /// </param>
        /// <returns>
        ///   The enum of type <typeparamref name="TEnum"/> that corresponds to the <paramref name="value"/> specified.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   The enum of type <typeparamref name="TEnum"/> doesn't contain the raw <paramref name="value"/> specified.
        /// </exception>
        [UsedImplicitly]
        public static TEnum ToEnum<TEnum>(this ulong value, bool includeImplicit = false)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return ExtendedEnum<TEnum>.GetValue((long) value, includeImplicit);
        }

        /// <summary>
        ///   Gets the enum that corresponds to the specified <see cref="long"/> value.
        /// </summary>
        /// <typeparam name="TEnum">The enum type.</typeparam>
        /// <param name="value">The numerical value.</param>
        /// <param name="includeImplicit">
        ///   <para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        ///   <para>By default this is set to <see langword="false"/>.</para>
        /// </param>
        /// <returns>
        ///   The enum of type <typeparamref name="TEnum"/> that corresponds to the <paramref name="value"/> specified.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   The enum of type <typeparamref name="TEnum"/> doesn't contain the raw <paramref name="value"/> specified.
        /// </exception>
        [UsedImplicitly]
        public static TEnum ToEnum<TEnum>(this long value, bool includeImplicit = false)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return ExtendedEnum<TEnum>.GetValue(value, includeImplicit);
        }
        #endregion

        /// <summary>
        ///   Gets a <see cref="bool"/> indicating whether the specified value is a combination flag.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="value">The enum value.</param>
        /// <param name="includeImplicit">
        ///   <para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        ///   <para>By default this is set to <see langword="false"/>.</para></param>
        /// <returns>
        ///   Returns <see langword="true"/> if <paramref name="value"/> is a combination flag; otherwise returns <see langword="false"/>.
        /// </returns>
        /// <example>
        /// Example of a combination flag:
        /// <code>
        ///   [Flags]
        ///   private enum Days
        ///   {
        ///       None = 0,
        ///       Fri = 1,
        ///       Sat = 1 &lt;&lt; 1,
        ///       Sun = 1 &lt;&lt; 2,
        ///       Weekend = Sat | Sun    // Combination flag
        ///   };
        /// 
        ///   Console.WriteLine(Days.Fri.IsCombinationFlag());        // Outputs false
        ///   Console.WriteLine(Days.Weekend.IsCombinationFlag());    // Outputs true
        /// </code>
        /// </example>
        [UsedImplicitly]
        public static bool IsCombinationFlag<TEnum>(this TEnum value, bool includeImplicit = false)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return ExtendedEnum<TEnum>.GetValueDetail(value, includeImplicit).Flags > 1;
        }

        /// <summary>
        ///   Gets the number of flags defined in a flag enum.
        /// </summary>
        /// <typeparam name="TEnum">The enum type.</typeparam>
        /// <param name="value">The enum value.</param>
        /// <param name="includeImplicit">
        ///   <para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        ///   <para>By default this is set to <see langword="false"/>.</para>
        /// </param>
        /// <returns>The number of flags.</returns>
        /// <example>
        /// <code>
        ///   [Flags]
        ///   private enum Days
        ///   {
        ///       None = 0,
        ///       Fri = 1,
        ///       Sat = 1 &lt;&lt; 1,
        ///       Sun = 1 &lt;&lt; 2,
        ///       Weekend = Sat | Sun
        ///   };
        /// 
        ///   Console.WriteLine(Days.Weekend.NumberOfFlags());    // Outputs 2
        ///   Console.WriteLine(Days.Fri.NumberOfFlags());        // Outputs 1
        /// </code>
        /// </example>
        [UsedImplicitly]
        public static int NumberOfFlags<TEnum>(this TEnum value, bool includeImplicit = false)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return ExtendedEnum<TEnum>.GetValueDetail(value, includeImplicit).Flags;
        }

        /// <summary>
        ///   Indicates whether the specified value was a flag enum and is the equivalent of setting all flags for that type.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="value">The enum value.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if <paramref name="value"/> is all the flags for the type <typeparamref name="TEnum"/>;
        ///   otherwise returns <see langword="false"/>.
        /// </returns>
        /// <example>
        /// <code>
        ///   [Flags]
        ///   private enum Days
        ///   {
        ///       None = 0,
        ///       Fri = 1,
        ///       Sat = 1 &lt;&lt; 1,
        ///       Sun = 1 &lt;&lt; 2,
        ///       Weekend = Sat | Sun
        ///   };
        /// 
        ///   Console.WriteLine(Days.Weekend.IsAll());                        // Outputs false
        ///   Console.WriteLine((Days.Sat | Days.Sun).IsAll());               // Outputs false
        ///   Console.WriteLine((Days.Fri | Days.Weekend).IsAll());           // Outputs true
        ///   Console.WriteLine((Days.Fri | Days.Sat | Days.Sun).IsAll());    // Outputs true
        /// </code>
        /// </example>
        [UsedImplicitly]
        public static bool IsAll<TEnum>(this TEnum value)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return ExtendedEnum<TEnum>.AllValue.CompareTo(value) == 0;
        }

        /// <summary>
        ///   Indicates whether the specified value is a flag enum and is the equivalent of setting no flags.
        /// </summary>
        /// <typeparam name="TEnum">The enum type.</typeparam>
        /// <param name="value">The enum value.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the specified value is none; otherwise returns <see langword="false"/>.
        /// </returns>
        /// <remarks>
        ///   Flag enums should contain a zero value constant that represents no flags being set.
        /// </remarks>
        [UsedImplicitly]
        public static bool IsNone<TEnum>(this TEnum value)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return ExtendedEnum<TEnum>.NoneValue.CompareTo(value) == 0;
        }

        /// <summary>
        ///   Gets the enum's <see cref="ExtendedEnum{TEnum}.ValueDetail">ValueDetail</see>.
        /// </summary>
        /// <typeparam name="TEnum">The enum type.</typeparam>
        /// <param name="value">The enum value.</param>
        /// <param name="includeImplicit">
        /// <para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        /// <para>By default this is set to <see langword="false"/>.</para>
        /// </param>
        /// <returns>The <see cref="ExtendedEnum{TEnum}.ValueDetail">ValueDetail</see>.</returns>
        /// <example>
        /// <code>
        ///   [Flags]
        ///   private enum Days
        ///   {
        ///       None = 0,
        ///       Fri = 1,
        ///       Sat = 1 &lt;&lt; 1,
        ///       Sun = 1 &lt;&lt; 2,
        ///       Weekend = Sat | Sun
        ///   };
        /// 
        ///   Console.WriteLine(Days.Weekend.GetValueDetail());   // Outputs 'Weekend' = 3 [2 flags]
        ///   Console.WriteLine(Days.Sat.GetValueDetail());       // Outputs 'Sat' = 2
        /// </code>
        /// </example>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   The enum of type <typeparamref name="TEnum"/> doesn't contain the <paramref name="value"/> specified.
        /// </exception>
        [UsedImplicitly]
        public static ExtendedEnum<TEnum>.ValueDetail GetValueDetail<TEnum>(
            this TEnum value,
            bool includeImplicit = false)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return ExtendedEnum<TEnum>.GetValueDetail(value, includeImplicit);
        }

        /// <summary>
        ///   Tries to get the enum's <see cref="ExtendedEnum{TEnum}.ValueDetail">ValueDetail</see>.
        /// </summary>
        /// <typeparam name="TEnum">The enum type.</typeparam>
        /// <param name="value">The enum value.</param>
        /// <param name="valueDetail">
        ///   <para>The value detail.</para>
        ///   <para>If a value detail isn't found then the output is <see langword="null">null</see>.</para>
        /// </param>
        /// <param name="includeImplicit">
        ///   <para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        ///   <para>By default this is set to <see langword="false"/>.</para>
        /// </param>
        /// <returns>
        ///   Returns <see langword="true"/> if the <paramref name="valueDetail"/> was found; otherwise returns <see langword="false"/>.
        /// </returns>
        [UsedImplicitly]
        public static bool TryGetValueDetail<TEnum>(
            this TEnum value,
            out ExtendedEnum<TEnum>.ValueDetail valueDetail,
            bool includeImplicit = false)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return ExtendedEnum<TEnum>.TryGetValueDetail(value, out valueDetail, includeImplicit);
        }

        /// <summary>
        ///   Gets the <see cref="long"/> value equivalent of an enum
        /// </summary>
        /// <typeparam name="TEnum">The enum type.</typeparam>
        /// <param name="value">  this is set to <see langword="false"/>.</para>
        /// </param>
        /// <returns>
        ///   Returns <see langword="true"/> if the <paramref name="value"/> was found; otherwise returns <see langword="false"/>.
        /// </returns>
        [UsedImplicitly]
        public static bool TryGetLong<TEnum>(this TEnum value, out long vLong, bool includeImplicit = false)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return ExtendedEnum<TEnum>.TryGetLong(value, out vLong, includeImplicit);
        }

        /// <summary>
        ///   Gets the name of the enum.
        /// </summary>
        /// <typeparam name="TEnum">The enum type.</typeparam>
        /// <param name="value">The enum value.</param>
        /// <param name="includeImplicit">
        ///   <para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        ///   <para>By default this is set to <see langword="false"/>.</para>
        /// </param>
        /// <returns>A <see cref="string"/> containing the enum's name.</returns>
        [UsedImplicitly]
        [NotNull]
        public static string GetName<TEnum>(this TEnum value, bool includeImplicit = false)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return ExtendedEnum<TEnum>.GetName(value, includeImplicit);
        }

        /// <summary>
        ///   Tries to get the name of an enum.
        /// </summary>
        /// <typeparam name="TEnum">The enum type.</typeparam>
        /// <param name="value">The enum value.</param>
        /// <param name="name">
        ///   <para>The name.</para>
        ///   <para>If a name isn't found then the output is <see langword="null"/>.</para>
        /// </param>
        /// <param name="includeImplicit">
        ///   <para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        ///   <para>By default this is set to <see langword="false"/>.</para>
        /// </param>
        /// <returns>
        ///   Returns <see langword="true"/> if the name was found; otherwise returns <see langword="false"/>.
        /// </returns>
        [UsedImplicitly]
        public static bool TryGetName<TEnum>(this TEnum value, out string name, bool includeImplicit = false)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return ExtendedEnum<TEnum>.TryGetName(value, out name, includeImplicit);
        }

        /// <summary>
        ///   Gets the enum's <see cref="System.ComponentModel.DescriptionAttribute"/>.
        /// </summary>
        /// <typeparam name="TEnum">The enum type.</typeparam>
        /// <param name="value">The enum value.</param>
        /// <returns>A <see cref="System.String"/> containing the description (if any).</returns>
        /// <example>
        /// <code>
        ///   [Flags]
        ///   private enum Days
        ///   {
        ///      None,
        ///      Fri = 1,
        ///      Sat = 1 &lt;&lt; 1,
        ///      Sun = 1 &lt;&lt; 2,
        ///      [Description("The best days of the week.")] Weekend = Fri | Sat
        ///   };
        /// 
        ///   Console.WriteLine(Days.Weekend.GetDescription());   // Outputs "The best days of the week"
        /// </code>
        /// </example>
        /// <remarks>
        ///   If more than one enum of the same type has the same integral value all of their descriptions will
        ///   be concatenated into a single result, each on a new line.
        /// </remarks>
        [UsedImplicitly]
        [NotNull]
        public static string GetDescription<TEnum>(this TEnum value)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return ExtendedEnum<TEnum>.GetDescription(value);
        }

        /// <summary>
        ///   Tries to get the enum's <see cref="System.ComponentModel.DescriptionAttribute"/>.
        /// </summary>
        /// <typeparam name="TEnum">The enum type.</typeparam>
        /// <param name="value">The enum value.</param>
        /// <param name="name">
        ///   <para>The description.</para>
        ///   <para>If no description is found then the output is an <see cref="String.Empty">empty string</see>.</para>
        /// </param>
        /// <returns>
        ///   Returns <see langword="true"/> if a description was found; otherwise returns <see langword="false"/>.
        /// </returns>
        /// <remarks>
        ///   If more than one enum of the same type has the same integral value all of their descriptions will
        ///   be concatenated into a single result, each on a new line.
        /// </remarks>
        [UsedImplicitly]
        public static bool TryGetDescription<TEnum>(this TEnum value, out string name)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return ExtendedEnum<TEnum>.TryGetDescription(value, out name);
        }

        /// <summary>
        ///   Determines whether the specified flags are set on the value.
        /// </summary>
        /// <typeparam name="TEnum">The enum type.</typeparam>
        /// <param name="value">The enum value.</param>
        /// <param name="flags">The flags to check.</param>
        /// <param name="includeImplicit">
        ///   <para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        ///   <para>By default this is set to <see langword="false"/>.</para>
        /// </param>
        /// <returns>
        ///   Returns <see langword="true"/> if all the specified <paramref name="flags"/> are set; otherwise returns <see langword="false"/>.
        /// </returns>
        /// <example>
        /// <code>
        ///   [Flags]
        ///   private enum Days
        ///   {
        ///      None,
        ///      Fri = 1,
        ///      Sat = 1 &lt;&lt; 1,
        ///      Sun = 1 &lt;&lt; 2,
        ///      Weekend = Fri | Sat
        ///   };
        /// 
        ///   Console.WriteLine(Days.Weekend.AreSet(Days.Sun | Days.Fri));  // False
        ///   Console.WriteLine(Days.Weekend.AreSet(Days.Sun));             // False
        ///   Console.WriteLine(Days.Weekend.AreSet(Days.Fri));             // True
        /// </code>
        /// </example>
        [UsedImplicitly]
        public static bool AreSet<TEnum>(this TEnum value, TEnum flags, bool includeImplicit = false)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return ExtendedEnum<TEnum>.AreSet(value, flags, includeImplicit);
        }

        /// <summary>
        ///   Determines whether the specified flags are clear of the value.
        /// </summary>
        /// <typeparam name="TEnum">The enum type.</typeparam>
        /// <param name="value">The enum value.</param>
        /// <param name="flags">The flags to check.</param>
        /// <param name="includeImplicit">
        ///   <para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        ///   <para>By default this is set to <see langword="false"/>.</para>
        /// </param>
        /// <returns>
        ///   Returns <see langword="true"/> if the <paramref name="flags"/> are clear of the specified enum;
        ///   otherwise returns <see langword="false"/>.
        /// </returns>
        /// <example>
        /// <code>
        ///   [Flags]
        ///   private enum Days
        ///   {
        ///      None,
        ///      Fri = 1,
        ///      Sat = 1 &lt;&lt; 1,
        ///      Sun = 1 &lt;&lt; 2,
        ///      Weekend = Fri | Sat
        ///   };
        /// 
        ///   Console.WriteLine(Days.Weekend.AreClear(Days.Fri));               // False
        ///   Console.WriteLine(Days.Weekend.AreClear(Days.Fri | Days.Sun));    // False
        ///   Console.WriteLine(Days.Weekend.AreClear(Days.Sun));               // True
        ///   Console.WriteLine(Days.Weekend.AreClear(Days.Sun | Days.None));   // True
        /// </code>
        /// </example>
        [UsedImplicitly]
        public static bool AreClear<TEnum>(this TEnum value, TEnum flags, bool includeImplicit = false)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return ExtendedEnum<TEnum>.AreClear(value, flags, includeImplicit);
        }

        /// <summary>
        ///   Sets the flags on the value.
        /// </summary>
        /// <typeparam name="TEnum">The enum type.</typeparam>
        /// <param name="value">The enum value.</param>
        /// <param name="flags">The flags to set.</param>
        /// <param name="includeImplicit">
        ///   <para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        ///   <para>By default this is set to <see langword="false"/>.</para>
        /// </param>
        /// <returns>The result of the <paramref name="flags"/> set to <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   The <paramref name="value"/> cannot be set with the <paramref name="flags"/> specified.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   <para><typeparamref name="TEnum"/> is not an enum.</para>
        ///   <para>-or-</para>
        ///   <para><typeparamref name="TEnum"/> is not a flag enum.</para>
        /// </exception>
        [UsedImplicitly]
        [Pure]
        public static TEnum Set<TEnum>(this TEnum value, TEnum flags, bool includeImplicit = false)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return ExtendedEnum<TEnum>.Set(value, flags, includeImplicit);
        }

        /// <summary>
        ///   Tries to set the specified flags on the value.
        /// </summary>
        /// <typeparam name="TEnum">The enum type.</typeparam>
        /// <param name="value">The enum value.</param>
        /// <param name="flags">The flags to set.</param>
        /// <param name="result">
        ///   The result of the <paramref name="flags"/> set to <paramref name="value"/>.
        /// </param>
        /// <param name="includeImplicit">
        ///   <para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        ///   <para>By default this is set to <see langword="false"/>.</para>
        /// </param>
        /// <returns>
        ///   Returns <see langword="true"/> if the flags can be set; otherwise returns <see langword="false"/>.
        /// </returns>
        [UsedImplicitly]
        [Pure]
        public static bool TrySet<TEnum>(this TEnum value, TEnum flags, out TEnum result, bool includeImplicit = false)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return ExtendedEnum<TEnum>.TrySet(value, flags, out result, includeImplicit);
        }

        /// <summary>
        ///   Clears the specified flags on the value.
        /// </summary>
        /// <typeparam name="TEnum">The enum type.</typeparam>
        /// <param name="value">The enum value.</param>
        /// <param name="flags">The flags to clear.</param>
        /// <param name="includeImplicit">
        ///   <para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        ///   <para>By default this is set to <see langword="false"/>.</para>
        /// </param>
        /// <returns>
        ///   The result of the <paramref name="flags"/> cleared from <paramref name="value"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   The enum cannot be cleared of the specified <paramref name="flags"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   <para><typeparamref name="TEnum"/> is not an enum.</para>
        ///   <para>-or-</para>
        ///   <para><typeparamref name="TEnum"/> is not a flag enum.</para>
        /// </exception>
        [UsedImplicitly]
        [Pure]
        public static TEnum Clear<TEnum>(this TEnum value, TEnum flags, bool includeImplicit = false)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return ExtendedEnum<TEnum>.Clear(value, flags, includeImplicit);
        }

        /// <summary>
        ///   Tries to clear the flags on the value.
        /// </summary>
        /// <typeparam name="TEnum">The enum type.</typeparam>
        /// <param name="value">The enum value.</param>
        /// <param name="flags">The flags to clear.</param>
        /// <param name="result">
        ///   The result of the <paramref name="flags"/> cleared from <paramref name="value"/>.
        /// </param>
        /// <param name="includeImplicit">
        ///   <para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        ///   <para>By default this is set to <see langword="false"/>.</para>
        /// </param>
        /// <returns>
        ///   Returns <see langword="true"/> if the flags can be cleared; otherwise returns <see langword="false"/>.
        /// </returns>
        [UsedImplicitly]
        [Pure]
        public static bool TryClear<TEnum>(
            this TEnum value,
            TEnum flags,
            out TEnum result,
            bool includeImplicit = false)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return ExtendedEnum<TEnum>.TryClear(value, flags, out result, includeImplicit);
        }

        /// <summary>
        ///   Gets the intersection of the enum and the specified flags (returns a flags which are set on both).
        /// </summary>
        /// <typeparam name="TEnum">The enum type.</typeparam>
        /// <param name="value">The enum value.</param>
        /// <param name="flags">The flags.</param>
        /// <param name="includeImplicit">
        ///   <para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        ///   <para>By default this is set to <see langword="false"/>.</para>
        /// </param>
        /// <returns>
        ///   The intersection of <paramref name="value"/> and <paramref name="flags"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        ///   <para><typeparamref name="TEnum"/> is not an enum.</para>
        ///   <para>-or-</para>
        ///   <para><typeparamref name="TEnum"/> is not a flag enum.</para>
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   The <paramref name="value"/> type could not be intersected with the <paramref name="flags"/> specified.
        /// </exception>
        [UsedImplicitly]
        [Pure]
        public static TEnum Intersect<TEnum>(this TEnum value, TEnum flags, bool includeImplicit = false)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return ExtendedEnum<TEnum>.Intersect(value, flags, includeImplicit);
        }

        /// <summary>
        ///   Tries to get the intersection of the enum and the specified flags (returns a flags which are set on both).
        /// </summary>
        /// <typeparam name="TEnum">The enum type.</typeparam>
        /// <param name="value">The enum value.</param>
        /// <param name="flags">The flags.</param>
        /// <param name="result">The intersection of <paramref name="value"/> and <paramref name="flags"/>.</param>
        /// <param name="includeImplicit">
        ///   <para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        ///   <para>By default this is set to <see langword="false"/>.</para>
        /// </param>
        /// <returns>
        ///   Returns <see langword="true"/> if the flags can be intersected; otherwise returns <see langword="false"/>.
        /// </returns>
        [UsedImplicitly]
        [Pure]
        public static bool TryIntersect<TEnum>(
            this TEnum value,
            TEnum flags,
            out TEnum result,
            bool includeImplicit = false)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return ExtendedEnum<TEnum>.TryIntersect(value, flags, out result, includeImplicit);
        }

        /// <summary>
        ///   Inverts all the valid flags.
        /// </summary>
        /// <typeparam name="TEnum">The enum type.</typeparam>
        /// <param name="value">The enum value.</param>
        /// <param name="includeImplicit">
        ///   <para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        ///   <para>By default this is set to <see langword="false"/>.</para>
        /// </param>
        /// <returns>The inverted flags for the specified <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   The <paramref name="value"/> provided could not be inverted.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   <para><typeparamref name="TEnum"/> is not an enum.</para>
        ///   <para>-or-</para>
        ///   <para><typeparamref name="TEnum"/> is not a flag enum.</para>
        /// </exception>
        /// <example>
        /// <code>
        ///   [Flags]
        ///   private enum Days
        ///   {
        ///      None,
        ///      Fri = 1,
        ///      Sat = 1 &lt;&lt; 1,
        ///      Sun = 1 &lt;&lt; 2,
        ///      Weekend = Fri | Sat,
        ///      All = Sun | Weekend
        ///   };
        /// 
        ///   Console.WriteLine(ExtendedEnum&lt;Days&gt;.Invert(Days.All));       // Outputs None
        ///   Console.WriteLine(ExtendedEnum&lt;Days&gt;.Invert(Days.Weekend));   // Outputs Sun
        /// </code>
        /// </example>
        [UsedImplicitly]
        [Pure]
        public static TEnum Invert<TEnum>(this TEnum value, bool includeImplicit = false)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return ExtendedEnum<TEnum>.Invert(value, includeImplicit);
        }

        /// <summary>
        ///   Tries to invert all the valid flags.
        /// </summary>
        /// <typeparam name="TEnum">The enum type.</typeparam>
        /// <param name="value">The enum value.</param>
        /// <param name="result">
        ///   The inverted flags for the specified <paramref name="value"/>.
        /// </param>
        /// <param name="includeImplicit">
        ///   <para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        ///   <para>By default this is set to <see langword="false"/>.</para>
        /// </param>
        /// <returns>
        ///   Returns <see langword="true"/> if the flags can be inverted; otherwise returns <see langword="false"/>.
        /// </returns>
        [UsedImplicitly]
        [Pure]
        public static bool TryInvert<TEnum>(this TEnum value, out TEnum result, bool includeImplicit = false)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return ExtendedEnum<TEnum>.TryInvert(value, out result, includeImplicit);
        }

        /// <summary>
        ///   Combines all of the specified flags.
        /// </summary>
        /// <typeparam name="TEnum">The enum type.</typeparam>
        /// <param name="flags">The flags.</param>
        /// <param name="includeImplicit">
        ///   <para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        ///   <para>By default this is set to <see langword="false"/>.</para>
        /// </param>
        /// <returns>The combined <paramref name="flags"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   The flags of type <typeparamref name="TEnum"/> cannot be combined.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   <para><typeparamref name="TEnum"/> is not an enum.</para>
        ///   <para>-or-</para>
        ///   <para><typeparamref name="TEnum"/> is not a flag enum.</para>
        /// </exception>
        [UsedImplicitly]
        [Pure]
        public static TEnum Combine<TEnum>([NotNull] this IEnumerable<TEnum> flags, bool includeImplicit = false)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return ExtendedEnum<TEnum>.Combine(flags, includeImplicit);
        }

        /// <summary>
        ///   Tries to combine all of the specified flags.
        /// </summary>
        /// <typeparam name="TEnum">The enum type.</typeparam>
        /// <param name="flags">The flags.</param>
        /// <param name="result">
        ///   <para>The combined <paramref name="flags"/>.</para>
        ///   <para>If unsuccessful then this returns an enum with the raw value of zero.</para>
        /// </param>
        /// <param name="includeImplicit">
        ///   <para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        ///   <para>By default this is set to <see langword="false"/>.</para>
        /// </param>
        /// <returns>
        ///   Returns <see langword="true"/> if the flags can be inverted; otherwise returns <see langword="false"/>.
        /// </returns>
        [UsedImplicitly]
        [Pure]
        public static bool TryCombine<TEnum>(
            [NotNull] this IEnumerable<TEnum> flags,
            out TEnum result,
            bool includeImplicit = false)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return ExtendedEnum<TEnum>.TryCombine(flags, out result, includeImplicit);
        }

        /// <summary>
        ///   Splits the specified flags.
        /// </summary>
        /// <typeparam name="TEnum">The enum type.</typeparam>
        /// <param name="flags">The flags to split.</param>
        /// <param name="includeImplicit">
        ///   <para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        ///   <para>By default this is set to false.</para>
        /// </param>
        /// <param name="includeCombinations">
        ///   <para>If set to <see langword="true"/> will split into the minimum number of flags (allows combinations).</para>
        ///   <para>By default this is set to <see langword="false"/>.</para>
        /// </param>
        /// <returns>The result of the split <paramref name="flags"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   Couldn't split the <paramref name="flags"/> for type <typeparamref name="TEnum"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///   <para><typeparamref name="TEnum"/> is not an enum.</para>
        ///   <para>-or-</para>
        ///   <para><typeparamref name="TEnum"/> is not a flag enum.</para>
        /// </exception>
        [UsedImplicitly]
        [NotNull]
        public static IEnumerable<TEnum> SplitFlags<TEnum>(
            this TEnum flags,
            bool includeImplicit = false,
            bool includeCombinations = false)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return ExtendedEnum<TEnum>.SplitFlags(flags, includeImplicit, includeCombinations);
        }

        /// <summary>
        ///   Tries to split all the specified flags.
        /// </summary>
        /// <typeparam name="TEnum">The enum type.</typeparam>
        /// <param name="flags">The flags to split.</param>
        /// <param name="result">
        ///   <para>The result of the split <paramref name="flags"/>.</para>
        ///   <para>The default result if the split fails is <see cref="System.Linq.Enumerable.Empty{TEnum}"/>.</para>
        /// </param>
        /// <param name="includeImplicit">
        ///   <para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        ///   <para>By default this is set to <see langword="false"/>.</para>
        /// </param>
        /// <param name="includeCombinations">
        ///   <para>If set to <see langword="true"/> will split into the minimum number of flags (allows combinations).</para>
        ///   <para>By default this is set to <see langword="false"/>.</para>
        /// </param>
        /// <returns>
        ///   Returns <see langword="true"/> if the flags can be split successfully; otherwise returns <see langword="false"/>.
        /// </returns>
        [UsedImplicitly]
        public static bool TrySplitFlags<TEnum>(
            this TEnum flags,
            out IEnumerable<TEnum> result,
            bool includeImplicit = false,
            bool includeCombinations = false)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return ExtendedEnum<TEnum>.TrySplitFlags(flags, out result, includeImplicit, includeCombinations);
        }
    }
}