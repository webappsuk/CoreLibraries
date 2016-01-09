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
    /// Allows blitting of 16-byte structures
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    [PublicAPI]
    public struct Blittable16
    {
        /// <summary>
        /// Performs an implicit conversion from <see cref="Blittable16"/> to <see cref="T:byte[]"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator byte[] (Blittable16 value) => value.Bytes;

        /// <summary>
        /// Performs an explicit conversion from <see cref="T:byte[]"/> to <see cref="Blittable16"/>.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator Blittable16([NotNull] byte[] bytes) => new Blittable16(bytes);

        #region Decimal
        /// <summary>
        /// The decimal value.
        /// </summary>
        [FieldOffset(0)]
        public readonly decimal Decimal;

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable16"/> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public Blittable16(decimal value)
            : this()
        {
            Decimal = value;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="decimal"/> to <see cref="Blittable16"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Blittable16(decimal value) => new Blittable16(value);

        /// <summary>
        /// Performs an implicit conversion from <see cref="Blittable16"/> to <see cref="decimal"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator decimal(Blittable16 value) => value.Decimal;
        #endregion

        #region Guid
        /// <summary>
        /// The Guid value.
        /// </summary>
        [FieldOffset(0)]
        public readonly Guid Guid;

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable16"/> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public Blittable16(Guid value)
            : this()
        {
            Guid = value;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Guid"/> to <see cref="Blittable16"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Blittable16(Guid value) => new Blittable16(value);

        /// <summary>
        /// Performs an implicit conversion from <see cref="Blittable16"/> to <see cref="Guid"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Guid(Blittable16 value) => value.Guid;
        #endregion

        #region TimeSpan
        /// <summary>
        /// The first 8 bytes as a <see cref="TimeSpan"/>
        /// </summary>
        [FieldOffset(0)]
        public readonly TimeSpan TimeSpan0;

        /// <summary>
        /// The second 8 bytes as a <see cref="TimeSpan"/>
        /// </summary>
        [FieldOffset(8)]
        public readonly TimeSpan TimeSpan1;

        /// <summary>
        /// Gets the data as an array of <see cref="TimeSpan"/>.
        /// </summary>
        /// <value>The ints.</value>
        public TimeSpan[] TimeSpans => new[] { TimeSpan0, TimeSpan1 };

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable16"/> struct.
        /// </summary>
        /// <param name="timeSpan0">The 0th <see cref="TimeSpan"/>.</param>
        /// <param name="timeSpan1">The 1st <see cref="TimeSpan"/>.</param>
        public Blittable16(TimeSpan timeSpan0, TimeSpan timeSpan1)
            : this()
        {
            TimeSpan0 = timeSpan0;
            TimeSpan1 = timeSpan1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable16" /> struct from an array of <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="timeSpans">The values array.</param>
        public Blittable16([NotNull] TimeSpan[] timeSpans)
            : this()
        {
            TimeSpan0 = timeSpans[0];
            TimeSpan1 = timeSpans[1];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable16" /> struct from an array of <see cref="TimeSpan" />.
        /// </summary>
        /// <param name="timeSpans">The values array.</param>
        /// <param name="offset">The offset into the array.</param>
        public Blittable16([NotNull] TimeSpan[] timeSpans, long offset)
            : this()
        {
            TimeSpan0 = timeSpans[offset];
            TimeSpan1 = timeSpans[offset + 1];
        }
        #endregion

        #region DateTime
        /// <summary>
        /// The first 8 bytes as a <see cref="DateTime"/>
        /// </summary>
        [FieldOffset(0)]
        public readonly DateTime DateTime0;

        /// <summary>
        /// The second 8 bytes as a <see cref="DateTime"/>
        /// </summary>
        [FieldOffset(8)]
        public readonly DateTime DateTime1;

        /// <summary>
        /// Gets the data as an array of <see cref="DateTime"/>.
        /// </summary>
        /// <value>The ints.</value>
        public DateTime[] DateTimes => new[] { DateTime0, DateTime1 };

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable16"/> struct.
        /// </summary>
        /// <param name="dateTime0">The 0th <see cref="DateTime"/>.</param>
        /// <param name="dateTime1">The 1st <see cref="DateTime"/>.</param>
        public Blittable16(DateTime dateTime0, DateTime dateTime1)
            : this()
        {
            DateTime0 = dateTime0;
            DateTime1 = dateTime1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable16" /> struct from an array of <see cref="DateTime"/>.
        /// </summary>
        /// <param name="dateTimes">The values array.</param>
        public Blittable16([NotNull] DateTime[] dateTimes)
            : this()
        {
            DateTime0 = dateTimes[0];
            DateTime1 = dateTimes[1];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable16" /> struct from an array of <see cref="DateTime" />.
        /// </summary>
        /// <param name="dateTimes">The values array.</param>
        /// <param name="offset">The offset into the array.</param>
        public Blittable16([NotNull] DateTime[] dateTimes, long offset)
            : this()
        {
            DateTime0 = dateTimes[offset];
            DateTime1 = dateTimes[offset + 1];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable16" /> struct from an array of <see cref="DateTime" />.
        /// </summary>
        /// <param name="dateTimes">The values array.</param>
        /// <param name="offset">The offset into the array.</param>
        public Blittable16([NotNull] DateTime[] dateTimes, ref long offset)
            : this()
        {
            DateTime0 = dateTimes[offset++];
            DateTime1 = dateTimes[offset++];
        }
        #endregion

        #region Double
        /// <summary>
        /// The first 8 bytes as a <see cref="double"/>
        /// </summary>
        [FieldOffset(0)]
        public readonly double Double0;

        /// <summary>
        /// The second 8 bytes as a <see cref="double"/>
        /// </summary>
        [FieldOffset(8)]
        public readonly double Double1;

        /// <summary>
        /// Gets the data as an array of <see cref="double"/>.
        /// </summary>
        /// <value>The ints.</value>
        public double[] Doubles => new[] { Double0, Double1 };

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable16"/> struct.
        /// </summary>
        /// <param name="double0">The 0th <see cref="double"/>.</param>
        /// <param name="double1">The 1st <see cref="double"/>.</param>
        public Blittable16(double double0, double double1)
            : this()
        {
            Double0 = double0;
            Double1 = double1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable16" /> struct from an array of <see cref="double"/>.
        /// </summary>
        /// <param name="doubles">The values array.</param>
        public Blittable16([NotNull] double[] doubles)
            : this()
        {
            Double0 = doubles[0];
            Double1 = doubles[1];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable16" /> struct from an array of <see cref="double" />.
        /// </summary>
        /// <param name="doubles">The values array.</param>
        /// <param name="offset">The offset into the array.</param>
        public Blittable16([NotNull] double[] doubles, long offset)
            : this()
        {
            Double0 = doubles[offset];
            Double1 = doubles[offset + 1];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable16" /> struct from an array of <see cref="double" />.
        /// </summary>
        /// <param name="doubles">The values array.</param>
        /// <param name="offset">The offset into the array.</param>
        public Blittable16([NotNull] double[] doubles, ref long offset)
            : this()
        {
            Double0 = doubles[offset++];
            Double1 = doubles[offset++];
        }
        #endregion

        #region ULong
        /// <summary>
        /// The first 8 bytes as a <see cref="ulong"/>
        /// </summary>
        [FieldOffset(0)]
        public readonly ulong ULong0;

        /// <summary>
        /// The second 8 bytes as a <see cref="ulong"/>
        /// </summary>
        [FieldOffset(8)]
        public readonly ulong ULong1;

        /// <summary>
        /// Gets the data as an array of <see cref="ulong"/>.
        /// </summary>
        /// <value>The ints.</value>
        public ulong[] ULongs => new[] { ULong0, ULong1 };

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable16"/> struct.
        /// </summary>
        /// <param name="ulong0">The 0th <see cref="ulong"/>.</param>
        /// <param name="ulong1">The 1st <see cref="ulong"/>.</param>
        public Blittable16(ulong ulong0, ulong ulong1)
            : this()
        {
            ULong0 = ulong0;
            ULong1 = ulong1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable16" /> struct from an array of <see cref="ulong"/>.
        /// </summary>
        /// <param name="ulongs">The values array.</param>
        public Blittable16([NotNull] ulong[] ulongs)
            : this()
        {
            ULong0 = ulongs[0];
            ULong1 = ulongs[1];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable16" /> struct from an array of <see cref="ulong" />.
        /// </summary>
        /// <param name="ulongs">The values array.</param>
        /// <param name="offset">The offset into the array.</param>
        public Blittable16([NotNull] ulong[] ulongs, long offset)
            : this()
        {
            ULong0 = ulongs[offset];
            ULong1 = ulongs[offset + 1];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable16" /> struct from an array of <see cref="ulong" />.
        /// </summary>
        /// <param name="ulongs">The values array.</param>
        /// <param name="offset">The offset into the array.</param>
        public Blittable16([NotNull] ulong[] ulongs, ref long offset)
            : this()
        {
            ULong0 = ulongs[offset++];
            ULong1 = ulongs[offset++];
        }
        #endregion

        #region Long
        /// <summary>
        /// The first 8 bytes as a <see cref="long"/>
        /// </summary>
        [FieldOffset(0)]
        public readonly long Long0;

        /// <summary>
        /// The second 8 bytes as a <see cref="long"/>
        /// </summary>
        [FieldOffset(8)]
        public readonly long Long1;

        /// <summary>
        /// Gets the data as an array of <see cref="long"/>.
        /// </summary>
        /// <value>The ints.</value>
        public long[] Longs => new[] { Long0, Long1 };

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable16"/> struct.
        /// </summary>
        /// <param name="long0">The 0th <see cref="long"/>.</param>
        /// <param name="long1">The 1st <see cref="long"/>.</param>
        public Blittable16(long long0, long long1)
            : this()
        {
            Long0 = long0;
            Long1 = long1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable16" /> struct from an array of <see cref="long"/>.
        /// </summary>
        /// <param name="longs">The values array.</param>
        public Blittable16([NotNull] long[] longs)
            : this()
        {
            Long0 = longs[0];
            Long1 = longs[1];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable16" /> struct from an array of <see cref="long" />.
        /// </summary>
        /// <param name="longs">The values array.</param>
        /// <param name="offset">The offset into the array.</param>
        public Blittable16([NotNull] long[] longs, long offset)
            : this()
        {
            Long0 = longs[offset];
            Long1 = longs[offset + 1];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable16" /> struct from an array of <see cref="long" />.
        /// </summary>
        /// <param name="longs">The values array.</param>
        /// <param name="offset">The offset into the array.</param>
        public Blittable16([NotNull] long[] longs, ref long offset)
            : this()
        {
            Long0 = longs[offset++];
            Long1 = longs[offset++];
        }
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
        /// The third 4 bytes as a <see cref="float"/>
        /// </summary>
        [FieldOffset(8)]
        public readonly float Float2;

        /// <summary>
        /// The forth 4 bytes as a <see cref="float"/>
        /// </summary>
        [FieldOffset(12)]
        public readonly float Float3;

        /// <summary>
        /// Gets the data as an array of <see cref="float"/>.
        /// </summary>
        /// <value>The ints.</value>
        public float[] Floats => new[] { Float0, Float1, Float2, Float3 };

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable16"/> struct.
        /// </summary>
        /// <param name="float0">The 0th <see cref="float"/>.</param>
        /// <param name="float1">The 1st <see cref="float"/>.</param>
        /// <param name="float2">The 2nd <see cref="float"/>.</param>
        /// <param name="float3">The 3rd <see cref="float"/>.</param>
        public Blittable16(float float0, float float1, float float2, float float3)
            : this()
        {
            Float0 = float0;
            Float1 = float1;
            Float2 = float2;
            Float3 = float3;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable16" /> struct from an array of <see cref="float"/>.
        /// </summary>
        /// <param name="floats">The values array.</param>
        public Blittable16([NotNull] float[] floats)
            : this()
        {
            Float0 = floats[0];
            Float1 = floats[1];
            Float2 = floats[2];
            Float3 = floats[3];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable16" /> struct from an array of <see cref="float" />.
        /// </summary>
        /// <param name="floats">The values array.</param>
        /// <param name="offset">The offset into the array.</param>
        public Blittable16([NotNull] float[] floats, long offset)
            : this()
        {
            Float0 = floats[offset];
            Float1 = floats[offset + 1];
            Float2 = floats[offset + 2];
            Float3 = floats[offset + 3];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable16" /> struct from an array of <see cref="float" />.
        /// </summary>
        /// <param name="floats">The values array.</param>
        /// <param name="offset">The offset into the array.</param>
        public Blittable16([NotNull] float[] floats, ref long offset)
            : this()
        {
            Float0 = floats[offset++];
            Float1 = floats[offset++];
            Float2 = floats[offset++];
            Float3 = floats[offset++];
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
        /// The third 4 bytes as a <see cref="uint"/>
        /// </summary>
        [FieldOffset(8)]
        public readonly uint UInt2;

        /// <summary>
        /// The forth 4 bytes as a <see cref="uint"/>
        /// </summary>
        [FieldOffset(12)]
        public readonly uint UInt3;

        /// <summary>
        /// Gets the data as an array of <see cref="uint"/>.
        /// </summary>
        /// <value>The ints.</value>
        public uint[] UInts => new[] { UInt0, UInt1, UInt2, UInt3 };

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable16"/> struct.
        /// </summary>
        /// <param name="uint0">The 0th <see cref="uint"/>.</param>
        /// <param name="uint1">The 1st <see cref="uint"/>.</param>
        /// <param name="uint2">The 2nd <see cref="uint"/>.</param>
        /// <param name="uint3">The 3rd <see cref="uint"/>.</param>
        public Blittable16(uint uint0, uint uint1, uint uint2, uint uint3)
            : this()
        {
            UInt0 = uint0;
            UInt1 = uint1;
            UInt2 = uint2;
            UInt3 = uint3;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable16" /> struct from an array of <see cref="uint"/>.
        /// </summary>
        /// <param name="uints">The values array.</param>
        public Blittable16([NotNull] uint[] uints)
            : this()
        {
            UInt0 = uints[0];
            UInt1 = uints[1];
            UInt2 = uints[2];
            UInt3 = uints[3];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable16" /> struct from an array of <see cref="uint" />.
        /// </summary>
        /// <param name="uints">The values array.</param>
        /// <param name="offset">The offset into the array.</param>
        public Blittable16([NotNull] uint[] uints, long offset)
            : this()
        {
            UInt0 = uints[offset];
            UInt1 = uints[offset + 1];
            UInt2 = uints[offset + 2];
            UInt3 = uints[offset + 3];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable16" /> struct from an array of <see cref="uint" />.
        /// </summary>
        /// <param name="uints">The values array.</param>
        /// <param name="offset">The offset into the array.</param>
        public Blittable16([NotNull] uint[] uints, ref long offset)
            : this()
        {
            UInt0 = uints[offset++];
            UInt1 = uints[offset++];
            UInt2 = uints[offset++];
            UInt3 = uints[offset++];
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
        /// The third 4 bytes as a <see cref="int"/>
        /// </summary>
        [FieldOffset(8)]
        public readonly int Int2;

        /// <summary>
        /// The forth 4 bytes as a <see cref="int"/>
        /// </summary>
        [FieldOffset(12)]
        public readonly int Int3;

        /// <summary>
        /// Gets the data as an array of <see cref="int"/>.
        /// </summary>
        /// <value>The ints.</value>
        public int[] Ints => new[] { Int0, Int1, Int2, Int3 };

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable16"/> struct.
        /// </summary>
        /// <param name="int0">The 0th <see cref="int"/>.</param>
        /// <param name="int1">The 1st <see cref="int"/>.</param>
        /// <param name="int2">The 2nd <see cref="int"/>.</param>
        /// <param name="int3">The 3rd <see cref="int"/>.</param>
        public Blittable16(int int0, int int1, int int2, int int3)
            : this()
        {
            Int0 = int0;
            Int1 = int1;
            Int2 = int2;
            Int3 = int3;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable16" /> struct from an array of <see cref="int"/>.
        /// </summary>
        /// <param name="ints">The values array.</param>
        public Blittable16([NotNull] int[] ints)
            : this()
        {
            Int0 = ints[0];
            Int1 = ints[1];
            Int2 = ints[2];
            Int3 = ints[3];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable16" /> struct from an array of <see cref="int" />.
        /// </summary>
        /// <param name="ints">The values array.</param>
        /// <param name="offset">The offset into the array.</param>
        public Blittable16([NotNull] int[] ints, long offset)
            : this()
        {
            Int0 = ints[offset];
            Int1 = ints[offset + 1];
            Int2 = ints[offset + 2];
            Int3 = ints[offset + 3];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable16" /> struct from an array of <see cref="int" />.
        /// </summary>
        /// <param name="ints">The values array.</param>
        /// <param name="offset">The offset into the array.</param>
        public Blittable16([NotNull] int[] ints, ref long offset)
            : this()
        {
            Int0 = ints[offset++];
            Int1 = ints[offset++];
            Int2 = ints[offset++];
            Int3 = ints[offset++];
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
        /// The fifth 2 bytes as a <see cref="ushort"/>
        /// </summary>
        [FieldOffset(8)]
        public readonly ushort UShort4;

        /// <summary>
        /// The sixth 2 bytes as a <see cref="ushort"/>
        /// </summary>
        [FieldOffset(10)]
        public readonly ushort UShort5;

        /// <summary>
        /// The seventh 2 bytes as a <see cref="ushort"/>
        /// </summary>
        [FieldOffset(12)]
        public readonly ushort UShort6;

        /// <summary>
        /// The eigthh 2 bytes as a <see cref="ushort"/>
        /// </summary>
        [FieldOffset(14)]
        public readonly ushort UShort7;

        /// <summary>
        /// Gets the data as an array of <see cref="int"/>.
        /// </summary>
        /// <value>The ints.</value>
        public ushort[] UShorts => new[] { UShort0, UShort1, UShort2, UShort3, UShort4, UShort5, UShort6, UShort7 };

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable16" /> struct.
        /// </summary>
        /// <param name="ushort0">The 0th <see cref="ushort"/>.</param>
        /// <param name="ushort1">The 1st <see cref="ushort"/>.</param>
        /// <param name="ushort2">The 2nd <see cref="ushort"/>.</param>
        /// <param name="ushort3">The 3rd <see cref="ushort"/>.</param>
        /// <param name="ushort4">The 4th <see cref="ushort"/>.</param>
        /// <param name="ushort5">The 5th <see cref="ushort"/>.</param>
        /// <param name="ushort6">The 6th <see cref="ushort"/>.</param>
        /// <param name="ushort7">The 7th <see cref="ushort"/>.</param>
        public Blittable16(
            ushort ushort0,
            ushort ushort1,
            ushort ushort2,
            ushort ushort3,
            ushort ushort4,
            ushort ushort5,
            ushort ushort6,
            ushort ushort7)
            : this()
        {
            UShort0 = ushort0;
            UShort1 = ushort1;
            UShort2 = ushort2;
            UShort3 = ushort3;
            UShort4 = ushort4;
            UShort5 = ushort5;
            UShort6 = ushort6;
            UShort7 = ushort7;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable16"/> struct from an array of <see cref="ushort"/>
        /// </summary>
        /// <param name="ushorts">The ushorts array.</param>
        public Blittable16([NotNull] ushort[] ushorts)
            : this()
        {
            UShort0 = ushorts[0];
            UShort1 = ushorts[1];
            UShort2 = ushorts[2];
            UShort3 = ushorts[3];
            UShort4 = ushorts[4];
            UShort5 = ushorts[5];
            UShort6 = ushorts[6];
            UShort7 = ushorts[7];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable16" /> struct from an array of <see cref="ushort"/>.
        /// </summary>
        /// <param name="ushorts">The ushorts array.</param>
        /// <param name="offset">The offset into the array.</param>
        public Blittable16([NotNull] ushort[] ushorts, long offset)
            : this()
        {
            UShort0 = ushorts[offset];
            UShort1 = ushorts[offset + 1];
            UShort2 = ushorts[offset + 2];
            UShort3 = ushorts[offset + 3];
            UShort4 = ushorts[offset + 4];
            UShort5 = ushorts[offset + 5];
            UShort6 = ushorts[offset + 6];
            UShort7 = ushorts[offset + 7];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable16" /> struct from an array of <see cref="ushort"/>.
        /// </summary>
        /// <param name="ushorts">The ushorts array.</param>
        /// <param name="offset">The offset into the array.</param>
        public Blittable16([NotNull] ushort[] ushorts, ref long offset)
            : this()
        {
            UShort0 = ushorts[offset++];
            UShort1 = ushorts[offset++];
            UShort2 = ushorts[offset++];
            UShort3 = ushorts[offset++];
            UShort4 = ushorts[offset++];
            UShort5 = ushorts[offset++];
            UShort6 = ushorts[offset++];
            UShort7 = ushorts[offset++];
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
        /// The fifth 2 bytes as a <see cref="short"/>
        /// </summary>
        [FieldOffset(8)]
        public readonly short Short4;

        /// <summary>
        /// The sixth 2 bytes as a <see cref="short"/>
        /// </summary>
        [FieldOffset(10)]
        public readonly short Short5;

        /// <summary>
        /// The seventh 2 bytes as a <see cref="short"/>
        /// </summary>
        [FieldOffset(12)]
        public readonly short Short6;

        /// <summary>
        /// The eigthh 2 bytes as a <see cref="short"/>
        /// </summary>
        [FieldOffset(14)]
        public readonly short Short7;

        /// <summary>
        /// Gets the data as an array of <see cref="int"/>.
        /// </summary>
        /// <value>The ints.</value>
        public short[] Shorts => new[] { Short0, Short1, Short2, Short3, Short4, Short5, Short6, Short7 };

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable16" /> struct.
        /// </summary>
        /// <param name="short0">The 0th <see cref="short"/>.</param>
        /// <param name="short1">The 1st <see cref="short"/>.</param>
        /// <param name="short2">The 2nd <see cref="short"/>.</param>
        /// <param name="short3">The 3rd <see cref="short"/>.</param>
        /// <param name="short4">The 4th <see cref="short"/>.</param>
        /// <param name="short5">The 5th <see cref="short"/>.</param>
        /// <param name="short6">The 6th <see cref="short"/>.</param>
        /// <param name="short7">The 7th <see cref="short"/>.</param>
        /// <param name="short8">The 8th <see cref="short"/>.</param>
        /// <param name="short9">The 9th <see cref="short"/>.</param>
        /// <param name="short10">The 10th <see cref="short"/>.</param>
        /// <param name="short11">The 11th <see cref="short"/>.</param>
        /// <param name="short12">The 12th <see cref="short"/>.</param>
        /// <param name="short13">The 13th <see cref="short"/>.</param>
        /// <param name="short14">The 14th <see cref="short"/>.</param>
        /// <param name="short15">The 15th <see cref="short"/>.</param>
        public Blittable16(
            short short0,
            short short1,
            short short2,
            short short3,
            short short4,
            short short5,
            short short6,
            short short7,
            short short8,
            short short9,
            short short10,
            short short11,
            short short12,
            short short13,
            short short14,
            short short15)
            : this()
        {
            Short0 = short0;
            Short1 = short1;
            Short2 = short2;
            Short3 = short3;
            Short4 = short4;
            Short5 = short5;
            Short6 = short6;
            Short7 = short7;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable16"/> struct from an array of <see cref="short"/>
        /// </summary>
        /// <param name="shorts">The shorts array.</param>
        public Blittable16([NotNull] short[] shorts)
            : this()
        {
            Short0 = shorts[0];
            Short1 = shorts[1];
            Short2 = shorts[2];
            Short3 = shorts[3];
            Short4 = shorts[4];
            Short5 = shorts[5];
            Short6 = shorts[6];
            Short7 = shorts[7];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable16" /> struct from an array of <see cref="short"/>.
        /// </summary>
        /// <param name="shorts">The shorts array.</param>
        /// <param name="offset">The offset into the array.</param>
        public Blittable16([NotNull] short[] shorts, long offset)
            : this()
        {
            Short0 = shorts[offset];
            Short1 = shorts[offset + 1];
            Short2 = shorts[offset + 2];
            Short3 = shorts[offset + 3];
            Short4 = shorts[offset + 4];
            Short5 = shorts[offset + 5];
            Short6 = shorts[offset + 6];
            Short7 = shorts[offset + 7];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable16" /> struct from an array of <see cref="short"/>.
        /// </summary>
        /// <param name="shorts">The shorts array.</param>
        /// <param name="offset">The offset into the array.</param>
        public Blittable16([NotNull] short[] shorts, ref long offset)
            : this()
        {
            Short0 = shorts[offset++];
            Short1 = shorts[offset++];
            Short2 = shorts[offset++];
            Short3 = shorts[offset++];
            Short4 = shorts[offset++];
            Short5 = shorts[offset++];
            Short6 = shorts[offset++];
            Short7 = shorts[offset++];
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
        /// The 8th <see cref="byte"/>.
        /// </summary>
        [FieldOffset(8)]
        public readonly byte Byte8;

        /// <summary>
        /// The 9th <see cref="byte"/>.
        /// </summary>
        [FieldOffset(9)]
        public readonly byte Byte9;

        /// <summary>
        /// The 10th <see cref="byte"/>.
        /// </summary>
        [FieldOffset(10)]
        public readonly byte Byte10;

        /// <summary>
        /// The 11th <see cref="byte"/>.
        /// </summary>
        [FieldOffset(11)]
        public readonly byte Byte11;

        /// <summary>
        /// The 12th <see cref="byte"/>.
        /// </summary>
        [FieldOffset(12)]
        public readonly byte Byte12;

        /// <summary>
        /// The 13th <see cref="byte"/>.
        /// </summary>
        [FieldOffset(13)]
        public readonly byte Byte13;

        /// <summary>
        /// The 14th <see cref="byte"/>.
        /// </summary>
        [FieldOffset(14)]
        public readonly byte Byte14;

        /// <summary>
        /// The 15th <see cref="byte"/>.
        /// </summary>
        [FieldOffset(15)]
        public readonly byte Byte15;

        /// <summary>
        /// Gets the data as an array of <see cref="byte"/>.
        /// </summary>
        /// <value>The bytes.</value>
        [NotNull]
        public byte[] Bytes => new[] { Byte0, Byte1, Byte2, Byte3, Byte4, Byte5, Byte6, Byte7, Byte8, Byte9, Byte10, Byte11, Byte12, Byte13, Byte14, Byte15 };

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable16" /> struct.
        /// </summary>
        /// <param name="byte0">The 0th <see cref="byte"/>.</param>
        /// <param name="byte1">The 1st <see cref="byte"/>.</param>
        /// <param name="byte2">The 2nd <see cref="byte"/>.</param>
        /// <param name="byte3">The 3rd <see cref="byte"/>.</param>
        /// <param name="byte4">The 4th <see cref="byte"/>.</param>
        /// <param name="byte5">The 5th <see cref="byte"/>.</param>
        /// <param name="byte6">The 6th <see cref="byte"/>.</param>
        /// <param name="byte7">The 7th <see cref="byte"/>.</param>
        /// <param name="byte8">The 8th <see cref="byte"/>.</param>
        /// <param name="byte9">The 9th <see cref="byte"/>.</param>
        /// <param name="byte10">The 10th <see cref="byte"/>.</param>
        /// <param name="byte11">The 11th <see cref="byte"/>.</param>
        /// <param name="byte12">The 12th <see cref="byte"/>.</param>
        /// <param name="byte13">The 13th <see cref="byte"/>.</param>
        /// <param name="byte14">The 14th <see cref="byte"/>.</param>
        /// <param name="byte15">The 15th <see cref="byte"/>.</param>
        public Blittable16(
            byte byte0,
            byte byte1,
            byte byte2,
            byte byte3,
            byte byte4,
            byte byte5,
            byte byte6,
            byte byte7,
            byte byte8,
            byte byte9,
            byte byte10,
            byte byte11,
            byte byte12,
            byte byte13,
            byte byte14,
            byte byte15)
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
            Byte8 = byte8;
            Byte9 = byte9;
            Byte10 = byte10;
            Byte11 = byte11;
            Byte12 = byte12;
            Byte13 = byte13;
            Byte14 = byte14;
            Byte15 = byte15;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable16"/> struct from an array of <see cref="byte"/>
        /// </summary>
        /// <param name="bytes">The bytes array.</param>
        public Blittable16([NotNull] byte[] bytes)
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
            Byte8 = bytes[8];
            Byte9 = bytes[9];
            Byte10 = bytes[10];
            Byte11 = bytes[11];
            Byte12 = bytes[12];
            Byte13 = bytes[13];
            Byte14 = bytes[14];
            Byte15 = bytes[15];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable16" /> struct from an array of <see cref="byte"/>.
        /// </summary>
        /// <param name="bytes">The bytes array.</param>
        /// <param name="offset">The offset into the array.</param>
        public Blittable16([NotNull] byte[] bytes, long offset)
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
            Byte8 = bytes[offset + 8];
            Byte9 = bytes[offset + 9];
            Byte10 = bytes[offset + 10];
            Byte11 = bytes[offset + 11];
            Byte12 = bytes[offset + 12];
            Byte13 = bytes[offset + 13];
            Byte14 = bytes[offset + 14];
            Byte15 = bytes[offset + 15];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable16" /> struct from an array of <see cref="byte"/>.
        /// </summary>
        /// <param name="bytes">The bytes array.</param>
        /// <param name="offset">The offset into the array.</param>
        public Blittable16([NotNull] byte[] bytes, ref long offset)
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
            Byte8 = bytes[offset++];
            Byte9 = bytes[offset++];
            Byte10 = bytes[offset++];
            Byte11 = bytes[offset++];
            Byte12 = bytes[offset++];
            Byte13 = bytes[offset++];
            Byte14 = bytes[offset++];
            Byte15 = bytes[offset++];
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
        /// The 8th <see cref="sbyte"/>.
        /// </summary>
        [FieldOffset(8)]
        public readonly sbyte SByte8;

        /// <summary>
        /// The 9th <see cref="sbyte"/>.
        /// </summary>
        [FieldOffset(9)]
        public readonly sbyte SByte9;

        /// <summary>
        /// The 10th <see cref="sbyte"/>.
        /// </summary>
        [FieldOffset(10)]
        public readonly sbyte SByte10;

        /// <summary>
        /// The 11th <see cref="sbyte"/>.
        /// </summary>
        [FieldOffset(11)]
        public readonly sbyte SByte11;

        /// <summary>
        /// The 12th <see cref="sbyte"/>.
        /// </summary>
        [FieldOffset(12)]
        public readonly sbyte SByte12;

        /// <summary>
        /// The 13th <see cref="sbyte"/>.
        /// </summary>
        [FieldOffset(13)]
        public readonly sbyte SByte13;

        /// <summary>
        /// The 14th <see cref="sbyte"/>.
        /// </summary>
        [FieldOffset(14)]
        public readonly sbyte SByte14;

        /// <summary>
        /// The 15th <see cref="sbyte"/>.
        /// </summary>
        [FieldOffset(15)]
        public readonly sbyte SByte15;

        /// <summary>
        /// Gets the data as an array of <see cref="sbyte"/>.
        /// </summary>
        /// <value>The sbytes.</value>
        [NotNull]
        public sbyte[] SBytes => new[] { SByte0, SByte1, SByte2, SByte3, SByte4, SByte5, SByte6, SByte7, SByte8, SByte9, SByte10, SByte11, SByte12, SByte13, SByte14, SByte15 };

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable16" /> struct.
        /// </summary>
        /// <param name="sbyte0">The 0th <see cref="sbyte"/>.</param>
        /// <param name="sbyte1">The 1st <see cref="sbyte"/>.</param>
        /// <param name="sbyte2">The 2nd <see cref="sbyte"/>.</param>
        /// <param name="sbyte3">The 3rd <see cref="sbyte"/>.</param>
        /// <param name="sbyte4">The 4th <see cref="sbyte"/>.</param>
        /// <param name="sbyte5">The 5th <see cref="sbyte"/>.</param>
        /// <param name="sbyte6">The 6th <see cref="sbyte"/>.</param>
        /// <param name="sbyte7">The 7th <see cref="sbyte"/>.</param>
        /// <param name="sbyte8">The 8th <see cref="sbyte"/>.</param>
        /// <param name="sbyte9">The 9th <see cref="sbyte"/>.</param>
        /// <param name="sbyte10">The 10th <see cref="sbyte"/>.</param>
        /// <param name="sbyte11">The 11th <see cref="sbyte"/>.</param>
        /// <param name="sbyte12">The 12th <see cref="sbyte"/>.</param>
        /// <param name="sbyte13">The 13th <see cref="sbyte"/>.</param>
        /// <param name="sbyte14">The 14th <see cref="sbyte"/>.</param>
        /// <param name="sbyte15">The 15th <see cref="sbyte"/>.</param>
        public Blittable16(
            sbyte sbyte0,
            sbyte sbyte1,
            sbyte sbyte2,
            sbyte sbyte3,
            sbyte sbyte4,
            sbyte sbyte5,
            sbyte sbyte6,
            sbyte sbyte7,
            sbyte sbyte8,
            sbyte sbyte9,
            sbyte sbyte10,
            sbyte sbyte11,
            sbyte sbyte12,
            sbyte sbyte13,
            sbyte sbyte14,
            sbyte sbyte15)
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
            SByte8 = sbyte8;
            SByte9 = sbyte9;
            SByte10 = sbyte10;
            SByte11 = sbyte11;
            SByte12 = sbyte12;
            SByte13 = sbyte13;
            SByte14 = sbyte14;
            SByte15 = sbyte15;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable16"/> struct from an array of <see cref="sbyte"/>
        /// </summary>
        /// <param name="sbytes">The sbytes array.</param>
        public Blittable16([NotNull] sbyte[] sbytes)
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
            SByte8 = sbytes[8];
            SByte9 = sbytes[9];
            SByte10 = sbytes[10];
            SByte11 = sbytes[11];
            SByte12 = sbytes[12];
            SByte13 = sbytes[13];
            SByte14 = sbytes[14];
            SByte15 = sbytes[15];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable16" /> struct from an array of <see cref="sbyte"/>.
        /// </summary>
        /// <param name="sbytes">The sbytes array.</param>
        /// <param name="offset">The offset into the array.</param>
        public Blittable16([NotNull] sbyte[] sbytes, long offset)
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
            SByte8 = sbytes[offset + 8];
            SByte9 = sbytes[offset + 9];
            SByte10 = sbytes[offset + 10];
            SByte11 = sbytes[offset + 11];
            SByte12 = sbytes[offset + 12];
            SByte13 = sbytes[offset + 13];
            SByte14 = sbytes[offset + 14];
            SByte15 = sbytes[offset + 15];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable16" /> struct from an array of <see cref="sbyte"/>.
        /// </summary>
        /// <param name="sbytes">The sbytes array.</param>
        /// <param name="offset">The offset into the array.</param>
        public Blittable16([NotNull] sbyte[] sbytes, ref long offset)
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
            SByte8 = sbytes[offset++];
            SByte9 = sbytes[offset++];
            SByte10 = sbytes[offset++];
            SByte11 = sbytes[offset++];
            SByte12 = sbytes[offset++];
            SByte13 = sbytes[offset++];
            SByte14 = sbytes[offset++];
            SByte15 = sbytes[offset++];
        }
        #endregion
    }
}