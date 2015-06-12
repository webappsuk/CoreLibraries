#region © Copyright Web Applications (UK) Ltd, 2015.  All rights reserved.
// Copyright (c) 2015, Web Applications UK Ltd
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

using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.IO
{
    /// <summary>
    /// Implements a Named Pipe Client that support asynchronous read/write and overlapping.
    /// </summary>
    [PublicAPI]
    public class OverlappingPipeClientStream : OverlappingPipeStream
    {
        /// <summary>
        /// The read mode.
        /// </summary>
        [PublicAPI]
        public readonly PipeTransmissionMode ReadMode;

        /// <summary>
        /// Initializes a new instance of the <see cref="OverlappingPipeClientStream"/> class.
        /// </summary>
        /// <param name="serverName">Name of the server.</param>
        /// <param name="pipeName">Name of the pipe.</param>
        /// <param name="transmissionMode"></param>
        /// <param name="impersonationLevel">The impersonation level.</param>
        /// <param name="inheritability">The inheritability.</param>
        public OverlappingPipeClientStream(
            [NotNull] string serverName,
            [NotNull] string pipeName,
            PipeTransmissionMode transmissionMode,
            TokenImpersonationLevel impersonationLevel = TokenImpersonationLevel.None,
            HandleInheritability inheritability = HandleInheritability.None)
            : base(new NamedPipeClientStream(
                       serverName,
                       pipeName,
                       PipeDirection.InOut,
                       PipeOptions.Asynchronous,
                       impersonationLevel,
                       inheritability))
        {
            Debug.Assert(Stream != null);
            ReadMode = transmissionMode;
        }

        /// <summary>
        /// Waits for a connection.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>An awaitable task.</returns>
        [NotNull]
        [PublicAPI]
        public Task Connect(CancellationToken token)
        {
            NamedPipeClientStream stream = Stream as NamedPipeClientStream;
            if (stream == null) return TaskResult.False;
            if (stream.IsConnected) return TaskResult.True;
            // TODO May be a better way using PInvoke...
            // ReSharper disable once PossibleNullReferenceException
            return Task.Run(() => stream.Connect(60000), token)
                .ContinueWith(
                    t => stream.ReadMode = ReadMode,
                    token,
                    TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion,
                    // ReSharper disable once AssignNullToNotNullAttribute
                    TaskScheduler.Default);
        }
    }
}