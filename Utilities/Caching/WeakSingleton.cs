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

using System;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Caching
{
    /// <summary>
    /// Holds a <see cref="WeakReference{T}"/> to an instance of an object, lazily initialising the value if the object has been garbage collected.
    /// </summary>
    /// <typeparam name="T">The type of the singleton.</typeparam>
    [PublicAPI]
    public class WeakSingleton<T>
        where T : class
    {
        [NotNull]
        private readonly WeakReference<T> _weakRef;

        [NotNull]
        private readonly Func<T> _factory;

        /// <summary>
        /// Gets the instance of the type.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        [NotNull]
        public T Instance
        {
            get
            {
                lock (_weakRef)
                {
                    T t = _weakRef.Target;
                    if (t != null) return t;
                    _weakRef.Target = t = _factory();
                    if (t == null) throw new NullReferenceException("The instance factory returned a null instance.");
                    return t;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakSingleton{T}"/> class.
        /// </summary>
        /// <param name="initial">The initial value.</param>
        /// <param name="factory">The factory function. If null, the default constructor for the type will be used.</param>
        public WeakSingleton(T initial = null, Func<T> factory = null)
        {
            _weakRef = new WeakReference<T>(initial, true);
            _factory = factory ?? typeof(T).ConstructorFunc<T>();
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="WeakSingleton{T}"/> to <typeparamref name="T"/>.
        /// </summary>
        /// <param name="weak">The weak.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator T(WeakSingleton<T> weak)
        {
            return weak?.Instance;
        }
    }
}