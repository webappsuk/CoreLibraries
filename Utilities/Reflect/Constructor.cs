#region © Copyright Web Applications (UK) Ltd, 2015.  All rights reserved.
// Copyright (c) 2015, Web Applications UK Ltd
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of Web Applications UK Ltd nor the
//       names of its contributors may be used to endorse or promote products
//       derived from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL WEB APPLICATIONS UK LTD BE LIABLE FOR ANY
// DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using WebApplications.Utilities.Annotations;

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
        /// Initializes the <see cref="Constructor"/> class.
        /// </summary>
        /// <param name="extendedType">Type of the extended.</param>
        /// <param name="info">The info.</param>
        /// <remarks></remarks>
        internal Constructor([NotNull] ExtendedType extendedType, [NotNull] ConstructorInfo info)
        {
            ExtendedType = extendedType;
            Info = info;
            _parameters = new Lazy<ParameterInfo[]>(info.GetParameters, LazyThreadSafetyMode.PublicationOnly);
        }

        /// <summary>
        ///   The parameters.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public IEnumerable<ParameterInfo> Parameters
        {
            get
            {
                Debug.Assert(_parameters.Value != null);
                return _parameters.Value;
            }
        }

        /// <summary>
        /// Gets the parameters count.
        /// </summary>
        /// <remarks></remarks>
        [PublicAPI]
        public int ParametersCount
        {
            get
            {
                Debug.Assert(_parameters.Value != null);
                return _parameters.Value.Length;
            }
        }

        #region ISignature Members
        /// <inheritdoc/>
        public Type DeclaringType
        {
            get { return ExtendedType.Type; }
        }

        /// <inheritdoc/>
        public IEnumerable<GenericArgument> TypeGenericArguments
        {
            get { return ExtendedType.GenericArguments; }
        }

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
                Debug.Assert(_parameters.Value != null);
                // ReSharper disable once PossibleNullReferenceException
                return _parameters.Value.Select(p => p.ParameterType);
            }
        }

        /// <inheritdoc/>
        public Type ReturnType
        {
            get { return ExtendedType.Type; }
        }


        /// <inheritdoc/>
        ISignature ISignature.Close(Type[] typeClosures, Type[] signatureClosures)
        {
            // Constructors don't support signature closures.
            return signatureClosures.Length != 0 ? null : Close(typeClosures);
        }
        #endregion

        /// <summary>
        /// Performs an implicit conversion from <see cref="WebApplications.Utilities.Reflect.Constructor"/> to <see cref="System.Reflection.ConstructorInfo"/>.
        /// </summary>
        /// <param name="constructor">The constructor.</param>
        /// <returns>The result of the conversion.</returns>
        /// <remarks></remarks>
        public static implicit operator ConstructorInfo(Constructor constructor)
        {
            return constructor == null ? null : constructor.Info;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Reflection.ConstructorInfo"/> to <see cref="WebApplications.Utilities.Reflect.Constructor"/>.
        /// </summary>
        /// <param name="constructorInfo">The constructor info.</param>
        /// <returns>The result of the conversion.</returns>
        /// <remarks></remarks>
        public static implicit operator Constructor(ConstructorInfo constructorInfo)
        {
            return constructorInfo == null
                ? null
                // ReSharper disable once PossibleNullReferenceException
                : ((ExtendedType)constructorInfo.DeclaringType).GetConstructor(constructorInfo);
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
        [PublicAPI]
        public Constructor Close([NotNull] Type[] typeClosures)
        {
            if (typeClosures == null) throw new ArgumentNullException("typeClosures");

            // Check input arrays are valid.
            if (typeClosures.Length != ExtendedType.GenericArguments.Count())
                return null;

            // If we haven't got any type closures, we can return this constructor.
            if (typeClosures.All(t => t == null))
                return this;

            // Close type
            ExtendedType et = ExtendedType.CloseType(typeClosures);

            // Check closure succeeded.
            if (et == null)
                return null;

            // Create new search.
            Debug.Assert(_parameters.Value != null);
            int pCount = _parameters.Value.Length;
            TypeSearch[] searchTypes = new TypeSearch[pCount + 1];
            Type[] typeGenericArguments = et.GenericArguments.Select(g => g.Type).ToArray();
            // Search for closed 
            for (int i = 0; i < pCount; i++)
            {
                Debug.Assert(_parameters.Value[i] != null);
                Type pType = _parameters.Value[i].ParameterType;
                Debug.Assert(pType != null);
                searchTypes[i] = Reflection.ExpandParameterType(pType, Array<Type>.Empty, typeGenericArguments);
            }

            // Add return type
            searchTypes[pCount] = et.Type;

            // Search for constructor on new type.
            return et.GetConstructor(searchTypes);
        }

        /// <summary>
        /// Gets the lambda function equivalent of a method, for much better runtime performance than an invocation.
        /// </summary>
        /// <param name="funcTypes">The parameter types, followed by the return type.</param>
        /// <returns>A functional equivalent of the specified method info.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="ArgumentOutOfRangeException">
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

            ParameterInfo[] parameters = _parameters.Value;
            Debug.Assert(parameters != null);

            // Now add parameter types.
            int pCount = parameters.Length;
            for (; a < tCount - 1; a++)
            {
                // Check if we run out of parameters.
                if (a >= pCount)
                    return null;

                Debug.Assert(parameters[a] != null);
                // Funcs don't support output, pointer, or by reference parameters
                if (parameters[a].IsOut ||
                    parameters[a].ParameterType.IsByRef ||
                    parameters[a].ParameterType.IsPointer)
                    return null;
                signatureTypes[a] = parameters[a].ParameterType;
            }

            // Any remaining parameters must be optional.
            // ReSharper disable once PossibleNullReferenceException
            if ((a < pCount) &&
                (!parameters[a].IsOptional))
                return null;

            // Create expressions
            ParameterExpression[] parameterExpressions = new ParameterExpression[tCount - 1];
            Expression[] pExpressions = new Expression[tCount - 1];
            for (int i = 0; i < tCount - 1; i++)
            {
                Type funcType = funcTypes[i];
                Type signatureType = signatureTypes[i];

                Debug.Assert(funcType != null);
                Debug.Assert(signatureType != null);

                // Create parameter
                parameterExpressions[i] = Expression.Parameter(funcType);
                if (funcType != signatureType)
                {
                    // Try to convert from the input funcType to the underlying parameter type.
                    if (!parameterExpressions[i].TryConvert(signatureType, out pExpressions[i]))
                        return null;
                }
                else
                // No conversion necessary.
                    pExpressions[i] = parameterExpressions[i];
            }

            // Create call expression, instance methods use the first parameter of the Func<> as the instance, static
            // methods do not supply an instance.
            Expression expression = Expression.New(Info, pExpressions);

            // Check if we need to do a cast to the func result type
            if (funcTypes[tCount - 1] != ExtendedType.Type &&
                // ReSharper disable once AssignNullToNotNullAttribute
                !expression.TryConvert(funcTypes[tCount - 1], out expression))
                return null;

            return Expression.Lambda(expression, parameterExpressions).Compile();
        }
    }
}