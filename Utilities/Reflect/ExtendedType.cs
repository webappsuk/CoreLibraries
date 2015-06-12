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
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Reflect
{
    /// <summary>
    /// Holds extended information (including reflection info) for a type.
    /// </summary>
    [DebuggerDisplay("{Type} [Extended]")]
    public class ExtendedType
    {
        /// <summary>
        /// Binding flags for returning all fields/properties from a type.
        /// </summary>
        [UsedImplicitly]
        public const BindingFlags AllMembersBindingFlags =
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static |
            BindingFlags.DeclaredOnly;

        /// <summary>
        /// Holds all known extended types.
        /// </summary>
        [NotNull]
        private static readonly ConcurrentDictionary<Type, ExtendedType> _extendedTypes =
            new ConcurrentDictionary<Type, ExtendedType>();

        /// <summary>
        /// The standard conversion methods implemented by <see cref="System.IConvertible"/>.
        /// </summary>
        /// <remarks>
        /// Does not include:
        ///  <list type="bullet">
        ///    <item><description><see cref="System.IConvertible.ToType">ToType</see> - It isn't specific.</description></item>
        ///    <item><description><see cref="System.IConvertible.GetTypeCode">GetTypeCode</see> - Isn't actually a conversion method.</description></item>
        ///  </list>
        /// </remarks>
        [NotNull]
        private static readonly Dictionary<Type, string> _iConvertibleMethods =
            new Dictionary<Type, string>
            {
                { typeof(bool), "ToBoolean" },
                { typeof(char), "ToChar" },
                { typeof(sbyte), "ToSByte" },
                { typeof(byte), "ToByte" },
                { typeof(short), "ToInt16" },
                { typeof(ushort), "ToUInt16" },
                { typeof(int), "ToInt32" },
                { typeof(uint), "ToUInt32" },
                { typeof(long), "ToInt64" },
                { typeof(ulong), "ToUInt64" },
                { typeof(float), "ToSingle" },
                { typeof(double), "ToDouble" },
                { typeof(decimal), "ToDecimal" },
                { typeof(DateTime), "ToDateTime" },
                { typeof(string), "ToString" }
            };

        /// <summary>
        /// The underlying type.
        /// </summary>
        [NotNull]
        public readonly Type Type;

        /// <summary>
        /// Creates a cache for casts on demand.
        /// </summary>
        [NotNull]
        private readonly Lazy<ConcurrentDictionary<Type, bool>> _convertToCache =
            new Lazy<ConcurrentDictionary<Type, bool>>(
                () => new ConcurrentDictionary<Type, bool>(),
                LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// Calculates custom attributes on demand.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [NotNull]
        private readonly Lazy<IEnumerable<Attribute>>
            _customAttributes;

        /// <summary>
        /// Calculates default member on demand.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [NotNull]
        private readonly Lazy<string> _defaultMember;

        /// <summary>
        /// Holds all events.
        /// </summary>
        [NotNull]
        private readonly Dictionary<string, Event> _events = new Dictionary<string, Event>();

        /// <summary>
        /// Holds all fields.
        /// </summary>
        [NotNull]
        private readonly Dictionary<string, Field> _fields = new Dictionary<string, Field>();

        /// <summary>
        /// Creates array of generic arguments on demand.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [NotNull]
        private readonly Lazy<List<GenericArgument>>
            _genericArguments;

        [NotNull]
        private readonly Lazy<Dictionary<string, Type>> _interfaces;

        [NotNull]
        private readonly Lazy<bool> _isConvertible;

        /// <summary>
        /// Holds all methods.
        /// </summary>
        [NotNull]
        private readonly Dictionary<string, List<Method>> _methods = new Dictionary<string, List<Method>>();

        [NotNull]
        private readonly Lazy<Type> _nonNullableType;

        /// <summary>
        /// Holds all properties.
        /// </summary>
        [NotNull]
        private readonly Dictionary<string, Property> _properties = new Dictionary<string, Property>();

        /// <summary>
        /// Creates a signature on demand.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [NotNull]
        private readonly Lazy<string> _signature;

        /// <summary>
        /// Creates a simple full name on demand.
        /// </summary>
        [NotNull]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Lazy<string> _simpleFullName;

        /// <summary>
        /// Holds user defined methods that cast to this type from another type.
        /// </summary>
        [NotNull]
        private readonly Dictionary<Type, CastMethod> _castsFrom = new Dictionary<Type, CastMethod>();

        /// <summary>
        /// Holds user defined methods that cast from this type to another type.
        /// </summary>
        [NotNull]
        private readonly Dictionary<Type, CastMethod> _castsTo = new Dictionary<Type, CastMethod>();

        /// <summary>
        /// Caches closed types.
        /// </summary>
        [NotNull]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Lazy<ConcurrentDictionary<string, ExtendedType>> _closedTypes =
            new Lazy<ConcurrentDictionary<string, ExtendedType>>(
                () => new ConcurrentDictionary<string, ExtendedType>(),
                LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// Holds all constructors.
        /// </summary>
        [ItemNotNull]
        private IEnumerable<Constructor> _constructors;

        /// <summary>
        /// Holds all indexers.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        private IEnumerable<Indexer> _indexers = Enumerable.Empty<Indexer>();

        /// <summary>
        /// Spinlock for locking during member load.
        /// </summary>
        private SpinLock _loadLock = new SpinLock();

        /// <summary>
        /// Indicates whether members have been loaded.
        /// </summary>
        private bool _loaded;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedType"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public ExtendedType([NotNull] Type type)
        {
            Type = type;

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
                            CustomAttributes.OfType<DefaultMemberAttribute>().SingleOrDefault();
                        return defaultMemberAttribute != null
                            ? defaultMemberAttribute.MemberName
                            : null;
                    },
                    LazyThreadSafetyMode.PublicationOnly);

            _signature
                = new Lazy<string>(
                    () =>
                    {
                        Type elementType = type;

                        while (elementType.HasElementType)
                        {
                            elementType = elementType.GetElementType();
                            Debug.Assert(elementType != null);
                        }

                        if (elementType.IsNested)
                            return type.Name;

                        string sigToString = type.ToString();

                        if (elementType.IsPrimitive ||
                            elementType == typeof(void) ||
                            elementType == typeof(TypedReference))
                            sigToString = sigToString.Substring(7);

                        return sigToString;
                    },
                    LazyThreadSafetyMode.PublicationOnly);

            _simpleFullName
                = new Lazy<string>(
                    () => type.SimplifiedTypeFullName(),
                    LazyThreadSafetyMode.PublicationOnly);

            _genericArguments = new Lazy<List<GenericArgument>>(
                () => Type.GetGenericArguments()
                    // ReSharper disable once AssignNullToNotNullAttribute
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
                    Debug.Assert(t != null);

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

            _interfaces =
                new Lazy<Dictionary<string, Type>>(
                    // ReSharper disable once PossibleNullReferenceException
                    () => Type.GetInterfaces().ToDictionary(t => t.FullName ?? t.Name, t => t),
                    LazyThreadSafetyMode.PublicationOnly);
        }

        /// <summary>
        /// Gets the <see cref="ExtendedType"/> for the Base Type.
        /// </summary>
        /// <value>The type of the base.</value>
        [PublicAPI]
        public ExtendedType BaseType
        {
            get
            {
                return Type.BaseType == null
                    ? null
                    : Get(Type.BaseType);
            }
        }

        /// <summary>
        /// Gets the fields.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public IEnumerable<Field> Fields
        {
            get
            {
                if (!_loaded) LoadMembers();
                return _fields.Values;
            }
        }

        /// <summary>
        /// Gets all the fields, including from base types.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public IEnumerable<Field> AllFields
        {
            get
            {
                if (!_loaded) LoadMembers();
                ExtendedType type = this;
                while (type != null)
                {
                    foreach (Field field in type.Fields)
                        yield return field;
                    type = type.BaseType;
                }
            }
        }

        /// <summary>
        /// Gets the indexers.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public IEnumerable<Indexer> Indexers
        {
            get
            {
                if (_loaded) LoadMembers();
                return _indexers;
            }
        }

        /// <summary>
        /// Gets all the indexers, including from base types.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public IEnumerable<Indexer> AllIndexers
        {
            get
            {
                if (_loaded) LoadMembers();
                ExtendedType type = this;
                while (type != null)
                {
                    foreach (Indexer indexer in type.Indexers)
                        yield return indexer;
                    type = type.BaseType;
                }
            }
        }

        /// <summary>
        /// Gets the properties (doesn't include <see cref="Indexers"/>).
        /// </summary>
        [NotNull]
        [PublicAPI]
        public IEnumerable<Property> Properties
        {
            get
            {
                if (!_loaded) LoadMembers();
                return _properties.Values;
            }
        }

        /// <summary>
        /// Gets all the properties, including from base types (doesn't include <see cref="Indexers"/>).
        /// </summary>
        [NotNull]
        [PublicAPI]
        public IEnumerable<Property> AllProperties
        {
            get
            {
                if (!_loaded) LoadMembers();
                ExtendedType type = this;
                while (type != null)
                {
                    foreach (Property property in type.Properties)
                        yield return property;
                    type = type.BaseType;
                }
            }
        }

        /// <summary>
        /// Gets the events.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public IEnumerable<Event> Events
        {
            get
            {
                if (!_loaded) LoadMembers();
                return _events.Values;
            }
        }

        /// <summary>
        /// Gets all the events, including from base types.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public IEnumerable<Event> AllEvents
        {
            get
            {
                if (!_loaded) LoadMembers();
                ExtendedType type = this;
                while (type != null)
                {
                    foreach (Event @event in type.Events)
                        yield return @event;
                    type = type.BaseType;
                }
            }
        }

        /// <summary>
        /// Gets all the methods.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public IEnumerable<Method> Methods
        {
            get
            {
                if (!_loaded) LoadMembers();
                return _methods.Values.SelectMany(m => m);
            }
        }

        /// <summary>
        /// Gets all the methods, including from base types.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public IEnumerable<Method> AllMethods
        {
            get
            {
                if (!_loaded) LoadMembers();
                ExtendedType type = this;
                while (type != null)
                {
                    foreach (Method method in type.Methods)
                        yield return method;
                    type = type.BaseType;
                }
            }
        }

        /// <summary>
        /// Gets the static constructor, if any.
        /// </summary>
        /// <value>The static constructor if found; otherwise <see langword="null"/>.</value>
        /// <remarks>The static constructor is a special case and does not appear directly in overloads.</remarks>
        [PublicAPI]
        public Constructor StaticConstructor { get; private set; }

        /// <summary>
        /// Gets the constructors.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public IEnumerable<Constructor> Constructors
        {
            get
            {
                if (!_loaded) LoadMembers();
                Debug.Assert(_constructors != null);
                return _constructors;
            }
        }

        /// <summary>
        /// Gets all the constructors, including from base types.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public IEnumerable<Constructor> AllConstructors
        {
            get
            {
                if (!_loaded) LoadMembers();
                ExtendedType type = this;
                while (type != null)
                {
                    foreach (Constructor ctor in type.Constructors)
                        yield return ctor;
                    type = type.BaseType;
                }
            }
        }

        /// <summary>
        /// All the customer attributes on the type.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public IEnumerable<Attribute> CustomAttributes
        {
            get { return _customAttributes.Value ?? Enumerable.Empty<Attribute>(); }
        }

        /// <summary>
        /// All the customer attributes on the type and base types.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public IEnumerable<Attribute> AllCustomAttributes
        {
            get
            {
                ExtendedType type = this;
                while (type != null)
                {
                    foreach (Attribute att in type.CustomAttributes)
                        yield return att;
                    type = type.BaseType;
                }
            }
        }

        /// <summary>
        /// If this type has a default member (indexer), indicates its name.
        /// </summary>
        [CanBeNull]
        [PublicAPI]
        public string DefaultMember
        {
            get { return _defaultMember.Value; }
        }

        /// <summary>
        /// Gets the signature of the type.
        /// </summary>
        /// <remarks>This is modelled after the Type.SigToString internal method.</remarks>
        [NotNull]
        [PublicAPI]
        public string Signature
        {
            get { return _signature.Value ?? Type.FullName ?? Type.Name; }
        }

        /// <summary>
        /// Gets the simple full name for the type.
        /// </summary>
        [PublicAPI]
        public string SimpleFullName
        {
            get { return _simpleFullName.Value ?? Type.FullName ?? Type.Name; }
        }

        /// <summary>
        /// The generic arguments.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public IEnumerable<GenericArgument> GenericArguments
        {
            get
            {
                Debug.Assert(_genericArguments.Value != null);
                return _genericArguments.Value;
            }
        }

        /// <summary>
        /// If this is a nullable type (i.e. <see cref="Nullable{T}"/>) then returns the underlying non-nullable type;
        /// otherwise returns <see cref="Type"/>.
        /// </summary>
        /// <value>The type of the non nullable type.</value>
        [PublicAPI]
        [NotNull]
        public Type NonNullableType
        {
            get
            {
                Debug.Assert(_nonNullableType.Value != null);
                return _nonNullableType.Value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this type is a nullable type (i.e. <see cref="Nullable{T}"/>).
        /// </summary>
        /// <value><see langword="true" /> if this type is nullable type; otherwise, <see langword="false" />.</value>
        [PublicAPI]
        public bool IsNullableType
        {
            get { return Type != _nonNullableType.Value; }
        }

        /// <summary>
        /// Gets a value indicating whether this type is convertible.
        /// </summary>
        /// <value><see langword="true" /> if this instance is convertible; otherwise, <see langword="false" />.</value>
        [PublicAPI]
        public bool IsConvertible
        {
            get { return _isConvertible.Value; }
        }

        /// <summary>
        /// Gets the interfaces implemented by this type.
        /// </summary>
        /// <value>The interfaces.</value>
        [NotNull]
        [PublicAPI]
        public IEnumerable<Type> Interfaces
        {
            get
            {
                Debug.Assert(_interfaces.Value != null);
                return _interfaces.Value.Values;
            }
        }

        /// <summary>
        /// Gets the extended type information for the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The type as an extended type.</returns>
        [NotNull]
        [PublicAPI]
        public static ExtendedType Get([NotNull] Type type)
        {
            // ReSharper disable AssignNullToNotNullAttribute
            return _extendedTypes.GetOrAdd(type, t => new ExtendedType(t));
            // ReSharper restore AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Loads all the members in one go.
        /// </summary>
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
                foreach (MemberInfo memberInfo in Type.GetMembers(AllMembersBindingFlags))
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
                        if (p.Name == DefaultMember ||
                            p.GetIndexParameters().Length > 0)
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
                                Debug.Assert(cm.ToType != Type);
                                _castsTo.Add(cm.ToType, cm);
                            }
                            else
                            {
                                Debug.Assert(cm.ToType == Type);
                                _castsFrom.Add(cm.FromType, cm);
                            }

                            // Use the cast method as a method
                            method = cm;
                        }
                        else
                            method = new Method(this, m);

                        List<Method> methods;
                        if (!_methods.TryGetValue(m.Name, out methods))
                        {
                            methods = new List<Method>();
                            _methods.Add(m.Name, methods);
                        }
                        Debug.Assert(methods != null);
                        methods.Add(method);

                        // If the method name is fully qualified (e.g. explicti interface implementation) then
                        // we need to get the method name without the qualification
                        int mdot = m.Name.LastIndexOf('.');
                        if (mdot > -1)
                        {
                            string shortName = m.Name.Substring(mdot + 1);
                            if (!_methods.TryGetValue(shortName, out methods))
                            {
                                methods = new List<Method>();
                                _methods.Add(shortName, methods);
                            }
                            Debug.Assert(methods != null);
                            methods.Add(method);
                        }
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
                            Debug.Assert(StaticConstructor == null);
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
        /// <param name="implicitOnly">If set to <see langword="true"/> only allows implicit casts.</param>
        /// <returns>
        /// <see langword="true"/> this type implements a cast from the specified type; otherwise <see langword="false"/>.
        /// </returns>
        [PublicAPI]
        public bool ImplementsCastFrom([NotNull] Type fromType, bool implicitOnly = true)
        {
            CastMethod cast = GetCastFromMethod(fromType);
            return cast != null && (!implicitOnly || !cast.IsExplicit);
        }

        /// <summary>
        /// Gets the user-defined method that implements a cast from the given type to this type (if any exist on this type).
        /// </summary>
        /// <param name="fromType">The type to cast from.</param>
        /// <returns>The method that implements the cast; otherwise <see langword="null"/>.</returns>
        [CanBeNull]
        [PublicAPI]
        public CastMethod GetCastFromMethod([NotNull] Type fromType)
        {
            CastMethod cast;
            return _castsFrom.TryGetValue(fromType, out cast) ? cast : null;
        }

        /// <summary>
        /// Whether this type implements a cast to the specified type.
        /// </summary>
        /// <param name="toType">The type to cast to.</param>
        /// <param name="implicitOnly">If set to <see langword="true" /> only allows implicit casts.</param>
        /// <returns>
        /// <see langword="true"/> this type implements a cast to the specified type; otherwise <see langword="false"/>.
        /// </returns>
        [PublicAPI]
        public bool ImplementsCastTo([NotNull] Type toType, bool implicitOnly = true)
        {
            CastMethod cast = GetCastToMethod(toType);
            return cast != null && (!implicitOnly || !cast.IsExplicit);
        }

        /// <summary>
        /// Gets the user-defined method that implements a cast to the given type from this type (if any exist on this type).
        /// </summary>
        /// <param name="toType">The type to cast to.</param>
        /// <returns>The method that implements the cast; otherwise <see langword="null"/>.</returns>
        [CanBeNull]
        [PublicAPI]
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
        [PublicAPI]
        public bool Implements([NotNull] Type interfaceType)
        {
            if (interfaceType == null) throw new ArgumentNullException("interfaceType");
            Debug.Assert(_interfaces.Value != null);
            return _interfaces.Value.ContainsKey(interfaceType.FullName ?? interfaceType.Name);
        }

        /// <summary>
        /// Gets the field.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="includeBase">if set to <see langword="true" /> includes fields from the base type.</param>
        /// <returns>The <see cref="Field" /> if found; otherwise <see langword="null" />.</returns>
        [PublicAPI]
        public Field GetField([NotNull] string name, bool includeBase = true)
        {
            ExtendedType type = this;
            while (type != null)
            {
                if (!type._loaded) type.LoadMembers();
                Field field;
                if (type._fields.TryGetValue(name, out field) ||
                    !includeBase)
                    return field;
                type = type.BaseType;
            }
            return null;
        }

        /// <summary>
        /// Gets the <see cref="Field" /> matching the <see cref="FieldInfo" /> if found.
        /// </summary>
        /// <param name="fieldInfo">The field info.</param>
        /// <param name="includeBase">if set to <see langword="true" /> includes fields from the base type.</param>
        /// <returns>The <see cref="Field" /> if found; otherwise <see langword="null" />.</returns>
        [PublicAPI]
        public Field GetField([NotNull] FieldInfo fieldInfo, bool includeBase = true)
        {
            ExtendedType type = this;
            while (type != null)
            {
                if (!type._loaded) type.LoadMembers();
                // ReSharper disable once PossibleNullReferenceException
                Field field = type._fields.Values.FirstOrDefault(f => f.Info == fieldInfo);
                if ((field != null) ||
                    (!includeBase))
                    return field;
                type = type.BaseType;
            }
            return null;
        }

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="includeBase">if set to <see langword="true" /> includes properties from the base type.</param>
        /// <returns>The <see cref="Property" /> if found; otherwise <see langword="null" />.</returns>
        public Property GetProperty([NotNull] string name, bool includeBase = true)
        {
            ExtendedType type = this;
            while (type != null)
            {
                if (!type._loaded) type.LoadMembers();
                Property property;
                if (type._properties.TryGetValue(name, out property) ||
                    !includeBase)
                    return property;
                type = type.BaseType;
            }
            return null;
        }

        /// <summary>
        /// Gets the <see cref="Property"/> matching the <see cref="PropertyInfo"/> if any.
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        /// <param name="includeBase">if set to <see langword="true" /> includes properties from the base type.</param>
        /// <returns>The <see cref="Property"/> if found; otherwise <see langword="null"/>.</returns>
        public Property GetProperty([NotNull] PropertyInfo propertyInfo, bool includeBase = true)
        {
            ExtendedType type = this;
            while (type != null)
            {
                if (!type._loaded) type.LoadMembers();
                // ReSharper disable once PossibleNullReferenceException
                Property property = type._properties.Values.FirstOrDefault(p => p.Info == propertyInfo);
                if ((property != null) ||
                    !includeBase)
                    return property;
                type = type.BaseType;
            }
            return null;
        }

        /// <summary>
        /// Gets the indexer from the type (or base types).
        /// </summary>
        /// <param name="types">The types.</param>
        /// <returns>The indexer.</returns>
        public Indexer GetIndexer([NotNull] params TypeSearch[] types)
        {
            bool[] castsRequired;
            return GetIndexer(true, out castsRequired, types);
        }

        /// <summary>
        /// Gets the indexer from the type (or base types).
        /// </summary>
        /// <param name="types">The types.</param>
        /// <param name="castsRequired">Any array indicating which parameters require a cast (the last element is for the return type).</param>
        /// <returns>The indexer.</returns>
        [PublicAPI]
        public Indexer GetIndexer(out bool[] castsRequired, [NotNull] params TypeSearch[] types)
        {
            return GetIndexer(true, out castsRequired, types);
        }

        /// <summary>
        /// Gets the indexer.
        /// </summary>
        /// <param name="includeBase">if set to <see langword="true" /> includes indexers from the base type.</param>
        /// <param name="types">The types.</param>
        /// <returns>The indexer.</returns>
        [PublicAPI]
        public Indexer GetIndexer(bool includeBase, [NotNull] params TypeSearch[] types)
        {
            bool[] castsRequired;
            return GetIndexer(includeBase, out castsRequired, types);
        }

        /// <summary>
        /// Gets the indexer.
        /// </summary>
        /// <param name="includeBase">if set to <see langword="true" /> includes indexers from the base type.</param>
        /// <param name="castsRequired">Any array indicating which parameters require a cast (the last element is for the return type).</param>
        /// <param name="types">The types.</param>
        /// <returns>The indexer.</returns>
        [PublicAPI]
        public Indexer GetIndexer(bool includeBase, out bool[] castsRequired, [NotNull] params TypeSearch[] types)
        {
            ExtendedType type = this;
            while (type != null)
            {
                if (!type._loaded) type.LoadMembers();
                Indexer indexer = type._indexers.BestMatch(0, true, true, out castsRequired, types) as Indexer;
                if ((indexer != null) ||
                    !includeBase)
                    return indexer;
                type = type.BaseType;
            }
            castsRequired = Array<bool>.Empty;
            return null;
        }

        /// <summary>
        /// Gets the <see cref="Indexer"/> matching the <see cref="PropertyInfo"/> (if any).
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        /// <param name="includeBase">if set to <see langword="true" /> includes indexers from the base type.</param>
        /// <returns>The indexer.</returns>
        public Indexer GetIndexer([NotNull] PropertyInfo propertyInfo, bool includeBase = true)
        {
            if (propertyInfo == null) throw new ArgumentNullException("propertyInfo");

            ExtendedType type = this;
            while (type != null)
            {
                if (!type._loaded) type.LoadMembers();
                if (propertyInfo.Name == type.DefaultMember)
                {
                    Indexer indexer = type._indexers.FirstOrDefault(i => i.Info == propertyInfo);
                    if (indexer != null) return indexer;
                }
                if (!includeBase)
                    return null;
                type = type.BaseType;
            }
            return null;
        }

        /// <summary>
        /// Gets the methods.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="includeBase">if set to <see langword="true" /> includes methods from the base type.</param>
        /// <returns>The <see cref="Methods"/> if found; otherwise <see langword="null"/>.</returns>
        public IEnumerable<Method> GetMethods([NotNull] string name, bool includeBase = true)
        {
            ExtendedType type = this;
            while (type != null)
            {
                if (!type._loaded) type.LoadMembers();
                List<Method> methods;
                // Use name lookup
                if (type._methods.TryGetValue(name, out methods) ||
                    !includeBase)
                    return methods;
                type = type.BaseType;
            }
            return null;
        }

        /// <summary>
        /// Gets the <see cref="Method"/> matching the <see cref="MethodInfo"/> (if any).
        /// </summary>
        /// <param name="methodInfo">The method info.</param>
        /// <param name="includeBase">if set to <see langword="true" /> includes methods from the base type.</param>
        /// <returns>The <see cref="Method"/> if found; otherwise <see langword="null"/>.</returns>
        public Method GetMethod([NotNull] MethodInfo methodInfo, bool includeBase = true)
        {
            if (methodInfo == null) throw new ArgumentNullException("methodInfo");

            ExtendedType type = this;
            while (type != null)
            {
                if (!type._loaded) type.LoadMembers();
                List<Method> methods;
                if (type._methods.TryGetValue(methodInfo.Name, out methods))
                {
                    Debug.Assert(methods != null);
                    // ReSharper disable once PossibleNullReferenceException
                    Method method = methods.FirstOrDefault(m => m.Info == methodInfo);
                    if (method != null)
                        return method;
                }
                if (!includeBase) return null;
                type = type.BaseType;
            }
            return null;
        }

        /// <summary>
        /// Gets the method.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="types">The parameter types and return type.</param>
        /// <returns>The <see cref="Method"/> if found; otherwise <see langword="null"/>.</returns>
        public Method GetMethod([NotNull] string name, [NotNull] params TypeSearch[] types)
        {
            bool[] castsRequired;
            return GetMethod(name, 0, true, true, out castsRequired, true, types);
        }

        /// <summary>
        /// Gets the method.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="includeBase">if set to <see langword="true" /> includes methods from the base type.</param>
        /// <param name="types">The parameter types and return type.</param>
        /// <returns>The <see cref="Method"/> if found; otherwise <see langword="null"/>.</returns>
        [PublicAPI]
        public Method GetMethod([NotNull] string name, bool includeBase, [NotNull] params TypeSearch[] types)
        {
            bool[] castsRequired;
            return GetMethod(name, 0, true, true, out castsRequired, includeBase, types);
        }

        /// <summary>
        /// Gets the method.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="genericArguments">The number of generic arguments.</param>
        /// <param name="types">The parameter types and return type.</param>
        /// <returns>The <see cref="Method" /> if found; otherwise <see langword="null" />.</returns>
        public Method GetMethod([NotNull] string name, int genericArguments, [NotNull] params TypeSearch[] types)
        {
            bool[] castsRequired;
            return GetMethod(name, genericArguments, true, true, out castsRequired, true, types);
        }

        /// <summary>
        /// Gets the method.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="genericArguments">The number of generic arguments.</param>
        /// <param name="includeBase">if set to <see langword="true" /> includes methods from the base type.</param>
        /// <param name="types">The parameter types and return type.</param>
        /// <returns>The <see cref="Method" /> if found; otherwise <see langword="null" />.</returns>
        [PublicAPI]
        public Method GetMethod(
            [NotNull] string name,
            int genericArguments,
            bool includeBase,
            [NotNull] params TypeSearch[] types)
        {
            bool[] castsRequired;
            return GetMethod(name, genericArguments, true, true, out castsRequired, includeBase, types);
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
        [PublicAPI]
        public Method GetMethod(
            [NotNull] string name,
            int genericArguments,
            bool allowClosure,
            bool allowCasts,
            [NotNull] params TypeSearch[] types)
        {
            bool[] castsRequired;
            return GetMethod(name, genericArguments, allowClosure, allowCasts, out castsRequired, true, types);
        }

        /// <summary>
        /// Gets the method.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="genericArguments">The number of generic arguments.</param>
        /// <param name="allowClosure">if set to <see langword="true" /> will automatically close the signatures generic types if possible.</param>
        /// <param name="allowCasts">if set to <see langword="true" /> then types will match if they can be cast to the required type.</param>
        /// <param name="includeBase">if set to <see langword="true" /> includes methods from the base type.</param>
        /// <param name="types">The parameter types and return type.</param>
        /// <returns>The <see cref="Method" /> if found; otherwise <see langword="null" />.</returns>
        [PublicAPI]
        public Method GetMethod(
            [NotNull] string name,
            int genericArguments,
            bool allowClosure,
            bool allowCasts,
            bool includeBase,
            [NotNull] params TypeSearch[] types)
        {
            bool[] castsRequired;
            return GetMethod(name, genericArguments, allowClosure, allowCasts, out castsRequired, includeBase, types);
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
        public Method GetMethod(
            [NotNull] string name,
            int genericArguments,
            bool allowClosure,
            bool allowCasts,
            out bool[] castsRequired,
            [NotNull] params TypeSearch[] types)
        {
            return GetMethod(name, genericArguments, allowClosure, allowCasts, out castsRequired, true, types);
        }

        /// <summary>
        /// Gets the method.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="genericArguments">The number of generic arguments.</param>
        /// <param name="allowClosure">if set to <see langword="true" /> will automatically close the signatures generic types if possible.</param>
        /// <param name="allowCasts">if set to <see langword="true" /> then types will match if they can be cast to the required type.</param>
        /// <param name="castsRequired">Any array indicating which parameters require a cast (the last element is for the return type).</param>
        /// <param name="includeBase">if set to <see langword="true" /> includes methods from the base type.</param>
        /// <param name="types">The parameter types and return type.</param>
        /// <returns>The <see cref="Method" /> if found; otherwise <see langword="null" />.</returns>
        [PublicAPI]
        public Method GetMethod(
            [NotNull] string name,
            int genericArguments,
            bool allowClosure,
            bool allowCasts,
            out bool[] castsRequired,
            bool includeBase,
            [NotNull] params TypeSearch[] types)
        {
            ExtendedType type = this;
            while (type != null)
            {
                if (!type._loaded) type.LoadMembers();
                List<Method> methods;
                if (type._methods.TryGetValue(name, out methods))
                {
                    Debug.Assert(methods != null);
                    Method method =
                        methods.BestMatch(genericArguments, allowClosure, allowCasts, out castsRequired, types) as
                            Method;
                    if (method != null) return method;
                }
                if (!includeBase) break;
                type = type.BaseType;
            }
            castsRequired = Array<bool>.Empty;
            return null;
        }

        /// <summary>
        /// Gets the constructor.
        /// </summary>
        /// <param name="types">The parameter types and return type (normally the same as <see cref="Type" />).</param>
        /// <returns>The <see cref="Constructor" /> if found; otherwise <see langword="null" />.</returns>
        public Constructor GetConstructor([NotNull] params TypeSearch[] types)
        {
            bool[] castsRequired;
            return GetConstructor(out castsRequired, true, types);
        }

        /// <summary>
        /// Gets the constructor.
        /// </summary>
        /// <param name="includeBase">if set to <see langword="true" /> includes constructors from the base type.</param>
        /// <param name="types">The parameter types and return type (normally the same as <see cref="Type" />).</param>
        /// <returns>The <see cref="Constructor" /> if found; otherwise <see langword="null" />.</returns>
        [PublicAPI]
        public Constructor GetConstructor(bool includeBase, [NotNull] params TypeSearch[] types)
        {
            bool[] castsRequired;
            return GetConstructor(out castsRequired, includeBase, types);
        }

        /// <summary>
        /// Gets the constructor.
        /// </summary>
        /// <param name="castsRequired">Any array indicating which parameters require a cast (the last element is for the return type).</param>
        /// <param name="types">The parameter types and return type (normally the same as <see cref="Type" />).</param>
        /// <returns>The <see cref="Constructor" /> if found; otherwise <see langword="null" />.</returns>
        [PublicAPI]
        public Constructor GetConstructor(out bool[] castsRequired, [NotNull] params TypeSearch[] types)
        {
            return GetConstructor(out castsRequired, true, types);
        }

        /// <summary>
        /// Gets the constructor.
        /// </summary>
        /// <param name="castsRequired">Any array indicating which parameters require a cast (the last element is for the return type).</param>
        /// <param name="includeBase">if set to <see langword="true" /> includes constructors from the base type.</param>
        /// <param name="types">The parameter types and return type (normally the same as <see cref="Type"/>).</param>
        /// <returns>The <see cref="Constructor"/> if found; otherwise <see langword="null"/>.</returns>
        [PublicAPI]
        public Constructor GetConstructor(
            out bool[] castsRequired,
            bool includeBase,
            [NotNull] params TypeSearch[] types)
        {
            ExtendedType type = this;
            while (type != null)
            {
                if (!type._loaded) type.LoadMembers();
                Constructor constructor =
                    type._constructors.BestMatch(0, true, true, out castsRequired, types) as Constructor;
                if ((constructor != null) ||
                    !includeBase)
                    return constructor;
                type = type.BaseType;
            }
            castsRequired = Array<bool>.Empty;
            return null;
        }

        /// <summary>
        /// Gets the <see cref="Constructor"/> matching the <see cref="ConstructorInfo"/> (if any).
        /// </summary>
        /// <param name="includeBase">if set to <see langword="true" /> includes constructors from the base type.</param>
        /// <param name="constructorInfo">The constructor info.</param>
        /// <returns>The constructor information wrapped with useful accessors.</returns>
        public Constructor GetConstructor([NotNull] ConstructorInfo constructorInfo, bool includeBase = true)
        {
            ExtendedType type = this;
            while (type != null)
            {
                if (!type._loaded) type.LoadMembers();
                Constructor constructor = type._constructors == null
                    ? null
                    // ReSharper disable once PossibleNullReferenceException
                    : type._constructors.FirstOrDefault(c => c.Info == constructorInfo);
                if ((constructor != null) ||
                    !includeBase)
                    return constructor;
                type = type.BaseType;
            }
            return null;
        }

        /// <summary>
        /// Gets the event.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="includeBase">if set to <see langword="true" /> includes events from the base type.</param>
        /// <returns>The <see cref="Event"/> if found; otherwise <see langword="null"/>.</returns>
        [PublicAPI]
        public Event GetEvent([NotNull] string name, bool includeBase = true)
        {
            ExtendedType type = this;
            while (type != null)
            {
                if (!type._loaded) type.LoadMembers();
                Event @event;
                if (_events.TryGetValue(name, out @event) ||
                    !includeBase)
                    return @event;
                type = type.BaseType;
            }
            return null;
        }

        /// <summary>
        /// Gets the <see cref="Event"/> matching the <see cref="EventInfo"/> if any.
        /// </summary>
        /// <param name="eventInfo">The event info.</param>
        /// <param name="includeBase">if set to <see langword="true" /> includes events from the base type.</param>
        /// <returns>The <see cref="Event"/> if found; otherwise <see langword="null"/>.</returns>
        public Event GetEvent([NotNull] EventInfo eventInfo, bool includeBase = true)
        {
            ExtendedType type = this;
            while (type != null)
            {
                if (!type._loaded) type.LoadMembers();
                // ReSharper disable once PossibleNullReferenceException
                Event @event = _events.Values.FirstOrDefault(e => e.Info == eventInfo);
                if ((@event != null) ||
                    !includeBase)
                    return @event;
                type = type.BaseType;
            }
            return null;
        }

        /// <summary>
        /// Closes the type if it is generic and has generic parameters, or creates equivalent type.
        /// </summary>
        /// <param name="genericTypes">The generic types (null to use existing type from closed type).</param>
        /// <returns>The <see cref="ExtendedType"/> without open generic parameter if the supplied generic types are able to close the type; otherwise <see langword="null"/>.</returns>
        public ExtendedType CloseType([NotNull] params Type[] genericTypes)
        {
            if (genericTypes == null) throw new ArgumentNullException("genericTypes");

            int length = genericTypes.Length;

            Debug.Assert(_genericArguments.Value != null);

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
                    if (et.IsGenericType)
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
            // ReSharper disable once PossibleNullReferenceException
            string key = String.Join("|", gta.Select(t => t.FullName));

            Debug.Assert(_closedTypes.Value != null);
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
        [ContractAnnotation("type:null=>null;type:notnull=>notnull")]
        public static implicit operator Type(ExtendedType extendedType)
        {
            return extendedType == null ? null : extendedType.Type;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Type"/> to <see cref="ExtendedType"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The result of the conversion.</returns>
        [ContractAnnotation("type:null=>null;type:notnull=>notnull")]
        public static implicit operator ExtendedType(Type type)
        {
            return type == null ? null : Get(type);
        }

        /// <summary>
        /// Determines whether this instance can be converted to the specified type.  Unlike the built in cast system,
        /// this returns true if a cast is possible AND if IConvertible can be used.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><see langword="true" /> if this instance can be cast to the specified type; otherwise, <see langword="false" />.</returns>
        public bool CanConvertTo(Type type)
        {
            if (Type == type)
                return true;

            if ((type == null) ||
                (type.IsGenericTypeDefinition) ||
                (type.ContainsGenericParameters))
                return false;

            Debug.Assert(_convertToCache.Value != null);
            return _convertToCache.Value.GetOrAdd(
                type,
                t =>
                {
                    // Get extended type information for destination type.
                    ExtendedType dest = type;

                    // First we check to see if a cast is possible
                    if ((NonNullableType == dest.NonNullableType) ||
                        (NonNullableType.IsEquivalentTo(NonNullableType)) ||
                        (dest.NonNullableType != typeof(bool) && IsConvertible && dest.IsConvertible) ||
                        (ImplementsCastTo(dest)) ||
                        (dest.ImplementsCastFrom(this)) ||
                        (Implements(typeof(IConvertible)) && _iConvertibleMethods.ContainsKey(type)))
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
        public bool TryConvert([NotNull] Expression expression, [NotNull] out Expression outputExpression)
        {
            if (expression == null) throw new ArgumentNullException("expression");

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
            if (et.Implements(typeof(IConvertible)))
            {
                string methodName;
                IEnumerable<Method> methods;
                if ((_iConvertibleMethods.TryGetValue(Type, out methodName)) &&
                    // ReSharper disable once AssignNullToNotNullAttribute
                    ((methods = et.GetMethods(methodName)) != null))
                {
                    Method cm = methods.FirstOrDefault(
                        // ReSharper disable once PossibleNullReferenceException
                        m => m.ParameterTypes.Count() == 1 && m.ParameterTypes.First() == typeof(IFormatProvider));
                    if (cm != null)
                    {
                        // Call the IConvertible method on the object, passing in CultureInfo.CurrentCulture as the parameter.
                        outputExpression = Expression.Call(
                            expression,
                            cm.Info,
                            Reflection.CurrentCultureExpression);
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
                                    ? Expression.Call(
                                        Expression.New(typeConverterType),
                                        mi,
                                        expression,
                                        Expression.Constant(Type, typeof(Type)))
                                    : Expression.Call(
                                        Expression.New(typeConverterType),
                                        mi,
                                        expression);

                                if (outputExpression.Type != Type)
                                    outputExpression = Expression.Convert(outputExpression, Type);

                                return true;
                            }
                        }
                    }
                }
                catch
                {
                    // Ignore exceptions
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
        /// <returns>
        /// <see langword="true"/> if the type extends from the <paramref name="baseType"/> specified; otherwise <see langword="false"/>
        /// </returns> 
        [PublicAPI]
        public bool DescendsFrom([NotNull] Type baseType)
        {
            Type sourceType = Type;
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