using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Relection
{
    /// <summary>
    ///   Wraps the constructor information with accessors for retrieving parameters.
    /// </summary>
    public class Constructor
    {
        /// <summary>
        /// The extended type.
        /// </summary>
        [NotNull]
        public readonly ExtendedType ExtendedType;

        /// <summary>
        ///   The constructor info.
        /// </summary>
        [NotNull]
        public readonly ConstructorInfo Info;

        /// <summary>
        ///   The parameters.
        /// </summary>
        [NotNull]
        public readonly IEnumerable<ParameterInfo> Parameters;

        /// <summary>
        /// Initializes the <see cref="Constructor"/> class.
        /// </summary>
        /// <param name="extendedType">The extended type.</param>
        /// <param name="info">The info.</param>
        /// <param name="parameters">The parameters.</param>
        /// <remarks></remarks>
        internal Constructor([NotNull]ExtendedType extendedType, [NotNull]ConstructorInfo info, [NotNull]IEnumerable<ParameterInfo> parameters)
        {
            ExtendedType = extendedType;
            Info = info;
            Parameters = parameters;
        }
    }
}