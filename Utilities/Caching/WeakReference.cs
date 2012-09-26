#region © Copyright Web Applications (UK) Ltd, 2012.  All rights reserved.
// Copyright (c) 2012, Web Applications UK Ltd
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