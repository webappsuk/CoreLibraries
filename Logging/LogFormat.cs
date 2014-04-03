using System;
using System.ComponentModel;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Logging
{
    /// <summary>
    /// Log formatting flags
    /// </summary>
    [Flags]
    public enum LogFormat
    {
        /// <summary>
        /// Nothing displayed.
        /// </summary>
        [Description("Nothing displayed.")]
        [PublicAPI]
        None = 0,

        /// <summary>
        /// When set will include the application name.
        /// </summary>
        [Description("When set will include the application name.")]
        [PublicAPI]
        ApplicationName = 1 << 0,
        /// <summary>
        /// When set will include the application name.
        /// </summary>
        [Description("When set will include the application name.")]
        [PublicAPI]
        // ReSharper disable once InconsistentNaming
        AN = ApplicationName,

        /// <summary>
        /// When set will include the application GUID.
        /// </summary>
        [Description("When set will include the application GUID.")]
        [PublicAPI]
        ApplicationGuid = 1 << 1,
        /// <summary>
        /// When set will include the application GUID.
        /// </summary>
        [Description("When set will include the application GUID.")]
        [PublicAPI]
        // ReSharper disable once InconsistentNaming
        AG = ApplicationGuid,

        /// <summary>
        /// Include the message.
        /// </summary>
        [Description("Include the message.")]
        [PublicAPI]
        Message = 1 << 2,
        /// <summary>
        /// Include the message.
        /// </summary>
        [Description("Include the message.")]
        [PublicAPI]
        M = Message,

        /// <summary>
        /// Include the time stamp.
        /// </summary>
        [Description("Include the time stamp.")]
        [PublicAPI]
        TimeStamp = 1 << 3,
        /// <summary>
        /// Include the time stamp.
        /// </summary>
        [Description("Include the time stamp.")]
        [PublicAPI]
        D = TimeStamp,

        /// <summary>
        /// Include the level.
        /// </summary>
        [Description("Include the level.")]
        [PublicAPI]
        Level = 1 << 4,
        /// <summary>
        /// Include the level.
        /// </summary>
        [Description("Include the level.")]
        [PublicAPI]
        L = Level,

        /// <summary>
        /// Include the GUID.
        /// </summary>
        [Description("Include the GUID.")]
        [PublicAPI]
        Guid = 1 << 5,
        /// <summary>
        /// Include the GUID.
        /// </summary>
        [Description("Include the GUID.")]
        [PublicAPI]
        I = Guid,
        
        /*
         * 1 << 6 Used to be used by Group
         */

        /// <summary>
        /// Include the thread ID.
        /// </summary>
        [Description("Include the thread ID.")]
        [PublicAPI]
        ThreadID = 1 << 7,
        /// <summary>
        /// Include the thread ID.
        /// </summary>
        [Description("Include the thread ID.")]
        [PublicAPI]
        // ReSharper disable once InconsistentNaming
        TD = ThreadID,

        /// <summary>
        /// Include the thread Name.
        /// </summary>
        [Description("Include the thread name.")]
        [PublicAPI]
        ThreadName = 1 << 8,
        /// <summary>
        /// Include the thread Name.
        /// </summary>
        [Description("Include the thread name.")]
        [PublicAPI]
        // ReSharper disable once InconsistentNaming
        TN = ThreadName,

        /// <summary>
        /// Include additional context.
        /// </summary>
        [Description("Include additional context.")]
        [PublicAPI]
        Context = 1 << 9,
        /// <summary>
        /// Include additional context.
        /// </summary>
        [Description("Include additional context.")]
        [PublicAPI]
        C = Context,

        /// <summary>
        /// Include the exception type.
        /// </summary>
        [Description("Include the exception type.")]
        [PublicAPI]
        Exception = 1 << 10,
        /// <summary>
        /// Include the exception type.
        /// </summary>
        [Description("Include the exception type.")]
        [PublicAPI]
        E = Exception,

        /// <summary>
        /// Include the SQL Exception info.
        /// </summary>
        [Description("Include the SQL Exception info.")]
        [PublicAPI]
        // ReSharper disable once InconsistentNaming
        SQLException = 1 << 11,
        /// <summary>
        /// Include the SQL Exception info.
        /// </summary>
        [Description("Include the SQL Exception info.")]
        [PublicAPI]
        S = SQLException,

        /// <summary>
        /// Include the stack trace.
        /// </summary>
        [Description("Include the stack trace.")]
        [PublicAPI]
        StackTrace = 1 << 12,
        /// <summary>
        /// Include the stack trace.
        /// </summary>
        [Description("Include the stack trace.")]
        [PublicAPI]
        T = StackTrace,

        /// <summary>
        /// When set will format as JSON.
        /// </summary>
        /// <remarks><para>Setting this and <see cref="Xml"/> at this same time 
        /// will result in a <see cref="FormatException"/>.</para></remarks>
        [Description("When set will format as JSON.")]
        [PublicAPI]
        Json = 1 << 28 | Header,
        /// <summary>
        /// When set will format as JSON.
        /// </summary>
        /// <remarks><para>Setting this and <see cref="Xml"/> at this same time 
        /// will result in a <see cref="FormatException"/>.</para></remarks>
        [Description("When set will format as JSON.")]
        [PublicAPI]
        J = Json,

        /// <summary>
        /// When set will format as XML.
        /// </summary>
        /// <remarks><para>Setting this and <see cref="Json"/> at this same time 
        /// will result in a <see cref="FormatException"/>.</para></remarks>
        [Description("When set will format as XML.")]
        [PublicAPI]
        Xml = 1 << 29 | Header,
        /// <summary>
        /// When set will format as XML.
        /// </summary>
        /// <remarks><para>Setting this and <see cref="Json"/> at this same time 
        /// will result in a <see cref="FormatException"/>.</para></remarks>
        [Description("When set will format as XML.")]
        [PublicAPI]
        X = Xml,

        /// <summary>
        /// Include a header and footer.
        /// </summary>
        [Description("Include a header and footer")]
        [PublicAPI]
        Header = 1 << 30,
        /// <summary>
        /// Include a header and footer.
        /// </summary>
        [Description("Include a header and footer")]
        [PublicAPI]
        H = Header,

        /// <summary>
        /// When set will include elements even if missing.
        /// </summary>
        [Description("When set will include elements even if missing.")]
        [PublicAPI]
        IncludeMissing = 1 << 31,
        /// <summary>
        /// When set will include elements even if missing.
        /// </summary>
        [Description("When set will include elements even if missing.")]
        [PublicAPI]
        Z = IncludeMissing,

        /// <summary>
        /// Includes everything (except missing elements).
        /// </summary>
        [Description("Includes everything (except missing elements).")]
        [PublicAPI]
        Verbose = Message | TimeStamp | Level | Guid | ThreadID | ThreadName | Context | Exception | SQLException | StackTrace,
        /// <summary>
        /// Includes everything (except missing elements).
        /// </summary>
        [Description("Includes everything (except missing elements).")]
        [PublicAPI]
        V = Verbose,

        /// <summary>
        /// Includes essential information.
        /// </summary>
        [Description("Includes main information.")]
        [PublicAPI]
        General = Message | TimeStamp | Level | ThreadName | Exception | SQLException,
        /// <summary>
        /// Includes essential information.
        /// </summary>
        [Description("Includes main information.")]
        [PublicAPI]
        R = General,

        /// <summary>
        /// Includes essential information.
        /// </summary>
        [Description("Includes essential information.")]
        [PublicAPI]
        Basic = Message | TimeStamp | Level | Exception | SQLException,
        /// <summary>
        /// Includes essential information.
        /// </summary>
        [Description("Includes essential information.")]
        [PublicAPI]
        B = Basic,

        /// <summary>
        /// Includes everything (even when missing).
        /// </summary>
        [Description("Includes everything (even when missing).")]
        [PublicAPI]
        All = Verbose | ApplicationGuid | ApplicationName | IncludeMissing
    }
}