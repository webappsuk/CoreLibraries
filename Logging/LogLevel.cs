#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities.Logging 
// Project: WebApplications.Utilities.Logging
// File: LogLevel.cs
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

using System.ComponentModel;

namespace WebApplications.Utilities.Logging
{
    /// <summary>
    ///   The severity of a Log Entry.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        ///   Used for critical, unrecoverable errors that can cause damage. The system should be stopped immediately.
        /// </summary>
        [Description("Used for critical, unrecoverable errors that can cause damage.")]
        Emergency = 128,

        /// <summary>
        ///   Used for critical, unrecoverable errors that don't cause damage.
        ///   You should stop the system and repair the error, but it may be capable of continuing in some capacity.
        /// </summary>
        [Description("Used for critical, unrecoverable errors that don't cause damage.")]
        Critical = 64,

        /// <summary>
        ///   Used for errors.
        /// </summary>
        [Description("Used for errors.")]
        Error = 32,

        /// <summary>
        ///   Used to indicate potential problems that should be addressed.
        /// </summary>
        [Description("Used to indicate potential problems that should be addressed.")]
        Warning = 16,

        /// <summary>
        ///   Used by system to notify key events.
        /// </summary>
        [Description("Used by system to notify key events.")]
        SystemNotification = 8,

        /// <summary>
        ///   Used by modules to notify key events.
        /// </summary>
        [Description("Used by modules to notify key events.")]
        Notification = 4,

        /// <summary>
        ///   Informational use.
        /// </summary>
        [Description("Informational use.")]
        Information = 2,

        /// <summary>
        ///   Debugging information.
        /// </summary>
        [Description("Debugging information.")]
        Debugging = 1
    }
}