using System;
using System.Collections;
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
                Debug.Assert(StaticConstructor == null);
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
        /// <param name="types">The parameter types.</param>
        /// <returns>The <see cref="Constructor"/>; otherwise <see langword="null"/> if not found.</returns>
        /// <remarks></remarks>
        [CanBeNull]
        public Constructor GetOverload([NotNull]params TypeSearch[] types)
        {
            // Holds matches along with order.
            Constructor bestMatch = null;
            int bestMatchRequiredCasts = int.MaxValue;

            // Add return type of parent type.
            TypeSearch[] t = new TypeSearch[types.Length + 1];
            Array.Copy(types, t, types.Length);
            t[types.Length] = ExtendedType.Type;

            Type[] bestMatchRequiredTypeClosures = null;

            foreach (Constructor constructor in _constructors)
            {
                // Match constructor signature
                bool[] castsRequired;
                Type[] typeClosures;
                Type[] methodClosures;
                if (!constructor.Matches(out castsRequired, out typeClosures, out methodClosures, 0, t))
                    continue;

                Debug.Assert(methodClosures.Length == 0);

                // Check to see if this beats the current best match
                int? bmcc = null;
                if (bestMatch != null)
                {
                    int btcl = bestMatchRequiredTypeClosures.Length;
                    int tcl = typeClosures.Length;
                    // If we have to close more type generic arguments then existing match is better.
                    if (btcl < tcl) continue;

                    // If type level closures are equal, see which has more casts.
                    if (btcl == tcl)
                    {
                        bmcc = castsRequired.Count(c => c);
                        if (bestMatchRequiredCasts <= bmcc)
                            continue;
                    }
                }

                bestMatch = constructor;
                bestMatchRequiredCasts = bmcc ?? castsRequired.Count(c => c);
                bestMatchRequiredTypeClosures = typeClosures;
            }

            // If we don't have a match return null.
            if (bestMatch == null) return null;

            // Check to see if we have to close the type
            if (bestMatchRequiredTypeClosures.Any(c => c != null))
            {
                Debug.Assert(ExtendedType.GenericArguments.Count() == bestMatchRequiredTypeClosures.Length);
                
                // Close type.
                ExtendedType et = this.ExtendedType;
                et = et.CloseType(bestMatchRequiredTypeClosures);

                // If we failed to close our type, we're done.
                if (et == null)
                    return null;

                // Research for constructor on extended type.
                bestMatch = et.GetConstructor(types);
            }

            return bestMatch;
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