using System;
using System.Reflection;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Relection
{
    /// <summary>
    ///   Wraps a <see cref="System.Reflection.PropertyInfo"/> with accessors.
    /// </summary>
    public class Property
    {
        /// <summary>
        ///   The property info object, which provides access to property metadata. 
        /// </summary>
        [NotNull]
        public readonly PropertyInfo Info;

        /// <summary>
        ///   Grabs the getter method lazily.
        /// </summary>
        [NotNull]
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
        /// <param name="info">
        ///   The <see cref="System.Reflection.PropertyInfo">property info</see>.
        /// </param>
        internal Property([NotNull]PropertyInfo info)
        {
            Info = info;
            _getMethod = new Lazy<MethodInfo>(() => info.GetGetMethod(true));
            _setMethod = new Lazy<MethodInfo>(() => info.GetSetMethod(true));
        }
    }
}