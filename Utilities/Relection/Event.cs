using System;
using System.Reflection;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Relection
{
    /// <summary>
    ///   Wraps an <see cref="System.Reflection.EventInfo"/> with accessors.
    /// </summary>
    public class Event
    {
        /// <summary>
        /// The extended type.
        /// </summary>
        [NotNull]
        public readonly ExtendedType ExtendedType;

        /// <summary>
        ///   The event info object, provides access to event metadata.
        /// </summary>
        [NotNull]
        public readonly EventInfo Info;

        /// <summary>
        ///   Grabs the add method lazily.
        /// </summary>
        [NotNull]
        private readonly Lazy<MethodInfo> _addMethod;

        /// <summary>
        ///   Gets the add method.
        /// </summary>
        [CanBeNull]
        public MethodInfo AddMethod
        {
            get { return _addMethod.Value; }
        }

        /// <summary>
        ///   Grabs the remove method lazily.
        /// </summary>
        [NotNull]
        private readonly Lazy<MethodInfo> _removeMethod;

        /// <summary>
        ///   Gets the remove method.
        /// </summary>
        [CanBeNull]
        public MethodInfo RemoveMethod
        {
            get { return _removeMethod.Value; }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Event"/> class.
        /// </summary>
        /// <param name="extendedType">The extended type.</param>
        /// <param name="info">The event metadata.</param>
        internal Event([NotNull]ExtendedType extendedType, [NotNull]EventInfo info)
        {
            ExtendedType = extendedType;
            Info = info;
            _addMethod = new Lazy<MethodInfo>(() => info.GetAddMethod(true));
            _removeMethod = new Lazy<MethodInfo>(() => info.GetRemoveMethod(true));

            // Note events also support 'raise' and 'other' methods, neither of which are currently used in C#
            // Adding support is trivial (identical to above), but would create unnecessary overhead for all
            // but very edge cases (i.e. when working with types created in IL directly).
        }
    }
}