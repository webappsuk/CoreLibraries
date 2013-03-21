#region © Copyright Web Applications (UK) Ltd, 2013.  All rights reserved.
// Copyright (c) 2013, Web Applications UK Ltd
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;

namespace WebApplications.Utilities
{
    /// <summary>
    /// Holds a readonly byte array with an associated hash for rapid true equality comparison and dictionary
    /// insertion.
    /// </summary>
    public class HashedByteArray : IEquatable<HashedByteArray>, IEquatable<byte[]>, IEnumerable<byte>
    {
        [NotNull] private readonly byte[] _data;

        [NotNull] private readonly Lazy<string> _encoded;

        /// <summary>
        /// Holds the hash for the associated data.
        /// </summary>
        private readonly long _hash;

        /// <summary>
        /// Initializes a new instance of the <see cref="HashedByteArray"/> class from a base-64 encoded string.
        /// </summary>
        /// <param name="encoded">The base-64 encoded string.</param>
        public HashedByteArray([NotNull] string encoded)
            : this(Convert.FromBase64String(encoded), new Lazy<string>(() => encoded, LazyThreadSafetyMode.None))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HashedByteArray"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        public HashedByteArray([NotNull] byte[] data)
            : this(
                data, new Lazy<string>(() => Convert.ToBase64String(data), LazyThreadSafetyMode.ExecutionAndPublication)
                )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HashedByteArray"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        private HashedByteArray([NotNull] byte[] data, [NotNull] Lazy<string> encoded)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            _encoded = encoded;

            _data = data;
            if (data.Length > 8)
            {
                unchecked
                {
                    _hash = _data.Skip(data.Length - 8).Aggregate(0L, (h, b) => (b*397) ^ h);
                }
            }
            else if (data.Length > 4)
            {
                if (data.Length != 8)
                    Array.Resize(ref data, 8);
                _hash = BitConverter.ToInt64(data, 0);
            }
            else if (data.Length > 2)
            {
                if (data.Length != 4)
                    Array.Resize(ref data, 4);
                _hash = BitConverter.ToInt32(data, 0);
            }
            else if (data.Length > 1)
                _hash = BitConverter.ToInt16(data, 0);
            else if (data.Length > 0)
                _hash = data[0];
        }

        /// <summary>
        /// Gets the encoded version of the byte[].
        /// </summary>
        /// <value>The encoded.</value>
        [NotNull]
        public string Encoded
        {
            get { return _encoded.Value; }
        }

        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value>The length.</value>
        public int Length
        {
            get { return _data.Length; }
        }

        /// <summary>
        /// Gets the long length.
        /// </summary>
        /// <value>The length.</value>
        public long LongLength
        {
            get { return _data.LongLength; }
        }

        /// <summary>
        /// Allows retrieval of the bytes from the byte array.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public byte this[int index]
        {
            get { return _data[index]; }
        }

        #region IEquatable<byte[]> Members
        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public bool Equals(byte[] other)
        {
            return !ReferenceEquals(other, null) && _data.SequenceEqual(other);
        }
        #endregion

        #region IEquatable<HashedByteArray> Members
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals(HashedByteArray other)
        {
            if (ReferenceEquals(other, null)) return false;
            return (_hash == other._hash) &&
                   (_data.Length == other._data.Length) &&
                   ((_data.Length < 9) || _data.SequenceEqual(other._data));
        }
        #endregion

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        public IEnumerator<byte> GetEnumerator()
        {
            return _data.Select(b => b).GetEnumerator();
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (int) _hash;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><see langword="true" /> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <see langword="false" />.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null)) return false;
            HashedByteArray other = obj as HashedByteArray;
            if (!ReferenceEquals(other, null))
            {
                return (_hash == other._hash) &&
                       (_data.Length == other._data.Length) &&
                       ((_data.Length < 9) || _data.SequenceEqual(other._data));
            }

            byte[] bytes = obj as byte[];
            return !ReferenceEquals(bytes, null) && _data.SequenceEqual(bytes);
        }

        /// <summary>
        /// Implements the ==.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(HashedByteArray a, HashedByteArray b)
        {
            if (ReferenceEquals(a, null))
                return ReferenceEquals(b, null);
            if (ReferenceEquals(b, null)) return false;
            return (a._hash == b._hash) &&
                   (a._data.Length == b._data.Length) &&
                   ((a._data.Length < 9) || a._data.SequenceEqual(b._data));
        }

        /// <summary>
        /// Implements the !=.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(HashedByteArray a, HashedByteArray b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Byte[][]"/> to <see cref="HashedByteArray"/>.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator HashedByteArray(byte[] bytes)
        {
            return ReferenceEquals(null, bytes) ? null : new HashedByteArray(bytes);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="HashedByteArray"/> to <see cref="System.Byte[][]"/>.
        /// </summary>
        /// <param name="hashedByteArray">The hashed byte array.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator byte[](HashedByteArray hashedByteArray)
        {
            if (ReferenceEquals(null, hashedByteArray))
                return null;

            long length = hashedByteArray._data.LongLength;
            byte[] clone = new byte[length];
            Array.Copy(hashedByteArray._data, clone, length);
            return clone;
        }
    }
}