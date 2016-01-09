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

using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Threading;

namespace WebApplications.Utilities.IO
{
    /// <summary>
    /// Implements a <see cref="PipeStream"/> that support asynchronous read/write and overlapping.
    /// </summary>
    [PublicAPI]
    public class OverlappingPipeStream : IDisposable
    {
        #region PInvoke
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CancelIoEx(SafePipeHandle hFile, ref NativeOverlapped lpOverlapped);

        // TODO Change to ReadFileEx
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadFile(
            SafePipeHandle hFile,
            [Out] byte[] lpBuffer,
            uint nNumberOfBytesToRead,
            out uint lpNumberOfBytesRead,
            ref NativeOverlapped lpOverlapped);

        private const int ErrorMoreData = 234;
        private const int ErrorIoPending = 997;
        #endregion

        /// <summary>
        /// The underlaying stream.
        /// </summary>
        protected PipeStream Stream;

        [NotNull]
        private readonly byte[] _buffer;

        [NotNull]
        private readonly AsyncLock _readLock = new AsyncLock();

        [NotNull]
        private readonly AsyncLock _writeLock = new AsyncLock();

        [NotNull]
        private ManualResetEvent _writeRequiredSignal = new ManualResetEvent(false);

        [NotNull]
        private ManualResetEvent _writeCompleteSignal = new ManualResetEvent(false);

        [NotNull]
        private ManualResetEvent _disposeSignal = new ManualResetEvent(false);

        /// <summary>
        /// Initializes a new instance of the <see cref="OverlappingPipeStream"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="bufferSize">Size of the buffer.</param>
        public OverlappingPipeStream([NotNull] PipeStream stream, int bufferSize = 0)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            Stream = stream;
            if (bufferSize < 1) bufferSize = 1024;
            _buffer = new byte[bufferSize];
        }

        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value><see langword="true" /> if this instance is connected; otherwise, <see langword="false" />.</value>
        public bool IsConnected
        {
            get
            {
                PipeStream stream = Stream;
                return stream != null && stream.IsConnected;
            }
        }

        /// <summary>
        /// Reads a message or block of bytes asynchronously, whilst interleaving reads.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="token">The token.</param>
        /// <returns>Task.</returns>
        [NotNull]
        public async Task<bool> WriteAsync([NotNull] byte[] data, CancellationToken token = default(CancellationToken))
        {
            using (await _writeLock.LockAsync(token).ConfigureAwait(false))
            {
                if (token.IsCancellationRequested) return false;

                PipeStream stream = Stream;
                if (stream == null)
                    return false;
                _writeRequiredSignal.Set();
                try
                {
                    // ReSharper disable once PossibleNullReferenceException
                    await stream.WriteAsync(data, 0, data.Length, token).ConfigureAwait(false);
                    return !token.IsCancellationRequested;
                }
                catch (TaskCanceledException)
                {
                }
                finally
                {
                    _writeRequiredSignal.Reset();
                    _writeCompleteSignal.Set();
                }
                return false;
            }
        }

        /// <summary>
        /// Reads a message or block of bytes asynchronously, whilst interleaving writes.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>
        /// An awaitable task that contains the data that was read.
        /// </returns>
        [NotNull]
        public async Task<byte[]> ReadAsync(CancellationToken token = default(CancellationToken))
        {
            try
            {
                PipeStream stream = Stream;
                if (stream == null) return null;

                using (await _readLock.LockAsync(token).ConfigureAwait(false))
                {
                    ManualResetEvent dataReadyHandle = new ManualResetEvent(false);

                    // Create a list of all the things that can signal a read stop.
                    WaitHandle[] breakConditions = new WaitHandle[token.CanBeCanceled ? 4 : 3];
                    breakConditions[0] = dataReadyHandle;
                    breakConditions[1] = _writeRequiredSignal;
                    breakConditions[2] = _disposeSignal;
                    if (token.CanBeCanceled)
                        breakConditions[3] = token.WaitHandle;

                    using (MemoryStream ms = new MemoryStream())
                        do
                        {
                            NativeOverlapped lpOverlapped;
                            lpOverlapped.InternalHigh = IntPtr.Zero;
                            lpOverlapped.InternalLow = IntPtr.Zero;
                            lpOverlapped.OffsetHigh = 0;
                            lpOverlapped.OffsetLow = 0;
                            // ReSharper disable once PossibleNullReferenceException
                            lpOverlapped.EventHandle = dataReadyHandle.SafeWaitHandle.DangerousGetHandle();
                            byte[] b = Array<byte>.Empty;
                            // Set to read zero bytes, should work asynchronously.
                            uint read;
                            bool rval = ReadFile(
                                stream.SafePipeHandle,
                                b,
                                0,
                                out read,
                                ref lpOverlapped);

                            int breakCause;
                            if (!rval)
                            {
                                // Operation is completing asynchronously
                                int lastError = Marshal.GetLastWin32Error();
                                if (lastError != ErrorIoPending &&
                                    lastError != ErrorMoreData)
                                    throw new Win32Exception(lastError);

                                breakCause = WaitHandle.WaitAny(breakConditions);
                            }
                            else
                            //operation completed synchronously; there is data available
                                breakCause = 0; //jump into the reading code in the switch below
                            switch (breakCause)
                            {
                                case 0:
                                    // We have data so we can read it straight out
                                    // ReSharper disable once PossibleNullReferenceException
                                    int bytesRead =
                                        await stream.ReadAsync(_buffer, 0, _buffer.Length, token).ConfigureAwait(false);
                                    ms.Write(_buffer, 0, bytesRead);

                                    if (stream.ReadMode == PipeTransmissionMode.Byte ||
                                        stream.IsMessageComplete)
                                        return ms.Length > 0
                                            ? ms.GetBuffer()
                                            : null;

                                    // We haven't finished our message so continue
                                    continue;

                                case 1:
                                    //asked to yield
                                    //first kill that read operation
                                    if (!CancelIoEx(stream.SafePipeHandle, ref lpOverlapped))
                                        throw new Win32Exception(Marshal.GetLastWin32Error());

                                    //should hand over the pipe mutex and wait to be told to take it back
                                    _writeRequiredSignal.Reset();
                                    _writeCompleteSignal.WaitOne();
                                    _writeCompleteSignal.Reset();
                                    continue;

                                default:
                                    //asked to die, either due to disposal or cancellation.
                                    //we are the ones responsible for cleaning up the pipe
                                    if (!CancelIoEx(stream.SafePipeHandle, ref lpOverlapped))
                                        throw new Win32Exception(Marshal.GetLastWin32Error());
                                    //finally block will clean up the pipe and the mutex
                                    return null; //quit the thread
                            }
                        } while (true);
                }
            }
            catch (TaskCanceledException)
            {
            }
            return null;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            ManualResetEvent ds = Interlocked.Exchange(ref _disposeSignal, null);
            if (ds != null)
            {
                ds.Set();
                ds.Dispose();
            }

            ManualResetEvent wc = Interlocked.Exchange(ref _writeCompleteSignal, null);
            if (wc != null)
            {
                wc.Set();
                wc.Dispose();
            }

            ManualResetEvent wr = Interlocked.Exchange(ref _writeRequiredSignal, null);
            if (wr != null)
                wr.Dispose();

            PipeStream stream = Interlocked.Exchange(ref Stream, null);
            if (stream == null) return;
            stream.Dispose();
        }
    }
}