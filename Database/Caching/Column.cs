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

using System.Data;

namespace WebApplications.Utilities.Database.Caching
{
    /// <summary>
    /// Holds a column definition.
    /// </summary>
    internal class Column
    {
        /// <summary>
        /// The ordinal.
        /// </summary>
        public readonly int Ordinal;

        /// <summary>
        /// The name.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// The SQL database type.
        /// </summary>
        public readonly SqlDbType SqlDbType;

        /// <summary>
        /// Whether the column can be null.
        /// </summary>
        public readonly bool AllowDBNull;

        /// <summary>
        /// Initializes a new instance of the <see cref="Column"/> class.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <param name="name">The name.</param>
        /// <param name="sqlDbType">Type of the SQL database.</param>
        /// <param name="allowDBNull">if set to <see langword="true" /> column is nullable.</param>
        public Column(int ordinal, string name, SqlDbType sqlDbType, bool allowDBNull)
        {
            Ordinal = ordinal;
            Name = name;
            SqlDbType = sqlDbType;
            AllowDBNull = allowDBNull;
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString() => $"{Name} [{SqlDbType}]";
    }
}