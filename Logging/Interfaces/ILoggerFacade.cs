#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities.Logging 
// Project: WebApplications.Utilities.Logging
// File: ILoggerFacade.cs
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

namespace WebApplications.Utilities.Logging.Interfaces
{
    /// <summary>
    ///   A simple Logger interface.
    /// </summary>
    public interface ILoggerFacade
    {
        /// <summary>
        ///   Adds the specified log.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="level">The log level.</param>
        /// <param name="parameters">The parameters.</param>
        [StringFormatMethod("message")]
        void Add(string message, LogLevel level, params object[] parameters);

        /// <summary>
        ///   Adds the specified log.
        /// </summary>
        /// <param name="logGroup">The unique ID to group log items together.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="level">The log level.</param>
        /// <param name="parameters">The parameters.</param>
        [StringFormatMethod("message")]
        void Add(Guid logGroup, string message, LogLevel level, params object[] parameters);
    }
}