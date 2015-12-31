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

using System.Runtime.InteropServices;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Blit
{
    /// <summary>
    /// Allows blitting of 4-byte structures
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    [PublicAPI]
    public struct Blittable4
    {
        #region Float
        /// <summary>
        /// The float value.
        /// </summary>
        [FieldOffset(0)]
        public readonly float Float;

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable4"/> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public Blittable4(float value)
            : this()
        {
            Float = value;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="float"/> to <see cref="Blittable4"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Blittable4(float value) => new Blittable4(value);

        /// <summary>
        /// Performs an implicit conversion from <see cref="Blittable4"/> to <see cref="float"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator float(Blittable4 value) => value.Float;
        #endregion

        #region UInt
        /// <summary>
        /// The uint value.
        /// </summary>
        [FieldOffset(0)]
        public readonly uint UInt;

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable4"/> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public Blittable4(uint value)
            : this()
        {
            UInt = value;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="uint"/> to <see cref="Blittable4"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Blittable4(uint value) => new Blittable4(value);

        /// <summary>
        /// Performs an implicit conversion from <see cref="Blittable4"/> to <see cref="uint"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator uint(Blittable4 value) => value.UInt;
        #endregion

        #region Int
        /// <summary>
        /// The int value.
        /// </summary>
        [FieldOffset(0)]
        public readonly int Int;

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable4"/> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public Blittable4(int value)
            : this()
        {
            Int = value;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="int"/> to <see cref="Blittable4"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Blittable4(int value) => new Blittable4(value);

        /// <summary>
        /// Performs an implicit conversion from <see cref="Blittable4"/> to <see cref="int"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator int(Blittable4 value) => value.Int;
        #endregion

        #region UShort
        /// <summary>
        /// The first 2 bytes as a <see cref="ushort"/>
        /// </summary>
        [FieldOffset(0)]
        public readonly ushort UShort0;

        /// <summary>
        /// The second 2 bytes as a <see cref="ushort"/>
        /// </summary>
        [FieldOffset(2)]
        public readonly ushort UShort1;

        /// <summary>
        /// Gets the data as an array of <see cref="int"/>.
        /// </summary>
        /// <value>The ints.</value>
        public ushort[] UShorts => new[] { UShort0, UShort1 };

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable4" /> struct.
        /// </summary>
        /// <param name="ushort0">The 0th <see cref="ushort"/>.</param>
        /// <param name="ushort1">The 1st <see cref="ushort"/>.</param>
        public Blittable4(
            ushort ushort0,
            ushort ushort1)
            : this()
        {
            UShort0 = ushort0;
            UShort1 = ushort1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable4"/> struct from an array of <see cref="ushort"/>
        /// </summary>
        /// <param name="ushorts">The ushorts array.</param>
        public Blittable4([NotNull] ushort[] ushorts)
            : this()
        {
            UShort0 = ushorts[0];
            UShort1 = ushorts[1];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable4" /> struct from an array of <see cref="ushort"/>.
        /// </summary>
        /// <param name="ushorts">The ushorts array.</param>
        /// <param name="offset">The offset into the array.</param>
        public Blittable4([NotNull] ushort[] ushorts, long offset)
            : this()
        {
            UShort0 = ushorts[offset];
            UShort1 = ushorts[offset + 1];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable4" /> struct from an array of <see cref="ushort"/>.
        /// </summary>
        /// <param name="ushorts">The ushorts array.</param>
        /// <param name="offset">The offset into the array.</param>
        public Blittable4([NotNull] ushort[] ushorts, ref long offset)
            : this()
        {
            UShort0 = ushorts[offset++];
            UShort1 = ushorts[offset++];
        }
        #endregion

        #region Short
        /// <summary>
        /// The first 2 bytes as a <see cref="short"/>
        /// </summary>
        [FieldOffset(0)]
        public readonly short Short0;

        /// <summary>
        /// The second 2 bytes as a <see cref="short"/>
        /// </summary>
        [FieldOffset(2)]
        public readonly short Short1;

        /// <summary>
        /// Gets the data as an array of <see cref="int"/>.
        /// </summary>
        /// <value>The ints.</value>
        public short[] Shorts => new[] { Short0, Short1 };

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable4" /> struct.
        /// </summary>
        /// <param name="short0">The 0th <see cref="short"/>.</param>
        /// <param name="short1">The 1st <see cref="short"/>.</param>
        public Blittable4(
            short short0,
            short short1)
            : this()
        {
            Short0 = short0;
            Short1 = short1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable4"/> struct from an array of <see cref="short"/>
        /// </summary>
        /// <param name="shorts">The shorts array.</param>
        public Blittable4([NotNull] short[] shorts)
            : this()
        {
            Short0 = shorts[0];
            Short1 = shorts[1];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable4" /> struct from an array of <see cref="short"/>.
        /// </summary>
        /// <param name="shorts">The shorts array.</param>
        /// <param name="offset">The offset into the array.</param>
        public Blittable4([NotNull] short[] shorts, long offset)
            : this()
        {
            Short0 = shorts[offset];
            Short1 = shorts[offset + 1];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable4" /> struct from an array of <see cref="short"/>.
        /// </summary>
        /// <param name="shorts">The shorts array.</param>
        /// <param name="offset">The offset into the array.</param>
        public Blittable4([NotNull] short[] shorts, ref long offset)
            : this()
        {
            Short0 = shorts[offset++];
            Short1 = shorts[offset++];
        }
        #endregion

        #region Byte
        /// <summary>
        /// The 0th <see cref="byte"/>.
        /// </summary>
        [FieldOffset(0)]
        public readonly byte Byte0;

        /// <summary>
        /// The 1st <see cref="byte"/>.
        /// </summary>
        [FieldOffset(1)]
        public readonly byte Byte1;

        /// <summary>
        /// The 2nd <see cref="byte"/>.
        /// </summary>
        [FieldOffset(2)]
        public readonly byte Byte2;

        /// <summary>
        /// The 3rd <see cref="byte"/>.
        /// </summary>
        [FieldOffset(3)]
        public readonly byte Byte3;

        /// <summary>
        /// Gets the data as an array of <see cref="byte"/>.
        /// </summary>
        /// <value>The bytes.</value>
        [NotNull]
        public byte[] Bytes => new[] { Byte0, Byte1, Byte2, Byte3 };

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable4" /> struct.
        /// </summary>
        /// <param name="byte0">The 0th <see cref="byte"/>.</param>
        /// <param name="byte1">The 1st <see cref="byte"/>.</param>
        /// <param name="byte2">The 2nd <see cref="byte"/>.</param>
        /// <param name="byte3">The 3rd <see cref="byte"/>.</param>
        public Blittable4(
            byte byte0,
            byte byte1,
            byte byte2,
            byte byte3)
            : this()
        {
            Byte0 = byte0;
            Byte1 = byte1;
            Byte2 = byte2;
            Byte3 = byte3;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable4"/> struct from an array of <see cref="byte"/>
        /// </summary>
        /// <param name="bytes">The bytes array.</param>
        public Blittable4([NotNull] byte[] bytes)
            :this()
        {
            Byte0 = bytes[0];
            Byte1 = bytes[1];
            Byte2 = bytes[2];
            Byte3 = bytes[3];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable4" /> struct from an array of <see cref="byte"/>.
        /// </summary>
        /// <param name="bytes">The bytes array.</param>
        /// <param name="offset">The offset into the array.</param>
        public Blittable4([NotNull] byte[] bytes, long offset)
            : this()
        {
            Byte0 = bytes[offset];
            Byte1 = bytes[offset + 1];
            Byte2 = bytes[offset + 2];
            Byte3 = bytes[offset + 3];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable4" /> struct from an array of <see cref="byte"/>.
        /// </summary>
        /// <param name="bytes">The bytes array.</param>
        /// <param name="offset">The offset into the array.</param>
        public Blittable4([NotNull] byte[] bytes, ref long offset)
            : this()
        {
            Byte0 = bytes[offset++];
            Byte1 = bytes[offset++];
            Byte2 = bytes[offset++];
            Byte3 = bytes[offset++];
        }
        #endregion

        #region SByte
        /// <summary>
        /// The 0th <see cref="sbyte"/>.
        /// </summary>
        [FieldOffset(0)]
        public readonly sbyte SByte0;

        /// <summary>
        /// The 1st <see cref="sbyte"/>.
        /// </summary>
        [FieldOffset(1)]
        public readonly sbyte SByte1;

        /// <summary>
        /// The 2nd <see cref="sbyte"/>.
        /// </summary>
        [FieldOffset(2)]
        public readonly sbyte SByte2;

        /// <summary>
        /// The 3rd <see cref="sbyte"/>.
        /// </summary>
        [FieldOffset(3)]
        public readonly sbyte SByte3;

        /// <summary>
        /// Gets the data as an array of <see cref="sbyte"/>.
        /// </summary>
        /// <value>The sbytes.</value>
        [NotNull]
        public sbyte[] SBytes => new[] { SByte0, SByte1, SByte2, SByte3 };

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable4" /> struct.
        /// </summary>
        /// <param name="sbyte0">The 0th <see cref="sbyte"/>.</param>
        /// <param name="sbyte1">The 1st <see cref="sbyte"/>.</param>
        /// <param name="sbyte2">The 2nd <see cref="sbyte"/>.</param>
        /// <param name="sbyte3">The 3rd <see cref="sbyte"/>.</param>
        public Blittable4(
            sbyte sbyte0,
            sbyte sbyte1,
            sbyte sbyte2,
            sbyte sbyte3)
            : this()
        {
            SByte0 = sbyte0;
            SByte1 = sbyte1;
            SByte2 = sbyte2;
            SByte3 = sbyte3;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable4"/> struct from an array of <see cref="sbyte"/>
        /// </summary>
        /// <param name="sbytes">The sbytes array.</param>
        public Blittable4([NotNull] sbyte[] sbytes)
            : this()
        {
            SByte0 = sbytes[0];
            SByte1 = sbytes[1];
            SByte2 = sbytes[2];
            SByte3 = sbytes[3];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable4" /> struct from an array of <see cref="sbyte"/>.
        /// </summary>
        /// <param name="sbytes">The sbytes array.</param>
        /// <param name="offset">The offset into the array.</param>
        public Blittable4([NotNull] sbyte[] sbytes, long offset)
            : this()
        {
            SByte0 = sbytes[offset];
            SByte1 = sbytes[offset + 1];
            SByte2 = sbytes[offset + 2];
            SByte3 = sbytes[offset + 3];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable4" /> struct from an array of <see cref="sbyte"/>.
        /// </summary>
        /// <param name="sbytes">The sbytes array.</param>
        /// <param name="offset">The offset into the array.</param>
        public Blittable4([NotNull] sbyte[] sbytes, ref long offset)
            : this()
        {
            SByte0 = sbytes[offset++];
            SByte1 = sbytes[offset++];
            SByte2 = sbytes[offset++];
            SByte3 = sbytes[offset++];
        }
        #endregion
    }
}