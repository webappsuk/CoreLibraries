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