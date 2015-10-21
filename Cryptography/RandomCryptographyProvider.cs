using System.Xml.Linq;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Cryptography.Configuration;

namespace WebApplications.Utilities.Cryptography
{
    /// <summary>
    /// Base class for all random number generating cryptographic providers.
    /// </summary>
    public abstract class RandomCryptographyProvider : CryptographyProvider
    {
        /// <inheritdoc />
        public override bool CanEncrypt => true;

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomCryptographyProvider" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="providerElement">The provider element (if any).</param>
        /// <param name="configuration">The provider element (if any).</param>
        /// <param name="preservesLength">
        ///   <see langword="true" /> if the provider preserves the length.</param>
        protected RandomCryptographyProvider(
            [NotNull] string name,
            [CanBeNull] ProviderElement providerElement = null,
            [CanBeNull] XElement configuration = null,
            bool preservesLength = true)
            : base(name, providerElement, configuration, preservesLength)
        {
        }

    }
}