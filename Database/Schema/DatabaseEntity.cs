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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using JetBrains.Annotations;
using WebApplications.Utilities.Logging;
using WebApplications.Utilities.Reflect;

namespace WebApplications.Utilities.Database.Schema
{
    /// <summary>
    /// Base class of all entities in a <see cref="DatabaseSchema"/>.
    /// </summary>
    public abstract class DatabaseEntity : IEquatable<DatabaseEntity>
    {
        /// <summary>
        /// The <see cref="PropertyInfo"/> for <see cref="HashCode"/>.
        /// </summary>
        [NotNull]
        protected static readonly PropertyInfo HashCodeProperty =
            InfoHelper.GetPropertyInfo<DatabaseEntity, long>(e => e.HashCode);

        /// <summary>
        /// The <see cref="List{T}.Add"/> method for a <see cref="List{T}">list of</see> <see cref="Difference">differences</see>.
        /// </summary>
        [NotNull]
        protected static readonly MethodInfo DifferenceAddMethod =
            InfoHelper.GetMethodInfo<List<Difference>>(l => l.Add(default(Difference)));

        /// <summary>
        /// A hash code.
        /// </summary>
        [PublicAPI]
        public abstract long HashCode { get; }

        /// <summary>
        /// The full name.
        /// </summary>
        [PublicAPI]
        [NotNull]
        public string FullName;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseEntity{T}" /> class.
        /// </summary>
        /// <param name="fullName">The full name.</param>
        protected DatabaseEntity([NotNull] string fullName)
        {
            Contract.Requires(fullName != null);
            // ReSharper disable once PossibleNullReferenceException
            FullName = fullName;
        }

        /// <summary>
        /// Gets the differences between this instance and the <paramref namef="other"/>.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>Delta.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        [PublicAPI]
        [NotNull]
        public abstract Delta GetDifferences([NotNull] DatabaseEntity other);

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><see langword="true" /> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <see langword="false" />.</returns>
        public override bool Equals([CanBeNull] object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            DatabaseEntity other = (DatabaseEntity) obj;
            return HashCode.Equals(other.HashCode) &&
                   GetDifferences(other).IsEmpty;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals([CanBeNull] DatabaseEntity other)
        {
            if (ReferenceEquals(null, other)) return false;
            return ReferenceEquals(this, other) ||
                   (HashCode.Equals(other.HashCode) &&
                    other.GetType() == GetType() &&
                    GetDifferences(other).IsEmpty);
        }

        /// <summary>
        /// Implements the ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(DatabaseEntity left, DatabaseEntity right)
        {
            if (ReferenceEquals(null, left)) return ReferenceEquals(null, right);
            if (ReferenceEquals(left, right)) return true;
            return !ReferenceEquals(right, null) &&
                   left.HashCode.Equals(right.HashCode) &&
                   left.GetType() == right.GetType() &&
                   left.GetDifferences(right).IsEmpty;
        }

        /// <summary>
        /// Implements the !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(DatabaseEntity left, DatabaseEntity right)
        {
            if (ReferenceEquals(null, left)) return !ReferenceEquals(null, right);
            if (ReferenceEquals(left, right)) return false;
            return ReferenceEquals(right, null) ||
                   !left.HashCode.Equals(right.HashCode) ||
                   left.GetType() != right.GetType() ||
                   !left.GetDifferences(right).IsEmpty;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return HashCode.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return FullName;
        }
    }

    /// <summary>
    /// Base class of all entities in a <see cref="DatabaseSchema"/>.
    /// </summary>
    /// <typeparam name="T">This type</typeparam>
    public abstract class DatabaseEntity<T> : DatabaseEntity, IEquatable<DatabaseEntity<T>>, IEquatable<T>
        where T : DatabaseEntity<T>
    {
        [NotNull]
        private static readonly Func<T, long> _getHashCodeFunc;

        [NotNull]
        private static readonly Action<T, T, List<Difference>> _addDifferences;

        /// <summary>
        /// Initializes static members of the <see cref="DatabaseEntity{T}"/> class.
        /// </summary>
        static DatabaseEntity()
        {
            // ReSharper disable once PossibleNullReferenceException
            MemberInfo[] properties = typeof (T).GetGetter<Expression<Func<T, object>>[]>("_properties")()
                .Select(
                    e =>
                        ((MemberExpression)
                        (e.Body.NodeType == ExpressionType.Convert ? ((UnaryExpression) e.Body).Operand : e.Body))
                        .Member)
                .ToArray();
            Contract.Assert(properties != null);

            ParameterExpression inputExpression = Expression.Parameter(typeof (T), "input");
            Expression hcExpression = null;


            ParameterExpression leftExpression = Expression.Parameter(typeof (T), "left");
            ParameterExpression rightExpression = Expression.Parameter(typeof (T), "right");
            ParameterExpression differencesExpression = Expression.Parameter(typeof (List<Difference>), "differences");
            List<Expression> adExpressions = new List<Expression>(properties.Length);
            foreach (MemberInfo memberInfo in properties)
            {
                Contract.Assert(memberInfo != null);

                /*
                 * Build hash code function
                 */
                Expression e = Expression.MakeMemberAccess(inputExpression, memberInfo);

                // Calculate the hash code using the HashCode property if available, otherwise GetHashCode.
                bool isEnumerable = e.Type.IsGenericType &&
                                    e.Type.GetGenericTypeDefinition() == typeof (IEnumerable<>);
                if (isEnumerable)
                {
                    ParameterExpression variable = Expression.Variable(typeof (long));

                    Expression ee = new[]
                    {
                        e.ForEach(
                            item => Expression.Assign(variable, GetHashExpression(variable, item))),
                        variable
                    }.Blockify(variable);

                    if (e.Type.IsNullable())
                        ee = Expression.Condition(
                            Expression.ReferenceEqual(e, Expression.Constant(null, e.Type)),
                            Expression.Constant(0L),
                            ee);

                    hcExpression = hcExpression != null
                        ? Expression.ExclusiveOr(
                            Expression.Multiply(Expression.Constant(397L), hcExpression),
                            ee)
                        : ee;
                }
                else
                    hcExpression = GetHashExpression(hcExpression, e);

                /*
                 * Build add differences action
                 */
                Expression le = Expression.MakeMemberAccess(leftExpression, memberInfo);
                Expression re = Expression.MakeMemberAccess(rightExpression, memberInfo);

                Expression eq;
                if (isEnumerable)
                {
                    Type gType = le.Type.GenericTypeArguments.First();
                    eq = gType == typeof (string)
                        ? Expression.Call(
                            typeof (Enumerable).GetMethods()
                                .First(m => m.Name == "SequenceEqual" && m.GetParameters().Count() == 3)
                                .MakeGenericMethod(gType),
                            le,
                            re,
                            Expression.Constant(StringComparer.InvariantCultureIgnoreCase))
                        : Expression.Call(
                            typeof (Enumerable).GetMethods()
                                .First(m => m.Name == "SequenceEqual" && m.GetParameters().Count() == 2)
                                .MakeGenericMethod(gType),
                            le,
                            re);
                }
                else
                    eq = le.Type == typeof (string)
                        ? (Expression)
                            Expression.Call(
                                Expression.Constant(StringComparer.InvariantCultureIgnoreCase),
                                "Equals",
                                null,
                                le,
                                re)
                        : Expression.Equal(le, re);

                adExpressions.Add(
                    Expression.IfThenElse(
                        eq,
                        Expression.Empty(),
                        Expression.Call(
                            differencesExpression,
                            DifferenceAddMethod,
                            // ReSharper disable once PossiblyMistakenUseOfParamsMethod
                            Expression.New(
                                // ReSharper disable once AssignNullToNotNullAttribute
                                typeof (Difference<>).MakeGenericType(le.Type)
                                    .GetConstructor(
                                        BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance,
                                        null,
                                        new[] {typeof (string), le.Type, re.Type},
                                        null),
                                Expression.Constant(memberInfo.Name),
                                le,
                                re))));
            }
            Contract.Assert(hcExpression != null);
            Expression<Func<T, long>> ghcLambda = Expression.Lambda<Func<T, long>>(hcExpression, inputExpression);
            _getHashCodeFunc = ghcLambda.Compile();
            Expression<Action<T, T, List<Difference>>> addLambda = Expression.Lambda<Action<T, T, List<Difference>>>(
                adExpressions.Blockify(),
                leftExpression,
                rightExpression,
                differencesExpression);
            _addDifferences =
                addLambda.Compile();
        }

        /// <summary>
        /// Gets the hash expression.
        /// </summary>
        /// <param name="hashCodeExpression">The hash code expression.</param>
        /// <param name="propertyExpression">The property expression.</param>
        /// <returns>Expression.</returns>
        [NotNull]
        private static Expression GetHashExpression(
            [CanBeNull] Expression hashCodeExpression,
            [NotNull] Expression propertyExpression)
        {
            Contract.Requires(propertyExpression != null);
            Expression gh = propertyExpression.Type.DescendsFrom(typeof (DatabaseEntity))
                ? (Expression) Expression.Property(propertyExpression, HashCodeProperty)
                : Expression.Convert(
                    propertyExpression.Type == typeof (string)
                        ? Expression.Call(
                            Expression.Constant(StringComparer.InvariantCultureIgnoreCase),
                            "GetHashCode",
                            null,
                            propertyExpression)
                        : Expression.Call(propertyExpression, "GetHashCode", Reflection.EmptyTypes),
                    typeof (long));

            // Wrap with null check if necessary
            propertyExpression = propertyExpression.Type.IsNullable()
                ? Expression.Condition(
                    Expression.ReferenceEqual(propertyExpression, Expression.Constant(null, propertyExpression.Type)),
                    Expression.Constant(0L),
                    gh)
                : gh;

            hashCodeExpression = hashCodeExpression != null
                ? Expression.ExclusiveOr(
                    Expression.Multiply(Expression.Constant(397L), hashCodeExpression),
                    propertyExpression)
                : propertyExpression;
            return hashCodeExpression;
        }

        /// <summary>
        /// Lazy evaluates the hash code.
        /// </summary>
        [NotNull]
        private readonly Lazy<long> _hashCode;

        /// <summary>
        /// A hash code.
        /// </summary>
        /// <value>The hash code.</value>
        /// <exception cref="System.NotImplementedException"></exception>
        public override long HashCode
        {
            get { return _hashCode.Value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseEntity{T}" /> class.
        /// </summary>
        /// <param name="fullName">The full name.</param>
        protected DatabaseEntity([NotNull] string fullName)
            : base(fullName)
        {
            Contract.Requires(fullName != null);
            _hashCode = new Lazy<long>(() => _getHashCodeFunc((T) this), LazyThreadSafetyMode.PublicationOnly);
        }

        /// <summary>
        /// Gets the differences between this instance and the <paramref namef="other" />.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>Delta.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override Delta GetDifferences(DatabaseEntity other)
        {
            Contract.Requires(other != null);
            T entity = other as T;
            if (entity == null)
                throw new LoggingException(
                    () => "Cannot get the differences between diferrent types of DatabaseEntity.");

            T me = (T) this;
            List<Difference> differences = new List<Difference>();
            _addDifferences(me, entity, differences);

            // ReSharper disable once PossibleNullReferenceException
            if (!StringComparer.InvariantCultureIgnoreCase.Equals(FullName, other.FullName))
                differences.Add(new Difference<string>("FullName", FullName, other.FullName));
            return new Delta<T>(me, entity, differences);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals([CanBeNull] T other)
        {
            if (ReferenceEquals(null, other)) return false;
            return ReferenceEquals(this, other) ||
                   (HashCode.Equals(other.HashCode) &&
                    GetDifferences(other).IsEmpty);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals([CanBeNull] DatabaseEntity<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            return ReferenceEquals(this, other) ||
                   (HashCode.Equals(other.HashCode) &&
                    other.GetType() == GetType() &&
                    GetDifferences(other).IsEmpty);
        }
    }
}