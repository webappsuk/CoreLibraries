using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities
{
    /// <summary>
    /// Holds an ordered collection of mappings to underlying data.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    /// <seealso cref="IReadOnlyList{T}"/>
    /// <seealso cref="ICollection"/>
    /// <seealso cref="ReadOnlyWindow{T}"/>
    /// <remarks>
    /// <para>The <see cref="ReadOnlyMap{T}"/> extends a <see cref="List{T}"/> of <see cref="IReadOnlyList{T}"/>, and
    /// so can be initialized and manipulated as a list.  When in use it can be case to a <see cref="IReadOnlyList{T}"/>
    /// of <typeparamref name="T"/> or an <see cref="ICollection"/>, at which point it acts like a single collection
    /// mapping requests for calls onto the underlying data.</para>
    /// <para>The <see cref="ReadOnlyOffsetMap{T}"/> can be used where you wish to map onto a single data source.</para>
    /// </remarks>
    [Serializable]
    [PublicAPI]
    public class ReadOnlyMap<T> : List<IReadOnlyList<T>>, IReadOnlyList<T>, ICollection
    {
        /// <inheritdoc/>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            foreach (IReadOnlyList<T> window in this)
            {
                if (window == null) continue;
                IEnumerator<T> enumerator = window.GetEnumerator();
                while (enumerator.MoveNext())
                    yield return enumerator.Current;
            }
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>)this).GetEnumerator();

        /// <inheritdoc/>
        void ICollection.CopyTo(Array array, int index)
        {
            foreach (T item in ((IEnumerable<T>)this))
                array.SetValue(item, index++);
        }

        /// <inheritdoc/>
        object ICollection.SyncRoot => (object)((IEnumerable<IReadOnlyList<T>>)this).FirstOrDefault() ?? this;

        /// <inheritdoc/>
        bool ICollection.IsSynchronized => false;

        /// <inheritdoc/>
        int ICollection.Count => ((IEnumerable<IReadOnlyList<T>>)this).Sum(w => w?.Count ?? 0);

        /// <inheritdoc/>
        int IReadOnlyCollection<T>.Count => ((IEnumerable<IReadOnlyList<T>>)this).Sum(w => w?.Count ?? 0);

        /// <inheritdoc/>
        T IReadOnlyList<T>.this[int index]
        {
            get
            {
                if (index < 0)
                    throw new IndexOutOfRangeException(nameof(index));
                foreach (IReadOnlyList<T> window in this)
                {
                    if (window == null) continue;
                    if (index < window.Count) return window[index];
                    index -= window.Count;
                }
                throw new IndexOutOfRangeException(nameof(index));
            }
        }
    }
}