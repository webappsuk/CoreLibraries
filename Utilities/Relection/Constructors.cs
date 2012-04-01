using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Relection
{
    /// <summary>
    ///   Holds information about all constructors with a given name.
    /// </summary>
    [DebuggerDisplay("{DebugString}")]
    public class Constructors
    {
        /// <summary>
        /// The extended type.
        /// </summary>
        [NotNull]
        public readonly ExtendedType ExtendedType;

        /// <summary>
        ///   It is possible to have overloads with ref/out parameters,
        ///   hence type position is not always enough to disambiguate.
        /// </summary>
        [NotNull]
        private readonly List<Constructor> _constructors = new List<Constructor>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Constructors"/> class.
        /// </summary>
        /// <param name="extendedType">Type of the extended.</param>
        /// <param name="constructor">The constructor.</param>
        /// <remarks></remarks>
        internal Constructors([NotNull]ExtendedType extendedType, [NotNull]ConstructorInfo constructor)
        {
            ExtendedType = extendedType;
            _constructors.Add(new Constructor(ExtendedType, constructor));
        }

        /// <summary>
        /// Adds the specified constructor.
        /// </summary>
        /// <param name="constructor">The constructor to add.</param>
        /// <remarks></remarks>
        internal void Add([NotNull]ConstructorInfo constructor)
        {
            // Add constructor
            _constructors.Add(new Constructor(ExtendedType, constructor));
        }

        /// <summary>
        /// Gets the overloads for this constructor.
        /// </summary>
        /// <remarks></remarks>
        [NotNull]
        public IEnumerable<Constructor> Overloads { get { return _constructors; } }
        
        /// <summary>
        /// Gets the <see cref="Constructor"/> matching the parameter types.
        /// </summary>
        /// <param name="types">The parameter types and return type.</param>
        /// <returns>The <see cref="Constructor"/>; otherwise <see langword="null"/> if not found.</returns>
        /// <remarks></remarks>
        [CanBeNull]
        public Constructor GetOverload([NotNull]params TypeSearch[] types)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the overload matching the constructor info.
        /// </summary>
        /// <param name="constructorInfo">The constructor info.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public Constructor GetOverload([NotNull]ConstructorInfo constructorInfo)
        {
            return _constructors.FirstOrDefault(c => c.Info == constructorInfo);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public string DebugString
        {
            get
            {
                return String.Format("'{0}' Constructors [{1} overloads].",
                    ExtendedType.Signature,
                    _constructors.Count);
            }
        }
    }
}