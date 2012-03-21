#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: Utilities.Database 
// Project: Utilities.Database
// File: SqlProgramExecutionException.cs
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
using JetBrains.Annotations;
using WebApplications.Utilities.Logging;

namespace WebApplications.Utilities.Database.Exceptions
{
    /// <summary>
    ///   Exceptions thrown during the execution of a <see cref="WebApplications.Utilities.Database.SqlProgram"/>.
    /// </summary>
    public class SqlProgramExecutionException : LoggingException
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="SqlProgramExecutionException"/> class.
        /// </summary>
        /// <param name="sqlProgram">The executing SQL program.</param>
        /// <param name="innerException">The inner exception.</param>
        internal SqlProgramExecutionException([NotNull]SqlProgram sqlProgram, [NotNull]Exception innerException)
            : base(
                innerException,
                Resources.SqlProgramExecutionException_ErrorOccurredDuringExecution,
                LogLevel.Error,
                sqlProgram.Name,
                innerException.Message)
        {
        }
    }
}