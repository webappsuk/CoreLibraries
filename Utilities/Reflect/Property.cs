using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reflection;
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
        /// <param name="checkAssignability">If set to <see langword="true" /> performs assignability checks.</param>
        /// <returns>A function that takes an object of the type T and returns the value of the property.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        /// <remarks></remarks>
        [UsedImplicitly]
        [CanBeNull]
        public Func<TValue> Getter<TValue>(
#if DEBUG
 bool checkAssignability = true
#else
            bool checkAssignability = false
#endif
)
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
            if ((checkAssignability) && (returnType != propertyType) &&
                (!returnType.IsAssignableFrom(propertyType)))
            {
                throw new ArgumentOutOfRangeException(
                    String.Format(
                        Resources.Reflection_GetGetter_ReturnTypeNotAssignable,
                        Info.Name,
                        "property",
                        declaringType,
                        propertyType,
                        returnType));
            }

            // Get a member access expression
            Expression expression = Expression.Property(null, Info);

            Contract.Assert(expression != null);

            // Cast return value if necessary
            if (returnType != propertyType)
                expression = Expression.Convert(expression, returnType);

            Contract.Assert(expression != null);

            // Create lambda and compile
            return (Func<TValue>)Expression.Lambda(expression).Compile();
        }

        /// <summary>
        /// Retrieves the lambda function equivalent of the specified instance getter method.
        /// </summary>
        /// <typeparam name="T">The type of the parameter the function encapsulates.</typeparam>	
        /// <typeparam name="TValue">The type of the value returned.</typeparam>	
        /// <param name="checkAssignability">If set to <see langword="true" /> performs assignability checks.</param>
        /// <returns>A function that takes an object of the type T and returns the value of the property.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        /// <remarks></remarks>
        [UsedImplicitly]
        [CanBeNull]
        public Func<T, TValue> Getter<T, TValue>(
#if DEBUG
 bool checkAssignability = true
#else
            bool checkAssignability = false
#endif
)
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
            if ((checkAssignability) && (parameterType != declaringType) &&
                (!parameterType.IsAssignableFrom(declaringType)))
            {
                throw new ArgumentOutOfRangeException(
                    String.Format(
                        Resources.Reflection_GetGetter_ParameterTypeNotAssignable,
                        Info.Name,
                        "property",
                        declaringType,
                        parameterType));
            }

            Type returnType = typeof(TValue);

            // Check the return type can be assigned from the member type
            if ((checkAssignability) && (returnType != propertyType) &&
                (!returnType.IsAssignableFrom(propertyType)))
            {
                throw new ArgumentOutOfRangeException(
                    String.Format(
                        Resources.Reflection_GetGetter_ReturnTypeNotAssignable,
                        Info.Name,
                        "field",
                        declaringType,
                        propertyType,
                        returnType));
            }

            // Create input parameter expression
            ParameterExpression parameterExpression = Expression.Parameter(parameterType, "target");

            // Cast parameter if necessary
            Expression expression = parameterType != declaringType
                             ? (Expression)Expression.Convert(parameterExpression, declaringType)
                             : parameterExpression;

            // Get a member access expression
            expression = Expression.Property(expression, Info);

            Contract.Assert(expression != null);
            Contract.Assert(returnType != null);

            // Cast return value if necessary
            if (returnType != propertyType)
                expression = Expression.Convert(expression, returnType);

            Contract.Assert(expression != null);
            Contract.Assert(parameterExpression != null);

            // Create lambda and compile
            return (Func<T, TValue>)Expression.Lambda(expression, parameterExpression).Compile();
        }
    }
}