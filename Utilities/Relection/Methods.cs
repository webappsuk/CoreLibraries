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
            int bestMatchRequiredCasts = int.MaxValue;
            Type[] bestMatchRequiredTypeClosures = null;
            Type[] bestMatchRequiredMethodClosures = null;

            int tCount = types.Length;
            foreach (Method method in _methods)
            {
                // Match method signature
                bool[] castsRequired;
                Type[] typeClosures;
                Type[] methodClosures;
                if (!method.Matches(out castsRequired, out typeClosures, out methodClosures, genericArguments, types))
                    continue;

                // Check to see if this beats the current best match
                int? bmcc = null;
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
                        if (bmcl == mcl)
                        {
                            bmcc = castsRequired.Count(c => c);
                            if (bestMatchRequiredCasts <= bmcc)
                                continue;
                        }
                    }
                }

                // Set best match
                bestMatch = method;
                bestMatchRequiredCasts = bmcc ?? castsRequired.Count(c => c);
                bestMatchRequiredTypeClosures = typeClosures;
                bestMatchRequiredMethodClosures = methodClosures;
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

                // Research for method on extended type.
                bestMatch = et.GetMethod(bestMatch.Info.Name,
                                         genericArguments,
                                         types);
                return bestMatch;
            }

            // Check to see if we have to close the method
            if (bestMatchRequiredMethodClosures.Any(m => m != null))
                bestMatch = bestMatch.CloseMethod(bestMatchRequiredMethodClosures);

            return bestMatch;
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