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
using System.Collections.Concurrent;
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
    ///   Wraps the method information with accessors for retrieving parameters.
    /// </summary>
    [DebuggerDisplay("{Info} [Extended]")]
    [PublicAPI]
    public class Method : ISignature
    {
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
        /// Creates array of generic arguments on demand.
        /// </summary>
        [NotNull]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Lazy<List<GenericArgument>>
            _genericArguments;

        /// <summary>
        /// Create enumeration of parameters on demand.
        /// </summary>
        [NotNull]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Lazy<ParameterInfo[]> _parameters;

        /// <summary>
        /// Caches closed methods.
        /// </summary>
        [NotNull]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Lazy<ConcurrentDictionary<string, Method>> _closedMethods =
            new Lazy<ConcurrentDictionary<string, Method>>(
                () => new ConcurrentDictionary<string, Method>(),
                LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// Initializes the <see cref="Method"/> class.
        /// </summary>
        /// <param name="extendedType">Type of the extended.</param>
        /// <param name="info">The info.</param>
        /// <remarks></remarks>
        internal Method([NotNull] ExtendedType extendedType, [NotNull] MethodInfo info)
        {
            ExtendedType = extendedType;
            Info = info;
            _genericArguments = new Lazy<List<GenericArgument>>(
                () => info.GetGenericArguments()
                    // ReSharper disable once AssignNullToNotNullAttribute
                    .Select((g, i) => new GenericArgument(GenericArgumentLocation.Signature, i, g))
                    .ToList(),
                LazyThreadSafetyMode.PublicationOnly);
            _parameters = new Lazy<ParameterInfo[]>(info.GetParameters, LazyThreadSafetyMode.PublicationOnly);
        }

        /// <summary>
        ///   The parameters.
        /// </summary>
        [NotNull]
        public IEnumerable<ParameterInfo> Parameters
        {
            get
            {
                Debug.Assert(_parameters.Value != null);
                return _parameters.Value;
            }
        }

        #region ISignature Members
        /// <inheritdoc/>
        public Type DeclaringType
        {
            get { return ExtendedType.Type; }
        }

        /// <inheritdoc/>
        [NotNull]
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
        public IEnumerable<GenericArgument> TypeGenericArguments
        {
            get { return ExtendedType.GenericArguments; }
        }

        /// <inheritdoc/>
        public IEnumerable<GenericArgument> SignatureGenericArguments
        {
            get { return _genericArguments.Value; }
        }

        /// <inheritdoc/>
        public Type ReturnType
        {
            get { return Info.ReturnType; }
        }

        /// <inheritdoc/>
        ISignature ISignature.Close(Type[] typeClosures, Type[] signatureClosures)
        {
            return Close(typeClosures, signatureClosures);
        }
        #endregion

        /// <summary>
        /// Gets the closed version of a generic method.
        /// </summary>
        /// <param name="signatureClosures">The types required to close the method's generic arguments.</param>
        /// <returns>The closed <see cref="Method"/> if the generic types supplied are sufficient for closure; otherwise <see langword="null"/>.</returns>
        /// <remarks></remarks>
        [CanBeNull]
        public Method Close([NotNull] params Type[] signatureClosures)
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
        public Method Close([NotNull] Type[] typeClosures, [NotNull] Type[] signatureClosures)
        {
            if (typeClosures == null) throw new ArgumentNullException("typeClosures");
            if (signatureClosures == null) throw new ArgumentNullException("signatureClosures");

            Debug.Assert(_genericArguments.Value != null);

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
            // ReSharper disable once AssignNullToNotNullAttribute
            string key = String.Join("|", gta.Select(t => ExtendedType.Get(t).Signature));

            Debug.Assert(_closedMethods.Value != null);
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
            Debug.Assert(et != null);
            return et.GetMethod(methodInfo);
        }

        /// <summary>
        /// Gets the lambda function equivalent of a method, for much better runtime performance than an invocation.
        /// </summary>
        /// <param name="funcTypes">The parameter types.</param>
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
        public object GetAction([NotNull] params Type[] funcTypes)
        {
            // If the method is not closed or doesn't have return type, were' done.
            if (Info.ContainsGenericParameters)
                return null;

            bool isStatic = Info.IsStatic;

            int tCount = funcTypes.Count();
            // Create array for required func types - statics take an instance as the first parameter
            Type[] signatureTypes = new Type[tCount];
            int a = 0;
            if (!isStatic)
            {
                if (tCount < 1)
                    return null;
                signatureTypes[a++] = ExtendedType.Type;
            }

            // Now add parameter types.
            int p = 0;

            ParameterInfo[] parameters = _parameters.Value;
            Debug.Assert(parameters != null);

            int pCount = parameters.Length;
            for (; a < tCount; a++)
            {
                // Check if we run out of parameters.
                if (p >= pCount)
                    return null;

                Debug.Assert(parameters[p] != null);

                // actions don't support output, pointer, or by reference parameters
                if (parameters[p].IsOut ||
                    parameters[p].ParameterType.IsByRef ||
                    parameters[p].ParameterType.IsPointer)
                    return null;

                // ReSharper disable once PossibleNullReferenceException
                signatureTypes[a] = parameters[p++].ParameterType;
            }

            // Any remaining parameters must be optional.
            // ReSharper disable once PossibleNullReferenceException
            if ((p < pCount) &&
                (!parameters[p].IsOptional))
                return null;

            // Create expressions
            ParameterExpression[] parameterExpressions = new ParameterExpression[tCount];
            Expression[] pExpressions = new Expression[tCount];
            for (int i = 0; i < tCount; i++)
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
            Expression expression = isStatic
                ? Expression.Call(Info, pExpressions)
                : Expression.Call(pExpressions[0], Info, pExpressions.Skip(1));

            // Check if we have a return type.
            if (Info.ReturnType != typeof(void))
            {
                // Discard result by wrapping in a block and 'returning' void.
                LabelTarget returnTarget = Expression.Label();
                expression = Expression.Block(
                    expression,
                    Expression.Return(returnTarget),
                    Expression.Label(returnTarget));
            }

            return Expression.Lambda(expression, parameterExpressions).Compile();
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
        public object GetFunc([NotNull] params Type[] funcTypes)
        {
            // If the method is not closed or doesn't have return type, were' done.
            if ((Info.ContainsGenericParameters) ||
                (Info.ReturnType == typeof(void)))
                return null;

            bool isStatic = Info.IsStatic;

            int tCount = funcTypes.Count();
            // Create array for required func types - statics take an instance as the first parameter
            Type[] signatureTypes = new Type[tCount - 1];
            int a = 0;
            if (!isStatic)
            {
                if (tCount < 1)
                    return null;
                signatureTypes[a++] = ExtendedType.Type;
            }

            // Now add parameter types.
            int p = 0;
            ParameterInfo[] parameters = _parameters.Value;
            Debug.Assert(parameters != null);

            int pCount = parameters.Length;
            for (; a < tCount - 1; a++)
            {
                // Check if we run out of parameters.
                if (p >= pCount)
                    return null;

                Debug.Assert(parameters[p] != null);

                // Func's don't support output, pointer, or by reference parameters
                if (parameters[p].IsOut ||
                    parameters[p].ParameterType.IsByRef ||
                    parameters[p].ParameterType.IsPointer)
                    return null;

                // ReSharper disable once PossibleNullReferenceException
                signatureTypes[a] = parameters[p++].ParameterType;
            }

            // Any remaining parameters must be optional.
            // ReSharper disable once PossibleNullReferenceException
            if ((p < pCount) &&
                (!parameters[p].IsOptional))
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
            Expression expression = isStatic
                ? Expression.Call(Info, pExpressions)
                : Expression.Call(pExpressions[0], Info, pExpressions.Skip(1));

            // Check if we need to do a cast to the func result type
            if (funcTypes[tCount - 1] != Info.ReturnType &&
                // ReSharper disable once AssignNullToNotNullAttribute
                !expression.TryConvert(funcTypes[tCount - 1], out expression))
                return null;

            return Expression.Lambda(expression, parameterExpressions).Compile();
        }
    }
}