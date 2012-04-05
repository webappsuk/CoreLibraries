using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Relection
{
    /// <summary>
    ///   Wraps the constructor information with accessors for retrieving parameters.
    /// </summary>
    [DebuggerDisplay("{Info} [Extended]")]
    public class Constructor : ISignature
    {
        /// <summary>
        /// The extended type.
        /// </summary>
        [NotNull]
        public readonly ExtendedType ExtendedType;

        /// <summary>
        ///   The constructor info.
        /// </summary>
        [NotNull]
        public readonly ConstructorInfo Info;
        
        /// <summary>
        /// Create enumeration of parameters on demand.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [NotNull]
        private readonly Lazy<ParameterInfo[]> _parameters;
        
        /// <summary>
        ///   The parameters.
        /// </summary>
        [NotNull]
        public IEnumerable<ParameterInfo> Parameters { get { return _parameters.Value; } }

        /// <summary>
        /// Gets the parameters count.
        /// </summary>
        /// <remarks></remarks>
        public int ParametersCount { get { return _parameters.Value.Length; } }

        /// <summary>
        /// Initializes the <see cref="Constructor"/> class.
        /// </summary>
        /// <param name="extendedType">Type of the extended.</param>
        /// <param name="info">The info.</param>
        /// <remarks></remarks>
        internal Constructor([NotNull]ExtendedType extendedType, [NotNull]ConstructorInfo info)
        {
            ExtendedType = extendedType;
            Info = info;
            _parameters = new Lazy<ParameterInfo[]>(info.GetParameters, LazyThreadSafetyMode.PublicationOnly);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="WebApplications.Utilities.Relection.Constructor"/> to <see cref="System.Reflection.ConstructorInfo"/>.
        /// </summary>
        /// <param name="constructor">The constructor.</param>
        /// <returns>The result of the conversion.</returns>
        /// <remarks></remarks>
        public static implicit operator ConstructorInfo(Constructor constructor)
        {
            return constructor == null ? null : constructor.Info;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Reflection.ConstructorInfo"/> to <see cref="WebApplications.Utilities.Relection.Constructor"/>.
        /// </summary>
        /// <param name="constructorInfo">The constructor info.</param>
        /// <returns>The result of the conversion.</returns>
        /// <remarks></remarks>
        public static implicit operator Constructor(ConstructorInfo constructorInfo)
        {
            return constructorInfo == null
                       ? null
                       : ((ExtendedType)constructorInfo.DeclaringType).GetConstructor(constructorInfo);
        }

        /// <inheritdoc/>
        public Type DeclaringType
        {
            get { return ExtendedType.Type; }
        }

        /// <inheritdoc/>
        public IEnumerable<GenericArgument> TypeGenericArguments { get { return ExtendedType.GenericArguments; } }

        /// <inheritdoc/>
        public IEnumerable<GenericArgument> SignatureGenericArguments
        {
            get { return Enumerable.Empty<GenericArgument>(); }
        }

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
        Type ISignature.ReturnType
        {
            get { return ExtendedType.Type; }
        }


        /// <summary>
        /// Closes the constructor with the specified concrete generic types.
        /// </summary>
        /// <param name="typeClosures">The types required to close the current type.</param>
        /// <returns>A closed signature, if possible; otherwise <see langword="null" />.</returns>
        /// <remarks><para>If signature closure is unsupported this method should return <see langword="null" />.</para>
        /// <para>The closure arrays are ordered and contain the same number of elements as their corresponding
        /// generic arguments.  Where elements are <see langword="null"/> a closure is not required.</para></remarks>
        [CanBeNull]
        public Constructor Close([NotNull]Type[] typeClosures)
        {
            // Check input arrays are valid.
            if (typeClosures.Length != ExtendedType.GenericArguments.Count())
                return null;

            // If we haven't got any type closures, we can return this constructor.
            if (!typeClosures.Any(t => t != null))
                return this;

            // Close type
            ExtendedType et = ExtendedType.CloseType(typeClosures);

            // Check closure succeeded.
            if (et == null)
                return null;

            // Create new search.
            Contract.Assert(_parameters.Value != null);
            int pCount = _parameters.Value.Length;
            TypeSearch[] searchTypes = new TypeSearch[pCount];
            Type[] typeGenericArguments = et.GenericArguments.Select(g => g.Type).ToArray();

            // Search for closed 
            for (int i = 0; i < pCount; i++)
            {
                Contract.Assert(_parameters.Value[i] != null);
                Type pType = _parameters.Value[i].ParameterType;
                searchTypes[i] = Reflection.ExpandParameterType(pType, Reflection.EmptyTypes, typeGenericArguments);
            }

            // Search for constructor on new type.
            return et.GetConstructor(searchTypes);
        }

        /// <inheritdoc/>
        ISignature ISignature.Close(Type[] typeClosures, Type[] signatureClosures)
        {
            // Constructors don't support signature closures.
            return signatureClosures.Length != 0 ? null : Close(typeClosures);
        }
    }
}