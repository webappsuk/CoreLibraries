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
        /// <param name="p1Value">Value of SQL Parameter 1.</param>
        /// <param name="p2Value">Value of SQL Parameter 2.</param>
        /// <param name="p3Value">Value of SQL Parameter 3.</param>
        /// <param name="p4Value">Value of SQL Parameter 4.</param>
        /// <param name="p5Value">Value of SQL Parameter 5.</param>
        /// <param name="p6Value">Value of SQL Parameter 6.</param>
        /// <param name="mode">The constraint mode.</param>
        /// <returns>The parameters that were set</returns>
        public IEnumerable<SqlParameter> SetParameters<T1, T2, T3, T4, T5, T6>(T1 p1Value, T2 p2Value, T3 p3Value, T4 p4Value, T5 p5Value, T6 p6Value, TypeConstraintMode mode = TypeConstraintMode.Warn)
        {
            SqlProgramParameter[] parameters = _program.Definition.Parameters.ToArray();
            int pCount = parameters.GetLength(0);
            if (pCount < 6)
                throw new LoggingException(
                        "Too many parameters supplied for the '{0}' program, which only accepts '{1} parameter(s) but was supplied with '6'.",
                        LogLevel.Critical,
                        _program.Name,
                        pCount);

            List<SqlParameter> sqlParameters = new List<SqlParameter>(2);
            SqlParameter parameter;
            SqlProgramParameter programParameter;
            int index;
        
            // Find or create SQL Parameter 1.
            programParameter = parameters[0];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p1Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 2.
            programParameter = parameters[1];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p2Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 3.
            programParameter = parameters[2];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p3Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 4.
            programParameter = parameters[3];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p4Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 5.
            programParameter = parameters[4];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p5Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 6.
            programParameter = parameters[5];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p6Value, mode);
            sqlParameters.Add(parameter);
        
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
        /// <param name="names">The enumeration of parameters names.</param>
        /// <param name="p1Value">Value of SQL Parameter 1.</param>
        /// <param name="p2Value">Value of SQL Parameter 2.</param>
        /// <param name="p3Value">Value of SQL Parameter 3.</param>
        /// <param name="p4Value">Value of SQL Parameter 4.</param>
        /// <param name="p5Value">Value of SQL Parameter 5.</param>
        /// <param name="p6Value">Value of SQL Parameter 6.</param>
        /// <param name="mode">The constraint mode.</param>
        /// <returns>The parameters that were set</returns>
        public IEnumerable<SqlParameter> SetParameters<T1, T2, T3, T4, T5, T6>(IEnumerable<string> names, T1 p1Value, T2 p2Value, T3 p3Value, T4 p4Value, T5 p5Value, T6 p6Value, TypeConstraintMode mode = TypeConstraintMode.Warn)
        {
            if ((names == null) || (names.Count() != 6))
                throw new LoggingException(
                        "Wrong number of parameter names supplied for the '{0}' program, which expected '6' name(s) but was supplied with '{1}.",
                        LogLevel.Critical,
                        _program.Name,
                        names == null ? 0 : names.Count());

            SqlProgramParameter[] parameters = names.Select(
                    n =>
                        {
                            n = n.ToLower(); // Find parameter definition
                            SqlProgramParameter parameterDefinition;
                            if (!_program.Definition.TryGetParameter(n, out parameterDefinition))
                                throw new LoggingException(
                                        "The SQL Program '{0}' does not have a '{1}' parameter.",
                                        LogLevel.Critical,
                                        _program.Name,
                                        n);
                            return parameterDefinition;
                        }).ToArray();

            int pCount = parameters.GetLength(0);
            if (pCount < 6)
                throw new LoggingException(
                        "Too many parameters supplied for the '{0}' program, which only accepts '{1} parameter(s) but was supplied with '6'.",
                        LogLevel.Critical,
                        _program.Name,
                        pCount);

            List<SqlParameter> sqlParameters = new List<SqlParameter>(2);
            SqlParameter parameter;
            SqlProgramParameter programParameter;
            int index;
            
            // Find or create SQL Parameter 1.
            programParameter = parameters[0];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p1Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 2.
            programParameter = parameters[1];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p2Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 3.
            programParameter = parameters[2];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p3Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 4.
            programParameter = parameters[3];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p4Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 5.
            programParameter = parameters[4];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p5Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 6.
            programParameter = parameters[5];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p6Value, mode);
            sqlParameters.Add(parameter);
        
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
        /// <param name="parameters">The enumeration of parameters to set.</param>
        /// <param name="p1Value">Value of SQL Parameter 1.</param>
        /// <param name="p2Value">Value of SQL Parameter 2.</param>
        /// <param name="p3Value">Value of SQL Parameter 3.</param>
        /// <param name="p4Value">Value of SQL Parameter 4.</param>
        /// <param name="p5Value">Value of SQL Parameter 5.</param>
        /// <param name="p6Value">Value of SQL Parameter 6.</param>
        /// <param name="mode">The constraint mode.</param>
        /// <returns>The parameters that were set</returns>
        public IEnumerable<SqlParameter> SetParameters<T1, T2, T3, T4, T5, T6>(IEnumerable<SqlProgramParameter> parameters, T1 p1Value, T2 p2Value, T3 p3Value, T4 p4Value, T5 p5Value, T6 p6Value, TypeConstraintMode mode = TypeConstraintMode.Warn)
        {
            if ((parameters == null) || (parameters.Count() != 6))
                throw new LoggingException(
                        "Wrong number of parameter supplied for the '{0}' program, which expected '6' parameter(s) but was supplied with '{1}.",
                        LogLevel.Critical,
                        _program.Name,
                        parameters == null ? 0 : parameters.Count());

            SqlProgramParameter[] parametersArray = parameters.ToArray();

            int pCount = parametersArray.GetLength(0);
            if (pCount < 6)
                throw new LoggingException(
                        "Too many parameters supplied for the '{0}' program, which only accepts '{1} parameter(s) but was supplied with '6'.",
                        LogLevel.Critical,
                        _program.Name,
                        pCount);

            List<SqlParameter> sqlParameters = new List<SqlParameter>(2);
            SqlParameter parameter;
            SqlProgramParameter programParameter;
            int index;
            
            // Find or create SQL Parameter 1.
            programParameter = parametersArray[0];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p1Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 2.
            programParameter = parametersArray[1];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p2Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 3.
            programParameter = parametersArray[2];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p3Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 4.
            programParameter = parametersArray[3];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p4Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 5.
            programParameter = parametersArray[4];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p5Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 6.
            programParameter = parametersArray[5];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p6Value, mode);
            sqlParameters.Add(parameter);
        
            // Return parameters that were set
            return sqlParameters;
        }
    }
    #endregion

    #region SqlProgam<T1, T2, T3, T4, T5, T6>
    /// <summary>
    ///   Used to create an object for easy calling of stored procedures or functions in a database.
    /// </summary>
    public class SqlProgram<T1, T2, T3, T4, T5, T6> : SqlProgram
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlProgram&lt;T1, T2, T3, T4, T5, T6&gt;"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="name">The name of the stored procedure or function.</param>
        /// <param name="ignoreValidationErrors">if set to <see langword="true"/> does not throw validation errors (records them instead).</param>
        /// <param name="defaultCommandTimeout">The optional default command timeout, which will be used whenever this command is executed synchronously.
        /// Defaults to 30s.</param>
        /// <param name="constraintMode">The constraint mode.</param>
        public SqlProgram(
            string connectionString, 
            string name,
            bool ignoreValidationErrors = false,
            TimeSpan? defaultCommandTimeout = null,
            TypeConstraintMode constraintMode = TypeConstraintMode.Warn)
            : base(new LoadBalancedConnection(connectionString), name, ignoreValidationErrors, defaultCommandTimeout, constraintMode, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlProgram&lt;T1, T2, T3, T4, T5, T6&gt;"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="name">The name.</param>
        /// <param name="ignoreValidationErrors">if set to <see langword="true"/> does not throw validation errors (records them instead).</param>
        /// <param name="defaultCommandTimeout">The optional default command timeout, which will be used whenever this command is executed synchronously.
        /// Defaults to 30s.</param>
        /// <param name="constraintMode">The constraint mode.</param>
        public SqlProgram(
            LoadBalancedConnection connection,
            string name,
            bool ignoreValidationErrors = false,
            TimeSpan? defaultCommandTimeout = null,
            TypeConstraintMode constraintMode = TypeConstraintMode.Warn)
            : base(connection, name, ignoreValidationErrors, defaultCommandTimeout, constraintMode, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlProgram&lt;T1, T2, T3, T4, T5, T6&gt;"/> class.
        /// </summary>
        /// <param name="sqlProgram">The SQL program.</param>
        /// <param name="ignoreValidationErrors">if set to <see langword="true"/> does not throw validation errors (records them instead).</param>
        /// <param name="defaultCommandTimeout">The optional default command timeout, which will be used whenever this command is executed synchronously.
        /// Defaults to existing programs default.</param>
        /// <param name="constraintMode">The constraint mode.</param>
        public SqlProgram(
            SqlProgram sqlProgram,
            bool ignoreValidationErrors = false,
            TimeSpan? defaultCommandTimeout = null,
            TypeConstraintMode constraintMode = TypeConstraintMode.Warn)
            : base(sqlProgram, ignoreValidationErrors, defaultCommandTimeout, constraintMode, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlProgram&lt;T1, T2, T3, T4, T5, T6&gt;"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="name">The name of the stored procedure or function.</param>
        /// <param name="p1Name">Name of parameter 1.</param>
        /// <param name="p2Name">Name of parameter 2.</param>
        /// <param name="p3Name">Name of parameter 3.</param>
        /// <param name="p4Name">Name of parameter 4.</param>
        /// <param name="p5Name">Name of parameter 5.</param>
        /// <param name="p6Name">Name of parameter 6.</param>
        /// <param name="ignoreValidationErrors">if set to <see langword="true"/> does not throw validation errors (records them instead).</param>
        /// <param name="checkOrder">if set to <c>true</c> checks the parameter order matches.</param>
        /// <param name="defaultCommandTimeout">The optional default command timeout, which will be used whenever this command is executed synchronously.
        /// Defaults to 30s.</param>
        /// <param name="constraintMode">The constraint mode.</param>
        public SqlProgram(
            string connectionString,
            string name,
            string p1Name, string p2Name, string p3Name, string p4Name, string p5Name, string p6Name,
            bool ignoreValidationErrors = false,
            bool checkOrder = false,
            TimeSpan? defaultCommandTimeout = null,
            TypeConstraintMode constraintMode = TypeConstraintMode.Warn)
            : base(new LoadBalancedConnection(connectionString), name, ignoreValidationErrors, checkOrder, defaultCommandTimeout, constraintMode, new List<string>{ p1Name, p2Name, p3Name, p4Name, p5Name, p6Name }, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlProgram&lt;T1, T2, T3, T4, T5, T6&gt;"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="name">The name.</param>
        /// <param name="p1Name">Name of parameter 1.</param>
        /// <param name="p2Name">Name of parameter 2.</param>
        /// <param name="p3Name">Name of parameter 3.</param>
        /// <param name="p4Name">Name of parameter 4.</param>
        /// <param name="p5Name">Name of parameter 5.</param>
        /// <param name="p6Name">Name of parameter 6.</param>
        /// <param name="ignoreValidationErrors">if set to <see langword="true"/> does not throw validation errors (records them instead).</param>
        /// <param name="checkOrder">if set to <c>true</c> checks the parameter order matches.</param>
        /// <param name="defaultCommandTimeout">The optional default command timeout, which will be used whenever this command is executed synchronously.
        /// Defaults to 30s.</param>
        /// <param name="constraintMode">The constraint mode.</param>
        public SqlProgram(
            LoadBalancedConnection connection,
            string name,
            string p1Name, string p2Name, string p3Name, string p4Name, string p5Name, string p6Name,
            bool ignoreValidationErrors = false,
            bool checkOrder = false,
            TimeSpan? defaultCommandTimeout = null,
            TypeConstraintMode constraintMode = TypeConstraintMode.Warn)
            : base(connection, name, ignoreValidationErrors, checkOrder, defaultCommandTimeout, constraintMode, new List<string>{ p1Name, p2Name, p3Name, p4Name, p5Name, p6Name }, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlProgram&lt;T1, T2, T3, T4, T5, T6&gt;"/> class.
        /// </summary>
        /// <param name="sqlProgram">The SQL program.</param>
        /// <param name="p1Name">Name of parameter 1.</param>
        /// <param name="p2Name">Name of parameter 2.</param>
        /// <param name="p3Name">Name of parameter 3.</param>
        /// <param name="p4Name">Name of parameter 4.</param>
        /// <param name="p5Name">Name of parameter 5.</param>
        /// <param name="p6Name">Name of parameter 6.</param>
        /// <param name="ignoreValidationErrors">if set to <see langword="true"/> does not throw validation errors (records them instead).</param>
        /// <param name="checkOrder">if set to <c>true</c> checks the parameter order matches.</param>
        /// <param name="defaultCommandTimeout">The optional default command timeout, which will be used whenever this command is executed synchronously.
        /// Defaults to existing programs default.</param>
        /// <param name="constraintMode">The constraint mode.</param>
        public SqlProgram(
            SqlProgram sqlProgram,
            string p1Name, string p2Name, string p3Name, string p4Name, string p5Name, string p6Name,
            bool ignoreValidationErrors = false,
            bool checkOrder = false,
            TimeSpan? defaultCommandTimeout = null,
            TypeConstraintMode constraintMode = TypeConstraintMode.Warn)
            : base(sqlProgram, ignoreValidationErrors, checkOrder, defaultCommandTimeout, constraintMode, new List<string>{ p1Name, p2Name, p3Name, p4Name, p5Name, p6Name }, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6))
        {
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
        /// <param name="constraintMode">The constraint mode, if set will override the configurated default for this program.</param>
        /// <returns>The scalar value.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public TOut ExecuteScalar<TOut>(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), TypeConstraintMode? constraintMode = null)
        {
            return this.ExecuteScalar<TOut>(c => c.SetParameters(ProgramParameters, p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)));
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
        /// <param name="constraintMode">The constraint mode, if set will override the configurated default for this program.</param>
        /// <param name="state">The state.</param>
        /// <returns>The scalar value.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public async Task<TOut> ExecuteScalarAsync<TOut>(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), TypeConstraintMode? constraintMode = null, object state = null)
        {
            return await this.ExecuteScalarAsync<TOut>(c => c.SetParameters(ProgramParameters, p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), state);
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
        /// <param name="constraintMode">The constraint mode, if set will override the configurated default for this program.</param>
        /// <returns>The scalar value for each connection.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public IEnumerable<TOut> ExecuteScalarAll<TOut>(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), TypeConstraintMode? constraintMode = null)
        {
            return this.ExecuteScalarAll<TOut>(c => c.SetParameters(ProgramParameters, p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)));
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
        /// <param name="constraintMode">The constraint mode, if set will override the configurated default for this program.</param>
        /// <param name="state">The state.</param>
        /// <returns>The scalar value for each connection.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public async Task<IEnumerable<TOut>> ExecuteScalarAllAsync<TOut>(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), TypeConstraintMode? constraintMode = null, object state = null)
        {
            return await this.ExecuteScalarAllAsync<TOut>(c => c.SetParameters(ProgramParameters, p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), state);
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
        /// <param name="constraintMode">The constraint mode, if set will override the configurated default for this program.</param>
        /// <returns>Number of rows affected.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public int ExecuteNonQuery(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), TypeConstraintMode? constraintMode = null)
        {
            return this.ExecuteNonQuery(c => c.SetParameters(ProgramParameters, p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)));
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
        /// <param name="constraintMode">The constraint mode, if set will override the configurated default for this program.</param>
        /// <param name="state">The state.</param>
        /// <returns>Number of rows affected.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public async Task<int> ExecuteNonQueryAsync(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), TypeConstraintMode? constraintMode = null, object state = null)
        {
            return await this.ExecuteNonQueryAsync(c => c.SetParameters(ProgramParameters, p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), state);
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
        /// <param name="constraintMode">The constraint mode, if set will override the configurated default for this program.</param>
        /// <returns>Number of rows affected for each connection.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public IEnumerable<int> ExecuteNonQueryAll(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), TypeConstraintMode? constraintMode = null)
        {
            return this.ExecuteNonQueryAll(c => c.SetParameters(ProgramParameters, p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)));
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
        /// <param name="constraintMode">The constraint mode, if set will override the configurated default for this program.</param>
        /// <param name="state">The state.</param>
        /// <returns>Number of rows affected for each connection.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public async Task<IEnumerable<int>> ExecuteNonQueryAllAsync(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), TypeConstraintMode? constraintMode = null, object state = null)
        {
            return await this.ExecuteNonQueryAllAsync(c => c.SetParameters(ProgramParameters, p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), state);
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
        /// <param name="constraintMode">The constraint mode, if set will override the configurated default for this program.</param>
        /// <param name="resultAction">The result function.</param>
        /// <param name="behavior">The behaviour.</param>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public void ExecuteReader(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), Action<SqlDataReader> resultAction = null, CommandBehavior behavior = CommandBehavior.Default, TypeConstraintMode? constraintMode = null)
        {
            this.ExecuteReader(c => c.SetParameters(ProgramParameters, p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), resultAction, behavior);
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
        /// <param name="constraintMode">The constraint mode, if set will override the configurated default for this program.</param>
        /// <param name="resultAction">The result function.</param>
        /// <param name="behavior">The behaviour.</param>
        /// <param name="state">The state.</param>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public async Task ExecuteReaderAsync(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), Action<SqlDataReader> resultAction = null, CommandBehavior behavior = CommandBehavior.Default, TypeConstraintMode? constraintMode = null, object state = null)
        {
            await this.ExecuteReaderAsync(c => c.SetParameters(ProgramParameters, p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), resultAction, behavior, state);
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
        /// <param name="constraintMode">The constraint mode, if set will override the configurated default for this program.</param>
        /// <param name="resultAction">The result function.</param>
        /// <param name="behavior">The behaviour.</param>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public void ExecuteReaderAll(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), Action<SqlDataReader> resultAction = null, CommandBehavior behavior = CommandBehavior.Default, TypeConstraintMode? constraintMode = null)
        {
            this.ExecuteReaderAll(c => c.SetParameters(ProgramParameters, p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), resultAction, behavior);
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
        /// <param name="constraintMode">The constraint mode, if set will override the configurated default for this program.</param>
        /// <param name="resultAction">The result function.</param>
        /// <param name="behavior">The behaviour.</param>
        /// <param name="state">The state.</param>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public async Task ExecuteReaderAllAsync(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), Action<SqlDataReader> resultAction = null, CommandBehavior behavior = CommandBehavior.Default, TypeConstraintMode? constraintMode = null, object state = null)
        {
            await this.ExecuteReaderAllAsync(c => c.SetParameters(ProgramParameters, p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), resultAction, behavior, state);
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
        /// <param name="constraintMode">The constraint mode, if set will override the configurated default for this program.</param>
        /// <param name="resultFunc">The result function.</param>
        /// <param name="behavior">The behaviour.</param>
        /// <returns></returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public TOut ExecuteReader<TOut>(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), Func<SqlDataReader, TOut> resultFunc = null, CommandBehavior behavior = CommandBehavior.Default, TypeConstraintMode? constraintMode = null)
        {
            return this.ExecuteReader(c => c.SetParameters(ProgramParameters, p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), resultFunc, behavior);
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
        /// <param name="constraintMode">The constraint mode, if set will override the configurated default for this program.</param>
        /// <param name="resultFunc">The result function.</param>
        /// <param name="behavior">The behaviour.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public async Task<TOut> ExecuteReaderAsync<TOut>(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), Func<SqlDataReader, TOut> resultFunc = null, CommandBehavior behavior = CommandBehavior.Default, TypeConstraintMode? constraintMode = null, object state = null)
        {
            return await this.ExecuteReaderAsync(c => c.SetParameters(ProgramParameters, p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), resultFunc, behavior, state);
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
        /// <param name="constraintMode">The constraint mode, if set will override the configurated default for this program.</param>
        /// <param name="resultFunc">The result function.</param>
        /// <param name="behavior">The behaviour.</param>
        /// <returns></returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public IEnumerable<TOut> ExecuteReaderAll<TOut>(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), Func<SqlDataReader, TOut> resultFunc = null, CommandBehavior behavior = CommandBehavior.Default, TypeConstraintMode? constraintMode = null)
        {
            return this.ExecuteReaderAll(c => c.SetParameters(ProgramParameters, p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), resultFunc, behavior);
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
        /// <param name="constraintMode">The constraint mode, if set will override the configurated default for this program.</param>
        /// <param name="resultFunc">The result function.</param>
        /// <param name="behavior">The behaviour.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public async Task<IEnumerable<TOut>> ExecuteReaderAllAsync<TOut>(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), Func<SqlDataReader, TOut> resultFunc = null, CommandBehavior behavior = CommandBehavior.Default, TypeConstraintMode? constraintMode = null, object state = null)
        {
            return await this.ExecuteReaderAllAsync(c => c.SetParameters(ProgramParameters, p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), resultFunc, behavior, state);
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
        /// <param name="constraintMode">The constraint mode, if set will override the configurated default for this program.</param>
        /// <param name="resultAction">The result function.</param>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public void ExecuteXmlReader(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), Action<XmlReader> resultAction = null, TypeConstraintMode? constraintMode = null)
        {
            this.ExecuteXmlReader(c => c.SetParameters(ProgramParameters, p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), resultAction);
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
        /// <param name="constraintMode">The constraint mode, if set will override the configurated default for this program.</param>
        /// <param name="resultAction">The result function.</param>
        /// <param name="state">The state.</param>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public async Task ExecuteXmlReaderAsync(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), Action<XmlReader> resultAction = null, TypeConstraintMode? constraintMode = null, object state = null)
        {
            await this.ExecuteXmlReaderAsync(c => c.SetParameters(ProgramParameters, p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), resultAction, state);
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
        /// <param name="constraintMode">The constraint mode, if set will override the configurated default for this program.</param>
        /// <param name="resultAction">The result function.</param>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public void ExecuteXmlReaderAll(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), Action<XmlReader> resultAction = null, TypeConstraintMode? constraintMode = null)
        {
            this.ExecuteXmlReaderAll(c => c.SetParameters(ProgramParameters, p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), resultAction);
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
        /// <param name="constraintMode">The constraint mode, if set will override the configurated default for this program.</param>
        /// <param name="resultAction">The result function.</param>
        /// <param name="state">The state.</param>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public async Task ExecuteXmlReaderAllAsync(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), Action<XmlReader> resultAction = null, TypeConstraintMode? constraintMode = null, object state = null)
        {
            await this.ExecuteXmlReaderAllAsync(c => c.SetParameters(ProgramParameters, p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), resultAction, state);
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
        /// <param name="constraintMode">The constraint mode, if set will override the configurated default for this program.</param>
        /// <param name="resultFunc">The result function.</param>
        /// <returns></returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public TOut ExecuteXmlReader<TOut>(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), Func<XmlReader, TOut> resultFunc = null, TypeConstraintMode? constraintMode = null)
        {
            return this.ExecuteXmlReader(c => c.SetParameters(ProgramParameters, p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), resultFunc);
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
        /// <param name="constraintMode">The constraint mode, if set will override the configurated default for this program.</param>
        /// <param name="resultFunc">The result function.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public async Task<TOut> ExecuteXmlReaderAsync<TOut>(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), Func<XmlReader, TOut> resultFunc = null, TypeConstraintMode? constraintMode = null, object state = null)
        {
            return await this.ExecuteXmlReaderAsync(c => c.SetParameters(ProgramParameters, p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), resultFunc, state);
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
        /// <param name="constraintMode">The constraint mode, if set will override the configurated default for this program.</param>
        /// <param name="resultFunc">The result function.</param>
        /// <returns></returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public IEnumerable<TOut> ExecuteXmlReaderAll<TOut>(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), Func<XmlReader, TOut> resultFunc = null, TypeConstraintMode? constraintMode = null)
        {
            return this.ExecuteXmlReaderAll(c => c.SetParameters(ProgramParameters, p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), resultFunc);
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
        /// <param name="constraintMode">The constraint mode, if set will override the configurated default for this program.</param>
        /// <param name="resultFunc">The result function.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public async Task<IEnumerable<TOut>> ExecuteXmlReaderAllAsync<TOut>(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), Func<XmlReader, TOut> resultFunc = null, TypeConstraintMode? constraintMode = null, object state = null)
        {
            return await this.ExecuteXmlReaderAllAsync(c => c.SetParameters(ProgramParameters, p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), resultFunc, state);
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
        /// <param name="database">The database id.</param>
        /// <param name="name">The name.</param>
        /// <param name="p1Name">Name of parameter 1.</param>
        /// <param name="p2Name">Name of parameter 2.</param>
        /// <param name="p3Name">Name of parameter 3.</param>
        /// <param name="p4Name">Name of parameter 4.</param>
        /// <param name="p5Name">Name of parameter 5.</param>
        /// <param name="p6Name">Name of parameter 6.</param>
        /// <param name="ignoreValidationErrors">if set to <see langword="true"/> will ignore validation errors regardless of configuration.</param>
        /// <param name="checkOrder">if set to <see langword="true"/> will check parameter order matches regardless of configuration.</param>
        /// <param name="defaultCommandTimeout">The default command timeout, if set will override the configuration.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configuration.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        [NotNull]
        public static SqlProgram<T1, T2, T3, T4, T5, T6> GetConfiguredSqlProgram<T1, T2, T3, T4, T5, T6>(
            [NotNull] string database,
            [NotNull] string name,
            string p1Name, string p2Name, string p3Name, string p4Name, string p5Name, string p6Name,
            bool ignoreValidationErrors = false,
            bool checkOrder = false,
            TimeSpan? defaultCommandTimeout = null,
            TypeConstraintMode? constraintMode = null
            )
        {
            return Active.GetSqlProgram<T1, T2, T3, T4, T5, T6>(database, name, p1Name, p2Name, p3Name, p4Name, p5Name, p6Name,
                ignoreValidationErrors, checkOrder, defaultCommandTimeout, constraintMode);
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
        /// <param name="database">The database id.</param>
        /// <param name="name">The name.</param>
        /// <param name="p1Name">Name of parameter 1.</param>
        /// <param name="p2Name">Name of parameter 2.</param>
        /// <param name="p3Name">Name of parameter 3.</param>
        /// <param name="p4Name">Name of parameter 4.</param>
        /// <param name="p5Name">Name of parameter 5.</param>
        /// <param name="p6Name">Name of parameter 6.</param>
        /// <param name="ignoreValidationErrors">if set to <see langword="true"/> will ignore validation errors regardless of configuration.</param>
        /// <param name="checkOrder">if set to <see langword="true"/> will check parameter order matches regardless of configuration.</param>
        /// <param name="defaultCommandTimeout">The default command timeout, if set will override the configuration.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configuration.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        [NotNull]
        public SqlProgram<T1, T2, T3, T4, T5, T6> GetSqlProgram<T1, T2, T3, T4, T5, T6>(
            [NotNull] string database, 
            [NotNull] string name,
            string p1Name, string p2Name, string p3Name, string p4Name, string p5Name, string p6Name,
            bool ignoreValidationErrors = false,
            bool checkOrder = false,
            TimeSpan? defaultCommandTimeout = null,
            TypeConstraintMode? constraintMode = null
            )
        {
            // We have to find the database otherwise we cannot get a load balanced connection.
            DatabaseElement db = Databases[database];
            if ((db == null) || (!db.Enabled))
                throw new LoggingException("The database with id '{0}' could not be found in the configuration.",
                                           LogLevel.Error, database);

            return db.GetSqlProgram<T1, T2, T3, T4, T5, T6>(name, p1Name, p2Name, p3Name, p4Name, p5Name, p6Name, ignoreValidationErrors, checkOrder, defaultCommandTimeout, constraintMode);
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
        /// <param name="name">The name.</param>
        /// <param name="p1Name">Name of parameter 1.</param>
        /// <param name="p2Name">Name of parameter 2.</param>
        /// <param name="p3Name">Name of parameter 3.</param>
        /// <param name="p4Name">Name of parameter 4.</param>
        /// <param name="p5Name">Name of parameter 5.</param>
        /// <param name="p6Name">Name of parameter 6.</param>
        /// <param name="ignoreValidationErrors">if set to <see langword="true"/> will ignore validation errors regardless of configuration.</param>
        /// <param name="checkOrder">if set to <see langword="true"/> will check parameter order matches regardless of configuration.</param>
        /// <param name="defaultCommandTimeout">The default command timeout, if set will override the configuration.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configuration.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        [NotNull]
        public SqlProgram<T1, T2, T3, T4, T5, T6> GetSqlProgram<T1, T2, T3, T4, T5, T6>(
            [NotNull] string name,
            string p1Name, string p2Name, string p3Name, string p4Name, string p5Name, string p6Name,
            bool ignoreValidationErrors = false,
            bool checkOrder = false,
            TimeSpan? defaultCommandTimeout = null,
            TypeConstraintMode? constraintMode = null
            )
        {
            // Grab the default load balanced connection for the database.
            LoadBalancedConnectionElement connection = this.Connections.FirstOrDefault(c => c.Enabled);

            if (connection == null)
                throw new LoggingException(
                    "Could not find a default load balanced connection for the database with id '{0}'.",
                    LogLevel.Error, this.Id);
            
            // Look for program mapping information
            ProgramElement prog = this.Programs[name];
            if (prog != null)
            {
                // Check for name mapping
                if (!String.IsNullOrWhiteSpace(prog.MapTo))
                    name = prog.MapTo;

                // Set options if not already set.
                ignoreValidationErrors = ignoreValidationErrors && prog.IgnoreValidationErrors;
                checkOrder = checkOrder && prog.CheckOrder;
                defaultCommandTimeout = defaultCommandTimeout ?? prog.DefaultCommandTimeout;
                constraintMode = constraintMode ?? prog.ConstraintMode;

                if (!String.IsNullOrEmpty(prog.Connection))
                {
                    connection = this.Connections[prog.Connection];
                    if ((connection == null) ||
                        (!connection.Enabled))
                        throw new LoggingException(
                            "Could not find a load balanced connection with id '{0}' for the database with id '{1}' for use with the '{2}' SqlProgram.",
                            LogLevel.Error, prog.Connection, this.Id, name);
                }
                
                // Check for parameter mappings
                ParameterElement param;
                param = prog.Parameters[p1Name];
                if (param != null) {
                    if (String.IsNullOrWhiteSpace(param.MapTo))
                        throw new LoggingException(
                            "Must specify a valid mapping for '{0}' parameter on '{1}' program.",
                            LogLevel.Error, p1Name, prog.Name);
                
                    p1Name = param.MapTo;
                }
                param = prog.Parameters[p2Name];
                if (param != null) {
                    if (String.IsNullOrWhiteSpace(param.MapTo))
                        throw new LoggingException(
                            "Must specify a valid mapping for '{0}' parameter on '{1}' program.",
                            LogLevel.Error, p2Name, prog.Name);
                
                    p2Name = param.MapTo;
                }
                param = prog.Parameters[p3Name];
                if (param != null) {
                    if (String.IsNullOrWhiteSpace(param.MapTo))
                        throw new LoggingException(
                            "Must specify a valid mapping for '{0}' parameter on '{1}' program.",
                            LogLevel.Error, p3Name, prog.Name);
                
                    p3Name = param.MapTo;
                }
                param = prog.Parameters[p4Name];
                if (param != null) {
                    if (String.IsNullOrWhiteSpace(param.MapTo))
                        throw new LoggingException(
                            "Must specify a valid mapping for '{0}' parameter on '{1}' program.",
                            LogLevel.Error, p4Name, prog.Name);
                
                    p4Name = param.MapTo;
                }
                param = prog.Parameters[p5Name];
                if (param != null) {
                    if (String.IsNullOrWhiteSpace(param.MapTo))
                        throw new LoggingException(
                            "Must specify a valid mapping for '{0}' parameter on '{1}' program.",
                            LogLevel.Error, p5Name, prog.Name);
                
                    p5Name = param.MapTo;
                }
                param = prog.Parameters[p6Name];
                if (param != null) {
                    if (String.IsNullOrWhiteSpace(param.MapTo))
                        throw new LoggingException(
                            "Must specify a valid mapping for '{0}' parameter on '{1}' program.",
                            LogLevel.Error, p6Name, prog.Name);
                
                    p6Name = param.MapTo;
                }
            }
			
            if (constraintMode == null) constraintMode = TypeConstraintMode.Warn;

            return new SqlProgram<T1, T2, T3, T4, T5, T6>(connection, name, p1Name, p2Name, p3Name, p4Name, p5Name, p6Name, ignoreValidationErrors, checkOrder, defaultCommandTimeout, (TypeConstraintMode) constraintMode);
        }
    }
    #endregion
}
#endregion
        