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
using System.Runtime.InteropServices;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Blit
{
    /// <summary>
    /// Allows blitting of 8-byte structures
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    [PublicAPI]
    public struct Blittable8
    {
        /// <summary>
        /// Performs an implicit conversion from <see cref="Blittable8"/> to <see cref="T:byte[]"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator byte[] (Blittable8 value) => value.Bytes;

        /// <summary>
        /// Performs an explicit conversion from <see cref="T:byte[]"/> to <see cref="Blittable8"/>.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator Blittable8([NotNull] byte[] bytes) => new Blittable8(bytes);

        #region TimeSpan
        /// <summary>
        /// The TimeSpan value.
        /// </summary>
        [FieldOffset(0)]
        public readonly TimeSpan TimeSpan;

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable8"/> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public Blittable8(TimeSpan value)
            : this()
        {
            TimeSpan = value;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="TimeSpan"/> to <see cref="Blittable8"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Blittable8(TimeSpan value) => new Blittable8(value);

        /// <summary>
        /// Performs an implicit conversion from <see cref="Blittable8"/> to <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator TimeSpan(Blittable8 value) => value.TimeSpan;
        #endregion

        #region DateTime
        /// <summary>
        /// The DateTime value.
        /// </summary>
        [FieldOffset(0)]
        public readonly DateTime DateTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable8"/> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public Blittable8(DateTime value)
            : this()
        {
            DateTime = value;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="DateTime"/> to <see cref="Blittable8"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Blittable8(DateTime value) => new Blittable8(value);

        /// <summary>
        /// Performs an implicit conversion from <see cref="Blittable8"/> to <see cref="DateTime"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator DateTime(Blittable8 value) => value.DateTime;
        #endregion

        #region Double
        /// <summary>
        /// The double value.
        /// </summary>
        [FieldOffset(0)]
        public readonly double Double;

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable8"/> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public Blittable8(double value)
            : this()
        {
            Double = value;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="double"/> to <see cref="Blittable8"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Blittable8(double value) => new Blittable8(value);

        /// <summary>
        /// Performs an implicit conversion from <see cref="Blittable8"/> to <see cref="double"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator double(Blittable8 value) => value.Double;
        #endregion

        #region ULong
        /// <summary>
        /// The ulong value.
        /// </summary>
        [FieldOffset(0)]
        public readonly ulong ULong;

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable8"/> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public Blittable8(ulong value)
            : this()
        {
            ULong = value;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="ulong"/> to <see cref="Blittable8"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Blittable8(ulong value) => new Blittable8(value);

        /// <summary>
        /// Performs an implicit conversion from <see cref="Blittable8"/> to <see cref="ulong"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator ulong(Blittable8 value) => value.ULong;
        #endregion

        #region Long
        /// <summary>
        /// The long value.
        /// </summary>
        [FieldOffset(0)]
        public readonly long Long;

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable8"/> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public Blittable8(long value)
            : this()
        {
            Long = value;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="long"/> to <see cref="Blittable8"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Blittable8(long value) => new Blittable8(value);

        /// <summary>
        /// Performs an implicit conversion from <see cref="Blittable8"/> to <see cref="long"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator long(Blittable8 value) => value.Long;
        #endregion

        #region Float
        /// <summary>
        /// The first 4 bytes as a <see cref="float"/>
        /// </summary>
        [FieldOffset(0)]
        public readonly float Float0;

        /// <summary>
        /// The second 4 bytes as a <see cref="float"/>
        /// </summary>
        [FieldOffset(4)]
        public readonly float Float1;

        /// <summary>
        /// Gets the data as an array of <see cref="float"/>.
        /// </summary>
        /// <value>The ints.</value>
        public float[] Floats => new[] { Float0, Float1 };

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable8"/> struct.
        /// </summary>
        /// <param name="float0">The 0th <see cref="float"/>.</param>
        /// <param name="float1">The 1st <see cref="float"/>.</param>
        public Blittable8(float float0, float float1)
            : this()
        {
            Float0 = float0;
            Float1 = float1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable8" /> struct from an array of <see cref="float"/>.
        /// </summary>
        /// <param name="floats">The values array.</param>
        public Blittable8([NotNull] float[] floats)
            : this()
        {
            Float0 = floats[0];
            Float1 = floats[1];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable8" /> struct from an array of <see cref="float" />.
        /// </summary>
        /// <param name="floats">The values array.</param>
        /// <param name="offset">The offset into the array.</param>
        public Blittable8([NotNull] float[] floats, long offset)
            : this()
        {
            Float0 = floats[offset];
            Float1 = floats[offset + 1];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable8" /> struct from an array of <see cref="float" />.
        /// </summary>
        /// <param name="floats">The values array.</param>
        /// <param name="offset">The offset into the array.</param>
        public Blittable8([NotNull] float[] floats, ref long offset)
            : this()
        {
            Float0 = floats[offset++];
            Float1 = floats[offset++];
        }
        #endregion

        #region UInt
        /// <summary>
        /// The first 4 bytes as a <see cref="uint"/>
        /// </summary>
        [FieldOffset(0)]
        public readonly uint UInt0;

        /// <summary>
        /// The second 4 bytes as a <see cref="uint"/>
        /// </summary>
        [FieldOffset(4)]
        public readonly uint UInt1;

        /// <summary>
        /// Gets the data as an array of <see cref="uint"/>.
        /// </summary>
        /// <value>The ints.</value>
        public uint[] UInts => new[] { UInt0, UInt1 };

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable8"/> struct.
        /// </summary>
        /// <param name="uint0">The 0th <see cref="uint"/>.</param>
        /// <param name="uint1">The 1st <see cref="uint"/>.</param>
        public Blittable8(uint uint0, uint uint1)
            : this()
        {
            UInt0 = uint0;
            UInt1 = uint1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable8" /> struct from an array of <see cref="uint"/>.
        /// </summary>
        /// <param name="uints">The values array.</param>
        public Blittable8([NotNull] uint[] uints)
            : this()
        {
            UInt0 = uints[0];
            UInt1 = uints[1];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable8" /> struct from an array of <see cref="uint" />.
        /// </summary>
        /// <param name="uints">The values array.</param>
        /// <param name="offset">The offset into the array.</param>
        public Blittable8([NotNull] uint[] uints, long offset)
            : this()
        {
            UInt0 = uints[offset];
            UInt1 = uints[offset + 1];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable8" /> struct from an array of <see cref="uint" />.
        /// </summary>
        /// <param name="uints">The values array.</param>
        /// <param name="offset">The offset into the array.</param>
        public Blittable8([NotNull] uint[] uints, ref long offset)
            : this()
        {
            UInt0 = uints[offset++];
            UInt1 = uints[offset++];
        }
        #endregion

        #region Int
        /// <summary>
        /// The first 4 bytes as a <see cref="int"/>
        /// </summary>
        [FieldOffset(0)]
        public readonly int Int0;

        /// <summary>
        /// The second 4 bytes as a <see cref="int"/>
        /// </summary>
        [FieldOffset(4)]
        public readonly int Int1;

        /// <summary>
        /// Gets the data as an array of <see cref="int"/>.
        /// </summary>
        /// <value>The ints.</value>
        public int[] Ints => new[] { Int0, Int1 };

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable8"/> struct.
        /// </summary>
        /// <param name="int0">The 0th <see cref="int"/>.</param>
        /// <param name="int1">The 1st <see cref="int"/>.</param>
        public Blittable8(int int0, int int1)
            : this()
        {
            Int0 = int0;
            Int1 = int1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable8" /> struct from an array of <see cref="int"/>.
        /// </summary>
        /// <param name="ints">The values array.</param>
        public Blittable8([NotNull] int[] ints)
            : this()
        {
            Int0 = ints[0];
            Int1 = ints[1];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable8" /> struct from an array of <see cref="int" />.
        /// </summary>
        /// <param name="ints">The values array.</param>
        /// <param name="offset">The offset into the array.</param>
        public Blittable8([NotNull] int[] ints, long offset)
            : this()
        {
            Int0 = ints[offset];
            Int1 = ints[offset + 1];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable8" /> struct from an array of <see cref="int" />.
        /// </summary>
        /// <param name="ints">The values array.</param>
        /// <param name="offset">The offset into the array.</param>
        public Blittable8([NotNull] int[] ints, ref long offset)
            : this()
        {
            Int0 = ints[offset++];
            Int1 = ints[offset++];
        }
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
        /// The third 2 bytes as a <see cref="ushort"/>
        /// </summary>
        [FieldOffset(4)]
        public readonly ushort UShort2;

        /// <summary>
        /// The forth 2 bytes as a <see cref="ushort"/>
        /// </summary>
        [FieldOffset(6)]
        public readonly ushort UShort3;

        /// <summary>
        /// Gets the data as an array of <see cref="int"/>.
        /// </summary>
        /// <value>The ints.</value>
        public ushort[] UShorts => new[] { UShort0, UShort1, UShort2, UShort3 };

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable8" /> struct.
        /// </summary>
        /// <param name="ushort0">The 0th <see cref="ushort"/>.</param>
        /// <param name="ushort1">The 1st <see cref="ushort"/>.</param>
        /// <param name="ushort2">The 2nd <see cref="ushort"/>.</param>
        /// <param name="ushort3">The 3rd <see cref="ushort"/>.</param>
        public Blittable8(
            ushort ushort0,
            ushort ushort1,
            ushort ushort2,
            ushort ushort3)
            : this()
        {
            UShort0 = ushort0;
            UShort1 = ushort1;
            UShort2 = ushort2;
            UShort3 = ushort3;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable8"/> struct from an array of <see cref="ushort"/>
        /// </summary>
        /// <param name="ushorts">The ushorts array.</param>
        public Blittable8([NotNull] ushort[] ushorts)
            : this()
        {
            UShort0 = ushorts[0];
            UShort1 = ushorts[1];
            UShort2 = ushorts[2];
            UShort3 = ushorts[3];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable8" /> struct from an array of <see cref="ushort"/>.
        /// </summary>
        /// <param name="ushorts">The ushorts array.</param>
        /// <param name="offset">The offset into the array.</param>
        public Blittable8([NotNull] ushort[] ushorts, long offset)
            : this()
        {
            UShort0 = ushorts[offset];
            UShort1 = ushorts[offset + 1];
            UShort2 = ushorts[offset + 2];
            UShort3 = ushorts[offset + 3];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable8" /> struct from an array of <see cref="ushort"/>.
        /// </summary>
        /// <param name="ushorts">The ushorts array.</param>
        /// <param name="offset">The offset into the array.</param>
        public Blittable8([NotNull] ushort[] ushorts, ref long offset)
            : this()
        {
            UShort0 = ushorts[offset++];
            UShort1 = ushorts[offset++];
            UShort2 = ushorts[offset++];
            UShort3 = ushorts[offset++];
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
        /// The third 2 bytes as a <see cref="short"/>
        /// </summary>
        [FieldOffset(4)]
        public readonly short Short2;

        /// <summary>
        /// The forth 2 bytes as a <see cref="short"/>
        /// </summary>
        [FieldOffset(6)]
        public readonly short Short3;

        /// <summary>
        /// Gets the data as an array of <see cref="int"/>.
        /// </summary>
        /// <value>The ints.</value>
        public short[] Shorts => new[] { Short0, Short1, Short2, Short3 };

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable8" /> struct.
        /// </summary>
        /// <param name="short0">The 0th <see cref="short"/>.</param>
        /// <param name="short1">The 1st <see cref="short"/>.</param>
        /// <param name="short2">The 2nd <see cref="short"/>.</param>
        /// <param name="short3">The 3rd <see cref="short"/>.</param>
        public Blittable8(
            short short0,
            short short1,
            short short2,
            short short3)
            : this()
        {
            Short0 = short0;
            Short1 = short1;
            Short2 = short2;
            Short3 = short3;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable8"/> struct from an array of <see cref="short"/>
        /// </summary>
        /// <param name="shorts">The shorts array.</param>
        public Blittable8([NotNull] short[] shorts)
            : this()
        {
            Short0 = shorts[0];
            Short1 = shorts[1];
            Short2 = shorts[2];
            Short3 = shorts[3];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable8" /> struct from an array of <see cref="short"/>.
        /// </summary>
        /// <param name="shorts">The shorts array.</param>
        /// <param name="offset">The offset into the array.</param>
        public Blittable8([NotNull] short[] shorts, long offset)
            : this()
        {
            Short0 = shorts[offset];
            Short1 = shorts[offset + 1];
            Short2 = shorts[offset + 2];
            Short3 = shorts[offset + 3];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable8" /> struct from an array of <see cref="short"/>.
        /// </summary>
        /// <param name="shorts">The shorts array.</param>
        /// <param name="offset">The offset into the array.</param>
        public Blittable8([NotNull] short[] shorts, ref long offset)
            : this()
        {
            Short0 = shorts[offset++];
            Short1 = shorts[offset++];
            Short2 = shorts[offset++];
            Short3 = shorts[offset++];
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
        /// The 4th <see cref="byte"/>.
        /// </summary>
        [FieldOffset(4)]
        public readonly byte Byte4;

        /// <summary>
        /// The 5th <see cref="byte"/>.
        /// </summary>
        [FieldOffset(5)]
        public readonly byte Byte5;

        /// <summary>
        /// The 6th <see cref="byte"/>.
        /// </summary>
        [FieldOffset(6)]
        public readonly byte Byte6;

        /// <summary>
        /// The 7th <see cref="byte"/>.
        /// </summary>
        [FieldOffset(7)]
        public readonly byte Byte7;

        /// <summary>
        /// Gets the data as an array of <see cref="byte"/>.
        /// </summary>
        /// <value>The bytes.</value>
        [NotNull]
        public byte[] Bytes => new[] { Byte0, Byte1, Byte2, Byte3, Byte4, Byte5, Byte6, Byte7 };

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable8" /> struct.
        /// </summary>
        /// <param name="byte0">The 0th <see cref="byte"/>.</param>
        /// <param name="byte1">The 1st <see cref="byte"/>.</param>
        /// <param name="byte2">The 2nd <see cref="byte"/>.</param>
        /// <param name="byte3">The 3rd <see cref="byte"/>.</param>
        /// <param name="byte4">The 4th <see cref="byte"/>.</param>
        /// <param name="byte5">The 5th <see cref="byte"/>.</param>
        /// <param name="byte6">The 6th <see cref="byte"/>.</param>
        /// <param name="byte7">The 7th <see cref="byte"/>.</param>
        public Blittable8(
            byte byte0,
            byte byte1,
            byte byte2,
            byte byte3,
            byte byte4,
            byte byte5,
            byte byte6,
            byte byte7)
            : this()
        {
            Byte0 = byte0;
            Byte1 = byte1;
            Byte2 = byte2;
            Byte3 = byte3;
            Byte4 = byte4;
            Byte5 = byte5;
            Byte6 = byte6;
            Byte7 = byte7;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable8"/> struct from an array of <see cref="byte"/>
        /// </summary>
        /// <param name="bytes">The bytes array.</param>
        public Blittable8([NotNull] byte[] bytes)
            :this()
        {
            Byte0 = bytes[0];
            Byte1 = bytes[1];
            Byte2 = bytes[2];
            Byte3 = bytes[3];
            Byte4 = bytes[4];
            Byte5 = bytes[5];
            Byte6 = bytes[6];
            Byte7 = bytes[7];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable8" /> struct from an array of <see cref="byte"/>.
        /// </summary>
        /// <param name="bytes">The bytes array.</param>
        /// <param name="offset">The offset into the array.</param>
        public Blittable8([NotNull] byte[] bytes, long offset)
            : this()
        {
            Byte0 = bytes[offset];
            Byte1 = bytes[offset + 1];
            Byte2 = bytes[offset + 2];
            Byte3 = bytes[offset + 3];
            Byte4 = bytes[offset + 4];
            Byte5 = bytes[offset + 5];
            Byte6 = bytes[offset + 6];
            Byte7 = bytes[offset + 7];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable8" /> struct from an array of <see cref="byte"/>.
        /// </summary>
        /// <param name="bytes">The bytes array.</param>
        /// <param name="offset">The offset into the array.</param>
        public Blittable8([NotNull] byte[] bytes, ref long offset)
            : this()
        {
            Byte0 = bytes[offset++];
            Byte1 = bytes[offset++];
            Byte2 = bytes[offset++];
            Byte3 = bytes[offset++];
            Byte4 = bytes[offset++];
            Byte5 = bytes[offset++];
            Byte6 = bytes[offset++];
            Byte7 = bytes[offset++];
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
        /// The 4th <see cref="sbyte"/>.
        /// </summary>
        [FieldOffset(4)]
        public readonly sbyte SByte4;

        /// <summary>
        /// The 5th <see cref="sbyte"/>.
        /// </summary>
        [FieldOffset(5)]
        public readonly sbyte SByte5;

        /// <summary>
        /// The 6th <see cref="sbyte"/>.
        /// </summary>
        [FieldOffset(6)]
        public readonly sbyte SByte6;

        /// <summary>
        /// The 7th <see cref="sbyte"/>.
        /// </summary>
        [FieldOffset(7)]
        public readonly sbyte SByte7;

        /// <summary>
        /// Gets the data as an array of <see cref="sbyte"/>.
        /// </summary>
        /// <value>The sbytes.</value>
        [NotNull]
        public sbyte[] SBytes => new[] { SByte0, SByte1, SByte2, SByte3, SByte4, SByte5, SByte6, SByte7 };

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable8" /> struct.
        /// </summary>
        /// <param name="sbyte0">The 0th <see cref="sbyte"/>.</param>
        /// <param name="sbyte1">The 1st <see cref="sbyte"/>.</param>
        /// <param name="sbyte2">The 2nd <see cref="sbyte"/>.</param>
        /// <param name="sbyte3">The 3rd <see cref="sbyte"/>.</param>
        /// <param name="sbyte4">The 4th <see cref="sbyte"/>.</param>
        /// <param name="sbyte5">The 5th <see cref="sbyte"/>.</param>
        /// <param name="sbyte6">The 6th <see cref="sbyte"/>.</param>
        /// <param name="sbyte7">The 7th <see cref="sbyte"/>.</param>
        public Blittable8(
            sbyte sbyte0,
            sbyte sbyte1,
            sbyte sbyte2,
            sbyte sbyte3,
            sbyte sbyte4,
            sbyte sbyte5,
            sbyte sbyte6,
            sbyte sbyte7)
            : this()
        {
            SByte0 = sbyte0;
            SByte1 = sbyte1;
            SByte2 = sbyte2;
            SByte3 = sbyte3;
            SByte4 = sbyte4;
            SByte5 = sbyte5;
            SByte6 = sbyte6;
            SByte7 = sbyte7;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable8"/> struct from an array of <see cref="sbyte"/>
        /// </summary>
        /// <param name="sbytes">The sbytes array.</param>
        public Blittable8([NotNull] sbyte[] sbytes)
            : this()
        {
            SByte0 = sbytes[0];
            SByte1 = sbytes[1];
            SByte2 = sbytes[2];
            SByte3 = sbytes[3];
            SByte4 = sbytes[4];
            SByte5 = sbytes[5];
            SByte6 = sbytes[6];
            SByte7 = sbytes[7];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable8" /> struct from an array of <see cref="sbyte"/>.
        /// </summary>
        /// <param name="sbytes">The sbytes array.</param>
        /// <param name="offset">The offset into the array.</param>
        public Blittable8([NotNull] sbyte[] sbytes, long offset)
            : this()
        {
            SByte0 = sbytes[offset];
            SByte1 = sbytes[offset + 1];
            SByte2 = sbytes[offset + 2];
            SByte3 = sbytes[offset + 3];
            SByte4 = sbytes[offset + 4];
            SByte5 = sbytes[offset + 5];
            SByte6 = sbytes[offset + 6];
            SByte7 = sbytes[offset + 7];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable8" /> struct from an array of <see cref="sbyte"/>.
        /// </summary>
        /// <param name="sbytes">The sbytes array.</param>
        /// <param name="offset">The offset into the array.</param>
        public Blittable8([NotNull] sbyte[] sbytes, ref long offset)
            : this()
        {
            SByte0 = sbytes[offset++];
            SByte1 = sbytes[offset++];
            SByte2 = sbytes[offset++];
            SByte3 = sbytes[offset++];
            SByte4 = sbytes[offset++];
            SByte5 = sbytes[offset++];
            SByte6 = sbytes[offset++];
            SByte7 = sbytes[offset++];
        }
        #endregion
    }
}