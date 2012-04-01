using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Relection
{
    /// <summary>
    ///   Wraps a <see cref="System.Reflection.PropertyInfo"/> that actually refers to an indexer with accessors.
    /// </summary>
    /// <remarks></remarks>
    [DebuggerDisplay("{Info} [Extended]")]
    public class Indexer : Property
    {
        /// <summary>
        /// Creates index parameters on demand.
        /// </summary>
        private readonly Lazy<ParameterInfo[]>  _indexParameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="Property"/> class.
        /// </summary>
        /// <param name="extendedType">The extended type.</param>
        /// <param name="info">The <see cref="System.Reflection.PropertyInfo">property info</see>.</param>
        /// <remarks></remarks>
        internal Indexer([NotNull] ExtendedType extendedType, [NotNull] PropertyInfo info) : base(extendedType, info)
        {
            Debug.Assert(extendedType.DefaultMember == info.Name);
            _indexParameters = new Lazy<ParameterInfo[]>(info.GetIndexParameters, LazyThreadSafetyMode.PublicationOnly);
        }

        /// <summary>
        /// Gets the index parameters (if this is an index);
        /// </summary>
        /// <remarks></remarks>
        [NotNull]
        public IEnumerable<ParameterInfo> IndexParameters { get { return _indexParameters.Value; } }

        /// <summary>
        /// Performs an implicit conversion from <see cref="WebApplications.Utilities.Relection.Property"/> to <see cref="System.Reflection.PropertyInfo"/>.
        /// </summary>
        /// <param name="indexer">The indexer.</param>
        /// <returns>The result of the conversion.</returns>
        /// <remarks></remarks>
        public static implicit operator PropertyInfo(Indexer indexer)
        {
            return indexer == null ? null : indexer.Info;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Reflection.PropertyInfo"/> to <see cref="WebApplications.Utilities.Relection.Property"/>.
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        /// <returns>The result of the conversion.</returns>
        /// <remarks></remarks>
        public static implicit operator Indexer(PropertyInfo propertyInfo)
        {
            return propertyInfo == null
                       ? null
                       : ((ExtendedType)propertyInfo.DeclaringType).GetIndexer(propertyInfo);
        }
    }
}