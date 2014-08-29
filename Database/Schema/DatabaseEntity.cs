using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using JetBrains.Annotations;
using WebApplications.Utilities.Logging;

namespace WebApplications.Utilities.Database.Schema
{
    /// <summary>
    /// Base class of all entities in a <see cref="DatabaseSchema"/>.
    /// </summary>
    public abstract class DatabaseEntity : IEquatable<DatabaseEntity>
    {
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
        /// <param name="hashCode">The hash code.</param>
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
            if (obj.GetType() != this.GetType()) return false;
            DatabaseEntity other = (DatabaseEntity)obj;
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
                other.GetType() == this.GetType() &&
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
        /// <summary>
        /// A hash code.
        /// </summary>
        /// <value>The hash code.</value>
        /// <exception cref="System.NotImplementedException"></exception>
        public override long HashCode
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseEntity{T}" /> class.
        /// </summary>
        /// <param name="fullName">The full name.</param>
        /// <param name="propertyExpressions">The property expressions.</param>
        protected DatabaseEntity(
            [NotNull] string fullName,
            [NotNull] params Expression<Func<T, object>>[] propertyExpressions)
            : base(fullName)
        {
            Contract.Requires(fullName != null);
            Contract.Requires(propertyExpressions != null);
            // TODO Build delta generator
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
            List<Difference> differences = new List<Difference>();
            // TODO

            // ReSharper disable once PossibleNullReferenceException
            if (!StringComparer.InvariantCultureIgnoreCase.Equals(FullName, other.FullName))
                differences.Add(new Difference<string>("FullName", FullName, other.FullName));
            return new Delta<T>((T)this, entity, differences);
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
                other.GetType() == this.GetType() &&
                GetDifferences(other).IsEmpty);
        }
    }
}