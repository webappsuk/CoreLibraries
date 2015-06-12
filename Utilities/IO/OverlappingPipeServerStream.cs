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

using System;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.IO
{
    /// <summary>
    /// Implements a Named Pipe Server that support asynchronous read/write and overlapping.
    /// </summary>
    [PublicAPI]
    public class OverlappingPipeServerStream : OverlappingPipeStream
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OverlappingPipeServerStream" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="maximumConnections">The maximum connections.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="inBufferSize">Size of the in buffer.</param>
        /// <param name="outBufferSize">Size of the out buffer.</param>
        /// <param name="security">The security.</param>
        public OverlappingPipeServerStream(
            [NotNull] string name,
            int maximumConnections,
            PipeTransmissionMode mode,
            int inBufferSize,
            int outBufferSize,
            [CanBeNull] PipeSecurity security)
            : base(new NamedPipeServerStream(
                       name,
                       PipeDirection.InOut,
                       maximumConnections,
                       mode,
                       PipeOptions.Asynchronous,
                       inBufferSize,
                       outBufferSize,
                       security),
                   inBufferSize)
        {
            if (name == null) throw new ArgumentNullException("name");
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
            NamedPipeServerStream stream = Stream as NamedPipeServerStream;
            if (stream == null) return TaskResult.False;
            if (stream.IsConnected) return TaskResult.True;
            // ReSharper disable once PossibleNullReferenceException
            return Task.Factory
                .FromAsync(
                    stream.BeginWaitForConnection,
                    stream.EndWaitForConnection,
                    null)
                .WithCancellation(token);
        }
    }
}