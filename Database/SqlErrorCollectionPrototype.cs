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
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Database
{
    /// <summary>
    /// Allows creation of a <see cref="SqlErrorCollection"/>, and addition of <see cref="SqlError">SqlErrors</see>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// To add <see cref="SqlError">SqlErrors</see> you will probably want to create them with the <see cref="SqlErrorPrototype"/> class,
    /// which supports implicit conversion to <see cref="SqlError"/>.</para>
    /// <para>In the same way, this class supports implicit conversion to <see cref="SqlErrorCollection"/>.</para>
    /// </remarks>
    /// <seealso cref="SqlErrorPrototype"/>
    [PublicAPI]
    public sealed class SqlErrorCollectionPrototype : ICollection
    {
        /// <summary>
        /// Creates a new <see cref="SqlErrorCollection"/>.
        /// </summary>
        [NotNull]
        private static readonly Func<SqlErrorCollection> _constructor;

        /// <summary>
        /// Adds a <see cref="SqlError"/> to a <see cref="SqlErrorCollection"/>.
        /// </summary>
        [NotNull]
        private static readonly Action<SqlErrorCollection, SqlError> _adder;

        /// <summary>
        /// The equivalent <see cref="SqlErrorCollection" />.
        /// </summary>
        [NotNull]
        public readonly SqlErrorCollection SqlErrorCollection;

        /// <summary>
        /// Creates the <see cref="_constructor"/> function and the <see cref="_adder"/> action.
        /// </summary>
        static SqlErrorCollectionPrototype()
        {
            // Find constructor
            ConstructorInfo constructorInfo =
                typeof(SqlErrorCollection).GetConstructor(
                    BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance,
                    null,
                    Type.EmptyTypes,
                    null);
            Debug.Assert(constructorInfo != null);

            // Create lambda expression.
            _constructor = Expression.Lambda<Func<SqlErrorCollection>>(
                Expression.New(constructorInfo),
                Enumerable.Empty<ParameterExpression>()).Compile();

            // Find the add method
            MethodInfo addMethod = typeof(SqlErrorCollection).GetMethod(
                "Add",
                BindingFlags.NonPublic | BindingFlags.Instance |
                BindingFlags.InvokeMethod);
            Debug.Assert(addMethod != null);

            // Create instance parameter
            ParameterExpression instanceParameter = Expression.Parameter(typeof(SqlErrorCollection), "collection");

            // Create method parameter
            ParameterExpression errorParamter = Expression.Parameter(typeof(SqlError), "error");

            _adder =
                Expression.Lambda<Action<SqlErrorCollection, SqlError>>(
                    Expression.Call(instanceParameter, addMethod, errorParamter),
                    instanceParameter,
                    errorParamter).
                    Compile();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlErrorCollectionPrototype" /> class.
        /// </summary>
        /// <remarks></remarks>
        public SqlErrorCollectionPrototype()
        // ReSharper disable once AssignNullToNotNullAttribute
            : this(_constructor())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlErrorCollectionPrototype" /> class.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <remarks></remarks>
        public SqlErrorCollectionPrototype([NotNull] SqlErrorCollection collection)
        {
            SqlErrorCollection = collection ?? throw new ArgumentNullException(nameof(collection));
        }

        /// <summary>
        /// Gets the error at the specified index.
        /// </summary>
        /// 
        /// <returns>
        /// A <see cref="T:System.Data.SqlClient.SqlError"/> that contains the error at the specified index.
        /// </returns>
        /// <param name="index">The zero-based index of the error to retrieve. </param><exception cref="T:System.IndexOutOfRangeException">Index parameter is outside array bounds. </exception><filterpriority>2</filterpriority>
        public SqlError this[int index] => SqlErrorCollection[index];

        /// <summary>
        /// Adds the specified error to the collection.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <remarks></remarks>
        public void Add(SqlError error) => _adder(SqlErrorCollection, error);

        /// <inheritdoc/>
        public override string ToString() => SqlErrorCollection.ToString();

        /// <summary>
        /// Implicit conversion from <see cref="SqlErrorCollectionPrototype" /> to <see cref="SqlErrorCollection" />.
        /// </summary>
        /// <param name="prototype">The prototype.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks></remarks>
        public static implicit operator SqlErrorCollection(SqlErrorCollectionPrototype prototype) => prototype?.SqlErrorCollection;

        /// <summary>
        /// Implicit conversion from <see cref="SqlErrorCollection" /> to <see cref="SqlErrorCollectionPrototype" />.
        /// </summary>
        /// <param name="collection">The SQL error.</param>
        /// <returns>The result of the operator.</returns>
        /// <remarks></remarks>
        public static implicit operator SqlErrorCollectionPrototype(SqlErrorCollection collection)
        {
            return collection != null
                ? new SqlErrorCollectionPrototype(collection)
                : null;
        }

        #region ICollection Members
        /// <inheritdoc/>
        public IEnumerator GetEnumerator() => SqlErrorCollection.GetEnumerator();

        /// <inheritdoc/>
        public void CopyTo(Array array, int index) => SqlErrorCollection.CopyTo(array, index);

        /// <inheritdoc/>
        public int Count => SqlErrorCollection.Count;

        /// <inheritdoc/>
        public object SyncRoot => ((ICollection)SqlErrorCollection).SyncRoot;

        /// <inheritdoc/>
        public bool IsSynchronized => false;
        #endregion
    }
}