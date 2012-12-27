using System;
using System.ComponentModel;

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
        None = 0,

        /// <summary>
        /// When set will include the application name.
        /// </summary>
        [Description("When set will include the application name.")]
        ApplicationName = 1 << 0,
        /// <summary>
        /// When set will include the application name.
        /// </summary>
        [Description("When set will include the application name.")]
        AN = ApplicationName,

        /// <summary>
        /// When set will include the application GUID.
        /// </summary>
        [Description("When set will include the application GUID.")]
        ApplicationGuid = 1 << 1,
        /// <summary>
        /// When set will include the application GUID.
        /// </summary>
        [Description("When set will include the application GUID.")]
        AG = ApplicationGuid,

        /// <summary>
        /// Include the message.
        /// </summary>
        [Description("Include the message.")]
        Message = 1 << 2,
        /// <summary>
        /// Include the message.
        /// </summary>
        [Description("Include the message.")]
        M = Message,

        /// <summary>
        /// Include the time stamp.
        /// </summary>
        [Description("Include the time stamp.")]
        TimeStamp = 1 << 3,
        /// <summary>
        /// Include the time stamp.
        /// </summary>
        [Description("Include the time stamp.")]
        D = TimeStamp,

        /// <summary>
        /// Include the level.
        /// </summary>
        [Description("Include the level.")]
        Level = 1 << 4,
        /// <summary>
        /// Include the level.
        /// </summary>
        [Description("Include the level.")]
        L = Level,

        /// <summary>
        /// Include the GUID.
        /// </summary>
        [Description("Include the GUID.")]
        Guid = 1 << 5,
        /// <summary>
        /// Include the GUID.
        /// </summary>
        [Description("Include the GUID.")]
        I = Guid,

        /// <summary>
        /// Include the Group.
        /// </summary>
        [Description("Include the Group.")]
        Group = 1 << 6,
        /// <summary>
        /// Include the Group.
        /// </summary>
        [Description("Include the Group.")]
        G = Group,

        /// <summary>
        /// Include the thread ID.
        /// </summary>
        [Description("Include the thread ID.")]
        ThreadID = 1 << 7,
        /// <summary>
        /// Include the thread ID.
        /// </summary>
        [Description("Include the thread ID.")]
        TD = ThreadID,

        /// <summary>
        /// Include the thread Name.
        /// </summary>
        [Description("Include the thread name.")]
        ThreadName = 1 << 8,
        /// <summary>
        /// Include the thread Name.
        /// </summary>
        [Description("Include the thread name.")]
        TN = ThreadName,

        /// <summary>
        /// Incldude additional context.
        /// </summary>
        [Description("Include additional context.")]
        Context = 1 << 9,
        /// <summary>
        /// Incldude additional context.
        /// </summary>
        [Description("Include additional context.")]
        C = Context,

        /// <summary>
        /// Include the exception type.
        /// </summary>
        [Description("Include the exception type.")]
        Exception = 1 << 10,
        /// <summary>
        /// Include the exception type.
        /// </summary>
        [Description("Include the exception type.")]
        E = Exception,

        /// <summary>
        /// Include the SQL Exception info.
        /// </summary>
        [Description("Include the SQL Exception info.")]
        SQLException = 1 << 11,
        /// <summary>
        /// Include the SQL Exception info.
        /// </summary>
        [Description("Include the SQL Exception info.")]
        S = SQLException,

        /// <summary>
        /// Include the stack trace.
        /// </summary>
        [Description("Include the stack trace.")]
        StackTrace = 1 << 12,
        /// <summary>
        /// Include the stack trace.
        /// </summary>
        [Description("Include the stack trace.")]
        T = StackTrace,

        /// <summary>
        /// When set will format as JSON.
        /// </summary>
        /// <remarks><para>Setting this and <see cref="Xml"/> at this same time 
        /// will result in a <see cref="FormatException"/>.</para></remarks>
        [Description("When set will format as JSON.")]
        Json = 1 << 28,
        /// <summary>
        /// When set will format as JSON.
        /// </summary>
        /// <remarks><para>Setting this and <see cref="Xml"/> at this same time 
        /// will result in a <see cref="FormatException"/>.</para></remarks>
        [Description("When set will format as JSON.")]
        J = Json,

        /// <summary>
        /// When set will format as XML.
        /// </summary>
        /// <remarks><para>Setting this and <see cref="Json"/> at this same time 
        /// will result in a <see cref="FormatException"/>.</para></remarks>
        [Description("When set will format as XML.")]
        Xml = 1 << 29,
        /// <summary>
        /// When set will format as XML.
        /// </summary>
        /// <remarks><para>Setting this and <see cref="Json"/> at this same time 
        /// will result in a <see cref="FormatException"/>.</para></remarks>
        [Description("When set will format as XML.")]
        X = Xml,

        /// <summary>
        /// Include a header and footer.
        /// </summary>
        [Description("Include a header and footer")]
        Header = 1 << 30,
        /// <summary>
        /// Include a header and footer.
        /// </summary>
        [Description("Include a header and footer")]
        H = Header,

        /// <summary>
        /// When set will include elements even if missing.
        /// </summary>
        [Description("When set will include elements even if missing.")]
        IncludeMissing = 1 << 31,
        /// <summary>
        /// When set will include elements even if missing.
        /// </summary>
        [Description("When set will include elements even if missing.")]
        Z = IncludeMissing,

        /// <summary>
        /// Includes everything (except missing elements).
        /// </summary>
        [Description("Includes everything (except missing elements).")]
        Verbose = Message | TimeStamp | Level | Guid | Group | ThreadID | ThreadName | Context | Exception | SQLException | StackTrace | Header,
        /// <summary>
        /// Includes everything (except missing elements).
        /// </summary>
        [Description("Includes everything (except missing elements).")]
        V = Verbose,

        /// <summary>
        /// Includes essential information.
        /// </summary>
        [Description("Includes main information.")]
        General = Message | TimeStamp | Level | Group | ThreadName | Exception | SQLException | Header,
        /// <summary>
        /// Includes essential information.
        /// </summary>
        [Description("Includes main information.")]
        R = General,

        /// <summary>
        /// Includes essential information.
        /// </summary>
        [Description("Includes essential information.")]
        Basic = Message | TimeStamp | Level | Exception | SQLException,
        /// <summary>
        /// Includes essential information.
        /// </summary>
        [Description("Includes essential information.")]
        B = Basic,

        /// <summary>
        /// Includes everything (even when missing).
        /// </summary>
        [Description("Includes everything (even when missing).")]
        All = Verbose | ApplicationGuid | ApplicationName | IncludeMissing
    }
}