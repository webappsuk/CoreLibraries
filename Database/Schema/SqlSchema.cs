using System;
using System.Diagnostics.Contracts;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Database.Schema
{
    /// <summary>
    /// Holds information about a SQL schema (e.g. 'dbo').
    /// </summary>
    public class SqlSchema : IEquatable<SqlSchema>
    {
        /// <summary>
        /// The identifier.
        /// </summary>
        [PublicAPI]
        public readonly int ID;

        /// <summary>
        /// The name.
        /// </summary>
        [PublicAPI]
        [NotNull]
        public readonly string Name;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlSchema"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="name">The name.</param>
        // ReSharper disable once CodeAnnotationAnalyzer
        internal SqlSchema(int id, [NotNull]string name)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(name));
            ID = id;
            Name = name;
        }

        #region Equalities
        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><see langword="true" /> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <see langword="false" />.</returns>
        public override bool Equals([CanBeNull] object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            SqlSchema other = obj as SqlSchema;
            return !ReferenceEquals(other, null) &&
                   Equals(ID, other.ID) &&
                   string.Equals(Name, other.Name, StringComparison.InvariantCultureIgnoreCase);
        }
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals([CanBeNull] SqlSchema other)
        {
            if (ReferenceEquals(null, other)) return false;
            return ReferenceEquals(this, other) ||
                   (Equals(ID, other.ID) &&
                    string.Equals(Name, other.Name, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (ID * 397) ^ StringComparer.InvariantCultureIgnoreCase.GetHashCode(Name);
            }
        }

        /// <summary>
        /// Implements the ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(SqlSchema left, SqlSchema right)
        {
            if (ReferenceEquals(left, null)) return ReferenceEquals(right, null);
            return !ReferenceEquals(right, null) &&
                   Equals(left.ID, right.ID) &&
                   string.Equals(left.Name, right.Name, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Implements the !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(SqlSchema left, SqlSchema right)
        {
            if (ReferenceEquals(left, null)) return ReferenceEquals(right, null);
            return ReferenceEquals(right, null) ||
                   !Equals(left.ID, right.ID) ||
                   !string.Equals(left.Name, right.Name, StringComparison.InvariantCultureIgnoreCase);
        }
        #endregion

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return Name;
        }
    }
}