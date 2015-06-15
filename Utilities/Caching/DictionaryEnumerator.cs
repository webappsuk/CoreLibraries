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

using System.Collections;
using System.Collections.Generic;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Caching
{
    /// <summary>
    ///   A dictionary enumerator wraps the normal enumerator.
    /// </summary>
    [PublicAPI]
    public class DictionaryEnumerator<TKey, TValue> : IDictionaryEnumerator
    {
        /// <summary>
        ///   The actual enumerator.
        /// </summary>
        [NotNull]
        private readonly IEnumerator<KeyValuePair<TKey, TValue>> _enumerator;

        /// <summary>
        ///   Initializes a new instance of the <see cref="DictionaryEnumerator{TKey,TValue}"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        internal DictionaryEnumerator([NotNull] IEnumerable<KeyValuePair<TKey, TValue>> dictionary)
        {
            _enumerator = dictionary.GetEnumerator();
        }

        #region IDictionaryEnumerator Members
        /// <inheritdoc />
        public DictionaryEntry Entry
        {
            get
            {
                // ReSharper disable AssignNullToNotNullAttribute
                return new DictionaryEntry(_enumerator.Current.Key, _enumerator.Current.Value);
                // ReSharper restore AssignNullToNotNullAttribute
            }
        }

        /// <inheritdoc />
        public object Key
        {
            get { return _enumerator.Current.Key; }
        }

        /// <inheritdoc />
        public object Value
        {
            get { return _enumerator.Current.Value; }
        }

        /// <inheritdoc />
        public object Current
        {
            get { return Entry; }
        }

        /// <inheritdoc />
        public bool MoveNext()
        {
            return _enumerator.MoveNext();
        }

        /// <inheritdoc />
        public void Reset()
        {
            _enumerator.Reset();
        }
        #endregion
    }
}