using System.Xml.Linq;

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
        /// <param name="configuration">The provider element.</param>
        protected RandomCryptographyProvider(XElement configuration)
            : base(configuration)
        {
        }

    }
}