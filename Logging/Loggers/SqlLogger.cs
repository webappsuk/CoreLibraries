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
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.SqlServer.Server;
using WebApplications.Utilities.Logging.Configuration;

namespace WebApplications.Utilities.Logging.Loggers
{
    /// <summary>
    ///   Implements a logger that logs to a database.
    /// </summary>
    [UsedImplicitly]
    public class SqlLogger : LoggerBase
    {
        /// <summary>
        ///   Holds the SQL meta data for storing logs to a db.
        /// </summary>
        [NonSerialized] [NotNull] private static readonly SqlMetaData[] _tblLogEntry;

        /// <summary>
        ///   Holds the SQL meta data for storing operations to a db.
        /// </summary>
        [NonSerialized] [NotNull] private static readonly SqlMetaData[] _tblOperation;

        /// <summary>
        ///   Holds the SQL meta data for storing tuples to a db.
        /// </summary>
        [NonSerialized] [NotNull] private static readonly SqlMetaData[] _tblDictionary;

        /// <summary>
        ///   The database connection string.
        /// </summary>
        [NotNull] private readonly string _connectionString;

        /// <summary>
        ///   The last time a log was added.
        /// </summary>
        private DateTime _lastAddedOperation = DateTime.MinValue;

        /// <summary>
        ///   The time to wait (in seconds) for SQL commands.
        /// </summary>
        private readonly int _sqlTimeout;

        /// <summary>
        ///   The application Guid.
        /// </summary>
        private readonly Guid _applicationGuid;

        /// <summary>
        ///   How long logs should be left on the database.
        /// </summary>
        private readonly TimeSpan _expireAfter;

        static SqlLogger()
        {
            _tblLogEntry = new[]
                               {
                                   new SqlMetaData("Guid", SqlDbType.UniqueIdentifier),
                                   new SqlMetaData("Group", SqlDbType.UniqueIdentifier),
                                   new SqlMetaData("Level", SqlDbType.Int),
                                   new SqlMetaData("Message", SqlDbType.NVarChar, -1),
                                   new SqlMetaData("ThreadId", SqlDbType.Int),
                                   new SqlMetaData("ThreadName", SqlDbType.NVarChar, -1),
                                   new SqlMetaData("Expiry", SqlDbType.SmallDateTime),
                                   new SqlMetaData("OperationGuid", SqlDbType.UniqueIdentifier),
                                   new SqlMetaData("ExceptionType", SqlDbType.NVarChar, -1),
                                   new SqlMetaData("StackTrace", SqlDbType.NVarChar, -1)
                               };
            _tblOperation = new[]
                                {
                                    new SqlMetaData("GUID", SqlDbType.UniqueIdentifier),
                                    new SqlMetaData("ParentGUID", SqlDbType.UniqueIdentifier),
                                    new SqlMetaData("CategoryName", SqlDbType.NVarChar, -1),
                                    new SqlMetaData("Name", SqlDbType.NVarChar, -1),
                                    new SqlMetaData("InstanceHash", SqlDbType.Int),
                                    new SqlMetaData("Method", SqlDbType.NVarChar, -1),
                                    new SqlMetaData("ThreadId", SqlDbType.Int),
                                    new SqlMetaData("ThreadName", SqlDbType.NVarChar, -1),
                                };
            _tblDictionary = new[]
                                 {
                                     new SqlMetaData("GUID", SqlDbType.UniqueIdentifier),
                                     new SqlMetaData("Name", SqlDbType.NVarChar, 200),
                                     new SqlMetaData("Value", SqlDbType.NVarChar, -1)
                                 };
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="SqlLogger"/> class.
        /// </summary>
        /// <param name="name">The logger name.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="validLevels">
        ///   <para>The valid log levels.</para>
        ///   <para>By default this is allows <see cref="LogLevels">all log levels</see>.</para>
        /// </param>
        /// <param name="sqlTimeout">
        ///   <para>The time to wait (in seconds) for SQL commands.</para>
        ///   <para>By default this is set to 120 seconds.</para>
        /// </param>
        /// <param name="expireAfter">
        ///   <para>The amount of time to leave a log on the database.</para>
        ///   <para>By default this is set to 3 days.</para>
        /// </param>
        /// <exception cref="LoggingException">
        ///   <para><paramref name="connectionString"/> is <see cref="string.IsNullOrWhiteSpace">is null or whitespace</see>.</para>
        ///   <para>-or-</para>
        ///   <para><paramref name="sqlTimeout"/> was less than 1 second.</para>
        ///   <para>-or-</para>
        ///   <para><paramref name="expireAfter"/> cannot be less than 1 second.</para>
        ///   <para>-or-</para>
        ///   <para>The service failed to register.</para>
        /// </exception>
        public SqlLogger(
            [NotNull] string name,
            [NotNull] string connectionString = "",
            LogLevels validLevels = LogLevels.All,
            TimeSpan sqlTimeout = default(TimeSpan),
            TimeSpan expireAfter = default(TimeSpan))
            : base(name, false, validLevels) // TODO Re-enable CanRetrieve functionality.
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new LoggingException(Resources.SqlLogger_NoConnectionStringProvided,
                                           LogLevel.Critical);
            }

            _applicationGuid = LoggingConfiguration.Active.ApplicationGuid;
            string applicationName = LoggingConfiguration.Active.ApplicationName;

            _connectionString = connectionString;
            if (sqlTimeout == default(TimeSpan))
                _sqlTimeout = 120;
            else
            {
                _sqlTimeout = (int) Math.Ceiling(sqlTimeout.TotalSeconds);
                if (_sqlTimeout < 1)
                    throw new LoggingException(Resources.Sqllogger_TimeoutLessThanOneSecond,
                                               LogLevel.Critical, sqlTimeout);
            }

            if (expireAfter == default(TimeSpan))
                _expireAfter = TimeSpan.FromDays(3);
            else
            {
                _expireAfter = expireAfter;
                if (_expireAfter < TimeSpan.FromSeconds(1))
                    throw new LoggingException(Resources.Sqllogger_LogExpiryLessThanOneSecond,
                                               LogLevel.Critical);
            }

            // Register service & Check connection at this point
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    using (
                        SqlCommand command = new SqlCommand("spRegisterApplication", connection)
                                                 {
                                                     CommandType = CommandType.StoredProcedure,
                                                     CommandTimeout = _sqlTimeout
                                                 })
                    {
                        command.Parameters.AddWithValue("@ApplicationGuid", _applicationGuid);
                        command.Parameters.AddWithValue("@ApplicationName", applicationName);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new LoggingException(ex, Resources.SqlLogger_CouldNotRegisterService, LogLevel.Critical, Name);
            }
        }

        /// <summary>
        ///   Adds the specified log to the db in time order.
        /// </summary>
        /// <param name="log">The log to add.</param>
        public override void Add(Log log)
        {
            Add(new List<Log> {log});
        }

        /// <summary>
        ///   Adds the specified logs to the db in time order.
        /// </summary>
        /// <param name="logs">The logs to add.</param>
        public override void Add(IEnumerable<Log> logs)
        {
            if (!logs.Any())
                return;

            // Create a list of all operations
            List<Log> loglist = new List<Log>();
            List<Tuple<CombGuid, string, string>> context = new List<Tuple<CombGuid, string, string>>();
            List<Operation> operations = new List<Operation>();
            List<Tuple<CombGuid, string, string>> arguments = new List<Tuple<CombGuid, string, string>>();
            foreach (Log log in logs)
            {
                loglist.Add(log);

                // Build structure for context.
                context.AddRange(
                    log.Context.Select(
                        kvp => new Tuple<CombGuid, string, string>(log.Guid, kvp.Key, kvp.Value)));

                // Build structures for operations and their arguments
                Operation operation = log.Operation;
                while (operation != null)
                {
                    // Check we haven't seen the operation before
                    if (operations.Contains(operation))
                        break;
                    operations.Add(operation);

                    arguments.AddRange(
                        operation.Arguments.Select(
                            kvp => new Tuple<CombGuid, string, string>(operation.Guid, kvp.Key, kvp.Value)));

                    // If we're a newer operation than previously seen, update our latest operation
                    if (operation.Created >
                        _lastAddedOperation)
                        _lastAddedOperation = operation.Created;

                    // Select parent operation.
                    operation = operation.Parent;
                }
            }

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (
                    SqlCommand command = new SqlCommand("spLog", connection)
                                             {CommandType = CommandType.StoredProcedure, CommandTimeout = _sqlTimeout})
                {
                    command.Parameters.AddWithValue("@ApplicationGuid", _applicationGuid);

                    command.Parameters.Add(
                        new SqlParameter("@LogEntry", SqlDbType.Structured)
                            {Value = loglist.Any() ? loglist.Select(GetLogData) : null});
                    command.Parameters.Add(
                        new SqlParameter("@Context", SqlDbType.Structured)
                            {Value = context.Any() ? context.Select(GetTupleData) : null});
                    command.Parameters.Add(
                        new SqlParameter("@Operation", SqlDbType.Structured)
                            {Value = operations.Any() ? operations.Select(GetOperationData) : null});
                    command.Parameters.Add(
                        new SqlParameter("@Argument", SqlDbType.Structured)
                            {Value = arguments.Any() ? arguments.Select(GetTupleData) : null});

                    command.ExecuteNonQuery();
                }
            }
        }


        /// <summary>
        ///   Gets a <see cref="SqlDataRecord"/> from a <see cref="Log"/>.
        /// </summary>
        /// <param name="log">The log to get the SQL data record from.</param>
        /// <returns>The converted <see cref="Log">log</see> as a <see cref="SqlDataRecord"/> object.</returns>
        private SqlDataRecord GetLogData([NotNull] Log log)
        {
            // Create new record
            SqlDataRecord sqlDataRecord = new SqlDataRecord(_tblLogEntry);
            sqlDataRecord.SetGuid(0, log.Guid.Guid);
            sqlDataRecord.SetGuid(1, log.Group.Guid);
            sqlDataRecord.SetInt32(2, (int) log.Level);
            sqlDataRecord.SetString(3, log.Message);
            sqlDataRecord.SetInt32(4, log.ThreadId);
            sqlDataRecord.SetString(5, log.ThreadName);
            sqlDataRecord.SetDateTime(6, log.TimeStamp + _expireAfter);
            sqlDataRecord.Set(7,
                              log.Operation != null && log.Operation.Guid != CombGuid.Empty
                                  ? log.Operation.Guid.Guid
                                  : (Guid?) null);
            sqlDataRecord.Set(8, log.ExceptionType);
            sqlDataRecord.Set(9, log.StackTrace);
            return sqlDataRecord;
        }

        /// <summary>
        ///   Gets a <see cref="SqlDataRecord"/> from an <see cref="Operation"/>.
        /// </summary>
        /// <param name="operation">The operation to get the SQL data record from.</param>
        /// <returns>
        ///   The converted <see cref="Operation">operation</see> as a <see cref="SqlDataRecord"/> object.
        /// </returns>
        private static SqlDataRecord GetOperationData([NotNull] Operation operation)
        {
            // Create new record
            SqlDataRecord sqlDataRecord = new SqlDataRecord(_tblOperation);
            sqlDataRecord.SetGuid(0, operation.Guid.Guid);
            if (operation.Parent != null)
                sqlDataRecord.SetGuid(1, operation.Parent.Guid.Guid);
            else
                sqlDataRecord.SetDBNull(1);
            sqlDataRecord.SetString(2, operation.CategoryName);
            sqlDataRecord.SetString(3, operation.Name);
            sqlDataRecord.Set(4, operation.InstanceHash);
            sqlDataRecord.SetString(5, operation.Method);
            sqlDataRecord.SetInt32(6, operation.ThreadId);
            sqlDataRecord.SetString(7, operation.ThreadName);
            return sqlDataRecord;
        }

        /// <summary>
        ///   Gets a <see cref="SqlDataRecord"/> from a <see cref="Tuple{T1, T2, T3}">Tuple&lt;Guid,string,string&gt;</see>.
        /// </summary>
        /// <param name="tuple">The tuple to get the SQL data record from.</param>
        /// <returns>The converted tuple as a <see cref="SqlDataRecord"/> object.</returns>
        private static SqlDataRecord GetTupleData([NotNull] Tuple<CombGuid, string, string> tuple)
        {
            // Create new record
            SqlDataRecord sqlDataRecord = new SqlDataRecord(_tblDictionary);
            sqlDataRecord.SetGuid(0, tuple.Item1.Guid);
            sqlDataRecord.SetString(1,
                                    tuple.Item2 == null
                                        ? string.Empty
                                        : tuple.Item2.Length < 200 ? tuple.Item2 : tuple.Item2.Substring(0, 200));
            sqlDataRecord.Set(2, tuple.Item3);
            return sqlDataRecord;
        }

#if false

    /// <summary>
    ///   Gets all of the logs from the end date backwards up to the start date.
    /// </summary>
    /// <param name="endDate">The last date to get logs from (exclusive).</param>
    /// <param name="startDate">The start date to get logs up to (inclusive).</param>
    /// <returns>
    ///   The retrieved logs in reverse date order, the first being the newest log before the <paramref name="endDate"/>
    ///   and the last being the first log after the <paramref name="startDate"/>.
    /// </returns>
        public override IEnumerable<Log> Get(DateTime endDate, DateTime startDate)
        {
            return GetLogsFromDatabase(startDate, endDate, int.MinValue);
        }

        /// <summary>
        ///   Gets all of the logs from the end date backwards up to the specified limit.
        /// </summary>
        /// <param name="endDate">The last date to get logs up to (exclusive).</param>
        /// <param name="limit">The maximum number of logs to retrieve.</param>
        /// <returns>
        ///   The retrieved logs in reverse date order, the first being the newest log before the <paramref name="endDate"/>.
        /// </returns>
        public override IEnumerable<Log> Get(DateTime endDate, int limit)
        {
            return GetLogsFromDatabase(DateTime.MinValue, endDate, limit);
        }

        /// <summary>
        ///   Gets all of the logs from the start date forwards up to the specified limit.
        /// </summary>
        /// <param name="startDate">The start date to get logs from (inclusive).</param>
        /// <param name="limit">The maximum number of logs to retrieve.</param>
        /// <returns>
        ///   The retrieved logs starting from the <paramref name="startDate"/> up to the specified <paramref name="limit"/>.
        /// </returns>
        public override IEnumerable<Log> GetForward(DateTime startDate, int limit)
        {
            return GetLogsFromDatabase(startDate, DateTime.MinValue, limit);
        }

        /// <summary>
        ///   Gets the logs from database.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="limit">The maximum number of logs to retrieve.</param>
        /// <returns>The retrieved logs. If no logs are found returns an empty list of log elements.</returns>
        private IEnumerable<Log> GetLogsFromDatabase(DateTime startDate, DateTime endDate, int limit)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (
                    SqlCommand command = new SqlCommand(_getStoredProcedure, connection)
                                             {CommandType = CommandType.StoredProcedure})
                {
                    try
                    {
                        command.Parameters.AddWithValue("@sqlLogGroupGuid", SqlLogGroupGuid);
                        if (startDate > DateTime.MinValue)
                            command.Parameters.AddWithValue("@StartDate", startDate);
                        if (endDate > DateTime.MinValue)
                            command.Parameters.AddWithValue("@EndDate", endDate);
                        if (limit > int.MinValue)
                            command.Parameters.AddWithValue("@Limit", limit);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            IEnumerable<Log.LogRow> logItems = Log.LogRow.GetLogRows(reader);

                            // No log items, so just return.
                            if (logItems.Count() < 1)
                                return new List<Log>(0);

                            // Next Result set (Operations)
                            reader.NextResult();
                            Dictionary<Guid, Operation.OperationRow> operationItems =
                                Operation.OperationRow.GetOperationRows(reader);

                            // Next Result set (OperationArguments)
                            reader.NextResult();
                            ILookup<Guid, OperationArgument.OperationArgumentRow> operationArgumentItems =
                                OperationArgument.OperationArgumentRow.GetOperationArgumentRows(reader);

                            // Now we have log, operation and argument objects, we need to link them together.
                            return Log.GetLogs(logItems, operationItems, operationArgumentItems);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new LoggingException(ex, "Could not retrieve log items.", LogLevel.Error);
                    }
                }
            }
        }

        /// <summary>
        ///   Gets the logger with the specified <see cref="Guid"/> from the specified database.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="sqlLogGroupGuid">The SQL log group Guid.</param>
        /// <returns>The logger retrieved.</returns>
        public static SqlLogger GetLogger(string connectionString, Guid sqlLogGroupGuid)
        {
            throw new NotImplementedException();
        }

        // TODO - This could be dangerous, difficult to secure...
        /// <summary>
        ///   Gets all of the loggers found in the specified database
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>The loggers that were retrieved from the db.</returns>
        public static IEnumerable<SqlLogger> GetLoggers(string connectionString)
        {
            throw new NotImplementedException();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (
                    SqlCommand command = new SqlCommand("spLoggersGet", connection)
                                             {CommandType = CommandType.StoredProcedure})
                {
                    try
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new LoggingException(ex, "Could not retrieve loggers.", LogLevel.Error);
                    }
                }
            }
        }
#endif
    }
}