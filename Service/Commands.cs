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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WebApplications.Utilities.Logging;
using WebApplications.Utilities.Logging.Interfaces;
using WebApplications.Utilities.Logging.Loggers;
using WebApplications.Utilities.Performance;
using WebApplications.Utilities.Threading;

namespace WebApplications.Utilities.Service
{
    /// <summary>
    /// Base implementation of a service.
    /// </summary>
    public abstract partial class BaseService<TService>
    {
        /// <summary>
        /// Provides command help.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="commandName">Name of the command.</param>
        /// <param name="parameter">The parameter.</param>
        // ReSharper disable once CodeAnnotationAnalyzer
        protected override void Help(TextWriter writer, string commandName = null, string parameter = null)
        {
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <param name="args">The arguments.</param>
        [PublicAPI]
        [ServiceRunnerCommand(typeof(ServiceResources), "Cmd_Start_Names", "Cmd_Start_Description")]
        public void Start([CanBeNull] string[] args)
        {
            lock (_lock)
            {
                if (State != ServiceState.Stopped)
                {
                    Log.Add(LoggingLevel.Error, () => ServiceResources.Err_ServiceRunner_ServiceAlreadyRunning, ServiceName);
                    return;
                }

                if (args == null)
                    args = new string[0];

                Log.Add(LoggingLevel.Information, () => ServiceResources.Inf_ServiceRunner_Start_Starting, ServiceName);
                Stopwatch s = Stopwatch.StartNew();
                try
                {
                    if (IsService)
                    {
                        Contract.Assert(ServiceController != null);
                        ServiceController.Start(args);
                    }
                    else
                    {
                        OnStart(args);
                    }
                    Log.Add(
                        LoggingLevel.Information,
                        () => ServiceResources.Inf_ServiceRunner_Start_Started,
                        ServiceName,
                        s.Elapsed.TotalMilliseconds);
                }
                catch (TargetInvocationException exception)
                {
                    Contract.Assert(exception.InnerException != null);
                    Log.Add(exception.InnerException);
                }
            }
        }
    }
}