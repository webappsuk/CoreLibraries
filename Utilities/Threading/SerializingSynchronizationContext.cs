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
using System.Collections.Concurrent;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Threading
{
    /// <summary>
    /// Allows serialization of actions.
    /// </summary>
    /// <remarks>See https://github.com/revane/SerializingSynchronizationContext.
    /// </remarks>
    [PublicAPI]
    public sealed class SerializingSynchronizationContext : SynchronizationContext
    {
        [NotNull]
        private readonly object _lock = new object();

        [NotNull]
        private readonly ConcurrentQueue<CallbackInfo> _queue = new ConcurrentQueue<CallbackInfo>();

        /// <summary>
        /// When overridden in a derived class, dispatches an asynchronous message to a synchronization context.
        /// </summary>
        /// <param name="callback">The <see cref="T:System.Threading.SendOrPostCallback" /> delegate to call.</param>
        /// <param name="state">The object passed to the delegate.</param>
        public override void Post(SendOrPostCallback callback, [NotNull] object state)
        {
            Contract.Requires(callback != null);
            Contract.Requires(state != null);
            _queue.Enqueue(new CallbackInfo(callback, state));

            bool lockTaken = false;
            try
            {
                Monitor.TryEnter(_lock, ref lockTaken);

                if (lockTaken)
                    ProcessQueue();
                else
                    Task.Run((Action) ProcessQueue);
            }
            finally
            {
                if (lockTaken)
                    Monitor.Exit(_lock);
            }
        }

        /// <summary>
        /// When overridden in a derived class, dispatches a synchronous message to a synchronization context.
        /// </summary>
        /// <param name="callback">The <see cref="T:System.Threading.SendOrPostCallback" /> delegate to call.</param>
        /// <param name="state">The object passed to the delegate.</param>
        public override void Send(SendOrPostCallback callback, [NotNull] object state)
        {
            Contract.Requires(callback != null);
            Contract.Requires(state != null);
            lock (_lock)
            {
                SynchronizationContext outer = Current;
                try
                {
                    SetSynchronizationContext(this);
                    callback(state);
                }
                finally
                {
                    SetSynchronizationContext(outer);
                }
            }
        }

        /// <summary>
        /// Processes the queue.
        /// </summary>
        private void ProcessQueue()
        {
            if (_queue.IsEmpty) return;

            lock (_lock)
            {
                SynchronizationContext outer = Current;
                try
                {
                    SetSynchronizationContext(this);

                    CallbackInfo callback;
                    while (_queue.TryDequeue(out callback))
                        try
                        {
                            callback.Callback(callback.State);
                        }
                        catch (Exception e)
                        {
                            throw new Exception(
                                string.Format(
                                    "Exception in posted callback on {0}: {1}",
                                    GetType().FullName,
                                    e.Message),
                                e);
                        }
                }
                finally
                {
                    SetSynchronizationContext(outer);
                }
            }
        }

        /// <summary>
        /// Groups the state with the callback.
        /// </summary>
        private struct CallbackInfo
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="CallbackInfo"/> struct.
            /// </summary>
            /// <param name="callback">The Callback.</param>
            /// <param name="state">The state.</param>
            public CallbackInfo([NotNull] SendOrPostCallback callback, [CanBeNull] object state)
                : this()
            {
                Contract.Requires(callback != null);
                Callback = callback;
                State = state;
            }

            /// <summary>
            /// Gets or sets the Callback.
            /// </summary>
            /// <value>The Callback.</value>
            [NotNull]
            public SendOrPostCallback Callback { get; private set; }

            /// <summary>
            /// Gets or sets the state.
            /// </summary>
            /// <value>The state.</value>
            [CanBeNull]
            public object State { get; private set; }
        }
    }
}