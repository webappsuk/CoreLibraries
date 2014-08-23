using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Database
{
    /// <summary>
    ///   Holds information about a connection.
    /// </summary>
    public class Connection
    {
        /// <summary>
        ///   The <see cref="SqlConnectionStringBuilder">connection string builder</see>.
        /// </summary>
        [NotNull]
        private readonly SqlConnectionStringBuilder _connectionStringBuilder;

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
            // Coerce async
            _connectionStringBuilder = new SqlConnectionStringBuilder(connectionString)
            {
                AsynchronousProcessing = true
            };
            Weight = weight;
        }

        /// <summary>
        ///   Gets the connection <see cref="string"/>.
        /// </summary>
        [NotNull]
        public string ConnectionString
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            get { return _connectionStringBuilder.ConnectionString; }
        }

        /// <summary>
        ///   Gets a <see cref="double"/> indicating the relative weight of the <see cref="Connection"/>.
        /// </summary>
        /// <value>
        ///   <para>The weight of the connection.</para>
        ///   <para>The default weighting is 1.0.</para>
        /// </value>
        public double Weight { get; internal set; }

        /// <summary>
        /// Indicates whether the current instance is equal or equi to another <see cref="Connection" />.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>Returns <see langword="true" /> if the current <see cref="Connection" /> is equal to the
        /// <paramref name="other" /> specified; otherwise returns <see langword="false" />.</returns>
        public bool EquivalentTo([CanBeNull]Connection other)
        {
            return other != null && (_connectionStringBuilder.EquivalentTo(other._connectionStringBuilder));
        }
    }
}