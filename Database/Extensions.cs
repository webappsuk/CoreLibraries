#region © Copyright Web Applications (UK) Ltd, 2015.  All rights reserved.
// Copyright (c) 2015, Web Applications UK Ltd
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
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Database.Schema;
using WebApplications.Utilities.Logging;
using WebApplications.Utilities.Reflect;
using WebApplications.Utilities.Serialization;

namespace WebApplications.Utilities.Database
{
    /// <summary>
    ///   Extension methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Gets a serialized object from a data reader by column name.
        /// </summary>
        /// <param name="reader">The data reader object.</param>
        /// <param name="column">The name of the column to get.</param>
        /// <param name="nullValue">The value to use if the data is null.</param>
        /// <param name="context">The de-serialization context.</param>
        /// <param name="contextState">The de-serialization context state.</param>
        /// <returns>The de-serialized object from the specified <paramref name="column" />.</returns>
        /// <exception cref="IndexOutOfRangeException">The <paramref name="column" /> name provided is invalid.</exception>
        /// <exception cref="System.Security.SecurityException">The caller doesn't have the right permissions for deserialization.</exception>
        /// <exception cref="SerializationException">The serialization stream supports seeking but its length is 0.</exception>
        [CanBeNull]
        public static object GetObjectByName(
            [NotNull] this DbDataReader reader,
            [NotNull] string column,
            [CanBeNull] object nullValue = null,
            [CanBeNull] object context = null,
            StreamingContextStates contextState = StreamingContextStates.Other)
        {
            if (reader == null) throw new ArgumentNullException("reader");
            if (column == null) throw new ArgumentNullException("column");

            int ordinal = reader.GetOrdinal(column);
            if (reader.IsDBNull(ordinal))
                return nullValue;
            
            using (Stream serializationStream = reader.GetStream(ordinal))
            {
                Debug.Assert(serializationStream != null);
                return Serialize.GetFormatter(context, contextState).Deserialize(serializationStream);
            }
        }

        /// <summary>
        /// Gets the index of the parameter with the name given, using the string comparer for name equality.
        /// </summary>
        /// <param name="collection">The collection to search.</param>
        /// <param name="parameterName">Name of the parameter to get the index of.</param>
        /// <param name="comparer">The comparer to use.</param>
        /// <returns></returns>
        public static int IndexOf(
            [NotNull] [ItemNotNull] this SqlParameterCollection collection,
            [NotNull] string parameterName,
            [NotNull] IEqualityComparer<string> comparer)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (parameterName == null) throw new ArgumentNullException(nameof(parameterName));
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));

            int index = 0;
            foreach (SqlParameter parameter in collection)
            {
                if (comparer.Equals(parameter.ParameterName, parameterName))
                    return index;
                index++;
            }
            return -1;
        }

        /// <summary>
        /// Begins a TRY block in the <paramref name="args"/> <see cref="BatchProcessArgs.SqlBuilder"/>.
        /// </summary>
        /// <param name="item">The item the block is for.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="startIndex">The start index.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">IsolationLevel</exception>
        internal static void BeginTry([NotNull] this IBatchItem item, [NotNull] BatchProcessArgs args, out int startIndex)
        {
            bool hasTransaction = item.Transaction != TransactionType.None;
            bool hasTryCatch = item.SuppressErrors || hasTransaction;

            if (hasTryCatch)
            {
                if (hasTransaction)
                {
                    string isoLevel = GetIsolationLevelStr(item.IsolationLevel);
                    if (isoLevel == null)
                        throw new ArgumentOutOfRangeException(
                            nameof(IsolationLevel),
                            item.IsolationLevel,
                            string.Format(Resources.IsolationLevelNotSupported, item.IsolationLevel));

                    string transactionName = item.TransactionName;
                    Debug.Assert(transactionName != null);

                    // Set the isolation level and begin or save a transaction for the batch
                    args.SqlBuilder
                        .AppendLine()
                        .Append("SET TRANSACTION ISOLATION LEVEL ")
                        .Append(isoLevel)
                        .AppendLine(";")

                        .Append(args.InTransaction ? "SAVE" : "BEGIN")
                        .Append(" TRANSACTION ")
                        .AppendIdentifier(transactionName)
                        .AppendLine(";");
                    args.TransactionStack.Push(transactionName, isoLevel, new List<ushort>());
                }

                // Wrap the contents of the batch in a TRY ... CATCH block
                args.SqlBuilder
                    .AppendLine()
                    .AppendLine("BEGIN TRY")
                    .AppendLine()
                    .GetLength(out startIndex);
            }
            else
                startIndex = -1;
        }

        /// <summary>
        /// Ends a TRY block started with <see cref="BeginTry"/>.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="startIndex">The start index.</param>
        internal static void EndTry([NotNull] this IBatchItem item, [NotNull] BatchProcessArgs args, int startIndex)
        {
            TransactionType transaction = item.Transaction;
            bool hasTransaction = transaction != TransactionType.None;
            bool hasTryCatch = item.SuppressErrors || hasTransaction;

            if (hasTryCatch)
            {
                string tranName = item.TransactionName;
                List<ushort> successFlagIndexes = null;
                if (hasTransaction)
                {
                    args.TransactionStack.Pop(out string name, out _, out successFlagIndexes);
                    Debug.Assert(name == tranName);
                    Debug.Assert(tranName != null);
                    if (args.TransactionStack.Count > 0)
                        args.TransactionStack.Peek().Item3.AddRange(successFlagIndexes);
                }

                // If the transaction type is Commit and this is a root transaction, commit it
                if (transaction == TransactionType.Commit && !args.InTransaction)
                {
                    args.SqlBuilder
                        .AppendLine()
                        .Append("COMMIT TRANSACTION ")
                        .AppendIdentifier(tranName)
                        .AppendLine(";");
                }
                // If the transaction is Rollback, always roll it back
                else if (transaction == TransactionType.Rollback)
                {
                    args.SqlBuilder
                        .Append("ROLLBACK TRANSACTION ")
                        .AppendIdentifier(tranName)
                        .AppendLine(";");

                    // If there were any "@Cmd#Success" flags created within this transaction, their value needs to be set to 0
                    if (successFlagIndexes?.Count > 0)
                    {
                        args.SqlBuilder.Append("SELECT ");
                        bool first = true;
                        foreach (ushort index in successFlagIndexes)
                        {
                            if (first) first = false;
                            else args.SqlBuilder.AppendLine(",");
                            args.SqlBuilder.Append("\t@Cmd").Append(index).Append("Success = 0");
                        }
                        args.SqlBuilder.AppendLine(";");
                    }
                }

                // End the TRY block and start the CATCH block
                args.SqlBuilder
                    .IndentRegion(startIndex)
                    .AppendLine("END TRY")
                    .AppendLine("BEGIN CATCH")
                    .GetLength(out startIndex);

                // If there is a transaction, roll it back if possible
                if (hasTransaction)
                {
                    if (args.InTransaction)
                        args.SqlBuilder
                            .AppendLine("IF XACT_STATE() <> -1 ")
                            .Append("\t");

                    args.SqlBuilder
                        .Append("ROLLBACK TRANSACTION ")
                        .AppendIdentifier(tranName)
                        .AppendLine(";");

                    // If there were any "@Cmd#Success" flags created within this transaction, their value needs to be set to 0
                    if (successFlagIndexes?.Count > 0)
                    {
                        args.SqlBuilder.Append("SELECT ");
                        bool first = true;
                        foreach (ushort index in successFlagIndexes)
                        {
                            if (first) first = false;
                            else args.SqlBuilder.AppendLine(",");
                            args.SqlBuilder.Append("\t@Cmd").Append(index).Append("Success = 0");
                        }
                        args.SqlBuilder.AppendLine(";");
                    }
                }

                // Output an Error info message then select the error information
                SqlBatch.AppendInfo(args, Constants.ExecuteState.Error, "%d", "@CmdIndex")
                    .AppendLine("SELECT\tERROR_NUMBER(),\r\n\tCAST(ERROR_STATE() AS tinyint),\r\n\tCAST(ERROR_SEVERITY() AS tinyint)," +
                                "\r\n\t@@SERVERNAME,\r\n\tERROR_MESSAGE(),\r\n\tERROR_PROCEDURE(),\r\n\tERROR_LINE();");

                // If the error isnt being suppressed, rethrow it for any outer catches to handle it
                if (!item.SuppressErrors)
                {
                    if (args.ServerVersion < DatabaseSchema.Sql2012Version)
                    {
                        // Cant rethrow the actual error, so raise a special error message
                        SqlBatch.AppendInfo(args, Constants.ExecuteState.ReThrow, "%d", "@CmdIndex", true);
                    }
                    else
                    {
                        args.SqlBuilder.AppendLine("THROW;");
                    }
                }

                // End the CATCH block
                args.SqlBuilder
                    .IndentRegion(startIndex)
                    .AppendLine("END CATCH")
                    .AppendLine();

                // Reset the isolation level
                if (hasTransaction)
                {
                    if (!args.TransactionStack.TryPeek(out _, out string isoLevel, out _))
                        isoLevel = GetIsolationLevelStr(IsolationLevel.Unspecified);

                    if (isoLevel != null)
                        args.SqlBuilder
                            .AppendLine()
                            .Append("SET TRANSACTION ISOLATION LEVEL ")
                            .Append(isoLevel)
                            .AppendLine(";")
                            .AppendLine();
                }
            }
        }

        /// <summary>
        /// Gets the isolation level string.
        /// </summary>
        /// <param name="isoLevel">The isolation level.</param>
        /// <returns></returns>
        private static string GetIsolationLevelStr(IsolationLevel isoLevel)
        {
            switch (isoLevel)
            {
                case IsolationLevel.ReadUncommitted:
                    return "READ UNCOMMITTED";
                case IsolationLevel.ReadCommitted:
                case IsolationLevel.Unspecified:
                    return "READ COMMITTED";
                case IsolationLevel.RepeatableRead:
                    return "REPEATABLE READ";
                case IsolationLevel.Serializable:
                    return "SERIALIZABLE";
                case IsolationLevel.Snapshot:
                    return "SNAPSHOT";
                case IsolationLevel.Chaos:
                default:
                    return null;
            }
        }

        /// <summary>
        /// Sets the exceptions that occurred for the item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="connectionIndex">Index of the connection.</param>
        /// <param name="exceptions">The exceptions.</param>
        internal static void SetException([NotNull] this IBatchItem item, int connectionIndex, [NotNull] params Exception[] exceptions)
        {
            item.Result.SetException(connectionIndex, exceptions);
            foreach (Exception exception in exceptions)
                HandleException(exception, item);
        }

        /// <summary>
        /// Handles an exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="item">The item whose handler should be used first.</param>
        private static void HandleException(Exception exception, [NotNull] IBatchItem item)
        {
            do
            {
                try
                {
                    // If the item has a handler and it handles the exception, just return
                    if (item.ExceptionHandler != null && item.ExceptionHandler(exception))
                        return;
                }
                catch (Exception e)
                {
                    Log.Add(
                        e,
                        LoggingLevel.Error,
                        () => Resources.SqlBatch_HandleException_Error);
                }

                // Move on to the parent item
                item = item.Owner;
            } while (item != null);
        }

        /// <summary>
        /// Determines whether the type given is an <see cref="Out{T}"/>, and gets the value type if it is.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <param name="valueType">The value type, if the type is an <see cref="Out{T}"/>; otherwise <paramref name="type"/>.</param>
        /// <returns>
        ///   <see langword="true" /> if the type is an <see cref="Out{T}"/>; otherwise, <see langword="false" />.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">type</exception>
        public static bool IsOutputType([NotNull] this Type type, [NotNull] out Type valueType)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            if (!type.IsConstructedGenericType)
            {
                valueType = type;
                return false;
            }

            Type typeDef = type.GetGenericTypeDefinition();
            if (typeDef == typeof(Out<>))
            {
                valueType = type.GetGenericArguments()[0];
                return true;
            }

            valueType = type;
            return false;
        }
    }
}