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
    public struct Blittable2
    {
        #region UShort
        /// <summary>
        /// The ushort value.
        /// </summary>
        [FieldOffset(0)]
        public readonly ushort UShort;

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable2"/> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public Blittable2(ushort value)
            : this()
        {
            UShort = value;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="ushort"/> to <see cref="Blittable2"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Blittable2(ushort value) => new Blittable2(value);

        /// <summary>
        /// Performs an implicit conversion from <see cref="Blittable2"/> to <see cref="ushort"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator ushort(Blittable2 value) => value.UShort;
        #endregion

        #region Short
        /// <summary>
        /// The short value.
        /// </summary>
        [FieldOffset(0)]
        public readonly short Short;

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable2"/> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public Blittable2(short value)
            : this()
        {
            Short = value;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="short"/> to <see cref="Blittable2"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Blittable2(short value) => new Blittable2(value);

        /// <summary>
        /// Performs an implicit conversion from <see cref="Blittable2"/> to <see cref="short"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator short(Blittable2 value) => value.Short;
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
        /// Gets the data as an array of <see cref="byte"/>.
        /// </summary>
        /// <value>The bytes.</value>
        [NotNull]
        public byte[] Bytes => new[] { Byte0, Byte1 };

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable2" /> struct.
        /// </summary>
        /// <param name="byte0">The 0th <see cref="byte"/>.</param>
        /// <param name="byte1">The 1st <see cref="byte"/>.</param>
        public Blittable2(
            byte byte0,
            byte byte1)
            : this()
        {
            Byte0 = byte0;
            Byte1 = byte1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable2"/> struct from an array of <see cref="byte"/>
        /// </summary>
        /// <param name="bytes">The bytes array.</param>
        public Blittable2([NotNull] byte[] bytes)
            :this()
        {
            Byte0 = bytes[0];
            Byte1 = bytes[1];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable2" /> struct from an array of <see cref="byte"/>.
        /// </summary>
        /// <param name="bytes">The bytes array.</param>
        /// <param name="offset">The offset into the array.</param>
        public Blittable2([NotNull] byte[] bytes, long offset)
            : this()
        {
            Byte0 = bytes[offset];
            Byte1 = bytes[offset + 1];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable2" /> struct from an array of <see cref="byte"/>.
        /// </summary>
        /// <param name="bytes">The bytes array.</param>
        /// <param name="offset">The offset into the array.</param>
        public Blittable2([NotNull] byte[] bytes, ref long offset)
            : this()
        {
            Byte0 = bytes[offset++];
            Byte1 = bytes[offset++];
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
        /// Gets the data as an array of <see cref="sbyte"/>.
        /// </summary>
        /// <value>The sbytes.</value>
        [NotNull]
        public sbyte[] SBytes => new[] { SByte0, SByte1 };

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable2" /> struct.
        /// </summary>
        /// <param name="sbyte0">The 0th <see cref="sbyte"/>.</param>
        /// <param name="sbyte1">The 1st <see cref="sbyte"/>.</param>
        public Blittable2(
            sbyte sbyte0,
            sbyte sbyte1)
            : this()
        {
            SByte0 = sbyte0;
            SByte1 = sbyte1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable2"/> struct from an array of <see cref="sbyte"/>
        /// </summary>
        /// <param name="sbytes">The sbytes array.</param>
        public Blittable2([NotNull] sbyte[] sbytes)
            : this()
        {
            SByte0 = sbytes[0];
            SByte1 = sbytes[1];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable2" /> struct from an array of <see cref="sbyte"/>.
        /// </summary>
        /// <param name="sbytes">The sbytes array.</param>
        /// <param name="offset">The offset into the array.</param>
        public Blittable2([NotNull] sbyte[] sbytes, long offset)
            : this()
        {
            SByte0 = sbytes[offset];
            SByte1 = sbytes[offset + 1];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Blittable2" /> struct from an array of <see cref="sbyte"/>.
        /// </summary>
        /// <param name="sbytes">The sbytes array.</param>
        /// <param name="offset">The offset into the array.</param>
        public Blittable2([NotNull] sbyte[] sbytes, ref long offset)
            : this()
        {
            SByte0 = sbytes[offset++];
            SByte1 = sbytes[offset++];
        }
        #endregion
    }
}