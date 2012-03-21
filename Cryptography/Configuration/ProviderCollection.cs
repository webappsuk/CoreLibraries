using WebApplications.Utilities.Configuration;

namespace WebApplications.Utilities.Cryptography.Configuration
{
    /// <summary>
    /// A collection of <see cref="ProviderElement"/> crypto provider elements.
    /// </summary>
    public class ProviderCollection : ConfigurationElementCollection<string, ProviderElement>
    {
        /// <summary>
        /// Gets the element key.
        /// </summary>
        /// <param name="element">The element whose key we want to retrieve.</param>
        /// <returns>
        /// An <see cref="object"/> that acts as the key for the specified element.
        /// </returns>
        protected override string GetElementKey(ProviderElement element)
        {
            return element.Id;
        }
    }
}
