using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Relection
{
    /// <summary>
    ///   Wraps a <see cref="System.Reflection.PropertyInfo"/> with accessors.
    /// </summary>
    [DebuggerDisplay("{Info} [Extended]")]
    public class Property
    {
        /// <summary>
        /// The extended type.
        /// </summary>
        [NotNull]
        public readonly ExtendedType ExtendedType;

        /// <summary>
        ///   The property info object, which provides access to property metadata. 
        /// </summary>
        [NotNull]
        public readonly PropertyInfo Info;

        /// <summary>
        ///   Grabs the getter method lazily.
        /// </summary>
        [NotNull]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Lazy<MethodInfo> _getMethod;

        /// <summary>
        ///   Gets the getter method.
        /// </summary>
        [CanBeNull]
        public MethodInfo GetMethod
        {
            get { return _getMethod.Value; }
        }

        /// <summary>
        ///   Grabs the setter method lazily.
        /// </summary>
        [NotNull]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Lazy<MethodInfo> _setMethod;

        /// <summary>
        ///   Gets the setter method.
        /// </summary>
        [CanBeNull]
        public MethodInfo SetMethod
        {
            get { return _setMethod.Value; }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Property"/> class.
        /// </summary>
        /// <param name="extendedType">The extended type.</param>
        /// <param name="info">
        ///   The <see cref="System.Reflection.PropertyInfo">property info</see>.
        /// </param>
        internal Property([NotNull]ExtendedType extendedType, [NotNull] PropertyInfo info)
        {
            ExtendedType = extendedType;
            Info = info;
            _getMethod = new Lazy<MethodInfo>(() => info.GetGetMethod(true), LazyThreadSafetyMode.PublicationOnly);
            _setMethod = new Lazy<MethodInfo>(() => info.GetSetMethod(true), LazyThreadSafetyMode.PublicationOnly);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="WebApplications.Utilities.Relection.Property"/> to <see cref="System.Reflection.PropertyInfo"/>.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>The result of the conversion.</returns>
        /// <remarks></remarks>
        public static implicit operator PropertyInfo(Property property)
        {
            return property == null ? null : property.Info;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Reflection.PropertyInfo"/> to <see cref="WebApplications.Utilities.Relection.Property"/>.
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        /// <returns>The result of the conversion.</returns>
        /// <remarks></remarks>
        public static implicit operator Property(PropertyInfo propertyInfo)
        {
            return propertyInfo == null
                       ? null
                       : ((ExtendedType) propertyInfo.DeclaringType).GetProperty(propertyInfo);
        }
    }
}