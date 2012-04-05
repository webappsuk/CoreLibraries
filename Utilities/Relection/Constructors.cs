using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
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
        private readonly List<Constructor> _constructors;

        /// <summary>
        /// The static constructor is a special case and does not appear directly in overloads.
        /// </summary>
        public Constructor StaticConstructor { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Constructors"/> class.
        /// </summary>
        /// <param name="extendedType">Type of the extended.</param>
        /// <remarks></remarks>
        internal Constructors([NotNull]ExtendedType extendedType)
        {
            _constructors = new List<Constructor>(0);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Constructors"/> class.
        /// </summary>
        /// <param name="extendedType">Type of the extended.</param>
        /// <param name="constructor">The constructor.</param>
        /// <remarks></remarks>
        internal Constructors([NotNull]ExtendedType extendedType, [NotNull]ConstructorInfo constructor)
        {
            _constructors = new List<Constructor>();
            ExtendedType = extendedType;
            Add(constructor);
        }

        /// <summary>
        /// Adds the specified constructor.
        /// </summary>
        /// <param name="constructor">The constructor to add.</param>
        /// <remarks></remarks>
        internal void Add([NotNull]ConstructorInfo constructor)
        {
            Constructor c = new Constructor(ExtendedType, constructor);
            if (constructor.IsStatic)
            {
                Contract.Assert(StaticConstructor == null);
                StaticConstructor = c;
            }

            // Add constructor
            _constructors.Add(c);
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
        /// <param name="types">The parameter types, and return type (i.e. the constructor's type).</param>
        /// <returns>The <see cref="Constructor"/>; otherwise <see langword="null"/> if not found.</returns>
        /// <remarks></remarks>
        [CanBeNull]
        public Constructor GetOverload([NotNull]params TypeSearch[] types)
        {
            return _constructors.BestMatch(types) as Constructor;
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
                return String.Format("'{0}' Constructors [{1} overloads{2}].",
                    ExtendedType.Signature,
                    _constructors.Count,
                    StaticConstructor != null ? " , and 1 static" : String.Empty);
            }
        }
    }
}