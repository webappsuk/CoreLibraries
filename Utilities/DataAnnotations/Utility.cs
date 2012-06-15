using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WebApplications.Utilities.DataAnnotations
{
    public static class Utility
    {
        public static IEnumerable<Attribute> GetInheritedAttributes(PropertyInfo propertyInfo)
        {
            return propertyInfo.DeclaringType
                .GetInterfaces()
                .SelectMany(
                    t => t.GetProperties()
                        .Where(p => p.Name == propertyInfo.Name)
                        .SelectMany(property => property.GetCustomAttributes()))
                .Union(Attribute.GetCustomAttributes(propertyInfo, true))
                .Distinct();
        }
    }
}
