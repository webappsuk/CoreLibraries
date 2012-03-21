#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities
// File: WeakReference.cs
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
using JetBrains.Annotations;

namespace WebApplications.Utilities.Caching
{
    /// <summary>
    ///   A type safe weak reference.
    /// </summary>
    /// <typeparam name="T">The type of the referenced element.</typeparam>
    [UsedImplicitly]
    public class WeakReference<T> where T : class
    {
        /// <summary>
        ///   The underlying weak reference.
        /// </summary>
        private readonly WeakReference _weakReference;

        /// <summary>
        ///   Initializes a new instance of the <see cref="WeakReference&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="target">The target, which is the underlying object being referenced.</param>
        /// <param name="trackResurrection">If set to <see langword="true"/> then the weak reference will be long.</param>
        /// <remarks>
        ///   <para>If <paramref name="trackResurrection"/> is <see langword="true"/> then a long weak reference is created
        ///   otherwise a short weak reference is created. A long reference is retained after the objects Finalize method is
        ///   called up until it is collected.</para>
        ///   <para>If the object type doesn't offer a Finalize method then short and long references behave the same.</para>
        /// </remarks>
        public WeakReference([NotNull] T target, bool trackResurrection = false)
        {
            _weakReference = new WeakReference(target, trackResurrection);
        }

        /// <summary>
        ///   Returns a <see cref="bool"/> indicating whether or not we're tracking objects until they're collected
        ///   (<see langword="true"/>) or just until they're finalized (<see langword="false"/>).
        /// </summary>
        [UsedImplicitly]
        public bool TrackResurrection
        {
            get { return _weakReference.TrackResurrection; }
        }

        /// <summary>
        ///   Gets a <see langword="bool"/> value indicating whether this instance is alive.
        /// </summary>
        /// <remarks>
        ///   If this is <see langword="true"/> then the instance has not yet been garbage collected, meaning it's still accessible.
        /// </remarks>
        [UsedImplicitly]
        public bool IsAlive
        {
            get { return _weakReference.IsAlive; }
        }

        /// <summary>
        ///   Gets or sets the target, which is the underlying object being referenced.
        /// </summary>
        [UsedImplicitly]
        public T Target
        {
            get { return (T) _weakReference.Target; }
            set { _weakReference.Target = value; }
        }

        /// <summary>
        ///   Tries to get the target, which is the underlying object being referenced.
        /// </summary>
        /// <param name="target">The weak reference's target.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the reference is still alive meaning the target object can be retrieved;
        ///   otherwise returns <see langword="false"/>.
        /// </returns>
        public bool TryGetTarget(out T target)
        {
            if (_weakReference.IsAlive)
            {
                object targetObj = _weakReference.Target;
                if (targetObj != null)
                    target = (T) targetObj;
                else
                    target = default(T);
                return true;
            }
            target = default(T);
            return false;
        }
    }
}