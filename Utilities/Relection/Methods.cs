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
            return _methods.BestMatch(genericArguments, types) as Method;
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