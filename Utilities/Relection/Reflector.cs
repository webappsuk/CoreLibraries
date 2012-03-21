using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Relection
{
    /// <summary>
    ///   Holds reflection information for a class.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    public static class Reflector<T>
    {
        /// <summary>
        ///   Binding flags for returning all fields/properties from a type.
        /// </summary>
        [UsedImplicitly] public const BindingFlags AllMembersBindingFlags =
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static |
            BindingFlags.DeclaredOnly;

        /// <summary>
        ///   A cache for the getter methods, so that when requested they can be retrieved rather than recomputed.
        /// </summary>
        [NotNull]
        private static readonly ConcurrentDictionary<string, object> _getterCache =
            new ConcurrentDictionary<string, object>();

        /// <summary>
        ///   A cache for the setter methods, so that when requested they can be retrieved rather than recomputed.
        /// </summary>
        [NotNull]
        private static readonly ConcurrentDictionary<string, object> _setterCache =
            new ConcurrentDictionary<string, object>();

        /// <summary>
        ///   A cache for all methods, so that when requested they can be retrieved rather than recomputed.
        /// </summary>
        [NotNull]
        private static readonly ConcurrentDictionary<string, object> _funcCache =
            new ConcurrentDictionary<string, object>();

        /// <summary>
        ///   Holds all fields.
        /// </summary>
        [NotNull]
        private static readonly Dictionary<string, FieldInfo> _fields = new Dictionary<string, FieldInfo>();

        /// <summary>
        ///   Gets the fields.
        /// </summary>
        public static IEnumerable<FieldInfo> Fields { get { return _fields.Values; } }

        /// <summary>
        ///   Holds all properties.
        /// </summary>
        [NotNull]
        private static readonly Dictionary<string, Property> _properties = new Dictionary<string, Property>();

        /// <summary>
        ///   Gets the properties.
        /// </summary>
        public static IEnumerable<Property> Properties { get { return _properties.Values; } }

        /// <summary>
        ///   Holds all events.
        /// </summary>
        [NotNull]
        private static readonly Dictionary<string, Event> _events = new Dictionary<string, Event>();

        /// <summary>
        ///   Gets the events.
        /// </summary>
        public static IEnumerable<Event> Events { get { return _events.Values; } }

        /// <summary>
        ///   Holds all methods.
        /// </summary>
        [NotNull]
        private static readonly Dictionary<string, Method> _methods = new Dictionary<string, Method>();

        /// <summary>
        ///   Gets the methods.
        /// </summary>
        public static IEnumerable<Method> Methods { get { return _methods.Values; } }

        /// <summary>
        /// Holds all constructors.
        /// </summary>
        [NotNull]
        private static readonly Dictionary<string, Constructor> _constructors = new Dictionary<string, Constructor>();

        /// <summary>
        ///   Gets the constructors.
        /// </summary>
        public static IEnumerable<Constructor> Constructors { get { return _constructors.Values; } }

        /// <summary>
        ///   All the customer attributes on the type.
        /// </summary>
        [NotNull]
        public static readonly IEnumerable<Attribute> CustomerAttributes;

        /// <summary>
        ///   If this type has a default member (indexer), indicates its name.
        /// </summary>
        [CanBeNull]
        public static readonly string DefaultMember;

        /// <summary>
        ///   Initializes a new instance of the <see cref="object"/> class.
        /// </summary>
        static Reflector()
        {
            // Get customer attributes
            CustomerAttributes = typeof (T).GetCustomAttributes(false).Cast<Attribute>();

            // Look for default member.
            DefaultMemberAttribute defaultMemberAttribute =
                CustomerAttributes.OfType<DefaultMemberAttribute>().SingleOrDefault();
            DefaultMember = defaultMemberAttribute != null
                                ? defaultMemberAttribute.MemberName
                                : null;

            // Get all members in one go - this is significantly faster than getting individual calls later.
            foreach (MemberInfo memberInfo in typeof(T).GetMembers(AllMembersBindingFlags))
            {
                // Store fields
                FieldInfo f = memberInfo as FieldInfo;
                if (f != null)
                {
                    _fields.Add(f.Name, f);
                    continue;
                }

                // Store properties/indexers
                PropertyInfo p = memberInfo as PropertyInfo;
                if (p != null)
                {
                    if (p.Name.Equals(DefaultMember))
                    {
                        var indexTypes = p.GetIndexParameters();
                        // TODO Add indexer support.
                    }
                    else
                        _properties.Add(p.Name, new Property(p));
                    continue;
                }

                // Store methods
                MethodInfo m = memberInfo as MethodInfo;
                if (m != null)
                {
                    Method method;
                    if (!_methods.TryGetValue(m.Name, out method))
                    {
                        method = new Method(m);
                        _methods.Add(m.Name, method);
                    }
                    else
                        method.Add(m);
                    continue;
                }

                // Store constructors
                ConstructorInfo c = memberInfo as ConstructorInfo;
                if (c != null)
                {
                    Constructor constructor;
                    if (!_constructors.TryGetValue(c.Name, out constructor))
                    {
                        constructor = new Constructor(c);
                        _constructors.Add(c.Name, constructor);
                    }
                    else
                        constructor.Add(c);
                    continue;
                }

                // Store events
                EventInfo e = memberInfo as EventInfo;
                if (e != null)
                {
                    _events.Add(e.Name, new Event(e));
                    continue;
                }

                // Store types
                Type t = memberInfo as Type;
                if (t == null) return;
            }
        }

        /// <summary>
        ///   Retrieves the lambda function equivalent of the specified getter method.
        /// </summary>
        /// <typeparam name="TValue">The type of the value returned.</typeparam>
        /// <param name="name">The name of the field or property whose getter we want to retrieve.</param>
        /// <param name="checkAssignability">If set to <see langword="true"/> performs assignability checks.</param>
        /// <returns>
        ///   A function that takes an object of the type T and returns the value of the property or field.
        /// </returns>
        [NotNull]
        public static Func<T, TValue> GetGetter<TValue>([NotNull]string name, bool checkAssignability = false)
        {
            return (Func<T, TValue>)GetGetter(name, null, typeof(TValue), checkAssignability);
        }

        /// <summary>
        /// Retrieves the lambda function equivalent of the specified getter method.
        /// </summary>
        /// <param name="name">The name of the field or property whose getter we want.</param>
        /// <param name="parameterType">
        ///   <para>The type of the parameter.</para>
        ///   <para>By default this is reflected type.</para>
        /// </param>
        /// <param name="returnType">
        ///   <para>The return value's type.</para>
        ///   <para>By default this is the member (the field or property) type.</para>
        /// </param>
        /// <param name="checkAssignability">
        ///   If <see langword="true"/> this checks that the member type is assignable to the <paramref name="returnType"/>
        ///   and that the parameter type is assignable from the reflected type.
        /// </param>
        /// <returns>
        ///   A function that takes an object of the reflected type and returns the value of the property or field.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <para>There is no getter for the field or property specified.</para>
        ///   <para>-or-</para>
        ///   <para><paramref name="parameterType"/> is not assignable from reflected type.</para>
        ///   <para>-or-</para>
        ///   <para>The <paramref name="returnType"/> is not assignable from the member type.</para>
        /// </exception>
        /// <seealso cref="System.Type.IsAssignableFrom"/>
        [NotNull]
        public static object GetGetter(
            [NotNull] string name,
            [CanBeNull] Type parameterType = null,
            [CanBeNull] Type returnType = null,
            bool checkAssignability = false)
        {
            if (parameterType == null)
                parameterType = typeof (T);
            MemberInfo memberInfo = typeof (T).GetMember(name, AllMembersBindingFlags).SingleOrDefault();
            if (memberInfo == null)
                throw new InvalidOperationException(String.Format(Resources.Reflector_GetGetter_MemberDoesNotExist,
                                                                  name, typeof (T).FullName));

            return
                _getterCache.GetOrAdd(
                    String.Format("{0}:{1}|{2}|{3}", typeof(T), name, parameterType, returnType),
                    k =>
                    {
                        // Find the field or property
                        bool isField;
                        bool isStatic;
                        Type memberType;
                        FieldInfo fieldInfo = typeof(T).GetField(name, AllMembersBindingFlags);
                        MethodInfo propertyAccesssor;
                        if (fieldInfo != null)
                        {
                            memberType = fieldInfo.FieldType;
                            propertyAccesssor = null;
                            isStatic = fieldInfo.IsStatic;
                            isField = true;
                        }
                        else
                        {
                            PropertyInfo propertyInfo = typeof(T).GetProperty(
                                name, AllMembersBindingFlags);
                            if ((propertyInfo == null) ||
                                ((propertyAccesssor = propertyInfo.GetGetMethod(true)) == null))
                            {
                                throw new ArgumentOutOfRangeException(
                                    "name",
                                    String.Format(
                                        Resources.Reflector_GetGetter_DoesNotHaveGetter,
                                        name,
                                        typeof(T)));
                            }
                            memberType = propertyInfo.PropertyType;
                            isStatic = propertyAccesssor.IsStatic;
                            isField = false;
                        }

                        //  Check the parameter type can be assigned from the declaring type.
                        if (parameterType != null)
                        {
                            if ((checkAssignability) && (parameterType != typeof(T)) &&
                                (!parameterType.IsAssignableFrom(typeof(T))))
                            {
                                throw new ArgumentOutOfRangeException(
                                    "parameterType",
                                    String.Format(
                                        Resources.Reflector_GetGetter_ParameterTypeNotAssignable,
                                        name,
                                        isField ? "field" : "property",
                                        typeof(T),
                                        parameterType));
                            }
                        }
                        else
                            parameterType = typeof(T);

                        // Check the return type can be assigned from the member type
                        if (returnType != null)
                        {
                            if ((checkAssignability) && (returnType != memberType) &&
                                (!returnType.IsAssignableFrom(memberType)))
                            {
                                throw new ArgumentOutOfRangeException(
                                    "returnType",
                                    String.Format(
                                        Resources.Reflector_GetGetter_ReturnTypeNotAssignable,
                                        name,
                                        isField ? "field" : "property",
                                        typeof(T),
                                        memberType,
                                        returnType));
                            }
                        }
                        else
                            returnType = memberType;

                        Expression expression = null;
                        // Create input parameter expression
                        ParameterExpression parameterExpression = Expression.Parameter(
                            parameterType, "target");

                        // If we're not static we need a parameter expression
                        if (!isStatic)
                        {
                            expression = parameterExpression;

                            // Cast parameter if necessary
                            if (parameterType != typeof(T))
                                expression = Expression.Convert(expression, typeof(T));
                        }

                        // Get a member access expression
                        expression = isField
                                         ? Expression.Field(expression, fieldInfo)
                                         : Expression.Property(expression, propertyAccesssor);

                        // Cast return value if necessary
                        if (returnType != memberType)
                            expression = Expression.Convert(expression, returnType);

                        // Create lambda and compile
                        return Expression.Lambda(expression, parameterExpression).Compile();
                    });
        }
    }
}
