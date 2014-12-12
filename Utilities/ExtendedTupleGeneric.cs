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
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities
{
    /// <summary>
    ///   Provides additional information and accessors for a <see cref="Tuple"/>.
    /// </summary>
    /// <typeparam name="T">The tuple type.</typeparam>
    [UsedImplicitly]
    public sealed class ExtendedTuple<T> : IEnumerable
        where T : class, IStructuralEquatable, IStructuralComparable, IComparable
    {
        /// <summary>
        ///   Accepts a <see cref="Tuple"/> and an index and returns the item at that index.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public static readonly Func<T, int, object> Indexer;

        /// <summary>
        ///   An <see cref="Array"/> containing the types of all the items in the <see cref="Tuple"/>.
        /// </summary>
        private static readonly Type[] _types;

        /// <summary>
        ///   Accepts a <see cref="Tuple"/> and returns its size (the number of items).
        /// </summary>
        [UsedImplicitly]
        public static readonly int Size;

        /// <summary>
        ///   Holds the instance of the extended <see cref="Tuple"/>.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public readonly T Tuple;

        /// <summary>
        ///   Initialises the current <see cref="Tuple"/> type.
        /// </summary>
        static ExtendedTuple()
        {
            Type tupleType = typeof (T);
            if ((!tupleType.IsGenericType) ||
                (!tupleType.GetGenericTypeDefinition().FullName.StartsWith("System.Tuple`")))
                throw new InvalidOperationException(
                    String.Format(
                        Resources.ExtendedTuple_TypeIsNotValidTuple,
                        typeof (T)));

            // Create the tuple parameter.
            ParameterExpression tupleParameter = Expression.Parameter(typeof (T), "tuple");

            /*
             * Next create the indexer
             */
            ParameterExpression indexParameter = Expression.Parameter(typeof (int), "index");

            // Expression to hold the current tuple's value.
            Expression currentTuple = tupleParameter;

            // We're going to build a list of switch cases for a switch statement.
            List<SwitchCase> switchCases = new List<SwitchCase>();
            List<Type> tupleTypes = new List<Type>();
            int index = 0;
            bool recurse;
            do
            {
                // Get the tuple item types
                Type[] types = tupleType.GetGenericArguments();
                int size = types.Length;

                // Sanity check, currently tuples only support 7 items and a 'Rest' item.
                if (size > 8)
                    throw new InvalidOperationException(
                        String.Format(Resources.ExtendedTuple_MoreThanEightGenericArguments, tupleType));

                recurse = false;
                // Add a switch case for each tuple 
                for (int tIndex = 0; tIndex < size; tIndex++)
                {
                    string propertyName = tIndex < 7 ? "Item" + (tIndex + 1) : "Rest";

                    // If last (7th) element is a tuple we should recurse - the tuple standard doesn't
                    // support recursion based on just the last element, though it would be trivial
                    // for us to do so, it would however mean a mismatch with the underlying implementation.
                    if ((tIndex == 7) &&
                        (types[tIndex].IsGenericType) &&
                        (types[tIndex].GetGenericTypeDefinition().FullName.StartsWith("System.Tuple`")))
                        recurse = true;

                    // Get the relevant property accessor
                    MethodInfo propertyMethod = tupleType.GetProperty(
                        propertyName,
                        BindingFlags.Public | BindingFlags.Instance |
                        BindingFlags.DeclaredOnly).GetGetMethod();
                    if (propertyMethod == null)
                        throw new InvalidOperationException(
                            string.Format(Resources.ExtendedTuple_CouldNotFindProperty, tupleType, propertyName));

                    // Create the property accessor expression.
                    Expression propertyExpression = Expression.Call(currentTuple, propertyMethod);

                    if (!recurse)
                    {
                        // If we're not an object convert to object.
                        if (types[tIndex] != typeof (object))
                            propertyExpression = propertyExpression.Convert(typeof (object));

                        // Add switch case to return value of property.
                        switchCases.Add(
                            Expression.SwitchCase(
                                propertyExpression,
                                Expression.Constant(index++)));

                        tupleTypes.Add(types[tIndex]);
                    }
                    else
                    {
                        // TRest is a tuple so we actually need to recurse.
                        tupleType = types[tIndex];

                        // Set the current tuple to the result of the last property expression and continue
                        currentTuple = propertyExpression;
                        break;
                    }
                }
            } while (recurse);

            // Store the tuple size.
            Size = switchCases.Count;

            // Store the item types
            _types = tupleTypes.ToArray();

            // Create compiled lambda from the switch statement.
            Indexer = Expression.Lambda<Func<T, int, object>>(
                Expression.Block(
                    Expression.IfThen(
                        Expression.Or(
                            Expression.LessThan(indexParameter, Expression.Constant(0)),
                            Expression.GreaterThanOrEqual(indexParameter, Expression.Constant(Size))),
                        Expression.Throw(Expression.Constant(new IndexOutOfRangeException()))),
                    Expression.Switch(
                        indexParameter,
                        Expression.Constant(null, typeof (object)),
                        switchCases.ToArray())),
                tupleParameter,
                indexParameter).Compile();
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="ExtendedTuple&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="tuple">The <see cref="Tuple"/> to extend.</param>
        public ExtendedTuple([NotNull] T tuple)
        {
            Tuple = tuple;
        }

        /// <summary>
        ///   Gets the types of all the items in the <see cref="Tuple"/>.
        /// </summary>
        [NotNull]
        public static Type[] Types
        {
            get { return (Type[]) _types.Clone(); }
        }

        /// <summary>
        ///   Retrieves the item at the index specified.
        /// </summary>
        /// <returns>The item located at the index specified.</returns>
        [CanBeNull]
        public object this[int index]
        {
            get { return Indexer(Tuple, index); }
        }

        #region IEnumerable Members
        /// <summary>
        ///   Returns an enumerator that iterates through all the items in the collection.
        /// </summary>
        /// <returns>
        ///   An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through all the items in the collection.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            for (int a = 0; a < Size; a++)
                yield return Indexer(Tuple, a);
        }
        #endregion

        /// <summary>
        ///   Retrieves the item's type using the index specified.
        /// </summary>
        /// <param name="index">The item index.</param>
        /// <returns>The type of the item at the <paramref name="index"/> specified.</returns>
        [NotNull]
        public static Type GetIndexType(int index)
        {
            return _types[index];
        }

        /// <summary>
        ///   Retrieves the item at the index specified.
        /// </summary>
        /// <typeparam name="TItem">The item type.</typeparam>
        /// <param name="index">The item index.</param>
        /// <returns>The item located at the <paramref name="index"/> specified.</returns>
        /// <exception cref="InvalidCastException">
        ///   The item located at <paramref name="index"/> cannot be cast to type <typeparamref name="TItem"/>.
        /// </exception>
        public TItem GetItem<TItem>(int index)
        {
            Type t = _types[index];
            if (!typeof (TItem).IsAssignableFrom(t))
                throw new InvalidCastException(
                    string.Format(
                        Resources.ExtendedTuple_CannotCastItemAtIndex,
                        index,
                        t,
                        typeof (TItem)));
            return (TItem) this[index];
        }
    }
}