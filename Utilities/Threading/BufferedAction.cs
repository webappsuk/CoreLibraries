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

using NodaTime;
using System;
using System.Collections.Generic;
using System.Threading;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Threading
{
    /// <summary>
    /// Buffers calls to an action.
    /// </summary>
    [PublicAPI]
    public class BufferedAction : IDisposable
    {
        /// <summary>
        /// The asynchronous action to run.
        /// </summary>
        [NotNull]
        private readonly Action<object[][]> _action;

        /// <summary>
        /// The duration (in milliseconds) to buffer the action for.
        /// </summary>
        public readonly long Duration;

        /// <summary>
        /// The number of executions to buffer.
        /// </summary>
        public readonly int Count;

        /// <summary>
        /// The current buffer (if any).
        /// </summary>
        private ActionBuffer _buffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <param name="count">The number of executions to buffer, or less than or equal to zero to buffer only by time.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <para><paramref name="duration"/> is less than or equal to zero.</para>
        /// <para>-or-</para>
        /// <para><paramref name="duration"/> is equal to <see cref="Timeout.Infinite"/> and <paramref name="count"/> is less than or equal to zero.</para>
        /// </exception>
        public BufferedAction(
            [NotNull] Action<object[][]> action,
            Duration duration,
            int count = 0)
            : this(action, (long)duration.TotalMilliseconds(), count)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <param name="count">The number of executions to buffer, or less than or equal to zero to buffer only by time.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <para><paramref name="duration"/> is less than or equal to zero.</para>
        /// <para>-or-</para>
        /// <para><paramref name="duration"/> is equal to <see cref="Timeout.Infinite"/> and <paramref name="count"/> is less than or equal to zero.</para>
        /// </exception>
        public BufferedAction(
            [NotNull] Action<object[][]> action,
            TimeSpan duration,
            int count = 0)
            : this(action, (long)duration.TotalMilliseconds, count)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedAction" /> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="duration">The duration is the amount of time the result of a successful execution is held, after the point a successful request was made.</param>
        /// <param name="count">The number of executions to buffer, or less than or equal to zero to buffer only by time.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <para><paramref name="duration"/> is less than or equal to zero.</para>
        /// <para>-or-</para>
        /// <para><paramref name="duration"/> is equal to <see cref="Timeout.Infinite"/> and <paramref name="count"/> is less than or equal to zero.</para>
        /// </exception>
        public BufferedAction(
            [NotNull] Action<object[][]> action,
            long duration,
            int count = 0)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (duration <= 0 && duration != Timeout.Infinite && count <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(duration),
                    Resources.BufferedAction_BufferedAction_Invalid_Duration);

            _action = action;
            Duration = duration;
            Count = count;
        }

        /// <summary>
        /// Buffers a call to the underlying action.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        public void Run(params object[] arguments)
        {
            lock (_action)
            {
                if (_buffer == null)
                    _buffer = new ActionBuffer(this);
                _buffer.Add(arguments);
            }
        }

        /// <summary>
        /// Flushes the buffer, calling the underlying action.
        /// </summary>
        public void Flush()
        {
            lock (_action)
                _buffer?.RunAction();
        }

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the specified instance.
        /// </summary>
        /// <param name="disposing">Whether this is disposing or finalizing.</param>
        /// <remarks>
        /// <para><paramref name="disposing"/> indicates whether the method was invoked from the 
        /// <see cref="IDisposable.Dispose"/> implementation or from the finalizer. The implementation should check the
        /// parameter before  accessing other reference objects. Such objects should  only be accessed when the method 
        /// is called from the <see cref="IDisposable.Dispose"/> implementation (when the <paramref name="disposing"/> 
        /// parameter is equal to <see langword="true"/>). If the method is invoked from the finalizer
        /// (disposing is false), other objects should not be accessed. The reason is that objects are finalized in an 
        /// unpredictable order and so they, or any of their dependencies, might already have been finalized.</para>
        /// </remarks>
        protected void Dispose(bool disposing)
        {
            if (!disposing) return;
            lock (_action)
            {
                ActionBuffer buffer = Interlocked.Exchange(ref _buffer, null);
                buffer?.Dispose();
            }
        }

        /// <summary>
        /// An individual buffered action.
        /// </summary>
        private sealed class ActionBuffer : IDisposable
        {
            [NotNull]
            private readonly BufferedAction _action;

            [NotNull]
            private readonly List<object[]> _arguments = new List<object[]>();

            private Timer _timer;

            /// <summary>
            /// Initializes a new instance of the <see cref="ActionBuffer" /> class.
            /// </summary>
            /// <param name="action">The action.</param>
            public ActionBuffer([NotNull] BufferedAction action)
            {
                _action = action;
                if (action.Duration >= 0)
                    _timer = new Timer(OnTick, this, action.Duration, Timeout.Infinite);
            }

            public void Add(object[] arguments)
            {
                lock (_arguments)
                {
                    _arguments.Add(arguments);

                    if (_action.Count > 0 && _arguments.Count >= _action.Count)
                        RunAction();
                }
            }

            /// <summary>
            /// Called when we have a timer tick.
            /// </summary>
            /// <param name="state">The state.</param>
            private void OnTick(object state) => RunAction();

            /// <summary>
            /// Runs the action and clears the buffer.
            /// </summary>
            public void RunAction()
            {
                if (_action._buffer == null)
                    return;

                lock (_action._action)
                {
                    if (_action._buffer == null)
                        return;

                    _action._buffer = null;
                }

                object[][] arguments;
                lock (_arguments)
                {
                    arguments = _arguments.ToArray();
                    Dispose();
                }

                _action._action(arguments);
            }

            /// <summary>
            /// Disposes this instance.
            /// </summary>
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            /// <summary>
            /// Disposes the specified instance.
            /// </summary>
            /// <param name="disposing">Whether this is disposing or finalizing.</param>
            /// <remarks>
            /// <para><paramref name="disposing"/> indicates whether the method was invoked from the 
            /// <see cref="IDisposable.Dispose"/> implementation or from the finalizer. The implementation should check the
            /// parameter before  accessing other reference objects. Such objects should  only be accessed when the method 
            /// is called from the <see cref="IDisposable.Dispose"/> implementation (when the <paramref name="disposing"/> 
            /// parameter is equal to <see langword="true"/>). If the method is invoked from the finalizer
            /// (disposing is false), other objects should not be accessed. The reason is that objects are finalized in an 
            /// unpredictable order and so they, or any of their dependencies, might already have been finalized.</para>
            /// </remarks>
            private void Dispose(bool disposing)
            {
                if (!disposing) return;
                Timer timer = Interlocked.Exchange(ref _timer, null);
                timer?.Dispose();
                // ReSharper disable once InconsistentlySynchronizedField
                _arguments.Clear();
            }
        }
    }
}