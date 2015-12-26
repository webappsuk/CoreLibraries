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

using Microsoft.SqlServer.Server;
using System;
using System.Collections.Concurrent;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Database.Exceptions;

namespace WebApplications.Utilities.Database.Schema
{
    /// <summary>
    /// Holds UDT information.
    /// </summary>
    [PublicAPI]
    public class SqlUdtInfo
    {
        [NotNull]
        private static ConcurrentDictionary<Type, SqlUdtInfo> _types2UdtInfo = new ConcurrentDictionary<Type, SqlUdtInfo>();

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
        /// <param name="attribute">The attribute.</param>
        private SqlUdtInfo([NotNull] SqlUserDefinedTypeAttribute attribute)
        {
            if (attribute == null) throw new ArgumentNullException(nameof(attribute));

            SerializationFormat = attribute.Format;
            IsByteOrdered = attribute.IsByteOrdered;
            IsFixedLength = attribute.IsFixedLength;
            MaxByteSize = attribute.MaxByteSize;
            // ReSharper disable AssignNullToNotNullAttribute
            Name = attribute.Name;
            ValidationMethodName = attribute.ValidationMethodName;
            // ReSharper restore AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Gets the <see cref="SqlUdtInfo"/> from the <paramref name="target">specified target</paramref>.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>SqlUdtInfo.</returns>
        /// <exception cref="DatabaseSchemaException">The <paramref name="target"/> type does not correspond to a SQL UDT Type.</exception>
        [NotNull]
        public static SqlUdtInfo GetFromType([NotNull] Type target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));

            SqlUdtInfo type;
            if (!TryGetFromType(target, out type))
                throw new DatabaseSchemaException(() => Resources.SqlUdtInfo_GetFromType_Unknown_Type, target);
            return type;
        }

        /// <summary>
        /// Tries to get the <see cref="SqlUdtInfo" /> from the <paramref name="target">specified target</paramref>.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="info">The <see cref="SqlUdtInfo"/>.</param>
        /// <returns><see langword="true"/> if found; otherwise <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
        [ContractAnnotation("=>true,info:notnull;=>false,info:null")]
        public static bool TryGetFromType([NotNull] Type target, out SqlUdtInfo info)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));

            info = _types2UdtInfo.GetOrAdd(
                target,
                t =>
                {
                    object[] customAttributes = target.GetCustomAttributes(typeof(SqlUserDefinedTypeAttribute), false);
                    return customAttributes.Length == 1
                        // ReSharper disable once AssignNullToNotNullAttribute
                        ? new SqlUdtInfo((SqlUserDefinedTypeAttribute)customAttributes[0])
                        : null;
                });
            return info != null;
        }
    }
}