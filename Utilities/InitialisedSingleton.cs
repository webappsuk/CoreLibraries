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

using System.Diagnostics;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities
{
    /// <summary>
    ///   The base class for an initialised singleton.
    /// </summary>
    /// <typeparam name="TKey">The singleton key type.</typeparam>
    /// <typeparam name="TSingleton">The singleton type.</typeparam>
    /// <remarks>
    ///   An initialised singleton performs the majority of work after construction.
    /// </remarks>
    public abstract class InitialisedSingleton<TKey, TSingleton> : Singleton<TKey, TSingleton>
        where TSingleton : InitialisedSingleton<TKey, TSingleton>
    {
        /// <summary>
        ///   The initialisation lock object.
        /// </summary>
        [NotNull]
        private readonly object _initialisationLock = new object();

        /// <summary>
        ///   Initialises a new instance of the <see cref="InitialisedSingleton&lt;TKey, TSingleton&gt;"/> class.
        /// </summary>
        /// <param name="key">The singleton key.</param>
        /// <remarks>
        ///   A singleton can be created more than once however only one created object will 'win'.
        /// </remarks>
        protected InitialisedSingleton([NotNull] TKey key)
            : base(key)
        {
        }

        /// <summary>
        ///   Returns a <see cref="bool"/> value that indicates whether or not this singleton has been initialised yet.
        /// </summary>
        [UsedImplicitly]
        protected bool IsInitialised { get; private set; }

        /// <summary>
        ///   Creates the singleton.
        /// </summary>
        /// <param name="key">The singleton key.</param>
        /// <remarks>
        ///   Uses optimistic locking to ensure that the singleton is only initialised once.
        /// </remarks>
        [NotNull]
        protected new static TSingleton GetSingleton([NotNull] TKey key)
        {
            TSingleton singleton = Singletons.GetOrAdd(key, k => Constructor(k));
            Debug.Assert(singleton != null);

            // Use optimistic locking to see if we're initialised
            // Most of the time we will be initialised so we won't get a lock.
            if (!singleton.IsInitialised)
                lock (singleton._initialisationLock)
                {
                    // Once we have a lock, we need to re-check, incase a different thread
                    // won the race to initialise.
                    if (!singleton.IsInitialised)
                        // We're the first thread with a lock so initialise.
                        singleton.Initialise();

                    // We are now initialised (so long as an exception isn't thrown).
                    singleton.IsInitialised = true;
                }
            return singleton;
        }

        /// <summary>
        ///   Override this in the inheriting class to initialise the singleton.
        /// </summary>
        /// <remarks>
        ///   This function is called once, and only once, on singleton creation.
        /// </remarks>
        protected abstract void Initialise();
    }
}