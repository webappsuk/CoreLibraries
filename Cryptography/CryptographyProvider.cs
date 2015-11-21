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
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Cryptography
{
    /// <summary>
    /// Base class for all cryptography providers.
    /// </summary>
    public abstract class CryptographyProvider
    {
        /// <summary>
        /// <see langword="true"/> if the cryptography provider preserves length during encryption/decryption.
        /// </summary>
        /// <remarks>
        /// <para>When using the <see cref="GetEncryptionStream"/>, <see cref="GetDecryptionStream"/>,
        /// <see cref="GetEncryptor"/> and/or <see cref="GetDecryptor"/> methods to perform encryption/decryption, care
        /// should be taken to observe the value of this flag, as the encrypted/decrypted stream may not be the same length as the
        /// original input stream, and may have trailing zero bytes.</para>
        /// <para>All other encryption/decryption methods respect this flag and will automatically mark the end of an
        /// input stream.</para>
        /// </remarks>
        [PublicAPI]
        public readonly bool PreservesLength;

        /// <summary>
        /// The name of the provider.
        /// </summary>
        [PublicAPI]
        [NotNull]
        public readonly string Name;

        /// <summary>
        /// The configuration as a string.
        /// </summary>
        [NotNull]
        private readonly string _configuration;

        /// <summary>
        /// The configuration <see cref="XElement"/>, if any.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public XElement Configuration => XElement.Parse(_configuration, LoadOptions.PreserveWhitespace);

        /// <summary>
        /// Whether the provider can decrypt.
        /// </summary>
        /// <remarks>
        /// <para><see langword="false"/> when only the public key is available for an asymmetric algorithm.</para>
        /// </remarks>
        [PublicAPI]
        public virtual bool CanEncrypt => false;

        /// <summary>
        /// Whether the provider can decrypt.
        /// </summary>
        /// <remarks>
        /// <para><see langword="false"/> for hashing algorithms.</para>
        /// </remarks>
        [PublicAPI]
        public virtual bool CanDecrypt => false;

        /// <summary>
        /// Initializes a new instance of the <see cref="CryptographyProvider" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="configuration">The configuration (if any).</param>
        /// <param name="preservesLength">
        ///   <see langword="true" /> if the provider preserves the length.</param>
        protected CryptographyProvider(
            [NotNull] string name,
            [NotNull] XElement configuration,
            bool preservesLength = true)
        {
            Name = name;
            _configuration = configuration.ToString(SaveOptions.DisableFormatting);
            PreservesLength = preservesLength;
        }

        /// <summary>
        /// Gets the identifier, if the provider is persisted in the configuration; otherwise <see langword="null" />.
        /// </summary>
        /// <value>The identifier.</value>
        [PublicAPI]
        [CanBeNull]
        public string Id { get; internal set; }

        /// <summary>
        /// Gets the <see cref="ICryptoTransform"/> for encryption.
        /// </summary>
        /// <returns>An <see cref="ICryptoTransform"/>.</returns>
        [PublicAPI]
        [NotNull]
        public abstract ICryptoTransform GetEncryptor();

        /// <summary>
        /// Gets the <see cref="ICryptoTransform"/> for decryption.
        /// </summary>
        /// <returns>An <see cref="ICryptoTransform"/>.</returns>
        [PublicAPI]
        [NotNull]
        public abstract ICryptoTransform GetDecryptor();

        #region Encryption overloads
        /// <summary>
        /// Encrypts the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="preserveLength">If <see langword="true"/> preserves the length of the input when decrypting;
        /// otherwise, if <see langword="false"/> the length may be wrong on decryption, if the provider does not
        /// automatically <see cref="PreservesLength">preserve length</see> itself.</param>
        /// <returns>A base 64 encoded string of the encrypted data.</returns>
        /// <exception cref="CryptographicException">The cryptographic provider cannot perform encryption.</exception>
        [PublicAPI]
        [ContractAnnotation("input:null => null; input:notnull => notnull")]
        public string EncryptToString(string input, bool preserveLength = true)
        {
            if (!CanEncrypt) throw new CryptographicException("The cryptographic provider cannot perform encryption.");
            return input == null ? null : Convert.ToBase64String(Encrypt(Encoding.Unicode.GetBytes(input), preserveLength));
        }

        /// <summary>
        /// Encrypts the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="preserveLength">If <see langword="true"/> preserves the length of the input when decrypting;
        /// otherwise, if <see langword="false"/> the length may be wrong on decryption, if the provider does not
        /// automatically <see cref="PreservesLength">preserve length</see> itself.</param>
        /// <returns>A base 64 encoded string of the encrypted data.</returns>
        /// <exception cref="CryptographicException">The cryptographic provider cannot perform encryption.</exception>
        [PublicAPI]
        [ContractAnnotation("input:null => null; input:notnull => notnull")]
        public byte[] Encrypt(string input, bool preserveLength = true)
        {
            if (!CanEncrypt) throw new CryptographicException("The cryptographic provider cannot perform encryption.");
            return input == null ? null : Encrypt(Encoding.Unicode.GetBytes(input), preserveLength);
        }

        /// <summary>
        /// Encrypts the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="preserveLength">If <see langword="true"/> preserves the length of the input when decrypting;
        /// otherwise, if <see langword="false"/> the length may be wrong on decryption, if the provider does not
        /// automatically <see cref="PreservesLength">preserve length</see> itself.</param>
        /// <returns>The encrypted data.</returns>
        /// <exception cref="CryptographicException">The cryptographic provider cannot perform encryption.</exception>
        [PublicAPI]
        [ContractAnnotation("input:null => null; input:notnull => notnull")]
        public byte[] Encrypt(byte[] input, bool preserveLength = true)
        {
            if (!CanEncrypt) throw new CryptographicException("The cryptographic provider cannot perform encryption.");
            if (input == null) return null;
            using (MemoryStream stream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = GetEncryptionStream(stream))
                {
                    cryptoStream.Write(input, 0, input.Length);

                    // Add termination byte if the provider does not respect the length.
                    if (preserveLength && !PreservesLength)
                        cryptoStream.WriteByte(0xFF);

                    cryptoStream.Clear();
                }

                return stream.ToArray();
            }
        }

        /// <summary>
        /// Gets the encryption stream.
        /// </summary>
        /// <param name="outputStream">The output stream.</param>
        /// <returns>A <see cref="CryptoStream" /> that can be used to write encrypted data from the <paramref name="outputStream" />.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="outputStream"/> is <see langword="null" />.</exception>
        /// <exception cref="CryptographicException">The cryptographic provider cannot perform encryption.</exception>
        /// <remarks>
        /// <para>To encrypt data, write it to the <see cref="CryptoStream"/> and read from the <paramref name="outputStream"/>.</para>
        /// <para>When using a provider that does not <see cref="PreservesLength">preserve the length</see> you may want to consider
        /// adding a termination byte (e.g. 0xFF) to help detect the end of the stream.</para>
        /// </remarks>
        [NotNull]
        [PublicAPI]
        public CryptoStream GetEncryptionStream([NotNull] Stream outputStream)
        {
            if (outputStream == null) throw new ArgumentNullException(nameof(outputStream));
            if (!CanEncrypt) throw new CryptographicException("The cryptographic provider cannot perform encryption.");
            return new CryptoStream(outputStream, GetEncryptor(), CryptoStreamMode.Write);
        }
        #endregion

        #region Decryption overloads
        /// <summary>
        /// Decrypts the specified byte[] to a string.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="preserveLength"><para>If <see langword="true"/>, and the provider doesn't 
        /// <see cref="PreservesLength">preserve length</see> itself, then the output will be truncated based on the
        /// presence of a termination byte (0xFF) followed by only zero-bytes.</para>
        /// <para>
        /// It is vital that this flag is set the same for encryption and decryption.
        /// </para></param>
        /// <returns>The decrypted string.</returns>
        /// <exception cref="CryptographicException">The cryptographic provider cannot perform decryption.</exception>
        [PublicAPI]
        [ContractAnnotation("input:null => null; input:notnull => notnull")]
        public string DecryptToString(byte[] input, bool preserveLength = true)
        {
            if (!CanDecrypt) throw new CryptographicException(Resources.CryptographyProvider_DecryptToString_Decryption_Unsupported);
            return input == null ? null : Encoding.Unicode.GetString(Decrypt(input, preserveLength));
        }

        /// <summary>
        /// Decrypts the specified base 64 input string to a string.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="preserveLength"><para>If <see langword="true"/>, and the provider doesn't 
        /// <see cref="PreservesLength">preserve length</see> itself, then the output will be truncated based on the
        /// presence of a termination byte (0xFF) followed by only zero-bytes.</para>
        /// <para>
        /// It is vital that this flag is set the same for encryption and decryption.
        /// </para></param>
        /// <returns>The decrypted string.</returns>
        /// <exception cref="CryptographicException">The cryptographic provider cannot perform decryption.</exception>
        [PublicAPI]
        [ContractAnnotation("input:null => null; input:notnull => notnull")]
        public string DecryptToString(string input, bool preserveLength = true)
        {
            if (!CanDecrypt) throw new CryptographicException(Resources.CryptographyProvider_DecryptToString_Decryption_Unsupported);
            return input == null ? null : Encoding.Unicode.GetString(Decrypt(Convert.FromBase64String(input), preserveLength));
        }

        /// <summary>
        /// Decrypts the specified base 64 input string to a string.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="preserveLength"><para>If <see langword="true"/>, and the provider doesn't 
        /// <see cref="PreservesLength">preserve length</see> itself, then the output will be truncated based on the
        /// presence of a termination byte (0xFF) followed by only zero-bytes.</para>
        /// <para>
        /// It is vital that this flag is set the same for encryption and decryption.
        /// </para></param>
        /// <returns>The decrypted data.</returns>
        /// <exception cref="CryptographicException">The cryptographic provider cannot perform decryption.</exception>
        [PublicAPI]
        [ContractAnnotation("input:null => null; input:notnull => notnull")]
        public byte[] Decrypt(string input, bool preserveLength = true)
        {
            if (!CanDecrypt) throw new CryptographicException(Resources.CryptographyProvider_DecryptToString_Decryption_Unsupported);
            return input == null ? null : Decrypt(Convert.FromBase64String(input), preserveLength);
        }

        /// <summary>
        /// Tries to decrypt the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="output">The decrypted output.</param>
        /// <param name="preserveLength"><para>If <see langword="true"/>, and the provider doesn't 
        /// <see cref="PreservesLength">preserve length</see> itself, then the output will be truncated based on the
        /// presence of a termination byte (0xFF) followed by only zero-bytes.</para>
        /// <para>
        /// It is vital that this flag is set the same for encryption and decryption.
        /// </para></param>
        /// <returns><see langword="true"/> if succeeded; otherwise <see langword="false"/>.</returns>
        [PublicAPI]
        [ContractAnnotation("input:null=>true,output:null; true<=input:notnull, output:notnull; false<=output:null")]
        public bool TryDecryptToString(byte[] input, out string output, bool preserveLength = true)
        {
            if (input == null)
            {
                output = null;
                return true;
            }
            try
            {
                output = Encoding.Unicode.GetString(Decrypt(input, preserveLength));
                return true;
            }
            // ReSharper disable once CatchAllClause
            catch (Exception)
            {
                output = null;
                return false;
            }
        }

        /// <summary>
        /// Tries to decrypt the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="output">The decrypted output.</param>
        /// <param name="preserveLength"><para>If <see langword="true"/>, and the provider doesn't 
        /// <see cref="PreservesLength">preserve length</see> itself, then the output will be truncated based on the
        /// presence of a termination byte (0xFF) followed by only zero-bytes.</para>
        /// <para>
        /// It is vital that this flag is set the same for encryption and decryption.
        /// </para></param>
        /// <returns><see langword="true"/> if succeeded; otherwise <see langword="false"/>.</returns>
        [PublicAPI]
        [ContractAnnotation("input:null=>true,output:null; true<=input:notnull, output:notnull; false<=output:null")]
        public bool TryDecryptToString(string input, out string output, bool preserveLength = true)
        {
            if (input == null)
            {
                output = null;
                return true;
            }
            try
            {
                output = Encoding.Unicode.GetString(Decrypt(Convert.FromBase64String(input), preserveLength));
                return true;
            }
                // ReSharper disable once CatchAllClause
            catch (Exception)
            {
                output = null;
                return false;
            }
        }

        /// <summary>
        /// Tries to decrypt the specified base-64 encoded input string.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="output">The output.</param>
        /// <param name="preserveLength"><para>If <see langword="true"/>, and the provider doesn't 
        /// <see cref="PreservesLength">preserve length</see> itself, then the output will be truncated based on the
        /// presence of a termination byte (0xFF) followed by only zero-bytes.</para>
        /// <para>
        /// It is vital that this flag is set the same for encryption and decryption.
        /// </para></param>
        /// <returns><see langword="true"/> if succeeded; otherwise <see langword="false"/>.</returns>
        [PublicAPI]
        [ContractAnnotation("input:null=>true,output:null; true<=input:notnull, output:notnull; false<=output:null")]
        public bool TryDecrypt(string input, out byte[] output, bool preserveLength = true)
        {
            if (!CanDecrypt)
            {
                output = null;
                return false;
            }
            if (input == null)
            {
                output = null;
                return true;
            }
            try
            {
                output = Decrypt(Convert.FromBase64String(input), preserveLength);
                return true;
            }
                // ReSharper disable once CatchAllClause
            catch (Exception)
            {
                output = null;
                return false;
            }
        }

        /// <summary>
        /// Tries to decrypt the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="output">The output.</param>
        /// <param name="preserveLength"><para>If <see langword="true"/>, and the provider doesn't 
        /// <see cref="PreservesLength">preserve length</see> itself, then the output will be truncated based on the
        /// presence of a termination byte (0xFF) followed by only zero-bytes.</para>
        /// <para>
        /// It is vital that this flag is set the same for encryption and decryption.
        /// </para></param>
        /// <returns><see langword="true"/> if succeeded; otherwise <see langword="false"/>.</returns>
        [PublicAPI]
        [ContractAnnotation("input:null=>true,output:null; true<=input:notnull, output:notnull; false<=output:null")]
        public bool TryDecrypt(byte[] input, out byte[] output, bool preserveLength = true)
        {
            if (!CanDecrypt)
            {
                output = null;
                return false;
            }
            if (input == null)
            {
                output = null;
                return true;
            }
            try
            {
                output = Decrypt(input, preserveLength);
                return true;
            }
                // ReSharper disable once CatchAllClause
            catch (Exception)
            {
                output = null;
                return false;
            }
        }

        /// <summary>
        /// Decrypts the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="preserveLength"><para>If <see langword="true"/>, and the provider doesn't 
        /// <see cref="PreservesLength">preserve length</see> itself, then the output will be truncated based on the
        /// presence of a termination byte (0xFF) followed by only zero-bytes.</para>
        /// <para>
        /// It is vital that this flag is set the same for encryption and decryption.
        /// </para></param>
        /// <returns>The decrypted data.</returns>
        /// <exception cref="CryptographicException">The cryptographic provider cannot perform decryption.</exception>
        [PublicAPI]
        [ContractAnnotation("input:null => null; input:notnull => notnull")]
        public byte[] Decrypt(byte[] input, bool preserveLength = true)
        {
            if (!CanDecrypt) throw new CryptographicException("The cryptographic provider cannot perform encryption.");
            if (input == null) return null;

            // Guestimate buffer size, based on input size.
            ulong lbs = (ulong)(input.LongLength * 1.5);
            int bufferSize = lbs > 1024 ? 1024 : (int)lbs;
            if (bufferSize < 128) bufferSize = 128;

            byte[] inputBuffer = new byte[bufferSize];
            byte[] outputBuffer;
            using (MemoryStream stream = new MemoryStream(input))
            using (CryptoStream cryptoStream = GetDecryptionStream(stream))
            {
                using (MemoryStream output = new MemoryStream())
                {
                    int read;
                    while ((read = cryptoStream.Read(inputBuffer, 0, inputBuffer.Length)) > 0)
                        output.Write(inputBuffer, 0, read);
                    outputBuffer = output.ToArray();
                }
                cryptoStream.Clear();
            }

            if (preserveLength && !PreservesLength)
            {
                int length = outputBuffer.Length;
                do
                {
                } while (outputBuffer[--length] < 1);

                // Check last byte is 0xFF
                if (length >= 0 && outputBuffer[length] == 0xFF)
                    Array.Resize(ref outputBuffer, length);
            }

            return outputBuffer;
        }

        /// <summary>
        /// Gets the decryption stream.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <returns>A <see cref="CryptoStream" /> that can be used to read decrypted data from the <paramref name="inputStream" />.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="inputStream"/> is <see langword="null" />.</exception>
        /// <exception cref="CryptographicException">The cryptographic provider cannot perform decryption.</exception>
        /// <remarks>
        /// <para>To decrypt data, write it to the <paramref name="inputStream"/> and read it from the <see cref="CryptoStream"/>.</para>
        /// <para>When using a provider that does not <see cref="PreservesLength">preserve the length</see> you may want to consider
        /// adding a termination byte (e.g. 0xFF) to help detect the end of the stream.</para>
        /// </remarks>
        [NotNull]
        [PublicAPI]
        public CryptoStream GetDecryptionStream([NotNull] Stream inputStream)
        {
            if (inputStream == null) throw new ArgumentNullException(nameof(inputStream));
            if (!CanDecrypt) throw new CryptographicException(Resources.CryptographyProvider_DecryptToString_Decryption_Unsupported);
            return new CryptoStream(inputStream, GetDecryptor(), CryptoStreamMode.Read);
        }
        #endregion

        /// <summary>
        /// Creates a <see cref="CryptographyProvider" /> from a name. See https://msdn.microsoft.com/en-us/library/system.security.cryptography.cryptoconfig(v=vs.110).aspx for details.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>A <see cref="CryptographyProvider" />.</returns>
        /// <exception cref="TargetInvocationException">The algorithm described by the <paramref name="name" /> parameter was used with Federal Information Processing Standards (FIPS) mode enabled, but is not FIPS compatible.</exception>
        [NotNull]
        [PublicAPI]
        public static CryptographyProvider Create([NotNull] string name)
            => Create(null, name);

        /// <summary>
        /// Creates a <see cref="CryptographyProvider" /> from a name. See https://msdn.microsoft.com/en-us/library/system.security.cryptography.cryptoconfig(v=vs.110).aspx for details.
        /// </summary>
        /// <param name="configuration">The configuration element.</param>
        /// <param name="name">The name.</param>
        /// <returns>A <see cref="CryptographyProvider" />.</returns>
        /// <exception cref="TargetInvocationException">The algorithm described by the <paramref name="name" /> parameter was used with Federal Information Processing Standards (FIPS) mode enabled, but is not FIPS compatible.</exception>
        [NotNull]
        [PublicAPI]
        public static CryptographyProvider Create(
            [CanBeNull] XElement configuration,
            [NotNull] string name)
        {
            using (IDisposable provider = (IDisposable)CryptoConfig.CreateFromName(name))
            {
                AsymmetricAlgorithm asymm = provider as AsymmetricAlgorithm;
                if (asymm != null) return AsymmetricCryptographyProvider.Create(asymm, configuration);

                SymmetricAlgorithm sym = provider as SymmetricAlgorithm;
                if (sym != null) return SymmetricCryptographyProvider.Create(name, sym, configuration);

                HashAlgorithm hash = provider as HashAlgorithm;
                // TODO if (hash != null) return HashingCryptographyProvider.Create(hash, configuration);

                RandomNumberGenerator rnd = provider as RandomNumberGenerator;
                // TODO if (rnd != null) return RandomCryptographyProvider.Create(rnd, configuration);

                throw new CryptographicException(
                    string.Format(Resources.CryptographyProvider_Create_Unknown_Provider, name));
            }
        }

        /// <summary>
        /// Class <see cref="CryptoTransform{T}"/> is a base implementation of the <see cref="ICryptoTransform"/> interface.
        /// </summary>
        /// <typeparam name="T">The provider.</typeparam>
        protected sealed class CryptoTransform<T> : ICryptoTransform
            where T : class, IDisposable
        {
            [NotNull]
            private readonly Func<T, byte[], int, int, byte[], int, int> _transformBlockFunc;

            [NotNull]
            private readonly Func<T, byte[], int, int, byte[]> _transformFinalBlockFunc;

            private T _provider;

            /// <summary>
            /// Initializes a new instance of the <see cref="CryptoTransform{T}" /> class.
            /// </summary>
            /// <param name="provider">The provider.</param>
            /// <param name="transformBlockFunc">The transform block function.</param>
            /// <param name="transformFinalBlockFunc">The transform final block function.</param>
            /// <param name="inputBlockSize">Size of the input block.</param>
            /// <param name="outputBlockSize">Size of the output block.</param>
            /// <param name="canTransformMultipleBlocks">The can transform multiple blocks.</param>
            /// <param name="canReuseTransform">The can reuse transform.</param>
            public CryptoTransform(
                [NotNull] T provider,
                [NotNull] Func<T, byte[], int, int, byte[], int, int> transformBlockFunc,
                [NotNull] Func<T, byte[], int, int, byte[]> transformFinalBlockFunc,
                int inputBlockSize,
                int outputBlockSize,
                bool canTransformMultipleBlocks = false,
                bool canReuseTransform = true)
            {
                _provider = provider;
                _transformBlockFunc = transformBlockFunc;
                _transformFinalBlockFunc = transformFinalBlockFunc;
                InputBlockSize = inputBlockSize;
                OutputBlockSize = outputBlockSize;
                CanTransformMultipleBlocks = canTransformMultipleBlocks;
                CanReuseTransform = canReuseTransform;
            }

            /// <inheritdoc />
            public void Dispose()
            {
                T provider = Interlocked.Exchange(ref _provider, null);
                provider?.Dispose();
            }

            /// <inheritdoc />
            public int TransformBlock(
                byte[] inputBuffer,
                int inputOffset,
                int inputCount,
                byte[] outputBuffer,
                int outputOffset)
            {
                T provider = _provider;
                if (provider == null) throw new ObjectDisposedException("CryptoTransform");
                return _transformBlockFunc(provider, inputBuffer, inputOffset, inputCount, outputBuffer, outputOffset);
            }

            /// <inheritdoc />
            public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
            {
                T provider = _provider;
                if (provider == null) throw new ObjectDisposedException("CryptoTransform");
                return _transformFinalBlockFunc(provider, inputBuffer, inputOffset, inputCount);
            }

            /// <inheritdoc />
            public int InputBlockSize { get; }

            /// <inheritdoc />
            public int OutputBlockSize { get; }

            /// <inheritdoc />
            public bool CanTransformMultipleBlocks { get; }

            /// <inheritdoc />
            public bool CanReuseTransform { get; }
        }
    }
}