#region © Copyright Web Applications (UK) Ltd, 2016.  All rights reserved.
// Copyright (c) 2016, Web Applications UK Ltd
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

using System.Data;
using System.IO;
using System.Text;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Database.Exceptions;

namespace WebApplications.Utilities.Database.Caching
{
    /// <summary>
    /// Holds a column definition.
    /// </summary>
    public class Column
    {
        /// <summary>
        /// The ordinal.
        /// </summary>
        public readonly int Ordinal;

        /// <summary>
        /// The name.
        /// </summary>
        [CanBeNull]
        public readonly string Name;

        /// <summary>
        /// The SQL database type.
        /// </summary>
        [NotNull]
        public readonly SqlDbTypeInfo SqlDbTypeInfo;

        /// <summary>
        /// Whether the column can be null.
        /// </summary>
        public readonly bool AllowDBNull;

        /// <summary>
        /// Whether the column fits in a bit.
        /// </summary>
        public readonly bool IsBit;

        /// <summary>
        /// Initializes a new instance of the <see cref="Column"/> class.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <param name="name">The name.</param>
        /// <param name="sqlDbType">Type of the SQL database.</param>
        /// <param name="allowDbNull">if set to <see langword="true" /> [allow database null].</param>
        private Column(int ordinal, string name, SqlDbType sqlDbType, bool allowDbNull)
        {
            Ordinal = ordinal;
            Name = name;
            SqlDbTypeInfo = sqlDbType.GetInfo();
            AllowDBNull = allowDbNull;
            IsBit = sqlDbType == SqlDbType.Bit;
        }
        
        /// <summary>
        /// Reads a <see cref="Column" /> from the <paramref name="row" />.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="ordinalIndex">Index of the ordinal.</param>
        /// <param name="nameIndex">Index of the name.</param>
        /// <param name="providerTypeIndex">Index of the provider type.</param>
        /// <param name="allowDBNullIndex">Index of the allow database null.</param>
        /// <returns>A <see cref="Column" />.</returns>
        [NotNull]
        internal static Column Read(
            DataRow row,
            int ordinalIndex,
            int nameIndex,
            int providerTypeIndex,
            int allowDBNullIndex)
            // ReSharper disable PossibleNullReferenceException
            => new Column(
                (int)row[ordinalIndex],
                row[nameIndex] as string,
                (SqlDbType)row[providerTypeIndex],
                (bool)row[allowDBNullIndex]);
            // ReSharper restore PossibleNullReferenceException

        /// <summary>
        /// Reads a <see cref="Column" /> from the <paramref name="stream">specified stream</paramref>.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>A <see cref="Column" />.</returns>
        [NotNull]
        internal static Column Read([NotNull] Stream stream)
        {
            // Get ordinal
            int ordinal = (int)VariableLengthEncoding.DecodeUInt(stream);
            
            // Get column flags and type
            int flags = stream.ReadByte();
            if (flags < 0) throw new SqlCachingException(() => Resources.Deserialize_EOF);
            bool allowDbNull = 1 == (flags & 1);
            bool cnIsNull = 2 == (flags & 2);
            SqlDbType sqlDbType = (SqlDbType)(flags / 4);
            string name;
            if (!cnIsNull)
            {
                int length = (int)VariableLengthEncoding.DecodeUInt(stream);
                byte[] buffer = new byte[length];
                stream.Read(buffer, 0, length);
                name = Encoding.UTF8.GetString(buffer);
            }
            else name = null;
            return new Column(ordinal, name, sqlDbType, allowDbNull);
        }

        /// <summary>
        /// Serializes this instance to the <paramref name="stream">specified stream</paramref>.
        /// </summary>
        /// <param name="stream">The stream.</param>
        internal void Serialize([NotNull]Stream stream)
        {
            bool cnIsNull = Name == null;

            // Write out ordinal.
            VariableLengthEncoding.Encode((uint)Ordinal, stream);

            // Write type and flags together
            // ReSharper disable once PossibleNullReferenceException
            stream.WriteByte((byte)((AllowDBNull ? 1 : 0) + (cnIsNull ? 2 : 0) + (4 * (byte)SqlDbTypeInfo.SqlDbType)));
            
            if (cnIsNull) return;

            // Write name (if any)
            byte[] buffer = Encoding.UTF8.GetBytes(Name);
            VariableLengthEncoding.Encode((uint)buffer.Length, stream);
            stream.Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString()
            => $"Column #{Ordinal} '{Name}'\t[{SqlDbTypeInfo.SqlDbType} {(AllowDBNull ? "" : "Not ")}Null]";
    }
}