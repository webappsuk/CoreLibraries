#region © Copyright Web Applications (UK) Ltd, 2012.  All rights reserved.
// Copyright (c) 2012, Web Applications UK Ltd
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
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;

namespace WebApplications.Utilities
{
    /// <summary>
    ///   Extension methods for <see cref="System.Tuple">tuples</see>.
    /// </summary>
    public static partial class ExtendedTuple
    {
        /// <summary>
        ///   A cache for indexers (by tuple type), so that when requested they can be retrieved rather than recomputed.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, Func<object, int, object>> _tupleIndexers =
            new ConcurrentDictionary<Type, Func<object, int, object>>();

        /// <summary>
        ///   A cache of the item types for a tuple (by tuple type), so that when requested they can be retrieved rather than recomputed.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, Type[]> _tupleTypes =
            new ConcurrentDictionary<Type, Type[]>();

        /// <summary>
        ///   A cache of the iterators (by tuple type) so that when requested they can be retrieved rather than recomputed.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, Func<object, IEnumerable>> _tupleIterators =
            new ConcurrentDictionary<Type, Func<object, IEnumerable>>();

        /// <summary>
        ///   Retrieves the item at the index specified.
        /// </summary>
        /// <typeparam name="TTuple">The tuple type.</typeparam>
        /// <typeparam name="TItem">The item type.</typeparam>
        /// <param name="tuple">The tuple.</param>
        /// <param name="index">The index of the item to retrieve.</param>
        /// <returns>The item located at the <paramref name="index"/> specified.</returns>
        /// <exception cref="InvalidCastException">
        ///   The item located at <paramref name="index"/> cannot be cast to type <typeparamref name="TItem"/>.
        /// </exception>
        public static TItem GetTupleItem<TTuple, TItem>([NotNull] this TTuple tuple, int index)
            where TTuple : class, IStructuralEquatable, IStructuralComparable, IComparable
        {
            Type t = GetIndexType(tuple, index);
            if (!typeof (TItem).IsAssignableFrom(t))
                throw new InvalidCastException(
                    string.Format(Resources.ExtendedTuple_CannotCastItemAtIndex,
                                  index,
                                  t,
                                  typeof (TItem)));
            return (TItem) ExtendedTuple<TTuple>.Indexer(tuple, index);
        }

        /// <summary>
        ///   Gets the indexer for the specified <see cref="Tuple"/>.
        /// </summary>
        /// <typeparam name="T">The tuple type.</typeparam>
        /// <param name="tuple">The tuple whose indexer we want.</param>
        /// <returns>
        ///   A function that takes an index and retrieves the corresponding item from the specified <paramref name="tuple"/>.
        /// </returns>
        [UsedImplicitly]
        public static Func<int, object> GetTupleIndexer<T>([NotNull] this T tuple)
            where T : class, IStructuralEquatable, IStructuralComparable, IComparable
        {
            return i => ExtendedTuple<T>.Indexer(tuple, i);
        }

        /// <summary>
        ///   Retrieves the indexer for type of <see cref="Tuple"/> specified.
        /// </summary>
        /// <param name="tupleType">The type of the tuple.</param>
        /// <returns>The indexer (as a function) for the <paramref name="tupleType"/> specified</returns>
        /// <remarks>
        ///   <para>Supports standard tuple extension, where TRest is itself a <see cref="Tuple"/>.</para>
        ///   <para>For example index 7 (indices are zero-indexed so that is the 8th item) of a tuple of type:
        ///   Tuple&lt;int, int, int, int, int, int, int, <b>Tuple&lt;string&gt;</b>&gt; is the equivalent of
        ///   <b>tuple.Rest.Item1</b> and is of type <see cref="string"/>.</para>
        ///   <para>The item at index 7 of a tuple of type Tuple&lt;int, int, int, int, int, int, int, <b>string&gt;</b>
        ///   is the equivalent of <b>tuple.Rest</b> and is of type <see cref="string"/>.</para>
        /// </remarks>
        [UsedImplicitly]
        [NotNull]
        public static Func<object, int, object> GetTupleIndexer([NotNull] this Type tupleType)
        {
            return _tupleIndexers.GetOrAdd(
                tupleType,
                t =>
                    {
                        // Create extended tuple type
                        Type extendedType = typeof (ExtendedTuple<>).MakeGenericType(tupleType);

                        // Create parameters
                        ParameterExpression tupleObjectParameter = Expression.Parameter(typeof (object), "tuple");
                        ParameterExpression indexParameter = Expression.Parameter(typeof (int), "index");

                        // Convert parameter from object to tuple type.
                        Expression castTuple = tupleObjectParameter.Convert(tupleType);

                        // Get the indexer lambda and convert to a constant
                        Expression lambda = Expression.Constant(
                            extendedType.GetField("Indexer", BindingFlags.Static | BindingFlags.Public)
                                .GetValue(null));

                        return Expression.Lambda<Func<object, int, object>>(
                            Expression.Invoke(lambda, castTuple, indexParameter),
                            tupleObjectParameter,
                            indexParameter).Compile();
                    });
        }

        /// <summary>
        ///   Gets the type of the item at the specified index.
        /// </summary>
        /// <typeparam name="T">The tuple type.</typeparam>
        /// <param name="tuple">The tuple.</param>
        /// <param name="index">The item index.</param>
        /// <returns>The type of the item at the specified <paramref name="index"/>.</returns>
        [NotNull]
        public static Type GetIndexType<T>([NotNull] this T tuple, int index)
            where T : class, IStructuralEquatable, IStructuralComparable, IComparable
        {
            return ExtendedTuple<T>.GetIndexType(index);
        }

        /// <summary>
        ///   Gets an <see cref="Array"/> of the types for all the items in the <see cref="Tuple"/>.
        /// </summary>
        /// <typeparam name="T">The tuple type.</typeparam>
        /// <param name="tuple">The tuple.</param>
        /// <returns>
        ///   An <see cref="Array"/> containing all the item types of the specified <paramref name="tuple"/>.
        /// </returns>
        [NotNull]
        public static Type[] GetIndexTypes<T>([NotNull] this T tuple)
            where T : class, IStructuralEquatable, IStructuralComparable, IComparable
        {
            return ExtendedTuple<T>.Types;
        }

        /// <summary>
        ///   Gets an <see cref="Array"/> representation of all the types in the <see cref="Tuple"/>.
        /// </summary>
        /// <param name="tupleType">The tuple type.</param>
        /// <returns>
        ///   An <see cref="Array"/> containing all the item types of the specified <paramref name="tupleType"/>.
        /// </returns>
        [NotNull]
        public static Type[] GetIndexTypes([NotNull] this Type tupleType)
        {
            // ReSharper disable AssignNullToNotNullAttribute
            return _tupleTypes.GetOrAdd(
                tupleType,
                t =>
                    {
                        // Create extended tuple type
                        Type extendedType = typeof (ExtendedTuple<>).MakeGenericType(tupleType);
                        return (Type[]) extendedType.GetProperty("Types", BindingFlags.Static | BindingFlags.Public)
                                            .GetValue(null, null);
                    });
            // ReSharper restore AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Gets the <see cref="Tuple">tuple</see>'s iterator.
        /// </summary>
        /// <typeparam name="T">The tuple type.</typeparam>
        /// <param name="tuple">The tuple.</param>
        /// <returns>An enumerator that can be used to iterate through all the items in the collection.</returns>
        public static IEnumerable GetTupleIterator<T>([NotNull] this T tuple)
            where T : class, IStructuralEquatable, IStructuralComparable, IComparable
        {
            return new ExtendedTuple<T>(tuple);
        }

        /// <summary>
        ///   Gets a function that accepts a <see cref="Tuple"/> and returns an iterator.
        /// </summary>
        /// <param name="tupleType">The type of the tuple.</param>
        /// <returns>A function that takes a <see cref="Tuple"/> and returns the enumerator.</returns>
        public static Func<object, IEnumerable> GetTupleIterator([NotNull] this Type tupleType)
        {
            return _tupleIterators.GetOrAdd(
                tupleType,
                t =>
                    {
                        // Create extended tuple type
                        Type extendedType = typeof (ExtendedTuple<>).MakeGenericType(tupleType);

                        // Create parameter
                        ParameterExpression tupleObjectParameter = Expression.Parameter(typeof (object), "tuple");

                        // Convert parameter from object to tuple type.
                        Expression castTuple = tupleObjectParameter.Convert(tupleType);

                        // Find constructor that takes the tuple type
                        ConstructorInfo constructor = extendedType.GetConstructor(new[] {tupleType});

                        // Create a lambda that creates a new ExtendedTuple<tupletype> and casts it to an IEnumerable.
                        return Expression.Lambda<Func<object, IEnumerable>>(
                            Expression.New(constructor, castTuple).Convert(typeof (IEnumerable)), tupleObjectParameter).
                            Compile();
                    });
        }
    }
}