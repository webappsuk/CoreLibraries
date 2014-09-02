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
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using JetBrains.Annotations;
using WebApplications.Utilities.Serialization;

namespace WebApplications.Utilities.Database
{
    /// <summary>
    ///   Extension methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        ///   A random number generator per thread, for thread safety.
        /// </summary>
        /// TODO Move to Utilities
        [NotNull]
        private static readonly ThreadLocal<Random> _random = new ThreadLocal<Random>(() => new Random(), false);

        /// <summary>
        /// Chooses a random item from the specified enumerable.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <returns>A random item.</returns>
        /// TODO Move to Utilities
        [PublicAPI]
        [CanBeNull]
        public static T Choose<T>([NotNull] this IEnumerable<T> enumerable)
        {
            Contract.Requires(enumerable != null);
            // Get an array
            T[] array = enumerable.ToArray();
            return array.Length > 0
                // ReSharper disable once PossibleNullReferenceException
                ? array[_random.Value.Next(array.Length)]
                : default(T);
        }

        /// <summary>
        /// Chooses a random item from the specified enumerable, where each item is weighted.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="getWeightFunc">The get weight function, any item with a weight &lt;= 0 will be ignored.</param>
        /// <returns>A random item.</returns>
        /// TODO Move to Utilities
        [PublicAPI]
        [CanBeNull]
        public static T Choose<T>([NotNull] this IEnumerable<T> enumerable, [NotNull] Func<T, double> getWeightFunc)
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
            double next = _random.Value.NextDouble() * totalWeight;

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
        /// Gets a serialized object from a data reader by column name.
        /// </summary>
        /// <param name="reader">The data reader object.</param>
        /// <param name="column">The name of the column to get.</param>
        /// <param name="nullValue">The value to use if the data is null.</param>
        /// <param name="context">The de-serialization context.</param>
        /// <param name="contextState">The de-serialization context state.</param>
        /// <returns>The de-serialized object from the specified <paramref name="column" />.</returns>
        /// <exception cref="IndexOutOfRangeException">The <paramref name="column" /> name provided is invalid.</exception>
        /// <exception cref="System.Security.SecurityException">The caller doesn't have the right permissions for deserialization.</exception>
        /// <exception cref="SerializationException">The serialization stream supports seeking but its length is 0.</exception>
        [CanBeNull]
        public static object GetObjectByName(
            [NotNull] this SqlDataReader reader,
            [NotNull] string column,
            [CanBeNull] object nullValue = null,
            [CanBeNull] object context = null,
            StreamingContextStates contextState = StreamingContextStates.Other)
        {
            Contract.Requires(reader != null);
            Contract.Requires(column != null);
            int ordinal = reader.GetOrdinal(column);
            if (reader.IsDBNull(ordinal))
                return nullValue;

            SqlBytes bytes = reader.GetSqlBytes(ordinal);
            using (Stream serializationStream = bytes.Stream)
                return Serialize.GetFormatter(context, contextState).Deserialize(serializationStream);
        }
    }
}