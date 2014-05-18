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
using System.Runtime.InteropServices;
using System.Threading;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Service
{
    /// <summary>
    /// Handles manipulation of services.
    /// </summary>
    /// <remarks>
    /// See http://stackoverflow.com/questions/358700/how-to-install-a-windows-service-programmatically-in-c
    /// </remarks>
    internal static class ServiceUtils
    {
        #region PInvoke
        private const int STANDARD_RIGHTS_REQUIRED = 0xF0000;
        private const int SERVICE_WIN32_OWN_PROCESS = 0x00000010;
        private const int ERROR_SERVICE_DOES_NOT_EXIST = 0x424;
        private const int SC_STATUS_PROCESS_INFO = 0;
        private const int SERVICE_CONTROL_STATUS_REASON_INFO = 1;
        private const int ERROR_INSUFFICIENT_BUFFER = 122;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private class SERVICE_STATUS_PROCESS
        {
            public int ServiceType;
            public ServiceState CurrentState;
            public int ControlsAccepted;
            public int Win32ExitCode;
            public int ServiceSpecificExitCode;
            public int CheckPoint;
            public int WaitHint;
            public int ProcessID;
            public int ServiceFlags;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct SERVICE_DESCRIPTION
        {
            public IntPtr description;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct SERVICE_CONTROL_STATUS_REASON_PARAMS
        {
            public ServiceControlReason Reason;
            public string Comment;
            public SERVICE_STATUS_PROCESS Status;
        }

        #region OpenSCManager
        [DllImport("advapi32.dll", EntryPoint = "OpenSCManagerW", ExactSpelling = true, CharSet = CharSet.Unicode,
            SetLastError = true)]
        private static extern IntPtr OpenSCManager(
            string machineName,
            string databaseName,
            ScmAccessRights dwDesiredAccess);
        #endregion

        #region OpenService
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr OpenService(
            IntPtr hSCManager,
            string lpServiceName,
            ServiceAccessRights dwDesiredAccess);
        #endregion

        #region CreateService
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr CreateService(
            IntPtr hSCManager,
            string lpServiceName,
            string lpDisplayName,
            ServiceAccessRights dwDesiredAccess,
            int dwServiceType,
            ServiceBootFlag dwStartType,
            ServiceError dwErrorControl,
            string lpBinaryPathName,
            string lpLoadOrderGroup,
            IntPtr lpdwTagId,
            string lpDependencies,
            string lp,
            string lpPassword);
        #endregion

        #region CloseServiceHandle
        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseServiceHandle(IntPtr hSCObject);
        #endregion

        #region QueryServiceStatusEx
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool QueryServiceStatusEx(IntPtr hService, int infoLevel, IntPtr lpBuffer, int cbBufSize, out int pcbBytesNeeded);

        #endregion

        #region DeleteService
        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteService(IntPtr hService);
        #endregion

        #region ControlServiceEx
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern int ControlServiceEx(
            IntPtr hService,
            ServiceControl dwControl,
            int dwInfoLevel,
            ref SERVICE_CONTROL_STATUS_REASON_PARAMS pControlParams);
        #endregion

        #region StartService
        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool StartService(IntPtr hService, int dwNumServiceArgs, int lpServiceArgVectors);
        #endregion

        #region ChangeServiceConfig2
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ChangeServiceConfig2(
            IntPtr hService,
            ServiceConfig dwInfoLevel,
            ref SERVICE_DESCRIPTION lpInfo);
        #endregion
        #endregion

        public static void Uninstall(string serviceName)
        {
            IntPtr scm = OpenSCManager(ScmAccessRights.AllAccess);
            try
            {
                IntPtr service = OpenService(scm, serviceName, ServiceAccessRights.AllAccess);
                if (service == IntPtr.Zero)
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                try
                {
                    SERVICE_STATUS_PROCESS status = GetServiceStatus(service);
                    bool changedStatus;
                    switch (status.CurrentState)
                    {
                        case ServiceState.Unknown:
                        case ServiceState.NotFound:
                            throw new ApplicationException("Service state unknown.");
                            break;
                        case ServiceState.Stopped:
                            break;
                        case ServiceState.StopPending:
                            changedStatus = WaitForServiceStatus(service, ServiceState.StopPending, ServiceState.Stopped);
                            if (!changedStatus)
                                throw new ApplicationException("Unable to stop service");
                            break;
                        case ServiceState.StartPending:
                        case ServiceState.Running:
                        case ServiceState.ContinuePending:
                        case ServiceState.PausePending:
                        case ServiceState.Paused:
                            SERVICE_CONTROL_STATUS_REASON_PARAMS reason = new SERVICE_CONTROL_STATUS_REASON_PARAMS
                            {
                                Reason = ServiceControlReason.Planned |
                                         ServiceControlReason.Application |
                                         ServiceControlReason.Uninstall,
                                Comment = "User initiated using service utilities."
                            };
                            if (ControlServiceEx(service, ServiceControl.Stop, SERVICE_CONTROL_STATUS_REASON_INFO, ref reason) == 0)
                                throw new Win32Exception(Marshal.GetLastWin32Error());

                            changedStatus = WaitForServiceStatus(service, ServiceState.StopPending, ServiceState.Stopped);
                            if (!changedStatus)
                                throw new ApplicationException("Unable to stop service");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    if (!DeleteService(service))
                        throw new ApplicationException("Could not delete service " + Marshal.GetLastWin32Error());
                }
                finally
                {
                    CloseServiceHandle(service);
                }
            }
            finally
            {
                CloseServiceHandle(scm);
            }
        }

        public static bool ServiceIsInstalled(string serviceName)
        {
            IntPtr scm = OpenSCManager(ScmAccessRights.Connect);
            try
            {
                IntPtr service = OpenService(scm, serviceName, ServiceAccessRights.QueryStatus);
                if (service == IntPtr.Zero)
                {
                    int lastError = Marshal.GetLastWin32Error();
                    if (lastError != ERROR_SERVICE_DOES_NOT_EXIST)
                        throw new Win32Exception();
                    return false;
                }

                CloseServiceHandle(service);
                return true;
            }
            finally
            {
                CloseServiceHandle(scm);
            }
        }

        public static void Install(string serviceName, string displayName, string description, string fileName, string userName = null, string password = null)
        {
            IntPtr scm = OpenSCManager(ScmAccessRights.AllAccess);
            try
            {
                IntPtr service = CreateService(
                        scm,
                        serviceName,
                        displayName,
                        ServiceAccessRights.AllAccess,
                        SERVICE_WIN32_OWN_PROCESS,
                        ServiceBootFlag.AutoStart,
                        ServiceError.Normal,
                        fileName,
                        null,
                        IntPtr.Zero,
                        null,
                        userName,
                        password);

                if (service == IntPtr.Zero)
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                // Set description
                SERVICE_DESCRIPTION sd = new SERVICE_DESCRIPTION
                {
                    description = Marshal.StringToHGlobalUni(description)
                };
                try
                {
                    bool flag = ChangeServiceConfig2(service, ServiceConfig.Description, ref sd);
                    if (!flag)
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                }
                finally
                {
                    Marshal.FreeHGlobal(sd.description);
                }

            }
            finally
            {
                CloseServiceHandle(scm);
            }
        }

        public static void StartService(string serviceName)
        {
            IntPtr scm = OpenSCManager(ScmAccessRights.AllAccess);
            try
            {
                IntPtr service = OpenService(
                    scm,
                    serviceName,
                    ServiceAccessRights.QueryStatus | ServiceAccessRights.Start);
                if (service == IntPtr.Zero)
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                try
                {
                    if (!StartService(service, 0, 0))
                        throw new Win32Exception(Marshal.GetLastWin32Error());

                    bool changedStatus = WaitForServiceStatus(service, ServiceState.StartPending, ServiceState.Running);
                    if (!changedStatus)
                        throw new ApplicationException("Unable to start service");
                }
                finally
                {
                    CloseServiceHandle(service);
                }
            }
            finally
            {
                CloseServiceHandle(scm);
            }
        }

        public static void PauseService(string serviceName)
        {
            IntPtr scm = OpenSCManager(ScmAccessRights.Connect);
            try
            {
                IntPtr service = OpenService(
                    scm,
                    serviceName,
                    ServiceAccessRights.QueryStatus | ServiceAccessRights.PauseContinue);
                if (service == IntPtr.Zero)
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                try
                {
                    SERVICE_CONTROL_STATUS_REASON_PARAMS reason = new SERVICE_CONTROL_STATUS_REASON_PARAMS();
                    if (ControlServiceEx(service, ServiceControl.Pause, SERVICE_CONTROL_STATUS_REASON_INFO, ref reason) == 0)
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                    bool changedStatus = WaitForServiceStatus(service, ServiceState.PausePending, ServiceState.Paused);
                    if (!changedStatus)
                        throw new ApplicationException("Unable to pause service");
                }
                finally
                {
                    CloseServiceHandle(service);
                }
            }
            finally
            {
                CloseServiceHandle(scm);
            }
        }

        public static void ContinueService(string serviceName)
        {
            IntPtr scm = OpenSCManager(ScmAccessRights.Connect);
            try
            {
                IntPtr service = OpenService(
                    scm,
                    serviceName,
                    ServiceAccessRights.QueryStatus | ServiceAccessRights.PauseContinue);
                if (service == IntPtr.Zero)
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                try
                {
                    SERVICE_CONTROL_STATUS_REASON_PARAMS reason = new SERVICE_CONTROL_STATUS_REASON_PARAMS();
                    if (ControlServiceEx(service, ServiceControl.Continue, SERVICE_CONTROL_STATUS_REASON_INFO, ref reason) == 0)
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                    bool changedStatus = WaitForServiceStatus(service, ServiceState.ContinuePending, ServiceState.Running);
                    if (!changedStatus)
                        throw new ApplicationException("Unable to pause service");
                }
                finally
                {
                    CloseServiceHandle(service);
                }
            }
            finally
            {
                CloseServiceHandle(scm);
            }
        }

        public static void StopService(string serviceName)
        {
            IntPtr scm = OpenSCManager(ScmAccessRights.Connect);
            try
            {
                IntPtr service = OpenService(
                    scm,
                    serviceName,
                    ServiceAccessRights.QueryStatus | ServiceAccessRights.Stop);
                if (service == IntPtr.Zero)
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                try
                {
                    SERVICE_CONTROL_STATUS_REASON_PARAMS reason = new SERVICE_CONTROL_STATUS_REASON_PARAMS
                    {
                        Reason = ServiceControlReason.Planned |
                                 ServiceControlReason.Application |
                                 ServiceControlReason.Maintenance,
                        Comment = "User initiated using service utilities."
                    };
                    if (ControlServiceEx(service, ServiceControl.Stop, SERVICE_CONTROL_STATUS_REASON_INFO, ref reason) == 0)
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                    bool changedStatus = WaitForServiceStatus(service, ServiceState.StopPending, ServiceState.Stopped);
                    if (!changedStatus)
                        throw new ApplicationException("Unable to stop service");
                }
                finally
                {
                    CloseServiceHandle(service);
                }
            }
            finally
            {
                CloseServiceHandle(scm);
            }
        }

        public static void CommandService(string serviceName, int command)
        {
            IntPtr scm = OpenSCManager(ScmAccessRights.Connect);
            try
            {
                IntPtr service = OpenService(
                    scm,
                    serviceName,
                    ServiceAccessRights.QueryStatus | ServiceAccessRights.Stop);
                if (service == IntPtr.Zero)
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                try
                {
                    SERVICE_CONTROL_STATUS_REASON_PARAMS reason = new SERVICE_CONTROL_STATUS_REASON_PARAMS();
                    if (ControlServiceEx(service, (ServiceControl)command, SERVICE_CONTROL_STATUS_REASON_INFO, ref reason) == 0)
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                }
                finally
                {
                    CloseServiceHandle(service);
                }
            }
            finally
            {
                CloseServiceHandle(scm);
            }
        }

        public static ServiceState GetServiceStatus(string serviceName)
        {
            IntPtr scm = OpenSCManager(ScmAccessRights.Connect);
            try
            {
                IntPtr service = OpenService(scm, serviceName, ServiceAccessRights.QueryStatus);
                if (service == IntPtr.Zero)
                {
                    int lastError = Marshal.GetLastWin32Error();
                    if (lastError != ERROR_SERVICE_DOES_NOT_EXIST)
                        throw new Win32Exception();
                    return ServiceState.NotFound;
                }

                try
                {
                    return GetServiceStatus(service).CurrentState;
                }
                finally
                {
                    CloseServiceHandle(service);
                }
            }
            finally
            {
                CloseServiceHandle(scm);
            }
        }

        [NotNull]
        private static SERVICE_STATUS_PROCESS GetServiceStatus(IntPtr service)
        {
            IntPtr buf = IntPtr.Zero;
            try
            {
                int size = 0;
                // Request buffer size
                if (!QueryServiceStatusEx(service, 0, buf, size, out size))
                {
                    int error = Marshal.GetLastWin32Error();
                    if (error != ERROR_INSUFFICIENT_BUFFER)
                        throw new Win32Exception(error);
                } else
                    throw new ApplicationException("Could not get query info buffer size.");

                buf = Marshal.AllocHGlobal(size);
                if (!QueryServiceStatusEx(service, 0, buf, size, out size))
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                return (SERVICE_STATUS_PROCESS)Marshal.PtrToStructure(buf, typeof(SERVICE_STATUS_PROCESS));
            }
            finally
            {
                if (!buf.Equals(IntPtr.Zero))
                    Marshal.FreeHGlobal(buf);
            }
        }

        private static bool WaitForServiceStatus(IntPtr service, ServiceState waitStatus, ServiceState desiredStatus)
        {
            SERVICE_STATUS_PROCESS status = GetServiceStatus(service);
            if (status.CurrentState == desiredStatus) return true;

            int dwStartTickCount = Environment.TickCount;
            int dwOldCheckPoint = status.CheckPoint;

            while (status.CurrentState == waitStatus)
            {
                // Do not wait longer than the wait hint. A good interval is
                // one tenth the wait hint, but no less than 1 second and no
                // more than 10 seconds.

                int dwWaitTime = status.WaitHint / 10;

                if (dwWaitTime < 1000) dwWaitTime = 1000;
                else if (dwWaitTime > 10000) dwWaitTime = 10000;

                Thread.Sleep(dwWaitTime);

                // Check the status again.
                status = GetServiceStatus(service);

                if (status.CheckPoint > dwOldCheckPoint)
                {
                    // The service is making progress.
                    dwStartTickCount = Environment.TickCount;
                    dwOldCheckPoint = status.CheckPoint;
                }
                else if (Environment.TickCount - dwStartTickCount > status.WaitHint)
                    // No progress made within the wait hint
                    break;
            }
            return (status.CurrentState == desiredStatus);
        }

        private static IntPtr OpenSCManager(ScmAccessRights rights)
        {
            IntPtr scm = OpenSCManager(null, null, rights);
            if (scm == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            return scm;
        }
    }
}