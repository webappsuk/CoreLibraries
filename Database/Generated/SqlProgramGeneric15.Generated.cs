#region © Copyright Web Applications (UK) Ltd, 2010.  All rights reserved.
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
// ©  Copyright Web Applications (UK) Ltd, 2010.  All rights reserved.
#endregion

#region Designer generated code
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using JetBrains.Annotations;
using Microsoft.SqlServer.Server;
using WebApplications.Utilities.Configuration;
using WebApplications.Utilities.Database.Exceptions;
using WebApplications.Utilities.Database.Schema;
using WebApplications.Utilities.Logging;

namespace WebApplications.Utilities.Database
{
    #region Extensions to SqlProgramCommand
    /// <summary>
    /// A specialised command that allows finer grained control when using SqlPrograms.
    /// </summary>
    public partial class SqlProgramCommand
    {
        /// <summary>
        /// Sets the parameters in ordinal order.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <typeparam name="T3">The type of parameter 3.</typeparam>
        /// <typeparam name="T4">The type of parameter 4.</typeparam>
        /// <typeparam name="T5">The type of parameter 5.</typeparam>
        /// <typeparam name="T6">The type of parameter 6.</typeparam>
        /// <typeparam name="T7">The type of parameter 7.</typeparam>
        /// <typeparam name="T8">The type of parameter 8.</typeparam>
        /// <typeparam name="T9">The type of parameter 9.</typeparam>
        /// <typeparam name="T10">The type of parameter 10.</typeparam>
        /// <typeparam name="T11">The type of parameter 11.</typeparam>
        /// <typeparam name="T12">The type of parameter 12.</typeparam>
        /// <typeparam name="T13">The type of parameter 13.</typeparam>
        /// <typeparam name="T14">The type of parameter 14.</typeparam>
        /// <typeparam name="T15">The type of parameter 15.</typeparam>
        /// <param name="p1Value">Value of SQL Parameter 1.</param>
        /// <param name="p2Value">Value of SQL Parameter 2.</param>
        /// <param name="p3Value">Value of SQL Parameter 3.</param>
        /// <param name="p4Value">Value of SQL Parameter 4.</param>
        /// <param name="p5Value">Value of SQL Parameter 5.</param>
        /// <param name="p6Value">Value of SQL Parameter 6.</param>
        /// <param name="p7Value">Value of SQL Parameter 7.</param>
        /// <param name="p8Value">Value of SQL Parameter 8.</param>
        /// <param name="p9Value">Value of SQL Parameter 9.</param>
        /// <param name="p10Value">Value of SQL Parameter 10.</param>
        /// <param name="p11Value">Value of SQL Parameter 11.</param>
        /// <param name="p12Value">Value of SQL Parameter 12.</param>
        /// <param name="p13Value">Value of SQL Parameter 13.</param>
        /// <param name="p14Value">Value of SQL Parameter 14.</param>
        /// <param name="p15Value">Value of SQL Parameter 15.</param>
        /// <param name="mode">The constraint mode.</param>
        /// <returns>The parameters that were set</returns>
        [NotNull]
        public IEnumerable<SqlParameter> SetParameters<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(T1 p1Value, T2 p2Value, T3 p3Value, T4 p4Value, T5 p5Value, T6 p6Value, T7 p7Value, T8 p8Value, T9 p9Value, T10 p10Value, T11 p11Value, T12 p12Value, T13 p13Value, T14 p14Value, T15 p15Value, TypeConstraintMode mode = TypeConstraintMode.Warn)
        {
            SqlProgramParameter[] parameters = _mapping.Parameters.ToArray();
            int pCount = parameters.GetLength(0);
            if (pCount < 15)
                throw new LoggingException(
                        LoggingLevel.Critical,
                        () => Resources.SqlProgramCommand_SetParameters_Too_Many_Parameters,
                        _program.Name,
                        pCount,
                        15);

            List<SqlParameter> sqlParameters = new List<SqlParameter>(15);
            SqlParameter parameter;
            SqlProgramParameter programParameter;
            int index;
            lock (_parameters)
            {
                // Find or create SQL Parameter 1.
                programParameter = parameters[0];
                index = _parameters.IndexOf(programParameter.FullName);
                parameter = index < 0 ? _parameters.Add(programParameter.CreateSqlParameter()) : _parameters[index];
                parameter.Value = programParameter.CastCLRValue(p1Value, mode);
                sqlParameters.Add(parameter);
                // Find or create SQL Parameter 2.
                programParameter = parameters[1];
                index = _parameters.IndexOf(programParameter.FullName);
                parameter = index < 0 ? _parameters.Add(programParameter.CreateSqlParameter()) : _parameters[index];
                parameter.Value = programParameter.CastCLRValue(p2Value, mode);
                sqlParameters.Add(parameter);
                // Find or create SQL Parameter 3.
                programParameter = parameters[2];
                index = _parameters.IndexOf(programParameter.FullName);
                parameter = index < 0 ? _parameters.Add(programParameter.CreateSqlParameter()) : _parameters[index];
                parameter.Value = programParameter.CastCLRValue(p3Value, mode);
                sqlParameters.Add(parameter);
                // Find or create SQL Parameter 4.
                programParameter = parameters[3];
                index = _parameters.IndexOf(programParameter.FullName);
                parameter = index < 0 ? _parameters.Add(programParameter.CreateSqlParameter()) : _parameters[index];
                parameter.Value = programParameter.CastCLRValue(p4Value, mode);
                sqlParameters.Add(parameter);
                // Find or create SQL Parameter 5.
                programParameter = parameters[4];
                index = _parameters.IndexOf(programParameter.FullName);
                parameter = index < 0 ? _parameters.Add(programParameter.CreateSqlParameter()) : _parameters[index];
                parameter.Value = programParameter.CastCLRValue(p5Value, mode);
                sqlParameters.Add(parameter);
                // Find or create SQL Parameter 6.
                programParameter = parameters[5];
                index = _parameters.IndexOf(programParameter.FullName);
                parameter = index < 0 ? _parameters.Add(programParameter.CreateSqlParameter()) : _parameters[index];
                parameter.Value = programParameter.CastCLRValue(p6Value, mode);
                sqlParameters.Add(parameter);
                // Find or create SQL Parameter 7.
                programParameter = parameters[6];
                index = _parameters.IndexOf(programParameter.FullName);
                parameter = index < 0 ? _parameters.Add(programParameter.CreateSqlParameter()) : _parameters[index];
                parameter.Value = programParameter.CastCLRValue(p7Value, mode);
                sqlParameters.Add(parameter);
                // Find or create SQL Parameter 8.
                programParameter = parameters[7];
                index = _parameters.IndexOf(programParameter.FullName);
                parameter = index < 0 ? _parameters.Add(programParameter.CreateSqlParameter()) : _parameters[index];
                parameter.Value = programParameter.CastCLRValue(p8Value, mode);
                sqlParameters.Add(parameter);
                // Find or create SQL Parameter 9.
                programParameter = parameters[8];
                index = _parameters.IndexOf(programParameter.FullName);
                parameter = index < 0 ? _parameters.Add(programParameter.CreateSqlParameter()) : _parameters[index];
                parameter.Value = programParameter.CastCLRValue(p9Value, mode);
                sqlParameters.Add(parameter);
                // Find or create SQL Parameter 10.
                programParameter = parameters[9];
                index = _parameters.IndexOf(programParameter.FullName);
                parameter = index < 0 ? _parameters.Add(programParameter.CreateSqlParameter()) : _parameters[index];
                parameter.Value = programParameter.CastCLRValue(p10Value, mode);
                sqlParameters.Add(parameter);
                // Find or create SQL Parameter 11.
                programParameter = parameters[10];
                index = _parameters.IndexOf(programParameter.FullName);
                parameter = index < 0 ? _parameters.Add(programParameter.CreateSqlParameter()) : _parameters[index];
                parameter.Value = programParameter.CastCLRValue(p11Value, mode);
                sqlParameters.Add(parameter);
                // Find or create SQL Parameter 12.
                programParameter = parameters[11];
                index = _parameters.IndexOf(programParameter.FullName);
                parameter = index < 0 ? _parameters.Add(programParameter.CreateSqlParameter()) : _parameters[index];
                parameter.Value = programParameter.CastCLRValue(p12Value, mode);
                sqlParameters.Add(parameter);
                // Find or create SQL Parameter 13.
                programParameter = parameters[12];
                index = _parameters.IndexOf(programParameter.FullName);
                parameter = index < 0 ? _parameters.Add(programParameter.CreateSqlParameter()) : _parameters[index];
                parameter.Value = programParameter.CastCLRValue(p13Value, mode);
                sqlParameters.Add(parameter);
                // Find or create SQL Parameter 14.
                programParameter = parameters[13];
                index = _parameters.IndexOf(programParameter.FullName);
                parameter = index < 0 ? _parameters.Add(programParameter.CreateSqlParameter()) : _parameters[index];
                parameter.Value = programParameter.CastCLRValue(p14Value, mode);
                sqlParameters.Add(parameter);
                // Find or create SQL Parameter 15.
                programParameter = parameters[14];
                index = _parameters.IndexOf(programParameter.FullName);
                parameter = index < 0 ? _parameters.Add(programParameter.CreateSqlParameter()) : _parameters[index];
                parameter.Value = programParameter.CastCLRValue(p15Value, mode);
                sqlParameters.Add(parameter);
            }

            // Return parameters that were set
            return sqlParameters;
        }

        /// <summary>
        /// Sets the parameters in ordinal order.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <typeparam name="T3">The type of parameter 3.</typeparam>
        /// <typeparam name="T4">The type of parameter 4.</typeparam>
        /// <typeparam name="T5">The type of parameter 5.</typeparam>
        /// <typeparam name="T6">The type of parameter 6.</typeparam>
        /// <typeparam name="T7">The type of parameter 7.</typeparam>
        /// <typeparam name="T8">The type of parameter 8.</typeparam>
        /// <typeparam name="T9">The type of parameter 9.</typeparam>
        /// <typeparam name="T10">The type of parameter 10.</typeparam>
        /// <typeparam name="T11">The type of parameter 11.</typeparam>
        /// <typeparam name="T12">The type of parameter 12.</typeparam>
        /// <typeparam name="T13">The type of parameter 13.</typeparam>
        /// <typeparam name="T14">The type of parameter 14.</typeparam>
        /// <typeparam name="T15">The type of parameter 15.</typeparam>
        /// <param name="names">The enumeration of parameters names.</param>
        /// <param name="p1Value">Value of SQL Parameter 1.</param>
        /// <param name="p2Value">Value of SQL Parameter 2.</param>
        /// <param name="p3Value">Value of SQL Parameter 3.</param>
        /// <param name="p4Value">Value of SQL Parameter 4.</param>
        /// <param name="p5Value">Value of SQL Parameter 5.</param>
        /// <param name="p6Value">Value of SQL Parameter 6.</param>
        /// <param name="p7Value">Value of SQL Parameter 7.</param>
        /// <param name="p8Value">Value of SQL Parameter 8.</param>
        /// <param name="p9Value">Value of SQL Parameter 9.</param>
        /// <param name="p10Value">Value of SQL Parameter 10.</param>
        /// <param name="p11Value">Value of SQL Parameter 11.</param>
        /// <param name="p12Value">Value of SQL Parameter 12.</param>
        /// <param name="p13Value">Value of SQL Parameter 13.</param>
        /// <param name="p14Value">Value of SQL Parameter 14.</param>
        /// <param name="p15Value">Value of SQL Parameter 15.</param>
        /// <param name="mode">The constraint mode.</param>
        /// <returns>The parameters that were set</returns>
        [NotNull]
        public IEnumerable<SqlParameter> SetParameters<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(IEnumerable<string> names, T1 p1Value, T2 p2Value, T3 p3Value, T4 p4Value, T5 p5Value, T6 p6Value, T7 p7Value, T8 p8Value, T9 p9Value, T10 p10Value, T11 p11Value, T12 p12Value, T13 p13Value, T14 p14Value, T15 p15Value, TypeConstraintMode mode = TypeConstraintMode.Warn)
        {
            if ((names == null) || (names.Count() != 15))
                throw new LoggingException(
                        LoggingLevel.Critical,
                        () => Resources.SqlProgramCommand_SetParameters_Wrong_Number_Of_Parameters,
                        _program.Name,
                        15,
                        names == null ? 0 : names.Count());

            SqlProgramParameter[] parameters = names.Select(
                    n =>
                        {
                            n = n.ToLower(); // Find parameter definition
                            SqlProgramParameter parameterDefinition;
                            if (!_mapping.Definition.TryGetParameter(n, out parameterDefinition))
                                throw new LoggingException(
                                        LoggingLevel.Critical,
                                        () => Resources.SqlProgramCommand_SetParameters_Unknown_Parameter,
                                        _program.Name,
                                        n);
                            return parameterDefinition;
                        }).ToArray();

            int pCount = parameters.GetLength(0);
            if (pCount < 15)
                throw new LoggingException(
                        LoggingLevel.Critical,
                        () => Resources.SqlProgramCommand_SetParameters_Too_Many_Parameters,
                        _program.Name,
                        pCount,
                        15);

            List<SqlParameter> sqlParameters = new List<SqlParameter>(2);
            SqlParameter parameter;
            SqlProgramParameter programParameter;
            int index;
            lock (_parameters)
            {
                // Find or create SQL Parameter 1.
                programParameter = parameters[0];
                index = _parameters.IndexOf(programParameter.FullName);
                parameter = index < 0 ? _parameters.Add(programParameter.CreateSqlParameter()) : _parameters[index];
                parameter.Value = programParameter.CastCLRValue(p1Value, mode);
                sqlParameters.Add(parameter);
                // Find or create SQL Parameter 2.
                programParameter = parameters[1];
                index = _parameters.IndexOf(programParameter.FullName);
                parameter = index < 0 ? _parameters.Add(programParameter.CreateSqlParameter()) : _parameters[index];
                parameter.Value = programParameter.CastCLRValue(p2Value, mode);
                sqlParameters.Add(parameter);
                // Find or create SQL Parameter 3.
                programParameter = parameters[2];
                index = _parameters.IndexOf(programParameter.FullName);
                parameter = index < 0 ? _parameters.Add(programParameter.CreateSqlParameter()) : _parameters[index];
                parameter.Value = programParameter.CastCLRValue(p3Value, mode);
                sqlParameters.Add(parameter);
                // Find or create SQL Parameter 4.
                programParameter = parameters[3];
                index = _parameters.IndexOf(programParameter.FullName);
                parameter = index < 0 ? _parameters.Add(programParameter.CreateSqlParameter()) : _parameters[index];
                parameter.Value = programParameter.CastCLRValue(p4Value, mode);
                sqlParameters.Add(parameter);
                // Find or create SQL Parameter 5.
                programParameter = parameters[4];
                index = _parameters.IndexOf(programParameter.FullName);
                parameter = index < 0 ? _parameters.Add(programParameter.CreateSqlParameter()) : _parameters[index];
                parameter.Value = programParameter.CastCLRValue(p5Value, mode);
                sqlParameters.Add(parameter);
                // Find or create SQL Parameter 6.
                programParameter = parameters[5];
                index = _parameters.IndexOf(programParameter.FullName);
                parameter = index < 0 ? _parameters.Add(programParameter.CreateSqlParameter()) : _parameters[index];
                parameter.Value = programParameter.CastCLRValue(p6Value, mode);
                sqlParameters.Add(parameter);
                // Find or create SQL Parameter 7.
                programParameter = parameters[6];
                index = _parameters.IndexOf(programParameter.FullName);
                parameter = index < 0 ? _parameters.Add(programParameter.CreateSqlParameter()) : _parameters[index];
                parameter.Value = programParameter.CastCLRValue(p7Value, mode);
                sqlParameters.Add(parameter);
                // Find or create SQL Parameter 8.
                programParameter = parameters[7];
                index = _parameters.IndexOf(programParameter.FullName);
                parameter = index < 0 ? _parameters.Add(programParameter.CreateSqlParameter()) : _parameters[index];
                parameter.Value = programParameter.CastCLRValue(p8Value, mode);
                sqlParameters.Add(parameter);
                // Find or create SQL Parameter 9.
                programParameter = parameters[8];
                index = _parameters.IndexOf(programParameter.FullName);
                parameter = index < 0 ? _parameters.Add(programParameter.CreateSqlParameter()) : _parameters[index];
                parameter.Value = programParameter.CastCLRValue(p9Value, mode);
                sqlParameters.Add(parameter);
                // Find or create SQL Parameter 10.
                programParameter = parameters[9];
                index = _parameters.IndexOf(programParameter.FullName);
                parameter = index < 0 ? _parameters.Add(programParameter.CreateSqlParameter()) : _parameters[index];
                parameter.Value = programParameter.CastCLRValue(p10Value, mode);
                sqlParameters.Add(parameter);
                // Find or create SQL Parameter 11.
                programParameter = parameters[10];
                index = _parameters.IndexOf(programParameter.FullName);
                parameter = index < 0 ? _parameters.Add(programParameter.CreateSqlParameter()) : _parameters[index];
                parameter.Value = programParameter.CastCLRValue(p11Value, mode);
                sqlParameters.Add(parameter);
                // Find or create SQL Parameter 12.
                programParameter = parameters[11];
                index = _parameters.IndexOf(programParameter.FullName);
                parameter = index < 0 ? _parameters.Add(programParameter.CreateSqlParameter()) : _parameters[index];
                parameter.Value = programParameter.CastCLRValue(p12Value, mode);
                sqlParameters.Add(parameter);
                // Find or create SQL Parameter 13.
                programParameter = parameters[12];
                index = _parameters.IndexOf(programParameter.FullName);
                parameter = index < 0 ? _parameters.Add(programParameter.CreateSqlParameter()) : _parameters[index];
                parameter.Value = programParameter.CastCLRValue(p13Value, mode);
                sqlParameters.Add(parameter);
                // Find or create SQL Parameter 14.
                programParameter = parameters[13];
                index = _parameters.IndexOf(programParameter.FullName);
                parameter = index < 0 ? _parameters.Add(programParameter.CreateSqlParameter()) : _parameters[index];
                parameter.Value = programParameter.CastCLRValue(p14Value, mode);
                sqlParameters.Add(parameter);
                // Find or create SQL Parameter 15.
                programParameter = parameters[14];
                index = _parameters.IndexOf(programParameter.FullName);
                parameter = index < 0 ? _parameters.Add(programParameter.CreateSqlParameter()) : _parameters[index];
                parameter.Value = programParameter.CastCLRValue(p15Value, mode);
                sqlParameters.Add(parameter);
            }

            // Return parameters that were set
            return sqlParameters;
        }
    }
    #endregion

    #region SqlProgam<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>
    /// <summary>
    ///   Used to create an object for easy calling of stored procedures or functions in a database.
    /// </summary>
    public class SqlProgram<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> : SqlProgram
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlProgram" /> class.
        /// </summary>
        /// <param name="connection">The load balanced connection.</param>
        /// <param name="name">The <see cref="SqlProgram.Name">name</see> of the program.</param>
        /// <param name="parameters">The program <see cref="SqlProgram.Parameters">parameters</see>.</param>
        /// <param name="defaultCommandTimeout"><para>The <see cref="SqlProgram.DefaultCommandTimeout">default command timeout</see></para>
        /// <para>This is the time to wait for the command to execute.</para>
        /// <para>If set to <see langword="null" /> then the timeout will be 30 seconds.</para></param>
        /// <param name="constraintMode"><para>The type constraint mode.</para>
        /// <para>By default this is set to log a warning if truncation/loss of precision occurs.</para></param>
        protected SqlProgram(
            [NotNull] LoadBalancedConnection connection,
            [NotNull] string name,
            [CanBeNull] IEnumerable<KeyValuePair<string, Type>> parameters = null,
            TimeSpan? defaultCommandTimeout = null,
            TypeConstraintMode constraintMode = TypeConstraintMode.Warn)
            : base(connection, name, parameters, defaultCommandTimeout, constraintMode)
        {
            Contract.Requires(connection != null);
            Contract.Requires(name != null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlProgram" /> class.
        /// </summary>
        /// <param name="program">The base program (stored procedure/function).</param>
        /// <param name="parameters">The program <see cref="SqlProgram.Parameters">parameters</see>.</param>
        /// <param name="defaultCommandTimeout"><para>The <see cref="SqlProgram.DefaultCommandTimeout">default command timeout</see></para>
        /// <para>This is the time to wait for the command to execute.</para>
        /// <para>If set to <see langword="null" /> then the timeout the default timeout from the base program.</para></param>
        /// <param name="constraintMode">The type constraint mode, this defined the behavior when truncation/loss of precision occurs.</param>
        protected SqlProgram(
            [NotNull] SqlProgram program,
            [NotNull] IEnumerable<KeyValuePair<string, Type>> parameters,
            TimeSpan? defaultCommandTimeout,
            TypeConstraintMode constraintMode)
            : base(program, parameters, defaultCommandTimeout, constraintMode)
        {
            Contract.Requires(program != null);
            Contract.Requires(parameters != null);
        }
        #endregion
        
        #region Create overloads
        /// <summary>
        /// Creates a new instance of the <see cref="SqlProgram&lt;T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15&gt;"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="name">The name.</param>
        /// <param name="ignoreValidationErrors">if set to <see langword="true"/> does not throw validation errors (records them instead).</param>
        /// <param name="defaultCommandTimeout">The optional default command timeout, which will be used whenever this command is executed synchronously.
        /// Defaults to 30s.</param>
        /// <param name="constraintMode">The constraint mode.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An awaitable task, resulting in a <see cref="SqlProgram"/>.</returns>
        public static async Task<SqlProgram<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>> Create(
            [NotNull] LoadBalancedConnection connection,
            [NotNull] string name,
            bool ignoreValidationErrors = false,
            TimeSpan? defaultCommandTimeout = null,
            TypeConstraintMode constraintMode = TypeConstraintMode.Warn,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Contract.Requires(connection != null);
            SqlProgram<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> newProgram = new SqlProgram<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
                connection,
                name,
                new[] { new KeyValuePair<string, Type>(null, typeof(T1)), new KeyValuePair<string, Type>(null, typeof(T2)), new KeyValuePair<string, Type>(null, typeof(T3)), new KeyValuePair<string, Type>(null, typeof(T4)), new KeyValuePair<string, Type>(null, typeof(T5)), new KeyValuePair<string, Type>(null, typeof(T6)), new KeyValuePair<string, Type>(null, typeof(T7)), new KeyValuePair<string, Type>(null, typeof(T8)), new KeyValuePair<string, Type>(null, typeof(T9)), new KeyValuePair<string, Type>(null, typeof(T10)), new KeyValuePair<string, Type>(null, typeof(T11)), new KeyValuePair<string, Type>(null, typeof(T12)), new KeyValuePair<string, Type>(null, typeof(T13)), new KeyValuePair<string, Type>(null, typeof(T14)), new KeyValuePair<string, Type>(null, typeof(T15)) },
                defaultCommandTimeout,
                constraintMode);

            // Validate
            await newProgram.Validate(true, false, !ignoreValidationErrors, cancellationToken).ConfigureAwait(false);

            return newProgram;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="SqlProgram&lt;T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15&gt;"/> class.
        /// </summary>
        /// <param name="sqlProgram">The SQL program.</param>
        /// <param name="ignoreValidationErrors">if set to <see langword="true"/> does not throw validation errors (records them instead).</param>
        /// <param name="defaultCommandTimeout">The optional default command timeout, which will be used whenever this command is executed synchronously.
        /// Defaults to existing programs default.</param>
        /// <param name="constraintMode">The constraint mode.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An awaitable task, resulting in a <see cref="SqlProgram"/>.</returns>
        public static async Task<SqlProgram<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>> Create(
            [NotNull] SqlProgram sqlProgram,
            bool ignoreValidationErrors = false,
            TimeSpan? defaultCommandTimeout = null,
            TypeConstraintMode constraintMode = TypeConstraintMode.Warn,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Contract.Requires(sqlProgram != null);
            SqlProgram<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> newProgram = new SqlProgram<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
                sqlProgram,
                new[] { new KeyValuePair<string, Type>(null, typeof(T1)), new KeyValuePair<string, Type>(null, typeof(T2)), new KeyValuePair<string, Type>(null, typeof(T3)), new KeyValuePair<string, Type>(null, typeof(T4)), new KeyValuePair<string, Type>(null, typeof(T5)), new KeyValuePair<string, Type>(null, typeof(T6)), new KeyValuePair<string, Type>(null, typeof(T7)), new KeyValuePair<string, Type>(null, typeof(T8)), new KeyValuePair<string, Type>(null, typeof(T9)), new KeyValuePair<string, Type>(null, typeof(T10)), new KeyValuePair<string, Type>(null, typeof(T11)), new KeyValuePair<string, Type>(null, typeof(T12)), new KeyValuePair<string, Type>(null, typeof(T13)), new KeyValuePair<string, Type>(null, typeof(T14)), new KeyValuePair<string, Type>(null, typeof(T15)) },
                defaultCommandTimeout,
                constraintMode);

            // Validate
            await newProgram.Validate(true, false, !ignoreValidationErrors, cancellationToken).ConfigureAwait(false);

            return newProgram;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="SqlProgram&lt;T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15&gt;"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="name">The name.</param>
        /// <param name="p1Name">Name of parameter 1.</param>
        /// <param name="p2Name">Name of parameter 2.</param>
        /// <param name="p3Name">Name of parameter 3.</param>
        /// <param name="p4Name">Name of parameter 4.</param>
        /// <param name="p5Name">Name of parameter 5.</param>
        /// <param name="p6Name">Name of parameter 6.</param>
        /// <param name="p7Name">Name of parameter 7.</param>
        /// <param name="p8Name">Name of parameter 8.</param>
        /// <param name="p9Name">Name of parameter 9.</param>
        /// <param name="p10Name">Name of parameter 10.</param>
        /// <param name="p11Name">Name of parameter 11.</param>
        /// <param name="p12Name">Name of parameter 12.</param>
        /// <param name="p13Name">Name of parameter 13.</param>
        /// <param name="p14Name">Name of parameter 14.</param>
        /// <param name="p15Name">Name of parameter 15.</param>
        /// <param name="ignoreValidationErrors">if set to <see langword="true"/> does not throw validation errors (records them instead).</param>
        /// <param name="checkOrder">if set to <c>true</c> checks the parameter order matches.</param>
        /// <param name="defaultCommandTimeout">The optional default command timeout, which will be used whenever this command is executed synchronously.
        /// Defaults to 30s.</param>
        /// <param name="constraintMode">The constraint mode.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An awaitable task, resulting in a <see cref="SqlProgram"/>.</returns>
        public static async Task<SqlProgram<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>> Create(
            [NotNull] LoadBalancedConnection connection,
            [NotNull] string name,
            [NotNull] string p1Name, 
            [NotNull] string p2Name, 
            [NotNull] string p3Name, 
            [NotNull] string p4Name, 
            [NotNull] string p5Name, 
            [NotNull] string p6Name, 
            [NotNull] string p7Name, 
            [NotNull] string p8Name, 
            [NotNull] string p9Name, 
            [NotNull] string p10Name, 
            [NotNull] string p11Name, 
            [NotNull] string p12Name, 
            [NotNull] string p13Name, 
            [NotNull] string p14Name, 
            [NotNull] string p15Name,
            bool ignoreValidationErrors = false,
            bool checkOrder = false,
            TimeSpan? defaultCommandTimeout = null,
            TypeConstraintMode constraintMode = TypeConstraintMode.Warn,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Contract.Requires(connection != null);
            Contract.Requires(p1Name != null);
            Contract.Requires(p2Name != null);
            Contract.Requires(p3Name != null);
            Contract.Requires(p4Name != null);
            Contract.Requires(p5Name != null);
            Contract.Requires(p6Name != null);
            Contract.Requires(p7Name != null);
            Contract.Requires(p8Name != null);
            Contract.Requires(p9Name != null);
            Contract.Requires(p10Name != null);
            Contract.Requires(p11Name != null);
            Contract.Requires(p12Name != null);
            Contract.Requires(p13Name != null);
            Contract.Requires(p14Name != null);
            Contract.Requires(p15Name != null);
            SqlProgram<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> newProgram = new SqlProgram<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
                connection,
                name,
                new[] { new KeyValuePair<string, Type>(p1Name, typeof(T1)), new KeyValuePair<string, Type>(p2Name, typeof(T2)), new KeyValuePair<string, Type>(p3Name, typeof(T3)), new KeyValuePair<string, Type>(p4Name, typeof(T4)), new KeyValuePair<string, Type>(p5Name, typeof(T5)), new KeyValuePair<string, Type>(p6Name, typeof(T6)), new KeyValuePair<string, Type>(p7Name, typeof(T7)), new KeyValuePair<string, Type>(p8Name, typeof(T8)), new KeyValuePair<string, Type>(p9Name, typeof(T9)), new KeyValuePair<string, Type>(p10Name, typeof(T10)), new KeyValuePair<string, Type>(p11Name, typeof(T11)), new KeyValuePair<string, Type>(p12Name, typeof(T12)), new KeyValuePair<string, Type>(p13Name, typeof(T13)), new KeyValuePair<string, Type>(p14Name, typeof(T14)), new KeyValuePair<string, Type>(p15Name, typeof(T15)) },
                defaultCommandTimeout,
                constraintMode);

            // Validate
            await newProgram.Validate(checkOrder, false, !ignoreValidationErrors, cancellationToken).ConfigureAwait(false);

            return newProgram;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="SqlProgram&lt;T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15&gt;"/> class.
        /// </summary>
        /// <param name="sqlProgram">The SQL program.</param>
        /// <param name="p1Name">Name of parameter 1.</param>
        /// <param name="p2Name">Name of parameter 2.</param>
        /// <param name="p3Name">Name of parameter 3.</param>
        /// <param name="p4Name">Name of parameter 4.</param>
        /// <param name="p5Name">Name of parameter 5.</param>
        /// <param name="p6Name">Name of parameter 6.</param>
        /// <param name="p7Name">Name of parameter 7.</param>
        /// <param name="p8Name">Name of parameter 8.</param>
        /// <param name="p9Name">Name of parameter 9.</param>
        /// <param name="p10Name">Name of parameter 10.</param>
        /// <param name="p11Name">Name of parameter 11.</param>
        /// <param name="p12Name">Name of parameter 12.</param>
        /// <param name="p13Name">Name of parameter 13.</param>
        /// <param name="p14Name">Name of parameter 14.</param>
        /// <param name="p15Name">Name of parameter 15.</param>
        /// <param name="ignoreValidationErrors">if set to <see langword="true"/> does not throw validation errors (records them instead).</param>
        /// <param name="checkOrder">if set to <c>true</c> checks the parameter order matches.</param>
        /// <param name="defaultCommandTimeout">The optional default command timeout, which will be used whenever this command is executed synchronously.
        /// Defaults to existing programs default.</param>
        /// <param name="constraintMode">The constraint mode.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An awaitable task, resulting in a <see cref="SqlProgram"/>.</returns>
        public static async Task<SqlProgram<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>> Create(
            [NotNull] SqlProgram sqlProgram,
            [NotNull] string p1Name, 
            [NotNull] string p2Name, 
            [NotNull] string p3Name, 
            [NotNull] string p4Name, 
            [NotNull] string p5Name, 
            [NotNull] string p6Name, 
            [NotNull] string p7Name, 
            [NotNull] string p8Name, 
            [NotNull] string p9Name, 
            [NotNull] string p10Name, 
            [NotNull] string p11Name, 
            [NotNull] string p12Name, 
            [NotNull] string p13Name, 
            [NotNull] string p14Name, 
            [NotNull] string p15Name,
            bool ignoreValidationErrors = false,
            bool checkOrder = false,
            TimeSpan? defaultCommandTimeout = null,
            TypeConstraintMode constraintMode = TypeConstraintMode.Warn,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Contract.Requires(sqlProgram != null);
            Contract.Requires(p1Name != null);
            Contract.Requires(p2Name != null);
            Contract.Requires(p3Name != null);
            Contract.Requires(p4Name != null);
            Contract.Requires(p5Name != null);
            Contract.Requires(p6Name != null);
            Contract.Requires(p7Name != null);
            Contract.Requires(p8Name != null);
            Contract.Requires(p9Name != null);
            Contract.Requires(p10Name != null);
            Contract.Requires(p11Name != null);
            Contract.Requires(p12Name != null);
            Contract.Requires(p13Name != null);
            Contract.Requires(p14Name != null);
            Contract.Requires(p15Name != null);
            SqlProgram<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> newProgram = new SqlProgram<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
                sqlProgram,
                new[] { new KeyValuePair<string, Type>(p1Name, typeof(T1)), new KeyValuePair<string, Type>(p2Name, typeof(T2)), new KeyValuePair<string, Type>(p3Name, typeof(T3)), new KeyValuePair<string, Type>(p4Name, typeof(T4)), new KeyValuePair<string, Type>(p5Name, typeof(T5)), new KeyValuePair<string, Type>(p6Name, typeof(T6)), new KeyValuePair<string, Type>(p7Name, typeof(T7)), new KeyValuePair<string, Type>(p8Name, typeof(T8)), new KeyValuePair<string, Type>(p9Name, typeof(T9)), new KeyValuePair<string, Type>(p10Name, typeof(T10)), new KeyValuePair<string, Type>(p11Name, typeof(T11)), new KeyValuePair<string, Type>(p12Name, typeof(T12)), new KeyValuePair<string, Type>(p13Name, typeof(T13)), new KeyValuePair<string, Type>(p14Name, typeof(T14)), new KeyValuePair<string, Type>(p15Name, typeof(T15)) },
                defaultCommandTimeout,
                constraintMode);

            // Validate
            await newProgram.Validate(checkOrder, false, !ignoreValidationErrors, cancellationToken).ConfigureAwait(false);

            return newProgram;
        }
        #endregion

        /// <summary>
        /// Executes the query, and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
        /// </summary>
        /// <typeparam name="TOut">The type of the output.</typeparam>
        /// <param name="p1Value">Value of SQL Parameter 1.</param>
        /// <param name="p2Value">Value of SQL Parameter 2.</param>
        /// <param name="p3Value">Value of SQL Parameter 3.</param>
        /// <param name="p4Value">Value of SQL Parameter 4.</param>
        /// <param name="p5Value">Value of SQL Parameter 5.</param>
        /// <param name="p6Value">Value of SQL Parameter 6.</param>
        /// <param name="p7Value">Value of SQL Parameter 7.</param>
        /// <param name="p8Value">Value of SQL Parameter 8.</param>
        /// <param name="p9Value">Value of SQL Parameter 9.</param>
        /// <param name="p10Value">Value of SQL Parameter 10.</param>
        /// <param name="p11Value">Value of SQL Parameter 11.</param>
        /// <param name="p12Value">Value of SQL Parameter 12.</param>
        /// <param name="p13Value">Value of SQL Parameter 13.</param>
        /// <param name="p14Value">Value of SQL Parameter 14.</param>
        /// <param name="p15Value">Value of SQL Parameter 15.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configured default for this program.</param>
        /// <returns>The scalar value.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public TOut ExecuteScalar<TOut>(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), T7 p7Value = default(T7), T8 p8Value = default(T8), T9 p9Value = default(T9), T10 p10Value = default(T10), T11 p11Value = default(T11), T12 p12Value = default(T12), T13 p13Value = default(T13), T14 p14Value = default(T14), T15 p15Value = default(T15), TypeConstraintMode? constraintMode = null)
        {
            return this.ExecuteScalar<TOut>(c => c.SetParameters(p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, p7Value, p8Value, p9Value, p10Value, p11Value, p12Value, p13Value, p14Value, p15Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)));
        }

        /// <summary>
        /// Executes the query, and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
        /// </summary>
        /// <typeparam name="TOut">The type of the output.</typeparam>
        /// <param name="p1Value">Value of SQL Parameter 1.</param>
        /// <param name="p2Value">Value of SQL Parameter 2.</param>
        /// <param name="p3Value">Value of SQL Parameter 3.</param>
        /// <param name="p4Value">Value of SQL Parameter 4.</param>
        /// <param name="p5Value">Value of SQL Parameter 5.</param>
        /// <param name="p6Value">Value of SQL Parameter 6.</param>
        /// <param name="p7Value">Value of SQL Parameter 7.</param>
        /// <param name="p8Value">Value of SQL Parameter 8.</param>
        /// <param name="p9Value">Value of SQL Parameter 9.</param>
        /// <param name="p10Value">Value of SQL Parameter 10.</param>
        /// <param name="p11Value">Value of SQL Parameter 11.</param>
        /// <param name="p12Value">Value of SQL Parameter 12.</param>
        /// <param name="p13Value">Value of SQL Parameter 13.</param>
        /// <param name="p14Value">Value of SQL Parameter 14.</param>
        /// <param name="p15Value">Value of SQL Parameter 15.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configured default for this program.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The scalar value.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        [NotNull]
        public Task<TOut> ExecuteScalarAsync<TOut>(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), T7 p7Value = default(T7), T8 p8Value = default(T8), T9 p9Value = default(T9), T10 p10Value = default(T10), T11 p11Value = default(T11), T12 p12Value = default(T12), T13 p13Value = default(T13), T14 p14Value = default(T14), T15 p15Value = default(T15), TypeConstraintMode? constraintMode = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.ExecuteScalarAsync<TOut>(c => c.SetParameters(p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, p7Value, p8Value, p9Value, p10Value, p11Value, p12Value, p13Value, p14Value, p15Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), cancellationToken);
        }

        /// <summary>
        /// Executes the query, and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
        /// </summary>
        /// <typeparam name="TOut">The type of the output.</typeparam>
        /// <param name="p1Value">Value of SQL Parameter 1.</param>
        /// <param name="p2Value">Value of SQL Parameter 2.</param>
        /// <param name="p3Value">Value of SQL Parameter 3.</param>
        /// <param name="p4Value">Value of SQL Parameter 4.</param>
        /// <param name="p5Value">Value of SQL Parameter 5.</param>
        /// <param name="p6Value">Value of SQL Parameter 6.</param>
        /// <param name="p7Value">Value of SQL Parameter 7.</param>
        /// <param name="p8Value">Value of SQL Parameter 8.</param>
        /// <param name="p9Value">Value of SQL Parameter 9.</param>
        /// <param name="p10Value">Value of SQL Parameter 10.</param>
        /// <param name="p11Value">Value of SQL Parameter 11.</param>
        /// <param name="p12Value">Value of SQL Parameter 12.</param>
        /// <param name="p13Value">Value of SQL Parameter 13.</param>
        /// <param name="p14Value">Value of SQL Parameter 14.</param>
        /// <param name="p15Value">Value of SQL Parameter 15.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configured default for this program.</param>
        /// <returns>The scalar value for each connection.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        [NotNull]
        public IEnumerable<TOut> ExecuteScalarAll<TOut>(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), T7 p7Value = default(T7), T8 p8Value = default(T8), T9 p9Value = default(T9), T10 p10Value = default(T10), T11 p11Value = default(T11), T12 p12Value = default(T12), T13 p13Value = default(T13), T14 p14Value = default(T14), T15 p15Value = default(T15), TypeConstraintMode? constraintMode = null)
        {
            return this.ExecuteScalarAll<TOut>(c => c.SetParameters(p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, p7Value, p8Value, p9Value, p10Value, p11Value, p12Value, p13Value, p14Value, p15Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)));
        }

        /// <summary>
        /// Executes the query, and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
        /// </summary>
        /// <typeparam name="TOut">The type of the output.</typeparam>
        /// <param name="p1Value">Value of SQL Parameter 1.</param>
        /// <param name="p2Value">Value of SQL Parameter 2.</param>
        /// <param name="p3Value">Value of SQL Parameter 3.</param>
        /// <param name="p4Value">Value of SQL Parameter 4.</param>
        /// <param name="p5Value">Value of SQL Parameter 5.</param>
        /// <param name="p6Value">Value of SQL Parameter 6.</param>
        /// <param name="p7Value">Value of SQL Parameter 7.</param>
        /// <param name="p8Value">Value of SQL Parameter 8.</param>
        /// <param name="p9Value">Value of SQL Parameter 9.</param>
        /// <param name="p10Value">Value of SQL Parameter 10.</param>
        /// <param name="p11Value">Value of SQL Parameter 11.</param>
        /// <param name="p12Value">Value of SQL Parameter 12.</param>
        /// <param name="p13Value">Value of SQL Parameter 13.</param>
        /// <param name="p14Value">Value of SQL Parameter 14.</param>
        /// <param name="p15Value">Value of SQL Parameter 15.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configured default for this program.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The scalar value for each connection.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        [NotNull]
        public Task<IEnumerable<TOut>> ExecuteScalarAllAsync<TOut>(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), T7 p7Value = default(T7), T8 p8Value = default(T8), T9 p9Value = default(T9), T10 p10Value = default(T10), T11 p11Value = default(T11), T12 p12Value = default(T12), T13 p13Value = default(T13), T14 p14Value = default(T14), T15 p15Value = default(T15), TypeConstraintMode? constraintMode = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.ExecuteScalarAllAsync<TOut>(c => c.SetParameters(p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, p7Value, p8Value, p9Value, p10Value, p11Value, p12Value, p13Value, p14Value, p15Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), cancellationToken);
        }

        /// <summary>
        /// Executes a Transact-SQL statement against the connection and returns the number of rows affected.
        /// </summary>
        /// <param name="p1Value">Value of SQL Parameter 1.</param>
        /// <param name="p2Value">Value of SQL Parameter 2.</param>
        /// <param name="p3Value">Value of SQL Parameter 3.</param>
        /// <param name="p4Value">Value of SQL Parameter 4.</param>
        /// <param name="p5Value">Value of SQL Parameter 5.</param>
        /// <param name="p6Value">Value of SQL Parameter 6.</param>
        /// <param name="p7Value">Value of SQL Parameter 7.</param>
        /// <param name="p8Value">Value of SQL Parameter 8.</param>
        /// <param name="p9Value">Value of SQL Parameter 9.</param>
        /// <param name="p10Value">Value of SQL Parameter 10.</param>
        /// <param name="p11Value">Value of SQL Parameter 11.</param>
        /// <param name="p12Value">Value of SQL Parameter 12.</param>
        /// <param name="p13Value">Value of SQL Parameter 13.</param>
        /// <param name="p14Value">Value of SQL Parameter 14.</param>
        /// <param name="p15Value">Value of SQL Parameter 15.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configured default for this program.</param>
        /// <returns>Number of rows affected.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public int ExecuteNonQuery(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), T7 p7Value = default(T7), T8 p8Value = default(T8), T9 p9Value = default(T9), T10 p10Value = default(T10), T11 p11Value = default(T11), T12 p12Value = default(T12), T13 p13Value = default(T13), T14 p14Value = default(T14), T15 p15Value = default(T15), TypeConstraintMode? constraintMode = null)
        {
            return this.ExecuteNonQuery(c => c.SetParameters(p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, p7Value, p8Value, p9Value, p10Value, p11Value, p12Value, p13Value, p14Value, p15Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)));
        }

        /// <summary>
        /// Executes a Transact-SQL statement against the connection and returns the number of rows affected.
        /// </summary>
        /// <param name="p1Value">Value of SQL Parameter 1.</param>
        /// <param name="p2Value">Value of SQL Parameter 2.</param>
        /// <param name="p3Value">Value of SQL Parameter 3.</param>
        /// <param name="p4Value">Value of SQL Parameter 4.</param>
        /// <param name="p5Value">Value of SQL Parameter 5.</param>
        /// <param name="p6Value">Value of SQL Parameter 6.</param>
        /// <param name="p7Value">Value of SQL Parameter 7.</param>
        /// <param name="p8Value">Value of SQL Parameter 8.</param>
        /// <param name="p9Value">Value of SQL Parameter 9.</param>
        /// <param name="p10Value">Value of SQL Parameter 10.</param>
        /// <param name="p11Value">Value of SQL Parameter 11.</param>
        /// <param name="p12Value">Value of SQL Parameter 12.</param>
        /// <param name="p13Value">Value of SQL Parameter 13.</param>
        /// <param name="p14Value">Value of SQL Parameter 14.</param>
        /// <param name="p15Value">Value of SQL Parameter 15.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configured default for this program.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Number of rows affected.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        [NotNull]
        public Task<int> ExecuteNonQueryAsync(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), T7 p7Value = default(T7), T8 p8Value = default(T8), T9 p9Value = default(T9), T10 p10Value = default(T10), T11 p11Value = default(T11), T12 p12Value = default(T12), T13 p13Value = default(T13), T14 p14Value = default(T14), T15 p15Value = default(T15), TypeConstraintMode? constraintMode = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.ExecuteNonQueryAsync(c => c.SetParameters(p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, p7Value, p8Value, p9Value, p10Value, p11Value, p12Value, p13Value, p14Value, p15Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), cancellationToken);
        }

        /// <summary>
        /// Executes a Transact-SQL statement against all the connections and returns the number of rows affected.
        /// </summary>
        /// <param name="p1Value">Value of SQL Parameter 1.</param>
        /// <param name="p2Value">Value of SQL Parameter 2.</param>
        /// <param name="p3Value">Value of SQL Parameter 3.</param>
        /// <param name="p4Value">Value of SQL Parameter 4.</param>
        /// <param name="p5Value">Value of SQL Parameter 5.</param>
        /// <param name="p6Value">Value of SQL Parameter 6.</param>
        /// <param name="p7Value">Value of SQL Parameter 7.</param>
        /// <param name="p8Value">Value of SQL Parameter 8.</param>
        /// <param name="p9Value">Value of SQL Parameter 9.</param>
        /// <param name="p10Value">Value of SQL Parameter 10.</param>
        /// <param name="p11Value">Value of SQL Parameter 11.</param>
        /// <param name="p12Value">Value of SQL Parameter 12.</param>
        /// <param name="p13Value">Value of SQL Parameter 13.</param>
        /// <param name="p14Value">Value of SQL Parameter 14.</param>
        /// <param name="p15Value">Value of SQL Parameter 15.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configured default for this program.</param>
        /// <returns>Number of rows affected for each connection.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        [NotNull]
        public IEnumerable<int> ExecuteNonQueryAll(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), T7 p7Value = default(T7), T8 p8Value = default(T8), T9 p9Value = default(T9), T10 p10Value = default(T10), T11 p11Value = default(T11), T12 p12Value = default(T12), T13 p13Value = default(T13), T14 p14Value = default(T14), T15 p15Value = default(T15), TypeConstraintMode? constraintMode = null)
        {
            return this.ExecuteNonQueryAll(c => c.SetParameters(p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, p7Value, p8Value, p9Value, p10Value, p11Value, p12Value, p13Value, p14Value, p15Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)));
        }

        /// <summary>
        /// Executes a Transact-SQL statement against all the connections and returns the number of rows affected.
        /// </summary>
        /// <param name="p1Value">Value of SQL Parameter 1.</param>
        /// <param name="p2Value">Value of SQL Parameter 2.</param>
        /// <param name="p3Value">Value of SQL Parameter 3.</param>
        /// <param name="p4Value">Value of SQL Parameter 4.</param>
        /// <param name="p5Value">Value of SQL Parameter 5.</param>
        /// <param name="p6Value">Value of SQL Parameter 6.</param>
        /// <param name="p7Value">Value of SQL Parameter 7.</param>
        /// <param name="p8Value">Value of SQL Parameter 8.</param>
        /// <param name="p9Value">Value of SQL Parameter 9.</param>
        /// <param name="p10Value">Value of SQL Parameter 10.</param>
        /// <param name="p11Value">Value of SQL Parameter 11.</param>
        /// <param name="p12Value">Value of SQL Parameter 12.</param>
        /// <param name="p13Value">Value of SQL Parameter 13.</param>
        /// <param name="p14Value">Value of SQL Parameter 14.</param>
        /// <param name="p15Value">Value of SQL Parameter 15.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configured default for this program.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Number of rows affected for each connection.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        [NotNull]
        public Task<IEnumerable<int>> ExecuteNonQueryAllAsync(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), T7 p7Value = default(T7), T8 p8Value = default(T8), T9 p9Value = default(T9), T10 p10Value = default(T10), T11 p11Value = default(T11), T12 p12Value = default(T12), T13 p13Value = default(T13), T14 p14Value = default(T14), T15 p15Value = default(T15), TypeConstraintMode? constraintMode = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.ExecuteNonQueryAllAsync(c => c.SetParameters(p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, p7Value, p8Value, p9Value, p10Value, p11Value, p12Value, p13Value, p14Value, p15Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), cancellationToken);
        }

        /// <summary>
        /// Executes the program with the specified parameters.
        /// </summary>
        /// <param name="p1Value">Value of SQL Parameter 1.</param>
        /// <param name="p2Value">Value of SQL Parameter 2.</param>
        /// <param name="p3Value">Value of SQL Parameter 3.</param>
        /// <param name="p4Value">Value of SQL Parameter 4.</param>
        /// <param name="p5Value">Value of SQL Parameter 5.</param>
        /// <param name="p6Value">Value of SQL Parameter 6.</param>
        /// <param name="p7Value">Value of SQL Parameter 7.</param>
        /// <param name="p8Value">Value of SQL Parameter 8.</param>
        /// <param name="p9Value">Value of SQL Parameter 9.</param>
        /// <param name="p10Value">Value of SQL Parameter 10.</param>
        /// <param name="p11Value">Value of SQL Parameter 11.</param>
        /// <param name="p12Value">Value of SQL Parameter 12.</param>
        /// <param name="p13Value">Value of SQL Parameter 13.</param>
        /// <param name="p14Value">Value of SQL Parameter 14.</param>
        /// <param name="p15Value">Value of SQL Parameter 15.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configured default for this program.</param>
        /// <param name="resultAction">The result function.</param>
        /// <param name="behavior">The behaviour.</param>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public void ExecuteReader([NotNull] ResultDelegate resultAction, T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), T7 p7Value = default(T7), T8 p8Value = default(T8), T9 p9Value = default(T9), T10 p10Value = default(T10), T11 p11Value = default(T11), T12 p12Value = default(T12), T13 p13Value = default(T13), T14 p14Value = default(T14), T15 p15Value = default(T15), CommandBehavior behavior = CommandBehavior.Default, TypeConstraintMode? constraintMode = null)
        {
            Contract.Requires(resultAction != null);
            ExecuteReader(c => c.SetParameters(p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, p7Value, p8Value, p9Value, p10Value, p11Value, p12Value, p13Value, p14Value, p15Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), resultAction, behavior);
        }

        /// <summary>
        /// Executes the program with the specified parameters.
        /// </summary>
        /// <param name="p1Value">Value of SQL Parameter 1.</param>
        /// <param name="p2Value">Value of SQL Parameter 2.</param>
        /// <param name="p3Value">Value of SQL Parameter 3.</param>
        /// <param name="p4Value">Value of SQL Parameter 4.</param>
        /// <param name="p5Value">Value of SQL Parameter 5.</param>
        /// <param name="p6Value">Value of SQL Parameter 6.</param>
        /// <param name="p7Value">Value of SQL Parameter 7.</param>
        /// <param name="p8Value">Value of SQL Parameter 8.</param>
        /// <param name="p9Value">Value of SQL Parameter 9.</param>
        /// <param name="p10Value">Value of SQL Parameter 10.</param>
        /// <param name="p11Value">Value of SQL Parameter 11.</param>
        /// <param name="p12Value">Value of SQL Parameter 12.</param>
        /// <param name="p13Value">Value of SQL Parameter 13.</param>
        /// <param name="p14Value">Value of SQL Parameter 14.</param>
        /// <param name="p15Value">Value of SQL Parameter 15.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configured default for this program.</param>
        /// <param name="resultAction">The result function.</param>
        /// <param name="behavior">The behaviour.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        [NotNull]
        public Task ExecuteReaderAsync([NotNull] ResultDelegateAsync resultAction, T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), T7 p7Value = default(T7), T8 p8Value = default(T8), T9 p9Value = default(T9), T10 p10Value = default(T10), T11 p11Value = default(T11), T12 p12Value = default(T12), T13 p13Value = default(T13), T14 p14Value = default(T14), T15 p15Value = default(T15), CommandBehavior behavior = CommandBehavior.Default, TypeConstraintMode? constraintMode = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            Contract.Requires(resultAction != null);
            return this.ExecuteReaderAsync(c => c.SetParameters(p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, p7Value, p8Value, p9Value, p10Value, p11Value, p12Value, p13Value, p14Value, p15Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), resultAction, behavior, cancellationToken);
        }

        /// <summary>
        /// Executes the program with the specified parameters.
        /// </summary>
        /// <param name="p1Value">Value of SQL Parameter 1.</param>
        /// <param name="p2Value">Value of SQL Parameter 2.</param>
        /// <param name="p3Value">Value of SQL Parameter 3.</param>
        /// <param name="p4Value">Value of SQL Parameter 4.</param>
        /// <param name="p5Value">Value of SQL Parameter 5.</param>
        /// <param name="p6Value">Value of SQL Parameter 6.</param>
        /// <param name="p7Value">Value of SQL Parameter 7.</param>
        /// <param name="p8Value">Value of SQL Parameter 8.</param>
        /// <param name="p9Value">Value of SQL Parameter 9.</param>
        /// <param name="p10Value">Value of SQL Parameter 10.</param>
        /// <param name="p11Value">Value of SQL Parameter 11.</param>
        /// <param name="p12Value">Value of SQL Parameter 12.</param>
        /// <param name="p13Value">Value of SQL Parameter 13.</param>
        /// <param name="p14Value">Value of SQL Parameter 14.</param>
        /// <param name="p15Value">Value of SQL Parameter 15.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configured default for this program.</param>
        /// <param name="resultAction">The result function.</param>
        /// <param name="behavior">The behaviour.</param>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public void ExecuteReaderAll([NotNull] ResultDelegate resultAction, T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), T7 p7Value = default(T7), T8 p8Value = default(T8), T9 p9Value = default(T9), T10 p10Value = default(T10), T11 p11Value = default(T11), T12 p12Value = default(T12), T13 p13Value = default(T13), T14 p14Value = default(T14), T15 p15Value = default(T15), CommandBehavior behavior = CommandBehavior.Default, TypeConstraintMode? constraintMode = null)
        {
            Contract.Requires(resultAction != null);
            this.ExecuteReaderAll(c => c.SetParameters(p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, p7Value, p8Value, p9Value, p10Value, p11Value, p12Value, p13Value, p14Value, p15Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), resultAction, behavior);
        }

        /// <summary>
        /// Executes the program with the specified parameters.
        /// </summary>
        /// <param name="p1Value">Value of SQL Parameter 1.</param>
        /// <param name="p2Value">Value of SQL Parameter 2.</param>
        /// <param name="p3Value">Value of SQL Parameter 3.</param>
        /// <param name="p4Value">Value of SQL Parameter 4.</param>
        /// <param name="p5Value">Value of SQL Parameter 5.</param>
        /// <param name="p6Value">Value of SQL Parameter 6.</param>
        /// <param name="p7Value">Value of SQL Parameter 7.</param>
        /// <param name="p8Value">Value of SQL Parameter 8.</param>
        /// <param name="p9Value">Value of SQL Parameter 9.</param>
        /// <param name="p10Value">Value of SQL Parameter 10.</param>
        /// <param name="p11Value">Value of SQL Parameter 11.</param>
        /// <param name="p12Value">Value of SQL Parameter 12.</param>
        /// <param name="p13Value">Value of SQL Parameter 13.</param>
        /// <param name="p14Value">Value of SQL Parameter 14.</param>
        /// <param name="p15Value">Value of SQL Parameter 15.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configured default for this program.</param>
        /// <param name="resultAction">The result function.</param>
        /// <param name="behavior">The behaviour.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        [NotNull]
        public Task ExecuteReaderAllAsync([NotNull] ResultDelegateAsync resultAction, T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), T7 p7Value = default(T7), T8 p8Value = default(T8), T9 p9Value = default(T9), T10 p10Value = default(T10), T11 p11Value = default(T11), T12 p12Value = default(T12), T13 p13Value = default(T13), T14 p14Value = default(T14), T15 p15Value = default(T15), CommandBehavior behavior = CommandBehavior.Default, TypeConstraintMode? constraintMode = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            Contract.Requires(resultAction != null);
            return this.ExecuteReaderAllAsync(c => c.SetParameters(p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, p7Value, p8Value, p9Value, p10Value, p11Value, p12Value, p13Value, p14Value, p15Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), resultAction, behavior, cancellationToken);
        }

        /// <summary>
        /// Executes the program with the specified parameters.
        /// </summary>
        /// <typeparam name="TOut">The type to return from the result function</typeparam>
        /// <param name="p1Value">Value of SQL Parameter 1.</param>
        /// <param name="p2Value">Value of SQL Parameter 2.</param>
        /// <param name="p3Value">Value of SQL Parameter 3.</param>
        /// <param name="p4Value">Value of SQL Parameter 4.</param>
        /// <param name="p5Value">Value of SQL Parameter 5.</param>
        /// <param name="p6Value">Value of SQL Parameter 6.</param>
        /// <param name="p7Value">Value of SQL Parameter 7.</param>
        /// <param name="p8Value">Value of SQL Parameter 8.</param>
        /// <param name="p9Value">Value of SQL Parameter 9.</param>
        /// <param name="p10Value">Value of SQL Parameter 10.</param>
        /// <param name="p11Value">Value of SQL Parameter 11.</param>
        /// <param name="p12Value">Value of SQL Parameter 12.</param>
        /// <param name="p13Value">Value of SQL Parameter 13.</param>
        /// <param name="p14Value">Value of SQL Parameter 14.</param>
        /// <param name="p15Value">Value of SQL Parameter 15.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configured default for this program.</param>
        /// <param name="resultFunc">The result function.</param>
        /// <param name="behavior">The behaviour.</param>
        /// <returns></returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public TOut ExecuteReader<TOut>([NotNull] ResultDelegate<TOut> resultFunc, T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), T7 p7Value = default(T7), T8 p8Value = default(T8), T9 p9Value = default(T9), T10 p10Value = default(T10), T11 p11Value = default(T11), T12 p12Value = default(T12), T13 p13Value = default(T13), T14 p14Value = default(T14), T15 p15Value = default(T15), CommandBehavior behavior = CommandBehavior.Default, TypeConstraintMode? constraintMode = null)
        {
            Contract.Requires(resultFunc != null);
            return this.ExecuteReader(c => c.SetParameters(p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, p7Value, p8Value, p9Value, p10Value, p11Value, p12Value, p13Value, p14Value, p15Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), resultFunc, behavior);
        }

        /// <summary>
        /// Executes the program with the specified parameters.
        /// </summary>
        /// <typeparam name="TOut">The type to return from the result function</typeparam>
        /// <param name="p1Value">Value of SQL Parameter 1.</param>
        /// <param name="p2Value">Value of SQL Parameter 2.</param>
        /// <param name="p3Value">Value of SQL Parameter 3.</param>
        /// <param name="p4Value">Value of SQL Parameter 4.</param>
        /// <param name="p5Value">Value of SQL Parameter 5.</param>
        /// <param name="p6Value">Value of SQL Parameter 6.</param>
        /// <param name="p7Value">Value of SQL Parameter 7.</param>
        /// <param name="p8Value">Value of SQL Parameter 8.</param>
        /// <param name="p9Value">Value of SQL Parameter 9.</param>
        /// <param name="p10Value">Value of SQL Parameter 10.</param>
        /// <param name="p11Value">Value of SQL Parameter 11.</param>
        /// <param name="p12Value">Value of SQL Parameter 12.</param>
        /// <param name="p13Value">Value of SQL Parameter 13.</param>
        /// <param name="p14Value">Value of SQL Parameter 14.</param>
        /// <param name="p15Value">Value of SQL Parameter 15.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configured default for this program.</param>
        /// <param name="resultFunc">The result function.</param>
        /// <param name="behavior">The behaviour.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        [NotNull]
        public Task<TOut> ExecuteReaderAsync<TOut>([NotNull] ResultDelegateAsync<TOut> resultFunc, T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), T7 p7Value = default(T7), T8 p8Value = default(T8), T9 p9Value = default(T9), T10 p10Value = default(T10), T11 p11Value = default(T11), T12 p12Value = default(T12), T13 p13Value = default(T13), T14 p14Value = default(T14), T15 p15Value = default(T15), CommandBehavior behavior = CommandBehavior.Default, TypeConstraintMode? constraintMode = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            Contract.Requires(resultFunc != null);
            return this.ExecuteReaderAsync(c => c.SetParameters(p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, p7Value, p8Value, p9Value, p10Value, p11Value, p12Value, p13Value, p14Value, p15Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), resultFunc, behavior, cancellationToken);
        }

        /// <summary>
        /// Executes the program with the specified parameters.
        /// </summary>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="p1Value">Value of SQL Parameter 1.</param>
        /// <param name="p2Value">Value of SQL Parameter 2.</param>
        /// <param name="p3Value">Value of SQL Parameter 3.</param>
        /// <param name="p4Value">Value of SQL Parameter 4.</param>
        /// <param name="p5Value">Value of SQL Parameter 5.</param>
        /// <param name="p6Value">Value of SQL Parameter 6.</param>
        /// <param name="p7Value">Value of SQL Parameter 7.</param>
        /// <param name="p8Value">Value of SQL Parameter 8.</param>
        /// <param name="p9Value">Value of SQL Parameter 9.</param>
        /// <param name="p10Value">Value of SQL Parameter 10.</param>
        /// <param name="p11Value">Value of SQL Parameter 11.</param>
        /// <param name="p12Value">Value of SQL Parameter 12.</param>
        /// <param name="p13Value">Value of SQL Parameter 13.</param>
        /// <param name="p14Value">Value of SQL Parameter 14.</param>
        /// <param name="p15Value">Value of SQL Parameter 15.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configured default for this program.</param>
        /// <param name="resultFunc">The result function.</param>
        /// <param name="behavior">The behaviour.</param>
        /// <returns></returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        [NotNull]
        public IEnumerable<TOut> ExecuteReaderAll<TOut>([NotNull] ResultDelegate<TOut> resultFunc, T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), T7 p7Value = default(T7), T8 p8Value = default(T8), T9 p9Value = default(T9), T10 p10Value = default(T10), T11 p11Value = default(T11), T12 p12Value = default(T12), T13 p13Value = default(T13), T14 p14Value = default(T14), T15 p15Value = default(T15), CommandBehavior behavior = CommandBehavior.Default, TypeConstraintMode? constraintMode = null)
        {
            Contract.Requires(resultFunc != null);
            return this.ExecuteReaderAll(c => c.SetParameters(p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, p7Value, p8Value, p9Value, p10Value, p11Value, p12Value, p13Value, p14Value, p15Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), resultFunc, behavior);
        }

        /// <summary>
        /// Executes the program with the specified parameters.
        /// </summary>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="p1Value">Value of SQL Parameter 1.</param>
        /// <param name="p2Value">Value of SQL Parameter 2.</param>
        /// <param name="p3Value">Value of SQL Parameter 3.</param>
        /// <param name="p4Value">Value of SQL Parameter 4.</param>
        /// <param name="p5Value">Value of SQL Parameter 5.</param>
        /// <param name="p6Value">Value of SQL Parameter 6.</param>
        /// <param name="p7Value">Value of SQL Parameter 7.</param>
        /// <param name="p8Value">Value of SQL Parameter 8.</param>
        /// <param name="p9Value">Value of SQL Parameter 9.</param>
        /// <param name="p10Value">Value of SQL Parameter 10.</param>
        /// <param name="p11Value">Value of SQL Parameter 11.</param>
        /// <param name="p12Value">Value of SQL Parameter 12.</param>
        /// <param name="p13Value">Value of SQL Parameter 13.</param>
        /// <param name="p14Value">Value of SQL Parameter 14.</param>
        /// <param name="p15Value">Value of SQL Parameter 15.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configured default for this program.</param>
        /// <param name="resultFunc">The result function.</param>
        /// <param name="behavior">The behaviour.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        [NotNull]
        public Task<IEnumerable<TOut>> ExecuteReaderAllAsync<TOut>([NotNull] ResultDelegateAsync<TOut> resultFunc, T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), T7 p7Value = default(T7), T8 p8Value = default(T8), T9 p9Value = default(T9), T10 p10Value = default(T10), T11 p11Value = default(T11), T12 p12Value = default(T12), T13 p13Value = default(T13), T14 p14Value = default(T14), T15 p15Value = default(T15), CommandBehavior behavior = CommandBehavior.Default, TypeConstraintMode? constraintMode = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            Contract.Requires(resultFunc != null);
            return this.ExecuteReaderAllAsync(c => c.SetParameters(p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, p7Value, p8Value, p9Value, p10Value, p11Value, p12Value, p13Value, p14Value, p15Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), resultFunc, behavior, cancellationToken);
        }

        /// <summary>
        /// Executes the program with the specified parameters.
        /// </summary>
        /// <param name="p1Value">Value of SQL Parameter 1.</param>
        /// <param name="p2Value">Value of SQL Parameter 2.</param>
        /// <param name="p3Value">Value of SQL Parameter 3.</param>
        /// <param name="p4Value">Value of SQL Parameter 4.</param>
        /// <param name="p5Value">Value of SQL Parameter 5.</param>
        /// <param name="p6Value">Value of SQL Parameter 6.</param>
        /// <param name="p7Value">Value of SQL Parameter 7.</param>
        /// <param name="p8Value">Value of SQL Parameter 8.</param>
        /// <param name="p9Value">Value of SQL Parameter 9.</param>
        /// <param name="p10Value">Value of SQL Parameter 10.</param>
        /// <param name="p11Value">Value of SQL Parameter 11.</param>
        /// <param name="p12Value">Value of SQL Parameter 12.</param>
        /// <param name="p13Value">Value of SQL Parameter 13.</param>
        /// <param name="p14Value">Value of SQL Parameter 14.</param>
        /// <param name="p15Value">Value of SQL Parameter 15.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configured default for this program.</param>
        /// <param name="resultAction">The result function.</param>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public void ExecuteXmlReader([NotNull] XmlResultDelegate resultAction, T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), T7 p7Value = default(T7), T8 p8Value = default(T8), T9 p9Value = default(T9), T10 p10Value = default(T10), T11 p11Value = default(T11), T12 p12Value = default(T12), T13 p13Value = default(T13), T14 p14Value = default(T14), T15 p15Value = default(T15), TypeConstraintMode? constraintMode = null)
        {
            Contract.Requires(resultAction != null);
            this.ExecuteXmlReader(c => c.SetParameters(p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, p7Value, p8Value, p9Value, p10Value, p11Value, p12Value, p13Value, p14Value, p15Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), resultAction);
        }

        /// <summary>
        /// Executes the program with the specified parameters.
        /// </summary>
        /// <param name="p1Value">Value of SQL Parameter 1.</param>
        /// <param name="p2Value">Value of SQL Parameter 2.</param>
        /// <param name="p3Value">Value of SQL Parameter 3.</param>
        /// <param name="p4Value">Value of SQL Parameter 4.</param>
        /// <param name="p5Value">Value of SQL Parameter 5.</param>
        /// <param name="p6Value">Value of SQL Parameter 6.</param>
        /// <param name="p7Value">Value of SQL Parameter 7.</param>
        /// <param name="p8Value">Value of SQL Parameter 8.</param>
        /// <param name="p9Value">Value of SQL Parameter 9.</param>
        /// <param name="p10Value">Value of SQL Parameter 10.</param>
        /// <param name="p11Value">Value of SQL Parameter 11.</param>
        /// <param name="p12Value">Value of SQL Parameter 12.</param>
        /// <param name="p13Value">Value of SQL Parameter 13.</param>
        /// <param name="p14Value">Value of SQL Parameter 14.</param>
        /// <param name="p15Value">Value of SQL Parameter 15.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configured default for this program.</param>
        /// <param name="resultAction">The result function.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        [NotNull]
        public Task ExecuteXmlReaderAsync([NotNull] XmlResultDelegateAsync resultAction, T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), T7 p7Value = default(T7), T8 p8Value = default(T8), T9 p9Value = default(T9), T10 p10Value = default(T10), T11 p11Value = default(T11), T12 p12Value = default(T12), T13 p13Value = default(T13), T14 p14Value = default(T14), T15 p15Value = default(T15), TypeConstraintMode? constraintMode = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            Contract.Requires(resultAction != null);
            return this.ExecuteXmlReaderAsync(c => c.SetParameters(p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, p7Value, p8Value, p9Value, p10Value, p11Value, p12Value, p13Value, p14Value, p15Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), resultAction, cancellationToken);
        }

        /// <summary>
        /// Executes the program with the specified parameters.
        /// </summary>
        /// <param name="p1Value">Value of SQL Parameter 1.</param>
        /// <param name="p2Value">Value of SQL Parameter 2.</param>
        /// <param name="p3Value">Value of SQL Parameter 3.</param>
        /// <param name="p4Value">Value of SQL Parameter 4.</param>
        /// <param name="p5Value">Value of SQL Parameter 5.</param>
        /// <param name="p6Value">Value of SQL Parameter 6.</param>
        /// <param name="p7Value">Value of SQL Parameter 7.</param>
        /// <param name="p8Value">Value of SQL Parameter 8.</param>
        /// <param name="p9Value">Value of SQL Parameter 9.</param>
        /// <param name="p10Value">Value of SQL Parameter 10.</param>
        /// <param name="p11Value">Value of SQL Parameter 11.</param>
        /// <param name="p12Value">Value of SQL Parameter 12.</param>
        /// <param name="p13Value">Value of SQL Parameter 13.</param>
        /// <param name="p14Value">Value of SQL Parameter 14.</param>
        /// <param name="p15Value">Value of SQL Parameter 15.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configured default for this program.</param>
        /// <param name="resultAction">The result function.</param>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public void ExecuteXmlReaderAll([NotNull] XmlResultDelegate resultAction, T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), T7 p7Value = default(T7), T8 p8Value = default(T8), T9 p9Value = default(T9), T10 p10Value = default(T10), T11 p11Value = default(T11), T12 p12Value = default(T12), T13 p13Value = default(T13), T14 p14Value = default(T14), T15 p15Value = default(T15), TypeConstraintMode? constraintMode = null)
        {
            Contract.Requires(resultAction != null);
            this.ExecuteXmlReaderAll(c => c.SetParameters(p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, p7Value, p8Value, p9Value, p10Value, p11Value, p12Value, p13Value, p14Value, p15Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), resultAction);
        }

        /// <summary>
        /// Executes the program with the specified parameters.
        /// </summary>
        /// <param name="p1Value">Value of SQL Parameter 1.</param>
        /// <param name="p2Value">Value of SQL Parameter 2.</param>
        /// <param name="p3Value">Value of SQL Parameter 3.</param>
        /// <param name="p4Value">Value of SQL Parameter 4.</param>
        /// <param name="p5Value">Value of SQL Parameter 5.</param>
        /// <param name="p6Value">Value of SQL Parameter 6.</param>
        /// <param name="p7Value">Value of SQL Parameter 7.</param>
        /// <param name="p8Value">Value of SQL Parameter 8.</param>
        /// <param name="p9Value">Value of SQL Parameter 9.</param>
        /// <param name="p10Value">Value of SQL Parameter 10.</param>
        /// <param name="p11Value">Value of SQL Parameter 11.</param>
        /// <param name="p12Value">Value of SQL Parameter 12.</param>
        /// <param name="p13Value">Value of SQL Parameter 13.</param>
        /// <param name="p14Value">Value of SQL Parameter 14.</param>
        /// <param name="p15Value">Value of SQL Parameter 15.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configured default for this program.</param>
        /// <param name="resultAction">The result function.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        [NotNull]
        public Task ExecuteXmlReaderAllAsync([NotNull] XmlResultDelegateAsync resultAction, T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), T7 p7Value = default(T7), T8 p8Value = default(T8), T9 p9Value = default(T9), T10 p10Value = default(T10), T11 p11Value = default(T11), T12 p12Value = default(T12), T13 p13Value = default(T13), T14 p14Value = default(T14), T15 p15Value = default(T15), TypeConstraintMode? constraintMode = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            Contract.Requires(resultAction != null);
            return this.ExecuteXmlReaderAllAsync(c => c.SetParameters(p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, p7Value, p8Value, p9Value, p10Value, p11Value, p12Value, p13Value, p14Value, p15Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), resultAction, cancellationToken);
        }

        /// <summary>
        /// Executes the program with the specified parameters.
        /// </summary>
        /// <typeparam name="TOut">The type to return from the result function</typeparam>
        /// <param name="p1Value">Value of SQL Parameter 1.</param>
        /// <param name="p2Value">Value of SQL Parameter 2.</param>
        /// <param name="p3Value">Value of SQL Parameter 3.</param>
        /// <param name="p4Value">Value of SQL Parameter 4.</param>
        /// <param name="p5Value">Value of SQL Parameter 5.</param>
        /// <param name="p6Value">Value of SQL Parameter 6.</param>
        /// <param name="p7Value">Value of SQL Parameter 7.</param>
        /// <param name="p8Value">Value of SQL Parameter 8.</param>
        /// <param name="p9Value">Value of SQL Parameter 9.</param>
        /// <param name="p10Value">Value of SQL Parameter 10.</param>
        /// <param name="p11Value">Value of SQL Parameter 11.</param>
        /// <param name="p12Value">Value of SQL Parameter 12.</param>
        /// <param name="p13Value">Value of SQL Parameter 13.</param>
        /// <param name="p14Value">Value of SQL Parameter 14.</param>
        /// <param name="p15Value">Value of SQL Parameter 15.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configured default for this program.</param>
        /// <param name="resultFunc">The result function.</param>
        /// <returns></returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public TOut ExecuteXmlReader<TOut>([NotNull] XmlResultDelegate<TOut> resultFunc, T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), T7 p7Value = default(T7), T8 p8Value = default(T8), T9 p9Value = default(T9), T10 p10Value = default(T10), T11 p11Value = default(T11), T12 p12Value = default(T12), T13 p13Value = default(T13), T14 p14Value = default(T14), T15 p15Value = default(T15), TypeConstraintMode? constraintMode = null)
        {
            Contract.Requires(resultFunc != null);
            return this.ExecuteXmlReader(c => c.SetParameters(p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, p7Value, p8Value, p9Value, p10Value, p11Value, p12Value, p13Value, p14Value, p15Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), resultFunc);
        }

        /// <summary>
        /// Executes the program with the specified parameters.
        /// </summary>
        /// <typeparam name="TOut">The type to return from the result function</typeparam>
        /// <param name="p1Value">Value of SQL Parameter 1.</param>
        /// <param name="p2Value">Value of SQL Parameter 2.</param>
        /// <param name="p3Value">Value of SQL Parameter 3.</param>
        /// <param name="p4Value">Value of SQL Parameter 4.</param>
        /// <param name="p5Value">Value of SQL Parameter 5.</param>
        /// <param name="p6Value">Value of SQL Parameter 6.</param>
        /// <param name="p7Value">Value of SQL Parameter 7.</param>
        /// <param name="p8Value">Value of SQL Parameter 8.</param>
        /// <param name="p9Value">Value of SQL Parameter 9.</param>
        /// <param name="p10Value">Value of SQL Parameter 10.</param>
        /// <param name="p11Value">Value of SQL Parameter 11.</param>
        /// <param name="p12Value">Value of SQL Parameter 12.</param>
        /// <param name="p13Value">Value of SQL Parameter 13.</param>
        /// <param name="p14Value">Value of SQL Parameter 14.</param>
        /// <param name="p15Value">Value of SQL Parameter 15.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configured default for this program.</param>
        /// <param name="resultFunc">The result function.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        [NotNull]
        public Task<TOut> ExecuteXmlReaderAsync<TOut>([NotNull] XmlResultDelegateAsync<TOut> resultFunc, T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), T7 p7Value = default(T7), T8 p8Value = default(T8), T9 p9Value = default(T9), T10 p10Value = default(T10), T11 p11Value = default(T11), T12 p12Value = default(T12), T13 p13Value = default(T13), T14 p14Value = default(T14), T15 p15Value = default(T15), TypeConstraintMode? constraintMode = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            Contract.Requires(resultFunc != null);
            return this.ExecuteXmlReaderAsync(c => c.SetParameters(p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, p7Value, p8Value, p9Value, p10Value, p11Value, p12Value, p13Value, p14Value, p15Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), resultFunc, cancellationToken);
        }

        /// <summary>
        /// Executes the program with the specified parameters.
        /// </summary>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="p1Value">Value of SQL Parameter 1.</param>
        /// <param name="p2Value">Value of SQL Parameter 2.</param>
        /// <param name="p3Value">Value of SQL Parameter 3.</param>
        /// <param name="p4Value">Value of SQL Parameter 4.</param>
        /// <param name="p5Value">Value of SQL Parameter 5.</param>
        /// <param name="p6Value">Value of SQL Parameter 6.</param>
        /// <param name="p7Value">Value of SQL Parameter 7.</param>
        /// <param name="p8Value">Value of SQL Parameter 8.</param>
        /// <param name="p9Value">Value of SQL Parameter 9.</param>
        /// <param name="p10Value">Value of SQL Parameter 10.</param>
        /// <param name="p11Value">Value of SQL Parameter 11.</param>
        /// <param name="p12Value">Value of SQL Parameter 12.</param>
        /// <param name="p13Value">Value of SQL Parameter 13.</param>
        /// <param name="p14Value">Value of SQL Parameter 14.</param>
        /// <param name="p15Value">Value of SQL Parameter 15.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configured default for this program.</param>
        /// <param name="resultFunc">The result function.</param>
        /// <returns></returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        [NotNull]
        public IEnumerable<TOut> ExecuteXmlReaderAll<TOut>([NotNull] XmlResultDelegate<TOut> resultFunc, T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), T7 p7Value = default(T7), T8 p8Value = default(T8), T9 p9Value = default(T9), T10 p10Value = default(T10), T11 p11Value = default(T11), T12 p12Value = default(T12), T13 p13Value = default(T13), T14 p14Value = default(T14), T15 p15Value = default(T15), TypeConstraintMode? constraintMode = null)
        {
            Contract.Requires(resultFunc != null);
            return this.ExecuteXmlReaderAll(c => c.SetParameters(p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, p7Value, p8Value, p9Value, p10Value, p11Value, p12Value, p13Value, p14Value, p15Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), resultFunc);
        }

        /// <summary>
        /// Executes the program with the specified parameters.
        /// </summary>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="p1Value">Value of SQL Parameter 1.</param>
        /// <param name="p2Value">Value of SQL Parameter 2.</param>
        /// <param name="p3Value">Value of SQL Parameter 3.</param>
        /// <param name="p4Value">Value of SQL Parameter 4.</param>
        /// <param name="p5Value">Value of SQL Parameter 5.</param>
        /// <param name="p6Value">Value of SQL Parameter 6.</param>
        /// <param name="p7Value">Value of SQL Parameter 7.</param>
        /// <param name="p8Value">Value of SQL Parameter 8.</param>
        /// <param name="p9Value">Value of SQL Parameter 9.</param>
        /// <param name="p10Value">Value of SQL Parameter 10.</param>
        /// <param name="p11Value">Value of SQL Parameter 11.</param>
        /// <param name="p12Value">Value of SQL Parameter 12.</param>
        /// <param name="p13Value">Value of SQL Parameter 13.</param>
        /// <param name="p14Value">Value of SQL Parameter 14.</param>
        /// <param name="p15Value">Value of SQL Parameter 15.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configured default for this program.</param>
        /// <param name="resultFunc">The result function.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        [NotNull]
        public Task<IEnumerable<TOut>> ExecuteXmlReaderAllAsync<TOut>([NotNull] XmlResultDelegateAsync<TOut> resultFunc, T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), T7 p7Value = default(T7), T8 p8Value = default(T8), T9 p9Value = default(T9), T10 p10Value = default(T10), T11 p11Value = default(T11), T12 p12Value = default(T12), T13 p13Value = default(T13), T14 p14Value = default(T14), T15 p15Value = default(T15), TypeConstraintMode? constraintMode = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            Contract.Requires(resultFunc != null);
            return this.ExecuteXmlReaderAllAsync(c => c.SetParameters(p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, p7Value, p8Value, p9Value, p10Value, p11Value, p12Value, p13Value, p14Value, p15Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), resultFunc, cancellationToken);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.Name;
        }
    }
    #endregion
}

namespace WebApplications.Utilities.Database.Configuration
{
    #region Extensions to DatabasesConfiguration
    /// <summary>
    /// Used to specify database configuration.
    /// </summary>
    /// <remarks></remarks>
    public partial class DatabasesConfiguration : ConfigurationSection<DatabasesConfiguration>
    {    
        /// <summary>
        /// Gets the SQL program with the specified name and parameters, respecting the active configured options.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <typeparam name="T3">The type of parameter 3.</typeparam>
        /// <typeparam name="T4">The type of parameter 4.</typeparam>
        /// <typeparam name="T5">The type of parameter 5.</typeparam>
        /// <typeparam name="T6">The type of parameter 6.</typeparam>
        /// <typeparam name="T7">The type of parameter 7.</typeparam>
        /// <typeparam name="T8">The type of parameter 8.</typeparam>
        /// <typeparam name="T9">The type of parameter 9.</typeparam>
        /// <typeparam name="T10">The type of parameter 10.</typeparam>
        /// <typeparam name="T11">The type of parameter 11.</typeparam>
        /// <typeparam name="T12">The type of parameter 12.</typeparam>
        /// <typeparam name="T13">The type of parameter 13.</typeparam>
        /// <typeparam name="T14">The type of parameter 14.</typeparam>
        /// <typeparam name="T15">The type of parameter 15.</typeparam>
        /// <param name="database">The database id.</param>
        /// <param name="name">The name.</param>
        /// <param name="p1Name">Name of parameter 1.</param>
        /// <param name="p2Name">Name of parameter 2.</param>
        /// <param name="p3Name">Name of parameter 3.</param>
        /// <param name="p4Name">Name of parameter 4.</param>
        /// <param name="p5Name">Name of parameter 5.</param>
        /// <param name="p6Name">Name of parameter 6.</param>
        /// <param name="p7Name">Name of parameter 7.</param>
        /// <param name="p8Name">Name of parameter 8.</param>
        /// <param name="p9Name">Name of parameter 9.</param>
        /// <param name="p10Name">Name of parameter 10.</param>
        /// <param name="p11Name">Name of parameter 11.</param>
        /// <param name="p12Name">Name of parameter 12.</param>
        /// <param name="p13Name">Name of parameter 13.</param>
        /// <param name="p14Name">Name of parameter 14.</param>
        /// <param name="p15Name">Name of parameter 15.</param>
        /// <param name="ignoreValidationErrors">if set to <see langword="true"/> will ignore validation errors regardless of configuration.</param>
        /// <param name="checkOrder">if set to <see langword="true"/> will check parameter order matches regardless of configuration.</param>
        /// <param name="defaultCommandTimeout">The default command timeout, if set will override the configuration.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configuration.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An awaitable task, resulting in a <see cref="SqlProgram&lt;T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15&gt;"/>.</returns>
        [NotNull]
        public static Task<SqlProgram<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>> GetConfiguredSqlProgram<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
            [NotNull] string database,
            [NotNull] string name,
            [NotNull] string p1Name, 
            [NotNull] string p2Name, 
            [NotNull] string p3Name, 
            [NotNull] string p4Name, 
            [NotNull] string p5Name, 
            [NotNull] string p6Name, 
            [NotNull] string p7Name, 
            [NotNull] string p8Name, 
            [NotNull] string p9Name, 
            [NotNull] string p10Name, 
            [NotNull] string p11Name, 
            [NotNull] string p12Name, 
            [NotNull] string p13Name, 
            [NotNull] string p14Name, 
            [NotNull] string p15Name,
            bool ignoreValidationErrors = false,
            bool checkOrder = false,
            TimeSpan? defaultCommandTimeout = null,
            TypeConstraintMode? constraintMode = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Contract.Requires(database != null);
            Contract.Requires(name != null);
            Contract.Requires(p1Name != null);
            Contract.Requires(p2Name != null);
            Contract.Requires(p3Name != null);
            Contract.Requires(p4Name != null);
            Contract.Requires(p5Name != null);
            Contract.Requires(p6Name != null);
            Contract.Requires(p7Name != null);
            Contract.Requires(p8Name != null);
            Contract.Requires(p9Name != null);
            Contract.Requires(p10Name != null);
            Contract.Requires(p11Name != null);
            Contract.Requires(p12Name != null);
            Contract.Requires(p13Name != null);
            Contract.Requires(p14Name != null);
            Contract.Requires(p15Name != null);
            return Active.GetSqlProgram<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(database, name, p1Name, p2Name, p3Name, p4Name, p5Name, p6Name, p7Name, p8Name, p9Name, p10Name, p11Name, p12Name, p13Name, p14Name, p15Name, ignoreValidationErrors, checkOrder, defaultCommandTimeout, constraintMode, cancellationToken);
        }

        /// <summary>
        /// Gets the SQL program with the specified name and parameters, respecting configured options.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <typeparam name="T3">The type of parameter 3.</typeparam>
        /// <typeparam name="T4">The type of parameter 4.</typeparam>
        /// <typeparam name="T5">The type of parameter 5.</typeparam>
        /// <typeparam name="T6">The type of parameter 6.</typeparam>
        /// <typeparam name="T7">The type of parameter 7.</typeparam>
        /// <typeparam name="T8">The type of parameter 8.</typeparam>
        /// <typeparam name="T9">The type of parameter 9.</typeparam>
        /// <typeparam name="T10">The type of parameter 10.</typeparam>
        /// <typeparam name="T11">The type of parameter 11.</typeparam>
        /// <typeparam name="T12">The type of parameter 12.</typeparam>
        /// <typeparam name="T13">The type of parameter 13.</typeparam>
        /// <typeparam name="T14">The type of parameter 14.</typeparam>
        /// <typeparam name="T15">The type of parameter 15.</typeparam>
        /// <param name="database">The database id.</param>
        /// <param name="name">The name.</param>
        /// <param name="p1Name">Name of parameter 1.</param>
        /// <param name="p2Name">Name of parameter 2.</param>
        /// <param name="p3Name">Name of parameter 3.</param>
        /// <param name="p4Name">Name of parameter 4.</param>
        /// <param name="p5Name">Name of parameter 5.</param>
        /// <param name="p6Name">Name of parameter 6.</param>
        /// <param name="p7Name">Name of parameter 7.</param>
        /// <param name="p8Name">Name of parameter 8.</param>
        /// <param name="p9Name">Name of parameter 9.</param>
        /// <param name="p10Name">Name of parameter 10.</param>
        /// <param name="p11Name">Name of parameter 11.</param>
        /// <param name="p12Name">Name of parameter 12.</param>
        /// <param name="p13Name">Name of parameter 13.</param>
        /// <param name="p14Name">Name of parameter 14.</param>
        /// <param name="p15Name">Name of parameter 15.</param>
        /// <param name="ignoreValidationErrors">if set to <see langword="true"/> will ignore validation errors regardless of configuration.</param>
        /// <param name="checkOrder">if set to <see langword="true"/> will check parameter order matches regardless of configuration.</param>
        /// <param name="defaultCommandTimeout">The default command timeout, if set will override the configuration.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configuration.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An awaitable task, resulting in a <see cref="SqlProgram&lt;T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15&gt;"/>.</returns>
        [NotNull]
        public Task<SqlProgram<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>> GetSqlProgram<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
            [NotNull] string database, 
            [NotNull] string name,
            [NotNull] string p1Name, 
            [NotNull] string p2Name, 
            [NotNull] string p3Name, 
            [NotNull] string p4Name, 
            [NotNull] string p5Name, 
            [NotNull] string p6Name, 
            [NotNull] string p7Name, 
            [NotNull] string p8Name, 
            [NotNull] string p9Name, 
            [NotNull] string p10Name, 
            [NotNull] string p11Name, 
            [NotNull] string p12Name, 
            [NotNull] string p13Name, 
            [NotNull] string p14Name, 
            [NotNull] string p15Name,
            bool ignoreValidationErrors = false,
            bool checkOrder = false,
            TimeSpan? defaultCommandTimeout = null,
            TypeConstraintMode? constraintMode = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Contract.Requires(database != null);
            Contract.Requires(name != null);
            Contract.Requires(p1Name != null);
            Contract.Requires(p2Name != null);
            Contract.Requires(p3Name != null);
            Contract.Requires(p4Name != null);
            Contract.Requires(p5Name != null);
            Contract.Requires(p6Name != null);
            Contract.Requires(p7Name != null);
            Contract.Requires(p8Name != null);
            Contract.Requires(p9Name != null);
            Contract.Requires(p10Name != null);
            Contract.Requires(p11Name != null);
            Contract.Requires(p12Name != null);
            Contract.Requires(p13Name != null);
            Contract.Requires(p14Name != null);
            Contract.Requires(p15Name != null);
            // We have to find the database otherwise we cannot get a load balanced connection.
            DatabaseElement db = Databases[database];
            if ((db == null) ||
                (!db.Enabled))
                {
                    return TaskResult<SqlProgram<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>>.FromException(
                        new LoggingException(
                            () => Resources.DatabaseConfiguration_GetSqlProgram_DatabaseIdNotFound,
                            database));
                }

            return db.GetSqlProgram<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(name, p1Name, p2Name, p3Name, p4Name, p5Name, p6Name, p7Name, p8Name, p9Name, p10Name, p11Name, p12Name, p13Name, p14Name, p15Name, ignoreValidationErrors, checkOrder, defaultCommandTimeout, constraintMode, cancellationToken);
        }
    }
    #endregion
    
    #region Extensions to DatabaseElement
    /// <summary>
    /// Used to specify database configuration.
    /// </summary>
    /// <remarks></remarks>
    public partial class DatabaseElement : Utilities.Configuration.ConfigurationElement
    {
        /// <summary>
        /// Gets the SQL program with the specified name and parameters, respecting configured options.
        /// </summary>
        /// <typeparam name="T1">The type of parameter 1.</typeparam>
        /// <typeparam name="T2">The type of parameter 2.</typeparam>
        /// <typeparam name="T3">The type of parameter 3.</typeparam>
        /// <typeparam name="T4">The type of parameter 4.</typeparam>
        /// <typeparam name="T5">The type of parameter 5.</typeparam>
        /// <typeparam name="T6">The type of parameter 6.</typeparam>
        /// <typeparam name="T7">The type of parameter 7.</typeparam>
        /// <typeparam name="T8">The type of parameter 8.</typeparam>
        /// <typeparam name="T9">The type of parameter 9.</typeparam>
        /// <typeparam name="T10">The type of parameter 10.</typeparam>
        /// <typeparam name="T11">The type of parameter 11.</typeparam>
        /// <typeparam name="T12">The type of parameter 12.</typeparam>
        /// <typeparam name="T13">The type of parameter 13.</typeparam>
        /// <typeparam name="T14">The type of parameter 14.</typeparam>
        /// <typeparam name="T15">The type of parameter 15.</typeparam>
        /// <param name="name">The name.</param>
        /// <param name="p1Name">Name of parameter 1.</param>
        /// <param name="p2Name">Name of parameter 2.</param>
        /// <param name="p3Name">Name of parameter 3.</param>
        /// <param name="p4Name">Name of parameter 4.</param>
        /// <param name="p5Name">Name of parameter 5.</param>
        /// <param name="p6Name">Name of parameter 6.</param>
        /// <param name="p7Name">Name of parameter 7.</param>
        /// <param name="p8Name">Name of parameter 8.</param>
        /// <param name="p9Name">Name of parameter 9.</param>
        /// <param name="p10Name">Name of parameter 10.</param>
        /// <param name="p11Name">Name of parameter 11.</param>
        /// <param name="p12Name">Name of parameter 12.</param>
        /// <param name="p13Name">Name of parameter 13.</param>
        /// <param name="p14Name">Name of parameter 14.</param>
        /// <param name="p15Name">Name of parameter 15.</param>
        /// <param name="ignoreValidationErrors">if set to <see langword="true"/> will ignore validation errors regardless of configuration.</param>
        /// <param name="checkOrder">if set to <see langword="true"/> will check parameter order matches regardless of configuration.</param>
        /// <param name="defaultCommandTimeout">The default command timeout, if set will override the configuration.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configuration.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An awaitable task, resulting in a <see cref="SqlProgram&lt;T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15&gt;"/>.</returns>
        [NotNull]
        public async Task<SqlProgram<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>> GetSqlProgram<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
            [NotNull] string name,
            [NotNull] string p1Name, 
            [NotNull] string p2Name, 
            [NotNull] string p3Name, 
            [NotNull] string p4Name, 
            [NotNull] string p5Name, 
            [NotNull] string p6Name, 
            [NotNull] string p7Name, 
            [NotNull] string p8Name, 
            [NotNull] string p9Name, 
            [NotNull] string p10Name, 
            [NotNull] string p11Name, 
            [NotNull] string p12Name, 
            [NotNull] string p13Name, 
            [NotNull] string p14Name, 
            [NotNull] string p15Name,
            bool? ignoreValidationErrors = null,
            bool? checkOrder = null,
            TimeSpan? defaultCommandTimeout = null,
            TypeConstraintMode? constraintMode = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Contract.Requires(name != null);
            Contract.Requires(p1Name != null);
            Contract.Requires(p2Name != null);
            Contract.Requires(p3Name != null);
            Contract.Requires(p4Name != null);
            Contract.Requires(p5Name != null);
            Contract.Requires(p6Name != null);
            Contract.Requires(p7Name != null);
            Contract.Requires(p8Name != null);
            Contract.Requires(p9Name != null);
            Contract.Requires(p10Name != null);
            Contract.Requires(p11Name != null);
            Contract.Requires(p12Name != null);
            Contract.Requires(p13Name != null);
            Contract.Requires(p14Name != null);
            Contract.Requires(p15Name != null);
            // Grab the default load balanced connection for the database.
            LoadBalancedConnectionElement connectionElement = this.Connections.FirstOrDefault(c => c.Enabled);

            if (connectionElement == null)
                throw new LoggingException(() => Resources.DatabaseElement_GetSqlProgram_Unknown_Database, this.Id);
            
            // Look for program mapping information
            ProgramElement prog = this.Programs[name];
            if (prog != null)
            {
                // Check for name mapping
                if (!String.IsNullOrWhiteSpace(prog.MapTo))
                    name = prog.MapTo;

                // Set options if not already set.
                ignoreValidationErrors = ignoreValidationErrors ?? prog.IgnoreValidationErrors;
                checkOrder = checkOrder ?? prog.CheckOrder;
                defaultCommandTimeout = defaultCommandTimeout ?? prog.DefaultCommandTimeout;
                constraintMode = constraintMode ?? prog.ConstraintMode;

                if (!String.IsNullOrEmpty(prog.Connection))
                {
                    connectionElement = this.Connections[prog.Connection];
                    if ((connectionElement == null) ||
                        (!connectionElement.Enabled))
                        throw new LoggingException(() => Resources.DatabaseElement_GetSqlProgram_Unknown_Database_Program,
                            prog.Connection, this.Id, name);
                }
                
                // Check for parameter mappings
                ParameterElement param;
                param = prog.Parameters[p1Name];
                if (param != null) {
                    if (String.IsNullOrWhiteSpace(param.MapTo))
                        throw new LoggingException(() => Resources.DatabaseElement_GetSqlProgram_Invalid_Mapping,
                            p1Name, prog.Name);
                
                    p1Name = param.MapTo;
                }
                param = prog.Parameters[p2Name];
                if (param != null) {
                    if (String.IsNullOrWhiteSpace(param.MapTo))
                        throw new LoggingException(() => Resources.DatabaseElement_GetSqlProgram_Invalid_Mapping,
                            p2Name, prog.Name);
                
                    p2Name = param.MapTo;
                }
                param = prog.Parameters[p3Name];
                if (param != null) {
                    if (String.IsNullOrWhiteSpace(param.MapTo))
                        throw new LoggingException(() => Resources.DatabaseElement_GetSqlProgram_Invalid_Mapping,
                            p3Name, prog.Name);
                
                    p3Name = param.MapTo;
                }
                param = prog.Parameters[p4Name];
                if (param != null) {
                    if (String.IsNullOrWhiteSpace(param.MapTo))
                        throw new LoggingException(() => Resources.DatabaseElement_GetSqlProgram_Invalid_Mapping,
                            p4Name, prog.Name);
                
                    p4Name = param.MapTo;
                }
                param = prog.Parameters[p5Name];
                if (param != null) {
                    if (String.IsNullOrWhiteSpace(param.MapTo))
                        throw new LoggingException(() => Resources.DatabaseElement_GetSqlProgram_Invalid_Mapping,
                            p5Name, prog.Name);
                
                    p5Name = param.MapTo;
                }
                param = prog.Parameters[p6Name];
                if (param != null) {
                    if (String.IsNullOrWhiteSpace(param.MapTo))
                        throw new LoggingException(() => Resources.DatabaseElement_GetSqlProgram_Invalid_Mapping,
                            p6Name, prog.Name);
                
                    p6Name = param.MapTo;
                }
                param = prog.Parameters[p7Name];
                if (param != null) {
                    if (String.IsNullOrWhiteSpace(param.MapTo))
                        throw new LoggingException(() => Resources.DatabaseElement_GetSqlProgram_Invalid_Mapping,
                            p7Name, prog.Name);
                
                    p7Name = param.MapTo;
                }
                param = prog.Parameters[p8Name];
                if (param != null) {
                    if (String.IsNullOrWhiteSpace(param.MapTo))
                        throw new LoggingException(() => Resources.DatabaseElement_GetSqlProgram_Invalid_Mapping,
                            p8Name, prog.Name);
                
                    p8Name = param.MapTo;
                }
                param = prog.Parameters[p9Name];
                if (param != null) {
                    if (String.IsNullOrWhiteSpace(param.MapTo))
                        throw new LoggingException(() => Resources.DatabaseElement_GetSqlProgram_Invalid_Mapping,
                            p9Name, prog.Name);
                
                    p9Name = param.MapTo;
                }
                param = prog.Parameters[p10Name];
                if (param != null) {
                    if (String.IsNullOrWhiteSpace(param.MapTo))
                        throw new LoggingException(() => Resources.DatabaseElement_GetSqlProgram_Invalid_Mapping,
                            p10Name, prog.Name);
                
                    p10Name = param.MapTo;
                }
                param = prog.Parameters[p11Name];
                if (param != null) {
                    if (String.IsNullOrWhiteSpace(param.MapTo))
                        throw new LoggingException(() => Resources.DatabaseElement_GetSqlProgram_Invalid_Mapping,
                            p11Name, prog.Name);
                
                    p11Name = param.MapTo;
                }
                param = prog.Parameters[p12Name];
                if (param != null) {
                    if (String.IsNullOrWhiteSpace(param.MapTo))
                        throw new LoggingException(() => Resources.DatabaseElement_GetSqlProgram_Invalid_Mapping,
                            p12Name, prog.Name);
                
                    p12Name = param.MapTo;
                }
                param = prog.Parameters[p13Name];
                if (param != null) {
                    if (String.IsNullOrWhiteSpace(param.MapTo))
                        throw new LoggingException(() => Resources.DatabaseElement_GetSqlProgram_Invalid_Mapping,
                            p13Name, prog.Name);
                
                    p13Name = param.MapTo;
                }
                param = prog.Parameters[p14Name];
                if (param != null) {
                    if (String.IsNullOrWhiteSpace(param.MapTo))
                        throw new LoggingException(() => Resources.DatabaseElement_GetSqlProgram_Invalid_Mapping,
                            p14Name, prog.Name);
                
                    p14Name = param.MapTo;
                }
                param = prog.Parameters[p15Name];
                if (param != null) {
                    if (String.IsNullOrWhiteSpace(param.MapTo))
                        throw new LoggingException(() => Resources.DatabaseElement_GetSqlProgram_Invalid_Mapping,
                            p15Name, prog.Name);
                
                    p15Name = param.MapTo;
                }
            }
            
            if (constraintMode == null) constraintMode = TypeConstraintMode.Warn;

            LoadBalancedConnection connection = await connectionElement.GetLoadBalancedConnection(cancellationToken).ConfigureAwait(false);

            return await SqlProgram<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.Create(connection, name, p1Name, p2Name, p3Name, p4Name, p5Name, p6Name, p7Name, p8Name, p9Name, p10Name, p11Name, p12Name, p13Name, p14Name, p15Name, ignoreValidationErrors.Value, checkOrder.Value, defaultCommandTimeout, (TypeConstraintMode) constraintMode, cancellationToken);
        }
    }
    #endregion
}
#endregion
