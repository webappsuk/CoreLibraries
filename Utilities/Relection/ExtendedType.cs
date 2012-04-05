using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Relection
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
        public IEnumerable<Attribute> CustomerAttributes
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
                            Enumerable.OfType<DefaultMemberAttribute>(this.CustomerAttributes).SingleOrDefault();
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
                        Method method = new Method(this, m);
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
            if (!_loaded) LoadMembers();
            return _indexers.BestMatch(types) as Indexer;
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
        /// Gets the method.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="types">The parameter types and return type.</param>
        /// <returns>The <see cref="Method"/> if found; otherwise <see langword="null"/>.</returns>
        /// <remarks></remarks>
        public Method GetMethod([NotNull] string name, [NotNull] params TypeSearch[] types)
        {
            return GetMethod(name, 0, types);
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
        /// <param name="genericArguments">The number of generic arguments.</param>
        /// <param name="types">The parameter types and return type.</param>
        /// <returns>The <see cref="Method"/> if found; otherwise <see langword="null"/>.</returns>
        /// <remarks></remarks>
        public Method GetMethod([NotNull] string name, int genericArguments, [NotNull] params TypeSearch[] types)
        {
            if (!_loaded) LoadMembers();
            List<Method> methods;
            if (!_methods.TryGetValue(name, out methods))
                return null;
            Contract.Assert(methods != null);
            return methods.BestMatch(genericArguments, types) as Method;
        }

        /// <summary>
        /// Gets the constructor.
        /// </summary>
        /// <param name="types">The parameter types and return type, and return type (i.e. the constructor's type).</param>
        /// <returns>The <see cref="Constructor"/> if found; otherwise <see langword="null"/>.</returns>
        /// <remarks></remarks>
        public Constructor GetConstructor([NotNull] params TypeSearch[] types)
        {
            if (!_loaded) LoadMembers();
            return _constructors.BestMatch(types) as Constructor;
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
        /// Performs an implicit conversion from <see cref="WebApplications.Utilities.Relection.ExtendedType"/> to <see cref="System.Type"/>.
        /// </summary>
        /// <param name="extendedType">Type of the extended.</param>
        /// <returns>The result of the conversion.</returns>
        /// <remarks></remarks>
        public static implicit operator Type(ExtendedType extendedType)
        {
            return extendedType != null ? extendedType.Type : null;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Type"/> to <see cref="WebApplications.Utilities.Relection.ExtendedType"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The result of the conversion.</returns>
        /// <remarks></remarks>
        public static implicit operator ExtendedType(Type type)
        {
            return type == null ? null : Get(type);
        }
    }
}
