using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;

namespace WebApplications.Testing
{
    /// <summary>
    /// Useful extension methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Returns a formatted <see cref="string"/> with ' completed in {ms}ms.' appended.
        /// </summary>
        /// <param name="stopwatch">The stopwatch.</param>
        /// <param name="format">The format string.</param>
        /// <param name="parameters">The objects to format in the string.</param>
        /// <returns>
        /// A <see cref="string"/> containing the <paramref name="parameters"/> in the specified <paramref name="format"/>
        /// with ' completed in {ms}ms.' appended. The time duration is taken from the <paramref name="stopwatch"/>.
        /// </returns>
        /// <exception cref="System.FormatException">
        /// <para>The <paramref name="format"/> is invalid</para>
        /// <para>-or-</para>
        /// <para>The index of the format item is less than zero, or greater than or equal to the length or the <paramref name="parameters"/>.</para>
        /// </exception>
        [StringFormatMethod("format")]
        [NotNull]
        public static string ToString([NotNull] this Stopwatch stopwatch, [CanBeNull] string format = null,
                                      [NotNull] params object[] parameters)
        {

            if (String.IsNullOrEmpty(format))
            {
                format = "Stopwatch";
            }
            else if (parameters.Length > 0)
            {
                try
                {
                    format = String.Format(format, parameters);
                }
                catch (FormatException)
                {
                }
            }

            return String.Format("{0} completed in {1}ms.", format, (stopwatch.ElapsedTicks*1000M)/Stopwatch.Frequency);
        }

        /// <summary>
        /// Returns a random element from an enumeration, that matches the predicate; otherwise returns the default value.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="enumeration">The enumeration.</param>
        /// <param name="predicate">The optional predicate.</param>
        /// <returns>A random element or default.</returns>
        [CanBeNull]
        public static T RandomOrDefault<T>([NotNull]this IEnumerable<T> enumeration, Func<T, bool> predicate = null)
        {
            if (enumeration == null)
                throw new ArgumentNullException("enumeration", "The enumeration cannot be null.");

            // We may as well build a list, as we have to count elements anyway.
            List<T> filtered = predicate == null ? enumeration.ToList() : enumeration.Where(predicate).ToList();

            int count = filtered.Count;
            return count < 1 ? default(T) : filtered[TestBase.Random.Next(count)];
        }

        /// <summary>
        /// Returns a random element from an enumeration, that matches the predicate; otherwise throws an exception if the predicate is not matched.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="enumeration">The enumeration.</param>
        /// <param name="predicate">The optional predicate.</param>
        /// <returns>A random element.</returns>
        [CanBeNull]
        public static T Random<T>([NotNull]this IEnumerable<T> enumeration, Func<T, bool> predicate = null)
        {
            if (enumeration == null)
                throw new ArgumentNullException("enumeration", "The enumeration cannot be null.");

            // We may as well build a list, as we have to count elements anyway.
            List<T> filtered = predicate == null ? enumeration.ToList() : enumeration.Where(predicate).ToList();

            int count = filtered.Count;
            if (count < 1)
                throw new InvalidOperationException("The enumeration did not return any results.");
            return filtered[TestBase.Random.Next(count)];
        }
    }
}