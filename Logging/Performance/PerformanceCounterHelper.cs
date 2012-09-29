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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Logging.Performance
{
    /// <summary>
    ///   Performance counters used for operations.
    /// </summary>
    public abstract class PerformanceCounterHelper
    {
        /// <summary>
        /// The current instance name for all performance counters.
        /// </summary>
        [NotNull]
        public static readonly string InstanceGuid = Guid.NewGuid().ToString();

        /// <summary>
        /// The machine name used to access performance counters.
        /// </summary>
        [NotNull]
        public const string MachineName = ".";

        /// <summary>
        /// Whether the current process has access to performance counters.
        /// </summary>
        public static readonly bool HasAccess;

        /// <summary>
        /// Initializes static members of the <see cref="PerformanceCounterHelper" /> class.
        /// </summary>
        /// <remarks>We only check access to performance counters once.</remarks>
        static PerformanceCounterHelper()
        {
            try
            {
                // Check we have access to the performance counters.
                PerformanceCounterCategory.Exists("TestAccess", MachineName);
            }
            catch (Exception exception)
            {
                Log.Add(exception, LogLevel.SystemNotification);
                Log.Add(Resources.PerformanceCounterHelper_ProcessDoesNotHaveAccess, LogLevel.SystemNotification);
                HasAccess = false;
            }
        }

        /// <summary>
        ///   The performance counter's category.
        /// </summary>
        [NotNull]
        [UsedImplicitly]
        public readonly string CategoryName;

        /// <summary>
        /// Whether the counter is valid.
        /// </summary>
        public readonly bool IsValid;

        /// <summary>
        /// The underlying counters.
        /// </summary>
        [NotNull]
        protected System.Diagnostics.PerformanceCounter[] Counters;

        /// <summary>
        ///   Prevents a default instance of the <see cref="PerformanceCounterHelper"/> class from being created.
        /// </summary>
        /// <param name="categoryName">
        ///   The performance counter's <see cref="PerformanceCounterHelper.CategoryName">category name</see>.
        /// </param>
        protected PerformanceCounterHelper([NotNull] string categoryName, [NotNull]IEnumerable<CounterCreationData> counters)
        {
            CategoryName = categoryName;
            if (!HasAccess)
                return;

            CounterCreationData[] cArray = counters as CounterCreationData[] ?? counters.ToArray();
            if (cArray.Length < 1)
            {
                IsValid = false;
                return;
            }

            // Set up the performance counter(s)
            try
            {
                if (!PerformanceCounterCategory.Exists(CategoryName))
                {
                    Log.Add(
                        Resources.PerformanceCounterHelper_CategoryDoesNotExist,
                        LogLevel.SystemNotification,
                        CategoryName);
                    IsValid = false;
                    return;
                }

                Counters = new System.Diagnostics.PerformanceCounter[cArray.Length];
                for (int c = 0; c < cArray.Length; c++)
                {
                    CounterCreationData counter = cArray[c];
                    if (!PerformanceCounterCategory.CounterExists(counter.CounterName, categoryName))
                    {
                        Log.Add(
                            Resources.PerformanceCounterHelper_CounterDoesNotExist,
                            LogLevel.SystemNotification,
                            CategoryName, counter.CounterName);
                        IsValid = false;
                        return;
                    }
                    Counters[c] = new System.Diagnostics.PerformanceCounter()
                        {
                            CategoryName = categoryName,
                            CounterName = counter.CounterName,
                            MachineName = MachineName,
                            InstanceLifetime = PerformanceCounterInstanceLifetime.Process,
                            InstanceName = InstanceGuid,
                            ReadOnly = false
                        };

                    // Read the first value to 'start' the counters.
                    Counters[c].NextValue();
                }
            }
            catch (UnauthorizedAccessException unauthorizedAccessException)
            {
                Log.Add(unauthorizedAccessException, LogLevel.SystemNotification);
                Log.Add(Resources.PerformanceCounterHelper_ProcessDoesNotHaveAccess, LogLevel.SystemNotification);
                IsValid = false;
            }
            catch (Exception exception)
            {
                // Create error but don't throw.
                new LoggingException(exception,
                                     Resources.PerformanceCounterHelper_UnhandledExceptionOccurred,
                                     LogLevel.SystemNotification,
                                     CategoryName);
                IsValid = false;
            }
        }

        /// <summary>
        /// Creates the specified performance counter (use during installation only).
        /// </summary>
        /// <param name="categoryName">Name of the category.</param>
        /// <param name="categoryHelp">The category help.</param>
        /// <param name="counters">The optional counter data collection (leave <see langword="null"/>
        /// to use the <see cref="DefaultCounters">default counter collection</see>).</param>
        /// <returns><see langword="true" /> if the category was created; otherwise <see langword="false" />.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <remarks><para>
        /// It is strongly recommended that new performance counter categories
        /// be created during the installation of the application, not during the execution
        /// of the application. This allows time for the operating system to refresh its list
        /// of registered performance counter categories. If the list has not been refreshed,
        /// the attempt to use the category will fail.
        ///   </para>
        ///   <para>
        /// To read performance counters in Windows Vista and later, Windows XP Professional
        /// x64 Edition, or Windows Server 2003, you must either be a member of the Performance
        /// Monitor Users group or have administrative privileges.
        ///   </para>
        ///   <para>
        /// To avoid having to elevate your privileges to access performance counters in Windows
        /// Vista and later, add yourself to the Performance Monitor Users group.
        ///   </para>
        ///   <para>
        /// In Windows Vista and later, User Account Control (UAC) determines the privileges of
        /// a user. If you are a member of the Built-in Administrators group, you are assigned
        /// two run-time access tokens: a standard user access token and an administrator access
        /// token. By default, you are in the standard user role. To execute the code that
        /// accesses performance counters, you must first elevate your privileges from standard
        /// user to administrator. You can do this when you start an application by right-clicking
        /// the application icon and indicating that you want to run as an administrator.
        ///   </para>
        ///   <para>
        /// For more information see MSDN : http://msdn.microsoft.com/EN-US/library/sb32hxtc(v=VS.110,d=hv.2).aspx
        ///   </para></remarks>
        protected static bool Create([NotNull] string categoryName, [NotNull]string categoryHelp, [NotNull] IEnumerable<CounterCreationData> counters)
        {
            if (!HasAccess)
                return false;

            CounterCreationData[] cArray = counters as CounterCreationData[] ?? counters.ToArray();
            if (cArray.Length < 1)
                return false;
            try
            {
                if (PerformanceCounterCategory.Exists(categoryName))
                    return cArray.All(c => PerformanceCounterCategory.CounterExists(c.CounterName, categoryName));
                PerformanceCounterCategory.Create(categoryName, categoryHelp,
                                                  PerformanceCounterCategoryType.MultiInstance, new CounterCreationDataCollection(cArray));
                return cArray.All(c => PerformanceCounterCategory.CounterExists(c.CounterName, categoryName));
            }
            catch (Exception e)
            {
                Log.Add(e, LogLevel.SystemNotification);
                Log.Add(Resources.PerformanceCounterHelper_Create_Failed,
                        LogLevel.SystemNotification,
                        categoryName);
                return false;
            }
        }

        /// <summary>
        /// Deletes the specified performance counter (use during uninstall only).
        /// </summary>
        /// <param name="categoryName">Name of the category.</param>
        /// <returns><see langword="true"/> if the category no longer exists; otherwise <see langword="false"/>.</returns>
        /// <remarks><para>
        /// It is strongly recommended that performance counter categories are only removed
        /// during the removal of the application, not during the execution
        /// of the application.</para>
        ///   <para>
        /// To read performance counters in Windows Vista and later, Windows XP Professional
        /// x64 Edition, or Windows Server 2003, you must either be a member of the Performance
        /// Monitor Users group or have administrative privileges.
        ///   </para>
        ///   <para>
        /// To avoid having to elevate your privileges to access performance counters in Windows
        /// Vista and later, add yourself to the Performance Monitor Users group.
        ///   </para>
        ///   <para>
        /// In Windows Vista and later, User Account Control (UAC) determines the privileges of
        /// a user. If you are a member of the Built-in Administrators group, you are assigned
        /// two run-time access tokens: a standard user access token and an administrator access
        /// token. By default, you are in the standard user role. To execute the code that
        /// accesses performance counters, you must first elevate your privileges from standard
        /// user to administrator. You can do this when you start an application by right-clicking
        /// the application icon and indicating that you want to run as an administrator.
        ///   </para>
        ///   <para>
        /// For more information see MSDN : http://msdn.microsoft.com/EN-US/library/s55bz6c1(v=VS.110,d=hv.2).aspx
        ///   </para></remarks>
        protected static bool Delete([NotNull] string categoryName, [NotNull] IEnumerable<CounterCreationData> counters)
        {
            if (!HasAccess)
                return false;

            CounterCreationData[] cArray = counters as CounterCreationData[] ?? counters.ToArray();
            if (cArray.Length < 1)
                return false;

            try
            {
                if (!PerformanceCounterCategory.Exists(categoryName) ||
                    !cArray.All(c => PerformanceCounterCategory.CounterExists(c.CounterName, categoryName)))
                    return false;

                PerformanceCounterCategory.Delete(categoryName);
                return !cArray.Any(c => PerformanceCounterCategory.CounterExists(c.CounterName, categoryName));
            }
            catch (Exception e)
            {
                Log.Add(e, LogLevel.SystemNotification);
                Log.Add(Resources.PerformanceCounterHelper_Delete_Failed,
                        LogLevel.SystemNotification,
                        categoryName);
                return false;
            }
        }
    }
}