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
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Enumerations;
using WebApplications.Utilities.Threading;

namespace WebApplications.Utilities
{
    /// <summary>
    /// Represents the method that compares two objects of the same type for equality.
    /// </summary>
    /// <typeparam name="T">The type of the objects to compare.</typeparam>
    /// <param name="x">The first object to compare.</param>
    /// <param name="y">The second object to compare.</param>
    /// <returns><see langword="true"/> if the specified objects are equal; otherwise, <see langword="false"/>.</returns>
    public delegate bool EqualityComparison<in T>(T x, T y);

    /// <summary>
    ///   Useful extension methods
    /// </summary>
    [PublicAPI]
    public static class UtilityExtensions
    {
        /// <summary>
        ///   A dictionary of equality operators by their runtime type.
        ///   This is so that, when requested, they can be retrieved rather than recomputed.
        /// </summary>
        [NotNull]
        private static readonly ConcurrentDictionary<Type, Func<object, object, bool>> _equalityFunctions =
            new ConcurrentDictionary<Type, Func<object, object, bool>>();

        /// <summary>
        /// Characters to escape for JSON (and their new value).
        /// </summary>
        [NotNull]
        private static readonly Dictionary<char, string> _jsonEscapedCharacters = new Dictionary<char, string>
        {
            { '\\', @"\\" },
            { '\"', @"\""" },
            { '\b', @"\b" },
            { '\f', @"\f" },
            { '\n', @"\n" },
            { '\r', @"\r" },
            { '\t', @"\t" }
        };

        /// <summary>
        ///   The Epoch date time (used by JavaScript).
        /// </summary>
        public static readonly DateTime EpochStart = new DateTime(1970, 1, 1);

        [NotNull]
        private static readonly string[] _onesMapping =
        {
            "Zero",
            "One",
            "Two",
            "Three",
            "Four",
            "Five",
            "Six",
            "Seven",
            "Eight",
            "Nine",
            "Ten",
            "Eleven",
            "Twelve",
            "Thirteen",
            "Fourteen",
            "Fifteen",
            "Sixteen",
            "Seventeen",
            "Eighteen",
            "Nineteen"
        };

        [NotNull]
        private static readonly string[] _tensMapping =
        {
            "Twenty",
            "Thirty",
            "Forty",
            "Fifty",
            "Sixty",
            "Seventy",
            "Eighty",
            "Ninety"
        };

        // NOTE: 10^303 is approaching the limits of double, as ~1.7e308 is where we are going
        // 10^303 is a centillion and a 10^309 is a duocentillion
        [NotNull]
        private static readonly string[] _groupMapping =
        {
            "Hundred",
            "Thousand",
            "Million",
            "Billion",
            "Trillion",
            "Quadrillion",
            "Quintillion",
            "Sextillian",
            "Septillion",
            "Octillion",
            "Nonillion",
            "Decillion",
            "Undecillion",
            "Duodecillion",
            "Tredecillion",
            "Quattuordecillion",
            "Quindecillion",
            "Sexdecillion",
            "Septendecillion",
            "Octodecillion",
            "Novemdecillion",
            "Vigintillion",
            "Unvigintillion",
            "Duovigintillion",
            "Tresvigintillion",
            "Quattuorvigintillion",
            "Quinquavigintillion",
            "Sesvigintillion",
            "Septemvigintillion",
            "Octovigintillion",
            "Vigintinonillion",
            "Trigintillion",
            "Untrigintillion",
            "Duotrigintillion",
            "Trestrigintillion"
        };

        /// <summary>
        ///   The default split characters for splitting strings.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly char[] DefaultSplitChars = new[] { ' ', ',', '\t', '\r', '\n', '|' };

        [NotNull]
        private static readonly Regex _htmlRegex = new Regex(
            @"<[^<>]*>",
            RegexOptions.Compiled | RegexOptions.IgnoreCase |
            RegexOptions.Multiline);

        [NotNull]
        private static readonly Regex _lineSplitter = new Regex(@"\r?\n|\r", RegexOptions.Compiled);

        /// <summary>
        /// The URI for the current AppDomains <see cref="AppDomain.BaseDirectory"/>.
        /// </summary>
        [NotNull]
        public static readonly Uri AppDomainBaseUri = new Uri(AppDomain.CurrentDomain.BaseDirectory);

        /// <summary>
        ///   Gets the ordinal representation of an <see cref="int">integer</see> ('1st', '2nd', etc.) as a <see cref="string"/>.
        /// </summary>
        /// <param name="number">The number to add the suffix to.</param>
        /// <returns>The <paramref name="number"/> + the correct suffix.</returns>
        [NotNull]
        [PublicAPI]
        public static string ToOrdinal(this int number)
        {
            string suf = "th";
            if (((number % 100) / 10) != 1)
                switch (number % 10)
                {
                    case 1:
                        suf = "st";
                        break;
                    case 2:
                        suf = "nd";
                        break;
                    case 3:
                        suf = "rd";
                        break;
                }
            return number + suf;
        }

        /// <summary>
        ///   Gets the ordinal for an integer ('st', 'nd', etc.)
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>The ordinal (suffix) for the <paramref name="number"/> specified.</returns>
        [NotNull]
        [PublicAPI]
        public static string GetOrdinal(this int number)
        {
            string suf = "th";
            if (((number % 100) / 10) != 1)
                switch (number % 10)
                {
                    case 1:
                        suf = "st";
                        break;
                    case 2:
                        suf = "nd";
                        break;
                    case 3:
                        suf = "rd";
                        break;
                }
            return suf;
        }

        /// <summary>
        ///   Converts a <see cref="int"/> to its English textual equivalent.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        /// <returns>
        ///   The textual equivalent of <paramref name="number"/> specified.
        /// </returns>
        [PublicAPI]
        [NotNull]
        public static string ToEnglish(this int number)
        {
            using (StringWriter writer = new StringWriter())
            {
                WriteEnglish(writer, number);
                return writer.ToString();
            }
        }

        /// <summary>
        ///   Converts a <see cref="double"/> to its English textual equivalent.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        /// <returns>
        ///   The textual equivalent of <paramref name="number"/> specified.
        /// </returns>
        [PublicAPI]
        [NotNull]
        public static string ToEnglish(this double number)
        {
            using (StringWriter writer = new StringWriter())
            {
                WriteEnglish(writer, number);
                return writer.ToString();
            }
        }

        /// <summary>
        ///   Converts a <see cref="long"/> to its English textual equivalent.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        /// <returns>
        ///   The textual equivalent of <paramref name="number"/> specified.
        /// </returns>
        [PublicAPI]
        [NotNull]
        public static string ToEnglish(this long number)
        {
            using (StringWriter writer = new StringWriter())
            {
                WriteEnglish(writer, number);
                return writer.ToString();
            }
        }

        /// <summary>
        /// Appends a <see cref="double" /> to its English textual equivalent.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="number">The number to convert.</param>
        /// <returns>The textual equivalent of <paramref name="number" /> specified.</returns>
        [PublicAPI]
        public static void AppendEnglish([NotNull] this StringBuilder builder, double number)
        {
            Contract.Requires(builder != null);
            using (StringWriter writer = new StringWriter(builder))
                WriteEnglish(writer, number);
        }

        /// <summary>
        /// Appends an <see cref="int" /> to its English textual equivalent.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="number">The number to convert.</param>
        /// <returns>The textual equivalent of <paramref name="number" /> specified.</returns>
        [PublicAPI]
        public static void AppendEnglish([NotNull] this StringBuilder builder, int number)
        {
            Contract.Requires(builder != null);
            using (StringWriter writer = new StringWriter(builder))
                WriteEnglish(writer, number);
        }

        /// <summary>
        /// Appends an <see cref="long" /> to its English textual equivalent.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="number">The number to convert.</param>
        /// <returns>The textual equivalent of <paramref name="number" /> specified.</returns>
        [PublicAPI]
        public static void AppendEnglish([NotNull] this StringBuilder builder, long number)
        {
            Contract.Requires(builder != null);
            using (StringWriter writer = new StringWriter(builder))
                WriteEnglish(writer, number);
        }

        /// <summary>
        /// Writes a <see cref="double" /> in its English textual equivalent.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="number">The number to convert.</param>
        /// <returns>The textual equivalent of <paramref name="number" /> specified.</returns>
        [PublicAPI]
        public static void WriteEnglish([NotNull] this TextWriter writer, double number)
        {
            Contract.Requires(writer != null);
            WriteEnglish(writer, (long)number);

            if (number < 0)
                number = Math.Abs(number);

            // Note this isn't very safe, we cast to a decimal due to the lack of precision in double making it near impossible
            // to accurately tell when we're finished.
            decimal d;
            unchecked
            {
                d = (decimal)number;
            }
            // Get the approximate number after the point.
            bool first = true;
            while ((d = d % 1) > 0)
            {
                if (first)
                {
                    writer.Write(" Point");
                    first = false;
                }
                d *= 10;
                writer.Write(' ');
                writer.Write(_onesMapping[(int)d]);
            }
        }

        /// <summary>
        /// Writes an <see cref="int" /> in its English textual equivalent.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="number">The number.</param>
        [PublicAPI]
        public static void WriteEnglish([NotNull] this TextWriter writer, long number)
        {
            Contract.Requires(writer != null);
            bool start = true;
            if (number < 0)
            {
                writer.Write("Negative");
                start = false;
                number = Math.Abs(number);
            }
            if (number < 1)
            {
                writer.Write(_onesMapping[0]);
                return;
            }

            Stack<int, byte> groups = new Stack<int, byte>();
            byte group = 0;
            while (number >= 1)
            {
                groups.Push((int)(number % 1000), @group++);
                number = number / 1000;
            }

            int firstGroup = groups.Count - 1;
            while (groups.Count > 0)
            {
                int numberToProcess;
                groups.Pop(out numberToProcess, out @group);

                if (numberToProcess < 1) continue;

                int tens = numberToProcess % 100;
                int hundreds = numberToProcess / 100;

                if (hundreds > 0)
                {
                    if (!start)
                        writer.Write(' ');
                    else
                        start = false;

                    writer.Write(_onesMapping[hundreds]);
                    writer.Write(' ');
                    writer.Write(_groupMapping[0]);
                }

                if (tens > 0)
                {
                    if (hundreds > 0 ||
                        @group < firstGroup)
                        writer.Write(" And");

                    if (!start)
                        writer.Write(' ');
                    else
                        start = false;

                    if (tens < 20)
                        writer.Write(_onesMapping[tens]);
                    else
                    {
                        tens = (tens / 10) - 2; // 20's offset

                        writer.Write(_tensMapping[tens]);

                        int ones = tens % 10;
                        if (ones > 0)
                        {
                            writer.Write(' ');
                            writer.Write(_onesMapping[tens]);
                        }
                    }
                }

                if (@group > 0)
                {
                    writer.Write(' ');
                    writer.Write(_groupMapping[@group]);
                }
            }
        }

        /// <summary>
        ///   Performs an equality between the two <see cref="object"/>s, respecting their runtime type.
        /// </summary>
        /// <param name="objA">The first object to compare.</param>
        /// <param name="objB">The second object to compare.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the objects value and type are equal; otherwise returns <see langword="false"/>.
        /// </returns>
        /// <remarks>
        ///   This is slow compared to a standard equality and the vast majority of times this can be replaced with a solution 
        ///   that leverages generics, or by making a call to <see cref="GetTypeEqualityFunction"/> and storing the resulting function
        ///   to avoid the dictionary lookup.
        /// </remarks>
        public static bool EqualsByRuntimeType([CanBeNull] this object objA, [CanBeNull] object objB)
        {
            if (objA == null)
                return objB == null;

            if (objB == null)
                return false;

            // Get the runtime type
            Type type = objA.GetType();

            return GetTypeEqualityFunction(type)(objA, objB);
        }

        /// <summary>
        ///   Gets the type's equality function as a lambda expression.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The type's equality function.</returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="type"/> was <see langword="null"/>
        /// </exception>
        [NotNull]
        public static Func<object, object, bool> GetTypeEqualityFunction([NotNull] this Type type)
        {
            Contract.Requires(type != null);

            return _equalityFunctions.GetOrAdd(
                type,
                t =>
                {
                    ParameterInfo[] plist;
                    int pCount = 0;
                    // Grab the most relevant equals method
                    MethodInfo equalsMethod =
                        (t.GetMethods(
                            BindingFlags.FlattenHierarchy | BindingFlags.Public |
                            BindingFlags.InvokeMethod | BindingFlags.Instance |
                            BindingFlags.Static)
                            .Where(
                                mi => (mi.Name == "Equals") &&
                                      ((pCount = (plist = mi.GetParameters()).Count()) < 3) &&
                                      (pCount > 0) && plist[0].ParameterType.IsAssignableFrom(t) &&
                                      (mi.IsStatic
                                          ? (pCount == 2) &&
                                            plist[1].ParameterType.IsAssignableFrom(t)
                                          : (pCount == 1))))
                            .First();

                    return equalsMethod.Func<object, object, bool>(false);
                });
        }

        /// <summary>
        /// Compares two enumerables to see if they contain the same elements.
        /// </summary>
        /// <typeparam name="T">The type of the object contained in the enumerable.</typeparam>
        /// <param name="enumerableA">The first enumerable object.</param>
        /// <param name="enumerableB">The second enumerable object</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns>
        /// Returns <see langword="true" /> if both of the enumerable objects are the same size and contain the same elements.
        /// </returns>
        /// <remarks>
        /// <para>This does a robust check of enumerable equality with an algorithm that's no worse that O(N).</para>
        /// <para>The order in which items appear in the enumerable doesn't matter. For example {1, 2, 2, 3} would be
        /// considered equal to {2, 1, 3, 2} but not equal to {1, 2, 3, 3}.</para>
        /// <para>If the list does not contain duplicate items use DeepEqualsSimple.</para>
        /// <para>If the lists are sorted then use <see cref="System.Linq.Enumerable" />'s Sequence Equal.</para>
        /// </remarks>
        [PublicAPI]
        public static bool DeepEquals<T>(
            [CanBeNull] [InstantHandle] this IEnumerable<T> enumerableA,
            [CanBeNull] [InstantHandle] IEnumerable<T> enumerableB,
            [CanBeNull] IEqualityComparer<T> comparer = null)
        {
            // Check for nulls
            if (enumerableA == null)
                return enumerableB == null;
            if (enumerableB == null)
                return false;

            IReadOnlyCollection<T> enumA = enumerableA.Enumerate();
            IReadOnlyCollection<T> enumB = enumerableB.Enumerate();

            int count;
            // Check counts are the same
            if ((count = enumA.Count) != enumB.Count)
                return false;

            // Create counters for each element in first enumerable
            Dictionary<T, TypeCounter> counters = new Dictionary<T, TypeCounter>(count, comparer);
            TypeCounter t;

            // Nulls cannot be used as the key of a dictionary so have a separate counter for them
            TypeCounter nullCounter = null;
            foreach (T element in enumA)
            {
                if (ReferenceEquals(element, null))
                {
                    if (nullCounter == null)
                        nullCounter = new TypeCounter();
                    nullCounter.Increment();
                    continue;
                }

                if (counters.TryGetValue(element, out t))
                {
                    t.Increment();
                    continue;
                }

                t = new TypeCounter();
                counters.Add(element, t);
            }

            // Decrement counters in second enumerable
            foreach (T element in enumB)
            {
                if (ReferenceEquals(element, null))
                {
                    if (nullCounter == null)
                        return false;

                    nullCounter.Decrement();
                    if (nullCounter.Count < 0)
                        return false;

                    continue;
                }

                //  If we have an element that was not present in the first enumerable, enumerables are not equal
                if (!counters.TryGetValue(element, out t))
                    return false;

                t.Decrement();
                if (t.Count < 0)
                    return false;
            }

            // If any of the counters are not zero then we had unequal counts.
            return (nullCounter == null || nullCounter.Count == 0) &&
                   !counters.Values.Any(counter => counter.Count != 0);
        }

        /// <summary>
        ///   Compares two enumerables to see if they contain the same elements.
        /// </summary>
        /// <typeparam name="T">The type of the object contained in the enumerable.</typeparam>
        /// <param name="enumerableA">The first enumerable object.</param>
        /// <param name="enumerableB">The second enumerable object</param>
        /// <param name="comparer">The equality comparer.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if both of the enumerable objects are the same size and contain the same elements.
        /// </returns>
        /// <remarks>
        ///   <para>For speed, the number of times elements appear in each enumerable is not taken into account, therefore a
        ///   list of {1, 1, 2} and {1, 2, 2} would be considered equal. Therefore it is recommended that this is only used for
        ///   enumerables that do not contain duplicates.</para>
        ///   <para>If the lists are sorted then use <see cref="System.Linq.Enumerable"/>'s Sequence Equal.</para>
        /// </remarks>
        [PublicAPI]
        public static bool DeepEqualsSimple<T>(
            [CanBeNull] [InstantHandle] this IEnumerable<T> enumerableA,
            [CanBeNull] [InstantHandle] IEnumerable<T> enumerableB,
            [CanBeNull] IEqualityComparer<T> comparer = null)
        {
            if (enumerableA == null) return enumerableB == null;
            if (enumerableB == null) return false;

            IReadOnlyCollection<T> enumA = enumerableA.Enumerate();
            IReadOnlyCollection<T> enumB = enumerableB.Enumerate();

            return enumA.Count == enumB.Count && enumA.All(i => enumB.Contains(i, comparer));
        }

        /// <summary>
        ///   Converts a NameValueCollection to a Dictionary.
        /// </summary>
        /// <param name="collection">The <see cref="System.Collections.Specialized.NameValueCollection"/>.</param>
        /// <returns>
        ///   The <see cref="T:System.Collections.Generic.IDictionary`2">Dictionary</see> version of the <paramref name="collection"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="collection"/> is a <see langword="null"/>.
        /// </exception>
        [PublicAPI]
        [NotNull]
        public static Dictionary<string, string> ToDictionary([NotNull] this NameValueCollection collection)
        {
            Contract.Requires(collection != null);

            Dictionary<string, string> d = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
            foreach (string name in collection)
                // ReSharper disable once AssignNullToNotNullAttribute
                d.Add(name, collection[name]);
            return d;
        }

        /// <summary>
        /// Creates a <see cref="Dictionary{TKey,TValue}"/> from an <see cref="IEnumerable{KeyValuePair}"/>
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{KeyValuePair}"/> to create a <see cref="Dictionary{TKey,TValue}"/> from.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{TKey}"/> to compare keys.</param>
        [NotNull]
        [PublicAPI]
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(
            [NotNull] [InstantHandle] this IEnumerable<KeyValuePair<TKey, TValue>> source,
            [CanBeNull] IEqualityComparer<TKey> comparer = null)
        {
            Contract.Requires(source != null);

            Dictionary<TKey, TValue> d = new Dictionary<TKey, TValue>(comparer);

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (KeyValuePair<TKey, TValue> kvp in source)
                // ReSharper disable once AssignNullToNotNullAttribute
                d.Add(kvp.Key, kvp.Value);

            return d;
        }

        /// <summary>
        /// Encodes a string for JSON.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        [NotNull]
        [PublicAPI]
        public static string ToJSON([CanBeNull] this string input)
        {
            if (input == null)
                return "null";
            // TODO establish approx. increase in length of JSON encoding.
            StringBuilder stringBuilder = new StringBuilder((int)(input.Length * 1.3) + 2);
            stringBuilder.AppendJSON(input);
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Appends a string to the specified <see cref="StringBuilder"/> encoding it for JSON.
        /// </summary>
        /// <param name="stringBuilder">The string builder.</param>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        [NotNull]
        [PublicAPI]
        public static StringBuilder AppendJSON([NotNull] this StringBuilder stringBuilder, [CanBeNull] string input)
        {
            Contract.Requires(stringBuilder != null);

            if (input == null)
            {
                stringBuilder.Append("null");
                return stringBuilder;
            }

            stringBuilder.Append("\"");
            foreach (char c in input)
            {
                string replacement;

                // TODO May be worth adding escaping for unicode character.
                if (_jsonEscapedCharacters.TryGetValue(c, out replacement))
                    stringBuilder.Append(replacement);
                else
                    stringBuilder.Append(c);
            }
            stringBuilder.Append("\"");

            return stringBuilder;
        }

        /// <summary>
        ///   Converts a list to its JSON representation.
        /// </summary>
        /// <param name="list">The list to convert.</param>
        /// <returns>The <paramref name="list"/> in JSON.</returns>
        /// <remarks>
        ///   Can be used with other enumerable objects such as Queues, Stacks, etc.
        /// </remarks>
        [NotNull]
        [PublicAPI]
        public static string ToJSON([CanBeNull] [InstantHandle] this IEnumerable<string> list)
        {
            if (list == null)
                return "[]";

            IReadOnlyCollection<string> collection = list.Enumerate();
            if (collection.Count < 1)
                return "[]";

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("[");
            bool includeComma = false;
            foreach (string s in collection)
            {
                if (includeComma)
                    stringBuilder.Append(",");
                else
                    includeComma = true;
                stringBuilder.AppendJSON(s);
            }

            stringBuilder.Append("]");
            return stringBuilder.ToString();
        }

        [NotNull]
        [ItemNotNull]
        private static readonly string[] _hexLower = Enumerable.Range(0, 255).Select(i => i.ToString("x2")).ToArray();

        [NotNull]
        [ItemNotNull]
        private static readonly string[] _hexUpper = Enumerable.Range(0, 255).Select(i => i.ToString("X2")).ToArray();

        /// <summary>
        /// Converts a byte to lower case hex.
        /// </summary>
        /// <param name="value">The value.</param>
        [NotNull]
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToHexLower(this byte value)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            return _hexLower[value];
        }

        /// <summary>
        /// Converts an array of bytes to lower case hex.
        /// </summary>
        /// <param name="value">The value.</param>
        [NotNull]
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToHexLower(this byte[] value)
        {
            Contract.Requires(value != null);
            char[] chars = new char[value.Length << 1];
            for (int i = 0, j = 0; i < value.Length; i++)
            {
                string h = _hexLower[value[i]];
                chars[j++] = h[0];
                chars[j++] = h[1];
            }
            return new string(chars);
        }

        /// <summary>
        /// Converts a byte to upper case hex.
        /// </summary>
        /// <param name="value">The value.</param>
        [NotNull]
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToHexUpper(this byte value)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            return _hexUpper[value];
        }

        /// <summary>
        /// Converts an array of bytes to upper case hex.
        /// </summary>
        /// <param name="value">The value.</param>
        [NotNull]
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToHexUpper([NotNull] this byte[] value)
        {
            Contract.Requires(value != null);
            char[] chars = new char[value.Length << 1];
            for (int i = 0, j = 0; i < value.Length; i++)
            {
                string h = _hexUpper[value[i]];
                chars[j++] = h[0];
                chars[j++] = h[1];
            }
            return new string(chars);
        }

        /// <summary>
        ///   Takes a <see cref="string"/> of <see cref="int">integer</see> ids and calls a function to retrieve an
        ///   enumeration of <see cref="object"/>s.
        /// </summary>
        /// <typeparam name="T">The result type for <paramref name="getObject"/>.</typeparam>
        /// <param name="integers">The integers to split.</param>
        /// <param name="getObject">A function that takes in the split integer and can be used to retrieve an object.</param>
        /// <param name="splitChars">
        ///   <para>The characters used to separate the <paramref name="integers"/>.</para>
        ///   <para>This defaults to ' ' , \t \r \n or |.</para>
        /// </param>
        /// <param name="executeImmediately">
        ///   If set to <see langword="true"/> executes object retrieval instantly; otherwise uses deferred execution.
        /// </param>
        /// <returns>The enumerator to iterate through the retrieved objects.</returns>
        /// <exception cref="NullReferenceException"><paramref name="integers"/> is <see langword="null"/>.</exception>
        [NotNull]
        [PublicAPI]
        public static IEnumerable<T> GetObjectsById<T>(
            [NotNull] this string integers,
            [NotNull] [InstantHandle] Func<int, T> getObject,
            [CanBeNull] char[] splitChars = null,
            bool executeImmediately = false)
        {
            Contract.Requires<ArgumentNullException>(integers != null);
            Contract.Requires<ArgumentNullException>(getObject != null);

            IEnumerable<T> enumeration =
                integers.Split(splitChars ?? DefaultSplitChars, StringSplitOptions.RemoveEmptyEntries).Select(
                    s =>
                    {
                        int id;
                        return Int32.TryParse(s, out id) ? (int?)id : null;
                    }).Where(id => id.HasValue)
                    .Distinct()
                    .Select(id => getObject(id.Value));

            if (!typeof(T).IsValueType)
                enumeration = enumeration.Where(o => !ReferenceEquals(o, null));

            // Only retrieve distinct elements
            enumeration = enumeration.Distinct();

            // If executing immediately, we need to convert to a list
            if (executeImmediately)
                enumeration = enumeration.ToList();

            return enumeration;
        }

        /// <summary>
        ///   Takes a <see cref="string"/> of <see cref="int">integer</see> ids and calls a function to retrieve an enumeration of objects.
        /// </summary>
        /// <typeparam name="T">The result type for <paramref name="getObject"/>.</typeparam>
        /// <param name="integers">The integers to split.</param>
        /// <param name="getObject">A function that takes in the split integer as a short and can be used to retrieve an object.</param>
        /// <param name="splitChars">
        ///   <para>The characters used to separate the <paramref name="integers"/>.</para>
        ///   <para>This defaults to ' ' , \t \r \n or |.</para>
        /// </param>
        /// <param name="executeImmediately">
        ///   If set to <see langword="true"/> executes object retrieval instantly; otherwise uses deferred execution.
        /// </param>
        /// <returns>The enumerator to iterate through the retrieved objects.</returns>
        /// <exception cref="NullReferenceException"><paramref name="integers"/> is <see langword="null"/>.</exception>
        [NotNull]
        [PublicAPI]
        public static IEnumerable<T> GetObjectsById16<T>(
            [NotNull] this string integers,
            [NotNull] [InstantHandle] Func<short, T> getObject,
            [CanBeNull] char[] splitChars = null,
            bool executeImmediately = false)
        {
            Contract.Requires<ArgumentNullException>(integers != null);
            Contract.Requires<ArgumentNullException>(getObject != null);

            IEnumerable<T> enumeration =
                integers.Split(splitChars ?? DefaultSplitChars, StringSplitOptions.RemoveEmptyEntries).Select(
                    s =>
                    {
                        Int16 id;
                        return Int16.TryParse(s, out id) ? (Int16?)id : null;
                    }).Where(id => id.HasValue)
                    .Distinct()
                    .Select(id => getObject(id.Value));
            if (!typeof(T).IsValueType)
                enumeration = enumeration.Where(o => o != null);

            // Only retrieve distinct elements
            enumeration = enumeration.Distinct();

            // If executing immediately, we need to convert to a list
            if (executeImmediately)
                enumeration = enumeration.ToList();

            return enumeration;
        }

        /// <summary>
        ///   Takes a <see cref="string"/> of <see cref="int">integer</see> ids and calls a function to retrieve an enumeration of objects.
        /// </summary>
        /// <typeparam name="T">The result type for <paramref name="getObject"/>.</typeparam>
        /// <param name="integers">The integers to split.</param>
        /// <param name="getObject">A function that takes in the split integer as a long and can be used to retrieve an object.</param>
        /// <param name="splitChars">
        ///   <para>The characters used to separate the <paramref name="integers"/>.</para>
        ///   <para>This defaults to ' ' , \t \r \n or |.</para>
        /// </param>
        /// <param name="executeImmediately">
        ///   If set to <see langword="true"/> executes object retrieval instantly; otherwise uses deferred execution.
        /// </param>
        /// <returns>The enumerator to iterate through the retrieved objects.</returns>
        /// <exception cref="NullReferenceException"><paramref name="integers"/> is <see langword="null"/>.</exception>
        [NotNull]
        [PublicAPI]
        public static IEnumerable<T> GetObjectsById64<T>(
            [NotNull] this string integers,
            [NotNull] [InstantHandle] Func<long, T> getObject,
            [CanBeNull] char[] splitChars = null,
            bool executeImmediately = false)
        {
            Contract.Requires<ArgumentNullException>(integers != null);
            Contract.Requires<ArgumentNullException>(getObject != null);

            IEnumerable<T> enumeration =
                integers.Split(splitChars ?? DefaultSplitChars, StringSplitOptions.RemoveEmptyEntries).Select(
                    s =>
                    {
                        Int64 id;
                        return Int64.TryParse(s, out id) ? (Int64?)id : null;
                    }).Where(id => id.HasValue)
                    .Distinct()
                    .Select(id => getObject(id.Value));
            if (!typeof(T).IsValueType)
                enumeration = enumeration.Where(o => o != null);

            // Only retrieve distinct elements
            enumeration = enumeration.Distinct();

            // If executing immediately, we need to convert to a list
            if (executeImmediately)
                enumeration = enumeration.ToList();

            return enumeration;
        }

        /// <summary>
        ///   Escapes invalid XML characters from a <see cref="string"/>.
        /// </summary>
        /// <param name="raw">The raw string.</param>
        /// <returns>The escaped <see cref="string"/>.</returns>
        [CanBeNull]
        [ContractAnnotation("raw:null => null; raw:notnull => notnull")]
        [PublicAPI]
        public static string XmlEscape([CanBeNull] this string raw)
        {
            string stripped = String.IsNullOrEmpty(raw)
                ? raw
                : new string(raw.Where(c => c.IsValidXmlChar()).ToArray());
            return SecurityElement.Escape(stripped);
        }

        /// <summary>
        ///   Escapes invalid XML characters from the <see cref="string"/> representation of the specified <see cref="object"/>.
        /// </summary>
        /// <param name="raw">The object containing the raw value.</param>
        /// <returns>The escaped <see cref="string"/>.</returns>
        [NotNull]
        [PublicAPI]
        public static string XmlEscape([NotNull] this object raw)
        {
            Contract.Requires<ArgumentNullException>(raw != null);
            return raw.ToString().XmlEscape();
        }

        /// <summary>
        /// Determines whether the specified character is valid in an XML document.
        /// </summary>
        /// <param name="c">The character.</param>
        /// <returns><see langword="true" /> if the character is valid; otherwise, <see langword="false" />.</returns>
        /// <remarks>
        /// <para>This does not specify whether the character needs to be further encoded further.</para>
        /// <para>Consider using <see cref="IsValidXmlChar"/> to also exclude characters that can cause compaibility issues.</para>
        /// <para>See http://www.w3.org/TR/xml/#charsets for further info.</para>
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [PublicAPI]
        public static bool IsValidXmlCharStrict(this char c)
        {
            return (c >= 0x20 && c <= 0xD7FF) ||
                   (c >= 0xE000 && c <= 0xFFFD) ||
                   (c == 0x9) ||
                   (c == 0xA) ||
                   (c == 0xD) ||
                   (c == 0x85);
        }

        /// <summary>
        /// Determines whether the specified character is valid in an XML document and won't cause compatibility issues.
        /// </summary>
        /// <param name="c">The character.</param>
        /// <returns><see langword="true" /> if the character is valid; otherwise, <see langword="false" />.</returns>
        /// <remarks>
        /// <para>This does not specify whether the character needs to be further encoded further.</para>
        /// <para>This is the preferred compatibility check (compared to <see cref="IsValidXmlCharStrict"/> as it
        /// excludes characters that can cause issues.</para>
        /// <para>See http://www.w3.org/TR/xml/#charsets for further info.</para>
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [PublicAPI]
        public static bool IsValidXmlChar(this char c)
        {
            return (c >= 0x20 && c <= 0x7E) ||
                   (c >= 0xA0 && c <= 0xD7FF) ||
                   (c >= 0xE000 && c <= 0xFDCF) ||
                   (c >= 0xFDF0 && c <= 0xFFFD) ||
                   (c == 0x9) ||
                   (c == 0xA) ||
                   (c == 0xD) ||
                   (c == 0x85);
        }

        /// <summary>
        ///   Gets the embedded XML from an assembly with the specified filename.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="filename">The filename.</param>
        /// <returns>
        ///   An <see cref="System.Xml.Linq.XDocument">XDocument</see> containing the retrieved XML.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <para><paramref name="assembly"/> is a <see langword="null"/>.</para>
        ///   <para>-or-</para>
        ///   <para><paramref name="filename"/> is a <see langword="null"/>, empty or consists of only whitespace.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///   Couldn't load the embedded resource from the specified <paramref name="assembly"/>.
        /// </exception>
        [NotNull]
        [PublicAPI]
        public static XDocument GetEmbeddedXml([NotNull] this Assembly assembly, [NotNull] string filename)
        {
            try
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (assembly == null)
                    // ReSharper disable HeuristicUnreachableCode
                    throw new ArgumentNullException("assembly");
                // ReSharper restore HeuristicUnreachableCode

                if (String.IsNullOrWhiteSpace(filename))
                    throw new ArgumentNullException("filename");

                using (Stream stream = assembly.GetManifestResourceStream(filename))
                {
                    if (stream == null)
                        throw new InvalidOperationException(
                            String.Format(
                                Resources.Extensions_EmbeddedXml_CouldntLoadEmbeddedResource,
                                filename,
                                assembly));
                    return XDocument.Load(stream);
                }
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(
                    String.Format(
                        Resources.Extensions_EmbeddedXml_Exception,
                        filename,
                        assembly,
                        e.Message),
                    e);
            }
        }

        /// <summary>
        ///   Gets the embedded XmlSchemaSet (XSD) from an assembly with the specified filename.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="targetNamespace">The target namespace.</param>
        /// <returns>
        ///   An <see cref="System.Xml.Schema.XmlSchemaSet"/> containing the retrieved schema.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <para><paramref name="assembly"/> is a <see langword="null"/>.</para>
        ///   <para>-or-</para>
        ///   <para><paramref name="filename"/> is a <see langword="null"/>, empty or consists of only whitespace.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///   Couldn't load the embedded resource from the specified <paramref name="assembly"/>.
        /// </exception>
        [NotNull]
        [PublicAPI]
        public static XmlSchemaSet GetEmbeddedXmlSchemaSet(
            [NotNull] this Assembly assembly,
            [NotNull] string filename,
            [NotNull] string targetNamespace = "")
        {
            try
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (assembly == null)
                    // ReSharper disable HeuristicUnreachableCode
                    throw new ArgumentNullException("assembly");
                // ReSharper restore HeuristicUnreachableCode

                if (String.IsNullOrWhiteSpace(filename))
                    throw new ArgumentNullException("filename");

                using (Stream stream = assembly.GetManifestResourceStream(filename))
                {
                    if (stream == null)
                        throw new InvalidOperationException(
                            String.Format(
                                Resources.Extensions_EmbeddedXml_CouldntLoadEmbeddedResource,
                                filename,
                                assembly));

                    using (XmlReader reader = XmlReader.Create(stream))
                    {
                        XmlSchemaSet schema = new XmlSchemaSet();
                        schema.Add(targetNamespace, reader);
                        return schema;
                    }
                }
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(
                    String.Format(
                        Resources.Extensions_EmbeddedXml_Exception,
                        filename,
                        assembly,
                        e.Message),
                    e);
            }
        }

        /// <summary>
        ///   Gets the epoch time.
        ///   This is the number of milliseconds since the 1st January 1970 to the specified <see cref="DateTime"/>.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>The number of milliseconds since the 1st January 1970 to the <paramref name="dateTime"/> specified.</returns>
        /// <remarks>
        ///   This is the best way to pass a date time to JavaScript because it ensures that the time will parse successfully
        ///   regardless of the <see cref="DateTime">date</see>'s localisation/format.
        /// </remarks>
        [PublicAPI]
        public static Int64 GetEpochTime(this DateTime dateTime)
        {
            return (Int64)(dateTime - EpochStart).TotalMilliseconds;
        }

        /// <summary>
        ///   Gets the <see cref="DateTime"/> from an epoch time.
        /// </summary>
        /// <param name="epochTime">The epoch time (the number of milliseconds since the 1st January 1970).</param>
        /// <returns>The <paramref name="epochTime"/> as a <see cref="DateTime"/> object.</returns>
        /// <remarks>
        ///   This is the best way to retrieve a date time from JavaScript because it ensures that the time will parse successfully
        ///   regardless of the <see cref="DateTime">date</see>'s localisation/format.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   The result was either less than <see cref="DateTime.MinValue"/> or greater than <see cref="DateTime.MaxValue"/>.
        /// </exception>
        [PublicAPI]
        public static DateTime GetDateTime(Int64 epochTime)
        {
            return EpochStart.AddMilliseconds(epochTime);
        }

        /// <summary>
        ///   Strips the HTML tags from the specified <see cref="string"/>.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>
        ///   A <see cref="string"/> containing the contents of <paramref name="input"/> but with the HTML tags stripped out.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="input"/> was <see langword="null"/>.
        /// </exception>
        [NotNull]
        [PublicAPI]
        public static string StripHTML([NotNull] this string input)
        {
            Contract.Requires(input != null);
            // TODO: a) make more efficient b) question purpose/usage
            int depthCounter = 0;
            StringBuilder builder = new StringBuilder(input.Length);
            foreach (char c in input)
                if (c == '<')
                    depthCounter++;
                else if (depthCounter > 0)
                {
                    if (c == '>')
                        depthCounter--;
                }
                else
                    builder.Append(c);
            return builder.ToString();
        }

        /// <summary>
        ///   Truncates the <see cref="string"/> input to the specified length and appends on an ellipsis if specified.
        /// </summary>
        /// <param name="valueToTruncate">The string input.</param>
        /// <param name="maxLength">The length to truncate to.</param>
        /// <param name="options">The truncate options.</param>
        /// <param name="ellipsisString">The ellipsis string.</param>
        /// <param name="ellipsisLength">
        ///   <para>The length that the ellipsis contributes to the <paramref name="maxLength"/>.</para>
        ///   <para>If no value is specified then this is the length of <paramref name="ellipsisString"/>.</para>
        /// </param>
        /// <returns>The truncated <see cref="string"/>.</returns>
        /// <exception cref="NullReferenceException">
        ///   <paramref name="ellipsisString"/> is a <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="maxLength"/> is less than the <paramref name="ellipsisLength"/>.
        /// </exception>
        [NotNull]
        [PublicAPI]
        public static string Truncate(
            [CanBeNull] this string valueToTruncate,
            int maxLength,
            TruncateOptions options = TruncateOptions.None,
            [CanBeNull] string ellipsisString = "...",
            int ellipsisLength = -1)
        {
            Contract.Requires<ArgumentOutOfRangeException>(maxLength > 0);
            Contract.Requires<ArgumentOutOfRangeException>(
                (options & TruncateOptions.IncludeEllipsis) != TruncateOptions.IncludeEllipsis ||
                maxLength > (ellipsisString != null && ellipsisLength < 0 ? ellipsisString.Length : ellipsisLength));

            if (String.IsNullOrEmpty(valueToTruncate) ||
                valueToTruncate.Length <= maxLength)
                return valueToTruncate ?? String.Empty;

            if (ellipsisLength < 0)
                ellipsisLength = ellipsisString.Length;

            bool includeEllipsis = (options & TruncateOptions.IncludeEllipsis) == TruncateOptions.IncludeEllipsis;
            bool finishWord = (options & TruncateOptions.FinishWord) == TruncateOptions.FinishWord;
            bool allowLastWordOverflow =
                (options & TruncateOptions.AllowLastWordToGoOverMaxLength) ==
                TruncateOptions.AllowLastWordToGoOverMaxLength;

            string retValue = valueToTruncate;

            if (includeEllipsis)
                maxLength -= ellipsisLength;

            int lastSpaceIndex = retValue.LastIndexOf(" ", maxLength, StringComparison.CurrentCultureIgnoreCase);

            if (!finishWord)
                retValue = retValue.Remove(maxLength);
            else if (allowLastWordOverflow)
            {
                int spaceIndex = retValue.IndexOf(" ", maxLength, StringComparison.CurrentCultureIgnoreCase);
                if (spaceIndex != -1)
                    retValue = retValue.Remove(spaceIndex);
            }
            else if (lastSpaceIndex > -1)
                retValue = retValue.Remove(lastSpaceIndex);
            else
                retValue = "";

            return String.Format(
                "{0}{1}",
                retValue,
                includeEllipsis && retValue.Length < valueToTruncate.Length
                    ? ellipsisString
                    : String.Empty);
        }

        /// <summary>
        ///   Converts the specified value from degrees to radians.
        /// </summary>
        /// <param name="d">The value in degrees to convert.</param>
        /// <returns>The value (<paramref name="d"/>) in radians.</returns>
        [PublicAPI]
        public static double ToRadians(this double d)
        {
            return (Math.PI / 180) * d;
        }

        /// <summary>
        ///   Converts the specified value from radians to degrees.
        /// </summary>
        /// <param name="r">The value in radians to convert.</param>
        /// <returns>The value (<paramref name="r"/>) in degrees.</returns>
        [PublicAPI]
        public static double ToDegrees(this double r)
        {
            return r * (180.0 / Math.PI);
        }

        /// <summary>
        /// The lower-case hexadecimal digits.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly char[] HexDigitsLower = "0123456789abcdef".ToCharArray();

        /// <summary>
        /// Unescapes the specified string.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>System.String.</returns>
        [NotNull]
        [PublicAPI]
        public static string Unescape([CanBeNull] this string str)
        {
            if (String.IsNullOrEmpty(str)) return str ?? String.Empty;
            StringBuilder builder = new StringBuilder(str.Length);
            builder.AddUnescaped(str);
            return builder.ToString();
        }

        /// <summary>
        /// Unescapes the specified string.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>System.String.</returns>
        [NotNull]
        [PublicAPI]
        public static string Escape([CanBeNull] this string str)
        {
            if (String.IsNullOrEmpty(str)) return str ?? String.Empty;
            StringBuilder builder = new StringBuilder(str.Length + 10);
            builder.AddEscaped(str);
            return builder.ToString();
        }

        /// <summary>
        /// Adds the string, removing unescaping.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="str">The string.</param>
        [PublicAPI]
        public static void AddUnescaped([NotNull] this StringBuilder builder, [CanBeNull] string str)
        {
            Contract.Requires(builder != null);
            if (String.IsNullOrEmpty(str)) return;
            int i = 0;
            bool escaped = false;
            while (i < str.Length)
            {
                char c = str[i++];
                if (!escaped)
                {
                    if (c == '\\')
                    {
                        escaped = true;
                        continue;
                    }
                    builder.Append(c);
                    continue;
                }

                escaped = false;
                switch (c)
                {
                    case '0':
                        builder.Append('\0');
                        break;
                    case 'a':
                        builder.Append('\a');
                        break;
                    case 'b':
                        builder.Append('\b');
                        break;
                    case 'f':
                        builder.Append('\f');
                        break;
                    case 'n':
                        builder.Append('\n');
                        break;
                    case 'r':
                        builder.Append('\r');
                        break;
                    case 't':
                        builder.Append('\t');
                        break;
                    case 'v':
                        builder.Append('\v');
                        break;
                    case 'u':
                        if (i + 4 > str.Length)
                        {
                            builder.Append(c);
                            continue;
                        }
                        string d4 = str.Substring(i, i + 4);
                        int n4;
                        if (!Int32.TryParse(d4, NumberStyles.HexNumber, null, out n4))
                        {
                            builder.Append(c);
                            continue;
                        }
                        builder.Append((Char)n4);
                        i += 4;
                        break;
                    case 'U':
                        if (i + 8 > str.Length)
                        {
                            builder.Append(c);
                            continue;
                        }
                        string d8 = str.Substring(i, i + 8);
                        int n8;
                        if (!Int32.TryParse(d8, NumberStyles.HexNumber, null, out n8))
                        {
                            builder.Append(c);
                            continue;
                        }
                        builder.Append(Char.ConvertFromUtf32(n8));
                        i += 8;
                        break;
                    case 'x':
                        if (i + 1 > str.Length)
                        {
                            builder.Append(c);
                            continue;
                        }
                        int j = 0;
                        StringBuilder dx = new StringBuilder(4);
                        while ((i + j) < str.Length &&
                               (j < 4))
                        {
                            char h = str[i + j++];
                            if (!HexDigitsLower.Contains(h)) break;
                            dx.Append(h);
                        }
                        int nx;
                        if ((dx.Length < 1) ||
                            !Int32.TryParse(dx.ToString(), NumberStyles.HexNumber, null, out nx))
                        {
                            builder.Append(c);
                            continue;
                        }
                        builder.Append((Char)nx);
                        i += j;
                        break;
                    default:
                        builder.Append(c);
                        break;
                }
            }
            if (escaped)
                builder.Append('\\');
        }

        /// <summary>
        /// Adds the string, escaping characters.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="str">The string.</param>
        [PublicAPI]
        public static void AddEscaped([NotNull] this StringBuilder builder, [CanBeNull] string str)
        {
            Contract.Requires(builder != null);
            if (String.IsNullOrEmpty(str)) return;
            int i = 0;
            while (i < str.Length)
            {
                char c = str[i++];
                switch (c)
                {
                    case '\'':
                        builder.Append(@"\'");
                        break;
                    case '\"':
                        builder.Append("\\\"");
                        break;
                    case '\\':
                        builder.Append(@"\\");
                        break;
                    case '\0':
                        builder.Append(@"\0");
                        break;
                    case '\a':
                        builder.Append(@"\a");
                        break;
                    case '\b':
                        builder.Append(@"\b");
                        break;
                    case '\f':
                        builder.Append(@"\f");
                        break;
                    case '\n':
                        builder.Append(@"\n");
                        break;
                    case '\r':
                        builder.Append(@"\r");
                        break;
                    case '\t':
                        builder.Append(@"\t");
                        break;
                    case '\v':
                        builder.Append(@"\v");
                        break;
                    default:
                        if (Char.GetUnicodeCategory(c) != UnicodeCategory.Control)
                            builder.Append(c);
                        else
                            builder.Append(@"\u")
                                .Append(HexDigitsLower[c >> 12 & 0x0F])
                                .Append(HexDigitsLower[c >> 8 & 0x0F])
                                .Append(HexDigitsLower[c >> 4 & 0x0F])
                                .Append(HexDigitsLower[c & 0x0F]);
                        break;
                }
            }
        }

        /// <summary>
        ///   Gets the creation date from a <see cref="Guid"/> that is a <see cref="CombGuid"/>.
        /// </summary>
        /// <param name="guid">The Guid.</param>
        /// <returns>The <see cref="System.Guid"/>'s creation <see cref="DateTime"/>.</returns>
        [PublicAPI]
        public static DateTime GetDateTime(this Guid guid)
        {
            return CombGuid.GetDateTime(guid);
        }

        /// <summary>
        ///   Wraps the specified <see cref="IAsyncResult"/> as an
        ///   <see cref="WebApplications.Utilities.Threading.ApmWrap&lt;T&gt;"/> object, allowing associated data to be attached.
        /// </summary>
        /// <typeparam name="T">The type of the data to embed.</typeparam>
        /// <param name="result">The result to wrap.</param>
        /// <param name="data">The data to embed.</param>
        /// <returns>The wrapped result.</returns>
        [Obsolete("Consider using TPL or Async.")]
        [NotNull]
        [PublicAPI]
        public static IAsyncResult Wrap<T>([NotNull] this IAsyncResult result, T data)
        {
            return ApmWrap<T>.Wrap(result, data);
        }

        /// <summary>
        ///   Unwraps the specified result and gets the <see cref="IAsyncResult"/> as well as the attached data.
        /// </summary>
        /// <typeparam name="T">The type of the embedded data.</typeparam>
        /// <param name="result">The result to unwrap.</param>
        /// <returns>The embedded data.</returns>
        /// <seealso cref="T:WebApplications.Utilities.Threading.ApmWrap`1"/>
        [Obsolete("Consider using TPL or Async.")]
        [PublicAPI]
        public static T Unwrap<T>([NotNull] this IAsyncResult result)
        {
            Contract.Requires(result != null);
            return ApmWrap<T>.Unwrap(ref result);
        }

        /// <summary>
        ///   Unwraps the specified result, retrieving the attached data as well as the <see cref="IAsyncResult"/>.
        /// </summary>
        /// <typeparam name="T">The type of the embedded data.</typeparam>
        /// <param name="result">The result to unwrap.</param>
        /// <param name="unwrappedResult">The unwrapped result.</param>
        /// <returns>The embedded data.</returns>
        /// <seealso cref="T:WebApplications.Utilities.Threading.ApmWrap`1"/>
        [Obsolete("Consider using TPL or Async.")]
        [PublicAPI]
        public static T Unwrap<T>([NotNull] this IAsyncResult result, out IAsyncResult unwrappedResult)
        {
            Contract.Requires(result != null);
            unwrappedResult = result;
            return ApmWrap<T>.Unwrap(ref unwrappedResult);
        }

        /// <summary>
        ///   Creates a wrapper on a call back.
        /// </summary>
        /// <typeparam name="T">The type of the data to embed.</typeparam>
        /// <param name="callback">The callback to wrap.</param>
        /// <param name="data">The data to embed.</param>
        /// <param name="syncContext">The synchronization context.</param>
        /// <returns>The wrapped result.</returns>
        /// <seealso cref="T:WebApplications.Utilities.Threading.ApmWrap`1"/>
        [Obsolete("Consider using TPL or Async.")]
        [NotNull]
        [PublicAPI]
        public static AsyncCallback WrapCallback<T>(
            [NotNull] this AsyncCallback callback,
            [CanBeNull] T data,
            [CanBeNull] SynchronizationContext syncContext = null)
        {
            Contract.Requires(callback != null);
            return ApmWrap<T>.WrapCallback(callback, data, syncContext);
        }

        /// <summary>
        /// Splits the specified array at the selected indices.
        /// </summary>
        /// <typeparam name="T">The array object type.</typeparam>
        /// <param name="array">The array.</param>
        /// <param name="indices">The indices.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        [NotNull]
        [PublicAPI]
        public static T[][] Split<T>([NotNull] this T[] array, [NotNull] params int[] indices)
        {
            Contract.Requires(array != null);
            Contract.Requires(indices != null);

            int length = array.Length;

            // Sort indices, removing any out of bounds and adding an end value of length.
            int[] orderedIndices = indices
                .Where(i => i < length && i > -1)
                .OrderBy(i => i)
                .Concat(new[] { length })
                .ToArray();

            // If there is only one index we return the original
            // aray in a single element enumeration.
            if (orderedIndices.Length < 2)
                return new[] { array };

            T[][] arrays = new T[orderedIndices.Length][];
            int start = 0;
            int chunkIndex = 0;
            foreach (int index in orderedIndices)
            {
                // If end and start are equal, add an empty array.
                if (index == start)
                {
                    arrays[chunkIndex++] = Array<T>.Empty;
                    continue;
                }

                int chunkLength = index - start;
                T[] chunk = new T[chunkLength];
                Array.Copy(array, start, chunk, 0, chunkLength);
                arrays[chunkIndex++] = chunk;

                start = index;
            }
            return arrays;
        }

        /// <summary>
        /// Joins the specified elements. Calls <see cref="String.Join(string,IEnumerable{string})"/>
        /// </summary>
        /// <param name="elements">The elements.</param>
        /// <param name="separator">The separator.</param>
        /// <returns>The joined elements.</returns>
        [NotNull]
        [PublicAPI]
        public static string Join(
            [NotNull] [InstantHandle] this IEnumerable<string> elements,
            [NotNull] string separator = "")
        {
            Contract.Requires(elements != null);
            Contract.Requires(separator != null);

            return String.Join(separator, elements);
        }

        /// <summary>
        /// Joins elements that are not null or empty with the separator.
        /// </summary>
        /// <param name="elements">The elements.</param>
        /// <param name="separator">The separator.</param>
        /// <returns>The joined elements.</returns>
        [NotNull]
        [PublicAPI]
        public static string JoinNotNull(
            [NotNull] [InstantHandle] this IEnumerable<string> elements,
            [NotNull] string separator = "")
        {
            Contract.Requires(elements != null);
            Contract.Requires(separator != null);

            StringBuilder builder = new StringBuilder();
            bool any = false;
            foreach (string element in elements)
            {
                if (element == null) continue;
                if (any)
                    builder.Append(separator);
                else
                    any = true;
                builder.Append(element);
            }
            return builder.ToString();
        }

        /// <summary>
        /// Joins elements that are not null or empty with the separator.
        /// </summary>
        /// <param name="elements">The elements.</param>
        /// <param name="separator">The separator.</param>
        /// <returns>The joined elements.</returns>
        [NotNull]
        [PublicAPI]
        public static string JoinNotNullOrEmpty(
            [NotNull] [InstantHandle] this IEnumerable<string> elements,
            [NotNull] string separator = "")
        {
            Contract.Requires(elements != null);
            Contract.Requires(separator != null);

            StringBuilder builder = new StringBuilder();
            bool any = false;
            foreach (string element in elements)
            {
                if (String.IsNullOrEmpty(element)) continue;
                if (any)
                    builder.Append(separator);
                else
                    any = true;
                builder.Append(element);
            }
            return builder.ToString();
        }

        /// <summary>
        /// Joins elements that are not null or white space with the separator.
        /// </summary>
        /// <param name="elements">The elements.</param>
        /// <param name="separator">The separator.</param>
        /// <returns>The joined elements.</returns>
        [NotNull]
        [PublicAPI]
        public static string JoinNotNullOrWhiteSpace(
            [NotNull] [InstantHandle] this IEnumerable<string> elements,
            [NotNull] string separator = "")
        {
            Contract.Requires(elements != null);
            Contract.Requires(separator != null);

            StringBuilder builder = new StringBuilder();
            bool any = false;
            foreach (string element in elements)
            {
                if (String.IsNullOrWhiteSpace(element)) continue;
                if (any)
                    builder.Append(separator);
                else
                    any = true;
                builder.Append(element);
            }
            return builder.ToString();
        }

        /// <summary>
        /// Splits the string into lines.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        [NotNull]
        [PublicAPI]
        public static IEnumerable<string> SplitLines([NotNull] this string input)
        {
            Contract.Requires(input != null);
            return _lineSplitter.Split(input);
        }

        /// <summary>
        /// Lowers the case of the first letter.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        [NotNull]
        [PublicAPI]
        public static string LowerCaseFirstLetter([NotNull] this string input)
        {
            Contract.Requires(input != null);
            return Char.ToLower(input[0]) + input.Substring(1);
        }

        /// <summary>
        /// Increase number by a percentage.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="increaseByPercent">Percentage to increase by.</param>
        /// <returns>The number increased by given percentage.</returns>
        [PublicAPI]
        public static double AddPercentage(this double number, double increaseByPercent)
        {
            return number + (number * increaseByPercent) / 100;
        }

        /// <summary>
        /// Gets all bytes necessary to represent an IPAddress.  See also <see cref="GetFullIPAddress"/>.
        /// </summary>
        /// <param name="ipAddress">The ip address.</param>
        /// <returns>A byte[] that fully encodes an IPAddress.</returns>
        /// <remarks>
        /// <para>This is necessary as GetAddressBytes() doesn't include the IPv6 scope ID.</para>
        /// </remarks>
        [CanBeNull]
        [PublicAPI]
        public static byte[] GetAllBytes([CanBeNull] this IPAddress ipAddress)
        {
            if (ipAddress == null) return null;
            byte[] bytes = ipAddress.GetAddressBytes();
            if (ipAddress.AddressFamily == AddressFamily.InterNetworkV6)
            {
                Contract.Assert(bytes.Length == 16);

                // We need to add Scope for v6 addresses.
                int offset = bytes.Length;
                Array.Resize(ref bytes, offset + 4);

                // Note that scope is actually a uint not a long, but uint isn't CLS compliant so the framework
                // uses a long - we don't need to waste the extra 4 bytes though.
                uint scope = (uint)ipAddress.ScopeId;
                byte[] scopeBytes = BitConverter.GetBytes(scope);
                Array.Copy(scopeBytes, 0, bytes, offset, 4);
            }
            else
                Contract.Assert(bytes.Length == 4);
            return bytes;
        }

        /// <summary>
        /// Gets an IPAddress from a byte[] that also encode the IPv6 scope.  See also <see cref="GetAllBytes"/>.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns>A byte[] that fully encodes an IPAddress.</returns>
        /// <remarks>This is necessary as GetAddressBytes() doesn't include the IPv6 scope ID.</remarks>
        [CanBeNull]
        [PublicAPI]
        public static IPAddress GetFullIPAddress([CanBeNull] this byte[] bytes)
        {
            if (bytes == null) return null;
            int l = bytes.Length;
            if (l > 4)
            {
                Contract.Assert(l == 20);
                // This is an IPv6 address, remove the scope from the end - note we encode it as a uint32 but have to supply
                // it to IPAddress as a long (which is CLS compliant).
                l -= 4;
                long scope = BitConverter.ToUInt32(bytes, l);
                Array.Resize(ref bytes, l);

                return new IPAddress(bytes, scope);
            }
            // We have an IP4 address.
            Contract.Assert(l == 4);
            return new IPAddress(bytes);
        }

        /// <summary>
        /// Determines if all the elements of the enumerable are distinct.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="equalityComparer">The equality comparer.</param>
        /// <returns><see langword="true" /> if all the elements of the enumerable are distinct, <see langword="false" /> otherwise.</returns>
        [PublicAPI]
        public static bool AreDistinct<T>(
            [NotNull] [InstantHandle] this IEnumerable<T> enumerable,
            [CanBeNull] IEqualityComparer<T> equalityComparer = null)
        {
            Contract.Requires(enumerable != null);
            IReadOnlyCollection<T> collection = enumerable.Enumerate();
            return collection.Count == collection.Distinct(equalityComparer ?? EqualityComparer<T>.Default).Count();
        }

        /// <summary>
        /// Adds a key/value pair to the <see cref="Dictionary{TKey,TValue}" /> by using the specified function, if the key does not already exist.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
        /// <param name="dict">The dictionary to add the element to.</param>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        [CanBeNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [PublicAPI]
        public static TValue GetOrAdd<TKey, TValue>(
            [NotNull] this Dictionary<TKey, TValue> dict,
            [NotNull] TKey key,
            [CanBeNull] TValue value)
        {
            Contract.Requires(dict != null);
            Contract.Requires(!ReferenceEquals(key, null));
            TValue val;
            if (!dict.TryGetValue(key, out val))
                dict.Add(key, val = value);
            return val;
        }

        /// <summary>
        /// Adds a key/value pair to the <see cref="Dictionary{TKey,TValue}"/> by using the specified function, if the key does not already exist.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
        /// <param name="dict">The dictionary to add the element to.</param>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="valueFactory">The function used to generate a value for the key.</param>
        /// <returns></returns>
        [CanBeNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [PublicAPI]
        public static TValue GetOrAdd<TKey, TValue>(
            [NotNull] this Dictionary<TKey, TValue> dict,
            [NotNull] TKey key,
            [NotNull] [InstantHandle] Func<TKey, TValue> valueFactory)
        {
            Contract.Requires(dict != null);
            Contract.Requires(!ReferenceEquals(key, null));
            Contract.Requires(valueFactory != null);
            TValue val;
            if (!dict.TryGetValue(key, out val))
                dict.Add(key, val = valueFactory(key));
            return val;
        }

        #region StdDev
        /// <summary>
        /// Calculate the standard deviation of an average
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>The standard deviation.</returns>
        [PublicAPI]
        public static double StdDev([CanBeNull] [InstantHandle] this IEnumerable<double> values)
        {
            if (values == null) return 0;

            IReadOnlyCollection<double> collection = values.Enumerate();

            int count = collection.Count;
            if (count < 2) return 0;

            // Compute the Average
            double avg = collection.Average();

            // Return standard deviation
            return Math.Sqrt(collection.Sum(d => (d - avg) * (d - avg)) / count);
        }

        /// <summary>
        /// Calculate the standard deviation of an average
        /// </summary>
        /// <typeparam name="T">The type of each element.</typeparam>
        /// <param name="values">The values.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>The standard deviation.</returns>
        [PublicAPI]
        public static double StdDev<T>(
            [CanBeNull] [InstantHandle] this IEnumerable<T> values,
            [NotNull] [InstantHandle] Func<T, double> selector)
        {
            Contract.Requires(selector != null);
            if (values == null) return 0;

            double[] array = values.Select(selector).ToArray();

            int count = array.Length;
            if (count < 2) return 0;

            // Compute the Average
            double avg = array.Average();

            // Return standard deviation
            return Math.Sqrt(array.Sum(d => (d - avg) * (d - avg)) / count);
        }
        #endregion

        #region Min/Max
        /// <summary>
        /// Returns the maximal element of the given sequence, based on
        /// the given projection.
        /// </summary>
        /// <remarks>
        /// If more than one element has the maximal projected value, the first
        /// one encountered will be returned. This overload uses the default comparer
        /// for the projected type. This operator uses immediate execution, but
        /// only buffers a single result (the current maximal element).
        /// </remarks>
        /// <typeparam name="TSource">Type of the source sequence</typeparam>
        /// <typeparam name="TKey">Type of the projected element</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="selector">Selector to use to pick the results to compare</param>
        /// <returns>The maximal element, according to the projection.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> is empty</exception>
        [PublicAPI]
        [CanBeNull]
        public static TSource MaxBy<TSource, TKey>(
            [NotNull] [InstantHandle] this IEnumerable<TSource> source,
            [NotNull] [InstantHandle] Func<TSource, TKey> selector)
        {
            Contract.Requires(source != null);
            Contract.Requires(selector != null);

            return source.MaxBy(selector, Comparer<TKey>.Default);
        }

        /// <summary>
        /// Returns the maximal element of the given sequence, based on
        /// the given projection.
        /// </summary>
        /// <remarks>
        /// If more than one element has the maximal projected value, the first
        /// one encountered will be returned. This overload uses the default comparer
        /// for the projected type. This operator uses immediate execution, but
        /// only buffers a single result (the current maximal element).
        /// </remarks>
        /// <typeparam name="TSource">Type of the source sequence</typeparam>
        /// <typeparam name="TKey">Type of the projected element</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="selector">Selector to use to pick the results to compare</param>
        /// <returns>The maximal element, according to the projection.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> is empty</exception>
        [PublicAPI]
        [CanBeNull]
        public static TSource MaxByOrDefault<TSource, TKey>(
            [NotNull] [InstantHandle] this IEnumerable<TSource> source,
            [NotNull] [InstantHandle] Func<TSource, TKey> selector)
        {
            Contract.Requires(source != null);
            Contract.Requires(selector != null);

            return source.MaxByOrDefault(selector, Comparer<TKey>.Default);
        }

        /// <summary>
        /// Returns the maximal element of the given sequence, based on
        /// the given projection and the specified comparer for projected values. 
        /// </summary>
        /// <remarks>
        /// If more than one element has the maximal projected value, the first
        /// one encountered will be returned. This overload uses the default comparer
        /// for the projected type. This operator uses immediate execution, but
        /// only buffers a single result (the current maximal element).
        /// </remarks>
        /// <typeparam name="TSource">Type of the source sequence</typeparam>
        /// <typeparam name="TKey">Type of the projected element</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="selector">Selector to use to pick the results to compare</param>
        /// <param name="comparer">Comparer to use to compare projected values</param>
        /// <returns>The maximal element, according to the projection.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/>, <paramref name="selector"/> 
        /// or <paramref name="comparer"/> is null</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> is empty</exception>
        [PublicAPI]
        [CanBeNull]
        public static TSource MaxBy<TSource, TKey>(
            [NotNull] [InstantHandle] this IEnumerable<TSource> source,
            [NotNull] [InstantHandle] Func<TSource, TKey> selector,
            [NotNull] IComparer<TKey> comparer)
        {
            Contract.Requires(source != null);
            Contract.Requires(selector != null);
            Contract.Requires(comparer != null);

            using (IEnumerator<TSource> sourceIterator = source.GetEnumerator())
            {
                if (!sourceIterator.MoveNext())
                    throw new InvalidOperationException("Sequence was empty");
                TSource max = sourceIterator.Current;
                TKey maxKey = selector(max);
                while (sourceIterator.MoveNext())
                {
                    TSource candidate = sourceIterator.Current;
                    TKey candidateProjected = selector(candidate);
                    if (comparer.Compare(candidateProjected, maxKey) <= 0) continue;
                    max = candidate;
                    maxKey = candidateProjected;
                }
                return max;
            }
        }

        /// <summary>
        /// Returns the maximal element of the given sequence, based on
        /// the given projection and the specified comparer for projected values. 
        /// </summary>
        /// <remarks>
        /// If more than one element has the maximal projected value, the first
        /// one encountered will be returned. This overload uses the default comparer
        /// for the projected type. This operator uses immediate execution, but
        /// only buffers a single result (the current maximal element).
        /// </remarks>
        /// <typeparam name="TSource">Type of the source sequence</typeparam>
        /// <typeparam name="TKey">Type of the projected element</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="selector">Selector to use to pick the results to compare</param>
        /// <param name="comparer">Comparer to use to compare projected values</param>
        /// <returns>The maximal element, according to the projection.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/>, <paramref name="selector"/> 
        /// or <paramref name="comparer"/> is null</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> is empty</exception>
        [PublicAPI]
        [CanBeNull]
        public static TSource MaxByOrDefault<TSource, TKey>(
            [NotNull] [InstantHandle] this IEnumerable<TSource> source,
            [NotNull] [InstantHandle] Func<TSource, TKey> selector,
            [NotNull] IComparer<TKey> comparer)
        {
            Contract.Requires(source != null);
            Contract.Requires(selector != null);
            Contract.Requires(comparer != null);

            using (IEnumerator<TSource> sourceIterator = source.GetEnumerator())
            {
                if (!sourceIterator.MoveNext())
                    return default(TSource);
                TSource max = sourceIterator.Current;
                TKey maxKey = selector(max);
                while (sourceIterator.MoveNext())
                {
                    TSource candidate = sourceIterator.Current;
                    TKey candidateProjected = selector(candidate);
                    if (comparer.Compare(candidateProjected, maxKey) <= 0) continue;
                    max = candidate;
                    maxKey = candidateProjected;
                }
                return max;
            }
        }

        /// <summary>
        /// Returns the minimal element of the given sequence, based on
        /// the given projection.
        /// </summary>
        /// <remarks>
        /// If more than one element has the minimal projected value, the first
        /// one encountered will be returned. This overload uses the default comparer
        /// for the projected type. This operator uses immediate execution, but
        /// only buffers a single result (the current minimal element).
        /// </remarks>
        /// <typeparam name="TSource">Type of the source sequence</typeparam>
        /// <typeparam name="TKey">Type of the projected element</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="selector">Selector to use to pick the results to compare</param>
        /// <returns>The minimal element, according to the projection.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> is empty</exception>
        [PublicAPI]
        [CanBeNull]
        public static TSource MinBy<TSource, TKey>(
            [NotNull] [InstantHandle] this IEnumerable<TSource> source,
            [NotNull] [InstantHandle] Func<TSource, TKey> selector)
        {
            Contract.Requires(source != null);
            Contract.Requires(selector != null);

            return source.MinBy(selector, Comparer<TKey>.Default);
        }

        /// <summary>
        /// Returns the minimal element of the given sequence, based on
        /// the given projection.
        /// </summary>
        /// <remarks>
        /// If more than one element has the minimal projected value, the first
        /// one encountered will be returned. This overload uses the default comparer
        /// for the projected type. This operator uses immediate execution, but
        /// only buffers a single result (the current minimal element).
        /// </remarks>
        /// <typeparam name="TSource">Type of the source sequence</typeparam>
        /// <typeparam name="TKey">Type of the projected element</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="selector">Selector to use to pick the results to compare</param>
        /// <returns>The minimal element, according to the projection.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> is empty</exception>
        [PublicAPI]
        [CanBeNull]
        public static TSource MinByOrDefault<TSource, TKey>(
            [NotNull] [InstantHandle] this IEnumerable<TSource> source,
            [NotNull] [InstantHandle] Func<TSource, TKey> selector)
        {
            Contract.Requires(source != null);
            Contract.Requires(selector != null);

            return source.MinByOrDefault(selector, Comparer<TKey>.Default);
        }

        /// <summary>
        /// Returns the minimal element of the given sequence, based on
        /// the given projection and the specified comparer for projected values.
        /// </summary>
        /// <remarks>
        /// If more than one element has the minimal projected value, the first
        /// one encountered will be returned. This overload uses the default comparer
        /// for the projected type. This operator uses immediate execution, but
        /// only buffers a single result (the current minimal element).
        /// </remarks>
        /// <typeparam name="TSource">Type of the source sequence</typeparam>
        /// <typeparam name="TKey">Type of the projected element</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="selector">Selector to use to pick the results to compare</param>
        /// <param name="comparer">Comparer to use to compare projected values</param>
        /// <returns>The minimal element, according to the projection.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/>, <paramref name="selector"/> 
        /// or <paramref name="comparer"/> is null</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> is empty</exception>
        [PublicAPI]
        [CanBeNull]
        public static TSource MinBy<TSource, TKey>(
            [NotNull] [InstantHandle] this IEnumerable<TSource> source,
            [NotNull] [InstantHandle] Func<TSource, TKey> selector,
            [NotNull] IComparer<TKey> comparer)
        {
            Contract.Requires(source != null);
            Contract.Requires(selector != null);
            Contract.Requires(comparer != null);

            using (IEnumerator<TSource> sourceIterator = source.GetEnumerator())
            {
                if (!sourceIterator.MoveNext())
                    throw new InvalidOperationException("Sequence was empty");
                TSource min = sourceIterator.Current;
                TKey minKey = selector(min);
                while (sourceIterator.MoveNext())
                {
                    TSource candidate = sourceIterator.Current;
                    TKey candidateProjected = selector(candidate);
                    if (comparer.Compare(candidateProjected, minKey) >= 0) continue;
                    min = candidate;
                    minKey = candidateProjected;
                }
                return min;
            }
        }

        /// <summary>
        /// Returns the minimal element of the given sequence, based on
        /// the given projection and the specified comparer for projected values.
        /// </summary>
        /// <remarks>
        /// If more than one element has the minimal projected value, the first
        /// one encountered will be returned. This overload uses the default comparer
        /// for the projected type. This operator uses immediate execution, but
        /// only buffers a single result (the current minimal element).
        /// </remarks>
        /// <typeparam name="TSource">Type of the source sequence</typeparam>
        /// <typeparam name="TKey">Type of the projected element</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="selector">Selector to use to pick the results to compare</param>
        /// <param name="comparer">Comparer to use to compare projected values</param>
        /// <returns>The minimal element, according to the projection.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/>, <paramref name="selector"/> 
        /// or <paramref name="comparer"/> is null</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> is empty</exception>
        [PublicAPI]
        [CanBeNull]
        public static TSource MinByOrDefault<TSource, TKey>(
            [NotNull] [InstantHandle] this IEnumerable<TSource> source,
            [NotNull] [InstantHandle] Func<TSource, TKey> selector,
            [NotNull] IComparer<TKey> comparer)
        {
            Contract.Requires(source != null);
            Contract.Requires(selector != null);
            Contract.Requires(comparer != null);

            using (IEnumerator<TSource> sourceIterator = source.GetEnumerator())
            {
                if (!sourceIterator.MoveNext())
                    return default(TSource);
                TSource min = sourceIterator.Current;
                TKey minKey = selector(min);
                while (sourceIterator.MoveNext())
                {
                    TSource candidate = sourceIterator.Current;
                    TKey candidateProjected = selector(candidate);
                    if (comparer.Compare(candidateProjected, minKey) >= 0) continue;
                    min = candidate;
                    minKey = candidateProjected;
                }
                return min;
            }
        }

        /// <summary>
        /// Invokes a transform function on each element of a generic sequence and returns the maximum resulting value.
        /// </summary>
        /// <remarks>
        /// This overload uses the default comparer for the projected type. This operator uses immediate execution, but
        /// only buffers a single result (the current maximum value).
        /// </remarks>
        /// <typeparam name="TSource">Type of the source sequence</typeparam>
        /// <typeparam name="TKey">Type of the projected element</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="selector">Selector to use to pick the results to compare</param>
        /// <returns>The maximal element, according to the projection.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> is empty</exception>
        [PublicAPI]
        [CanBeNull]
        public static TKey MaxOrDefault<TSource, TKey>(
            [NotNull] [InstantHandle] this IEnumerable<TSource> source,
            [NotNull] [InstantHandle] Func<TSource, TKey> selector)
        {
            Contract.Requires(source != null);
            Contract.Requires(selector != null);

            return source.MaxOrDefault(selector, Comparer<TKey>.Default);
        }

        /// <summary>
        /// Invokes a transform function on each element of a generic sequence and returns the maximum resulting value,
        /// using the specified comparer for comparing the projected values.
        /// </summary>
        /// <remarks>
        /// This operator uses immediate execution, but
        /// only buffers a single result (the current maximal element).
        /// </remarks>
        /// <typeparam name="TSource">Type of the source sequence</typeparam>
        /// <typeparam name="TKey">Type of the projected element</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="selector">Selector to use to pick the results to compare</param>
        /// <param name="comparer">Comparer to use to compare projected values</param>
        /// <returns>The maximal element, according to the projection.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/>, <paramref name="selector"/> 
        /// or <paramref name="comparer"/> is null</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> is empty</exception>
        [PublicAPI]
        [CanBeNull]
        public static TKey MaxOrDefault<TSource, TKey>(
            [NotNull] [InstantHandle] this IEnumerable<TSource> source,
            [NotNull] [InstantHandle] Func<TSource, TKey> selector,
            [NotNull] IComparer<TKey> comparer)
        {
            Contract.Requires(source != null);
            Contract.Requires(selector != null);
            Contract.Requires(comparer != null);

            using (IEnumerator<TSource> sourceIterator = source.GetEnumerator())
            {
                if (!sourceIterator.MoveNext())
                    return default(TKey);
                TKey max = selector(sourceIterator.Current);
                while (sourceIterator.MoveNext())
                {
                    TKey candidateProjected = selector(sourceIterator.Current);
                    if (comparer.Compare(candidateProjected, max) <= 0) continue;
                    max = candidateProjected;
                }
                return max;
            }
        }

        /// <summary>
        /// Invokes a transform function on each element of a generic sequence and returns the minimum resulting value.
        /// </summary>
        /// <remarks>
        /// This overload uses the default comparer for the projected type. This operator uses immediate execution, but
        /// only buffers a single result (the current maximum value).
        /// </remarks>
        /// <typeparam name="TSource">Type of the source sequence</typeparam>
        /// <typeparam name="TKey">Type of the projected element</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="selector">Selector to use to pick the results to compare</param>
        /// <returns>The minimal element, according to the projection.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> is empty</exception>
        [PublicAPI]
        [CanBeNull]
        public static TKey MinOrDefault<TSource, TKey>(
            [NotNull] [InstantHandle] this IEnumerable<TSource> source,
            [NotNull] [InstantHandle] Func<TSource, TKey> selector)
        {
            Contract.Requires(source != null);
            Contract.Requires(selector != null);

            return source.MinOrDefault(selector, Comparer<TKey>.Default);
        }


        /// <summary>
        /// Invokes a transform function on each element of a generic sequence and returns the minimum resulting value,
        /// using the specified comparer for comparing the projected values.
        /// </summary>
        /// <remarks>
        /// This operator uses immediate execution, but
        /// only buffers a single result (the current maximal element).
        /// </remarks>
        /// <typeparam name="TSource">Type of the source sequence</typeparam>
        /// <typeparam name="TKey">Type of the projected element</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="selector">Selector to use to pick the results to compare</param>
        /// <param name="comparer">Comparer to use to compare projected values</param>
        /// <returns>The minimal element, according to the projection.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/>, <paramref name="selector"/> 
        /// or <paramref name="comparer"/> is null</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> is empty</exception>
        [PublicAPI]
        [CanBeNull]
        public static TKey MinOrDefault<TSource, TKey>(
            [NotNull] [InstantHandle] this IEnumerable<TSource> source,
            [NotNull] [InstantHandle] Func<TSource, TKey> selector,
            [NotNull] IComparer<TKey> comparer)
        {
            Contract.Requires(source != null);
            Contract.Requires(selector != null);
            Contract.Requires(comparer != null);

            using (IEnumerator<TSource> sourceIterator = source.GetEnumerator())
            {
                if (!sourceIterator.MoveNext())
                    return default(TKey);
                TKey max = selector(sourceIterator.Current);
                while (sourceIterator.MoveNext())
                {
                    TKey candidateProjected = selector(sourceIterator.Current);
                    if (comparer.Compare(candidateProjected, max) >= 0) continue;
                    max = candidateProjected;
                }
                return max;
            }
        }

        /// <summary>
        /// Returns the minimum value from the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns>``0.</returns>
        [PublicAPI]
        [CanBeNull]
        public static T Min<T>([NotNull] [InstantHandle] this IEnumerable<T> source, [NotNull] Comparer<T> comparer)
            where T : IComparable<T>
        {
            Contract.Requires(source != null);
            Contract.Requires(source.Any());
            Contract.Requires(comparer != null);

            T value = default(T);
            if (ReferenceEquals(value, null))
            {
                foreach (T x in source)
                    if (x != null &&
                        (value == null || comparer.Compare(x, value) < 0))
                        value = x;
                return value;
            }

            bool hasValue = false;
            foreach (T x in source)
                if (hasValue)
                {
                    if (comparer.Compare(x, value) < 0)
                        value = x;
                }
                else
                {
                    value = x;
                    hasValue = true;
                }
            return value;
        }

        /// <summary>
        /// Returns the maximum value from the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns>``0.</returns>
        [PublicAPI]
        [CanBeNull]
        public static T Max<T>([NotNull] [InstantHandle] this IEnumerable<T> source, [NotNull] Comparer<T> comparer)
            where T : IComparable<T>
        {
            Contract.Requires(source != null);
            Contract.Requires(source.Any());
            Contract.Requires(comparer != null);

            T value = default(T);
            if (ReferenceEquals(value, null))
            {
                foreach (T x in source)
                    if (x != null &&
                        (value == null || comparer.Compare(x, value) > 0))
                        value = x;
                return value;
            }

            bool hasValue = false;
            foreach (T x in source)
                if (hasValue)
                {
                    if (comparer.Compare(x, value) > 0)
                        value = x;
                }
                else
                {
                    value = x;
                    hasValue = true;
                }
            return value;
        }
        #endregion

        #region UnionSingle/Append/Prepend
        /// <summary>
        /// Produces the set union of a sequence and a single item by using a specified <see cref="IEqualityComparer{T}"/>.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains the elements from both input sequences, excluding duplicates.
        /// </returns>
        /// <param name="first">First element to return.</param>
        /// <param name="second">An <see cref="T:System.Collections.Generic.IEnumerable`1"/> whose distinct elements form the second set for the union.</param>
        /// <param name="comparer">The <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> to compare values.</param>
        /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
        [NotNull]
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<TSource> UnionSingle<TSource>(
            [CanBeNull] this TSource first,
            [NotNull] IEnumerable<TSource> second,
            [CanBeNull] IEqualityComparer<TSource> comparer = null)
        {
            Contract.Requires(second != null);
            return UnionSingleIterator(first, second, comparer ?? EqualityComparer<TSource>.Default);
        }

        [NotNull]
        private static IEnumerable<TSource> UnionSingleIterator<TSource>(
            [CanBeNull] TSource first,
            [NotNull] IEnumerable<TSource> second,
            [NotNull] IEqualityComparer<TSource> comparer)
        {
            yield return first;

            foreach (TSource element in second)
                if (!comparer.Equals(element, first))
                    yield return element;
        }

        /// <summary>
        /// Produces the set union of a sequence and a single item by using a specified <see cref="IEqualityComparer{T}" />.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
        /// <param name="first">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose distinct elements form the first set for the union.</param>
        /// <param name="last">The last element to union.</param>
        /// <param name="comparer">The <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> to compare values.</param>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.IEnumerable`1" /> that contains the elements from both input sequences, excluding duplicates.
        /// </returns>
        [NotNull]
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<TSource> UnionSingle<TSource>(
            [NotNull] this IEnumerable<TSource> first,
            [CanBeNull] TSource last,
            [CanBeNull] IEqualityComparer<TSource> comparer = null)
        {
            Contract.Requires(first != null);
            return UnionSingleIterator(first, last, comparer ?? EqualityComparer<TSource>.Default);
        }

        [NotNull]
        private static IEnumerable<TSource> UnionSingleIterator<TSource>(
            [NotNull] this IEnumerable<TSource> first,
            [CanBeNull] TSource last,
            [NotNull] IEqualityComparer<TSource> comparer)
        {
            foreach (TSource element in first)
                if (!comparer.Equals(element, last))
                    yield return element;

            yield return last;
        }

        /// <summary>
        /// Prepends an item to the start of an <see cref="IEnumerable{T}"/> sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
        /// <param name="first">First element to return.</param>
        /// <param name="sequence">The sequence to be prepended to.</param>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains the first item followed by the elements of the sequence.
        /// </returns>
        [NotNull]
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<TSource> Prepend<TSource>(
            [NotNull] this IEnumerable<TSource> sequence,
            [CanBeNull] TSource first)
        {
            Contract.Requires(sequence != null);

            return PrependIterator(sequence, first);
        }

        /// <summary>
        /// Prepends this item to the start of an <see cref="IEnumerable{T}"/> sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
        /// <param name="first">First element to return.</param>
        /// <param name="sequence">The sequence to be prepended to.</param>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains the first item followed by the elements of the sequence.
        /// </returns>
        [NotNull]
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<TSource> PrependTo<TSource>(
            [CanBeNull] this TSource first,
            [NotNull] IEnumerable<TSource> sequence)
        {
            Contract.Requires(sequence != null);

            return PrependIterator(sequence, first);
        }

        [NotNull]
        private static IEnumerable<TSource> PrependIterator<TSource>(
            [NotNull] IEnumerable<TSource> sequence,
            [CanBeNull] TSource first)
        {
            yield return first;

            foreach (TSource element in sequence)
                yield return element;
        }

        /// <summary>
        /// Appends an item to an <see cref="IEnumerable{T}"/> sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
        /// <param name="sequence">The sequence to append the item to.</param>
        /// <param name="last">The item to append.</param>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains the elements of the sequence followed by the last item.
        /// </returns>
        [NotNull]
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<TSource> Append<TSource>(
            [NotNull] this IEnumerable<TSource> sequence,
            [CanBeNull] TSource last)
        {
            Contract.Requires(sequence != null);


            return AppendIterator(sequence, last);
        }

        /// <summary>
        /// Appends this item to an <see cref="IEnumerable{T}"/> sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
        /// <param name="sequence">The sequence to append the item to.</param>
        /// <param name="last">The item to append.</param>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.IEnumerable`1"/> that contains the elements of the sequence followed by the last item.
        /// </returns>
        [NotNull]
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<TSource> AppendTo<TSource>(
            [CanBeNull] this TSource last,
            [NotNull] IEnumerable<TSource> sequence)
        {
            Contract.Requires(sequence != null);

            return AppendIterator(sequence, last);
        }

        [NotNull]
        private static IEnumerable<TSource> AppendIterator<TSource>(
            [NotNull] IEnumerable<TSource> sequence,
            [CanBeNull] TSource last)
        {
            foreach (TSource element in sequence)
                yield return element;

            yield return last;
        }

        /// <summary>
        /// Concatenates two sequences.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="first">The first sequence to concatenate.</param>
        /// <param name="second">The second sequence to concatenate.</param>
        /// <returns></returns>
        [NotNull, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<TSource> Concat<TSource>([NotNull] this IEnumerable<TSource> first, [NotNull] params TSource[] second)
        {
            Contract.Requires(first != null);
            Contract.Requires(second != null);
            return ConcatIterator(first, second);
        }

        [NotNull]
        private static IEnumerable<TSource> ConcatIterator<TSource>([NotNull] IEnumerable<TSource> first, [NotNull] IEnumerable<TSource> second)
        {
            foreach (TSource element in first) yield return element;
            foreach (TSource element in second) yield return element;
        }

        #endregion

        #region HasAtLeast/HasExact
        /// <summary>
        /// Determines whether a sequence contains at least <paramref cref="count" /> elements.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source <see cref="IEnumerable{T}"/>.</param>
        /// <param name="count">The minimum number of elements the <paramref cref="source"/> needs.</param>
        /// <returns><see langword="true"/> if the sequence has at least <paramref name="count"/> items, otherwise <see langword="false"/>.</returns>
        [PublicAPI]
        public static bool HasAtLeast<TSource>([NotNull] [InstantHandle] this IEnumerable<TSource> source, int count)
        {
            Contract.Requires(source != null);
            Contract.Requires(count > 0);

            ICollection<TSource> collection1 = source as ICollection<TSource>;
            if (collection1 != null)
                return collection1.Count >= count;
            ICollection collection2 = source as ICollection;
            if (collection2 != null)
                return collection2.Count >= count;

            foreach (TSource item in source)
            {
                count--;
                if (count < 1)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Determines whether a sequence contains at least <paramref cref="count" /> elements that satisfies a condition.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source <see cref="IEnumerable{T}"/>.</param>
        /// <param name="count">The minimum number of elements the <paramref cref="source"/> needs.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns><see langword="true"/> if the sequence has at least <paramref name="count"/> items that match the <paramref name="predicate"/>, otherwise <see langword="false"/>.</returns>
        [PublicAPI]
        public static bool HasAtLeast<TSource>(
            [NotNull] [InstantHandle] this IEnumerable<TSource> source,
            int count,
            [NotNull] [InstantHandle] Func<TSource, bool> predicate)
        {
            Contract.Requires(source != null);
            Contract.Requires(count > 0);
            Contract.Requires(predicate != null);

            ICollection<TSource> collection1 = source as ICollection<TSource>;
            if (collection1 != null && collection1.Count < count)
                return false;
            ICollection collection2 = source as ICollection;
            if (collection2 != null && collection2.Count < count)
                return false;

            foreach (TSource item in source)
                if (predicate(item))
                {
                    count--;
                    if (count < 1)
                        return true;
                }
            return false;
        }

        /// <summary>
        /// Determines whether a sequence contains exactly <paramref cref="count" /> elements.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source <see cref="IEnumerable{T}"/>.</param>
        /// <param name="count">The exact number of elements the <paramref cref="source"/> needs.</param>
        /// <returns><see langword="true"/> if the sequence has exactly <paramref name="count"/> items, otherwise <see langword="false"/>.</returns>
        [PublicAPI]
        public static bool HasExact<TSource>([NotNull] [InstantHandle] this IEnumerable<TSource> source, int count)
        {
            Contract.Requires(source != null);
            Contract.Requires(count > 0);

            ICollection<TSource> collection1 = source as ICollection<TSource>;
            if (collection1 != null)
                return collection1.Count == count;
            ICollection collection2 = source as ICollection;
            if (collection2 != null)
                return collection2.Count == count;

            foreach (TSource item in source)
            {
                count--;
                if (count < 0)
                    return false;
            }
            return count == 0;
        }

        /// <summary>
        /// Determines whether a sequence contains exactly <paramref cref="count" /> elements that satisfies a condition.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source <see cref="IEnumerable{T}" />.</param>
        /// <param name="count">The exact number of elements the <paramref cref="source" /> needs.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns><see langword="true"/> if the sequence has exactly <paramref name="count"/> items that match the <paramref name="predicate"/>, otherwise <see langword="false"/>.</returns>
        [PublicAPI]
        public static bool HasExact<TSource>(
            [NotNull] [InstantHandle] this IEnumerable<TSource> source,
            int count,
            [NotNull] [InstantHandle] Func<TSource, bool> predicate)
        {
            Contract.Requires(source != null);
            Contract.Requires(count > 0);

            ICollection<TSource> collection1 = source as ICollection<TSource>;
            if (collection1 != null && collection1.Count < count)
                return false;
            ICollection collection2 = source as ICollection;
            if (collection2 != null && collection2.Count < count)
                return false;

            foreach (TSource item in source)
                if (predicate(item))
                {
                    count--;
                    if (count < 0)
                        return false;
                }
            return count == 0;
        }
        #endregion

        #region ToMemorySize
        [NotNull]
        private static readonly string[] _memoryUnitsLong =
        {
            " byte",
            " kilobyte",
            " megabyte",
            " gigabyte",
            " terabyte",
            " petabyte",
            " exabyte"
        };

        [NotNull]
        private static readonly string[] _memoryUnitsShort = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };

        /// <summary>
        /// Converts a number of bytes to a friendly memory size.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="longUnits">if set to <see langword="true" /> use long form unit names instead of symbols.</param>
        /// <param name="decimalPlaces">The number of decimal places between 0 and 16 (ignored for bytes).</param>
        /// <param name="breakPoint">The break point between 0 and 1024 (or 0D to base on decimal points).</param>
        /// <returns>System.String.</returns>
        [NotNull]
        [PublicAPI]
        public static string ToMemorySize(
            this short bytes,
            bool longUnits = false,
            uint decimalPlaces = 1,
            double breakPoint = 512D)
        {
            return ToMemorySize((double)bytes, longUnits, decimalPlaces, breakPoint);
        }

        /// <summary>
        /// Converts a number of bytes to a friendly memory size.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="longUnits">if set to <see langword="true" /> use long form unit names instead of symbols.</param>
        /// <param name="decimalPlaces">The number of decimal places between 0 and 16 (ignored for bytes).</param>
        /// <param name="breakPoint">The break point between 0 and 1024 (or 0D to base on decimal points).</param>
        /// <returns>System.String.</returns>
        [NotNull]
        [PublicAPI]
        public static string ToMemorySize(
            this ushort bytes,
            bool longUnits = false,
            uint decimalPlaces = 1,
            double breakPoint = 512D)
        {
            return ToMemorySize((double)bytes, longUnits, decimalPlaces, breakPoint);
        }

        /// <summary>
        /// Converts a number of bytes to a friendly memory size.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="longUnits">if set to <see langword="true" /> use long form unit names instead of symbols.</param>
        /// <param name="decimalPlaces">The number of decimal places between 0 and 16 (ignored for bytes).</param>
        /// <param name="breakPoint">The break point between 0 and 1024 (or 0D to base on decimal points).</param>
        /// <returns>System.String.</returns>
        [NotNull]
        [PublicAPI]
        public static string ToMemorySize(
            this int bytes,
            bool longUnits = false,
            uint decimalPlaces = 1,
            double breakPoint = 512D)
        {
            return ToMemorySize((double)bytes, longUnits, decimalPlaces, breakPoint);
        }

        /// <summary>
        /// Converts a number of bytes to a friendly memory size.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="longUnits">if set to <see langword="true" /> use long form unit names instead of symbols.</param>
        /// <param name="decimalPlaces">The number of decimal places between 0 and 16 (ignored for bytes).</param>
        /// <param name="breakPoint">The break point between 0 and 1024 (or 0D to base on decimal points).</param>
        /// <returns>System.String.</returns>
        [NotNull]
        [PublicAPI]
        public static string ToMemorySize(
            this uint bytes,
            bool longUnits = false,
            uint decimalPlaces = 1,
            double breakPoint = 512D)
        {
            return ToMemorySize((double)bytes, longUnits, decimalPlaces, breakPoint);
        }

        /// <summary>
        /// Converts a number of bytes to a friendly memory size.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="longUnits">if set to <see langword="true" /> use long form unit names instead of symbols.</param>
        /// <param name="decimalPlaces">The number of decimal places between 0 and 16 (ignored for bytes).</param>
        /// <param name="breakPoint">The break point between 0 and 1024 (or 0D to base on decimal points).</param>
        /// <returns>System.String.</returns>
        [NotNull]
        [PublicAPI]
        public static string ToMemorySize(
            this long bytes,
            bool longUnits = false,
            uint decimalPlaces = 1,
            double breakPoint = 512D)
        {
            return ToMemorySize((double)bytes, longUnits, decimalPlaces, breakPoint);
        }

        /// <summary>
        /// Converts a number of bytes to a friendly memory size.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="longUnits">if set to <see langword="true" /> use long form unit names instead of symbols.</param>
        /// <param name="decimalPlaces">The number of decimal places between 0 and 16 (ignored for bytes).</param>
        /// <param name="breakPoint">The break point between 0 and 1024 (or 0D to base on decimal points).</param>
        /// <returns>System.String.</returns>
        [NotNull]
        [PublicAPI]
        public static string ToMemorySize(
            this ulong bytes,
            bool longUnits = false,
            uint decimalPlaces = 1,
            double breakPoint = 512D)
        {
            return ToMemorySize((double)bytes, longUnits, decimalPlaces, breakPoint);
        }

        /// <summary>
        /// Converts a number of bytes to a friendly memory size.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="longUnits">if set to <see langword="true" /> use long form unit names instead of symbols.</param>
        /// <param name="decimalPlaces">The number of decimal places between 0 and 16 (ignored for bytes).</param>
        /// <param name="breakPoint">The break point between 0 and 1024 (or 0D to base on decimal points).</param>
        /// <returns>System.String.</returns>
        [NotNull]
        [PublicAPI]
        public static string ToMemorySize(
            this double bytes,
            bool longUnits = false,
            uint decimalPlaces = 1,
            double breakPoint = 512D)
        {
            if (decimalPlaces < 1) decimalPlaces = 0;
            else if (decimalPlaces > 16) decimalPlaces = 16;

            // 921.6 is 0.9*1024, this means that be default the breakpoint will round up the last decimal place.
            if (breakPoint < 1) breakPoint = 921.6D * Math.Pow(10, -decimalPlaces);
            else if (breakPoint > 1023) breakPoint = 1023;

            uint maxDecimalPlaces = 0;
            uint unit = 0;
            double amount = bytes;
            while ((Math.Abs(amount) >= breakPoint) &&
                   (unit < 6))
            {
                amount /= 1024;
                unit++;
                maxDecimalPlaces = Math.Min(decimalPlaces, maxDecimalPlaces + 3);
            }
            return String.Format(
                "{0:N" + maxDecimalPlaces + "}{1}",
                amount,
                longUnits
                    ? _memoryUnitsLong[unit]
                    : _memoryUnitsShort[unit]);
        }
        #endregion

        #region Nested type: TypeCounter
        /// <summary>
        ///   <para>A utility class to simplify the DeepEquals code.</para>
        ///   <para>Used for counting the enumerable object's types.</para>
        /// </summary>
        private class TypeCounter
        {
            public TypeCounter()
            {
                Count = 1;
            }

            public int Count { get; private set; }

            public void Increment()
            {
                Count++;
            }

            public void Decrement()
            {
                Count--;
            }
        }
        #endregion

        #region Mod
        /// <summary>
        /// Calculates the modulus of the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="modulus">The modulus.</param>
        /// <returns>The modulus.</returns>
        /// <remarks></remarks>
        [PublicAPI]
        public static short Mod(this short value, short modulus)
        {
            int mod = value % modulus;
            if (mod < 0) mod += modulus;
            return (short)mod;
        }

        /// <summary>
        /// Calculates the modulus of the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="modulus">The modulus.</param>
        /// <returns>The modulus.</returns>
        /// <remarks></remarks>
        [PublicAPI]
        public static ushort Mod(this ushort value, ushort modulus)
        {
            return (ushort)(value % modulus);
        }

        /// <summary>
        /// Calculates the modulus of the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="modulus">The modulus.</param>
        /// <returns>The modulus.</returns>
        /// <remarks></remarks>
        [PublicAPI]
        public static int Mod(this int value, int modulus)
        {
            int mod = value % modulus;
            if (mod < 0) mod += modulus;
            return mod;
        }

        /// <summary>
        /// Calculates the modulus of the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="modulus">The modulus.</param>
        /// <returns>The modulus.</returns>
        /// <remarks></remarks>
        [PublicAPI]
        public static uint Mod(this uint value, uint modulus)
        {
            return value % modulus;
        }

        /// <summary>
        /// Calculates the modulus of the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="modulus">The modulus.</param>
        /// <returns>The modulus.</returns>
        /// <remarks></remarks>
        [PublicAPI]
        public static long Mod(this long value, long modulus)
        {
            long mod = value % modulus;
            if (mod < 0) mod += modulus;
            return mod;
        }

        /// <summary>
        /// Calculates the modulus of the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="modulus">The modulus.</param>
        /// <returns>The modulus.</returns>
        /// <remarks></remarks>
        [PublicAPI]
        public static ulong Mod(this ulong value, ulong modulus)
        {
            return value % modulus;
        }
        #endregion

        #region TopologicalSort
        /// <summary>
        ///   Performs a topological sort on an enumeration.
        /// </summary>
        /// <typeparam name="T">The enumerable type.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="getDependants">
        ///   The function to get dependants for each element of the enumeration.
        /// </param>
        /// <returns>
        ///   The enumeration sorted so that all elements that have dependencies follow their dependencies.
        /// </returns>
        /// <exception cref="InvalidOperationException">The dependencies are cyclical.</exception>
        [NotNull]
        [PublicAPI]
        public static IEnumerable<T> TopologicalSortDependants<T>(
            [NotNull] [InstantHandle] this IEnumerable<T> enumerable,
            [NotNull] [InstantHandle] Func<T, IEnumerable<T>> getDependants)
        {
            Contract.Requires(enumerable != null);
            Contract.Requires(getDependants != null);

            enumerable = enumerable.Enumerate();
            // Create a dictionary of dependencies.
            return TopologicalSortEdges(
                enumerable,
                enumerable.SelectMany(
                    getDependants,
                    (e, d) => new KeyValuePair<T, T>(e, d)));
        }

        /// <summary>
        ///   Performs a topological sort on an enumeration.
        /// </summary>
        /// <typeparam name="T">The enumerable type.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="getDependencies">
        ///   The function to get the dependencies of the individual collection element.
        /// </param>
        /// <returns>
        ///   The enumeration sorted so that all elements that have dependencies follow their dependencies.
        /// </returns>
        /// <exception cref="InvalidOperationException">The dependencies are cyclical.</exception>
        /// <exception cref="ArgumentNullException">
        ///   <para><paramref name="enumerable"/> is <see langword="null"/>.</para>
        ///   <para>-or-</para>
        ///   <para><paramref name="getDependencies"/> is <see langword="null"/>.</para>
        /// </exception>
        [NotNull]
        [UsedImplicitly]
        public static IEnumerable<T> TopologicalSortDependencies<T>(
            [NotNull] [InstantHandle] this IEnumerable<T> enumerable,
            [NotNull] [InstantHandle] Func<T, IEnumerable<T>> getDependencies)
        {
            Contract.Requires(enumerable != null);
            Contract.Requires(getDependencies != null);

            enumerable = enumerable.Enumerate();
            // Create a dictionary of dependencies.
            return TopologicalSortEdges(
                enumerable,
                enumerable.SelectMany(
                    getDependencies,
                    (e, d) => new KeyValuePair<T, T>(d, e)));
        }

        /// <summary>
        ///   Performs a topological sort on an enumeration.
        /// </summary>
        /// <typeparam name="T">The enumerable type.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="edges">The edges.</param>
        /// <returns>
        ///   The enumeration sorted so that all elements that have dependencies follow their dependencies.
        /// </returns>
        /// <exception cref="InvalidOperationException">The dependencies are cyclical.</exception>
        /// <exception cref="ArgumentException">There are duplicates in <paramref name="enumerable"/>.</exception>
        [NotNull]
        [UsedImplicitly]
        public static IEnumerable<T> TopologicalSortEdges<T>(
            [NotNull] [InstantHandle] this IEnumerable<T> enumerable,
            [NotNull] [InstantHandle] IEnumerable<KeyValuePair<T, T>> edges)
        {
            // Create lookup dictionaries from edges
            Dictionary<T, List<T>> dependants = new Dictionary<T, List<T>>();
            Dictionary<T, List<T>> dependencies = new Dictionary<T, List<T>>();

            // Make sure all our elements are added.
            foreach (T t in enumerable)
            {
                // Add dependants and dependencies, will throw exception on duplicates in enumeration.
                dependants.Add(t, new List<T>());
                dependencies.Add(t, new List<T>());
            }

            // Now add edges
            foreach (KeyValuePair<T, T> kvp in edges)
            {
                List<T> l;
                if ((dependencies.TryGetValue(kvp.Value, out l)) &&
                    (!l.Contains(kvp.Key)))
                    l.Add(kvp.Key);

                if ((dependants.TryGetValue(kvp.Key, out l)) &&
                    (!l.Contains(kvp.Value)))
                    l.Add(kvp.Value);
            }

            // Create a queue from all elements that don't have any dependencies.
            Queue<T> outputQueue = new Queue<T>(dependencies.Where(kvp => kvp.Value.Count < 1).Select(kvp => kvp.Key));

            // Process the output queue
            while (outputQueue.Count > 0)
            {
                T t = outputQueue.Dequeue();
                yield return t;

                // Get dependants of the yielded element and remove the reverse reference from their depenencies
                List<T> dependentsOfLastYield;
                if (!dependants.TryGetValue(t, out dependentsOfLastYield)) continue;
                dependants.Remove(t);

                Contract.Assert(dependentsOfLastYield != null);

                foreach (T dependant in dependentsOfLastYield)
                {
                    Contract.Assert(dependant != null);

                    // Check the dependant was actually included in enumerable
                    List<T> deps;
                    if (!dependencies.TryGetValue(dependant, out deps)) continue;
                    Contract.Assert(deps != null);

                    // Remove dependency
                    deps.Remove(t);

                    // if we have no dependencies left add to output queue for processing.
                    if (deps.Count < 1)
                        outputQueue.Enqueue(dependant);
                }
            }

            // If we have any dependants left we have a cycle.
            if (dependants.Count > 0)
                throw new InvalidOperationException(Resources.Extensions_TopologicalSortEdges_DependencyCyclesFound);
        }
        #endregion

        /// <summary>
        /// Enumerates the given <see cref="IEnumerable{T}"/>. 
        /// If the enumerable implements <see cref="IReadOnlyCollection{T}"/> then it will just be returned; otherwise it will be enumerated.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        [NotNull]
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IReadOnlyCollection<T> Enumerate<T>([NotNull] [InstantHandle] this IEnumerable<T> source)
        {
            return source as IReadOnlyCollection<T> ?? source.ToArray();
        }

        /// <summary>
        /// Gets the semantic version of an assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns></returns>
        [NotNull]
        [PublicAPI]
        public static SemanticVersion SemanticVersion([NotNull] this Assembly assembly)
        {
            Contract.Requires(assembly != null);
            Contract.Ensures(Contract.Result<SemanticVersion>() != null);

            // Get the assembly version.
            Version version = assembly.GetName().Version;
            Contract.Assert(version != null);

            // Get the semantic version attribute.
            AssemblySemanticVersionAttribute attribute = assembly.GetCustomAttribute<AssemblySemanticVersionAttribute>();

            SemanticVersion result;
            if ((attribute != null) &&
                Utilities.SemanticVersion.TryParse(
                    attribute.SemanticVersion
                        .Replace("{Major}", version.Major.ToString("D"))
                        .Replace("{Minor}", version.Minor.ToString("D"))
                        .Replace("{Build}", version.Build.ToString("D"))
                        .Replace("{Revision}", version.Revision.ToString("D")),
                    out result))
                return result;

            // Create a semantic version from the assembly version directly.
            return new SemanticVersion(version);
        }


        [NotNull]
        private static readonly ConcurrentDictionary<Type, Func<IEnumerable, ISet>> _hashCollectionCreators =
            new ConcurrentDictionary<Type, Func<IEnumerable, ISet>>();

        /// <summary>
        /// Creates a strongly typed <see cref="HashCollection{T}" /> from the value or values.
        /// </summary>
        /// <param name="elementType">Type of the element.</param>
        /// <param name="values">The values.</param>
        /// <returns>A hash set of the values.</returns>
        [NotNull]
        [PublicAPI]
        public static ISet CreateSet(
            [NotNull] this Type elementType,
            [CanBeNull] [InstantHandle] IEnumerable values = null)
        {
            Contract.Requires(elementType != null);
            return _hashCollectionCreators.GetOrAdd(
                elementType,
                t =>
                {
                    Type hashCollectionType = typeof(HashCollection<>).MakeGenericType(t);
                    Type strongEnumerableType = typeof(IEnumerable<>).MakeGenericType(t);

                    ConstructorInfo constructor = hashCollectionType.GetConstructor(Type.EmptyTypes);
                    MethodInfo addMethod = hashCollectionType.GetMethod(
                        "Add",
                        BindingFlags.Public | BindingFlags.Instance);
                    Contract.Assert(addMethod != null);

                    ParameterExpression valuesParameter = Expression.Parameter(typeof(IEnumerable), "values");
                    ParameterExpression strongEnumerable = Expression.Variable(
                        strongEnumerableType,
                        "strongEnumarble");
                    ParameterExpression result = Expression.Variable(hashCollectionType, "result");

                    return Expression.Lambda<Func<IEnumerable, ISet>>(
                        Expression.Block(
                            new[] { result },
                            Expression.Assign(result, Expression.New(constructor)),
                            Expression.IfThen(
                                Expression.ReferenceNotEqual(
                                    valuesParameter,
                                    Expression.Constant(null, typeof(IEnumerable))),
                                Expression.Block(
                                    new[] { strongEnumerable },
                                    Expression.Assign(
                                        strongEnumerable,
                                        Expression.TypeAs(valuesParameter, strongEnumerableType)),
                                    Expression.IfThenElse(
                                        Expression.ReferenceNotEqual(
                                            strongEnumerable,
                                            Expression.Constant(null, strongEnumerableType)),
                                        strongEnumerable.ForEach(
                                            item =>
                                                Expression.Call(result, addMethod, item)),
                                        Expression.IfThen(
                                            Expression.ReferenceNotEqual(
                                                valuesParameter,
                                                Expression.Constant(null, valuesParameter.Type)),
                                            valuesParameter.ForEach(
                                                item =>
                                                    Expression.Call(result, addMethod, Expression.Convert(item, t)))
                                            )))),
                            Expression.Convert(result, typeof(ISet))),
                        valuesParameter
                        ).Compile();
                })(values);
        }

        /// <summary>
        /// Chooses a random item from the specified enumerable.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <returns>A random item.</returns>
        [PublicAPI]
        [CanBeNull]
        public static T Choose<T>([NotNull] [InstantHandle] this IEnumerable<T> enumerable)
        {
            Contract.Requires(enumerable != null);

            IList<T> list = enumerable as IList<T> ?? enumerable.ToArray();
            return list.Count > 0
                // ReSharper disable once PossibleNullReferenceException
                ? list[ThreadLocal.Random.Next(list.Count)]
                : default(T);
        }

        /// <summary>
        /// Chooses a random item from the specified enumerable, where each item is weighted.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="getWeightFunc">The get weight function, any item with a weight &lt;= 0 will be ignored.</param>
        /// <returns>A random item.</returns>
        [PublicAPI]
        [CanBeNull]
        public static T Choose<T>(
            [NotNull] [InstantHandle] this IEnumerable<T> enumerable,
            [NotNull] [InstantHandle] Func<T, double> getWeightFunc)
        {
            Contract.Requires(enumerable != null);
            Contract.Requires(getWeightFunc != null);
            // Get the weights total weight whilst building a list to prevent multiple enumerations.
            double totalWeight = 0D;
            List<T, double> list = new List<T, double>();
            foreach (T item in enumerable)
            {
                double weight = getWeightFunc(item);
                if (weight <= 0D) continue;

                list.Add(item, weight);
                totalWeight += weight;
            }

            // Validate our connections
            if (list.Count < 1)
                return default(T);

            // Calculate a random value between 0 and the total weight.
            // ReSharper disable once PossibleNullReferenceException
            double next = ThreadLocal.Random.NextDouble() * totalWeight;

            // Pick a connection string
            foreach (Tuple<T, double> item in list)
            {
                Contract.Assert(item != null);
                next -= item.Item2;
                if (next <= 0)
                    return item.Item1;
            }

            // Should never get here, just return last connection as sanity check
            // ReSharper disable PossibleNullReferenceException
            return list.Last().Item1;
            // ReSharper restore PossibleNullReferenceException
        }

        /// <summary>
        /// Flattens the specified <see cref="Exception"/> to an enumeration of <see cref="Exception">Exceptions</see>.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <remarks>Unwraps <see cref="TargetInvocationException"/> and <see cref="AggregateException"/> automatically.</remarks>
        [NotNull]
        [PublicAPI]
        public static IEnumerable<Exception> Flatten([CanBeNull] this Exception exception)
        {
            if (exception == null) yield break;

            Stack<Exception> exceptions = new Stack<Exception>();
            exceptions.Push(exception);

            while (exceptions.Count > 0)
            {
                exception = exceptions.Pop();
                if (ReferenceEquals(exception, null)) continue;

                AggregateException aggregateException = exception as AggregateException;
                if (!ReferenceEquals(aggregateException, null))
                {
                    if (aggregateException.InnerExceptions != null)
                    {
                        // Place the inner exceptions on the stack for further processing.
                        foreach (Exception e in aggregateException.InnerExceptions.Reverse())
                            exceptions.Push(e);
                        continue;
                    }
                    // Fall through
                }
                else
                {
                    TargetInvocationException targetInvocationException = exception as TargetInvocationException;
                    if (!ReferenceEquals(targetInvocationException, null) &&
                        !ReferenceEquals(targetInvocationException.InnerException, null))
                    {
                        exceptions.Push(targetInvocationException.InnerException);
                        continue;
                    }
                }

                // Convert the exception
                yield return exception;
            }
        }

        /// <summary>
        /// Flattens the specified <see cref="Exception"/> to an enumeration of the <typeparamref name="TException">required exception</typeparamref>.
        /// </summary>
        /// <typeparam name="TException">The type of the exception required.</typeparam>
        /// <param name="exception">The exception.</param>
        /// <param name="convert">The function that converts any <see cref="Exception"/> that doesn't already descend from <typeparamref name="TException"/>.</param>
        /// <returns>An enumeration of <typeparamref name="TException"/></returns>
        /// <remarks>Unwraps <see cref="TargetInvocationException"/> and <see cref="AggregateException"/> automatically.</remarks>
        [NotNull]
        [PublicAPI]
        public static IEnumerable<TException> Flatten<TException>([CanBeNull] this Exception exception, [NotNull] Func<Exception, TException> convert)
            where TException : Exception
        {
            Contract.Requires(convert != null);

            if (exception == null) yield break;

            Stack<Exception> exceptions = new Stack<Exception>();
            exceptions.Push(exception);

            while (exceptions.Count > 0)
            {
                exception = exceptions.Pop();
                if (ReferenceEquals(exception, null)) continue;

                TException tException = exception as TException;
                if (!ReferenceEquals(tException, null))
                {
                    yield return tException;
                    continue;
                }

                AggregateException aggregateException = exception as AggregateException;
                if (!ReferenceEquals(aggregateException, null))
                {
                    if (aggregateException.InnerExceptions != null)
                    {
                        // Place the inner exceptions on the stack for further processing.
                        foreach (Exception e in aggregateException.InnerExceptions.Reverse())
                            exceptions.Push(e);
                        continue;
                    }
                    // Fall through
                }
                else
                {
                    TargetInvocationException targetInvocationException = exception as TargetInvocationException;
                    if (!ReferenceEquals(targetInvocationException, null) &&
                        !ReferenceEquals(targetInvocationException.InnerException, null))
                    {
                        exceptions.Push(targetInvocationException.InnerException);
                        continue;
                    }
                }

                // Convert the exception
                tException = convert(exception);
                if (!ReferenceEquals(tException, null))
                    yield return tException;
            }
        }

        /// <summary>
        /// Unwraps the specified exception if possible.
        /// </summary>
        /// <remarks>
        /// The exception can be unwraped if it is an <see cref="AggregateException"/> with a single inner exception, 
        /// or a <see cref="TargetInvocationException"/>.
        /// </remarks>
        /// <param name="exception">The exception.</param>
        [ContractAnnotation("exception:null=>null;exception:notnull=>notnull")]
        public static Exception Unwrap([CanBeNull] this Exception exception)
        {
            if (exception == null)
                return null;

            while (true)
            {
                Contract.Assert(exception != null);

                AggregateException aggregate = exception as AggregateException;
                if (aggregate != null)
                {
                    Contract.Assert(aggregate.InnerExceptions != null);
                    if (aggregate.InnerExceptions.Count == 1)
                    {
                        exception = aggregate.InnerException;
                        continue;
                    }
                    break;
                }

                TargetInvocationException target = exception as TargetInvocationException;
                if (target != null)
                {
                    if (!ReferenceEquals(target.InnerException, null))
                    {
                        exception = target.InnerException;
                        continue;
                    }
                    break;
                }

                break;
            }

            return exception;
        }

        /// <summary>
        /// Re-throws the exception with the original stack trace.
        /// </summary>
        /// <param name="exception">The exception.</param>
        [PublicAPI]
        [ContractAnnotation("=>halt")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReThrow([NotNull] this Exception exception)
        {
            ExceptionDispatchInfo.Capture(exception).Throw();
            // Just in case
            throw exception;
        }

        /// <summary>
        /// Attempts to remove the object at the top of the <see cref="Stack{T}"/>, returning it if successful.
        /// </summary>
        /// <typeparam name="T">The type of the items in the stack.</typeparam>
        /// <param name="stack">The stack to pop.</param>
        /// <param name="value">The value at the top of the stack, or <see langword="default{T}"/> if the stack is empty.</param>
        /// <returns><see langword="true"/> if the stack was not empty; otherwise <see langword="false"/>.</returns>
        [PublicAPI]
        public static bool TryPop<T>([NotNull] this Stack<T> stack, out T value)
        {
            Contract.Requires(stack != null);
            if (stack.Count < 1)
            {
                value = default(T);
                return false;
            }

            value = stack.Pop();
            return true;
        }

        /// <summary>
        /// Attempts to return the object at the top of the <see cref="Stack{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the items in the stack.</typeparam>
        /// <param name="stack">The stack to peek.</param>
        /// <param name="value">The value at the top of the stack, or <see langword="default{T}"/> if the stack is empty.</param>
        /// <returns><see langword="true"/> if the stack was not empty; otherwise <see langword="false"/>.</returns>
        [PublicAPI]
        public static bool TryPeek<T>([NotNull] this Stack<T> stack, out T value)
        {
            Contract.Requires(stack != null);
            if (stack.Count < 1)
            {
                value = default(T);
                return false;
            }

            value = stack.Peek();
            return true;
        }

        /// <summary>
        /// Attempts to remove the object at the beginning of the <see cref="Queue{T}"/>, returning it if successful.
        /// </summary>
        /// <typeparam name="T">The type of the items in the queue.</typeparam>
        /// <param name="queue">The queue to dequeue.</param>
        /// <param name="value">The value at the beginning of the queue, or <see langword="default{T}"/> if the queue is empty.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        [PublicAPI]
        public static bool TryDequeue<T>([NotNull] this Queue<T> queue, out T value)
        {
            Contract.Requires(queue != null);
            if (queue.Count < 1)
            {
                value = default(T);
                return false;
            }

            value = queue.Dequeue();
            return true;
        }

        /// <summary>
        /// Attempts to return the object at the beginning of the <see cref="Queue{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the items in the queue.</typeparam>
        /// <param name="queue">The queue to peek.</param>
        /// <param name="value">The value at the beginning of the queue, or <see langword="default{T}"/> if the queue is empty.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        [PublicAPI]
        public static bool TryPeek<T>([NotNull] this Queue<T> queue, out T value)
        {
            Contract.Requires(queue != null);
            if (queue.Count < 1)
            {
                value = default(T);
                return false;
            }

            value = queue.Peek();
            return true;
        }

        /// <summary>
        /// Performs the given action on each element of the sequence.
        /// </summary>
        /// <typeparam name="T">The type of them elements in the sequence</typeparam>
        /// <param name="sequence">The sequence.</param>
        /// <param name="action">The action to perform on each element.</param>
        /// <returns>The sequence.</returns>
        [NotNull]
        [PublicAPI]
        public static IEnumerable<T> Do<T>([NotNull] this IEnumerable<T> sequence, [NotNull] Action<T> action)
        {
            Contract.Requires(sequence != null);
            Contract.Requires(action != null);

            return DoEnumerator(sequence, action);
        }

        [NotNull]
        private static IEnumerable<T> DoEnumerator<T>([NotNull] IEnumerable<T> sequence, [NotNull] Action<T> action)
        {
            foreach (T item in sequence)
            {
                action(item);
                yield return item;
            }
        }

        /// <summary>
        /// Performs a cross product on two enumerables.
        /// </summary>
        /// <typeparam name="T1">The type of the first enumerable.</typeparam>
        /// <typeparam name="T2">The type of the second enumerable.</typeparam>
        /// <param name="enumerable1">The first enumerable.</param>
        /// <param name="enumerable2">The second enumerable.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Tuple{T1, T2}"/> containing all possible pairs of the elements in the two enumerables.</returns>
        /// <remarks>The order of the elements of the enumerables will be maintained.</remarks>
        [NotNull]
        public static IEnumerable<Tuple<T1, T2>> Cross<T1, T2>(
            [NotNull] this IEnumerable<T1> enumerable1,
            [NotNull] IEnumerable<T2> enumerable2)
        {
            Contract.Requires(enumerable1 != null);
            Contract.Requires(enumerable2 != null);
            return CrossIterator(enumerable1, enumerable2);
        }

        [NotNull]
        private static IEnumerable<Tuple<T1, T2>> CrossIterator<T1, T2>(
            [NotNull] IEnumerable<T1> enumerable1,
            [NotNull] IEnumerable<T2> enumerable2)
        {
            ICollection<T2> collection = enumerable2 as ICollection<T2>;
            if (collection == null)
                collection = new List<T2>();
            else // ReSharper disable once AssignNullToNotNullAttribute
                enumerable2 = null;

            foreach (T1 item1 in enumerable1)
            {
                if (enumerable2 == null)
                {
                    foreach (T2 item2 in collection)
                        yield return new Tuple<T1, T2>(item1, item2);
                    continue;
                }

                // The elements are added to the collection so that the enumerable wont be enumerated multiple times
                foreach (T2 item2 in enumerable2)
                {
                    collection.Add(item2);
                    yield return new Tuple<T1, T2>(item1, item2);
                }

                // ReSharper disable once AssignNullToNotNullAttribute
                enumerable2 = null;
            }
        }

        /// <summary>
        /// Bypasses a specified number of elements in a sequence and then returns a specified number of contiguous elements from the remaining elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The sequence to return elements from.</param>
        /// <param name="skipCount">The number of elements to skip.</param>
        /// <param name="takeCount">The number of elements to return.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that contains the specified number of elements elements that occur after the specified index in the input sequence.</returns>
        [NotNull]
        [PublicAPI]
        public static IEnumerable<T> SkipTake<T>([NotNull] this IEnumerable<T> source, int skipCount, int takeCount)
        {
            Contract.Requires(source != null);
            Contract.Requires(skipCount >= 0);
            Contract.Requires(takeCount > 0);

            return SkipTakeIterator(source, skipCount, takeCount);
        }

        [NotNull]
        private static IEnumerable<T> SkipTakeIterator<T>([NotNull] IEnumerable<T> source, int skipCount, int takeCount)
        {
            if (takeCount <= 0)
                yield break;

            using (IEnumerator<T> e = source.GetEnumerator())
            {
                while (skipCount > 0 &&
                       e.MoveNext())
                    skipCount--;

                if (skipCount > 0)
                    yield break;

                while (e.MoveNext())
                {
                    if (--takeCount < 0) break;
                    yield return e.Current;
                }
            }
        }

        /// <summary>
        /// Bypasses elements in a sequence as long as a specified condition is true and then returns a specified number of contiguous 
        /// elements from the remaining elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source" />.</typeparam>
        /// <param name="source">The sequence to return elements from.</param>
        /// <param name="skipPredicate">A function to test each element for a condition.</param>
        /// <param name="takeCount">The number of elements to return.</param>
        /// <param name="includeFirst">If set to <see langword="true" /> the first element that passes the test will be the first element returned.</param>
        /// <returns>
        /// An <see cref="IEnumerable{T}" /> that contains the specified number of elements starting at the 
        /// first element in the linear series that does not pass the test specified by <paramref name="skipPredicate" />.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public static IEnumerable<T> SkipWhileTake<T>(
            [NotNull] this IEnumerable<T> source,
            [NotNull] Func<T, bool> skipPredicate,
            int takeCount,
            bool includeFirst = true)
        {
            Contract.Requires(source != null);
            Contract.Requires(skipPredicate != null);
            Contract.Requires(takeCount > 0);

            return SkipWhileTakeIterator(source, skipPredicate, takeCount, includeFirst);
        }

        [NotNull]
        private static IEnumerable<T> SkipWhileTakeIterator<T>(
            [NotNull] IEnumerable<T> source,
            [NotNull] Func<T, bool> skipPredicate,
            int takeCount,
            bool includeFirst)
        {
            if (takeCount <= 0)
                yield break;

            using (IEnumerator<T> e = source.GetEnumerator())
            {
                bool yielding = false;
                while (e.MoveNext())
                {
                    if (yielding)
                    {
                        if (--takeCount < 0) break;
                        yield return e.Current;
                    }
                    else if (!skipPredicate(e.Current))
                    {
                        yielding = true;
                        if (includeFirst)
                        {
                            if (--takeCount < 0) break;
                            yield return e.Current;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Bypasses elements in a sequence as long as a specified condition is true and then returns a specified number of the previous elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source" />.</typeparam>
        /// <param name="source">The sequence to return elements from.</param>
        /// <param name="skipPredicate">A function to test each element for a condition.</param>
        /// <param name="takeCount">The number of elements to return.</param>
        /// <param name="includeFirst">If set to <see langword="true" /> the first element that passes the test will be the first element returned.</param>
        /// <returns>
        /// An <see cref="IEnumerable{T}" /> that contains the specified number of elements starting at the 
        /// first element in the linear series that does not pass the test specified by <paramref name="skipPredicate" />.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public static IEnumerable<T> SkipWhileTakeLast<T>(
            [NotNull] this IEnumerable<T> source,
            [NotNull] Func<T, bool> skipPredicate,
            int takeCount,
            bool includeFirst = true)
        {
            Contract.Requires(source != null);
            Contract.Requires(skipPredicate != null);
            Contract.Requires(takeCount > 0);

            return SkipWhileTakeLastIterator(source, skipPredicate, takeCount, includeFirst);
        }

        [NotNull]
        private static IEnumerable<T> SkipWhileTakeLastIterator<T>(
            [NotNull] IEnumerable<T> source,
            [NotNull] Func<T, bool> skipPredicate,
            int takeCount,
            bool includeFirst)
        {
            if (takeCount <= 0)
                yield break;

            using (IEnumerator<T> e = source.GetEnumerator())
            {
                CyclicQueue<T> queue = new CyclicQueue<T>(takeCount);

                while (e.MoveNext())
                {
                    if (skipPredicate(e.Current))
                    {
                        queue.Enqueue(e.Current);
                        continue;
                    }

                    if (includeFirst)
                    {
                        queue.Enqueue(e.Current);
                    }

                    foreach (T item in queue)
                    {
                        yield return item;
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// Gets an <see cref="IEnumerable{T}"/> that yields a single item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        [NotNull]
        [PublicAPI]
        public static IEnumerable<T> Yield<T>(this T value)
        {
            return new SingleEnumerable<T>(value);
        }

        /// <summary>
        /// An <see cref="IEnumerable{T}"/> that yields a single item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private sealed class SingleEnumerable<T> : IList<T>, IList, IReadOnlyList<T>
        {
            private readonly T _value;

            /// <summary>
            /// Initializes a new instance of the <see cref="SingleEnumerable{T}"/> class.
            /// </summary>
            /// <param name="value">The value.</param>
            public SingleEnumerable(T value)
            {
                _value = value;
            }

            /// <summary>
            /// Returns an enumerator that iterates through the collection.
            /// </summary>
            /// <returns>
            /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
            /// </returns>
            public IEnumerator<T> GetEnumerator()
            {
                yield return _value;
            }

            /// <summary>
            /// Returns an enumerator that iterates through a collection.
            /// </summary>
            /// <returns>
            /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
            /// </returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                yield return _value;
            }

            /// <summary>
            /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
            /// </summary>
            /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
            /// <exception cref="System.NotSupportedException"></exception>
            public void Add(T item)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Adds an item to the <see cref="T:System.Collections.IList" />.
            /// </summary>
            /// <param name="value">The object to add to the <see cref="T:System.Collections.IList" />.</param>
            /// <returns>
            /// The position into which the new element was inserted, or -1 to indicate that the item was not inserted into the collection.
            /// </returns>
            /// <exception cref="System.NotSupportedException"></exception>
            public int Add(object value)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Determines whether the <see cref="T:System.Collections.IList" /> contains a specific value.
            /// </summary>
            /// <param name="value">The object to locate in the <see cref="T:System.Collections.IList" />.</param>
            /// <returns>
            /// true if the <see cref="T:System.Object" /> is found in the <see cref="T:System.Collections.IList" />; otherwise, false.
            /// </returns>
            public bool Contains(object value)
            {
                return Equals(value, _value);
            }

            /// <summary>
            /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
            /// </summary>
            /// <exception cref="System.NotSupportedException"></exception>
            public void Clear()
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Determines the index of a specific item in the <see cref="T:System.Collections.IList" />.
            /// </summary>
            /// <param name="value">The object to locate in the <see cref="T:System.Collections.IList" />.</param>
            /// <returns>
            /// The index of <paramref name="value" /> if found in the list; otherwise, -1.
            /// </returns>
            public int IndexOf(object value)
            {
                return Equals(value, _value) ? 0 : -1;
            }

            /// <summary>
            /// Inserts an item to the <see cref="T:System.Collections.IList" /> at the specified index.
            /// </summary>
            /// <param name="index">The zero-based index at which <paramref name="value" /> should be inserted.</param>
            /// <param name="value">The object to insert into the <see cref="T:System.Collections.IList" />.</param>
            /// <exception cref="System.NotSupportedException"></exception>
            public void Insert(int index, object value)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.IList" />.
            /// </summary>
            /// <param name="value">The object to remove from the <see cref="T:System.Collections.IList" />.</param>
            /// <exception cref="System.NotSupportedException"></exception>
            public void Remove(object value)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Removes the <see cref="T:System.Collections.Generic.IList`1" /> item at the specified index.
            /// </summary>
            /// <param name="index">The zero-based index of the item to remove.</param>
            /// <exception cref="System.NotSupportedException"></exception>
            void IList.RemoveAt(int index)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Gets or sets the element at the specified index.
            /// </summary>
            /// <param name="index">The index.</param>
            /// <returns></returns>
            /// <exception cref="System.IndexOutOfRangeException"></exception>
            /// <exception cref="System.NotSupportedException"></exception>
            object IList.this[int index]
            {
                get
                {
                    if (index == 0) return _value;
                    throw new IndexOutOfRangeException();
                }
                set { throw new NotSupportedException(); }
            }

            /// <summary>
            /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.
            /// </summary>
            /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
            /// <returns>
            /// true if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false.
            /// </returns>
            public bool Contains(T item)
            {
                return Equals(item, _value);
            }

            /// <summary>
            /// Copies to.
            /// </summary>
            /// <param name="array">The array.</param>
            /// <param name="arrayIndex">Index of the array.</param>
            public void CopyTo(T[] array, int arrayIndex)
            {
                array[arrayIndex] = _value;
            }

            /// <summary>
            /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.
            /// </summary>
            /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
            /// <returns>
            /// true if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.
            /// </returns>
            /// <exception cref="System.NotSupportedException"></exception>
            public bool Remove(T item)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Copies the elements of the <see cref="T:System.Collections.ICollection" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
            /// </summary>
            /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
            /// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
            public void CopyTo(Array array, int index)
            {
                array.SetValue(_value, index);
            }

            /// <summary>
            /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
            /// </summary>
            /// <returns>The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
            public int Count
            {
                get { return 1; }
            }

            /// <summary>
            /// Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.
            /// </summary>
            /// <returns>An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.</returns>
            public object SyncRoot
            {
                get { return this; }
            }

            /// <summary>
            /// Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe).
            /// </summary>
            /// <returns>true if access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe); otherwise, false.</returns>
            public bool IsSynchronized
            {
                get { return false; }
            }

            /// <summary>
            /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
            /// </summary>
            /// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only; otherwise, false.</returns>
            public bool IsReadOnly
            {
                get { return true; }
            }

            /// <summary>
            /// Gets a value indicating whether the <see cref="T:System.Collections.IList" /> has a fixed size.
            /// </summary>
            /// <returns>true if the <see cref="T:System.Collections.IList" /> has a fixed size; otherwise, false.</returns>
            public bool IsFixedSize
            {
                get { return true; }
            }

            /// <summary>
            /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1" />.
            /// </summary>
            /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1" />.</param>
            /// <returns>
            /// The index of <paramref name="item" /> if found in the list; otherwise, -1.
            /// </returns>
            public int IndexOf(T item)
            {
                return Equals(item, _value) ? 0 : -1;
            }

            /// <summary>
            /// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1" /> at the specified index.
            /// </summary>
            /// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
            /// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1" />.</param>
            /// <exception cref="System.NotSupportedException"></exception>
            public void Insert(int index, T item)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Removes the <see cref="T:System.Collections.Generic.IList`1" /> item at the specified index.
            /// </summary>
            /// <param name="index">The zero-based index of the item to remove.</param>
            /// <exception cref="System.NotSupportedException"></exception>
            void IList<T>.RemoveAt(int index)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Gets or sets the element at the specified index.
            /// </summary>
            /// <param name="index">The index.</param>
            /// <returns></returns>
            /// <exception cref="System.IndexOutOfRangeException"></exception>
            /// <exception cref="System.NotSupportedException"></exception>
            public T this[int index]
            {
                get
                {
                    if (index == 0) return _value;
                    throw new IndexOutOfRangeException();
                }
                set { throw new NotSupportedException(); }
            }
        }

        /// <summary>
        /// Gets the first index of the <paramref name="value"/> in the <paramref name="source"/> enumerable.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the enumerable.</typeparam>
        /// <param name="source">The source enumerable.</param>
        /// <param name="value">The value to find the index of.</param>
        /// <param name="comparer">The comparer to use, or <see langword="null"/> to use the default comparer for the type.</param>
        /// <returns>The index of the value if found; otherwise -1.</returns>
        /// <exception cref="System.ArgumentException">The length of the enumerable exceeded Int32.MaxValue</exception>
        [System.Diagnostics.Contracts.Pure]
        [Annotations.Pure]
        [PublicAPI]
        public static int IndexOf<T>(
            [NotNull][InstantHandle] this IEnumerable<T> source,
            [CanBeNull] T value,
            [CanBeNull] IEqualityComparer<T> comparer = null)
        {
            Contract.Requires(source != null);

            if (comparer == null) comparer = EqualityComparer<T>.Default;

            int index = 0;

            foreach (T item in source)
            {
                if (comparer.Equals(item, value))
                    return index;
                index++;
                if (index < 0)
                    throw new ArgumentException("The length of the enumerable exceeded Int32.MaxValue");
            }

            return -1;
        }

        /// <summary>
        /// Gets the first index of the <paramref name="value" /> in the <paramref name="source" /> enumerable.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the enumerable.</typeparam>
        /// <param name="source">The source enumerable.</param>
        /// <param name="value">The value to find the index of.</param>
        /// <param name="equals">The equality comparison method to use.</param>
        /// <returns>The index of the value if found; otherwise -1.</returns>
        /// <exception cref="System.ArgumentException">The length of the enumerable exceeded Int32.MaxValue</exception>
        [System.Diagnostics.Contracts.Pure]
        [Annotations.Pure]
        [PublicAPI]
        public static int IndexOf<T>(
            [NotNull][InstantHandle] this IEnumerable<T> source,
            [CanBeNull] T value,
            [NotNull][InstantHandle] EqualityComparison<T> @equals)
        {
            Contract.Requires(source != null);
            Contract.Requires(@equals != null);

            int index = 0;

            foreach (T item in source)
            {
                if (@equals(item, value))
                    return index;
                index++;
                if (index < 0)
                    throw new ArgumentException("The length of the enumerable exceeded Int32.MaxValue");
            }

            return -1;
        }

        /// <summary>
        /// Gets the last index of the <paramref name="value"/> in the <paramref name="source"/> enumerable.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the enumerable.</typeparam>
        /// <param name="source">The source enumerable.</param>
        /// <param name="value">The value to find the index of.</param>
        /// <param name="comparer">The comparer to use, or <see langword="null"/> to use the default comparer for the type.</param>
        /// <returns>The last index of the value if found; otherwise -1.</returns>
        /// <exception cref="System.ArgumentException">The length of the enumerable exceeded Int32.MaxValue</exception>
        [System.Diagnostics.Contracts.Pure]
        [Annotations.Pure]
        [PublicAPI]
        public static int LastIndexOf<T>(
            [NotNull][InstantHandle] this IEnumerable<T> source,
            [CanBeNull] T value,
            [CanBeNull] IEqualityComparer<T> comparer = null)
        {
            Contract.Requires(source != null);

            if (comparer == null) comparer = EqualityComparer<T>.Default;

            int result = -1;
            int index = 0;

            foreach (T item in source)
            {
                if (comparer.Equals(item, value))
                    result = index;
                index++;
                if (index < 0)
                    throw new ArgumentException("The length of the enumerable exceeded Int32.MaxValue");
            }

            return result;
        }

        /// <summary>
        /// Gets the last index of the <paramref name="value" /> in the <paramref name="source" /> enumerable.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the enumerable.</typeparam>
        /// <param name="source">The source enumerable.</param>
        /// <param name="value">The value to find the index of.</param>
        /// <param name="equals">The equality comparison method to use.</param>
        /// <returns>The last index of the value if found; otherwise -1.</returns>
        /// <exception cref="System.ArgumentException">The length of the enumerable exceeded Int32.MaxValue</exception>
        [System.Diagnostics.Contracts.Pure]
        [Annotations.Pure]
        [PublicAPI]
        public static int LastIndexOf<T>(
            [NotNull][InstantHandle] this IEnumerable<T> source,
            [CanBeNull] T value,
            [NotNull][InstantHandle] EqualityComparison<T> @equals)
        {
            Contract.Requires(source != null);
            Contract.Requires(@equals != null);

            int result = -1;
            int index = 0;

            foreach (T item in source)
            {
                if (@equals(item, value))
                    result = index;
                index++;
                if (index < 0)
                    throw new ArgumentException("The length of the enumerable exceeded Int32.MaxValue");
            }

            return result;
        }

        /// <summary>
        /// Gets a generic version of the <see cref="IEqualityComparer" />.
        /// </summary>
        /// <typeparam name="T">The type of object to compare.</typeparam>
        /// <param name="comparer">The comparer.</param>
        /// <returns>If the <paramref name="comparer"/> is already generic, then it is just returned; otherwise it is wrapped in a generic comparer.</returns>
        [NotNull]
        [PublicAPI]
        public static IEqualityComparer<T> ToGenericComparer<T>([NotNull] this IEqualityComparer comparer)
        {
            Contract.Requires(comparer != null, "Parameter_Null");
            return comparer as IEqualityComparer<T> ?? new WrappedEqualityComparer<T>(comparer);
        }

        /// <summary>
        /// Gets a generic version of the <see cref="IComparer"/>.
        /// </summary>
        /// <typeparam name="T">The type of object to compare.</typeparam>
        /// <param name="comparer">The comparer.</param>
        /// <returns>If the <paramref name="comparer"/> is already generic, then it is just returned; otherwise it is wrapped in a generic comparer.</returns>
        [NotNull]
        [PublicAPI]
        public static IComparer<T> ToGenericComparer<T>([NotNull] this IComparer comparer)
        {
            Contract.Requires(comparer != null, "Parameter_Null");
            return comparer as IComparer<T> ?? new WrappedComparer<T>(comparer);
        }

        /// <summary>
        /// A generic wrapper around a non-generic <see cref="IEqualityComparer"/>.
        /// </summary>
        /// <typeparam name="T">The type of object to compare.</typeparam>
        private sealed class WrappedEqualityComparer<T> : IEqualityComparer<T>, IEqualityComparer
        {
            [NotNull]
            private readonly IEqualityComparer _comparer;

            /// <summary>
            /// Initializes a new instance of the <see cref="WrappedEqualityComparer{T}"/> class.
            /// </summary>
            /// <param name="comparer">The comparer.</param>
            public WrappedEqualityComparer([NotNull] IEqualityComparer comparer)
            {
                _comparer = comparer;
            }

            /// <summary>
            /// Determines whether the specified objects are equal.
            /// </summary>
            /// <param name="x">The first object of type <typeparamref name="T" /> to compare.</param>
            /// <param name="y">The second object of type <typeparamref name="T" /> to compare.</param>
            /// <returns>
            /// true if the specified objects are equal; otherwise, false.
            /// </returns>
            public bool Equals(T x, T y)
            {
                return _comparer.Equals(x, y);
            }

            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <param name="obj">The object.</param>
            /// <returns>
            /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
            /// </returns>
            public int GetHashCode(T obj)
            {
                return _comparer.GetHashCode(obj);
            }

            /// <summary>
            /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
            /// </summary>
            /// <param name="x">The <see cref="System.Object" /> to compare with this instance.</param>
            /// <param name="y">The y.</param>
            /// <returns>
            ///   <see langword="true" /> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <see langword="false" />.
            /// </returns>
            bool IEqualityComparer.Equals(object x, object y)
            {
                return _comparer.Equals(x, y);
            }

            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <param name="obj">The object.</param>
            /// <returns>
            /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
            /// </returns>
            int IEqualityComparer.GetHashCode(object obj)
            {
                return _comparer.GetHashCode(obj);
            }
        }

        /// <summary>
        /// A generic wrapper around a non-generic <see cref="IComparer"/>.
        /// </summary>
        /// <typeparam name="T">The type of object to compare.</typeparam>
        private sealed class WrappedComparer<T> : IComparer<T>, IComparer
        {
            [NotNull]
            private readonly IComparer _comparer;

            /// <summary>
            /// Initializes a new instance of the <see cref="WrappedComparer{T}"/> class.
            /// </summary>
            /// <param name="comparer">The comparer.</param>
            public WrappedComparer([NotNull] IComparer comparer)
            {
                _comparer = comparer;
            }

            /// <summary>
            /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
            /// </summary>
            /// <param name="x">The first object to compare.</param>
            /// <param name="y">The second object to compare.</param>
            /// <returns>
            /// A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as shown in the following table.Value Meaning Less than zero<paramref name="x" /> is less than <paramref name="y" />.Zero<paramref name="x" /> equals <paramref name="y" />.Greater than zero<paramref name="x" /> is greater than <paramref name="y" />.
            /// </returns>
            public int Compare(T x, T y)
            {
                return _comparer.Compare(x, y);
            }

            /// <summary>
            /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
            /// </summary>
            /// <param name="x">The first object to compare.</param>
            /// <param name="y">The second object to compare.</param>
            /// <returns>
            /// A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as shown in the following table.Value Meaning Less than zero <paramref name="x" /> is less than <paramref name="y" />. Zero <paramref name="x" /> equals <paramref name="y" />. Greater than zero <paramref name="x" /> is greater than <paramref name="y" />.
            /// </returns>
            public int Compare(object x, object y)
            {
                return _comparer.Compare(x, y);
            }
        }
    }
}