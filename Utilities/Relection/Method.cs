using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Relection
{
    /// <summary>
    ///   Wraps the method information with accessors for retrieving parameters.
    /// </summary>
    [DebuggerDisplay("{Info} [Extended]")]
    public class Method : ISignature
    {
        /// <summary>
        /// Caches closed methods.
        /// </summary>
        [NotNull]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Lazy<ConcurrentDictionary<string, Method>> _closedMethods =
            new Lazy<ConcurrentDictionary<string, Method>>(
                () => new ConcurrentDictionary<string, Method>(), LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// The extended type.
        /// </summary>
        [NotNull]
        public readonly ExtendedType ExtendedType;

        /// <summary>
        ///   The method info.
        /// </summary>
        [NotNull]
        public readonly MethodInfo Info;

        /// <summary>
        /// Create enumeration of parameters on demand.
        /// </summary>
        [NotNull]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Lazy<ParameterInfo[]> _parameters;

        /// <summary>
        ///   The parameters.
        /// </summary>
        [NotNull]
        public IEnumerable<ParameterInfo> Parameters { get { return _parameters.Value; } }

        /// <inheritdoc/>
        public IEnumerable<Type> ParameterTypes
        {
            get { return _parameters.Value.Select(p => p.ParameterType); }
        }

        /// <summary>
        /// Gets the parameters count.
        /// </summary>
        /// <remarks></remarks>
        public int ParametersCount { get { return _parameters.Value.Length; } }
        
        /// <inheritdoc/>
        public IEnumerable<GenericArgument> TypeGenericArguments { get { return ExtendedType.GenericArguments; } }

        /// <summary>
        /// Creates array of generic arguments on demand.
        /// </summary>
        [NotNull]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Lazy<List<GenericArgument>> _genericArguments;

        /// <inheritdoc/>
        public IEnumerable<GenericArgument> SignatureGenericArguments
        {
            get { return _genericArguments.Value; }
        }

        /// <summary>
        /// Gets the type of the return.
        /// </summary>
        /// <remarks></remarks>
        [NotNull]
        public Type ReturnType { get { return Info.ReturnType; } }

        /// <summary>
        /// Initializes the <see cref="Method"/> class.
        /// </summary>
        /// <param name="extendedType">Type of the extended.</param>
        /// <param name="info">The info.</param>
        /// <remarks></remarks>
        internal Method([NotNull]ExtendedType extendedType, [NotNull]MethodInfo info)
        {
            ExtendedType = extendedType;
            Info = info;
            _genericArguments = new Lazy<List<GenericArgument>>(
                () => info.GetGenericArguments()
                          .Select((g, i) => new GenericArgument(GenericArgumentLocation.Signature, i, g))
                          .ToList(),
                LazyThreadSafetyMode.PublicationOnly);
            _parameters = new Lazy<ParameterInfo[]>(info.GetParameters, LazyThreadSafetyMode.PublicationOnly);
        }

        /// <summary>
        /// Gets the closed version of a generic method.
        /// </summary>
        /// <param name="genericTypes">The generic types.</param>
        /// <returns>The closed <see cref="Method"/> if the generic types supplied are sufficient for closure; otherwise <see langword="null"/>.</returns>
        /// <remarks></remarks>
        public Method CloseMethod([NotNull]params Type[] genericTypes)
        {
            int length = genericTypes.Length;

            // Check we have exactly the right number of paramters.
            if (_genericArguments.Value.Count != length)
                return null;

            // Substitute missing types with concrete ones.
            Type[] gta = new Type[length];
            for (int i = 0; i < length; i++)
            {
                Type gt = genericTypes[i];

                // Must supply concrete types.
                if (gt == null)
                {
                    // See if we have a concrete type for this index.
                    Type et = _genericArguments.Value[i].Type;
                    if ((et == null) ||
                        (et.IsGenericType))
                        return null;
                    gt = et;
                }
                else if (gt.IsGenericType)
                    return null;

                gta[i] = gt;
            }

            // Create closed method, cache it and return.
            string key = String.Join("|", gta.Select(t => ExtendedType.Get(t).Signature));
            return _closedMethods.Value.GetOrAdd(
                key,
                k =>
                    {
                        try
                        {
                            return new Method(ExtendedType, Info.MakeGenericMethod(gta));
                        }
                        catch (ArgumentException)
                        {
                            return null;
                        }
                    });
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="WebApplications.Utilities.Relection.Method"/> to <see cref="System.Reflection.MethodInfo"/>.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>The result of the conversion.</returns>
        /// <remarks></remarks>
        public static implicit operator MethodInfo(Method method)
        {
            return method == null ? null : method.Info;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Reflection.MethodInfo"/> to <see cref="WebApplications.Utilities.Relection.Method"/>.
        /// </summary>
        /// <param name="methodInfo">The method info.</param>
        /// <returns>The result of the conversion.</returns>
        /// <remarks></remarks>
        public static implicit operator Method(MethodInfo methodInfo)
        {
            return methodInfo == null
                       ? null
                       : ((ExtendedType) methodInfo.DeclaringType).GetMethod(methodInfo);
        }
    }
}