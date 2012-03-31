using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
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
        ///   Holds all properties.
        /// </summary>
        [NotNull]
        private readonly Dictionary<string, Property> _properties = new Dictionary<string, Property>();

        /// <summary>
        ///   Gets the properties.
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
        private readonly Dictionary<string, Methods> _methods = new Dictionary<string, Methods>();

        /// <summary>
        ///   Gets the methods.
        /// </summary>
        [NotNull]
        public IEnumerable<Methods> Methods
        {
            get
            {
                if (!_loaded) LoadMembers();
                return _methods.Values;
            }
        }

        /// <summary>
        /// Holds all constructors.
        /// </summary>
        [NotNull]
        private readonly Dictionary<string, Constructors> _constructors = new Dictionary<string, Constructors>();

        /// <summary>
        ///   Gets the constructors.
        /// </summary>
        [NotNull]
        public IEnumerable<Constructors> Constructors
        {
            get
            {
                if (!_loaded) LoadMembers();
                return _constructors.Values;
            }
        }


        /// <summary>
        /// Calculates custom attributes on demand.
        /// </summary>
        [NotNull]
        private readonly Lazy<IEnumerable<Attribute>> _customAttributes;

        /// <summary>
        ///   All the customer attributes on the type.
        /// </summary>
        [NotNull]
        public IEnumerable<Attribute> CustomerAttributes { get { return _customAttributes.Value ?? Enumerable.Empty<Attribute>(); } }

        /// <summary>
        /// Calculates default member on demand.
        /// </summary>
        [NotNull]
        private readonly Lazy<string> _defaultMember;

        /// <summary>
        ///   If this type has a default member (indexer), indicates its name.
        /// </summary>
        [CanBeNull]
        public string DefaultMember { get { return _defaultMember.Value; } }

        /// <summary>
        /// Creates a signature on demand.
        /// </summary>
        [NotNull]
        private readonly Lazy<string> _signature;

        /// <summary>
        /// Gets the signature of the type.
        /// </summary>
        /// <remarks>This is modelled after the Type.SigToString internal method.</remarks>
        [NotNull]
        public string Signature { get { return _signature.Value ?? this.Type.FullName ?? this.Type.Name; } }

        /// <summary>
        /// Creates a simple full name on demand.
        /// </summary>
        [NotNull]
        private readonly Lazy<string> _simpleFullName;

        /// <summary>
        /// Gets the simple full name for the type.
        /// </summary>
        /// <remarks></remarks>
        public string SimpleFullName { get { return _simpleFullName.Value ?? this.Type.FullName ?? this.Type.Name; } }

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
        public ExtendedType([NotNull]Type type)
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
        }

        /// <summary>
        /// Gets the extended type information for the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        [NotNull]
        public static ExtendedType Get([NotNull]Type type)
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
                        if (p.Name.Equals(DefaultMember))
                        {
                            var indexTypes = p.GetIndexParameters();
                            // TODO Add indexer support.
                        }
                        else
                            _properties.Add(p.Name, new Property(this, p));
                        continue;
                    }

                    // Store methods
                    MethodInfo m = memberInfo as MethodInfo;
                    if (m != null)
                    {
                        Methods methods;
                        if (!_methods.TryGetValue(m.Name, out methods))
                        {
                            methods = new Methods(this, m);
                            _methods.Add(m.Name, methods);
                        }
                        else
                            methods.Add(m);
                        continue;
                    }

                    // Store constructors
                    ConstructorInfo c = memberInfo as ConstructorInfo;
                    if (c != null)
                    {
                        Constructors constructors;
                        if (!_constructors.TryGetValue(c.Name, out constructors))
                        {
                            constructors = new Constructors(this, c);
                            _constructors.Add(c.Name, constructors);
                        }
                        else
                            constructors.Add(c);
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
        /// <returns>The <see cref="FieldInfo"/> if found; otherwise <see langword="null"/>.</returns>
        /// <remarks></remarks>
        public Field GetField([NotNull]string name)
        {
            if (!_loaded) LoadMembers();
            Field field;
            return _fields.TryGetValue(name, out field) ? field : null;
        }

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The <see cref="Property"/> if found; otherwise <see langword="null"/>.</returns>
        /// <remarks></remarks>
        public Property GetProperty([NotNull]string name)
        {
            if (!_loaded) LoadMembers();
            Property property;
            return _properties.TryGetValue(name, out property) ? property : null;
        }

        /// <summary>
        /// Gets the methods.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The <see cref="Methods"/> if found; otherwise <see langword="null"/>.</returns>
        /// <remarks></remarks>
        public Methods GetMethods([NotNull]string name)
        {
            if (!_loaded) LoadMembers();
            Methods methods;
            return _methods.TryGetValue(name, out methods) ? methods : null;
        }

        /// <summary>
        /// Gets the method.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="types">The parameter types and return type.</param>
        /// <returns>The <see cref="Method"/> if found; otherwise <see langword="null"/>.</returns>
        /// <remarks></remarks>
        public Method GetMethod([NotNull]string name, [NotNull]params Type[] types)
        {
            return GetMethod(name, 0, types);
        }

        /// <summary>
        /// Gets the method.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="genericArguments">The number of generic arguments.</param>
        /// <param name="types">The parameter types and return type.</param>
        /// <returns>The <see cref="Method"/> if found; otherwise <see langword="null"/>.</returns>
        /// <remarks></remarks>
        public Method GetMethod([NotNull]string name, int genericArguments, [NotNull]params Type[] types)
        {
            if (!_loaded) LoadMembers();
            Methods methods;
            if (!_methods.TryGetValue(name, out methods))
                return null;
            Debug.Assert(methods != null);
            return methods.GetOverload(genericArguments, types);
        }

        /// <summary>
        /// Gets the constructors.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The <see cref="Constructors"/> if found; otherwise <see langword="null"/>.</returns>
        /// <remarks></remarks>
        public Constructors GetConstructors([NotNull]string name)
        {
            if (!_loaded) LoadMembers();
            Constructors constructors;
            return _constructors.TryGetValue(name, out constructors) ? constructors : null;
        }

        /// <summary>
        /// Gets the constructor.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="types">The parameter types and return type.</param>
        /// <returns>The <see cref="Constructor"/> if found; otherwise <see langword="null"/>.</returns>
        /// <remarks></remarks>
        public Constructor GetConstructor([NotNull]string name, [NotNull]params Type[] types)
        {
            if (!_loaded) LoadMembers();
            Constructors constructors;
            if (!_constructors.TryGetValue(name, out constructors))
                return null;
            Debug.Assert(constructors != null);
            Constructor constructor;
            return constructors.GetOverload(types);
        }


        /// <summary>
        /// Gets the event.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The <see cref="Event"/> if found; otherwise <see langword="null"/>.</returns>
        /// <remarks></remarks>
        public Event GetEvent([NotNull]string name)
        {
            if (!_loaded) LoadMembers();
            Event @event;
            return _events.TryGetValue(name, out @event) ? @event : null;
        }

        /// <summary>
        /// Closes the type if it is generic and has generic parameters.
        /// </summary>
        /// <param name="genericTypes">The generic types.</param>
        /// <returns>The <see cref="ExtendedType"/> without open generic parameter if the supplied generic types are able to close the type; otherwise <see langword="null"/>.</returns>
        /// <remarks></remarks>
        public ExtendedType CloseType([NotNull]params Type[] genericTypes)
        {
            if (!Type.ContainsGenericParameters)
                return genericTypes.Length < 1 ? this : null;

            int args = Type.GetGenericArguments().Length;

            if (genericTypes.Length != args)
                return null;
            
            // Make a closed type using the first types in the array.
            Type closedType = Type.MakeGenericType(genericTypes);

            return Get(closedType);
        }


        /// <summary>
        /// Closes the type if it is generic and has generic parameters.
        /// </summary>
        /// <param name="genericTypes">The generic types.</param>
        /// <returns>The <see cref="ExtendedType"/> without open generic parameter if the supplied generic types are sufficient; otherwise <see langword="null"/>.</returns>
        /// <remarks>
        /// On returning, the <see paramref="genericTypes"/> parameter contains a pointer to an array containing any unused types.
        /// </remarks>
        public ExtendedType CloseType([NotNull]ref Type[] genericTypes)
        {
            if (!Type.ContainsGenericParameters)
                return this;

            int args = Type.GetGenericArguments().Length;

            Debug.Assert(genericTypes != null);
            if (genericTypes.Length < args)
                return null;

            // Split the generic types array.
            Type[][] split = genericTypes.Split(args);
            Debug.Assert(
                split.Length > 0 &&
                split.Length < 3);

            // Change generic types to the second half of the split
            genericTypes = split.Length > 1 ? split[1] : new Type[0];

            // Make a closed type using the first types in the array.
            Type closedType = Type.MakeGenericType(split[0]);

            return Get(closedType);
        }
    }

    /// <summary>
    /// Generic convenience type for accessing extended type information.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <remarks></remarks>
    public static class ExtendedType<T>
    {
        /// <summary>
        /// The underlying <see cref="ExtendedType"/> object.
        /// </summary>
        [NotNull]
        public static readonly ExtendedType ExtendedTypeObject;

        /// <summary>
        /// Loads the underlying <see cref="ExtendedTypeObject"/>.
        /// </summary>
        static ExtendedType()
        {
            ExtendedTypeObject = ExtendedType.Get(typeof(T));
        }

        /// <summary>
        /// Gets the underlying type.
        /// </summary>
        /// <remarks></remarks>
        public static Type Type { get { return typeof(T); } }

        /// <summary>
        ///   Gets the fields.
        /// </summary>
        public static IEnumerable<Field> Fields
        {
            get { return ExtendedTypeObject.Fields; }
        }

        /// <summary>
        ///   Gets the properties.
        /// </summary>
        [NotNull]
        public static IEnumerable<Property> Properties
        {
            get { return ExtendedTypeObject.Properties; }
        }

        /// <summary>
        ///   Gets the events.
        /// </summary>
        [NotNull]
        public static IEnumerable<Event> Events
        {
            get { return ExtendedTypeObject.Events; }
        }

        /// <summary>
        ///   Gets the methods.
        /// </summary>
        [NotNull]
        public static IEnumerable<Methods> Methods
        {
            get { return ExtendedTypeObject.Methods; }
        }

        /// <summary>
        ///   Gets the constructors.
        /// </summary>
        [NotNull]
        public static IEnumerable<Constructors> Constructors
        {
            get { return ExtendedTypeObject.Constructors; }
        }

        /// <summary>
        ///   All the customer attributes on the type.
        /// </summary>
        [NotNull]
        public static IEnumerable<Attribute> CustomerAttributes
        {
            get { return ExtendedTypeObject.CustomerAttributes; }
        }

        /// <summary>
        ///   If this type has a default member (indexer), indicates its name.
        /// </summary>
        [CanBeNull]
        public static string DefaultMember { get { return ExtendedTypeObject.DefaultMember; } }

        /// <summary>
        /// Gets the signature of the type.
        /// </summary>
        /// <remarks>This is modelled after the Type.SigToString internal method.</remarks>
        [NotNull]
        public static string Signature { get { return ExtendedTypeObject.Signature; } }

        /// <summary>
        /// Gets the simple full name for the type.
        /// </summary>
        /// <remarks></remarks>
        public static string SimpleFullName { get { return ExtendedTypeObject.SimpleFullName; } }

        /// <summary>
        /// Gets the field.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The <see cref="Field"/> if found; otherwise <see langword="null"/>.</returns>
        /// <remarks></remarks>
        public static Field GetField([NotNull]string name)
        {
            return ExtendedTypeObject.GetField(name);
        }

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The <see cref="Property"/> if found; otherwise <see langword="null"/>.</returns>
        /// <remarks></remarks>
        public static Property GetProperty([NotNull]string name)
        {
            return ExtendedTypeObject.GetProperty(name);
        }

        /// <summary>
        /// Gets the methods.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The <see cref="Methods"/> if found; otherwise <see langword="null"/>.</returns>
        /// <remarks></remarks>
        public static Methods GetMethods([NotNull]string name)
        {
            return ExtendedTypeObject.GetMethods(name);
        }


        /// <summary>
        /// Gets the method.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="types">The parameter types and return type.</param>
        /// <returns>The <see cref="Method"/> if found; otherwise <see langword="null"/>.</returns>
        /// <remarks></remarks>
        public static Method GetMethod([NotNull]string name, [NotNull]params Type[] types)
        {
            return ExtendedTypeObject.GetMethod(name, types);
        }

        /// <summary>
        /// Gets the method.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="genericArguments">The number of generic arguments.</param>
        /// <param name="types">The parameter types and return type.</param>
        /// <returns>The <see cref="Method"/> if found; otherwise <see langword="null"/>.</returns>
        /// <remarks></remarks>
        public static Method GetMethod([NotNull]string name, int genericArguments, [NotNull]params Type[] types)
        {
            return ExtendedTypeObject.GetMethod(name, genericArguments, types);
        }

        /// <summary>
        /// Gets the constructors.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The <see cref="Constructors"/> if found; otherwise <see langword="null"/>.</returns>
        /// <remarks></remarks>
        public static Constructors GetConstructors([NotNull]string name)
        {
            return ExtendedTypeObject.GetConstructors(name);
        }

        /// <summary>
        /// Gets the constructor.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="types">The parameter types and return type.</param>
        /// <returns>The <see cref="Constructor"/> if found; otherwise <see langword="null"/>.</returns>
        /// <remarks></remarks>
        public static Constructor GetConstructor([NotNull]string name, params Type[] types)
        {
            return ExtendedTypeObject.GetConstructor(name, types);
        }

        /// <summary>
        /// Gets the event.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The <see cref="Event"/> if found; otherwise <see langword="null"/>.</returns>
        /// <remarks></remarks>
        public static Event GetEvent([NotNull]string name)
        {
            return ExtendedTypeObject.GetEvent(name);
        }
    }
}
