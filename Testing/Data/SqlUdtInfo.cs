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
using System.Diagnostics;
using Microsoft.SqlServer.Server;
using WebApplications.Testing.Annotations;

namespace WebApplications.Testing.Data
{
    /// <summary>
    /// Holds UDT information.
    /// </summary>
    [PublicAPI]
    public class SqlUdtInfo
    {
        [ThreadStatic]
        private static Dictionary<Type, SqlUdtInfo> _types2UdtInfo;

        /// <summary>
        /// The bytes are ordered.
        /// </summary>
        public readonly bool IsByteOrdered;

        /// <summary>
        /// Is fixed length
        /// </summary>
        public readonly bool IsFixedLength;

        /// <summary>
        /// The maximum byte size
        /// </summary>
        public readonly int MaxByteSize;

        /// <summary>
        /// The name
        /// </summary>
        [NotNull]
        public readonly string Name;

        /// <summary>
        /// The serialization format
        /// </summary>
        public readonly Format SerializationFormat;

        /// <summary>
        /// The validation method name
        /// </summary>
        [NotNull]
        public readonly string ValidationMethodName;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlUdtInfo"/> class.
        /// </summary>
        /// <param name="attr">The attribute.</param>
        private SqlUdtInfo([NotNull] SqlUserDefinedTypeAttribute attr)
        {
            if (attr == null) throw new ArgumentNullException("attr");

            SerializationFormat = attr.Format;
            IsByteOrdered = attr.IsByteOrdered;
            IsFixedLength = attr.IsFixedLength;
            MaxByteSize = attr.MaxByteSize;
            // ReSharper disable AssignNullToNotNullAttribute
            Name = attr.Name;
            ValidationMethodName = attr.ValidationMethodName;
            // ReSharper restore AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Gets from type.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>SqlUdtInfo.</returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        [NotNull]
        internal static SqlUdtInfo GetFromType([NotNull] Type target)
        {
            if (target == null) throw new ArgumentNullException("target");

            SqlUdtInfo fromType = TryGetFromType(target);
            if (fromType == null)
                throw new InvalidOperationException();
            return fromType;
        }

        /// <summary>
        /// Tries the type of the get from.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>SqlUdtInfo.</returns>
        [NotNull]
        internal static SqlUdtInfo TryGetFromType([NotNull] Type target)
        {
            if (target == null) throw new ArgumentNullException("target");

            if (_types2UdtInfo == null)
                _types2UdtInfo = new Dictionary<Type, SqlUdtInfo>();
            SqlUdtInfo sqlUdtInfo;
            if (!_types2UdtInfo.TryGetValue(target, out sqlUdtInfo))
            {
                object[] customAttributes = target.GetCustomAttributes(typeof(SqlUserDefinedTypeAttribute), false);
                if (customAttributes.Length == 1)
                    // ReSharper disable once AssignNullToNotNullAttribute
                    sqlUdtInfo = new SqlUdtInfo((SqlUserDefinedTypeAttribute)customAttributes[0]);
                _types2UdtInfo.Add(target, sqlUdtInfo);
            }

            Debug.Assert(sqlUdtInfo != null);
            return sqlUdtInfo;
        }
    }
}