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
    ///   Holds information about all methods with a particular name.
    /// </summary>
    [DebuggerDisplay("{DebugString}")]
    public class Methods
    {
        /// <summary>
        /// The extended type.
        /// </summary>
        [NotNull]
        public readonly ExtendedType ExtendedType;

        /// <summary>
        /// The methods name.
        /// </summary>
        [NotNull]
        public readonly string Name;

        /// <summary>
        ///   It is possible to have overloads with ref/out parameters,
        ///   hence type position is not always enough to disambiguate.
        /// </summary>
        [NotNull]
        private readonly List<Method> _methods = new List<Method>();

        /// <summary>
        ///   Initializes a new instance of the <see cref="Overloadable&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="extendedType">The extended type.</param>
        /// <param name="method">The method to add.</param>
        internal Methods([NotNull]ExtendedType extendedType, [NotNull]MethodInfo method)
        {
            ExtendedType = extendedType;
            Name = method.Name;
            _methods.Add(new Method(extendedType, method));
        }

        /// <summary>
        ///   Adds the specified method.
        /// </summary>
        /// <param name="method">The method to add.</param>
        internal void Add([NotNull]MethodInfo method)
        {
            _methods.Add(new Method(ExtendedType, method));
        }

        /// <summary>
        /// Gets the overloads for this method.
        /// </summary>
        /// <remarks></remarks>
        [NotNull]
        public IEnumerable<Method> Overloads { get { return _methods; } }
        
        /// <summary>
        /// Gets the <see cref="Method"/> matching the number of generic arguments, the parameter types and the return type specified (in that order).
        /// </summary>
        /// <param name="genericArguments">The generic arguments.</param>
        /// <param name="types">The parameter types and return type.</param>
        /// <returns>The <see cref="Method"/>; otherwise <see langword="null"/> if not found.</returns>
        /// <remarks></remarks>
        [CanBeNull]
        public Method GetOverload(int genericArguments, [NotNull]params TypeSearch[] types)
        {
            // Holds matches along with order.
            Method bestMatch = null;
            List<int> bestMatchRequiredCasts = null;

            int typeArgCount = ExtendedType.Type.IsGenericTypeDefinition ? ExtendedType.GenericArgumentsCount : 0;

            Type[] bestMatchRequiredTypeClosures = null;
            Type[] bestMatchRequiredMethodClosures = null;

            int tCount = types.Length;
            foreach (Method method in _methods)
            {
                Debug.Assert(method != null);
                // Check we have enough parameters
                if (method.ParametersCount + 1 != tCount) continue;

                // Check we have enough generic arguments
                if (method.GenericArgumentsCount != genericArguments) continue;

                // Create lists for casts and closures.
                List<int> casts = new List<int>(tCount);
                Type[] typeClosures = new Type[typeArgCount];
                Type[] methodClosures = new Type[method.GenericArgumentsCount];

                // Check return type
                TypeSearch returnSearch = types.Last();
                Debug.Assert(returnSearch != null);
                bool requiresCast;
                GenericArgumentLocation closureLocation;
                int closurePosition;
                Type closureType;

                if (!method.ReturnType.Matches(returnSearch, out requiresCast, out closureLocation, out closurePosition, out closureType) ||
                    !UpdateContext(casts, typeClosures, methodClosures, requiresCast, closureLocation, closurePosition, closureType))
                    continue;

                if (tCount > 1)
                {
                    // Check parameters
                    IEnumerator<ParameterInfo> pe = method.Parameters.GetEnumerator();
                    IEnumerator te = types.GetEnumerator();
                    bool paramsMatch = true;
                    while (pe.MoveNext())
                    {
                        te.MoveNext();
                        Debug.Assert(pe.Current != null);
                        Debug.Assert(te.Current != null);
                        ParameterInfo t = pe.Current;
                        TypeSearch s = ((TypeSearch)te.Current);
                        if (
                            t.ParameterType.Matches(s, out requiresCast, out closureLocation, out closurePosition, out closureType) &&
                            UpdateContext(casts, typeClosures, methodClosures, requiresCast, closureLocation, closurePosition, closureType))
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

                    // If type level closures are equal, look more closely
                    if (btcl == tcl)
                    {
                        int bmcl = bestMatchRequiredMethodClosures.Length;
                        int mcl = methodClosures.Length;

                        // If we have to close more method generic arguments then existing match is better.
                        if (bmcl < mcl) continue;

                        // If method level closures are equal, see which has more casts.
                        if (bmcl == mcl && bestMatchRequiredCasts.Count < casts.Count) continue;
                    }
                }
                bestMatch = method;
                bestMatchRequiredCasts = casts;
                bestMatchRequiredTypeClosures = typeClosures;
                bestMatchRequiredMethodClosures = methodClosures;
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

                // Research for method on extended type.
                bestMatch = et.GetMethod(bestMatch.Info.Name,
                                         genericArguments,
                                         types);
                return bestMatch;
            }

            if (bestMatchRequiredMethodClosures.Length > 0)
            {
                int nullCount = bestMatchRequiredMethodClosures.Count(t => t == null);
                // If we have any nulls, we haven't fully closed method.
                // Only return match if we haven't tried to close method.
                if (nullCount > 0)
                    return (nullCount == bestMatchRequiredMethodClosures.Length) ? bestMatch : null;

                bestMatch = bestMatch.CloseMethod(bestMatchRequiredMethodClosures);
            }

            return bestMatch;
        }

        private bool UpdateContext([NotNull]List<int> casts, [NotNull]Type[] typeClosures, [NotNull]Type[] methodClosures, bool requiresCast, GenericArgumentLocation closureLocation, int closurePosition, Type closureType)
        {
            Debug.Assert(closureLocation != GenericArgumentLocation.Any);

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
            if (closureLocation == GenericArgumentLocation.Method)
            {
                // Requires method closure
                Debug.Assert(closurePosition < methodClosures.Length);

                // If we already have a closure, ensure it matches!
                if (methodClosures[closurePosition] != null)
                {
                    if (methodClosures[closurePosition] != closureType) return false;
                }
                else
                {
                    methodClosures[closurePosition] = closureType;
                }
            }
            else
            {
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
            }
            return true;
        }

        /// <summary>
        /// Gets the <see cref="Method"/> matching the parameter types and the return type specified (in that order).
        /// </summary>
        /// <param name="types">The parameter types and return type.</param>
        /// <returns>The <see cref="Method"/>; otherwise <see langword="null"/> if not found.</returns>
        /// <remarks></remarks>
        [CanBeNull]
        public Method GetOverload([NotNull]params TypeSearch[] types)
        {
            return GetOverload(0, types);
        }

        /// <summary>
        /// Gets the overload matching the method info.
        /// </summary>
        /// <param name="methodInfo">The method info.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public Method GetOverload([NotNull]MethodInfo methodInfo)
        {
            return _methods.FirstOrDefault(m => m.Info == methodInfo);
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
                return String.Format("{0}.{1} Methods [{2} overloads].",
                    ExtendedType.Signature,
                    Name,
                    _methods.Count);
            }
        }
    }
}