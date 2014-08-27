using System;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WebApplications.Utilities.Database.Schema;

namespace WebApplications.Utilities.Database
{
    /// <summary>
    ///   Holds information about a connection.
    /// </summary>
    public class Connection : IEquatable<Connection>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Connection"/> class.  Used by <see cref="AddWeight"/> to avoid re-evaluating connection string.
        /// </summary>
        /// <param name="weight">The weight.</param>
        /// <param name="connectionString">The connection string.</param>
        private Connection(double weight, [NotNull] string connectionString)
        {
            Contract.Requires(connectionString != null);
            Weight = weight;
            ConnectionString = connectionString;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Connection"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="weight">
        ///   <para>The weighting of the connection.</para>
        ///   <para>By default this is set to 1.0.</para>
        /// </param>
        public Connection([NotNull] string connectionString, double weight = 1.0D)
        {
            Contract.Requires(connectionString != null);
            Weight = weight;

            // Load string into builder and coerce to async
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString)
            {
                AsynchronousProcessing = true,
            };

            // Connection reset is now obsolete.
            builder.Remove("Connection Reset");

            // Convert case-insensitive properties to lower case.
            if (builder.ApplicationName != null)
                builder.ApplicationName = builder.ApplicationName.ToLowerInvariant();
            if (builder.AttachDBFilename != null)
                builder.AttachDBFilename = builder.AttachDBFilename.ToLowerInvariant();
            if (builder.CurrentLanguage != null)
                builder.CurrentLanguage = builder.CurrentLanguage.ToLowerInvariant();
            if (builder.DataSource != null)
                builder.DataSource = builder.DataSource.ToLowerInvariant();
            if (builder.FailoverPartner != null)
                builder.FailoverPartner = builder.FailoverPartner.ToLowerInvariant();
            if (builder.InitialCatalog != null)
                builder.InitialCatalog = builder.InitialCatalog.ToLowerInvariant();
            if (builder.TransactionBinding != null)
                builder.TransactionBinding = builder.TransactionBinding.ToLowerInvariant();
            if (builder.TypeSystemVersion != null)
                builder.TypeSystemVersion = builder.TypeSystemVersion.ToLowerInvariant();
            if (builder.UserID != null)
                builder.UserID = builder.UserID.ToLowerInvariant();
            if (builder.WorkstationID != null)
                builder.WorkstationID = builder.WorkstationID.ToLowerInvariant();

            // ReSharper disable once AssignNullToNotNullAttribute
            ConnectionString = builder.ConnectionString;
        }

        /// <summary>
        ///   Gets the connection <see cref="string"/>.
        /// </summary>
        [NotNull]
        public readonly string ConnectionString;

        /// <summary>
        ///   Gets a <see cref="double"/> indicating the relative weight of the <see cref="Connection"/>.
        /// </summary>
        /// <value>
        ///   <para>The weight of the connection.</para>
        ///   <para>The default weighting is 1.0.</para>
        /// </value>
        public readonly double Weight;

        /// <summary>
        /// Returns a new connection with the <see cref="Weight"/> increased by <paramref name="weight"/>.
        /// </summary>
        /// <param name="weight">The weight.</param>
        /// <returns>A new connection.</returns>
        [NotNull]
        internal Connection AddWeight(double weight)
        {
            return weight.Equals(0D) ? this : new Connection(weight + Weight, ConnectionString);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Connection"/> to <see cref="System.String"/>.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns>The result of the conversion.</returns>
        [ContractAnnotation("connection:null =>null;connection:notnull =>notnull")]
        public static implicit operator string([CanBeNull] Connection connection)
        {
            return connection != null
                ? connection.ConnectionString
                : null;
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Connection" /> to <see cref="System.String" />.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>The result of the conversion.</returns>
        [ContractAnnotation("connectionString:null =>null;connectionString:notnull =>notnull")]
        public static explicit operator Connection([CanBeNull] string connectionString)
        {
            return connectionString != null
                ? new Connection(connectionString)
                : null;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Connection"/> to <see cref="LoadBalancedConnection"/>.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns>The result of the conversion.</returns>
        [ContractAnnotation("connection:null =>null;connection:notnull =>notnull")]
        public static implicit operator LoadBalancedConnection([CanBeNull] Connection connection)
        {
            return connection == null ? null : new LoadBalancedConnection(connection);
        }
        
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return ConnectionString;
        }

        /// <summary>
        /// Gets the schema.
        /// </summary>
        /// <param name="forceReload">if set to <see langword="true" /> forces the schema to be reloaded.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An awaitable task, containing the <see cref="DatabaseSchema"/> for this <see cref="Connection"/>.</returns>
        [NotNull]
        [PublicAPI]
        public Task<DatabaseSchema> GetSchema(bool forceReload, CancellationToken cancellationToken = default(CancellationToken))
        {
            return DatabaseSchema.GetOrAdd(this, false, cancellationToken);
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
            Connection other = obj as Connection;
            return !ReferenceEquals(other, null) &&
                   Weight.Equals(other.Weight) &&
                   string.Equals(ConnectionString, other.ConnectionString, StringComparison.InvariantCulture);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals([CanBeNull] Connection other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Weight.Equals(other.Weight) &&
                string.Equals(ConnectionString, other.ConnectionString, StringComparison.InvariantCulture);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (ConnectionString.GetHashCode() * 397) ^ Weight.GetHashCode();
            }
        }

        /// <summary>
        /// Implements the ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(Connection left, Connection right)
        {
            if (ReferenceEquals(left, null)) return ReferenceEquals(right, null);
            return !ReferenceEquals(right, null) &&
                   Equals(left.Weight, right.Weight) &&
                   string.Equals(left.ConnectionString, right.ConnectionString, StringComparison.InvariantCulture);
        }

        /// <summary>
        /// Implements the !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(Connection left, Connection right)
        {
            if (ReferenceEquals(left, null)) return !ReferenceEquals(right, null);
            return ReferenceEquals(right, null) ||
                   !Equals(left.Weight, right.Weight) ||
                   !string.Equals(left.ConnectionString, right.ConnectionString, StringComparison.InvariantCulture);
        }
        #endregion
    }
}