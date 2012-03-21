#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities.Logging 
// Project: WebApplications.Utilities.Logging
// File: LogRow.cs
// 
// This software, its object code and source code and all modifications made to
// the same (the “Software”) are, and shall at all times remain, the proprietary
// information and intellectual property rights of Web Applications (UK) Limited. 
// You are only entitled to use the Software as expressly permitted by Web
// Applications (UK) Limited within the Software Customisation and
// Licence Agreement (the “Agreement”).  Any copying, modification, decompiling,
// distribution, licensing, sale, transfer or other use of the Software other than
// as expressly permitted in the Agreement is expressly forbidden.  Web
// Applications (UK) Limited reserves its rights to take action against you and
// your employer in accordance with its contractual and common law rights
// (including injunctive relief) should you breach the terms of the Agreement or
// otherwise infringe its copyright or other intellectual property rights in the
// Software.
// 
// © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
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