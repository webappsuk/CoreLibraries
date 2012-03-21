#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities.Logging 
// Project: WebApplications.Utilities.Logging
// File: TraceLogger.cs
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

using System.Diagnostics;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Logging.Loggers
{
    /// <summary>
    ///   Allows logging to the debug window.
    /// </summary>
    [UsedImplicitly]
    internal class TraceLogger : LoggerBase
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="TraceLogger" /> class.
        /// </summary>
        /// <param name="name">The logger name.</param>
        /// <param name="validLevels">The valid log levels.</param>
        public TraceLogger([NotNull]string name, LogLevels validLevels)
            : base(name, false, validLevels)
        {
        }

        /// <summary>
        ///   Adds the specified log to the <see cref="DefaultTraceListener"/>.
        /// </summary>
        /// <param name="log">The log to write in time order.</param>
        public override void Add(Log log)
        {
            Trace.WriteLine(log.ToString());
        }

        /// <summary>
        ///   Flushes the logger.
        ///   This should be overridden in classes that require Flush logic.
        /// </summary>
        public override void Flush()
        {
            Trace.Flush();
        }
    }
}