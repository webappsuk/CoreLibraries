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
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Threading
{
    /// <summary>
    /// Interface to a task that can be canceled.
    /// </summary>
    public interface ICancelableTask : IDisposable
    {
        /// <summary>
        /// Gets the underlaying task that can be cancelled.
        /// </summary>
        [NotNull]
        [PublicAPI]
        Task Task { get; }

        /// <summary>
        /// Gets whether this <see cref="ICancelableTask"/> instance has completed execution due to being canceled.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the task has completed due to being canceled; otherwise <see langword="false" />.
        /// </value>
        [PublicAPI]
        bool IsCanceled { get; }

        /// <summary>
        /// Gets whether this <see cref="ICancelableTask"/> has completed.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the task has completed; otherwise <see langword="false" />.
        /// </value>
        [PublicAPI]
        bool IsCompleted { get; }

        /// <summary>
        /// Gets whether the <see cref="ICancelableTask"/> completed due to an unhandled exception.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the task has thrown an unhandled exception; otherwise <see langword="false" />.
        /// </value>
        [PublicAPI]
        bool IsFaulted { get; }

        /// <summary>
        /// Gets the <see cref="TaskStatus"/> of this task.
        /// </summary>
        /// <value>
        /// The current <see cref="TaskStatus"/> of this task instance.
        /// </value>
        [PublicAPI]
        TaskStatus Status { get; }

        /// <summary>
        /// Gets the <see cref="Exception"/> that caused the <see cref="ICancelableTask"/> to end prematurely. 
        /// If the <see cref="ICancelableTask"/> completed successfully or has not yet thrown any exceptions, this will return <see langword="null"/>.
        /// </summary>
        /// <value>
        /// The <see cref="Exception"/> that caused the <see cref="ICancelableTask"/> to end prematurely.
        /// </value>
        [PublicAPI]
        Exception Exception { get; }

        /// <summary>
        /// Cancels the task.
        /// </summary>
        [PublicAPI]
        void Cancel();

        /// <summary>
        /// Gets an awaiter used to await this <see cref="ICancelableTask"/>.
        /// </summary>
        [PublicAPI]
        INotifyCompletion GetAwaiter();

        /// <summary>
        /// Schedules a cancel operation on this <see cref="ICancelableTask"/> after the specified number of milliseconds.
        /// </summary>
        /// <param name="millisecondsDelay">The time span to wait before canceling this <see cref="T:System.Threading.CancellationTokenSource"/>.</param>
        [PublicAPI]
        void CancelAfter(int millisecondsDelay);

        /// <summary>
        /// Schedules a cancel operation on this <see cref="ICancelableTokenSource"/> after the specified time span.
        /// </summary>
        /// <param name="delay">The time span to wait before canceling this <see cref="ICancelableTokenSource"/>.</param>
        [PublicAPI]
        void CancelAfter(TimeSpan delay);

        /// <summary>
        /// Waits for the <see cref="ICancelableTask"/> to complete execution.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        [PublicAPI]
        void Wait(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Waits for the <see cref="ICancelableTask"/> to complete execution within a specified number of milliseconds.
        /// </summary>
        /// <param name="millisecondsTimeout">The number of milliseconds to wait, or <see cref="Timeout.Infinite"/> (-1) to wait indefinitely.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns></returns>
        [PublicAPI]
        bool Wait(int millisecondsTimeout, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Waits for the <see cref="ICancelableTask"/> to complete execution within a specified time interval.
        /// </summary>
        /// <param name="timeout">A <see cref="TimeSpan"/> that represents the number of milliseconds to wait, or a <see cref="TimeSpan"/> that represents -1 milliseconds to wait indefinitely.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns></returns>
        [PublicAPI]
        bool Wait(TimeSpan timeout, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Starts the <see cref="ICancelableTask"/>, scheduling it for execution to the specified <see cref="TaskScheduler"/>.
        /// </summary>
        /// <param name="scheduler">The <see cref="TaskScheduler"/> with which to associate and execute this task.</param>
        [PublicAPI]
        void Start(TaskScheduler scheduler = null);
    }
}