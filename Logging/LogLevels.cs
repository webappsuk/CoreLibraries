#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities.Logging 
// Project: WebApplications.Utilities.Logging
// File: LogLevels.cs
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
using System.ComponentModel;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Logging
{
    /// <summary>
    ///   Allows the specification of multiple <see cref="WebApplications.Utilities.Logging.LogLevel">log levels</see>.
    /// </summary>
    [Flags]
    public enum LogLevels
    {
        /// <summary>
        ///   All Levels.
        /// </summary>
        [Description("All Levels.")]
        All = Emergency | Critical | Error | Warning | SystemNotification | Notification | Information | Debugging,

        /// <summary>
        ///   No Levels.
        /// </summary>
        [Description("No Levels.")]
        None = 0,

        /// <summary>
        ///   At least critical.
        /// </summary>
        [Description("At least critical.")]
        [UsedImplicitly]
        AtLeastCritical = Emergency | Critical,

        /// <summary>
        ///   At least an error.
        /// </summary>
        [Description("At least an error.")]
        [UsedImplicitly]
        AtLeastError = Emergency | Critical | Error,

        /// <summary>
        ///   At least a warning.
        /// </summary>
        [Description("At least a warning.")]
        [UsedImplicitly]
        AtLeastWarning = Emergency | Critical | Error | Warning,

        /// <summary>
        ///   At least a system notification.
        /// </summary>
        [Description("At least a system notification.")]
        [UsedImplicitly]
        AtLeastSystemNotification = Emergency | Critical | Error | Warning | SystemNotification,

        /// <summary>
        ///   At least a notification.
        /// </summary>
        [Description("At least a notification.")]
        [UsedImplicitly]
        AtLeastNotification = Emergency | Critical | Error | Warning | SystemNotification | Notification,

        /// <summary>
        ///   At least information.
        /// </summary>
        [Description("At least information.")]
        AtLeastInformation = Emergency | Critical | Error | Warning | SystemNotification | Notification | Information,

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