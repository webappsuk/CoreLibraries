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

#region Using Namespaces
using System;
using System.Runtime.Serialization;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Cryptography.Configuration;

#endregion

namespace WebApplications.Utilities.Cryptography
{
    /// <summary>
    /// Wraps together encrypted and unencrypted versions of a string, only encrypting/decrypting when necessary.
    /// </summary>
    [Serializable]
    [PublicAPI]
    [Obsolete("This is legacy code and is of dubious value.")]
    public class EncryptedString : ISerializable, IEquatable<string>, IEquatable<EncryptedString>
    {
        /// <summary>
        /// Serialization tag.
        /// </summary>
        private const string TagID = "ID";

        /// <summary>
        /// Serialization tag.
        /// </summary>
        private const string TagHidden = "Hidden";

        /// <summary>
        /// Serialization tag.
        /// </summary>
        private const string TagValue = "Value";

        [NotNull]
        private readonly CryptographyProvider _provider;

        /// <summary>
        /// Flag indicating which value (if either) is out of date
        /// </summary>
        private Dirty _dirty = Dirty.Neither;

        /// <summary>
        /// The encrypted value.
        /// </summary>
        private string _encrypted;

        /// <summary>
        /// The unencrypted value.
        /// </summary>
        private string _unencrypted;

        /// <summary>
        /// Initializes a new instance of the <see cref="EncryptedString"/> class.
        /// </summary>
        /// <param name="provider">The cryptography provider.</param>
        /// <param name="value">The value.</param>
        /// <param name="isEncrypted">if set to <see langword="true"/> is encrypted.</param>
        /// <param name="isHidden">if set to <see langword="true"/> is hidden.</param>
        /// <exception cref="ArgumentNullException"><paramref name="provider"/> is <see langword="null" />.</exception>
        public EncryptedString(
            [NotNull] CryptographyProvider provider,
            string value = null,
            bool isEncrypted = false,
            bool isHidden = false)
        {
            if (provider == null) throw new ArgumentNullException(nameof(provider));

            _provider = provider;
            if (isEncrypted)
                Encrypted = value;
            else
                Unencrypted = value;
            Hidden = isHidden;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EncryptedString"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        private EncryptedString([NotNull] SerializationInfo info, StreamingContext context)
        {
            string providerId = info.GetString(TagID);
            CryptographyProvider provider = CryptographyConfiguration.Active.Provider(providerId);

            if (provider == null)
                throw new SerializationException(
                    string.Format(Resources.EncryptedString_EncryptedString_InvalidProviderId, providerId));

            _provider = provider;
            Encrypted = info.GetString(TagValue);
            Hidden = info.GetBoolean(TagHidden);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="EncryptedString"/> is hidden.
        /// Setting this to <see langword="true"/> prevents the unencrypted value being output by
        /// the <see cref="ToString"/> method.  It should also be respected by your own code.
        /// </summary>
        /// <value>
        /// 	<see langword="true"/> if hidden; otherwise, <see langword="false"/>.
        /// </value>
        public bool Hidden { get; set; }

        /// <summary>
        /// Gets or sets the encrypted version of the string.
        /// </summary>
        /// <value>The encrypted version of the string.</value>
        public string Encrypted
        {
            get
            {
                if (_dirty == Dirty.Encrypted)
                {
                    _encrypted = !string.IsNullOrEmpty(_unencrypted)
                        ? _provider.EncryptToString(_unencrypted)
                        : _unencrypted;
                    _dirty = Dirty.Neither;
                }
                return _encrypted;
            }
            set
            {
                if (Equals(_encrypted, value))
                    return;

                _encrypted = value;
                if (string.IsNullOrEmpty(value))
                {
                    _unencrypted = value;
                    _dirty = Dirty.Neither;
                }
                else
                    _dirty = Dirty.Unencrypted;
            }
        }

        /// <summary>
        /// Gets or sets the unencrypted version of the string.
        /// </summary>
        /// <value>The unencrypted version of the string.</value>
        public string Unencrypted
        {
            get
            {
                if (_dirty == Dirty.Unencrypted)
                {
                    _unencrypted = !string.IsNullOrEmpty(_encrypted)
                        ? _provider.DecryptFromStringToString(_encrypted)
                        : _encrypted;
                    _dirty = Dirty.Neither;
                }
                return _unencrypted;
            }
            set
            {
                if (Equals(_unencrypted, value))
                    return;
                _unencrypted = value;
                if (string.IsNullOrEmpty(value))
                {
                    _encrypted = value;
                    _dirty = Dirty.Neither;
                }
                else
                    _dirty = Dirty.Encrypted;
            }
        }

        #region IEquatable<EncryptedString> Members
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false"/>.
        /// </returns>
        /// <param name="other">
        /// An object to compare with this object.
        /// </param>
        public bool Equals([CanBeNull] EncryptedString other)
        {
            if (other == null)
                return false;
            if ((_dirty != other._dirty) ||
                (_dirty != Dirty.Encrypted))
                return Equals(other.Unencrypted);
            return Equals(_encrypted, other._encrypted);
        }
        #endregion

        #region IEquatable<string> Members
        /// <summary>
        ///                     Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false"/>.
        /// </returns>
        /// <param name="other">
        ///                     An object to compare with this object.
        ///                 </param>
        public bool Equals([CanBeNull] string other)
        {
            return Equals(Unencrypted, other);
        }
        #endregion

        #region ISerializable Members        
        /// <inheritdoc />
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (string.IsNullOrEmpty(_provider.Id))
                throw new SerializationException(Resources.EncryptedString_GetObjectData_No_Provider_Id);

            info.AddValue(TagID, _provider.Id);
            info.AddValue(TagValue, Encrypted);
            info.AddValue(TagHidden, Hidden);
        }
        #endregion

        /// <summary>
        /// Returns a <see cref="String"/> that represents the current <see cref="EncryptedString"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="String"/> that represents the current <see cref="EncryptedString"/>.
        /// </returns>
        public override string ToString()
        {
            return Hidden ? "Encrypted String is hidden." : Unencrypted ?? "";
        }

        #region Nested type: Dirty
        /// <summary>
        /// Internal enumeration used to indicate which value is out of date.
        /// </summary>
        private enum Dirty
        {
            /// <summary>
            /// Encrypted value is out of date.
            /// </summary>
            Encrypted,

            /// <summary>
            /// Unencrypted value is out of date.
            /// </summary>
            Unencrypted,

            /// <summary>
            /// Both values are up to date.
            /// </summary>
            Neither
        }
        #endregion
    }
}