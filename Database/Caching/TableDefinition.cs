#region © Copyright Web Applications (UK) Ltd, 2016.  All rights reserved.
// Copyright (c) 2016, Web Applications UK Ltd
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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.Linq;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Database.Exceptions;
using WebApplications.Utilities.Serialization;

namespace WebApplications.Utilities.Database.Caching
{
    /// <summary>
    /// Holds a table definition.
    /// </summary>
    [PublicAPI]
    public class TableDefinition : IReadOnlyCollection<Column>
    {
        /// <summary>
        /// The <see cref="TableDefinition"/> indicating the end of stream.
        /// </summary>
        [NotNull]
        public static readonly TableDefinition End = new TableDefinition();

        /// <summary>
        /// The empty <see cref="TableDefinition"/>.
        /// </summary>
        [NotNull]
        public static readonly TableDefinition Empty = new TableDefinition();

        /// <summary>
        /// The empty columns dictionary.
        /// </summary>
        [NotNull]
        private static readonly IReadOnlyDictionary<string, Column> _emptyColumnsDictionary
            = new Dictionary<string, Column>(0);

        /// <summary>
        /// Whether the table contains rows.
        /// </summary>
        private readonly bool _hasRows;

        /// <summary>
        /// The columns by name (case-insensitive).
        /// </summary>
        [NotNull, ItemNotNull]
        private readonly IReadOnlyDictionary<string, Column> _columnsByName;

        /// <summary>
        /// The columns in ordinal order.
        /// </summary>
        [NotNull, ItemNotNull]
        private readonly Column[] _columns;

        /// <summary>
        /// The number of flag bits.
        /// </summary>
        private readonly int _flagBits;

        /// <summary>
        /// The number of flag bytes.
        /// </summary>
        private readonly int _flagBytes;

        /// <summary>
        /// Creates a default, invalid instance of the <see cref="TableDefinition"/>.
        /// </summary>
        private TableDefinition()
        {
            _hasRows = false;
            _columns = Array<Column>.Empty;
            _flagBits = 0;
            _flagBytes = 0;
            _columnsByName = _emptyColumnsDictionary;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableDefinition" /> class.
        /// </summary>
        /// <param name="columns">The columns.</param>
        /// <param name="hasRows">if set to <see langword="true" /> [has rows].</param>
        private TableDefinition([NotNull] Column[] columns, bool hasRows)
        {
            Debug.Assert(columns.Length > 0);
            _columns = columns;
            _hasRows = hasRows;
            _flagBits = columns.Where(c => c != null).Sum(c => (c.AllowDBNull ? 1 : 0) + (c.IsBit ? 1 : 0));
            _flagBytes = 1 + ((_flagBits - 1) >> 3);
            _columnsByName = columns.Where(c => c?.Name != null).ToDictionary(c => c.Name);
        }

        /// <summary>
        /// Reads a <see cref="TableDefinition" /> from the <paramref name="dataReader">specified data reader</paramref>.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <returns>A <see cref="TableDefinition" />.</returns>
        /// <exception cref="SqlCachingException">Invalid structure.</exception>
        [NotNull]
        internal static TableDefinition Read([NotNull] DbDataReader dataReader)
        {
            // Get the field count.
            int fieldCount = dataReader.FieldCount;

            // Empty result (no fields!)
            if (fieldCount == 0) return Empty;
            
            // Create the column arrays and lookup dictionary.
            Column[] columns = new Column[fieldCount];

            // Get the schema so we can store information about the columns
            using (DataTable table = dataReader.GetSchemaTable())
            {
                // Validate we have the correct number of columns.
                if (table?.Rows?.Count != fieldCount)
                    throw new SqlCachingException(() => Resources.TableDefinition_Invalid_Column_Info);

                // Find ordinals for key columns
                int ordinalIndex = table.Columns.IndexOf("ColumnOrdinal");
                int nameIndex = table.Columns.IndexOf("ColumnName");
                int providerTypeIndex = table.Columns.IndexOf("ProviderType");
                int allowDBNullIndex = table.Columns.IndexOf("AllowDBNull");

                // Validate we have found the required column information.
                if (nameIndex < 0 || providerTypeIndex < 0 || allowDBNullIndex < 0 || ordinalIndex < 0)
                    throw new SqlCachingException(() => Resources.TableDefinition_Invalid_Column_Info);
                
                // Convert rows to column and add into structures.
                foreach (Column column in table.Rows
                    .Cast<DataRow>()
                    .Select(r => Column.Read(r, ordinalIndex, nameIndex, providerTypeIndex, allowDBNullIndex)))
                    // Insert column into array
                    // ReSharper disable once PossibleNullReferenceException
                    columns[column.Ordinal] = column;
            }
            
            // Create table definition and return.
            return new TableDefinition(columns, dataReader.HasRows);
        }

        /// <summary>
        /// Reads a <see cref="TableDefinition" /> from the <paramref name="stream">specified stream</paramref>.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>A <see cref="TableDefinition" />.</returns>
        [NotNull]
        internal static TableDefinition Read([NotNull] Stream stream)
        {
            uint flags = VariableLengthEncoding.DecodeUInt(stream);

            // Flag is set if we have rows (and columns) or if we're the end.
            bool flag = (flags & 1) == 1;
            int count = (int)(flags / 2);

            if (count < 1)
                return flag ? End : Empty;

            // Read columns
            Column[] columns = new Column[count];
            for (int o = 0; o < count; o++)
                columns[o] = Column.Read(stream);

            // Return new table definition.
            return new TableDefinition(columns, flag);
        }


        /// <summary>
        /// Serializes this instance to the <paramref name="stream">specified stream</paramref>.
        /// </summary>
        /// <param name="stream">The stream.</param>
        internal void Serialize([NotNull] Stream stream)
        {
            // Encode number of columns, and flags for has rows or end indicator.
            int count = _columns.Length;
            VariableLengthEncoding.Encode(
                (uint)(count * 2 + (_hasRows || ReferenceEquals(this, End) ? 1 : 0)),
                stream);
            if (count < 1) return;

            // ReSharper disable once PossibleNullReferenceException
            foreach (Column column in _columns)
                column.Serialize(stream);
        }

        /// <summary>
        /// Whether the table contains rows.
        /// </summary>
        public bool HasRows
        {
            get
            {
                if (_columns.Length < 1)
                    throw new InvalidOperationException("Invalid attempt to read when no data is present.");
                return _hasRows;
            }
        }
        /// <summary>
        /// Gets a value indicating whether this is the end of the data.
        /// </summary>
        /// <value><see langword="true" /> if this instance is end; otherwise, <see langword="false" />.</value>
        public bool IsEnd => ReferenceEquals(this, End);

        /// <summary>
        /// Gets the <see cref="Column"/> with the specified ordinal.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>Column.</returns>
        /// <exception cref="InvalidOperationException">Invalid attempt to read when no data is present.</exception>
        [NotNull]
        public Column this[int ordinal]
        {
            get
            {
                if (_columns.Length < 1)
                    throw new InvalidOperationException("Invalid attempt to read when no data is present.");
                // ReSharper disable once AssignNullToNotNullAttribute
                return _columns[ordinal];
            }
        }

        /// <summary>
        /// Gets the <see cref="Column"/> with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Column.</returns>
        /// <exception cref="InvalidOperationException">Invalid attempt to read when no data is present.</exception>
        [NotNull]
        public Column this[string name]
        {
            get
            {
                if (_columns.Length < 1)
                    throw new InvalidOperationException("Invalid attempt to read when no data is present.");
                // ReSharper disable once AssignNullToNotNullAttribute
                return _columnsByName[name];
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        /// <exception cref="InvalidOperationException">Invalid attempt to read when no data is present.</exception>
        public IEnumerator<Column> GetEnumerator() => ((IEnumerable<Column>)_columns).GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        /// <value>The count.</value>
        /// <exception cref="System.InvalidOperationException">Invalid attempt to read when no data is present.</exception>
        public int Count => _columns.Length;

        /// <summary>
        /// Serializes the row.
        /// </summary>
        /// <param name="sqlValues">The SQL values.</param>
        /// <returns>System.Byte[].</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        internal byte[] SerializeRow([NotNull] object[] sqlValues)
        {
            int length = _columns.Length;
            if (length < 1)
                throw new SqlCachingException(() => "Cannot serialize row when no columns defined.");

            if (length < sqlValues.Length)
                throw new SqlCachingException(() => "Not enough values provided to serialize row.");
            
            List<bool> bits = new List<bool>(_flagBits);

            // Write data to a byte[]
            using (MemoryStream memoryStream = new MemoryStream())
            {
                // Skip flag bytes
                memoryStream.Seek(_flagBytes, SeekOrigin.Begin);

                foreach (Column column in _columns)
                {
                    object sqlValue = sqlValues[column.Ordinal];
                    bool isNull = sqlValue.IsNull();
                    if (column.AllowDBNull)
                        bits.Add(isNull);
                    else if (isNull)
                        throw new SqlCachingException(
                            () => "Null value supplied for column {0}, which does not accept nulls.",
                            column.Ordinal);

                    if (isNull)
                    {
                        // Always write out bits to flags.
                        if (column.IsBit)
                            bits.Add(false);
                        continue;
                    }

                    // Serialize value to stream.
                    // ReSharper disable once AssignNullToNotNullAttribute
                    column.SqlDbTypeInfo.Serialize(memoryStream, sqlValue);
                }

                // Write flag bytes to start of stream.
                memoryStream.Seek(0, SeekOrigin.Begin);
                memoryStream.WriteBits(bits);

                // Return the serialization buffer
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Deserializes the row to it's SQL values.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>System.Object[].</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        internal object[] DeserializeSqlValues([NotNull] byte[] data)
        {
            int length = _columns.Length;
            if (length < 1)
                throw new SqlCachingException(() => "Cannot de-serialize row when no columns.");

            // Read flags and update offset.
            bool[] flags = data.ReadBits(_flagBits);
            int f = 0;
            long offset = 1 + ((length - 1) >> 3);

            object[] sqlValues = new object[length];
            foreach (Column column in _columns)
            {
                bool isNull = column.AllowDBNull && flags[f++];
                object value;
                if (column.IsBit)
                {
                    value = isNull ? SqlBoolean.Null : new SqlBoolean(flags[f]);
                    f++;
                }
                else
                    value = column.SqlDbTypeInfo.DeserializeSqlValue(data, ref offset);
                sqlValues[column.Ordinal] = value;
            }
            return sqlValues;
        }

        /// <summary>
        /// Deserializes the row to it's values.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>System.Object[].</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        internal object[] DeserializeValues([NotNull] byte[] data)
        {
            int length = _columns.Length;
            if (length < 1)
                throw new SqlCachingException(() => "Cannot de-serialize row when no columns.");

            // Read flags and update offset.
            bool[] flags = data.ReadBits(_flagBits);
            int f = 0;
            long offset = 1 + ((length - 1) >> 3);

            object[] sqlValues = new object[length];
            foreach (Column column in _columns)
            {
                bool isNull = column.AllowDBNull && flags[f++];
                object value;
                if (column.IsBit)
                {
                    value = isNull ? null : (object)flags[f];
                    f++;
                }
                else
                    value = column.SqlDbTypeInfo.DeserializeValue(data, ref offset);
                sqlValues[column.Ordinal] = value;
            }
            return sqlValues;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
            => IsEnd
                ? "Table End"
                : $"Table [{Count} Columns]{Environment.NewLine}\t{string.Join(Environment.NewLine + '\t', this)}";
    }
}