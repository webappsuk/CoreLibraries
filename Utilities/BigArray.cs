#region © Copyright Web Applications (UK) Ltd, 2012.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities
// File: BigArray.cs
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
// © Copyright Web Applications (UK) Ltd, 2012.  All rights reserved.
#endregion

using System;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Security;
using JetBrains.Annotations;

namespace WebApplications.Utilities
{
    /// <summary>
    /// Allows creation of arrays that take > 2GB of memory.
    /// </summary>
    /// <typeparam name="T">The array contents.</typeparam>
    /// <remarks>
    /// Use with caution, this will deliberately eat up a lot of memory.
    /// </remarks>
    public class BigArray<T> : ICloneable, IList, IStructuralComparable, IStructuralEquatable
    {
        // These need to be const so that the getter/setter get inlined by the JIT into 
        // calling methods just like with a real array to have any chance of meeting our 
        // performance goals.
        //
        // BLOCK_SIZE must be a power of 2, and we want it to be big enough that we allocate
        // blocks in the large object heap so that they don't move.
        /// <summary>
        /// The block size, if creating arrays smaller than the block size, consider using
        /// standard arrays.
        /// </summary>
        public const int BlockSize = 262144;
        private const int BlockSizeLog2 = 18;

        // Don't use a multi-dimensional array here because then we can't right size the last
        // block and we have to do range checking on our own and since there will then be 
        // exception throwing in our code there is a good chance that the JIT won't inline.
        // maximum BigArray size = BLOCK_SIZE * Int.MaxValue
        [NotNull]
        private readonly T[][] _elements;
        private readonly ulong _length;


        /// <summary>
        /// Initializes a new instance of the <see cref="BigArray&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <remarks></remarks>
        public BigArray(int size)
            : this((ulong)size)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BigArray&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <remarks></remarks>
        public BigArray(ulong size)
        {
            int numBlocks = (int)Math.Ceiling((double)size / BlockSize);

            _length = size;
            _elements = new T[numBlocks][];
            for (int i = 0; i < (numBlocks - 1); i++)
            {
                _elements[i] = new T[BlockSize];
                size -= BlockSize;
            }

            // by making sure to make the last block right sized then we get the range checks 
            // for free with the normal array range checks and don't have to add our own
            _elements[numBlocks - 1] = new T[size];
        }

        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <remarks></remarks>
        public ulong Length
        {
            get { return _length; }
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <returns>The element at the specified index.</returns>
        ///   
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///   <paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.IList"/>. </exception>
        ///   
        /// <exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.IList"/> is read-only. </exception>
        /// <remarks></remarks>
        public T this[ulong index]
        {
            get
            {
                int blockNum = (int)(index >> BlockSizeLog2);
                int elementNumberInBlock = (int)(index & (BlockSize - 1));
                return _elements[blockNum][elementNumberInBlock];
            }
            set
            {
                int blockNum = (int)(index >> BlockSizeLog2);
                int elementNumberInBlock = (int)(index & (BlockSize - 1));
                _elements[blockNum][elementNumberInBlock] = value;
            }
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <returns>The element at the specified index.</returns>
        ///   
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///   <paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.IList"/>. </exception>
        ///   
        /// <exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.IList"/> is read-only. </exception>
        /// <remarks></remarks>
        public T this[int index]
        {
            get
            {
                int blockNum = index >> BlockSizeLog2;
                int elementNumberInBlock = index & (BlockSize - 1);
                return _elements[blockNum][elementNumberInBlock];
            }
            set
            {
                int blockNum = index >> BlockSizeLog2;
                int elementNumberInBlock = index & (BlockSize - 1);
                _elements[blockNum][elementNumberInBlock] = value;
            }
        }

        /// <inheritdoc/>
        [SecuritySafeCritical]
        public object Clone()
        {
            return MemberwiseClone(); 
        }

        /// <inheritdoc/>
        public IEnumerator GetEnumerator()
        {
            for (ulong ul = 0; ul < _length; ul++)
                yield return this[ul];
        }

        /// <inheritdoc/>
        public void CopyTo(Array array, int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public int Count
        {
            get
            {
                if (_length > int.MaxValue)
                    throw new InvalidOperationException(String.Format("Count '{0}' is too big to be expressed as an interger.", _length));
                return (int)_length;
            }
        }

        /// <inheritdoc/>
        public object SyncRoot
        {
            get { return this; }
        }

        /// <inheritdoc/>
        public bool IsSynchronized
        {
            get { return false; }
        }

        /// <inheritdoc/>
        public int Add(object value)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public bool Contains(object value)
        {
            if (!(value is T))
                return false;
            return LongIndexOf((T)value) != ulong.MaxValue;
        }

        /// <inheritdoc/>
        public void Clear()
        {
            foreach (T[] array in _elements)
            {
                Contract.Assert(array != null);
                Array.Clear(array, 0, array.Length);
            }
        }

        /// <summary>
        /// Get the long index of the value; otherwise <see cref="ulong.MaxValue"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The long index of the value; otherwise <see cref="ulong.MaxValue"/>.</returns>
        /// <remarks></remarks>
        public ulong LongIndexOf(T value)
        {
            ulong offset = 0UL;
            foreach (T[] array in _elements)
            {
                Contract.Assert(array != null);
                int index = Array.IndexOf(array, value);
                if (index >= 0)
                    return (ulong) index + offset;
                offset += BlockSize;
            }
            return ulong.MaxValue;
        }

        /// <inheritdoc/>
        public int IndexOf(object value)
        {
            if (!(value is T))
                return -1;

            ulong index = LongIndexOf((T)value);
            if (index == ulong.MaxValue)
                return -1;
            if (index > int.MaxValue)
                throw new InvalidOperationException(String.Format("Index '{0}' is too big to be expressed as an interger.", index));
            return (int)index;
        }

        /// <inheritdoc/>
        public void Insert(int index, object value)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public void Remove(object value)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        object IList.this[int index]
        {
            get { return this[(ulong)index]; }
            set { this[(ulong) index] = (T) value; }
        }

        /// <inheritdoc/>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <inheritdoc/>
        public bool IsFixedSize
        {
            get { return true; }
        }

        /// <inheritdoc/>
        public int CompareTo(object other, [NotNull]IComparer comparer)
        {
            if (other == null)
            {
                return 1;
            }

            BigArray<T> o = other as BigArray<T>;

            if (o == null || this._length != o._length)
            {
                throw new ArgumentException(String.Format("The other must be a BigArray of type '{0}'.", typeof (T)),
                                            "other");
            }

            ulong i = 0;
            int c = 0;

            while (i < o._length && c == 0)
            {
                object left = this[i];
                object right = o[i];

                c = comparer.Compare(left, right);
                i++;
            }

            return c;
        }

        /// <inheritdoc/>
        public bool Equals(object other, [NotNull]IEqualityComparer comparer)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            BigArray<T> o = other as BigArray<T>;

            if (o == null || o.Length != this.Length)
                return false;

            ulong i = 0;
            while (i < o._length)
            {
                object left = this[i];
                object right = o[i];

                if (!comparer.Equals(left, right))
                    return false;
                i++;
            }

            return true;
        }

        /// <inheritdoc/>
        public int GetHashCode(IEqualityComparer comparer)
        {
            if (comparer == null)
                throw new ArgumentNullException("comparer");
            Contract.EndContractBlock();

            int ret = 0;

            for (ulong i = (this._length >= 8 ? this._length - 8 : 0); i < this._length; i++)
                ret = (((ret << 5) + ret) ^ comparer.GetHashCode(this[i]));

            return ret; 
        }
    }
}