#region © Copyright Web Applications (UK) Ltd, 2014.  All rights reserved.
// Copyright (c) 2014, Web Applications UK Ltd
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
        /// Include the resource property name.
        /// </summary>
        [Description("Include the resource property name.")]
        [PublicAPI]
        ResourceProperty = 1 << 3,

        /// <summary>
        /// Include the resource property name.
        /// </summary>
        [Description("Include the resource property name.")]
        [PublicAPI]
        R = ResourceProperty,

        /// <summary>
        /// Include the resource culture.
        /// </summary>
        [Description("Include the resource culture.")]
        [PublicAPI]
        Culture = 1 << 4,

        /// <summary>
        /// Include the resource culture.
        /// </summary>
        [Description("Include the resource culture.")]
        [PublicAPI]
        U = Culture,

        /// <summary>
        /// Include the time stamp.
        /// </summary>
        [Description("Include the time stamp.")]
        [PublicAPI]
        TimeStamp = 1 << 5,

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
        Level = 1 << 6,

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
        Guid = 1 << 7,

        /// <summary>
        /// Include the GUID.
        /// </summary>
        [Description("Include the GUID.")]
        [PublicAPI]
        I = Guid,

        /// <summary>
        /// Include the inner exceptions.
        /// </summary>
        [Description("Include the inner exceptions.")]
        [PublicAPI]
        InnerException = 1 << 8,

        /// <summary>
        /// Include the inner exceptions.
        /// </summary>
        [Description("Include the inner exceptions.")]
        [PublicAPI]
        // ReSharper disable once InconsistentNaming
        IE = InnerException,

        /// <summary>
        /// Include the thread ID.
        /// </summary>
        [Description("Include the thread ID.")]
        [PublicAPI]
        ThreadID = 1 << 9,

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
        ThreadName = 1 << 10,

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
        Context = 1 << 11,

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
        Exception = 1 << 12,

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
        SQLException = 1 << 13,

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
        StackTrace = 1 << 14,

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
        Verbose =
            Message | ResourceProperty | Culture | TimeStamp | Level | Guid | ThreadID | ThreadName | Context |
            Exception | SQLException | InnerException | StackTrace,

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
        General = Message | TimeStamp | Level | ThreadName | Exception | SQLException | InnerException,

        /// <summary>
        /// Includes essential information.
        /// </summary>
        [Description("Includes main information.")]
        [PublicAPI]
        F = General,

        /// <summary>
        /// Includes essential information.
        /// </summary>
        [Description("Includes essential information.")]
        [PublicAPI]
        Basic = Message | TimeStamp | Level | Exception | SQLException | InnerException,

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
        All = Verbose | ApplicationGuid | ApplicationName | IncludeMissing,

        /// <summary>
        /// Includes everything (even when missing).
        /// </summary>
        [Description("Includes everything (even when missing).")]
        [PublicAPI]
        A = All,
    }
}