using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace WebApplications.Testing.Data
{
    /// <summary>
    /// Defines a record set.
    /// </summary>
    /// <remarks></remarks>
    public class RecordSetDefinition
    {
        /// <summary>
        /// Gets the column definitions array.
        /// </summary>
        /// <value>The column definitions array.</value>
        /// <remarks></remarks>
        [NotNull]
        internal ColumnDefinition[] ColumnsArray { get; private set; }

         /// <summary>
        /// Gets the column definitions in the record set.
        /// </summary>
        /// <value>The column definitions.</value>
        /// <remarks></remarks>
        [NotNull]
        public IEnumerable<ColumnDefinition> Columns { get { return ColumnsArray; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordSetDefinition" /> class.
        /// </summary>
        /// <param name="columnDefinitions">The column definitions.</param>
        /// <remarks></remarks>
        public RecordSetDefinition([NotNull]IEnumerable<ColumnDefinition> columnDefinitions)
        {
            ColumnsArray = columnDefinitions.ToArray();
        }
    }
}