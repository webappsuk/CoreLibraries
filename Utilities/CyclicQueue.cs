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
using System.Collections;
using System.Collections.Generic;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities
{
    /// <summary>
    /// Represents a first-in, first-out collection of objects. As the size exceeds <see cref="CyclicQueue{T}.Capacity"/> the queue automatically throws away head items.
    /// </summary>
    /// <typeparam name="T">Specifies the type of elements in the queue.</typeparam>
    [PublicAPI]
    public class CyclicQueue<T> : IReadOnlyCollection<T>
    {
        [NotNull]
        private readonly T[] _array;

        private int _head;
        private int _tail;
        private int _size;
        private int _version;

        /// <summary>
        /// Initializes a new instance of the <see cref="CyclicQueue{T}"/> class.
        /// </summary>
        /// <param name="size">The size.</param>
        public CyclicQueue(int size)
        {
            _array = new T[size];
        }

        /// <summary>
        /// Gets the maximum number of elements in the collection.
        /// </summary>
        /// <returns>The maximum number of elements in the collection. </returns>
        public int Capacity
        {
            get { return _array.Length; }
        }

        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        /// <returns>The number of elements in the collection. </returns>
        public int Count
        {
            get { return _size; }
        }

        /// <summary>
        /// Removes all objects from the <see cref="CyclicQueue{T}" />.
        /// </summary>
        public void Clear()
        {
            if (_head < _tail)
                Array.Clear(_array, _head, _size);
            else
            {
                Array.Clear(_array, _head, _array.Length - _head);
                Array.Clear(_array, 0, _tail);
            }

            _head = 0;
            _tail = 0;
            _size = 0;
            _version++;
        }

        /// <summary>
        /// Adds an object to the end of the <see cref="CyclicQueue{T}" />.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="CyclicQueue{T}" />.</param>
        public void Enqueue(T item)
        {
            if (_size == _array.Length)
            {
                _head = (_head + 1) % _array.Length;
                _size--;
            }

            _array[_tail] = item;
            _tail = (_tail + 1) % _array.Length;
            _size++;
            _version++;
        }

        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="CyclicQueue{T}" />.
        /// </summary>
        /// <returns>
        /// The object that is removed from the beginning of the <see cref="CyclicQueue{T}" />.
        /// </returns>
        public T Dequeue()
        {
            if (Count < 1) throw new InvalidOperationException(Resources.CyclicQueue_QueueEmpty);

            T removed = _array[_head];
            _array[_head] = default(T);
            _head = (_head + 1) % _array.Length;
            _size--;
            _version++;
            return removed;
        }

        /// <summary>
        /// Returns the object at the beginning of the <see cref="CyclicQueue{T}" /> without removing it.
        /// </summary>
        /// <returns>
        /// The object at the beginning of the <see cref="CyclicQueue{T}" />.
        /// </returns>
        public T Peek()
        {
            if (Count < 1) throw new InvalidOperationException(Resources.CyclicQueue_QueueEmpty);

            return _array[_head];
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        private T GetElement(int i)
        {
            return _array[(_head + i) % _array.Length];
        }

        private struct Enumerator : IEnumerator<T>
        {
            [NotNull]
            private readonly CyclicQueue<T> _q;

            private int _index; // -1 = not started, -2 = ended/disposed
            private readonly int _version;
            private T _currentElement;

            internal Enumerator([NotNull] CyclicQueue<T> q)
            {
                _q = q;
                _version = _q._version;
                _index = -1;
                _currentElement = default(T);
            }

            public void Dispose()
            {
                _index = -2;
                _currentElement = default(T);
            }

            public bool MoveNext()
            {
                if (_version != _q._version)
                    throw new InvalidOperationException("Collection was modified");

                if (_index == -2)
                    return false;

                _index++;

                if (_index == _q._size)
                {
                    _index = -2;
                    _currentElement = default(T);
                    return false;
                }

                _currentElement = _q.GetElement(_index);
                return true;
            }

            public T Current
            {
                get
                {
                    if (_index < 0)
                        throw new InvalidOperationException("Enumeration has not started. Call MoveNext.");
                    return _currentElement;
                }
            }

            [CanBeNull]
            object IEnumerator.Current
            {
                get
                {
                    if (_index < 0)
                        throw new InvalidOperationException("Enumeration has not started. Call MoveNext.");
                    return _currentElement;
                }
            }

            void IEnumerator.Reset()
            {
                if (_version != _q._version)
                    throw new InvalidOperationException("Collection was modified");

                _index = -1;
                _currentElement = default(T);
            }
        }
    }
}