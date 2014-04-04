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

using System.ComponentModel;
using System.Runtime.Serialization;
using ProtoBuf;

namespace WebApplications.Utilities.Logging
{
    /// <summary>
    ///   The severity of a Log Entry.
    /// </summary>
    [ProtoContract]
    public enum LoggingLevel : byte
    {
        /// <summary>
        ///   Used for critical, unrecoverable errors that can cause damage. The system should be stopped immediately.
        /// </summary>
        [Description("Used for critical, unrecoverable errors that can cause damage.")]
        [EnumMember]
        [ProtoEnum(Name = "Emergency", Value = 1 << 7)]
        Emergency = 1 << 7,

        /// <summary>
        ///   Used for critical, unrecoverable errors that don't cause damage.
        ///   You should stop the system and repair the error, but it may be capable of continuing in some capacity.
        /// </summary>
        [Description("Used for critical, unrecoverable errors that don't cause damage.")]
        [EnumMember]
        [ProtoEnum(Name = "Critical", Value = 1 << 6)]
        Critical = 1 << 6,

        /// <summary>
        ///   Used for errors.
        /// </summary>
        [Description("Used for errors.")]
        [EnumMember]
        [ProtoEnum(Name = "Error", Value = 1 << 5)]
        Error = 1 << 5,

        /// <summary>
        ///   Used to indicate potential problems that should be addressed.
        /// </summary>
        [Description("Used to indicate potential problems that should be addressed.")]
        [EnumMember]
        [ProtoEnum(Name = "Warning", Value = 1 << 4)]
        Warning = 1 << 4,

        /// <summary>
        ///   Used by system to notify key events.
        /// </summary>
        [Description("Used by system to notify key events.")]
        [EnumMember]
        [ProtoEnum(Name = "SystemNotification", Value = 1 << 3)]
        SystemNotification = 1 << 3,

        /// <summary>
        ///   Used by modules to notify key events.
        /// </summary>
        [Description("Used by modules to notify key events.")]
        [EnumMember]
        [ProtoEnum(Name = "Notification", Value = 1 << 2)]
        Notification = 1 << 2,

        /// <summary>
        ///   Informational use.
        /// </summary>
        [Description("Informational use.")]
        [EnumMember]
        [ProtoEnum(Name = "Information", Value = 1 << 1)]
        Information = 1 << 1,

        /// <summary>
        ///   Debugging information.
        /// </summary>
        [Description("Debugging information.")]
        [EnumMember]
        [ProtoEnum(Name = "Debugging", Value = 1 << 0)]
        Debugging = 1 << 0
    }
}