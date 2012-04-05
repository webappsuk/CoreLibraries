using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
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

        /// <inheritdoc/>
        public Type DeclaringType
        {
            get { return ExtendedType.Type; }
        }

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
            get
            {
                Contract.Assert(_parameters.Value != null);
                return _parameters.Value.Select(p => p.ParameterType);
            }
        }

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

        /// <inheritdoc/>
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
        /// <param name="signatureClosures">The types required to close the method's generic arguments.</param>
        /// <returns>The closed <see cref="Method"/> if the generic types supplied are sufficient for closure; otherwise <see langword="null"/>.</returns>
        /// <remarks></remarks>
        [CanBeNull]
        public Method Close([NotNull]params Type[] signatureClosures)
        {
            return Close(new Type[ExtendedType.GenericArguments.Count()], signatureClosures);
        }

        /// <summary>
        /// Gets the closed version of a generic method.
        /// </summary>
        /// <param name="typeClosures">The types required to close the current type.</param>
        /// <param name="signatureClosures">The types required to close the method's generic arguments.</param>
        /// <returns>The closed <see cref="Method"/> if the generic types supplied are sufficient for closure; otherwise <see langword="null"/>.</returns>
        /// <remarks></remarks>
        [CanBeNull]
        public Method Close([NotNull]Type[] typeClosures, [NotNull]Type[] signatureClosures)
        {
            Contract.Assert(_genericArguments.Value != null);

            // Check input arrays are valid.
            if ((typeClosures.Length != ExtendedType.GenericArguments.Count()) ||
                (signatureClosures.Length != _genericArguments.Value.Count()))
                return null;
            
            // If we have any type closures then we need to close the type and look for the method on there.
            if (typeClosures.Any(t => t != null))
            {
                // Close type
                ExtendedType et = ExtendedType.CloseType(typeClosures);
                
                // Check closure succeeded.
                if (et == null)
                    return null;

                // Create new search.
                Contract.Assert(_parameters.Value != null);
                int pCount = _parameters.Value.Length;
                TypeSearch[] searchTypes = new TypeSearch[pCount + 1];
                Type[] typeGenericArguments = et.GenericArguments.Select(g => g.Type).ToArray();
                
                Type gType;
                // Search for closed 
                for (int i = 0; i < pCount; i++)
                {
                    Contract.Assert(_parameters.Value[i] != null);
                    Type pType = _parameters.Value[i].ParameterType;
                    Contract.Assert(pType != null);
                    searchTypes[i] = Reflection.ExpandParameterType(pType, signatureClosures, typeGenericArguments);
                }

                // Add return type
                Type rType = Info.ReturnType;
                searchTypes[pCount] = Reflection.ExpandParameterType(rType, signatureClosures, typeGenericArguments);

                // Search for method on new type.
                return et.GetMethod(Info.Name, signatureClosures.Length, searchTypes);
            }

            // Substitute missing types with concrete ones.
            int closures = signatureClosures.Length;
            Type[] gta = new Type[closures];
            for (int i = 0; i < closures; i++)
                gta[i] = signatureClosures[i] ?? _genericArguments.Value[i].Type;

            // Create closed method, cache it and return.
            string key = String.Join("|", gta.Select(t => ExtendedType.Get(t).Signature));

            Contract.Assert(_closedMethods.Value != null);
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

        /// <inheritdoc/>
        ISignature ISignature.Close(Type[] typeClosures, Type[] signatureClosures)
        {
            return Close(typeClosures, signatureClosures);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Method"/> to <see cref="System.Reflection.MethodInfo"/>.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>The result of the conversion.</returns>
        /// <remarks></remarks>
        public static implicit operator MethodInfo(Method method)
        {
            return method == null ? null : method.Info;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Reflection.MethodInfo"/> to <see cref="Method"/>.
        /// </summary>
        /// <param name="methodInfo">The method info.</param>
        /// <returns>The result of the conversion.</returns>
        /// <remarks></remarks>
        public static implicit operator Method(MethodInfo methodInfo)
        {
            if (methodInfo == null)
                return null;
            ExtendedType et = methodInfo.DeclaringType;
            Contract.Assert(et != null);
            return et.GetMethod(methodInfo);
        }
    }
}