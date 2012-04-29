using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;

namespace WebApplications.Testing.Data
{
    /// <summary>
    /// Record that throws an exception.
    /// </summary>
    /// <remarks></remarks>
    public class ExceptionRecord : IObjectRecord
    {
        /// <summary>
        /// The exception that will be thrown when accessing this record.
        /// </summary>
        [NotNull]
        public readonly Exception Exception;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionRecord" /> class.
        /// </summary>
        /// <param name="exception">The exception that will be thrown when accessing this record.</param>
        /// <remarks></remarks>
        public ExceptionRecord([NotNull]Exception exception = null)
        {
            Contract.Requires(exception != null);
            Exception = exception;
        }

        /// <inheritdoc />
        public string GetName(int i)
        {
            throw Exception;
        }

        /// <inheritdoc />
        public string GetDataTypeName(int i)
        {
            throw Exception;
        }

        /// <inheritdoc />
        public Type GetFieldType(int i)
        {
            throw Exception;
        }

        /// <inheritdoc />
        public object GetValue(int i)
        {
            throw Exception;
        }

        /// <inheritdoc />
        public int GetValues(object[] values)
        {
            throw Exception;
        }

        /// <inheritdoc />
        public int GetOrdinal(string name)
        {
            throw Exception;
        }

        /// <inheritdoc />
        public bool GetBoolean(int i)
        {
            throw Exception;
        }

        /// <inheritdoc />
        public byte GetByte(int i)
        {
            throw Exception;
        }

        /// <inheritdoc />
        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw Exception;
        }

        /// <inheritdoc />
        public char GetChar(int i)
        {
            throw Exception;
        }

        /// <inheritdoc />
        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw Exception;
        }

        /// <inheritdoc />
        public Guid GetGuid(int i)
        {
            throw Exception;
        }

        /// <inheritdoc />
        public short GetInt16(int i)
        {
            throw Exception;
        }

        /// <inheritdoc />
        public int GetInt32(int i)
        {
            throw Exception;
        }

        /// <inheritdoc />
        public long GetInt64(int i)
        {
            throw Exception;
        }

        /// <inheritdoc />
        public float GetFloat(int i)
        {
            throw Exception;
        }

        /// <inheritdoc />
        public double GetDouble(int i)
        {
            throw Exception;
        }

        /// <inheritdoc />
        public string GetString(int i)
        {
            throw Exception;
        }

        /// <inheritdoc />
        public decimal GetDecimal(int i)
        {
            throw Exception;
        }

        /// <inheritdoc />
        public DateTime GetDateTime(int i)
        {
            throw Exception;
        }

        /// <inheritdoc />
        public IDataReader GetData(int i)
        {
            throw Exception;
        }

        /// <inheritdoc />
        public bool IsDBNull(int i)
        {
            throw Exception;
        }

        /// <inheritdoc />
        public int FieldCount
        {
            get { throw Exception; }
        }

        /// <inheritdoc />
        object IDataRecord.this[int i]
        {
            get { throw Exception; }
        }

        /// <inheritdoc />
        object IDataRecord.this[string name]
        {
            get { throw Exception; }
        }

        public RecordSetDefinition RecordSetDefinition
        {
            get { throw Exception; }
        }
    }
}
