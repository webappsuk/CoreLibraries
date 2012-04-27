using System;
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
        /// Gets the field count (number of columns).
        /// </summary>
        /// <value>The field count.</value>
        /// <remarks></remarks>
        public int FieldCount { get { return ColumnsArray.Length; }}

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordSetDefinition" /> class.
        /// </summary>
        /// <param name="columnDefinitions">The column definitions.</param>
        /// <remarks></remarks>
        public RecordSetDefinition([NotNull]IEnumerable<ColumnDefinition> columnDefinitions)
        {
            ColumnsArray = columnDefinitions.ToArray();

            if (ColumnsArray.Length < 1)
                throw new ArgumentOutOfRangeException("columnDefinitions", columnDefinitions, "The column definitions must have at least one column.");

            for (int c = 0; c < ColumnsArray.Length; c++)
            {
                ColumnDefinition columnDefinition = ColumnsArray[c];
                if (columnDefinition == null)
                    throw new ArgumentOutOfRangeException("columnDefinitions", columnDefinitions,
                                                          string.Format(
                                                              "The column definition at index '{0} must not be null.", c));

                if (columnDefinition.RecordSetDefinition != null)
                    throw new InvalidOperationException("The column definition cannot be added to the recordset definition as it already belongs to a different record set definition.");

                // ReSharper disable HeuristicUnreachableCode
                columnDefinition.RecordSetDefinition = this;
                columnDefinition.Ordinal = c;
                // ReSharper restore HeuristicUnreachableCode
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordSetDefinition" /> class.
        /// </summary>
        /// <param name="columnDefinitions">The column definitions.</param>
        /// <remarks></remarks>
        public RecordSetDefinition([NotNull]params ColumnDefinition[] columnDefinitions)
            : this((IEnumerable<ColumnDefinition>) columnDefinitions)
        {
        }
    }
}