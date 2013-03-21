#region © Copyright Web Applications (UK) Ltd, 2013.  All rights reserved.
// Copyright (c) 2013, Web Applications UK Ltd
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
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using WebApplications.Utilities.Enumerations;

namespace WebApplications.Utilities.Caching
{
    /// <summary>
    ///   A potentially observable weak reference that allows notification when a reference is finalized,
    ///   as long as we are not tracking for resurrection and the type implements
    ///   <see cref="WebApplications.Utilities.Caching.IObservableFinalize"/>.
    /// </summary>
    /// <typeparam name="T">The type of the referenced element.</typeparam>
    /// <remarks>
    ///   Article explaining object resurrection: (http://blogs.msdn.com/b/clyon/archive/2006/04/25/583698.aspx)
    /// </remarks>
    [UsedImplicitly]
    public sealed class ObservableWeakReference<T> : WeakReference<T> where T : class
    {
        // ReSharper disable StaticFieldInGenericType
        /// <summary>
        ///   Whether the type is actually observable.
        /// </summary>
        [UsedImplicitly] public static readonly bool ObservableFinalize;

        /// <summary>
        ///   Whether the type is disposable.
        /// </summary>
        [UsedImplicitly] public static readonly bool Disposable;

        // ReSharper restore StaticFieldInGenericType

        /// <summary>
        ///   Keeps track of added handlers.
        /// </summary>
        [NotNull] private readonly List<EventHandler> _handlers = new List<EventHandler>();

        /// <summary>
        ///   Initializes the <see cref="ObservableWeakReference&lt;T&gt;"/> class.
        /// </summary>
        static ObservableWeakReference()
        {
            ObservableFinalize = typeof (IObservableFinalize).IsAssignableFrom(typeof (T));
            Disposable = typeof (IDisposable).IsAssignableFrom(typeof (T));
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="ObservableWeakReference&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="target">The target, which is the underlying object being referenced.</param>
        /// <param name="trackResurrection">
        ///   If set to <see langword="true"/> then the weak reference will be long.
        ///   If unset will track resurrection if the type does not support dispose.</param>
        /// <param name="handlers">Optional event handlers to add to finalized event.</param>
        public ObservableWeakReference([NotNull] T target, TriState trackResurrection = default(TriState),
                                       [NotNull] params EventHandler[] handlers)
            : base(target, trackResurrection == TriState.Unknown ? !Disposable : (trackResurrection == TriState.Yes))
        {
            foreach (EventHandler handler in handlers.Where(handler => handler != null))
                Finalized += handler;
        }

        /// <summary>
        ///   The event raised when the object is finalized (i.e. when its destructor is called).
        /// </summary>
        /// <remarks>
        ///   The event only fires if the object is observable finalize.
        /// </remarks>
        public event EventHandler Finalized
        {
            add
            {
                if (!ObservableFinalize || TrackResurrection) return;

                T target;
                if (!TryGetTarget(out target)) return;

                ((IObservableFinalize) target).Finalized += value;

                _handlers.Add(value);
            }
            remove
            {
                if (!ObservableFinalize ||
                    TrackResurrection ||
                    !_handlers.Remove(value)) return;

                T target;

                if ((TryGetTarget(out target)))
                    ((IObservableFinalize) target).Finalized -= value;
            }
        }

        /// <summary>
        ///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            // ReSharper disable ConditionIsAlwaysTrueOrFalse
            // ReSharper disable HeuristicUnreachableCode
            if (!ObservableFinalize ||
                TrackResurrection) return;
            // ReSharper restore HeuristicUnreachableCode
            // ReSharper restore ConditionIsAlwaysTrueOrFalse

            // Remove all handlers.
            List<EventHandler> handlers = new List<EventHandler>(_handlers);
            foreach (EventHandler handler in handlers)
                Finalized -= handler;
        }
    }
}