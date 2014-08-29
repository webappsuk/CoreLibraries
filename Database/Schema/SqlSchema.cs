using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Database.Schema
{
    /// <summary>
    /// Holds information about a SQL schema (e.g. 'dbo').
    /// </summary>
    public class SqlSchema : DatabaseEntity<SqlSchema>
    {
        /// <summary>
        /// The properties used for calculating differences.
        /// </summary>
        [UsedImplicitly]
        [NotNull]
        private static readonly Expression<Func<SqlSchema, object>>[] _properties =
            new Expression<Func<SqlSchema, object>>[]
            {
                s => s.ID
            };

        /// <summary>
        /// The identifier.
        /// </summary>
        [PublicAPI]
        public readonly int ID;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlSchema"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="fullName">The name.</param>
        // ReSharper disable once CodeAnnotationAnalyzer
        internal SqlSchema(int id, [NotNull]string fullName)
            : base(fullName)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(fullName));
            ID = id;
        }
    }
}