using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace WebApplications.Testing.Data
{
    /// <summary>
    /// Holds a collection of records.
    /// </summary>
    /// <remarks></remarks>
    public class ObjectSet : IObjectSet, ICollection<IObjectRecord>
    {
        /// <summary>
        /// The underlying list of records.
        /// </summary>
        [NotNull]
        private readonly List<IObjectRecord> _records = new List<IObjectRecord>();

        /// <summary>
        /// Holds the <see cref="RecordSetDefinition"/>.
        /// </summary>
        [NotNull]
        private readonly RecordSetDefinition _definition;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectSet" /> class.
        /// </summary>
        /// <param name="recordSetDefinition">The record set definition.</param>
        /// <param name="records">The records.</param>
        /// <remarks></remarks>
        public ObjectSet([NotNull]RecordSetDefinition recordSetDefinition, IEnumerable<IObjectRecord> records = null)
        {
            Contract.Requires(recordSetDefinition != null);
            _definition = recordSetDefinition;

            if (records == null)
                return;

            foreach (IObjectRecord record in records)
                Add(record);
        }

        /// <inheritdoc/>
        public RecordSetDefinition Definition { get { return _definition; } }

        /// <inheritdoc/>
        public IEnumerator<IObjectRecord> GetEnumerator()
        {
            return _records.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc/>
        public void Add(IObjectRecord item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (item.RecordSetDefinition != _definition)
                throw new ArgumentException(
                    "The record must have an identical recordset definition to be added to the current record.", "item");

            _records.Add(item);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            _records.Clear();
        }

        /// <inheritdoc/>
        public bool Contains(IObjectRecord item)
        {
            return _records.Contains(item);
        }

        /// <inheritdoc/>
        public void CopyTo(IObjectRecord[] array, int arrayIndex)
        {
            _records.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc/>
        public bool Remove(IObjectRecord item)
        {
            return _records.Remove(item);
        }

        /// <inheritdoc/>
        public int Count
        {
            get { return _records.Count; }
        }

        /// <inheritdoc/>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return _records.ToString();
        }
    }
}
