#region © Copyright Web Applications (UK) Ltd, 2013.  All rights reserved.
// Copyright (c) 2013, Web Applications UK Ltd
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
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Threading;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Reflect
{
    /// <summary>
    ///   Wraps a <see cref="System.Reflection.PropertyInfo"/> with accessors.
    /// </summary>
    [DebuggerDisplay("{Info} [Extended]")]
    public class Property
    {
        /// <summary>
        /// The extended type.
        /// </summary>
        [NotNull] public readonly ExtendedType ExtendedType;

        /// <summary>
        ///   The property info object, which provides access to property metadata. 
        /// </summary>
        [NotNull] public readonly PropertyInfo Info;

        [NotNull] private readonly Lazy<Field> _automaticField;

        /// <summary>
        ///   Grabs the getter method lazily.
        /// </summary>
        [NotNull] [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly Lazy<MethodInfo> _getMethod;

        /// <summary>
        ///   Grabs the setter method lazily.
        /// </summary>
        [NotNull] [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly Lazy<MethodInfo> _setMethod;

        /// <summary>
        ///   Initializes a new instance of the <see cref="Property"/> class.
        /// </summary>
        /// <param name="extendedType">The extended type.</param>
        /// <param name="info">
        ///   The <see cref="System.Reflection.PropertyInfo">property info</see>.
        /// </param>
        internal Property([NotNull] ExtendedType extendedType, [NotNull] PropertyInfo info)
        {
            ExtendedType = extendedType;
            Info = info;
            _getMethod = new Lazy<MethodInfo>(() => info.GetGetMethod(true), LazyThreadSafetyMode.PublicationOnly);
            _setMethod = new Lazy<MethodInfo>(() => info.GetSetMethod(true), LazyThreadSafetyMode.PublicationOnly);

            // Tries to find the underlying field for an automatic property.
            _automaticField = new Lazy<Field>(
                () =>
                    {
                        MethodInfo getMethod;
                        MethodBody methodBody;

                        // If the get/set accessor is missing or we can't retrieve the method body for the get accessor,
                        // then we're not an automatic property.
                        if (!info.CanRead || !info.CanWrite || ((getMethod = info.GetGetMethod()) == null) ||
                            ((methodBody = getMethod.GetMethodBody()) == null))
                            return null;

                        // Evaluate MSIL to resolve underlying field that is accessed.
                        byte[] getter = methodBody.GetILAsByteArray();
                        byte ldfld = (byte) (info.GetGetMethod().IsStatic ? OpCodes.Ldsfld : OpCodes.Ldfld).Value;
                        byte[] fieldToken = getter.SkipWhile(b => b != ldfld).Skip(1).Take(4).ToArray();
                        if (fieldToken.Length != 4)
                            return null;

                        // Grab the field
                        FieldInfo field;
                        try
                        {
                            Type[] typeArguments = ExtendedType.GenericArguments.Select(g => g.Type).ToArray();
                            if (typeArguments.Length < 1)
                                typeArguments = null;
                            field = info.DeclaringType.Module.ResolveField(BitConverter.ToInt32(fieldToken, 0),
                                                                           typeArguments, null);
                        }
                        catch
                        {
                            return null;
                        }

                        // Compilers don't strictly have to add this attribute, so could relax this check, but this ensures
                        // that we are indeed looking at an automatic property.
                        return field != null && field.IsDefined(typeof (CompilerGeneratedAttribute), false)
                                   ? field
                                   : null;
                    }, LazyThreadSafetyMode.PublicationOnly);
        }

        /// <summary>
        ///   Gets the getter method.
        /// </summary>
        [CanBeNull]
        public MethodInfo GetMethod
        {
            get { return _getMethod.Value; }
        }

        /// <summary>
        ///   Gets the setter method.
        /// </summary>
        [CanBeNull]
        public MethodInfo SetMethod
        {
            get { return _setMethod.Value; }
        }

        /// <summary>
        /// Gets the field type.
        /// </summary>
        /// <value>The field type.</value>
        /// <remarks></remarks>
        public Type ReturnType
        {
            get { return Info.PropertyType; }
        }

        /// <summary>
        /// Gets a value indicating whether this property is automatic.
        /// </summary>
        /// <value><see langword="true" /> if this instance is automatic; otherwise, <see langword="false" />.</value>
        /// <remarks></remarks>
        public bool IsAutomatic
        {
            get { return _automaticField.Value != null; }
        }

        /// <summary>
        /// Gets the underlying automatic field (if any).
        /// </summary>
        /// <value>The automatic field.</value>
        /// <remarks></remarks>
        [CanBeNull]
        public Field AutomaticField
        {
            get { return _automaticField.Value; }
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="WebApplications.Utilities.Reflect.Property"/> to <see cref="System.Reflection.PropertyInfo"/>.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>The result of the conversion.</returns>
        /// <remarks></remarks>
        public static implicit operator PropertyInfo(Property property)
        {
            return property == null ? null : property.Info;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Reflection.PropertyInfo"/> to <see cref="WebApplications.Utilities.Reflect.Property"/>.
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        /// <returns>The result of the conversion.</returns>
        /// <remarks></remarks>
        public static implicit operator Property(PropertyInfo propertyInfo)
        {
            return propertyInfo == null
                       ? null
                       : ((ExtendedType) propertyInfo.DeclaringType).GetProperty(propertyInfo);
        }

        /// <summary>
        /// Retrieves the lambda function equivalent of the specified static getter method.
        /// </summary>
        /// <typeparam name="TValue">The type of the value returned.</typeparam>	
        /// <returns>A function that takes an object of the type T and returns the value of the property.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        /// <remarks></remarks>
        [UsedImplicitly]
        [CanBeNull]
        public Func<TValue> Getter<TValue>()
        {
            MethodInfo getMethod = GetMethod;

            // Only valid for static properties.
            if (getMethod == null ||
                !getMethod.IsStatic)
                return null;

            Type propertyType = Info.PropertyType;
            Type returnType = typeof (TValue);

            // Check the return type can be assigned from the member type
            if ((returnType != propertyType) &&
                (!returnType.IsAssignableFrom(propertyType)))
                return null;

            // Get a member access expression
            Expression expression = Expression.Property(null, Info);

            Contract.Assert(expression != null);

            // Cast return value if necessary
            if ((returnType != propertyType) &&
                !expression.TryConvert(returnType, out expression))
                return null;

            Contract.Assert(expression != null);

            // Create lambda and compile
            return Expression.Lambda<Func<TValue>>(expression).Compile();
        }

        /// <summary>
        /// Retrieves the lambda function equivalent of the specified instance getter method.
        /// </summary>
        /// <typeparam name="T">The type of the parameter the function encapsulates.</typeparam>	
        /// <typeparam name="TValue">The type of the value returned.</typeparam>	
        /// <returns>A function that takes an object of the type T and returns the value of the property.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        /// <remarks></remarks>
        [UsedImplicitly]
        [CanBeNull]
        public Func<T, TValue> Getter<T, TValue>()
        {
            MethodInfo getMethod = GetMethod;

            // Only valid for instance properties.
            if (getMethod == null ||
                getMethod.IsStatic)
                return null;

            Type propertyType = Info.PropertyType;
            Type declaringType = ExtendedType.Type;
            Type parameterType = typeof (T);
            Type returnType = typeof (TValue);

            // Create input parameter expression
            ParameterExpression parameterExpression = Expression.Parameter(parameterType, "target");

            // Cast parameter if necessary
            Expression expression = parameterExpression;
            if ((parameterType != declaringType) &&
                !expression.TryConvert(declaringType, out expression))
                return null;

            // Get a member access expression
            expression = Expression.Property(expression, Info);

            Contract.Assert(expression != null);
            Contract.Assert(returnType != null);

            // Cast return value if necessary
            if ((returnType != propertyType) &&
                !expression.TryConvert(returnType, out expression))
                return null;

            Contract.Assert(expression != null);
            Contract.Assert(parameterExpression != null);

            // Create lambda and compile
            return Expression.Lambda<Func<T, TValue>>(expression, parameterExpression).Compile();
        }

        /// <summary>
        /// Retrieves the lambda action equivalent of the specified static setter method.
        /// </summary>
        /// <typeparam name="TValue">The type of the value returned.</typeparam>	
        /// <returns>An action that sets the value of the relevant static field.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        /// <remarks></remarks>
        [UsedImplicitly]
        [CanBeNull]
        public Action<TValue> Setter<TValue>()
        {
            MethodInfo setMethod = SetMethod;

            // Only valid for static properties.
            if ((setMethod == null) ||
                (!setMethod.IsStatic))
                return null;

            Type propertyType = Info.PropertyType;
            Type valueType = typeof (TValue);

            // Get a field access expression
            Expression expression = Expression.Property(null, Info);

            // Create value parameter expression
            ParameterExpression valueParameterExpression = Expression.Parameter(
                valueType, "value");
            Expression valueExpression = valueParameterExpression;

            // Convert value parameter if necessary
            if ((valueType != propertyType) &&
                !valueExpression.TryConvert(propertyType, out valueExpression))
                return null;

            Contract.Assert(expression != null);
            Contract.Assert(valueExpression != null);

            // Create assignment
            expression = Expression.Assign(expression, valueExpression);

            Contract.Assert(expression != null);

            // Create lambda and compile
            return
                Expression.Lambda<Action<TValue>>(expression, valueParameterExpression).Compile();
        }

        /// <summary>
        /// Retrieves the lambda action equivalent of the specified instance setter method.
        /// </summary>
        /// <typeparam name="T">The type of the parameter the function encapsulates.</typeparam>	
        /// <typeparam name="TValue">The type of the value returned.</typeparam>	
        /// <returns>An action that takes an object of the type T and sets the value of the relevant field.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        /// <remarks></remarks>
        [UsedImplicitly]
        [CanBeNull]
        public Action<T, TValue> Setter<T, TValue>()
        {
            MethodInfo setMethod = SetMethod;

            // Only valid for instance properties.
            if ((setMethod == null) ||
                (setMethod.IsStatic))
                return null;

            Type declaringType = ExtendedType.Type;
            Type parameterType = typeof (T);
            Type propertyType = Info.PropertyType;
            Type valueType = typeof (TValue);

            // Create input parameter expression
            ParameterExpression parameterExpression = Expression.Parameter(
                parameterType, "target");

            // Cast parameter if necessary
            Expression expression = parameterExpression;
            if ((parameterType != declaringType) &&
                !expression.TryConvert(declaringType, out expression))
                return null;

            // Get a member access expression
            expression = Expression.Property(expression, Info);

            // Create value parameter expression
            ParameterExpression valueParameterExpression = Expression.Parameter(
                valueType, "value");
            Expression valueExpression = valueParameterExpression;

            // Convert value parameter if necessary
            if ((valueType != propertyType) &&
                !valueExpression.TryConvert(propertyType, out valueExpression))
                return null;

            Contract.Assert(expression != null);
            Contract.Assert(valueExpression != null);

            // Create assignment
            expression = Expression.Assign(expression, valueExpression);

            Contract.Assert(expression != null);
            Contract.Assert(parameterExpression != null);

            // Create lambda and compile
            return
                Expression.Lambda<Action<T, TValue>>(expression, parameterExpression, valueParameterExpression)
                          .Compile();
        }
    }
}