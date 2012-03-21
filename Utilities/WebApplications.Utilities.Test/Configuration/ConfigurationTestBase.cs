using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using WebApplications.Testing;
using WebApplications.Utilities.Configuration;

namespace WebApplications.Utilities.Test.Configuration
{
    public abstract class ConfigurationTestBase : TestBase
    {

        /// <summary>
        /// Chooses a random type from a list of options.
        /// </summary>
        /// <param name="types">The list of items to choose from.</param>
        /// <exception cref="ArgumentException">Thrown if the list to choose from contains no items.</exception>
        protected static Type ChooseRandomTypeFromList(List<Type> types)
        {
            if (types == null || !types.Any())
                throw new ArgumentException("The list of types to choose from must contain at least one item.");
            return types.ElementAt(Random.Next(0, types.Count()));
        }

        protected static List<ConfigurationPropertyAttribute> GetConfigurationPropertyAttributesForProperty(Type classToTest, string propertyName)
        {
            System.Reflection.MemberInfo propertyInfo = classToTest.GetMember(propertyName).First();
            return propertyInfo.GetCustomAttributes(typeof(ConfigurationPropertyAttribute), false).Cast<ConfigurationPropertyAttribute>().ToList();
        }
    }
}
