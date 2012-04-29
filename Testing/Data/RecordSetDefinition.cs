using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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
        [NotNull] private readonly ColumnDefinition[] _columnsArray;

        /// <summary>
        /// Gets the column definitions in the record set.
        /// </summary>
        /// <value>The column definitions.</value>
        /// <remarks></remarks>
        [NotNull]
        public IEnumerable<ColumnDefinition> Columns { get { return _columnsArray; } }

        /// <summary>
        /// Gets the field count (number of columns).
        /// </summary>
        /// <value>The field count.</value>
        /// <remarks></remarks>
        public int FieldCount { get { return _columnsArray.Length; }}

        /// <summary>
        /// Gets the <see cref="ColumnDefinition" /> at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The <see cref="ColumnDefinition"/>.</returns>
        /// <remarks></remarks>
        [NotNull]
        public ColumnDefinition this[int index]
        {
            get { return _columnsArray[index]; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordSetDefinition" /> class.
        /// </summary>
        /// <param name="columnDefinitions">The column definitions.</param>
        /// <remarks></remarks>
        public RecordSetDefinition([NotNull]IEnumerable<ColumnDefinition> columnDefinitions)
        {
            _columnsArray = columnDefinitions.ToArray();

            if (_columnsArray.Length < 1)
                throw new ArgumentOutOfRangeException("columnDefinitions", columnDefinitions, "The column definitions must have at least one column.");

            for (int c = 0; c < _columnsArray.Length; c++)
            {
                ColumnDefinition columnDefinition = _columnsArray[c];
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


        /// <summary>
        /// Gets the ordinal.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        /// <exception cref="System.IndexOutOfRangeException"></exception>
        /// <remarks></remarks>
        public int GetOrdinal(string name)
        {
            CompareInfo compare = CultureInfo.InvariantCulture.CompareInfo;
            for (int c = 0; c < FieldCount; c++)
            {
                if (compare.Compare(_columnsArray[c].Name, name, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth) == 0)
                    return c;
            }
            throw new IndexOutOfRangeException();
        }
    }
}