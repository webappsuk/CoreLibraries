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
using JetBrains.Annotations;

namespace WebApplications.Utilities.Service
{
    /// <summary>
    /// Enum ServiceControlReason
    /// </summary>
    /// <remarks>See http://msdn.microsoft.com/en-us/library/windows/desktop/ms685154(v=vs.85).aspx</remarks>
    [Flags]
    [PublicAPI]
    public enum ServiceControlReason
    {
        #region General reason codes
        /// <summary>
        /// The service stop was not planned.
        /// </summary>
        Unplanned = 0x10000000,

        /// <summary>
        /// The reason code is defined by the user. If this flag is not present, the reason code is defined by the system. 
        /// If this flag is specified with a system reason code, the function call fails.
        /// </summary>
        Custom = 0x20000000,

        /// <summary>
        /// The service stop was planned.
        /// </summary>
        Planned = 0x40000000,
        #endregion

        #region Major reason codes
        /// <summary>
        /// Application issue.
        /// </summary>
        Application = 0x00050000,

        /// <summary>
        /// Hardware issue.
        /// </summary>
        Hardware = 0x00020000,

        /// <summary>
        /// No major reason.
        /// </summary>
        None = 0x00060000,

        /// <summary>
        /// Operating system issue.
        /// </summary>
        OperatingSystem = 0x00030000,

        /// <summary>
        /// Other issue.
        /// </summary>
        Other = 0x00010000,

        /// <summary>
        /// Software issue.
        /// </summary>
        Software = 0x00010000,
        #endregion

        #region Minor reasons
#pragma warning disable 1591
        // ReSharper disable UnusedMember.Global
        Disk = 0x00000008,
        Environment = 0x0000000a,
        HardwareDriver = 0x0000000b,
        Hung = 0x00000006,
        Installation = 0x00000003,
        Maintenance = 0x00000002,
        // ReSharper disable once InconsistentNaming
        MMC = 0x00000016,
        NetworkConnectivity = 0x00000011,
        NetworCard = 0x00000009,
        MinorOther = 0x00000001,
        OtherDriver = 0x0000000c,
        ReConfigure = 0x00000005,
        Security = 0x00000005,
        SecurityFix = 0x0000000f,
        SecurityFixUninstall = 0x00000015,
        ServicePack = 0x0000000d,
        Uninstall = 0x00000013,
        Update = 0x0000000e,
        UpdateUninstall = Update,
        Unstable = 0x0000000e,
        Upgrade = 0x00000004,
        WMI = 0x00000012
        // ReSharper restore UnusedMember.Global
#pragma warning restore 1591
        #endregion
    }
}