using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Holds extended information (including reflection info) for a type.
    /// </summary>
    /// <remarks></remarks>
    [DebuggerDisplay("{Type} [Extended]")]
    public class ExtendedType
    {
        /// <summary>
        /// Holds all known extended types.
        /// </summary>
        [NotNull]
        private static readonly ConcurrentDictionary<Type, ExtendedType> _extendedTypes =
            new ConcurrentDictionary<Type, ExtendedType>();

        /// <summary>
        ///   Binding flags for returning all fields/properties from a type.
        /// </summary>
        [UsedImplicitly]
        public const BindingFlags AllMembersBindingFlags =
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static |
            BindingFlags.DeclaredOnly;

        /// <summary>
        ///   The standard conversion methods implemented by <see cref="System.IConvertible"/>.
        /// </summary>
        /// <remarks>
        ///   Does not include:
        ///   <list type="bullet">
        ///     <item><description><see cref="System.IConvertible.ToType">ToType</see> - It isn't specific.</description></item>
        ///     <item><description><see cref="System.IConvertible.GetTypeCode">GetTypeCode</see> - Isn't actually a conversion method.</description></item>
        ///   </list>
        /// </remarks>
        private static readonly Dictionary<Type, string> _iConvertibleMethods =
            new Dictionary<Type, string>
                {
                    {typeof (bool), "ToBoolean"},
                    {typeof (char), "ToChar"},
                    {typeof (sbyte), "ToSByte"},
                    {typeof (byte), "ToByte"},
                    {typeof (short), "ToInt16"},
                    {typeof (ushort), "ToUInt16"},
                    {typeof (int), "ToInt32"},
                    {typeof (uint), "ToUInt32"},
                    {typeof (long), "ToInt64"},
                    {typeof (ulong), "ToUInt64"},
                    {typeof (float), "ToSingle"},
                    {typeof (double), "ToDouble"},
                    {typeof (decimal), "ToDecimal"},
                    {typeof (DateTime), "ToDateTime"},
                    {typeof (string), "ToString"}
                };

        /// <summary>
        /// The underlying type.
        /// </summary>
        [NotNull]
        public readonly Type Type;

        /// <summary>
        ///   Holds all fields.
        /// </summary>
        [NotNull]
        private readonly Dictionary<string, Field> _fields = new Dictionary<string, Field>();

        /// <summary>
        ///   Gets the fields.
        /// </summary>
        [NotNull]
        public IEnumerable<Field> Fields
        {
            get
            {
                if (!_loaded) LoadMembers();
                return _fields.Values;
            }
        }

        /// <summary>
        /// Holds all indexers.
        /// </summary>
        [NotNull]
        private IEnumerable<Indexer> _indexers;
        /// <summary>
        /// Gets the indexers.
        /// </summary>
        /// <remarks></remarks>
        [NotNull]
        public IEnumerable<Indexer> Indexers
        {
            get
            {
                if (_loaded) LoadMembers();
                return _indexers;
            }
        }

        /// <summary>
        /// Holds all properties.
        /// </summary>
        [NotNull]
        private readonly Dictionary<string, Property> _properties = new Dictionary<string, Property>();

        /// <summary>
        ///   Gets the properties (doesn't include <see cref="Indexers"/>).
        /// </summary>
        [NotNull]
        public IEnumerable<Property> Properties
        {
            get
            {
                if (!_loaded) LoadMembers();
                return _properties.Values;
            }
        }

        /// <summary>
        ///   Holds all events.
        /// </summary>
        [NotNull]
        private readonly Dictionary<string, Event> _events = new Dictionary<string, Event>();

        /// <summary>
        ///   Gets the events.
        /// </summary>
        [NotNull]
        public IEnumerable<Event> Events
        {
            get
            {
                if (!_loaded) LoadMembers();
                return _events.Values;
            }
        }

        /// <summary>
        ///   Holds all methods.
        /// </summary>
        [NotNull]
        private readonly Dictionary<string, List<Method>> _methods = new Dictionary<string, List<Method>>();

        /// <summary>
        ///   Gets all the methods.
        /// </summary>
        [NotNull]
        public IEnumerable<Method> Methods
        {
            get
            {
                if (!_loaded) LoadMembers();
                return _methods.Values.SelectMany(m => m);
            }
        }

        /// <summary>
        /// Gets the static constructor, if any.
        /// </summary>
        /// <value>The static constructor if found; otherwise <see langword="null"/>.</value>
        /// <remarks>The static constructor is a special case and does not appear directly in overloads.</remarks>
        public Constructor StaticConstructor { get; private set; }

        /// <summary>
        /// Holds all constructors.
        /// </summary>
        private IEnumerable<Constructor> _constructors;

        /// <summary>
        ///   Gets the constructors.
        /// </summary>
        [NotNull]
        public IEnumerable<Constructor> Constructors
        {
            get
            {
                if (!_loaded) LoadMembers();
                return _constructors;
            }
        }

        /// <summary>
        /// Calculates custom attributes on demand.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [NotNull]
        private readonly Lazy<IEnumerable<Attribute>> _customAttributes;

        /// <summary>
        ///   All the customer attributes on the type.
        /// </summary>
        [NotNull]
        public IEnumerable<Attribute> CustomAttributes
        {
            get { return _customAttributes.Value ?? Enumerable.Empty<Attribute>(); }
        }

        /// <summary>
        /// Calculates default member on demand.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [NotNull]
        private readonly Lazy<string> _defaultMember;

        /// <summary>
        ///   If this type has a default member (indexer), indicates its name.
        /// </summary>
        [CanBeNull]
        public string DefaultMember
        {
            get { return _defaultMember.Value; }
        }

        /// <summary>
        /// Creates a signature on demand.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [NotNull]
        private readonly Lazy<string> _signature;

        /// <summary>
        /// Gets the signature of the type.
        /// </summary>
        /// <remarks>This is modelled after the Type.SigToString internal method.</remarks>
        [NotNull]
        public string Signature
        {
            get { return _signature.Value ?? this.Type.FullName ?? this.Type.Name; }
        }

        /// <summary>
        /// Creates a simple full name on demand.
        /// </summary>
        [NotNull]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Lazy<string> _simpleFullName;

        /// <summary>
        /// Gets the simple full name for the type.
        /// </summary>
        /// <remarks></remarks>
        public string SimpleFullName
        {
            get { return _simpleFullName.Value ?? this.Type.FullName ?? this.Type.Name; }
        }

        /// <summary>
        /// Creates array of generic arguments on demand.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [NotNull]
        private readonly Lazy<List<GenericArgument>> _genericArguments;

        /// <summary>
        /// The generic arguments.
        /// </summary>
        [NotNull]
        public IEnumerable<GenericArgument> GenericArguments
        {
            get { return _genericArguments.Value; }
        }

        private readonly Lazy<Type> _nonNullableType;

        /// <summary>
        /// If this is a nullable type (i.e. <see cref="Nullable{T}"/>) then returns the underlying non-nullable type;
        /// otherwise returns <see cref="Type"/>.
        /// </summary>
        /// <value>The type of the non nullable type.</value>
        /// <remarks></remarks>
        public Type NonNullableType { get { return _nonNullableType.Value; } }

        /// <summary>
        /// Gets a value indicating whether this type is a nullable type (i.e. <see cref="Nullable{T}"/>).
        /// </summary>
        /// <value><see langword="true" /> if this type is nullable type; otherwise, <see langword="false" />.</value>
        /// <remarks></remarks>
        public bool IsNullableType { get { return Type != _nonNullableType.Value; } }

        private readonly Lazy<bool> _isConvertible;

        /// <summary>
        /// Gets a value indicating whether this type is convertible.
        /// </summary>
        /// <value><see langword="true" /> if this instance is convertible; otherwise, <see langword="false" />.</value>
        /// <remarks></remarks>
        public bool IsConvertible { get { return _isConvertible.Value; } }

        private readonly Lazy<Dictionary<string, Type>> _interfaces;

        /// <summary>
        /// Gets the interfaces implemented by this type.
        /// </summary>
        /// <value>The interfaces.</value>
        /// <remarks></remarks>
        [NotNull]
        public IEnumerable<Type> Interfaces { get { return _interfaces.Value.Values; } }

        /// <summary>
        /// Holds user defined methods that cast from this type to another type.
        /// </summary>
        private Dictionary<Type, CastMethod> _castsTo = new Dictionary<Type, CastMethod>();

        /// <summary>
        /// Holds user defined methods that cast to this type from another type.
        /// </summary>
        private Dictionary<Type, CastMethod> _castsFrom = new Dictionary<Type, CastMethod>();

        /// <summary>
        /// Indicates whether members have been loaded.
        /// </summary>
        private bool _loaded;

        /// <summary>
        /// Spinlock for locking during member load.
        /// </summary>
        private SpinLock _loadLock = new SpinLock();

        // ReSharper restore StaticFieldInGenericType

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedType"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <remarks></remarks>
        public ExtendedType([NotNull] Type type)
        {
            this.Type = type;

            // Create lazy initialisers.
            _customAttributes =
                new Lazy<IEnumerable<Attribute>>(
                    () => type.GetCustomAttributes(false).Cast<Attribute>().ToList(),
                    LazyThreadSafetyMode.PublicationOnly);

            _defaultMember =
                new Lazy<string>(
                    () =>
                    {
                        // Look for default member.
                        DefaultMemberAttribute defaultMemberAttribute =
                            Enumerable.OfType<DefaultMemberAttribute>(this.CustomAttributes).SingleOrDefault();
                        return defaultMemberAttribute != null
                                   ? defaultMemberAttribute.MemberName
                                   : null;
                    }, LazyThreadSafetyMode.PublicationOnly);

            _signature
                = new Lazy<string>(
                    () =>
                    {
                        Type elementType = type;

                        while (elementType.HasElementType)
                            elementType = elementType.GetElementType();

                        if (elementType.IsNested)
                            return type.Name;

                        string sigToString = type.ToString();

                        if (elementType.IsPrimitive ||
                            elementType == typeof(void) ||
                            elementType == typeof(TypedReference))
                            sigToString = sigToString.Substring(7);

                        return sigToString;
                    }, LazyThreadSafetyMode.PublicationOnly);

            _simpleFullName
                = new Lazy<string>(
                    () => Reflection.SimpleTypeFullName(type.FullName ?? type.Name),
                    LazyThreadSafetyMode.PublicationOnly);

            _genericArguments = new Lazy<List<GenericArgument>>(
                () => Type.GetGenericArguments()
                          .Select((g, i) => new GenericArgument(GenericArgumentLocation.Type, i, g))
                          .ToList(),
                LazyThreadSafetyMode.PublicationOnly);

            _nonNullableType =
                    new Lazy<Type>(
                            () =>
                            (Type.IsGenericType && Type.GetGenericTypeDefinition() == typeof(Nullable<>))
                                    ? Type.GetGenericArguments()[0]
                                    : Type,
                            LazyThreadSafetyMode.PublicationOnly);

            _isConvertible = new Lazy<bool>(
                    () =>
                    {
                        Type t = _nonNullableType.Value;
                        if (t.IsEnum)
                            return true;
                        switch (Type.GetTypeCode(t))
                        {
                            case TypeCode.Boolean:
                            case TypeCode.Char:
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
                    },
                    LazyThreadSafetyMode.PublicationOnly);

            _interfaces = new Lazy<Dictionary<string, Type>>(() => Type.GetInterfaces().ToDictionary(t => t.FullName, t => t), LazyThreadSafetyMode.PublicationOnly);
        }

        /// <summary>
        /// Gets the extended type information for the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        [NotNull]
        public static ExtendedType Get([NotNull] Type type)
        {
            return _extendedTypes.GetOrAdd(type, t => new ExtendedType(t));
        }

        /// <summary>
        /// Loads all the members in one go.
        /// </summary>
        /// <remarks></remarks>
        private void LoadMembers()
        {
            // Grab spin lock
            bool taken = false;
            _loadLock.Enter(ref taken);

            if (!_loaded)
            {
                List<Constructor> constructors = null;
                List<Indexer> indexers = null;
                // Get all members in one go - this is significantly faster than getting individual calls later - at the cost of potentially
                // loading members that are not requested.
                foreach (MemberInfo memberInfo in this.Type.GetMembers(AllMembersBindingFlags))
                {
                    // Store fields
                    FieldInfo f = memberInfo as FieldInfo;
                    if (f != null)
                    {
                        _fields.Add(f.Name, new Field(this, f));
                        continue;
                    }

                    // Store properties/indexers
                    PropertyInfo p = memberInfo as PropertyInfo;
                    if (p != null)
                    {
                        // Seperate out indexers (which can not be disambiguated by name).
                        if (p.Name == DefaultMember)
                        {
                            if (indexers == null) indexers = new List<Indexer>();
                            indexers.Add(new Indexer(this, p));
                            continue;
                        }
                        _properties.Add(p.Name, new Property(this, p));
                        continue;
                    }

                    // Store methods
                    MethodInfo m = memberInfo as MethodInfo;
                    if (m != null)
                    {
                        // Check for cast methods
                        bool isCast;
                        bool isExplicit;
                        switch (m.Name)
                        {
                            case "op_Implicit":
                                isCast = true;
                                isExplicit = false;
                                break;
                            case "op_Explicit":
                                isCast = true;
                                isExplicit = true;
                                break;
                            default:
                                isCast = false;
                                isExplicit = false;
                                break;
                        }

                        Method method;
                        if (isCast)
                        {
                            // Create cast method and add to our cast dictionary.
                            CastMethod cm = new CastMethod(this, m, isExplicit);
                            if (cm.FromType == Type)
                            {
                                Contract.Assert(cm.ToType != Type);
                                _castsTo.Add(cm.ToType, cm);
                            }
                            else
                            {
                                Contract.Assert(cm.ToType == Type);
                                _castsFrom.Add(cm.FromType, cm);
                            }

                            // Use the cast method as a method
                            method = cm;
                        }
                        else
                        {
                            method = new Method(this, m);
                        }

                        List<Method> methods;
                        if (!_methods.TryGetValue(m.Name, out methods))
                        {
                            methods = new List<Method>();
                            _methods.Add(m.Name, methods);
                        }
                        Contract.Assert(methods != null);
                        methods.Add(method);
                        continue;
                    }

                    // Store constructors
                    ConstructorInfo c = memberInfo as ConstructorInfo;
                    if (c != null)
                    {
                        Constructor constructor = new Constructor(this, c);
                        // Check if we're the static constructor
                        if (c.IsStatic)
                        {
                            Contract.Assert(StaticConstructor == null);
                            StaticConstructor = constructor;
                            continue;
                        }

                        if (constructors == null) constructors = new List<Constructor>();
                        constructors.Add(constructor);
                        continue;
                    }

                    // Store events
                    EventInfo e = memberInfo as EventInfo;
                    if (e != null)
                    {
                        _events.Add(e.Name, new Event(this, e));
                        continue;
                    }

                    // Store types
                    Type t = memberInfo as Type;
                    if (t == null) return;
                }

                _constructors = constructors ?? Enumerable.Empty<Constructor>();
                _indexers = indexers ?? Enumerable.Empty<Indexer>();

                _loaded = true;
            }
            // Release spin lock.
            if (taken)
                _loadLock.Exit();
        }

        /// <summary>
        /// Whether this type implements a cast from the specified type.
        /// </summary>
        /// <param name="fromType">The type to cast from.</param>
        /// <param name="implicitOnly">if set to <see langword="true" /> only allows implicit casts.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool ImplementsCastFrom([NotNull] Type fromType, bool implicitOnly = true)
        {
            CastMethod cast = this.GetCastFromMethod(fromType);
            return cast != null && (!implicitOnly || !cast.IsExplicit);
        }

        /// <summary>
        /// Gets the user-defined method that implements a cast from the given type to this type (if any
        /// exist on this type).
        /// </summary>
        /// <param name="fromType">The type to cast from.</param>
        /// <returns>The method that implements the cast; otherwise <see langword="null"/>.</returns>
        /// <remarks></remarks>
        [CanBeNull]
        public CastMethod GetCastFromMethod([NotNull] Type fromType)
        {
            CastMethod cast;
            return _castsFrom.TryGetValue(fromType, out cast) ? cast : null;
        }

        /// <summary>
        /// Whether this type implements a cast to the specified type.
        /// </summary>
        /// <param name="toType">The type to cast to.</param>
        /// <param name="implicitOnly">if set to <see langword="true" /> only allows implicit casts.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool ImplementsCastTo([NotNull] Type toType, bool implicitOnly = true)
        {
            CastMethod cast = this.GetCastToMethod(toType);
            return cast != null && (!implicitOnly || !cast.IsExplicit);
        }

        /// <summary>
        /// Gets the user-defined method that implements a cast to the given type from this type (if any
        /// exist on this type).
        /// </summary>
        /// <param name="toType">The type to cast to.</param>
        /// <returns>The method that implements the cast; otherwise <see langword="null"/>.</returns>
        /// <remarks></remarks>
        [CanBeNull]
        public CastMethod GetCastToMethod([NotNull] Type toType)
        {
            CastMethod cast;
            return _castsTo.TryGetValue(toType, out cast) ? cast : null;
        }

        /// <summary>
        /// Checks to see if this type implements a particular interface.
        /// </summary>
        /// <param name="interfaceType">Type of the interface.</param>
        /// <returns><see langword="true"/> if <see cref="Type"/> implements the interface type; otherwise <see langword="false"/>.</returns>
        /// <remarks></remarks>
        public bool Implements([NotNull] Type interfaceType)
        {
            return _interfaces.Value.ContainsKey(interfaceType.FullName);
        }

        /// <summary>
        /// Gets the field.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The <see cref="Field"/> if found; otherwise <see langword="null"/>.</returns>
        /// <remarks></remarks>
        public Field GetField([NotNull] string name)
        {
            if (!_loaded) LoadMembers();
            Field field;
            return _fields.TryGetValue(name, out field) ? field : null;
        }

        /// <summary>
        /// Gets the <see cref="Field"/> matching the <see cref="FieldInfo"/> if found.
        /// </summary>
        /// <param name="fieldInfo">The field info.</param>
        /// <returns>The <see cref="Field"/> if found; otherwise <see langword="null"/>.</returns>
        /// <remarks></remarks>
        public Field GetField([NotNull] FieldInfo fieldInfo)
        {
            if (fieldInfo.DeclaringType != Type) return null;
            if (!_loaded) LoadMembers();
            return _fields.Values.FirstOrDefault(f => f.Info == fieldInfo);
        }

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The <see cref="Property"/> if found; otherwise <see langword="null"/>.</returns>
        /// <remarks></remarks>
        public Property GetProperty([NotNull] string name)
        {
            if (!_loaded) LoadMembers();
            Property property;
            return _properties.TryGetValue(name, out property) ? property : null;
        }

        /// <summary>
        /// Gets the <see cref="Property"/> matching the <see cref="PropertyInfo"/> if any.
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        /// <returns>The <see cref="Property"/> if found; otherwise <see langword="null"/>.</returns>
        /// <remarks></remarks>
        public Property GetProperty([NotNull] PropertyInfo propertyInfo)
        {
            if (propertyInfo.DeclaringType != Type) return null;
            if (!_loaded) LoadMembers();
            return _properties.Values.FirstOrDefault(p => p.Info == propertyInfo);
        }

        /// <summary>
        /// Gets the indexer.
        /// </summary>
        /// <param name="types">The types.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public Indexer GetIndexer([NotNull] params TypeSearch[] types)
        {
            bool[] castsRequired;
            return this.GetIndexer(out castsRequired, types);
        }

        /// <summary>
        /// Gets the indexer.
        /// </summary>
        /// <param name="types">The types.</param>
        /// <param name="castsRequired">Any array indicating which parameters require a cast (the last element is for the return type).</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public Indexer GetIndexer(out bool[] castsRequired, [NotNull] params TypeSearch[] types)
        {
            if (!_loaded) LoadMembers();
            return _indexers.BestMatch(0, true, true, out castsRequired, types) as Indexer;
        }

        /// <summary>
        /// Gets the <see cref="Indexer"/> matching the <see cref="PropertyInfo"/> (if any).
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public Indexer GetIndexer([NotNull] PropertyInfo propertyInfo)
        {
            if ((propertyInfo.DeclaringType != Type) ||
                (propertyInfo.Name != DefaultMember))
                return null;
            if (!_loaded) LoadMembers();
            return _indexers.FirstOrDefault(i => i.Info == propertyInfo);
        }

        /// <summary>
        /// Gets the methods.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The <see cref="Methods"/> if found; otherwise <see langword="null"/>.</returns>
        /// <remarks></remarks>
        public IEnumerable<Method> GetMethods([NotNull] string name)
        {
            if (!_loaded) LoadMembers();
            List<Method> methods;
            return _methods.TryGetValue(name, out methods) ? methods : null;
        }

        /// <summary>
        /// Gets the <see cref="Method"/> matching the <see cref="MethodInfo"/> (if any).
        /// </summary>
        /// <param name="methodInfo">The method info.</param>
        /// <returns>The <see cref="Method"/> if found; otherwise <see langword="null"/>.</returns>
        /// <remarks></remarks>
        public Method GetMethod([NotNull]MethodInfo methodInfo)
        {
            if (methodInfo.DeclaringType != Type)
                return null;
            if (!_loaded) LoadMembers();
            List<Method> methods;
            if (!_methods.TryGetValue(methodInfo.Name, out methods))
                return null;
            Contract.Assert(methods != null);
            return methods.FirstOrDefault(m => m.Info == methodInfo);
        }

        /// <summary>
        /// Gets the method.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="types">The parameter types and return type.</param>
        /// <returns>The <see cref="Method"/> if found; otherwise <see langword="null"/>.</returns>
        /// <remarks></remarks>
        public Method GetMethod([NotNull] string name, [NotNull] params TypeSearch[] types)
        {
            bool[] castsRequired;
            return GetMethod(name, 0, true, true, out castsRequired, types);
        }

        /// <summary>
        /// Gets the method.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="genericArguments">The number of generic arguments.</param>
        /// <param name="types">The parameter types and return type.</param>
        /// <returns>The <see cref="Method" /> if found; otherwise <see langword="null" />.</returns>
        /// <remarks></remarks>
        public Method GetMethod([NotNull] string name, int genericArguments, [NotNull] params TypeSearch[] types)
        {
            bool[] castsRequired;
            return GetMethod(name, genericArguments, true, true, out castsRequired, types);
        }

        /// <summary>
        /// Gets the method.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="genericArguments">The number of generic arguments.</param>
        /// <param name="allowClosure">if set to <see langword="true" /> will automatically close the signatures generic types if possible.</param>
        /// <param name="allowCasts">if set to <see langword="true" /> then types will match if they can be cast to the required type.</param>
        /// <param name="types">The parameter types and return type.</param>
        /// <returns>The <see cref="Method" /> if found; otherwise <see langword="null" />.</returns>
        /// <remarks></remarks>
        public Method GetMethod([NotNull] string name, int genericArguments, bool allowClosure, bool allowCasts, [NotNull] params TypeSearch[] types)
        {
            bool[] castsRequired;
            return GetMethod(name, genericArguments, allowClosure, allowCasts, out castsRequired, types);
        }

        /// <summary>
        /// Gets the method.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="genericArguments">The number of generic arguments.</param>
        /// <param name="allowClosure">if set to <see langword="true" /> will automatically close the signatures generic types if possible.</param>
        /// <param name="allowCasts">if set to <see langword="true" /> then types will match if they can be cast to the required type.</param>
        /// <param name="castsRequired">Any array indicating which parameters require a cast (the last element is for the return type).</param>
        /// <param name="types">The parameter types and return type.</param>
        /// <returns>The <see cref="Method" /> if found; otherwise <see langword="null" />.</returns>
        /// <remarks></remarks>
        public Method GetMethod([NotNull] string name, int genericArguments, bool allowClosure, bool allowCasts, out bool[] castsRequired, [NotNull] params TypeSearch[] types)
        {
            if (!_loaded) LoadMembers();
            List<Method> methods;
            if (!_methods.TryGetValue(name, out methods))
            {
                castsRequired = Reflection.EmptyBools;
                return null;
            }
            Contract.Assert(methods != null);
            return methods.BestMatch(genericArguments, allowClosure, allowCasts, out castsRequired, types) as Method;
        }
        
        /// <summary>
        /// Gets the constructor.
        /// </summary>
        /// <param name="types">The parameter types and return type (normally the same as <see cref="Type" />).</param>
        /// <returns>The <see cref="Constructor" /> if found; otherwise <see langword="null" />.</returns>
        /// <remarks></remarks>
        public Constructor GetConstructor([NotNull] params TypeSearch[] types)
        {
            bool[] castsRequired;
            return this.GetConstructor(out castsRequired, types);
        }

        /// <summary>
        /// Gets the constructor.
        /// </summary>
        /// <param name="castsRequired">Any array indicating which parameters require a cast (the last element is for the return type).</param>
        /// <param name="types">The parameter types and return type (normally the same as <see cref="Type"/>).</param>
        /// <returns>The <see cref="Constructor"/> if found; otherwise <see langword="null"/>.</returns>
        /// <remarks></remarks>
        public Constructor GetConstructor(out bool[] castsRequired, [NotNull] params TypeSearch[] types)
        {
            if (!_loaded) LoadMembers();
            return _constructors.BestMatch(0, true, true, out castsRequired, types) as Constructor;
        }

        /// <summary>
        /// Gets the <see cref="Constructor"/> matching the <see cref="ConstructorInfo"/> (if any).
        /// </summary>
        /// <param name="constructorInfo">The constructor info.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public Constructor GetConstructor([NotNull]ConstructorInfo constructorInfo)
        {
            if (constructorInfo.DeclaringType != Type)
                return null;
            if (!_loaded) LoadMembers();
            return _constructors == null ? null : _constructors.FirstOrDefault(c => c.Info == constructorInfo);
        }

        /// <summary>
        /// Gets the event.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The <see cref="Event"/> if found; otherwise <see langword="null"/>.</returns>
        /// <remarks></remarks>
        public Event GetEvent([NotNull] string name)
        {
            if (!_loaded) LoadMembers();
            Event @event;
            return _events.TryGetValue(name, out @event) ? @event : null;
        }

        /// <summary>
        /// Gets the <see cref="Event"/> matching the <see cref="EventInfo"/> if any.
        /// </summary>
        /// <param name="eventInfo">The event info.</param>
        /// <returns>The <see cref="Event"/> if found; otherwise <see langword="null"/>.</returns>
        /// <remarks></remarks>
        public Event GetEvent([NotNull] EventInfo eventInfo)
        {
            if (eventInfo.DeclaringType != Type) return null;
            if (!_loaded) LoadMembers();
            return _events.Values.FirstOrDefault(e => e.Info == eventInfo);
        }

        /// <summary>
        /// Caches closed types.
        /// </summary>
        [NotNull]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Lazy<ConcurrentDictionary<string, ExtendedType>> _closedTypes =
            new Lazy<ConcurrentDictionary<string, ExtendedType>>(
                () => new ConcurrentDictionary<string, ExtendedType>(), LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// Closes the type if it is generic and has generic parameters, or creates equivalent type.
        /// </summary>
        /// <param name="genericTypes">The generic types (null to use existing type from closed type).</param>
        /// <returns>The <see cref="ExtendedType"/> without open generic parameter if the supplied generic types are able to close the type; otherwise <see langword="null"/>.</returns>
        /// <remarks></remarks>
        public ExtendedType CloseType([NotNull] params Type[] genericTypes)
        {
            int length = genericTypes.Length;
            // Check length matches.
            if (length != _genericArguments.Value.Count)
                return null;

            // Substitute missing types with concrete ones.
            Type[] gta = new Type[length];
            for (int i = 0; i < length; i++)
            {
                Type gt = genericTypes[i];

                // Must supply concrete types.
                if (gt == null)
                {
                    // See if we have a concrete type for this index.
                    Type et = _genericArguments.Value[i].Type;
                    if ((et == null) ||
                        (et.IsGenericType))
                        return null;
                    gt = et;
                }
                else if (gt.IsGenericType)
                    return null;

                gta[i] = gt;
            }

            // Make a closed type using the first types in the array.
            // TODO There is no way of avoiding a catch (unfortunately) without
            // implementing a constraint validation algorithm - as the .NET one is
            // not exposed - this could be done using Type.GenericParameterAttributes
            // followed by Type.GetGenericParameterConstraints.
            string key = String.Join("|", gta.Select(t => t.FullName));
            return _closedTypes.Value.GetOrAdd(
                key,
                k =>
                {
                    try
                    {
                        return Get(Type.MakeGenericType(gta));
                    }
                    catch (ArgumentException)
                    {
                        return null;
                    }
                });
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="ExtendedType"/> to <see cref="System.Type"/>.
        /// </summary>
        /// <param name="extendedType">Type of the extended.</param>
        /// <returns>The result of the conversion.</returns>
        /// <remarks></remarks>
        public static implicit operator Type(ExtendedType extendedType)
        {
            return extendedType != null ? extendedType.Type : null;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Type"/> to <see cref="ExtendedType"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The result of the conversion.</returns>
        /// <remarks></remarks>
        public static implicit operator ExtendedType(Type type)
        {
            return type == null ? null : Get(type);
        }

        /// <summary>
        /// Creates a cache for casts on demand.
        /// </summary>
        private readonly Lazy<ConcurrentDictionary<Type, bool>> _convertToCache =
            new Lazy<ConcurrentDictionary<Type, bool>>(() => new ConcurrentDictionary<Type, bool>(), LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// Determines whether this instance can be converted to the specified type.  Unlike the built in cast system,
        /// this returns true if a cast is possible AND if IConvertible can be used.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><see langword="true" /> if this instance can be cast to the specified type; otherwise, <see langword="false" />.</returns>
        /// <remarks></remarks>
        public bool CanConvertTo(Type type)
        {
            if (Type == type)
                return true;

            if ((type == null) ||
                (type.IsGenericTypeDefinition) ||
                (type.ContainsGenericParameters))
                return false;

            return this._convertToCache.Value.GetOrAdd(
                type,
                t =>
                {
                    // Get extended type information for destination type.
                    ExtendedType dest = type;

                    // First we check to see if a cast is possible
                    if ((NonNullableType == dest.NonNullableType) || (NonNullableType.IsEquivalentTo(NonNullableType)) ||
                        (dest.NonNullableType != typeof(bool) && this.IsConvertible && dest.IsConvertible) ||
                        (this.ImplementsCastTo(dest)) ||
                        (dest.ImplementsCastFrom(this)) ||
                        (this.Implements(typeof(IConvertible)) && _iConvertibleMethods.ContainsKey(type)))
                        return true;

                    // TODO SUPPORT TYPE CONVERTERS
                    return false;
                });
        }

        /// <summary>
        /// Converts the specified expression to the output type.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="outputExpression">The output expression.</param>
        /// <returns>Returns <see langword="true" /> if the conversion succeeded; otherwise returns <see langword="false" />.</returns>
        /// <remarks>This version is more powerful than the Expression.Convert CLR method in that it supports
        /// ToString() conversion, IConvertible and TypeConverters. It also prevents exceptions being thrown.</remarks>
        [UsedImplicitly]
        public bool TryConvert([NotNull]Expression expression, [NotNull]out Expression outputExpression)
        {
            Type expressionType = expression.Type;
            // If the types are the same we don't need to convert.
            if (expression.Type == Type)
            {
                outputExpression = expression;
                return true;
            }

            outputExpression = expression;

            // Check the cast is possible
            ExtendedType et = expressionType;
            if (!et.CanConvertTo(Type))
                return false;

            try
            {
                // Try creating conversion.
                outputExpression = Expression.Convert(expression, Type);
                return true;
            }
            catch (InvalidOperationException)
            {
                // Ignore failures due to lack of coercion operator.
            }

            // Look for IConvertible method
            string method;
            if (et.Implements(typeof(IConvertible)))
            {
                string methodName;
                IEnumerable<Method> methods;
                if ((_iConvertibleMethods.TryGetValue(Type, out methodName)) &&
                    ((methods=et.GetMethods(methodName)) != null))
                {
                    Method cm =
                            methods.FirstOrDefault(
                                    m =>
                                    m.ParameterTypes.Count() == 1 && m.ParameterTypes.First() == typeof(IFormatProvider));
                    if (cm != null)
                    {
                        // Call the IConvertible method on the object, passing in CultureInfo.CurrentCulture as the parameter.
                        outputExpression = Expression.Call(
                                expression, cm.Info, Reflection.CurrentCultureExpression);
                        return true;
                    }
                }
            }

            /*
             * TypeConverter support
             * TODO MORE OPTIMISATION REQUIRED
             */

            // Look for TypeConverter on output type.
            bool useTo = false;
            TypeConverterAttribute typeConverterAttribute = Type
                .GetCustomAttributes(typeof(TypeConverterAttribute), false)
                .OfType<TypeConverterAttribute>()
                .FirstOrDefault();

            if ((typeConverterAttribute == null) ||
                (string.IsNullOrWhiteSpace(typeConverterAttribute.ConverterTypeName)))
            {
                // Look for TypeConverter on expression type.
                useTo = true;
                typeConverterAttribute = expression.Type
                    .GetCustomAttributes(typeof(TypeConverterAttribute), false)
                    .OfType<TypeConverterAttribute>()
                    .FirstOrDefault();
            }

            if ((typeConverterAttribute != null) &&
                (!string.IsNullOrWhiteSpace(typeConverterAttribute.ConverterTypeName)))
            {
                try
                {
                    // Try to get the type for the typeconverter
                    Type typeConverterType = Type.GetType(typeConverterAttribute.ConverterTypeName);

                    if (typeConverterType != null)
                    {
                        // Try to create an instance of the typeconverter without parameters
                        TypeConverter converter = Activator.CreateInstance(typeConverterType) as TypeConverter;
                        if ((converter != null) &&
                            (useTo ? converter.CanConvertTo(Type) : converter.CanConvertFrom(expression.Type)))
                        {
                            // We have a converter that supports the necessary conversion
                            MethodInfo mi = useTo
                                                ? typeConverterType.GetMethod(
                                                    "ConvertTo",
                                                    BindingFlags.Instance | BindingFlags.Public |
                                                    BindingFlags.FlattenHierarchy,
                                                    null,
                                                    new[] { typeof(object), typeof(Type) },
                                                    null)
                                                : typeConverterType.GetMethod(
                                                    "ConvertFrom",
                                                    BindingFlags.Instance | BindingFlags.Public |
                                                    BindingFlags.FlattenHierarchy,
                                                    null,
                                                    new[] { typeof(object) },
                                                    null);
                            if (mi != null)
                            {
                                // The convert methods accepts the value as an object parameters, so we may need a cast.
                                if (expression.Type != typeof(object))
                                    expression = Expression.Convert(expression, typeof(object));

                                // Create an expression which creates a new instance of the type converter and passes in
                                // the existing expression as the first parameter to ConvertTo or ConvertFrom.
                                outputExpression = useTo
                                                       ? Expression.Call(Expression.New(typeConverterType), mi,
                                                                         expression,
                                                                         Expression.Constant(Type, typeof(Type)))
                                                       : Expression.Call(Expression.New(typeConverterType), mi,
                                                                         expression);

                                return true;
                            }
                        }
                    }
                }
                catch
                {
                }
            }

            // Finally, if we want to output to string, call ToString() method.
            if (Type == typeof(string))
            {
                outputExpression = Expression.Call(expression, Reflection.ToStringMethodInfo);
                return true;
            }

            outputExpression = expression;
            return false;
        }

        /// <summary> 
        /// Checks to see if a type descends from another type. 
        /// </summary> 
        /// <param name="baseType">Type of the base.</param> 
        /// <returns></returns> 
        /// <remarks></remarks> 
        public bool DescendsFrom([NotNull]Type baseType)
        {
            Type sourceType = this.Type;
            do
            {
                if (sourceType == baseType)
                    return true;
                sourceType = sourceType.BaseType;
            } while (sourceType != null);
            return false;
        } 
    }
}
