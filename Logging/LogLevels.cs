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
        [Description("All Levels.")] All =
            Emergency | Critical | Error | Warning | SystemNotification | Notification | Information | Debugging,

        /// <summary>
        ///   No Levels.
        /// </summary>
        [Description("No Levels.")] None = 0,

        /// <summary>
        ///   At least critical.
        /// </summary>
        [Description("At least critical.")] [UsedImplicitly] AtLeastCritical = Emergency | Critical,

        /// <summary>
        ///   At least an error.
        /// </summary>
        [Description("At least an error.")] [UsedImplicitly] AtLeastError = Emergency | Critical | Error,

        /// <summary>
        ///   At least a warning.
        /// </summary>
        [Description("At least a warning.")] [UsedImplicitly] AtLeastWarning = Emergency | Critical | Error | Warning,

        /// <summary>
        ///   At least a system notification.
        /// </summary>
        [Description("At least a system notification.")] [UsedImplicitly] AtLeastSystemNotification =
            Emergency | Critical | Error | Warning | SystemNotification,

        /// <summary>
        ///   At least a notification.
        /// </summary>
        [Description("At least a notification.")] [UsedImplicitly] AtLeastNotification =
            Emergency | Critical | Error | Warning | SystemNotification | Notification,

        /// <summary>
        ///   At least information.
        /// </summary>
        [Description("At least information.")] AtLeastInformation =
            Emergency | Critical | Error | Warning | SystemNotification | Notification | Information,

        /// <summary>
        ///   Used for critical, unrecoverable errors that can cause damage. The system should be stopped immediately.
        /// </summary>
        [Description("Used for critical, unrecoverable errors that can cause damage.")] Emergency = 128,

        /// <summary>
        ///   Used for critical, unrecoverable errors that don't cause damage.
        ///   You should stop the system and repair the error, but it may be capable of continuing in some capacity.
        /// </summary>
        [Description("Used for critical, unrecoverable errors that don't cause damage.")] Critical = 64,

        /// <summary>
        ///   Used for errors.
        /// </summary>
        [Description("Used for errors.")] Error = 32,

        /// <summary>
        ///   Used to indicate potential problems that should be addressed.
        /// </summary>
        [Description("Used to indicate potential problems that should be addressed.")] Warning = 16,

        /// <summary>
        ///   Used by system to notify key events.
        /// </summary>
        [Description("Used by system to notify key events.")] SystemNotification = 8,

        /// <summary>
        ///   Used by modules to notify key events.
        /// </summary>
        [Description("Used by modules to notify key events.")] Notification = 4,

        /// <summary>
        ///   Informational use.
        /// </summary>
        [Description("Informational use.")] Information = 2,

        /// <summary>
        ///   Debugging information.
        /// </summary>
        [Description("Debugging information.")] Debugging = 1
    }
}