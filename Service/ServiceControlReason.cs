using System;

namespace WebApplications.Utilities.Service
{
    /// <summary>
    /// Enum ServiceControlReason
    /// </summary>
    /// <remarks>See http://msdn.microsoft.com/en-us/library/windows/desktop/ms685154(v=vs.85).aspx</remarks>
    [Flags]
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
        Disk = 0x00000008,
        Environment = 0x0000000a,
        HardwareDriver = 0x0000000b,
        Hung = 0x00000006,
        Installation = 0x00000003,
        Maintenance = 0x00000002,
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
        #endregion
    }
}