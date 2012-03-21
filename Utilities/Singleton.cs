#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities
// File: Singleton.cs
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
using System.Collections.Concurrent;
using JetBrains.Annotations;

namespace WebApplications.Utilities
{
    /// <summary>
    ///   A base class for singletons.
    /// </summary>
    /// <typeparam name="TKey">The singleton key type.</typeparam>
    /// <typeparam name="TSingleton">The singleton type.</typeparam>
    /// <remarks>A singleton is a globally available object that can only be initialised once.</remarks>
    public abstract class Singleton<TKey, TSingleton>
        where TSingleton : Singleton<TKey, TSingleton>
    {
        // ReSharper disable StaticFieldInGenericType
        /// <summary>
        ///   Holds all singletons of the type <typeparamref name="TSingleton"/>.
        /// </summary>
        [UsedImplicitly] protected static readonly ConcurrentDictionary<TKey, TSingleton> Singletons =
            new ConcurrentDictionary<TKey, TSingleton>();

        /// <summary>
        ///   Holds a function to create a singleton.
        /// </summary>
        [UsedImplicitly] protected static readonly Func<TKey, TSingleton> Constructor;

        // ReSharper restore StaticFieldInGenericType

        /// <summary>
        ///   The singleton key.
        /// </summary>
        [UsedImplicitly] [NotNull] protected readonly TKey Key;

        /// <summary>
        ///   Gets the singleton's type constructor.
        /// </summary>
        /// <exception cref="TypeInitializationException">Failed to get the type constructor.</exception>
        static Singleton()
        {
            // Get the constructor for this type.
            try
            {
                Constructor = typeof (TSingleton).ConstructorFunc<TKey, TSingleton>();
            }
            catch (Exception exception)
            {
                throw new TypeInitializationException(typeof (TSingleton).FullName, exception);
            }
        }

        /// <summary>
        ///   Prevents an instance of the <see cref="Singleton&lt;TKey, TSingleton&gt;"/> class from being created externally,
        ///   which therefore enforces the singleton pattern.
        /// </summary>
        /// <param name="key">The singleton key.</param>
        /// <remarks>
        ///   A singleton can be created more than once but only one created object will 'win'.
        /// </remarks>
        protected Singleton([NotNull] TKey key)
        {
            Key = key;
        }

        /// <summary>
        ///   Creates and returns the singleton.
        /// </summary>
        /// <param name="key">The singleton key.</param>
        [NotNull]
        [UsedImplicitly]
        protected static TSingleton GetSingleton([NotNull] TKey key)
        {
            return Singletons.GetOrAdd(key, k => Constructor(k));
        }
    }
}