#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities
// File: InitialisedSingleton.cs
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

using JetBrains.Annotations;

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

            // Use optimistic locking to see if we're initialised
            // Most of the time we will be initialised so we won't get a lock.
            if (!singleton.IsInitialised)
            {
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