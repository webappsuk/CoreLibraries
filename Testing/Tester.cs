using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace WebApplications.Testing
{
    /// <summary>
    /// Useful extension methods.
    /// </summary>
    public static class Tester
    {
        /// <summary>
        /// A random number generator.
        /// </summary>
        [NotNull]
        public static readonly Random RandomGenerator = new Random();

        public static bool GenerateRandomBoolean(Random random = null)
        {
            return (random ?? RandomGenerator).Next(2) == 1;
        }
        public static byte GenerateRandomByte(Random random = null)
        {
            return (byte)(random ?? RandomGenerator).Next(0x100);
        }
        public static char GenerateRandomChar(Random random = null)
        {
            return (char)(random ?? RandomGenerator).Next(0x10000);
        }
        public static short GenerateRandomInt16(Random random = null)
        {
            byte[] bytes = new byte[2];
            (random ?? RandomGenerator).NextBytes(bytes);
            return BitConverter.ToInt16(bytes, 0);
        }
        public static int GenerateRandomInt32(Random random = null)
        {
            byte[] bytes = new byte[4];
            (random ?? RandomGenerator).NextBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }
        public static long GenerateRandomInt64(Random random = null)
        {
            byte[] bytes = new byte[8];
            (random ?? RandomGenerator).NextBytes(bytes);
            return BitConverter.ToInt64(bytes, 0);
        }
        public static float GenerateRandomFloat(Random random = null)
        {
            byte[] bytes = new byte[4];
            (random ?? RandomGenerator).NextBytes(bytes);
            return BitConverter.ToSingle(bytes, 0);
        }
        public static decimal GenerateDecimal(Random random = null)
        {
            return
                new decimal(new[]
                                {
                                    GenerateRandomInt32(random),
                                    GenerateRandomInt32(random),
                                    GenerateRandomInt32(random),
                                    GenerateRandomInt32(random)
                                });
        }
        public static DateTime GenerateRandomDateTime(Random random = null)
        {
            return new DateTime(GenerateRandomInt64());
        }
        
        /// <summary>
        /// Generates a random string.
        /// </summary>
        /// <param name="maxLength">Maximum length.</param>
        /// <param name="unicode">if set to <see langword="true" /> string is UTF16; otherwise it uses ASCII.</param>
        /// <param name="nullProbability">The probability of a null being returned (0.0 for no nulls).</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string GenerateRandomString(int maxLength = -1, bool unicode = true, double nullProbability = 0.0)
        {
            // Check for random nulls
            if ((nullProbability > 0.0) &&
                (RandomGenerator.NextDouble() < nullProbability))
                return null;

            // Get string length, if there's no maximum then use 8001 (as 8000 is max specific size in SQL Server).
            int length = (maxLength < 0 ? 8001 : maxLength) * (unicode ? 2 : 1);
            byte[] bytes = new byte[length];
            Tester.RandomGenerator.NextBytes(bytes);
            return unicode ? new UnicodeEncoding().GetString(bytes) : new ASCIIEncoding().GetString(bytes);
        }

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
            return count < 1 ? default(T) : filtered[RandomGenerator.Next(count)];
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
            return filtered[RandomGenerator.Next(count)];
        }

        public static bool IsNull(this object value)
        {
            if (value == null || DBNull.Value == value)
                return true;
            INullable nullable = value as INullable;
            return nullable != null && nullable.IsNull;
        }
    }
}