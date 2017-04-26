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
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Serialization;

namespace WebApplications.Utilities.Database
{
    /// <summary>
    ///   Extension methods.
    /// </summary>
    public static class Extensions
    {
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
            if (reader == null) throw new ArgumentNullException("reader");
            if (column == null) throw new ArgumentNullException("column");

            int ordinal = reader.GetOrdinal(column);
            if (reader.IsDBNull(ordinal))
                return nullValue;

            SqlBytes bytes = reader.GetSqlBytes(ordinal);
            using (Stream serializationStream = bytes.Stream)
            {
                Debug.Assert(serializationStream != null);
                return Serialize.GetFormatter(context, contextState).Deserialize(serializationStream);
            }
        }

        /// <summary>
        /// Gets the index of the parameter with the name given, using the string comparer for name equality.
        /// </summary>
        /// <param name="collection">The collection to search.</param>
        /// <param name="parameterName">Name of the parameter to get the index of.</param>
        /// <param name="comparer">The comparer to use.</param>
        /// <returns></returns>
        public static int IndexOf(
            [NotNull] [ItemNotNull] this SqlParameterCollection collection,
            [NotNull] string parameterName,
            [NotNull] IEqualityComparer<string> comparer)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (parameterName == null) throw new ArgumentNullException(nameof(parameterName));
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));

            int index = 0;
            foreach (SqlParameter parameter in collection)
            {
                if (comparer.Equals(parameter.ParameterName, parameterName))
                    return index;
                index++;
            }
            return -1;
        }
    }
}