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
using System.Collections.Generic;
using System.Data.SqlClient;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Logging
{
    /// <summary>
    ///   A class used to retrieve Logs from a database.
    /// </summary>
    public sealed partial class Log
    {
        #region Nested type: LogRow
        internal class LogRow
        {
            [NotNull] public readonly LogContext Context;
            public readonly string ExceptionType;
            public readonly Guid ID;

            public readonly int Level;
            public readonly Guid LogGroup;

            public readonly string Message;
            public readonly Guid OperationGuid;
            public readonly Guid ServiceGuid;
            public readonly string StackTrace;

            public readonly int ThreadID;

            public readonly string ThreadName;

            public readonly DateTime TimeStamp;

            /// <summary>
            ///   Initializes a new instance of the <see cref="LogRow"/> class.
            /// </summary>
            /// <param name="dataReader">The SQL data reader.</param>
            private LogRow(SqlDataReader dataReader)
            {
                ID = dataReader.GetValue<Guid>("Guid");
                StackTrace = dataReader.GetValue<string>("StackTrace");
                ExceptionType = dataReader.GetValue<string>("ExceptionType");
                OperationGuid = dataReader.GetValue<Guid>("OperationGuid");
                TimeStamp = dataReader.GetValue<DateTime>("TimeStamp");
                ThreadName = dataReader.GetValue<string>("ThreadName");
                ThreadID = dataReader.GetValue<int>("ThreadId");
                Message = dataReader.GetValue<string>("Message");
                Level = dataReader.GetValue<int>("Level");
                LogGroup = dataReader.GetValue<Guid>("Group");
                ServiceGuid = dataReader.GetValue<Guid>("ServiceGuid");
                //TODO SET CONTEXT
                Context = new LogContext();
            }

            /// <summary>
            ///   Returns a list of <see cref="LogRow"/> elements using the provided <see cref="SqlDataReader"/>.
            /// </summary>
            /// <param name="dataReader">The SQL data reader.</param>
            /// <returns>
            ///   A list containing the <see cref="LogRow"/> elements read from the provided <paramref name="dataReader"/>.
            /// </returns>
            public static IEnumerable<LogRow> GetLogRows(SqlDataReader dataReader)
            {
                List<LogRow> rows = new List<LogRow>();

                while (dataReader.Read())
                {
                    try
                    {
                        LogRow row = new LogRow(dataReader);
                        rows.Add(row);
                    }
                        // ReSharper disable EmptyGeneralCatchClause
                    catch (Exception)
                        // ReSharper restore EmptyGeneralCatchClause
                    {
                        // Do nothing, ignore log item.
                    }
                }

                return rows;
            }
        }
        #endregion
    }
}