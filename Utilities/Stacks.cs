 
 
#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities
// File: TaskExtensionOverloads.cs
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
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using JetBrains.Annotations;

namespace WebApplications.Utilities
{

	#region Stack with 2 items.
    /// <summary>
    /// A stack of <see cref="Tuple{T1, T2}"/>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    [UsedImplicitly]
    public class Stack<T1, T2> : Stack<Tuple<T1, T2>>
    {
        /// <summary>
        /// Inserts an object at the top of the <see cref="Stack{T1, T2}" />.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        public void Push(
            T1 item1,
            T2 item2)
        {
            Push(new Tuple<T1, T2>(item1, item2));
        }
	    
        /// <summary>
        /// Removes and returns the object at the top of the <see cref="Stack{T1, T2}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        public void Pop(
            out T1 item1,
            out T2 item2)
        {
            Tuple<T1, T2> tuple = Pop();
            Contract.Assert(tuple != null);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
        }
	    
        /// <summary>
        /// Returns the object at the top of the <see cref="Stack{T1, T2}"/> without removing it.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        public void Peek(
            out T1 item1,
            out T2 item2)
        {
            Tuple<T1, T2> tuple = Peek();
            Contract.Assert(tuple != null);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
        }
    }
	#endregion

	#region Stack with 3 items.
    /// <summary>
    /// A stack of <see cref="Tuple{T1, T2, T3}"/>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    [UsedImplicitly]
    public class Stack<T1, T2, T3> : Stack<Tuple<T1, T2, T3>>
    {
        /// <summary>
        /// Inserts an object at the top of the <see cref="Stack{T1, T2, T3}" />.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        public void Push(
            T1 item1,
            T2 item2,
            T3 item3)
        {
            Push(new Tuple<T1, T2, T3>(item1, item2, item3));
        }
	    
        /// <summary>
        /// Removes and returns the object at the top of the <see cref="Stack{T1, T2, T3}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        public void Pop(
            out T1 item1,
            out T2 item2,
            out T3 item3)
        {
            Tuple<T1, T2, T3> tuple = Pop();
            Contract.Assert(tuple != null);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
        }
	    
        /// <summary>
        /// Returns the object at the top of the <see cref="Stack{T1, T2, T3}"/> without removing it.
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
            Contract.Assert(tuple != null);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
        }
    }
	#endregion

	#region Stack with 4 items.
    /// <summary>
    /// A stack of <see cref="Tuple{T1, T2, T3, T4}"/>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    [UsedImplicitly]
    public class Stack<T1, T2, T3, T4> : Stack<Tuple<T1, T2, T3, T4>>
    {
        /// <summary>
        /// Inserts an object at the top of the <see cref="Stack{T1, T2, T3, T4}" />.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        public void Push(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4)
        {
            Push(new Tuple<T1, T2, T3, T4>(item1, item2, item3, item4));
        }
	    
        /// <summary>
        /// Removes and returns the object at the top of the <see cref="Stack{T1, T2, T3, T4}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        public void Pop(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4)
        {
            Tuple<T1, T2, T3, T4> tuple = Pop();
            Contract.Assert(tuple != null);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
        }
	    
        /// <summary>
        /// Returns the object at the top of the <see cref="Stack{T1, T2, T3, T4}"/> without removing it.
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
            Contract.Assert(tuple != null);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
        }
    }
	#endregion

	#region Stack with 5 items.
    /// <summary>
    /// A stack of <see cref="Tuple{T1, T2, T3, T4, T5}"/>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    [UsedImplicitly]
    public class Stack<T1, T2, T3, T4, T5> : Stack<Tuple<T1, T2, T3, T4, T5>>
    {
        /// <summary>
        /// Inserts an object at the top of the <see cref="Stack{T1, T2, T3, T4, T5}" />.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        public void Push(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4,
            T5 item5)
        {
            Push(new Tuple<T1, T2, T3, T4, T5>(item1, item2, item3, item4, item5));
        }
	    
        /// <summary>
        /// Removes and returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        public void Pop(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5)
        {
            Tuple<T1, T2, T3, T4, T5> tuple = Pop();
            Contract.Assert(tuple != null);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
        }
	    
        /// <summary>
        /// Returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5}"/> without removing it.
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
            Contract.Assert(tuple != null);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
        }
    }
	#endregion

	#region Stack with 6 items.
    /// <summary>
    /// A stack of <see cref="Tuple{T1, T2, T3, T4, T5, T6}"/>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    [UsedImplicitly]
    public class Stack<T1, T2, T3, T4, T5, T6> : Stack<Tuple<T1, T2, T3, T4, T5, T6>>
    {
        /// <summary>
        /// Inserts an object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6}" />.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        public void Push(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4,
            T5 item5,
            T6 item6)
        {
            Push(new Tuple<T1, T2, T3, T4, T5, T6>(item1, item2, item3, item4, item5, item6));
        }
	    
        /// <summary>
        /// Removes and returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        public void Pop(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6)
        {
            Tuple<T1, T2, T3, T4, T5, T6> tuple = Pop();
            Contract.Assert(tuple != null);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
        }
	    
        /// <summary>
        /// Returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6}"/> without removing it.
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

	#region Stack with 7 items.
    /// <summary>
    /// A stack of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7}"/>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    [UsedImplicitly]
    public class Stack<T1, T2, T3, T4, T5, T6, T7> : Stack<Tuple<T1, T2, T3, T4, T5, T6, T7>>
    {
        /// <summary>
        /// Inserts an object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7}" />.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        public void Push(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4,
            T5 item5,
            T6 item6,
            T7 item7)
        {
            Push(new Tuple<T1, T2, T3, T4, T5, T6, T7>(item1, item2, item3, item4, item5, item6, item7));
        }
	    
        /// <summary>
        /// Removes and returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        public void Pop(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7> tuple = Pop();
            Contract.Assert(tuple != null);
            item1 = tuple.Item1;
            item2 = tuple.Item2;
            item3 = tuple.Item3;
            item4 = tuple.Item4;
            item5 = tuple.Item5;
            item6 = tuple.Item6;
            item7 = tuple.Item7;
        }
	    
        /// <summary>
        /// Returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7}"/> without removing it.
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

	#region Stack with 8 items.
    /// <summary>
    /// A stack of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8}"/>.
    /// </summary>
    /// <typeparam name="T1">The type of item 1.</typeparam>
    /// <typeparam name="T2">The type of item 2.</typeparam>
    /// <typeparam name="T3">The type of item 3.</typeparam>
    /// <typeparam name="T4">The type of item 4.</typeparam>
    /// <typeparam name="T5">The type of item 5.</typeparam>
    /// <typeparam name="T6">The type of item 6.</typeparam>
    /// <typeparam name="T7">The type of item 7.</typeparam>
    /// <typeparam name="T8">The type of item 8.</typeparam>
    [UsedImplicitly]
    public class Stack<T1, T2, T3, T4, T5, T6, T7, T8> : Stack<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>>>
    {
        /// <summary>
        /// Inserts an object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8}" />.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        public void Push(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4,
            T5 item5,
            T6 item6,
            T7 item7,
            T8 item8)
        {
            Push(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8>(item8)));
        }
	    
        /// <summary>
        /// Removes and returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8}"/>.
        /// </summary>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        public void Pop(
            out T1 item1,
            out T2 item2,
            out T3 item3,
            out T4 item4,
            out T5 item5,
            out T6 item6,
            out T7 item7,
            out T8 item8)
        {
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>> tuple = Pop();
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
	    
        /// <summary>
        /// Returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8}"/> without removing it.
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

	#region Stack with 9 items.
    /// <summary>
    /// A stack of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8, T9}"/>.
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
    [UsedImplicitly]
    public class Stack<T1, T2, T3, T4, T5, T6, T7, T8, T9> : Stack<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9>>>
    {
        /// <summary>
        /// Inserts an object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9}" />.
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
        public void Push(
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
            Push(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9>(item8, item9)));
        }
	    
        /// <summary>
        /// Removes and returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9}"/>.
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
        public void Pop(
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
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9>> tuple = Pop();
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
	    
        /// <summary>
        /// Returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9}"/> without removing it.
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

	#region Stack with 10 items.
    /// <summary>
    /// A stack of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10}"/>.
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
    [UsedImplicitly]
    public class Stack<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : Stack<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10>>>
    {
        /// <summary>
        /// Inserts an object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10}" />.
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
        public void Push(
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
            Push(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10>(item8, item9, item10)));
        }
	    
        /// <summary>
        /// Removes and returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10}"/>.
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
        public void Pop(
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
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10>> tuple = Pop();
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
	    
        /// <summary>
        /// Returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10}"/> without removing it.
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

	#region Stack with 11 items.
    /// <summary>
    /// A stack of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11}"/>.
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
    [UsedImplicitly]
    public class Stack<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : Stack<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11>>>
    {
        /// <summary>
        /// Inserts an object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11}" />.
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
        public void Push(
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
            Push(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11>(item8, item9, item10, item11)));
        }
	    
        /// <summary>
        /// Removes and returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11}"/>.
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
        public void Pop(
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
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11>> tuple = Pop();
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
	    
        /// <summary>
        /// Returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11}"/> without removing it.
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

	#region Stack with 12 items.
    /// <summary>
    /// A stack of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12}"/>.
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
    [UsedImplicitly]
    public class Stack<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : Stack<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12>>>
    {
        /// <summary>
        /// Inserts an object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12}" />.
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
        public void Push(
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
            Push(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12>(item8, item9, item10, item11, item12)));
        }
	    
        /// <summary>
        /// Removes and returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12}"/>.
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
        public void Pop(
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
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12>> tuple = Pop();
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
	    
        /// <summary>
        /// Returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12}"/> without removing it.
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

	#region Stack with 13 items.
    /// <summary>
    /// A stack of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13}"/>.
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
    [UsedImplicitly]
    public class Stack<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> : Stack<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13>>>
    {
        /// <summary>
        /// Inserts an object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13}" />.
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
        public void Push(
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
            Push(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13>(item8, item9, item10, item11, item12, item13)));
        }
	    
        /// <summary>
        /// Removes and returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13}"/>.
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
        public void Pop(
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
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13>> tuple = Pop();
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
	    
        /// <summary>
        /// Returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13}"/> without removing it.
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

	#region Stack with 14 items.
    /// <summary>
    /// A stack of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14}"/>.
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
    [UsedImplicitly]
    public class Stack<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> : Stack<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14>>>
    {
        /// <summary>
        /// Inserts an object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14}" />.
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
        public void Push(
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
            Push(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14>(item8, item9, item10, item11, item12, item13, item14)));
        }
	    
        /// <summary>
        /// Removes and returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14}"/>.
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
        public void Pop(
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
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14>> tuple = Pop();
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
	    
        /// <summary>
        /// Returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14}"/> without removing it.
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

	#region Stack with 15 items.
    /// <summary>
    /// A stack of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15}"/>.
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
    [UsedImplicitly]
    public class Stack<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> : Stack<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15>>>>
    {
        /// <summary>
        /// Inserts an object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15}" />.
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
        public void Push(
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
            Push(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15>(item15))));
        }
	    
        /// <summary>
        /// Removes and returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15}"/>.
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
        public void Pop(
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
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15>>> tuple = Pop();
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
	    
        /// <summary>
        /// Returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15}"/> without removing it.
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

	#region Stack with 16 items.
    /// <summary>
    /// A stack of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16}"/>.
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
    [UsedImplicitly]
    public class Stack<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> : Stack<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16>>>>
    {
        /// <summary>
        /// Inserts an object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16}" />.
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
        public void Push(
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
            Push(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16>(item15, item16))));
        }
	    
        /// <summary>
        /// Removes and returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16}"/>.
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
        public void Pop(
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
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16>>> tuple = Pop();
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
	    
        /// <summary>
        /// Returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16}"/> without removing it.
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

	#region Stack with 17 items.
    /// <summary>
    /// A stack of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17}"/>.
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
    [UsedImplicitly]
    public class Stack<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17> : Stack<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17>>>>
    {
        /// <summary>
        /// Inserts an object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17}" />.
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
        public void Push(
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
            Push(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17>(item15, item16, item17))));
        }
	    
        /// <summary>
        /// Removes and returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17}"/>.
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
        public void Pop(
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
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17>>> tuple = Pop();
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
	    
        /// <summary>
        /// Returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17}"/> without removing it.
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

	#region Stack with 18 items.
    /// <summary>
    /// A stack of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18}"/>.
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
    [UsedImplicitly]
    public class Stack<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18> : Stack<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18>>>>
    {
        /// <summary>
        /// Inserts an object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18}" />.
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
        public void Push(
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
            Push(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17, T18>(item15, item16, item17, item18))));
        }
	    
        /// <summary>
        /// Removes and returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18}"/>.
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
        public void Pop(
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
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18>>> tuple = Pop();
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
	    
        /// <summary>
        /// Returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18}"/> without removing it.
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

	#region Stack with 19 items.
    /// <summary>
    /// A stack of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19}"/>.
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
    [UsedImplicitly]
    public class Stack<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19> : Stack<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19>>>>
    {
        /// <summary>
        /// Inserts an object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19}" />.
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
        public void Push(
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
            Push(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17, T18, T19>(item15, item16, item17, item18, item19))));
        }
	    
        /// <summary>
        /// Removes and returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19}"/>.
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
        public void Pop(
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
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19>>> tuple = Pop();
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
	    
        /// <summary>
        /// Returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19}"/> without removing it.
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

	#region Stack with 20 items.
    /// <summary>
    /// A stack of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20}"/>.
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
    [UsedImplicitly]
    public class Stack<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20> : Stack<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20>>>>
    {
        /// <summary>
        /// Inserts an object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20}" />.
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
        public void Push(
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
            Push(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17, T18, T19, T20>(item15, item16, item17, item18, item19, item20))));
        }
	    
        /// <summary>
        /// Removes and returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20}"/>.
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
        public void Pop(
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
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20>>> tuple = Pop();
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
	    
        /// <summary>
        /// Returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20}"/> without removing it.
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

	#region Stack with 21 items.
    /// <summary>
    /// A stack of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21}"/>.
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
    [UsedImplicitly]
    public class Stack<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21> : Stack<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21>>>>
    {
        /// <summary>
        /// Inserts an object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21}" />.
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
        public void Push(
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
            Push(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17, T18, T19, T20, T21>(item15, item16, item17, item18, item19, item20, item21))));
        }
	    
        /// <summary>
        /// Removes and returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21}"/>.
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
        public void Pop(
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
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21>>> tuple = Pop();
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
	    
        /// <summary>
        /// Returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21}"/> without removing it.
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

	#region Stack with 22 items.
    /// <summary>
    /// A stack of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22}"/>.
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
    [UsedImplicitly]
    public class Stack<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22> : Stack<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22>>>>>
    {
        /// <summary>
        /// Inserts an object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22}" />.
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
        public void Push(
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
            Push(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22>>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22>>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22>>(item15, item16, item17, item18, item19, item20, item21, new Tuple<T22>(item22)))));
        }
	    
        /// <summary>
        /// Removes and returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22}"/>.
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
        public void Pop(
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
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22>>>> tuple = Pop();
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
	    
        /// <summary>
        /// Returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22}"/> without removing it.
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

	#region Stack with 23 items.
    /// <summary>
    /// A stack of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23}"/>.
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
    [UsedImplicitly]
    public class Stack<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23> : Stack<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23>>>>>
    {
        /// <summary>
        /// Inserts an object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23}" />.
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
        public void Push(
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
            Push(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23>>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23>>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23>>(item15, item16, item17, item18, item19, item20, item21, new Tuple<T22, T23>(item22, item23)))));
        }
	    
        /// <summary>
        /// Removes and returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23}"/>.
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
        public void Pop(
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
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23>>>> tuple = Pop();
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
	    
        /// <summary>
        /// Returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23}"/> without removing it.
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

	#region Stack with 24 items.
    /// <summary>
    /// A stack of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24}"/>.
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
    [UsedImplicitly]
    public class Stack<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24> : Stack<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24>>>>>
    {
        /// <summary>
        /// Inserts an object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24}" />.
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
        public void Push(
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
            Push(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24>>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24>>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24>>(item15, item16, item17, item18, item19, item20, item21, new Tuple<T22, T23, T24>(item22, item23, item24)))));
        }
	    
        /// <summary>
        /// Removes and returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24}"/>.
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
        public void Pop(
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
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24>>>> tuple = Pop();
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
	    
        /// <summary>
        /// Returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24}"/> without removing it.
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

	#region Stack with 25 items.
    /// <summary>
    /// A stack of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25}"/>.
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
    [UsedImplicitly]
    public class Stack<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25> : Stack<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25>>>>>
    {
        /// <summary>
        /// Inserts an object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25}" />.
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
        public void Push(
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
            Push(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25>>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25>>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25>>(item15, item16, item17, item18, item19, item20, item21, new Tuple<T22, T23, T24, T25>(item22, item23, item24, item25)))));
        }
	    
        /// <summary>
        /// Removes and returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25}"/>.
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
        public void Pop(
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
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25>>>> tuple = Pop();
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
	    
        /// <summary>
        /// Returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25}"/> without removing it.
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

	#region Stack with 26 items.
    /// <summary>
    /// A stack of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26}"/>.
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
    [UsedImplicitly]
    public class Stack<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26> : Stack<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26>>>>>
    {
        /// <summary>
        /// Inserts an object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26}" />.
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
        public void Push(
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
            Push(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26>>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26>>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26>>(item15, item16, item17, item18, item19, item20, item21, new Tuple<T22, T23, T24, T25, T26>(item22, item23, item24, item25, item26)))));
        }
	    
        /// <summary>
        /// Removes and returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26}"/>.
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
        public void Pop(
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
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26>>>> tuple = Pop();
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
	    
        /// <summary>
        /// Returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26}"/> without removing it.
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

	#region Stack with 27 items.
    /// <summary>
    /// A stack of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27}"/>.
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
    [UsedImplicitly]
    public class Stack<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27> : Stack<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27>>>>>
    {
        /// <summary>
        /// Inserts an object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27}" />.
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
        public void Push(
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
            Push(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27>>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27>>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27>>(item15, item16, item17, item18, item19, item20, item21, new Tuple<T22, T23, T24, T25, T26, T27>(item22, item23, item24, item25, item26, item27)))));
        }
	    
        /// <summary>
        /// Removes and returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27}"/>.
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
        public void Pop(
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
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27>>>> tuple = Pop();
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
	    
        /// <summary>
        /// Returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27}"/> without removing it.
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

	#region Stack with 28 items.
    /// <summary>
    /// A stack of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28}"/>.
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
    [UsedImplicitly]
    public class Stack<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28> : Stack<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28>>>>>
    {
        /// <summary>
        /// Inserts an object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28}" />.
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
        public void Push(
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
            Push(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28>>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28>>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28>>(item15, item16, item17, item18, item19, item20, item21, new Tuple<T22, T23, T24, T25, T26, T27, T28>(item22, item23, item24, item25, item26, item27, item28)))));
        }
	    
        /// <summary>
        /// Removes and returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28}"/>.
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
        public void Pop(
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
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28>>>> tuple = Pop();
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
	    
        /// <summary>
        /// Returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28}"/> without removing it.
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

	#region Stack with 29 items.
    /// <summary>
    /// A stack of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29}"/>.
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
    [UsedImplicitly]
    public class Stack<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29> : Stack<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29>>>>>>
    {
        /// <summary>
        /// Inserts an object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29}" />.
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
        public void Push(
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
            Push(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29>>>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29>>>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29>>>(item15, item16, item17, item18, item19, item20, item21, new Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29>>(item22, item23, item24, item25, item26, item27, item28, new Tuple<T29>(item29))))));
        }
	    
        /// <summary>
        /// Removes and returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29}"/>.
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
        public void Pop(
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
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29>>>>> tuple = Pop();
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
	    
        /// <summary>
        /// Returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29}"/> without removing it.
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

	#region Stack with 30 items.
    /// <summary>
    /// A stack of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30}"/>.
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
    [UsedImplicitly]
    public class Stack<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30> : Stack<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30>>>>>>
    {
        /// <summary>
        /// Inserts an object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30}" />.
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
        public void Push(
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
            Push(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30>>>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30>>>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30>>>(item15, item16, item17, item18, item19, item20, item21, new Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30>>(item22, item23, item24, item25, item26, item27, item28, new Tuple<T29, T30>(item29, item30))))));
        }
	    
        /// <summary>
        /// Removes and returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30}"/>.
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
        public void Pop(
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
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30>>>>> tuple = Pop();
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
	    
        /// <summary>
        /// Returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30}"/> without removing it.
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

	#region Stack with 31 items.
    /// <summary>
    /// A stack of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31}"/>.
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
    [UsedImplicitly]
    public class Stack<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31> : Stack<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31>>>>>>
    {
        /// <summary>
        /// Inserts an object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31}" />.
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
        public void Push(
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
            Push(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31>>>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31>>>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31>>>(item15, item16, item17, item18, item19, item20, item21, new Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31>>(item22, item23, item24, item25, item26, item27, item28, new Tuple<T29, T30, T31>(item29, item30, item31))))));
        }
	    
        /// <summary>
        /// Removes and returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31}"/>.
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
        public void Pop(
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
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31>>>>> tuple = Pop();
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
	    
        /// <summary>
        /// Returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31}"/> without removing it.
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

	#region Stack with 32 items.
    /// <summary>
    /// A stack of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32}"/>.
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
    [UsedImplicitly]
    public class Stack<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32> : Stack<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32>>>>>>
    {
        /// <summary>
        /// Inserts an object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32}" />.
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
        public void Push(
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
            Push(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32>>>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32>>>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32>>>(item15, item16, item17, item18, item19, item20, item21, new Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32>>(item22, item23, item24, item25, item26, item27, item28, new Tuple<T29, T30, T31, T32>(item29, item30, item31, item32))))));
        }
	    
        /// <summary>
        /// Removes and returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32}"/>.
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
        public void Pop(
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
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32>>>>> tuple = Pop();
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
	    
        /// <summary>
        /// Returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32}"/> without removing it.
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

	#region Stack with 33 items.
    /// <summary>
    /// A stack of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33}"/>.
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
    [UsedImplicitly]
    public class Stack<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33> : Stack<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33>>>>>>
    {
        /// <summary>
        /// Inserts an object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33}" />.
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
        public void Push(
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
            Push(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33>>>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33>>>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33>>>(item15, item16, item17, item18, item19, item20, item21, new Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33>>(item22, item23, item24, item25, item26, item27, item28, new Tuple<T29, T30, T31, T32, T33>(item29, item30, item31, item32, item33))))));
        }
	    
        /// <summary>
        /// Removes and returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33}"/>.
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
        public void Pop(
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
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33>>>>> tuple = Pop();
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
	    
        /// <summary>
        /// Returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33}"/> without removing it.
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

	#region Stack with 34 items.
    /// <summary>
    /// A stack of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34}"/>.
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
    [UsedImplicitly]
    public class Stack<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34> : Stack<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34>>>>>>
    {
        /// <summary>
        /// Inserts an object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34}" />.
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
        public void Push(
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
            Push(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34>>>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34>>>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34>>>(item15, item16, item17, item18, item19, item20, item21, new Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34>>(item22, item23, item24, item25, item26, item27, item28, new Tuple<T29, T30, T31, T32, T33, T34>(item29, item30, item31, item32, item33, item34))))));
        }
	    
        /// <summary>
        /// Removes and returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34}"/>.
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
        public void Pop(
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
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34>>>>> tuple = Pop();
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
	    
        /// <summary>
        /// Returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34}"/> without removing it.
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

	#region Stack with 35 items.
    /// <summary>
    /// A stack of <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35}"/>.
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
    [UsedImplicitly]
    public class Stack<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35> : Stack<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34, T35>>>>>>
    {
        /// <summary>
        /// Inserts an object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35}" />.
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
        public void Push(
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
            Push(new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34, T35>>>>>(item1, item2, item3, item4, item5, item6, item7, new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34, T35>>>>(item8, item9, item10, item11, item12, item13, item14, new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34, T35>>>(item15, item16, item17, item18, item19, item20, item21, new Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34, T35>>(item22, item23, item24, item25, item26, item27, item28, new Tuple<T29, T30, T31, T32, T33, T34, T35>(item29, item30, item31, item32, item33, item34, item35))))));
        }
	    
        /// <summary>
        /// Removes and returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35}"/>.
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
        public void Pop(
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
            Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34, T35>>>>> tuple = Pop();
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
	    
        /// <summary>
        /// Returns the object at the top of the <see cref="Stack{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35}"/> without removing it.
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
 
