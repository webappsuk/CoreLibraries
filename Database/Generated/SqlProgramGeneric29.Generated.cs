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
        /// <typeparam name="T7">The type of parameter 7.</typeparam>
        /// <typeparam name="T8">The type of parameter 8.</typeparam>
        /// <typeparam name="T9">The type of parameter 9.</typeparam>
        /// <typeparam name="T10">The type of parameter 10.</typeparam>
        /// <typeparam name="T11">The type of parameter 11.</typeparam>
        /// <typeparam name="T12">The type of parameter 12.</typeparam>
        /// <typeparam name="T13">The type of parameter 13.</typeparam>
        /// <typeparam name="T14">The type of parameter 14.</typeparam>
        /// <typeparam name="T15">The type of parameter 15.</typeparam>
        /// <typeparam name="T16">The type of parameter 16.</typeparam>
        /// <typeparam name="T17">The type of parameter 17.</typeparam>
        /// <typeparam name="T18">The type of parameter 18.</typeparam>
        /// <typeparam name="T19">The type of parameter 19.</typeparam>
        /// <typeparam name="T20">The type of parameter 20.</typeparam>
        /// <typeparam name="T21">The type of parameter 21.</typeparam>
        /// <typeparam name="T22">The type of parameter 22.</typeparam>
        /// <typeparam name="T23">The type of parameter 23.</typeparam>
        /// <typeparam name="T24">The type of parameter 24.</typeparam>
        /// <typeparam name="T25">The type of parameter 25.</typeparam>
        /// <typeparam name="T26">The type of parameter 26.</typeparam>
        /// <typeparam name="T27">The type of parameter 27.</typeparam>
        /// <typeparam name="T28">The type of parameter 28.</typeparam>
        /// <typeparam name="T29">The type of parameter 29.</typeparam>
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
        /// <param name="p16Value">Value of SQL Parameter 16.</param>
        /// <param name="p17Value">Value of SQL Parameter 17.</param>
        /// <param name="p18Value">Value of SQL Parameter 18.</param>
        /// <param name="p19Value">Value of SQL Parameter 19.</param>
        /// <param name="p20Value">Value of SQL Parameter 20.</param>
        /// <param name="p21Value">Value of SQL Parameter 21.</param>
        /// <param name="p22Value">Value of SQL Parameter 22.</param>
        /// <param name="p23Value">Value of SQL Parameter 23.</param>
        /// <param name="p24Value">Value of SQL Parameter 24.</param>
        /// <param name="p25Value">Value of SQL Parameter 25.</param>
        /// <param name="p26Value">Value of SQL Parameter 26.</param>
        /// <param name="p27Value">Value of SQL Parameter 27.</param>
        /// <param name="p28Value">Value of SQL Parameter 28.</param>
        /// <param name="p29Value">Value of SQL Parameter 29.</param>
        /// <param name="mode">The constraint mode.</param>
        /// <returns>The parameters that were set</returns>
        public IEnumerable<SqlParameter> SetParameters<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29>(T1 p1Value, T2 p2Value, T3 p3Value, T4 p4Value, T5 p5Value, T6 p6Value, T7 p7Value, T8 p8Value, T9 p9Value, T10 p10Value, T11 p11Value, T12 p12Value, T13 p13Value, T14 p14Value, T15 p15Value, T16 p16Value, T17 p17Value, T18 p18Value, T19 p19Value, T20 p20Value, T21 p21Value, T22 p22Value, T23 p23Value, T24 p24Value, T25 p25Value, T26 p26Value, T27 p27Value, T28 p28Value, T29 p29Value, TypeConstraintMode mode = TypeConstraintMode.Warn)
        {
            SqlProgramParameter[] parameters = _program.Definition.Parameters.ToArray();
            int pCount = parameters.GetLength(0);
            if (pCount < 29)
                throw new LoggingException(
                        "Too many parameters supplied for the '{0}' program, which only accepts '{1} parameter(s) but was supplied with '29'.",
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
        
            // Find or create SQL Parameter 7.
            programParameter = parameters[6];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p7Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 8.
            programParameter = parameters[7];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p8Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 9.
            programParameter = parameters[8];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p9Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 10.
            programParameter = parameters[9];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p10Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 11.
            programParameter = parameters[10];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p11Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 12.
            programParameter = parameters[11];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p12Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 13.
            programParameter = parameters[12];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p13Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 14.
            programParameter = parameters[13];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p14Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 15.
            programParameter = parameters[14];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p15Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 16.
            programParameter = parameters[15];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p16Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 17.
            programParameter = parameters[16];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p17Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 18.
            programParameter = parameters[17];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p18Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 19.
            programParameter = parameters[18];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p19Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 20.
            programParameter = parameters[19];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p20Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 21.
            programParameter = parameters[20];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p21Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 22.
            programParameter = parameters[21];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p22Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 23.
            programParameter = parameters[22];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p23Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 24.
            programParameter = parameters[23];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p24Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 25.
            programParameter = parameters[24];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p25Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 26.
            programParameter = parameters[25];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p26Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 27.
            programParameter = parameters[26];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p27Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 28.
            programParameter = parameters[27];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p28Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 29.
            programParameter = parameters[28];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p29Value, mode);
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
        /// <typeparam name="T7">The type of parameter 7.</typeparam>
        /// <typeparam name="T8">The type of parameter 8.</typeparam>
        /// <typeparam name="T9">The type of parameter 9.</typeparam>
        /// <typeparam name="T10">The type of parameter 10.</typeparam>
        /// <typeparam name="T11">The type of parameter 11.</typeparam>
        /// <typeparam name="T12">The type of parameter 12.</typeparam>
        /// <typeparam name="T13">The type of parameter 13.</typeparam>
        /// <typeparam name="T14">The type of parameter 14.</typeparam>
        /// <typeparam name="T15">The type of parameter 15.</typeparam>
        /// <typeparam name="T16">The type of parameter 16.</typeparam>
        /// <typeparam name="T17">The type of parameter 17.</typeparam>
        /// <typeparam name="T18">The type of parameter 18.</typeparam>
        /// <typeparam name="T19">The type of parameter 19.</typeparam>
        /// <typeparam name="T20">The type of parameter 20.</typeparam>
        /// <typeparam name="T21">The type of parameter 21.</typeparam>
        /// <typeparam name="T22">The type of parameter 22.</typeparam>
        /// <typeparam name="T23">The type of parameter 23.</typeparam>
        /// <typeparam name="T24">The type of parameter 24.</typeparam>
        /// <typeparam name="T25">The type of parameter 25.</typeparam>
        /// <typeparam name="T26">The type of parameter 26.</typeparam>
        /// <typeparam name="T27">The type of parameter 27.</typeparam>
        /// <typeparam name="T28">The type of parameter 28.</typeparam>
        /// <typeparam name="T29">The type of parameter 29.</typeparam>
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
        /// <param name="p16Value">Value of SQL Parameter 16.</param>
        /// <param name="p17Value">Value of SQL Parameter 17.</param>
        /// <param name="p18Value">Value of SQL Parameter 18.</param>
        /// <param name="p19Value">Value of SQL Parameter 19.</param>
        /// <param name="p20Value">Value of SQL Parameter 20.</param>
        /// <param name="p21Value">Value of SQL Parameter 21.</param>
        /// <param name="p22Value">Value of SQL Parameter 22.</param>
        /// <param name="p23Value">Value of SQL Parameter 23.</param>
        /// <param name="p24Value">Value of SQL Parameter 24.</param>
        /// <param name="p25Value">Value of SQL Parameter 25.</param>
        /// <param name="p26Value">Value of SQL Parameter 26.</param>
        /// <param name="p27Value">Value of SQL Parameter 27.</param>
        /// <param name="p28Value">Value of SQL Parameter 28.</param>
        /// <param name="p29Value">Value of SQL Parameter 29.</param>
        /// <param name="mode">The constraint mode.</param>
        /// <returns>The parameters that were set</returns>
        public IEnumerable<SqlParameter> SetParameters<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29>(IEnumerable<string> names, T1 p1Value, T2 p2Value, T3 p3Value, T4 p4Value, T5 p5Value, T6 p6Value, T7 p7Value, T8 p8Value, T9 p9Value, T10 p10Value, T11 p11Value, T12 p12Value, T13 p13Value, T14 p14Value, T15 p15Value, T16 p16Value, T17 p17Value, T18 p18Value, T19 p19Value, T20 p20Value, T21 p21Value, T22 p22Value, T23 p23Value, T24 p24Value, T25 p25Value, T26 p26Value, T27 p27Value, T28 p28Value, T29 p29Value, TypeConstraintMode mode = TypeConstraintMode.Warn)
        {
            if ((names == null) || (names.Count() != 29))
                throw new LoggingException(
                        "Wrong number of parameter names supplied for the '{0}' program, which expected '29' name(s) but was supplied with '{1}.",
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
            if (pCount < 29)
                throw new LoggingException(
                        "Too many parameters supplied for the '{0}' program, which only accepts '{1} parameter(s) but was supplied with '29'.",
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
        
            // Find or create SQL Parameter 7.
            programParameter = parameters[6];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p7Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 8.
            programParameter = parameters[7];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p8Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 9.
            programParameter = parameters[8];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p9Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 10.
            programParameter = parameters[9];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p10Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 11.
            programParameter = parameters[10];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p11Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 12.
            programParameter = parameters[11];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p12Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 13.
            programParameter = parameters[12];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p13Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 14.
            programParameter = parameters[13];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p14Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 15.
            programParameter = parameters[14];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p15Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 16.
            programParameter = parameters[15];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p16Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 17.
            programParameter = parameters[16];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p17Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 18.
            programParameter = parameters[17];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p18Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 19.
            programParameter = parameters[18];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p19Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 20.
            programParameter = parameters[19];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p20Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 21.
            programParameter = parameters[20];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p21Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 22.
            programParameter = parameters[21];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p22Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 23.
            programParameter = parameters[22];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p23Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 24.
            programParameter = parameters[23];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p24Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 25.
            programParameter = parameters[24];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p25Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 26.
            programParameter = parameters[25];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p26Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 27.
            programParameter = parameters[26];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p27Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 28.
            programParameter = parameters[27];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p28Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 29.
            programParameter = parameters[28];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p29Value, mode);
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
        /// <typeparam name="T7">The type of parameter 7.</typeparam>
        /// <typeparam name="T8">The type of parameter 8.</typeparam>
        /// <typeparam name="T9">The type of parameter 9.</typeparam>
        /// <typeparam name="T10">The type of parameter 10.</typeparam>
        /// <typeparam name="T11">The type of parameter 11.</typeparam>
        /// <typeparam name="T12">The type of parameter 12.</typeparam>
        /// <typeparam name="T13">The type of parameter 13.</typeparam>
        /// <typeparam name="T14">The type of parameter 14.</typeparam>
        /// <typeparam name="T15">The type of parameter 15.</typeparam>
        /// <typeparam name="T16">The type of parameter 16.</typeparam>
        /// <typeparam name="T17">The type of parameter 17.</typeparam>
        /// <typeparam name="T18">The type of parameter 18.</typeparam>
        /// <typeparam name="T19">The type of parameter 19.</typeparam>
        /// <typeparam name="T20">The type of parameter 20.</typeparam>
        /// <typeparam name="T21">The type of parameter 21.</typeparam>
        /// <typeparam name="T22">The type of parameter 22.</typeparam>
        /// <typeparam name="T23">The type of parameter 23.</typeparam>
        /// <typeparam name="T24">The type of parameter 24.</typeparam>
        /// <typeparam name="T25">The type of parameter 25.</typeparam>
        /// <typeparam name="T26">The type of parameter 26.</typeparam>
        /// <typeparam name="T27">The type of parameter 27.</typeparam>
        /// <typeparam name="T28">The type of parameter 28.</typeparam>
        /// <typeparam name="T29">The type of parameter 29.</typeparam>
        /// <param name="parameters">The enumeration of parameters to set.</param>
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
        /// <param name="p16Value">Value of SQL Parameter 16.</param>
        /// <param name="p17Value">Value of SQL Parameter 17.</param>
        /// <param name="p18Value">Value of SQL Parameter 18.</param>
        /// <param name="p19Value">Value of SQL Parameter 19.</param>
        /// <param name="p20Value">Value of SQL Parameter 20.</param>
        /// <param name="p21Value">Value of SQL Parameter 21.</param>
        /// <param name="p22Value">Value of SQL Parameter 22.</param>
        /// <param name="p23Value">Value of SQL Parameter 23.</param>
        /// <param name="p24Value">Value of SQL Parameter 24.</param>
        /// <param name="p25Value">Value of SQL Parameter 25.</param>
        /// <param name="p26Value">Value of SQL Parameter 26.</param>
        /// <param name="p27Value">Value of SQL Parameter 27.</param>
        /// <param name="p28Value">Value of SQL Parameter 28.</param>
        /// <param name="p29Value">Value of SQL Parameter 29.</param>
        /// <param name="mode">The constraint mode.</param>
        /// <returns>The parameters that were set</returns>
        public IEnumerable<SqlParameter> SetParameters<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29>(IEnumerable<SqlProgramParameter> parameters, T1 p1Value, T2 p2Value, T3 p3Value, T4 p4Value, T5 p5Value, T6 p6Value, T7 p7Value, T8 p8Value, T9 p9Value, T10 p10Value, T11 p11Value, T12 p12Value, T13 p13Value, T14 p14Value, T15 p15Value, T16 p16Value, T17 p17Value, T18 p18Value, T19 p19Value, T20 p20Value, T21 p21Value, T22 p22Value, T23 p23Value, T24 p24Value, T25 p25Value, T26 p26Value, T27 p27Value, T28 p28Value, T29 p29Value, TypeConstraintMode mode = TypeConstraintMode.Warn)
        {
            if ((parameters == null) || (parameters.Count() != 29))
                throw new LoggingException(
                        "Wrong number of parameter supplied for the '{0}' program, which expected '29' parameter(s) but was supplied with '{1}.",
                        LogLevel.Critical,
                        _program.Name,
                        parameters == null ? 0 : parameters.Count());

            SqlProgramParameter[] parametersArray = parameters.ToArray();

            int pCount = parametersArray.GetLength(0);
            if (pCount < 29)
                throw new LoggingException(
                        "Too many parameters supplied for the '{0}' program, which only accepts '{1} parameter(s) but was supplied with '29'.",
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
        
            // Find or create SQL Parameter 7.
            programParameter = parametersArray[6];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p7Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 8.
            programParameter = parametersArray[7];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p8Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 9.
            programParameter = parametersArray[8];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p9Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 10.
            programParameter = parametersArray[9];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p10Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 11.
            programParameter = parametersArray[10];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p11Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 12.
            programParameter = parametersArray[11];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p12Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 13.
            programParameter = parametersArray[12];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p13Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 14.
            programParameter = parametersArray[13];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p14Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 15.
            programParameter = parametersArray[14];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p15Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 16.
            programParameter = parametersArray[15];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p16Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 17.
            programParameter = parametersArray[16];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p17Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 18.
            programParameter = parametersArray[17];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p18Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 19.
            programParameter = parametersArray[18];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p19Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 20.
            programParameter = parametersArray[19];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p20Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 21.
            programParameter = parametersArray[20];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p21Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 22.
            programParameter = parametersArray[21];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p22Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 23.
            programParameter = parametersArray[22];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p23Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 24.
            programParameter = parametersArray[23];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p24Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 25.
            programParameter = parametersArray[24];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p25Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 26.
            programParameter = parametersArray[25];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p26Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 27.
            programParameter = parametersArray[26];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p27Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 28.
            programParameter = parametersArray[27];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p28Value, mode);
            sqlParameters.Add(parameter);
        
            // Find or create SQL Parameter 29.
            programParameter = parametersArray[28];
            index = _command.Parameters.IndexOf(programParameter.Name);
            parameter = index < 0 ? this._command.Parameters.Add(programParameter.CreateSqlParameter()) : this._command.Parameters[index];
            parameter.Value = programParameter.CastCLRValue(p29Value, mode);
            sqlParameters.Add(parameter);
        
            // Return parameters that were set
            return sqlParameters;
        }
    }
    #endregion

    #region SqlProgam<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29>
    /// <summary>
    ///   Used to create an object for easy calling of stored procedures or functions in a database.
    /// </summary>
    public class SqlProgram<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29> : SqlProgram
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlProgram&lt;T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29&gt;"/> class.
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
            : base(new LoadBalancedConnection(connectionString), name, ignoreValidationErrors, defaultCommandTimeout, constraintMode, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14), typeof(T15), typeof(T16), typeof(T17), typeof(T18), typeof(T19), typeof(T20), typeof(T21), typeof(T22), typeof(T23), typeof(T24), typeof(T25), typeof(T26), typeof(T27), typeof(T28), typeof(T29))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlProgram&lt;T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29&gt;"/> class.
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
            : base(connection, name, ignoreValidationErrors, defaultCommandTimeout, constraintMode, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14), typeof(T15), typeof(T16), typeof(T17), typeof(T18), typeof(T19), typeof(T20), typeof(T21), typeof(T22), typeof(T23), typeof(T24), typeof(T25), typeof(T26), typeof(T27), typeof(T28), typeof(T29))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlProgram&lt;T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29&gt;"/> class.
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
            : base(sqlProgram, ignoreValidationErrors, defaultCommandTimeout, constraintMode, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14), typeof(T15), typeof(T16), typeof(T17), typeof(T18), typeof(T19), typeof(T20), typeof(T21), typeof(T22), typeof(T23), typeof(T24), typeof(T25), typeof(T26), typeof(T27), typeof(T28), typeof(T29))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlProgram&lt;T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29&gt;"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="name">The name of the stored procedure or function.</param>
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
        /// <param name="p16Name">Name of parameter 16.</param>
        /// <param name="p17Name">Name of parameter 17.</param>
        /// <param name="p18Name">Name of parameter 18.</param>
        /// <param name="p19Name">Name of parameter 19.</param>
        /// <param name="p20Name">Name of parameter 20.</param>
        /// <param name="p21Name">Name of parameter 21.</param>
        /// <param name="p22Name">Name of parameter 22.</param>
        /// <param name="p23Name">Name of parameter 23.</param>
        /// <param name="p24Name">Name of parameter 24.</param>
        /// <param name="p25Name">Name of parameter 25.</param>
        /// <param name="p26Name">Name of parameter 26.</param>
        /// <param name="p27Name">Name of parameter 27.</param>
        /// <param name="p28Name">Name of parameter 28.</param>
        /// <param name="p29Name">Name of parameter 29.</param>
        /// <param name="ignoreValidationErrors">if set to <see langword="true"/> does not throw validation errors (records them instead).</param>
        /// <param name="checkOrder">if set to <c>true</c> checks the parameter order matches.</param>
        /// <param name="defaultCommandTimeout">The optional default command timeout, which will be used whenever this command is executed synchronously.
        /// Defaults to 30s.</param>
        /// <param name="constraintMode">The constraint mode.</param>
        public SqlProgram(
            string connectionString,
            string name,
            string p1Name, string p2Name, string p3Name, string p4Name, string p5Name, string p6Name, string p7Name, string p8Name, string p9Name, string p10Name, string p11Name, string p12Name, string p13Name, string p14Name, string p15Name, string p16Name, string p17Name, string p18Name, string p19Name, string p20Name, string p21Name, string p22Name, string p23Name, string p24Name, string p25Name, string p26Name, string p27Name, string p28Name, string p29Name,
            bool ignoreValidationErrors = false,
            bool checkOrder = false,
            TimeSpan? defaultCommandTimeout = null,
            TypeConstraintMode constraintMode = TypeConstraintMode.Warn)
            : base(new LoadBalancedConnection(connectionString), name, ignoreValidationErrors, checkOrder, defaultCommandTimeout, constraintMode, new List<string>{ p1Name, p2Name, p3Name, p4Name, p5Name, p6Name, p7Name, p8Name, p9Name, p10Name, p11Name, p12Name, p13Name, p14Name, p15Name, p16Name, p17Name, p18Name, p19Name, p20Name, p21Name, p22Name, p23Name, p24Name, p25Name, p26Name, p27Name, p28Name, p29Name }, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14), typeof(T15), typeof(T16), typeof(T17), typeof(T18), typeof(T19), typeof(T20), typeof(T21), typeof(T22), typeof(T23), typeof(T24), typeof(T25), typeof(T26), typeof(T27), typeof(T28), typeof(T29))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlProgram&lt;T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29&gt;"/> class.
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
        /// <param name="p16Name">Name of parameter 16.</param>
        /// <param name="p17Name">Name of parameter 17.</param>
        /// <param name="p18Name">Name of parameter 18.</param>
        /// <param name="p19Name">Name of parameter 19.</param>
        /// <param name="p20Name">Name of parameter 20.</param>
        /// <param name="p21Name">Name of parameter 21.</param>
        /// <param name="p22Name">Name of parameter 22.</param>
        /// <param name="p23Name">Name of parameter 23.</param>
        /// <param name="p24Name">Name of parameter 24.</param>
        /// <param name="p25Name">Name of parameter 25.</param>
        /// <param name="p26Name">Name of parameter 26.</param>
        /// <param name="p27Name">Name of parameter 27.</param>
        /// <param name="p28Name">Name of parameter 28.</param>
        /// <param name="p29Name">Name of parameter 29.</param>
        /// <param name="ignoreValidationErrors">if set to <see langword="true"/> does not throw validation errors (records them instead).</param>
        /// <param name="checkOrder">if set to <c>true</c> checks the parameter order matches.</param>
        /// <param name="defaultCommandTimeout">The optional default command timeout, which will be used whenever this command is executed synchronously.
        /// Defaults to 30s.</param>
        /// <param name="constraintMode">The constraint mode.</param>
        public SqlProgram(
            LoadBalancedConnection connection,
            string name,
            string p1Name, string p2Name, string p3Name, string p4Name, string p5Name, string p6Name, string p7Name, string p8Name, string p9Name, string p10Name, string p11Name, string p12Name, string p13Name, string p14Name, string p15Name, string p16Name, string p17Name, string p18Name, string p19Name, string p20Name, string p21Name, string p22Name, string p23Name, string p24Name, string p25Name, string p26Name, string p27Name, string p28Name, string p29Name,
            bool ignoreValidationErrors = false,
            bool checkOrder = false,
            TimeSpan? defaultCommandTimeout = null,
            TypeConstraintMode constraintMode = TypeConstraintMode.Warn)
            : base(connection, name, ignoreValidationErrors, checkOrder, defaultCommandTimeout, constraintMode, new List<string>{ p1Name, p2Name, p3Name, p4Name, p5Name, p6Name, p7Name, p8Name, p9Name, p10Name, p11Name, p12Name, p13Name, p14Name, p15Name, p16Name, p17Name, p18Name, p19Name, p20Name, p21Name, p22Name, p23Name, p24Name, p25Name, p26Name, p27Name, p28Name, p29Name }, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14), typeof(T15), typeof(T16), typeof(T17), typeof(T18), typeof(T19), typeof(T20), typeof(T21), typeof(T22), typeof(T23), typeof(T24), typeof(T25), typeof(T26), typeof(T27), typeof(T28), typeof(T29))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlProgram&lt;T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29&gt;"/> class.
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
        /// <param name="p16Name">Name of parameter 16.</param>
        /// <param name="p17Name">Name of parameter 17.</param>
        /// <param name="p18Name">Name of parameter 18.</param>
        /// <param name="p19Name">Name of parameter 19.</param>
        /// <param name="p20Name">Name of parameter 20.</param>
        /// <param name="p21Name">Name of parameter 21.</param>
        /// <param name="p22Name">Name of parameter 22.</param>
        /// <param name="p23Name">Name of parameter 23.</param>
        /// <param name="p24Name">Name of parameter 24.</param>
        /// <param name="p25Name">Name of parameter 25.</param>
        /// <param name="p26Name">Name of parameter 26.</param>
        /// <param name="p27Name">Name of parameter 27.</param>
        /// <param name="p28Name">Name of parameter 28.</param>
        /// <param name="p29Name">Name of parameter 29.</param>
        /// <param name="ignoreValidationErrors">if set to <see langword="true"/> does not throw validation errors (records them instead).</param>
        /// <param name="checkOrder">if set to <c>true</c> checks the parameter order matches.</param>
        /// <param name="defaultCommandTimeout">The optional default command timeout, which will be used whenever this command is executed synchronously.
        /// Defaults to existing programs default.</param>
        /// <param name="constraintMode">The constraint mode.</param>
        public SqlProgram(
            SqlProgram sqlProgram,
            string p1Name, string p2Name, string p3Name, string p4Name, string p5Name, string p6Name, string p7Name, string p8Name, string p9Name, string p10Name, string p11Name, string p12Name, string p13Name, string p14Name, string p15Name, string p16Name, string p17Name, string p18Name, string p19Name, string p20Name, string p21Name, string p22Name, string p23Name, string p24Name, string p25Name, string p26Name, string p27Name, string p28Name, string p29Name,
            bool ignoreValidationErrors = false,
            bool checkOrder = false,
            TimeSpan? defaultCommandTimeout = null,
            TypeConstraintMode constraintMode = TypeConstraintMode.Warn)
            : base(sqlProgram, ignoreValidationErrors, checkOrder, defaultCommandTimeout, constraintMode, new List<string>{ p1Name, p2Name, p3Name, p4Name, p5Name, p6Name, p7Name, p8Name, p9Name, p10Name, p11Name, p12Name, p13Name, p14Name, p15Name, p16Name, p17Name, p18Name, p19Name, p20Name, p21Name, p22Name, p23Name, p24Name, p25Name, p26Name, p27Name, p28Name, p29Name }, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14), typeof(T15), typeof(T16), typeof(T17), typeof(T18), typeof(T19), typeof(T20), typeof(T21), typeof(T22), typeof(T23), typeof(T24), typeof(T25), typeof(T26), typeof(T27), typeof(T28), typeof(T29))
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
        /// <param name="p7Value">Value of SQL Parameter 7.</param>
        /// <param name="p8Value">Value of SQL Parameter 8.</param>
        /// <param name="p9Value">Value of SQL Parameter 9.</param>
        /// <param name="p10Value">Value of SQL Parameter 10.</param>
        /// <param name="p11Value">Value of SQL Parameter 11.</param>
        /// <param name="p12Value">Value of SQL Parameter 12.</param>
        /// <param name="p13Value">Value of SQL Parameter 13.</param>
        /// <param name="p14Value">Value of SQL Parameter 14.</param>
        /// <param name="p15Value">Value of SQL Parameter 15.</param>
        /// <param name="p16Value">Value of SQL Parameter 16.</param>
        /// <param name="p17Value">Value of SQL Parameter 17.</param>
        /// <param name="p18Value">Value of SQL Parameter 18.</param>
        /// <param name="p19Value">Value of SQL Parameter 19.</param>
        /// <param name="p20Value">Value of SQL Parameter 20.</param>
        /// <param name="p21Value">Value of SQL Parameter 21.</param>
        /// <param name="p22Value">Value of SQL Parameter 22.</param>
        /// <param name="p23Value">Value of SQL Parameter 23.</param>
        /// <param name="p24Value">Value of SQL Parameter 24.</param>
        /// <param name="p25Value">Value of SQL Parameter 25.</param>
        /// <param name="p26Value">Value of SQL Parameter 26.</param>
        /// <param name="p27Value">Value of SQL Parameter 27.</param>
        /// <param name="p28Value">Value of SQL Parameter 28.</param>
        /// <param name="p29Value">Value of SQL Parameter 29.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configurated default for this program.</param>
        /// <returns>The scalar value.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public TOut ExecuteScalar<TOut>(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), T7 p7Value = default(T7), T8 p8Value = default(T8), T9 p9Value = default(T9), T10 p10Value = default(T10), T11 p11Value = default(T11), T12 p12Value = default(T12), T13 p13Value = default(T13), T14 p14Value = default(T14), T15 p15Value = default(T15), T16 p16Value = default(T16), T17 p17Value = default(T17), T18 p18Value = default(T18), T19 p19Value = default(T19), T20 p20Value = default(T20), T21 p21Value = default(T21), T22 p22Value = default(T22), T23 p23Value = default(T23), T24 p24Value = default(T24), T25 p25Value = default(T25), T26 p26Value = default(T26), T27 p27Value = default(T27), T28 p28Value = default(T28), T29 p29Value = default(T29), TypeConstraintMode? constraintMode = null)
        {
            return this.ExecuteScalar<TOut>(c => c.SetParameters(ProgramParameters, p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, p7Value, p8Value, p9Value, p10Value, p11Value, p12Value, p13Value, p14Value, p15Value, p16Value, p17Value, p18Value, p19Value, p20Value, p21Value, p22Value, p23Value, p24Value, p25Value, p26Value, p27Value, p28Value, p29Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)));
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
        /// <param name="p16Value">Value of SQL Parameter 16.</param>
        /// <param name="p17Value">Value of SQL Parameter 17.</param>
        /// <param name="p18Value">Value of SQL Parameter 18.</param>
        /// <param name="p19Value">Value of SQL Parameter 19.</param>
        /// <param name="p20Value">Value of SQL Parameter 20.</param>
        /// <param name="p21Value">Value of SQL Parameter 21.</param>
        /// <param name="p22Value">Value of SQL Parameter 22.</param>
        /// <param name="p23Value">Value of SQL Parameter 23.</param>
        /// <param name="p24Value">Value of SQL Parameter 24.</param>
        /// <param name="p25Value">Value of SQL Parameter 25.</param>
        /// <param name="p26Value">Value of SQL Parameter 26.</param>
        /// <param name="p27Value">Value of SQL Parameter 27.</param>
        /// <param name="p28Value">Value of SQL Parameter 28.</param>
        /// <param name="p29Value">Value of SQL Parameter 29.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configurated default for this program.</param>
        /// <param name="state">The state.</param>
        /// <returns>The scalar value.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public async Task<TOut> ExecuteScalarAsync<TOut>(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), T7 p7Value = default(T7), T8 p8Value = default(T8), T9 p9Value = default(T9), T10 p10Value = default(T10), T11 p11Value = default(T11), T12 p12Value = default(T12), T13 p13Value = default(T13), T14 p14Value = default(T14), T15 p15Value = default(T15), T16 p16Value = default(T16), T17 p17Value = default(T17), T18 p18Value = default(T18), T19 p19Value = default(T19), T20 p20Value = default(T20), T21 p21Value = default(T21), T22 p22Value = default(T22), T23 p23Value = default(T23), T24 p24Value = default(T24), T25 p25Value = default(T25), T26 p26Value = default(T26), T27 p27Value = default(T27), T28 p28Value = default(T28), T29 p29Value = default(T29), TypeConstraintMode? constraintMode = null, object state = null)
        {
            return await this.ExecuteScalarAsync<TOut>(c => c.SetParameters(ProgramParameters, p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, p7Value, p8Value, p9Value, p10Value, p11Value, p12Value, p13Value, p14Value, p15Value, p16Value, p17Value, p18Value, p19Value, p20Value, p21Value, p22Value, p23Value, p24Value, p25Value, p26Value, p27Value, p28Value, p29Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), state);
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
        /// <param name="p16Value">Value of SQL Parameter 16.</param>
        /// <param name="p17Value">Value of SQL Parameter 17.</param>
        /// <param name="p18Value">Value of SQL Parameter 18.</param>
        /// <param name="p19Value">Value of SQL Parameter 19.</param>
        /// <param name="p20Value">Value of SQL Parameter 20.</param>
        /// <param name="p21Value">Value of SQL Parameter 21.</param>
        /// <param name="p22Value">Value of SQL Parameter 22.</param>
        /// <param name="p23Value">Value of SQL Parameter 23.</param>
        /// <param name="p24Value">Value of SQL Parameter 24.</param>
        /// <param name="p25Value">Value of SQL Parameter 25.</param>
        /// <param name="p26Value">Value of SQL Parameter 26.</param>
        /// <param name="p27Value">Value of SQL Parameter 27.</param>
        /// <param name="p28Value">Value of SQL Parameter 28.</param>
        /// <param name="p29Value">Value of SQL Parameter 29.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configurated default for this program.</param>
        /// <returns>The scalar value for each connection.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public IEnumerable<TOut> ExecuteScalarAll<TOut>(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), T7 p7Value = default(T7), T8 p8Value = default(T8), T9 p9Value = default(T9), T10 p10Value = default(T10), T11 p11Value = default(T11), T12 p12Value = default(T12), T13 p13Value = default(T13), T14 p14Value = default(T14), T15 p15Value = default(T15), T16 p16Value = default(T16), T17 p17Value = default(T17), T18 p18Value = default(T18), T19 p19Value = default(T19), T20 p20Value = default(T20), T21 p21Value = default(T21), T22 p22Value = default(T22), T23 p23Value = default(T23), T24 p24Value = default(T24), T25 p25Value = default(T25), T26 p26Value = default(T26), T27 p27Value = default(T27), T28 p28Value = default(T28), T29 p29Value = default(T29), TypeConstraintMode? constraintMode = null)
        {
            return this.ExecuteScalarAll<TOut>(c => c.SetParameters(ProgramParameters, p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, p7Value, p8Value, p9Value, p10Value, p11Value, p12Value, p13Value, p14Value, p15Value, p16Value, p17Value, p18Value, p19Value, p20Value, p21Value, p22Value, p23Value, p24Value, p25Value, p26Value, p27Value, p28Value, p29Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)));
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
        /// <param name="p16Value">Value of SQL Parameter 16.</param>
        /// <param name="p17Value">Value of SQL Parameter 17.</param>
        /// <param name="p18Value">Value of SQL Parameter 18.</param>
        /// <param name="p19Value">Value of SQL Parameter 19.</param>
        /// <param name="p20Value">Value of SQL Parameter 20.</param>
        /// <param name="p21Value">Value of SQL Parameter 21.</param>
        /// <param name="p22Value">Value of SQL Parameter 22.</param>
        /// <param name="p23Value">Value of SQL Parameter 23.</param>
        /// <param name="p24Value">Value of SQL Parameter 24.</param>
        /// <param name="p25Value">Value of SQL Parameter 25.</param>
        /// <param name="p26Value">Value of SQL Parameter 26.</param>
        /// <param name="p27Value">Value of SQL Parameter 27.</param>
        /// <param name="p28Value">Value of SQL Parameter 28.</param>
        /// <param name="p29Value">Value of SQL Parameter 29.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configurated default for this program.</param>
        /// <param name="state">The state.</param>
        /// <returns>The scalar value for each connection.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public async Task<IEnumerable<TOut>> ExecuteScalarAllAsync<TOut>(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), T7 p7Value = default(T7), T8 p8Value = default(T8), T9 p9Value = default(T9), T10 p10Value = default(T10), T11 p11Value = default(T11), T12 p12Value = default(T12), T13 p13Value = default(T13), T14 p14Value = default(T14), T15 p15Value = default(T15), T16 p16Value = default(T16), T17 p17Value = default(T17), T18 p18Value = default(T18), T19 p19Value = default(T19), T20 p20Value = default(T20), T21 p21Value = default(T21), T22 p22Value = default(T22), T23 p23Value = default(T23), T24 p24Value = default(T24), T25 p25Value = default(T25), T26 p26Value = default(T26), T27 p27Value = default(T27), T28 p28Value = default(T28), T29 p29Value = default(T29), TypeConstraintMode? constraintMode = null, object state = null)
        {
            return await this.ExecuteScalarAllAsync<TOut>(c => c.SetParameters(ProgramParameters, p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, p7Value, p8Value, p9Value, p10Value, p11Value, p12Value, p13Value, p14Value, p15Value, p16Value, p17Value, p18Value, p19Value, p20Value, p21Value, p22Value, p23Value, p24Value, p25Value, p26Value, p27Value, p28Value, p29Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), state);
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
        /// <param name="p16Value">Value of SQL Parameter 16.</param>
        /// <param name="p17Value">Value of SQL Parameter 17.</param>
        /// <param name="p18Value">Value of SQL Parameter 18.</param>
        /// <param name="p19Value">Value of SQL Parameter 19.</param>
        /// <param name="p20Value">Value of SQL Parameter 20.</param>
        /// <param name="p21Value">Value of SQL Parameter 21.</param>
        /// <param name="p22Value">Value of SQL Parameter 22.</param>
        /// <param name="p23Value">Value of SQL Parameter 23.</param>
        /// <param name="p24Value">Value of SQL Parameter 24.</param>
        /// <param name="p25Value">Value of SQL Parameter 25.</param>
        /// <param name="p26Value">Value of SQL Parameter 26.</param>
        /// <param name="p27Value">Value of SQL Parameter 27.</param>
        /// <param name="p28Value">Value of SQL Parameter 28.</param>
        /// <param name="p29Value">Value of SQL Parameter 29.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configurated default for this program.</param>
        /// <returns>Number of rows affected.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public int ExecuteNonQuery(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), T7 p7Value = default(T7), T8 p8Value = default(T8), T9 p9Value = default(T9), T10 p10Value = default(T10), T11 p11Value = default(T11), T12 p12Value = default(T12), T13 p13Value = default(T13), T14 p14Value = default(T14), T15 p15Value = default(T15), T16 p16Value = default(T16), T17 p17Value = default(T17), T18 p18Value = default(T18), T19 p19Value = default(T19), T20 p20Value = default(T20), T21 p21Value = default(T21), T22 p22Value = default(T22), T23 p23Value = default(T23), T24 p24Value = default(T24), T25 p25Value = default(T25), T26 p26Value = default(T26), T27 p27Value = default(T27), T28 p28Value = default(T28), T29 p29Value = default(T29), TypeConstraintMode? constraintMode = null)
        {
            return this.ExecuteNonQuery(c => c.SetParameters(ProgramParameters, p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, p7Value, p8Value, p9Value, p10Value, p11Value, p12Value, p13Value, p14Value, p15Value, p16Value, p17Value, p18Value, p19Value, p20Value, p21Value, p22Value, p23Value, p24Value, p25Value, p26Value, p27Value, p28Value, p29Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)));
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
        /// <param name="p16Value">Value of SQL Parameter 16.</param>
        /// <param name="p17Value">Value of SQL Parameter 17.</param>
        /// <param name="p18Value">Value of SQL Parameter 18.</param>
        /// <param name="p19Value">Value of SQL Parameter 19.</param>
        /// <param name="p20Value">Value of SQL Parameter 20.</param>
        /// <param name="p21Value">Value of SQL Parameter 21.</param>
        /// <param name="p22Value">Value of SQL Parameter 22.</param>
        /// <param name="p23Value">Value of SQL Parameter 23.</param>
        /// <param name="p24Value">Value of SQL Parameter 24.</param>
        /// <param name="p25Value">Value of SQL Parameter 25.</param>
        /// <param name="p26Value">Value of SQL Parameter 26.</param>
        /// <param name="p27Value">Value of SQL Parameter 27.</param>
        /// <param name="p28Value">Value of SQL Parameter 28.</param>
        /// <param name="p29Value">Value of SQL Parameter 29.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configurated default for this program.</param>
        /// <param name="state">The state.</param>
        /// <returns>Number of rows affected.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public async Task<int> ExecuteNonQueryAsync(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), T7 p7Value = default(T7), T8 p8Value = default(T8), T9 p9Value = default(T9), T10 p10Value = default(T10), T11 p11Value = default(T11), T12 p12Value = default(T12), T13 p13Value = default(T13), T14 p14Value = default(T14), T15 p15Value = default(T15), T16 p16Value = default(T16), T17 p17Value = default(T17), T18 p18Value = default(T18), T19 p19Value = default(T19), T20 p20Value = default(T20), T21 p21Value = default(T21), T22 p22Value = default(T22), T23 p23Value = default(T23), T24 p24Value = default(T24), T25 p25Value = default(T25), T26 p26Value = default(T26), T27 p27Value = default(T27), T28 p28Value = default(T28), T29 p29Value = default(T29), TypeConstraintMode? constraintMode = null, object state = null)
        {
            return await this.ExecuteNonQueryAsync(c => c.SetParameters(ProgramParameters, p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, p7Value, p8Value, p9Value, p10Value, p11Value, p12Value, p13Value, p14Value, p15Value, p16Value, p17Value, p18Value, p19Value, p20Value, p21Value, p22Value, p23Value, p24Value, p25Value, p26Value, p27Value, p28Value, p29Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), state);
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
        /// <param name="p16Value">Value of SQL Parameter 16.</param>
        /// <param name="p17Value">Value of SQL Parameter 17.</param>
        /// <param name="p18Value">Value of SQL Parameter 18.</param>
        /// <param name="p19Value">Value of SQL Parameter 19.</param>
        /// <param name="p20Value">Value of SQL Parameter 20.</param>
        /// <param name="p21Value">Value of SQL Parameter 21.</param>
        /// <param name="p22Value">Value of SQL Parameter 22.</param>
        /// <param name="p23Value">Value of SQL Parameter 23.</param>
        /// <param name="p24Value">Value of SQL Parameter 24.</param>
        /// <param name="p25Value">Value of SQL Parameter 25.</param>
        /// <param name="p26Value">Value of SQL Parameter 26.</param>
        /// <param name="p27Value">Value of SQL Parameter 27.</param>
        /// <param name="p28Value">Value of SQL Parameter 28.</param>
        /// <param name="p29Value">Value of SQL Parameter 29.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configurated default for this program.</param>
        /// <returns>Number of rows affected for each connection.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public IEnumerable<int> ExecuteNonQueryAll(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), T7 p7Value = default(T7), T8 p8Value = default(T8), T9 p9Value = default(T9), T10 p10Value = default(T10), T11 p11Value = default(T11), T12 p12Value = default(T12), T13 p13Value = default(T13), T14 p14Value = default(T14), T15 p15Value = default(T15), T16 p16Value = default(T16), T17 p17Value = default(T17), T18 p18Value = default(T18), T19 p19Value = default(T19), T20 p20Value = default(T20), T21 p21Value = default(T21), T22 p22Value = default(T22), T23 p23Value = default(T23), T24 p24Value = default(T24), T25 p25Value = default(T25), T26 p26Value = default(T26), T27 p27Value = default(T27), T28 p28Value = default(T28), T29 p29Value = default(T29), TypeConstraintMode? constraintMode = null)
        {
            return this.ExecuteNonQueryAll(c => c.SetParameters(ProgramParameters, p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, p7Value, p8Value, p9Value, p10Value, p11Value, p12Value, p13Value, p14Value, p15Value, p16Value, p17Value, p18Value, p19Value, p20Value, p21Value, p22Value, p23Value, p24Value, p25Value, p26Value, p27Value, p28Value, p29Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)));
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
        /// <param name="p16Value">Value of SQL Parameter 16.</param>
        /// <param name="p17Value">Value of SQL Parameter 17.</param>
        /// <param name="p18Value">Value of SQL Parameter 18.</param>
        /// <param name="p19Value">Value of SQL Parameter 19.</param>
        /// <param name="p20Value">Value of SQL Parameter 20.</param>
        /// <param name="p21Value">Value of SQL Parameter 21.</param>
        /// <param name="p22Value">Value of SQL Parameter 22.</param>
        /// <param name="p23Value">Value of SQL Parameter 23.</param>
        /// <param name="p24Value">Value of SQL Parameter 24.</param>
        /// <param name="p25Value">Value of SQL Parameter 25.</param>
        /// <param name="p26Value">Value of SQL Parameter 26.</param>
        /// <param name="p27Value">Value of SQL Parameter 27.</param>
        /// <param name="p28Value">Value of SQL Parameter 28.</param>
        /// <param name="p29Value">Value of SQL Parameter 29.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configurated default for this program.</param>
        /// <param name="state">The state.</param>
        /// <returns>Number of rows affected for each connection.</returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public async Task<IEnumerable<int>> ExecuteNonQueryAllAsync(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), T7 p7Value = default(T7), T8 p8Value = default(T8), T9 p9Value = default(T9), T10 p10Value = default(T10), T11 p11Value = default(T11), T12 p12Value = default(T12), T13 p13Value = default(T13), T14 p14Value = default(T14), T15 p15Value = default(T15), T16 p16Value = default(T16), T17 p17Value = default(T17), T18 p18Value = default(T18), T19 p19Value = default(T19), T20 p20Value = default(T20), T21 p21Value = default(T21), T22 p22Value = default(T22), T23 p23Value = default(T23), T24 p24Value = default(T24), T25 p25Value = default(T25), T26 p26Value = default(T26), T27 p27Value = default(T27), T28 p28Value = default(T28), T29 p29Value = default(T29), TypeConstraintMode? constraintMode = null, object state = null)
        {
            return await this.ExecuteNonQueryAllAsync(c => c.SetParameters(ProgramParameters, p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, p7Value, p8Value, p9Value, p10Value, p11Value, p12Value, p13Value, p14Value, p15Value, p16Value, p17Value, p18Value, p19Value, p20Value, p21Value, p22Value, p23Value, p24Value, p25Value, p26Value, p27Value, p28Value, p29Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), state);
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
        /// <param name="p16Value">Value of SQL Parameter 16.</param>
        /// <param name="p17Value">Value of SQL Parameter 17.</param>
        /// <param name="p18Value">Value of SQL Parameter 18.</param>
        /// <param name="p19Value">Value of SQL Parameter 19.</param>
        /// <param name="p20Value">Value of SQL Parameter 20.</param>
        /// <param name="p21Value">Value of SQL Parameter 21.</param>
        /// <param name="p22Value">Value of SQL Parameter 22.</param>
        /// <param name="p23Value">Value of SQL Parameter 23.</param>
        /// <param name="p24Value">Value of SQL Parameter 24.</param>
        /// <param name="p25Value">Value of SQL Parameter 25.</param>
        /// <param name="p26Value">Value of SQL Parameter 26.</param>
        /// <param name="p27Value">Value of SQL Parameter 27.</param>
        /// <param name="p28Value">Value of SQL Parameter 28.</param>
        /// <param name="p29Value">Value of SQL Parameter 29.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configurated default for this program.</param>
        /// <param name="resultAction">The result function.</param>
        /// <param name="behavior">The behaviour.</param>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public void ExecuteReader(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), T7 p7Value = default(T7), T8 p8Value = default(T8), T9 p9Value = default(T9), T10 p10Value = default(T10), T11 p11Value = default(T11), T12 p12Value = default(T12), T13 p13Value = default(T13), T14 p14Value = default(T14), T15 p15Value = default(T15), T16 p16Value = default(T16), T17 p17Value = default(T17), T18 p18Value = default(T18), T19 p19Value = default(T19), T20 p20Value = default(T20), T21 p21Value = default(T21), T22 p22Value = default(T22), T23 p23Value = default(T23), T24 p24Value = default(T24), T25 p25Value = default(T25), T26 p26Value = default(T26), T27 p27Value = default(T27), T28 p28Value = default(T28), T29 p29Value = default(T29), Action<SqlDataReader> resultAction = null, CommandBehavior behavior = CommandBehavior.Default, TypeConstraintMode? constraintMode = null)
        {
            this.ExecuteReader(c => c.SetParameters(ProgramParameters, p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, p7Value, p8Value, p9Value, p10Value, p11Value, p12Value, p13Value, p14Value, p15Value, p16Value, p17Value, p18Value, p19Value, p20Value, p21Value, p22Value, p23Value, p24Value, p25Value, p26Value, p27Value, p28Value, p29Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), resultAction, behavior);
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
        /// <param name="p16Value">Value of SQL Parameter 16.</param>
        /// <param name="p17Value">Value of SQL Parameter 17.</param>
        /// <param name="p18Value">Value of SQL Parameter 18.</param>
        /// <param name="p19Value">Value of SQL Parameter 19.</param>
        /// <param name="p20Value">Value of SQL Parameter 20.</param>
        /// <param name="p21Value">Value of SQL Parameter 21.</param>
        /// <param name="p22Value">Value of SQL Parameter 22.</param>
        /// <param name="p23Value">Value of SQL Parameter 23.</param>
        /// <param name="p24Value">Value of SQL Parameter 24.</param>
        /// <param name="p25Value">Value of SQL Parameter 25.</param>
        /// <param name="p26Value">Value of SQL Parameter 26.</param>
        /// <param name="p27Value">Value of SQL Parameter 27.</param>
        /// <param name="p28Value">Value of SQL Parameter 28.</param>
        /// <param name="p29Value">Value of SQL Parameter 29.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configurated default for this program.</param>
        /// <param name="resultAction">The result function.</param>
        /// <param name="behavior">The behaviour.</param>
        /// <param name="state">The state.</param>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public async Task ExecuteReaderAsync(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), T7 p7Value = default(T7), T8 p8Value = default(T8), T9 p9Value = default(T9), T10 p10Value = default(T10), T11 p11Value = default(T11), T12 p12Value = default(T12), T13 p13Value = default(T13), T14 p14Value = default(T14), T15 p15Value = default(T15), T16 p16Value = default(T16), T17 p17Value = default(T17), T18 p18Value = default(T18), T19 p19Value = default(T19), T20 p20Value = default(T20), T21 p21Value = default(T21), T22 p22Value = default(T22), T23 p23Value = default(T23), T24 p24Value = default(T24), T25 p25Value = default(T25), T26 p26Value = default(T26), T27 p27Value = default(T27), T28 p28Value = default(T28), T29 p29Value = default(T29), Action<SqlDataReader> resultAction = null, CommandBehavior behavior = CommandBehavior.Default, TypeConstraintMode? constraintMode = null, object state = null)
        {
            await this.ExecuteReaderAsync(c => c.SetParameters(ProgramParameters, p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, p7Value, p8Value, p9Value, p10Value, p11Value, p12Value, p13Value, p14Value, p15Value, p16Value, p17Value, p18Value, p19Value, p20Value, p21Value, p22Value, p23Value, p24Value, p25Value, p26Value, p27Value, p28Value, p29Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), resultAction, behavior, state);
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
        /// <param name="p16Value">Value of SQL Parameter 16.</param>
        /// <param name="p17Value">Value of SQL Parameter 17.</param>
        /// <param name="p18Value">Value of SQL Parameter 18.</param>
        /// <param name="p19Value">Value of SQL Parameter 19.</param>
        /// <param name="p20Value">Value of SQL Parameter 20.</param>
        /// <param name="p21Value">Value of SQL Parameter 21.</param>
        /// <param name="p22Value">Value of SQL Parameter 22.</param>
        /// <param name="p23Value">Value of SQL Parameter 23.</param>
        /// <param name="p24Value">Value of SQL Parameter 24.</param>
        /// <param name="p25Value">Value of SQL Parameter 25.</param>
        /// <param name="p26Value">Value of SQL Parameter 26.</param>
        /// <param name="p27Value">Value of SQL Parameter 27.</param>
        /// <param name="p28Value">Value of SQL Parameter 28.</param>
        /// <param name="p29Value">Value of SQL Parameter 29.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configurated default for this program.</param>
        /// <param name="resultAction">The result function.</param>
        /// <param name="behavior">The behaviour.</param>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public void ExecuteReaderAll(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), T7 p7Value = default(T7), T8 p8Value = default(T8), T9 p9Value = default(T9), T10 p10Value = default(T10), T11 p11Value = default(T11), T12 p12Value = default(T12), T13 p13Value = default(T13), T14 p14Value = default(T14), T15 p15Value = default(T15), T16 p16Value = default(T16), T17 p17Value = default(T17), T18 p18Value = default(T18), T19 p19Value = default(T19), T20 p20Value = default(T20), T21 p21Value = default(T21), T22 p22Value = default(T22), T23 p23Value = default(T23), T24 p24Value = default(T24), T25 p25Value = default(T25), T26 p26Value = default(T26), T27 p27Value = default(T27), T28 p28Value = default(T28), T29 p29Value = default(T29), Action<SqlDataReader> resultAction = null, CommandBehavior behavior = CommandBehavior.Default, TypeConstraintMode? constraintMode = null)
        {
            this.ExecuteReaderAll(c => c.SetParameters(ProgramParameters, p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, p7Value, p8Value, p9Value, p10Value, p11Value, p12Value, p13Value, p14Value, p15Value, p16Value, p17Value, p18Value, p19Value, p20Value, p21Value, p22Value, p23Value, p24Value, p25Value, p26Value, p27Value, p28Value, p29Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), resultAction, behavior);
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
        /// <param name="p16Value">Value of SQL Parameter 16.</param>
        /// <param name="p17Value">Value of SQL Parameter 17.</param>
        /// <param name="p18Value">Value of SQL Parameter 18.</param>
        /// <param name="p19Value">Value of SQL Parameter 19.</param>
        /// <param name="p20Value">Value of SQL Parameter 20.</param>
        /// <param name="p21Value">Value of SQL Parameter 21.</param>
        /// <param name="p22Value">Value of SQL Parameter 22.</param>
        /// <param name="p23Value">Value of SQL Parameter 23.</param>
        /// <param name="p24Value">Value of SQL Parameter 24.</param>
        /// <param name="p25Value">Value of SQL Parameter 25.</param>
        /// <param name="p26Value">Value of SQL Parameter 26.</param>
        /// <param name="p27Value">Value of SQL Parameter 27.</param>
        /// <param name="p28Value">Value of SQL Parameter 28.</param>
        /// <param name="p29Value">Value of SQL Parameter 29.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configurated default for this program.</param>
        /// <param name="resultAction">The result function.</param>
        /// <param name="behavior">The behaviour.</param>
        /// <param name="state">The state.</param>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public async Task ExecuteReaderAllAsync(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), T7 p7Value = default(T7), T8 p8Value = default(T8), T9 p9Value = default(T9), T10 p10Value = default(T10), T11 p11Value = default(T11), T12 p12Value = default(T12), T13 p13Value = default(T13), T14 p14Value = default(T14), T15 p15Value = default(T15), T16 p16Value = default(T16), T17 p17Value = default(T17), T18 p18Value = default(T18), T19 p19Value = default(T19), T20 p20Value = default(T20), T21 p21Value = default(T21), T22 p22Value = default(T22), T23 p23Value = default(T23), T24 p24Value = default(T24), T25 p25Value = default(T25), T26 p26Value = default(T26), T27 p27Value = default(T27), T28 p28Value = default(T28), T29 p29Value = default(T29), Action<SqlDataReader> resultAction = null, CommandBehavior behavior = CommandBehavior.Default, TypeConstraintMode? constraintMode = null, object state = null)
        {
            await this.ExecuteReaderAllAsync(c => c.SetParameters(ProgramParameters, p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, p7Value, p8Value, p9Value, p10Value, p11Value, p12Value, p13Value, p14Value, p15Value, p16Value, p17Value, p18Value, p19Value, p20Value, p21Value, p22Value, p23Value, p24Value, p25Value, p26Value, p27Value, p28Value, p29Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), resultAction, behavior, state);
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
        /// <param name="p16Value">Value of SQL Parameter 16.</param>
        /// <param name="p17Value">Value of SQL Parameter 17.</param>
        /// <param name="p18Value">Value of SQL Parameter 18.</param>
        /// <param name="p19Value">Value of SQL Parameter 19.</param>
        /// <param name="p20Value">Value of SQL Parameter 20.</param>
        /// <param name="p21Value">Value of SQL Parameter 21.</param>
        /// <param name="p22Value">Value of SQL Parameter 22.</param>
        /// <param name="p23Value">Value of SQL Parameter 23.</param>
        /// <param name="p24Value">Value of SQL Parameter 24.</param>
        /// <param name="p25Value">Value of SQL Parameter 25.</param>
        /// <param name="p26Value">Value of SQL Parameter 26.</param>
        /// <param name="p27Value">Value of SQL Parameter 27.</param>
        /// <param name="p28Value">Value of SQL Parameter 28.</param>
        /// <param name="p29Value">Value of SQL Parameter 29.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configurated default for this program.</param>
        /// <param name="resultFunc">The result function.</param>
        /// <param name="behavior">The behaviour.</param>
        /// <returns></returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public TOut ExecuteReader<TOut>(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), T7 p7Value = default(T7), T8 p8Value = default(T8), T9 p9Value = default(T9), T10 p10Value = default(T10), T11 p11Value = default(T11), T12 p12Value = default(T12), T13 p13Value = default(T13), T14 p14Value = default(T14), T15 p15Value = default(T15), T16 p16Value = default(T16), T17 p17Value = default(T17), T18 p18Value = default(T18), T19 p19Value = default(T19), T20 p20Value = default(T20), T21 p21Value = default(T21), T22 p22Value = default(T22), T23 p23Value = default(T23), T24 p24Value = default(T24), T25 p25Value = default(T25), T26 p26Value = default(T26), T27 p27Value = default(T27), T28 p28Value = default(T28), T29 p29Value = default(T29), Func<SqlDataReader, TOut> resultFunc = null, CommandBehavior behavior = CommandBehavior.Default, TypeConstraintMode? constraintMode = null)
        {
            return this.ExecuteReader(c => c.SetParameters(ProgramParameters, p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, p7Value, p8Value, p9Value, p10Value, p11Value, p12Value, p13Value, p14Value, p15Value, p16Value, p17Value, p18Value, p19Value, p20Value, p21Value, p22Value, p23Value, p24Value, p25Value, p26Value, p27Value, p28Value, p29Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), resultFunc, behavior);
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
        /// <param name="p16Value">Value of SQL Parameter 16.</param>
        /// <param name="p17Value">Value of SQL Parameter 17.</param>
        /// <param name="p18Value">Value of SQL Parameter 18.</param>
        /// <param name="p19Value">Value of SQL Parameter 19.</param>
        /// <param name="p20Value">Value of SQL Parameter 20.</param>
        /// <param name="p21Value">Value of SQL Parameter 21.</param>
        /// <param name="p22Value">Value of SQL Parameter 22.</param>
        /// <param name="p23Value">Value of SQL Parameter 23.</param>
        /// <param name="p24Value">Value of SQL Parameter 24.</param>
        /// <param name="p25Value">Value of SQL Parameter 25.</param>
        /// <param name="p26Value">Value of SQL Parameter 26.</param>
        /// <param name="p27Value">Value of SQL Parameter 27.</param>
        /// <param name="p28Value">Value of SQL Parameter 28.</param>
        /// <param name="p29Value">Value of SQL Parameter 29.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configurated default for this program.</param>
        /// <param name="resultFunc">The result function.</param>
        /// <param name="behavior">The behaviour.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public async Task<TOut> ExecuteReaderAsync<TOut>(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), T7 p7Value = default(T7), T8 p8Value = default(T8), T9 p9Value = default(T9), T10 p10Value = default(T10), T11 p11Value = default(T11), T12 p12Value = default(T12), T13 p13Value = default(T13), T14 p14Value = default(T14), T15 p15Value = default(T15), T16 p16Value = default(T16), T17 p17Value = default(T17), T18 p18Value = default(T18), T19 p19Value = default(T19), T20 p20Value = default(T20), T21 p21Value = default(T21), T22 p22Value = default(T22), T23 p23Value = default(T23), T24 p24Value = default(T24), T25 p25Value = default(T25), T26 p26Value = default(T26), T27 p27Value = default(T27), T28 p28Value = default(T28), T29 p29Value = default(T29), Func<SqlDataReader, TOut> resultFunc = null, CommandBehavior behavior = CommandBehavior.Default, TypeConstraintMode? constraintMode = null, object state = null)
        {
            return await this.ExecuteReaderAsync(c => c.SetParameters(ProgramParameters, p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, p7Value, p8Value, p9Value, p10Value, p11Value, p12Value, p13Value, p14Value, p15Value, p16Value, p17Value, p18Value, p19Value, p20Value, p21Value, p22Value, p23Value, p24Value, p25Value, p26Value, p27Value, p28Value, p29Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), resultFunc, behavior, state);
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
        /// <param name="p16Value">Value of SQL Parameter 16.</param>
        /// <param name="p17Value">Value of SQL Parameter 17.</param>
        /// <param name="p18Value">Value of SQL Parameter 18.</param>
        /// <param name="p19Value">Value of SQL Parameter 19.</param>
        /// <param name="p20Value">Value of SQL Parameter 20.</param>
        /// <param name="p21Value">Value of SQL Parameter 21.</param>
        /// <param name="p22Value">Value of SQL Parameter 22.</param>
        /// <param name="p23Value">Value of SQL Parameter 23.</param>
        /// <param name="p24Value">Value of SQL Parameter 24.</param>
        /// <param name="p25Value">Value of SQL Parameter 25.</param>
        /// <param name="p26Value">Value of SQL Parameter 26.</param>
        /// <param name="p27Value">Value of SQL Parameter 27.</param>
        /// <param name="p28Value">Value of SQL Parameter 28.</param>
        /// <param name="p29Value">Value of SQL Parameter 29.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configurated default for this program.</param>
        /// <param name="resultFunc">The result function.</param>
        /// <param name="behavior">The behaviour.</param>
        /// <returns></returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public IEnumerable<TOut> ExecuteReaderAll<TOut>(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), T7 p7Value = default(T7), T8 p8Value = default(T8), T9 p9Value = default(T9), T10 p10Value = default(T10), T11 p11Value = default(T11), T12 p12Value = default(T12), T13 p13Value = default(T13), T14 p14Value = default(T14), T15 p15Value = default(T15), T16 p16Value = default(T16), T17 p17Value = default(T17), T18 p18Value = default(T18), T19 p19Value = default(T19), T20 p20Value = default(T20), T21 p21Value = default(T21), T22 p22Value = default(T22), T23 p23Value = default(T23), T24 p24Value = default(T24), T25 p25Value = default(T25), T26 p26Value = default(T26), T27 p27Value = default(T27), T28 p28Value = default(T28), T29 p29Value = default(T29), Func<SqlDataReader, TOut> resultFunc = null, CommandBehavior behavior = CommandBehavior.Default, TypeConstraintMode? constraintMode = null)
        {
            return this.ExecuteReaderAll(c => c.SetParameters(ProgramParameters, p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, p7Value, p8Value, p9Value, p10Value, p11Value, p12Value, p13Value, p14Value, p15Value, p16Value, p17Value, p18Value, p19Value, p20Value, p21Value, p22Value, p23Value, p24Value, p25Value, p26Value, p27Value, p28Value, p29Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), resultFunc, behavior);
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
        /// <param name="p16Value">Value of SQL Parameter 16.</param>
        /// <param name="p17Value">Value of SQL Parameter 17.</param>
        /// <param name="p18Value">Value of SQL Parameter 18.</param>
        /// <param name="p19Value">Value of SQL Parameter 19.</param>
        /// <param name="p20Value">Value of SQL Parameter 20.</param>
        /// <param name="p21Value">Value of SQL Parameter 21.</param>
        /// <param name="p22Value">Value of SQL Parameter 22.</param>
        /// <param name="p23Value">Value of SQL Parameter 23.</param>
        /// <param name="p24Value">Value of SQL Parameter 24.</param>
        /// <param name="p25Value">Value of SQL Parameter 25.</param>
        /// <param name="p26Value">Value of SQL Parameter 26.</param>
        /// <param name="p27Value">Value of SQL Parameter 27.</param>
        /// <param name="p28Value">Value of SQL Parameter 28.</param>
        /// <param name="p29Value">Value of SQL Parameter 29.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configurated default for this program.</param>
        /// <param name="resultFunc">The result function.</param>
        /// <param name="behavior">The behaviour.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public async Task<IEnumerable<TOut>> ExecuteReaderAllAsync<TOut>(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), T7 p7Value = default(T7), T8 p8Value = default(T8), T9 p9Value = default(T9), T10 p10Value = default(T10), T11 p11Value = default(T11), T12 p12Value = default(T12), T13 p13Value = default(T13), T14 p14Value = default(T14), T15 p15Value = default(T15), T16 p16Value = default(T16), T17 p17Value = default(T17), T18 p18Value = default(T18), T19 p19Value = default(T19), T20 p20Value = default(T20), T21 p21Value = default(T21), T22 p22Value = default(T22), T23 p23Value = default(T23), T24 p24Value = default(T24), T25 p25Value = default(T25), T26 p26Value = default(T26), T27 p27Value = default(T27), T28 p28Value = default(T28), T29 p29Value = default(T29), Func<SqlDataReader, TOut> resultFunc = null, CommandBehavior behavior = CommandBehavior.Default, TypeConstraintMode? constraintMode = null, object state = null)
        {
            return await this.ExecuteReaderAllAsync(c => c.SetParameters(ProgramParameters, p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, p7Value, p8Value, p9Value, p10Value, p11Value, p12Value, p13Value, p14Value, p15Value, p16Value, p17Value, p18Value, p19Value, p20Value, p21Value, p22Value, p23Value, p24Value, p25Value, p26Value, p27Value, p28Value, p29Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), resultFunc, behavior, state);
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
        /// <param name="p16Value">Value of SQL Parameter 16.</param>
        /// <param name="p17Value">Value of SQL Parameter 17.</param>
        /// <param name="p18Value">Value of SQL Parameter 18.</param>
        /// <param name="p19Value">Value of SQL Parameter 19.</param>
        /// <param name="p20Value">Value of SQL Parameter 20.</param>
        /// <param name="p21Value">Value of SQL Parameter 21.</param>
        /// <param name="p22Value">Value of SQL Parameter 22.</param>
        /// <param name="p23Value">Value of SQL Parameter 23.</param>
        /// <param name="p24Value">Value of SQL Parameter 24.</param>
        /// <param name="p25Value">Value of SQL Parameter 25.</param>
        /// <param name="p26Value">Value of SQL Parameter 26.</param>
        /// <param name="p27Value">Value of SQL Parameter 27.</param>
        /// <param name="p28Value">Value of SQL Parameter 28.</param>
        /// <param name="p29Value">Value of SQL Parameter 29.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configurated default for this program.</param>
        /// <param name="resultAction">The result function.</param>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public void ExecuteXmlReader(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), T7 p7Value = default(T7), T8 p8Value = default(T8), T9 p9Value = default(T9), T10 p10Value = default(T10), T11 p11Value = default(T11), T12 p12Value = default(T12), T13 p13Value = default(T13), T14 p14Value = default(T14), T15 p15Value = default(T15), T16 p16Value = default(T16), T17 p17Value = default(T17), T18 p18Value = default(T18), T19 p19Value = default(T19), T20 p20Value = default(T20), T21 p21Value = default(T21), T22 p22Value = default(T22), T23 p23Value = default(T23), T24 p24Value = default(T24), T25 p25Value = default(T25), T26 p26Value = default(T26), T27 p27Value = default(T27), T28 p28Value = default(T28), T29 p29Value = default(T29), Action<XmlReader> resultAction = null, TypeConstraintMode? constraintMode = null)
        {
            this.ExecuteXmlReader(c => c.SetParameters(ProgramParameters, p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, p7Value, p8Value, p9Value, p10Value, p11Value, p12Value, p13Value, p14Value, p15Value, p16Value, p17Value, p18Value, p19Value, p20Value, p21Value, p22Value, p23Value, p24Value, p25Value, p26Value, p27Value, p28Value, p29Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), resultAction);
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
        /// <param name="p16Value">Value of SQL Parameter 16.</param>
        /// <param name="p17Value">Value of SQL Parameter 17.</param>
        /// <param name="p18Value">Value of SQL Parameter 18.</param>
        /// <param name="p19Value">Value of SQL Parameter 19.</param>
        /// <param name="p20Value">Value of SQL Parameter 20.</param>
        /// <param name="p21Value">Value of SQL Parameter 21.</param>
        /// <param name="p22Value">Value of SQL Parameter 22.</param>
        /// <param name="p23Value">Value of SQL Parameter 23.</param>
        /// <param name="p24Value">Value of SQL Parameter 24.</param>
        /// <param name="p25Value">Value of SQL Parameter 25.</param>
        /// <param name="p26Value">Value of SQL Parameter 26.</param>
        /// <param name="p27Value">Value of SQL Parameter 27.</param>
        /// <param name="p28Value">Value of SQL Parameter 28.</param>
        /// <param name="p29Value">Value of SQL Parameter 29.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configurated default for this program.</param>
        /// <param name="resultAction">The result function.</param>
        /// <param name="state">The state.</param>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public async Task ExecuteXmlReaderAsync(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), T7 p7Value = default(T7), T8 p8Value = default(T8), T9 p9Value = default(T9), T10 p10Value = default(T10), T11 p11Value = default(T11), T12 p12Value = default(T12), T13 p13Value = default(T13), T14 p14Value = default(T14), T15 p15Value = default(T15), T16 p16Value = default(T16), T17 p17Value = default(T17), T18 p18Value = default(T18), T19 p19Value = default(T19), T20 p20Value = default(T20), T21 p21Value = default(T21), T22 p22Value = default(T22), T23 p23Value = default(T23), T24 p24Value = default(T24), T25 p25Value = default(T25), T26 p26Value = default(T26), T27 p27Value = default(T27), T28 p28Value = default(T28), T29 p29Value = default(T29), Action<XmlReader> resultAction = null, TypeConstraintMode? constraintMode = null, object state = null)
        {
            await this.ExecuteXmlReaderAsync(c => c.SetParameters(ProgramParameters, p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, p7Value, p8Value, p9Value, p10Value, p11Value, p12Value, p13Value, p14Value, p15Value, p16Value, p17Value, p18Value, p19Value, p20Value, p21Value, p22Value, p23Value, p24Value, p25Value, p26Value, p27Value, p28Value, p29Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), resultAction, state);
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
        /// <param name="p16Value">Value of SQL Parameter 16.</param>
        /// <param name="p17Value">Value of SQL Parameter 17.</param>
        /// <param name="p18Value">Value of SQL Parameter 18.</param>
        /// <param name="p19Value">Value of SQL Parameter 19.</param>
        /// <param name="p20Value">Value of SQL Parameter 20.</param>
        /// <param name="p21Value">Value of SQL Parameter 21.</param>
        /// <param name="p22Value">Value of SQL Parameter 22.</param>
        /// <param name="p23Value">Value of SQL Parameter 23.</param>
        /// <param name="p24Value">Value of SQL Parameter 24.</param>
        /// <param name="p25Value">Value of SQL Parameter 25.</param>
        /// <param name="p26Value">Value of SQL Parameter 26.</param>
        /// <param name="p27Value">Value of SQL Parameter 27.</param>
        /// <param name="p28Value">Value of SQL Parameter 28.</param>
        /// <param name="p29Value">Value of SQL Parameter 29.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configurated default for this program.</param>
        /// <param name="resultAction">The result function.</param>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public void ExecuteXmlReaderAll(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), T7 p7Value = default(T7), T8 p8Value = default(T8), T9 p9Value = default(T9), T10 p10Value = default(T10), T11 p11Value = default(T11), T12 p12Value = default(T12), T13 p13Value = default(T13), T14 p14Value = default(T14), T15 p15Value = default(T15), T16 p16Value = default(T16), T17 p17Value = default(T17), T18 p18Value = default(T18), T19 p19Value = default(T19), T20 p20Value = default(T20), T21 p21Value = default(T21), T22 p22Value = default(T22), T23 p23Value = default(T23), T24 p24Value = default(T24), T25 p25Value = default(T25), T26 p26Value = default(T26), T27 p27Value = default(T27), T28 p28Value = default(T28), T29 p29Value = default(T29), Action<XmlReader> resultAction = null, TypeConstraintMode? constraintMode = null)
        {
            this.ExecuteXmlReaderAll(c => c.SetParameters(ProgramParameters, p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, p7Value, p8Value, p9Value, p10Value, p11Value, p12Value, p13Value, p14Value, p15Value, p16Value, p17Value, p18Value, p19Value, p20Value, p21Value, p22Value, p23Value, p24Value, p25Value, p26Value, p27Value, p28Value, p29Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), resultAction);
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
        /// <param name="p16Value">Value of SQL Parameter 16.</param>
        /// <param name="p17Value">Value of SQL Parameter 17.</param>
        /// <param name="p18Value">Value of SQL Parameter 18.</param>
        /// <param name="p19Value">Value of SQL Parameter 19.</param>
        /// <param name="p20Value">Value of SQL Parameter 20.</param>
        /// <param name="p21Value">Value of SQL Parameter 21.</param>
        /// <param name="p22Value">Value of SQL Parameter 22.</param>
        /// <param name="p23Value">Value of SQL Parameter 23.</param>
        /// <param name="p24Value">Value of SQL Parameter 24.</param>
        /// <param name="p25Value">Value of SQL Parameter 25.</param>
        /// <param name="p26Value">Value of SQL Parameter 26.</param>
        /// <param name="p27Value">Value of SQL Parameter 27.</param>
        /// <param name="p28Value">Value of SQL Parameter 28.</param>
        /// <param name="p29Value">Value of SQL Parameter 29.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configurated default for this program.</param>
        /// <param name="resultAction">The result function.</param>
        /// <param name="state">The state.</param>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public async Task ExecuteXmlReaderAllAsync(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), T7 p7Value = default(T7), T8 p8Value = default(T8), T9 p9Value = default(T9), T10 p10Value = default(T10), T11 p11Value = default(T11), T12 p12Value = default(T12), T13 p13Value = default(T13), T14 p14Value = default(T14), T15 p15Value = default(T15), T16 p16Value = default(T16), T17 p17Value = default(T17), T18 p18Value = default(T18), T19 p19Value = default(T19), T20 p20Value = default(T20), T21 p21Value = default(T21), T22 p22Value = default(T22), T23 p23Value = default(T23), T24 p24Value = default(T24), T25 p25Value = default(T25), T26 p26Value = default(T26), T27 p27Value = default(T27), T28 p28Value = default(T28), T29 p29Value = default(T29), Action<XmlReader> resultAction = null, TypeConstraintMode? constraintMode = null, object state = null)
        {
            await this.ExecuteXmlReaderAllAsync(c => c.SetParameters(ProgramParameters, p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, p7Value, p8Value, p9Value, p10Value, p11Value, p12Value, p13Value, p14Value, p15Value, p16Value, p17Value, p18Value, p19Value, p20Value, p21Value, p22Value, p23Value, p24Value, p25Value, p26Value, p27Value, p28Value, p29Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), resultAction, state);
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
        /// <param name="p16Value">Value of SQL Parameter 16.</param>
        /// <param name="p17Value">Value of SQL Parameter 17.</param>
        /// <param name="p18Value">Value of SQL Parameter 18.</param>
        /// <param name="p19Value">Value of SQL Parameter 19.</param>
        /// <param name="p20Value">Value of SQL Parameter 20.</param>
        /// <param name="p21Value">Value of SQL Parameter 21.</param>
        /// <param name="p22Value">Value of SQL Parameter 22.</param>
        /// <param name="p23Value">Value of SQL Parameter 23.</param>
        /// <param name="p24Value">Value of SQL Parameter 24.</param>
        /// <param name="p25Value">Value of SQL Parameter 25.</param>
        /// <param name="p26Value">Value of SQL Parameter 26.</param>
        /// <param name="p27Value">Value of SQL Parameter 27.</param>
        /// <param name="p28Value">Value of SQL Parameter 28.</param>
        /// <param name="p29Value">Value of SQL Parameter 29.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configurated default for this program.</param>
        /// <param name="resultFunc">The result function.</param>
        /// <returns></returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public TOut ExecuteXmlReader<TOut>(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), T7 p7Value = default(T7), T8 p8Value = default(T8), T9 p9Value = default(T9), T10 p10Value = default(T10), T11 p11Value = default(T11), T12 p12Value = default(T12), T13 p13Value = default(T13), T14 p14Value = default(T14), T15 p15Value = default(T15), T16 p16Value = default(T16), T17 p17Value = default(T17), T18 p18Value = default(T18), T19 p19Value = default(T19), T20 p20Value = default(T20), T21 p21Value = default(T21), T22 p22Value = default(T22), T23 p23Value = default(T23), T24 p24Value = default(T24), T25 p25Value = default(T25), T26 p26Value = default(T26), T27 p27Value = default(T27), T28 p28Value = default(T28), T29 p29Value = default(T29), Func<XmlReader, TOut> resultFunc = null, TypeConstraintMode? constraintMode = null)
        {
            return this.ExecuteXmlReader(c => c.SetParameters(ProgramParameters, p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, p7Value, p8Value, p9Value, p10Value, p11Value, p12Value, p13Value, p14Value, p15Value, p16Value, p17Value, p18Value, p19Value, p20Value, p21Value, p22Value, p23Value, p24Value, p25Value, p26Value, p27Value, p28Value, p29Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), resultFunc);
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
        /// <param name="p16Value">Value of SQL Parameter 16.</param>
        /// <param name="p17Value">Value of SQL Parameter 17.</param>
        /// <param name="p18Value">Value of SQL Parameter 18.</param>
        /// <param name="p19Value">Value of SQL Parameter 19.</param>
        /// <param name="p20Value">Value of SQL Parameter 20.</param>
        /// <param name="p21Value">Value of SQL Parameter 21.</param>
        /// <param name="p22Value">Value of SQL Parameter 22.</param>
        /// <param name="p23Value">Value of SQL Parameter 23.</param>
        /// <param name="p24Value">Value of SQL Parameter 24.</param>
        /// <param name="p25Value">Value of SQL Parameter 25.</param>
        /// <param name="p26Value">Value of SQL Parameter 26.</param>
        /// <param name="p27Value">Value of SQL Parameter 27.</param>
        /// <param name="p28Value">Value of SQL Parameter 28.</param>
        /// <param name="p29Value">Value of SQL Parameter 29.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configurated default for this program.</param>
        /// <param name="resultFunc">The result function.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public async Task<TOut> ExecuteXmlReaderAsync<TOut>(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), T7 p7Value = default(T7), T8 p8Value = default(T8), T9 p9Value = default(T9), T10 p10Value = default(T10), T11 p11Value = default(T11), T12 p12Value = default(T12), T13 p13Value = default(T13), T14 p14Value = default(T14), T15 p15Value = default(T15), T16 p16Value = default(T16), T17 p17Value = default(T17), T18 p18Value = default(T18), T19 p19Value = default(T19), T20 p20Value = default(T20), T21 p21Value = default(T21), T22 p22Value = default(T22), T23 p23Value = default(T23), T24 p24Value = default(T24), T25 p25Value = default(T25), T26 p26Value = default(T26), T27 p27Value = default(T27), T28 p28Value = default(T28), T29 p29Value = default(T29), Func<XmlReader, TOut> resultFunc = null, TypeConstraintMode? constraintMode = null, object state = null)
        {
            return await this.ExecuteXmlReaderAsync(c => c.SetParameters(ProgramParameters, p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, p7Value, p8Value, p9Value, p10Value, p11Value, p12Value, p13Value, p14Value, p15Value, p16Value, p17Value, p18Value, p19Value, p20Value, p21Value, p22Value, p23Value, p24Value, p25Value, p26Value, p27Value, p28Value, p29Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), resultFunc, state);
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
        /// <param name="p16Value">Value of SQL Parameter 16.</param>
        /// <param name="p17Value">Value of SQL Parameter 17.</param>
        /// <param name="p18Value">Value of SQL Parameter 18.</param>
        /// <param name="p19Value">Value of SQL Parameter 19.</param>
        /// <param name="p20Value">Value of SQL Parameter 20.</param>
        /// <param name="p21Value">Value of SQL Parameter 21.</param>
        /// <param name="p22Value">Value of SQL Parameter 22.</param>
        /// <param name="p23Value">Value of SQL Parameter 23.</param>
        /// <param name="p24Value">Value of SQL Parameter 24.</param>
        /// <param name="p25Value">Value of SQL Parameter 25.</param>
        /// <param name="p26Value">Value of SQL Parameter 26.</param>
        /// <param name="p27Value">Value of SQL Parameter 27.</param>
        /// <param name="p28Value">Value of SQL Parameter 28.</param>
        /// <param name="p29Value">Value of SQL Parameter 29.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configurated default for this program.</param>
        /// <param name="resultFunc">The result function.</param>
        /// <returns></returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public IEnumerable<TOut> ExecuteXmlReaderAll<TOut>(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), T7 p7Value = default(T7), T8 p8Value = default(T8), T9 p9Value = default(T9), T10 p10Value = default(T10), T11 p11Value = default(T11), T12 p12Value = default(T12), T13 p13Value = default(T13), T14 p14Value = default(T14), T15 p15Value = default(T15), T16 p16Value = default(T16), T17 p17Value = default(T17), T18 p18Value = default(T18), T19 p19Value = default(T19), T20 p20Value = default(T20), T21 p21Value = default(T21), T22 p22Value = default(T22), T23 p23Value = default(T23), T24 p24Value = default(T24), T25 p25Value = default(T25), T26 p26Value = default(T26), T27 p27Value = default(T27), T28 p28Value = default(T28), T29 p29Value = default(T29), Func<XmlReader, TOut> resultFunc = null, TypeConstraintMode? constraintMode = null)
        {
            return this.ExecuteXmlReaderAll(c => c.SetParameters(ProgramParameters, p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, p7Value, p8Value, p9Value, p10Value, p11Value, p12Value, p13Value, p14Value, p15Value, p16Value, p17Value, p18Value, p19Value, p20Value, p21Value, p22Value, p23Value, p24Value, p25Value, p26Value, p27Value, p28Value, p29Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), resultFunc);
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
        /// <param name="p16Value">Value of SQL Parameter 16.</param>
        /// <param name="p17Value">Value of SQL Parameter 17.</param>
        /// <param name="p18Value">Value of SQL Parameter 18.</param>
        /// <param name="p19Value">Value of SQL Parameter 19.</param>
        /// <param name="p20Value">Value of SQL Parameter 20.</param>
        /// <param name="p21Value">Value of SQL Parameter 21.</param>
        /// <param name="p22Value">Value of SQL Parameter 22.</param>
        /// <param name="p23Value">Value of SQL Parameter 23.</param>
        /// <param name="p24Value">Value of SQL Parameter 24.</param>
        /// <param name="p25Value">Value of SQL Parameter 25.</param>
        /// <param name="p26Value">Value of SQL Parameter 26.</param>
        /// <param name="p27Value">Value of SQL Parameter 27.</param>
        /// <param name="p28Value">Value of SQL Parameter 28.</param>
        /// <param name="p29Value">Value of SQL Parameter 29.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configurated default for this program.</param>
        /// <param name="resultFunc">The result function.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        /// <exception cref="SqlProgramExecutionException">An error occurred executing the program.</exception>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.ReflectionPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="MemberAccess"/><IPermission class="System.Security.Permissions.RegistryPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence, ControlPolicy, ControlAppDomain"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Data.SqlClient.SqlClientPermission, System.Data, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public async Task<IEnumerable<TOut>> ExecuteXmlReaderAllAsync<TOut>(T1 p1Value = default(T1), T2 p2Value = default(T2), T3 p3Value = default(T3), T4 p4Value = default(T4), T5 p5Value = default(T5), T6 p6Value = default(T6), T7 p7Value = default(T7), T8 p8Value = default(T8), T9 p9Value = default(T9), T10 p10Value = default(T10), T11 p11Value = default(T11), T12 p12Value = default(T12), T13 p13Value = default(T13), T14 p14Value = default(T14), T15 p15Value = default(T15), T16 p16Value = default(T16), T17 p17Value = default(T17), T18 p18Value = default(T18), T19 p19Value = default(T19), T20 p20Value = default(T20), T21 p21Value = default(T21), T22 p22Value = default(T22), T23 p23Value = default(T23), T24 p24Value = default(T24), T25 p25Value = default(T25), T26 p26Value = default(T26), T27 p27Value = default(T27), T28 p28Value = default(T28), T29 p29Value = default(T29), Func<XmlReader, TOut> resultFunc = null, TypeConstraintMode? constraintMode = null, object state = null)
        {
            return await this.ExecuteXmlReaderAllAsync(c => c.SetParameters(ProgramParameters, p1Value, p2Value, p3Value, p4Value, p5Value, p6Value, p7Value, p8Value, p9Value, p10Value, p11Value, p12Value, p13Value, p14Value, p15Value, p16Value, p17Value, p18Value, p19Value, p20Value, p21Value, p22Value, p23Value, p24Value, p25Value, p26Value, p27Value, p28Value, p29Value, (TypeConstraintMode)(constraintMode ?? ConstraintMode)), resultFunc, state);
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
        /// <typeparam name="T16">The type of parameter 16.</typeparam>
        /// <typeparam name="T17">The type of parameter 17.</typeparam>
        /// <typeparam name="T18">The type of parameter 18.</typeparam>
        /// <typeparam name="T19">The type of parameter 19.</typeparam>
        /// <typeparam name="T20">The type of parameter 20.</typeparam>
        /// <typeparam name="T21">The type of parameter 21.</typeparam>
        /// <typeparam name="T22">The type of parameter 22.</typeparam>
        /// <typeparam name="T23">The type of parameter 23.</typeparam>
        /// <typeparam name="T24">The type of parameter 24.</typeparam>
        /// <typeparam name="T25">The type of parameter 25.</typeparam>
        /// <typeparam name="T26">The type of parameter 26.</typeparam>
        /// <typeparam name="T27">The type of parameter 27.</typeparam>
        /// <typeparam name="T28">The type of parameter 28.</typeparam>
        /// <typeparam name="T29">The type of parameter 29.</typeparam>
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
        /// <param name="p16Name">Name of parameter 16.</param>
        /// <param name="p17Name">Name of parameter 17.</param>
        /// <param name="p18Name">Name of parameter 18.</param>
        /// <param name="p19Name">Name of parameter 19.</param>
        /// <param name="p20Name">Name of parameter 20.</param>
        /// <param name="p21Name">Name of parameter 21.</param>
        /// <param name="p22Name">Name of parameter 22.</param>
        /// <param name="p23Name">Name of parameter 23.</param>
        /// <param name="p24Name">Name of parameter 24.</param>
        /// <param name="p25Name">Name of parameter 25.</param>
        /// <param name="p26Name">Name of parameter 26.</param>
        /// <param name="p27Name">Name of parameter 27.</param>
        /// <param name="p28Name">Name of parameter 28.</param>
        /// <param name="p29Name">Name of parameter 29.</param>
        /// <param name="ignoreValidationErrors">if set to <see langword="true"/> will ignore validation errors regardless of configuration.</param>
        /// <param name="checkOrder">if set to <see langword="true"/> will check parameter order matches regardless of configuration.</param>
        /// <param name="defaultCommandTimeout">The default command timeout, if set will override the configuration.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configuration.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        [NotNull]
        public static SqlProgram<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29> GetConfiguredSqlProgram<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29>(
            [NotNull] string database,
            [NotNull] string name,
            string p1Name, string p2Name, string p3Name, string p4Name, string p5Name, string p6Name, string p7Name, string p8Name, string p9Name, string p10Name, string p11Name, string p12Name, string p13Name, string p14Name, string p15Name, string p16Name, string p17Name, string p18Name, string p19Name, string p20Name, string p21Name, string p22Name, string p23Name, string p24Name, string p25Name, string p26Name, string p27Name, string p28Name, string p29Name,
            bool ignoreValidationErrors = false,
            bool checkOrder = false,
            TimeSpan? defaultCommandTimeout = null,
            TypeConstraintMode? constraintMode = null
            )
        {
            return Active.GetSqlProgram<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29>(database, name, p1Name, p2Name, p3Name, p4Name, p5Name, p6Name, p7Name, p8Name, p9Name, p10Name, p11Name, p12Name, p13Name, p14Name, p15Name, p16Name, p17Name, p18Name, p19Name, p20Name, p21Name, p22Name, p23Name, p24Name, p25Name, p26Name, p27Name, p28Name, p29Name,
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
        /// <typeparam name="T7">The type of parameter 7.</typeparam>
        /// <typeparam name="T8">The type of parameter 8.</typeparam>
        /// <typeparam name="T9">The type of parameter 9.</typeparam>
        /// <typeparam name="T10">The type of parameter 10.</typeparam>
        /// <typeparam name="T11">The type of parameter 11.</typeparam>
        /// <typeparam name="T12">The type of parameter 12.</typeparam>
        /// <typeparam name="T13">The type of parameter 13.</typeparam>
        /// <typeparam name="T14">The type of parameter 14.</typeparam>
        /// <typeparam name="T15">The type of parameter 15.</typeparam>
        /// <typeparam name="T16">The type of parameter 16.</typeparam>
        /// <typeparam name="T17">The type of parameter 17.</typeparam>
        /// <typeparam name="T18">The type of parameter 18.</typeparam>
        /// <typeparam name="T19">The type of parameter 19.</typeparam>
        /// <typeparam name="T20">The type of parameter 20.</typeparam>
        /// <typeparam name="T21">The type of parameter 21.</typeparam>
        /// <typeparam name="T22">The type of parameter 22.</typeparam>
        /// <typeparam name="T23">The type of parameter 23.</typeparam>
        /// <typeparam name="T24">The type of parameter 24.</typeparam>
        /// <typeparam name="T25">The type of parameter 25.</typeparam>
        /// <typeparam name="T26">The type of parameter 26.</typeparam>
        /// <typeparam name="T27">The type of parameter 27.</typeparam>
        /// <typeparam name="T28">The type of parameter 28.</typeparam>
        /// <typeparam name="T29">The type of parameter 29.</typeparam>
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
        /// <param name="p16Name">Name of parameter 16.</param>
        /// <param name="p17Name">Name of parameter 17.</param>
        /// <param name="p18Name">Name of parameter 18.</param>
        /// <param name="p19Name">Name of parameter 19.</param>
        /// <param name="p20Name">Name of parameter 20.</param>
        /// <param name="p21Name">Name of parameter 21.</param>
        /// <param name="p22Name">Name of parameter 22.</param>
        /// <param name="p23Name">Name of parameter 23.</param>
        /// <param name="p24Name">Name of parameter 24.</param>
        /// <param name="p25Name">Name of parameter 25.</param>
        /// <param name="p26Name">Name of parameter 26.</param>
        /// <param name="p27Name">Name of parameter 27.</param>
        /// <param name="p28Name">Name of parameter 28.</param>
        /// <param name="p29Name">Name of parameter 29.</param>
        /// <param name="ignoreValidationErrors">if set to <see langword="true"/> will ignore validation errors regardless of configuration.</param>
        /// <param name="checkOrder">if set to <see langword="true"/> will check parameter order matches regardless of configuration.</param>
        /// <param name="defaultCommandTimeout">The default command timeout, if set will override the configuration.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configuration.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        [NotNull]
        public SqlProgram<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29> GetSqlProgram<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29>(
            [NotNull] string database, 
            [NotNull] string name,
            string p1Name, string p2Name, string p3Name, string p4Name, string p5Name, string p6Name, string p7Name, string p8Name, string p9Name, string p10Name, string p11Name, string p12Name, string p13Name, string p14Name, string p15Name, string p16Name, string p17Name, string p18Name, string p19Name, string p20Name, string p21Name, string p22Name, string p23Name, string p24Name, string p25Name, string p26Name, string p27Name, string p28Name, string p29Name,
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

            return db.GetSqlProgram<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29>(name, p1Name, p2Name, p3Name, p4Name, p5Name, p6Name, p7Name, p8Name, p9Name, p10Name, p11Name, p12Name, p13Name, p14Name, p15Name, p16Name, p17Name, p18Name, p19Name, p20Name, p21Name, p22Name, p23Name, p24Name, p25Name, p26Name, p27Name, p28Name, p29Name, ignoreValidationErrors, checkOrder, defaultCommandTimeout, constraintMode);
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
        /// <typeparam name="T16">The type of parameter 16.</typeparam>
        /// <typeparam name="T17">The type of parameter 17.</typeparam>
        /// <typeparam name="T18">The type of parameter 18.</typeparam>
        /// <typeparam name="T19">The type of parameter 19.</typeparam>
        /// <typeparam name="T20">The type of parameter 20.</typeparam>
        /// <typeparam name="T21">The type of parameter 21.</typeparam>
        /// <typeparam name="T22">The type of parameter 22.</typeparam>
        /// <typeparam name="T23">The type of parameter 23.</typeparam>
        /// <typeparam name="T24">The type of parameter 24.</typeparam>
        /// <typeparam name="T25">The type of parameter 25.</typeparam>
        /// <typeparam name="T26">The type of parameter 26.</typeparam>
        /// <typeparam name="T27">The type of parameter 27.</typeparam>
        /// <typeparam name="T28">The type of parameter 28.</typeparam>
        /// <typeparam name="T29">The type of parameter 29.</typeparam>
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
        /// <param name="p16Name">Name of parameter 16.</param>
        /// <param name="p17Name">Name of parameter 17.</param>
        /// <param name="p18Name">Name of parameter 18.</param>
        /// <param name="p19Name">Name of parameter 19.</param>
        /// <param name="p20Name">Name of parameter 20.</param>
        /// <param name="p21Name">Name of parameter 21.</param>
        /// <param name="p22Name">Name of parameter 22.</param>
        /// <param name="p23Name">Name of parameter 23.</param>
        /// <param name="p24Name">Name of parameter 24.</param>
        /// <param name="p25Name">Name of parameter 25.</param>
        /// <param name="p26Name">Name of parameter 26.</param>
        /// <param name="p27Name">Name of parameter 27.</param>
        /// <param name="p28Name">Name of parameter 28.</param>
        /// <param name="p29Name">Name of parameter 29.</param>
        /// <param name="ignoreValidationErrors">if set to <see langword="true"/> will ignore validation errors regardless of configuration.</param>
        /// <param name="checkOrder">if set to <see langword="true"/> will check parameter order matches regardless of configuration.</param>
        /// <param name="defaultCommandTimeout">The default command timeout, if set will override the configuration.</param>
        /// <param name="constraintMode">The constraint mode, if set will override the configuration.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        [NotNull]
        public SqlProgram<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29> GetSqlProgram<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29>(
            [NotNull] string name,
            string p1Name, string p2Name, string p3Name, string p4Name, string p5Name, string p6Name, string p7Name, string p8Name, string p9Name, string p10Name, string p11Name, string p12Name, string p13Name, string p14Name, string p15Name, string p16Name, string p17Name, string p18Name, string p19Name, string p20Name, string p21Name, string p22Name, string p23Name, string p24Name, string p25Name, string p26Name, string p27Name, string p28Name, string p29Name,
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
                param = prog.Parameters[p7Name];
                if (param != null) {
                    if (String.IsNullOrWhiteSpace(param.MapTo))
                        throw new LoggingException(
                            "Must specify a valid mapping for '{0}' parameter on '{1}' program.",
                            LogLevel.Error, p7Name, prog.Name);
                
                    p7Name = param.MapTo;
                }
                param = prog.Parameters[p8Name];
                if (param != null) {
                    if (String.IsNullOrWhiteSpace(param.MapTo))
                        throw new LoggingException(
                            "Must specify a valid mapping for '{0}' parameter on '{1}' program.",
                            LogLevel.Error, p8Name, prog.Name);
                
                    p8Name = param.MapTo;
                }
                param = prog.Parameters[p9Name];
                if (param != null) {
                    if (String.IsNullOrWhiteSpace(param.MapTo))
                        throw new LoggingException(
                            "Must specify a valid mapping for '{0}' parameter on '{1}' program.",
                            LogLevel.Error, p9Name, prog.Name);
                
                    p9Name = param.MapTo;
                }
                param = prog.Parameters[p10Name];
                if (param != null) {
                    if (String.IsNullOrWhiteSpace(param.MapTo))
                        throw new LoggingException(
                            "Must specify a valid mapping for '{0}' parameter on '{1}' program.",
                            LogLevel.Error, p10Name, prog.Name);
                
                    p10Name = param.MapTo;
                }
                param = prog.Parameters[p11Name];
                if (param != null) {
                    if (String.IsNullOrWhiteSpace(param.MapTo))
                        throw new LoggingException(
                            "Must specify a valid mapping for '{0}' parameter on '{1}' program.",
                            LogLevel.Error, p11Name, prog.Name);
                
                    p11Name = param.MapTo;
                }
                param = prog.Parameters[p12Name];
                if (param != null) {
                    if (String.IsNullOrWhiteSpace(param.MapTo))
                        throw new LoggingException(
                            "Must specify a valid mapping for '{0}' parameter on '{1}' program.",
                            LogLevel.Error, p12Name, prog.Name);
                
                    p12Name = param.MapTo;
                }
                param = prog.Parameters[p13Name];
                if (param != null) {
                    if (String.IsNullOrWhiteSpace(param.MapTo))
                        throw new LoggingException(
                            "Must specify a valid mapping for '{0}' parameter on '{1}' program.",
                            LogLevel.Error, p13Name, prog.Name);
                
                    p13Name = param.MapTo;
                }
                param = prog.Parameters[p14Name];
                if (param != null) {
                    if (String.IsNullOrWhiteSpace(param.MapTo))
                        throw new LoggingException(
                            "Must specify a valid mapping for '{0}' parameter on '{1}' program.",
                            LogLevel.Error, p14Name, prog.Name);
                
                    p14Name = param.MapTo;
                }
                param = prog.Parameters[p15Name];
                if (param != null) {
                    if (String.IsNullOrWhiteSpace(param.MapTo))
                        throw new LoggingException(
                            "Must specify a valid mapping for '{0}' parameter on '{1}' program.",
                            LogLevel.Error, p15Name, prog.Name);
                
                    p15Name = param.MapTo;
                }
                param = prog.Parameters[p16Name];
                if (param != null) {
                    if (String.IsNullOrWhiteSpace(param.MapTo))
                        throw new LoggingException(
                            "Must specify a valid mapping for '{0}' parameter on '{1}' program.",
                            LogLevel.Error, p16Name, prog.Name);
                
                    p16Name = param.MapTo;
                }
                param = prog.Parameters[p17Name];
                if (param != null) {
                    if (String.IsNullOrWhiteSpace(param.MapTo))
                        throw new LoggingException(
                            "Must specify a valid mapping for '{0}' parameter on '{1}' program.",
                            LogLevel.Error, p17Name, prog.Name);
                
                    p17Name = param.MapTo;
                }
                param = prog.Parameters[p18Name];
                if (param != null) {
                    if (String.IsNullOrWhiteSpace(param.MapTo))
                        throw new LoggingException(
                            "Must specify a valid mapping for '{0}' parameter on '{1}' program.",
                            LogLevel.Error, p18Name, prog.Name);
                
                    p18Name = param.MapTo;
                }
                param = prog.Parameters[p19Name];
                if (param != null) {
                    if (String.IsNullOrWhiteSpace(param.MapTo))
                        throw new LoggingException(
                            "Must specify a valid mapping for '{0}' parameter on '{1}' program.",
                            LogLevel.Error, p19Name, prog.Name);
                
                    p19Name = param.MapTo;
                }
                param = prog.Parameters[p20Name];
                if (param != null) {
                    if (String.IsNullOrWhiteSpace(param.MapTo))
                        throw new LoggingException(
                            "Must specify a valid mapping for '{0}' parameter on '{1}' program.",
                            LogLevel.Error, p20Name, prog.Name);
                
                    p20Name = param.MapTo;
                }
                param = prog.Parameters[p21Name];
                if (param != null) {
                    if (String.IsNullOrWhiteSpace(param.MapTo))
                        throw new LoggingException(
                            "Must specify a valid mapping for '{0}' parameter on '{1}' program.",
                            LogLevel.Error, p21Name, prog.Name);
                
                    p21Name = param.MapTo;
                }
                param = prog.Parameters[p22Name];
                if (param != null) {
                    if (String.IsNullOrWhiteSpace(param.MapTo))
                        throw new LoggingException(
                            "Must specify a valid mapping for '{0}' parameter on '{1}' program.",
                            LogLevel.Error, p22Name, prog.Name);
                
                    p22Name = param.MapTo;
                }
                param = prog.Parameters[p23Name];
                if (param != null) {
                    if (String.IsNullOrWhiteSpace(param.MapTo))
                        throw new LoggingException(
                            "Must specify a valid mapping for '{0}' parameter on '{1}' program.",
                            LogLevel.Error, p23Name, prog.Name);
                
                    p23Name = param.MapTo;
                }
                param = prog.Parameters[p24Name];
                if (param != null) {
                    if (String.IsNullOrWhiteSpace(param.MapTo))
                        throw new LoggingException(
                            "Must specify a valid mapping for '{0}' parameter on '{1}' program.",
                            LogLevel.Error, p24Name, prog.Name);
                
                    p24Name = param.MapTo;
                }
                param = prog.Parameters[p25Name];
                if (param != null) {
                    if (String.IsNullOrWhiteSpace(param.MapTo))
                        throw new LoggingException(
                            "Must specify a valid mapping for '{0}' parameter on '{1}' program.",
                            LogLevel.Error, p25Name, prog.Name);
                
                    p25Name = param.MapTo;
                }
                param = prog.Parameters[p26Name];
                if (param != null) {
                    if (String.IsNullOrWhiteSpace(param.MapTo))
                        throw new LoggingException(
                            "Must specify a valid mapping for '{0}' parameter on '{1}' program.",
                            LogLevel.Error, p26Name, prog.Name);
                
                    p26Name = param.MapTo;
                }
                param = prog.Parameters[p27Name];
                if (param != null) {
                    if (String.IsNullOrWhiteSpace(param.MapTo))
                        throw new LoggingException(
                            "Must specify a valid mapping for '{0}' parameter on '{1}' program.",
                            LogLevel.Error, p27Name, prog.Name);
                
                    p27Name = param.MapTo;
                }
                param = prog.Parameters[p28Name];
                if (param != null) {
                    if (String.IsNullOrWhiteSpace(param.MapTo))
                        throw new LoggingException(
                            "Must specify a valid mapping for '{0}' parameter on '{1}' program.",
                            LogLevel.Error, p28Name, prog.Name);
                
                    p28Name = param.MapTo;
                }
                param = prog.Parameters[p29Name];
                if (param != null) {
                    if (String.IsNullOrWhiteSpace(param.MapTo))
                        throw new LoggingException(
                            "Must specify a valid mapping for '{0}' parameter on '{1}' program.",
                            LogLevel.Error, p29Name, prog.Name);
                
                    p29Name = param.MapTo;
                }
            }
			
            if (constraintMode == null) constraintMode = TypeConstraintMode.Warn;

            return new SqlProgram<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29>(connection, name, p1Name, p2Name, p3Name, p4Name, p5Name, p6Name, p7Name, p8Name, p9Name, p10Name, p11Name, p12Name, p13Name, p14Name, p15Name, p16Name, p17Name, p18Name, p19Name, p20Name, p21Name, p22Name, p23Name, p24Name, p25Name, p26Name, p27Name, p28Name, p29Name, ignoreValidationErrors, checkOrder, defaultCommandTimeout, (TypeConstraintMode) constraintMode);
        }
    }
    #endregion
}
#endregion
        