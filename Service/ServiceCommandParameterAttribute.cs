using System;
using System.Diagnostics.Contracts;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Service
{
    /// <summary>
    /// Add to a parameter of a command indicated by a <see cref="ServiceCommandAttribute"/> to add a description for the parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    [Serializable]
    [PublicAPI]
    public class ServiceCommandParameterAttribute : Attribute
    {
        /// <summary>
        /// The resource type.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public readonly Type ResourceType;

        /// <summary>
        /// The resource property for the description.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public readonly string DescriptionProperty;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceCommandParameterAttribute"/> class.
        /// </summary>
        /// <param name="resourceType">Type of the resource.</param>
        /// <param name="descriptionProperty">The description property.</param>
        public ServiceCommandParameterAttribute([NotNull] Type resourceType, [NotNull] string descriptionProperty)
        {
            Contract.Requires<RequiredContractException>(resourceType != null, "Parameter_Null");
            Contract.Requires<RequiredContractException>(descriptionProperty != null, "Parameter_Null");
            ResourceType = resourceType;
            DescriptionProperty = descriptionProperty;
        }
    }
}