










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
using System.Collections.Generic;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities
{


    #region Queue with 2 items.
    /// <summary>
    /// A queue of <c>Tuple&lt;T1, T2&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    [PublicAPI]
    public class Queue<T1, T2> : Queue<Tuple<T1, T2>>, IEnumerable<T1, T2>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2}"/> class that is empty 
        /// and has the specified initial capacity or the default initial capacity, whichever is greater.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        public Queue(int capacity = 4) : base(capacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2}"/> class that contains elements copied 
        /// from the specified collection and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection to copy elements from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public Queue([NotNull] IEnumerable<Tuple<T1, T2>> collection) : base(collection)
        {
        }

        /// <summary>
        /// Inserts an object at the beginning of the <see cref="Queue{T1, T2}" />.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        public void Enqueue(
            T1 item1,
            T2 item2)
        {
            Enqueue(new Tuple<T1, T2>(item1, item2));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        public void Dequeue(
            out T1 item1,
            out T2 item2)
        {
            Tuple<T1, T2> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        public void Peek(
            out T1 item1,
            out T2 item2)
        {
            Tuple<T1, T2> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryDequeue(
            out T1 item1,
            out T2 item2)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                return false;
            }
            Tuple<T1, T2> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            return true;
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryPeek(
            out T1 item1,
            out T2 item2)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                return false;
            }
            Tuple<T1, T2> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            return true;
        }
    }
    #endregion


    #region Queue with 3 items.
    /// <summary>
    /// A queue of <c>Tuple&lt;T1, T2, T3&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    [PublicAPI]
    public class Queue<T1, T2, T3> : Queue<Tuple<T1, T2, T3>>, IEnumerable<T1, T2, T3>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3}"/> class that is empty 
        /// and has the specified initial capacity or the default initial capacity, whichever is greater.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        public Queue(int capacity = 4) : base(capacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3}"/> class that contains elements copied 
        /// from the specified collection and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection to copy elements from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public Queue([NotNull] IEnumerable<Tuple<T1, T2, T3>> collection) : base(collection)
        {
        }

        /// <summary>
        /// Inserts an object at the beginning of the <see cref="Queue{T1, T2, T3}" />.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        public void Enqueue(
            T1 item1,
            T2 item2,
            T3 item3)
        {
            Enqueue(new Tuple<T1, T2, T3>(item1, item2, item3));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        public void Dequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3)
        {
            Tuple<T1, T2, T3> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        public void Peek(
            out T1 item1,
            out T2 item2,
            out T3 item3)
        {
            Tuple<T1, T2, T3> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryDequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                return false;
            }
            Tuple<T1, T2, T3> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            return true;
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryPeek(
            out T1 item1,
            out T2 item2,
            out T3 item3)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                return false;
            }
            Tuple<T1, T2, T3> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            return true;
        }
    }
    #endregion


    #region Queue with 4 items.
    /// <summary>
    /// A queue of <c>Tuple&lt;T1, T2, T3, T4&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    [PublicAPI]
    public class Queue<T1, T2, T3, T4> : Queue<Tuple<T1, T2, T3, T4>>, IEnumerable<T1, T2, T3, T4>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4}"/> class that is empty 
        /// and has the specified initial capacity or the default initial capacity, whichever is greater.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        public Queue(int capacity = 4) : base(capacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4}"/> class that contains elements copied 
        /// from the specified collection and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection to copy elements from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public Queue([NotNull] IEnumerable<Tuple<T1, T2, T3, T4>> collection) : base(collection)
        {
        }

        /// <summary>
        /// Inserts an object at the beginning of the <see cref="Queue{T1, T2, T3, T4}" />.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        public void Enqueue(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4)
        {
            Enqueue(new Tuple<T1, T2, T3, T4>(item1, item2, item3, item4));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        public void Dequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4)
        {
            Tuple<T1, T2, T3, T4> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        public void Peek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4)
        {
            Tuple<T1, T2, T3, T4> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryDequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                return false;
            }
            Tuple<T1, T2, T3, T4> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            return true;
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryPeek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                return false;
            }
            Tuple<T1, T2, T3, T4> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            return true;
        }
    }
    #endregion


    #region Queue with 5 items.
    /// <summary>
    /// A queue of <c>Tuple&lt;T1, T2, T3, T4, T5&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    [PublicAPI]
    public class Queue<T1, T2, T3, T4, T5> : Queue<Tuple<T1, T2, T3, T4, T5>>, IEnumerable<T1, T2, T3, T4, T5>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5}"/> class that is empty 
        /// and has the specified initial capacity or the default initial capacity, whichever is greater.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        public Queue(int capacity = 4) : base(capacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5}"/> class that contains elements copied 
        /// from the specified collection and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection to copy elements from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public Queue([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5>> collection) : base(collection)
        {
        }

        /// <summary>
        /// Inserts an object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5}" />.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        public void Enqueue(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4,
            T5 item5)
        {
            Enqueue(new Tuple<T1, T2, T3, T4, T5>(item1, item2, item3, item4, item5));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        public void Dequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5)
        {
            Tuple<T1, T2, T3, T4, T5> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        public void Peek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5)
        {
            Tuple<T1, T2, T3, T4, T5> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryDequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            return true;
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryPeek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            return true;
        }
    }
    #endregion


    #region Queue with 6 items.
    /// <summary>
    /// A queue of <c>Tuple&lt;T1, T2, T3, T4, T5, T6&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    [PublicAPI]
    public class Queue<T1, T2, T3, T4, T5, T6> : Queue<Tuple<T1, T2, T3, T4, T5, T6>>, IEnumerable<T1, T2, T3, T4, T5, T6>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6}"/> class that is empty 
        /// and has the specified initial capacity or the default initial capacity, whichever is greater.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        public Queue(int capacity = 4) : base(capacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6}"/> class that contains elements copied 
        /// from the specified collection and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection to copy elements from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public Queue([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6>> collection) : base(collection)
        {
        }

        /// <summary>
        /// Inserts an object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6}" />.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        public void Enqueue(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4,
            T5 item5,
            T6 item6)
        {
            Enqueue(new Tuple<T1, T2, T3, T4, T5, T6>(item1, item2, item3, item4, item5, item6));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        public void Dequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6)
        {
            Tuple<T1, T2, T3, T4, T5, T6> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        public void Peek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6)
        {
            Tuple<T1, T2, T3, T4, T5, T6> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryDequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            return true;
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryPeek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            return true;
        }
    }
    #endregion


    #region Queue with 7 items.
    /// <summary>
    /// A queue of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    [PublicAPI]
    public class Queue<T1, T2, T3, T4, T5, T6, T7> : Queue<Tuple<T1, T2, T3, T4, T5, T6, T7>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7}"/> class that is empty 
        /// and has the specified initial capacity or the default initial capacity, whichever is greater.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        public Queue(int capacity = 4) : base(capacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7}"/> class that contains elements copied 
        /// from the specified collection and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection to copy elements from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public Queue([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7>> collection) : base(collection)
        {
        }

        /// <summary>
        /// Inserts an object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7}" />.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        public void Enqueue(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4,
            T5 item5,
            T6 item6,
            T7 item7)
        {
            Enqueue(new Tuple<T1, T2, T3, T4, T5, T6, T7>(item1, item2, item3, item4, item5, item6, item7));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        public void Dequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        public void Peek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryDequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            return true;
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryPeek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            return true;
        }
    }
    #endregion


    #region Queue with 8 items.
    /// <summary>
    /// A queue of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    [PublicAPI]
    public class Queue<T1, T2, T3, T4, T5, T6, T7, T8> : Queue<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8}"/> class that is empty 
        /// and has the specified initial capacity or the default initial capacity, whichever is greater.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        public Queue(int capacity = 4) : base(capacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8}"/> class that contains elements copied 
        /// from the specified collection and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection to copy elements from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public Queue([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>>> collection) : base(collection)
        {
        }

        /// <summary>
        /// Inserts an object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8}" />.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        public void Enqueue(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4,
            T5 item5,
            T6 item6,
            T7 item7,
            T8 item8)
        {
            Enqueue(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8>(item8)));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        public void Dequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        public void Peek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryDequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryPeek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
    }
    #endregion


    #region Queue with 9 items.
    /// <summary>
    /// A queue of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8, T9&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    /// <typeparam name="T9">The type of item 9.</typeparam>
    [PublicAPI]
    public class Queue<T1, T2, T3, T4, T5, T6, T7, T8, T9> : Queue<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9}"/> class that is empty 
        /// and has the specified initial capacity or the default initial capacity, whichever is greater.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        public Queue(int capacity = 4) : base(capacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9}"/> class that contains elements copied 
        /// from the specified collection and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection to copy elements from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public Queue([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9>>> collection) : base(collection)
        {
        }

        /// <summary>
        /// Inserts an object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9}" />.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        public void Enqueue(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4,
            T5 item5,
            T6 item6,
            T7 item7,
            T8 item8,
            T9 item9)
        {
            Enqueue(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9>(item8, item9)));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        public void Dequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        public void Peek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryDequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                item9 = default(T9);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryPeek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                item9 = default(T9);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
    }
    #endregion


    #region Queue with 10 items.
    /// <summary>
    /// A queue of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8, T9, T10&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    /// <typeparam name="T9">The type of item 9.</typeparam>
    /// <typeparam name="T10">The type of item 10.</typeparam>
    [PublicAPI]
    public class Queue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : Queue<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10}"/> class that is empty 
        /// and has the specified initial capacity or the default initial capacity, whichever is greater.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        public Queue(int capacity = 4) : base(capacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10}"/> class that contains elements copied 
        /// from the specified collection and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection to copy elements from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public Queue([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10>>> collection) : base(collection)
        {
        }

        /// <summary>
        /// Inserts an object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10}" />.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        public void Enqueue(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4,
            T5 item5,
            T6 item6,
            T7 item7,
            T8 item8,
            T9 item9,
            T10 item10)
        {
            Enqueue(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10>(item8, item9, item10)));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        public void Dequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        public void Peek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryDequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                item9 = default(T9);
                item10 = default(T10);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryPeek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                item9 = default(T9);
                item10 = default(T10);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
    }
    #endregion


    #region Queue with 11 items.
    /// <summary>
    /// A queue of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8, T9, T10, T11&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    /// <typeparam name="T9">The type of item 9.</typeparam>
    /// <typeparam name="T10">The type of item 10.</typeparam>
    /// <typeparam name="T11">The type of item 11.</typeparam>
    [PublicAPI]
    public class Queue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : Queue<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11}"/> class that is empty 
        /// and has the specified initial capacity or the default initial capacity, whichever is greater.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        public Queue(int capacity = 4) : base(capacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11}"/> class that contains elements copied 
        /// from the specified collection and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection to copy elements from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public Queue([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11>>> collection) : base(collection)
        {
        }

        /// <summary>
        /// Inserts an object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11}" />.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        public void Enqueue(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4,
            T5 item5,
            T6 item6,
            T7 item7,
            T8 item8,
            T9 item9,
            T10 item10,
            T11 item11)
        {
            Enqueue(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11>(item8, item9, item10, item11)));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        public void Dequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        public void Peek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryDequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                item9 = default(T9);
                item10 = default(T10);
                item11 = default(T11);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryPeek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                item9 = default(T9);
                item10 = default(T10);
                item11 = default(T11);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
    }
    #endregion


    #region Queue with 12 items.
    /// <summary>
    /// A queue of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8, T9, T10, T11, T12&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    /// <typeparam name="T9">The type of item 9.</typeparam>
    /// <typeparam name="T10">The type of item 10.</typeparam>
    /// <typeparam name="T11">The type of item 11.</typeparam>
    /// <typeparam name="T12">The type of item 12.</typeparam>
    [PublicAPI]
    public class Queue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : Queue<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12}"/> class that is empty 
        /// and has the specified initial capacity or the default initial capacity, whichever is greater.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        public Queue(int capacity = 4) : base(capacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12}"/> class that contains elements copied 
        /// from the specified collection and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection to copy elements from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public Queue([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12>>> collection) : base(collection)
        {
        }

        /// <summary>
        /// Inserts an object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12}" />.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        public void Enqueue(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4,
            T5 item5,
            T6 item6,
            T7 item7,
            T8 item8,
            T9 item9,
            T10 item10,
            T11 item11,
            T12 item12)
        {
            Enqueue(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12>(item8, item9, item10, item11, item12)));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        public void Dequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        public void Peek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryDequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                item9 = default(T9);
                item10 = default(T10);
                item11 = default(T11);
                item12 = default(T12);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryPeek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                item9 = default(T9);
                item10 = default(T10);
                item11 = default(T11);
                item12 = default(T12);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
    }
    #endregion


    #region Queue with 13 items.
    /// <summary>
    /// A queue of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8, T9, T10, T11, T12, T13&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    /// <typeparam name="T9">The type of item 9.</typeparam>
    /// <typeparam name="T10">The type of item 10.</typeparam>
    /// <typeparam name="T11">The type of item 11.</typeparam>
    /// <typeparam name="T12">The type of item 12.</typeparam>
    /// <typeparam name="T13">The type of item 13.</typeparam>
    [PublicAPI]
    public class Queue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> : Queue<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13}"/> class that is empty 
        /// and has the specified initial capacity or the default initial capacity, whichever is greater.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        public Queue(int capacity = 4) : base(capacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13}"/> class that contains elements copied 
        /// from the specified collection and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection to copy elements from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public Queue([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13>>> collection) : base(collection)
        {
        }

        /// <summary>
        /// Inserts an object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13}" />.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        public void Enqueue(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4,
            T5 item5,
            T6 item6,
            T7 item7,
            T8 item8,
            T9 item9,
            T10 item10,
            T11 item11,
            T12 item12,
            T13 item13)
        {
            Enqueue(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13>(item8, item9, item10, item11, item12, item13)));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        public void Dequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        public void Peek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryDequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                item9 = default(T9);
                item10 = default(T10);
                item11 = default(T11);
                item12 = default(T12);
                item13 = default(T13);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryPeek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                item9 = default(T9);
                item10 = default(T10);
                item11 = default(T11);
                item12 = default(T12);
                item13 = default(T13);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
    }
    #endregion


    #region Queue with 14 items.
    /// <summary>
    /// A queue of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8, T9, T10, T11, T12, T13, T14&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    /// <typeparam name="T9">The type of item 9.</typeparam>
    /// <typeparam name="T10">The type of item 10.</typeparam>
    /// <typeparam name="T11">The type of item 11.</typeparam>
    /// <typeparam name="T12">The type of item 12.</typeparam>
    /// <typeparam name="T13">The type of item 13.</typeparam>
    /// <typeparam name="T14">The type of item 14.</typeparam>
    [PublicAPI]
    public class Queue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> : Queue<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14}"/> class that is empty 
        /// and has the specified initial capacity or the default initial capacity, whichever is greater.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        public Queue(int capacity = 4) : base(capacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14}"/> class that contains elements copied 
        /// from the specified collection and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection to copy elements from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public Queue([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14>>> collection) : base(collection)
        {
        }

        /// <summary>
        /// Inserts an object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14}" />.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        public void Enqueue(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4,
            T5 item5,
            T6 item6,
            T7 item7,
            T8 item8,
            T9 item9,
            T10 item10,
            T11 item11,
            T12 item12,
            T13 item13,
            T14 item14)
        {
            Enqueue(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14>(item8, item9, item10, item11, item12, item13, item14)));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        public void Dequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        public void Peek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryDequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                item9 = default(T9);
                item10 = default(T10);
                item11 = default(T11);
                item12 = default(T12);
                item13 = default(T13);
                item14 = default(T14);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryPeek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                item9 = default(T9);
                item10 = default(T10);
                item11 = default(T11);
                item12 = default(T12);
                item13 = default(T13);
                item14 = default(T14);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
    }
    #endregion


    #region Queue with 15 items.
    /// <summary>
    /// A queue of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8, T9, T10, T11, T12, T13, T14, Tuple&lt;T15&gt;&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    /// <typeparam name="T9">The type of item 9.</typeparam>
    /// <typeparam name="T10">The type of item 10.</typeparam>
    /// <typeparam name="T11">The type of item 11.</typeparam>
    /// <typeparam name="T12">The type of item 12.</typeparam>
    /// <typeparam name="T13">The type of item 13.</typeparam>
    /// <typeparam name="T14">The type of item 14.</typeparam>
    /// <typeparam name="T15">The type of item 15.</typeparam>
    [PublicAPI]
    public class Queue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> : Queue<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15>>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15}"/> class that is empty 
        /// and has the specified initial capacity or the default initial capacity, whichever is greater.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        public Queue(int capacity = 4) : base(capacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15}"/> class that contains elements copied 
        /// from the specified collection and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection to copy elements from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public Queue([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15>>>> collection) : base(collection)
        {
        }

        /// <summary>
        /// Inserts an object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15}" />.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        public void Enqueue(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4,
            T5 item5,
            T6 item6,
            T7 item7,
            T8 item8,
            T9 item9,
            T10 item10,
            T11 item11,
            T12 item12,
            T13 item13,
            T14 item14,
            T15 item15)
        {
            Enqueue(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15>(item15))));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        public void Dequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15>>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        public void Peek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15>>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryDequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                item9 = default(T9);
                item10 = default(T10);
                item11 = default(T11);
                item12 = default(T12);
                item13 = default(T13);
                item14 = default(T14);
                item15 = default(T15);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15>>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryPeek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                item9 = default(T9);
                item10 = default(T10);
                item11 = default(T11);
                item12 = default(T12);
                item13 = default(T13);
                item14 = default(T14);
                item15 = default(T15);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15>>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
    }
    #endregion


    #region Queue with 16 items.
    /// <summary>
    /// A queue of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8, T9, T10, T11, T12, T13, T14, Tuple&lt;T15, T16&gt;&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    /// <typeparam name="T9">The type of item 9.</typeparam>
    /// <typeparam name="T10">The type of item 10.</typeparam>
    /// <typeparam name="T11">The type of item 11.</typeparam>
    /// <typeparam name="T12">The type of item 12.</typeparam>
    /// <typeparam name="T13">The type of item 13.</typeparam>
    /// <typeparam name="T14">The type of item 14.</typeparam>
    /// <typeparam name="T15">The type of item 15.</typeparam>
    /// <typeparam name="T16">The type of item 16.</typeparam>
    [PublicAPI]
    public class Queue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> : Queue<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16>>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16}"/> class that is empty 
        /// and has the specified initial capacity or the default initial capacity, whichever is greater.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        public Queue(int capacity = 4) : base(capacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16}"/> class that contains elements copied 
        /// from the specified collection and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection to copy elements from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public Queue([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16>>>> collection) : base(collection)
        {
        }

        /// <summary>
        /// Inserts an object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16}" />.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        public void Enqueue(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4,
            T5 item5,
            T6 item6,
            T7 item7,
            T8 item8,
            T9 item9,
            T10 item10,
            T11 item11,
            T12 item12,
            T13 item13,
            T14 item14,
            T15 item15,
            T16 item16)
        {
            Enqueue(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16>(item15, item16))));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        public void Dequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16>>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        public void Peek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16>>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryDequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                item9 = default(T9);
                item10 = default(T10);
                item11 = default(T11);
                item12 = default(T12);
                item13 = default(T13);
                item14 = default(T14);
                item15 = default(T15);
                item16 = default(T16);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16>>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryPeek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                item9 = default(T9);
                item10 = default(T10);
                item11 = default(T11);
                item12 = default(T12);
                item13 = default(T13);
                item14 = default(T14);
                item15 = default(T15);
                item16 = default(T16);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16>>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
    }
    #endregion


    #region Queue with 17 items.
    /// <summary>
    /// A queue of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8, T9, T10, T11, T12, T13, T14, Tuple&lt;T15, T16, T17&gt;&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    /// <typeparam name="T9">The type of item 9.</typeparam>
    /// <typeparam name="T10">The type of item 10.</typeparam>
    /// <typeparam name="T11">The type of item 11.</typeparam>
    /// <typeparam name="T12">The type of item 12.</typeparam>
    /// <typeparam name="T13">The type of item 13.</typeparam>
    /// <typeparam name="T14">The type of item 14.</typeparam>
    /// <typeparam name="T15">The type of item 15.</typeparam>
    /// <typeparam name="T16">The type of item 16.</typeparam>
    /// <typeparam name="T17">The type of item 17.</typeparam>
    [PublicAPI]
    public class Queue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17> : Queue<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17>>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17}"/> class that is empty 
        /// and has the specified initial capacity or the default initial capacity, whichever is greater.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        public Queue(int capacity = 4) : base(capacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17}"/> class that contains elements copied 
        /// from the specified collection and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection to copy elements from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public Queue([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17>>>> collection) : base(collection)
        {
        }

        /// <summary>
        /// Inserts an object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17}" />.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        public void Enqueue(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4,
            T5 item5,
            T6 item6,
            T7 item7,
            T8 item8,
            T9 item9,
            T10 item10,
            T11 item11,
            T12 item12,
            T13 item13,
            T14 item14,
            T15 item15,
            T16 item16,
            T17 item17)
        {
            Enqueue(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17>(item15, item16, item17))));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        public void Dequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17>>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        public void Peek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17>>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryDequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                item9 = default(T9);
                item10 = default(T10);
                item11 = default(T11);
                item12 = default(T12);
                item13 = default(T13);
                item14 = default(T14);
                item15 = default(T15);
                item16 = default(T16);
                item17 = default(T17);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17>>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryPeek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                item9 = default(T9);
                item10 = default(T10);
                item11 = default(T11);
                item12 = default(T12);
                item13 = default(T13);
                item14 = default(T14);
                item15 = default(T15);
                item16 = default(T16);
                item17 = default(T17);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17>>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
    }
    #endregion


    #region Queue with 18 items.
    /// <summary>
    /// A queue of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8, T9, T10, T11, T12, T13, T14, Tuple&lt;T15, T16, T17, T18&gt;&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    /// <typeparam name="T9">The type of item 9.</typeparam>
    /// <typeparam name="T10">The type of item 10.</typeparam>
    /// <typeparam name="T11">The type of item 11.</typeparam>
    /// <typeparam name="T12">The type of item 12.</typeparam>
    /// <typeparam name="T13">The type of item 13.</typeparam>
    /// <typeparam name="T14">The type of item 14.</typeparam>
    /// <typeparam name="T15">The type of item 15.</typeparam>
    /// <typeparam name="T16">The type of item 16.</typeparam>
    /// <typeparam name="T17">The type of item 17.</typeparam>
    /// <typeparam name="T18">The type of item 18.</typeparam>
    [PublicAPI]
    public class Queue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18> : Queue<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18>>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18}"/> class that is empty 
        /// and has the specified initial capacity or the default initial capacity, whichever is greater.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        public Queue(int capacity = 4) : base(capacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18}"/> class that contains elements copied 
        /// from the specified collection and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection to copy elements from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public Queue([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18>>>> collection) : base(collection)
        {
        }

        /// <summary>
        /// Inserts an object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18}" />.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        public void Enqueue(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4,
            T5 item5,
            T6 item6,
            T7 item7,
            T8 item8,
            T9 item9,
            T10 item10,
            T11 item11,
            T12 item12,
            T13 item13,
            T14 item14,
            T15 item15,
            T16 item16,
            T17 item17,
            T18 item18)
        {
            Enqueue(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17, T18>(item15, item16, item17, item18))));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        public void Dequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18>>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        public void Peek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18>>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryDequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                item9 = default(T9);
                item10 = default(T10);
                item11 = default(T11);
                item12 = default(T12);
                item13 = default(T13);
                item14 = default(T14);
                item15 = default(T15);
                item16 = default(T16);
                item17 = default(T17);
                item18 = default(T18);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18>>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryPeek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                item9 = default(T9);
                item10 = default(T10);
                item11 = default(T11);
                item12 = default(T12);
                item13 = default(T13);
                item14 = default(T14);
                item15 = default(T15);
                item16 = default(T16);
                item17 = default(T17);
                item18 = default(T18);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18>>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
    }
    #endregion


    #region Queue with 19 items.
    /// <summary>
    /// A queue of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8, T9, T10, T11, T12, T13, T14, Tuple&lt;T15, T16, T17, T18, T19&gt;&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    /// <typeparam name="T9">The type of item 9.</typeparam>
    /// <typeparam name="T10">The type of item 10.</typeparam>
    /// <typeparam name="T11">The type of item 11.</typeparam>
    /// <typeparam name="T12">The type of item 12.</typeparam>
    /// <typeparam name="T13">The type of item 13.</typeparam>
    /// <typeparam name="T14">The type of item 14.</typeparam>
    /// <typeparam name="T15">The type of item 15.</typeparam>
    /// <typeparam name="T16">The type of item 16.</typeparam>
    /// <typeparam name="T17">The type of item 17.</typeparam>
    /// <typeparam name="T18">The type of item 18.</typeparam>
    /// <typeparam name="T19">The type of item 19.</typeparam>
    [PublicAPI]
    public class Queue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19> : Queue<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19>>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19}"/> class that is empty 
        /// and has the specified initial capacity or the default initial capacity, whichever is greater.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        public Queue(int capacity = 4) : base(capacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19}"/> class that contains elements copied 
        /// from the specified collection and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection to copy elements from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public Queue([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19>>>> collection) : base(collection)
        {
        }

        /// <summary>
        /// Inserts an object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19}" />.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        public void Enqueue(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4,
            T5 item5,
            T6 item6,
            T7 item7,
            T8 item8,
            T9 item9,
            T10 item10,
            T11 item11,
            T12 item12,
            T13 item13,
            T14 item14,
            T15 item15,
            T16 item16,
            T17 item17,
            T18 item18,
            T19 item19)
        {
            Enqueue(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17, T18, T19>(item15, item16, item17, item18, item19))));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        public void Dequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19>>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        public void Peek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19>>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryDequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                item9 = default(T9);
                item10 = default(T10);
                item11 = default(T11);
                item12 = default(T12);
                item13 = default(T13);
                item14 = default(T14);
                item15 = default(T15);
                item16 = default(T16);
                item17 = default(T17);
                item18 = default(T18);
                item19 = default(T19);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19>>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryPeek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                item9 = default(T9);
                item10 = default(T10);
                item11 = default(T11);
                item12 = default(T12);
                item13 = default(T13);
                item14 = default(T14);
                item15 = default(T15);
                item16 = default(T16);
                item17 = default(T17);
                item18 = default(T18);
                item19 = default(T19);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19>>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
    }
    #endregion


    #region Queue with 20 items.
    /// <summary>
    /// A queue of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8, T9, T10, T11, T12, T13, T14, Tuple&lt;T15, T16, T17, T18, T19, T20&gt;&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    /// <typeparam name="T9">The type of item 9.</typeparam>
    /// <typeparam name="T10">The type of item 10.</typeparam>
    /// <typeparam name="T11">The type of item 11.</typeparam>
    /// <typeparam name="T12">The type of item 12.</typeparam>
    /// <typeparam name="T13">The type of item 13.</typeparam>
    /// <typeparam name="T14">The type of item 14.</typeparam>
    /// <typeparam name="T15">The type of item 15.</typeparam>
    /// <typeparam name="T16">The type of item 16.</typeparam>
    /// <typeparam name="T17">The type of item 17.</typeparam>
    /// <typeparam name="T18">The type of item 18.</typeparam>
    /// <typeparam name="T19">The type of item 19.</typeparam>
    /// <typeparam name="T20">The type of item 20.</typeparam>
    [PublicAPI]
    public class Queue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20> : Queue<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20>>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20}"/> class that is empty 
        /// and has the specified initial capacity or the default initial capacity, whichever is greater.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        public Queue(int capacity = 4) : base(capacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20}"/> class that contains elements copied 
        /// from the specified collection and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection to copy elements from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public Queue([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20>>>> collection) : base(collection)
        {
        }

        /// <summary>
        /// Inserts an object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20}" />.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        public void Enqueue(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4,
            T5 item5,
            T6 item6,
            T7 item7,
            T8 item8,
            T9 item9,
            T10 item10,
            T11 item11,
            T12 item12,
            T13 item13,
            T14 item14,
            T15 item15,
            T16 item16,
            T17 item17,
            T18 item18,
            T19 item19,
            T20 item20)
        {
            Enqueue(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17, T18, T19, T20>(item15, item16, item17, item18, item19, item20))));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        public void Dequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20>>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        public void Peek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20>>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryDequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                item9 = default(T9);
                item10 = default(T10);
                item11 = default(T11);
                item12 = default(T12);
                item13 = default(T13);
                item14 = default(T14);
                item15 = default(T15);
                item16 = default(T16);
                item17 = default(T17);
                item18 = default(T18);
                item19 = default(T19);
                item20 = default(T20);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20>>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryPeek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                item9 = default(T9);
                item10 = default(T10);
                item11 = default(T11);
                item12 = default(T12);
                item13 = default(T13);
                item14 = default(T14);
                item15 = default(T15);
                item16 = default(T16);
                item17 = default(T17);
                item18 = default(T18);
                item19 = default(T19);
                item20 = default(T20);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20>>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
    }
    #endregion


    #region Queue with 21 items.
    /// <summary>
    /// A queue of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8, T9, T10, T11, T12, T13, T14, Tuple&lt;T15, T16, T17, T18, T19, T20, T21&gt;&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    /// <typeparam name="T9">The type of item 9.</typeparam>
    /// <typeparam name="T10">The type of item 10.</typeparam>
    /// <typeparam name="T11">The type of item 11.</typeparam>
    /// <typeparam name="T12">The type of item 12.</typeparam>
    /// <typeparam name="T13">The type of item 13.</typeparam>
    /// <typeparam name="T14">The type of item 14.</typeparam>
    /// <typeparam name="T15">The type of item 15.</typeparam>
    /// <typeparam name="T16">The type of item 16.</typeparam>
    /// <typeparam name="T17">The type of item 17.</typeparam>
    /// <typeparam name="T18">The type of item 18.</typeparam>
    /// <typeparam name="T19">The type of item 19.</typeparam>
    /// <typeparam name="T20">The type of item 20.</typeparam>
    /// <typeparam name="T21">The type of item 21.</typeparam>
    [PublicAPI]
    public class Queue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21> : Queue<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21>>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21}"/> class that is empty 
        /// and has the specified initial capacity or the default initial capacity, whichever is greater.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        public Queue(int capacity = 4) : base(capacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21}"/> class that contains elements copied 
        /// from the specified collection and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection to copy elements from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public Queue([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21>>>> collection) : base(collection)
        {
        }

        /// <summary>
        /// Inserts an object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21}" />.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        public void Enqueue(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4,
            T5 item5,
            T6 item6,
            T7 item7,
            T8 item8,
            T9 item9,
            T10 item10,
            T11 item11,
            T12 item12,
            T13 item13,
            T14 item14,
            T15 item15,
            T16 item16,
            T17 item17,
            T18 item18,
            T19 item19,
            T20 item20,
            T21 item21)
        {
            Enqueue(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17, T18, T19, T20, T21>(item15, item16, item17, item18, item19, item20, item21))));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        public void Dequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21>>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        public void Peek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21>>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryDequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                item9 = default(T9);
                item10 = default(T10);
                item11 = default(T11);
                item12 = default(T12);
                item13 = default(T13);
                item14 = default(T14);
                item15 = default(T15);
                item16 = default(T16);
                item17 = default(T17);
                item18 = default(T18);
                item19 = default(T19);
                item20 = default(T20);
                item21 = default(T21);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21>>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryPeek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                item9 = default(T9);
                item10 = default(T10);
                item11 = default(T11);
                item12 = default(T12);
                item13 = default(T13);
                item14 = default(T14);
                item15 = default(T15);
                item16 = default(T16);
                item17 = default(T17);
                item18 = default(T18);
                item19 = default(T19);
                item20 = default(T20);
                item21 = default(T21);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21>>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
    }
    #endregion


    #region Queue with 22 items.
    /// <summary>
    /// A queue of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8, T9, T10, T11, T12, T13, T14, Tuple&lt;T15, T16, T17, T18, T19, T20, T21, Tuple&lt;T22&gt;&gt;&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    /// <typeparam name="T9">The type of item 9.</typeparam>
    /// <typeparam name="T10">The type of item 10.</typeparam>
    /// <typeparam name="T11">The type of item 11.</typeparam>
    /// <typeparam name="T12">The type of item 12.</typeparam>
    /// <typeparam name="T13">The type of item 13.</typeparam>
    /// <typeparam name="T14">The type of item 14.</typeparam>
    /// <typeparam name="T15">The type of item 15.</typeparam>
    /// <typeparam name="T16">The type of item 16.</typeparam>
    /// <typeparam name="T17">The type of item 17.</typeparam>
    /// <typeparam name="T18">The type of item 18.</typeparam>
    /// <typeparam name="T19">The type of item 19.</typeparam>
    /// <typeparam name="T20">The type of item 20.</typeparam>
    /// <typeparam name="T21">The type of item 21.</typeparam>
    /// <typeparam name="T22">The type of item 22.</typeparam>
    [PublicAPI]
    public class Queue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22> : Queue<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22>>>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22}"/> class that is empty 
        /// and has the specified initial capacity or the default initial capacity, whichever is greater.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        public Queue(int capacity = 4) : base(capacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22}"/> class that contains elements copied 
        /// from the specified collection and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection to copy elements from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public Queue([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22>>>>> collection) : base(collection)
        {
        }

        /// <summary>
        /// Inserts an object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22}" />.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        public void Enqueue(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4,
            T5 item5,
            T6 item6,
            T7 item7,
            T8 item8,
            T9 item9,
            T10 item10,
            T11 item11,
            T12 item12,
            T13 item13,
            T14 item14,
            T15 item15,
            T16 item16,
            T17 item17,
            T18 item18,
            T19 item19,
            T20 item20,
            T21 item21,
            T22 item22)
        {
            Enqueue(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22>>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22>>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22>>(item15, item16, item17, item18, item19, item20, item21, new Tuple<T22>(item22)))));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        public void Dequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22>>>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        public void Peek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22>>>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryDequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                item9 = default(T9);
                item10 = default(T10);
                item11 = default(T11);
                item12 = default(T12);
                item13 = default(T13);
                item14 = default(T14);
                item15 = default(T15);
                item16 = default(T16);
                item17 = default(T17);
                item18 = default(T18);
                item19 = default(T19);
                item20 = default(T20);
                item21 = default(T21);
                item22 = default(T22);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22>>>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryPeek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                item9 = default(T9);
                item10 = default(T10);
                item11 = default(T11);
                item12 = default(T12);
                item13 = default(T13);
                item14 = default(T14);
                item15 = default(T15);
                item16 = default(T16);
                item17 = default(T17);
                item18 = default(T18);
                item19 = default(T19);
                item20 = default(T20);
                item21 = default(T21);
                item22 = default(T22);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22>>>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
    }
    #endregion


    #region Queue with 23 items.
    /// <summary>
    /// A queue of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8, T9, T10, T11, T12, T13, T14, Tuple&lt;T15, T16, T17, T18, T19, T20, T21, Tuple&lt;T22, T23&gt;&gt;&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    /// <typeparam name="T9">The type of item 9.</typeparam>
    /// <typeparam name="T10">The type of item 10.</typeparam>
    /// <typeparam name="T11">The type of item 11.</typeparam>
    /// <typeparam name="T12">The type of item 12.</typeparam>
    /// <typeparam name="T13">The type of item 13.</typeparam>
    /// <typeparam name="T14">The type of item 14.</typeparam>
    /// <typeparam name="T15">The type of item 15.</typeparam>
    /// <typeparam name="T16">The type of item 16.</typeparam>
    /// <typeparam name="T17">The type of item 17.</typeparam>
    /// <typeparam name="T18">The type of item 18.</typeparam>
    /// <typeparam name="T19">The type of item 19.</typeparam>
    /// <typeparam name="T20">The type of item 20.</typeparam>
    /// <typeparam name="T21">The type of item 21.</typeparam>
    /// <typeparam name="T22">The type of item 22.</typeparam>
    /// <typeparam name="T23">The type of item 23.</typeparam>
    [PublicAPI]
    public class Queue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23> : Queue<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23>>>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23}"/> class that is empty 
        /// and has the specified initial capacity or the default initial capacity, whichever is greater.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        public Queue(int capacity = 4) : base(capacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23}"/> class that contains elements copied 
        /// from the specified collection and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection to copy elements from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public Queue([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23>>>>> collection) : base(collection)
        {
        }

        /// <summary>
        /// Inserts an object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23}" />.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        public void Enqueue(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4,
            T5 item5,
            T6 item6,
            T7 item7,
            T8 item8,
            T9 item9,
            T10 item10,
            T11 item11,
            T12 item12,
            T13 item13,
            T14 item14,
            T15 item15,
            T16 item16,
            T17 item17,
            T18 item18,
            T19 item19,
            T20 item20,
            T21 item21,
            T22 item22,
            T23 item23)
        {
            Enqueue(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23>>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23>>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23>>(item15, item16, item17, item18, item19, item20, item21, new Tuple<T22, T23>(item22, item23)))));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        public void Dequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22,
            out T23 item23)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23>>>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            item23 = tuple.Rest.Rest.Rest.Item2;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        public void Peek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22,
            out T23 item23)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23>>>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            item23 = tuple.Rest.Rest.Rest.Item2;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryDequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22,
            out T23 item23)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                item9 = default(T9);
                item10 = default(T10);
                item11 = default(T11);
                item12 = default(T12);
                item13 = default(T13);
                item14 = default(T14);
                item15 = default(T15);
                item16 = default(T16);
                item17 = default(T17);
                item18 = default(T18);
                item19 = default(T19);
                item20 = default(T20);
                item21 = default(T21);
                item22 = default(T22);
                item23 = default(T23);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23>>>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            item23 = tuple.Rest.Rest.Rest.Item2;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryPeek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22,
            out T23 item23)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                item9 = default(T9);
                item10 = default(T10);
                item11 = default(T11);
                item12 = default(T12);
                item13 = default(T13);
                item14 = default(T14);
                item15 = default(T15);
                item16 = default(T16);
                item17 = default(T17);
                item18 = default(T18);
                item19 = default(T19);
                item20 = default(T20);
                item21 = default(T21);
                item22 = default(T22);
                item23 = default(T23);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23>>>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            item23 = tuple.Rest.Rest.Rest.Item2;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
    }
    #endregion


    #region Queue with 24 items.
    /// <summary>
    /// A queue of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8, T9, T10, T11, T12, T13, T14, Tuple&lt;T15, T16, T17, T18, T19, T20, T21, Tuple&lt;T22, T23, T24&gt;&gt;&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    /// <typeparam name="T9">The type of item 9.</typeparam>
    /// <typeparam name="T10">The type of item 10.</typeparam>
    /// <typeparam name="T11">The type of item 11.</typeparam>
    /// <typeparam name="T12">The type of item 12.</typeparam>
    /// <typeparam name="T13">The type of item 13.</typeparam>
    /// <typeparam name="T14">The type of item 14.</typeparam>
    /// <typeparam name="T15">The type of item 15.</typeparam>
    /// <typeparam name="T16">The type of item 16.</typeparam>
    /// <typeparam name="T17">The type of item 17.</typeparam>
    /// <typeparam name="T18">The type of item 18.</typeparam>
    /// <typeparam name="T19">The type of item 19.</typeparam>
    /// <typeparam name="T20">The type of item 20.</typeparam>
    /// <typeparam name="T21">The type of item 21.</typeparam>
    /// <typeparam name="T22">The type of item 22.</typeparam>
    /// <typeparam name="T23">The type of item 23.</typeparam>
    /// <typeparam name="T24">The type of item 24.</typeparam>
    [PublicAPI]
    public class Queue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24> : Queue<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24>>>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24}"/> class that is empty 
        /// and has the specified initial capacity or the default initial capacity, whichever is greater.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        public Queue(int capacity = 4) : base(capacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24}"/> class that contains elements copied 
        /// from the specified collection and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection to copy elements from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public Queue([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24>>>>> collection) : base(collection)
        {
        }

        /// <summary>
        /// Inserts an object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24}" />.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        public void Enqueue(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4,
            T5 item5,
            T6 item6,
            T7 item7,
            T8 item8,
            T9 item9,
            T10 item10,
            T11 item11,
            T12 item12,
            T13 item13,
            T14 item14,
            T15 item15,
            T16 item16,
            T17 item17,
            T18 item18,
            T19 item19,
            T20 item20,
            T21 item21,
            T22 item22,
            T23 item23,
            T24 item24)
        {
            Enqueue(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24>>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24>>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24>>(item15, item16, item17, item18, item19, item20, item21, new Tuple<T22, T23, T24>(item22, item23, item24)))));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        public void Dequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22,
            out T23 item23,
            out T24 item24)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24>>>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            item23 = tuple.Rest.Rest.Rest.Item2;
            item24 = tuple.Rest.Rest.Rest.Item3;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        public void Peek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22,
            out T23 item23,
            out T24 item24)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24>>>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            item23 = tuple.Rest.Rest.Rest.Item2;
            item24 = tuple.Rest.Rest.Rest.Item3;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryDequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22,
            out T23 item23,
            out T24 item24)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                item9 = default(T9);
                item10 = default(T10);
                item11 = default(T11);
                item12 = default(T12);
                item13 = default(T13);
                item14 = default(T14);
                item15 = default(T15);
                item16 = default(T16);
                item17 = default(T17);
                item18 = default(T18);
                item19 = default(T19);
                item20 = default(T20);
                item21 = default(T21);
                item22 = default(T22);
                item23 = default(T23);
                item24 = default(T24);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24>>>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            item23 = tuple.Rest.Rest.Rest.Item2;
            item24 = tuple.Rest.Rest.Rest.Item3;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryPeek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22,
            out T23 item23,
            out T24 item24)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                item9 = default(T9);
                item10 = default(T10);
                item11 = default(T11);
                item12 = default(T12);
                item13 = default(T13);
                item14 = default(T14);
                item15 = default(T15);
                item16 = default(T16);
                item17 = default(T17);
                item18 = default(T18);
                item19 = default(T19);
                item20 = default(T20);
                item21 = default(T21);
                item22 = default(T22);
                item23 = default(T23);
                item24 = default(T24);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24>>>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            item23 = tuple.Rest.Rest.Rest.Item2;
            item24 = tuple.Rest.Rest.Rest.Item3;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
    }
    #endregion


    #region Queue with 25 items.
    /// <summary>
    /// A queue of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8, T9, T10, T11, T12, T13, T14, Tuple&lt;T15, T16, T17, T18, T19, T20, T21, Tuple&lt;T22, T23, T24, T25&gt;&gt;&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    /// <typeparam name="T9">The type of item 9.</typeparam>
    /// <typeparam name="T10">The type of item 10.</typeparam>
    /// <typeparam name="T11">The type of item 11.</typeparam>
    /// <typeparam name="T12">The type of item 12.</typeparam>
    /// <typeparam name="T13">The type of item 13.</typeparam>
    /// <typeparam name="T14">The type of item 14.</typeparam>
    /// <typeparam name="T15">The type of item 15.</typeparam>
    /// <typeparam name="T16">The type of item 16.</typeparam>
    /// <typeparam name="T17">The type of item 17.</typeparam>
    /// <typeparam name="T18">The type of item 18.</typeparam>
    /// <typeparam name="T19">The type of item 19.</typeparam>
    /// <typeparam name="T20">The type of item 20.</typeparam>
    /// <typeparam name="T21">The type of item 21.</typeparam>
    /// <typeparam name="T22">The type of item 22.</typeparam>
    /// <typeparam name="T23">The type of item 23.</typeparam>
    /// <typeparam name="T24">The type of item 24.</typeparam>
    /// <typeparam name="T25">The type of item 25.</typeparam>
    [PublicAPI]
    public class Queue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25> : Queue<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25>>>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25}"/> class that is empty 
        /// and has the specified initial capacity or the default initial capacity, whichever is greater.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        public Queue(int capacity = 4) : base(capacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25}"/> class that contains elements copied 
        /// from the specified collection and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection to copy elements from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public Queue([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25>>>>> collection) : base(collection)
        {
        }

        /// <summary>
        /// Inserts an object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25}" />.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        public void Enqueue(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4,
            T5 item5,
            T6 item6,
            T7 item7,
            T8 item8,
            T9 item9,
            T10 item10,
            T11 item11,
            T12 item12,
            T13 item13,
            T14 item14,
            T15 item15,
            T16 item16,
            T17 item17,
            T18 item18,
            T19 item19,
            T20 item20,
            T21 item21,
            T22 item22,
            T23 item23,
            T24 item24,
            T25 item25)
        {
            Enqueue(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25>>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25>>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25>>(item15, item16, item17, item18, item19, item20, item21, new Tuple<T22, T23, T24, T25>(item22, item23, item24, item25)))));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        public void Dequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22,
            out T23 item23,
            out T24 item24,
            out T25 item25)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25>>>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            item23 = tuple.Rest.Rest.Rest.Item2;
            item24 = tuple.Rest.Rest.Rest.Item3;
            item25 = tuple.Rest.Rest.Rest.Item4;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        public void Peek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22,
            out T23 item23,
            out T24 item24,
            out T25 item25)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25>>>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            item23 = tuple.Rest.Rest.Rest.Item2;
            item24 = tuple.Rest.Rest.Rest.Item3;
            item25 = tuple.Rest.Rest.Rest.Item4;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryDequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22,
            out T23 item23,
            out T24 item24,
            out T25 item25)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                item9 = default(T9);
                item10 = default(T10);
                item11 = default(T11);
                item12 = default(T12);
                item13 = default(T13);
                item14 = default(T14);
                item15 = default(T15);
                item16 = default(T16);
                item17 = default(T17);
                item18 = default(T18);
                item19 = default(T19);
                item20 = default(T20);
                item21 = default(T21);
                item22 = default(T22);
                item23 = default(T23);
                item24 = default(T24);
                item25 = default(T25);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25>>>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            item23 = tuple.Rest.Rest.Rest.Item2;
            item24 = tuple.Rest.Rest.Rest.Item3;
            item25 = tuple.Rest.Rest.Rest.Item4;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryPeek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22,
            out T23 item23,
            out T24 item24,
            out T25 item25)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                item9 = default(T9);
                item10 = default(T10);
                item11 = default(T11);
                item12 = default(T12);
                item13 = default(T13);
                item14 = default(T14);
                item15 = default(T15);
                item16 = default(T16);
                item17 = default(T17);
                item18 = default(T18);
                item19 = default(T19);
                item20 = default(T20);
                item21 = default(T21);
                item22 = default(T22);
                item23 = default(T23);
                item24 = default(T24);
                item25 = default(T25);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25>>>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            item23 = tuple.Rest.Rest.Rest.Item2;
            item24 = tuple.Rest.Rest.Rest.Item3;
            item25 = tuple.Rest.Rest.Rest.Item4;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
    }
    #endregion


    #region Queue with 26 items.
    /// <summary>
    /// A queue of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8, T9, T10, T11, T12, T13, T14, Tuple&lt;T15, T16, T17, T18, T19, T20, T21, Tuple&lt;T22, T23, T24, T25, T26&gt;&gt;&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    /// <typeparam name="T9">The type of item 9.</typeparam>
    /// <typeparam name="T10">The type of item 10.</typeparam>
    /// <typeparam name="T11">The type of item 11.</typeparam>
    /// <typeparam name="T12">The type of item 12.</typeparam>
    /// <typeparam name="T13">The type of item 13.</typeparam>
    /// <typeparam name="T14">The type of item 14.</typeparam>
    /// <typeparam name="T15">The type of item 15.</typeparam>
    /// <typeparam name="T16">The type of item 16.</typeparam>
    /// <typeparam name="T17">The type of item 17.</typeparam>
    /// <typeparam name="T18">The type of item 18.</typeparam>
    /// <typeparam name="T19">The type of item 19.</typeparam>
    /// <typeparam name="T20">The type of item 20.</typeparam>
    /// <typeparam name="T21">The type of item 21.</typeparam>
    /// <typeparam name="T22">The type of item 22.</typeparam>
    /// <typeparam name="T23">The type of item 23.</typeparam>
    /// <typeparam name="T24">The type of item 24.</typeparam>
    /// <typeparam name="T25">The type of item 25.</typeparam>
    /// <typeparam name="T26">The type of item 26.</typeparam>
    [PublicAPI]
    public class Queue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26> : Queue<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26>>>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26}"/> class that is empty 
        /// and has the specified initial capacity or the default initial capacity, whichever is greater.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        public Queue(int capacity = 4) : base(capacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26}"/> class that contains elements copied 
        /// from the specified collection and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection to copy elements from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public Queue([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26>>>>> collection) : base(collection)
        {
        }

        /// <summary>
        /// Inserts an object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26}" />.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        /// <param name="item26">Item 26 of the tuple.</param>
        public void Enqueue(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4,
            T5 item5,
            T6 item6,
            T7 item7,
            T8 item8,
            T9 item9,
            T10 item10,
            T11 item11,
            T12 item12,
            T13 item13,
            T14 item14,
            T15 item15,
            T16 item16,
            T17 item17,
            T18 item18,
            T19 item19,
            T20 item20,
            T21 item21,
            T22 item22,
            T23 item23,
            T24 item24,
            T25 item25,
            T26 item26)
        {
            Enqueue(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26>>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26>>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26>>(item15, item16, item17, item18, item19, item20, item21, new Tuple<T22, T23, T24, T25, T26>(item22, item23, item24, item25, item26)))));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        /// <param name="item26">Item 26 of the tuple.</param>
        public void Dequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22,
            out T23 item23,
            out T24 item24,
            out T25 item25,
            out T26 item26)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26>>>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            item23 = tuple.Rest.Rest.Rest.Item2;
            item24 = tuple.Rest.Rest.Rest.Item3;
            item25 = tuple.Rest.Rest.Rest.Item4;
            item26 = tuple.Rest.Rest.Rest.Item5;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        /// <param name="item26">Item 26 of the tuple.</param>
        public void Peek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22,
            out T23 item23,
            out T24 item24,
            out T25 item25,
            out T26 item26)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26>>>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            item23 = tuple.Rest.Rest.Rest.Item2;
            item24 = tuple.Rest.Rest.Rest.Item3;
            item25 = tuple.Rest.Rest.Rest.Item4;
            item26 = tuple.Rest.Rest.Rest.Item5;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        /// <param name="item26">Item 26 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryDequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22,
            out T23 item23,
            out T24 item24,
            out T25 item25,
            out T26 item26)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                item9 = default(T9);
                item10 = default(T10);
                item11 = default(T11);
                item12 = default(T12);
                item13 = default(T13);
                item14 = default(T14);
                item15 = default(T15);
                item16 = default(T16);
                item17 = default(T17);
                item18 = default(T18);
                item19 = default(T19);
                item20 = default(T20);
                item21 = default(T21);
                item22 = default(T22);
                item23 = default(T23);
                item24 = default(T24);
                item25 = default(T25);
                item26 = default(T26);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26>>>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            item23 = tuple.Rest.Rest.Rest.Item2;
            item24 = tuple.Rest.Rest.Rest.Item3;
            item25 = tuple.Rest.Rest.Rest.Item4;
            item26 = tuple.Rest.Rest.Rest.Item5;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        /// <param name="item26">Item 26 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryPeek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22,
            out T23 item23,
            out T24 item24,
            out T25 item25,
            out T26 item26)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                item9 = default(T9);
                item10 = default(T10);
                item11 = default(T11);
                item12 = default(T12);
                item13 = default(T13);
                item14 = default(T14);
                item15 = default(T15);
                item16 = default(T16);
                item17 = default(T17);
                item18 = default(T18);
                item19 = default(T19);
                item20 = default(T20);
                item21 = default(T21);
                item22 = default(T22);
                item23 = default(T23);
                item24 = default(T24);
                item25 = default(T25);
                item26 = default(T26);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26>>>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            item23 = tuple.Rest.Rest.Rest.Item2;
            item24 = tuple.Rest.Rest.Rest.Item3;
            item25 = tuple.Rest.Rest.Rest.Item4;
            item26 = tuple.Rest.Rest.Rest.Item5;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
    }
    #endregion


    #region Queue with 27 items.
    /// <summary>
    /// A queue of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8, T9, T10, T11, T12, T13, T14, Tuple&lt;T15, T16, T17, T18, T19, T20, T21, Tuple&lt;T22, T23, T24, T25, T26, T27&gt;&gt;&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    /// <typeparam name="T9">The type of item 9.</typeparam>
    /// <typeparam name="T10">The type of item 10.</typeparam>
    /// <typeparam name="T11">The type of item 11.</typeparam>
    /// <typeparam name="T12">The type of item 12.</typeparam>
    /// <typeparam name="T13">The type of item 13.</typeparam>
    /// <typeparam name="T14">The type of item 14.</typeparam>
    /// <typeparam name="T15">The type of item 15.</typeparam>
    /// <typeparam name="T16">The type of item 16.</typeparam>
    /// <typeparam name="T17">The type of item 17.</typeparam>
    /// <typeparam name="T18">The type of item 18.</typeparam>
    /// <typeparam name="T19">The type of item 19.</typeparam>
    /// <typeparam name="T20">The type of item 20.</typeparam>
    /// <typeparam name="T21">The type of item 21.</typeparam>
    /// <typeparam name="T22">The type of item 22.</typeparam>
    /// <typeparam name="T23">The type of item 23.</typeparam>
    /// <typeparam name="T24">The type of item 24.</typeparam>
    /// <typeparam name="T25">The type of item 25.</typeparam>
    /// <typeparam name="T26">The type of item 26.</typeparam>
    /// <typeparam name="T27">The type of item 27.</typeparam>
    [PublicAPI]
    public class Queue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27> : Queue<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27>>>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27}"/> class that is empty 
        /// and has the specified initial capacity or the default initial capacity, whichever is greater.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        public Queue(int capacity = 4) : base(capacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27}"/> class that contains elements copied 
        /// from the specified collection and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection to copy elements from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public Queue([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27>>>>> collection) : base(collection)
        {
        }

        /// <summary>
        /// Inserts an object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27}" />.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        /// <param name="item26">Item 26 of the tuple.</param>
        /// <param name="item27">Item 27 of the tuple.</param>
        public void Enqueue(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4,
            T5 item5,
            T6 item6,
            T7 item7,
            T8 item8,
            T9 item9,
            T10 item10,
            T11 item11,
            T12 item12,
            T13 item13,
            T14 item14,
            T15 item15,
            T16 item16,
            T17 item17,
            T18 item18,
            T19 item19,
            T20 item20,
            T21 item21,
            T22 item22,
            T23 item23,
            T24 item24,
            T25 item25,
            T26 item26,
            T27 item27)
        {
            Enqueue(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27>>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27>>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27>>(item15, item16, item17, item18, item19, item20, item21, new Tuple<T22, T23, T24, T25, T26, T27>(item22, item23, item24, item25, item26, item27)))));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        /// <param name="item26">Item 26 of the tuple.</param>
        /// <param name="item27">Item 27 of the tuple.</param>
        public void Dequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22,
            out T23 item23,
            out T24 item24,
            out T25 item25,
            out T26 item26,
            out T27 item27)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27>>>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            item23 = tuple.Rest.Rest.Rest.Item2;
            item24 = tuple.Rest.Rest.Rest.Item3;
            item25 = tuple.Rest.Rest.Rest.Item4;
            item26 = tuple.Rest.Rest.Rest.Item5;
            item27 = tuple.Rest.Rest.Rest.Item6;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        /// <param name="item26">Item 26 of the tuple.</param>
        /// <param name="item27">Item 27 of the tuple.</param>
        public void Peek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22,
            out T23 item23,
            out T24 item24,
            out T25 item25,
            out T26 item26,
            out T27 item27)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27>>>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            item23 = tuple.Rest.Rest.Rest.Item2;
            item24 = tuple.Rest.Rest.Rest.Item3;
            item25 = tuple.Rest.Rest.Rest.Item4;
            item26 = tuple.Rest.Rest.Rest.Item5;
            item27 = tuple.Rest.Rest.Rest.Item6;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        /// <param name="item26">Item 26 of the tuple.</param>
        /// <param name="item27">Item 27 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryDequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22,
            out T23 item23,
            out T24 item24,
            out T25 item25,
            out T26 item26,
            out T27 item27)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                item9 = default(T9);
                item10 = default(T10);
                item11 = default(T11);
                item12 = default(T12);
                item13 = default(T13);
                item14 = default(T14);
                item15 = default(T15);
                item16 = default(T16);
                item17 = default(T17);
                item18 = default(T18);
                item19 = default(T19);
                item20 = default(T20);
                item21 = default(T21);
                item22 = default(T22);
                item23 = default(T23);
                item24 = default(T24);
                item25 = default(T25);
                item26 = default(T26);
                item27 = default(T27);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27>>>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            item23 = tuple.Rest.Rest.Rest.Item2;
            item24 = tuple.Rest.Rest.Rest.Item3;
            item25 = tuple.Rest.Rest.Rest.Item4;
            item26 = tuple.Rest.Rest.Rest.Item5;
            item27 = tuple.Rest.Rest.Rest.Item6;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        /// <param name="item26">Item 26 of the tuple.</param>
        /// <param name="item27">Item 27 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryPeek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22,
            out T23 item23,
            out T24 item24,
            out T25 item25,
            out T26 item26,
            out T27 item27)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                item9 = default(T9);
                item10 = default(T10);
                item11 = default(T11);
                item12 = default(T12);
                item13 = default(T13);
                item14 = default(T14);
                item15 = default(T15);
                item16 = default(T16);
                item17 = default(T17);
                item18 = default(T18);
                item19 = default(T19);
                item20 = default(T20);
                item21 = default(T21);
                item22 = default(T22);
                item23 = default(T23);
                item24 = default(T24);
                item25 = default(T25);
                item26 = default(T26);
                item27 = default(T27);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27>>>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            item23 = tuple.Rest.Rest.Rest.Item2;
            item24 = tuple.Rest.Rest.Rest.Item3;
            item25 = tuple.Rest.Rest.Rest.Item4;
            item26 = tuple.Rest.Rest.Rest.Item5;
            item27 = tuple.Rest.Rest.Rest.Item6;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
    }
    #endregion


    #region Queue with 28 items.
    /// <summary>
    /// A queue of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8, T9, T10, T11, T12, T13, T14, Tuple&lt;T15, T16, T17, T18, T19, T20, T21, Tuple&lt;T22, T23, T24, T25, T26, T27, T28&gt;&gt;&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    /// <typeparam name="T9">The type of item 9.</typeparam>
    /// <typeparam name="T10">The type of item 10.</typeparam>
    /// <typeparam name="T11">The type of item 11.</typeparam>
    /// <typeparam name="T12">The type of item 12.</typeparam>
    /// <typeparam name="T13">The type of item 13.</typeparam>
    /// <typeparam name="T14">The type of item 14.</typeparam>
    /// <typeparam name="T15">The type of item 15.</typeparam>
    /// <typeparam name="T16">The type of item 16.</typeparam>
    /// <typeparam name="T17">The type of item 17.</typeparam>
    /// <typeparam name="T18">The type of item 18.</typeparam>
    /// <typeparam name="T19">The type of item 19.</typeparam>
    /// <typeparam name="T20">The type of item 20.</typeparam>
    /// <typeparam name="T21">The type of item 21.</typeparam>
    /// <typeparam name="T22">The type of item 22.</typeparam>
    /// <typeparam name="T23">The type of item 23.</typeparam>
    /// <typeparam name="T24">The type of item 24.</typeparam>
    /// <typeparam name="T25">The type of item 25.</typeparam>
    /// <typeparam name="T26">The type of item 26.</typeparam>
    /// <typeparam name="T27">The type of item 27.</typeparam>
    /// <typeparam name="T28">The type of item 28.</typeparam>
    [PublicAPI]
    public class Queue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28> : Queue<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28>>>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28}"/> class that is empty 
        /// and has the specified initial capacity or the default initial capacity, whichever is greater.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        public Queue(int capacity = 4) : base(capacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28}"/> class that contains elements copied 
        /// from the specified collection and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection to copy elements from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public Queue([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28>>>>> collection) : base(collection)
        {
        }

        /// <summary>
        /// Inserts an object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28}" />.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        /// <param name="item26">Item 26 of the tuple.</param>
        /// <param name="item27">Item 27 of the tuple.</param>
        /// <param name="item28">Item 28 of the tuple.</param>
        public void Enqueue(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4,
            T5 item5,
            T6 item6,
            T7 item7,
            T8 item8,
            T9 item9,
            T10 item10,
            T11 item11,
            T12 item12,
            T13 item13,
            T14 item14,
            T15 item15,
            T16 item16,
            T17 item17,
            T18 item18,
            T19 item19,
            T20 item20,
            T21 item21,
            T22 item22,
            T23 item23,
            T24 item24,
            T25 item25,
            T26 item26,
            T27 item27,
            T28 item28)
        {
            Enqueue(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28>>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28>>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28>>(item15, item16, item17, item18, item19, item20, item21, new Tuple<T22, T23, T24, T25, T26, T27, T28>(item22, item23, item24, item25, item26, item27, item28)))));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        /// <param name="item26">Item 26 of the tuple.</param>
        /// <param name="item27">Item 27 of the tuple.</param>
        /// <param name="item28">Item 28 of the tuple.</param>
        public void Dequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22,
            out T23 item23,
            out T24 item24,
            out T25 item25,
            out T26 item26,
            out T27 item27,
            out T28 item28)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28>>>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            item23 = tuple.Rest.Rest.Rest.Item2;
            item24 = tuple.Rest.Rest.Rest.Item3;
            item25 = tuple.Rest.Rest.Rest.Item4;
            item26 = tuple.Rest.Rest.Rest.Item5;
            item27 = tuple.Rest.Rest.Rest.Item6;
            item28 = tuple.Rest.Rest.Rest.Item7;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        /// <param name="item26">Item 26 of the tuple.</param>
        /// <param name="item27">Item 27 of the tuple.</param>
        /// <param name="item28">Item 28 of the tuple.</param>
        public void Peek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22,
            out T23 item23,
            out T24 item24,
            out T25 item25,
            out T26 item26,
            out T27 item27,
            out T28 item28)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28>>>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            item23 = tuple.Rest.Rest.Rest.Item2;
            item24 = tuple.Rest.Rest.Rest.Item3;
            item25 = tuple.Rest.Rest.Rest.Item4;
            item26 = tuple.Rest.Rest.Rest.Item5;
            item27 = tuple.Rest.Rest.Rest.Item6;
            item28 = tuple.Rest.Rest.Rest.Item7;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        /// <param name="item26">Item 26 of the tuple.</param>
        /// <param name="item27">Item 27 of the tuple.</param>
        /// <param name="item28">Item 28 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryDequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22,
            out T23 item23,
            out T24 item24,
            out T25 item25,
            out T26 item26,
            out T27 item27,
            out T28 item28)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                item9 = default(T9);
                item10 = default(T10);
                item11 = default(T11);
                item12 = default(T12);
                item13 = default(T13);
                item14 = default(T14);
                item15 = default(T15);
                item16 = default(T16);
                item17 = default(T17);
                item18 = default(T18);
                item19 = default(T19);
                item20 = default(T20);
                item21 = default(T21);
                item22 = default(T22);
                item23 = default(T23);
                item24 = default(T24);
                item25 = default(T25);
                item26 = default(T26);
                item27 = default(T27);
                item28 = default(T28);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28>>>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            item23 = tuple.Rest.Rest.Rest.Item2;
            item24 = tuple.Rest.Rest.Rest.Item3;
            item25 = tuple.Rest.Rest.Rest.Item4;
            item26 = tuple.Rest.Rest.Rest.Item5;
            item27 = tuple.Rest.Rest.Rest.Item6;
            item28 = tuple.Rest.Rest.Rest.Item7;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        /// <param name="item26">Item 26 of the tuple.</param>
        /// <param name="item27">Item 27 of the tuple.</param>
        /// <param name="item28">Item 28 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryPeek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22,
            out T23 item23,
            out T24 item24,
            out T25 item25,
            out T26 item26,
            out T27 item27,
            out T28 item28)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                item9 = default(T9);
                item10 = default(T10);
                item11 = default(T11);
                item12 = default(T12);
                item13 = default(T13);
                item14 = default(T14);
                item15 = default(T15);
                item16 = default(T16);
                item17 = default(T17);
                item18 = default(T18);
                item19 = default(T19);
                item20 = default(T20);
                item21 = default(T21);
                item22 = default(T22);
                item23 = default(T23);
                item24 = default(T24);
                item25 = default(T25);
                item26 = default(T26);
                item27 = default(T27);
                item28 = default(T28);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28>>>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            item23 = tuple.Rest.Rest.Rest.Item2;
            item24 = tuple.Rest.Rest.Rest.Item3;
            item25 = tuple.Rest.Rest.Rest.Item4;
            item26 = tuple.Rest.Rest.Rest.Item5;
            item27 = tuple.Rest.Rest.Rest.Item6;
            item28 = tuple.Rest.Rest.Rest.Item7;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
    }
    #endregion


    #region Queue with 29 items.
    /// <summary>
    /// A queue of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8, T9, T10, T11, T12, T13, T14, Tuple&lt;T15, T16, T17, T18, T19, T20, T21, Tuple&lt;T22, T23, T24, T25, T26, T27, T28, Tuple&lt;T29&gt;&gt;&gt;&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    /// <typeparam name="T9">The type of item 9.</typeparam>
    /// <typeparam name="T10">The type of item 10.</typeparam>
    /// <typeparam name="T11">The type of item 11.</typeparam>
    /// <typeparam name="T12">The type of item 12.</typeparam>
    /// <typeparam name="T13">The type of item 13.</typeparam>
    /// <typeparam name="T14">The type of item 14.</typeparam>
    /// <typeparam name="T15">The type of item 15.</typeparam>
    /// <typeparam name="T16">The type of item 16.</typeparam>
    /// <typeparam name="T17">The type of item 17.</typeparam>
    /// <typeparam name="T18">The type of item 18.</typeparam>
    /// <typeparam name="T19">The type of item 19.</typeparam>
    /// <typeparam name="T20">The type of item 20.</typeparam>
    /// <typeparam name="T21">The type of item 21.</typeparam>
    /// <typeparam name="T22">The type of item 22.</typeparam>
    /// <typeparam name="T23">The type of item 23.</typeparam>
    /// <typeparam name="T24">The type of item 24.</typeparam>
    /// <typeparam name="T25">The type of item 25.</typeparam>
    /// <typeparam name="T26">The type of item 26.</typeparam>
    /// <typeparam name="T27">The type of item 27.</typeparam>
    /// <typeparam name="T28">The type of item 28.</typeparam>
    /// <typeparam name="T29">The type of item 29.</typeparam>
    [PublicAPI]
    public class Queue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29> : Queue<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29>>>>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29}"/> class that is empty 
        /// and has the specified initial capacity or the default initial capacity, whichever is greater.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        public Queue(int capacity = 4) : base(capacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29}"/> class that contains elements copied 
        /// from the specified collection and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection to copy elements from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public Queue([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29>>>>>> collection) : base(collection)
        {
        }

        /// <summary>
        /// Inserts an object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29}" />.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        /// <param name="item26">Item 26 of the tuple.</param>
        /// <param name="item27">Item 27 of the tuple.</param>
        /// <param name="item28">Item 28 of the tuple.</param>
        /// <param name="item29">Item 29 of the tuple.</param>
        public void Enqueue(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4,
            T5 item5,
            T6 item6,
            T7 item7,
            T8 item8,
            T9 item9,
            T10 item10,
            T11 item11,
            T12 item12,
            T13 item13,
            T14 item14,
            T15 item15,
            T16 item16,
            T17 item17,
            T18 item18,
            T19 item19,
            T20 item20,
            T21 item21,
            T22 item22,
            T23 item23,
            T24 item24,
            T25 item25,
            T26 item26,
            T27 item27,
            T28 item28,
            T29 item29)
        {
            Enqueue(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29>>>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29>>>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29>>>(item15, item16, item17, item18, item19, item20, item21, new Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29>>(item22, item23, item24, item25, item26, item27, item28, new Tuple<T29>(item29))))));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        /// <param name="item26">Item 26 of the tuple.</param>
        /// <param name="item27">Item 27 of the tuple.</param>
        /// <param name="item28">Item 28 of the tuple.</param>
        /// <param name="item29">Item 29 of the tuple.</param>
        public void Dequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22,
            out T23 item23,
            out T24 item24,
            out T25 item25,
            out T26 item26,
            out T27 item27,
            out T28 item28,
            out T29 item29)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29>>>>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            item23 = tuple.Rest.Rest.Rest.Item2;
            item24 = tuple.Rest.Rest.Rest.Item3;
            item25 = tuple.Rest.Rest.Rest.Item4;
            item26 = tuple.Rest.Rest.Rest.Item5;
            item27 = tuple.Rest.Rest.Rest.Item6;
            item28 = tuple.Rest.Rest.Rest.Item7;
            item29 = tuple.Rest.Rest.Rest.Rest.Item1;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        /// <param name="item26">Item 26 of the tuple.</param>
        /// <param name="item27">Item 27 of the tuple.</param>
        /// <param name="item28">Item 28 of the tuple.</param>
        /// <param name="item29">Item 29 of the tuple.</param>
        public void Peek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22,
            out T23 item23,
            out T24 item24,
            out T25 item25,
            out T26 item26,
            out T27 item27,
            out T28 item28,
            out T29 item29)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29>>>>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            item23 = tuple.Rest.Rest.Rest.Item2;
            item24 = tuple.Rest.Rest.Rest.Item3;
            item25 = tuple.Rest.Rest.Rest.Item4;
            item26 = tuple.Rest.Rest.Rest.Item5;
            item27 = tuple.Rest.Rest.Rest.Item6;
            item28 = tuple.Rest.Rest.Rest.Item7;
            item29 = tuple.Rest.Rest.Rest.Rest.Item1;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        /// <param name="item26">Item 26 of the tuple.</param>
        /// <param name="item27">Item 27 of the tuple.</param>
        /// <param name="item28">Item 28 of the tuple.</param>
        /// <param name="item29">Item 29 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryDequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22,
            out T23 item23,
            out T24 item24,
            out T25 item25,
            out T26 item26,
            out T27 item27,
            out T28 item28,
            out T29 item29)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                item9 = default(T9);
                item10 = default(T10);
                item11 = default(T11);
                item12 = default(T12);
                item13 = default(T13);
                item14 = default(T14);
                item15 = default(T15);
                item16 = default(T16);
                item17 = default(T17);
                item18 = default(T18);
                item19 = default(T19);
                item20 = default(T20);
                item21 = default(T21);
                item22 = default(T22);
                item23 = default(T23);
                item24 = default(T24);
                item25 = default(T25);
                item26 = default(T26);
                item27 = default(T27);
                item28 = default(T28);
                item29 = default(T29);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29>>>>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            item23 = tuple.Rest.Rest.Rest.Item2;
            item24 = tuple.Rest.Rest.Rest.Item3;
            item25 = tuple.Rest.Rest.Rest.Item4;
            item26 = tuple.Rest.Rest.Rest.Item5;
            item27 = tuple.Rest.Rest.Rest.Item6;
            item28 = tuple.Rest.Rest.Rest.Item7;
            item29 = tuple.Rest.Rest.Rest.Rest.Item1;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        /// <param name="item26">Item 26 of the tuple.</param>
        /// <param name="item27">Item 27 of the tuple.</param>
        /// <param name="item28">Item 28 of the tuple.</param>
        /// <param name="item29">Item 29 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryPeek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22,
            out T23 item23,
            out T24 item24,
            out T25 item25,
            out T26 item26,
            out T27 item27,
            out T28 item28,
            out T29 item29)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                item9 = default(T9);
                item10 = default(T10);
                item11 = default(T11);
                item12 = default(T12);
                item13 = default(T13);
                item14 = default(T14);
                item15 = default(T15);
                item16 = default(T16);
                item17 = default(T17);
                item18 = default(T18);
                item19 = default(T19);
                item20 = default(T20);
                item21 = default(T21);
                item22 = default(T22);
                item23 = default(T23);
                item24 = default(T24);
                item25 = default(T25);
                item26 = default(T26);
                item27 = default(T27);
                item28 = default(T28);
                item29 = default(T29);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29>>>>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            item23 = tuple.Rest.Rest.Rest.Item2;
            item24 = tuple.Rest.Rest.Rest.Item3;
            item25 = tuple.Rest.Rest.Rest.Item4;
            item26 = tuple.Rest.Rest.Rest.Item5;
            item27 = tuple.Rest.Rest.Rest.Item6;
            item28 = tuple.Rest.Rest.Rest.Item7;
            item29 = tuple.Rest.Rest.Rest.Rest.Item1;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
    }
    #endregion


    #region Queue with 30 items.
    /// <summary>
    /// A queue of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8, T9, T10, T11, T12, T13, T14, Tuple&lt;T15, T16, T17, T18, T19, T20, T21, Tuple&lt;T22, T23, T24, T25, T26, T27, T28, Tuple&lt;T29, T30&gt;&gt;&gt;&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    /// <typeparam name="T9">The type of item 9.</typeparam>
    /// <typeparam name="T10">The type of item 10.</typeparam>
    /// <typeparam name="T11">The type of item 11.</typeparam>
    /// <typeparam name="T12">The type of item 12.</typeparam>
    /// <typeparam name="T13">The type of item 13.</typeparam>
    /// <typeparam name="T14">The type of item 14.</typeparam>
    /// <typeparam name="T15">The type of item 15.</typeparam>
    /// <typeparam name="T16">The type of item 16.</typeparam>
    /// <typeparam name="T17">The type of item 17.</typeparam>
    /// <typeparam name="T18">The type of item 18.</typeparam>
    /// <typeparam name="T19">The type of item 19.</typeparam>
    /// <typeparam name="T20">The type of item 20.</typeparam>
    /// <typeparam name="T21">The type of item 21.</typeparam>
    /// <typeparam name="T22">The type of item 22.</typeparam>
    /// <typeparam name="T23">The type of item 23.</typeparam>
    /// <typeparam name="T24">The type of item 24.</typeparam>
    /// <typeparam name="T25">The type of item 25.</typeparam>
    /// <typeparam name="T26">The type of item 26.</typeparam>
    /// <typeparam name="T27">The type of item 27.</typeparam>
    /// <typeparam name="T28">The type of item 28.</typeparam>
    /// <typeparam name="T29">The type of item 29.</typeparam>
    /// <typeparam name="T30">The type of item 30.</typeparam>
    [PublicAPI]
    public class Queue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30> : Queue<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30>>>>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30}"/> class that is empty 
        /// and has the specified initial capacity or the default initial capacity, whichever is greater.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        public Queue(int capacity = 4) : base(capacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30}"/> class that contains elements copied 
        /// from the specified collection and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection to copy elements from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public Queue([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30>>>>>> collection) : base(collection)
        {
        }

        /// <summary>
        /// Inserts an object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30}" />.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        /// <param name="item26">Item 26 of the tuple.</param>
        /// <param name="item27">Item 27 of the tuple.</param>
        /// <param name="item28">Item 28 of the tuple.</param>
        /// <param name="item29">Item 29 of the tuple.</param>
        /// <param name="item30">Item 30 of the tuple.</param>
        public void Enqueue(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4,
            T5 item5,
            T6 item6,
            T7 item7,
            T8 item8,
            T9 item9,
            T10 item10,
            T11 item11,
            T12 item12,
            T13 item13,
            T14 item14,
            T15 item15,
            T16 item16,
            T17 item17,
            T18 item18,
            T19 item19,
            T20 item20,
            T21 item21,
            T22 item22,
            T23 item23,
            T24 item24,
            T25 item25,
            T26 item26,
            T27 item27,
            T28 item28,
            T29 item29,
            T30 item30)
        {
            Enqueue(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30>>>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30>>>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30>>>(item15, item16, item17, item18, item19, item20, item21, new Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30>>(item22, item23, item24, item25, item26, item27, item28, new Tuple<T29, T30>(item29, item30))))));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        /// <param name="item26">Item 26 of the tuple.</param>
        /// <param name="item27">Item 27 of the tuple.</param>
        /// <param name="item28">Item 28 of the tuple.</param>
        /// <param name="item29">Item 29 of the tuple.</param>
        /// <param name="item30">Item 30 of the tuple.</param>
        public void Dequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22,
            out T23 item23,
            out T24 item24,
            out T25 item25,
            out T26 item26,
            out T27 item27,
            out T28 item28,
            out T29 item29,
            out T30 item30)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30>>>>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            item23 = tuple.Rest.Rest.Rest.Item2;
            item24 = tuple.Rest.Rest.Rest.Item3;
            item25 = tuple.Rest.Rest.Rest.Item4;
            item26 = tuple.Rest.Rest.Rest.Item5;
            item27 = tuple.Rest.Rest.Rest.Item6;
            item28 = tuple.Rest.Rest.Rest.Item7;
            item29 = tuple.Rest.Rest.Rest.Rest.Item1;
            item30 = tuple.Rest.Rest.Rest.Rest.Item2;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        /// <param name="item26">Item 26 of the tuple.</param>
        /// <param name="item27">Item 27 of the tuple.</param>
        /// <param name="item28">Item 28 of the tuple.</param>
        /// <param name="item29">Item 29 of the tuple.</param>
        /// <param name="item30">Item 30 of the tuple.</param>
        public void Peek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22,
            out T23 item23,
            out T24 item24,
            out T25 item25,
            out T26 item26,
            out T27 item27,
            out T28 item28,
            out T29 item29,
            out T30 item30)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30>>>>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            item23 = tuple.Rest.Rest.Rest.Item2;
            item24 = tuple.Rest.Rest.Rest.Item3;
            item25 = tuple.Rest.Rest.Rest.Item4;
            item26 = tuple.Rest.Rest.Rest.Item5;
            item27 = tuple.Rest.Rest.Rest.Item6;
            item28 = tuple.Rest.Rest.Rest.Item7;
            item29 = tuple.Rest.Rest.Rest.Rest.Item1;
            item30 = tuple.Rest.Rest.Rest.Rest.Item2;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        /// <param name="item26">Item 26 of the tuple.</param>
        /// <param name="item27">Item 27 of the tuple.</param>
        /// <param name="item28">Item 28 of the tuple.</param>
        /// <param name="item29">Item 29 of the tuple.</param>
        /// <param name="item30">Item 30 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryDequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22,
            out T23 item23,
            out T24 item24,
            out T25 item25,
            out T26 item26,
            out T27 item27,
            out T28 item28,
            out T29 item29,
            out T30 item30)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                item9 = default(T9);
                item10 = default(T10);
                item11 = default(T11);
                item12 = default(T12);
                item13 = default(T13);
                item14 = default(T14);
                item15 = default(T15);
                item16 = default(T16);
                item17 = default(T17);
                item18 = default(T18);
                item19 = default(T19);
                item20 = default(T20);
                item21 = default(T21);
                item22 = default(T22);
                item23 = default(T23);
                item24 = default(T24);
                item25 = default(T25);
                item26 = default(T26);
                item27 = default(T27);
                item28 = default(T28);
                item29 = default(T29);
                item30 = default(T30);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30>>>>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            item23 = tuple.Rest.Rest.Rest.Item2;
            item24 = tuple.Rest.Rest.Rest.Item3;
            item25 = tuple.Rest.Rest.Rest.Item4;
            item26 = tuple.Rest.Rest.Rest.Item5;
            item27 = tuple.Rest.Rest.Rest.Item6;
            item28 = tuple.Rest.Rest.Rest.Item7;
            item29 = tuple.Rest.Rest.Rest.Rest.Item1;
            item30 = tuple.Rest.Rest.Rest.Rest.Item2;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        /// <param name="item26">Item 26 of the tuple.</param>
        /// <param name="item27">Item 27 of the tuple.</param>
        /// <param name="item28">Item 28 of the tuple.</param>
        /// <param name="item29">Item 29 of the tuple.</param>
        /// <param name="item30">Item 30 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryPeek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22,
            out T23 item23,
            out T24 item24,
            out T25 item25,
            out T26 item26,
            out T27 item27,
            out T28 item28,
            out T29 item29,
            out T30 item30)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                item9 = default(T9);
                item10 = default(T10);
                item11 = default(T11);
                item12 = default(T12);
                item13 = default(T13);
                item14 = default(T14);
                item15 = default(T15);
                item16 = default(T16);
                item17 = default(T17);
                item18 = default(T18);
                item19 = default(T19);
                item20 = default(T20);
                item21 = default(T21);
                item22 = default(T22);
                item23 = default(T23);
                item24 = default(T24);
                item25 = default(T25);
                item26 = default(T26);
                item27 = default(T27);
                item28 = default(T28);
                item29 = default(T29);
                item30 = default(T30);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30>>>>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            item23 = tuple.Rest.Rest.Rest.Item2;
            item24 = tuple.Rest.Rest.Rest.Item3;
            item25 = tuple.Rest.Rest.Rest.Item4;
            item26 = tuple.Rest.Rest.Rest.Item5;
            item27 = tuple.Rest.Rest.Rest.Item6;
            item28 = tuple.Rest.Rest.Rest.Item7;
            item29 = tuple.Rest.Rest.Rest.Rest.Item1;
            item30 = tuple.Rest.Rest.Rest.Rest.Item2;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
    }
    #endregion


    #region Queue with 31 items.
    /// <summary>
    /// A queue of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8, T9, T10, T11, T12, T13, T14, Tuple&lt;T15, T16, T17, T18, T19, T20, T21, Tuple&lt;T22, T23, T24, T25, T26, T27, T28, Tuple&lt;T29, T30, T31&gt;&gt;&gt;&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    /// <typeparam name="T9">The type of item 9.</typeparam>
    /// <typeparam name="T10">The type of item 10.</typeparam>
    /// <typeparam name="T11">The type of item 11.</typeparam>
    /// <typeparam name="T12">The type of item 12.</typeparam>
    /// <typeparam name="T13">The type of item 13.</typeparam>
    /// <typeparam name="T14">The type of item 14.</typeparam>
    /// <typeparam name="T15">The type of item 15.</typeparam>
    /// <typeparam name="T16">The type of item 16.</typeparam>
    /// <typeparam name="T17">The type of item 17.</typeparam>
    /// <typeparam name="T18">The type of item 18.</typeparam>
    /// <typeparam name="T19">The type of item 19.</typeparam>
    /// <typeparam name="T20">The type of item 20.</typeparam>
    /// <typeparam name="T21">The type of item 21.</typeparam>
    /// <typeparam name="T22">The type of item 22.</typeparam>
    /// <typeparam name="T23">The type of item 23.</typeparam>
    /// <typeparam name="T24">The type of item 24.</typeparam>
    /// <typeparam name="T25">The type of item 25.</typeparam>
    /// <typeparam name="T26">The type of item 26.</typeparam>
    /// <typeparam name="T27">The type of item 27.</typeparam>
    /// <typeparam name="T28">The type of item 28.</typeparam>
    /// <typeparam name="T29">The type of item 29.</typeparam>
    /// <typeparam name="T30">The type of item 30.</typeparam>
    /// <typeparam name="T31">The type of item 31.</typeparam>
    [PublicAPI]
    public class Queue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31> : Queue<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31>>>>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31}"/> class that is empty 
        /// and has the specified initial capacity or the default initial capacity, whichever is greater.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        public Queue(int capacity = 4) : base(capacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31}"/> class that contains elements copied 
        /// from the specified collection and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection to copy elements from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public Queue([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31>>>>>> collection) : base(collection)
        {
        }

        /// <summary>
        /// Inserts an object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31}" />.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        /// <param name="item26">Item 26 of the tuple.</param>
        /// <param name="item27">Item 27 of the tuple.</param>
        /// <param name="item28">Item 28 of the tuple.</param>
        /// <param name="item29">Item 29 of the tuple.</param>
        /// <param name="item30">Item 30 of the tuple.</param>
        /// <param name="item31">Item 31 of the tuple.</param>
        public void Enqueue(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4,
            T5 item5,
            T6 item6,
            T7 item7,
            T8 item8,
            T9 item9,
            T10 item10,
            T11 item11,
            T12 item12,
            T13 item13,
            T14 item14,
            T15 item15,
            T16 item16,
            T17 item17,
            T18 item18,
            T19 item19,
            T20 item20,
            T21 item21,
            T22 item22,
            T23 item23,
            T24 item24,
            T25 item25,
            T26 item26,
            T27 item27,
            T28 item28,
            T29 item29,
            T30 item30,
            T31 item31)
        {
            Enqueue(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31>>>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31>>>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31>>>(item15, item16, item17, item18, item19, item20, item21, new Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31>>(item22, item23, item24, item25, item26, item27, item28, new Tuple<T29, T30, T31>(item29, item30, item31))))));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        /// <param name="item26">Item 26 of the tuple.</param>
        /// <param name="item27">Item 27 of the tuple.</param>
        /// <param name="item28">Item 28 of the tuple.</param>
        /// <param name="item29">Item 29 of the tuple.</param>
        /// <param name="item30">Item 30 of the tuple.</param>
        /// <param name="item31">Item 31 of the tuple.</param>
        public void Dequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22,
            out T23 item23,
            out T24 item24,
            out T25 item25,
            out T26 item26,
            out T27 item27,
            out T28 item28,
            out T29 item29,
            out T30 item30,
            out T31 item31)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31>>>>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            item23 = tuple.Rest.Rest.Rest.Item2;
            item24 = tuple.Rest.Rest.Rest.Item3;
            item25 = tuple.Rest.Rest.Rest.Item4;
            item26 = tuple.Rest.Rest.Rest.Item5;
            item27 = tuple.Rest.Rest.Rest.Item6;
            item28 = tuple.Rest.Rest.Rest.Item7;
            item29 = tuple.Rest.Rest.Rest.Rest.Item1;
            item30 = tuple.Rest.Rest.Rest.Rest.Item2;
            item31 = tuple.Rest.Rest.Rest.Rest.Item3;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        /// <param name="item26">Item 26 of the tuple.</param>
        /// <param name="item27">Item 27 of the tuple.</param>
        /// <param name="item28">Item 28 of the tuple.</param>
        /// <param name="item29">Item 29 of the tuple.</param>
        /// <param name="item30">Item 30 of the tuple.</param>
        /// <param name="item31">Item 31 of the tuple.</param>
        public void Peek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22,
            out T23 item23,
            out T24 item24,
            out T25 item25,
            out T26 item26,
            out T27 item27,
            out T28 item28,
            out T29 item29,
            out T30 item30,
            out T31 item31)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31>>>>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            item23 = tuple.Rest.Rest.Rest.Item2;
            item24 = tuple.Rest.Rest.Rest.Item3;
            item25 = tuple.Rest.Rest.Rest.Item4;
            item26 = tuple.Rest.Rest.Rest.Item5;
            item27 = tuple.Rest.Rest.Rest.Item6;
            item28 = tuple.Rest.Rest.Rest.Item7;
            item29 = tuple.Rest.Rest.Rest.Rest.Item1;
            item30 = tuple.Rest.Rest.Rest.Rest.Item2;
            item31 = tuple.Rest.Rest.Rest.Rest.Item3;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        /// <param name="item26">Item 26 of the tuple.</param>
        /// <param name="item27">Item 27 of the tuple.</param>
        /// <param name="item28">Item 28 of the tuple.</param>
        /// <param name="item29">Item 29 of the tuple.</param>
        /// <param name="item30">Item 30 of the tuple.</param>
        /// <param name="item31">Item 31 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryDequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22,
            out T23 item23,
            out T24 item24,
            out T25 item25,
            out T26 item26,
            out T27 item27,
            out T28 item28,
            out T29 item29,
            out T30 item30,
            out T31 item31)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                item9 = default(T9);
                item10 = default(T10);
                item11 = default(T11);
                item12 = default(T12);
                item13 = default(T13);
                item14 = default(T14);
                item15 = default(T15);
                item16 = default(T16);
                item17 = default(T17);
                item18 = default(T18);
                item19 = default(T19);
                item20 = default(T20);
                item21 = default(T21);
                item22 = default(T22);
                item23 = default(T23);
                item24 = default(T24);
                item25 = default(T25);
                item26 = default(T26);
                item27 = default(T27);
                item28 = default(T28);
                item29 = default(T29);
                item30 = default(T30);
                item31 = default(T31);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31>>>>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            item23 = tuple.Rest.Rest.Rest.Item2;
            item24 = tuple.Rest.Rest.Rest.Item3;
            item25 = tuple.Rest.Rest.Rest.Item4;
            item26 = tuple.Rest.Rest.Rest.Item5;
            item27 = tuple.Rest.Rest.Rest.Item6;
            item28 = tuple.Rest.Rest.Rest.Item7;
            item29 = tuple.Rest.Rest.Rest.Rest.Item1;
            item30 = tuple.Rest.Rest.Rest.Rest.Item2;
            item31 = tuple.Rest.Rest.Rest.Rest.Item3;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        /// <param name="item26">Item 26 of the tuple.</param>
        /// <param name="item27">Item 27 of the tuple.</param>
        /// <param name="item28">Item 28 of the tuple.</param>
        /// <param name="item29">Item 29 of the tuple.</param>
        /// <param name="item30">Item 30 of the tuple.</param>
        /// <param name="item31">Item 31 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryPeek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22,
            out T23 item23,
            out T24 item24,
            out T25 item25,
            out T26 item26,
            out T27 item27,
            out T28 item28,
            out T29 item29,
            out T30 item30,
            out T31 item31)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                item9 = default(T9);
                item10 = default(T10);
                item11 = default(T11);
                item12 = default(T12);
                item13 = default(T13);
                item14 = default(T14);
                item15 = default(T15);
                item16 = default(T16);
                item17 = default(T17);
                item18 = default(T18);
                item19 = default(T19);
                item20 = default(T20);
                item21 = default(T21);
                item22 = default(T22);
                item23 = default(T23);
                item24 = default(T24);
                item25 = default(T25);
                item26 = default(T26);
                item27 = default(T27);
                item28 = default(T28);
                item29 = default(T29);
                item30 = default(T30);
                item31 = default(T31);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31>>>>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            item23 = tuple.Rest.Rest.Rest.Item2;
            item24 = tuple.Rest.Rest.Rest.Item3;
            item25 = tuple.Rest.Rest.Rest.Item4;
            item26 = tuple.Rest.Rest.Rest.Item5;
            item27 = tuple.Rest.Rest.Rest.Item6;
            item28 = tuple.Rest.Rest.Rest.Item7;
            item29 = tuple.Rest.Rest.Rest.Rest.Item1;
            item30 = tuple.Rest.Rest.Rest.Rest.Item2;
            item31 = tuple.Rest.Rest.Rest.Rest.Item3;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
    }
    #endregion


    #region Queue with 32 items.
    /// <summary>
    /// A queue of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8, T9, T10, T11, T12, T13, T14, Tuple&lt;T15, T16, T17, T18, T19, T20, T21, Tuple&lt;T22, T23, T24, T25, T26, T27, T28, Tuple&lt;T29, T30, T31, T32&gt;&gt;&gt;&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    /// <typeparam name="T9">The type of item 9.</typeparam>
    /// <typeparam name="T10">The type of item 10.</typeparam>
    /// <typeparam name="T11">The type of item 11.</typeparam>
    /// <typeparam name="T12">The type of item 12.</typeparam>
    /// <typeparam name="T13">The type of item 13.</typeparam>
    /// <typeparam name="T14">The type of item 14.</typeparam>
    /// <typeparam name="T15">The type of item 15.</typeparam>
    /// <typeparam name="T16">The type of item 16.</typeparam>
    /// <typeparam name="T17">The type of item 17.</typeparam>
    /// <typeparam name="T18">The type of item 18.</typeparam>
    /// <typeparam name="T19">The type of item 19.</typeparam>
    /// <typeparam name="T20">The type of item 20.</typeparam>
    /// <typeparam name="T21">The type of item 21.</typeparam>
    /// <typeparam name="T22">The type of item 22.</typeparam>
    /// <typeparam name="T23">The type of item 23.</typeparam>
    /// <typeparam name="T24">The type of item 24.</typeparam>
    /// <typeparam name="T25">The type of item 25.</typeparam>
    /// <typeparam name="T26">The type of item 26.</typeparam>
    /// <typeparam name="T27">The type of item 27.</typeparam>
    /// <typeparam name="T28">The type of item 28.</typeparam>
    /// <typeparam name="T29">The type of item 29.</typeparam>
    /// <typeparam name="T30">The type of item 30.</typeparam>
    /// <typeparam name="T31">The type of item 31.</typeparam>
    /// <typeparam name="T32">The type of item 32.</typeparam>
    [PublicAPI]
    public class Queue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32> : Queue<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32>>>>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32}"/> class that is empty 
        /// and has the specified initial capacity or the default initial capacity, whichever is greater.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        public Queue(int capacity = 4) : base(capacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32}"/> class that contains elements copied 
        /// from the specified collection and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection to copy elements from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public Queue([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32>>>>>> collection) : base(collection)
        {
        }

        /// <summary>
        /// Inserts an object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32}" />.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        /// <param name="item26">Item 26 of the tuple.</param>
        /// <param name="item27">Item 27 of the tuple.</param>
        /// <param name="item28">Item 28 of the tuple.</param>
        /// <param name="item29">Item 29 of the tuple.</param>
        /// <param name="item30">Item 30 of the tuple.</param>
        /// <param name="item31">Item 31 of the tuple.</param>
        /// <param name="item32">Item 32 of the tuple.</param>
        public void Enqueue(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4,
            T5 item5,
            T6 item6,
            T7 item7,
            T8 item8,
            T9 item9,
            T10 item10,
            T11 item11,
            T12 item12,
            T13 item13,
            T14 item14,
            T15 item15,
            T16 item16,
            T17 item17,
            T18 item18,
            T19 item19,
            T20 item20,
            T21 item21,
            T22 item22,
            T23 item23,
            T24 item24,
            T25 item25,
            T26 item26,
            T27 item27,
            T28 item28,
            T29 item29,
            T30 item30,
            T31 item31,
            T32 item32)
        {
            Enqueue(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32>>>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32>>>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32>>>(item15, item16, item17, item18, item19, item20, item21, new Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32>>(item22, item23, item24, item25, item26, item27, item28, new Tuple<T29, T30, T31, T32>(item29, item30, item31, item32))))));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        /// <param name="item26">Item 26 of the tuple.</param>
        /// <param name="item27">Item 27 of the tuple.</param>
        /// <param name="item28">Item 28 of the tuple.</param>
        /// <param name="item29">Item 29 of the tuple.</param>
        /// <param name="item30">Item 30 of the tuple.</param>
        /// <param name="item31">Item 31 of the tuple.</param>
        /// <param name="item32">Item 32 of the tuple.</param>
        public void Dequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22,
            out T23 item23,
            out T24 item24,
            out T25 item25,
            out T26 item26,
            out T27 item27,
            out T28 item28,
            out T29 item29,
            out T30 item30,
            out T31 item31,
            out T32 item32)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32>>>>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            item23 = tuple.Rest.Rest.Rest.Item2;
            item24 = tuple.Rest.Rest.Rest.Item3;
            item25 = tuple.Rest.Rest.Rest.Item4;
            item26 = tuple.Rest.Rest.Rest.Item5;
            item27 = tuple.Rest.Rest.Rest.Item6;
            item28 = tuple.Rest.Rest.Rest.Item7;
            item29 = tuple.Rest.Rest.Rest.Rest.Item1;
            item30 = tuple.Rest.Rest.Rest.Rest.Item2;
            item31 = tuple.Rest.Rest.Rest.Rest.Item3;
            item32 = tuple.Rest.Rest.Rest.Rest.Item4;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        /// <param name="item26">Item 26 of the tuple.</param>
        /// <param name="item27">Item 27 of the tuple.</param>
        /// <param name="item28">Item 28 of the tuple.</param>
        /// <param name="item29">Item 29 of the tuple.</param>
        /// <param name="item30">Item 30 of the tuple.</param>
        /// <param name="item31">Item 31 of the tuple.</param>
        /// <param name="item32">Item 32 of the tuple.</param>
        public void Peek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22,
            out T23 item23,
            out T24 item24,
            out T25 item25,
            out T26 item26,
            out T27 item27,
            out T28 item28,
            out T29 item29,
            out T30 item30,
            out T31 item31,
            out T32 item32)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32>>>>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            item23 = tuple.Rest.Rest.Rest.Item2;
            item24 = tuple.Rest.Rest.Rest.Item3;
            item25 = tuple.Rest.Rest.Rest.Item4;
            item26 = tuple.Rest.Rest.Rest.Item5;
            item27 = tuple.Rest.Rest.Rest.Item6;
            item28 = tuple.Rest.Rest.Rest.Item7;
            item29 = tuple.Rest.Rest.Rest.Rest.Item1;
            item30 = tuple.Rest.Rest.Rest.Rest.Item2;
            item31 = tuple.Rest.Rest.Rest.Rest.Item3;
            item32 = tuple.Rest.Rest.Rest.Rest.Item4;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        /// <param name="item26">Item 26 of the tuple.</param>
        /// <param name="item27">Item 27 of the tuple.</param>
        /// <param name="item28">Item 28 of the tuple.</param>
        /// <param name="item29">Item 29 of the tuple.</param>
        /// <param name="item30">Item 30 of the tuple.</param>
        /// <param name="item31">Item 31 of the tuple.</param>
        /// <param name="item32">Item 32 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryDequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22,
            out T23 item23,
            out T24 item24,
            out T25 item25,
            out T26 item26,
            out T27 item27,
            out T28 item28,
            out T29 item29,
            out T30 item30,
            out T31 item31,
            out T32 item32)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                item9 = default(T9);
                item10 = default(T10);
                item11 = default(T11);
                item12 = default(T12);
                item13 = default(T13);
                item14 = default(T14);
                item15 = default(T15);
                item16 = default(T16);
                item17 = default(T17);
                item18 = default(T18);
                item19 = default(T19);
                item20 = default(T20);
                item21 = default(T21);
                item22 = default(T22);
                item23 = default(T23);
                item24 = default(T24);
                item25 = default(T25);
                item26 = default(T26);
                item27 = default(T27);
                item28 = default(T28);
                item29 = default(T29);
                item30 = default(T30);
                item31 = default(T31);
                item32 = default(T32);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32>>>>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            item23 = tuple.Rest.Rest.Rest.Item2;
            item24 = tuple.Rest.Rest.Rest.Item3;
            item25 = tuple.Rest.Rest.Rest.Item4;
            item26 = tuple.Rest.Rest.Rest.Item5;
            item27 = tuple.Rest.Rest.Rest.Item6;
            item28 = tuple.Rest.Rest.Rest.Item7;
            item29 = tuple.Rest.Rest.Rest.Rest.Item1;
            item30 = tuple.Rest.Rest.Rest.Rest.Item2;
            item31 = tuple.Rest.Rest.Rest.Rest.Item3;
            item32 = tuple.Rest.Rest.Rest.Rest.Item4;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        /// <param name="item26">Item 26 of the tuple.</param>
        /// <param name="item27">Item 27 of the tuple.</param>
        /// <param name="item28">Item 28 of the tuple.</param>
        /// <param name="item29">Item 29 of the tuple.</param>
        /// <param name="item30">Item 30 of the tuple.</param>
        /// <param name="item31">Item 31 of the tuple.</param>
        /// <param name="item32">Item 32 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryPeek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22,
            out T23 item23,
            out T24 item24,
            out T25 item25,
            out T26 item26,
            out T27 item27,
            out T28 item28,
            out T29 item29,
            out T30 item30,
            out T31 item31,
            out T32 item32)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                item9 = default(T9);
                item10 = default(T10);
                item11 = default(T11);
                item12 = default(T12);
                item13 = default(T13);
                item14 = default(T14);
                item15 = default(T15);
                item16 = default(T16);
                item17 = default(T17);
                item18 = default(T18);
                item19 = default(T19);
                item20 = default(T20);
                item21 = default(T21);
                item22 = default(T22);
                item23 = default(T23);
                item24 = default(T24);
                item25 = default(T25);
                item26 = default(T26);
                item27 = default(T27);
                item28 = default(T28);
                item29 = default(T29);
                item30 = default(T30);
                item31 = default(T31);
                item32 = default(T32);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32>>>>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            item23 = tuple.Rest.Rest.Rest.Item2;
            item24 = tuple.Rest.Rest.Rest.Item3;
            item25 = tuple.Rest.Rest.Rest.Item4;
            item26 = tuple.Rest.Rest.Rest.Item5;
            item27 = tuple.Rest.Rest.Rest.Item6;
            item28 = tuple.Rest.Rest.Rest.Item7;
            item29 = tuple.Rest.Rest.Rest.Rest.Item1;
            item30 = tuple.Rest.Rest.Rest.Rest.Item2;
            item31 = tuple.Rest.Rest.Rest.Rest.Item3;
            item32 = tuple.Rest.Rest.Rest.Rest.Item4;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
    }
    #endregion


    #region Queue with 33 items.
    /// <summary>
    /// A queue of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8, T9, T10, T11, T12, T13, T14, Tuple&lt;T15, T16, T17, T18, T19, T20, T21, Tuple&lt;T22, T23, T24, T25, T26, T27, T28, Tuple&lt;T29, T30, T31, T32, T33&gt;&gt;&gt;&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    /// <typeparam name="T9">The type of item 9.</typeparam>
    /// <typeparam name="T10">The type of item 10.</typeparam>
    /// <typeparam name="T11">The type of item 11.</typeparam>
    /// <typeparam name="T12">The type of item 12.</typeparam>
    /// <typeparam name="T13">The type of item 13.</typeparam>
    /// <typeparam name="T14">The type of item 14.</typeparam>
    /// <typeparam name="T15">The type of item 15.</typeparam>
    /// <typeparam name="T16">The type of item 16.</typeparam>
    /// <typeparam name="T17">The type of item 17.</typeparam>
    /// <typeparam name="T18">The type of item 18.</typeparam>
    /// <typeparam name="T19">The type of item 19.</typeparam>
    /// <typeparam name="T20">The type of item 20.</typeparam>
    /// <typeparam name="T21">The type of item 21.</typeparam>
    /// <typeparam name="T22">The type of item 22.</typeparam>
    /// <typeparam name="T23">The type of item 23.</typeparam>
    /// <typeparam name="T24">The type of item 24.</typeparam>
    /// <typeparam name="T25">The type of item 25.</typeparam>
    /// <typeparam name="T26">The type of item 26.</typeparam>
    /// <typeparam name="T27">The type of item 27.</typeparam>
    /// <typeparam name="T28">The type of item 28.</typeparam>
    /// <typeparam name="T29">The type of item 29.</typeparam>
    /// <typeparam name="T30">The type of item 30.</typeparam>
    /// <typeparam name="T31">The type of item 31.</typeparam>
    /// <typeparam name="T32">The type of item 32.</typeparam>
    /// <typeparam name="T33">The type of item 33.</typeparam>
    [PublicAPI]
    public class Queue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33> : Queue<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33>>>>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33}"/> class that is empty 
        /// and has the specified initial capacity or the default initial capacity, whichever is greater.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        public Queue(int capacity = 4) : base(capacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33}"/> class that contains elements copied 
        /// from the specified collection and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection to copy elements from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public Queue([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33>>>>>> collection) : base(collection)
        {
        }

        /// <summary>
        /// Inserts an object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33}" />.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        /// <param name="item26">Item 26 of the tuple.</param>
        /// <param name="item27">Item 27 of the tuple.</param>
        /// <param name="item28">Item 28 of the tuple.</param>
        /// <param name="item29">Item 29 of the tuple.</param>
        /// <param name="item30">Item 30 of the tuple.</param>
        /// <param name="item31">Item 31 of the tuple.</param>
        /// <param name="item32">Item 32 of the tuple.</param>
        /// <param name="item33">Item 33 of the tuple.</param>
        public void Enqueue(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4,
            T5 item5,
            T6 item6,
            T7 item7,
            T8 item8,
            T9 item9,
            T10 item10,
            T11 item11,
            T12 item12,
            T13 item13,
            T14 item14,
            T15 item15,
            T16 item16,
            T17 item17,
            T18 item18,
            T19 item19,
            T20 item20,
            T21 item21,
            T22 item22,
            T23 item23,
            T24 item24,
            T25 item25,
            T26 item26,
            T27 item27,
            T28 item28,
            T29 item29,
            T30 item30,
            T31 item31,
            T32 item32,
            T33 item33)
        {
            Enqueue(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33>>>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33>>>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33>>>(item15, item16, item17, item18, item19, item20, item21, new Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33>>(item22, item23, item24, item25, item26, item27, item28, new Tuple<T29, T30, T31, T32, T33>(item29, item30, item31, item32, item33))))));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        /// <param name="item26">Item 26 of the tuple.</param>
        /// <param name="item27">Item 27 of the tuple.</param>
        /// <param name="item28">Item 28 of the tuple.</param>
        /// <param name="item29">Item 29 of the tuple.</param>
        /// <param name="item30">Item 30 of the tuple.</param>
        /// <param name="item31">Item 31 of the tuple.</param>
        /// <param name="item32">Item 32 of the tuple.</param>
        /// <param name="item33">Item 33 of the tuple.</param>
        public void Dequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22,
            out T23 item23,
            out T24 item24,
            out T25 item25,
            out T26 item26,
            out T27 item27,
            out T28 item28,
            out T29 item29,
            out T30 item30,
            out T31 item31,
            out T32 item32,
            out T33 item33)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33>>>>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            item23 = tuple.Rest.Rest.Rest.Item2;
            item24 = tuple.Rest.Rest.Rest.Item3;
            item25 = tuple.Rest.Rest.Rest.Item4;
            item26 = tuple.Rest.Rest.Rest.Item5;
            item27 = tuple.Rest.Rest.Rest.Item6;
            item28 = tuple.Rest.Rest.Rest.Item7;
            item29 = tuple.Rest.Rest.Rest.Rest.Item1;
            item30 = tuple.Rest.Rest.Rest.Rest.Item2;
            item31 = tuple.Rest.Rest.Rest.Rest.Item3;
            item32 = tuple.Rest.Rest.Rest.Rest.Item4;
            item33 = tuple.Rest.Rest.Rest.Rest.Item5;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        /// <param name="item26">Item 26 of the tuple.</param>
        /// <param name="item27">Item 27 of the tuple.</param>
        /// <param name="item28">Item 28 of the tuple.</param>
        /// <param name="item29">Item 29 of the tuple.</param>
        /// <param name="item30">Item 30 of the tuple.</param>
        /// <param name="item31">Item 31 of the tuple.</param>
        /// <param name="item32">Item 32 of the tuple.</param>
        /// <param name="item33">Item 33 of the tuple.</param>
        public void Peek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22,
            out T23 item23,
            out T24 item24,
            out T25 item25,
            out T26 item26,
            out T27 item27,
            out T28 item28,
            out T29 item29,
            out T30 item30,
            out T31 item31,
            out T32 item32,
            out T33 item33)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33>>>>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            item23 = tuple.Rest.Rest.Rest.Item2;
            item24 = tuple.Rest.Rest.Rest.Item3;
            item25 = tuple.Rest.Rest.Rest.Item4;
            item26 = tuple.Rest.Rest.Rest.Item5;
            item27 = tuple.Rest.Rest.Rest.Item6;
            item28 = tuple.Rest.Rest.Rest.Item7;
            item29 = tuple.Rest.Rest.Rest.Rest.Item1;
            item30 = tuple.Rest.Rest.Rest.Rest.Item2;
            item31 = tuple.Rest.Rest.Rest.Rest.Item3;
            item32 = tuple.Rest.Rest.Rest.Rest.Item4;
            item33 = tuple.Rest.Rest.Rest.Rest.Item5;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        /// <param name="item26">Item 26 of the tuple.</param>
        /// <param name="item27">Item 27 of the tuple.</param>
        /// <param name="item28">Item 28 of the tuple.</param>
        /// <param name="item29">Item 29 of the tuple.</param>
        /// <param name="item30">Item 30 of the tuple.</param>
        /// <param name="item31">Item 31 of the tuple.</param>
        /// <param name="item32">Item 32 of the tuple.</param>
        /// <param name="item33">Item 33 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryDequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22,
            out T23 item23,
            out T24 item24,
            out T25 item25,
            out T26 item26,
            out T27 item27,
            out T28 item28,
            out T29 item29,
            out T30 item30,
            out T31 item31,
            out T32 item32,
            out T33 item33)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                item9 = default(T9);
                item10 = default(T10);
                item11 = default(T11);
                item12 = default(T12);
                item13 = default(T13);
                item14 = default(T14);
                item15 = default(T15);
                item16 = default(T16);
                item17 = default(T17);
                item18 = default(T18);
                item19 = default(T19);
                item20 = default(T20);
                item21 = default(T21);
                item22 = default(T22);
                item23 = default(T23);
                item24 = default(T24);
                item25 = default(T25);
                item26 = default(T26);
                item27 = default(T27);
                item28 = default(T28);
                item29 = default(T29);
                item30 = default(T30);
                item31 = default(T31);
                item32 = default(T32);
                item33 = default(T33);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33>>>>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            item23 = tuple.Rest.Rest.Rest.Item2;
            item24 = tuple.Rest.Rest.Rest.Item3;
            item25 = tuple.Rest.Rest.Rest.Item4;
            item26 = tuple.Rest.Rest.Rest.Item5;
            item27 = tuple.Rest.Rest.Rest.Item6;
            item28 = tuple.Rest.Rest.Rest.Item7;
            item29 = tuple.Rest.Rest.Rest.Rest.Item1;
            item30 = tuple.Rest.Rest.Rest.Rest.Item2;
            item31 = tuple.Rest.Rest.Rest.Rest.Item3;
            item32 = tuple.Rest.Rest.Rest.Rest.Item4;
            item33 = tuple.Rest.Rest.Rest.Rest.Item5;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        /// <param name="item26">Item 26 of the tuple.</param>
        /// <param name="item27">Item 27 of the tuple.</param>
        /// <param name="item28">Item 28 of the tuple.</param>
        /// <param name="item29">Item 29 of the tuple.</param>
        /// <param name="item30">Item 30 of the tuple.</param>
        /// <param name="item31">Item 31 of the tuple.</param>
        /// <param name="item32">Item 32 of the tuple.</param>
        /// <param name="item33">Item 33 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryPeek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22,
            out T23 item23,
            out T24 item24,
            out T25 item25,
            out T26 item26,
            out T27 item27,
            out T28 item28,
            out T29 item29,
            out T30 item30,
            out T31 item31,
            out T32 item32,
            out T33 item33)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                item9 = default(T9);
                item10 = default(T10);
                item11 = default(T11);
                item12 = default(T12);
                item13 = default(T13);
                item14 = default(T14);
                item15 = default(T15);
                item16 = default(T16);
                item17 = default(T17);
                item18 = default(T18);
                item19 = default(T19);
                item20 = default(T20);
                item21 = default(T21);
                item22 = default(T22);
                item23 = default(T23);
                item24 = default(T24);
                item25 = default(T25);
                item26 = default(T26);
                item27 = default(T27);
                item28 = default(T28);
                item29 = default(T29);
                item30 = default(T30);
                item31 = default(T31);
                item32 = default(T32);
                item33 = default(T33);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33>>>>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            item23 = tuple.Rest.Rest.Rest.Item2;
            item24 = tuple.Rest.Rest.Rest.Item3;
            item25 = tuple.Rest.Rest.Rest.Item4;
            item26 = tuple.Rest.Rest.Rest.Item5;
            item27 = tuple.Rest.Rest.Rest.Item6;
            item28 = tuple.Rest.Rest.Rest.Item7;
            item29 = tuple.Rest.Rest.Rest.Rest.Item1;
            item30 = tuple.Rest.Rest.Rest.Rest.Item2;
            item31 = tuple.Rest.Rest.Rest.Rest.Item3;
            item32 = tuple.Rest.Rest.Rest.Rest.Item4;
            item33 = tuple.Rest.Rest.Rest.Rest.Item5;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
    }
    #endregion


    #region Queue with 34 items.
    /// <summary>
    /// A queue of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8, T9, T10, T11, T12, T13, T14, Tuple&lt;T15, T16, T17, T18, T19, T20, T21, Tuple&lt;T22, T23, T24, T25, T26, T27, T28, Tuple&lt;T29, T30, T31, T32, T33, T34&gt;&gt;&gt;&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    /// <typeparam name="T9">The type of item 9.</typeparam>
    /// <typeparam name="T10">The type of item 10.</typeparam>
    /// <typeparam name="T11">The type of item 11.</typeparam>
    /// <typeparam name="T12">The type of item 12.</typeparam>
    /// <typeparam name="T13">The type of item 13.</typeparam>
    /// <typeparam name="T14">The type of item 14.</typeparam>
    /// <typeparam name="T15">The type of item 15.</typeparam>
    /// <typeparam name="T16">The type of item 16.</typeparam>
    /// <typeparam name="T17">The type of item 17.</typeparam>
    /// <typeparam name="T18">The type of item 18.</typeparam>
    /// <typeparam name="T19">The type of item 19.</typeparam>
    /// <typeparam name="T20">The type of item 20.</typeparam>
    /// <typeparam name="T21">The type of item 21.</typeparam>
    /// <typeparam name="T22">The type of item 22.</typeparam>
    /// <typeparam name="T23">The type of item 23.</typeparam>
    /// <typeparam name="T24">The type of item 24.</typeparam>
    /// <typeparam name="T25">The type of item 25.</typeparam>
    /// <typeparam name="T26">The type of item 26.</typeparam>
    /// <typeparam name="T27">The type of item 27.</typeparam>
    /// <typeparam name="T28">The type of item 28.</typeparam>
    /// <typeparam name="T29">The type of item 29.</typeparam>
    /// <typeparam name="T30">The type of item 30.</typeparam>
    /// <typeparam name="T31">The type of item 31.</typeparam>
    /// <typeparam name="T32">The type of item 32.</typeparam>
    /// <typeparam name="T33">The type of item 33.</typeparam>
    /// <typeparam name="T34">The type of item 34.</typeparam>
    [PublicAPI]
    public class Queue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34> : Queue<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34>>>>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34}"/> class that is empty 
        /// and has the specified initial capacity or the default initial capacity, whichever is greater.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        public Queue(int capacity = 4) : base(capacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34}"/> class that contains elements copied 
        /// from the specified collection and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection to copy elements from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public Queue([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34>>>>>> collection) : base(collection)
        {
        }

        /// <summary>
        /// Inserts an object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34}" />.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        /// <param name="item26">Item 26 of the tuple.</param>
        /// <param name="item27">Item 27 of the tuple.</param>
        /// <param name="item28">Item 28 of the tuple.</param>
        /// <param name="item29">Item 29 of the tuple.</param>
        /// <param name="item30">Item 30 of the tuple.</param>
        /// <param name="item31">Item 31 of the tuple.</param>
        /// <param name="item32">Item 32 of the tuple.</param>
        /// <param name="item33">Item 33 of the tuple.</param>
        /// <param name="item34">Item 34 of the tuple.</param>
        public void Enqueue(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4,
            T5 item5,
            T6 item6,
            T7 item7,
            T8 item8,
            T9 item9,
            T10 item10,
            T11 item11,
            T12 item12,
            T13 item13,
            T14 item14,
            T15 item15,
            T16 item16,
            T17 item17,
            T18 item18,
            T19 item19,
            T20 item20,
            T21 item21,
            T22 item22,
            T23 item23,
            T24 item24,
            T25 item25,
            T26 item26,
            T27 item27,
            T28 item28,
            T29 item29,
            T30 item30,
            T31 item31,
            T32 item32,
            T33 item33,
            T34 item34)
        {
            Enqueue(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34>>>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34>>>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34>>>(item15, item16, item17, item18, item19, item20, item21, new Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34>>(item22, item23, item24, item25, item26, item27, item28, new Tuple<T29, T30, T31, T32, T33, T34>(item29, item30, item31, item32, item33, item34))))));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        /// <param name="item26">Item 26 of the tuple.</param>
        /// <param name="item27">Item 27 of the tuple.</param>
        /// <param name="item28">Item 28 of the tuple.</param>
        /// <param name="item29">Item 29 of the tuple.</param>
        /// <param name="item30">Item 30 of the tuple.</param>
        /// <param name="item31">Item 31 of the tuple.</param>
        /// <param name="item32">Item 32 of the tuple.</param>
        /// <param name="item33">Item 33 of the tuple.</param>
        /// <param name="item34">Item 34 of the tuple.</param>
        public void Dequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22,
            out T23 item23,
            out T24 item24,
            out T25 item25,
            out T26 item26,
            out T27 item27,
            out T28 item28,
            out T29 item29,
            out T30 item30,
            out T31 item31,
            out T32 item32,
            out T33 item33,
            out T34 item34)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34>>>>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            item23 = tuple.Rest.Rest.Rest.Item2;
            item24 = tuple.Rest.Rest.Rest.Item3;
            item25 = tuple.Rest.Rest.Rest.Item4;
            item26 = tuple.Rest.Rest.Rest.Item5;
            item27 = tuple.Rest.Rest.Rest.Item6;
            item28 = tuple.Rest.Rest.Rest.Item7;
            item29 = tuple.Rest.Rest.Rest.Rest.Item1;
            item30 = tuple.Rest.Rest.Rest.Rest.Item2;
            item31 = tuple.Rest.Rest.Rest.Rest.Item3;
            item32 = tuple.Rest.Rest.Rest.Rest.Item4;
            item33 = tuple.Rest.Rest.Rest.Rest.Item5;
            item34 = tuple.Rest.Rest.Rest.Rest.Item6;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        /// <param name="item26">Item 26 of the tuple.</param>
        /// <param name="item27">Item 27 of the tuple.</param>
        /// <param name="item28">Item 28 of the tuple.</param>
        /// <param name="item29">Item 29 of the tuple.</param>
        /// <param name="item30">Item 30 of the tuple.</param>
        /// <param name="item31">Item 31 of the tuple.</param>
        /// <param name="item32">Item 32 of the tuple.</param>
        /// <param name="item33">Item 33 of the tuple.</param>
        /// <param name="item34">Item 34 of the tuple.</param>
        public void Peek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22,
            out T23 item23,
            out T24 item24,
            out T25 item25,
            out T26 item26,
            out T27 item27,
            out T28 item28,
            out T29 item29,
            out T30 item30,
            out T31 item31,
            out T32 item32,
            out T33 item33,
            out T34 item34)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34>>>>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            item23 = tuple.Rest.Rest.Rest.Item2;
            item24 = tuple.Rest.Rest.Rest.Item3;
            item25 = tuple.Rest.Rest.Rest.Item4;
            item26 = tuple.Rest.Rest.Rest.Item5;
            item27 = tuple.Rest.Rest.Rest.Item6;
            item28 = tuple.Rest.Rest.Rest.Item7;
            item29 = tuple.Rest.Rest.Rest.Rest.Item1;
            item30 = tuple.Rest.Rest.Rest.Rest.Item2;
            item31 = tuple.Rest.Rest.Rest.Rest.Item3;
            item32 = tuple.Rest.Rest.Rest.Rest.Item4;
            item33 = tuple.Rest.Rest.Rest.Rest.Item5;
            item34 = tuple.Rest.Rest.Rest.Rest.Item6;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        /// <param name="item26">Item 26 of the tuple.</param>
        /// <param name="item27">Item 27 of the tuple.</param>
        /// <param name="item28">Item 28 of the tuple.</param>
        /// <param name="item29">Item 29 of the tuple.</param>
        /// <param name="item30">Item 30 of the tuple.</param>
        /// <param name="item31">Item 31 of the tuple.</param>
        /// <param name="item32">Item 32 of the tuple.</param>
        /// <param name="item33">Item 33 of the tuple.</param>
        /// <param name="item34">Item 34 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryDequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22,
            out T23 item23,
            out T24 item24,
            out T25 item25,
            out T26 item26,
            out T27 item27,
            out T28 item28,
            out T29 item29,
            out T30 item30,
            out T31 item31,
            out T32 item32,
            out T33 item33,
            out T34 item34)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                item9 = default(T9);
                item10 = default(T10);
                item11 = default(T11);
                item12 = default(T12);
                item13 = default(T13);
                item14 = default(T14);
                item15 = default(T15);
                item16 = default(T16);
                item17 = default(T17);
                item18 = default(T18);
                item19 = default(T19);
                item20 = default(T20);
                item21 = default(T21);
                item22 = default(T22);
                item23 = default(T23);
                item24 = default(T24);
                item25 = default(T25);
                item26 = default(T26);
                item27 = default(T27);
                item28 = default(T28);
                item29 = default(T29);
                item30 = default(T30);
                item31 = default(T31);
                item32 = default(T32);
                item33 = default(T33);
                item34 = default(T34);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34>>>>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            item23 = tuple.Rest.Rest.Rest.Item2;
            item24 = tuple.Rest.Rest.Rest.Item3;
            item25 = tuple.Rest.Rest.Rest.Item4;
            item26 = tuple.Rest.Rest.Rest.Item5;
            item27 = tuple.Rest.Rest.Rest.Item6;
            item28 = tuple.Rest.Rest.Rest.Item7;
            item29 = tuple.Rest.Rest.Rest.Rest.Item1;
            item30 = tuple.Rest.Rest.Rest.Rest.Item2;
            item31 = tuple.Rest.Rest.Rest.Rest.Item3;
            item32 = tuple.Rest.Rest.Rest.Rest.Item4;
            item33 = tuple.Rest.Rest.Rest.Rest.Item5;
            item34 = tuple.Rest.Rest.Rest.Rest.Item6;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        /// <param name="item26">Item 26 of the tuple.</param>
        /// <param name="item27">Item 27 of the tuple.</param>
        /// <param name="item28">Item 28 of the tuple.</param>
        /// <param name="item29">Item 29 of the tuple.</param>
        /// <param name="item30">Item 30 of the tuple.</param>
        /// <param name="item31">Item 31 of the tuple.</param>
        /// <param name="item32">Item 32 of the tuple.</param>
        /// <param name="item33">Item 33 of the tuple.</param>
        /// <param name="item34">Item 34 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryPeek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22,
            out T23 item23,
            out T24 item24,
            out T25 item25,
            out T26 item26,
            out T27 item27,
            out T28 item28,
            out T29 item29,
            out T30 item30,
            out T31 item31,
            out T32 item32,
            out T33 item33,
            out T34 item34)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                item9 = default(T9);
                item10 = default(T10);
                item11 = default(T11);
                item12 = default(T12);
                item13 = default(T13);
                item14 = default(T14);
                item15 = default(T15);
                item16 = default(T16);
                item17 = default(T17);
                item18 = default(T18);
                item19 = default(T19);
                item20 = default(T20);
                item21 = default(T21);
                item22 = default(T22);
                item23 = default(T23);
                item24 = default(T24);
                item25 = default(T25);
                item26 = default(T26);
                item27 = default(T27);
                item28 = default(T28);
                item29 = default(T29);
                item30 = default(T30);
                item31 = default(T31);
                item32 = default(T32);
                item33 = default(T33);
                item34 = default(T34);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34>>>>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            item23 = tuple.Rest.Rest.Rest.Item2;
            item24 = tuple.Rest.Rest.Rest.Item3;
            item25 = tuple.Rest.Rest.Rest.Item4;
            item26 = tuple.Rest.Rest.Rest.Item5;
            item27 = tuple.Rest.Rest.Rest.Item6;
            item28 = tuple.Rest.Rest.Rest.Item7;
            item29 = tuple.Rest.Rest.Rest.Rest.Item1;
            item30 = tuple.Rest.Rest.Rest.Rest.Item2;
            item31 = tuple.Rest.Rest.Rest.Rest.Item3;
            item32 = tuple.Rest.Rest.Rest.Rest.Item4;
            item33 = tuple.Rest.Rest.Rest.Rest.Item5;
            item34 = tuple.Rest.Rest.Rest.Rest.Item6;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
    }
    #endregion


    #region Queue with 35 items.
    /// <summary>
    /// A queue of <c>Tuple&lt;T1, T2, T3, T4, T5, T6, T7, Tuple&lt;T8, T9, T10, T11, T12, T13, T14, Tuple&lt;T15, T16, T17, T18, T19, T20, T21, Tuple&lt;T22, T23, T24, T25, T26, T27, T28, Tuple&lt;T29, T30, T31, T32, T33, T34, T35&gt;&gt;&gt;&gt;&gt;</c>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    /// <typeparam name="T9">The type of item 9.</typeparam>
    /// <typeparam name="T10">The type of item 10.</typeparam>
    /// <typeparam name="T11">The type of item 11.</typeparam>
    /// <typeparam name="T12">The type of item 12.</typeparam>
    /// <typeparam name="T13">The type of item 13.</typeparam>
    /// <typeparam name="T14">The type of item 14.</typeparam>
    /// <typeparam name="T15">The type of item 15.</typeparam>
    /// <typeparam name="T16">The type of item 16.</typeparam>
    /// <typeparam name="T17">The type of item 17.</typeparam>
    /// <typeparam name="T18">The type of item 18.</typeparam>
    /// <typeparam name="T19">The type of item 19.</typeparam>
    /// <typeparam name="T20">The type of item 20.</typeparam>
    /// <typeparam name="T21">The type of item 21.</typeparam>
    /// <typeparam name="T22">The type of item 22.</typeparam>
    /// <typeparam name="T23">The type of item 23.</typeparam>
    /// <typeparam name="T24">The type of item 24.</typeparam>
    /// <typeparam name="T25">The type of item 25.</typeparam>
    /// <typeparam name="T26">The type of item 26.</typeparam>
    /// <typeparam name="T27">The type of item 27.</typeparam>
    /// <typeparam name="T28">The type of item 28.</typeparam>
    /// <typeparam name="T29">The type of item 29.</typeparam>
    /// <typeparam name="T30">The type of item 30.</typeparam>
    /// <typeparam name="T31">The type of item 31.</typeparam>
    /// <typeparam name="T32">The type of item 32.</typeparam>
    /// <typeparam name="T33">The type of item 33.</typeparam>
    /// <typeparam name="T34">The type of item 34.</typeparam>
    /// <typeparam name="T35">The type of item 35.</typeparam>
    [PublicAPI]
    public class Queue<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35> : Queue<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34, T35>>>>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35}"/> class that is empty 
        /// and has the specified initial capacity or the default initial capacity, whichever is greater.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        public Queue(int capacity = 4) : base(capacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35}"/> class that contains elements copied 
        /// from the specified collection and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection to copy elements from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public Queue([NotNull] IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34, T35>>>>>> collection) : base(collection)
        {
        }

        /// <summary>
        /// Inserts an object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35}" />.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        /// <param name="item26">Item 26 of the tuple.</param>
        /// <param name="item27">Item 27 of the tuple.</param>
        /// <param name="item28">Item 28 of the tuple.</param>
        /// <param name="item29">Item 29 of the tuple.</param>
        /// <param name="item30">Item 30 of the tuple.</param>
        /// <param name="item31">Item 31 of the tuple.</param>
        /// <param name="item32">Item 32 of the tuple.</param>
        /// <param name="item33">Item 33 of the tuple.</param>
        /// <param name="item34">Item 34 of the tuple.</param>
        /// <param name="item35">Item 35 of the tuple.</param>
        public void Enqueue(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4,
            T5 item5,
            T6 item6,
            T7 item7,
            T8 item8,
            T9 item9,
            T10 item10,
            T11 item11,
            T12 item12,
            T13 item13,
            T14 item14,
            T15 item15,
            T16 item16,
            T17 item17,
            T18 item18,
            T19 item19,
            T20 item20,
            T21 item21,
            T22 item22,
            T23 item23,
            T24 item24,
            T25 item25,
            T26 item26,
            T27 item27,
            T28 item28,
            T29 item29,
            T30 item30,
            T31 item31,
            T32 item32,
            T33 item33,
            T34 item34,
            T35 item35)
        {
            Enqueue(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34, T35>>>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34, T35>>>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34, T35>>>(item15, item16, item17, item18, item19, item20, item21, new Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34, T35>>(item22, item23, item24, item25, item26, item27, item28, new Tuple<T29, T30, T31, T32, T33, T34, T35>(item29, item30, item31, item32, item33, item34, item35))))));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        /// <param name="item26">Item 26 of the tuple.</param>
        /// <param name="item27">Item 27 of the tuple.</param>
        /// <param name="item28">Item 28 of the tuple.</param>
        /// <param name="item29">Item 29 of the tuple.</param>
        /// <param name="item30">Item 30 of the tuple.</param>
        /// <param name="item31">Item 31 of the tuple.</param>
        /// <param name="item32">Item 32 of the tuple.</param>
        /// <param name="item33">Item 33 of the tuple.</param>
        /// <param name="item34">Item 34 of the tuple.</param>
        /// <param name="item35">Item 35 of the tuple.</param>
        public void Dequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22,
            out T23 item23,
            out T24 item24,
            out T25 item25,
            out T26 item26,
            out T27 item27,
            out T28 item28,
            out T29 item29,
            out T30 item30,
            out T31 item31,
            out T32 item32,
            out T33 item33,
            out T34 item34,
            out T35 item35)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34, T35>>>>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            item23 = tuple.Rest.Rest.Rest.Item2;
            item24 = tuple.Rest.Rest.Rest.Item3;
            item25 = tuple.Rest.Rest.Rest.Item4;
            item26 = tuple.Rest.Rest.Rest.Item5;
            item27 = tuple.Rest.Rest.Rest.Item6;
            item28 = tuple.Rest.Rest.Rest.Item7;
            item29 = tuple.Rest.Rest.Rest.Rest.Item1;
            item30 = tuple.Rest.Rest.Rest.Rest.Item2;
            item31 = tuple.Rest.Rest.Rest.Rest.Item3;
            item32 = tuple.Rest.Rest.Rest.Rest.Item4;
            item33 = tuple.Rest.Rest.Rest.Rest.Item5;
            item34 = tuple.Rest.Rest.Rest.Rest.Item6;
            item35 = tuple.Rest.Rest.Rest.Rest.Item7;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        /// <param name="item26">Item 26 of the tuple.</param>
        /// <param name="item27">Item 27 of the tuple.</param>
        /// <param name="item28">Item 28 of the tuple.</param>
        /// <param name="item29">Item 29 of the tuple.</param>
        /// <param name="item30">Item 30 of the tuple.</param>
        /// <param name="item31">Item 31 of the tuple.</param>
        /// <param name="item32">Item 32 of the tuple.</param>
        /// <param name="item33">Item 33 of the tuple.</param>
        /// <param name="item34">Item 34 of the tuple.</param>
        /// <param name="item35">Item 35 of the tuple.</param>
        public void Peek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22,
            out T23 item23,
            out T24 item24,
            out T25 item25,
            out T26 item26,
            out T27 item27,
            out T28 item28,
            out T29 item29,
            out T30 item30,
            out T31 item31,
            out T32 item32,
            out T33 item33,
            out T34 item34,
            out T35 item35)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34, T35>>>>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            item23 = tuple.Rest.Rest.Rest.Item2;
            item24 = tuple.Rest.Rest.Rest.Item3;
            item25 = tuple.Rest.Rest.Rest.Item4;
            item26 = tuple.Rest.Rest.Rest.Item5;
            item27 = tuple.Rest.Rest.Rest.Item6;
            item28 = tuple.Rest.Rest.Rest.Item7;
            item29 = tuple.Rest.Rest.Rest.Rest.Item1;
            item30 = tuple.Rest.Rest.Rest.Rest.Item2;
            item31 = tuple.Rest.Rest.Rest.Rest.Item3;
            item32 = tuple.Rest.Rest.Rest.Rest.Item4;
            item33 = tuple.Rest.Rest.Rest.Rest.Item5;
            item34 = tuple.Rest.Rest.Rest.Rest.Item6;
            item35 = tuple.Rest.Rest.Rest.Rest.Item7;
            // ReSharper restore PossibleNullReferenceException
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        /// <param name="item26">Item 26 of the tuple.</param>
        /// <param name="item27">Item 27 of the tuple.</param>
        /// <param name="item28">Item 28 of the tuple.</param>
        /// <param name="item29">Item 29 of the tuple.</param>
        /// <param name="item30">Item 30 of the tuple.</param>
        /// <param name="item31">Item 31 of the tuple.</param>
        /// <param name="item32">Item 32 of the tuple.</param>
        /// <param name="item33">Item 33 of the tuple.</param>
        /// <param name="item34">Item 34 of the tuple.</param>
        /// <param name="item35">Item 35 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryDequeue(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22,
            out T23 item23,
            out T24 item24,
            out T25 item25,
            out T26 item26,
            out T27 item27,
            out T28 item28,
            out T29 item29,
            out T30 item30,
            out T31 item31,
            out T32 item32,
            out T33 item33,
            out T34 item34,
            out T35 item35)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                item9 = default(T9);
                item10 = default(T10);
                item11 = default(T11);
                item12 = default(T12);
                item13 = default(T13);
                item14 = default(T14);
                item15 = default(T15);
                item16 = default(T16);
                item17 = default(T17);
                item18 = default(T18);
                item19 = default(T19);
                item20 = default(T20);
                item21 = default(T21);
                item22 = default(T22);
                item23 = default(T23);
                item24 = default(T24);
                item25 = default(T25);
                item26 = default(T26);
                item27 = default(T27);
                item28 = default(T28);
                item29 = default(T29);
                item30 = default(T30);
                item31 = default(T31);
                item32 = default(T32);
                item33 = default(T33);
                item34 = default(T34);
                item35 = default(T35);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34, T35>>>>> tuple = Dequeue();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            item23 = tuple.Rest.Rest.Rest.Item2;
            item24 = tuple.Rest.Rest.Rest.Item3;
            item25 = tuple.Rest.Rest.Rest.Item4;
            item26 = tuple.Rest.Rest.Rest.Item5;
            item27 = tuple.Rest.Rest.Rest.Item6;
            item28 = tuple.Rest.Rest.Rest.Item7;
            item29 = tuple.Rest.Rest.Rest.Rest.Item1;
            item30 = tuple.Rest.Rest.Rest.Rest.Item2;
            item31 = tuple.Rest.Rest.Rest.Rest.Item3;
            item32 = tuple.Rest.Rest.Rest.Rest.Item4;
            item33 = tuple.Rest.Rest.Rest.Rest.Item5;
            item34 = tuple.Rest.Rest.Rest.Rest.Item6;
            item35 = tuple.Rest.Rest.Rest.Rest.Item7;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
        
        /// <summary>
        /// Returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <param name="item11">Item 11 of the tuple.</param>
        /// <param name="item12">Item 12 of the tuple.</param>
        /// <param name="item13">Item 13 of the tuple.</param>
        /// <param name="item14">Item 14 of the tuple.</param>
        /// <param name="item15">Item 15 of the tuple.</param>
        /// <param name="item16">Item 16 of the tuple.</param>
        /// <param name="item17">Item 17 of the tuple.</param>
        /// <param name="item18">Item 18 of the tuple.</param>
        /// <param name="item19">Item 19 of the tuple.</param>
        /// <param name="item20">Item 20 of the tuple.</param>
        /// <param name="item21">Item 21 of the tuple.</param>
        /// <param name="item22">Item 22 of the tuple.</param>
        /// <param name="item23">Item 23 of the tuple.</param>
        /// <param name="item24">Item 24 of the tuple.</param>
        /// <param name="item25">Item 25 of the tuple.</param>
        /// <param name="item26">Item 26 of the tuple.</param>
        /// <param name="item27">Item 27 of the tuple.</param>
        /// <param name="item28">Item 28 of the tuple.</param>
        /// <param name="item29">Item 29 of the tuple.</param>
        /// <param name="item30">Item 30 of the tuple.</param>
        /// <param name="item31">Item 31 of the tuple.</param>
        /// <param name="item32">Item 32 of the tuple.</param>
        /// <param name="item33">Item 33 of the tuple.</param>
        /// <param name="item34">Item 34 of the tuple.</param>
        /// <param name="item35">Item 35 of the tuple.</param>
        /// <returns><see langword="true"/> if the queue was not empty; otherwise <see langword="false"/>.</returns>
        public bool TryPeek(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8,
            out T9 item9,
            out T10 item10,
            out T11 item11,
            out T12 item12,
            out T13 item13,
            out T14 item14,
            out T15 item15,
            out T16 item16,
            out T17 item17,
            out T18 item18,
            out T19 item19,
            out T20 item20,
            out T21 item21,
            out T22 item22,
            out T23 item23,
            out T24 item24,
            out T25 item25,
            out T26 item26,
            out T27 item27,
            out T28 item28,
            out T29 item29,
            out T30 item30,
            out T31 item31,
            out T32 item32,
            out T33 item33,
            out T34 item34,
            out T35 item35)
        {
            if (Count < 1) 
            {
                item1 = default(T1);
                item2 = default(T2);
                item3 = default(T3);
                item4 = default(T4);
                item5 = default(T5);
                item6 = default(T6);
                item7 = default(T7);
                item8 = default(T8);
                item9 = default(T9);
                item10 = default(T10);
                item11 = default(T11);
                item12 = default(T12);
                item13 = default(T13);
                item14 = default(T14);
                item15 = default(T15);
                item16 = default(T16);
                item17 = default(T17);
                item18 = default(T18);
                item19 = default(T19);
                item20 = default(T20);
                item21 = default(T21);
                item22 = default(T22);
                item23 = default(T23);
                item24 = default(T24);
                item25 = default(T25);
                item26 = default(T26);
                item27 = default(T27);
                item28 = default(T28);
                item29 = default(T29);
                item30 = default(T30);
                item31 = default(T31);
                item32 = default(T32);
                item33 = default(T33);
                item34 = default(T34);
                item35 = default(T35);
                return false;
            }
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34, T35>>>>> tuple = Peek();
            if (tuple == null) throw new NullReferenceException(Resources.Queue_TupleNull);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            // ReSharper disable PossibleNullReferenceException
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
            item17 = tuple.Rest.Rest.Item3;
            item18 = tuple.Rest.Rest.Item4;
            item19 = tuple.Rest.Rest.Item5;
            item20 = tuple.Rest.Rest.Item6;
            item21 = tuple.Rest.Rest.Item7;
            item22 = tuple.Rest.Rest.Rest.Item1;
            item23 = tuple.Rest.Rest.Rest.Item2;
            item24 = tuple.Rest.Rest.Rest.Item3;
            item25 = tuple.Rest.Rest.Rest.Item4;
            item26 = tuple.Rest.Rest.Rest.Item5;
            item27 = tuple.Rest.Rest.Rest.Item6;
            item28 = tuple.Rest.Rest.Rest.Item7;
            item29 = tuple.Rest.Rest.Rest.Rest.Item1;
            item30 = tuple.Rest.Rest.Rest.Rest.Item2;
            item31 = tuple.Rest.Rest.Rest.Rest.Item3;
            item32 = tuple.Rest.Rest.Rest.Rest.Item4;
            item33 = tuple.Rest.Rest.Rest.Rest.Item5;
            item34 = tuple.Rest.Rest.Rest.Rest.Item6;
            item35 = tuple.Rest.Rest.Rest.Rest.Item7;
            // ReSharper restore PossibleNullReferenceException
            return true;
        }
    }
    #endregion

}
 
