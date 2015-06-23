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
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Reflect;

namespace WebApplications.Utilities
{
    /// <summary>
    ///   Extensions to the <see cref="System.Reflection">reflection namespace</see>.
    /// </summary>
    [PublicAPI]
    public static partial class Reflection
    {
        /// <summary>
        ///   Binding flags for returning all fields/properties from a type.
        /// </summary>
        public const BindingFlags AccessorBindingFlags =
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static |
            BindingFlags.FlattenHierarchy;

        /// <summary>
        /// Cache the conversions so that when requested they can be retrieved rather than recomputed.
        /// </summary>
        [NotNull]
        private static readonly ConcurrentDictionary<string, object> _converters =
            new ConcurrentDictionary<string, object>();

        /// <summary>
        /// The <see cref="MethodInfo"/> for <see cref="object.ToString()"/>.
        /// </summary>
        [NotNull]
        public static readonly MethodInfo ToStringMethodInfo = InfoHelper.GetMethodInfo<object>(o => o.ToString(), true);

        /// <summary>
        ///   The <see cref="Expression"/> to get the <see cref="CultureInfo.CurrentCulture">current CultureInfo</see>.
        /// </summary>
        [NotNull]
        public static readonly Expression CurrentCultureExpression =
            Expression.Property(null, InfoHelper.GetPropertyInfo(() => CultureInfo.CurrentCulture, true));

        /// <summary>
        /// Retrieves the lambda function equivalent of the specified property/field getter static method.
        /// </summary>
        /// <typeparam name="TValue">The type of the value returned.</typeparam>	
        /// <param name="type">The type from which to retrieve the getter..</param>
        /// <param name="name">The name of the field or property whose getter we want to retrieve.</param>
        /// <returns>A function that takes an object of the type T and returns the value of the property or field.</returns>
        /// <remarks></remarks>
        [CanBeNull]
        public static Func<TValue> GetGetter<TValue>([NotNull] this Type type, [NotNull] string name)
        {
            ExtendedType et = type;
            Field field = et.GetField(name);
            if (field != null)
                return field.Getter<TValue>();

            Property property = et.GetProperty(name);
            return property == null ? null : property.Getter<TValue>();
        }

        /// <summary>
        /// Retrieves the lambda function equivalent of the specified property/field getter instance method.
        /// </summary>
        /// <typeparam name="T">The type of the parameter the function encapsulates.</typeparam>	
        /// <typeparam name="TValue">The type of the value returned.</typeparam>	
        /// <param name="type">The type from which to retrieve the getter..</param>
        /// <param name="name">The name of the field or property whose getter we want to retrieve.</param>
        /// <returns>A function that takes an object of the type T and returns the value of the property or field.</returns>
        /// <remarks></remarks>
        [CanBeNull]
        public static Func<T, TValue> GetGetter<T, TValue>([NotNull] this Type type, [NotNull] string name)
        {
            ExtendedType et = type;
            Field field = et.GetField(name);
            if (field != null)
                return field.Getter<T, TValue>();

            Property property = et.GetProperty(name);
            return property == null ? null : property.Getter<T, TValue>();
        }

        /// <summary>
        /// Retrieves the lambda action equivalent of the specified static setter method.
        /// </summary>
        /// <typeparam name="TValue">The type of the value returned.</typeparam>	
        /// <param name="type">The type.</param>
        /// <param name="name">The name of the field or property whose setter we want to retrieve.</param>
        /// <returns>A function that takes an object of the declaring type as well as a value and sets the field or property to that value.</returns>
        /// <remarks></remarks>
        [CanBeNull]
        public static Action<TValue> GetSetter<TValue>([NotNull] this Type type, [NotNull] string name)
        {
            ExtendedType et = type;
            Field field = et.GetField(name);
            if (field != null)
                return field.Setter<TValue>();

            Property property = et.GetProperty(name);
            return property == null ? null : property.Setter<TValue>();
        }

        /// <summary>
        /// Retrieves the lambda action equivalent of the specified instance setter method.
        /// </summary>
        /// <typeparam name="T">The declaring class' type.</typeparam>	
        /// <typeparam name="TValue">The type of the value returned.</typeparam>	
        /// <param name="type">The type.</param>
        /// <param name="name">The name of the field or property whose setter we want to retrieve.</param>
        /// <returns>A function that takes an object of the declaring type as well as a value and sets the field or property to that value.</returns>
        /// <remarks></remarks>
        [CanBeNull]
        public static Action<T, TValue> GetSetter<T, TValue>([NotNull] this Type type, [NotNull] string name)
        {
            ExtendedType et = type;
            Field field = et.GetField(name);
            if (field != null)
                return field.Setter<T, TValue>();

            Property property = et.GetProperty(name);
            return property == null ? null : property.Setter<T, TValue>();
        }

        /// <summary>
        ///   Gets the lambda function equivalent of a method, for much better runtime performance than an invocation.
        /// </summary>
        /// <param name="methodBase">The information about the method.</param>
        /// <param name="checkParameterAssignability">
        ///   If set to <see langword="true"/> checks that the function parameters can be assigned to the method safely.
        /// </param>
        /// <param name="funcTypes">The parameter types, followed by the return type.</param>
        /// <returns>
        ///   A functional equivalent of the specified method info.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="methodBase"/> is a <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <para>The method specified is a static constructor.</para>
        ///   <para>-or-</para>
        ///   <para>No parameter/return types specified.</para>
        ///   <para>-or-</para>
        ///   <para>The method specified doesn't return a value. (An Action should be used instead.)</para>
        ///   <para>-or-</para>
        ///   <para>The number of parameters specified in <paramref name="funcTypes"/> is incorrect.</para>
        ///   <para>-or-</para>
        ///   <para>The parameter type is not assignable to the type specified.</para>
        ///   <para>-or-</para>
        ///   <para>The return type is not assignable to the type specified.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">No parameter/return types specified.</exception>
        [NotNull]
        public static object GetFunc(
            [NotNull] this MethodBase methodBase,
            bool checkParameterAssignability,
            [NotNull] params Type[] funcTypes)
        {
            if (methodBase == null) throw new ArgumentNullException("methodBase");
            if (funcTypes == null) throw new ArgumentNullException("funcTypes");

            bool isConstructor = methodBase.IsConstructor;
            bool isStatic = methodBase.IsStatic;
            MethodInfo methodInfo;
            ConstructorInfo constructorInfo;
            if (isConstructor)
            {
                // Cannot support static constructors (cannot be called directly!)
                if (isStatic)
                    throw new ArgumentOutOfRangeException(
                        "methodBase",
                        String.Format(
                            // ReSharper disable once AssignNullToNotNullAttribute
                            Resources.Reflection_GetFunc_MethodIsStaticConstructor,
                            methodBase));

                constructorInfo = methodBase as ConstructorInfo;
                Debug.Assert(constructorInfo != null);
                methodInfo = null;
            }
            else
            {
                methodInfo = methodBase as MethodInfo;
                constructorInfo = null;
                if ((methodInfo == null) ||
                    (methodInfo.ReturnType == typeof(void)))
                    throw new ArgumentOutOfRangeException(
                        "methodBase",
                        String.Format(
                            // ReSharper disable once AssignNullToNotNullAttribute
                            Resources.Reflection_GetFunc_MethodHasNoReturnType,
                            methodBase));
            }

            int count = funcTypes.Count();
            if (count < 1)
                throw new ArgumentOutOfRangeException(
                    "funcTypes",
                    String.Format(
                        // ReSharper disable once AssignNullToNotNullAttribute
                        Resources.Reflection_GetFunc_NoFuncTypesSpecified,
                        methodBase));

            // Validate method info
            ParameterInfo[] parameterInfos = methodBase.GetParameters();
            List<Type> methodTypes = new List<Type>(count);

            // If we're not static and not a constructor, the first parameter is implicitly the declaring type of the method.
            if (!isStatic &&
                !isConstructor)
                methodTypes.Add(methodBase.DeclaringType);
            // ReSharper disable PossibleNullReferenceException
            methodTypes.AddRange(parameterInfos.Select(pi => pi.ParameterType));
            // ReSharper restore PossibleNullReferenceException

            // Finally add return type - in the case of a constructor, this is the object type.
            methodTypes.Add(isConstructor ? methodBase.ReflectedType : methodInfo.ReturnType);

            if (methodTypes.Count() != count)
                throw new ArgumentOutOfRangeException(
                    "methodBase",
                    String.Format(
                        // ReSharper disable once AssignNullToNotNullAttribute
                        Resources.Reflection_GetFunc_IncorrectParameterCount,
                        methodBase));

            // Create expressions
            ParameterExpression[] parameterExpressions = new ParameterExpression[count - 1];
            List<Expression> pExpressions = new List<Expression>(count - 1);
            for (int i = 0; i < count; i++)
            {
                Type funcType = funcTypes[i];
                Type methodType = methodTypes[i];

                Debug.Assert(funcType != null);
                Debug.Assert(methodType != null);

                Expression expression;
                // Create parameter expressions for all
                if (i < count - 1)
                {
                    // Check assignability
                    if (checkParameterAssignability && !methodType.IsAssignableFrom(funcType))
                        throw new ArgumentOutOfRangeException(
                            "methodBase",
                            String.Format(
                                // ReSharper disable once AssignNullToNotNullAttribute
                                Resources.Reflection_GetFunc_ParameterNotAssignable,
                                methodBase,
                                funcType,
                                methodType));

                    // Create parameter expression
                    expression = parameterExpressions[i] = Expression.Parameter(funcType);

                    // Check if we need to do a cast to the method type
                    if (funcType != methodType)
                        expression = expression.Convert(methodType);

                    pExpressions.Add(expression);
                    continue;
                }

                // Check assignability
                if (checkParameterAssignability && !funcType.IsAssignableFrom(methodType))
                    throw new ArgumentOutOfRangeException(
                        "methodBase",
                        String.Format(
                            // ReSharper disable once AssignNullToNotNullAttribute
                            Resources.Reflection_GetFunc_ReturnTypeNotAssignable,
                            methodBase,
                            methodType,
                            funcType));

                if (isConstructor)
                    // We are a constructor so use the New expression.
                    expression = Expression.New(constructorInfo, pExpressions);
                else
                // Create call expression, instance methods use the first parameter of the Func<> as the instance, static
                // methods do not supply an instance.
                    expression = isStatic
                        ? Expression.Call(methodInfo, pExpressions)
                        : Expression.Call(
                            pExpressions[0],
                            methodInfo,
                            pExpressions.Skip(1));

                // Check if we need to do a cast to the func result type
                if (funcType != methodType)
                    expression = expression.Convert(funcType);

                return Expression.Lambda(expression, parameterExpressions).Compile();
            }
            // Sanity check, shouldn't be able to get here anyway.
            throw new InvalidOperationException(
                String.Format(
                    // ReSharper disable once AssignNullToNotNullAttribute
                    Resources.Reflection_GetFunc_NoFuncTypesSpecified,
                    methodBase));
        }

        /// <summary>
        ///   Gets the lambda action equivalent of a method, for much better runtime performance than an invocation.
        /// </summary>
        /// <param name="methodInfo">The information about the method.</param>
        /// <param name="checkParameterAssignability">
        ///   If set to <see langword="true"/> checks that the function parameters can be assigned to the method safely.
        /// </param>
        /// <param name="paramTypes">The parameter types.</param>
        /// <returns>
        ///   A functional equivalent of the specified method info.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="methodInfo"/> is a <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <para>The number of parameters specified in <paramref name="paramTypes"/> is incorrect.</para>
        ///   <para>-or-</para>
        ///   <para>The parameter type is not assignable to the type specified.</para>
        /// </exception>
        [NotNull]
        public static object GetAction(
            [NotNull] this MethodInfo methodInfo,
            bool checkParameterAssignability,
            [NotNull] params Type[] paramTypes)
        {
            if (methodInfo == null) throw new ArgumentNullException("methodInfo");
            if (paramTypes == null) throw new ArgumentNullException("paramTypes");

            bool isStatic = methodInfo.IsStatic;

            int count = paramTypes.Count();

            // Validate method info
            ParameterInfo[] parameterInfos = methodInfo.GetParameters();
            List<Type> methodTypes = new List<Type>(count);

            // If we're not static the first parameter is implicitly the declaring type of the method.
            if (!isStatic)
                methodTypes.Add(methodInfo.DeclaringType);
            // ReSharper disable once PossibleNullReferenceException
            methodTypes.AddRange(parameterInfos.Select(pi => pi.ParameterType));

            if (methodTypes.Count() != count)
                throw new ArgumentOutOfRangeException(
                    "methodInfo",
                    String.Format(
                        // ReSharper disable once AssignNullToNotNullAttribute
                        Resources.Reflection_GetAction_IncorrectParameterCount,
                        methodInfo));

            // Create expressions
            ParameterExpression[] parameterExpressions = new ParameterExpression[count];
            List<Expression> pExpressions = new List<Expression>(count);

            Expression expression;
            for (int i = 0; i < count; i++)
            {
                Type funcType = paramTypes[i];
                Type methodType = methodTypes[i];

                Debug.Assert(funcType != null);
                Debug.Assert(methodType != null);

                // Check assignability
                if (checkParameterAssignability && !methodType.IsAssignableFrom(funcType))
                    throw new ArgumentOutOfRangeException(
                        "methodInfo",
                        String.Format(
                            // ReSharper disable once AssignNullToNotNullAttribute
                            Resources.Reflection_GetAction_ParameterNotAssignable,
                            methodInfo,
                            funcType,
                            methodType));

                // Create parameter expression
                expression = parameterExpressions[i] = Expression.Parameter(funcType);

                // Check if we need to do a cast to the method type
                if (funcType != methodType)
                    expression = expression.Convert(methodType);

                pExpressions.Add(expression);
            }


            // Create call expression, instance methods use the first parameter of the action as the instance, static
            // methods do not supply an instance.
            expression = isStatic
                ? Expression.Call(methodInfo, pExpressions)
                : Expression.Call(
                    pExpressions[0],
                    methodInfo,
                    pExpressions.Skip(1));
            return
                Expression.Lambda(Expression.Block(expression), parameterExpressions).
                    Compile();
        }

        /// <summary>
        ///   Gets the lambda function equivalent of a constructor, for much better runtime performance than an invocation.
        /// </summary>
        /// <param name="type">The object type.</param>
        /// <param name="checkParameterAssignability">
        ///   If set to <see langword="true"/> checks that the function parameters can be assigned to the method safely.
        /// </param>
        /// <param name="paramTypes">The parameter types.</param>
        /// <returns>
        ///   A functional equivalent of a constructor.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="type"/> is a <see langword="null"/>.
        /// </exception>
        [NotNull]
        public static object GetConstructorFunc(
            [NotNull] this Type type,
            bool checkParameterAssignability,
            [NotNull] params Type[] paramTypes)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (paramTypes == null) throw new ArgumentNullException("paramTypes");

            ConstructorInfo constructor = type.GetConstructor(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
                BindingFlags.CreateInstance,
                null,
                paramTypes.Take(paramTypes.Length - 1).ToArray(),
                null);

            if (constructor == null)
                throw new ArgumentException("Constructor not found");

            return GetFunc(
                constructor,
                checkParameterAssignability,
                paramTypes);
        }

        /// <summary>
        ///   Determines whether or not the specified <see cref="PropertyInfo">property</see> is automatic.
        /// </summary>
        /// <param name="property">The property we want to look at.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the specified property is automatic; otherwise <see langword="false"/>.
        /// </returns>
        public static bool IsAutomatic([NotNull] this PropertyInfo property)
        {
            Property p = property;
            return p.IsAutomatic;
        }

        /// <summary>
        ///   Determines whether or not the specified <see cref="PropertyInfo">property</see> is automatic
        ///   and returns its underlying field.
        /// </summary>
        /// <param name="property">The property we want to look at.</param>
        /// <returns>
        ///   Returns the field info if the property is automatic; otherwise returns null.
        /// </returns>
        [CanBeNull]
        public static Field GetAutomaticFieldInfo([NotNull] this PropertyInfo property)
        {
            Property p = property;
            return p.AutomaticField;
        }

        /// <summary>
        ///   Gets the conversion from the input type to the output type as a lambda expression.
        /// </summary>
        /// <typeparam name="TIn">The type to convert from.</typeparam>
        /// <typeparam name="TOut">The type to convert to.</typeparam>
        /// <returns>A lambda expression for converting (if any); otherwise returns <see langword="null"/>.</returns>
        /// <remarks>
        ///   Conversions are cached, which saves a conversion being recomputed if it's requested more than once.
        /// </remarks>
        public static Func<TIn, TOut> GetConversion<TIn, TOut>()
        {
            return GetConversion<TIn, TOut>(typeof(TIn));
        }

        /// <summary>
        /// Gets the conversion from the input type to the output type as a lambda expression.
        /// </summary>
        /// <returns>A lambda expression for converting (if any); otherwise returns <see langword="null"/>.</returns>
        /// <remarks>
        ///   Conversions are cached, which saves a conversion being recomputed if it's requested more than once.
        /// </remarks>
        public static Func<object, object> GetConversion()
        {
            return GetConversion<object, object>(typeof(object));
        }

        /// <summary>
        ///   Gets the conversion from the input type to the output type as a lambda expression.
        /// </summary>
        /// <param name="inputType">The type to convert from.</param>
        /// <param name="outputType">The type to convert to.</param>
        /// <returns>A lambda expression for converting between the two types (if any); otherwise returns <see langword="null"/>.</returns>
        /// <remarks>
        ///   Conversions are cached, which saves a conversion being recomputed if it's requested more than once.
        /// </remarks>
        public static Func<object, object> GetConversion([NotNull] this Type inputType, Type outputType = null)
        {
            if (inputType == null) throw new ArgumentNullException("inputType");
            return GetConversion<object, object>(inputType, outputType);
        }

        /// <summary>
        ///   Gets the conversion from the input type to the output type as a lambda expression.
        /// </summary>
        /// <typeparam name="TIn">The type of the input for the function.</typeparam>
        /// <typeparam name="TOut">The type of the output for the function.</typeparam>
        /// <param name="inputType">The type to convert from.</param>
        /// <param name="outputType">The type to convert to.</param>
        /// <returns>A lambda expression for converting (if any); otherwise null.</returns>
        /// <remarks>
        ///   Conversions are cached, which saves a conversion being recomputed if it's requested more than once.
        /// </remarks>
        public static Func<TIn, TOut> GetConversion<TIn, TOut>([NotNull] this Type inputType, Type outputType = null)
        {
            if (inputType == null) throw new ArgumentNullException("inputType");

            if (outputType == null)
                outputType = typeof(TOut);

            return (Func<TIn, TOut>)_converters.GetOrAdd(
                String.Format(
                    "{0}|{1}|{2}|{3}",
                    typeof(TIn).FullName,
                    inputType.FullName,
                    outputType.FullName,
                    typeof(TOut).FullName),
                k =>
                {
                    // Build the expression as a series of conversions.
                    ParameterExpression parameterExpression = Expression.Parameter(typeof(TIn), "inputValue");
                    Expression body = parameterExpression;
                    return !body.TryConvert(inputType, out body) ||
                           !body.TryConvert(outputType, out body) ||
                           !body.TryConvert(typeof(TOut), out body)
                        ? (object)null
                        : Expression.Lambda<Func<TIn, TOut>>(Expression.Block(body), parameterExpression)
                            .Compile();
                });
        }

        /// <summary>
        ///   Converts the specified expression to the output type.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="outputType">The type of the output.</param>
        /// <returns>An expression where the type has been converted.</returns>
        /// <remarks>
        ///   This version is more powerful than the Expression.Convert CLR method in that it supports
        ///   ToString() conversion, IConvertible and TypeConverters.
        /// </remarks>
        /// <exception cref="InvalidOperationException">The conversion is not supported.</exception>
        [NotNull]
        public static Expression Convert([NotNull] this Expression expression, [NotNull] Type outputType)
        {
            Expression outputExpression;
            if (!TryConvert(expression, outputType, out outputExpression))
                throw new InvalidOperationException(
                    String.Format(
                        // ReSharper disable once AssignNullToNotNullAttribute
                        Resources.Reflection_Convert_ConversionFailed,
                        expression.Type,
                        outputType));
            return outputExpression;
        }

        /// <summary>
        ///   Converts the specified expression to the output type.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="outputType">The type of the output.</param>
        /// <param name="outputExpression">The output expression.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the conversion succeeded; otherwise returns <see langword="false"/>.
        /// </returns>
        /// <remarks>
        ///   This version is more powerful than the Expression.Convert CLR method in that it supports
        ///   ToString() conversion, IConvertible and TypeConverters. It also prevents exceptions being thrown.
        /// </remarks>
        public static bool TryConvert(
            [NotNull] this Expression expression,
            [NotNull] Type outputType,
            [NotNull] out Expression outputExpression)
        {
            ExtendedType et = outputType;
            return et.TryConvert(expression, out outputExpression);
        }


        /// <summary>
        ///   Returns the method info of an explicit/implicit cast on <paramref name="type"/>
        ///   that casts between <paramref name="type"/> to <paramref name="destinationType"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="destinationType">The destination type.</param>
        /// <param name="forwards">
        ///   If set to <see langword="true"/> returns cast from <paramref name="type"/> to <paramref name="destinationType"/>;
        ///   otherwise returns casts in the reverse direction (from <paramref name="destinationType"/> to <paramref name="type"/>).
        /// </param>
        /// <param name="includeImplicit">If <see langword="true"/> include implicit casts.</param>
        /// <param name="includeExplicit">If <see langword="true"/> include explicit casts.</param>
        /// <returns>
        ///   Returns the <see cref="MethodInfo"/> (if any); otherwise returns a <see langword="null"/>.
        /// </returns>
        [CanBeNull]
        public static MethodInfo GetCastMethod(
            [NotNull] this Type type,
            [NotNull] Type destinationType,
            bool forwards = true,
            bool includeImplicit = true,
            bool includeExplicit = true)
        {
            return type
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .FirstOrDefault(
                    m =>
                    {
                        Debug.Assert(m != null);

                        // Check for correct name, and return type
                        if (((!includeImplicit || m.Name != "op_Implicit") &&
                             (!includeExplicit || m.Name != "op_Explicit")) ||
                            (m.ReturnType != (forwards ? destinationType : type)))
                            return false;

                        // Check parameters
                        ParameterInfo[] parameters = m.GetParameters();
                        return (parameters.Length == 1) &&
                               // ReSharper disable once PossibleNullReferenceException
                               (parameters[0].ParameterType == (forwards ? type : destinationType));
                    });
        }

        /// <summary>
        ///   Determines whether or not a type implicitly casts to the specified destination type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="destinationType">The destination type.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the types are the same or if <paramref name="type"/> can be implicitly cast
        ///   to <paramref name="destinationType"/>; otherwise returns <see langword="false"/>.
        /// </returns>
        public static bool ImplicitlyCastsTo([NotNull] this Type type, [NotNull] Type destinationType)
        {
            return (type == destinationType) ||
                   (destinationType.IsAssignableFrom(type)) ||
                   (GetCastMethod(type, destinationType, includeExplicit: false) != null) ||
                   (GetCastMethod(destinationType, type, forwards: false, includeExplicit: false) != null);
        }

        /// <summary>
        ///   Determines whether or not a type explicitly casts to the specified destination type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="destinationType">The destination type.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the types are the same or if <paramref name="type"/> can be explicitly cast
        ///   to <paramref name="destinationType"/>; otherwise returns <see langword="false"/>.
        /// </returns>
        public static bool ExplicitlyCastsTo([NotNull] this Type type, [NotNull] Type destinationType)
        {
            return (type != destinationType) &&
                   ((GetCastMethod(type, destinationType, includeImplicit: false) != null) ||
                    (GetCastMethod(destinationType, type, forwards: false, includeImplicit: false) != null));
        }

        /// <summary>
        ///   Determines whether or not a type casts to the specified destination type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="destinationType">The destination type.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the types are the same or if <paramref name="type"/> can be cast
        ///   to <paramref name="destinationType"/>; otherwise returns <see langword="false"/>.
        /// </returns>
        public static bool CastsTo([NotNull] this Type type, [NotNull] Type destinationType)
        {
            return (type == destinationType) ||
                   (destinationType.IsAssignableFrom(type)) ||
                   (GetCastMethod(type, destinationType) != null) ||
                   (GetCastMethod(destinationType, type, forwards: false) != null);
        }

        /// <summary>
        /// Determines whether this type can be converted to the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="destinationType">Type of the destination.</param>
        /// <returns><see langword="true" /> if this types can convert to the specified destination type; otherwise, <see langword="false" />.</returns>
        /// <remarks></remarks>
        public static bool CanConvertTo([NotNull] this Type type, [NotNull] Type destinationType)
        {
            // ReSharper disable once PossibleNullReferenceException
            return ((ExtendedType)type).CanConvertTo(destinationType);
        }

        /// <summary>
        ///   Gets the equivalent method on the new type specified.
        /// </summary>
        /// <param name="methodInfo">The method info.</param>
        /// <param name="declaringType">The declaring type.</param>
        /// <returns>
        ///   Returns the equivalent method. If the <paramref name="declaringType"/> is equal to the
        ///   <paramref name="methodInfo"/> declaring type then <paramref name="methodInfo"/> is returned. 
        /// </returns>
        [NotNull]
        public static MethodInfo GetEquivalent([NotNull] this MethodInfo methodInfo, [NotNull] Type declaringType)
        {
            if (methodInfo == null) throw new ArgumentNullException("methodInfo");
            if (declaringType == null) throw new ArgumentNullException("declaringType");

            // ReSharper disable once AssignNullToNotNullAttribute
            return methodInfo.DeclaringType == declaringType
                ? methodInfo
                : declaringType.GetMethod(
                    methodInfo.Name,
                    // ReSharper disable once PossibleNullReferenceException
                    methodInfo.GetParameters().Select(p => p.ParameterType).ToArray());
        }

        /// <summary>
        ///   Implements a safe version of <see cref="ParameterInfo.RawDefaultValue"/> that always returns a default value.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>The default value.</returns>
        /// <remarks>
        ///   If the CLR fails to grab a raw default value then this will return the default value for the type instead.
        /// </remarks>
        /// <seealso cref="System.Reflection.ParameterInfo.RawDefaultValue"/>
        [CanBeNull]
        public static object RawDefaultValueSafe([NotNull] this ParameterInfo parameter)
        {
            if (parameter == null) throw new ArgumentNullException("parameter");
            object value;
            try
            {
                value = parameter.RawDefaultValue;
            }
            catch (FormatException)
            {
                // RawDefaultValue doesn't cope with default(struct) being the default value.
                // just specify the default of the type.
                value = null;
            }
            return parameter.ParameterType.IsValueType && (value == null)
                ? Activator.CreateInstance(parameter.ParameterType, true)
                : value;
        }

        /// <summary>
        ///   Gets the default instance for the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The default instance for the specified type.</returns>
        [CanBeNull]
        public static object Default([NotNull] this Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            return !type.IsValueType ? null : Activator.CreateInstance(type, true);
        }

        /// <summary>
        /// Simplifies the full name, removing the specified assemblies, version, culture info and public keys (including in nested generic types).
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="excludedAssemblies">The excluded assemblies (if none specified defaults to mscorlib and calling assembly).</param>
        /// <returns>System.String.</returns>
        [NotNull]
        public static string SimplifiedTypeFullName(
            [NotNull] this Type type,
            [CanBeNull] params string[] excludedAssemblies)
        {
            if (type == null) throw new ArgumentNullException("type");
            // ReSharper disable once AssignNullToNotNullAttribute
            return SimplifiedTypeFullName(type.AssemblyQualifiedName, excludedAssemblies);
        }

        /// <summary>
        /// Simplifies the full name, removing the specified assemblies, version, culture info and public keys (including in nested generic types).
        /// </summary>
        /// <param name="typeAssemblyQualifiedName">The type's assembly qualified name.</param>
        /// <param name="excludedAssemblies">The excluded assemblies (if none specified defaults to mscorlib and calling assembly).</param>
        /// <returns>System.String.</returns>
        [NotNull]
        public static string SimplifiedTypeFullName(
            [NotNull] string typeAssemblyQualifiedName,
            [CanBeNull] params string[] excludedAssemblies)
        {
            if (typeAssemblyQualifiedName == null) throw new ArgumentNullException("typeAssemblyQualifiedName");

            HashSet<string> exclude = (excludedAssemblies == null) ||
                                      (excludedAssemblies.Length < 1)
                ? new HashSet<string> { "mscorlib" }
                : new HashSet<string>(excludedAssemblies);
            StringBuilder builder = new StringBuilder(typeAssemblyQualifiedName.Length);
            StringBuilder assemblyBuilder = new StringBuilder(typeAssemblyQualifiedName.Length);
            int state = 0;
            int depth = 0;
            foreach (char c in typeAssemblyQualifiedName)
                switch (state)
                {
                    // Type name
                    case 0:
                        builder.Append(c);
                        switch (c)
                        {
                            case '[':
                                depth++;
                                break;
                            case ']':
                                if (depth < 1)
                                    return typeAssemblyQualifiedName;
                                depth--;
                                break;
                            case ',':
                                assemblyBuilder.Clear();
                                state++;
                                break;
                        }
                        break;

                    // Assembly name
                    case 1:
                        switch (c)
                        {
                            case '[':
                                builder.Append(c);
                                depth++;
                                state = 0;
                                break;
                            case ',':
                                string assembly = assemblyBuilder.ToString().Trim();
                                if ((assembly.Length < 1) ||
                                    !exclude.Contains(assembly))
                                    builder.Append(assembly);
                                else
                                    builder.Remove(builder.Length - 1, 1);
                                assemblyBuilder.Clear();
                                state++;
                                break;
                            default:
                                assemblyBuilder.Append(c);
                                break;
                        }
                        break;

                    // Version
                    case 2:
                        if (c == ',') state++;
                        break;

                    // Culture
                    case 3:
                        if (c == ',') state++;
                        break;

                    // Public Key Token
                    case 4:
                        if (c == ']')
                        {
                            if (depth < 1)
                                return typeAssemblyQualifiedName;
                            builder.Append(c);
                            state = 0;
                            depth--;
                        }
                        break;
                }
            return depth > 0
                ? typeAssemblyQualifiedName
                : builder.ToString();
        }

        /// <summary>
        /// Matches the specified type against the specified search..
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="typeSearch">The type search.</param>
        /// <param name="allowCasts">if set to <see langword="true"/> allows casts.</param>
        /// <param name="allowTypeClosures">if set to <see langword="true"/> allows type closures.</param>
        /// <param name="allowSignatureClosures">if set to <see langword="true"/> allows signature closures.</param>
        /// <param name="isOutputType">if set to <see langword="true" /> then the type is an output so casts are performed to the search type, rather than from the search type..</param>
        /// <returns><see langword="true"/> if matches; otherwise <see langword="false"/></returns>
        /// <remarks></remarks>
        public static bool Matches(
            this Type type,
            TypeSearch typeSearch,
            bool allowCasts = true,
            bool allowTypeClosures = true,
            bool allowSignatureClosures = true,
            bool isOutputType = false)
        {
            bool requiresCast;
            GenericArgumentLocation closureLocation;
            int closurePosition;
            Type closureType;
            bool match = Matches(
                type,
                typeSearch,
                out requiresCast,
                out closureLocation,
                out closurePosition,
                out closureType,
                isOutputType);
            if (!match) return false;
            if (requiresCast) return allowCasts;
            switch (closureLocation)
            {
                case GenericArgumentLocation.Signature:
                    return allowSignatureClosures;
                case GenericArgumentLocation.Type:
                    return allowTypeClosures;
            }
            return true;
        }

        /// <summary>
        /// Matches the specified type against the specified search..
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="typeSearch">The type search.</param>
        /// <param name="requiresCast">if set to <see langword="true" /> the type requires a cast to match.</param>
        /// <param name="closureLocation">The closure location if a closure is required to match (will always be None, Method or Type).</param>
        /// <param name="closurePosition">The closure position if a closure is required to match.</param>
        /// <param name="closureType">Type of the closure if a closure is required to match.</param>
        /// <param name="isOutputType">if set to <see langword="true" /> then the type is an output so casts are performed to the search type, rather than from the search type..</param>
        /// <returns><see langword="true" /> if matches; otherwise <see langword="false" /></returns>
        /// <remarks></remarks>
        public static bool Matches(
            this Type type,
            TypeSearch typeSearch,
            out bool requiresCast,
            out GenericArgumentLocation closureLocation,
            out int closurePosition,
            out Type closureType,
            bool isOutputType = false)
        {
            requiresCast = false;
            closureLocation = GenericArgumentLocation.None;
            closurePosition = -1;
            closureType = null;

            // Weed out nulls
            if (type == null) return typeSearch == null;
            if (typeSearch == null) return false;

            if (type.IsByRef != typeSearch.IsByReference) return false;
            if (type.IsPointer != typeSearch.IsPointer) return false;

            // If types are equal we have a match
            if (type == typeSearch.Type) return true;

            // If we're allowing casting and search type can be cast to type, we have a match.
            // TODO Arguably, we need to be able to check whether the Expression.Convert works.
            if (typeSearch.Type != null &&
                (isOutputType
                    ? type.CanConvertTo(typeSearch.Type)
                    : typeSearch.Type.CanConvertTo(type)))
            {
                requiresCast = true;
                return true;
            }

            // If we have a reference or pointer type - retrieve the element type.
            if (type.IsByRef ||
                type.IsPointer)
            {
                if (!type.HasElementType) return false;
                type = type.GetElementType();
                Debug.Assert(type != null);
            }

            // If our type is not a generic parameter then we're done.
            if (!type.IsGenericParameter)
                return false;

            if (typeSearch.Type != null)
            {
                // If we were looking for an actual type, andf the type is a generic parameter
                // then we need to perform a closure.
                closureLocation = type.DeclaringMethod != null
                    ? GenericArgumentLocation.Signature
                    : GenericArgumentLocation.Type;
                closurePosition = type.GenericParameterPosition;
                closureType = (typeSearch.IsByReference || typeSearch.IsPointer) && typeSearch.Type.HasElementType
                    ? typeSearch.Type.GetElementType()
                    : typeSearch.Type;

                // Check closure type is valid for constraint

                return true;
            }

            // If we're looking for a particular name, ensure it matches.
            if ((typeSearch.GenericArgumentName != null) &&
                (type.Name != typeSearch.GenericArgumentName))
                return false;

            if ((typeSearch.GenericArgumentPosition >= 0) &&
                (type.GenericParameterPosition != typeSearch.GenericArgumentPosition))
                return false;

            switch (typeSearch.GenericArgumentLocation)
            {
                case GenericArgumentLocation.None:
                    return false;
                case GenericArgumentLocation.Signature:
                    if (type.DeclaringMethod == null)
                        return false;
                    break;
                case GenericArgumentLocation.Type:
                    if (type.DeclaringMethod != null)
                        return false;
                    break;
            }

            return true;
        }

        /// <summary>
        /// Matches the specified signature.
        /// </summary>
        /// <param name="signature">The signature.</param>
        /// <param name="types">The types to match against (last type should be return type).</param>
        /// <returns><see langword="true"/> if matches; otherwise <see langword="false"/></returns>
        /// <remarks></remarks>
        public static bool Matches(this ISignature signature, params TypeSearch[] types)
        {
            return Matches(signature, true, true, true, 0, types);
        }

        /// <summary>
        /// Matches the specified signature.
        /// </summary>
        /// <param name="signature">The signature.</param>
        /// <param name="genericArguments">The number of generic arguments on the signature.</param>
        /// <param name="types">The types to match against (last type should be return type).</param>
        /// <returns><see langword="true"/> if matches; otherwise <see langword="false"/></returns>
        /// <remarks></remarks>
        public static bool Matches(this ISignature signature, int genericArguments, params TypeSearch[] types)
        {
            return Matches(signature, true, true, true, genericArguments, types);
        }

        /// <summary>
        /// Matches the specified signature.
        /// </summary>
        /// <param name="signature">The signature.</param>
        /// <param name="allowCasts">if set to <see langword="true"/> allows casts.</param>
        /// <param name="allowTypeClosures">if set to <see langword="true"/> allows type closures.</param>
        /// <param name="allowSignatureClosures">if set to <see langword="true"/> allows signature closures.</param>
        /// <param name="types">The types to match against (last type should be return type).</param>
        /// <returns><see langword="true"/> if matches; otherwise <see langword="false"/></returns>
        /// <remarks></remarks>
        public static bool Matches(
            this ISignature signature,
            bool allowCasts,
            bool allowTypeClosures,
            bool allowSignatureClosures,
            params TypeSearch[] types)
        {
            return Matches(signature, allowCasts, allowTypeClosures, allowSignatureClosures, 0, types);
        }

        /// <summary>
        /// Matches the specified signature.
        /// </summary>
        /// <param name="signature">The signature.</param>
        /// <param name="allowCasts">if set to <see langword="true"/> allows casts.</param>
        /// <param name="allowTypeClosures">if set to <see langword="true"/> allows type closures.</param>
        /// <param name="allowSignatureClosures">if set to <see langword="true"/> allows signature closures.</param>
        /// <param name="genericArguments">The number of generic arguments on the signature.</param>
        /// <param name="types">The types to match against (last type should be return type).</param>
        /// <returns><see langword="true"/> if matches; otherwise <see langword="false"/></returns>
        /// <remarks></remarks>
        public static bool Matches(
            this ISignature signature,
            bool allowCasts,
            bool allowTypeClosures,
            bool allowSignatureClosures,
            int genericArguments,
            params TypeSearch[] types)
        {
            bool[] castsRequired;
            Type[] typeClosures;
            Type[] methodClosures;
            bool match = Matches(
                signature,
                out castsRequired,
                out typeClosures,
                out methodClosures,
                genericArguments,
                types);
            if (!match) return false;
            if (castsRequired.Length > 0) return allowCasts;
            if (typeClosures.Length > 0) return allowTypeClosures;
            if (methodClosures.Length > 0) return allowSignatureClosures;
            return true;
        }

        /// <summary>
        /// Matches the specified signature.
        /// </summary>
        /// <param name="signature">The signature.</param>
        /// <param name="castsRequired">Indicates which parameters (or return type) will require casting.</param>
        /// <param name="typeClosures">The type closures required to match (if any).</param>
        /// <param name="signatureClosures">The signature closures required to match (if any).</param>
        /// <param name="types">The types to match against (last type should be return type).</param>
        /// <returns><see langword="true"/> if matches; otherwise <see langword="false"/></returns>
        /// <remarks></remarks>
        public static bool Matches(
            this ISignature signature,
            [NotNull] out bool[] castsRequired,
            [NotNull] out Type[] typeClosures,
            [NotNull] out Type[] signatureClosures,
            params TypeSearch[] types)
        {
            return Matches(signature, out castsRequired, out typeClosures, out signatureClosures, 0, types);
        }

        /// <summary>
        /// Matches the specified signature.
        /// </summary>
        /// <param name="signature">The signature.</param>
        /// <param name="castsRequired">Indicates which parameters (or return type) will require casting.</param>
        /// <param name="typeClosures">The type closures required to match (if any).</param>
        /// <param name="signatureClosures">The signature closures required to match (if any).</param>
        /// <param name="genericArguments">The number of generic arguments on the signature.</param>
        /// <param name="types">The types to match against (last type should be return type).</param>
        /// <returns><see langword="true"/> if matches; otherwise <see langword="false"/></returns>
        /// <remarks></remarks>
        public static bool Matches(
            this ISignature signature,
            [NotNull] out bool[] castsRequired,
            [NotNull] out Type[] typeClosures,
            [NotNull] out Type[] signatureClosures,
            int genericArguments,
            params TypeSearch[] types)
        {
            castsRequired = Array<bool>.Empty;
            Type[] emptyTypes = Array<Type>.Empty;
            signatureClosures = typeClosures = emptyTypes;
            if ((signature == null) ||
                (types == null))
                return false;

            int searches = types.Length;

            // We have to have at least one search for return type.
            if (searches < 1)
                return false;

            // Get parameters as array.
            IEnumerable<Type> p = signature.ParameterTypes;
            Type[] parameters = p == null ? emptyTypes : p.ToArray();

            // Check we have right number of parameters (+1 for return type)
            if ((parameters.Length + 1) != searches)
                return false;

            // Grab signature generic arguments safely.
            IEnumerable<GenericArgument> sga = signature.SignatureGenericArguments;
            GenericArgument[] emptyGenericArguments = Array<GenericArgument>.Empty;
            GenericArgument[] signatureArguments = sga == null
                ? emptyGenericArguments
                : sga.Where(g => g.Location == GenericArgumentLocation.Signature)
                    .ToArray();

            // Check we have right number of generic arguments on the signature.
            if (signatureArguments.Length != genericArguments)
                return false;

            // Grab type generic arguments safely.
            IEnumerable<GenericArgument> tga = signature.TypeGenericArguments;
            GenericArgument[] typeArguments = tga == null
                ? emptyGenericArguments
                : tga.Where(g => g.Location == GenericArgumentLocation.Type).ToArray();

            // Initialise output arrays
            castsRequired = new bool[parameters.Length + 1];
            typeClosures = new Type[typeArguments.Length];
            signatureClosures = new Type[signatureArguments.Length];

            // Check return type
            Type returnType = signature.ReturnType;
            TypeSearch returnTypeSearch = types.Last();
            Debug.Assert(returnTypeSearch != null);
            bool requiresCast;
            GenericArgumentLocation closureLocation;
            int closurePosition;
            Type closureType;

            if (
                !returnType.Matches(
                    returnTypeSearch,
                    out requiresCast,
                    out closureLocation,
                    out closurePosition,
                    out closureType,
                    true) ||
                !UpdateSearchContext(
                    ref castsRequired[parameters.Length],
                    typeClosures,
                    signatureClosures,
                    requiresCast,
                    closureLocation,
                    closurePosition,
                    closureType))
                return false;

            // If we have more than one search we need to check parameters.
            if (searches > 1)
            {
                // Check parameters
                IEnumerator pe = parameters.GetEnumerator();
                IEnumerator te = types.GetEnumerator();
                int parameter = 0;
                while (pe.MoveNext())
                {
                    te.MoveNext();
                    Debug.Assert(pe.Current != null);
                    Debug.Assert(te.Current != null);
                    Type t = (Type)pe.Current;
                    TypeSearch s = ((TypeSearch)te.Current);
                    if (!t.Matches(s, out requiresCast, out closureLocation, out closurePosition, out closureType) ||
                        !UpdateSearchContext(
                            ref castsRequired[parameter++],
                            typeClosures,
                            signatureClosures,
                            requiresCast,
                            closureLocation,
                            closurePosition,
                            closureType))
                        // Parameter failed to match.
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Updates the search context.
        /// </summary>
        /// <param name="castRequired">if set to <see langword="true"/> [cast required].</param>
        /// <param name="typeClosures">The type closures.</param>
        /// <param name="methodClosures">The method closures.</param>
        /// <param name="requiresCast">if set to <see langword="true"/> [requires cast].</param>
        /// <param name="closureLocation">The closure location.</param>
        /// <param name="closurePosition">The closure position.</param>
        /// <param name="closureType">Type of the closure.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private static bool UpdateSearchContext(
            ref bool castRequired,
            [NotNull] Type[] typeClosures,
            [NotNull] Type[] methodClosures,
            bool requiresCast,
            GenericArgumentLocation closureLocation,
            int closurePosition,
            Type closureType)
        {
            Debug.Assert(closureLocation != GenericArgumentLocation.Any);

            if (requiresCast)
            {
                Debug.Assert(closureLocation == GenericArgumentLocation.None);
                Debug.Assert(closurePosition < 0);
                castRequired = true;
                return true;
            }

            // Check if we have a closure location
            if (closureLocation == GenericArgumentLocation.None)
            {
                Debug.Assert(closurePosition < 0);
                return true;
            }

            Debug.Assert(closureType != null);
            if (closureLocation == GenericArgumentLocation.Signature)
            {
                // Requires method closure
                Debug.Assert(closurePosition < methodClosures.Length);

                // If we already have a closure, ensure it matches!
                Type t = methodClosures[closurePosition];
                if (t != null)
                {
                    // Check if the types match.
                    if (t != closureType)
                        // If the existing type can be assigned from the closure type we're OK
                        if (!t.IsAssignableFrom(closureType))
                        {
                            // If the existing type cannot be assigned to the closure type then we don't have a match.
                            if (!closureType.IsAssignableFrom(t))
                                return false;

                            // As the closure type is a base, update our method closure type.
                            methodClosures[closurePosition] = closureType;
                        }
                }
                else
                    methodClosures[closurePosition] = closureType;
            }
            else
            {
                Debug.Assert(closureLocation == GenericArgumentLocation.Type);
                // Requires type closure
                Debug.Assert(closurePosition < typeClosures.Length);

                // If we already have a closure, ensure it matches!
                Type t = typeClosures[closurePosition];
                if (t != null)
                {
                    // Check if the types match.
                    if (t != closureType)
                        // If the existing type can be assigned from the closure type we're OK
                        if (!t.IsAssignableFrom(closureType))
                        {
                            // If the existing type cannot be assigned to the closure type then we don't have a match.
                            if (!closureType.IsAssignableFrom(t))
                                return false;

                            // As the closure type is a base, update our type closure type.
                            typeClosures[closurePosition] = closureType;
                        }
                }
                else
                    typeClosures[closurePosition] = closureType;
            }
            return true;
        }

        /// <summary>
        /// Finds the best match from an enumeration of signatures.
        /// </summary>
        /// <param name="signatures">The signatures.</param>
        /// <param name="types">The types to match against (last type should be return type).</param>
        /// <returns><see langword="true" /> if matches; otherwise <see langword="false" /></returns>
        /// <remarks></remarks>
        public static ISignature BestMatch(this IEnumerable<ISignature> signatures, params TypeSearch[] types)
        {
            bool[] castsRequired;
            return BestMatch(signatures, 0, true, true, out castsRequired, types);
        }

        /// <summary>
        /// Finds the best match from an enumeration of signatures.
        /// </summary>
        /// <param name="signatures">The signatures.</param>
        /// <param name="genericArguments">The generic arguments.</param>
        /// <param name="types">The types to match against (last type should be return type).</param>
        /// <returns><see langword="true" /> if matches; otherwise <see langword="false" /></returns>
        /// <remarks></remarks>
        public static ISignature BestMatch(
            this IEnumerable<ISignature> signatures,
            int genericArguments,
            params TypeSearch[] types)
        {
            bool[] castsRequired;
            return BestMatch(signatures, genericArguments, true, true, out castsRequired, types);
        }

        /// <summary>
        /// Finds the best match from an enumeration of signatures.
        /// </summary>
        /// <param name="signatures">The signatures.</param>
        /// <param name="allowClosure">if set to <see langword="true" /> will automatically close the signatures generic types if possible.</param>
        /// <param name="allowCasts">if set to <see langword="true" /> then types will match if they can be cast to the required type.</param>
        /// <param name="types">The types to match against (last type should be return type).</param>
        /// <returns><see langword="true" /> if matches; otherwise <see langword="false" /></returns>
        /// <remarks></remarks>
        public static ISignature BestMatch(
            this IEnumerable<ISignature> signatures,
            bool allowClosure,
            bool allowCasts,
            params TypeSearch[] types)
        {
            bool[] castsRequired;
            return BestMatch(signatures, 0, allowClosure, allowCasts, out castsRequired, types);
        }

        /// <summary>
        /// Finds the best match from an enumeration of signatures.
        /// </summary>
        /// <param name="signatures">The signatures.</param>
        /// <param name="genericArguments">The generic arguments.</param>
        /// <param name="allowClosure">if set to <see langword="true" /> will automatically close the signatures generic types if possible.</param>
        /// <param name="allowCasts">if set to <see langword="true" /> then types will match if they can be cast to the required type.</param>
        /// <param name="types">The types to match against (last type should be return type).</param>
        /// <returns><see langword="true" /> if matches; otherwise <see langword="false" /></returns>
        /// <remarks></remarks>
        public static ISignature BestMatch(
            this IEnumerable<ISignature> signatures,
            int genericArguments,
            bool allowClosure,
            bool allowCasts,
            params TypeSearch[] types)
        {
            bool[] castsRequired;
            return BestMatch(signatures, genericArguments, allowClosure, allowCasts, out castsRequired, types);
        }

        /// <summary>
        /// Finds the best match from an enumeration of signatures.
        /// </summary>
        /// <param name="signatures">The signatures.</param>
        /// <param name="genericArguments">The generic arguments.</param>
        /// <param name="allowClosure">if set to <see langword="true" /> will automatically close the signatures generic types if possible.</param>
        /// <param name="allowCasts">if set to <see langword="true" /> then types will match if they can be cast to the required type.</param>
        /// <param name="castsRequired">Any array indicating which parameters require a cast (the last element is for the return type).</param>
        /// <param name="types">The types to match against (last type should be return type).</param>
        /// <returns><see langword="true" /> if matches; otherwise <see langword="false" /></returns>
        /// <remarks></remarks>
        public static ISignature BestMatch(
            this IEnumerable<ISignature> signatures,
            int genericArguments,
            bool allowClosure,
            bool allowCasts,
            out bool[] castsRequired,
            params TypeSearch[] types)
        {
            // Holds matches along with order.
            ISignature bestMatch = null;
            castsRequired = Array<bool>.Empty;
            Type[] emptyTypes = Array<Type>.Empty;
            Type[] typeClosures = emptyTypes;
            Type[] signatureClosures = emptyTypes;
            int castsCount = Int32.MaxValue;
            int typeClosureCount = Int32.MaxValue;
            int signatureClosureCount = Int32.MaxValue;

            if (signatures != null)
                foreach (ISignature signature in signatures)
                {
                    // Match method signature
                    bool[] cr;
                    Type[] tc;
                    Type[] sc;
                    if (!signature.Matches(out cr, out tc, out sc, genericArguments, types))
                        continue;

                    // Check if any casts were required
                    int cc = cr.Count(c => c);
                    if (!allowCasts &&
                        (cc > 0))
                        continue;

                    // Check if any closures were required.
                    int tcc = tc.Count(t => t != null);
                    int scc = sc.Count(t => t != null);
                    if (!allowClosure &&
                        (tcc + scc > 0))
                        continue;

                    // Check to see if this beats the current best match
                    if (bestMatch != null)
                    {
                        // If we have to close more type generic arguments then existing match is better.
                        if (typeClosureCount < tcc) continue;

                        // If type level closures are equal, look more closely
                        if (typeClosureCount == tcc)
                        {
                            // If we have to close more signature generic arguments then existing match is better.
                            if (signatureClosureCount < scc) continue;

                            // If method level closures are equal, see which has more casts.
                            if ((signatureClosureCount == scc) &&
                                (castsCount <= cc))
                                continue;
                        }
                    }

                    // Set best match
                    bestMatch = signature;
                    castsCount = cc;
                    castsRequired = cr;
                    typeClosures = tc;
                    typeClosureCount = tcc;
                    signatureClosures = sc;
                    signatureClosureCount = scc;
                }

            // If we don't have a match return null.
            if (bestMatch == null) return null;

            // Check to see if we have to close the signature
            return typeClosures.Any(c => c != null) ||
                   signatureClosures.Any(c => c != null)
                ? bestMatch.Close(typeClosures, signatureClosures)
                : bestMatch;
        }


        /// <summary>
        /// Expands the parameter type.
        /// </summary>
        /// <param name="parameterType">Type of the input.</param>
        /// <param name="signatureArguments">The signature arguments.</param>
        /// <param name="typeArguments">The type arguments.</param>
        /// <returns>Given a parameter type, will expand the type based on a set of signature and type arguments.</returns>
        /// <remarks></remarks>
        internal static Type ExpandParameterType(
            [NotNull] Type parameterType,
            [NotNull] Type[] signatureArguments,
            [NotNull] Type[] typeArguments)
        {
            // Deal with pointers/references.
            Type t = parameterType;
            if ((parameterType.IsByRef || parameterType.IsPointer) &&
                (parameterType.HasElementType))
                t = parameterType.GetElementType();
            Debug.Assert(t != null);

            if (t.IsGenericParameter)
            {
                int position = t.GenericParameterPosition;

                // Grab the relevant type.
                Type nt = t.DeclaringMethod != null
                    ? signatureArguments[position]
                    : typeArguments[position];
                Debug.Assert(nt != null);
                if (parameterType.IsByRef)
                    parameterType = nt.MakeByRefType();
                else if (parameterType.IsPointer)
                    parameterType = nt.MakePointerType();
                else
                    parameterType = nt;
            }
            return parameterType;
        }

        /// <summary> 
        /// Checks to see if a type descends from another type. 
        /// </summary> 
        /// <param name="sourceType">Type of the source.</param> 
        /// <param name="baseType">Type of the base.</param> 
        /// <returns></returns> 
        /// <remarks></remarks> 
        [Pure]
        public static bool DescendsFrom([NotNull] this Type sourceType, [NotNull] Type baseType)
        {
            if (sourceType == null) throw new ArgumentNullException("sourceType");
            if (baseType == null) throw new ArgumentNullException("baseType");

            bool isGenericDef = baseType.IsGenericTypeDefinition;
            do
            {
                if (sourceType == baseType)
                    return true;
                if (isGenericDef &&
                    sourceType.IsGenericType &&
                    sourceType.GetGenericTypeDefinition() == baseType)
                    return true;
                if (baseType.IsSealed)
                    return false;
                sourceType = sourceType.BaseType;
            } while (sourceType != null);
            return false;
        }

        /// <summary>
        /// Checks to see if a type descends from another type.
        /// </summary>
        /// <typeparam name="TBase">The type of the base.</typeparam>
        /// <param name="sourceType">Type of the source.</param>
        /// <returns></returns>
        [Pure]
        public static bool DescendsFrom<TBase>([NotNull] this Type sourceType)
        {
            Type baseType = typeof(TBase);
            do
            {
                if (sourceType == baseType)
                    return true;
                if (baseType.IsSealed)
                    return false;
                sourceType = sourceType.BaseType;
            } while (sourceType != null);
            return false;
        }

        /// <summary>
        /// Checks to see if a type implements an interface.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <param name="interfaceType">The type of the interface.</param>
        /// <returns><see langword="true"/> if the type implements the interface; otherwise <see langword="false"/>.</returns>
        [Pure]
        public static bool ImplementsInterface([NotNull] this Type type, [NotNull] Type interfaceType)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (interfaceType == null) throw new ArgumentNullException("interfaceType");
            if (!interfaceType.IsInterface)
                throw new ArgumentException(Resources.Reflection_ImplementsInterface_MustBeInterface, "interfaceType");

            if (type == interfaceType ||
                ExtendedType.Get(type).Implements(interfaceType))
                return true;

            if (!interfaceType.IsGenericTypeDefinition)
                return false;

            if (type.IsGenericType &&
                type.GetGenericTypeDefinition() == interfaceType)
                return true;

            foreach (Type @interface in ExtendedType.Get(type).Interfaces)
            {
                Debug.Assert(@interface != null);
                if (@interface.IsGenericType &&
                    @interface.GetGenericTypeDefinition() == interfaceType)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks to see if a type implements an interface.
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <param name="type">The type to check.</param>
        /// <returns><see langword="true"/> if the type implements the interface; otherwise <see langword="false"/>.</returns>
        [Pure]
        public static bool ImplementsInterface<TInterface>([NotNull] this Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (!typeof(TInterface).IsInterface)
                // ReSharper disable once NotResolvedInText
                throw new ArgumentException(Resources.Reflection_ImplementsInterface_MustBeInterface, "TInterface");

            // ReSharper disable once PossibleNullReferenceException
            return type == typeof(TInterface) || ((ExtendedType)type).Implements(typeof(TInterface));
        }

        /// <summary>
        /// Creates a function that will add parameters of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of parameters to add.</typeparam>
        /// <returns>An addition function.</returns>
        public static Func<T, T, T> AddFunc<T>()
        {
            Type type = typeof(T);
            // Workaround for the fact that bytes don't have addition operators.
            if ((type == typeof(byte)) ||
                (type == typeof(sbyte)))
                type = typeof(short);
            return ExpressionFunc<T, T, T>(Expression.Add, type, type);
        }

        /// <summary>
        /// Creates a function that will add parameters of the specified type.
        /// </summary>
        /// <typeparam name="TLHS">The type of the LHS of the addition.</typeparam>
        /// <typeparam name="TRHS">The type of the RHS of the addition.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <returns>An addition function.</returns>
        public static Func<TLHS, TRHS, TResult> AddFunc<TLHS, TRHS, TResult>()
        {
            Type typeLHS = typeof(TLHS);
            Type typeRHS = typeof(TRHS);
            // Workaround for the fact that bytes don't have addition operators.
            if ((typeLHS == typeof(byte)) ||
                (typeLHS == typeof(sbyte)))
                typeLHS = typeof(short);
            if ((typeRHS == typeof(byte)) ||
                (typeRHS == typeof(sbyte)))
                typeRHS = typeof(short);
            return ExpressionFunc<TLHS, TRHS, TResult>(Expression.Add, typeLHS, typeRHS);
        }

        /// <summary>
        /// Creates a function that will add parameters of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of parameters to add.</typeparam>
        /// <param name="type">The actual type.</param>
        /// <returns>An addition function.</returns>
        public static Func<T, T, T> AddFunc<T>([NotNull] this Type type)
        {
            // Workaround for the fact that bytes don't have addition operators.
            if ((type == typeof(byte)) ||
                (type == typeof(sbyte)))
                type = typeof(short);
            return ExpressionFunc<T, T, T>(Expression.Add, type, type);
        }

        /// <summary>
        /// Creates a function that will add parameters of the specified type.
        /// </summary>
        /// <typeparam name="TParam">The type of parameters to add.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="type">The actual type.</param>
        /// <returns>An addition function.</returns>
        public static Func<TParam, TParam, TResult> AddFunc<TParam, TResult>([NotNull] this Type type)
        {
            // Workaround for the fact that bytes don't have addition operators.
            if ((type == typeof(byte)) ||
                (type == typeof(sbyte)))
                type = typeof(short);
            return ExpressionFunc<TParam, TParam, TResult>(Expression.Add, type, type);
        }

        /// <summary>
        /// Creates a function that will add parameters of the specified type, performing casts where necessary.
        /// </summary>
        /// <typeparam name="TLHS">The type of the LHS of the addition.</typeparam>
        /// <typeparam name="TRHS">The type of the RHS of the addition.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="typeLHS">The actual type of the LHS of the addition.</param>
        /// <param name="typeRHS">The actual type of the RHS of the addition.</param>
        /// <returns>An addition function.</returns>
        public static Func<TLHS, TRHS, TResult> AddFunc<TLHS, TRHS, TResult>(
            [NotNull] this Type typeLHS,
            [NotNull] Type typeRHS)
        {
            // Workaround for the fact that bytes don't have addition operators.
            if ((typeLHS == typeof(byte)) ||
                (typeLHS == typeof(sbyte)))
                typeLHS = typeof(short);
            if ((typeRHS == typeof(byte)) ||
                (typeRHS == typeof(sbyte)))
                typeRHS = typeof(short);
            return ExpressionFunc<TLHS, TRHS, TResult>(Expression.Add, typeLHS, typeRHS);
        }

        /// <summary>
        /// Creates a function that will subtract parameters of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of parameters to subtract.</typeparam>
        /// <param name="type">The actual type.</param>
        /// <returns>A subtraction function.</returns>
        public static Func<T, T, T> SubtractFunc<T>([NotNull] this Type type)
        {
            // Workaround for the fact that bytes don't have addition operators.
            if ((type == typeof(byte)) ||
                (type == typeof(sbyte)))
                type = typeof(short);
            return ExpressionFunc<T, T, T>(Expression.Subtract, type, type);
        }

        /// <summary>
        /// Creates a function that will subtract parameters of the specified type.
        /// </summary>
        /// <typeparam name="TParam">The type of parameters to subtract.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="type">The actual type.</param>
        /// <returns>A subtraction function.</returns>
        public static Func<TParam, TParam, TResult> SubtractFunc<TParam, TResult>([NotNull] this Type type)
        {
            // Workaround for the fact that bytes don't have addition operators.
            if ((type == typeof(byte)) ||
                (type == typeof(sbyte)))
                type = typeof(short);
            return ExpressionFunc<TParam, TParam, TResult>(Expression.Subtract, type, type);
        }

        /// <summary>
        /// Creates a function that will subtract parameters of the specified type, performing casts where necessary.
        /// </summary>
        /// <typeparam name="TLHS">The type of the LHS of the subtraction.</typeparam>
        /// <typeparam name="TRHS">The type of the RHS of the subtraction.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="typeLHS">The actual type of the LHS of the subtraction.</param>
        /// <param name="typeRHS">The actual type of the RHS of the subtraction.</param>
        /// <returns>A subtraction function.</returns>
        public static Func<TLHS, TRHS, TResult> SubtractFunc<TLHS, TRHS, TResult>(
            [NotNull] this Type typeLHS,
            [NotNull] Type typeRHS)
        {
            // Workaround for the fact that bytes don't have addition operators.
            if ((typeLHS == typeof(byte)) ||
                (typeLHS == typeof(sbyte)))
                typeLHS = typeof(short);
            if ((typeRHS == typeof(byte)) ||
                (typeRHS == typeof(sbyte)))
                typeRHS = typeof(short);
            return ExpressionFunc<TLHS, TRHS, TResult>(Expression.Subtract, typeLHS, typeRHS);
        }

        /// <summary>
        /// Creates a function that will evaluate a LessThan comparison on parameters of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of parameters to evaluate.</typeparam>
        /// <returns>A less than comparison function.</returns>
        public static Func<T, T, bool> LessThanFunc<T>()
        {
            return ExpressionFunc<T, T, bool>(Expression.LessThan, typeof(T), typeof(T));
        }

        /// <summary>
        /// Creates a function that will evaluate a LessThan comparison on parameters of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of parameters to evaluate.</typeparam>
        /// <param name="type">The actual type.</param>
        /// <returns>A less than comparison function.</returns>
        public static Func<T, T, bool> LessThanFunc<T>([NotNull] this Type type)
        {
            return ExpressionFunc<T, T, bool>(Expression.LessThan, type, type);
        }

        /// <summary>
        /// Creates a function that will evaluate a LessThan comparison on parameters of the specified type.
        /// </summary>
        /// <typeparam name="TLHS">The type of the TLHS.</typeparam>
        /// <typeparam name="TRHS">The type of the TRHS.</typeparam>
        /// <param name="typeLHS">The actual type of the LHS of the comparison.</param>
        /// <param name="typeRHS">The actual type of the RHS of the comparison.</param>
        /// <returns>A less than comparison function.</returns>
        public static Func<TLHS, TRHS, bool> LessThanFunc<TLHS, TRHS>(
            [NotNull] this Type typeLHS,
            [NotNull] Type typeRHS)
        {
            return ExpressionFunc<TLHS, TRHS, bool>(Expression.LessThan, typeLHS, typeRHS);
        }

        /// <summary>
        /// Creates a function that will evaluate a LessThanOrEqual comparison on parameters of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of parameters to evaluate.</typeparam>
        /// <returns>A less than or equal comparison function.</returns>
        public static Func<T, T, bool> LessThanOrEqualFunc<T>()
        {
            return ExpressionFunc<T, T, bool>(Expression.LessThanOrEqual, typeof(T), typeof(T));
        }

        /// <summary>
        /// Creates a function that will evaluate a LessThanOrEqual comparison on parameters of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of parameters to evaluate.</typeparam>
        /// <param name="type">The actual type.</param>
        /// <returns>A less than or equal comparison function.</returns>
        public static Func<T, T, bool> LessThanOrEqualFunc<T>([NotNull] this Type type)
        {
            return ExpressionFunc<T, T, bool>(Expression.LessThanOrEqual, type, type);
        }

        /// <summary>
        /// Creates a function that will evaluate a LessThanOrEqual comparison on parameters of the specified type.
        /// </summary>
        /// <typeparam name="TLHS">The type of the TLHS.</typeparam>
        /// <typeparam name="TRHS">The type of the TRHS.</typeparam>
        /// <param name="typeLHS">The actual type of the LHS of the comparison.</param>
        /// <param name="typeRHS">The actual type of the RHS of the comparison.</param>
        /// <returns>A less than or equal comparison function.</returns>
        public static Func<TLHS, TRHS, bool> LessThanOrEqualFunc<TLHS, TRHS>(
            [NotNull] this Type typeLHS,
            [NotNull] Type typeRHS)
        {
            return ExpressionFunc<TLHS, TRHS, bool>(Expression.LessThanOrEqual, typeLHS, typeRHS);
        }

        /// <summary>
        /// Creates a function that will evaluate a GreaterThan comparison on parameters of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of parameters to evaluate.</typeparam>
        /// <returns>A greater than comparison function.</returns>
        public static Func<T, T, bool> GreaterThanFunc<T>()
        {
            return ExpressionFunc<T, T, bool>(Expression.GreaterThan, typeof(T), typeof(T));
        }

        /// <summary>
        /// Creates a function that will evaluate a GreaterThan comparison on parameters of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of parameters to evaluate.</typeparam>
        /// <param name="type">The actual type.</param>
        /// <returns>A greater than comparison function.</returns>
        public static Func<T, T, bool> GreaterThanFunc<T>([NotNull] this Type type)
        {
            return ExpressionFunc<T, T, bool>(Expression.GreaterThan, type, type);
        }

        /// <summary>
        /// Creates a function that will evaluate a GreaterThan comparison on parameters of the specified type.
        /// </summary>
        /// <typeparam name="TLHS">The type of the TLHS.</typeparam>
        /// <typeparam name="TRHS">The type of the TRHS.</typeparam>
        /// <param name="typeLHS">The actual type of the LHS for the comparison.</param>
        /// <param name="typeRHS">The actual type of the RHS for the comparison.</param>
        /// <returns>A greater than comparison function.</returns>
        public static Func<TLHS, TRHS, bool> GreaterThanFunc<TLHS, TRHS>(
            [NotNull] this Type typeLHS,
            [NotNull] Type typeRHS)
        {
            return ExpressionFunc<TLHS, TRHS, bool>(Expression.GreaterThan, typeLHS, typeRHS);
        }

        /// <summary>
        /// Creates a function that will evaluate a GreaterThanOrEqual comparison on parameters of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of parameters to evaluate.</typeparam>
        /// <returns>A greater than or equal comparison function.</returns>
        public static Func<T, T, bool> GreaterThanOrEqualFunc<T>()
        {
            return ExpressionFunc<T, T, bool>(Expression.GreaterThanOrEqual, typeof(T), typeof(T));
        }

        /// <summary>
        /// Creates a function that will evaluate a GreaterThanOrEqual comparison on parameters of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of parameters to evaluate.</typeparam>
        /// <param name="type">The actual type.</param>
        /// <returns>A greater than or equal comparison function.</returns>
        public static Func<T, T, bool> GreaterThanOrEqualFunc<T>([NotNull] this Type type)
        {
            return ExpressionFunc<T, T, bool>(Expression.GreaterThanOrEqual, type, type);
        }

        /// <summary>
        /// Creates a function that will evaluate a GreaterThanOrEqual comparison on parameters of the specified type.
        /// </summary>
        /// <typeparam name="TLHS">The type of the TLHS.</typeparam>
        /// <typeparam name="TRHS">The type of the TRHS.</typeparam>
        /// <param name="typeLHS">The actual type of the LHS for the comparison.</param>
        /// <param name="typeRHS">The actual type of the RHS for the comparison.</param>
        /// <returns>A greater than or equal comparison function.</returns>
        public static Func<TLHS, TRHS, bool> GreaterThanOrEqualFunc<TLHS, TRHS>(
            [NotNull] this Type typeLHS,
            [NotNull] Type typeRHS)
        {
            return ExpressionFunc<TLHS, TRHS, bool>(Expression.GreaterThanOrEqual, typeLHS, typeRHS);
        }

        /// <summary>
        /// Creates a function that will evaluate a conditional AND operation on parameters of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of parameters to evaluate.</typeparam>
        /// <returns>A conditional AND operator function that only evaluates the second operand if the first operand evaluates true.</returns>
        public static Func<T, T, bool> AndAlsoFunc<T>()
        {
            return ExpressionFunc<T, T, bool>(Expression.AndAlso, typeof(T), typeof(T));
        }

        /// <summary>
        /// Creates a function that will evaluate a conditional AND operation on parameters of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of parameters to evaluate.</typeparam>
        /// <param name="type">The actual type.</param>
        /// <returns>A conditional AND operator function that only evaluates the second operand if the first operand evaluates true.</returns>
        public static Func<T, T, bool> AndAlsoFunc<T>([NotNull] this Type type)
        {
            return ExpressionFunc<T, T, bool>(Expression.AndAlso, type, type);
        }

        /// <summary>
        /// Creates a function that will evaluate a conditional AND operation on parameters of the specified type.
        /// </summary>
        /// <typeparam name="TLHS">The type of the TLHS.</typeparam>
        /// <typeparam name="TRHS">The type of the TRHS.</typeparam>
        /// <param name="typeLHS">The actual type of the LHS for the conditional operation.</param>
        /// <param name="typeRHS">The actual type of the RHS for the conditional operation.</param>
        /// <returns>A conditional AND operator function that only evaluates the second operand if the first operand evaluates true.</returns>
        public static Func<TLHS, TRHS, bool> AndAlsoFunc<TLHS, TRHS>(
            [NotNull] this Type typeLHS,
            [NotNull] Type typeRHS)
        {
            return ExpressionFunc<TLHS, TRHS, bool>(Expression.AndAlso, typeLHS, typeRHS);
        }

        /// <summary>
        /// Creates a function that will evaluate a conditional OR operation on parameters of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of parameters to evaluate.</typeparam>
        /// <returns>A conditional OR operator function that only evaluates the second operand if the first operand evaluates false.</returns>
        public static Func<T, T, bool> OrElseFunc<T>()
        {
            return ExpressionFunc<T, T, bool>(Expression.OrElse, typeof(T), typeof(T));
        }

        /// <summary>
        /// Creates a function that will evaluate a conditional OR operation on parameters of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of parameters to evaluate.</typeparam>
        /// <param name="type">The actual type.</param>
        /// <returns>A conditional OR operator function that only evaluates the second operand if the first operand evaluates false.</returns>
        public static Func<T, T, bool> OrElseFunc<T>([NotNull] this Type type)
        {
            return ExpressionFunc<T, T, bool>(Expression.OrElse, type, type);
        }

        /// <summary>
        /// Creates a function that will evaluate a conditional OR operation on parameters of the specified type.
        /// </summary>
        /// <typeparam name="TLHS">The type of the TLHS.</typeparam>
        /// <typeparam name="TRHS">The type of the TRHS.</typeparam>
        /// <param name="typeLHS">The actual type of the LHS for the conditional operation.</param>
        /// <param name="typeRHS">The actual type of the RHS for the conditional operation.</param>
        /// <returns>A conditional OR operator function that only evaluates the second operand if the first operand evaluates false.</returns>
        public static Func<TLHS, TRHS, bool> OrElseFunc<TLHS, TRHS>([NotNull] this Type typeLHS, [NotNull] Type typeRHS)
        {
            return ExpressionFunc<TLHS, TRHS, bool>(Expression.OrElse, typeLHS, typeRHS);
        }

        /// <summary>
        /// Creates a function from a binary expression that handles casts where necessary.
        /// </summary>
        /// <typeparam name="TLHS">The type of the LHS of the addition.</typeparam>
        /// <typeparam name="TRHS">The type of the RHS of the addition.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="binaryFunc">The binary func.</param>
        /// <param name="typeLHS">The actual type of the LHS of the addition.</param>
        /// <param name="typeRHS">The actual type of the RHS of the addition.</param>
        /// <returns>An addition function.</returns>
        public static Func<TLHS, TRHS, TResult> ExpressionFunc<TLHS, TRHS, TResult>(
            [NotNull] this Func<Expression, Expression, Expression> binaryFunc,
            [NotNull] Type typeLHS,
            [NotNull] Type typeRHS)
        {
            if (binaryFunc == null) throw new ArgumentNullException("binaryFunc");

            // Create input parameter expressions
            ParameterExpression parameterAExpression = Expression.Parameter(typeof(TLHS), "a");
            ParameterExpression parameterBExpression = Expression.Parameter(typeof(TRHS), "b");

            Expression lhs = typeLHS != typeof(TLHS)
                ? Convert(parameterAExpression, typeLHS)
                : parameterAExpression;
            Expression rhs = typeRHS != typeof(TRHS)
                ? Convert(parameterBExpression, typeRHS)
                : parameterBExpression;

            // Create lambda for addition and compile
            Expression expression = binaryFunc(lhs, rhs);
            if (expression == null)
                throw new ArgumentException(Resources.Reflection_ExpressionFunc_FunctionReturnedNull, "binaryFunc");

            if (expression.Type != typeof(TResult))
                expression = Expression.Convert(expression, typeof(TResult));

            return (Func<TLHS, TRHS, TResult>)
                Expression.Lambda(
                    expression,
                    parameterAExpression,
                    parameterBExpression).Compile();
        }

        /// <summary>
        /// Determines whether the specified type is optional.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///     <see langword="true" /> if the specified type is <see cref="Optional{T}" />; otherwise, <see langword="false" />.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOptional([NotNull] this Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Optional<>);
        }

        /// <summary>
        /// Determines whether the specified value is optional.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><see langword="true" /> if the specified type is <see cref="Optional{T}" />; otherwise, <see langword="false" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOptional(this object value)
        {
            IOptional optional = value as IOptional;
            return optional != null;
        }

        /// <summary>
        /// Gets the type of the non optional equivalent of a type (or the original type if already not optional).
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Type.</returns>
        [NotNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type GetNonOptionalType([NotNull] this Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            // ReSharper disable once AssignNullToNotNullAttribute
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Optional<>)
                ? type.GetGenericArguments()[0]
                : type;
        }

        /// <summary>
        /// Gets the optional type equivalent of the non optional equivalent of a type (or the original type if already optional).
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Type.</returns>
        [NotNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type GetOptionalType([NotNull] this Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Optional<>)
                ? type
                : typeof(Optional<>).MakeGenericType(type);
        }

        /// <summary>
        /// Holds optional nulls by type
        /// </summary>
        [NotNull]
        private static readonly ConcurrentDictionary<Type, object> _optionalDefaultAssigneds =
            new ConcurrentDictionary<Type, object>();

        /// <summary>
        /// Gets the optional default assigned value for the optional of type <paramref name="type" />
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Type.</returns>
        public static object DefaultAssigned([NotNull] this Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            if (!type.IsGenericType ||
                (type.GetGenericTypeDefinition() != typeof(Optional<>)))
                return type.Default();

            return _optionalDefaultAssigneds.GetOrAdd(
                type,
                t =>
                {
                    Debug.Assert(t != null);

                    if (!t.IsOptional())
                        t = typeof(Optional<>).MakeGenericType(t);
                    // ReSharper disable once PossibleNullReferenceException
                    return t.GetField(
                        "DefaultAssigned",
                        BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly |
                        BindingFlags.GetField)
                        .GetValue(null);
                });
        }

        /// <summary>
        /// Determines whether the specified value is an unassigned optional.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     <see langword="true" /> if the specified value is unassigned; otherwise, <see langword="false" />.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsUnassigned(this object value)
        {
            IOptional o = value as IOptional;
            return (o != null) && !o.IsAssigned;
        }

        /// <summary>
        /// Converts a value to an Optional assigned value, unless it is already Optional, in which case it gives a
        /// non-type specific version of the optional.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Object.</returns>
        public static Optional<object> ToOptional(this object value)
        {
            IOptional o = value as IOptional;
            if (o != null)
                return o.IsAssigned
                    ? new Optional<object>(o.Value)
                    : Optional<object>.Unassigned;

            return ReferenceEquals(value, null)
                ? Optional<object>.DefaultAssigned
                : new Optional<object>(value);
        }

        /// <summary>
        /// Whether the type can accept <see langword="null" />.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///     <see langword="true" /> if the specified value can accept <see langword="null" />; otherwise,
        ///     <see langword="false" />.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullable([NotNull] this Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            type = GetNonOptionalType(type);
            return type.IsClass ||
                   type.IsInterface ||
                   type.IsNullableType();
        }

        /// <summary>
        /// Determines whether the specified type is nullable.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///     <see langword="true" /> if the specified type is <see cref="Nullable{T}" />; otherwise, <see langword="false" />.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullableType([NotNull] this Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            type = GetNonOptionalType(type);
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        /// Gets the nullable version of a type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Type.</returns>
        [NotNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type GetNullableType([NotNull] this Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            type = GetNonOptionalType(type);
            if (!type.IsClass &&
                !type.IsInterface &&
                !type.IsNullableType())
                type = typeof(Nullable<>).MakeGenericType(type);
            return type;
        }

        /// <summary>
        /// Gets the non-nullable version of a type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Type.</returns>
        [NotNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type GetNonNullableType([NotNull] this Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            type = GetNonOptionalType(type);
            if (type.IsNullableType())
                type = type.GetGenericArguments()[0];
            Debug.Assert(type != null);
            return type;
        }

        /// <summary>
        /// Determines whether the specified type is numeric.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///     <see langword="true" /> if the specified type is numeric; otherwise, <see langword="false" />.
        /// </returns>
        public static bool IsNumeric([NotNull] this Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            type = type.GetNonNullableType();
            if (type.IsEnum)
                return false;

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Determines whether the specified value is null (includes <see cref="DBNull.Value"/>, <see cref="Optional{T}"/> and <see cref="INullable"/> support).
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><see langword="true" /> if the specified value is null; otherwise, <see langword="false" />.</returns>
        [ContractAnnotation("value:null=>true")]
        public static bool IsNull([CanBeNull] this object value)
        {
            if (ReferenceEquals(value, null) ||
                ReferenceEquals(value, DBNull.Value))
                return true;
            INullable nullable = value as INullable;
            return !ReferenceEquals(nullable, null) && nullable.IsNull;
        }

        /// <summary>
        /// Determines whether the given <paramref name="member" /> is compiler generated.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        public static bool IsCompilerGenerated(
            [NotNull] this MemberInfo member)
        {
            if (member == null) throw new ArgumentNullException("member");
            // ReSharper disable once AssignNullToNotNullAttribute
            return member.GetCustomAttributes(typeof(CompilerGeneratedAttribute)).Any();
        }

        /// <summary>
        /// Gets the type of the items in the enumerable type given, or <see langword="null"/> if the type is not an enumerable.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The type of the items in the enumerable type given, or <see langword="null"/> if the type is not an enumerable</returns>
        public static Type GetEnumerableItemType([NotNull] this Type type)
        {
            Type itemType;
            return IsEnumerable(type, out itemType) ? itemType : null;
        }

        /// <summary>
        /// Determines whether the specified type is an <see cref="IEnumerable{T}" />.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">type</exception>
        public static bool IsEnumerable([NotNull] this Type type)
        {
            Type itemType;
            return IsEnumerable(type, out itemType);
        }

        /// <summary>
        /// Determines whether the specified type is an <see cref="IEnumerable{T}" />.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <param name="itemType">If the type is an <see cref="IEnumerable{T}" /> with type arguments supplied,
        /// will contain the type of the items in the enumerable. Otherwise <see langword="null" />.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">type</exception>
        [ContractAnnotation("=>false,itemType:null;=>true,itemType:canbenull")]
        public static bool IsEnumerable([NotNull] this Type type, out Type itemType)
        {
            if (type == null) throw new ArgumentNullException("type");

            itemType = null;

            if (type == typeof(IEnumerable<>))
                return true;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                itemType = type.GetGenericArguments()[0];
                return true;
            }

            foreach (Type iface in type.GetInterfaces())
            {
                Debug.Assert(iface != null);

                if (iface == typeof(IEnumerable<>))
                    return true;
                if (iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    itemType = iface.GetGenericArguments()[0];
                    return true;
                }
            }

            return false;
        }
    }
}