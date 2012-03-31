using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Relection
{
    /// <summary>
    ///   Holds information about all methods with a particular name.
    /// </summary>
    public class Methods
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
        private readonly Dictionary<string, Method> _methods = new Dictionary<string, Method>();

        /// <summary>
        ///   Initializes a new instance of the <see cref="Overloadable&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="extendedType">The extended type.</param>
        /// <param name="method">The method to add.</param>
        internal Methods([NotNull]ExtendedType extendedType, [NotNull]MethodInfo method)
        {
            ExtendedType = extendedType;
            Add(method);
        }

        /// <summary>
        ///   Adds the specified method.
        /// </summary>
        /// <param name="method">The method to add.</param>
        internal void Add([NotNull]MethodInfo method)
        {
            // Build type array.
            int genericArguments = method.IsGenericMethod
                                       ? method.GetGenericArguments().Length
                                       : 0;

            ParameterInfo[] paramters = method.GetParameters();
            int tCount = paramters.Length + 1;
            Type[] types = new Type[tCount];
            int a = 0;
            for (; a < (tCount - 1); a++)
            {
                types[a] = paramters[a].ParameterType;
            }
            types[a] = method.ReturnType;

            // Add method
            _methods.Add(CreateKey(genericArguments, types), new Method(ExtendedType, genericArguments, method, paramters));
        }

        /// <summary>
        /// Gets the overloads for this method.
        /// </summary>
        /// <remarks></remarks>
        [NotNull]
        public IEnumerable<Method> Overloads { get { return _methods.Values; } }

        /// <summary>
        /// Creates a key for a signature.
        /// </summary>
        /// <param name="genericArguments">The number of generic arguments.</param>
        /// <param name="types">The generic argument types, the parameter types and the return type specified (in that order).</param>
        /// <returns></returns>
        /// <remarks>We prepend the <paramref name="types"/>
        /// 	<see cref="System.Array.Length">count</see>
        /// to the key as this will cause string comparison failures more quickly.</remarks>
        [NotNull]
        private string CreateKey(int genericArguments, [NotNull]params Type[] types)
        {
            int l = types.Length;
            Debug.Assert(l > 0);

            StringBuilder sb = new StringBuilder();
            sb.Append(types.Length);
            sb.Append("|");
            sb.Append(genericArguments);
            foreach (Type t in types)
            {
                sb.Append('|');
                sb.Append(ExtendedType.Get(t).Signature);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Gets the <see cref="Method"/> matching the number of generic arguments, the parameter types and the return type specified (in that order).
        /// </summary>
        /// <param name="genericArguments">The generic arguments.</param>
        /// <param name="types">The parameter types and return type.</param>
        /// <returns>The <see cref="Method"/>; otherwise <see langword="null"/> if not found.</returns>
        /// <remarks></remarks>
        [CanBeNull]
        public Method GetOverload(int genericArguments, [NotNull]params Type[] types)
        {
            Method method;
            return _methods.TryGetValue(CreateKey(genericArguments, types), out method) ? method : null;
        }

        /// <summary>
        /// Gets the <see cref="Method"/> matching the parameter types and the return type specified (in that order).
        /// </summary>
        /// <param name="types">The parameter types and return type.</param>
        /// <returns>The <see cref="Method"/>; otherwise <see langword="null"/> if not found.</returns>
        /// <remarks></remarks>
        [CanBeNull]
        public Method GetOverload([NotNull]params Type[] types)
        {
            return GetOverload(0, types);
        }
    }
}