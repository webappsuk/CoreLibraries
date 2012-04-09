using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Reflect
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
        public Type ReturnType
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
            TypeSearch[] searchTypes = new TypeSearch[pCount + 1];
            Type[] typeGenericArguments = et.GenericArguments.Select(g => g.Type).ToArray();
            // Search for closed 
            for (int i = 0; i < pCount; i++)
            {
                Contract.Assert(_parameters.Value[i] != null);
                Type pType = _parameters.Value[i].ParameterType;
                Contract.Assert(pType != null);
                searchTypes[i] = Reflection.ExpandParameterType(pType, Reflection.EmptyTypes, typeGenericArguments);
            }

            // Add return type
            searchTypes[pCount] = et.Type;

            // Search for constructor on new type.
            return et.GetConstructor(searchTypes);
        }

        /// <inheritdoc/>
        ISignature ISignature.Close(Type[] typeClosures, Type[] signatureClosures)
        {
            // Constructors don't support signature closures.
            return signatureClosures.Length != 0 ? null : Close(typeClosures);
        }

        /// <summary>
        /// Gets the lambda function equivalent of a method, for much better runtime performance than an invocation.
        /// </summary>
        /// <param name="funcTypes">The parameter types, followed by the return type.</param>
        /// <returns>A functional equivalent of the specified method info.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="methodBase" /> is a <see langword="null" />.
        ///   </exception>
        ///   <exception cref="ArgumentOutOfRangeException">
        ///   <para>The method specified is a static constructor.</para>
        ///   <para>-or-</para>
        ///   <para>No parameter/return types specified.</para>
        ///   <para>-or-</para>
        ///   <para>The method specified doesn't return a value. (An Action should be used instead.)</para>
        ///   <para>-or-</para>
        ///   <para>The number of parameters specified in <paramref name="funcTypes" /> is incorrect.</para>
        ///   <para>-or-</para>
        ///   <para>The parameter type is not assignable to the type specified.</para>
        ///   <para>-or-</para>
        ///   <para>The return type is not assignable to the type specified.</para>
        ///   </exception>
        ///   <exception cref="InvalidOperationException">No parameter/return types specified.</exception>
        /// <remarks></remarks>
        [CanBeNull]
        [UsedImplicitly]
        public object GetFunc([NotNull] params Type[] funcTypes)
        {
            // Can't construct statics or open types.
            if ((Info.IsStatic) ||
                (ExtendedType.Type.ContainsGenericParameters))
                return null;

            int tCount = funcTypes.Count();
            // Create array for required func types - statics take an instance as the first parameter
            Type[] signatureTypes = new Type[tCount - 1];
            int a = 0;
            // Now add parameter types.
            int pCount = _parameters.Value.Length;
            for (; a < tCount - 1; a++)
            {
                // Check if we run out of parameters.
                if (a >= pCount)
                    return null;

                // Funcs don't support output, pointer, or by reference parameters
                if (_parameters.Value[a].IsOut ||
                    _parameters.Value[a].ParameterType.IsByRef ||
                    _parameters.Value[a].ParameterType.IsPointer)
                    return null;
                signatureTypes[a] = _parameters.Value[a].ParameterType;
            }

            // Any remaining parameters must be optional.
            if ((a < pCount) &&
                (!_parameters.Value[a].IsOptional))
                return null;

            // Create expressions
            ParameterExpression[] parameterExpressions = new ParameterExpression[tCount - 1];
            Expression[] pExpressions = new Expression[tCount - 1];
            for (int i = 0; i < tCount - 1; i++)
            {
                Type funcType = funcTypes[i];
                Type signatureType = signatureTypes[i];

                // Create parameter
                parameterExpressions[i] = Expression.Parameter(funcType);
                if (funcType != signatureType)
                {
                    // Try to convert from the input funcType to the underlying parameter type.
                    if (!parameterExpressions[i].TryConvert(signatureType, out pExpressions[i]))
                        return null;
                }
                else
                {
                    // No conversion necessary.
                    pExpressions[i] = parameterExpressions[i];
                }
            }

            // Create call expression, instance methods use the first parameter of the Func<> as the instance, static
            // methods do not supply an instance.
            Expression expression = Expression.New(Info, pExpressions);

            // Check if we need to do a cast to the func result type
            if (funcTypes[tCount - 1] != ExtendedType.Type &&
                !expression.TryConvert(funcTypes[tCount - 1], out expression))
                return null;

            return Expression.Lambda(expression, parameterExpressions).Compile();
        }
    }
}