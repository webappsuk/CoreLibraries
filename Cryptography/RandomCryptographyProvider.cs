using System;
using System.Security.Cryptography;
using System.Xml.Linq;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Cryptography
{
    /// <summary>
    /// Base class for all random number generating cryptographic providers.
    /// </summary>
    [PublicAPI]
    public class RandomCryptographyProvider : CryptographyProvider
    {
        /// <inheritdoc />
        public override bool CanEncrypt => true;

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomCryptographyProvider" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="configuration">The configuration.</param>
        protected RandomCryptographyProvider(
            [NotNull] string name,
            [NotNull] XElement configuration)
            : base(name, configuration, false)
        {
        }

        /// <summary>
        /// Gets the algorithm.
        /// </summary>
        /// <returns>A <see cref="System.Security.Cryptography.HashAlgorithm"/>.</returns>
        [NotNull]
        protected virtual RandomNumberGenerator GetAlgorithm()
        {
            RandomNumberGenerator algorithm = CryptoConfig.CreateFromName(Name) as RandomNumberGenerator;
            if (algorithm == null)
                throw new InvalidOperationException(
                    string.Format(Resources.RandomCryptographyProvider_GetEncryptor_Create_Failed, Name));
            return algorithm;
        }

        /// <summary>
        /// Gets an array of random bytes.
        /// </summary>
        /// <param name="count">The number of bytes to return.</param>
        /// <returns>An array of random bytes.</returns>
        [PublicAPI]
        [NotNull]
        public byte[] GetBytes(int count)
        {
            if (count < 1) return Array<byte>.Empty;

            byte[] buffer = new byte[count];
            using (RandomNumberGenerator algorithm = GetAlgorithm())
                algorithm.GetBytes(buffer);
            return buffer;
        }

        /// <summary>
        /// Gets an array of non-zero random bytes.
        /// </summary>
        /// <param name="count">The number of bytes to return.</param>
        /// <returns>An array of random bytes.</returns>
        [PublicAPI]
        [NotNull]
        public byte[] GetNonZeroBytes(int count)
        {
            if (count < 1) return Array<byte>.Empty;

            byte[] buffer = new byte[count];
            using (RandomNumberGenerator algorithm = GetAlgorithm())
                algorithm.GetNonZeroBytes(buffer);
            return buffer;
        }

        /// <summary>
        /// Fills the byte array with random bytes.
        /// </summary>
        /// <param name="data">The array to fill.</param>
        [PublicAPI]
        public void GetBytes([NotNull] byte[] data)
        {
            using (RandomNumberGenerator algorithm = GetAlgorithm())
                algorithm.GetBytes(data);
        }

        /// <summary>
        /// Fills the byte array with non-zero random bytes.
        /// </summary>
        /// <param name="data">The array to fill.</param>
        [PublicAPI]
        public void GetNonZeroBytes([NotNull] byte[] data)
        {
            using (RandomNumberGenerator algorithm = GetAlgorithm())
                algorithm.GetNonZeroBytes(data);
        }

        #region Static shortcuts
        /// <summary>
        /// Gets an array of random bytes.
        /// </summary>
        /// <param name="count">The number of bytes to return.</param>
        /// <returns>An array of random bytes.</returns>
        [PublicAPI]
        [NotNull]
        public static byte[] GetRandomBytes(int count)
        {
            if (count < 1) return Array<byte>.Empty;

            byte[] buffer = new byte[count];
            using (RandomNumberGenerator algorithm = new RNGCryptoServiceProvider())
                algorithm.GetBytes(buffer);
            return buffer;
        }

        /// <summary>
        /// Gets an array of non-zero random bytes.
        /// </summary>
        /// <param name="count">The number of bytes to return.</param>
        /// <returns>An array of random bytes.</returns>
        [PublicAPI]
        [NotNull]
        public static byte[] GetNonZeroRandomBytes(int count)
        {
            if (count < 1) return Array<byte>.Empty;

            byte[] buffer = new byte[count];
            using (RandomNumberGenerator algorithm = new RNGCryptoServiceProvider())
                algorithm.GetNonZeroBytes(buffer);
            return buffer;
        }

        /// <summary>
        /// Fills the byte array with random bytes.
        /// </summary>
        /// <param name="data">The array to fill.</param>
        [PublicAPI]
        public static void GetRandomBytes([NotNull] byte[] data)
        {
            using (RandomNumberGenerator algorithm = new RNGCryptoServiceProvider())
                algorithm.GetBytes(data);
        }

        /// <summary>
        /// Fills the byte array with non-zero random bytes.
        /// </summary>
        /// <param name="data">The array to fill.</param>
        [PublicAPI]
        public static void GetNonZeroRandomBytes([NotNull] byte[] data)
        {
            using (RandomNumberGenerator algorithm = new RNGCryptoServiceProvider())
                algorithm.GetNonZeroBytes(data);
        }
        #endregion

        /// <inheritdoc />
        public override ICryptoTransform GetEncryptor() =>
            new CryptoTransform<RandomNumberGenerator>(
                GetAlgorithm(),
                EncryptBlock,
                EncryptFinalBlock,
                1024,
                1024,
                true,
                true);

        /// <summary>
        /// Encrypts the specified region of the input byte array and copies the resulting transform to the specified region of the output byte array.
        /// </summary>
        /// <param name="algorithm">The algorithm.</param>
        /// <param name="inputBuffer">The input for which to compute the transform.</param>
        /// <param name="inputOffset">The offset into the input byte array from which to begin using data.</param>
        /// <param name="inputCount">The number of bytes in the input byte array to use as data.</param>
        /// <param name="outputBuffer">The output to which to write the transform.</param>
        /// <param name="outputOffset">The offset into the output byte array from which to begin writing data.</param>
        /// <returns>The number of bytes written.</returns>
        private int EncryptBlock(
            [NotNull] RandomNumberGenerator algorithm,
            [NotNull] byte[] inputBuffer,
            int inputOffset,
            int inputCount,
            [NotNull] byte[] outputBuffer,
            int outputOffset)
        {
            byte[] buffer = outputOffset == 0 && outputBuffer.Length == inputCount
                ? outputBuffer
                : new byte[inputCount];
            algorithm.GetBytes(buffer);
            if (buffer != outputBuffer)
                Array.Copy(buffer, 0, outputBuffer, outputOffset, inputCount);
            return inputCount;
        }

        /// <summary>
        /// Encrypts the specified region of the specified byte array.
        /// </summary>
        /// <param name="algorithm">The algorithm.</param>
        /// <param name="inputBuffer">The input for which to compute the transform.</param>
        /// <param name="inputOffset">The offset into the byte array from which to begin using data.</param>
        /// <param name="inputCount">The number of bytes in the byte array to use as data.</param>
        /// <returns>The computed transform.</returns>
        private byte[] EncryptFinalBlock(
            [NotNull] RandomNumberGenerator algorithm,
            [NotNull] byte[] inputBuffer,
            int inputOffset,
            int inputCount)
        {
            byte[] buffer = new byte[inputCount];
            algorithm.GetBytes(buffer);
            return buffer;
        }

        /// <inheritdoc />
        public override ICryptoTransform GetDecryptor()
        {
            throw new CryptographicException(Resources.CryptographyProvider_Decryption_Not_Supported);
        }

        /// <summary>
        /// Creates a <see cref="CryptographyProvider" /> from a <see cref="RandomNumberGenerator" />.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="algorithm">The algorithm.</param>
        /// <param name="configurationElement">The configuration element.</param>
        /// <returns>WebApplications.Utilities.Cryptography.RandomCryptographyProvider.</returns>
        internal static RandomCryptographyProvider Create(
            [NotNull]string name, 
            [NotNull] RandomNumberGenerator algorithm,
            XElement configurationElement)
        {
            // Simple hashing algorithm, no real configuration.
            if (configurationElement == null)
                configurationElement = new XElement("configuration");

            return new RandomCryptographyProvider(name, configurationElement);
        }
    }
}