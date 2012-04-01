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
        private readonly List<Constructor> _constructors = new List<Constructor>();

        /// <summary>
        /// The static constructor is a special case and does not appear directly in overloads.
        /// </summary>
        public Constructor StaticConstructor { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Constructors"/> class.
        /// </summary>
        /// <param name="extendedType">Type of the extended.</param>
        /// <param name="constructor">The constructor.</param>
        /// <remarks></remarks>
        internal Constructors([NotNull]ExtendedType extendedType, [NotNull]ConstructorInfo constructor)
        {
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
        /// <param name="types">The parameter types and return type.</param>
        /// <returns>The <see cref="Constructor"/>; otherwise <see langword="null"/> if not found.</returns>
        /// <remarks></remarks>
        [CanBeNull]
        public Constructor GetOverload([NotNull]params TypeSearch[] types)
        {
            // Holds matches along with order.
            Constructor bestMatch = null;
            List<int> bestMatchRequiredCasts = null;

            int typeArgCount = ExtendedType.Type.IsGenericTypeDefinition ? ExtendedType.GenericArgumentsCount : 0;

            Type[] bestMatchRequiredTypeClosures = null;

            int tCount = types.Length;
            foreach (Constructor constructor in _constructors)
            {
                Debug.Assert(constructor != null);
                // Check we have exact parameter count.
                if (constructor.ParametersCount != tCount) continue;

                // Create lists for casts and closures.
                List<int> casts = new List<int>(tCount);
                Type[] typeClosures = new Type[typeArgCount];

                if (tCount > 1)
                {
                    // Check parameters
                    IEnumerator<ParameterInfo> pe = constructor.Parameters.GetEnumerator();
                    IEnumerator te = types.GetEnumerator();
                    bool paramsMatch = true;
                    while (pe.MoveNext())
                    {
                        te.MoveNext();
                        Debug.Assert(pe.Current != null);
                        Debug.Assert(te.Current != null);
                        ParameterInfo t = pe.Current;
                        TypeSearch s = ((TypeSearch)te.Current);

                        bool requiresCast;
                        GenericArgumentLocation closureLocation;
                        int closurePosition;
                        Type closureType;
                        if (
                            t.ParameterType.Matches(s, out requiresCast, out closureLocation, out closurePosition, out closureType) &&
                            UpdateContext(casts, typeClosures, requiresCast, closureLocation, closurePosition, closureType))
                            continue;

                        paramsMatch = false;
                        break;
                    }
                    if (!paramsMatch)
                        continue;
                }

                // Check to see if this beats the current best match
                if (bestMatch != null)
                {
                    int btcl = bestMatchRequiredTypeClosures.Length;
                    int tcl = typeClosures.Length;
                    // If we have to close more type generic arguments then existing match is better.
                    if (btcl < tcl) continue;

                    // If type level closures are equal, see which has more casts.
                    if (btcl == tcl && bestMatchRequiredCasts.Count < casts.Count) continue;
                }

                bestMatch = constructor;
                bestMatchRequiredCasts = casts;
                bestMatchRequiredTypeClosures = typeClosures;
            }

            // If we don't have a match return null.
            if (bestMatch == null) return null;

            // Check to see if we have to close the type
            if (bestMatchRequiredTypeClosures.Length > 0)
            {
                int nullCount = bestMatchRequiredTypeClosures.Count(t => t == null);
                // If we have any nulls, we haven't fully closed type.
                // Only return match if we haven't tried to close type.
                if (nullCount > 0)
                    return (nullCount == typeArgCount) ? bestMatch : null;

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
        /// Updates the context.
        /// </summary>
        /// <param name="casts">The casts.</param>
        /// <param name="typeClosures">The type closures.</param>
        /// <param name="requiresCast">if set to <see langword="true"/> [requires cast].</param>
        /// <param name="closureLocation">The closure location.</param>
        /// <param name="closurePosition">The closure position.</param>
        /// <param name="closureType">Type of the closure.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private bool UpdateContext([NotNull]List<int> casts, [NotNull]Type[] typeClosures, bool requiresCast, GenericArgumentLocation closureLocation, int closurePosition, Type closureType)
        {
            Debug.Assert(closureLocation != GenericArgumentLocation.Any);
            Debug.Assert(closureLocation != GenericArgumentLocation.Method);

            if (requiresCast)
            {
                Debug.Assert(closureLocation == GenericArgumentLocation.None);
                Debug.Assert(closurePosition < 0);
                casts.Add(-1);
                return true;
            }

            // Check if we have a closure location
            if (closureLocation == GenericArgumentLocation.None)
            {
                Debug.Assert(closurePosition < 0);
                return true;
            }

            Debug.Assert(closureType != null);
            Debug.Assert(closureLocation == GenericArgumentLocation.Type);

            // Requires type closure
            Debug.Assert(closurePosition < ExtendedType.GenericArgumentsCount);

            // If we already have a closure, ensure it matches!
            if (typeClosures[closurePosition] != null)
            {
                if (typeClosures[closurePosition] != closureType) return false;
            }
            else
            {
                typeClosures[closurePosition] = closureType;
            }
            return true;
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