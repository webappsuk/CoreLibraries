#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities
// File: ObservableWeakReference.cs
// 
// This software, its object code and source code and all modifications made to
// the same (the “Software”) are, and shall at all times remain, the proprietary
// information and intellectual property rights of Web Applications (UK) Limited. 
// You are only entitled to use the Software as expressly permitted by Web
// Applications (UK) Limited within the Software Customisation and
// Licence Agreement (the “Agreement”).  Any copying, modification, decompiling,
// distribution, licensing, sale, transfer or other use of the Software other than
// as expressly permitted in the Agreement is expressly forbidden.  Web
// Applications (UK) Limited reserves its rights to take action against you and
// your employer in accordance with its contractual and common law rights
// (including injunctive relief) should you breach the terms of the Agreement or
// otherwise infringe its copyright or other intellectual property rights in the
// Software.
// 
// © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
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