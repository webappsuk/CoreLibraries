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

namespace WebApplications.Utilities.Cryptography
{
#if false
    public class SecureIdentifier : IReadOnlyCollection<byte>, IComparable<SecureIdentifier>, IEquatable<SecureIdentifier>
    {
        /// <summary>
        /// The minimum length.
        /// </summary>
        [PublicAPI]
        public const byte MinLength = 12;

        /// <summary>
        /// The key used to validate hashes.
        /// </summary>
        [NotNull]
        private static readonly byte[] _key;

        /// <summary>
        /// The length of the identifier, in bytes.
        /// </summary>
        [PublicAPI]
        public readonly byte Length;

        /// <summary>
        /// The underlying bytes.
        /// </summary>
        [NotNull]
        private readonly byte[] _bytes;

        /// <summary>
        /// The base64 encoded string.
        /// </summary>
        [NotNull]
        private readonly string _base64Encoded;

        /// <summary>
        /// The hash code embedded into the byte array for consistency checking.
        /// </summary>
        private readonly int _hashCode;

        /// <summary>
        /// Initializes static members of the <see cref="SecureIdentifier"/> class.
        /// </summary>
        static SecureIdentifier()
        {
            // Load key from configuration
            CryptographyConfiguration cryptographyConfiguration = CryptographyConfiguration.Active;
            string key = cryptographyConfiguration.SecureIdentifierKey;

            if (string.IsNullOrWhiteSpace(key))
            {
                // Generate random key and attempt to save
                key = RandomKey();
                cryptographyConfiguration.SecureIdentifierKey = key;
            }

            // Update our key
            _key = new UTF8Encoding(false).GetBytes(key);
        }

        /// <summary>
        /// Generate a random unicode string of the specified <paramref name="length"/>.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns>A random unicode string.</returns>
        [PublicAPI]
        [NotNull]
        public static string RandomKey(byte length = 24)
        {
            StringBuilder builder = new StringBuilder(length);
            Random random = new Random();
            for (int c = 0; c < length; c++)
            {
                if (c < (length - 1) && random.NextDouble() < 0.1)
                {
                    builder.Append((char)random.Next(0xD800, 0xDBFF));
                    builder.Append((char)random.Next(0xDC00, 0xDFFF));
                    c++;
                    continue;
                }

                int character = random.Next(0xF7E1);
                switch (character)
                {
                    case 0:
                        character = 0x0009;
                        break;
                    case 1:
                        character = 0x000A;
                        break;
                    case 2:
                        character = 0x000D;
                        break;
                    default:
                        // Other valid characters are 0x0020-0xD7FF and 0xE000-0xFFFD:
                        character += character < 0xD7E3 ? 0x001D : 0x081D;
                        break;
                }
                builder.Append((char)character);
            }
            return builder.ToString();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SecureIdentifier" /> class.
        /// </summary>
        /// <param name="length">The length in bytes, defaults to 18 as this has no-padding when base64 encoded (multiple of 3) and has more resolution than
        /// a GUID (16 bytes).</param>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public SecureIdentifier(byte length = 18)
        {
            if (length < MinLength)
                throw new ArgumentOutOfRangeException(
                    nameof(length),
                    string.Format(Resources.SecureIdentifier_Ctor_Invalid_Length_Supplied, MinLength));
            
            // Guid.NewGuid and System.Random are not particularly random. By using a
            // cryptographically-secure random number generator, the caller is always
            // protected, regardless of use.
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                _bytes = new byte[length];
                rng.GetBytes(_bytes, 0, length - 4);

                //byte[] signed = _signer.Sign(_bytes, out _hashCode);

                _base64Encoded = Convert.ToBase64String(signed);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SecureIdentifier"/> class.
        /// This constructor is used internally for parse operations only.
        /// </summary>
        /// <param name="signer">The <see cref="IByteSigner"/> used to generate the verification signature in the identifier.</param>
        /// <param name="bytes">The random set of bytes which comprise this identifier.</param>
        /// <exception cref="System.ArgumentNullException">The bytes parameter cannot be null.</exception>
        /// <exception cref="System.ArgumentException">The bytes parameter was an array of bytes which failed consitency checks.</exception>
        private SecureIdentifier([NotNull]IByteSigner signer, byte[] bytes)
        {
            if (ReferenceEquals(signer, null))
                throw new ArgumentNullException(nameof(signer));

            if (ReferenceEquals(bytes, null))
                throw new ArgumentNullException(nameof(bytes));

            _signer = signer;
            _bytes = bytes;

            if (!_signer.Verify(bytes))
                throw new ArgumentException(Resources.SecureIdentifier_SecureIdentifier_Invalid_Byte_Array_Supplied, nameof(bytes));

            _base64Encoded = Convert.ToBase64String(_bytes);
        }

        /// <summary>
        /// Parses the specified value as a <see cref="SecureIdentifier"/>.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <param name="signer">The <see cref="IByteSigner"/> used to generate the verification signature in the identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"></exception>
        [PublicAPI]
        public static SecureIdentifier Parse([NotNull] string value, IByteSigner signer = null)
        {
            // Check we can base64 decode
            byte[] bytes = Convert.FromBase64String(value);

            // Default to SimplehashByteSigner
            if (ReferenceEquals(signer, null))
                signer = SimpleHashByteSigner.Instance;

            SecureIdentifier si;

            try
            {
                si = new SecureIdentifier(signer, bytes);
            }
            catch
            {
                throw new ArgumentException(Resources.SecureIdentifier_Parse_Invalid_SecureIdentifier_String_Supplied, nameof(value));
            }

            return si;
        }

        /// <summary>
        /// Tries the parse.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="result">The result. Out.</param>
        /// <returns></returns>
        [PublicAPI]
        public static bool TryParse([NotNull] string value, out SecureIdentifier result)
        {
            try
            {
                result = Parse(value);
            }
            catch
            {
                result = null;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the <see cref="System.Byte"/> at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="System.Byte"/>.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        /// <exception cref="System.IndexOutOfRangeException">The specified <paramref name="index" /> should be less than <see cref="Length"/>.</exception>
        [PublicAPI]
        public byte this[byte index]
        {
            get
            {
                if (index >= Length)
                    throw new IndexOutOfRangeException();

                return _bytes[index];
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<byte> GetEnumerator()
        {
            return ((IEnumerable<byte>)_bytes).GetEnumerator();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() => _base64Encoded;

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool Equals(SecureIdentifier other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Length == other.Length && _bytes.Equals(other._bytes) && string.Equals(_base64Encoded, other._base64Encoded) && _hashCode == other._hashCode;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((SecureIdentifier)obj);
        }

        public static bool operator ==(SecureIdentifier left, SecureIdentifier right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SecureIdentifier left, SecureIdentifier right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// Compares this SecureIdentifier to an object, which will be cast to a SecureIdentifier during the comparision.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        [PublicAPI]
        public int CompareTo(object other)
        {
            return CompareTo(other as SecureIdentifier);
        }

        /// <summary>
        /// Compares this SecureIdentifier to another.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        [PublicAPI]
        public int CompareTo(SecureIdentifier other)
        {
            if (ReferenceEquals(null, other)) return 1;
            if (Equals(this, other)) return 0;
            int lengthDiff = Length - other.Length;
            if (lengthDiff != 0) return lengthDiff;

            for (byte i = 0; i < Length; i++)
            {
                int compareResult = this[i].CompareTo(other[i]);
                if (compareResult != 0) return compareResult;
            }

            //Should never really reach here. Would mean equal bytes but different hashes.
            return 0;
        }

        public static bool operator >=(SecureIdentifier left, SecureIdentifier right)
        {
            if (ReferenceEquals(left, null)) return true;
            return (left.CompareTo(right) >= 0);
        }

        public static bool operator <=(SecureIdentifier left, SecureIdentifier right)
        {
            return ReferenceEquals(left, null)
                ? ReferenceEquals(right, null)
                : left.CompareTo(right) <= 0;
        }

        public static bool operator >(SecureIdentifier left, SecureIdentifier right)
        {
            return ReferenceEquals(left, null)
                ? !ReferenceEquals(right, null)
                : left.CompareTo(right) > 0;
        }

        public static bool operator <(SecureIdentifier left, SecureIdentifier right)
        {
            return !ReferenceEquals(left, null) && left.CompareTo(right) < 0;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode() => _hashCode;

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Gets the count, (which is the same as <see cref="Length"/>.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        int IReadOnlyCollection<byte>.Count => Length;

        /// <summary>
        /// Implements the operator implicit string.
        /// </summary>
        /// <param name="secureIdentifier">The secure identifier.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        [NotNull]
        public static implicit operator string ([NotNull] SecureIdentifier secureIdentifier) => secureIdentifier._base64Encoded;

        /// <summary>
        /// Implements the operator explicit SecureIdentifier.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        [CanBeNull]
        public static implicit operator SecureIdentifier([NotNull] string value) => Parse(value);
    }
#endif
}