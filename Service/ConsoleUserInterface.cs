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
using System.Diagnostics.Contracts;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WebApplications.Utilities.Formatting;

namespace WebApplications.Utilities.Service
{
    /// <summary>
    /// Implements a console-based service user interface, if running in a console.
    /// </summary>
    public class ConsoleUserInterface : IServiceUserInterface
    {
        /// <summary>
        /// The task completion source
        /// </summary>
        [NotNull]
        private readonly TaskCompletionSource<bool> _taskCompletionSource;

        /// <summary>
        /// Prevents a default instance of the <see cref="ConsoleUserInterface"/> class from being created.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        // ReSharper disable once CodeAnnotationAnalyzer
        private ConsoleUserInterface(CancellationToken token)
        {
            Contract.Requires<RequiredContractException>(ConsoleHelper.IsConsole, "Not_In_Console");
            _taskCompletionSource = new TaskCompletionSource<bool>();
            token.Register(() => _taskCompletionSource.TrySetCanceled());
        }

        /// <summary>
        /// Runs the specified service using the command console as a user interface.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="token">The token.</param>
        /// <returns>An awaitable task.</returns>
        // ReSharper disable once CodeAnnotationAnalyzer
        public static Task Run([NotNull] BaseService service, CancellationToken token = default(CancellationToken))
        {
            Contract.Requires<RequiredContractException>(service != null, "Parameter_Null");
            if (!ConsoleHelper.IsConsole)
                return TaskResult.Completed;

            ConsoleUserInterface ui = new ConsoleUserInterface(token);
            service.Connect(ui);
            return ui._taskCompletionSource.Task;
        }

        /// <summary>
        /// Gets the writer for outputting information from the service.
        /// </summary>
        /// <value>The writer.</value>
        public TextWriter Writer
        {
            get { return ConsoleTextWriter.Default; }
        }

        /// <summary>
        /// Gets the reader for reading input commands.
        /// </summary>
        /// <value>The reader.</value>
        public TextReader Reader
        {
            get { return Console.In; }
        }

        /// <summary>
        /// Called when the server disconnects the UI.
        /// </summary>
        public void OnDisconnect()
        {
            _taskCompletionSource.TrySetResult(true);
        }
    }
}