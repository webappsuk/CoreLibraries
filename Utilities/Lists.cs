








 
 
#region © Copyright Web Applications (UK) Ltd, 2014.  All rights reserved.
// Copyright (c) 2014, Web Applications UK Ltd
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
using System.Diagnostics.Contracts;
using JetBrains.Annotations;

namespace WebApplications.Utilities
{


	#region List with 2 items.
    /// <summary>
    /// A List of <see cref="Tuple{T1, T2}"/>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    [PublicAPI]
    public class List<T1, T2> : List<Tuple<T1, T2>>, IEnumerable<T1, T2>
    {
        /// <summary>
        /// Inserts an object at the beginning of the <see cref="Queue{T1, T2}" />.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        public void Add(
            T1 item1,
            T2 item2)
        {
            Add(new Tuple<T1, T2>(item1, item2));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2}"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        public void Get(int index, 
            out T1 item1,
            out T2 item2)
        {
            Tuple<T1, T2> tuple = this[index];
            Contract.Assert(tuple != null);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
        }
    }
	#endregion


	#region List with 3 items.
    /// <summary>
    /// A List of <see cref="Tuple{T1, T2, T3}"/>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    [PublicAPI]
    public class List<T1, T2, T3> : List<Tuple<T1, T2, T3>>, IEnumerable<T1, T2, T3>
    {
        /// <summary>
        /// Inserts an object at the beginning of the <see cref="Queue{T1, T2, T3}" />.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        public void Add(
            T1 item1,
            T2 item2,
            T3 item3)
        {
            Add(new Tuple<T1, T2, T3>(item1, item2, item3));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3}"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        public void Get(int index, 
            out T1 item1,
            out T2 item2,
            out T3 item3)
        {
            Tuple<T1, T2, T3> tuple = this[index];
            Contract.Assert(tuple != null);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
        }
    }
	#endregion


	#region List with 4 items.
    /// <summary>
    /// A List of <see cref="Tuple{T1, T2, T3, T4}"/>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    [PublicAPI]
    public class List<T1, T2, T3, T4> : List<Tuple<T1, T2, T3, T4>>, IEnumerable<T1, T2, T3, T4>
    {
        /// <summary>
        /// Inserts an object at the beginning of the <see cref="Queue{T1, T2, T3, T4}" />.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        public void Add(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4)
        {
            Add(new Tuple<T1, T2, T3, T4>(item1, item2, item3, item4));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4}"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        public void Get(int index, 
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4)
        {
            Tuple<T1, T2, T3, T4> tuple = this[index];
            Contract.Assert(tuple != null);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
        }
    }
	#endregion


	#region List with 5 items.
    /// <summary>
    /// A List of <see cref="Tuple{T1, T2, T3, T4, T5}"/>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    [PublicAPI]
    public class List<T1, T2, T3, T4, T5> : List<Tuple<T1, T2, T3, T4, T5>>, IEnumerable<T1, T2, T3, T4, T5>
    {
        /// <summary>
        /// Inserts an object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5}" />.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        public void Add(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4,
            T5 item5)
        {
            Add(new Tuple<T1, T2, T3, T4, T5>(item1, item2, item3, item4, item5));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5}"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        public void Get(int index, 
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5)
        {
            Tuple<T1, T2, T3, T4, T5> tuple = this[index];
            Contract.Assert(tuple != null);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
        }
    }
	#endregion


	#region List with 6 items.
    /// <summary>
    /// A List of <see cref="Tuple{T1, T2, T3, T4, T5, T6}"/>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    [PublicAPI]
    public class List<T1, T2, T3, T4, T5, T6> : List<Tuple<T1, T2, T3, T4, T5, T6>>, IEnumerable<T1, T2, T3, T4, T5, T6>
    {
        /// <summary>
        /// Inserts an object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6}" />.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        public void Add(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4,
            T5 item5,
            T6 item6)
        {
            Add(new Tuple<T1, T2, T3, T4, T5, T6>(item1, item2, item3, item4, item5, item6));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6}"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        public void Get(int index, 
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6)
        {
            Tuple<T1, T2, T3, T4, T5, T6> tuple = this[index];
            Contract.Assert(tuple != null);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
        }
    }
	#endregion


	#region List with 7 items.
    /// <summary>
    /// A List of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7}"/>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    [PublicAPI]
    public class List<T1, T2, T3, T4, T5, T6, T7> : List<Tuple<T1, T2, T3, T4, T5, T6, T7>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7>
    {
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
        public void Add(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4,
            T5 item5,
            T6 item6,
            T7 item7)
        {
            Add(new Tuple<T1, T2, T3, T4, T5, T6, T7>(item1, item2, item3, item4, item5, item6, item7));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7}"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        public void Get(int index, 
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7> tuple = this[index];
            Contract.Assert(tuple != null);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
        }
    }
	#endregion


	#region List with 8 items.
    /// <summary>
    /// A List of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8}"/>.
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
    public class List<T1, T2, T3, T4, T5, T6, T7, T8> : List<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8>
    {
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
        public void Add(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4,
            T5 item5,
            T6 item6,
            T7 item7,
            T8 item8)
        {
            Add(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8>(item8)));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8}"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        public void Get(int index, 
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>> tuple = this[index];
            Contract.Assert(tuple != null);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            item8 = tuple.Rest.Item1;
        }
    }
	#endregion


	#region List with 9 items.
    /// <summary>
    /// A List of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8, T9}"/>.
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
    public class List<T1, T2, T3, T4, T5, T6, T7, T8, T9> : List<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9>
    {
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
        public void Add(
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
            Add(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9>(item8, item9)));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9}"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        public void Get(int index, 
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
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9>> tuple = this[index];
            Contract.Assert(tuple != null);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
        }
    }
	#endregion


	#region List with 10 items.
    /// <summary>
    /// A List of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10}"/>.
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
    public class List<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : List<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
    {
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
        public void Add(
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
            Add(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10>(item8, item9, item10)));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10}"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
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
        public void Get(int index, 
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
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10>> tuple = this[index];
            Contract.Assert(tuple != null);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
        }
    }
	#endregion


	#region List with 11 items.
    /// <summary>
    /// A List of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11}"/>.
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
    public class List<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : List<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>
    {
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
        public void Add(
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
            Add(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11>(item8, item9, item10, item11)));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11}"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
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
        public void Get(int index, 
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
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11>> tuple = this[index];
            Contract.Assert(tuple != null);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
        }
    }
	#endregion


	#region List with 12 items.
    /// <summary>
    /// A List of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12}"/>.
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
    public class List<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : List<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>
    {
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
        public void Add(
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
            Add(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12>(item8, item9, item10, item11, item12)));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12}"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
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
        public void Get(int index, 
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
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12>> tuple = this[index];
            Contract.Assert(tuple != null);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
        }
    }
	#endregion


	#region List with 13 items.
    /// <summary>
    /// A List of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13}"/>.
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
    public class List<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> : List<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>
    {
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
        public void Add(
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
            Add(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13>(item8, item9, item10, item11, item12, item13)));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13}"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
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
        public void Get(int index, 
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
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13>> tuple = this[index];
            Contract.Assert(tuple != null);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
        }
    }
	#endregion


	#region List with 14 items.
    /// <summary>
    /// A List of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14}"/>.
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
    public class List<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> : List<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>
    {
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
        public void Add(
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
            Add(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14>(item8, item9, item10, item11, item12, item13, item14)));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14}"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
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
        public void Get(int index, 
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
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14>> tuple = this[index];
            Contract.Assert(tuple != null);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
        }
    }
	#endregion


	#region List with 15 items.
    /// <summary>
    /// A List of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15}"/>.
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
    public class List<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> : List<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15>>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>
    {
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
        public void Add(
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
            Add(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15>(item15))));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15}"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
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
        public void Get(int index, 
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
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15>>> tuple = this[index];
            Contract.Assert(tuple != null);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
        }
    }
	#endregion


	#region List with 16 items.
    /// <summary>
    /// A List of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16}"/>.
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
    public class List<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> : List<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16>>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>
    {
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
        public void Add(
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
            Add(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16>(item15, item16))));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16}"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
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
        public void Get(int index, 
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
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16>>> tuple = this[index];
            Contract.Assert(tuple != null);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
            item8 = tuple.Rest.Item1;
            item9 = tuple.Rest.Item2;
            item10 = tuple.Rest.Item3;
            item11 = tuple.Rest.Item4;
            item12 = tuple.Rest.Item5;
            item13 = tuple.Rest.Item6;
            item14 = tuple.Rest.Item7;
            item15 = tuple.Rest.Rest.Item1;
            item16 = tuple.Rest.Rest.Item2;
        }
    }
	#endregion


	#region List with 17 items.
    /// <summary>
    /// A List of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17}"/>.
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
    public class List<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17> : List<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17>>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>
    {
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
        public void Add(
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
            Add(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17>(item15, item16, item17))));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17}"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
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
        public void Get(int index, 
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
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17>>> tuple = this[index];
            Contract.Assert(tuple != null);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
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
        }
    }
	#endregion


	#region List with 18 items.
    /// <summary>
    /// A List of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18}"/>.
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
    public class List<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18> : List<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18>>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18>
    {
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
        public void Add(
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
            Add(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17, T18>(item15, item16, item17, item18))));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18}"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
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
        public void Get(int index, 
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
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18>>> tuple = this[index];
            Contract.Assert(tuple != null);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
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
        }
    }
	#endregion


	#region List with 19 items.
    /// <summary>
    /// A List of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19}"/>.
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
    public class List<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19> : List<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19>>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>
    {
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
        public void Add(
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
            Add(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17, T18, T19>(item15, item16, item17, item18, item19))));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19}"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
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
        public void Get(int index, 
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
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19>>> tuple = this[index];
            Contract.Assert(tuple != null);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
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
        }
    }
	#endregion


	#region List with 20 items.
    /// <summary>
    /// A List of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20}"/>.
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
    public class List<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20> : List<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20>>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20>
    {
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
        public void Add(
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
            Add(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17, T18, T19, T20>(item15, item16, item17, item18, item19, item20))));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20}"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
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
        public void Get(int index, 
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
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20>>> tuple = this[index];
            Contract.Assert(tuple != null);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
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
        }
    }
	#endregion


	#region List with 21 items.
    /// <summary>
    /// A List of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21}"/>.
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
    public class List<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21> : List<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21>>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21>
    {
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
        public void Add(
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
            Add(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17, T18, T19, T20, T21>(item15, item16, item17, item18, item19, item20, item21))));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21}"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
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
        public void Get(int index, 
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
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21>>> tuple = this[index];
            Contract.Assert(tuple != null);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
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
        }
    }
	#endregion


	#region List with 22 items.
    /// <summary>
    /// A List of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22}"/>.
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
    public class List<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22> : List<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22>>>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22>
    {
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
        public void Add(
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
            Add(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22>>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22>>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22>>(item15, item16, item17, item18, item19, item20, item21, new Tuple<T22>(item22)))));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22}"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
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
        public void Get(int index, 
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
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22>>>> tuple = this[index];
            Contract.Assert(tuple != null);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
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
        }
    }
	#endregion


	#region List with 23 items.
    /// <summary>
    /// A List of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23}"/>.
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
    public class List<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23> : List<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23>>>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23>
    {
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
        public void Add(
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
            Add(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23>>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23>>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23>>(item15, item16, item17, item18, item19, item20, item21, new Tuple<T22, T23>(item22, item23)))));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23}"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
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
        public void Get(int index, 
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
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23>>>> tuple = this[index];
            Contract.Assert(tuple != null);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
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
        }
    }
	#endregion


	#region List with 24 items.
    /// <summary>
    /// A List of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24}"/>.
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
    public class List<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24> : List<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24>>>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24>
    {
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
        public void Add(
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
            Add(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24>>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24>>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24>>(item15, item16, item17, item18, item19, item20, item21, new Tuple<T22, T23, T24>(item22, item23, item24)))));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24}"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
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
        public void Get(int index, 
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
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24>>>> tuple = this[index];
            Contract.Assert(tuple != null);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
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
        }
    }
	#endregion


	#region List with 25 items.
    /// <summary>
    /// A List of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25}"/>.
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
    public class List<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25> : List<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25>>>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25>
    {
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
        public void Add(
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
            Add(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25>>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25>>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25>>(item15, item16, item17, item18, item19, item20, item21, new Tuple<T22, T23, T24, T25>(item22, item23, item24, item25)))));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25}"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
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
        public void Get(int index, 
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
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25>>>> tuple = this[index];
            Contract.Assert(tuple != null);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
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
        }
    }
	#endregion


	#region List with 26 items.
    /// <summary>
    /// A List of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26}"/>.
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
    public class List<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26> : List<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26>>>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26>
    {
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
        public void Add(
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
            Add(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26>>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26>>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26>>(item15, item16, item17, item18, item19, item20, item21, new Tuple<T22, T23, T24, T25, T26>(item22, item23, item24, item25, item26)))));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26}"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
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
        public void Get(int index, 
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
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26>>>> tuple = this[index];
            Contract.Assert(tuple != null);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
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
        }
    }
	#endregion


	#region List with 27 items.
    /// <summary>
    /// A List of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27}"/>.
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
    public class List<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27> : List<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27>>>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27>
    {
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
        public void Add(
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
            Add(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27>>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27>>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27>>(item15, item16, item17, item18, item19, item20, item21, new Tuple<T22, T23, T24, T25, T26, T27>(item22, item23, item24, item25, item26, item27)))));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27}"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
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
        public void Get(int index, 
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
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27>>>> tuple = this[index];
            Contract.Assert(tuple != null);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
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
        }
    }
	#endregion


	#region List with 28 items.
    /// <summary>
    /// A List of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28}"/>.
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
    public class List<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28> : List<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28>>>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28>
    {
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
        public void Add(
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
            Add(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28>>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28>>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28>>(item15, item16, item17, item18, item19, item20, item21, new Tuple<T22, T23, T24, T25, T26, T27, T28>(item22, item23, item24, item25, item26, item27, item28)))));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28}"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
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
        public void Get(int index, 
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
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28>>>> tuple = this[index];
            Contract.Assert(tuple != null);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
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
        }
    }
	#endregion


	#region List with 29 items.
    /// <summary>
    /// A List of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29}"/>.
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
    public class List<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29> : List<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29>>>>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29>
    {
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
        public void Add(
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
            Add(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29>>>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29>>>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29>>>(item15, item16, item17, item18, item19, item20, item21, new Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29>>(item22, item23, item24, item25, item26, item27, item28, new Tuple<T29>(item29))))));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29}"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
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
        public void Get(int index, 
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
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29>>>>> tuple = this[index];
            Contract.Assert(tuple != null);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
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
        }
    }
	#endregion


	#region List with 30 items.
    /// <summary>
    /// A List of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30}"/>.
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
    public class List<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30> : List<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30>>>>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30>
    {
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
        public void Add(
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
            Add(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30>>>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30>>>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30>>>(item15, item16, item17, item18, item19, item20, item21, new Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30>>(item22, item23, item24, item25, item26, item27, item28, new Tuple<T29, T30>(item29, item30))))));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30}"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
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
        public void Get(int index, 
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
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30>>>>> tuple = this[index];
            Contract.Assert(tuple != null);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
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
        }
    }
	#endregion


	#region List with 31 items.
    /// <summary>
    /// A List of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31}"/>.
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
    public class List<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31> : List<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31>>>>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31>
    {
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
        public void Add(
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
            Add(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31>>>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31>>>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31>>>(item15, item16, item17, item18, item19, item20, item21, new Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31>>(item22, item23, item24, item25, item26, item27, item28, new Tuple<T29, T30, T31>(item29, item30, item31))))));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31}"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
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
        public void Get(int index, 
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
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31>>>>> tuple = this[index];
            Contract.Assert(tuple != null);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
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
        }
    }
	#endregion


	#region List with 32 items.
    /// <summary>
    /// A List of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32}"/>.
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
    public class List<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32> : List<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32>>>>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32>
    {
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
        public void Add(
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
            Add(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32>>>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32>>>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32>>>(item15, item16, item17, item18, item19, item20, item21, new Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32>>(item22, item23, item24, item25, item26, item27, item28, new Tuple<T29, T30, T31, T32>(item29, item30, item31, item32))))));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32}"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
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
        public void Get(int index, 
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
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32>>>>> tuple = this[index];
            Contract.Assert(tuple != null);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
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
        }
    }
	#endregion


	#region List with 33 items.
    /// <summary>
    /// A List of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33}"/>.
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
    public class List<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33> : List<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33>>>>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33>
    {
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
        public void Add(
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
            Add(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33>>>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33>>>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33>>>(item15, item16, item17, item18, item19, item20, item21, new Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33>>(item22, item23, item24, item25, item26, item27, item28, new Tuple<T29, T30, T31, T32, T33>(item29, item30, item31, item32, item33))))));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33}"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
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
        public void Get(int index, 
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
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33>>>>> tuple = this[index];
            Contract.Assert(tuple != null);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
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
        }
    }
	#endregion


	#region List with 34 items.
    /// <summary>
    /// A List of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34}"/>.
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
    public class List<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34> : List<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34>>>>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34>
    {
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
        public void Add(
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
            Add(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34>>>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34>>>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34>>>(item15, item16, item17, item18, item19, item20, item21, new Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34>>(item22, item23, item24, item25, item26, item27, item28, new Tuple<T29, T30, T31, T32, T33, T34>(item29, item30, item31, item32, item33, item34))))));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34}"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
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
        public void Get(int index, 
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
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34>>>>> tuple = this[index];
            Contract.Assert(tuple != null);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
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
        }
    }
	#endregion


	#region List with 35 items.
    /// <summary>
    /// A List of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35}"/>.
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
    public class List<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35> : List<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34, T35>>>>>>, IEnumerable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35>
    {
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
        public void Add(
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
            Add(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34, T35>>>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34, T35>>>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34, T35>>>(item15, item16, item17, item18, item19, item20, item21, new Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34, T35>>(item22, item23, item24, item25, item26, item27, item28, new Tuple<T29, T30, T31, T32, T33, T34, T35>(item29, item30, item31, item32, item33, item34, item35))))));
        }
        
        /// <summary>
        /// Removes and returns the object at the beginning of the <see cref="Queue{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35}"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
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
        public void Get(int index, 
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
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34, T35>>>>> tuple = this[index];
            Contract.Assert(tuple != null);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
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
        }
    }
	#endregion

}
 
