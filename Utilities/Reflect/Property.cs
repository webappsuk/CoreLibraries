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
        [NotNull]
        public readonly ExtendedType ExtendedType;

        /// <summary>
        ///   The property info object, which provides access to property metadata. 
        /// </summary>
        [NotNull]
        public readonly PropertyInfo Info;

        /// <summary>
        ///   Grabs the getter method lazily.
        /// </summary>
        [NotNull]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Lazy<MethodInfo> _getMethod;

        /// <summary>
        ///   Gets the getter method.
        /// </summary>
        [CanBeNull]
        public MethodInfo GetMethod
        {
            get { return _getMethod.Value; }
        }

        /// <summary>
        ///   Grabs the setter method lazily.
        /// </summary>
        [NotNull]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Lazy<MethodInfo> _setMethod;

        /// <summary>
        ///   Gets the setter method.
        /// </summary>
        [CanBeNull]
        public MethodInfo SetMethod
        {
            get { return _setMethod.Value; }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Property"/> class.
        /// </summary>
        /// <param name="extendedType">The extended type.</param>
        /// <param name="info">
        ///   The <see cref="System.Reflection.PropertyInfo">property info</see>.
        /// </param>
        internal Property([NotNull]ExtendedType extendedType, [NotNull] PropertyInfo info)
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
                            field = info.DeclaringType.Module.ResolveField(BitConverter.ToInt32(fieldToken, 0), typeArguments, null);
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
        /// Performs an implicit conversion from <see cref="WebApplications.Utilities.Relection.Property"/> to <see cref="System.Reflection.PropertyInfo"/>.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>The result of the conversion.</returns>
        /// <remarks></remarks>
        public static implicit operator PropertyInfo(Property property)
        {
            return property == null ? null : property.Info;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Reflection.PropertyInfo"/> to <see cref="WebApplications.Utilities.Relection.Property"/>.
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
        /// Gets the field type.
        /// </summary>
        /// <value>The field type.</value>
        /// <remarks></remarks>
        public Type ReturnType
        {
            get { return Info.PropertyType; }
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
            Type returnType = typeof(TValue);
            Type declaringType = ExtendedType.Type;

            // Check the return type can be assigned from the member type
            if ((returnType != propertyType) &&
                (!returnType.IsAssignableFrom(propertyType)))
                return null;

            // Get a member access expression
            Expression expression = Expression.Property(null, Info);

            Contract.Assert(expression != null);

            // Cast return value if necessary
            if (returnType != propertyType)
                expression = expression.Convert(returnType);

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
            Type parameterType = typeof(T);

            //  Check the parameter type can be assigned from the declaring type.
            if ((parameterType != declaringType) &&
                (!parameterType.IsAssignableFrom(declaringType)))
                return null;

            Type returnType = typeof(TValue);

            // Check the return type can be assigned from the member type
            if ((returnType != propertyType) &&
                (!returnType.IsAssignableFrom(propertyType)))
                return null;

            // Create input parameter expression
            ParameterExpression parameterExpression = Expression.Parameter(parameterType, "target");

            // Cast parameter if necessary
            Expression expression = parameterType != declaringType
                             ? parameterExpression.Convert(declaringType)
                             : parameterExpression;

            // Get a member access expression
            expression = Expression.Property(expression, Info);

            Contract.Assert(expression != null);
            Contract.Assert(returnType != null);

            // Cast return value if necessary
            if (returnType != propertyType)
                expression = expression.Convert(returnType);

            Contract.Assert(expression != null);
            Contract.Assert(parameterExpression != null);

            // Create lambda and compile
            return Expression.Lambda<Func<T, TValue>>(expression, parameterExpression).Compile();
        }

        /// <summary>
        /// Retrieves the lambda action equivalent of the specified static setter method.
        /// </summary>
        /// <typeparam name="T">The type of the parameter the function encapsulates.</typeparam>	
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
            Type valueType = typeof(TValue);

            // Check the value type can be assigned to the member type
            if ((valueType != propertyType) &&
                (!propertyType.IsAssignableFrom(valueType)))
                return null;

            // Get a field access expression
            Expression expression = Expression.Property(null, Info);

            // Create value parameter expression
            ParameterExpression valueParameterExpression = Expression.Parameter(
                valueType, "value");
            Expression valueExpression = valueType != propertyType
                                          ? valueParameterExpression.Convert(propertyType)
                                          : valueParameterExpression;

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
            Type parameterType = typeof(T);

            //  Check the parameter type can be assigned from the declaring type.
            if ((parameterType != declaringType) &&
                (!parameterType.IsAssignableFrom(declaringType)))
                return null;

            Type propertyType = Info.PropertyType;
            Type valueType = typeof(TValue);

            // Check the value type can be assigned to the member type
            if ((valueType != propertyType) &&
                (!propertyType.IsAssignableFrom(valueType)))
                return null;

            // Create input parameter expression
            ParameterExpression parameterExpression = Expression.Parameter(
                parameterType, "target");

            // Cast parameter if necessary
            Expression expression = parameterType != declaringType
                             ? parameterExpression.Convert(declaringType)
                             : parameterExpression;

            // Get a member access expression
            expression = Expression.Property(expression, Info);

            // Create value parameter expression
            ParameterExpression valueParameterExpression = Expression.Parameter(
                valueType, "value");
            Expression valueExpression = valueType != propertyType
                                          ? valueParameterExpression.Convert(propertyType)
                                          : valueParameterExpression;

            Contract.Assert(expression != null);
            Contract.Assert(valueExpression != null);

            // Create assignment
            expression = Expression.Assign(expression, valueExpression);

            Contract.Assert(expression != null);
            Contract.Assert(parameterExpression != null);

            // Create lambda and compile
            return
                Expression.Lambda<Action<T, TValue>>(expression, parameterExpression, valueParameterExpression).Compile();
        }

        [NotNull]
        private readonly Lazy<Field> _automaticField;

        /// <summary>
        /// Gets a value indicating whether this property is automatic.
        /// </summary>
        /// <value><see langword="true" /> if this instance is automatic; otherwise, <see langword="false" />.</value>
        /// <remarks></remarks>
        public bool IsAutomatic { get { return _automaticField.Value != null; } }

        /// <summary>
        /// Gets the underlying automatic field (if any).
        /// </summary>
        /// <value>The automatic field.</value>
        /// <remarks></remarks>
        [CanBeNull]
        public Field AutomaticField { get { return _automaticField.Value; } }
    }
}