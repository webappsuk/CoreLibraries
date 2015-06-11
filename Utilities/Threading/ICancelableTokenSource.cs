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
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Threading
{
    /// <summary>
    /// Interface for a token source.
    /// </summary>
    [PublicAPI]
    public interface ICancelableTokenSource : ITokenSource
    {
        /// <summary>
        /// Communicates a request for cancellation.
        /// </summary>
        /// <exception cref="T:System.ObjectDisposedException">This <see cref="ICancelableTokenSource"/> has been disposed.</exception>
        /// <exception cref="T:System.AggregateException">An aggregate exception containing all the exceptions thrown by the registered callbacks on the associated <see cref="T:System.Threading.CancellationToken"/>.</exception>
        [PublicAPI]
        void Cancel();

        /// <summary>
        /// Communicates a request for cancellation, and specifies whether remaining callbacks and cancelable operations should be processed.
        /// </summary>
        /// <param name="throwOnFirstException">true if exceptions should immediately propagate; otherwise, false.</param>
        /// <exception cref="T:System.ObjectDisposedException">This <see cref="ICancelableTokenSource"/> has been disposed.</exception>
        /// <exception cref="T:System.AggregateException">An aggregate exception containing all the exceptions thrown by the registered callbacks on the associated <see cref="T:System.Threading.CancellationToken"/>.</exception>
        [PublicAPI]
        void Cancel(bool throwOnFirstException);

        /// <summary>
        /// Schedules a cancel operation on this <see cref="ICancelableTokenSource"/> after the specified time span.
        /// </summary>
        /// <param name="delay">The time span to wait before canceling this <see cref="ICancelableTokenSource"/>.</param>
        /// <exception cref="T:System.ObjectDisposedException">The exception thrown when this <see cref="ICancelableTokenSource"/> has been disposed.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">The exception that is thrown when <paramref name="delay"/> is less than -1 or greater than Int32.MaxValue.</exception>
        [PublicAPI]
        void CancelAfter(TimeSpan delay);

        /// <summary>
        /// Schedules a cancel operation on this <see cref="ICancelableTokenSource"/> after the specified number of milliseconds.
        /// </summary>
        /// <param name="millisecondsDelay">The time span to wait before canceling this <see cref="ICancelableTokenSource"/>.</param>
        /// <exception cref="T:System.ObjectDisposedException">The exception thrown when this <see cref="ICancelableTokenSource"/> has been disposed.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">The exception thrown when <paramref name="millisecondsDelay"/> is less than -1.</exception>
        [PublicAPI]
        void CancelAfter(int millisecondsDelay);
    }
}