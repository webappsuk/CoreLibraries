using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Relection
{
    /// <summary>
    ///   Wraps the method information with accessors for retrieving parameters.
    /// </summary>
    public class Method
    {
        /// <summary>
        /// Caches closed methods.
        /// </summary>
        [NotNull]
        private Lazy<ConcurrentDictionary<string, Method>> _closedMethods =
            new Lazy<ConcurrentDictionary<string, Method>>(
                () => new ConcurrentDictionary<string, Method>(), LazyThreadSafetyMode.PublicationOnly);

            /// <summary>
        /// The extended type.
        /// </summary>
        [NotNull]
        public readonly ExtendedType ExtendedType;

        /// <summary>
        /// The number of generic arguments.
        /// </summary>
        public readonly int GenericArguments;

        /// <summary>
        ///   The method info.
        /// </summary>
        [NotNull]
        public readonly MethodInfo Info;

        /// <summary>
        ///   The parameters.
        /// </summary>
        [NotNull]
        public readonly IEnumerable<ParameterInfo> Parameters;

        /// <summary>
        /// Gets the type of the return.
        /// </summary>
        /// <remarks></remarks>
        [NotNull]
        public Type ReturnType { get { return Info.ReturnType; }}

        /// <summary>
        /// Initializes the <see cref="Method"/> class.
        /// </summary>
        /// <param name="extendedType">Type of the extended.</param>
        /// <param name="genericArguments">The generic arguments.</param>
        /// <param name="info">The info.</param>
        /// <param name="parameters">The parameters.</param>
        /// <remarks></remarks>
        internal Method([NotNull]ExtendedType extendedType, int genericArguments, [NotNull]MethodInfo info, [NotNull]IEnumerable<ParameterInfo> parameters)
        {
            ExtendedType = extendedType;
            GenericArguments = genericArguments;
            Info = info;
            Parameters = parameters;
        }

        /// <summary>
        /// Gets the closed version of a generic method.
        /// </summary>
        /// <param name="genericTypes">The generic types.</param>
        /// <returns>The closed <see cref="Method"/> if the generic types supplied allowed closure; otherwise <see langword="null"/>.</returns>
        /// <remarks></remarks>
        public Method CloseMethod(params Type[] genericTypes)
        {
            // Start with the current extended type and method.
            ExtendedType et = ExtendedType;
            Method method = this;

            // If our parent type is open then we have to start by closing the type.
            if (et.Type.ContainsGenericParameters)
            {
                // Close type and removed used types.
                et = et.CloseType(ref genericTypes);

                // If we failed to close our type, we're done.
                if (et == null)
                    return null;

                // Lookup method on extended type.
                method = et.GetMethod(Info.Name,
                                      GenericArguments,
                                      Parameters.Select(p => p.ParameterType).Concat(new[] {Info.ReturnType}).ToArray());

                // If we can't find the method we're done (shouldn't happen).
                if (method == null)
                    return null;
            }

            // If we don't have any generic parameters left, we're done so long as we have no types left over.
            if (!method.Info.ContainsGenericParameters)
                return genericTypes.Length < 1 ? method : null;

            // Check we have exactly the right number of paramters left.
            if (GenericArguments != genericTypes.Length)
                return null;

            // Create closed method, cache it and return.
            string key = String.Join("|", genericTypes.Select(t => ExtendedType.Get(t).Signature));
            return _closedMethods.Value.GetOrAdd(key,
                                                 k =>
                                                 new Method(et, GenericArguments,
                                                            method.Info.MakeGenericMethod(genericTypes),
                                                            method.Info.GetParameters()));
        }
    }
}