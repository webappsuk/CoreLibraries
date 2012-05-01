#region © Copyright Web Applications (UK) Ltd, 2012.  All rights reserved.
// Copyright (c) 2012, Web Applications UK Ltd
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
using System.Data;
using System.Diagnostics.Contracts;
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
        [NotNull] public readonly Exception Exception;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionRecord" /> class.
        /// </summary>
        /// <param name="exception">The exception that will be thrown when accessing this record.</param>
        /// <remarks></remarks>
        public ExceptionRecord([NotNull] Exception exception = null)
        {
            Contract.Requires(exception != null);
            Exception = exception;
        }

        #region IObjectRecord Members
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

        /// <inheritdoc />
        public RecordSetDefinition RecordSetDefinition
        {
            get { return RecordSetDefinition.ExceptionRecord; }
        }
        #endregion
    }
}