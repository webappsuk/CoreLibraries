using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Relection
{
    /// <summary>
    /// Used to match types in searches.
    /// </summary>
    /// <remarks></remarks>
    [DebuggerDisplay("{DebugString}")]
    public class TypeSearch
    {
        #region Defaults
        /// <summary>
        /// The void type.
        /// </summary>
        public static readonly TypeSearch Void = new TypeSearch(typeof(void));

        /// <summary>
        /// The first generic argument on the type.
        /// </summary>
        public static readonly TypeSearch T1 = new TypeSearch(GenericArgumentLocation.Type, 0);
        /// <summary>
        /// The first generic argument on the type.
        /// </summary>
        public static readonly TypeSearch T2 = new TypeSearch(GenericArgumentLocation.Type, 1);
        /// <summary>
        /// The first generic argument on the type.
        /// </summary>
        public static readonly TypeSearch T3 = new TypeSearch(GenericArgumentLocation.Type, 2);
        /// <summary>
        /// The first generic argument on the type.
        /// </summary>
        public static readonly TypeSearch T4 = new TypeSearch(GenericArgumentLocation.Type, 3);
        /// <summary>
        /// The first generic argument on the type.
        /// </summary>
        public static readonly TypeSearch T5 = new TypeSearch(GenericArgumentLocation.Type, 4);
        /// <summary>
        /// The first generic argument on the type.
        /// </summary>
        public static readonly TypeSearch T6 = new TypeSearch(GenericArgumentLocation.Type, 5);
        /// <summary>
        /// The first generic argument on the type.
        /// </summary>
        public static readonly TypeSearch T7 = new TypeSearch(GenericArgumentLocation.Type, 6);
        /// <summary>
        /// The first generic argument on the type.
        /// </summary>
        public static readonly TypeSearch T8 = new TypeSearch(GenericArgumentLocation.Type, 7);

        /// <summary>
        /// The first generic argument on the type.
        /// </summary>
        public static readonly TypeSearch M1 = new TypeSearch(GenericArgumentLocation.Method, 0);
        /// <summary>
        /// The first generic argument on the type.
        /// </summary>
        public static readonly TypeSearch M2 = new TypeSearch(GenericArgumentLocation.Method, 1);
        /// <summary>
        /// The first generic argument on the type.
        /// </summary>
        public static readonly TypeSearch M3 = new TypeSearch(GenericArgumentLocation.Method, 2);
        /// <summary>
        /// The first generic argument on the type.
        /// </summary>
        public static readonly TypeSearch M4 = new TypeSearch(GenericArgumentLocation.Method, 3);
        /// <summary>
        /// The first generic argument on the type.
        /// </summary>
        public static readonly TypeSearch M5 = new TypeSearch(GenericArgumentLocation.Method, 4);
        /// <summary>
        /// The first generic argument on the type.
        /// </summary>
        public static readonly TypeSearch M6 = new TypeSearch(GenericArgumentLocation.Method, 5);
        /// <summary>
        /// The first generic argument on the type.
        /// </summary>
        public static readonly TypeSearch M7 = new TypeSearch(GenericArgumentLocation.Method, 6);
        /// <summary>
        /// The first generic argument on the type.
        /// </summary>
        public static readonly TypeSearch M8 = new TypeSearch(GenericArgumentLocation.Method, 7);
        #endregion

        /// <summary>
        /// The generic argument location to search for (if any).
        /// </summary>
        public readonly GenericArgumentLocation GenericArgumentLocation;

        /// <summary>
        /// The generic argument position to search for (if any; otherwise -1).
        /// </summary>
        public readonly int GenericArgumentPosition;

        /// <summary>
        /// The generic argument name to search for (if any).
        /// </summary>
        [CanBeNull]
        public readonly string GenericArgumentName;

        /// <summary>
        /// Whether the type is by reference.
        /// </summary>
        public readonly bool IsByReference;

        /// <summary>
        /// Whether the type is a pointer.
        /// </summary>
        public readonly bool IsPointer;

        /// <summary>
        /// A concrete type to search for (if any).
        /// </summary>
        [CanBeNull]
        public readonly Type Type;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The name.</param>
        /// <param name="location">The location.</param>
        /// <param name="position">The position.</param>
        /// <param name="isByReference">if set to <see langword="true"/> [is by reference].</param>
        /// <param name="isPointer">if set to <see langword="true"/> [is pointer].</param>
        /// <remarks></remarks>
        private TypeSearch(Type type = null, string name = null, GenericArgumentLocation location = GenericArgumentLocation.None, int position = -1, bool isByReference = false, bool isPointer = false)
        {
            Type = type;
            GenericArgumentName = name;
            GenericArgumentLocation = location;
            GenericArgumentPosition = position;
            IsByReference = isByReference;
            IsPointer = isPointer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeSearch"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <remarks></remarks>
        public TypeSearch([NotNull]Type type)
            : this(type, isByReference: type.IsByRef, isPointer: type.IsPointer)
        {
            if (type.IsGenericParameter)
            {
                GenericArgumentLocation = type.DeclaringMethod != null
                                              ? GenericArgumentLocation.Method
                                              : GenericArgumentLocation.Type;
                GenericArgumentPosition = type.GenericParameterPosition;
                GenericArgumentName = type.Name;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeSearch"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="position">The position.</param>
        /// <param name="isByReference">if set to <see langword="true"/> is by reference.</param>
        /// <param name="isPointer">if set to <see langword="true"/> is pointer.</param>
        /// <remarks></remarks>
        public TypeSearch(GenericArgumentLocation location, int position, bool isByReference = false, bool isPointer = false)
            : this(null, null, location, position, isByReference, isPointer)
        {
            // Validate location
            switch (GenericArgumentLocation)
            {
                case GenericArgumentLocation.Method:
                case GenericArgumentLocation.Type:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("location");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeSearch"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="name">The name.</param>
        /// <param name="isByReference">if set to <see langword="true"/> is by reference.</param>
        /// <param name="isPointer">if set to <see langword="true"/> is pointer.</param>
        /// <remarks></remarks>
        public TypeSearch(GenericArgumentLocation location, [NotNull]string name, bool isByReference = false, bool isPointer = false)
            : this(null, name, location, -1, isByReference, isPointer)
        {
            // Validate location
            switch (GenericArgumentLocation)
            {
                case GenericArgumentLocation.Method:
                case GenericArgumentLocation.Type:
                case GenericArgumentLocation.Any:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("location");
            }
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="WebApplications.Utilities.Relection.TypeSearch"/> to <see cref="System.Type"/>.
        /// </summary>
        /// <param name="typeSearch">The type search.</param>
        /// <returns>The result of the conversion.</returns>
        /// <remarks></remarks>
        public static implicit operator Type(TypeSearch typeSearch)
        {
            return typeSearch != null ? typeSearch.Type : null;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="WebApplications.Utilities.Relection.TypeSearch"/> to <see cref="System.Type"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The result of the conversion.</returns>
        /// <remarks></remarks>
        public static implicit operator TypeSearch(Type type)
        {
            return type != null ? new TypeSearch(type) : null;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public string DebugString
        {
            get
            {
                return String.Format("Type: {0}{1}",
                                            Type == null ? "Unspecified" : "'" + ((ExtendedType)Type).Signature + "'",
                                            GenericArgumentLocation == GenericArgumentLocation.None
                                                ? String.Empty
                                                : String.Format(" [Location: {0}; Name: {1}; Position: {2}",
                                                                GenericArgumentLocation,
                                                                GenericArgumentName == null
                                                                    ? "Unspecified"
                                                                    : "'" + GenericArgumentName + "'",
                                                                GenericArgumentPosition < 0
                                                                    ? "Unspecified"
                                                                    : GenericArgumentPosition.ToString()));
            }
        }
    }
}