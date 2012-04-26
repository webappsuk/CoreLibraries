#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities
// File: Extensions.cs
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using JetBrains.Annotations;
using WebApplications.Utilities.Enumerations;
using WebApplications.Utilities.Threading;

namespace WebApplications.Utilities
{
    /// <summary>
    ///   Useful extension methods
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        ///   A dictionary of equality operators by their runtime type.
        ///   This is so that, when requested, they can be retrieved rather than recomputed.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, Func<object, object, bool>> _equalityFunctions =
            new ConcurrentDictionary<Type, Func<object, object, bool>>();

        private static readonly Dictionary<string, string> JSONEscapedCharacters = new Dictionary<string, string> { { "\\", @"\\" }, { "\"", @"\""" }, { "\b", @"\b" }, { "\f", @"\f" }, { "\n", @"\n" }, { "\r", @"\r" }, { "\t", @"\t" } };
        private static readonly Regex MatchJSONEscapedCharacters = new Regex(String.Join("|",JSONEscapedCharacters.Keys.Select(k=>String.Format("\\x{0:X2}",(int)k.ToCharArray()[0])).ToArray()));

        /// <summary>
        ///   The Epoch date time (used by JavaScript).
        /// </summary>
        public static readonly DateTime EpochStart = new DateTime(1970, 1, 1);

        private static readonly string[] _onesMapping = new[]
                                                            {
                                                                "Zero", "One", "Two", "Three", "Four", "Five", "Six",
                                                                "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve",
                                                                "Thirteen", "Fourteen", "Fifteen", "Sixteen",
                                                                "Seventeen", "Eighteen", "Nineteen"
                                                            };

        private static readonly string[] _tensMapping = new[]
                                                            {
                                                                "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy",
                                                                "Eighty", "Ninety"
                                                            };

        private static readonly string[] _groupMapping = new[]
                                                             {
                                                                 "Hundred", "Thousand", "Million", "Billion", "Trillion",
                                                                 "Quadrillion", "Quintillion", "Sextillian", "Septillion",
                                                                 "Octillion", "Nonillion", "Decillion", "Undecillion",
                                                                 "Duodecillion", "Tredecillion", "Quattuordecillion",
                                                                 "Quindecillion", "Sexdecillion", "Septendecillion",
                                                                 "Octodecillion", "Novemdecillion", "Vigintillion",
                                                                 "Unvigintillion", "Duovigintillion", "Tresvigintillion",
                                                                 "Quattuorvigintillion", "Quinquavigintillion", "Sesvigintillion",
                                                                 "Septemvigintillion", "Octovigintillion", "Vigintinonillion",
                                                                 "Trigintillion", "Untrigintillion", "Duotrigintillion",
                                                                 "Trestrigintillion"
                                                             };

        /// <summary>
        ///   The default split characters for splitting strings.
        /// </summary>
        public static readonly char[] DefaultSplitChars = new[] { ' ', ',', '\t', '\r', '\n', '|' };

        private static readonly Regex _htmlRegex = new Regex(@"<[^<>]*>",
                                                             RegexOptions.Compiled | RegexOptions.IgnoreCase |
                                                             RegexOptions.Multiline);

        /// <summary>
        ///   Gets the ordinal representation of an <see cref="int">integer</see> ('1st', '2nd', etc.) as a <see cref="string"/>.
        /// </summary>
        /// <param name="number">The number to add the suffix to.</param>
        /// <returns>The <paramref name="number"/> + the correct suffix.</returns>
        public static string ToOrdinal(this int number)
        {
            string suf = "th";
            if (((number % 100) / 10) != 1)
            {
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
            }
            return number + suf;
        }

        /// <summary>
        ///   Gets the ordinal for an integer ('st', 'nd', etc.)
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>The ordinal (suffix) for the <paramref name="number"/> specified.</returns>
        public static string GetOrdinal(this int number)
        {
            string suf = "th";
            if (((number % 100) / 10) != 1)
            {
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
            }
            return suf;
        }

        // NOTE: 10^303 is approaching the limits of double, as ~1.7e308 is where we are going
        // 10^303 is a centillion and a 10^309 is a duocentillion

        /// <summary>
        ///   Converts a <see cref="int"/> to its English textual equivalent.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        /// <returns>
        ///   The textual equivalent of <paramref name="number"/> specified.
        /// </returns>
        public static string ToEnglish(this int number)
        {
            return ToEnglish((long)number);
        }

        /// <summary>
        ///   Converts a <see cref="long"/> to its English textual equivalent.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        /// <returns>
        ///   The textual equivalent of <paramref name="number"/> specified.
        /// </returns>
        public static string ToEnglish(this long number)
        {
            return ToEnglish((double)number);
        }

        /// <summary>
        ///   Converts a <see cref="double"/> to its English textual equivalent.
        /// </summary>
        /// <param name="number">The number to convert.</param>
        /// <returns>
        ///   The textual equivalent of <paramref name="number"/> specified.
        /// </returns>
        public static string ToEnglish(this double number)
        {
            string sign = null;
            if (number < 0)
            {
                sign = "Negative";
                number = Math.Abs(number);
            }

            int decimalDigits = 0;
            Console.WriteLine(number);
            while (number < 1 ||
                   (number - Math.Floor(number) > 1e-10))
            {
                number *= 10;
                decimalDigits++;
            }
            Console.WriteLine("Total Decimal Digits: {0}", decimalDigits);

            string decimalString = null;
            while (decimalDigits-- > 0)
            {
                int digit = (int)(number % 10);
                number /= 10;
                decimalString = _onesMapping[digit] + " " + decimalString;
            }

            string retVal = null;
            int group = 0;
            if (number < 1)
                retVal = _onesMapping[0];
            else
            {
                while (number >= 1)
                {
                    int numberToProcess = (number >= 1e16) ? 0 : (int)(number % 1000);
                    number = number / 1000;

                    string groupDescription = ProcessGroup(numberToProcess);
                    if (groupDescription != null)
                    {
                        if (@group > 0)
                            retVal = _groupMapping[@group] + " " + retVal;
                        retVal = groupDescription + " " + retVal;
                    }

                    @group++;
                }
            }

            return
                String.Format(
                    "{0}{4}{1}{3}{2}",
                    sign,
                    retVal,
                    decimalString,
                    (decimalString != null) ? " Point " : "",
                    (sign != null) ? " " : "").Trim();
        }

        private static string ProcessGroup(int number)
        {
            int tens = number % 100;
            int hundreds = number / 100;

            string retVal = null;
            if (hundreds > 0)
                retVal = _onesMapping[hundreds] + " " + _groupMapping[0];
            if (tens > 0)
            {
                if (tens < 20)
                    retVal += ((retVal != null) ? " " : "") + _onesMapping[tens];
                else
                {
                    int ones = tens % 10;
                    tens = (tens / 10) - 2; // 20's offset

                    retVal += ((retVal != null) ? " " : "") + _tensMapping[tens];

                    if (ones > 0)
                        retVal += ((retVal != null) ? " " : "") + _onesMapping[ones];
                }
            }

            return retVal;
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
        public static bool EqualsByRuntimeType(this object objA, object objB)
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
        public static Func<object, object, bool> GetTypeEqualityFunction(this Type type)
        {
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
        ///   Compares two enumerables to see if they contain the same elements.
        /// </summary>
        /// <typeparam name="T">The type of the object contained in the enumerable.</typeparam>
        /// <param name="enumerableA">The first enumerable object.</param>
        /// <param name="enumerableB">The second enumerable object</param>
        /// <returns>
        ///   Returns <see langword="true"/> if both of the enumerable objects are the same size and contain the same elements.
        /// </returns>
        /// <remarks>
        ///   <para>This does a robust check of enumerable equality with an algorithm that's no worse that O(N).</para>
        ///   <para>The order in which items appear in the enumerable doesn't matter. For example {1, 2, 2, 3} would be
        ///   considered equal to {2, 1, 3, 2} but not equal to {1, 2, 3, 3}.</para>
        ///   <para>If the list does not contain duplicate items use DeepEqualsSimple.</para>
        ///   <para>If the lists are sorted then use <see cref="System.Linq.Enumerable"/>'s Sequence Equal.</para>
        /// </remarks>
        public static bool DeepEquals<T>(this IEnumerable<T> enumerableA, IEnumerable<T> enumerableB)
        {
            // Check for nulls
            if (enumerableA == null)
                return enumerableB == null;
            int count;
            // Check counts are the same
            if ((enumerableB == null) ||
                ((count = enumerableA.Count()) != enumerableB.Count()))
                return false;

            // Create counters for each element in first enumerable
            Dictionary<T, TypeCounter> counters = new Dictionary<T, TypeCounter>(count);
            TypeCounter t;
            foreach (T element in enumerableA)
            {
                if (counters.TryGetValue(element, out t))
                {
                    t.Increment();
                    continue;
                }
                t = new TypeCounter();
                counters.Add(element, t);
            }

            // Decrement counters in second enumerable
            foreach (T element in enumerableB)
            {
                //  If we have an element that was not present in the first enumerable, enumerables are not equal
                if (!counters.TryGetValue(element, out t))
                    return false;
                t.Decrement();
            }

            // If any of the counters are not zero then we had unequal counts.
            return !counters.Values.Any(counter => counter.Count != 0);
        }

        /// <summary>
        ///   Compares two enumerables to see if they contain the same elements.
        /// </summary>
        /// <remarks>
        ///   <para>This does a robust check of enumerable equality with an algorithm that's no worse that O(N).</para>
        ///    <para>The order in which items appear in the enumerable doesn't matter. For example {1, 2, 2, 3} would
        ///   be considered equal to {2, 1, 3, 2} but not equal to {1, 2, 3, 3}.</para>
        ///   <para>If the list does not contain duplicate items use DeepEqualsSimple.</para>
        ///   <para>If the lists are sorted then use <see cref="System.Linq.Enumerable">System.Linq.Enumerable</see>'s
        ///   Sequence Equal.</para>
        /// </remarks>
        /// <typeparam name="T">The type of the object contained in the enumerable.</typeparam>
        /// <param name="enumerableA">The first enumerable object.</param>
        /// <param name="enumerableB">The second enumerable object</param>
        /// <param name="comparer">The equality comparer.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if both of the enumerable objects are the same size and contain the same elements.
        /// </returns>
        public static bool DeepEquals<T>(
            this IEnumerable<T> enumerableA, IEnumerable<T> enumerableB, IEqualityComparer<T> comparer)
        {
            // Check for nulls
            if (enumerableA == null)
                return enumerableB == null;

            int count;
            // Check counts are the same
            if ((enumerableB == null) ||
                ((count = enumerableA.Count()) != enumerableB.Count()))
                return false;

            // Create counters for each element in first enumerable
            Dictionary<T, TypeCounter> counters = new Dictionary<T, TypeCounter>(count, comparer);
            TypeCounter t;
            foreach (T element in enumerableA)
            {
                if (counters.TryGetValue(element, out t))
                {
                    t.Increment();
                    continue;
                }
                t = new TypeCounter();
                counters.Add(element, t);
            }

            // Decrement counters in second enumerable
            foreach (T element in enumerableB)
            {
                //  If we have an element that was not present in the first enumerable, enumerables are not equal
                if (!counters.TryGetValue(element, out t))
                    return false;
                t.Decrement();
            }

            // If any of the counters are not zero then we had unequal counts.
            return !counters.Values.Any(counter => counter.Count != 0);
        }

        /// <summary>
        ///   Compares two enumerables to see if they contain the same elements.
        /// </summary>
        /// <typeparam name="T">The type of the object contained in the enumerable.</typeparam>
        /// <param name="enumerableA">The first enumerable object.</param>
        /// <param name="enumerableB">The second enumerable object</param>
        /// <returns>
        ///   Returns <see langword="true"/> if both of the enumerable objects are the same size and contain the same elements.
        /// </returns>
        /// <remarks>
        ///   <para>For speed, the number of times elements appear in each enumerable is not taken into account, therefore a
        ///   list of {1, 1, 2} and {1, 2, 2} would be considered equal. Therefore it is recommended that this is only used for
        ///   enumerables that do not contain duplicates.</para>
        ///   <para>If the lists are sorted then use <see cref="System.Linq.Enumerable"/>'s Sequence Equal.</para>
        /// </remarks>
        public static bool DeepEqualsSimple<T>(this IEnumerable<T> enumerableA, IEnumerable<T> enumerableB)
        {
            return enumerableA == null
                       ? enumerableB == null
                       : enumerableA.Count() == enumerableB.Count() &&
                         !enumerableA.Any(i => !enumerableB.Contains(i));
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
        public static bool DeepEqualsSimple<T>(
            this IEnumerable<T> enumerableA, IEnumerable<T> enumerableB, IEqualityComparer<T> comparer)
        {
            return enumerableA == null
                       ? enumerableB == null
                       : enumerableA.Count() == enumerableB.Count() &&
                         !enumerableA.Any(i => !enumerableB.Contains(i, comparer));
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
        public static Dictionary<string, string> ToDictionary(this NameValueCollection collection)
        {
            return collection.Cast<string>().ToDictionary(key => key, key => collection[key],
                                                          StringComparer.CurrentCultureIgnoreCase);
        }

        /// <summary>
        ///   Converts a list to its JSON representation.
        /// </summary>
        /// <param name="list">The list to convert.</param>
        /// <returns>The <paramref name="list"/> in JSON.</returns>
        /// <remarks>
        ///   Can be used with other enumerable objects such as Queues, Stacks, etc.
        /// </remarks>
        public static string ToJSON(this IEnumerable<string> list)
        {
            return list == null || !list.Any()
                       ? "[]"
                       : String.Format(
                           "[{0}]",
                           String.Join(", ", list.Select(s => "\"" + MatchJSONEscapedCharacters.Replace(s,m=>JSONEscapedCharacters[m.ToString()]) + "\"").ToArray()));
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
        public static IEnumerable<T> GetObjectsById<T>(this string integers, Func<int, T> getObject,
                                                       char[] splitChars = null, bool executeImmediately = false)
        {
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
        public static IEnumerable<T> GetObjectsById16<T>(this string integers, Func<short, T> getObject,
                                                         char[] splitChars = null, bool executeImmediately = false)
        {
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
        ///    a <see cref="string"/> of <see cref="int">integer</see> ids and calls a function to retrieve an enumeration of objects.
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
        public static IEnumerable<T> GetObjectsById64<T>(this string integers, Func<long, T> getObject,
                                                         char[] splitChars = null, bool executeImmediately = false)
        {
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
        public static string XmlEscape(this string raw)
        {
            string stripped = String.IsNullOrEmpty(raw)
                                  ? raw
                                  : new string(
                                        raw.Where(
                                            c => (0x1 <= c && c <= 0xD7FF) || (0xE000 <= c && c <= 0xFFFD)).
                                            Where(
                                                c =>
                                                !(0x1 <= c && c <= 0x8) && !new[] { 0xB, 0xC }.Contains(c) &&
                                                !(0xE <= c && c <= 0x1F) && !(0x7F <= c && c <= 0x84) &&
                                                !(0x86 <= c && c <= 9F)).ToArray());
            return SecurityElement.Escape(stripped);
        }

        /// <summary>
        ///   Escapes invalid XML characters from the <see cref="string"/> representation of the specified <see cref="object"/>.
        /// </summary>
        /// <param name="raw">The object containing the raw value.</param>
        /// <returns>The escaped <see cref="string"/>.</returns>
        public static string XmlEscape(this object raw)
        {
            return raw.ToString().XmlEscape();
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
        public static XDocument GetEmbeddedXml(this Assembly assembly, string filename)
        {
            try
            {
                if (assembly == null)
                    throw new ArgumentNullException("assembly");
                if (String.IsNullOrWhiteSpace(filename))
                    throw new ArgumentNullException("filename");

                using (Stream stream = assembly.GetManifestResourceStream(filename))
                {
                    if (stream == null)
                    {
                        throw new InvalidOperationException(
                            String.Format(
                                Resources.Extensions_EmbeddedXml_CouldntLoadEmbeddedResource,
                                filename,
                                assembly));
                    }
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
        public static XmlSchemaSet GetEmbeddedXmlSchemaSet(
            this Assembly assembly, string filename, string targetNamespace = "")
        {
            try
            {
                if (assembly == null)
                    throw new ArgumentNullException("assembly");
                if (String.IsNullOrWhiteSpace(filename))
                    throw new ArgumentNullException("filename");

                using (Stream stream = assembly.GetManifestResourceStream(filename))
                {
                    if (stream == null)
                    {
                        throw new InvalidOperationException(
                            String.Format(
                                Resources.Extensions_EmbeddedXml_CouldntLoadEmbeddedResource,
                                filename,
                                assembly));
                    }

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
        public static Int64 GetEpochTime(this DateTime dateTime)
        {
            return (Int64)(dateTime.ToUniversalTime() - EpochStart).TotalMilliseconds;
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
        public static string StripHTML(this string input)
        {
            return _htmlRegex.Replace(input, String.Empty);
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
        public static string Truncate(this string valueToTruncate, int maxLength,
                                      TruncateOptions options = TruncateOptions.None, string ellipsisString = "...",
                                      int ellipsisLength = -1)
        {
            if (String.IsNullOrEmpty(valueToTruncate) || valueToTruncate.Length <= maxLength)
                return valueToTruncate ?? String.Empty;

            if (ellipsisLength < 0)
            {
                ellipsisLength = ellipsisString.Length;
            }

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

            return String.Format("{0}{1}", retValue,
                                 includeEllipsis && retValue.Length < valueToTruncate.Length
                                     ? ellipsisString
                                     : String.Empty);
        }

        /// <summary>
        ///   Converts the specified value from degrees to radians.
        /// </summary>
        /// <param name="d">The value in degrees to convert.</param>
        /// <returns>The value (<paramref name="d"/>) in radians.</returns>
        public static double ToRadians(this double d)
        {
            return (Math.PI / 180) * d;
        }

        /// <summary>
        ///   Converts the specified value from radians to degrees.
        /// </summary>
        /// <param name="r">The value in radians to convert.</param>
        /// <returns>The value (<paramref name="r"/>) in degrees.</returns>
        public static double ToDegrees(this double r)
        {
            return r * (180.0 / Math.PI);
        }

        /// <summary>
        ///   A safe <see cref="string"/> format.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <param name="parameters">The values used in the format string.</param>
        /// <returns>
        ///   Returns the formatted <see cref="string"/> if successful; otherwise returns the <paramref name="format"/> string.
        /// </returns>
        [NotNull]
        public static string SafeFormat([NotNull] this string format, [NotNull] params object[] parameters)
        {
            Contract.Requires(format != null, Resources.Extensions_SafeFormat_FormatCannotBeNull);
            Contract.Requires(parameters != null, Resources.Extensions_SafeFormat_ParametersCannotBeNull);

            if (parameters.Length < 1)
                return format;
            try
            {
                return String.Format(format, parameters);
            }
            catch (FormatException)
            {
                return format;
            }
        }

        /// <summary>
        ///   Gets the creation date from a <see cref="Guid"/> that is a <see cref="CombGuid"/>.
        /// </summary>
        /// <param name="guid">The Guid.</param>
        /// <returns>The <see cref="System.Guid"/>'s creation <see cref="DateTime"/>.</returns>
        [UsedImplicitly]
        public static DateTime GetDateTime(this Guid guid)
        {
            return CombGuid.GetDateTime(guid);
        }

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
        [UsedImplicitly]
        public static IEnumerable<T> TopologicalSortDependants<T>([NotNull] this IEnumerable<T> enumerable,
                                                                  [NotNull] Func<T, IEnumerable<T>> getDependants)
        {
            // Create a dictionary of dependencies.
            return TopologicalSortEdges(enumerable, enumerable.SelectMany(getDependants,
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
        public static IEnumerable<T> TopologicalSortDependencies<T>([NotNull] this IEnumerable<T> enumerable,
                                                                    [NotNull] Func<T, IEnumerable<T>> getDependencies)
        {
            // Create a dictionary of dependencies.
            return TopologicalSortEdges(enumerable, enumerable.SelectMany(getDependencies,
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
        [NotNull]
        [UsedImplicitly]
        public static IEnumerable<T> TopologicalSortEdges<T>([NotNull] this IEnumerable<T> enumerable,
                                                             [NotNull] IEnumerable<KeyValuePair<T, T>> edges)
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
                if ((dependencies.TryGetValue(kvp.Value, out l)) && (!l.Contains(kvp.Key)))
                    l.Add(kvp.Key);

                if ((dependants.TryGetValue(kvp.Key, out l)) && (!l.Contains(kvp.Value)))
                    l.Add(kvp.Value);
            }

            // Create a queue from all elements that don't have any dependencies.
            Queue<T> outputQueue = new Queue<T>(dependencies.Where(kvp => kvp.Value.Count < 1).Select(kvp => kvp.Key));

            // Process the output queue
            while (outputQueue.Count > 0)
            {
                T t = outputQueue.Dequeue();
                yield return t;

                // Get dependants and remove
                List<T> deps;
                if (!dependants.TryGetValue(t, out deps)) continue;
                dependants.Remove(t);

                foreach (T dependant in deps)
                {
                    // Remove dependency
                    dependencies[dependant].Remove(t);

                    // if we have no dependencies left add to output queue for processing.
                    if (dependencies[dependant].Count < 1)
                        outputQueue.Enqueue(dependant);
                }
            }

            // If we have any dependants left we have a cycle.
            if (dependants.Count > 0)
                throw new InvalidOperationException(Resources.Extensions_TopologicalSortEdges_DependencyCyclesFound);
        }

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

        /// <summary>
        ///   Wraps the specified <see cref="IAsyncResult"/> as an
        ///   <see cref="WebApplications.Utilities.Threading.ApmWrap&lt;T&gt;"/> object, allowing associated data to be attached.
        /// </summary>
        /// <typeparam name="T">The type of the data to embed.</typeparam>
        /// <param name="result">The result to wrap.</param>
        /// <param name="data">The data to embed.</param>
        /// <returns>The wrapped result.</returns>
        [NotNull]
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
        public static T Unwrap<T>([NotNull] this IAsyncResult result)
        {
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
        public static T Unwrap<T>([NotNull] this IAsyncResult result, out IAsyncResult unwrappedResult)
        {
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
        [NotNull]
        public static AsyncCallback WrapCallback<T>([NotNull] this AsyncCallback callback, T data, SynchronizationContext syncContext = null)
        {
            return ApmWrap<T>.WrapCallback(callback, data, syncContext);
        }

        /// <summary>
        /// Calculates the modulus of the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="modulus">The modulus.</param>
        /// <returns>The modulus.</returns>
        /// <remarks></remarks>
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
        public static ushort Mod(this ushort value, ushort modulus)
        {
            int mod = value % modulus;
            if (mod < 0) mod += modulus;
            return (ushort)mod;
        }

        /// <summary>
        /// Calculates the modulus of the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="modulus">The modulus.</param>
        /// <returns>The modulus.</returns>
        /// <remarks></remarks>
        public static int Mod(this int value, int modulus)
        {
            int mod = value%modulus;
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
        public static uint Mod(this uint value, uint modulus)
        {
            long mod = value % modulus;
            if (mod < 0) mod += modulus;
            return (uint)mod;
        }

        /// <summary>
        /// Calculates the modulus of the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="modulus">The modulus.</param>
        /// <returns>The modulus.</returns>
        /// <remarks></remarks>
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
        public static ulong Mod(this ulong value, ulong modulus)
        {
            long mod = (long)value % (long)modulus;
            if (mod < 0) mod += (long)modulus;
            return (ulong)mod;
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
        public static T[][] Split<T>([NotNull]this T[] array, [NotNull] params int[] indices)
        {
            int length = array.Length;
            
            // Sort indices, removing any out of bounds and adding an end value of length.
            int[] orderedIndices = indices
                .Where(i => i < length && i > -1)
                .OrderBy(i => i)
                .Concat(new[] {length})
                .ToArray();

            // If there is only one index we return the original
            // aray in a single element enumeration.
            if (orderedIndices.Length < 2)
                return new[] {array};

            T[][] arrays = new T[orderedIndices.Length][];
            int start = 0;
            int chunkIndex = 0;
            foreach (int index in orderedIndices)
            {
                // If end and start are equal, add an empty array.
                if (index == start)
                {
                    arrays[chunkIndex++] = new T[0];
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
    }
}