using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Relection
{
    /// <summary>
    ///   Wraps an <see cref="System.Reflection.EventInfo"/> with accessors.
    /// </summary>
    [DebuggerDisplay("{Info} [Extended]")]
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
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
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
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
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
            _addMethod = new Lazy<MethodInfo>(() => info.GetAddMethod(true), LazyThreadSafetyMode.PublicationOnly);
            _removeMethod = new Lazy<MethodInfo>(() => info.GetRemoveMethod(true), LazyThreadSafetyMode.PublicationOnly);

            // Note events also support 'raise' and 'other' methods, neither of which are currently used in C#
            // Adding support is trivial (identical to above), but would create unnecessary overhead for all
            // but very edge cases (i.e. when working with types created in IL directly).
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="WebApplications.Utilities.Relection.Event"/> to <see cref="System.Reflection.EventInfo"/>.
        /// </summary>
        /// <param name="event">The event.</param>
        /// <returns>The result of the conversion.</returns>
        /// <remarks></remarks>
        public static implicit operator EventInfo(Event @event)
        {
            return @event == null ? null : @event.Info;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Reflection.EventInfo"/> to <see cref="WebApplications.Utilities.Relection.Event"/>.
        /// </summary>
        /// <param name="eventInfo">The event info.</param>
        /// <returns>The result of the conversion.</returns>
        /// <remarks></remarks>
        public static implicit operator Event(EventInfo eventInfo)
        {
            return eventInfo == null
                       ? null
                       : ((ExtendedType) eventInfo.DeclaringType).GetEvent(eventInfo);
        }
    }
}