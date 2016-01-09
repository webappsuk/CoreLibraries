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

using Microsoft.SqlServer.Server;
using Microsoft.SqlServer.Types;
using System;
using System.Data.SqlTypes;
using System.Globalization;
using System.IO;
using System.Text;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Blit;
using WebApplications.Utilities.Reflect;
using WebApplications.Utilities.Serialization;

namespace WebApplications.Utilities.Database.Caching
{
    /// <summary>
    /// Provides extra info and methods for a <see cref="SqlDbType"/>.
    /// </summary>
    public partial class SqlDbTypeInfo
    {
        #region SqlBoolean - for completeness, not used
        /// <summary>
        /// Serializes a <see cref="SqlBoolean"/>.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="sqlValue">The SQL value.</param>
        public static void SerializeSqlBoolean([NotNull] Stream stream, [NotNull] object sqlValue)
            => stream.WriteByte(((SqlBoolean)sqlValue).Value ? (byte)1 : (byte)0);

        /// <summary>
        /// Deserializes a <see cref="SqlBoolean"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="SqlBoolean"/></returns>
        [NotNull]
        public static object DeserializeSqlBoolean([NotNull] byte[] buffer, ref long offset)
            => new SqlBoolean(buffer[offset++] != 0);

        /// <summary>
        /// Deserializes a <see cref="bool"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="bool"/></returns>
        [NotNull]
        public static object DeserializeBoolean([NotNull] byte[] buffer, ref long offset)
            => buffer[offset++] != 0;
        #endregion

        #region SqlByte
        /// <summary>
        /// Serializes a <see cref="SqlByte"/>.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="sqlValue">The SQL value.</param>
        public static void SerializeSqlByte([NotNull] Stream stream, [NotNull] object sqlValue)
            => stream.WriteByte(((SqlByte)sqlValue).Value);

        /// <summary>
        /// Deserializes a <see cref="SqlByte"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="SqlByte"/></returns>
        [NotNull]
        public static object DeserializeSqlByte([NotNull] byte[] buffer, ref long offset)
            => new SqlByte(buffer[offset++]);

        /// <summary>
        /// Deserializes a <see cref="byte"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="byte"/></returns>
        [NotNull]
        public static object DeserializeByte([NotNull] byte[] buffer, ref long offset)
            => buffer[offset++];
        #endregion

        #region SqlInt16
        /// <summary>
        /// Serializes a <see cref="SqlInt16"/>.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="sqlValue">The SQL value.</param>
        public static void SerializeSqlInt16([NotNull] Stream stream, [NotNull] object sqlValue)
            => stream.Write((Blittable2)((SqlInt16)sqlValue).Value, 0, 2);

        /// <summary>
        /// Deserializes a <see cref="SqlInt16"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="SqlInt16"/></returns>
        [NotNull]
        public static object DeserializeSqlInt16([NotNull] byte[] buffer, ref long offset)
            => new SqlInt16(new Blittable2(buffer, ref offset));

        /// <summary>
        /// Deserializes a <see cref="short"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="short"/></returns>
        [NotNull]
        public static object DeserializeInt16([NotNull] byte[] buffer, ref long offset)
            => (short)new Blittable2(buffer, ref offset);
        #endregion

        #region SqlInt32
        /// <summary>
        /// Serializes a <see cref="SqlInt32"/>.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="sqlValue">The SQL value.</param>
        public static void SerializeSqlInt32([NotNull] Stream stream, [NotNull] object sqlValue)
            => VariableLengthEncoding.Encode(((SqlInt32)sqlValue).Value, stream);

        /// <summary>
        /// Deserializes a <see cref="SqlInt32"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="SqlInt32"/></returns>
        [NotNull]
        public static object DeserializeSqlInt32([NotNull] byte[] buffer, ref long offset)
            => new SqlInt32(VariableLengthEncoding.DecodeInt(buffer, ref offset));

        /// <summary>
        /// Deserializes a <see cref="int"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="int"/></returns>
        [NotNull]
        public static object DeserializeInt32([NotNull] byte[] buffer, ref long offset)
            => VariableLengthEncoding.DecodeInt(buffer, ref offset);
        #endregion

        #region SqlInt64
        /// <summary>
        /// Serializes a <see cref="SqlInt64"/>.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="sqlValue">The SQL value.</param>
        public static void SerializeSqlInt64([NotNull] Stream stream, [NotNull] object sqlValue)
            => VariableLengthEncoding.Encode(((SqlInt64)sqlValue).Value, stream);

        /// <summary>
        /// Deserializes a <see cref="SqlInt64"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="SqlInt64"/></returns>
        [NotNull]
        public static object DeserializeSqlInt64([NotNull] byte[] buffer, ref long offset)
            => new SqlInt64(VariableLengthEncoding.DecodeLong(buffer, ref offset));

        /// <summary>
        /// Deserializes a <see cref="long"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="long"/></returns>
        [NotNull]
        public static object DeserializeInt64([NotNull] byte[] buffer, ref long offset)
            => VariableLengthEncoding.DecodeLong(buffer, ref offset);
        #endregion

        #region SqlSingle
        /// <summary>
        /// Serializes a <see cref="SqlSingle"/>.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="sqlValue">The SQL value.</param>
        public static void SerializeSqlSingle([NotNull] Stream stream, [NotNull] object sqlValue)
            => VariableLengthEncoding.Encode((uint)(Blittable4)((SqlSingle)sqlValue).Value, stream);

        /// <summary>
        /// Deserializes a <see cref="SqlSingle"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="SqlSingle"/></returns>
        [NotNull]
        public static object DeserializeSqlSingle([NotNull] byte[] buffer, ref long offset)
            => new SqlSingle((Blittable4)VariableLengthEncoding.DecodeUInt(buffer, ref offset));

        /// <summary>
        /// Deserializes a <see cref="float"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="float"/></returns>
        [NotNull]
        public static object DeserializeFloat([NotNull] byte[] buffer, ref long offset)
            => (float)(Blittable4)VariableLengthEncoding.DecodeUInt(buffer, ref offset);
        #endregion

        #region SqlDouble
        /// <summary>
        /// Serializes a <see cref="SqlDouble"/>.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="sqlValue">The SQL value.</param>
        public static void SerializeSqlDouble([NotNull] Stream stream, [NotNull] object sqlValue)
            => VariableLengthEncoding.Encode((ulong)(Blittable8)((SqlDouble)sqlValue).Value, stream);

        /// <summary>
        /// Deserializes a <see cref="SqlSingle"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="SqlSingle"/></returns>
        [NotNull]
        public static object DeserializeSqlDouble([NotNull] byte[] buffer, ref long offset)
            => new SqlDouble((Blittable8)VariableLengthEncoding.DecodeULong(buffer, ref offset));

        /// <summary>
        /// Deserializes a <see cref="double"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="double"/></returns>
        [NotNull]
        public static object DeserializeDouble([NotNull] byte[] buffer, ref long offset)
            => (double)(Blittable8)VariableLengthEncoding.DecodeULong(buffer, ref offset);
        #endregion

        #region SqlDecimal
        /// <summary>
        /// Serializes a <see cref="SqlDecimal"/>.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="sqlValue">The SQL value.</param>
        public static void SerializeSqlDecimal([NotNull] Stream stream, [NotNull] object sqlValue)
        {
            Blittable16 b = ((SqlDecimal)sqlValue).Value;
            VariableLengthEncoding.Encode(b.ULong0, stream);
            VariableLengthEncoding.Encode(b.ULong1, stream);
        }

        /// <summary>
        /// Deserializes a <see cref="SqlSingle"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="SqlSingle"/></returns>
        [NotNull]
        public static object DeserializeSqlDecimal([NotNull] byte[] buffer, ref long offset)
            => new SqlDecimal(
                new Blittable16(
                    VariableLengthEncoding.DecodeULong(buffer, ref offset),
                    VariableLengthEncoding.DecodeULong(buffer, ref offset)));

        /// <summary>
        /// Deserializes a <see cref="decimal"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="decimal"/></returns>
        [NotNull]
        public static object DeserializeDecimal([NotNull] byte[] buffer, ref long offset)
            => new Blittable16(
                VariableLengthEncoding.DecodeULong(buffer, ref offset),
                VariableLengthEncoding.DecodeULong(buffer, ref offset));
        #endregion

        #region SqlMoney
        /// <summary>
        /// Serializes a <see cref="SqlMoney"/>.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="sqlValue">The SQL value.</param>
        public static void SerializeSqlMoney([NotNull] Stream stream, [NotNull] object sqlValue)
        {
            Blittable16 b = ((SqlMoney)sqlValue).Value;
            VariableLengthEncoding.Encode(b.ULong0, stream);
            VariableLengthEncoding.Encode(b.ULong1, stream);
        }

        /// <summary>
        /// Deserializes a <see cref="SqlSingle"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="SqlSingle"/></returns>
        [NotNull]
        public static object DeserializeSqlMoney([NotNull] byte[] buffer, ref long offset)
            => new SqlMoney(
                new Blittable16(
                    VariableLengthEncoding.DecodeULong(buffer, ref offset),
                    VariableLengthEncoding.DecodeULong(buffer, ref offset)));
        #endregion

        #region SqlDateTime
        /// <summary>
        /// Serializes a <see cref="SqlDateTime"/>.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="sqlValue">The SQL value.</param>
        public static void SerializeSqlDateTime([NotNull] Stream stream, [NotNull] object sqlValue)
            => VariableLengthEncoding.Encode((ulong)(Blittable8)((SqlDateTime)sqlValue).Value, stream);

        /// <summary>
        /// Serializes a <see cref="SqlDateTime"/>.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="sqlValue">The SQL value.</param>
        public static void SerializeDateTime([NotNull] Stream stream, [NotNull] object sqlValue)
            => VariableLengthEncoding.Encode((ulong)(Blittable8)(DateTime)sqlValue, stream);

        /// <summary>
        /// Deserializes a <see cref="SqlSingle"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="SqlSingle"/></returns>
        [NotNull]
        public static object DeserializeSqlDateTime([NotNull] byte[] buffer, ref long offset)
            => new SqlDateTime((Blittable8)VariableLengthEncoding.DecodeULong(buffer, ref offset));

        /// <summary>
        /// Deserializes a <see cref="double"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="double"/></returns>
        [NotNull]
        public static object DeserializeDateTime([NotNull] byte[] buffer, ref long offset)
            => (DateTime)(Blittable8)VariableLengthEncoding.DecodeULong(buffer, ref offset);
        #endregion

        #region DateTimeOffset
        /// <summary>
        /// Serializes a <see cref="DateTimeOffset"/>.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="sqlValue">The SQL value.</param>
        public static void SerializeDateTimeOffset([NotNull] Stream stream, [NotNull] object sqlValue)
        {
            DateTimeOffset d = (DateTimeOffset)sqlValue;
            stream.Write((Blittable2)(short)(d.Offset.Ticks / TimeSpan.TicksPerMinute), 0, 2);
            VariableLengthEncoding.Encode((ulong)d.Ticks, stream);
        }

        /// <summary>
        /// Deserializes a <see cref="SqlSingle"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="SqlSingle"/></returns>
        [NotNull]
        public static object DeserializeDateTimeOffset([NotNull] byte[] buffer, ref long offset)
        {
            short o = new Blittable2(buffer[offset++], buffer[offset++]);
            return new DateTimeOffset(
                (long)VariableLengthEncoding.DecodeULong(buffer, ref offset),
                new TimeSpan(o * TimeSpan.TicksPerMinute));
        }
        #endregion

        #region TimeSpan
        /// <summary>
        /// Serializes a <see cref="TimeSpan" />.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="sqlValue">The <see cref="TimeSpan" />.</param>
        public static void SerializeTimeSpan([NotNull] Stream stream, [NotNull] object sqlValue)
            => VariableLengthEncoding.Encode((ulong)(Blittable8)(TimeSpan)sqlValue, stream);

        /// <summary>
        /// Deserializes a <see cref="SqlSingle"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="SqlSingle"/></returns>
        [NotNull]
        public static object DeserializeTimeSpan([NotNull] byte[] buffer, ref long offset)
            => (TimeSpan)(Blittable8)VariableLengthEncoding.DecodeULong(buffer, ref offset);
        #endregion

        #region SqlGuid
        /// <summary>
        /// Serializes a <see cref="SqlGuid"/>.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="sqlValue">The SQL value.</param>
        public static void SerializeSqlGuid([NotNull] Stream stream, [NotNull] object sqlValue)
            => stream.Write((Blittable16)((SqlGuid)sqlValue).Value, 0, 16);

        /// <summary>
        /// Deserializes a <see cref="SqlSingle"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="SqlSingle"/></returns>
        [NotNull]
        public static object DeserializeSqlGuid([NotNull] byte[] buffer, ref long offset)
            => new SqlGuid(
                (Guid)
                    new Blittable16(
                    buffer[offset++],
                    buffer[offset++],
                    buffer[offset++],
                    buffer[offset++],
                    buffer[offset++],
                    buffer[offset++],
                    buffer[offset++],
                    buffer[offset++],
                    buffer[offset++],
                    buffer[offset++],
                    buffer[offset++],
                    buffer[offset++],
                    buffer[offset++],
                    buffer[offset++],
                    buffer[offset++],
                    buffer[offset++]));

        /// <summary>
        /// Deserializes a <see cref="decimal"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="decimal"/></returns>
        [NotNull]
        public static object DeserializeGuid([NotNull] byte[] buffer, ref long offset)
            => (Guid)new Blittable16(
                buffer[offset++],
                buffer[offset++],
                buffer[offset++],
                buffer[offset++],
                buffer[offset++],
                buffer[offset++],
                buffer[offset++],
                buffer[offset++],
                buffer[offset++],
                buffer[offset++],
                buffer[offset++],
                buffer[offset++],
                buffer[offset++],
                buffer[offset++],
                buffer[offset++],
                buffer[offset++]);
        #endregion

        #region SqlBinary
        /// <summary>
        /// Serializes a <see cref="SqlBinary"/>.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="sqlValue">The SQL value.</param>
        public static void SerializeSqlBinary([NotNull] Stream stream, [NotNull] object sqlValue)
        {
            SqlBinary b = (SqlBinary)sqlValue;
            int length = b.Length;
            VariableLengthEncoding.Encode((uint)length, stream);

            // TODO perf-tune, this avoids an uneccesary copy, but does that outweight the looping?
            for (int i = 0; i < length; i++)
                stream.WriteByte(b[i]);
        }

        /// <summary>
        /// Deserializes a <see cref="SqlSingle"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="SqlSingle"/></returns>
        [NotNull]
        public static object DeserializeSqlBinary([NotNull] byte[] buffer, ref long offset)
        {
            int length = (int)VariableLengthEncoding.DecodeUInt(buffer, ref offset);
            byte[] bytes = new byte[length];
            Array.Copy(buffer, offset, bytes, 0, length);
            offset += length;
            return new SqlBinary(bytes);
        }

        /// <summary>
        /// Deserializes a <see cref="decimal"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="decimal"/></returns>
        [NotNull]
        public static object DeserializeByteArray([NotNull] byte[] buffer, ref long offset)
        {
            int length = (int)VariableLengthEncoding.DecodeUInt(buffer, ref offset);
            byte[] bytes = new byte[length];
            Array.Copy(buffer, offset, bytes, 0, length);
            return bytes;
        }
        #endregion

        #region SqlString
        /// <summary>
        /// Serializes a <see cref="SqlString" />.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="sqlValue">The <see cref="SqlString" />.</param>
        public static void SerializeAnsiSqlString([NotNull] Stream stream, [NotNull] object sqlValue)
        {
            SqlString s = (SqlString)sqlValue;
            VariableLengthEncoding.Encode(s.LCID, stream);
            VariableLengthEncoding.Encode((uint)s.SqlCompareOptions, stream);
            // ReSharper disable once AssignNullToNotNullAttribute
            SerializeString(stream, Encoding.GetEncoding(new CultureInfo(s.LCID).TextInfo.ANSICodePage), s.Value);
        }

        /// <summary>
        /// Serializes a <see cref="SqlString" />.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="sqlValue">The <see cref="SqlString" />.</param>
        public static void SerializeSqlString([NotNull] Stream stream, [NotNull] object sqlValue)
        {
            SqlString s = (SqlString)sqlValue;
            VariableLengthEncoding.Encode(s.LCID, stream);
            VariableLengthEncoding.Encode((uint)s.SqlCompareOptions, stream);
            // ReSharper disable once AssignNullToNotNullAttribute
            SerializeString(stream, Encoding.UTF8, s.Value);
        }

        /// <summary>
        /// Serializes a <see cref="string" /> to the <paramref name="stream" /> using the
        /// <paramref name="encoding">specified encoding</paramref>.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="value">The value.</param>
        private static void SerializeString(
            [NotNull] Stream stream,
            [NotNull] Encoding encoding,
            [NotNull] string value)
        {
            uint length = (uint)encoding.GetByteCount(value);
            VariableLengthEncoding.Encode(length, stream);
            byte[] bytes = encoding.GetBytes(value);
            stream.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Deserializes a <see cref="SqlSingle"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="SqlSingle"/></returns>
        [NotNull]
        public static object DeserializeAnsiSqlString([NotNull] byte[] buffer, ref long offset)
        {
            int lcid = VariableLengthEncoding.DecodeInt(buffer, ref offset);
            SqlCompareOptions options = (SqlCompareOptions)VariableLengthEncoding.DecodeUInt(buffer, ref offset);
            int length = (int)VariableLengthEncoding.DecodeUInt(buffer, ref offset);

            SqlString result;
            if (offset + length <= int.MaxValue)
                result = new SqlString(lcid, options, buffer, (int)offset, length, false);
            else
            {
                byte[] bytes = new byte[length];
                Array.Copy(buffer, offset, bytes, 0, length);
                result = new SqlString(lcid, options, bytes, 0, length, false);
            }
            offset += length;
            return result;
        }

        /// <summary>
        /// Deserializes a <see cref="SqlSingle"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="SqlSingle"/></returns>
        [NotNull]
        public static object DeserializeSqlString([NotNull] byte[] buffer, ref long offset)
        {
            int lcid = VariableLengthEncoding.DecodeInt(buffer, ref offset);
            SqlCompareOptions options = (SqlCompareOptions)VariableLengthEncoding.DecodeUInt(buffer, ref offset);
            int length = (int)VariableLengthEncoding.DecodeUInt(buffer, ref offset);
            byte[] bytes;
            if (offset + length <= int.MaxValue)
                bytes = Encoding.Convert(Encoding.UTF8, Encoding.Unicode, buffer, (int)offset, length);
            else
            {
                byte[] b = new byte[length];
                Array.Copy(buffer, offset, b, 0, length);
                bytes = Encoding.Convert(Encoding.UTF8, Encoding.Unicode, b, 0, length);
            }
            offset += length;
            return new SqlString(lcid, options, bytes, true);
        }

        /// <summary>
        /// Deserializes a <see cref="string"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="string"/></returns>
        [NotNull]
        public static object DeserializeAnsiString([NotNull] byte[] buffer, ref long offset)
        {
            int lcid = VariableLengthEncoding.DecodeInt(buffer, ref offset);
            VariableLengthEncoding.DecodeUInt(buffer, ref offset);
            return DeserializeString(
                Encoding.GetEncoding(new CultureInfo(lcid).TextInfo.ANSICodePage),
                buffer,
                ref offset);
        }

        /// <summary>
        /// Deserializes a <see cref="string"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="string"/></returns>
        [NotNull]
        public static object DeserializeString([NotNull] byte[] buffer, ref long offset)
        {
            // Skip unused data
            VariableLengthEncoding.DecodeInt(buffer, ref offset);
            VariableLengthEncoding.DecodeUInt(buffer, ref offset);
            return DeserializeString(Encoding.UTF8, buffer, ref offset);
        }

        /// <summary>
        /// Deserializes a <see cref="string" />.
        /// </summary>
        /// <param name="encoding">The encoding.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="string" /></returns>
        [NotNull]
        private static string DeserializeString([NotNull] Encoding encoding, [NotNull] byte[] buffer, ref long offset)
        {
            int length = (int)VariableLengthEncoding.DecodeUInt(buffer, ref offset);

            string result;
            if (offset + length <= int.MaxValue)
                result = encoding.GetString(buffer, (int)offset, length);
            else
            {
                byte[] bytes = new byte[length];
                Array.Copy(buffer, offset, bytes, 0, length);
                result = encoding.GetString(buffer, 0, length);
            }
            offset += length;
            return result;
        }
        #endregion

        #region SqlXml
        /// <summary>
        /// Serializes a <see cref="SqlXml" />.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="sqlValue">The <see cref="SqlXml" />.</param>
        public static void SerializeSqlXml([NotNull] Stream stream, [NotNull] object sqlValue)
            => SerializeString(stream, Encoding.UTF8, ((SqlXml)sqlValue).Value);

        /// <summary>
        /// Deserializes a <see cref="SqlSingle"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="SqlSingle"/></returns>
        [NotNull]
        public static object DeserializeSqlXml([NotNull] byte[] buffer, ref long offset)
        {
            int length = (int)VariableLengthEncoding.DecodeUInt(buffer, ref offset);
            byte[] bytes;
            if (offset + length <= int.MaxValue)
                bytes = Encoding.Convert(Encoding.UTF8, Encoding.Unicode, buffer, (int)offset, length);
            else
            {
                byte[] b = new byte[length];
                Array.Copy(buffer, offset, b, 0, length);
                bytes = Encoding.Convert(Encoding.UTF8, Encoding.Unicode, b, 0, length);
            }
            offset += length;
            using (MemoryStream ms = new MemoryStream(bytes))
                return new SqlXml(ms);
        }

        /// <summary>
        /// Deserializes a <see cref="string"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized <see cref="string"/></returns>
        [NotNull]
        public static object DeserializeXmlString([NotNull] byte[] buffer, ref long offset)
            => DeserializeString(Encoding.UTF8, buffer, ref offset);
        #endregion

        #region UDT
        /// <summary>
        /// SQL User Defined Types, with special casing for performance.
        /// </summary>
        private enum UdtType : byte
        {
            BinarySerialize = 0,
            Geography = 1,
            GeographyNull = 2,
            GeographyZero = 3,
            Geometry = 8,
            GeometryNull = 9,
            GeometryZero = 10,
            HierarchyId = 16,
            HierarchyIdNull = 17,
            Serializable = 128
        }

        /// <summary>
        /// The earth SRID.
        /// </summary>
        public const int EarthSrid = 4326;

        /// <summary>
        /// The geography zero point (on <see cref="EarthSrid">Earth</see>).
        /// </summary>
        [NotNull]
        // ReSharper disable once AssignNullToNotNullAttribute
        public static SqlGeography GeographyZeroPoint = SqlGeography.Point(0D, 0D, EarthSrid);

        /// <summary>
        /// The geometry zero point (on <see cref="EarthSrid">Earth</see>).
        /// </summary>
        [NotNull]
        // ReSharper disable once AssignNullToNotNullAttribute
        public static SqlGeometry GeometryZeroPoint = SqlGeometry.Point(0D, 0D, EarthSrid);

        /// <summary>
        /// The <see cref="SqlGeography"/> simple full name.
        /// </summary>
        [NotNull]
        private static readonly string _sqlGeographySimpleFullName =
            ExtendedType.Get(typeof(SqlGeography)).SimpleFullName;

        /// <summary>
        /// The <see cref="SqlGeometry"/> simple full name.
        /// </summary>
        [NotNull]
        private static readonly string _sqlGeometrySimpleFullName = ExtendedType.Get(typeof(SqlGeometry)).SimpleFullName;

        /// <summary>
        /// The <see cref="SqlHierarchyId"/> simple full name.
        /// </summary>
        [NotNull]
        private static readonly string _sqlHierarchyIdSimpleFullName =
            ExtendedType.Get(typeof(SqlHierarchyId)).SimpleFullName;

        /// <summary>
        /// Serializes a UDT.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="sqlValue">The UDT.</param>
        public static void SerializeUdt([NotNull] Stream stream, [NotNull] object sqlValue)
        {
            IBinarySerialize value = sqlValue as IBinarySerialize;
            if (value != null)
            {
                // The UDT is Format.UserDefined
                // (see https://msdn.microsoft.com/en-us/library/microsoft.sqlserver.server.format(v=vs.110).aspx)

                // Get the type
                ExtendedType et = value.GetType();

                if (string.Equals(et.SimpleFullName, _sqlGeographySimpleFullName))
                {
                    SqlGeography geography = (SqlGeography)sqlValue;

                    // Special case for null
                    if (geography.IsNull)
                    {
                        stream.WriteByte((byte)UdtType.GeographyNull);
                        return;
                    }

                    // Special case for Zero point
                    if (!geography.HasM &&
                        !geography.HasZ &&
                        !geography.Lat.IsNull &&
                        !geography.Long.IsNull &&
                        !geography.STSrid.IsNull &&
                        // ReSharper disable CompareOfFloatsByEqualityOperator
                        geography.Lat.Value == 0D &&
                        geography.Long.Value == 0D &&
                        // ReSharper restore CompareOfFloatsByEqualityOperator
                        geography.STSrid.Value == EarthSrid)
                    {
                        stream.WriteByte((byte)UdtType.GeographyZero);
                        return;
                    }

                    stream.WriteByte((byte)UdtType.Geography);
                }
                else if (string.Equals(et.SimpleFullName, _sqlGeometrySimpleFullName))
                {
                    SqlGeometry geometry = (SqlGeometry)sqlValue;

                    // Special case for null
                    if (geometry.IsNull)
                    {
                        stream.WriteByte((byte)UdtType.GeometryNull);
                        return;
                    }

                    // Special case for Zero point
                    if (!geometry.HasM &&
                        !geometry.HasZ &&
                        !geometry.STX.IsNull &&
                        !geometry.STY.IsNull &&
                        !geometry.STSrid.IsNull &&
                        // ReSharper disable CompareOfFloatsByEqualityOperator
                        geometry.STX.Value == 0D &&
                        geometry.STY.Value == 0D &&
                        // ReSharper restore CompareOfFloatsByEqualityOperator
                        geometry.STSrid.Value == EarthSrid)
                    {
                        stream.WriteByte((byte)UdtType.GeometryZero);
                        return;
                    }

                    stream.WriteByte((byte)UdtType.Geometry);
                }
                else if (string.Equals(et.SimpleFullName, _sqlHierarchyIdSimpleFullName))
                {
                    SqlHierarchyId hierarchyId = (SqlHierarchyId)sqlValue;

                    // Special case for null
                    if (hierarchyId.IsNull)
                    {
                        stream.WriteByte((byte)UdtType.HierarchyIdNull);
                        return;
                    }

                    stream.WriteByte((byte)UdtType.HierarchyId);
                }
                else
                {
                    // Mark format
                    stream.WriteByte((byte)UdtType.BinarySerialize);

                    // Write the simplified type name so we can recreate the type later
                    SerializeString(stream, Encoding.UTF8, et.SimpleFullName);
                }

                // Serialize object
                using (MemoryStream ms = new MemoryStream())
                {
                    using (BinaryWriter writer = new BinaryWriter(ms))
                    {
                        value.Write(writer);

                        // Encode stream length
                        long length = ms.Length;
                        VariableLengthEncoding.Encode((ulong)length, stream);

                        // Return to start
                        ms.Seek(0, SeekOrigin.Begin);

                        // ReSharper disable once AssignNullToNotNullAttribute
                        stream.Write(ms.GetBuffer(), 0, length);
                        return;
                    }
                }
            }

            // We have to assume the object is serializable
            stream.WriteByte((byte)UdtType.Serializable);

            using (MemoryStream ms = new MemoryStream())
            {
                Serialization.Serialize.GetFormatter().Serialize(ms, sqlValue);

                // Encode stream length
                long length = ms.Length;
                VariableLengthEncoding.Encode((ulong)length, stream);

                // Return to start
                ms.Seek(0, SeekOrigin.Begin);

                // ReSharper disable once AssignNullToNotNullAttribute
                stream.Write(ms.GetBuffer(), 0, length);
            }
        }

        /// <summary>
        /// Deserializes a UDT.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized UDT.</returns>
        [NotNull]
        public static object DeserializeUdt([NotNull] byte[] buffer, ref long offset)
        {
            // Get the type and create the binary object 
            UdtType udtType = (UdtType)buffer[offset++];
            IBinarySerialize binaryObject;
            switch (udtType)
            {
                case UdtType.BinarySerialize:
                    binaryObject = (IBinarySerialize)Activator.CreateInstance(
                        // ReSharper disable once AssignNullToNotNullAttribute
                        ExtendedType.FindType(
                            DeserializeString(Encoding.UTF8, buffer, ref offset),
                            false,
                            true));
                    break;
                case UdtType.Geography:
                    binaryObject = new SqlGeography();
                    break;
                case UdtType.GeographyNull:
                    // ReSharper disable once AssignNullToNotNullAttribute
                    return SqlGeography.Null;
                case UdtType.GeographyZero:
                    return GeographyZeroPoint;
                case UdtType.Geometry:
                    binaryObject = new SqlGeometry();
                    break;
                case UdtType.GeometryNull:
                    // ReSharper disable once AssignNullToNotNullAttribute
                    return SqlGeometry.Null;
                case UdtType.GeometryZero:
                    return GeometryZeroPoint;
                case UdtType.HierarchyId:
                    binaryObject = new SqlHierarchyId();
                    break;
                case UdtType.HierarchyIdNull:
                    return SqlHierarchyId.Null;
                default:
                    binaryObject = null;
                    break;
            }

            // Get data length
            long length = (long)VariableLengthEncoding.DecodeULong(buffer, ref offset);

            // Make 32-bit safe
            byte[] b;
            int o;
            int l = checked ((int)length);
            if (offset + length > int.MaxValue)
            {
                b = new byte[length];
                o = 0;
                Array.Copy(buffer, offset, b, 0L, length);
            }
            else
            {
                b = buffer;
                o = checked ((int)offset);
            }

            // Update offset
            offset += length;

            using (MemoryStream ms = new MemoryStream(b, o, l))
            {
                // Deserialize object
                if (binaryObject == null) return Serialization.Serialize.GetFormatter().Deserialize(ms);

                // Deserialize data into binary object
                using (BinaryReader reader = new BinaryReader(ms))
                    binaryObject.Read(reader);
                return binaryObject;
            }
        }
        #endregion

        #region Variant
        /// <summary>
        /// Serializes a Variant.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="sqlValue">The UDT.</param>
        public static void SerializeVariant([NotNull] Stream stream, [NotNull] object sqlValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deserializes a Variant.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized Variant.</returns>
        [NotNull]
        public static object DeserializeVariant([NotNull] byte[] buffer, ref long offset)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Structured
        /// <summary>
        /// Serializes a Structured.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="sqlValue">The UDT.</param>
        public static void SerializeStructured([NotNull] Stream stream, [NotNull] object sqlValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deserializes a Structured.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The deserialized Structured.</returns>
        [NotNull]
        public static object DeserializeStructured([NotNull] byte[] buffer, ref long offset)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}