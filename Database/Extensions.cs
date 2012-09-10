#region © Copyright Web Applications (UK) Ltd, 2012.  All rights reserved.
// Copyright (c) 2012, Web Applications UK Ltd
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
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
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
        ///   A <see cref="bool"/> value that determines whether the specified <see cref="object"/> is <see langword="null"/>
        ///   (includes support for <see cref="System.DBNull.Value">DBNull</see>).
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the specified <paramref name="value"/> is <see langword="null"/>;
        ///   otherwise returns <see langword="false"/>.
        /// </returns>
        [UsedImplicitly]
        public static bool IsNull(this object value)
        {
            if (value == null || DBNull.Value == value)
                return true;
            INullable nullable = value as INullable;
            return nullable != null && nullable.IsNull;
        }

        /// <summary>
        ///   Returns an ordered, deduplicated enumeration of types that are assignable to the type specified
        ///   using the provided type enumeration.
        /// </summary>
        /// <param name="type">The type to check assignability with.</param>
        /// <param name="types">The types to search.</param>
        /// <returns>
        ///   An ordered enumeration containing types that <paramref name="type"/> is assignable to from <paramref name="types"/>.
        /// </returns>
        /// <remarks>
        ///   Matches occur in the following order:
        ///   <list type="number">
        ///     <item><description>An exact match comes first.</description></item>
        ///     <item><description>The type is a direct descendant of a type in the list
        ///     (the closer to the base type the closer the match).</description></item>
        ///     <item><description>The type implements an interface in the list, where child interfaces
        ///     take precedence over parent interfaces, and interfaces with more interfaces come first.</description></item>
        ///     <item><description>The type is a runtime type and is assignable.</description></item>
        ///   </list>
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="types"/> is <see langword="null"/>.
        /// </exception>
        [NotNull]
        [UsedImplicitly]
        public static IEnumerable<Type> IsAssignableTo([NotNull] this Type type, [NotNull] params Type[] types)
        {
            return IsAssignableTo(type, (IEnumerable<Type>) types);
        }

        /// <summary>
        ///   Returns an ordered, deduplicated enumeration of types that are assignable to the type specified
        ///   using the provided type enumeration.
        /// </summary>
        /// <param name="type">The type to check assignability with.</param>
        /// <param name="types">The types to search.</param>
        /// <returns>
        ///   An ordered enumeration containing types that <paramref name="type"/> is assignable to from <paramref name="types"/>.
        /// </returns>
        /// <remarks>
        ///   Matches occur in the following order:
        ///   <list type="number">
        ///     <item><description>An exact match comes first.</description></item>
        ///     <item><description>The type is a direct descendant of a type in the list
        ///     (the closer to the base type the closer the match).</description></item>
        ///     <item><description>The type implements an interface in the list, where child interfaces
        ///     take precedence over parent interfaces, and interfaces with more interfaces come first.</description></item>
        ///     <item><description>The type is a runtime type and is assignable.</description></item>
        ///   </list>
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="types"/> is <see langword="null"/>.
        /// </exception>
        [NotNull]
        [UsedImplicitly]
        public static IEnumerable<Type> IsAssignableTo([NotNull] this Type type, [NotNull] IEnumerable<Type> types)
        {
            List<KeyValuePair<int, Type>> descendents = new List<KeyValuePair<int, Type>>();
            List<KeyValuePair<int, Type>> interfaces = new List<KeyValuePair<int, Type>>();
            List<Type> runtimes = new List<Type>();
            Type[] typeInterfaces = type.GetInterfaces();
            foreach (Type t in types.Distinct())
            {
                if (t == null)
                    continue;

                if (!t.IsInterface)
                {
                    // Can we assign to this type or a base type.
                    Type baseType = t;
                    int depth = 0;
                    do
                    {
                        if (type == baseType)
                            break;

                        baseType = baseType.BaseType;

                        if ((type.IsInterface) ||
                            (baseType == typeof (object)) ||
                            (baseType == null))
                            depth = -1;
                        else
                            depth++;
                    } while (depth > -1);

                    if (depth > -1)
                    {
                        // Found a match
                        descendents.Add(new KeyValuePair<int, Type>(depth, t));
                        continue;
                    }
                }
                else
                {
                    // How many interfaces can we assign this to?
                    int interfaceCount = t.GetInterfaces().Count(typeInterfaces.Contains) +
                                         (typeInterfaces.Contains(t) ? 1 : 0);
                    if (interfaceCount > 0)
                        interfaces.Add(new KeyValuePair<int, Type>(interfaceCount, t));
                    continue;
                }
                if (t.IsAssignableFrom(type))
                    runtimes.Add(t);
            }

            // Return ordered enumeration
            return descendents.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value)
                .Union(interfaces.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value))
                .Union(runtimes);
        }

        /// <summary>
        ///   Gets a serialized object from a data reader by column name.
        /// </summary>
        /// <param name="reader">The data reader object.</param>
        /// <param name="column">The name of the column to get.</param>
        /// <param name="nullValue">The value to use if the data is null.</param>
        /// <param name="context">The de-serialization context.</param>
        /// <param name="contextState">The de-serialization context state.</param>
        /// <returns>
        /// The de-serialized object from the specified <paramref name="column"/>.
        /// </returns>
        /// <exception cref="IndexOutOfRangeException">
        ///   The <paramref name="column"/> name provided is invalid.
        /// </exception>
        /// <exception cref="System.Security.SecurityException">
        ///   The caller doesn't have the right permissions for deserialization.
        /// </exception>
        /// <exception cref="SerializationException">
        ///   The serialization stream supports seeking but its length is 0.
        /// </exception>
        public static object GetObjectByName(
            this SqlDataReader reader,
            string column,
            object nullValue = null,
            object context = null,
            StreamingContextStates contextState = StreamingContextStates.Other)
        {
            int ordinal = reader.GetOrdinal(column);
            if (reader.IsDBNull(ordinal))
                return nullValue;

            SqlBytes bytes = reader.GetSqlBytes(ordinal);
            using (Stream serializationStream = bytes.Stream)
            {
                return Serialize.GetFormatter(context, contextState).Deserialize(serializationStream);
            }
        }
    }
}