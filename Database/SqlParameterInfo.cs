#region © Copyright Web Applications (UK) Ltd, 2017.  All rights reserved.
// Copyright (c) 2017, Web Applications UK Ltd
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
using WebApplications.Utilities.Database.Schema;

namespace WebApplications.Utilities.Database
{
    /// <summary>
    /// Basic information about a parameter to a <see cref="SqlProgram"/>.
    /// </summary>
    public struct SqlParameterInfo
    {
        /// <summary>
        /// The name of the parameter.
        /// </summary>
        [CanBeNull]
        public readonly string Name;

        /// <summary>
        /// The type of the parameter.
        /// </summary>
        [CanBeNull]
        public readonly Type Type;

        /// <summary>
        /// The SQL type of the parameter.
        /// </summary>
        [CanBeNull]
        public readonly SqlTypeInfo SqlType;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlParameterInfo"/> struct.
        /// </summary>
        /// <param name="sqlType">The SQL type.</param>
        public SqlParameterInfo(SqlTypeInfo sqlType)
            : this(null, null, sqlType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlParameterInfo"/> struct.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="sqlType">The SQL type.</param>
        public SqlParameterInfo(string name, SqlTypeInfo sqlType)
            : this(name, null, sqlType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlParameterInfo"/> struct.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="sqlType">The SQL type.</param>
        public SqlParameterInfo(Type type, SqlTypeInfo sqlType = null)
            : this(null, type, sqlType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlParameterInfo"/> struct.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="sqlType">The SQL type.</param>
        public SqlParameterInfo(string name, Type type = null, SqlTypeInfo sqlType = null)
        {
            Name = name;
            Type = type;
            SqlType = sqlType;
        }

        /// <summary>
        /// Creates a new <see cref="SqlParameterInfo"/> with the <paramref name="sqlType"/> given.
        /// </summary>
        /// <param name="sqlType">The SQL type.</param>
        public SqlParameterInfo With(SqlTypeInfo sqlType) 
            => new SqlParameterInfo(Name, Type, sqlType ?? SqlType);

        /// <summary>
        /// Creates a new <see cref="SqlParameterInfo"/> with the <paramref name="name"/> and <paramref name="sqlType"/> given.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="sqlType">The SQL type.</param>
        public SqlParameterInfo With(string name, SqlTypeInfo sqlType) 
            => new SqlParameterInfo(name ?? Name, Type, sqlType ?? SqlType);

        /// <summary>
        /// Creates a new <see cref="SqlParameterInfo"/> with the <paramref name="type"/> and <paramref name="sqlType"/> given.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="sqlType">The SQL type.</param>
        public SqlParameterInfo With(Type type, SqlTypeInfo sqlType = null) 
            => new SqlParameterInfo(Name, type ?? Type, sqlType ?? SqlType);

        /// <summary>
        /// Creates a new <see cref="SqlParameterInfo"/> with the <paramref name="name"/>, <paramref name="type"/> and <paramref name="sqlType"/> given.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="sqlType">The SQL type.</param>
        public SqlParameterInfo With(string name, Type type = null, SqlTypeInfo sqlType = null)
            => new SqlParameterInfo(name ?? Name, type ?? Type, sqlType ?? SqlType);

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="SqlParameterInfo"/>.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator SqlParameterInfo(string name) => new SqlParameterInfo(name);

        /// <summary>
        /// Performs an implicit conversion from <see cref="Type"/> to <see cref="SqlParameterInfo"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator SqlParameterInfo(Type type) => new SqlParameterInfo(type);

        /// <summary>
        /// Performs an implicit conversion from <see cref="SqlTypeInfo"/> to <see cref="SqlParameterInfo"/>.
        /// </summary>
        /// <param name="sqlType">Type of the SQL.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator SqlParameterInfo(SqlTypeInfo sqlType) => new SqlParameterInfo(sqlType);

        /// <summary>
        /// Performs an implicit conversion from <see cref="KeyValuePair{TKey,TValue}"/> to <see cref="SqlParameterInfo"/>.
        /// </summary>
        /// <param name="kvp">The KVP.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        [Obsolete("Use the SqlParameterInfo(string,Type) constructor instead.")]
        public static implicit operator SqlParameterInfo(KeyValuePair<string, Type> kvp)
            => new SqlParameterInfo(kvp.Key, kvp.Value);
    }

    /// <summary>
    /// Information about a SQL type for a <see cref="SqlParameterInfo"/>.
    /// </summary>
    public class SqlTypeInfo
    {
        /// <summary>
        /// Creates a new <see cref="SqlTypeInfo"/>.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        public static SqlTypeInfo Create(SqlTypeSize? size) 
            => !size.HasValue ? null : new SqlTypeInfo(null, size);

        /// <summary>
        /// Creates a new <see cref="SqlTypeInfo"/>.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        public static SqlTypeInfo Create(string name, SqlTypeSize? size = null)
        {
            if (string.IsNullOrWhiteSpace(name)) name = null;
            return name == null && !size.HasValue ? null : new SqlTypeInfo(name, size);
        }

        /// <summary>
        /// The name of the type.
        /// </summary>
        [CanBeNull]
        public readonly string Name;

        /// <summary>
        /// The size of the type.
        /// </summary>
        [CanBeNull]
        public readonly SqlTypeSize? Size;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlTypeInfo"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="size">The size.</param>
        private SqlTypeInfo(string name, SqlTypeSize? size = null)
        {
            Name = name;
            Size = size;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="SqlType"/> to <see cref="SqlTypeInfo"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator SqlTypeInfo(SqlType type) => type == null
            ? null
            : new SqlTypeInfo(type.FullName, type.Size);
    }
}