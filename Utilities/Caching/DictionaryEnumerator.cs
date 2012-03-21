#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities
// File: DictionaryEnumerator.cs
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

using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Caching
{
    /// <summary>
    ///   A dictionary enumerator wraps the normal enumerator.
    /// </summary>
    [UsedImplicitly]
    public class DictionaryEnumerator<TKey, TValue> : IDictionaryEnumerator
    {
        /// <summary>
        ///   The actual enumerator.
        /// </summary>
        [NotNull] private readonly IEnumerator<KeyValuePair<TKey, TValue>> _enumerator;

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