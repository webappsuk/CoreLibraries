using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Relection
{
    /// <summary>
    ///   Wraps the constructor information with accessors for retrieving parameters.
    /// </summary>
    [DebuggerDisplay("{Info} [Extended]")]
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
        /// Create enumeration of parameters on demand.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Lazy<ParameterInfo[]> _parameters;
        
        /// <summary>
        ///   The parameters.
        /// </summary>
        [NotNull]
        public IEnumerable<ParameterInfo> Parameters { get { return _parameters.Value; } }

        /// <summary>
        /// Gets the parameters count.
        /// </summary>
        /// <remarks></remarks>
        public int ParametersCount { get { return _parameters.Value.Length; } }

        /// <summary>
        /// Initializes the <see cref="Constructor"/> class.
        /// </summary>
        /// <param name="extendedType">Type of the extended.</param>
        /// <param name="info">The info.</param>
        /// <remarks></remarks>
        internal Constructor([NotNull]ExtendedType extendedType, [NotNull]ConstructorInfo info)
        {
            ExtendedType = extendedType;
            Info = info;
            _parameters = new Lazy<ParameterInfo[]>(info.GetParameters, LazyThreadSafetyMode.PublicationOnly);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="WebApplications.Utilities.Relection.Constructor"/> to <see cref="System.Reflection.ConstructorInfo"/>.
        /// </summary>
        /// <param name="constructor">The constructor.</param>
        /// <returns>The result of the conversion.</returns>
        /// <remarks></remarks>
        public static implicit operator ConstructorInfo(Constructor constructor)
        {
            return constructor == null ? null : constructor.Info;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Reflection.ConstructorInfo"/> to <see cref="WebApplications.Utilities.Relection.Constructor"/>.
        /// </summary>
        /// <param name="constructorInfo">The constructor info.</param>
        /// <returns>The result of the conversion.</returns>
        /// <remarks></remarks>
        public static implicit operator Constructor(ConstructorInfo constructorInfo)
        {
            return constructorInfo == null
                       ? null
                       : ((ExtendedType)constructorInfo.DeclaringType).GetConstructor(constructorInfo);
        }
    }
}