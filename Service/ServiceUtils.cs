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
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

// TODO Move to common code so it can be used from the client.

namespace WebApplications.Utilities.Service
{
    /// <summary>
    /// Handles manipulation of services.
    /// </summary>
    /// <remarks>See http://stackoverflow.com/questions/358700/how-to-install-a-windows-service-programmatically-in-c</remarks>
    internal static class ServiceUtils
    {
        #region PInvoke
        // ReSharper disable InconsistentNaming, StringLiteralTypo, IdentifierTypo
        private const int SERVICE_WIN32_OWN_PROCESS = 0x00000010;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct SERVICE_DESCRIPTION
        {
            public IntPtr description;
        }

        [DllImport("advapi32.dll", EntryPoint = "OpenSCManagerW", ExactSpelling = true, CharSet = CharSet.Unicode,
            SetLastError = true)]
        private static extern IntPtr OpenSCManager(
            string machineName,
            string databaseName,
            ScmAccessRights dwDesiredAccess);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr OpenService(
            IntPtr hSCManager,
            string lpServiceName,
            ServiceAccessRights dwDesiredAccess);

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

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseServiceHandle(IntPtr hSCObject);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteService(IntPtr hService);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ChangeServiceConfig2(
            IntPtr hService,
            ServiceConfig dwInfoLevel,
            ref SERVICE_DESCRIPTION lpInfo);

        // ReSharper restore InconsistentNaming, StringLiteralTypo, IdentifierTypo
        #endregion

        /// <summary>
        /// Whether the specified service is installed.
        /// </summary>
        /// <param name="serviceName">Name of the service.</param>
        /// <returns><see langword="true" /> if installed, <see langword="false" /> otherwise.</returns>
        public static bool ServiceIsInstalled([NotNull] string serviceName)
        {
            Contract.Requires<RequiredContractException>(serviceName != null, "Parameter_Null");
            return
                ServiceController.GetServices()
                    // ReSharper disable once PossibleNullReferenceException
                    .Any(sc => string.Equals(sc.ServiceName, serviceName, StringComparison.CurrentCulture));
        }

        /// <summary>
        /// Uninstalls the specified service name.
        /// </summary>
        /// <param name="serviceName">Name of the service.</param>
        /// <param name="token">The token.</param>
        /// <returns>An awaitable task.</returns>
        /// <exception cref="System.ComponentModel.Win32Exception"></exception>
        /// <exception cref="System.ApplicationException">Service state unknown.
        /// or
        /// Unable to stop service
        /// or
        /// Unable to stop service
        /// or
        /// Could not delete service  + Marshal.GetLastWin32Error()</exception>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        [NotNull]
        public static async Task Uninstall(
            [NotNull] string serviceName,
            CancellationToken token = default(CancellationToken))
        {
            Contract.Requires<RequiredContractException>(serviceName != null, "Parameter_Null");
            if (!ServiceIsInstalled(serviceName))
                return;

            string servicePath;
            try
            {
                // Find location of service.
                using (ManagementClass mc = new ManagementClass("Win32_Service"))
                {
                    ManagementObject mo = mc.GetInstances()
                        .Cast<ManagementObject>()
                        // ReSharper disable PossibleNullReferenceException
                        .FirstOrDefault(o => string.Equals(o.GetPropertyValue("Name").ToString(), serviceName));
                    // ReSharper restore PossibleNullReferenceException
                    if (mo == null)
                        throw new ServiceException(
                            () => ServiceResources.Err_ServiceUtils_Uninstall_CouldNotFindLocation,
                            serviceName);
                    // ReSharper disable once PossibleNullReferenceException
                    servicePath = mo.GetPropertyValue("PathName").ToString().Trim('"');
                    if (!File.Exists(servicePath))
                        throw new ServiceException(
                            () => ServiceResources.Err_ServiceUtils_Uninstall_ServiceLocationDoesntExist,
                            serviceName,
                            servicePath);
                }
            }
            catch (ServiceException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new ServiceException(
                    e,
                    () => ServiceResources.Err_ServiceUtils_Uninstall_CouldNotFindLocation,
                    serviceName);
            }

            await StopService(serviceName, token).ConfigureAwait(false);

            IntPtr scm = OpenSCManager(ScmAccessRights.AllAccess);
            try
            {
                IntPtr service = OpenService(scm, serviceName, ServiceAccessRights.AllAccess);
                if (service == IntPtr.Zero)
                    throw new ServiceException(
                        new Win32Exception(),
                        () => ServiceResources.Err_ServiceUtils_Uninstall_CouldNotOpenService,
                        serviceName);

                try
                {
                    if (!DeleteService(service))
                        throw new ServiceException(
                            new Win32Exception(),
                            () => ServiceResources.Err_ServiceUtils_Uninstall_CouldNotUninstallService,
                            serviceName);
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

            // Try to remove service directory
            // ReSharper disable once AssignNullToNotNullAttribute
            Directory.Delete(Path.GetDirectoryName(servicePath), true);
        }

        /// <summary>
        /// Installs the specified service name.
        /// </summary>
        /// <param name="serviceName">Name of the service.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="description">The description.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <exception cref="System.ComponentModel.Win32Exception">
        /// </exception>
        public static void Install(
            [NotNull] string serviceName,
            [NotNull] string displayName,
            [NotNull] string description,
            [NotNull] string fileName,
            [CanBeNull] string userName = null,
            [CanBeNull] string password = null)
        {
            Contract.Requires<RequiredContractException>(serviceName != null, "Parameter_Null");
            Contract.Requires<RequiredContractException>(displayName != null, "Parameter_Null");
            Contract.Requires<RequiredContractException>(description != null, "Parameter_Null");
            Contract.Requires<RequiredContractException>(fileName != null, "Parameter_Null");
            if (ServiceIsInstalled(serviceName))
                return;

            IntPtr scm = OpenSCManager(ScmAccessRights.AllAccess);
            try
            {
                if (fileName.Contains(' '))
                    fileName = string.Format("\"{0}\"", fileName);

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
                    throw new ServiceException(
                        new Win32Exception(),
                        () => ServiceResources.Err_ServiceUtils_Install_CouldNotInstallService,
                        serviceName);
                try
                {
                    // Set description
                    SERVICE_DESCRIPTION sd = new SERVICE_DESCRIPTION
                    {
                        description = Marshal.StringToHGlobalUni(description)
                    };
                    try
                    {
                        bool flag = ChangeServiceConfig2(service, ServiceConfig.Description, ref sd);
                        if (!flag)
                            throw new ServiceException(
                                new Win32Exception(),
                                () => ServiceResources.Err_ServiceUtils_Install_CouldNotSetDescription,
                                serviceName);
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(sd.description);
                    }
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

        /// <summary>
        /// Waits for the service to enter a certain status..
        /// </summary>
        /// <param name="serviceController">The service controller.</param>
        /// <param name="status">The status.</param>
        /// <param name="token">The token.</param>
        /// <returns>An awaitable task that returns <see langword="true"/> if the service status is equal to <paramref name="status"/>.</returns>
        [NotNull]
        private static async Task<bool> WaitForAsync(
            [NotNull] ServiceController serviceController,
            ServiceControllerStatus status,
            CancellationToken token = default(CancellationToken))
        {
            Contract.Requires<RequiredContractException>(serviceController != null, "Parameter_Null");

            token = token.WithTimeout(TimeSpan.FromMinutes(1));

            serviceController.Refresh();
            while (serviceController.Status != status)
            {
                // ReSharper disable once PossibleNullReferenceException
                await Task.Delay(250, token).ConfigureAwait(false);
                if (token.IsCancellationRequested) return false;
                serviceController.Refresh();
            }
            return true;
        }

        /// <summary>
        /// Starts the service.
        /// </summary>
        /// <param name="serviceName">Name of the service.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="token">The token.</param>
        /// <returns>An awaitable task that returns <see langword="true"/> if the service is running.</returns>
        [NotNull]
        public static async Task<bool> StartService(
            [NotNull] string serviceName,
            [CanBeNull] string[] args = null,
            CancellationToken token = default(CancellationToken))
        {
            Contract.Requires<RequiredContractException>(serviceName != null, "Parameter_Null");
            if (args == null) args = new string[] {};
            try
            {
                new ServiceControllerPermission(
                    ServiceControllerPermissionAccess.Control,
                    Environment.MachineName,
                    serviceName).Assert();
                using (ServiceController serviceController = new ServiceController(serviceName, Environment.MachineName)
                    )
                    switch (serviceController.Status)
                    {
                        case ServiceControllerStatus.Running:
                            return true;
                        case ServiceControllerStatus.ContinuePending:
                        case ServiceControllerStatus.StartPending:
                            return await WaitForAsync(serviceController, ServiceControllerStatus.Running, token).ConfigureAwait(false);
                        case ServiceControllerStatus.Paused:
                            serviceController.Continue();
                            return await WaitForAsync(serviceController, ServiceControllerStatus.Running, token).ConfigureAwait(false);
                        case ServiceControllerStatus.PausePending:
                            if (!await WaitForAsync(serviceController, ServiceControllerStatus.Paused, token).ConfigureAwait(false))
                                return false;
                            serviceController.Continue();
                            return await WaitForAsync(serviceController, ServiceControllerStatus.Running, token).ConfigureAwait(false);
                        case ServiceControllerStatus.Stopped:
                            serviceController.Start(args);
                            return await WaitForAsync(serviceController, ServiceControllerStatus.Running, token).ConfigureAwait(false);
                        case ServiceControllerStatus.StopPending:
                            if (!await WaitForAsync(serviceController, ServiceControllerStatus.Stopped, token).ConfigureAwait(false))
                                return false;
                            serviceController.Start(args);
                            return await WaitForAsync(serviceController, ServiceControllerStatus.Running, token).ConfigureAwait(false);
                        default:
                            return false;
                    }
            }
            catch (TaskCanceledException)
            {
                return false;
            }
        }

        /// <summary>
        /// Pauses the service.
        /// </summary>
        /// <param name="serviceName">Name of the service.</param>
        /// <param name="token">The token.</param>
        /// <returns>An awaitable task that returns <see langword="true"/> if the service is paused.</returns>
        [NotNull]
        public static async Task<bool> PauseService(
            [NotNull] string serviceName,
            CancellationToken token = default(CancellationToken))
        {
            Contract.Requires<RequiredContractException>(serviceName != null, "Parameter_Null");
            try
            {
                new ServiceControllerPermission(
                    ServiceControllerPermissionAccess.Control,
                    Environment.MachineName,
                    serviceName).Assert();
                using (ServiceController serviceController = new ServiceController(serviceName, Environment.MachineName)
                    )
                    switch (serviceController.Status)
                    {
                        case ServiceControllerStatus.Running:
                            serviceController.Pause();
                            return await WaitForAsync(serviceController, ServiceControllerStatus.Paused, token).ConfigureAwait(false);
                        case ServiceControllerStatus.ContinuePending:
                        case ServiceControllerStatus.StartPending:
                            if (!await WaitForAsync(serviceController, ServiceControllerStatus.Running, token).ConfigureAwait(false))
                                return false;
                            serviceController.Pause();
                            return await WaitForAsync(serviceController, ServiceControllerStatus.Paused, token).ConfigureAwait(false);
                        case ServiceControllerStatus.Paused:
                            return true;
                        case ServiceControllerStatus.PausePending:
                            return await WaitForAsync(serviceController, ServiceControllerStatus.Paused, token).ConfigureAwait(false);
                        default:
                            return false;
                    }
            }
            catch (TaskCanceledException)
            {
                return false;
            }
        }

        /// <summary>
        /// Continues the service.
        /// </summary>
        /// <param name="serviceName">Name of the service.</param>
        /// <param name="token">The token.</param>
        /// <returns>An awaitable task that returns <see langword="true"/> if the service is running.</returns>
        [NotNull]
        public static async Task<bool> ContinueService(
            [NotNull] string serviceName,
            CancellationToken token = default(CancellationToken))
        {
            Contract.Requires<RequiredContractException>(serviceName != null, "Parameter_Null");
            try
            {
                new ServiceControllerPermission(
                    ServiceControllerPermissionAccess.Control,
                    Environment.MachineName,
                    serviceName).Assert();
                using (ServiceController serviceController = new ServiceController(serviceName, Environment.MachineName)
                    )
                    switch (serviceController.Status)
                    {
                        case ServiceControllerStatus.Running:
                            return true;
                        case ServiceControllerStatus.ContinuePending:
                        case ServiceControllerStatus.StartPending:
                            return await WaitForAsync(serviceController, ServiceControllerStatus.Running, token).ConfigureAwait(false);
                        case ServiceControllerStatus.Paused:
                            serviceController.Continue();
                            return await WaitForAsync(serviceController, ServiceControllerStatus.Running, token).ConfigureAwait(false);
                        case ServiceControllerStatus.PausePending:
                            if (!await WaitForAsync(serviceController, ServiceControllerStatus.Paused, token).ConfigureAwait(false))
                                return false;
                            serviceController.Continue();
                            return await WaitForAsync(serviceController, ServiceControllerStatus.Running, token).ConfigureAwait(false);
                        default:
                            return false;
                    }
            }
            catch (TaskCanceledException)
            {
                return false;
            }
        }

        /// <summary>
        /// Stops the service.
        /// </summary>
        /// <param name="serviceName">Name of the service.</param>
        /// <param name="token">The token.</param>
        /// <returns>An awaitable task that returns <see langword="true"/> if the service is stopped.</returns>
        [NotNull]
        public static async Task<bool> StopService(
            [NotNull] string serviceName,
            CancellationToken token = default(CancellationToken))
        {
            Contract.Requires<RequiredContractException>(serviceName != null, "Parameter_Null");
            try
            {
                new ServiceControllerPermission(
                    ServiceControllerPermissionAccess.Control,
                    Environment.MachineName,
                    serviceName).Assert();
                using (ServiceController serviceController = new ServiceController(serviceName, Environment.MachineName)
                    )
                    switch (serviceController.Status)
                    {
                        case ServiceControllerStatus.Running:
                            serviceController.Stop();
                            return await WaitForAsync(serviceController, ServiceControllerStatus.Stopped, token).ConfigureAwait(false);
                        case ServiceControllerStatus.ContinuePending:
                        case ServiceControllerStatus.StartPending:
                            if (!await WaitForAsync(serviceController, ServiceControllerStatus.Running, token).ConfigureAwait(false))
                                return false;
                            serviceController.Stop();
                            return await WaitForAsync(serviceController, ServiceControllerStatus.Stopped, token).ConfigureAwait(false);
                        case ServiceControllerStatus.Paused:
                            serviceController.Stop();
                            return await WaitForAsync(serviceController, ServiceControllerStatus.Stopped, token).ConfigureAwait(false);
                        case ServiceControllerStatus.PausePending:
                            if (!await WaitForAsync(serviceController, ServiceControllerStatus.Paused, token).ConfigureAwait(false))
                                return false;
                            serviceController.Stop();
                            return await WaitForAsync(serviceController, ServiceControllerStatus.Stopped, token).ConfigureAwait(false);
                        case ServiceControllerStatus.Stopped:
                            return true;
                        case ServiceControllerStatus.StopPending:
                            return await WaitForAsync(serviceController, ServiceControllerStatus.Stopped, token).ConfigureAwait(false);
                        default:
                            return false;
                    }
            }
            catch (TaskCanceledException)
            {
                return false;
            }
        }

        /// <summary>
        /// Commands the service.
        /// </summary>
        /// <param name="serviceName">Name of the service.</param>
        /// <param name="command">The command.</param>
        /// <param name="token">The token.</param>
        /// <returns>An awaitable task that returns <see langword="true"/> if the command was run.</returns>
        [NotNull]
        public static async Task<bool> CommandService(
            [NotNull] string serviceName,
            int command,
            CancellationToken token = default(CancellationToken))
        {
            Contract.Requires<RequiredContractException>(serviceName != null, "Parameter_Null");
            try
            {
                new ServiceControllerPermission(
                    ServiceControllerPermissionAccess.Control,
                    Environment.MachineName,
                    serviceName).Assert();
                using (ServiceController serviceController = new ServiceController(serviceName, Environment.MachineName)
                    )
                    switch (serviceController.Status)
                    {
                        case ServiceControllerStatus.Running:
                            // ReSharper disable once AccessToDisposedClosure, PossibleNullReferenceException
                            await Task.Run(() => serviceController.ExecuteCommand(command), token).ConfigureAwait(false);
                            return !token.IsCancellationRequested;
                        case ServiceControllerStatus.ContinuePending:
                        case ServiceControllerStatus.StartPending:
                            if (!await WaitForAsync(serviceController, ServiceControllerStatus.Running, token).ConfigureAwait(false))
                                return false;
                            // ReSharper disable once AccessToDisposedClosure, PossibleNullReferenceException
                            await Task.Run(() => serviceController.ExecuteCommand(command), token).ConfigureAwait(false);
                            return !token.IsCancellationRequested;
                        default:
                            return false;
                    }
            }
            catch (TaskCanceledException)
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the service status.
        /// </summary>
        /// <param name="serviceName">Name of the service.</param>
        /// <returns>ServiceControllerStatus.</returns>
        public static ServiceControllerStatus GetServiceStatus([NotNull] string serviceName)
        {
            Contract.Requires<RequiredContractException>(serviceName != null, "Parameter_Null");
            try
            {
                new ServiceControllerPermission(
                    ServiceControllerPermissionAccess.Browse,
                    Environment.MachineName,
                    serviceName).Assert();
                using (ServiceController serviceController = new ServiceController(serviceName, Environment.MachineName)
                    )
                    return serviceController.Status;
            }
            catch (TaskCanceledException)
            {
                return 0;
            }
        }

        /// <summary>
        /// Opens the sc manager.
        /// </summary>
        /// <param name="rights">The rights.</param>
        /// <returns>IntPtr.</returns>
        /// <exception cref="System.ComponentModel.Win32Exception"></exception>
        // ReSharper disable once InconsistentNaming
        private static IntPtr OpenSCManager(ScmAccessRights rights)
        {
            IntPtr scm = OpenSCManager(null, null, rights);
            if (scm == IntPtr.Zero)
                throw new ServiceException(
                    new Win32Exception(),
                    () => ServiceResources.Err_ServiceUtils_OpenSCManager_CouldNotOpenManager);

            return scm;
        }
    }
}