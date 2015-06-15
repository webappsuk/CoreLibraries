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
using System.Diagnostics;
using System.Linq;
using System.Threading;
using NodaTime;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Threading
{
    internal class TimedTokenSource : ICancelableTokenSource
    {
        /// <summary>
        /// The timeout source
        /// </summary>
        [CanBeNull]
        private CancellationTokenSource _timeoutSource;

        /// <summary>
        /// The _sources
        /// </summary>
        [CanBeNull]
        private CancellationTokenSource _source;

        /// <summary>
        /// Gets the <see cref="CancellationToken" /> associated with this <see cref="WrappedTokenSource" />.
        /// </summary>
        /// <value>
        /// The <see cref="CancellationToken" /> associated with this <see cref="WrappedTokenSource" />.
        /// </value>
        public CancellationToken Token
        {
            get
            {
                CancellationTokenSource source = _source;
                return source != null ? source.Token : TaskResult.CancelledToken;
            }
        }

        /// <summary>
        /// Gets whether cancellation has been requested for this token source.
        /// </summary>
        /// <value>
        /// Whether cancellation has been requested for this token source.
        /// </value>
        public bool IsCancellationRequested
        {
            get
            {
                CancellationTokenSource source = _source;
                return source == null || source.IsCancellationRequested;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimedTokenSource"/> class.
        /// </summary>
        /// <param name="milliseconds">The milliseconds.</param>
        /// <param name="token">The token.</param>
        public TimedTokenSource(int milliseconds, CancellationToken token)
        {
            Debug.Assert(token.CanBeCanceled);

            _timeoutSource = new CancellationTokenSource(milliseconds);
            _source = CancellationTokenSource.CreateLinkedTokenSource(
                token,
                _timeoutSource.Token);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimedTokenSource"/> class.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        /// <param name="token">The token.</param>
        public TimedTokenSource(TimeSpan timeout, CancellationToken token)
        {
            Debug.Assert(token.CanBeCanceled);

            _timeoutSource = new CancellationTokenSource(timeout);
            _source = CancellationTokenSource.CreateLinkedTokenSource(
                token,
                _timeoutSource.Token);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimedTokenSource"/> class.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        /// <param name="token">The token.</param>
        public TimedTokenSource(Duration timeout, CancellationToken token)
        {
            Debug.Assert(token.CanBeCanceled);

            _timeoutSource = new CancellationTokenSource(timeout.ToTimeSpan());
            _source = CancellationTokenSource.CreateLinkedTokenSource(
                token,
                _timeoutSource.Token);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimedTokenSource"/> class.
        /// </summary>
        /// <param name="milliseconds">The milliseconds.</param>
        /// <param name="tokens">The tokens.</param>
        public TimedTokenSource(int milliseconds, [NotNull] params CancellationToken[] tokens)
        {
            Debug.Assert(tokens != null);
            Debug.Assert(tokens.Length > 0);
            Debug.Assert(tokens.Any(t => t.CanBeCanceled));

            _timeoutSource = new CancellationTokenSource(milliseconds);
            _source = CancellationTokenSource.CreateLinkedTokenSource(
                tokens.Union(new[] { _timeoutSource.Token }).ToArray());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimedTokenSource"/> class.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        /// <param name="tokens">The tokens.</param>
        public TimedTokenSource(TimeSpan timeout, [NotNull] params CancellationToken[] tokens)
        {
            Debug.Assert(tokens != null);
            Debug.Assert(tokens.Length > 0);
            Debug.Assert(tokens.Any(t => t.CanBeCanceled));

            _timeoutSource = new CancellationTokenSource(timeout);
            _source = CancellationTokenSource.CreateLinkedTokenSource(
                tokens.Union(new[] { _timeoutSource.Token }).ToArray());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimedTokenSource"/> class.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        /// <param name="tokens">The tokens.</param>
        public TimedTokenSource(Duration timeout, [NotNull] params CancellationToken[] tokens)
        {
            Debug.Assert(tokens != null);
            Debug.Assert(tokens.Length > 0);
            Debug.Assert(tokens.Any(t => t.CanBeCanceled));

            _timeoutSource = new CancellationTokenSource(timeout.ToTimeSpan());
            _source = CancellationTokenSource.CreateLinkedTokenSource(
                tokens.Union(new[] { _timeoutSource.Token }).ToArray());
        }

        /// <summary>
        /// Communicates a request for cancellation.
        /// </summary>
        public void Cancel()
        {
            CancellationTokenSource source = _source;
            if (source != null)
                source.Cancel();
        }

        /// <summary>
        /// Communicates a request for cancellation, and specifies whether remaining callbacks and cancelable operations should be processed.
        /// </summary>
        /// <param name="throwOnFirstException">true if exceptions should immediately propagate; otherwise, false.</param>
        public void Cancel(bool throwOnFirstException)
        {
            CancellationTokenSource source = _source;
            if (source != null)
                source.Cancel(throwOnFirstException);
        }

        /// <summary>
        /// Schedules a cancel operation on this <see cref="T:System.Threading.CancellationTokenSource" /> after the specified time span.
        /// </summary>
        /// <param name="delay">The time span to wait before canceling this <see cref="T:System.Threading.CancellationTokenSource" />.</param>
        public void CancelAfter(TimeSpan delay)
        {
            CancellationTokenSource source = _source;
            if (source != null)
                source.CancelAfter(delay);
        }

        /// <summary>
        /// Schedules a cancel operation on this <see cref="T:System.Threading.CancellationTokenSource" /> after the specified duration.
        /// </summary>
        /// <param name="delay">The duration to wait before canceling this <see cref="T:System.Threading.CancellationTokenSource" />.</param>
        public void CancelAfter(Duration delay)
        {
            CancellationTokenSource source = _source;
            if (source != null)
                source.CancelAfter(delay.ToTimeSpan());
        }

        /// <summary>
        /// Schedules a cancel operation on this <see cref="T:System.Threading.CancellationTokenSource" /> after the specified number of milliseconds.
        /// </summary>
        /// <param name="millisecondsDelay">The time span to wait before canceling this <see cref="T:System.Threading.CancellationTokenSource" />.</param>
        public void CancelAfter(int millisecondsDelay)
        {
            CancellationTokenSource source = _source;
            if (source != null)
                source.CancelAfter(millisecondsDelay);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            CancellationTokenSource source = Interlocked.Exchange(ref _source, null);
            if (source != null)
                source.Dispose();
            source = Interlocked.Exchange(ref _timeoutSource, null);
            if (source != null)
                source.Dispose();
        }
    }
}