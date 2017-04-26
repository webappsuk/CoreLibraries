#region © Copyright Web Applications (UK) Ltd, 2017.  All rights reserved.
// Copyright (c) 2017, Web Applications UK Ltd
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
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Database.Schema;
using WebApplications.Utilities.Threading;

namespace WebApplications.Utilities.Database
{
    /// <summary>
    ///   Holds information about a connection.
    /// </summary>
    [PublicAPI]
    public class Connection : IEquatable<Connection>
    {
        /// <summary>
        /// Normalizes the connection string.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns></returns>
        [NotNull]
        public static string NormalizeConnectionString(string connectionString)
        {
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

            Debug.Assert(builder.ConnectionString != null);
            return builder.ConnectionString;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Connection" /> class.  Used by <see cref="AddWeight" /> to avoid re-evaluating connection string.
        /// </summary>
        /// <param name="weight">The weight.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="semaphore">The semaphore.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="connectionString"/> was null.</exception>
        private Connection(double weight, [NotNull] string connectionString, [CanBeNull] AsyncSemaphore semaphore)
        {
            Weight = weight;
            ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            Semaphore = semaphore;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Connection" /> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="weight"><para>The weighting of the connection.</para>
        /// <para>By default this is set to 1.0.</para></param>
        /// <param name="maxConcurrency">The the maximum number of concurrent program executions for the connection.</param>
        /// <exception cref="System.ArgumentNullException">connectionString</exception>
        public Connection([NotNull] string connectionString, double weight = 1.0D, int maxConcurrency = -1)
        {
            if (connectionString == null) throw new ArgumentNullException(nameof(connectionString));

            Weight = weight;
            // ReSharper disable once AssignNullToNotNullAttribute
            ConnectionString = NormalizeConnectionString(connectionString);
            if (maxConcurrency > 0) Semaphore = new AsyncSemaphore(maxConcurrency);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Connection"/> class.
        /// </summary>
        /// <param name="databaseId">The database identifier.</param>
        /// <param name="lbConnectionId">The lb connection identifier.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="weight">The weight.</param>
        /// <exception cref="System.ArgumentNullException">
        /// </exception>
        internal Connection(
            [NotNull] string databaseId,
            [NotNull] string lbConnectionId,
            [NotNull] string connectionString,
            double weight)
            : this(connectionString, weight)
        {
            if (databaseId == null) throw new ArgumentNullException(nameof(databaseId));
            if (lbConnectionId == null) throw new ArgumentNullException(nameof(lbConnectionId));
            Semaphore = ConcurrencyController.GetConnectionSemaphore(databaseId, lbConnectionId, this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Connection" /> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="weight"><para>The weighting of the connection.</para>
        /// <para>By default this is set to 1.0.</para></param>
        /// <param name="semaphore">The semaphore.</param>
        /// <exception cref="System.ArgumentNullException">connectionString</exception>
        public Connection([NotNull] string connectionString, double weight, AsyncSemaphore semaphore)
        {
            if (connectionString == null) throw new ArgumentNullException(nameof(connectionString));

            Weight = weight;
            // ReSharper disable once AssignNullToNotNullAttribute
            ConnectionString = NormalizeConnectionString(connectionString);
            Semaphore = semaphore;
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
        /// The semaphore for controlling the maximum number of concurrent program executions for the connection.
        /// </summary>
        [CanBeNull]
        internal readonly AsyncSemaphore Semaphore;

        /// <summary>
        /// Returns a new connection with the <see cref="Weight"/> increased by <paramref name="weight"/>.
        /// </summary>
        /// <param name="weight">The weight.</param>
        /// <returns>A new connection.</returns>
        [NotNull]
        internal Connection AddWeight(double weight)
        {
            return weight.Equals(0D) ? this : new Connection(weight + Weight, ConnectionString, Semaphore);
        }

        /// <summary>
        /// Returns a new connection with the <see cref="Weight"/> set to <paramref name="weight"/>.
        /// </summary>
        /// <param name="weight">The weight.</param>
        /// <returns>A new connection.</returns>
        [NotNull]
        internal Connection WithWeight(double weight)
        {
            return weight.Equals(Weight) ? this : new Connection(weight, ConnectionString, Semaphore);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Connection"/> to <see cref="System.String"/>.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns>The result of the conversion.</returns>
        [ContractAnnotation("connection:null =>null;connection:notnull =>notnull")]
        public static implicit operator string([CanBeNull] Connection connection) => connection?.ConnectionString;

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
        public override string ToString() => ConnectionString;

        /// <summary>
        /// Gets the schema.
        /// </summary>
        /// <param name="forceReload">if set to <see langword="true" /> forces the schema to be reloaded.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An awaitable task, containing the <see cref="DatabaseSchema"/> for this <see cref="Connection"/>.</returns>
        [NotNull]
        public Task<DatabaseSchema> GetSchema(
            bool forceReload,
            CancellationToken cancellationToken = default(CancellationToken))
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
            return Equals(other);
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
                   string.Equals(ConnectionString, other.ConnectionString, StringComparison.Ordinal) &&
                   Equals(Semaphore, other.Semaphore);
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
            return left.Equals(right);
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
            return !left.Equals(right);
        }
        #endregion
    }
}