#region © Copyright Web Applications (UK) Ltd, 2014.  All rights reserved.
// Copyright (c) 2014, Web Applications UK Ltd
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
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Reflect
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
        ///   Grabs the remove method lazily.
        /// </summary>
        [NotNull]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Lazy<MethodInfo> _removeMethod;

        /// <summary>
        ///   Initializes a new instance of the <see cref="Event"/> class.
        /// </summary>
        /// <param name="extendedType">The extended type.</param>
        /// <param name="info">The event metadata.</param>
        internal Event([NotNull] ExtendedType extendedType, [NotNull] EventInfo info)
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
        ///   Gets the add method.
        /// </summary>
        [CanBeNull]
        public MethodInfo AddMethod
        {
            get { return _addMethod.Value; }
        }

        /// <summary>
        ///   Gets the remove method.
        /// </summary>
        [CanBeNull]
        public MethodInfo RemoveMethod
        {
            get { return _removeMethod.Value; }
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="WebApplications.Utilities.Reflect.Event"/> to <see cref="System.Reflection.EventInfo"/>.
        /// </summary>
        /// <param name="event">The event.</param>
        /// <returns>The result of the conversion.</returns>
        /// <remarks></remarks>
        public static implicit operator EventInfo(Event @event)
        {
            return @event == null ? null : @event.Info;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Reflection.EventInfo"/> to <see cref="WebApplications.Utilities.Reflect.Event"/>.
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