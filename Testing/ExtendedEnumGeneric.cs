#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities
// File: ExtendedEnum.cs
// 
// This software, its object code and source code and all modifications made to
// the same (the “Software”) are, and shall at all times remain, the proprietary
// information and intellectual property rights of Web Applications (UK) Limited. 
// You are only entitled to use the Software as expressly permitted by Web
// Applications (UK) Limited within the Software Customisation and
// Licence Agreement (the “Agreement”).  Any copying, modification, decompiling,
// distribution, licensing, sale, transfer or other use of the Software other than
// as expressly permitted in the Agreement is expressly forbidden.  Web
// Applications (UK) Limited reserves its rights to take action against you and
// your employer in accordance with its contractual and common law rights
// (including injunctive relief) should you breach the terms of the Agreement or
// otherwise infringe its copyright or other intellectual property rights in the
// Software.
// 
// © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
#endregion

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace WebApplications.Testing
{
    /// <summary>
    /// Stores additional information about an <see cref="Enum"/> type including the names of its enumerations,
    /// their <see cref="System.ComponentModel.DescriptionAttribute">descriptions</see> (if any), its values
    /// and its raw values as a <see cref="long"/>.
    /// </summary>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    public static class ExtendedEnum<TEnum> where TEnum : struct, IComparable, IFormattable, IConvertible
    {
        // ReSharper disable StaticFieldInGenericType
        /// <summary>
        /// A <see cref="bool"/> for whether the type is actually an enum.
        /// </summary>
        [UsedImplicitly] public static readonly bool IsEnum;

        /// <summary>
        /// A <see cref="bool"/> for whether the enum is a flag enum.
        /// </summary>
        /// <remarks>
        /// For flag enums define enumeration constants in powers of two, this is so individual flags in combined
        /// enumeration constants don't overlap.
        /// </remarks>
        /// <seealso cref="System.FlagsAttribute"/>
        [UsedImplicitly] public static readonly bool IsFlag;

        /// <summary>
        /// The enumerator for all explicitly defined enum values.
        /// </summary>
        [UsedImplicitly] [NotNull] public static readonly IEnumerable<ValueDetail> ValueDetails;

        /// <summary>
        /// Contains the value details cached in reverse.
        /// </summary>
        /// <remarks>
        /// This is used for performance at the cost of a small memory hit, without a pre-reversed collection every time
        /// a build occurred a copy would need to be made by LINQ to reverse.
        /// </remarks>
        [UsedImplicitly] [NotNull] public static readonly IEnumerable<ValueDetail> ValueDetailsReversed;

        #region Convenient value accessor properties.
        // ReSharper disable PossibleNullReferenceException
        /// <summary>
        /// The enumerator for all explicitly defined non-combination enum values.
        /// </summary>
        [UsedImplicitly]
        [NotNull]
        public static IEnumerable<ValueDetail> IndividualValueDetails
        {
            get
            {
                Check(TriState.Yes);
                return ValueDetails.Where(vd => !vd.IsCombination);
            }
        }

        /// <summary>
        /// The enumerator for all explicitly defined combination enum values.
        /// </summary>
        [UsedImplicitly]
        [NotNull]
        public static IEnumerable<ValueDetail> CombinedValueDetails
        {
            get
            {
                Check(TriState.Yes);
                return ValueDetails.Where(vd => vd.IsCombination);
            }
        }

        /// <summary>
        /// The enumerator for all explicitly defined enum values.
        /// </summary>
        [UsedImplicitly]
        [NotNull]
        public static IEnumerable<TEnum> Values
        {
            get { return ValueDetails.Select(vd => vd.Value); }
        }

        /// <summary>
        /// <para>The enumerator for all explicitly defined individual enum values.</para>
        /// <para>This is identical to <see cref="Values"/> for a non-flag enum.</para>
        /// </summary>
        [UsedImplicitly]
        [NotNull]
        public static IEnumerable<TEnum> IndividualValues
        {
            get { return IndividualValueDetails.Select(vd => vd.Value); }
        }

        /// <summary>
        /// <para>The enumerator for all explicitly defined combination enum values.</para>
        /// <para>This is empty for a non-flag enum.</para>
        /// </summary>
        [UsedImplicitly]
        [NotNull]
        public static IEnumerable<TEnum> CombinationValues
        {
            get { return CombinedValueDetails.Select(vd => vd.Value); }
        }

        /// <summary>
        /// The enumerator for all explicitly defined enum values (the raw value).
        /// </summary>
        [UsedImplicitly]
        [NotNull]
        public static IEnumerable<long> RawValues
        {
            get { return ValueDetails.Select(vd => vd.RawValue); }
        }

        /// <summary>
        /// <para>The enumerator for all explicitly defined individual enum values (the raw value).</para>
        /// <para>This is identical to <see cref="RawValues"/> for a non-flag enum.</para>
        /// </summary>
        [UsedImplicitly]
        [NotNull]
        public static IEnumerable<long> IndividualRawValues
        {
            get { return IndividualValueDetails.Select(vd => vd.RawValue); }
        }

        /// <summary>
        /// <para>The enumerator for all explicitly defined combination enum values (the raw value).</para>
        /// <para>This is empty for a non-flag enum.</para>
        /// </summary>
        [UsedImplicitly]
        [NotNull]
        public static IEnumerable<long> CombinationRawValues
        {
            get { return CombinedValueDetails.Select(vd => vd.RawValue); }
        }

        /// <summary>
        /// The enumerator for all explicitly defined enum names.
        /// </summary>
        [UsedImplicitly]
        [NotNull]
        public static IEnumerable<string> Names
        {
            get { return ValueDetails.SelectMany(vd => vd.AllNames); }
        }

        /// <summary>
        /// <para>The enumerator for all explicitly defined individual enum names.</para>
        /// <para>This is identical to <see cref="Names"/> for a non-flag enum.</para>
        /// </summary>
        [UsedImplicitly]
        [NotNull]
        public static IEnumerable<string> IndividualNames
        {
            get { return IndividualValueDetails.SelectMany(vd => vd.AllNames); }
        }

        /// <summary>
        /// <para>The enumerator for all explicitly defined combination enum names.</para>
        /// <para>This is empty for a non-flag enum.</para>
        /// </summary>
        [UsedImplicitly]
        [NotNull]
        public static IEnumerable<string> CombinationNames
        {
            get { return CombinedValueDetails.SelectMany(vd => vd.AllNames); }
        }

        // ReSharper restore PossibleNullReferenceException
        #endregion

        #region None
        /// <summary>
        /// The <see cref="ValueDetail"/> that represents no flags set.
        /// </summary>
        [NotNull] private static readonly ValueDetail _noneValueDetail;

        /// <summary>
        /// Returns the <see cref="ValueDetail"/> that represents no flags set.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public static ValueDetail NoneValueDetail
        {
            get
            {
                Check(TriState.Yes);
                return _noneValueDetail;
            }
        }

        /// <summary>
        /// Returns <see langword="true"/> if the enum is a flag enum and there is an explicit definition for no flags being set.
        /// </summary>
        [UsedImplicitly]
        public static bool ExplicitNone
        {
            get
            {
                Check(TriState.Yes);
                return _noneValueDetail.IsExplicit;
            }
        }

        /// <summary>
        /// Returns the value that represents no flags being set.
        /// </summary>
        [UsedImplicitly]
        public static TEnum NoneValue
        {
            get
            {
                Check(TriState.Yes);
                return _noneValueDetail.Value;
            }
        }

        /// <summary>
        /// Returns the enum name that represents no flags being set.
        /// </summary>
        [UsedImplicitly]
        [NotNull]
        public static string NoneName
        {
            get
            {
                Check(TriState.Yes);
                return _noneValueDetail.Name;
            }
        }

        /// <summary>
        /// Returns the raw integral value that represents no flags being set.
        /// </summary>
        [UsedImplicitly]
        public static long NoneRawValue
        {
            get
            {
                Check(TriState.Yes);
                return 0L;
            }
        }
        #endregion

        #region All
        /// <summary>
        /// A <see cref="ValueDetail"/> that represents all of the flags being set.
        /// </summary>
        [NotNull] private static readonly ValueDetail _allValueDetail;

        /// <summary>
        /// Returns the <see cref="ValueDetail"/> that represents all of the flags being set.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public static ValueDetail AllValueDetails
        {
            get
            {
                Check(TriState.Yes);
                return _allValueDetail;
            }
        }

        /// <summary>
        /// Returns <see langword="true"/> if the enum is a flag enum and all the flags are defined explicitly.
        /// </summary>
        [UsedImplicitly]
        public static bool ExplicitAll
        {
            get
            {
                Check(TriState.Yes);
                return _allValueDetail.IsExplicit;
            }
        }

        /// <summary>
        /// Returns a value that represents all flags being set.
        /// </summary>
        public static TEnum AllValue
        {
            get
            {
                Check(TriState.Yes);
                return _allValueDetail.Value;
            }
        }

        /// <summary>
        /// Returns the name of the <see cref="ValueDetail"/> that represents all of the flags being set.
        /// </summary>
        [UsedImplicitly]
        [NotNull]
        public static string AllName
        {
            get
            {
                Check(TriState.Yes);
                return _allValueDetail.Name;
            }
        }

        /// <summary>
        /// Returns the raw integral value that represents all of the flags being set.
        /// </summary>
        [UsedImplicitly]
        public static long AllRawValue
        {
            get
            {
                Check(TriState.Yes);
                return _allValueDetail.RawValue;
            }
        }
        #endregion

        #region Lookups
        /*
         * Strongly typed lookup dictionaries for speed.
         */

        /// <summary>
        /// Looks up the <see cref="ValueDetail"/> from its value (the enum).
        /// </summary>
        [NotNull] private static readonly Dictionary<TEnum, ValueDetail> _valueLookup =
            new Dictionary<TEnum, ValueDetail>();

        /// <summary>
        /// Looks up the <see cref="ValueDetail"/> from its raw value.
        /// </summary>
        [NotNull] private static readonly Dictionary<long, ValueDetail> _rawValueLookup =
            new Dictionary<long, ValueDetail>();

        /// <summary>
        /// Looks up the <see cref="ValueDetail"/> from its name.
        /// </summary>
        [NotNull] private static readonly Dictionary<string, ValueDetail> _nameLookup =
            new Dictionary<string, ValueDetail>();

        /// <summary>
        /// A cache of the implicit value details so that when requested they can be retrieved rather than recomputed.
        /// </summary>
        [NotNull] private static readonly ConcurrentDictionary<long, ValueDetail> _implicitLookup;
        #endregion

        // ReSharper restore StaticFieldInGenericType

        /// <summary>
        /// Gets information about an enumeration.
        /// </summary>
        /// <remarks>
        /// Static constructors are called automatically before the first instance is created or if any static members are referenced.
        /// </remarks>
        static ExtendedEnum()
        {
            IsEnum = typeof (TEnum).IsEnum;

            if (!IsEnum)
            {
                // We're not an enum so we're done.
                ValueDetails = Enumerable.Empty<ValueDetail>();
                return;
            }

            // Check if this is a flag enum.
            IsFlag = Attribute.IsDefined(typeof (TEnum), typeof (FlagsAttribute));

            // Calculate the 'all value' as we go.
            long allRawValue = 0L;
            ValueDetail noneValueDetail = null;

            // Iterate through the enumerations fields (this is actually what enum does repeatedly
            // under the hood, in a private method on Type called GetEnumData, unlike our class it
            // doesn't remember the answers, or provide as much information)
            foreach (FieldInfo field in
                typeof (TEnum).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
            {
                // From the field we can get the name, value and description
                string name = field.Name;
                TEnum value = (TEnum) field.GetRawConstantValue();
                long rawValue = Convert.ToInt64(value);
                string description = field
                    .GetCustomAttributes(typeof (DescriptionAttribute), false)
                    .Cast<DescriptionAttribute>()
                    .Where(d => d != null && !string.IsNullOrWhiteSpace(d.Description))
                    .Aggregate(string.Empty,
                               (t, d) =>
                               // ReSharper disable PossibleNullReferenceException
                               t + (t == string.Empty ? string.Empty : Environment.NewLine) + d.Description);
                // ReSharper restore PossibleNullReferenceException

                // Calculate number of flags
                ushort flags;
                if (IsFlag)
                {
                    // Get bit count
                    flags = 0;
                    if (rawValue != 0)
                    {
                        long l = rawValue;
                        // Rotate counting bits.
                        while (l > 0)
                        {
                            if ((l & 1) > 0)
                                flags++;
                            l >>= 1;
                        }
                    }
                }
                else
                    flags = 1;

                // Check if we've already seen the value (essentially a duplicate name).
                ValueDetail valueDetail;
                if (_valueLookup.TryGetValue(value, out valueDetail))
                {
                    // We've already loaded the same value before, this is a duplicate definition
                    valueDetail.AddDefinition(name, description);
                }
                else
                {
                    // This is the first time we've seen this value, create a new value details object
                    // and add it to value and RawValue lookups.
                    valueDetail = new ValueDetail(value, rawValue, flags, name, true, description);
                    _valueLookup.Add(value, valueDetail);
                    _rawValueLookup.Add(rawValue, valueDetail);

                    // If we're a flag update none/all value details where appropriate.
                    if (IsFlag)
                    {
                        // If this is a flag enum and the raw value is 0,
                        // we have found the details for the 'none' value.
                        if (rawValue == 0L)
                            noneValueDetail = valueDetail;

                        // Calculate the all raw value.
                        allRawValue |= rawValue;
                    }
                }

                // We always have a new name to add to the lookup
                _nameLookup.Add(name, valueDetail);
            }

            // Sort the value details by key.
            ValueDetails = _valueLookup.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value).ToList();
            ValueDetailsReversed = ValueDetails.Reverse().ToList();

            // If we're not a flag we're done
            if (!IsFlag) return;

            // Create implicit lookup cache (only used by flag enums).
            _implicitLookup = new ConcurrentDictionary<long, ValueDetail>();

            // Set or create the none value detail
            _noneValueDetail = noneValueDetail ??
                               new ValueDetail((TEnum) Enum.ToObject(typeof (TEnum), 0L), 0L, 0, string.Empty, false,
                                               string.Empty);

            // Try to find or create an explicit all value from the long equivalent.
            if (!_rawValueLookup.TryGetValue(allRawValue, out _allValueDetail))
                _allValueDetail = CreateImplicitValueDetail(allRawValue);
        }

        /// <summary>
        /// Checks that the <see cref="object"/> is an enum as well as whether the object is/isn't a flag (depending on the
        /// parameter specified). An exception is thrown if the object is <b>not</b> an enum or if the object is/isn't a flag. 
        /// </summary>
        /// <param name="isFlag">
        /// <list type="bullet">
        /// <item><description>If set to <see cref="TriState.Yes"/> check that the object is an enum and that it <b>is</b> a flag.</description></item>
        /// <item><description>If set to <see cref="TriState.No"/> check that the object is an enum and that it <b>is not</b> a flag.</description></item>
        /// <item><description>If no value is specified then only check that the object is an enum.</description></item>
        /// </list></param>
        /// <example><code>
        /// [Flags]
        /// private enum Days
        /// {
        ///     Mon,
        ///     Tues,
        ///     Wed,
        ///     Thurs,
        ///     Fri
        /// };
        /// 
        /// ExtendedEnum&lt;Days&gt;.Check();               // Checks if Days is an enum. If it isn't an exception will be thrown.
        /// ExtendedEnum&lt;Days&gt;.Check(TriState.Yes);   // Also checks if Days is a flag. If it is NOT a flag or is NOT an enum then an exception will be thrown.
        /// ExtendedEnum&lt;Days&gt;.Check(TriState.No);    // Also checks if Days isn't a flag. If it IS a flag or is NOT an enum then an exception will be thrown.
        /// </code></example>
        [UsedImplicitly]
        public static void Check(TriState isFlag = default(TriState))
        {
            if (!IsEnum)
                throw new ArgumentException(string.Format(Resources.ExtendedEnumGeneric_TypeIsNotAnEnum, typeof (TEnum).FullName));
            if (isFlag == TriState.Undefined)
                return;
            if (isFlag == TriState.Yes)
            {
                if (!IsFlag)
                    throw new ArgumentException(string.Format(Resources.ExtendedEnumGeneric_TypeIsNotFlagEnum, typeof (TEnum).FullName));
                return;
            }
            if (IsFlag)
                throw new ArgumentException(string.Format(Resources.ExtendedEnumGeneric_TypeIsFlagEnum, typeof(TEnum).FullName));
        }

        /// <summary>
        /// Tries to get an implicit enum's <see cref="ValueDetail"/> with the specified <see cref="long"/> value.
        /// </summary>
        /// <param name="rawValue">The raw value.</param>
        /// <param name="valueDetail"><para>The value detail.</para>
        /// <para>If it fails to get the value then the output is a <see langword="null"/>.</para></param>
        /// <returns>Returns <see langword="true"/> if successful; otherwise returns <see langword="false"/>.</returns>
        private static bool TryGetImplicitValueDetail(long rawValue, out ValueDetail valueDetail)
        {
            // If raw value is zero, then we already have a value detail.
            if (rawValue == 0)
            {
                valueDetail = _noneValueDetail;
                return true;
            }

            if (rawValue == _allValueDetail.RawValue)
            {
                valueDetail = _allValueDetail;
                return true;
            }

            // If we have any invalid flags we fail.
            if ((rawValue & ~_allValueDetail.RawValue) != 0)
            {
                valueDetail = null;
                return false;
            }

            // Get from or add to the cache.
            valueDetail = _implicitLookup.GetOrAdd(rawValue, CreateImplicitValueDetail);

            return true;
        }

        /// <summary>
        /// Creates an implicit enum's <see cref="ValueDetail"/> with the specified <see cref="long"/> value.
        /// </summary>
        /// <param name="rawValue">The raw value.</param>
        /// <returns>The new <see cref="ValueDetail"/>.</returns>
        /// <remarks>
        /// This does not check the validity of the raw value nor does it cache the result, it is used internally by
        /// the constructor and <see cref="TryGetImplicitValueDetail"/>once they have determined that a new implicit
        /// <see cref="ValueDetail"/> is required.
        /// </remarks>
        [NotNull]
        private static ValueDetail CreateImplicitValueDetail(long rawValue)
        {
            // Build a name
            string name = string.Empty;
            string description = string.Empty;
            ushort flags = 0;

            long rv = rawValue;
            foreach (ValueDetail vd in ValueDetailsReversed)
            {
                // If this the none value, or not a match continue
                if ((vd.RawValue == 0) ||
                    (rv & vd.RawValue) != vd.RawValue)
                    continue;

                // We have a match prepend name & description.
                name = vd.Name + (name == string.Empty ? string.Empty : " | " + name);
                if (!string.IsNullOrWhiteSpace(vd.Description))
                    description = vd.Description +
                                  (description == string.Empty ? string.Empty : " | " + description);
                flags += vd.Flags;

                // Remove flags that matched.
                rv &= ~vd.RawValue;

                // If we've got no flags left we're done.
                if (rv < 1)
                    break;
            }

            // If we have bits left, then we are an implicit flag, so add the number.
            if (rv > 0)
            {
                name = rv.ToString() + (name == string.Empty ? string.Empty : " | " + name);

                // Count remaining bits
                while (rv > 0)
                {
                    if ((rv & 1) == 1)
                        flags++;
                    rv >>= 1;
                }
            }

            // Create enum value
            TEnum value = (TEnum) Enum.ToObject(typeof (TEnum), rawValue);

            // Create actual value detail
            return new ValueDetail(value, rawValue, flags, name, false, description);
        }

        /// <summary>
        /// Tries to get the <see cref="long"/> equivalent of an implicit enum using the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="rawValue"><para>The raw value.</para>
        /// <para>The output is zero by default if no implicit enum is found.</para></param>
        /// <returns>Returns <see langword="true"/> if successful; otherwise returns <see langword="false"/>.</returns>
        private static bool TryGetImplicitLong(string name, out long rawValue)
        {
            rawValue = 0L;
            // Empty string is the implicit name of 0.
            if (string.IsNullOrWhiteSpace(name))
            {
                // If we have an implicit none, the empty string is a name for it.
                if (!_noneValueDetail.IsExplicit)
                {
                    rawValue = _noneValueDetail.RawValue;
                    return true;
                }

                // Otherwise empty string is not a valid name.
                return false;
            }

            // Split name with '|'.
            foreach (string n in name.Split('|'))
            {
                // Trim spaces from name.
                string nt = n.Trim();

                long l;
                ValueDetail vd;

                // Try looking up the name first.
                if (_nameLookup.TryGetValue(nt, out vd))
                {
                    l = vd.RawValue;
                }
                else if ((!long.TryParse(nt, out l)) ||
                         ((l & ~_allValueDetail.RawValue) != 0))
                {
                    // Wasn't a name and wasn't a valid long number.
                    rawValue = 0;
                    return false;
                }

                // Or flags together.
                rawValue |= l;
            }

            // If we got here then we parsed every component successfully.
            return true;
        }

        #region Lookups

        #region IsDefined
        /// <summary>
        /// Determines whether the specified integral value is defined in the enumeration.
        /// </summary>
        /// <param name="rawValue">The raw value.</param>
        /// <param name="includeImplicit"><para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        /// <para>By default this is set to <see langword="false"/>.</para></param>
        /// <returns>Returns <see langword="true"/> if the specified value is defined; otherwise returns <see langword="false"/>.</returns>
        [UsedImplicitly]
        public static bool IsDefined(long rawValue, bool includeImplicit = false)
        {
            Check(TriState.Undefined);

            // If we find it in the lookup return true
            if (_rawValueLookup.ContainsKey(rawValue))
                return true;

            // If we're not including implicit values, or this isn't a flag enum, then the value is not defined.
            if ((!includeImplicit) ||
                (!IsFlag) ||
                (rawValue == 0))
                return false;

            return (rawValue & ~_allValueDetail.RawValue) == 0;
        }

        /// <summary>
        /// Determines whether the specified name is defined in the enumeration.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="includeImplicit"><para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        /// <para>By default this is set to <see langword="false"/>.</para></param>
        /// <returns>Returns <see langword="true"/> if the specified name is defined; otherwise returns <see langword="false"/>.</returns>
        [UsedImplicitly]
        public static bool IsDefined([NotNull] string name, bool includeImplicit = false)
        {
            Check(TriState.Undefined);

            long rawValue;
            return TryGetLong(name, out rawValue, includeImplicit);
        }

        /// <summary>
        /// Determines whether the specified value is defined in the enumeration.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="includeImplicit"><para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        /// <para>By default this is set to <see langword="false"/>.</para></param>
        /// <returns>Returns <see langword="true"/> if the specified value is defined; otherwise returns <see langword="false"/>.</returns>
        [UsedImplicitly]
        public static bool IsDefined(TEnum value, bool includeImplicit = false)
        {
            Check(TriState.Undefined);

            long rawValue;
            return TryGetLong(value, out rawValue, includeImplicit);
        }
        #endregion

        #region GetValueDetail and TryGetValueDetail
        /// <summary>
        /// Gets the <see cref="ValueDetail"/> from the enum value specified.
        /// </summary>
        /// <param name="value">The enum value.</param>
        /// <param name="includeImplicit"><para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        /// <para>By default this is set to <see langword="false"/>.</para></param>
        /// <returns>The <see cref="ValueDetail"/> that corresponds to the <paramref name="value"/> specified.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The enum type doesn't contain the <paramref name="value"/> specified.</exception>
        [NotNull]
        [UsedImplicitly]
        public static ValueDetail GetValueDetail(TEnum value, bool includeImplicit = false)
        {
            Check(includeImplicit ? TriState.Yes : TriState.Undefined);
            ValueDetail valueDetail;
            if (!TryGetValueDetail(value, out valueDetail, includeImplicit))
                throw new ArgumentOutOfRangeException("value",
                                                      string.Format(Resources.ExtendedEnumGeneric_DoesNotContainValue,
                                                                    typeof (TEnum).FullName, value));
            return valueDetail;
        }

        /// <summary>
        /// Gets the <see cref="ValueDetail"/> from the raw value specified.
        /// </summary>
        /// <param name="rawValue">The raw value.</param>
        /// <param name="includeImplicit"><para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        /// <para>By default this is set to <see langword="false"/>.</para></param>
        /// <returns>The <see cref="ValueDetail"/> that corresponds to the <paramref name="rawValue"/> specified.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The enum type doesn't contain the <paramref name="rawValue"/> specified.</exception>
        [NotNull]
        [UsedImplicitly]
        public static ValueDetail GetValueDetail(long rawValue, bool includeImplicit = false)
        {
            Check(includeImplicit ? TriState.Yes : TriState.Undefined);
            ValueDetail valueDetail;
            if (!TryGetValueDetail(rawValue, out valueDetail, includeImplicit))
                throw new ArgumentOutOfRangeException("rawValue",
                                                      string.Format(
                                                          Resources.ExtendedEnumGeneric_DoesNotContainRawValue,
                                                          typeof (TEnum).FullName, rawValue));
            return valueDetail;
        }

        /// <summary>
        /// Gets the <see cref="ValueDetail"/> from the name specified.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="includeImplicit"><para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        /// <para>By default this is set to <see langword="false"/>.</para></param>
        /// <returns>The <see cref="ValueDetail"/> that corresponds to the <paramref name="name"/> specified.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The enum type doesn't contain the <paramref name="name"/> specified.</exception>
        [NotNull]
        [UsedImplicitly]
        public static ValueDetail GetValueDetail([NotNull] string name, bool includeImplicit = false)
        {
            Check(includeImplicit ? TriState.Yes : TriState.Undefined);
            ValueDetail valueDetail;
            if (!TryGetValueDetail(name, out valueDetail, includeImplicit))
                throw new ArgumentOutOfRangeException("name",
                                                      string.Format(Resources.ExtendedEnumGeneric_DoesNotContainName,
                                                                    typeof (TEnum).FullName, name));
            return valueDetail;
        }

        /// <summary>
        /// Tries to get the <see cref="ValueDetail"/> from the enum value specified.
        /// </summary>
        /// <param name="value">The enum value.</param>
        /// <param name="valueDetail"><para>The value detail.</para>
        /// <para>If a value detail isn't found then the output is <see langword="null">null</see>.</para></param>
        /// <param name="includeImplicit"><para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        /// <para>By default this is set to <see langword="false"/>.</para></param>
        /// <returns>Returns <see langword="true"/> if the value detail was found; otherwise returns <see langword="false"/>.</returns>
        [UsedImplicitly]
        public static bool TryGetValueDetail(TEnum value, [NotNull] out ValueDetail valueDetail,
                                             bool includeImplicit = false)
        {
            // ReSharper disable AssignNullToNotNullAttribute
            valueDetail = null;
            // ReSharper restore AssignNullToNotNullAttribute

            // If we're not an enum, or we're asking to include implicit values and we're not a flag
            // then we've failed.
            if ((!IsEnum) || (includeImplicit && !IsFlag))
                return false;

            // Return the explicit value detail, or try to create an implicit one.
            return _valueLookup.TryGetValue(value, out valueDetail) ||
                   (includeImplicit && TryGetImplicitValueDetail(Convert.ToInt64(value), out valueDetail));
        }

        /// <summary>
        /// Tries to get the <see cref="ValueDetail"/> from the raw value specified.
        /// </summary>
        /// <param name="rawValue">The raw value.</param>
        /// <param name="valueDetail"><para>The value detail.</para>
        /// <para>If a value detail isn't found then the output is <see langword="null">null</see>.</para></param>
        /// <param name="includeImplicit"><para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        /// <para>By default this is set to <see langword="false"/>.</para></param>
        /// <returns>Returns <see langword="true"/> if the value detail was found; otherwise returns <see langword="false"/>.</returns>
        [UsedImplicitly]
        public static bool TryGetValueDetail(long rawValue, [NotNull] out ValueDetail valueDetail,
                                             bool includeImplicit = false)
        {
            // ReSharper disable AssignNullToNotNullAttribute
            valueDetail = null;
            // ReSharper restore AssignNullToNotNullAttribute

            // If we're not an enum, or we're asking to include implicit values and we're not a flag
            // then we've failed.
            if ((!IsEnum) || (includeImplicit && !IsFlag))
                return false;

            // Return the explicit value detail, or try to create an implicit one.
            return _rawValueLookup.TryGetValue(rawValue, out valueDetail) ||
                   (includeImplicit && TryGetImplicitValueDetail(rawValue, out valueDetail));
        }

        /// <summary>
        /// Tries to get the <see cref="ValueDetail"/> from the name specified.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="valueDetail"><para>The value detail.</para>
        /// <para>If a value detail isn't found then the output is <see langword="null">null</see>.</para></param>
        /// <param name="includeImplicit"><para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        /// <para>By default this is set to <see langword="false"/>.</para></param>
        /// <returns>Returns <see langword="true"/> if the value detail was found; otherwise returns <see langword="false"/>.</returns>
        [UsedImplicitly]
        public static bool TryGetValueDetail([NotNull] string name, [NotNull] out ValueDetail valueDetail,
                                             bool includeImplicit = false)
        {
            // ReSharper disable AssignNullToNotNullAttribute
            valueDetail = null;
            // ReSharper restore AssignNullToNotNullAttribute

            // If we're not an enum, or we're asking to include implicit values and we're not a flag
            // then we've failed.
            if ((!IsEnum) || (includeImplicit && !IsFlag))
                return false;

            // If the name is explicit we will find it in the lookup.
            if (_nameLookup.TryGetValue(name, out valueDetail))
                return true;

            // If we're not looking for implicit values, or we can't get an implicit value we didn't find it.
            long rawValue;
            if (!includeImplicit ||
                !TryGetImplicitLong(name, out rawValue))
                return false;

            // Try to create an implicit value.
            return TryGetImplicitValueDetail(rawValue, out valueDetail);
        }
        #endregion

        #region GetLong and TryGetLong
        /// <summary>
        /// Gets the raw value equivalent of an enum value.
        /// </summary>
        /// <param name="value">The enum value.</param>
        /// <param name="includeImplicit"><para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        /// <para>By default this is set to <see langword="false"/>.</para></param>
        /// <returns>The <see cref="long"/> value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The enum type doesn't contain the <paramref name="value"/> specified.
        /// </exception>
        [UsedImplicitly]
        public static long GetLong(TEnum value, bool includeImplicit = false)
        {
            Check(includeImplicit ? TriState.Yes : TriState.Undefined);

            long rawValue;
            if (!TryGetLong(value, out rawValue, includeImplicit))
                throw new ArgumentOutOfRangeException("value",
                                                      string.Format(Resources.ExtendedEnumGeneric_DoesNotContainValue,
                                                                    typeof (TEnum).FullName, value));
            return rawValue;
        }

        /// <summary>
        /// Gets the raw value equivalent of an enum using the name specified.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="includeImplicit"><para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        /// <para>By default this is set to <see langword="false"/>.</para></param>
        /// <returns>The <see cref="long"/> value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The enum type doesn't contain a value by the <paramref name="name"/> specified.
        /// </exception>
        [UsedImplicitly]
        public static long GetLong([NotNull] string name, bool includeImplicit = false)
        {
            Check(includeImplicit ? TriState.Yes : TriState.Undefined);

            long rawValue;
            if (!TryGetLong(name, out rawValue, includeImplicit))
                throw new ArgumentOutOfRangeException("name",
                                                      string.Format(Resources.ExtendedEnumGeneric_DoesNotContainName,
                                                                    typeof (TEnum).FullName, name));
            return rawValue;
        }

        /// <summary>
        /// Tries to get the raw value equivalent of an enum value.
        /// </summary>
        /// <param name="value">The enum value.</param>
        /// <param name="rawValue"><para>The raw value.</para>
        /// <para>If no value is found then the output is zero.</para></param>
        /// <param name="includeImplicit"><para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        /// <para>By default this is set to <see langword="false"/>.</para></param>
        /// <returns>Returns <see langword="true"/> if the value was found; otherwise returns <see langword="false"/>.</returns>
        [UsedImplicitly]
        public static bool TryGetLong(TEnum value, out long rawValue, bool includeImplicit = false)
        {
            rawValue = 0L;
            // If we're not an enum, or we're asking to include implicit values and we're not a flag
            // then we've failed.
            if ((!IsEnum) || (includeImplicit && !IsFlag))
                return false;

            // If we know the value anyway return true.
            ValueDetail valueDetail;
            if (_valueLookup.TryGetValue(value, out valueDetail))
            {
                rawValue = valueDetail.RawValue;
                return true;
            }

            // If we're not including implicit values we're done.
            if (!includeImplicit) return false;

            // Calculate the raw value using a conversion
            rawValue = Convert.ToInt64(value);

            // Check that it is a valid raw value.
            return (rawValue > 0) && ((rawValue & ~_allValueDetail.RawValue) == 0);
        }

        /// <summary>
        /// Tries to get the raw value equivalent from the name specified.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="rawValue"><para>The raw value.</para>
        /// <para>If no value is found then the output is zero.</para></param>
        /// <param name="includeImplicit"><para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        /// <para>By default this is set to <see langword="false"/>.</para></param>
        /// <returns>Returns <see langword="true"/> if the value was found; otherwise returns <see langword="false"/>.</returns>
        [UsedImplicitly]
        public static bool TryGetLong([NotNull] string name, out long rawValue, bool includeImplicit = false)
        {
            rawValue = 0L;
            // If we're not an enum, or we're asking to include implicit values and we're not a flag
            // then we've failed.
            if ((!IsEnum) || (includeImplicit && !IsFlag))
                return false;

            // If we know the value anyway return true.
            ValueDetail valueDetail;
            if (_nameLookup.TryGetValue(name, out valueDetail))
            {
                rawValue = valueDetail.RawValue;
                return true;
            }

            // If we're including implicit names try to get implicit long value.
            return includeImplicit && TryGetImplicitLong(name, out rawValue);
        }
        #endregion

        #region GetName and TryGetName
        /// <summary>
        /// Gets the name from the enum value specified.
        /// </summary>
        /// <param name="value">The enum value.</param>
        /// <param name="includeImplicit"><para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        /// <para>By default this is set to <see langword="false"/>.</para></param>
        /// <returns>The name that corresponds to the <paramref name="value"/> specified.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The enum type doesn't contain the <paramref name="value"/> specified.</exception>
        [UsedImplicitly]
        [NotNull]
        public static string GetName(TEnum value, bool includeImplicit = false)
        {
            // We use GetValueDetail under the hood, as it caches and name building is expensive.
            return GetValueDetail(value, includeImplicit).Name;
        }

        /// <summary>
        /// Gets the name from the raw value specified.
        /// </summary>
        /// <param name="rawValue">The raw value.</param>
        /// <param name="includeImplicit"><para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        /// <para>By default this is set to <see langword="false"/>.</para></param>
        /// <returns>The name that corresponds to the <paramref name="rawValue"/> specified.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The enum type doesn't contain the <paramref name="rawValue"/> specified.</exception>
        [UsedImplicitly]
        [NotNull]
        public static string GetName(long rawValue, bool includeImplicit = false)
        {
            // We use GetValueDetail under the hood, as it caches and name building is expensive.
            return GetValueDetail(rawValue, includeImplicit).Name;
        }

        /// <summary>
        /// Tries to get the name for the enum value specified.
        /// </summary>
        /// <param name="value">The enum value.</param>
        /// <param name="name"><para>The name.</para>
        /// <para>If no name is found the output is a <see langword="null">null</see> string.</para></param>
        /// <param name="includeImplicit"><para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        /// <para>By default this is set to <see langword="false"/>.</para></param>
        /// <returns>Returns <see langword="true"/> if the name was found; otherwise returns <see langword="false"/>.</returns>
        [UsedImplicitly]
        public static bool TryGetName(TEnum value, [NotNull] out string name, bool includeImplicit = false)
        {
            // We use GetValueDetail under the hood, as it caches and name building is expensive.
            ValueDetail valueDetail;
            if (TryGetValueDetail(value, out valueDetail))
            {
                name = valueDetail.Name;
                return true;
            }
            // ReSharper disable AssignNullToNotNullAttribute
            name = null;
            // ReSharper restore AssignNullToNotNullAttribute
            return false;
        }

        /// <summary>
        /// Tries to get the name using the raw value specified.
        /// </summary>
        /// <param name="rawValue">The raw value.</param>
        /// <param name="name"><para>The name.</para>
        /// <para>If no name is found the output is a <see langword="null">null</see> string.</para></param>
        /// <param name="includeImplicit"><para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        /// <para>By default this is set to <see langword="false"/>.</para></param>
        /// <returns>Returns <see langword="true"/> if the name was found; otherwise returns <see langword="false"/>.</returns>
        [UsedImplicitly]
        public static bool TryGetName(long rawValue, [NotNull] out string name, bool includeImplicit = false)
        {
            // We use GetValueDetail under the hood, as it caches and name building is expensive.
            ValueDetail valueDetail;
            if (TryGetValueDetail(rawValue, out valueDetail))
            {
                name = valueDetail.Name;
                return true;
            }
            // ReSharper disable AssignNullToNotNullAttribute
            name = null;
            // ReSharper restore AssignNullToNotNullAttribute
            return false;
        }
        #endregion

        #region GetValue and TryGetValue
        /// <summary>
        /// Gets the enum value from the name specified.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="includeImplicit"><para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        /// <para>By default this is set to <see langword="false"/>.</para></param>
        /// <returns>The enum value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The enum type doesn't contain a value with the <paramref name="name"/> specified.
        /// </exception>
        [UsedImplicitly]
        public static TEnum GetValue([NotNull] string name, bool includeImplicit = false)
        {
            Check(includeImplicit ? TriState.Yes : TriState.Undefined);

            TEnum value;
            if (!TryGetValue(name, out value, includeImplicit))
                throw new ArgumentOutOfRangeException("name",
                                                      string.Format(Resources.ExtendedEnumGeneric_DoesNotContainName,
                                                                    typeof (TEnum).FullName, name));
            return value;
        }

        /// <summary>
        /// Gets the enum value from the raw value specified.
        /// </summary>
        /// <param name="rawValue">The raw value.</param>
        /// <param name="includeImplicit"><para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        /// <para>By default this is set to <see langword="false"/>.</para></param>
        /// <returns>The enum value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The enum type doesn't contain a value with the <paramref name="rawValue"/> specified.
        /// </exception>
        [UsedImplicitly]
        public static TEnum GetValue(long rawValue, bool includeImplicit = false)
        {
            Check(includeImplicit ? TriState.Yes : TriState.Undefined);

            TEnum value;
            if (!TryGetValue(rawValue, out value, includeImplicit))
                throw new ArgumentOutOfRangeException("rawValue",
                                                      string.Format(
                                                          Resources.ExtendedEnumGeneric_DoesNotContainRawValue,
                                                          typeof (TEnum).FullName, rawValue));
            return value;
        }

        /// <summary>
        /// Tries to get the enum value using the name specified.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value"><para>The enum value.</para>
        /// <para>If no value is found then the output is the default value of the type.</para></param>
        /// <param name="includeImplicit"><para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        /// <para>By default this is set to <see langword="false"/>.</para></param>
        /// <returns>Returns <see langword="true"/> if the value was found; otherwise returns <see langword="false"/>.</returns>
        [UsedImplicitly]
        public static bool TryGetValue([NotNull] string name, out TEnum value, bool includeImplicit = false)
        {
            value = default(TEnum);
            // If we're not an enum, or we're asking to include implicit values and we're not a flag
            // then we've failed.
            if ((!IsEnum) || (includeImplicit && !IsFlag))
                return false;

            // If we know the value anyway return true.
            ValueDetail valueDetail;
            if (_nameLookup.TryGetValue(name, out valueDetail))
            {
                value = valueDetail.Value;
                return true;
            }

            // If we're including implicit names try to get implicit long value.
            long rawValue;
            if (includeImplicit && TryGetImplicitLong(name, out rawValue))
            {
                // Convert raw value to enum.
                value = (TEnum) Enum.ToObject(typeof (TEnum), rawValue);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Tries to get the enum value using the raw value specified.
        /// </summary>
        /// <param name="rawValue">The raw value.</param>
        /// <param name="value"><para>The enum value.</para>
        /// <para>If no value is found then the output is the default value of the type.</para></param>
        /// <param name="includeImplicit"><para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        /// <para>By default this is set to <see langword="false"/>.</para></param>
        /// <returns>Returns <see langword="true"/> if the value was found; otherwise returns <see langword="false"/>.</returns>
        [UsedImplicitly]
        public static bool TryGetValue(long rawValue, out TEnum value, bool includeImplicit = false)
        {
            value = default(TEnum);
            // If we're not an enum, or we're asking to include implicit values and we're not a flag
            // then we've failed.
            if ((!IsEnum) || (includeImplicit && !IsFlag))
                return false;

            // First see if we know the raw value anyway.
            ValueDetail valueDetail;
            if (_rawValueLookup.TryGetValue(rawValue, out valueDetail))
            {
                value = valueDetail.Value;
                return true;
            }

            // If we're including implicit names and we're a flag enum, and the raw value
            // only has valid flags then convert to TEnum.
            if (includeImplicit && ((rawValue & ~_allValueDetail.RawValue) == 0))
            {
                value = (TEnum) Enum.ToObject(typeof (TEnum), rawValue);
                return true;
            }
            return false;
        }
        #endregion

        #region GetDescription and TryGetDescription
        /// <summary>
        /// Gets the description for the enum value specified.
        /// </summary>
        /// <param name="value">The enum value.</param>
        /// <returns>The description.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The enum type doesn't contain the <paramref name="value"/> specified.</exception>
        [UsedImplicitly]
        [NotNull]
        public static string GetDescription(TEnum value)
        {
            return GetValueDetail(value).Description;
        }

        /// <summary>
        /// Gets the description using the raw value specified.
        /// </summary>
        /// <param name="rawValue">The raw value.</param>
        /// <returns>The description.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The enum type doesn't contain the <paramref name="rawValue"/> specified.</exception>
        [UsedImplicitly]
        [NotNull]
        public static string GetDescription(long rawValue)
        {
            return GetValueDetail(rawValue).Description;
        }

        /// <summary>
        /// Gets the description using the name specified.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The description.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The enum type doesn't contain a value with the <paramref name="name"/> specified.</exception>
        [UsedImplicitly]
        [NotNull]
        public static string GetDescription([NotNull] string name)
        {
            return GetValueDetail(name).Description;
        }

        /// <summary>
        /// Tries to get the description from the enum value specified.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="description"><para>The description.</para>
        /// <para>If no value is found then the output is an <see cref="System.String.Empty">empty string</see>.</para></param>
        /// <returns>Returns <see langword="true"/> if the value was found; otherwise returns <see langword="false"/>.</returns>
        [UsedImplicitly]
        public static bool TryGetDescription(TEnum value, [NotNull] out string description)
        {
            ValueDetail valueDetail;
            if (!IsEnum || !TryGetValueDetail(value, out valueDetail))
            {
                description = string.Empty;
                return false;
            }
            description = valueDetail.Description;
            return true;
        }

        /// <summary>
        /// Tries to get the description using the raw value specified.
        /// </summary>
        /// <param name="rawValue">The raw value.</param>
        /// <param name="description"><para>The description.</para>
        /// <para>If no value is found then the output is an <see cref="System.String.Empty">empty string</see>.</para></param>
        /// <returns>Returns <see langword="true"/> if the value was found; otherwise returns <see langword="false"/>.</returns>
        [UsedImplicitly]
        public static bool TryGetDescription(long rawValue, [NotNull] out string description)
        {
            ValueDetail valueDetail;
            if (!IsEnum || !TryGetValueDetail(rawValue, out valueDetail))
            {
                description = string.Empty;
                return false;
            }
            description = valueDetail.Description;
            return true;
        }

        /// <summary>
        /// Tries to get the description using the name specified.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="description"><para>The description.</para>
        /// <para>If no value is found then the output is an <see cref="System.String.Empty">empty string</see>.</para></param>
        /// <returns>Returns <see langword="true"/> if the value was found; otherwise returns <see langword="false"/>.</returns>
        [UsedImplicitly]
        public static bool TryGetDescription([NotNull] string name, [NotNull] out string description)
        {
            ValueDetail valueDetail;
            if (!IsEnum || !TryGetValueDetail(name, out valueDetail))
            {
                description = string.Empty;
                return false;
            }
            description = valueDetail.Description;
            return true;
        }
        #endregion

        #endregion

        #region Parsing
        /// <summary>
        /// Parse the enum from a <see cref="string"/> representation.
        /// </summary>
        /// <param name="value">The string representation.</param>
        /// <returns>The enum value.</returns>
        /// <example><code>
        /// [Flags]
        /// private enum Days
        /// {
        ///    None,
        ///    Fri = 1,
        ///    Sat = 1 &lt;&lt; 1,
        ///    Sun = 1 &lt;&lt; 2,
        ///    Weekend = Fri | Sat
        /// };
        /// 
        /// Days parsedValue = ExtendedEnum&lt;Days&gt;.Parse("Sat");
        /// </code></example>
        /// <exception cref="ArgumentException">
        /// <para>The <see cref="object"/> is not an enum.</para>
        /// <para>-or-</para>
        /// <para>The requested name (<paramref name="value"/>) was invalid.</para>
        /// </exception>
        [UsedImplicitly]
        public static TEnum Parse([NotNull] String value)
        {
            if (!IsEnum)
                throw new ArgumentException(string.Format(Resources.ExtendedEnumGeneric_TypeIsNotAnEnum, typeof (TEnum).FullName));

            ValueDetail valueDetail;
            // Quick direct lookup of name will resolve majority of cases.
            if (_nameLookup.TryGetValue(value, out valueDetail))
                return valueDetail.Value;

            return (TEnum) Enum.Parse(typeof (TEnum), value, false);
        }

        /// <summary>
        /// Parse the enum from a <see cref="string"/> representation.
        /// </summary>
        /// <param name="value">The string representation.</param>
        /// <param name="ignoreCase">If set to <see langword="true"/> case sensitivity is ignored.</param>
        /// <returns>The enum value.</returns>
        /// <example><code>
        /// [Flags]
        /// private enum Days
        /// {
        ///    None,
        ///    Fri = 1,
        ///    Sat = 1 &lt;&lt; 1,
        ///    Sun = 1 &lt;&lt; 2,
        ///    Weekend = Fri | Sat
        /// };
        /// 
        /// Days parsedValue = ExtendedEnum&lt;Days&gt;.Parse("sat", true);
        /// </code></example>
        /// <exception cref="ArgumentException">
        /// <para>The <see cref="object"/> is not an enum.</para>
        /// <para>-or-</para>
        /// <para>The requested name (<paramref name="value"/>) was invalid.</para>
        /// </exception>
        [UsedImplicitly]
        public static TEnum Parse([NotNull] String value, bool ignoreCase)
        {
            if (!IsEnum)
                throw new ArgumentException(string.Format(Resources.ExtendedEnumGeneric_TypeIsNotAnEnum, typeof (TEnum).FullName));

            ValueDetail valueDetail;
            // Quick direct lookup of name will resolve majority of cases.
            if ((!ignoreCase) &&
                (_nameLookup.TryGetValue(value, out valueDetail)))
                return valueDetail.Value;

            return (TEnum) Enum.Parse(typeof (TEnum), value, ignoreCase);
        }

        /// <summary>
        /// Tries to parse the enum from a <see cref="string"/> representation.
        /// </summary>
        /// <param name="value">The string representation.</param>
        /// <param name="result">The result.</param>
        /// <returns>Returns <see langword="true"/> if parsed successfully; otherwise returns <see langword="false"/>.</returns>
        [UsedImplicitly]
        public static bool TryParse([NotNull] String value, out TEnum result)
        {
            if (!IsEnum)
                throw new ArgumentException(string.Format(Resources.ExtendedEnumGeneric_TypeIsNotAnEnum, typeof (TEnum).FullName));

            // Quick direct lookup of name will resolve majority of cases.
            ValueDetail valueDetail;
            if (_nameLookup.TryGetValue(value, out valueDetail))
            {
                result = valueDetail.Value;
                return true;
            }

            return Enum.TryParse(value, false, out result);
        }

        /// <summary>
        /// Tries to parse the enum from a <see cref="string"/> representation.
        /// </summary>
        /// <param name="value">The string representation.</param>
        /// <param name="ignoreCase">If set to <see langword="true"/> case sensitivity is ignored.</param>
        /// <param name="result">The result.</param>
        /// <returns>Returns <see langword="true"/> if parsed successfully; otherwise returns <see langword="false"/>.</returns>
        [UsedImplicitly]
        public static bool TryParse([NotNull] String value, bool ignoreCase, out TEnum result)
        {
            if (!IsEnum)
                throw new ArgumentException(string.Format(Resources.ExtendedEnumGeneric_TypeIsNotAnEnum, typeof (TEnum).FullName));

            // Quick direct lookup of name will resolve majority of cases.
            ValueDetail valueDetail;
            if ((!ignoreCase) &&
                (_nameLookup.TryGetValue(value, out valueDetail)))
            {
                result = valueDetail.Value;
                return true;
            }

            return Enum.TryParse(value, ignoreCase, out result);
        }
        #endregion

        #region Flag manipulation
        /// <summary>
        /// Determines whether the specified flags are set on the value.
        /// </summary>
        /// <param name="value">The enum value.</param>
        /// <param name="flags">The flags.</param>
        /// <param name="includeImplicit"><para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        /// <para>By default this is set to <see langword="false"/>.</para></param>
        /// <returns>Returns <see langword="true"/> if the specified flags are set; otherwise returns <see langword="false"/>.</returns>
        [UsedImplicitly]
        public static bool AreSet(TEnum value, TEnum flags, bool includeImplicit = false)
        {
            if (!IsEnum || !IsFlag)
                return false;

            long rawValue;
            if (!TryGetLong(value, out rawValue, includeImplicit))
                return false;
            long fLong;
            if (!TryGetLong(flags, out fLong, includeImplicit))
                return false;

            return fLong == (fLong & rawValue);
        }

        /// <summary>
        /// Determines whether the specified flags are clear of the value.
        /// </summary>
        /// <param name="value">The enum value.</param>
        /// <param name="flags">The flags.</param>
        /// <param name="includeImplicit"><para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        /// <para>By default this is set to <see langword="false"/>.</para></param>
        /// <returns>Returns <see langword="true"/> if the specified flags are set; otherwise returns <see langword="false"/>.</returns>
        [UsedImplicitly]
        public static bool AreClear(TEnum value, TEnum flags, bool includeImplicit = false)
        {
            if (!IsEnum || !IsFlag)
                return false;

            long rawValue;
            if (!TryGetLong(value, out rawValue, includeImplicit))
                return false;
            long fLong;
            if (!TryGetLong(flags, out fLong, includeImplicit))
                return false;

            return (fLong & rawValue) == 0;
        }

        /// <summary>
        /// Sets the flags on the value.
        /// </summary>
        /// <param name="value">The enum.</param>
        /// <param name="flags">The flags.</param>
        /// <param name="includeImplicit"><para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        /// <para>By default this is set to <see langword="false"/>.</para></param>
        /// <returns>The result of the <paramref name="flags"/> set to <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> cannot be set with the <paramref name="flags"/> specified.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para>The <paramref name="value"/> type is not an enum.</para>
        /// <para>-or-</para>
        /// <para>The <paramref name="value"/> type is not an enum.</para>
        /// </exception>
        [UsedImplicitly]
        public static TEnum Set(TEnum value, TEnum flags, bool includeImplicit = false)
        {
            // Ensure we're a flag enum.
            if (!IsEnum)
                throw new ArgumentException(string.Format(Resources.ExtendedEnumGeneric_TypeIsNotAnEnum, typeof (TEnum).FullName));
            if (!IsFlag)
                throw new ArgumentException(string.Format(Resources.ExtendedEnumGeneric_TypeIsNotFlagEnum, typeof (TEnum).FullName));

            TEnum result;
            if (!TrySet(value, flags, out result, includeImplicit))
                throw new ArgumentOutOfRangeException("value",
                                                      string.Format(
                                                          Resources.ExtendedEnumGeneric_Set_CouldNotSetFlags,
                                                          value, typeof (TEnum).FullName, flags));
            return result;
        }

        /// <summary>
        /// Tries to set the flags on the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="flags">The flags.</param>
        /// <param name="result">The result of the <paramref name="flags"/> set to <paramref name="value"/>.</param>
        /// <param name="includeImplicit"><para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        /// <para>By default this is set to <see langword="false"/>.</para></param>
        /// <returns>Returns <see langword="true"/> if the flags can be set; otherwise returns <see langword="false"/>.</returns>
        [UsedImplicitly]
        public static bool TrySet(TEnum value, TEnum flags, out TEnum result, bool includeImplicit = false)
        {
            result = default(TEnum);
            if (!IsEnum || !IsFlag)
                return false;

            long rawValue;
            if (!TryGetLong(value, out rawValue, includeImplicit))
                return false;
            long fLong;
            if (!TryGetLong(flags, out fLong, includeImplicit))
                return false;

            long rLong = rawValue | fLong;
            return TryGetValue(rLong, out result, includeImplicit);
        }

        /// <summary>
        /// Clears the flags on the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="flags">The flags.</param>
        /// <param name="includeImplicit"><para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        /// <para>By default this is set to <see langword="false"/>.</para></param>
        /// <returns>The result of the <paramref name="flags"/> cleared from <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The enum cannot be cleared of the specified <paramref name="flags"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para>The <paramref name="value"/> type is not an enum.</para>
        /// <para> </para><para>-or-</para><para> </para>
        /// <para>The <paramref name="value"/> type is not an enum.</para>
        /// </exception>
        [UsedImplicitly]
        public static TEnum Clear(TEnum value, TEnum flags, bool includeImplicit = false)
        {
            // Ensure we're a flag enum.
            if (!IsEnum)
                throw new ArgumentException(string.Format(Resources.ExtendedEnumGeneric_TypeIsNotAnEnum, typeof (TEnum).FullName));
            if (!IsFlag)
                throw new ArgumentException(string.Format(Resources.ExtendedEnumGeneric_TypeIsNotFlagEnum, typeof (TEnum).FullName));

            TEnum result;
            if (!TryClear(value, flags, out result, includeImplicit))
                throw new ArgumentOutOfRangeException("value",
                                                      string.Format(
                                                          Resources.ExtendedEnumGeneric_Clear_CouldNotClearFlags,
                                                          value, typeof (TEnum).FullName, flags));
            return result;
        }

        /// <summary>
        /// Tries to clear the flags on the value.
        /// </summary>
        /// <param name="value">The enum value.</param>
        /// <param name="flags">The flags.</param>
        /// <param name="result">The result of the <paramref name="flags"/> cleared from <paramref name="value"/>.</param>
        /// <param name="includeImplicit"><para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        /// <para>By default this is set to <see langword="false"/>.</para></param>
        /// <returns>Returns <see langword="true"/> if the flags can be cleared; otherwise returns <see langword="false"/>.</returns>
        [UsedImplicitly]
        public static bool TryClear(TEnum value, TEnum flags, out TEnum result, bool includeImplicit = false)
        {
            result = default(TEnum);
            if (!IsEnum || !IsFlag)
                return false;

            long rawValue;
            if (!TryGetLong(value, out rawValue, includeImplicit))
                return false;
            long fLong;
            if (!TryGetLong(flags, out fLong, includeImplicit))
                return false;

            long rLong = rawValue & ~fLong;
            return TryGetValue(rLong, out result, includeImplicit);
        }

        /// <summary>
        /// Gets the intersection of the enum and the flags, that is it returns a value which only has flags that were set on both.
        /// </summary>
        /// <param name="value">The enum value.</param>
        /// <param name="flags">The flags.</param>
        /// <param name="includeImplicit"><para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        /// <para>By default this is set to <see langword="false"/>.</para></param>
        /// <returns>The intersection of <paramref name="value"/> and <paramref name="flags"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> type could not be intersected with the <paramref name="flags"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para>The <paramref name="value"/> type is not an enum.</para>
        /// <para>-or-</para>
        /// <para>The <paramref name="value"/> type is not an enum.</para>
        /// </exception>
        [UsedImplicitly]
        public static TEnum Intersect(TEnum value, TEnum flags, bool includeImplicit = false)
        {
            // Ensure we're a flag enum.
            if (!IsEnum)
                throw new ArgumentException(string.Format(Resources.ExtendedEnumGeneric_TypeIsNotAnEnum, typeof (TEnum).FullName));
            if (!IsFlag)
                throw new ArgumentException(string.Format(Resources.ExtendedEnumGeneric_TypeIsNotFlagEnum, typeof (TEnum).FullName));

            TEnum result;
            if (!TryIntersect(value, flags, out result, includeImplicit))
                throw new ArgumentOutOfRangeException("value",
                                                      string.Format(
                                                          Resources.ExtendedEnumGeneric_Intersect_CouldNotIntersectFlags,
                                                          value, typeof (TEnum).FullName, flags));
            return result;
        }

        /// <summary>
        /// Tries to get the intersection of the enum and the flags, that is it returns a value which only has flags that were set on both.
        /// </summary>
        /// <param name="value">The enum value.</param>
        /// <param name="flags">The flags.</param>
        /// <param name="result">The intersection of <paramref name="value"/> and <paramref name="flags"/>.</param>
        /// <param name="includeImplicit"><para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        /// <para>By default this is set to <see langword="false"/>.</para></param>
        /// <returns>Returns <see langword="true"/> if the flags can be intersected; otherwise returns <see langword="false"/>.</returns>
        [UsedImplicitly]
        public static bool TryIntersect(TEnum value, TEnum flags, out TEnum result, bool includeImplicit = false)
        {
            result = default(TEnum);
            if (!IsEnum || !IsFlag)
                return false;

            long rawValue;
            if (!TryGetLong(value, out rawValue, includeImplicit))
                return false;
            long fLong;
            if (!TryGetLong(flags, out fLong, includeImplicit))
                return false;

            long rLong = rawValue & fLong;
            return TryGetValue(rLong, out result, includeImplicit);
        }

        /// <summary>
        /// Inverts all the valid flags.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="includeImplicit"><para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        /// <para>By default this is set to <see langword="false"/>.</para></param>
        /// <returns>The inverted flags for the specified <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> provided could not be inverted.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para>The <paramref name="value"/> type is not an enum.</para>
        /// <para>-or-</para>
        /// <para>The <paramref name="value"/> type is not an enum.</para>
        /// </exception>
        [UsedImplicitly]
        public static TEnum Invert(TEnum value, bool includeImplicit = false)
        {
            // Ensure we're a flag enum.
            if (!IsEnum)
                throw new ArgumentException(string.Format(Resources.ExtendedEnumGeneric_TypeIsNotAnEnum, typeof (TEnum).FullName));
            if (!IsFlag)
                throw new ArgumentException(string.Format(Resources.ExtendedEnumGeneric_TypeIsNotFlagEnum, typeof (TEnum).FullName));

            TEnum result;
            if (!TryInvert(value, out result, includeImplicit))
                throw new ArgumentOutOfRangeException("value",
                                                      string.Format(
                                                          Resources.ExtendedEnumGeneric_Invert_CouldNotInvertFlags,
                                                          value, typeof (TEnum).FullName));
            return result;
        }

        /// <summary>
        /// Inverts all the valid flags.
        /// </summary>
        /// <param name="value">The enum value.</param>
        /// <param name="result">The inverted flags for the specified <paramref name="value"/>.</param>
        /// <param name="includeImplicit"><para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        /// <para>By default this is set to <see langword="false"/>.</para></param>
        /// <returns>Returns <see langword="true"/> if the flags can be inverted; otherwise returns <see langword="false"/>.</returns>
        [UsedImplicitly]
        public static bool TryInvert(TEnum value, out TEnum result, bool includeImplicit = false)
        {
            result = default(TEnum);
            if (!IsEnum || !IsFlag)
                return false;

            long rawValue;
            if (!TryGetLong(value, out rawValue, includeImplicit))
                return false;

            long rLong = rawValue ^ _allValueDetail.RawValue;
            return TryGetValue(rLong, out result, includeImplicit);
        }

        /// <summary>
        /// Combines all of the specified flags.
        /// </summary>
        /// <param name="flags">The flags to combine.</param>
        /// <param name="includeImplicit"><para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        /// <para>By default this is set to <see langword="false"/>.</para></param>
        /// <returns>The combined <paramref name="flags"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The flags of type <typeparamref name="TEnum"/> cannot be combined.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><typeparamref name="TEnum"/> is not an enum.</para>
        /// <para>-or-</para>
        /// <para><typeparamref name="TEnum"/> is not a flag enum.</para>
        /// </exception>
        [UsedImplicitly]
        public static TEnum Combine([NotNull] IEnumerable<TEnum> flags, bool includeImplicit = false)
        {
            // Ensure we're a flag enum.
            if (!IsEnum)
                throw new ArgumentException(string.Format(Resources.ExtendedEnumGeneric_TypeIsNotAnEnum, typeof (TEnum).FullName));
            if (!IsFlag)
                throw new ArgumentException(string.Format(Resources.ExtendedEnumGeneric_TypeIsNotFlagEnum, typeof(TEnum).FullName));

            TEnum result;
            if (!TryCombine(flags, out result, includeImplicit))
                throw new ArgumentOutOfRangeException("flags",
                                                      string.Format(Resources.ExtendedEnumGeneric_Combine_CouldNotCombineFlags,
                                                                    typeof (TEnum).FullName));
            return result;
        }

        /// <summary>
        /// Tries to combine all of the specified flags.
        /// </summary>
        /// <param name="flags">The flags to combine.</param>
        /// <param name="result"><para>The combined <paramref name="flags"/>.</para>
        /// <para>If unsuccessful then this returns an enum with the raw value of zero.</para></param>
        /// <param name="includeImplicit"><para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        /// <para>By default this is set to <see langword="false"/>.</para></param>
        /// <returns>Returns <see langword="true"/> if the flags can be inverted; otherwise returns <see langword="false"/>.</returns>
        [UsedImplicitly]
        public static bool TryCombine([NotNull] IEnumerable<TEnum> flags, out TEnum result, bool includeImplicit = false)
        {
            result = default(TEnum);
            if (!IsEnum || !IsFlag)
                return false;

            long rLong = 0L;
            foreach (TEnum flag in flags)
            {
                long fLong;
                if (!TryGetLong(flag, out fLong, includeImplicit))
                    return false;
                rLong |= fLong;
            }

            return TryGetValue(rLong, out result, includeImplicit);
        }

        /// <summary>
        /// Splits the specified flags.
        /// </summary>
        /// <param name="flags">The flags to split.</param>
        /// <param name="includeImplicit"><para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        /// <para>By default this is set to <see langword="false"/>.</para></param>
        /// <param name="includeCombinations">If set to <see langword="true"/> will split into the minimum number of flags (allows combinations).</param>
        /// <returns>The result of the split <paramref name="flags"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Couldn't split the <paramref name="flags"/> for type <typeparamref name="TEnum"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <para><typeparamref name="TEnum"/> is not an enum.</para>
        /// <para>-or-</para>
        /// <para><typeparamref name="TEnum"/> is not a flag enum.</para>
        /// </exception>
        [UsedImplicitly]
        [NotNull]
        public static IEnumerable<TEnum> SplitFlags(TEnum flags, bool includeImplicit = false,
                                                    bool includeCombinations = false)
        {
            // Ensure we're a flag enum.
            if (!IsEnum)
                throw new ArgumentException(string.Format(Resources.ExtendedEnumGeneric_TypeIsNotAnEnum, typeof(TEnum).FullName));
            if (!IsFlag)
                throw new ArgumentException(string.Format(Resources.ExtendedEnumGeneric_TypeIsNotFlagEnum, typeof(TEnum).FullName));

            IEnumerable<TEnum> result;
            if (!TrySplitFlags(flags, out result, includeImplicit, includeCombinations))
                throw new ArgumentOutOfRangeException("flags",
                                                      string.Format(Resources.ExtendedEnumGeneric_SplitFlags_CouldNotSplitFlags, flags,
                                                                    typeof (TEnum).FullName));
            return result ?? Enumerable.Empty<TEnum>();
        }

        /// <summary>
        /// Tries to split all the specified flags.
        /// </summary>
        /// <param name="flags">The flags to split.</param>
        /// <param name="result">The result of the split <paramref name="flags"/>.
        /// <para>The default result if the split fails is <see cref="System.Linq.Enumerable.Empty{T}()"/>.</para></param>
        /// <param name="includeImplicit"><para>If set to <see langword="true"/> includes implicit values of flag enums.</para>
        /// <para>By default this is set to <see langword="false"/>.</para></param>
        /// <param name="includeCombinations"><para>If set to <see langword="true"/> will split into the minimum number of flags (allows combinations).</para>
        /// <para>By default this is set to <see langword="false"/>.</para></param>
        /// <returns>Returns <see langword="true"/> if the flags can be split successfully; otherwise returns <see langword="false"/>.</returns>
        [UsedImplicitly]
        public static bool TrySplitFlags(TEnum flags, out IEnumerable<TEnum> result, bool includeImplicit = false,
                                         bool includeCombinations = false)
        {
            result = Enumerable.Empty<TEnum>();
            if (!IsEnum || !IsFlag)
                return false;

            long fLong;
            if (!TryGetLong(flags, out fLong, includeImplicit))
                return false;

            // If we got here but fLong is zero the result is an empty enum.
            if (fLong == 0)
                return true;

            // If we have any invalid bits we can't split.
            if ((fLong & ~_allValueDetail.RawValue) != 0)
                return false;

            List<ValueDetail> valueDetails = new List<ValueDetail>();
            foreach (ValueDetail valueDetail in ValueDetailsReversed)
            {
                // Skip explicit None.
                if (valueDetail.RawValue == 0)
                    continue;

                // Skip combination flags if we're not using them
                if (!includeCombinations && valueDetail.Flags > 1)
                    continue;

                // Skip flags that don't match
                if ((fLong & valueDetail.RawValue) != valueDetail.RawValue)
                    continue;

                // We have a match
                valueDetails.Add(valueDetail);
                fLong &= ~valueDetail.RawValue;

                // Check if we're done.
                if (fLong < 1)
                    break;
            }

            // ReSharper disable PossibleNullReferenceException
            List<TEnum> rList = valueDetails.Reverse<ValueDetail>().Select(nd => nd.Value).ToList();
            // ReSharper restore PossibleNullReferenceException

            // if we still have bits they were valid, so create an enum out of them.
            if (fLong > 0)
                rList.Add((TEnum) Enum.ToObject(typeof (TEnum), fLong));

            result = rList;

            return true;
        }
        #endregion

        #region Nested type: ValueDetail
        /// <summary>
        /// Holds details about enum values.
        /// </summary>
        [UsedImplicitly]
        public class ValueDetail : IEnumerable<string>
        {
            /// <summary>
            /// The number of flags.
            /// </summary>
            /// <remarks>
            /// If this is a non-flag enum then this value will always be 1.
            /// </remarks>
            [UsedImplicitly] public readonly ushort Flags;

            /// <summary>
            /// Whether the value is explicitly defined.
            /// </summary>
            /// <remarks>
            /// If this is a non-flag enum then this value will always be <see langword="true"/>.
            /// </remarks>
            [UsedImplicitly] public readonly bool IsExplicit;

            /// <summary>
            /// The raw value of the enum as a <see cref="long"/>.
            /// </summary>
            [UsedImplicitly] public readonly long RawValue;

            /// <summary>
            /// The actual enum value.
            /// </summary>
            [UsedImplicitly] public readonly TEnum Value;

            /// <summary>
            /// All known value names.
            /// </summary>
            [NotNull] [UsedImplicitly] private readonly List<string> _names;

            /// <summary>
            /// Initializes a new instance of the <see cref="ValueDetail"/> struct.
            /// </summary>
            /// <param name="value">The enum value.</param>
            /// <param name="rawValue">The raw value.</param>
            /// <param name="flags">The flags.</param>
            /// <param name="name">The name.</param>
            /// <param name="isExplicit">If set to <see langword="true"/> [is explicit].</param>
            /// <param name="description">The description (if any).</param>
            /// <remarks></remarks>
            internal ValueDetail(
                TEnum value,
                long rawValue,
                ushort flags,
                [NotNull] string name,
                bool isExplicit,
                [CanBeNull] string description)
            {
                Value = value;
                RawValue = rawValue;
                Flags = flags;
                _names = new List<string> {name};
                IsExplicit = isExplicit;
                Description = description ?? string.Empty;
            }

            /// <summary>
            /// Whether the flag enum is a combination of values.
            /// </summary>
            [UsedImplicitly]
            public bool IsCombination
            {
                get { return IsFlag && Flags > 1; }
            }

            /// <summary>
            /// The value name.
            /// </summary>
            [NotNull]
            [UsedImplicitly]
            // ReSharper disable AssignNullToNotNullAttribute
                public string Name
            {
                get { return _names.First(); }
            }

            // ReSharper restore AssignNullToNotNullAttribute

            /// <summary>
            /// Returns a list of the known names for the value.
            /// </summary>
            [NotNull]
            [UsedImplicitly]
            public IEnumerable<string> AllNames
            {
                get { return new List<string>(_names); }
            }

            /// <summary>
            /// Returns a count of the value's known names.
            /// </summary>
            [UsedImplicitly]
            public int Count
            {
                get { return _names.Count; }
            }

            /// <summary>
            /// The value's description attribute (if any).
            /// </summary>
            [NotNull]
            [UsedImplicitly]
            public string Description { get; private set; }

            #region IEnumerable<string> Members
            /// <summary>
            /// Returns an enumerator that iterates through the value names.
            /// </summary>
            /// <returns>
            /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
            /// </returns>
            /// <filterpriority>1</filterpriority>
            public IEnumerator<string> GetEnumerator()
            {
                return _names.GetEnumerator();
            }

            /// <summary>
            /// Returns an enumerator that iterates through the value details.
            /// </summary>
            /// <returns>
            /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
            /// </returns>
            /// <filterpriority>2</filterpriority>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
            #endregion

            /// <summary>
            /// Returns a <see cref="string"/> that represents this instance.
            /// </summary>
            /// <returns>
            /// A <see cref="string"/> representation of this instance.
            /// </returns>
            public override string ToString()
            {
                return string.Format("'{0}' = {1}{2}{3}",
                                     Name,
                                     RawValue,
                                     Flags > 1 ? string.Format(" [{0} flags]", Flags) : string.Empty,
                                     string.IsNullOrWhiteSpace(Description)
                                         ? string.Empty
                                         : Environment.NewLine + Description
                    );
            }

            /// <summary>
            /// Adds the specified name to the value's known names.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <param name="description">The description.</param>
            /// <remarks>Descriptions are separated by a <see cref="System.Environment.NewLine"/>.</remarks>
            internal void AddDefinition([NotNull] string name, [CanBeNull] string description)
            {
                _names.Add(name);
                if (!string.IsNullOrWhiteSpace(description))
                    Description += (Description == string.Empty ? string.Empty : Environment.NewLine) + description;
            }
        }
        #endregion
    }
}