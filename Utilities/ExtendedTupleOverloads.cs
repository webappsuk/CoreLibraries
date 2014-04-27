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
using System.Linq;
using JetBrains.Annotations;

namespace WebApplications.Utilities
{
    /// <summary>
    /// Extension methods for tuples.
    /// </summary>
    public static partial class ExtendedTuple
    {
        #region 1 items.
        /// <summary>
        /// Used to create a tuple in nested format.
        /// </summary>
        /// <typeparam name="T1">The type of item 1.</typeparam>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <returns>A tuple with 1 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static Tuple<T1> Create<T1>(
            T1 item1
            )
        {
            return new Tuple<T1>(item1);
        }

        /// <summary>
        /// Used to create an enumeration of tuples from an enumeration of objects, by specifying how each item is extracted.
        /// </summary>
        /// <typeparam name="TInput">The input type.</typeparam>
        /// <typeparam name="T1">The type of item 1.</typeparam>
        /// <param name="inputEnumeration">The enumeration of inputs.</param>
        /// <param name="func1">Function that returns item 1 of the tuple.</param>
        /// <returns>A tuple with 1 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static IEnumerable<Tuple<T1>> ToTuple<TInput, T1>(
            [NotNull] this IEnumerable<TInput> inputEnumeration,
            [NotNull] Func<TInput, T1> func1
            )
        {
            return inputEnumeration.Select(input => new Tuple<T1>(func1(input)));
        }
        #endregion

        #region 2 items.
        /// <summary>
        /// Used to create a tuple in nested format.
        /// </summary>
        /// <typeparam name="T1">The type of item 1.</typeparam>
        /// <typeparam name="T2">The type of item 2.</typeparam>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <returns>A tuple with 2 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static Tuple<T1, T2> Create<T1, T2>(
            T1 item1,
            T2 item2
            )
        {
            return new Tuple<T1, T2>(item1, item2);
        }

        /// <summary>
        /// Used to create an enumeration of tuples from an enumeration of objects, by specifying how each item is extracted.
        /// </summary>
        /// <typeparam name="TInput">The input type.</typeparam>
        /// <typeparam name="T1">The type of item 1.</typeparam>
        /// <typeparam name="T2">The type of item 2.</typeparam>
        /// <param name="inputEnumeration">The enumeration of inputs.</param>
        /// <param name="func1">Function that returns item 1 of the tuple.</param>
        /// <param name="func2">Function that returns item 2 of the tuple.</param>
        /// <returns>A tuple with 2 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static IEnumerable<Tuple<T1, T2>> ToTuple<TInput, T1, T2>(
            [NotNull] this IEnumerable<TInput> inputEnumeration,
            [NotNull] Func<TInput, T1> func1,
            [NotNull] Func<TInput, T2> func2
            )
        {
            return inputEnumeration.Select(input => new Tuple<T1, T2>(func1(input), func2(input)));
        }
        #endregion

        #region 3 items.
        /// <summary>
        /// Used to create a tuple in nested format.
        /// </summary>
        /// <typeparam name="T1">The type of item 1.</typeparam>
        /// <typeparam name="T2">The type of item 2.</typeparam>
        /// <typeparam name="T3">The type of item 3.</typeparam>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <returns>A tuple with 3 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static Tuple<T1, T2, T3> Create<T1, T2, T3>(
            T1 item1,
            T2 item2,
            T3 item3
            )
        {
            return new Tuple<T1, T2, T3>(item1, item2, item3);
        }

        /// <summary>
        /// Used to create an enumeration of tuples from an enumeration of objects, by specifying how each item is extracted.
        /// </summary>
        /// <typeparam name="TInput">The input type.</typeparam>
        /// <typeparam name="T1">The type of item 1.</typeparam>
        /// <typeparam name="T2">The type of item 2.</typeparam>
        /// <typeparam name="T3">The type of item 3.</typeparam>
        /// <param name="inputEnumeration">The enumeration of inputs.</param>
        /// <param name="func1">Function that returns item 1 of the tuple.</param>
        /// <param name="func2">Function that returns item 2 of the tuple.</param>
        /// <param name="func3">Function that returns item 3 of the tuple.</param>
        /// <returns>A tuple with 3 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static IEnumerable<Tuple<T1, T2, T3>> ToTuple<TInput, T1, T2, T3>(
            [NotNull] this IEnumerable<TInput> inputEnumeration,
            [NotNull] Func<TInput, T1> func1,
            [NotNull] Func<TInput, T2> func2,
            [NotNull] Func<TInput, T3> func3
            )
        {
            return inputEnumeration.Select(input => new Tuple<T1, T2, T3>(func1(input), func2(input), func3(input)));
        }
        #endregion

        #region 4 items.
        /// <summary>
        /// Used to create a tuple in nested format.
        /// </summary>
        /// <typeparam name="T1">The type of item 1.</typeparam>
        /// <typeparam name="T2">The type of item 2.</typeparam>
        /// <typeparam name="T3">The type of item 3.</typeparam>
        /// <typeparam name="T4">The type of item 4.</typeparam>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <returns>A tuple with 4 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static Tuple<T1, T2, T3, T4> Create<T1, T2, T3, T4>(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4
            )
        {
            return new Tuple<T1, T2, T3, T4>(item1, item2, item3, item4);
        }

        /// <summary>
        /// Used to create an enumeration of tuples from an enumeration of objects, by specifying how each item is extracted.
        /// </summary>
        /// <typeparam name="TInput">The input type.</typeparam>
        /// <typeparam name="T1">The type of item 1.</typeparam>
        /// <typeparam name="T2">The type of item 2.</typeparam>
        /// <typeparam name="T3">The type of item 3.</typeparam>
        /// <typeparam name="T4">The type of item 4.</typeparam>
        /// <param name="inputEnumeration">The enumeration of inputs.</param>
        /// <param name="func1">Function that returns item 1 of the tuple.</param>
        /// <param name="func2">Function that returns item 2 of the tuple.</param>
        /// <param name="func3">Function that returns item 3 of the tuple.</param>
        /// <param name="func4">Function that returns item 4 of the tuple.</param>
        /// <returns>A tuple with 4 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static IEnumerable<Tuple<T1, T2, T3, T4>> ToTuple<TInput, T1, T2, T3, T4>(
            [NotNull] this IEnumerable<TInput> inputEnumeration,
            [NotNull] Func<TInput, T1> func1,
            [NotNull] Func<TInput, T2> func2,
            [NotNull] Func<TInput, T3> func3,
            [NotNull] Func<TInput, T4> func4
            )
        {
            return
                inputEnumeration.Select(
                    input => new Tuple<T1, T2, T3, T4>(func1(input), func2(input), func3(input), func4(input)));
        }
        #endregion

        #region 5 items.
        /// <summary>
        /// Used to create a tuple in nested format.
        /// </summary>
        /// <typeparam name="T1">The type of item 1.</typeparam>
        /// <typeparam name="T2">The type of item 2.</typeparam>
        /// <typeparam name="T3">The type of item 3.</typeparam>
        /// <typeparam name="T4">The type of item 4.</typeparam>
        /// <typeparam name="T5">The type of item 5.</typeparam>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <returns>A tuple with 5 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static Tuple<T1, T2, T3, T4, T5> Create<T1, T2, T3, T4, T5>(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4,
            T5 item5
            )
        {
            return new Tuple<T1, T2, T3, T4, T5>(item1, item2, item3, item4, item5);
        }

        /// <summary>
        /// Used to create an enumeration of tuples from an enumeration of objects, by specifying how each item is extracted.
        /// </summary>
        /// <typeparam name="TInput">The input type.</typeparam>
        /// <typeparam name="T1">The type of item 1.</typeparam>
        /// <typeparam name="T2">The type of item 2.</typeparam>
        /// <typeparam name="T3">The type of item 3.</typeparam>
        /// <typeparam name="T4">The type of item 4.</typeparam>
        /// <typeparam name="T5">The type of item 5.</typeparam>
        /// <param name="inputEnumeration">The enumeration of inputs.</param>
        /// <param name="func1">Function that returns item 1 of the tuple.</param>
        /// <param name="func2">Function that returns item 2 of the tuple.</param>
        /// <param name="func3">Function that returns item 3 of the tuple.</param>
        /// <param name="func4">Function that returns item 4 of the tuple.</param>
        /// <param name="func5">Function that returns item 5 of the tuple.</param>
        /// <returns>A tuple with 5 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static IEnumerable<Tuple<T1, T2, T3, T4, T5>> ToTuple<TInput, T1, T2, T3, T4, T5>(
            [NotNull] this IEnumerable<TInput> inputEnumeration,
            [NotNull] Func<TInput, T1> func1,
            [NotNull] Func<TInput, T2> func2,
            [NotNull] Func<TInput, T3> func3,
            [NotNull] Func<TInput, T4> func4,
            [NotNull] Func<TInput, T5> func5
            )
        {
            return
                inputEnumeration.Select(
                    input =>
                        new Tuple<T1, T2, T3, T4, T5>(
                        func1(input),
                        func2(input),
                        func3(input),
                        func4(input),
                        func5(input)));
        }
        #endregion

        #region 6 items.
        /// <summary>
        /// Used to create a tuple in nested format.
        /// </summary>
        /// <typeparam name="T1">The type of item 1.</typeparam>
        /// <typeparam name="T2">The type of item 2.</typeparam>
        /// <typeparam name="T3">The type of item 3.</typeparam>
        /// <typeparam name="T4">The type of item 4.</typeparam>
        /// <typeparam name="T5">The type of item 5.</typeparam>
        /// <typeparam name="T6">The type of item 6.</typeparam>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <returns>A tuple with 6 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static Tuple<T1, T2, T3, T4, T5, T6> Create<T1, T2, T3, T4, T5, T6>(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4,
            T5 item5,
            T6 item6
            )
        {
            return new Tuple<T1, T2, T3, T4, T5, T6>(item1, item2, item3, item4, item5, item6);
        }

        /// <summary>
        /// Used to create an enumeration of tuples from an enumeration of objects, by specifying how each item is extracted.
        /// </summary>
        /// <typeparam name="TInput">The input type.</typeparam>
        /// <typeparam name="T1">The type of item 1.</typeparam>
        /// <typeparam name="T2">The type of item 2.</typeparam>
        /// <typeparam name="T3">The type of item 3.</typeparam>
        /// <typeparam name="T4">The type of item 4.</typeparam>
        /// <typeparam name="T5">The type of item 5.</typeparam>
        /// <typeparam name="T6">The type of item 6.</typeparam>
        /// <param name="inputEnumeration">The enumeration of inputs.</param>
        /// <param name="func1">Function that returns item 1 of the tuple.</param>
        /// <param name="func2">Function that returns item 2 of the tuple.</param>
        /// <param name="func3">Function that returns item 3 of the tuple.</param>
        /// <param name="func4">Function that returns item 4 of the tuple.</param>
        /// <param name="func5">Function that returns item 5 of the tuple.</param>
        /// <param name="func6">Function that returns item 6 of the tuple.</param>
        /// <returns>A tuple with 6 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static IEnumerable<Tuple<T1, T2, T3, T4, T5, T6>> ToTuple<TInput, T1, T2, T3, T4, T5, T6>(
            [NotNull] this IEnumerable<TInput> inputEnumeration,
            [NotNull] Func<TInput, T1> func1,
            [NotNull] Func<TInput, T2> func2,
            [NotNull] Func<TInput, T3> func3,
            [NotNull] Func<TInput, T4> func4,
            [NotNull] Func<TInput, T5> func5,
            [NotNull] Func<TInput, T6> func6
            )
        {
            return
                inputEnumeration.Select(
                    input =>
                        new Tuple<T1, T2, T3, T4, T5, T6>(
                        func1(input),
                        func2(input),
                        func3(input),
                        func4(input),
                        func5(input),
                        func6(input)));
        }
        #endregion

        #region 7 items.
        /// <summary>
        /// Used to create a tuple in nested format.
        /// </summary>
        /// <typeparam name="T1">The type of item 1.</typeparam>
        /// <typeparam name="T2">The type of item 2.</typeparam>
        /// <typeparam name="T3">The type of item 3.</typeparam>
        /// <typeparam name="T4">The type of item 4.</typeparam>
        /// <typeparam name="T5">The type of item 5.</typeparam>
        /// <typeparam name="T6">The type of item 6.</typeparam>
        /// <typeparam name="T7">The type of item 7.</typeparam>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <returns>A tuple with 7 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static Tuple<T1, T2, T3, T4, T5, T6, T7> Create<T1, T2, T3, T4, T5, T6, T7>(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4,
            T5 item5,
            T6 item6,
            T7 item7
            )
        {
            return new Tuple<T1, T2, T3, T4, T5, T6, T7>(item1, item2, item3, item4, item5, item6, item7);
        }

        /// <summary>
        /// Used to create an enumeration of tuples from an enumeration of objects, by specifying how each item is extracted.
        /// </summary>
        /// <typeparam name="TInput">The input type.</typeparam>
        /// <typeparam name="T1">The type of item 1.</typeparam>
        /// <typeparam name="T2">The type of item 2.</typeparam>
        /// <typeparam name="T3">The type of item 3.</typeparam>
        /// <typeparam name="T4">The type of item 4.</typeparam>
        /// <typeparam name="T5">The type of item 5.</typeparam>
        /// <typeparam name="T6">The type of item 6.</typeparam>
        /// <typeparam name="T7">The type of item 7.</typeparam>
        /// <param name="inputEnumeration">The enumeration of inputs.</param>
        /// <param name="func1">Function that returns item 1 of the tuple.</param>
        /// <param name="func2">Function that returns item 2 of the tuple.</param>
        /// <param name="func3">Function that returns item 3 of the tuple.</param>
        /// <param name="func4">Function that returns item 4 of the tuple.</param>
        /// <param name="func5">Function that returns item 5 of the tuple.</param>
        /// <param name="func6">Function that returns item 6 of the tuple.</param>
        /// <param name="func7">Function that returns item 7 of the tuple.</param>
        /// <returns>A tuple with 7 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7>> ToTuple<TInput, T1, T2, T3, T4, T5, T6, T7>(
            [NotNull] this IEnumerable<TInput> inputEnumeration,
            [NotNull] Func<TInput, T1> func1,
            [NotNull] Func<TInput, T2> func2,
            [NotNull] Func<TInput, T3> func3,
            [NotNull] Func<TInput, T4> func4,
            [NotNull] Func<TInput, T5> func5,
            [NotNull] Func<TInput, T6> func6,
            [NotNull] Func<TInput, T7> func7
            )
        {
            return
                inputEnumeration.Select(
                    input =>
                        new Tuple<T1, T2, T3, T4, T5, T6, T7>(
                        func1(input),
                        func2(input),
                        func3(input),
                        func4(input),
                        func5(input),
                        func6(input),
                        func7(input)));
        }
        #endregion

        #region 8 items.
        /// <summary>
        /// Used to create a tuple in nested format.
        /// </summary>
        /// <typeparam name="T1">The type of item 1.</typeparam>
        /// <typeparam name="T2">The type of item 2.</typeparam>
        /// <typeparam name="T3">The type of item 3.</typeparam>
        /// <typeparam name="T4">The type of item 4.</typeparam>
        /// <typeparam name="T5">The type of item 5.</typeparam>
        /// <typeparam name="T6">The type of item 6.</typeparam>
        /// <typeparam name="T7">The type of item 7.</typeparam>
        /// <typeparam name="T8">The type of item 8.</typeparam>
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <returns>A tuple with 8 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>> Create<T1, T2, T3, T4, T5, T6, T7, T8>(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4,
            T5 item5,
            T6 item6,
            T7 item7,
            T8 item8
            )
        {
            return new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>>(
                item1,
                item2,
                item3,
                item4,
                item5,
                item6,
                item7,
                new Tuple<T8>(item8));
        }

        /// <summary>
        /// Used to create an enumeration of tuples from an enumeration of objects, by specifying how each item is extracted.
        /// </summary>
        /// <typeparam name="TInput">The input type.</typeparam>
        /// <typeparam name="T1">The type of item 1.</typeparam>
        /// <typeparam name="T2">The type of item 2.</typeparam>
        /// <typeparam name="T3">The type of item 3.</typeparam>
        /// <typeparam name="T4">The type of item 4.</typeparam>
        /// <typeparam name="T5">The type of item 5.</typeparam>
        /// <typeparam name="T6">The type of item 6.</typeparam>
        /// <typeparam name="T7">The type of item 7.</typeparam>
        /// <typeparam name="T8">The type of item 8.</typeparam>
        /// <param name="inputEnumeration">The enumeration of inputs.</param>
        /// <param name="func1">Function that returns item 1 of the tuple.</param>
        /// <param name="func2">Function that returns item 2 of the tuple.</param>
        /// <param name="func3">Function that returns item 3 of the tuple.</param>
        /// <param name="func4">Function that returns item 4 of the tuple.</param>
        /// <param name="func5">Function that returns item 5 of the tuple.</param>
        /// <param name="func6">Function that returns item 6 of the tuple.</param>
        /// <param name="func7">Function that returns item 7 of the tuple.</param>
        /// <param name="func8">Function that returns item 8 of the tuple.</param>
        /// <returns>A tuple with 8 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>>> ToTuple
            <TInput, T1, T2, T3, T4, T5, T6, T7, T8>(
            [NotNull] this IEnumerable<TInput> inputEnumeration,
            [NotNull] Func<TInput, T1> func1,
            [NotNull] Func<TInput, T2> func2,
            [NotNull] Func<TInput, T3> func3,
            [NotNull] Func<TInput, T4> func4,
            [NotNull] Func<TInput, T5> func5,
            [NotNull] Func<TInput, T6> func6,
            [NotNull] Func<TInput, T7> func7,
            [NotNull] Func<TInput, T8> func8
            )
        {
            return
                inputEnumeration.Select(
                    input =>
                        new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>>(
                        func1(input),
                        func2(input),
                        func3(input),
                        func4(input),
                        func5(input),
                        func6(input),
                        func7(input),
                        new Tuple<T8>(func8(input))));
        }
        #endregion

        #region 9 items.
        /// <summary>
        /// Used to create a tuple in nested format.
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
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <returns>A tuple with 9 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9>> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4,
            T5 item5,
            T6 item6,
            T7 item7,
            T8 item8,
            T9 item9
            )
        {
            return new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9>>(
                item1,
                item2,
                item3,
                item4,
                item5,
                item6,
                item7,
                new Tuple<T8, T9>(item8, item9));
        }

        /// <summary>
        /// Used to create an enumeration of tuples from an enumeration of objects, by specifying how each item is extracted.
        /// </summary>
        /// <typeparam name="TInput">The input type.</typeparam>
        /// <typeparam name="T1">The type of item 1.</typeparam>
        /// <typeparam name="T2">The type of item 2.</typeparam>
        /// <typeparam name="T3">The type of item 3.</typeparam>
        /// <typeparam name="T4">The type of item 4.</typeparam>
        /// <typeparam name="T5">The type of item 5.</typeparam>
        /// <typeparam name="T6">The type of item 6.</typeparam>
        /// <typeparam name="T7">The type of item 7.</typeparam>
        /// <typeparam name="T8">The type of item 8.</typeparam>
        /// <typeparam name="T9">The type of item 9.</typeparam>
        /// <param name="inputEnumeration">The enumeration of inputs.</param>
        /// <param name="func1">Function that returns item 1 of the tuple.</param>
        /// <param name="func2">Function that returns item 2 of the tuple.</param>
        /// <param name="func3">Function that returns item 3 of the tuple.</param>
        /// <param name="func4">Function that returns item 4 of the tuple.</param>
        /// <param name="func5">Function that returns item 5 of the tuple.</param>
        /// <param name="func6">Function that returns item 6 of the tuple.</param>
        /// <param name="func7">Function that returns item 7 of the tuple.</param>
        /// <param name="func8">Function that returns item 8 of the tuple.</param>
        /// <param name="func9">Function that returns item 9 of the tuple.</param>
        /// <returns>A tuple with 9 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9>>> ToTuple
            <TInput, T1, T2, T3, T4, T5, T6, T7, T8, T9>(
            [NotNull] this IEnumerable<TInput> inputEnumeration,
            [NotNull] Func<TInput, T1> func1,
            [NotNull] Func<TInput, T2> func2,
            [NotNull] Func<TInput, T3> func3,
            [NotNull] Func<TInput, T4> func4,
            [NotNull] Func<TInput, T5> func5,
            [NotNull] Func<TInput, T6> func6,
            [NotNull] Func<TInput, T7> func7,
            [NotNull] Func<TInput, T8> func8,
            [NotNull] Func<TInput, T9> func9
            )
        {
            return
                inputEnumeration.Select(
                    input =>
                        new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9>>(
                        func1(input),
                        func2(input),
                        func3(input),
                        func4(input),
                        func5(input),
                        func6(input),
                        func7(input),
                        new Tuple<T8, T9>(func8(input), func9(input))));
        }
        #endregion

        #region 10 items.
        /// <summary>
        /// Used to create a tuple in nested format.
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
        /// <param name="item1">Item 1 of the tuple.</param>
        /// <param name="item2">Item 2 of the tuple.</param>
        /// <param name="item3">Item 3 of the tuple.</param>
        /// <param name="item4">Item 4 of the tuple.</param>
        /// <param name="item5">Item 5 of the tuple.</param>
        /// <param name="item6">Item 6 of the tuple.</param>
        /// <param name="item7">Item 7 of the tuple.</param>
        /// <param name="item8">Item 8 of the tuple.</param>
        /// <param name="item9">Item 9 of the tuple.</param>
        /// <param name="item10">Item 10 of the tuple.</param>
        /// <returns>A tuple with 10 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10>> Create
            <T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
            T1 item1,
            T2 item2,
            T3 item3,
            T4 item4,
            T5 item5,
            T6 item6,
            T7 item7,
            T8 item8,
            T9 item9,
            T10 item10
            )
        {
            return new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10>>(
                item1,
                item2,
                item3,
                item4,
                item5,
                item6,
                item7,
                new Tuple<T8, T9, T10>(item8, item9, item10));
        }

        /// <summary>
        /// Used to create an enumeration of tuples from an enumeration of objects, by specifying how each item is extracted.
        /// </summary>
        /// <typeparam name="TInput">The input type.</typeparam>
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
        /// <param name="inputEnumeration">The enumeration of inputs.</param>
        /// <param name="func1">Function that returns item 1 of the tuple.</param>
        /// <param name="func2">Function that returns item 2 of the tuple.</param>
        /// <param name="func3">Function that returns item 3 of the tuple.</param>
        /// <param name="func4">Function that returns item 4 of the tuple.</param>
        /// <param name="func5">Function that returns item 5 of the tuple.</param>
        /// <param name="func6">Function that returns item 6 of the tuple.</param>
        /// <param name="func7">Function that returns item 7 of the tuple.</param>
        /// <param name="func8">Function that returns item 8 of the tuple.</param>
        /// <param name="func9">Function that returns item 9 of the tuple.</param>
        /// <param name="func10">Function that returns item 10 of the tuple.</param>
        /// <returns>A tuple with 10 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10>>> ToTuple
            <TInput, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
            [NotNull] this IEnumerable<TInput> inputEnumeration,
            [NotNull] Func<TInput, T1> func1,
            [NotNull] Func<TInput, T2> func2,
            [NotNull] Func<TInput, T3> func3,
            [NotNull] Func<TInput, T4> func4,
            [NotNull] Func<TInput, T5> func5,
            [NotNull] Func<TInput, T6> func6,
            [NotNull] Func<TInput, T7> func7,
            [NotNull] Func<TInput, T8> func8,
            [NotNull] Func<TInput, T9> func9,
            [NotNull] Func<TInput, T10> func10
            )
        {
            return
                inputEnumeration.Select(
                    input =>
                        new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10>>(
                        func1(input),
                        func2(input),
                        func3(input),
                        func4(input),
                        func5(input),
                        func6(input),
                        func7(input),
                        new Tuple<T8, T9, T10>(func8(input), func9(input), func10(input))));
        }
        #endregion

        #region 11 items.
        /// <summary>
        /// Used to create a tuple in nested format.
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
        /// <param name="item1">Item 1 of the tuple.</param>
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
        /// <returns>A tuple with 11 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11>> Create
            <T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
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
            T11 item11
            )
        {
            return new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11>>(
                item1,
                item2,
                item3,
                item4,
                item5,
                item6,
                item7,
                new Tuple<T8, T9, T10, T11>(item8, item9, item10, item11));
        }

        /// <summary>
        /// Used to create an enumeration of tuples from an enumeration of objects, by specifying how each item is extracted.
        /// </summary>
        /// <typeparam name="TInput">The input type.</typeparam>
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
        /// <param name="inputEnumeration">The enumeration of inputs.</param>
        /// <param name="func1">Function that returns item 1 of the tuple.</param>
        /// <param name="func2">Function that returns item 2 of the tuple.</param>
        /// <param name="func3">Function that returns item 3 of the tuple.</param>
        /// <param name="func4">Function that returns item 4 of the tuple.</param>
        /// <param name="func5">Function that returns item 5 of the tuple.</param>
        /// <param name="func6">Function that returns item 6 of the tuple.</param>
        /// <param name="func7">Function that returns item 7 of the tuple.</param>
        /// <param name="func8">Function that returns item 8 of the tuple.</param>
        /// <param name="func9">Function that returns item 9 of the tuple.</param>
        /// <param name="func10">Function that returns item 10 of the tuple.</param>
        /// <param name="func11">Function that returns item 11 of the tuple.</param>
        /// <returns>A tuple with 11 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11>>> ToTuple
            <TInput, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
            [NotNull] this IEnumerable<TInput> inputEnumeration,
            [NotNull] Func<TInput, T1> func1,
            [NotNull] Func<TInput, T2> func2,
            [NotNull] Func<TInput, T3> func3,
            [NotNull] Func<TInput, T4> func4,
            [NotNull] Func<TInput, T5> func5,
            [NotNull] Func<TInput, T6> func6,
            [NotNull] Func<TInput, T7> func7,
            [NotNull] Func<TInput, T8> func8,
            [NotNull] Func<TInput, T9> func9,
            [NotNull] Func<TInput, T10> func10,
            [NotNull] Func<TInput, T11> func11
            )
        {
            return
                inputEnumeration.Select(
                    input =>
                        new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11>>(
                        func1(input),
                        func2(input),
                        func3(input),
                        func4(input),
                        func5(input),
                        func6(input),
                        func7(input),
                        new Tuple<T8, T9, T10, T11>(func8(input), func9(input), func10(input), func11(input))));
        }
        #endregion

        #region 12 items.
        /// <summary>
        /// Used to create a tuple in nested format.
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
        /// <param name="item1">Item 1 of the tuple.</param>
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
        /// <returns>A tuple with 12 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12>> Create
            <T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
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
            T12 item12
            )
        {
            return new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12>>(
                item1,
                item2,
                item3,
                item4,
                item5,
                item6,
                item7,
                new Tuple<T8, T9, T10, T11, T12>(item8, item9, item10, item11, item12));
        }

        /// <summary>
        /// Used to create an enumeration of tuples from an enumeration of objects, by specifying how each item is extracted.
        /// </summary>
        /// <typeparam name="TInput">The input type.</typeparam>
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
        /// <param name="inputEnumeration">The enumeration of inputs.</param>
        /// <param name="func1">Function that returns item 1 of the tuple.</param>
        /// <param name="func2">Function that returns item 2 of the tuple.</param>
        /// <param name="func3">Function that returns item 3 of the tuple.</param>
        /// <param name="func4">Function that returns item 4 of the tuple.</param>
        /// <param name="func5">Function that returns item 5 of the tuple.</param>
        /// <param name="func6">Function that returns item 6 of the tuple.</param>
        /// <param name="func7">Function that returns item 7 of the tuple.</param>
        /// <param name="func8">Function that returns item 8 of the tuple.</param>
        /// <param name="func9">Function that returns item 9 of the tuple.</param>
        /// <param name="func10">Function that returns item 10 of the tuple.</param>
        /// <param name="func11">Function that returns item 11 of the tuple.</param>
        /// <param name="func12">Function that returns item 12 of the tuple.</param>
        /// <returns>A tuple with 12 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12>>> ToTuple
            <TInput, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
            [NotNull] this IEnumerable<TInput> inputEnumeration,
            [NotNull] Func<TInput, T1> func1,
            [NotNull] Func<TInput, T2> func2,
            [NotNull] Func<TInput, T3> func3,
            [NotNull] Func<TInput, T4> func4,
            [NotNull] Func<TInput, T5> func5,
            [NotNull] Func<TInput, T6> func6,
            [NotNull] Func<TInput, T7> func7,
            [NotNull] Func<TInput, T8> func8,
            [NotNull] Func<TInput, T9> func9,
            [NotNull] Func<TInput, T10> func10,
            [NotNull] Func<TInput, T11> func11,
            [NotNull] Func<TInput, T12> func12
            )
        {
            return
                inputEnumeration.Select(
                    input =>
                        new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12>>(
                        func1(input),
                        func2(input),
                        func3(input),
                        func4(input),
                        func5(input),
                        func6(input),
                        func7(input),
                        new Tuple<T8, T9, T10, T11, T12>(
                            func8(input),
                            func9(input),
                            func10(input),
                            func11(input),
                            func12(input))));
        }
        #endregion

        #region 13 items.
        /// <summary>
        /// Used to create a tuple in nested format.
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
        /// <param name="item1">Item 1 of the tuple.</param>
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
        /// <returns>A tuple with 13 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13>> Create
            <T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
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
            T13 item13
            )
        {
            return new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13>>(
                item1,
                item2,
                item3,
                item4,
                item5,
                item6,
                item7,
                new Tuple<T8, T9, T10, T11, T12, T13>(item8, item9, item10, item11, item12, item13));
        }

        /// <summary>
        /// Used to create an enumeration of tuples from an enumeration of objects, by specifying how each item is extracted.
        /// </summary>
        /// <typeparam name="TInput">The input type.</typeparam>
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
        /// <param name="inputEnumeration">The enumeration of inputs.</param>
        /// <param name="func1">Function that returns item 1 of the tuple.</param>
        /// <param name="func2">Function that returns item 2 of the tuple.</param>
        /// <param name="func3">Function that returns item 3 of the tuple.</param>
        /// <param name="func4">Function that returns item 4 of the tuple.</param>
        /// <param name="func5">Function that returns item 5 of the tuple.</param>
        /// <param name="func6">Function that returns item 6 of the tuple.</param>
        /// <param name="func7">Function that returns item 7 of the tuple.</param>
        /// <param name="func8">Function that returns item 8 of the tuple.</param>
        /// <param name="func9">Function that returns item 9 of the tuple.</param>
        /// <param name="func10">Function that returns item 10 of the tuple.</param>
        /// <param name="func11">Function that returns item 11 of the tuple.</param>
        /// <param name="func12">Function that returns item 12 of the tuple.</param>
        /// <param name="func13">Function that returns item 13 of the tuple.</param>
        /// <returns>A tuple with 13 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13>>> ToTuple
            <TInput, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
            [NotNull] this IEnumerable<TInput> inputEnumeration,
            [NotNull] Func<TInput, T1> func1,
            [NotNull] Func<TInput, T2> func2,
            [NotNull] Func<TInput, T3> func3,
            [NotNull] Func<TInput, T4> func4,
            [NotNull] Func<TInput, T5> func5,
            [NotNull] Func<TInput, T6> func6,
            [NotNull] Func<TInput, T7> func7,
            [NotNull] Func<TInput, T8> func8,
            [NotNull] Func<TInput, T9> func9,
            [NotNull] Func<TInput, T10> func10,
            [NotNull] Func<TInput, T11> func11,
            [NotNull] Func<TInput, T12> func12,
            [NotNull] Func<TInput, T13> func13
            )
        {
            return
                inputEnumeration.Select(
                    input =>
                        new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13>>(
                        func1(input),
                        func2(input),
                        func3(input),
                        func4(input),
                        func5(input),
                        func6(input),
                        func7(input),
                        new Tuple<T8, T9, T10, T11, T12, T13>(
                            func8(input),
                            func9(input),
                            func10(input),
                            func11(input),
                            func12(input),
                            func13(input))));
        }
        #endregion

        #region 14 items.
        /// <summary>
        /// Used to create a tuple in nested format.
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
        /// <param name="item1">Item 1 of the tuple.</param>
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
        /// <returns>A tuple with 14 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14>> Create
            <T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
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
            T14 item14
            )
        {
            return new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14>>(
                item1,
                item2,
                item3,
                item4,
                item5,
                item6,
                item7,
                new Tuple<T8, T9, T10, T11, T12, T13, T14>(item8, item9, item10, item11, item12, item13, item14));
        }

        /// <summary>
        /// Used to create an enumeration of tuples from an enumeration of objects, by specifying how each item is extracted.
        /// </summary>
        /// <typeparam name="TInput">The input type.</typeparam>
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
        /// <param name="inputEnumeration">The enumeration of inputs.</param>
        /// <param name="func1">Function that returns item 1 of the tuple.</param>
        /// <param name="func2">Function that returns item 2 of the tuple.</param>
        /// <param name="func3">Function that returns item 3 of the tuple.</param>
        /// <param name="func4">Function that returns item 4 of the tuple.</param>
        /// <param name="func5">Function that returns item 5 of the tuple.</param>
        /// <param name="func6">Function that returns item 6 of the tuple.</param>
        /// <param name="func7">Function that returns item 7 of the tuple.</param>
        /// <param name="func8">Function that returns item 8 of the tuple.</param>
        /// <param name="func9">Function that returns item 9 of the tuple.</param>
        /// <param name="func10">Function that returns item 10 of the tuple.</param>
        /// <param name="func11">Function that returns item 11 of the tuple.</param>
        /// <param name="func12">Function that returns item 12 of the tuple.</param>
        /// <param name="func13">Function that returns item 13 of the tuple.</param>
        /// <param name="func14">Function that returns item 14 of the tuple.</param>
        /// <returns>A tuple with 14 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14>>> ToTuple
            <TInput, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
            [NotNull] this IEnumerable<TInput> inputEnumeration,
            [NotNull] Func<TInput, T1> func1,
            [NotNull] Func<TInput, T2> func2,
            [NotNull] Func<TInput, T3> func3,
            [NotNull] Func<TInput, T4> func4,
            [NotNull] Func<TInput, T5> func5,
            [NotNull] Func<TInput, T6> func6,
            [NotNull] Func<TInput, T7> func7,
            [NotNull] Func<TInput, T8> func8,
            [NotNull] Func<TInput, T9> func9,
            [NotNull] Func<TInput, T10> func10,
            [NotNull] Func<TInput, T11> func11,
            [NotNull] Func<TInput, T12> func12,
            [NotNull] Func<TInput, T13> func13,
            [NotNull] Func<TInput, T14> func14
            )
        {
            return
                inputEnumeration.Select(
                    input =>
                        new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14>>(
                        func1(input),
                        func2(input),
                        func3(input),
                        func4(input),
                        func5(input),
                        func6(input),
                        func7(input),
                        new Tuple<T8, T9, T10, T11, T12, T13, T14>(
                            func8(input),
                            func9(input),
                            func10(input),
                            func11(input),
                            func12(input),
                            func13(input),
                            func14(input))));
        }
        #endregion

        #region 15 items.
        /// <summary>
        /// Used to create a tuple in nested format.
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
        /// <param name="item1">Item 1 of the tuple.</param>
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
        /// <returns>A tuple with 15 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15>>> Create
            <T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
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
            T15 item15
            )
        {
            return new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15>>>(
                item1,
                item2,
                item3,
                item4,
                item5,
                item6,
                item7,
                new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15>>(
                    item8,
                    item9,
                    item10,
                    item11,
                    item12,
                    item13,
                    item14,
                    new Tuple<T15>(item15)));
        }

        /// <summary>
        /// Used to create an enumeration of tuples from an enumeration of objects, by specifying how each item is extracted.
        /// </summary>
        /// <typeparam name="TInput">The input type.</typeparam>
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
        /// <param name="inputEnumeration">The enumeration of inputs.</param>
        /// <param name="func1">Function that returns item 1 of the tuple.</param>
        /// <param name="func2">Function that returns item 2 of the tuple.</param>
        /// <param name="func3">Function that returns item 3 of the tuple.</param>
        /// <param name="func4">Function that returns item 4 of the tuple.</param>
        /// <param name="func5">Function that returns item 5 of the tuple.</param>
        /// <param name="func6">Function that returns item 6 of the tuple.</param>
        /// <param name="func7">Function that returns item 7 of the tuple.</param>
        /// <param name="func8">Function that returns item 8 of the tuple.</param>
        /// <param name="func9">Function that returns item 9 of the tuple.</param>
        /// <param name="func10">Function that returns item 10 of the tuple.</param>
        /// <param name="func11">Function that returns item 11 of the tuple.</param>
        /// <param name="func12">Function that returns item 12 of the tuple.</param>
        /// <param name="func13">Function that returns item 13 of the tuple.</param>
        /// <param name="func14">Function that returns item 14 of the tuple.</param>
        /// <param name="func15">Function that returns item 15 of the tuple.</param>
        /// <returns>A tuple with 15 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15>>>>
            ToTuple<TInput, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
            [NotNull] this IEnumerable<TInput> inputEnumeration,
            [NotNull] Func<TInput, T1> func1,
            [NotNull] Func<TInput, T2> func2,
            [NotNull] Func<TInput, T3> func3,
            [NotNull] Func<TInput, T4> func4,
            [NotNull] Func<TInput, T5> func5,
            [NotNull] Func<TInput, T6> func6,
            [NotNull] Func<TInput, T7> func7,
            [NotNull] Func<TInput, T8> func8,
            [NotNull] Func<TInput, T9> func9,
            [NotNull] Func<TInput, T10> func10,
            [NotNull] Func<TInput, T11> func11,
            [NotNull] Func<TInput, T12> func12,
            [NotNull] Func<TInput, T13> func13,
            [NotNull] Func<TInput, T14> func14,
            [NotNull] Func<TInput, T15> func15
            )
        {
            return
                inputEnumeration.Select(
                    input =>
                        new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15>>>(
                        func1(input),
                        func2(input),
                        func3(input),
                        func4(input),
                        func5(input),
                        func6(input),
                        func7(input),
                        new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15>>(
                            func8(input),
                            func9(input),
                            func10(input),
                            func11(input),
                            func12(input),
                            func13(input),
                            func14(input),
                            new Tuple<T15>(func15(input)))));
        }
        #endregion

        #region 16 items.
        /// <summary>
        /// Used to create a tuple in nested format.
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
        /// <param name="item1">Item 1 of the tuple.</param>
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
        /// <returns>A tuple with 16 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16>>> Create
            <T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(
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
            T16 item16
            )
        {
            return new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16>>>(
                item1,
                item2,
                item3,
                item4,
                item5,
                item6,
                item7,
                new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16>>(
                    item8,
                    item9,
                    item10,
                    item11,
                    item12,
                    item13,
                    item14,
                    new Tuple<T15, T16>(item15, item16)));
        }

        /// <summary>
        /// Used to create an enumeration of tuples from an enumeration of objects, by specifying how each item is extracted.
        /// </summary>
        /// <typeparam name="TInput">The input type.</typeparam>
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
        /// <param name="inputEnumeration">The enumeration of inputs.</param>
        /// <param name="func1">Function that returns item 1 of the tuple.</param>
        /// <param name="func2">Function that returns item 2 of the tuple.</param>
        /// <param name="func3">Function that returns item 3 of the tuple.</param>
        /// <param name="func4">Function that returns item 4 of the tuple.</param>
        /// <param name="func5">Function that returns item 5 of the tuple.</param>
        /// <param name="func6">Function that returns item 6 of the tuple.</param>
        /// <param name="func7">Function that returns item 7 of the tuple.</param>
        /// <param name="func8">Function that returns item 8 of the tuple.</param>
        /// <param name="func9">Function that returns item 9 of the tuple.</param>
        /// <param name="func10">Function that returns item 10 of the tuple.</param>
        /// <param name="func11">Function that returns item 11 of the tuple.</param>
        /// <param name="func12">Function that returns item 12 of the tuple.</param>
        /// <param name="func13">Function that returns item 13 of the tuple.</param>
        /// <param name="func14">Function that returns item 14 of the tuple.</param>
        /// <param name="func15">Function that returns item 15 of the tuple.</param>
        /// <param name="func16">Function that returns item 16 of the tuple.</param>
        /// <returns>A tuple with 16 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static
        IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16>>>> ToTuple
            <TInput, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(
            [NotNull] this IEnumerable<TInput> inputEnumeration,
            [NotNull] Func<TInput, T1> func1,
            [NotNull] Func<TInput, T2> func2,
            [NotNull] Func<TInput, T3> func3,
            [NotNull] Func<TInput, T4> func4,
            [NotNull] Func<TInput, T5> func5,
            [NotNull] Func<TInput, T6> func6,
            [NotNull] Func<TInput, T7> func7,
            [NotNull] Func<TInput, T8> func8,
            [NotNull] Func<TInput, T9> func9,
            [NotNull] Func<TInput, T10> func10,
            [NotNull] Func<TInput, T11> func11,
            [NotNull] Func<TInput, T12> func12,
            [NotNull] Func<TInput, T13> func13,
            [NotNull] Func<TInput, T14> func14,
            [NotNull] Func<TInput, T15> func15,
            [NotNull] Func<TInput, T16> func16
            )
        {
            return
                inputEnumeration.Select(
                    input =>
                        new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16>>>(
                        func1(input),
                        func2(input),
                        func3(input),
                        func4(input),
                        func5(input),
                        func6(input),
                        func7(input),
                        new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16>>(
                            func8(input),
                            func9(input),
                            func10(input),
                            func11(input),
                            func12(input),
                            func13(input),
                            func14(input),
                            new Tuple<T15, T16>(func15(input), func16(input)))));
        }
        #endregion

        #region 17 items.
        /// <summary>
        /// Used to create a tuple in nested format.
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
        /// <param name="item1">Item 1 of the tuple.</param>
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
        /// <returns>A tuple with 17 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17>>>
            Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>(
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
            T17 item17
            )
        {
            return
                new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17>>>(
                    item1,
                    item2,
                    item3,
                    item4,
                    item5,
                    item6,
                    item7,
                    new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17>>(
                        item8,
                        item9,
                        item10,
                        item11,
                        item12,
                        item13,
                        item14,
                        new Tuple<T15, T16, T17>(item15, item16, item17)));
        }

        /// <summary>
        /// Used to create an enumeration of tuples from an enumeration of objects, by specifying how each item is extracted.
        /// </summary>
        /// <typeparam name="TInput">The input type.</typeparam>
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
        /// <param name="inputEnumeration">The enumeration of inputs.</param>
        /// <param name="func1">Function that returns item 1 of the tuple.</param>
        /// <param name="func2">Function that returns item 2 of the tuple.</param>
        /// <param name="func3">Function that returns item 3 of the tuple.</param>
        /// <param name="func4">Function that returns item 4 of the tuple.</param>
        /// <param name="func5">Function that returns item 5 of the tuple.</param>
        /// <param name="func6">Function that returns item 6 of the tuple.</param>
        /// <param name="func7">Function that returns item 7 of the tuple.</param>
        /// <param name="func8">Function that returns item 8 of the tuple.</param>
        /// <param name="func9">Function that returns item 9 of the tuple.</param>
        /// <param name="func10">Function that returns item 10 of the tuple.</param>
        /// <param name="func11">Function that returns item 11 of the tuple.</param>
        /// <param name="func12">Function that returns item 12 of the tuple.</param>
        /// <param name="func13">Function that returns item 13 of the tuple.</param>
        /// <param name="func14">Function that returns item 14 of the tuple.</param>
        /// <param name="func15">Function that returns item 15 of the tuple.</param>
        /// <param name="func16">Function that returns item 16 of the tuple.</param>
        /// <param name="func17">Function that returns item 17 of the tuple.</param>
        /// <returns>A tuple with 17 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static
        IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17>>>>
            ToTuple<TInput, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>(
            [NotNull] this IEnumerable<TInput> inputEnumeration,
            [NotNull] Func<TInput, T1> func1,
            [NotNull] Func<TInput, T2> func2,
            [NotNull] Func<TInput, T3> func3,
            [NotNull] Func<TInput, T4> func4,
            [NotNull] Func<TInput, T5> func5,
            [NotNull] Func<TInput, T6> func6,
            [NotNull] Func<TInput, T7> func7,
            [NotNull] Func<TInput, T8> func8,
            [NotNull] Func<TInput, T9> func9,
            [NotNull] Func<TInput, T10> func10,
            [NotNull] Func<TInput, T11> func11,
            [NotNull] Func<TInput, T12> func12,
            [NotNull] Func<TInput, T13> func13,
            [NotNull] Func<TInput, T14> func14,
            [NotNull] Func<TInput, T15> func15,
            [NotNull] Func<TInput, T16> func16,
            [NotNull] Func<TInput, T17> func17
            )
        {
            return
                inputEnumeration.Select(
                    input =>
                        new Tuple
                        <T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17>>>(
                        func1(input),
                        func2(input),
                        func3(input),
                        func4(input),
                        func5(input),
                        func6(input),
                        func7(input),
                        new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17>>(
                            func8(input),
                            func9(input),
                            func10(input),
                            func11(input),
                            func12(input),
                            func13(input),
                            func14(input),
                            new Tuple<T15, T16, T17>(func15(input), func16(input), func17(input)))));
        }
        #endregion

        #region 18 items.
        /// <summary>
        /// Used to create a tuple in nested format.
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
        /// <param name="item1">Item 1 of the tuple.</param>
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
        /// <returns>A tuple with 18 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static
        Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18>>> Create
            <T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18>(
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
            T18 item18
            )
        {
            return
                new Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18>>>
                    (
                    item1,
                    item2,
                    item3,
                    item4,
                    item5,
                    item6,
                    item7,
                    new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18>>(
                        item8,
                        item9,
                        item10,
                        item11,
                        item12,
                        item13,
                        item14,
                        new Tuple<T15, T16, T17, T18>(item15, item16, item17, item18)));
        }

        /// <summary>
        /// Used to create an enumeration of tuples from an enumeration of objects, by specifying how each item is extracted.
        /// </summary>
        /// <typeparam name="TInput">The input type.</typeparam>
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
        /// <param name="inputEnumeration">The enumeration of inputs.</param>
        /// <param name="func1">Function that returns item 1 of the tuple.</param>
        /// <param name="func2">Function that returns item 2 of the tuple.</param>
        /// <param name="func3">Function that returns item 3 of the tuple.</param>
        /// <param name="func4">Function that returns item 4 of the tuple.</param>
        /// <param name="func5">Function that returns item 5 of the tuple.</param>
        /// <param name="func6">Function that returns item 6 of the tuple.</param>
        /// <param name="func7">Function that returns item 7 of the tuple.</param>
        /// <param name="func8">Function that returns item 8 of the tuple.</param>
        /// <param name="func9">Function that returns item 9 of the tuple.</param>
        /// <param name="func10">Function that returns item 10 of the tuple.</param>
        /// <param name="func11">Function that returns item 11 of the tuple.</param>
        /// <param name="func12">Function that returns item 12 of the tuple.</param>
        /// <param name="func13">Function that returns item 13 of the tuple.</param>
        /// <param name="func14">Function that returns item 14 of the tuple.</param>
        /// <param name="func15">Function that returns item 15 of the tuple.</param>
        /// <param name="func16">Function that returns item 16 of the tuple.</param>
        /// <param name="func17">Function that returns item 17 of the tuple.</param>
        /// <param name="func18">Function that returns item 18 of the tuple.</param>
        /// <returns>A tuple with 18 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static
        IEnumerable
            <Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18>>>>
            ToTuple<TInput, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18>(
            [NotNull] this IEnumerable<TInput> inputEnumeration,
            [NotNull] Func<TInput, T1> func1,
            [NotNull] Func<TInput, T2> func2,
            [NotNull] Func<TInput, T3> func3,
            [NotNull] Func<TInput, T4> func4,
            [NotNull] Func<TInput, T5> func5,
            [NotNull] Func<TInput, T6> func6,
            [NotNull] Func<TInput, T7> func7,
            [NotNull] Func<TInput, T8> func8,
            [NotNull] Func<TInput, T9> func9,
            [NotNull] Func<TInput, T10> func10,
            [NotNull] Func<TInput, T11> func11,
            [NotNull] Func<TInput, T12> func12,
            [NotNull] Func<TInput, T13> func13,
            [NotNull] Func<TInput, T14> func14,
            [NotNull] Func<TInput, T15> func15,
            [NotNull] Func<TInput, T16> func16,
            [NotNull] Func<TInput, T17> func17,
            [NotNull] Func<TInput, T18> func18
            )
        {
            return
                inputEnumeration.Select(
                    input =>
                        new Tuple
                        <T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18>>>(
                        func1(input),
                        func2(input),
                        func3(input),
                        func4(input),
                        func5(input),
                        func6(input),
                        func7(input),
                        new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18>>(
                            func8(input),
                            func9(input),
                            func10(input),
                            func11(input),
                            func12(input),
                            func13(input),
                            func14(input),
                            new Tuple<T15, T16, T17, T18>(func15(input), func16(input), func17(input), func18(input)))));
        }
        #endregion

        #region 19 items.
        /// <summary>
        /// Used to create a tuple in nested format.
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
        /// <param name="item1">Item 1 of the tuple.</param>
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
        /// <returns>A tuple with 19 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static
        Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19>>> Create
            <T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>(
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
            T19 item19
            )
        {
            return
                new Tuple
                    <T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19>>>
                    (
                    item1,
                    item2,
                    item3,
                    item4,
                    item5,
                    item6,
                    item7,
                    new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19>>(
                        item8,
                        item9,
                        item10,
                        item11,
                        item12,
                        item13,
                        item14,
                        new Tuple<T15, T16, T17, T18, T19>(item15, item16, item17, item18, item19)));
        }

        /// <summary>
        /// Used to create an enumeration of tuples from an enumeration of objects, by specifying how each item is extracted.
        /// </summary>
        /// <typeparam name="TInput">The input type.</typeparam>
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
        /// <param name="inputEnumeration">The enumeration of inputs.</param>
        /// <param name="func1">Function that returns item 1 of the tuple.</param>
        /// <param name="func2">Function that returns item 2 of the tuple.</param>
        /// <param name="func3">Function that returns item 3 of the tuple.</param>
        /// <param name="func4">Function that returns item 4 of the tuple.</param>
        /// <param name="func5">Function that returns item 5 of the tuple.</param>
        /// <param name="func6">Function that returns item 6 of the tuple.</param>
        /// <param name="func7">Function that returns item 7 of the tuple.</param>
        /// <param name="func8">Function that returns item 8 of the tuple.</param>
        /// <param name="func9">Function that returns item 9 of the tuple.</param>
        /// <param name="func10">Function that returns item 10 of the tuple.</param>
        /// <param name="func11">Function that returns item 11 of the tuple.</param>
        /// <param name="func12">Function that returns item 12 of the tuple.</param>
        /// <param name="func13">Function that returns item 13 of the tuple.</param>
        /// <param name="func14">Function that returns item 14 of the tuple.</param>
        /// <param name="func15">Function that returns item 15 of the tuple.</param>
        /// <param name="func16">Function that returns item 16 of the tuple.</param>
        /// <param name="func17">Function that returns item 17 of the tuple.</param>
        /// <param name="func18">Function that returns item 18 of the tuple.</param>
        /// <param name="func19">Function that returns item 19 of the tuple.</param>
        /// <returns>A tuple with 19 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static
        IEnumerable
            <Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19>>>>
            ToTuple<TInput, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>(
            [NotNull] this IEnumerable<TInput> inputEnumeration,
            [NotNull] Func<TInput, T1> func1,
            [NotNull] Func<TInput, T2> func2,
            [NotNull] Func<TInput, T3> func3,
            [NotNull] Func<TInput, T4> func4,
            [NotNull] Func<TInput, T5> func5,
            [NotNull] Func<TInput, T6> func6,
            [NotNull] Func<TInput, T7> func7,
            [NotNull] Func<TInput, T8> func8,
            [NotNull] Func<TInput, T9> func9,
            [NotNull] Func<TInput, T10> func10,
            [NotNull] Func<TInput, T11> func11,
            [NotNull] Func<TInput, T12> func12,
            [NotNull] Func<TInput, T13> func13,
            [NotNull] Func<TInput, T14> func14,
            [NotNull] Func<TInput, T15> func15,
            [NotNull] Func<TInput, T16> func16,
            [NotNull] Func<TInput, T17> func17,
            [NotNull] Func<TInput, T18> func18,
            [NotNull] Func<TInput, T19> func19
            )
        {
            return
                inputEnumeration.Select(
                    input =>
                        new Tuple
                        <T1, T2, T3, T4, T5, T6, T7,
                        Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19>>>(
                        func1(input),
                        func2(input),
                        func3(input),
                        func4(input),
                        func5(input),
                        func6(input),
                        func7(input),
                        new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19>>(
                            func8(input),
                            func9(input),
                            func10(input),
                            func11(input),
                            func12(input),
                            func13(input),
                            func14(input),
                            new Tuple<T15, T16, T17, T18, T19>(
                                func15(input),
                                func16(input),
                                func17(input),
                                func18(input),
                                func19(input)))));
        }
        #endregion

        #region 20 items.
        /// <summary>
        /// Used to create a tuple in nested format.
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
        /// <param name="item1">Item 1 of the tuple.</param>
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
        /// <returns>A tuple with 20 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static
        Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20>>>
            Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20>(
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
            T20 item20
            )
        {
            return
                new Tuple
                    <T1, T2, T3, T4, T5, T6, T7,
                        Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20>>>(
                    item1,
                    item2,
                    item3,
                    item4,
                    item5,
                    item6,
                    item7,
                    new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20>>(
                        item8,
                        item9,
                        item10,
                        item11,
                        item12,
                        item13,
                        item14,
                        new Tuple<T15, T16, T17, T18, T19, T20>(item15, item16, item17, item18, item19, item20)));
        }

        /// <summary>
        /// Used to create an enumeration of tuples from an enumeration of objects, by specifying how each item is extracted.
        /// </summary>
        /// <typeparam name="TInput">The input type.</typeparam>
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
        /// <param name="inputEnumeration">The enumeration of inputs.</param>
        /// <param name="func1">Function that returns item 1 of the tuple.</param>
        /// <param name="func2">Function that returns item 2 of the tuple.</param>
        /// <param name="func3">Function that returns item 3 of the tuple.</param>
        /// <param name="func4">Function that returns item 4 of the tuple.</param>
        /// <param name="func5">Function that returns item 5 of the tuple.</param>
        /// <param name="func6">Function that returns item 6 of the tuple.</param>
        /// <param name="func7">Function that returns item 7 of the tuple.</param>
        /// <param name="func8">Function that returns item 8 of the tuple.</param>
        /// <param name="func9">Function that returns item 9 of the tuple.</param>
        /// <param name="func10">Function that returns item 10 of the tuple.</param>
        /// <param name="func11">Function that returns item 11 of the tuple.</param>
        /// <param name="func12">Function that returns item 12 of the tuple.</param>
        /// <param name="func13">Function that returns item 13 of the tuple.</param>
        /// <param name="func14">Function that returns item 14 of the tuple.</param>
        /// <param name="func15">Function that returns item 15 of the tuple.</param>
        /// <param name="func16">Function that returns item 16 of the tuple.</param>
        /// <param name="func17">Function that returns item 17 of the tuple.</param>
        /// <param name="func18">Function that returns item 18 of the tuple.</param>
        /// <param name="func19">Function that returns item 19 of the tuple.</param>
        /// <param name="func20">Function that returns item 20 of the tuple.</param>
        /// <returns>A tuple with 20 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static
        IEnumerable
            <
                Tuple
                    <T1, T2, T3, T4, T5, T6, T7,
                        Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20>>>> ToTuple
            <TInput, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20>(
            [NotNull] this IEnumerable<TInput> inputEnumeration,
            [NotNull] Func<TInput, T1> func1,
            [NotNull] Func<TInput, T2> func2,
            [NotNull] Func<TInput, T3> func3,
            [NotNull] Func<TInput, T4> func4,
            [NotNull] Func<TInput, T5> func5,
            [NotNull] Func<TInput, T6> func6,
            [NotNull] Func<TInput, T7> func7,
            [NotNull] Func<TInput, T8> func8,
            [NotNull] Func<TInput, T9> func9,
            [NotNull] Func<TInput, T10> func10,
            [NotNull] Func<TInput, T11> func11,
            [NotNull] Func<TInput, T12> func12,
            [NotNull] Func<TInput, T13> func13,
            [NotNull] Func<TInput, T14> func14,
            [NotNull] Func<TInput, T15> func15,
            [NotNull] Func<TInput, T16> func16,
            [NotNull] Func<TInput, T17> func17,
            [NotNull] Func<TInput, T18> func18,
            [NotNull] Func<TInput, T19> func19,
            [NotNull] Func<TInput, T20> func20
            )
        {
            return
                inputEnumeration.Select(
                    input =>
                        new Tuple
                        <T1, T2, T3, T4, T5, T6, T7,
                        Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20>>>(
                        func1(input),
                        func2(input),
                        func3(input),
                        func4(input),
                        func5(input),
                        func6(input),
                        func7(input),
                        new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20>>(
                            func8(input),
                            func9(input),
                            func10(input),
                            func11(input),
                            func12(input),
                            func13(input),
                            func14(input),
                            new Tuple<T15, T16, T17, T18, T19, T20>(
                                func15(input),
                                func16(input),
                                func17(input),
                                func18(input),
                                func19(input),
                                func20(input)))));
        }
        #endregion

        #region 21 items.
        /// <summary>
        /// Used to create a tuple in nested format.
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
        /// <param name="item1">Item 1 of the tuple.</param>
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
        /// <returns>A tuple with 21 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static
        Tuple
            <T1, T2, T3, T4, T5, T6, T7,
                Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21>>> Create
            <T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21>(
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
            T21 item21
            )
        {
            return
                new Tuple
                    <T1, T2, T3, T4, T5, T6, T7,
                        Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21>>>(
                    item1,
                    item2,
                    item3,
                    item4,
                    item5,
                    item6,
                    item7,
                    new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21>>(
                        item8,
                        item9,
                        item10,
                        item11,
                        item12,
                        item13,
                        item14,
                        new Tuple<T15, T16, T17, T18, T19, T20, T21>(
                            item15,
                            item16,
                            item17,
                            item18,
                            item19,
                            item20,
                            item21)));
        }

        /// <summary>
        /// Used to create an enumeration of tuples from an enumeration of objects, by specifying how each item is extracted.
        /// </summary>
        /// <typeparam name="TInput">The input type.</typeparam>
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
        /// <param name="inputEnumeration">The enumeration of inputs.</param>
        /// <param name="func1">Function that returns item 1 of the tuple.</param>
        /// <param name="func2">Function that returns item 2 of the tuple.</param>
        /// <param name="func3">Function that returns item 3 of the tuple.</param>
        /// <param name="func4">Function that returns item 4 of the tuple.</param>
        /// <param name="func5">Function that returns item 5 of the tuple.</param>
        /// <param name="func6">Function that returns item 6 of the tuple.</param>
        /// <param name="func7">Function that returns item 7 of the tuple.</param>
        /// <param name="func8">Function that returns item 8 of the tuple.</param>
        /// <param name="func9">Function that returns item 9 of the tuple.</param>
        /// <param name="func10">Function that returns item 10 of the tuple.</param>
        /// <param name="func11">Function that returns item 11 of the tuple.</param>
        /// <param name="func12">Function that returns item 12 of the tuple.</param>
        /// <param name="func13">Function that returns item 13 of the tuple.</param>
        /// <param name="func14">Function that returns item 14 of the tuple.</param>
        /// <param name="func15">Function that returns item 15 of the tuple.</param>
        /// <param name="func16">Function that returns item 16 of the tuple.</param>
        /// <param name="func17">Function that returns item 17 of the tuple.</param>
        /// <param name="func18">Function that returns item 18 of the tuple.</param>
        /// <param name="func19">Function that returns item 19 of the tuple.</param>
        /// <param name="func20">Function that returns item 20 of the tuple.</param>
        /// <param name="func21">Function that returns item 21 of the tuple.</param>
        /// <returns>A tuple with 21 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static
        IEnumerable
            <
                Tuple
                    <T1, T2, T3, T4, T5, T6, T7,
                        Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21>>>> ToTuple
            <TInput, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21>(
            [NotNull] this IEnumerable<TInput> inputEnumeration,
            [NotNull] Func<TInput, T1> func1,
            [NotNull] Func<TInput, T2> func2,
            [NotNull] Func<TInput, T3> func3,
            [NotNull] Func<TInput, T4> func4,
            [NotNull] Func<TInput, T5> func5,
            [NotNull] Func<TInput, T6> func6,
            [NotNull] Func<TInput, T7> func7,
            [NotNull] Func<TInput, T8> func8,
            [NotNull] Func<TInput, T9> func9,
            [NotNull] Func<TInput, T10> func10,
            [NotNull] Func<TInput, T11> func11,
            [NotNull] Func<TInput, T12> func12,
            [NotNull] Func<TInput, T13> func13,
            [NotNull] Func<TInput, T14> func14,
            [NotNull] Func<TInput, T15> func15,
            [NotNull] Func<TInput, T16> func16,
            [NotNull] Func<TInput, T17> func17,
            [NotNull] Func<TInput, T18> func18,
            [NotNull] Func<TInput, T19> func19,
            [NotNull] Func<TInput, T20> func20,
            [NotNull] Func<TInput, T21> func21
            )
        {
            return
                inputEnumeration.Select(
                    input =>
                        new Tuple
                        <T1, T2, T3, T4, T5, T6, T7,
                        Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21>>>(
                        func1(input),
                        func2(input),
                        func3(input),
                        func4(input),
                        func5(input),
                        func6(input),
                        func7(input),
                        new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21>>(
                            func8(input),
                            func9(input),
                            func10(input),
                            func11(input),
                            func12(input),
                            func13(input),
                            func14(input),
                            new Tuple<T15, T16, T17, T18, T19, T20, T21>(
                                func15(input),
                                func16(input),
                                func17(input),
                                func18(input),
                                func19(input),
                                func20(input),
                                func21(input)))));
        }
        #endregion

        #region 22 items.
        /// <summary>
        /// Used to create a tuple in nested format.
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
        /// <param name="item1">Item 1 of the tuple.</param>
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
        /// <returns>A tuple with 22 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static
        Tuple
            <T1, T2, T3, T4, T5, T6, T7,
                Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22>>>> Create
            <T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22>(
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
            T22 item22
            )
        {
            return
                new Tuple
                    <T1, T2, T3, T4, T5, T6, T7,
                        Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22>>>>(
                    item1,
                    item2,
                    item3,
                    item4,
                    item5,
                    item6,
                    item7,
                    new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22>>>(
                        item8,
                        item9,
                        item10,
                        item11,
                        item12,
                        item13,
                        item14,
                        new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22>>(
                            item15,
                            item16,
                            item17,
                            item18,
                            item19,
                            item20,
                            item21,
                            new Tuple<T22>(item22))));
        }

        /// <summary>
        /// Used to create an enumeration of tuples from an enumeration of objects, by specifying how each item is extracted.
        /// </summary>
        /// <typeparam name="TInput">The input type.</typeparam>
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
        /// <param name="inputEnumeration">The enumeration of inputs.</param>
        /// <param name="func1">Function that returns item 1 of the tuple.</param>
        /// <param name="func2">Function that returns item 2 of the tuple.</param>
        /// <param name="func3">Function that returns item 3 of the tuple.</param>
        /// <param name="func4">Function that returns item 4 of the tuple.</param>
        /// <param name="func5">Function that returns item 5 of the tuple.</param>
        /// <param name="func6">Function that returns item 6 of the tuple.</param>
        /// <param name="func7">Function that returns item 7 of the tuple.</param>
        /// <param name="func8">Function that returns item 8 of the tuple.</param>
        /// <param name="func9">Function that returns item 9 of the tuple.</param>
        /// <param name="func10">Function that returns item 10 of the tuple.</param>
        /// <param name="func11">Function that returns item 11 of the tuple.</param>
        /// <param name="func12">Function that returns item 12 of the tuple.</param>
        /// <param name="func13">Function that returns item 13 of the tuple.</param>
        /// <param name="func14">Function that returns item 14 of the tuple.</param>
        /// <param name="func15">Function that returns item 15 of the tuple.</param>
        /// <param name="func16">Function that returns item 16 of the tuple.</param>
        /// <param name="func17">Function that returns item 17 of the tuple.</param>
        /// <param name="func18">Function that returns item 18 of the tuple.</param>
        /// <param name="func19">Function that returns item 19 of the tuple.</param>
        /// <param name="func20">Function that returns item 20 of the tuple.</param>
        /// <param name="func21">Function that returns item 21 of the tuple.</param>
        /// <param name="func22">Function that returns item 22 of the tuple.</param>
        /// <returns>A tuple with 22 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static
        IEnumerable
            <
                Tuple
                    <T1, T2, T3, T4, T5, T6, T7,
                        Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22>>>>>
            ToTuple
            <TInput, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22>
            (
            [NotNull] this IEnumerable<TInput> inputEnumeration,
            [NotNull] Func<TInput, T1> func1,
            [NotNull] Func<TInput, T2> func2,
            [NotNull] Func<TInput, T3> func3,
            [NotNull] Func<TInput, T4> func4,
            [NotNull] Func<TInput, T5> func5,
            [NotNull] Func<TInput, T6> func6,
            [NotNull] Func<TInput, T7> func7,
            [NotNull] Func<TInput, T8> func8,
            [NotNull] Func<TInput, T9> func9,
            [NotNull] Func<TInput, T10> func10,
            [NotNull] Func<TInput, T11> func11,
            [NotNull] Func<TInput, T12> func12,
            [NotNull] Func<TInput, T13> func13,
            [NotNull] Func<TInput, T14> func14,
            [NotNull] Func<TInput, T15> func15,
            [NotNull] Func<TInput, T16> func16,
            [NotNull] Func<TInput, T17> func17,
            [NotNull] Func<TInput, T18> func18,
            [NotNull] Func<TInput, T19> func19,
            [NotNull] Func<TInput, T20> func20,
            [NotNull] Func<TInput, T21> func21,
            [NotNull] Func<TInput, T22> func22
            )
        {
            return
                inputEnumeration.Select(
                    input =>
                        new Tuple
                        <T1, T2, T3, T4, T5, T6, T7,
                        Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22>>>>(
                        func1(input),
                        func2(input),
                        func3(input),
                        func4(input),
                        func5(input),
                        func6(input),
                        func7(input),
                        new Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22>>>
                            (
                            func8(input),
                            func9(input),
                            func10(input),
                            func11(input),
                            func12(input),
                            func13(input),
                            func14(input),
                            new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22>>(
                                func15(input),
                                func16(input),
                                func17(input),
                                func18(input),
                                func19(input),
                                func20(input),
                                func21(input),
                                new Tuple<T22>(func22(input))))));
        }
        #endregion

        #region 23 items.
        /// <summary>
        /// Used to create a tuple in nested format.
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
        /// <param name="item1">Item 1 of the tuple.</param>
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
        /// <returns>A tuple with 23 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static
        Tuple
            <T1, T2, T3, T4, T5, T6, T7,
                Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23>>>>
            Create
            <T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23>(
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
            T23 item23
            )
        {
            return
                new Tuple
                    <T1, T2, T3, T4, T5, T6, T7,
                        Tuple
                            <T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23>>>
                        >(
                    item1,
                    item2,
                    item3,
                    item4,
                    item5,
                    item6,
                    item7,
                    new Tuple
                        <T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23>>>(
                        item8,
                        item9,
                        item10,
                        item11,
                        item12,
                        item13,
                        item14,
                        new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23>>(
                            item15,
                            item16,
                            item17,
                            item18,
                            item19,
                            item20,
                            item21,
                            new Tuple<T22, T23>(item22, item23))));
        }

        /// <summary>
        /// Used to create an enumeration of tuples from an enumeration of objects, by specifying how each item is extracted.
        /// </summary>
        /// <typeparam name="TInput">The input type.</typeparam>
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
        /// <param name="inputEnumeration">The enumeration of inputs.</param>
        /// <param name="func1">Function that returns item 1 of the tuple.</param>
        /// <param name="func2">Function that returns item 2 of the tuple.</param>
        /// <param name="func3">Function that returns item 3 of the tuple.</param>
        /// <param name="func4">Function that returns item 4 of the tuple.</param>
        /// <param name="func5">Function that returns item 5 of the tuple.</param>
        /// <param name="func6">Function that returns item 6 of the tuple.</param>
        /// <param name="func7">Function that returns item 7 of the tuple.</param>
        /// <param name="func8">Function that returns item 8 of the tuple.</param>
        /// <param name="func9">Function that returns item 9 of the tuple.</param>
        /// <param name="func10">Function that returns item 10 of the tuple.</param>
        /// <param name="func11">Function that returns item 11 of the tuple.</param>
        /// <param name="func12">Function that returns item 12 of the tuple.</param>
        /// <param name="func13">Function that returns item 13 of the tuple.</param>
        /// <param name="func14">Function that returns item 14 of the tuple.</param>
        /// <param name="func15">Function that returns item 15 of the tuple.</param>
        /// <param name="func16">Function that returns item 16 of the tuple.</param>
        /// <param name="func17">Function that returns item 17 of the tuple.</param>
        /// <param name="func18">Function that returns item 18 of the tuple.</param>
        /// <param name="func19">Function that returns item 19 of the tuple.</param>
        /// <param name="func20">Function that returns item 20 of the tuple.</param>
        /// <param name="func21">Function that returns item 21 of the tuple.</param>
        /// <param name="func22">Function that returns item 22 of the tuple.</param>
        /// <param name="func23">Function that returns item 23 of the tuple.</param>
        /// <returns>A tuple with 23 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static
        IEnumerable
            <
                Tuple
                    <T1, T2, T3, T4, T5, T6, T7,
                        Tuple
                            <T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23>>>
                        >> ToTuple
            <TInput, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22,
                T23>(
            [NotNull] this IEnumerable<TInput> inputEnumeration,
            [NotNull] Func<TInput, T1> func1,
            [NotNull] Func<TInput, T2> func2,
            [NotNull] Func<TInput, T3> func3,
            [NotNull] Func<TInput, T4> func4,
            [NotNull] Func<TInput, T5> func5,
            [NotNull] Func<TInput, T6> func6,
            [NotNull] Func<TInput, T7> func7,
            [NotNull] Func<TInput, T8> func8,
            [NotNull] Func<TInput, T9> func9,
            [NotNull] Func<TInput, T10> func10,
            [NotNull] Func<TInput, T11> func11,
            [NotNull] Func<TInput, T12> func12,
            [NotNull] Func<TInput, T13> func13,
            [NotNull] Func<TInput, T14> func14,
            [NotNull] Func<TInput, T15> func15,
            [NotNull] Func<TInput, T16> func16,
            [NotNull] Func<TInput, T17> func17,
            [NotNull] Func<TInput, T18> func18,
            [NotNull] Func<TInput, T19> func19,
            [NotNull] Func<TInput, T20> func20,
            [NotNull] Func<TInput, T21> func21,
            [NotNull] Func<TInput, T22> func22,
            [NotNull] Func<TInput, T23> func23
            )
        {
            return
                inputEnumeration.Select(
                    input =>
                        new Tuple
                        <T1, T2, T3, T4, T5, T6, T7,
                        Tuple
                        <T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23>>>>(
                        func1(input),
                        func2(input),
                        func3(input),
                        func4(input),
                        func5(input),
                        func6(input),
                        func7(input),
                        new Tuple
                            <T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23>>>
                            (
                            func8(input),
                            func9(input),
                            func10(input),
                            func11(input),
                            func12(input),
                            func13(input),
                            func14(input),
                            new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23>>(
                                func15(input),
                                func16(input),
                                func17(input),
                                func18(input),
                                func19(input),
                                func20(input),
                                func21(input),
                                new Tuple<T22, T23>(func22(input), func23(input))))));
        }
        #endregion

        #region 24 items.
        /// <summary>
        /// Used to create a tuple in nested format.
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
        /// <param name="item1">Item 1 of the tuple.</param>
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
        /// <returns>A tuple with 24 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static
        Tuple
            <T1, T2, T3, T4, T5, T6, T7,
                Tuple<T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24>>>>
            Create
            <T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23,
                T24>(
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
            T24 item24
            )
        {
            return
                new Tuple
                    <T1, T2, T3, T4, T5, T6, T7,
                        Tuple
                            <T8, T9, T10, T11, T12, T13, T14,
                                Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24>>>>(
                    item1,
                    item2,
                    item3,
                    item4,
                    item5,
                    item6,
                    item7,
                    new Tuple
                        <T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24>>
                        >(
                        item8,
                        item9,
                        item10,
                        item11,
                        item12,
                        item13,
                        item14,
                        new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24>>(
                            item15,
                            item16,
                            item17,
                            item18,
                            item19,
                            item20,
                            item21,
                            new Tuple<T22, T23, T24>(item22, item23, item24))));
        }

        /// <summary>
        /// Used to create an enumeration of tuples from an enumeration of objects, by specifying how each item is extracted.
        /// </summary>
        /// <typeparam name="TInput">The input type.</typeparam>
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
        /// <param name="inputEnumeration">The enumeration of inputs.</param>
        /// <param name="func1">Function that returns item 1 of the tuple.</param>
        /// <param name="func2">Function that returns item 2 of the tuple.</param>
        /// <param name="func3">Function that returns item 3 of the tuple.</param>
        /// <param name="func4">Function that returns item 4 of the tuple.</param>
        /// <param name="func5">Function that returns item 5 of the tuple.</param>
        /// <param name="func6">Function that returns item 6 of the tuple.</param>
        /// <param name="func7">Function that returns item 7 of the tuple.</param>
        /// <param name="func8">Function that returns item 8 of the tuple.</param>
        /// <param name="func9">Function that returns item 9 of the tuple.</param>
        /// <param name="func10">Function that returns item 10 of the tuple.</param>
        /// <param name="func11">Function that returns item 11 of the tuple.</param>
        /// <param name="func12">Function that returns item 12 of the tuple.</param>
        /// <param name="func13">Function that returns item 13 of the tuple.</param>
        /// <param name="func14">Function that returns item 14 of the tuple.</param>
        /// <param name="func15">Function that returns item 15 of the tuple.</param>
        /// <param name="func16">Function that returns item 16 of the tuple.</param>
        /// <param name="func17">Function that returns item 17 of the tuple.</param>
        /// <param name="func18">Function that returns item 18 of the tuple.</param>
        /// <param name="func19">Function that returns item 19 of the tuple.</param>
        /// <param name="func20">Function that returns item 20 of the tuple.</param>
        /// <param name="func21">Function that returns item 21 of the tuple.</param>
        /// <param name="func22">Function that returns item 22 of the tuple.</param>
        /// <param name="func23">Function that returns item 23 of the tuple.</param>
        /// <param name="func24">Function that returns item 24 of the tuple.</param>
        /// <returns>A tuple with 24 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static
        IEnumerable
            <
                Tuple
                    <T1, T2, T3, T4, T5, T6, T7,
                        Tuple
                            <T8, T9, T10, T11, T12, T13, T14,
                                Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24>>>>> ToTuple
            <TInput, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22,
                T23, T24>(
            [NotNull] this IEnumerable<TInput> inputEnumeration,
            [NotNull] Func<TInput, T1> func1,
            [NotNull] Func<TInput, T2> func2,
            [NotNull] Func<TInput, T3> func3,
            [NotNull] Func<TInput, T4> func4,
            [NotNull] Func<TInput, T5> func5,
            [NotNull] Func<TInput, T6> func6,
            [NotNull] Func<TInput, T7> func7,
            [NotNull] Func<TInput, T8> func8,
            [NotNull] Func<TInput, T9> func9,
            [NotNull] Func<TInput, T10> func10,
            [NotNull] Func<TInput, T11> func11,
            [NotNull] Func<TInput, T12> func12,
            [NotNull] Func<TInput, T13> func13,
            [NotNull] Func<TInput, T14> func14,
            [NotNull] Func<TInput, T15> func15,
            [NotNull] Func<TInput, T16> func16,
            [NotNull] Func<TInput, T17> func17,
            [NotNull] Func<TInput, T18> func18,
            [NotNull] Func<TInput, T19> func19,
            [NotNull] Func<TInput, T20> func20,
            [NotNull] Func<TInput, T21> func21,
            [NotNull] Func<TInput, T22> func22,
            [NotNull] Func<TInput, T23> func23,
            [NotNull] Func<TInput, T24> func24
            )
        {
            return
                inputEnumeration.Select(
                    input =>
                        new Tuple
                        <T1, T2, T3, T4, T5, T6, T7,
                        Tuple
                        <T8, T9, T10, T11, T12, T13, T14, Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24>>
                        >>(
                        func1(input),
                        func2(input),
                        func3(input),
                        func4(input),
                        func5(input),
                        func6(input),
                        func7(input),
                        new Tuple
                            <T8, T9, T10, T11, T12, T13, T14,
                            Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24>>>(
                            func8(input),
                            func9(input),
                            func10(input),
                            func11(input),
                            func12(input),
                            func13(input),
                            func14(input),
                            new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24>>(
                                func15(input),
                                func16(input),
                                func17(input),
                                func18(input),
                                func19(input),
                                func20(input),
                                func21(input),
                                new Tuple<T22, T23, T24>(func22(input), func23(input), func24(input))))));
        }
        #endregion

        #region 25 items.
        /// <summary>
        /// Used to create a tuple in nested format.
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
        /// <param name="item1">Item 1 of the tuple.</param>
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
        /// <returns>A tuple with 25 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static
        Tuple
            <T1, T2, T3, T4, T5, T6, T7,
                Tuple
                    <T8, T9, T10, T11, T12, T13, T14,
                        Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25>>>> Create
            <T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23,
                T24, T25>(
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
            T25 item25
            )
        {
            return
                new Tuple
                    <T1, T2, T3, T4, T5, T6, T7,
                        Tuple
                            <T8, T9, T10, T11, T12, T13, T14,
                                Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25>>>>(
                    item1,
                    item2,
                    item3,
                    item4,
                    item5,
                    item6,
                    item7,
                    new Tuple
                        <T8, T9, T10, T11, T12, T13, T14,
                        Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25>>>(
                        item8,
                        item9,
                        item10,
                        item11,
                        item12,
                        item13,
                        item14,
                        new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25>>(
                            item15,
                            item16,
                            item17,
                            item18,
                            item19,
                            item20,
                            item21,
                            new Tuple<T22, T23, T24, T25>(item22, item23, item24, item25))));
        }

        /// <summary>
        /// Used to create an enumeration of tuples from an enumeration of objects, by specifying how each item is extracted.
        /// </summary>
        /// <typeparam name="TInput">The input type.</typeparam>
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
        /// <param name="inputEnumeration">The enumeration of inputs.</param>
        /// <param name="func1">Function that returns item 1 of the tuple.</param>
        /// <param name="func2">Function that returns item 2 of the tuple.</param>
        /// <param name="func3">Function that returns item 3 of the tuple.</param>
        /// <param name="func4">Function that returns item 4 of the tuple.</param>
        /// <param name="func5">Function that returns item 5 of the tuple.</param>
        /// <param name="func6">Function that returns item 6 of the tuple.</param>
        /// <param name="func7">Function that returns item 7 of the tuple.</param>
        /// <param name="func8">Function that returns item 8 of the tuple.</param>
        /// <param name="func9">Function that returns item 9 of the tuple.</param>
        /// <param name="func10">Function that returns item 10 of the tuple.</param>
        /// <param name="func11">Function that returns item 11 of the tuple.</param>
        /// <param name="func12">Function that returns item 12 of the tuple.</param>
        /// <param name="func13">Function that returns item 13 of the tuple.</param>
        /// <param name="func14">Function that returns item 14 of the tuple.</param>
        /// <param name="func15">Function that returns item 15 of the tuple.</param>
        /// <param name="func16">Function that returns item 16 of the tuple.</param>
        /// <param name="func17">Function that returns item 17 of the tuple.</param>
        /// <param name="func18">Function that returns item 18 of the tuple.</param>
        /// <param name="func19">Function that returns item 19 of the tuple.</param>
        /// <param name="func20">Function that returns item 20 of the tuple.</param>
        /// <param name="func21">Function that returns item 21 of the tuple.</param>
        /// <param name="func22">Function that returns item 22 of the tuple.</param>
        /// <param name="func23">Function that returns item 23 of the tuple.</param>
        /// <param name="func24">Function that returns item 24 of the tuple.</param>
        /// <param name="func25">Function that returns item 25 of the tuple.</param>
        /// <returns>A tuple with 25 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static
        IEnumerable
            <
                Tuple
                    <T1, T2, T3, T4, T5, T6, T7,
                        Tuple
                            <T8, T9, T10, T11, T12, T13, T14,
                                Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25>>>>> ToTuple
            <TInput, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22,
                T23, T24, T25>(
            [NotNull] this IEnumerable<TInput> inputEnumeration,
            [NotNull] Func<TInput, T1> func1,
            [NotNull] Func<TInput, T2> func2,
            [NotNull] Func<TInput, T3> func3,
            [NotNull] Func<TInput, T4> func4,
            [NotNull] Func<TInput, T5> func5,
            [NotNull] Func<TInput, T6> func6,
            [NotNull] Func<TInput, T7> func7,
            [NotNull] Func<TInput, T8> func8,
            [NotNull] Func<TInput, T9> func9,
            [NotNull] Func<TInput, T10> func10,
            [NotNull] Func<TInput, T11> func11,
            [NotNull] Func<TInput, T12> func12,
            [NotNull] Func<TInput, T13> func13,
            [NotNull] Func<TInput, T14> func14,
            [NotNull] Func<TInput, T15> func15,
            [NotNull] Func<TInput, T16> func16,
            [NotNull] Func<TInput, T17> func17,
            [NotNull] Func<TInput, T18> func18,
            [NotNull] Func<TInput, T19> func19,
            [NotNull] Func<TInput, T20> func20,
            [NotNull] Func<TInput, T21> func21,
            [NotNull] Func<TInput, T22> func22,
            [NotNull] Func<TInput, T23> func23,
            [NotNull] Func<TInput, T24> func24,
            [NotNull] Func<TInput, T25> func25
            )
        {
            return
                inputEnumeration.Select(
                    input =>
                        new Tuple
                        <T1, T2, T3, T4, T5, T6, T7,
                        Tuple
                        <T8, T9, T10, T11, T12, T13, T14,
                        Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25>>>>(
                        func1(input),
                        func2(input),
                        func3(input),
                        func4(input),
                        func5(input),
                        func6(input),
                        func7(input),
                        new Tuple
                            <T8, T9, T10, T11, T12, T13, T14,
                            Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25>>>(
                            func8(input),
                            func9(input),
                            func10(input),
                            func11(input),
                            func12(input),
                            func13(input),
                            func14(input),
                            new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25>>(
                                func15(input),
                                func16(input),
                                func17(input),
                                func18(input),
                                func19(input),
                                func20(input),
                                func21(input),
                                new Tuple<T22, T23, T24, T25>(
                                    func22(input),
                                    func23(input),
                                    func24(input),
                                    func25(input))))));
        }
        #endregion

        #region 26 items.
        /// <summary>
        /// Used to create a tuple in nested format.
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
        /// <param name="item1">Item 1 of the tuple.</param>
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
        /// <returns>A tuple with 26 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static
        Tuple
            <T1, T2, T3, T4, T5, T6, T7,
                Tuple
                    <T8, T9, T10, T11, T12, T13, T14,
                        Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26>>>> Create
            <T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23,
                T24, T25, T26>(
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
            T26 item26
            )
        {
            return
                new Tuple
                    <T1, T2, T3, T4, T5, T6, T7,
                        Tuple
                            <T8, T9, T10, T11, T12, T13, T14,
                                Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26>>>>(
                    item1,
                    item2,
                    item3,
                    item4,
                    item5,
                    item6,
                    item7,
                    new Tuple
                        <T8, T9, T10, T11, T12, T13, T14,
                        Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26>>>(
                        item8,
                        item9,
                        item10,
                        item11,
                        item12,
                        item13,
                        item14,
                        new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26>>(
                            item15,
                            item16,
                            item17,
                            item18,
                            item19,
                            item20,
                            item21,
                            new Tuple<T22, T23, T24, T25, T26>(item22, item23, item24, item25, item26))));
        }

        /// <summary>
        /// Used to create an enumeration of tuples from an enumeration of objects, by specifying how each item is extracted.
        /// </summary>
        /// <typeparam name="TInput">The input type.</typeparam>
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
        /// <param name="inputEnumeration">The enumeration of inputs.</param>
        /// <param name="func1">Function that returns item 1 of the tuple.</param>
        /// <param name="func2">Function that returns item 2 of the tuple.</param>
        /// <param name="func3">Function that returns item 3 of the tuple.</param>
        /// <param name="func4">Function that returns item 4 of the tuple.</param>
        /// <param name="func5">Function that returns item 5 of the tuple.</param>
        /// <param name="func6">Function that returns item 6 of the tuple.</param>
        /// <param name="func7">Function that returns item 7 of the tuple.</param>
        /// <param name="func8">Function that returns item 8 of the tuple.</param>
        /// <param name="func9">Function that returns item 9 of the tuple.</param>
        /// <param name="func10">Function that returns item 10 of the tuple.</param>
        /// <param name="func11">Function that returns item 11 of the tuple.</param>
        /// <param name="func12">Function that returns item 12 of the tuple.</param>
        /// <param name="func13">Function that returns item 13 of the tuple.</param>
        /// <param name="func14">Function that returns item 14 of the tuple.</param>
        /// <param name="func15">Function that returns item 15 of the tuple.</param>
        /// <param name="func16">Function that returns item 16 of the tuple.</param>
        /// <param name="func17">Function that returns item 17 of the tuple.</param>
        /// <param name="func18">Function that returns item 18 of the tuple.</param>
        /// <param name="func19">Function that returns item 19 of the tuple.</param>
        /// <param name="func20">Function that returns item 20 of the tuple.</param>
        /// <param name="func21">Function that returns item 21 of the tuple.</param>
        /// <param name="func22">Function that returns item 22 of the tuple.</param>
        /// <param name="func23">Function that returns item 23 of the tuple.</param>
        /// <param name="func24">Function that returns item 24 of the tuple.</param>
        /// <param name="func25">Function that returns item 25 of the tuple.</param>
        /// <param name="func26">Function that returns item 26 of the tuple.</param>
        /// <returns>A tuple with 26 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static
        IEnumerable
            <
                Tuple
                    <T1, T2, T3, T4, T5, T6, T7,
                        Tuple
                            <T8, T9, T10, T11, T12, T13, T14,
                                Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26>>>>> ToTuple
            <TInput, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22,
                T23, T24, T25, T26>(
            [NotNull] this IEnumerable<TInput> inputEnumeration,
            [NotNull] Func<TInput, T1> func1,
            [NotNull] Func<TInput, T2> func2,
            [NotNull] Func<TInput, T3> func3,
            [NotNull] Func<TInput, T4> func4,
            [NotNull] Func<TInput, T5> func5,
            [NotNull] Func<TInput, T6> func6,
            [NotNull] Func<TInput, T7> func7,
            [NotNull] Func<TInput, T8> func8,
            [NotNull] Func<TInput, T9> func9,
            [NotNull] Func<TInput, T10> func10,
            [NotNull] Func<TInput, T11> func11,
            [NotNull] Func<TInput, T12> func12,
            [NotNull] Func<TInput, T13> func13,
            [NotNull] Func<TInput, T14> func14,
            [NotNull] Func<TInput, T15> func15,
            [NotNull] Func<TInput, T16> func16,
            [NotNull] Func<TInput, T17> func17,
            [NotNull] Func<TInput, T18> func18,
            [NotNull] Func<TInput, T19> func19,
            [NotNull] Func<TInput, T20> func20,
            [NotNull] Func<TInput, T21> func21,
            [NotNull] Func<TInput, T22> func22,
            [NotNull] Func<TInput, T23> func23,
            [NotNull] Func<TInput, T24> func24,
            [NotNull] Func<TInput, T25> func25,
            [NotNull] Func<TInput, T26> func26
            )
        {
            return
                inputEnumeration.Select(
                    input =>
                        new Tuple
                        <T1, T2, T3, T4, T5, T6, T7,
                        Tuple
                        <T8, T9, T10, T11, T12, T13, T14,
                        Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26>>>>(
                        func1(input),
                        func2(input),
                        func3(input),
                        func4(input),
                        func5(input),
                        func6(input),
                        func7(input),
                        new Tuple
                            <T8, T9, T10, T11, T12, T13, T14,
                            Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26>>>(
                            func8(input),
                            func9(input),
                            func10(input),
                            func11(input),
                            func12(input),
                            func13(input),
                            func14(input),
                            new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26>>(
                                func15(input),
                                func16(input),
                                func17(input),
                                func18(input),
                                func19(input),
                                func20(input),
                                func21(input),
                                new Tuple<T22, T23, T24, T25, T26>(
                                    func22(input),
                                    func23(input),
                                    func24(input),
                                    func25(input),
                                    func26(input))))));
        }
        #endregion

        #region 27 items.
        /// <summary>
        /// Used to create a tuple in nested format.
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
        /// <param name="item1">Item 1 of the tuple.</param>
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
        /// <returns>A tuple with 27 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static
        Tuple
            <T1, T2, T3, T4, T5, T6, T7,
                Tuple
                    <T8, T9, T10, T11, T12, T13, T14,
                        Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27>>>> Create
            <T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23,
                T24, T25, T26, T27>(
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
            T27 item27
            )
        {
            return
                new Tuple
                    <T1, T2, T3, T4, T5, T6, T7,
                        Tuple
                            <T8, T9, T10, T11, T12, T13, T14,
                                Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27>>>>(
                    item1,
                    item2,
                    item3,
                    item4,
                    item5,
                    item6,
                    item7,
                    new Tuple
                        <T8, T9, T10, T11, T12, T13, T14,
                        Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27>>>(
                        item8,
                        item9,
                        item10,
                        item11,
                        item12,
                        item13,
                        item14,
                        new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27>>(
                            item15,
                            item16,
                            item17,
                            item18,
                            item19,
                            item20,
                            item21,
                            new Tuple<T22, T23, T24, T25, T26, T27>(item22, item23, item24, item25, item26, item27))));
        }

        /// <summary>
        /// Used to create an enumeration of tuples from an enumeration of objects, by specifying how each item is extracted.
        /// </summary>
        /// <typeparam name="TInput">The input type.</typeparam>
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
        /// <param name="inputEnumeration">The enumeration of inputs.</param>
        /// <param name="func1">Function that returns item 1 of the tuple.</param>
        /// <param name="func2">Function that returns item 2 of the tuple.</param>
        /// <param name="func3">Function that returns item 3 of the tuple.</param>
        /// <param name="func4">Function that returns item 4 of the tuple.</param>
        /// <param name="func5">Function that returns item 5 of the tuple.</param>
        /// <param name="func6">Function that returns item 6 of the tuple.</param>
        /// <param name="func7">Function that returns item 7 of the tuple.</param>
        /// <param name="func8">Function that returns item 8 of the tuple.</param>
        /// <param name="func9">Function that returns item 9 of the tuple.</param>
        /// <param name="func10">Function that returns item 10 of the tuple.</param>
        /// <param name="func11">Function that returns item 11 of the tuple.</param>
        /// <param name="func12">Function that returns item 12 of the tuple.</param>
        /// <param name="func13">Function that returns item 13 of the tuple.</param>
        /// <param name="func14">Function that returns item 14 of the tuple.</param>
        /// <param name="func15">Function that returns item 15 of the tuple.</param>
        /// <param name="func16">Function that returns item 16 of the tuple.</param>
        /// <param name="func17">Function that returns item 17 of the tuple.</param>
        /// <param name="func18">Function that returns item 18 of the tuple.</param>
        /// <param name="func19">Function that returns item 19 of the tuple.</param>
        /// <param name="func20">Function that returns item 20 of the tuple.</param>
        /// <param name="func21">Function that returns item 21 of the tuple.</param>
        /// <param name="func22">Function that returns item 22 of the tuple.</param>
        /// <param name="func23">Function that returns item 23 of the tuple.</param>
        /// <param name="func24">Function that returns item 24 of the tuple.</param>
        /// <param name="func25">Function that returns item 25 of the tuple.</param>
        /// <param name="func26">Function that returns item 26 of the tuple.</param>
        /// <param name="func27">Function that returns item 27 of the tuple.</param>
        /// <returns>A tuple with 27 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static
        IEnumerable
            <
                Tuple
                    <T1, T2, T3, T4, T5, T6, T7,
                        Tuple
                            <T8, T9, T10, T11, T12, T13, T14,
                                Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27>>>>> ToTuple
            <TInput, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22,
                T23, T24, T25, T26, T27>(
            [NotNull] this IEnumerable<TInput> inputEnumeration,
            [NotNull] Func<TInput, T1> func1,
            [NotNull] Func<TInput, T2> func2,
            [NotNull] Func<TInput, T3> func3,
            [NotNull] Func<TInput, T4> func4,
            [NotNull] Func<TInput, T5> func5,
            [NotNull] Func<TInput, T6> func6,
            [NotNull] Func<TInput, T7> func7,
            [NotNull] Func<TInput, T8> func8,
            [NotNull] Func<TInput, T9> func9,
            [NotNull] Func<TInput, T10> func10,
            [NotNull] Func<TInput, T11> func11,
            [NotNull] Func<TInput, T12> func12,
            [NotNull] Func<TInput, T13> func13,
            [NotNull] Func<TInput, T14> func14,
            [NotNull] Func<TInput, T15> func15,
            [NotNull] Func<TInput, T16> func16,
            [NotNull] Func<TInput, T17> func17,
            [NotNull] Func<TInput, T18> func18,
            [NotNull] Func<TInput, T19> func19,
            [NotNull] Func<TInput, T20> func20,
            [NotNull] Func<TInput, T21> func21,
            [NotNull] Func<TInput, T22> func22,
            [NotNull] Func<TInput, T23> func23,
            [NotNull] Func<TInput, T24> func24,
            [NotNull] Func<TInput, T25> func25,
            [NotNull] Func<TInput, T26> func26,
            [NotNull] Func<TInput, T27> func27
            )
        {
            return
                inputEnumeration.Select(
                    input =>
                        new Tuple
                        <T1, T2, T3, T4, T5, T6, T7,
                        Tuple
                        <T8, T9, T10, T11, T12, T13, T14,
                        Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27>>>>(
                        func1(input),
                        func2(input),
                        func3(input),
                        func4(input),
                        func5(input),
                        func6(input),
                        func7(input),
                        new Tuple
                            <T8, T9, T10, T11, T12, T13, T14,
                            Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27>>>(
                            func8(input),
                            func9(input),
                            func10(input),
                            func11(input),
                            func12(input),
                            func13(input),
                            func14(input),
                            new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27>>(
                                func15(input),
                                func16(input),
                                func17(input),
                                func18(input),
                                func19(input),
                                func20(input),
                                func21(input),
                                new Tuple<T22, T23, T24, T25, T26, T27>(
                                    func22(input),
                                    func23(input),
                                    func24(input),
                                    func25(input),
                                    func26(input),
                                    func27(input))))));
        }
        #endregion

        #region 28 items.
        /// <summary>
        /// Used to create a tuple in nested format.
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
        /// <param name="item1">Item 1 of the tuple.</param>
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
        /// <returns>A tuple with 28 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static
        Tuple
            <T1, T2, T3, T4, T5, T6, T7,
                Tuple
                    <T8, T9, T10, T11, T12, T13, T14,
                        Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28>>>> Create
            <T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23,
                T24, T25, T26, T27, T28>(
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
            T28 item28
            )
        {
            return
                new Tuple
                    <T1, T2, T3, T4, T5, T6, T7,
                        Tuple
                            <T8, T9, T10, T11, T12, T13, T14,
                                Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28>>>>(
                    item1,
                    item2,
                    item3,
                    item4,
                    item5,
                    item6,
                    item7,
                    new Tuple
                        <T8, T9, T10, T11, T12, T13, T14,
                        Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28>>>(
                        item8,
                        item9,
                        item10,
                        item11,
                        item12,
                        item13,
                        item14,
                        new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28>>(
                            item15,
                            item16,
                            item17,
                            item18,
                            item19,
                            item20,
                            item21,
                            new Tuple<T22, T23, T24, T25, T26, T27, T28>(
                                item22,
                                item23,
                                item24,
                                item25,
                                item26,
                                item27,
                                item28))));
        }

        /// <summary>
        /// Used to create an enumeration of tuples from an enumeration of objects, by specifying how each item is extracted.
        /// </summary>
        /// <typeparam name="TInput">The input type.</typeparam>
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
        /// <param name="inputEnumeration">The enumeration of inputs.</param>
        /// <param name="func1">Function that returns item 1 of the tuple.</param>
        /// <param name="func2">Function that returns item 2 of the tuple.</param>
        /// <param name="func3">Function that returns item 3 of the tuple.</param>
        /// <param name="func4">Function that returns item 4 of the tuple.</param>
        /// <param name="func5">Function that returns item 5 of the tuple.</param>
        /// <param name="func6">Function that returns item 6 of the tuple.</param>
        /// <param name="func7">Function that returns item 7 of the tuple.</param>
        /// <param name="func8">Function that returns item 8 of the tuple.</param>
        /// <param name="func9">Function that returns item 9 of the tuple.</param>
        /// <param name="func10">Function that returns item 10 of the tuple.</param>
        /// <param name="func11">Function that returns item 11 of the tuple.</param>
        /// <param name="func12">Function that returns item 12 of the tuple.</param>
        /// <param name="func13">Function that returns item 13 of the tuple.</param>
        /// <param name="func14">Function that returns item 14 of the tuple.</param>
        /// <param name="func15">Function that returns item 15 of the tuple.</param>
        /// <param name="func16">Function that returns item 16 of the tuple.</param>
        /// <param name="func17">Function that returns item 17 of the tuple.</param>
        /// <param name="func18">Function that returns item 18 of the tuple.</param>
        /// <param name="func19">Function that returns item 19 of the tuple.</param>
        /// <param name="func20">Function that returns item 20 of the tuple.</param>
        /// <param name="func21">Function that returns item 21 of the tuple.</param>
        /// <param name="func22">Function that returns item 22 of the tuple.</param>
        /// <param name="func23">Function that returns item 23 of the tuple.</param>
        /// <param name="func24">Function that returns item 24 of the tuple.</param>
        /// <param name="func25">Function that returns item 25 of the tuple.</param>
        /// <param name="func26">Function that returns item 26 of the tuple.</param>
        /// <param name="func27">Function that returns item 27 of the tuple.</param>
        /// <param name="func28">Function that returns item 28 of the tuple.</param>
        /// <returns>A tuple with 28 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static
        IEnumerable
            <
                Tuple
                    <T1, T2, T3, T4, T5, T6, T7,
                        Tuple
                            <T8, T9, T10, T11, T12, T13, T14,
                                Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28>>>>>
            ToTuple
            <TInput, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22,
                T23, T24, T25, T26, T27, T28>(
            [NotNull] this IEnumerable<TInput> inputEnumeration,
            [NotNull] Func<TInput, T1> func1,
            [NotNull] Func<TInput, T2> func2,
            [NotNull] Func<TInput, T3> func3,
            [NotNull] Func<TInput, T4> func4,
            [NotNull] Func<TInput, T5> func5,
            [NotNull] Func<TInput, T6> func6,
            [NotNull] Func<TInput, T7> func7,
            [NotNull] Func<TInput, T8> func8,
            [NotNull] Func<TInput, T9> func9,
            [NotNull] Func<TInput, T10> func10,
            [NotNull] Func<TInput, T11> func11,
            [NotNull] Func<TInput, T12> func12,
            [NotNull] Func<TInput, T13> func13,
            [NotNull] Func<TInput, T14> func14,
            [NotNull] Func<TInput, T15> func15,
            [NotNull] Func<TInput, T16> func16,
            [NotNull] Func<TInput, T17> func17,
            [NotNull] Func<TInput, T18> func18,
            [NotNull] Func<TInput, T19> func19,
            [NotNull] Func<TInput, T20> func20,
            [NotNull] Func<TInput, T21> func21,
            [NotNull] Func<TInput, T22> func22,
            [NotNull] Func<TInput, T23> func23,
            [NotNull] Func<TInput, T24> func24,
            [NotNull] Func<TInput, T25> func25,
            [NotNull] Func<TInput, T26> func26,
            [NotNull] Func<TInput, T27> func27,
            [NotNull] Func<TInput, T28> func28
            )
        {
            return
                inputEnumeration.Select(
                    input =>
                        new Tuple
                        <T1, T2, T3, T4, T5, T6, T7,
                        Tuple
                        <T8, T9, T10, T11, T12, T13, T14,
                        Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28>>>>(
                        func1(input),
                        func2(input),
                        func3(input),
                        func4(input),
                        func5(input),
                        func6(input),
                        func7(input),
                        new Tuple
                            <T8, T9, T10, T11, T12, T13, T14,
                            Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28>>>(
                            func8(input),
                            func9(input),
                            func10(input),
                            func11(input),
                            func12(input),
                            func13(input),
                            func14(input),
                            new Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28>>(
                                func15(input),
                                func16(input),
                                func17(input),
                                func18(input),
                                func19(input),
                                func20(input),
                                func21(input),
                                new Tuple<T22, T23, T24, T25, T26, T27, T28>(
                                    func22(input),
                                    func23(input),
                                    func24(input),
                                    func25(input),
                                    func26(input),
                                    func27(input),
                                    func28(input))))));
        }
        #endregion

        #region 29 items.
        /// <summary>
        /// Used to create a tuple in nested format.
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
        /// <param name="item1">Item 1 of the tuple.</param>
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
        /// <returns>A tuple with 29 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static
        Tuple
            <T1, T2, T3, T4, T5, T6, T7,
                Tuple
                    <T8, T9, T10, T11, T12, T13, T14,
                        Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29>>>>>
            Create
            <T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23,
                T24, T25, T26, T27, T28, T29>(
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
            T29 item29
            )
        {
            return
                new Tuple
                    <T1, T2, T3, T4, T5, T6, T7,
                        Tuple
                            <T8, T9, T10, T11, T12, T13, T14,
                                Tuple
                                    <T15, T16, T17, T18, T19, T20, T21,
                                        Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29>>>>>(
                    item1,
                    item2,
                    item3,
                    item4,
                    item5,
                    item6,
                    item7,
                    new Tuple
                        <T8, T9, T10, T11, T12, T13, T14,
                        Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29>>>>(
                        item8,
                        item9,
                        item10,
                        item11,
                        item12,
                        item13,
                        item14,
                        new Tuple
                            <T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29>>>(
                            item15,
                            item16,
                            item17,
                            item18,
                            item19,
                            item20,
                            item21,
                            new Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29>>(
                                item22,
                                item23,
                                item24,
                                item25,
                                item26,
                                item27,
                                item28,
                                new Tuple<T29>(item29)))));
        }

        /// <summary>
        /// Used to create an enumeration of tuples from an enumeration of objects, by specifying how each item is extracted.
        /// </summary>
        /// <typeparam name="TInput">The input type.</typeparam>
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
        /// <param name="inputEnumeration">The enumeration of inputs.</param>
        /// <param name="func1">Function that returns item 1 of the tuple.</param>
        /// <param name="func2">Function that returns item 2 of the tuple.</param>
        /// <param name="func3">Function that returns item 3 of the tuple.</param>
        /// <param name="func4">Function that returns item 4 of the tuple.</param>
        /// <param name="func5">Function that returns item 5 of the tuple.</param>
        /// <param name="func6">Function that returns item 6 of the tuple.</param>
        /// <param name="func7">Function that returns item 7 of the tuple.</param>
        /// <param name="func8">Function that returns item 8 of the tuple.</param>
        /// <param name="func9">Function that returns item 9 of the tuple.</param>
        /// <param name="func10">Function that returns item 10 of the tuple.</param>
        /// <param name="func11">Function that returns item 11 of the tuple.</param>
        /// <param name="func12">Function that returns item 12 of the tuple.</param>
        /// <param name="func13">Function that returns item 13 of the tuple.</param>
        /// <param name="func14">Function that returns item 14 of the tuple.</param>
        /// <param name="func15">Function that returns item 15 of the tuple.</param>
        /// <param name="func16">Function that returns item 16 of the tuple.</param>
        /// <param name="func17">Function that returns item 17 of the tuple.</param>
        /// <param name="func18">Function that returns item 18 of the tuple.</param>
        /// <param name="func19">Function that returns item 19 of the tuple.</param>
        /// <param name="func20">Function that returns item 20 of the tuple.</param>
        /// <param name="func21">Function that returns item 21 of the tuple.</param>
        /// <param name="func22">Function that returns item 22 of the tuple.</param>
        /// <param name="func23">Function that returns item 23 of the tuple.</param>
        /// <param name="func24">Function that returns item 24 of the tuple.</param>
        /// <param name="func25">Function that returns item 25 of the tuple.</param>
        /// <param name="func26">Function that returns item 26 of the tuple.</param>
        /// <param name="func27">Function that returns item 27 of the tuple.</param>
        /// <param name="func28">Function that returns item 28 of the tuple.</param>
        /// <param name="func29">Function that returns item 29 of the tuple.</param>
        /// <returns>A tuple with 29 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static
        IEnumerable
            <
                Tuple
                    <T1, T2, T3, T4, T5, T6, T7,
                        Tuple
                            <T8, T9, T10, T11, T12, T13, T14,
                                Tuple
                                    <T15, T16, T17, T18, T19, T20, T21,
                                        Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29>>>>>> ToTuple
            <TInput, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22,
                T23, T24, T25, T26, T27, T28, T29>(
            [NotNull] this IEnumerable<TInput> inputEnumeration,
            [NotNull] Func<TInput, T1> func1,
            [NotNull] Func<TInput, T2> func2,
            [NotNull] Func<TInput, T3> func3,
            [NotNull] Func<TInput, T4> func4,
            [NotNull] Func<TInput, T5> func5,
            [NotNull] Func<TInput, T6> func6,
            [NotNull] Func<TInput, T7> func7,
            [NotNull] Func<TInput, T8> func8,
            [NotNull] Func<TInput, T9> func9,
            [NotNull] Func<TInput, T10> func10,
            [NotNull] Func<TInput, T11> func11,
            [NotNull] Func<TInput, T12> func12,
            [NotNull] Func<TInput, T13> func13,
            [NotNull] Func<TInput, T14> func14,
            [NotNull] Func<TInput, T15> func15,
            [NotNull] Func<TInput, T16> func16,
            [NotNull] Func<TInput, T17> func17,
            [NotNull] Func<TInput, T18> func18,
            [NotNull] Func<TInput, T19> func19,
            [NotNull] Func<TInput, T20> func20,
            [NotNull] Func<TInput, T21> func21,
            [NotNull] Func<TInput, T22> func22,
            [NotNull] Func<TInput, T23> func23,
            [NotNull] Func<TInput, T24> func24,
            [NotNull] Func<TInput, T25> func25,
            [NotNull] Func<TInput, T26> func26,
            [NotNull] Func<TInput, T27> func27,
            [NotNull] Func<TInput, T28> func28,
            [NotNull] Func<TInput, T29> func29
            )
        {
            return
                inputEnumeration.Select(
                    input =>
                        new Tuple
                        <T1, T2, T3, T4, T5, T6, T7,
                        Tuple
                        <T8, T9, T10, T11, T12, T13, T14,
                        Tuple<T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29>>>>>
                        (
                        func1(input),
                        func2(input),
                        func3(input),
                        func4(input),
                        func5(input),
                        func6(input),
                        func7(input),
                        new Tuple
                            <T8, T9, T10, T11, T12, T13, T14,
                            Tuple
                            <T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29>>>>(
                            func8(input),
                            func9(input),
                            func10(input),
                            func11(input),
                            func12(input),
                            func13(input),
                            func14(input),
                            new Tuple
                                <T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29>>
                                >(
                                func15(input),
                                func16(input),
                                func17(input),
                                func18(input),
                                func19(input),
                                func20(input),
                                func21(input),
                                new Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29>>(
                                    func22(input),
                                    func23(input),
                                    func24(input),
                                    func25(input),
                                    func26(input),
                                    func27(input),
                                    func28(input),
                                    new Tuple<T29>(func29(input)))))));
        }
        #endregion

        #region 30 items.
        /// <summary>
        /// Used to create a tuple in nested format.
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
        /// <param name="item1">Item 1 of the tuple.</param>
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
        /// <returns>A tuple with 30 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static
        Tuple
            <T1, T2, T3, T4, T5, T6, T7,
                Tuple
                    <T8, T9, T10, T11, T12, T13, T14,
                        Tuple
                            <T15, T16, T17, T18, T19, T20, T21,
                                Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30>>>>> Create
            <T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23,
                T24, T25, T26, T27, T28, T29, T30>(
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
            T30 item30
            )
        {
            return
                new Tuple
                    <T1, T2, T3, T4, T5, T6, T7,
                        Tuple
                            <T8, T9, T10, T11, T12, T13, T14,
                                Tuple
                                    <T15, T16, T17, T18, T19, T20, T21,
                                        Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30>>>>>(
                    item1,
                    item2,
                    item3,
                    item4,
                    item5,
                    item6,
                    item7,
                    new Tuple
                        <T8, T9, T10, T11, T12, T13, T14,
                        Tuple
                        <T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30>>>>(
                        item8,
                        item9,
                        item10,
                        item11,
                        item12,
                        item13,
                        item14,
                        new Tuple
                            <T15, T16, T17, T18, T19, T20, T21,
                            Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30>>>(
                            item15,
                            item16,
                            item17,
                            item18,
                            item19,
                            item20,
                            item21,
                            new Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30>>(
                                item22,
                                item23,
                                item24,
                                item25,
                                item26,
                                item27,
                                item28,
                                new Tuple<T29, T30>(item29, item30)))));
        }

        /// <summary>
        /// Used to create an enumeration of tuples from an enumeration of objects, by specifying how each item is extracted.
        /// </summary>
        /// <typeparam name="TInput">The input type.</typeparam>
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
        /// <param name="inputEnumeration">The enumeration of inputs.</param>
        /// <param name="func1">Function that returns item 1 of the tuple.</param>
        /// <param name="func2">Function that returns item 2 of the tuple.</param>
        /// <param name="func3">Function that returns item 3 of the tuple.</param>
        /// <param name="func4">Function that returns item 4 of the tuple.</param>
        /// <param name="func5">Function that returns item 5 of the tuple.</param>
        /// <param name="func6">Function that returns item 6 of the tuple.</param>
        /// <param name="func7">Function that returns item 7 of the tuple.</param>
        /// <param name="func8">Function that returns item 8 of the tuple.</param>
        /// <param name="func9">Function that returns item 9 of the tuple.</param>
        /// <param name="func10">Function that returns item 10 of the tuple.</param>
        /// <param name="func11">Function that returns item 11 of the tuple.</param>
        /// <param name="func12">Function that returns item 12 of the tuple.</param>
        /// <param name="func13">Function that returns item 13 of the tuple.</param>
        /// <param name="func14">Function that returns item 14 of the tuple.</param>
        /// <param name="func15">Function that returns item 15 of the tuple.</param>
        /// <param name="func16">Function that returns item 16 of the tuple.</param>
        /// <param name="func17">Function that returns item 17 of the tuple.</param>
        /// <param name="func18">Function that returns item 18 of the tuple.</param>
        /// <param name="func19">Function that returns item 19 of the tuple.</param>
        /// <param name="func20">Function that returns item 20 of the tuple.</param>
        /// <param name="func21">Function that returns item 21 of the tuple.</param>
        /// <param name="func22">Function that returns item 22 of the tuple.</param>
        /// <param name="func23">Function that returns item 23 of the tuple.</param>
        /// <param name="func24">Function that returns item 24 of the tuple.</param>
        /// <param name="func25">Function that returns item 25 of the tuple.</param>
        /// <param name="func26">Function that returns item 26 of the tuple.</param>
        /// <param name="func27">Function that returns item 27 of the tuple.</param>
        /// <param name="func28">Function that returns item 28 of the tuple.</param>
        /// <param name="func29">Function that returns item 29 of the tuple.</param>
        /// <param name="func30">Function that returns item 30 of the tuple.</param>
        /// <returns>A tuple with 30 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static
        IEnumerable
            <
                Tuple
                    <T1, T2, T3, T4, T5, T6, T7,
                        Tuple
                            <T8, T9, T10, T11, T12, T13, T14,
                                Tuple
                                    <T15, T16, T17, T18, T19, T20, T21,
                                        Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30>>>>>> ToTuple
            <TInput, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22,
                T23, T24, T25, T26, T27, T28, T29, T30>(
            [NotNull] this IEnumerable<TInput> inputEnumeration,
            [NotNull] Func<TInput, T1> func1,
            [NotNull] Func<TInput, T2> func2,
            [NotNull] Func<TInput, T3> func3,
            [NotNull] Func<TInput, T4> func4,
            [NotNull] Func<TInput, T5> func5,
            [NotNull] Func<TInput, T6> func6,
            [NotNull] Func<TInput, T7> func7,
            [NotNull] Func<TInput, T8> func8,
            [NotNull] Func<TInput, T9> func9,
            [NotNull] Func<TInput, T10> func10,
            [NotNull] Func<TInput, T11> func11,
            [NotNull] Func<TInput, T12> func12,
            [NotNull] Func<TInput, T13> func13,
            [NotNull] Func<TInput, T14> func14,
            [NotNull] Func<TInput, T15> func15,
            [NotNull] Func<TInput, T16> func16,
            [NotNull] Func<TInput, T17> func17,
            [NotNull] Func<TInput, T18> func18,
            [NotNull] Func<TInput, T19> func19,
            [NotNull] Func<TInput, T20> func20,
            [NotNull] Func<TInput, T21> func21,
            [NotNull] Func<TInput, T22> func22,
            [NotNull] Func<TInput, T23> func23,
            [NotNull] Func<TInput, T24> func24,
            [NotNull] Func<TInput, T25> func25,
            [NotNull] Func<TInput, T26> func26,
            [NotNull] Func<TInput, T27> func27,
            [NotNull] Func<TInput, T28> func28,
            [NotNull] Func<TInput, T29> func29,
            [NotNull] Func<TInput, T30> func30
            )
        {
            return
                inputEnumeration.Select(
                    input =>
                        new Tuple
                        <T1, T2, T3, T4, T5, T6, T7,
                        Tuple
                        <T8, T9, T10, T11, T12, T13, T14,
                        Tuple
                        <T15, T16, T17, T18, T19, T20, T21, Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30>>>>>
                        (
                        func1(input),
                        func2(input),
                        func3(input),
                        func4(input),
                        func5(input),
                        func6(input),
                        func7(input),
                        new Tuple
                            <T8, T9, T10, T11, T12, T13, T14,
                            Tuple
                            <T15, T16, T17, T18, T19, T20, T21,
                            Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30>>>>(
                            func8(input),
                            func9(input),
                            func10(input),
                            func11(input),
                            func12(input),
                            func13(input),
                            func14(input),
                            new Tuple
                                <T15, T16, T17, T18, T19, T20, T21,
                                Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30>>>(
                                func15(input),
                                func16(input),
                                func17(input),
                                func18(input),
                                func19(input),
                                func20(input),
                                func21(input),
                                new Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30>>(
                                    func22(input),
                                    func23(input),
                                    func24(input),
                                    func25(input),
                                    func26(input),
                                    func27(input),
                                    func28(input),
                                    new Tuple<T29, T30>(func29(input), func30(input)))))));
        }
        #endregion

        #region 31 items.
        /// <summary>
        /// Used to create a tuple in nested format.
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
        /// <param name="item1">Item 1 of the tuple.</param>
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
        /// <returns>A tuple with 31 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static
        Tuple
            <T1, T2, T3, T4, T5, T6, T7,
                Tuple
                    <T8, T9, T10, T11, T12, T13, T14,
                        Tuple
                            <T15, T16, T17, T18, T19, T20, T21,
                                Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31>>>>> Create
            <T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23,
                T24, T25, T26, T27, T28, T29, T30, T31>(
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
            T31 item31
            )
        {
            return
                new Tuple
                    <T1, T2, T3, T4, T5, T6, T7,
                        Tuple
                            <T8, T9, T10, T11, T12, T13, T14,
                                Tuple
                                    <T15, T16, T17, T18, T19, T20, T21,
                                        Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31>>>>>(
                    item1,
                    item2,
                    item3,
                    item4,
                    item5,
                    item6,
                    item7,
                    new Tuple
                        <T8, T9, T10, T11, T12, T13, T14,
                        Tuple
                        <T15, T16, T17, T18, T19, T20, T21,
                        Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31>>>>(
                        item8,
                        item9,
                        item10,
                        item11,
                        item12,
                        item13,
                        item14,
                        new Tuple
                            <T15, T16, T17, T18, T19, T20, T21,
                            Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31>>>(
                            item15,
                            item16,
                            item17,
                            item18,
                            item19,
                            item20,
                            item21,
                            new Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31>>(
                                item22,
                                item23,
                                item24,
                                item25,
                                item26,
                                item27,
                                item28,
                                new Tuple<T29, T30, T31>(item29, item30, item31)))));
        }

        /// <summary>
        /// Used to create an enumeration of tuples from an enumeration of objects, by specifying how each item is extracted.
        /// </summary>
        /// <typeparam name="TInput">The input type.</typeparam>
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
        /// <param name="inputEnumeration">The enumeration of inputs.</param>
        /// <param name="func1">Function that returns item 1 of the tuple.</param>
        /// <param name="func2">Function that returns item 2 of the tuple.</param>
        /// <param name="func3">Function that returns item 3 of the tuple.</param>
        /// <param name="func4">Function that returns item 4 of the tuple.</param>
        /// <param name="func5">Function that returns item 5 of the tuple.</param>
        /// <param name="func6">Function that returns item 6 of the tuple.</param>
        /// <param name="func7">Function that returns item 7 of the tuple.</param>
        /// <param name="func8">Function that returns item 8 of the tuple.</param>
        /// <param name="func9">Function that returns item 9 of the tuple.</param>
        /// <param name="func10">Function that returns item 10 of the tuple.</param>
        /// <param name="func11">Function that returns item 11 of the tuple.</param>
        /// <param name="func12">Function that returns item 12 of the tuple.</param>
        /// <param name="func13">Function that returns item 13 of the tuple.</param>
        /// <param name="func14">Function that returns item 14 of the tuple.</param>
        /// <param name="func15">Function that returns item 15 of the tuple.</param>
        /// <param name="func16">Function that returns item 16 of the tuple.</param>
        /// <param name="func17">Function that returns item 17 of the tuple.</param>
        /// <param name="func18">Function that returns item 18 of the tuple.</param>
        /// <param name="func19">Function that returns item 19 of the tuple.</param>
        /// <param name="func20">Function that returns item 20 of the tuple.</param>
        /// <param name="func21">Function that returns item 21 of the tuple.</param>
        /// <param name="func22">Function that returns item 22 of the tuple.</param>
        /// <param name="func23">Function that returns item 23 of the tuple.</param>
        /// <param name="func24">Function that returns item 24 of the tuple.</param>
        /// <param name="func25">Function that returns item 25 of the tuple.</param>
        /// <param name="func26">Function that returns item 26 of the tuple.</param>
        /// <param name="func27">Function that returns item 27 of the tuple.</param>
        /// <param name="func28">Function that returns item 28 of the tuple.</param>
        /// <param name="func29">Function that returns item 29 of the tuple.</param>
        /// <param name="func30">Function that returns item 30 of the tuple.</param>
        /// <param name="func31">Function that returns item 31 of the tuple.</param>
        /// <returns>A tuple with 31 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static
        IEnumerable
            <
                Tuple
                    <T1, T2, T3, T4, T5, T6, T7,
                        Tuple
                            <T8, T9, T10, T11, T12, T13, T14,
                                Tuple
                                    <T15, T16, T17, T18, T19, T20, T21,
                                        Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31>>>>>> ToTuple
            <TInput, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22,
                T23, T24, T25, T26, T27, T28, T29, T30, T31>(
            [NotNull] this IEnumerable<TInput> inputEnumeration,
            [NotNull] Func<TInput, T1> func1,
            [NotNull] Func<TInput, T2> func2,
            [NotNull] Func<TInput, T3> func3,
            [NotNull] Func<TInput, T4> func4,
            [NotNull] Func<TInput, T5> func5,
            [NotNull] Func<TInput, T6> func6,
            [NotNull] Func<TInput, T7> func7,
            [NotNull] Func<TInput, T8> func8,
            [NotNull] Func<TInput, T9> func9,
            [NotNull] Func<TInput, T10> func10,
            [NotNull] Func<TInput, T11> func11,
            [NotNull] Func<TInput, T12> func12,
            [NotNull] Func<TInput, T13> func13,
            [NotNull] Func<TInput, T14> func14,
            [NotNull] Func<TInput, T15> func15,
            [NotNull] Func<TInput, T16> func16,
            [NotNull] Func<TInput, T17> func17,
            [NotNull] Func<TInput, T18> func18,
            [NotNull] Func<TInput, T19> func19,
            [NotNull] Func<TInput, T20> func20,
            [NotNull] Func<TInput, T21> func21,
            [NotNull] Func<TInput, T22> func22,
            [NotNull] Func<TInput, T23> func23,
            [NotNull] Func<TInput, T24> func24,
            [NotNull] Func<TInput, T25> func25,
            [NotNull] Func<TInput, T26> func26,
            [NotNull] Func<TInput, T27> func27,
            [NotNull] Func<TInput, T28> func28,
            [NotNull] Func<TInput, T29> func29,
            [NotNull] Func<TInput, T30> func30,
            [NotNull] Func<TInput, T31> func31
            )
        {
            return
                inputEnumeration.Select(
                    input =>
                        new Tuple
                        <T1, T2, T3, T4, T5, T6, T7,
                        Tuple
                        <T8, T9, T10, T11, T12, T13, T14,
                        Tuple
                        <T15, T16, T17, T18, T19, T20, T21,
                        Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31>>>>>(
                        func1(input),
                        func2(input),
                        func3(input),
                        func4(input),
                        func5(input),
                        func6(input),
                        func7(input),
                        new Tuple
                            <T8, T9, T10, T11, T12, T13, T14,
                            Tuple
                            <T15, T16, T17, T18, T19, T20, T21,
                            Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31>>>>(
                            func8(input),
                            func9(input),
                            func10(input),
                            func11(input),
                            func12(input),
                            func13(input),
                            func14(input),
                            new Tuple
                                <T15, T16, T17, T18, T19, T20, T21,
                                Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31>>>(
                                func15(input),
                                func16(input),
                                func17(input),
                                func18(input),
                                func19(input),
                                func20(input),
                                func21(input),
                                new Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31>>(
                                    func22(input),
                                    func23(input),
                                    func24(input),
                                    func25(input),
                                    func26(input),
                                    func27(input),
                                    func28(input),
                                    new Tuple<T29, T30, T31>(func29(input), func30(input), func31(input)))))));
        }
        #endregion

        #region 32 items.
        /// <summary>
        /// Used to create a tuple in nested format.
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
        /// <param name="item1">Item 1 of the tuple.</param>
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
        /// <returns>A tuple with 32 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static
        Tuple
            <T1, T2, T3, T4, T5, T6, T7,
                Tuple
                    <T8, T9, T10, T11, T12, T13, T14,
                        Tuple
                            <T15, T16, T17, T18, T19, T20, T21,
                                Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32>>>>> Create
            <T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23,
                T24, T25, T26, T27, T28, T29, T30, T31, T32>(
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
            T32 item32
            )
        {
            return
                new Tuple
                    <T1, T2, T3, T4, T5, T6, T7,
                        Tuple
                            <T8, T9, T10, T11, T12, T13, T14,
                                Tuple
                                    <T15, T16, T17, T18, T19, T20, T21,
                                        Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32>>>>>(
                    item1,
                    item2,
                    item3,
                    item4,
                    item5,
                    item6,
                    item7,
                    new Tuple
                        <T8, T9, T10, T11, T12, T13, T14,
                        Tuple
                        <T15, T16, T17, T18, T19, T20, T21,
                        Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32>>>>(
                        item8,
                        item9,
                        item10,
                        item11,
                        item12,
                        item13,
                        item14,
                        new Tuple
                            <T15, T16, T17, T18, T19, T20, T21,
                            Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32>>>(
                            item15,
                            item16,
                            item17,
                            item18,
                            item19,
                            item20,
                            item21,
                            new Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32>>(
                                item22,
                                item23,
                                item24,
                                item25,
                                item26,
                                item27,
                                item28,
                                new Tuple<T29, T30, T31, T32>(item29, item30, item31, item32)))));
        }

        /// <summary>
        /// Used to create an enumeration of tuples from an enumeration of objects, by specifying how each item is extracted.
        /// </summary>
        /// <typeparam name="TInput">The input type.</typeparam>
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
        /// <param name="inputEnumeration">The enumeration of inputs.</param>
        /// <param name="func1">Function that returns item 1 of the tuple.</param>
        /// <param name="func2">Function that returns item 2 of the tuple.</param>
        /// <param name="func3">Function that returns item 3 of the tuple.</param>
        /// <param name="func4">Function that returns item 4 of the tuple.</param>
        /// <param name="func5">Function that returns item 5 of the tuple.</param>
        /// <param name="func6">Function that returns item 6 of the tuple.</param>
        /// <param name="func7">Function that returns item 7 of the tuple.</param>
        /// <param name="func8">Function that returns item 8 of the tuple.</param>
        /// <param name="func9">Function that returns item 9 of the tuple.</param>
        /// <param name="func10">Function that returns item 10 of the tuple.</param>
        /// <param name="func11">Function that returns item 11 of the tuple.</param>
        /// <param name="func12">Function that returns item 12 of the tuple.</param>
        /// <param name="func13">Function that returns item 13 of the tuple.</param>
        /// <param name="func14">Function that returns item 14 of the tuple.</param>
        /// <param name="func15">Function that returns item 15 of the tuple.</param>
        /// <param name="func16">Function that returns item 16 of the tuple.</param>
        /// <param name="func17">Function that returns item 17 of the tuple.</param>
        /// <param name="func18">Function that returns item 18 of the tuple.</param>
        /// <param name="func19">Function that returns item 19 of the tuple.</param>
        /// <param name="func20">Function that returns item 20 of the tuple.</param>
        /// <param name="func21">Function that returns item 21 of the tuple.</param>
        /// <param name="func22">Function that returns item 22 of the tuple.</param>
        /// <param name="func23">Function that returns item 23 of the tuple.</param>
        /// <param name="func24">Function that returns item 24 of the tuple.</param>
        /// <param name="func25">Function that returns item 25 of the tuple.</param>
        /// <param name="func26">Function that returns item 26 of the tuple.</param>
        /// <param name="func27">Function that returns item 27 of the tuple.</param>
        /// <param name="func28">Function that returns item 28 of the tuple.</param>
        /// <param name="func29">Function that returns item 29 of the tuple.</param>
        /// <param name="func30">Function that returns item 30 of the tuple.</param>
        /// <param name="func31">Function that returns item 31 of the tuple.</param>
        /// <param name="func32">Function that returns item 32 of the tuple.</param>
        /// <returns>A tuple with 32 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static
        IEnumerable
            <
                Tuple
                    <T1, T2, T3, T4, T5, T6, T7,
                        Tuple
                            <T8, T9, T10, T11, T12, T13, T14,
                                Tuple
                                    <T15, T16, T17, T18, T19, T20, T21,
                                        Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32>>>>>> ToTuple
            <TInput, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22,
                T23, T24, T25, T26, T27, T28, T29, T30, T31, T32>(
            [NotNull] this IEnumerable<TInput> inputEnumeration,
            [NotNull] Func<TInput, T1> func1,
            [NotNull] Func<TInput, T2> func2,
            [NotNull] Func<TInput, T3> func3,
            [NotNull] Func<TInput, T4> func4,
            [NotNull] Func<TInput, T5> func5,
            [NotNull] Func<TInput, T6> func6,
            [NotNull] Func<TInput, T7> func7,
            [NotNull] Func<TInput, T8> func8,
            [NotNull] Func<TInput, T9> func9,
            [NotNull] Func<TInput, T10> func10,
            [NotNull] Func<TInput, T11> func11,
            [NotNull] Func<TInput, T12> func12,
            [NotNull] Func<TInput, T13> func13,
            [NotNull] Func<TInput, T14> func14,
            [NotNull] Func<TInput, T15> func15,
            [NotNull] Func<TInput, T16> func16,
            [NotNull] Func<TInput, T17> func17,
            [NotNull] Func<TInput, T18> func18,
            [NotNull] Func<TInput, T19> func19,
            [NotNull] Func<TInput, T20> func20,
            [NotNull] Func<TInput, T21> func21,
            [NotNull] Func<TInput, T22> func22,
            [NotNull] Func<TInput, T23> func23,
            [NotNull] Func<TInput, T24> func24,
            [NotNull] Func<TInput, T25> func25,
            [NotNull] Func<TInput, T26> func26,
            [NotNull] Func<TInput, T27> func27,
            [NotNull] Func<TInput, T28> func28,
            [NotNull] Func<TInput, T29> func29,
            [NotNull] Func<TInput, T30> func30,
            [NotNull] Func<TInput, T31> func31,
            [NotNull] Func<TInput, T32> func32
            )
        {
            return
                inputEnumeration.Select(
                    input =>
                        new Tuple
                        <T1, T2, T3, T4, T5, T6, T7,
                        Tuple
                        <T8, T9, T10, T11, T12, T13, T14,
                        Tuple
                        <T15, T16, T17, T18, T19, T20, T21,
                        Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32>>>>>(
                        func1(input),
                        func2(input),
                        func3(input),
                        func4(input),
                        func5(input),
                        func6(input),
                        func7(input),
                        new Tuple
                            <T8, T9, T10, T11, T12, T13, T14,
                            Tuple
                            <T15, T16, T17, T18, T19, T20, T21,
                            Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32>>>>(
                            func8(input),
                            func9(input),
                            func10(input),
                            func11(input),
                            func12(input),
                            func13(input),
                            func14(input),
                            new Tuple
                                <T15, T16, T17, T18, T19, T20, T21,
                                Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32>>>(
                                func15(input),
                                func16(input),
                                func17(input),
                                func18(input),
                                func19(input),
                                func20(input),
                                func21(input),
                                new Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32>>(
                                    func22(input),
                                    func23(input),
                                    func24(input),
                                    func25(input),
                                    func26(input),
                                    func27(input),
                                    func28(input),
                                    new Tuple<T29, T30, T31, T32>(
                                        func29(input),
                                        func30(input),
                                        func31(input),
                                        func32(input)))))));
        }
        #endregion

        #region 33 items.
        /// <summary>
        /// Used to create a tuple in nested format.
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
        /// <param name="item1">Item 1 of the tuple.</param>
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
        /// <returns>A tuple with 33 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static
        Tuple
            <T1, T2, T3, T4, T5, T6, T7,
                Tuple
                    <T8, T9, T10, T11, T12, T13, T14,
                        Tuple
                            <T15, T16, T17, T18, T19, T20, T21,
                                Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33>>>>> Create
            <T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23,
                T24, T25, T26, T27, T28, T29, T30, T31, T32, T33>(
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
            T33 item33
            )
        {
            return
                new Tuple
                    <T1, T2, T3, T4, T5, T6, T7,
                        Tuple
                            <T8, T9, T10, T11, T12, T13, T14,
                                Tuple
                                    <T15, T16, T17, T18, T19, T20, T21,
                                        Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33>>>>>(
                    item1,
                    item2,
                    item3,
                    item4,
                    item5,
                    item6,
                    item7,
                    new Tuple
                        <T8, T9, T10, T11, T12, T13, T14,
                        Tuple
                        <T15, T16, T17, T18, T19, T20, T21,
                        Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33>>>>(
                        item8,
                        item9,
                        item10,
                        item11,
                        item12,
                        item13,
                        item14,
                        new Tuple
                            <T15, T16, T17, T18, T19, T20, T21,
                            Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33>>>(
                            item15,
                            item16,
                            item17,
                            item18,
                            item19,
                            item20,
                            item21,
                            new Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33>>(
                                item22,
                                item23,
                                item24,
                                item25,
                                item26,
                                item27,
                                item28,
                                new Tuple<T29, T30, T31, T32, T33>(item29, item30, item31, item32, item33)))));
        }

        /// <summary>
        /// Used to create an enumeration of tuples from an enumeration of objects, by specifying how each item is extracted.
        /// </summary>
        /// <typeparam name="TInput">The input type.</typeparam>
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
        /// <param name="inputEnumeration">The enumeration of inputs.</param>
        /// <param name="func1">Function that returns item 1 of the tuple.</param>
        /// <param name="func2">Function that returns item 2 of the tuple.</param>
        /// <param name="func3">Function that returns item 3 of the tuple.</param>
        /// <param name="func4">Function that returns item 4 of the tuple.</param>
        /// <param name="func5">Function that returns item 5 of the tuple.</param>
        /// <param name="func6">Function that returns item 6 of the tuple.</param>
        /// <param name="func7">Function that returns item 7 of the tuple.</param>
        /// <param name="func8">Function that returns item 8 of the tuple.</param>
        /// <param name="func9">Function that returns item 9 of the tuple.</param>
        /// <param name="func10">Function that returns item 10 of the tuple.</param>
        /// <param name="func11">Function that returns item 11 of the tuple.</param>
        /// <param name="func12">Function that returns item 12 of the tuple.</param>
        /// <param name="func13">Function that returns item 13 of the tuple.</param>
        /// <param name="func14">Function that returns item 14 of the tuple.</param>
        /// <param name="func15">Function that returns item 15 of the tuple.</param>
        /// <param name="func16">Function that returns item 16 of the tuple.</param>
        /// <param name="func17">Function that returns item 17 of the tuple.</param>
        /// <param name="func18">Function that returns item 18 of the tuple.</param>
        /// <param name="func19">Function that returns item 19 of the tuple.</param>
        /// <param name="func20">Function that returns item 20 of the tuple.</param>
        /// <param name="func21">Function that returns item 21 of the tuple.</param>
        /// <param name="func22">Function that returns item 22 of the tuple.</param>
        /// <param name="func23">Function that returns item 23 of the tuple.</param>
        /// <param name="func24">Function that returns item 24 of the tuple.</param>
        /// <param name="func25">Function that returns item 25 of the tuple.</param>
        /// <param name="func26">Function that returns item 26 of the tuple.</param>
        /// <param name="func27">Function that returns item 27 of the tuple.</param>
        /// <param name="func28">Function that returns item 28 of the tuple.</param>
        /// <param name="func29">Function that returns item 29 of the tuple.</param>
        /// <param name="func30">Function that returns item 30 of the tuple.</param>
        /// <param name="func31">Function that returns item 31 of the tuple.</param>
        /// <param name="func32">Function that returns item 32 of the tuple.</param>
        /// <param name="func33">Function that returns item 33 of the tuple.</param>
        /// <returns>A tuple with 33 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static
        IEnumerable
            <
                Tuple
                    <T1, T2, T3, T4, T5, T6, T7,
                        Tuple
                            <T8, T9, T10, T11, T12, T13, T14,
                                Tuple
                                    <T15, T16, T17, T18, T19, T20, T21,
                                        Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33>>>>>>
            ToTuple
            <TInput, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22,
                T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33>(
            [NotNull] this IEnumerable<TInput> inputEnumeration,
            [NotNull] Func<TInput, T1> func1,
            [NotNull] Func<TInput, T2> func2,
            [NotNull] Func<TInput, T3> func3,
            [NotNull] Func<TInput, T4> func4,
            [NotNull] Func<TInput, T5> func5,
            [NotNull] Func<TInput, T6> func6,
            [NotNull] Func<TInput, T7> func7,
            [NotNull] Func<TInput, T8> func8,
            [NotNull] Func<TInput, T9> func9,
            [NotNull] Func<TInput, T10> func10,
            [NotNull] Func<TInput, T11> func11,
            [NotNull] Func<TInput, T12> func12,
            [NotNull] Func<TInput, T13> func13,
            [NotNull] Func<TInput, T14> func14,
            [NotNull] Func<TInput, T15> func15,
            [NotNull] Func<TInput, T16> func16,
            [NotNull] Func<TInput, T17> func17,
            [NotNull] Func<TInput, T18> func18,
            [NotNull] Func<TInput, T19> func19,
            [NotNull] Func<TInput, T20> func20,
            [NotNull] Func<TInput, T21> func21,
            [NotNull] Func<TInput, T22> func22,
            [NotNull] Func<TInput, T23> func23,
            [NotNull] Func<TInput, T24> func24,
            [NotNull] Func<TInput, T25> func25,
            [NotNull] Func<TInput, T26> func26,
            [NotNull] Func<TInput, T27> func27,
            [NotNull] Func<TInput, T28> func28,
            [NotNull] Func<TInput, T29> func29,
            [NotNull] Func<TInput, T30> func30,
            [NotNull] Func<TInput, T31> func31,
            [NotNull] Func<TInput, T32> func32,
            [NotNull] Func<TInput, T33> func33
            )
        {
            return
                inputEnumeration.Select(
                    input =>
                        new Tuple
                        <T1, T2, T3, T4, T5, T6, T7,
                        Tuple
                        <T8, T9, T10, T11, T12, T13, T14,
                        Tuple
                        <T15, T16, T17, T18, T19, T20, T21,
                        Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33>>>>>(
                        func1(input),
                        func2(input),
                        func3(input),
                        func4(input),
                        func5(input),
                        func6(input),
                        func7(input),
                        new Tuple
                            <T8, T9, T10, T11, T12, T13, T14,
                            Tuple
                            <T15, T16, T17, T18, T19, T20, T21,
                            Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33>>>>(
                            func8(input),
                            func9(input),
                            func10(input),
                            func11(input),
                            func12(input),
                            func13(input),
                            func14(input),
                            new Tuple
                                <T15, T16, T17, T18, T19, T20, T21,
                                Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33>>>(
                                func15(input),
                                func16(input),
                                func17(input),
                                func18(input),
                                func19(input),
                                func20(input),
                                func21(input),
                                new Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33>>(
                                    func22(input),
                                    func23(input),
                                    func24(input),
                                    func25(input),
                                    func26(input),
                                    func27(input),
                                    func28(input),
                                    new Tuple<T29, T30, T31, T32, T33>(
                                        func29(input),
                                        func30(input),
                                        func31(input),
                                        func32(input),
                                        func33(input)))))));
        }
        #endregion

        #region 34 items.
        /// <summary>
        /// Used to create a tuple in nested format.
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
        /// <param name="item1">Item 1 of the tuple.</param>
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
        /// <returns>A tuple with 34 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static
        Tuple
            <T1, T2, T3, T4, T5, T6, T7,
                Tuple
                    <T8, T9, T10, T11, T12, T13, T14,
                        Tuple
                            <T15, T16, T17, T18, T19, T20, T21,
                                Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34>>>>> Create
            <T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23,
                T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34>(
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
            T34 item34
            )
        {
            return
                new Tuple
                    <T1, T2, T3, T4, T5, T6, T7,
                        Tuple
                            <T8, T9, T10, T11, T12, T13, T14,
                                Tuple
                                    <T15, T16, T17, T18, T19, T20, T21,
                                        Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34>>>>>
                    (
                    item1,
                    item2,
                    item3,
                    item4,
                    item5,
                    item6,
                    item7,
                    new Tuple
                        <T8, T9, T10, T11, T12, T13, T14,
                        Tuple
                        <T15, T16, T17, T18, T19, T20, T21,
                        Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34>>>>(
                        item8,
                        item9,
                        item10,
                        item11,
                        item12,
                        item13,
                        item14,
                        new Tuple
                            <T15, T16, T17, T18, T19, T20, T21,
                            Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34>>>(
                            item15,
                            item16,
                            item17,
                            item18,
                            item19,
                            item20,
                            item21,
                            new Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34>>(
                                item22,
                                item23,
                                item24,
                                item25,
                                item26,
                                item27,
                                item28,
                                new Tuple<T29, T30, T31, T32, T33, T34>(item29, item30, item31, item32, item33, item34)))));
        }

        /// <summary>
        /// Used to create an enumeration of tuples from an enumeration of objects, by specifying how each item is extracted.
        /// </summary>
        /// <typeparam name="TInput">The input type.</typeparam>
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
        /// <param name="inputEnumeration">The enumeration of inputs.</param>
        /// <param name="func1">Function that returns item 1 of the tuple.</param>
        /// <param name="func2">Function that returns item 2 of the tuple.</param>
        /// <param name="func3">Function that returns item 3 of the tuple.</param>
        /// <param name="func4">Function that returns item 4 of the tuple.</param>
        /// <param name="func5">Function that returns item 5 of the tuple.</param>
        /// <param name="func6">Function that returns item 6 of the tuple.</param>
        /// <param name="func7">Function that returns item 7 of the tuple.</param>
        /// <param name="func8">Function that returns item 8 of the tuple.</param>
        /// <param name="func9">Function that returns item 9 of the tuple.</param>
        /// <param name="func10">Function that returns item 10 of the tuple.</param>
        /// <param name="func11">Function that returns item 11 of the tuple.</param>
        /// <param name="func12">Function that returns item 12 of the tuple.</param>
        /// <param name="func13">Function that returns item 13 of the tuple.</param>
        /// <param name="func14">Function that returns item 14 of the tuple.</param>
        /// <param name="func15">Function that returns item 15 of the tuple.</param>
        /// <param name="func16">Function that returns item 16 of the tuple.</param>
        /// <param name="func17">Function that returns item 17 of the tuple.</param>
        /// <param name="func18">Function that returns item 18 of the tuple.</param>
        /// <param name="func19">Function that returns item 19 of the tuple.</param>
        /// <param name="func20">Function that returns item 20 of the tuple.</param>
        /// <param name="func21">Function that returns item 21 of the tuple.</param>
        /// <param name="func22">Function that returns item 22 of the tuple.</param>
        /// <param name="func23">Function that returns item 23 of the tuple.</param>
        /// <param name="func24">Function that returns item 24 of the tuple.</param>
        /// <param name="func25">Function that returns item 25 of the tuple.</param>
        /// <param name="func26">Function that returns item 26 of the tuple.</param>
        /// <param name="func27">Function that returns item 27 of the tuple.</param>
        /// <param name="func28">Function that returns item 28 of the tuple.</param>
        /// <param name="func29">Function that returns item 29 of the tuple.</param>
        /// <param name="func30">Function that returns item 30 of the tuple.</param>
        /// <param name="func31">Function that returns item 31 of the tuple.</param>
        /// <param name="func32">Function that returns item 32 of the tuple.</param>
        /// <param name="func33">Function that returns item 33 of the tuple.</param>
        /// <param name="func34">Function that returns item 34 of the tuple.</param>
        /// <returns>A tuple with 34 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static
        IEnumerable
            <
                Tuple
                    <T1, T2, T3, T4, T5, T6, T7,
                        Tuple
                            <T8, T9, T10, T11, T12, T13, T14,
                                Tuple
                                    <T15, T16, T17, T18, T19, T20, T21,
                                        Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34>>>>>
                > ToTuple
            <TInput, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22,
                T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34>(
            [NotNull] this IEnumerable<TInput> inputEnumeration,
            [NotNull] Func<TInput, T1> func1,
            [NotNull] Func<TInput, T2> func2,
            [NotNull] Func<TInput, T3> func3,
            [NotNull] Func<TInput, T4> func4,
            [NotNull] Func<TInput, T5> func5,
            [NotNull] Func<TInput, T6> func6,
            [NotNull] Func<TInput, T7> func7,
            [NotNull] Func<TInput, T8> func8,
            [NotNull] Func<TInput, T9> func9,
            [NotNull] Func<TInput, T10> func10,
            [NotNull] Func<TInput, T11> func11,
            [NotNull] Func<TInput, T12> func12,
            [NotNull] Func<TInput, T13> func13,
            [NotNull] Func<TInput, T14> func14,
            [NotNull] Func<TInput, T15> func15,
            [NotNull] Func<TInput, T16> func16,
            [NotNull] Func<TInput, T17> func17,
            [NotNull] Func<TInput, T18> func18,
            [NotNull] Func<TInput, T19> func19,
            [NotNull] Func<TInput, T20> func20,
            [NotNull] Func<TInput, T21> func21,
            [NotNull] Func<TInput, T22> func22,
            [NotNull] Func<TInput, T23> func23,
            [NotNull] Func<TInput, T24> func24,
            [NotNull] Func<TInput, T25> func25,
            [NotNull] Func<TInput, T26> func26,
            [NotNull] Func<TInput, T27> func27,
            [NotNull] Func<TInput, T28> func28,
            [NotNull] Func<TInput, T29> func29,
            [NotNull] Func<TInput, T30> func30,
            [NotNull] Func<TInput, T31> func31,
            [NotNull] Func<TInput, T32> func32,
            [NotNull] Func<TInput, T33> func33,
            [NotNull] Func<TInput, T34> func34
            )
        {
            return
                inputEnumeration.Select(
                    input =>
                        new Tuple
                        <T1, T2, T3, T4, T5, T6, T7,
                        Tuple
                        <T8, T9, T10, T11, T12, T13, T14,
                        Tuple
                        <T15, T16, T17, T18, T19, T20, T21,
                        Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34>>>>>(
                        func1(input),
                        func2(input),
                        func3(input),
                        func4(input),
                        func5(input),
                        func6(input),
                        func7(input),
                        new Tuple
                            <T8, T9, T10, T11, T12, T13, T14,
                            Tuple
                            <T15, T16, T17, T18, T19, T20, T21,
                            Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34>>>>(
                            func8(input),
                            func9(input),
                            func10(input),
                            func11(input),
                            func12(input),
                            func13(input),
                            func14(input),
                            new Tuple
                                <T15, T16, T17, T18, T19, T20, T21,
                                Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34>>>(
                                func15(input),
                                func16(input),
                                func17(input),
                                func18(input),
                                func19(input),
                                func20(input),
                                func21(input),
                                new Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34>>(
                                    func22(input),
                                    func23(input),
                                    func24(input),
                                    func25(input),
                                    func26(input),
                                    func27(input),
                                    func28(input),
                                    new Tuple<T29, T30, T31, T32, T33, T34>(
                                        func29(input),
                                        func30(input),
                                        func31(input),
                                        func32(input),
                                        func33(input),
                                        func34(input)))))));
        }
        #endregion

        #region 35 items.
        /// <summary>
        /// Used to create a tuple in nested format.
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
        /// <param name="item1">Item 1 of the tuple.</param>
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
        /// <returns>A tuple with 35 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static
        Tuple
            <T1, T2, T3, T4, T5, T6, T7,
                Tuple
                    <T8, T9, T10, T11, T12, T13, T14,
                        Tuple
                            <T15, T16, T17, T18, T19, T20, T21,
                                Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34, T35>>>>>
            Create
            <T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23,
                T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35>(
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
            T35 item35
            )
        {
            return
                new Tuple
                    <T1, T2, T3, T4, T5, T6, T7,
                        Tuple
                            <T8, T9, T10, T11, T12, T13, T14,
                                Tuple
                                    <T15, T16, T17, T18, T19, T20, T21,
                                        Tuple
                                            <T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34, T35>
                                                >>>>(
                    item1,
                    item2,
                    item3,
                    item4,
                    item5,
                    item6,
                    item7,
                    new Tuple
                        <T8, T9, T10, T11, T12, T13, T14,
                        Tuple
                        <T15, T16, T17, T18, T19, T20, T21,
                        Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34, T35>>>>(
                        item8,
                        item9,
                        item10,
                        item11,
                        item12,
                        item13,
                        item14,
                        new Tuple
                            <T15, T16, T17, T18, T19, T20, T21,
                            Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34, T35>>>(
                            item15,
                            item16,
                            item17,
                            item18,
                            item19,
                            item20,
                            item21,
                            new Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34, T35>>(
                                item22,
                                item23,
                                item24,
                                item25,
                                item26,
                                item27,
                                item28,
                                new Tuple<T29, T30, T31, T32, T33, T34, T35>(
                                    item29,
                                    item30,
                                    item31,
                                    item32,
                                    item33,
                                    item34,
                                    item35)))));
        }

        /// <summary>
        /// Used to create an enumeration of tuples from an enumeration of objects, by specifying how each item is extracted.
        /// </summary>
        /// <typeparam name="TInput">The input type.</typeparam>
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
        /// <param name="inputEnumeration">The enumeration of inputs.</param>
        /// <param name="func1">Function that returns item 1 of the tuple.</param>
        /// <param name="func2">Function that returns item 2 of the tuple.</param>
        /// <param name="func3">Function that returns item 3 of the tuple.</param>
        /// <param name="func4">Function that returns item 4 of the tuple.</param>
        /// <param name="func5">Function that returns item 5 of the tuple.</param>
        /// <param name="func6">Function that returns item 6 of the tuple.</param>
        /// <param name="func7">Function that returns item 7 of the tuple.</param>
        /// <param name="func8">Function that returns item 8 of the tuple.</param>
        /// <param name="func9">Function that returns item 9 of the tuple.</param>
        /// <param name="func10">Function that returns item 10 of the tuple.</param>
        /// <param name="func11">Function that returns item 11 of the tuple.</param>
        /// <param name="func12">Function that returns item 12 of the tuple.</param>
        /// <param name="func13">Function that returns item 13 of the tuple.</param>
        /// <param name="func14">Function that returns item 14 of the tuple.</param>
        /// <param name="func15">Function that returns item 15 of the tuple.</param>
        /// <param name="func16">Function that returns item 16 of the tuple.</param>
        /// <param name="func17">Function that returns item 17 of the tuple.</param>
        /// <param name="func18">Function that returns item 18 of the tuple.</param>
        /// <param name="func19">Function that returns item 19 of the tuple.</param>
        /// <param name="func20">Function that returns item 20 of the tuple.</param>
        /// <param name="func21">Function that returns item 21 of the tuple.</param>
        /// <param name="func22">Function that returns item 22 of the tuple.</param>
        /// <param name="func23">Function that returns item 23 of the tuple.</param>
        /// <param name="func24">Function that returns item 24 of the tuple.</param>
        /// <param name="func25">Function that returns item 25 of the tuple.</param>
        /// <param name="func26">Function that returns item 26 of the tuple.</param>
        /// <param name="func27">Function that returns item 27 of the tuple.</param>
        /// <param name="func28">Function that returns item 28 of the tuple.</param>
        /// <param name="func29">Function that returns item 29 of the tuple.</param>
        /// <param name="func30">Function that returns item 30 of the tuple.</param>
        /// <param name="func31">Function that returns item 31 of the tuple.</param>
        /// <param name="func32">Function that returns item 32 of the tuple.</param>
        /// <param name="func33">Function that returns item 33 of the tuple.</param>
        /// <param name="func34">Function that returns item 34 of the tuple.</param>
        /// <param name="func35">Function that returns item 35 of the tuple.</param>
        /// <returns>A tuple with 35 items (using nested tuples where necessary).</returns>
        /// <remarks></remarks>
        [UsedImplicitly]
        public static
        IEnumerable
            <
                Tuple
                    <T1, T2, T3, T4, T5, T6, T7,
                        Tuple
                            <T8, T9, T10, T11, T12, T13, T14,
                                Tuple
                                    <T15, T16, T17, T18, T19, T20, T21,
                                        Tuple
                                            <T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34, T35>
                                                >>>>> ToTuple
            <TInput, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22,
                T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35>(
            [NotNull] this IEnumerable<TInput> inputEnumeration,
            [NotNull] Func<TInput, T1> func1,
            [NotNull] Func<TInput, T2> func2,
            [NotNull] Func<TInput, T3> func3,
            [NotNull] Func<TInput, T4> func4,
            [NotNull] Func<TInput, T5> func5,
            [NotNull] Func<TInput, T6> func6,
            [NotNull] Func<TInput, T7> func7,
            [NotNull] Func<TInput, T8> func8,
            [NotNull] Func<TInput, T9> func9,
            [NotNull] Func<TInput, T10> func10,
            [NotNull] Func<TInput, T11> func11,
            [NotNull] Func<TInput, T12> func12,
            [NotNull] Func<TInput, T13> func13,
            [NotNull] Func<TInput, T14> func14,
            [NotNull] Func<TInput, T15> func15,
            [NotNull] Func<TInput, T16> func16,
            [NotNull] Func<TInput, T17> func17,
            [NotNull] Func<TInput, T18> func18,
            [NotNull] Func<TInput, T19> func19,
            [NotNull] Func<TInput, T20> func20,
            [NotNull] Func<TInput, T21> func21,
            [NotNull] Func<TInput, T22> func22,
            [NotNull] Func<TInput, T23> func23,
            [NotNull] Func<TInput, T24> func24,
            [NotNull] Func<TInput, T25> func25,
            [NotNull] Func<TInput, T26> func26,
            [NotNull] Func<TInput, T27> func27,
            [NotNull] Func<TInput, T28> func28,
            [NotNull] Func<TInput, T29> func29,
            [NotNull] Func<TInput, T30> func30,
            [NotNull] Func<TInput, T31> func31,
            [NotNull] Func<TInput, T32> func32,
            [NotNull] Func<TInput, T33> func33,
            [NotNull] Func<TInput, T34> func34,
            [NotNull] Func<TInput, T35> func35
            )
        {
            return
                inputEnumeration.Select(
                    input =>
                        new Tuple
                        <T1, T2, T3, T4, T5, T6, T7,
                        Tuple
                        <T8, T9, T10, T11, T12, T13, T14,
                        Tuple
                        <T15, T16, T17, T18, T19, T20, T21,
                        Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34, T35>>>>>(
                        func1(input),
                        func2(input),
                        func3(input),
                        func4(input),
                        func5(input),
                        func6(input),
                        func7(input),
                        new Tuple
                            <T8, T9, T10, T11, T12, T13, T14,
                            Tuple
                            <T15, T16, T17, T18, T19, T20, T21,
                            Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34, T35>>>>(
                            func8(input),
                            func9(input),
                            func10(input),
                            func11(input),
                            func12(input),
                            func13(input),
                            func14(input),
                            new Tuple
                                <T15, T16, T17, T18, T19, T20, T21,
                                Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34, T35>>>(
                                func15(input),
                                func16(input),
                                func17(input),
                                func18(input),
                                func19(input),
                                func20(input),
                                func21(input),
                                new Tuple<T22, T23, T24, T25, T26, T27, T28, Tuple<T29, T30, T31, T32, T33, T34, T35>>(
                                    func22(input),
                                    func23(input),
                                    func24(input),
                                    func25(input),
                                    func26(input),
                                    func27(input),
                                    func28(input),
                                    new Tuple<T29, T30, T31, T32, T33, T34, T35>(
                                        func29(input),
                                        func30(input),
                                        func31(input),
                                        func32(input),
                                        func33(input),
                                        func34(input),
                                        func35(input)))))));
        }
        #endregion
    }
}