#region © Copyright Web Applications (UK) Ltd, 2017.  All rights reserved.
// Copyright (c) 2017, Web Applications UK Ltd
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

// TODO Remove when migrated to .net standard project format
#define NET452

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Threading
{
    /// <summary>
    /// Represents the producer side of a <see cref="T:System.Threading.Tasks.Task"/> unbound to a
    /// delegate, providing access to the consumer side through the <see cref="Task"/> property.
    /// </summary>
    /// <remarks>
    /// <para>
    /// It is often the case that a <see cref="T:System.Threading.Tasks.Task"/> is desired to
    /// represent another asynchronous operation.
    /// <see cref="TaskCompletionSource">TaskCompletionSource</see> is provided for this purpose. It enables
    /// the creation of a task that can be handed out to consumers, and those consumers can use the members
    /// of the task as they would any other. However, unlike most tasks, the state of a task created by a
    /// TaskCompletionSource is controlled explicitly by the methods on TaskCompletionSource. This enables the
    /// completion of the external asynchronous operation to be propagated to the underlying Task. The
    /// separation also ensures that consumers are not able to transition the state without access to the
    /// corresponding TaskCompletionSource.
    /// </para>
    /// <para>
    /// All members of <see cref="TaskCompletionSource"/> are thread-safe
    /// and may be used from multiple threads concurrently.
    /// </para>
    /// </remarks>
    [PublicAPI]
    public class TaskCompletionSource
    {
        [NotNull]
        private readonly TaskCompletionSource<bool> _tcs;

        /// <summary>
        /// Creates a <see cref="TaskCompletionSource"/>.
        /// </summary>
        public TaskCompletionSource()
        {
            _tcs = new TaskCompletionSource<bool>();
        }

        /// <summary>
        /// Creates a <see cref="TaskCompletionSource"/>
        /// with the specified options.
        /// </summary>
        /// <remarks>
        /// The <see cref="T:System.Threading.Tasks.Task"/> created
        /// by this instance and accessible through its <see cref="Task"/> property
        /// will be instantiated using the specified <paramref name="creationOptions"/>.
        /// </remarks>
        /// <param name="creationOptions">The options to use when creating the underlying
        /// <see cref="T:System.Threading.Tasks.Task"/>.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// The <paramref name="creationOptions"/> represent options invalid for use
        /// with a <see cref="TaskCompletionSource"/>.
        /// </exception>
        public TaskCompletionSource(TaskCreationOptions creationOptions)
        {
            _tcs = new TaskCompletionSource<bool>(creationOptions);
        }

        /// <summary>
        /// Creates a <see cref="TaskCompletionSource"/>
        /// with the specified state.
        /// </summary>
        /// <param name="state">The state to use as the underlying 
        /// <see cref="T:System.Threading.Tasks.Task"/>'s AsyncState.</param>
        public TaskCompletionSource(object state)
        {
            _tcs = new TaskCompletionSource<bool>(state);
        }

        /// <summary>
        /// Creates a <see cref="TaskCompletionSource"/> with
        /// the specified state and options.
        /// </summary>
        /// <param name="creationOptions">The options to use when creating the underlying
        /// <see cref="T:System.Threading.Tasks.Task"/>.</param>
        /// <param name="state">The state to use as the underlying 
        /// <see cref="T:System.Threading.Tasks.Task"/>'s AsyncState.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// The <paramref name="creationOptions"/> represent options invalid for use
        /// with a <see cref="TaskCompletionSource"/>.
        /// </exception>
        public TaskCompletionSource(object state, TaskCreationOptions creationOptions)
        {
            _tcs = new TaskCompletionSource<bool>(state, creationOptions);
        }


        /// <summary>
        /// Gets the <see cref="T:System.Threading.Tasks.Task"/> created
        /// by this <see cref="TaskCompletionSource"/>.
        /// </summary>
        /// <remarks>
        /// This property enables a consumer access to the <see
        /// cref="T:System.Threading.Tasks.Task"/> that is controlled by this instance.
        /// The <see cref="SetCompleted"/>, <see cref="SetException(System.Exception)"/>,
        /// <see cref="SetException(System.Collections.Generic.IEnumerable{System.Exception})"/>, and <see cref="SetCanceled"/>
        /// methods (and their "Try" variants) on this instance all result in the relevant state
        /// transitions on this underlying Task.
        /// </remarks>
        [NotNull]
        public Task Task => _tcs.Task;

        /// <summary>
        /// Attempts to transition the underlying
        /// <see cref="T:System.Threading.Tasks.Task"/> into the 
        /// <see cref="System.Threading.Tasks.TaskStatus.Faulted">Faulted</see>
        /// state.
        /// </summary>
        /// <param name="exception">The exception to bind to this <see 
        /// cref="T:System.Threading.Tasks.Task"/>.</param>
        /// <returns>True if the operation was successful; otherwise, false.</returns>
        /// <remarks>This operation will return false if the 
        /// <see cref="T:System.Threading.Tasks.Task"/> is already in one
        /// of the three final states:
        /// <see cref="System.Threading.Tasks.TaskStatus.RanToCompletion">RanToCompletion</see>, 
        /// <see cref="System.Threading.Tasks.TaskStatus.Faulted">Faulted</see>, or
        /// <see cref="System.Threading.Tasks.TaskStatus.Canceled">Canceled</see>.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="exception"/> argument is null.</exception>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="Task"/> was disposed.</exception>
        public bool TrySetException([NotNull] Exception exception) => _tcs.TrySetException(exception);

        /// <summary>
        /// Attempts to transition the underlying
        /// <see cref="T:System.Threading.Tasks.Task"/> into the 
        /// <see cref="System.Threading.Tasks.TaskStatus.Faulted">Faulted</see>
        /// state.
        /// </summary>
        /// <param name="exceptions">The collection of exceptions to bind to this <see 
        /// cref="T:System.Threading.Tasks.Task"/>.</param>
        /// <returns>True if the operation was successful; otherwise, false.</returns>
        /// <remarks>This operation will return false if the 
        /// <see cref="T:System.Threading.Tasks.Task"/> is already in one
        /// of the three final states:
        /// <see cref="System.Threading.Tasks.TaskStatus.RanToCompletion">RanToCompletion</see>, 
        /// <see cref="System.Threading.Tasks.TaskStatus.Faulted">Faulted</see>, or
        /// <see cref="System.Threading.Tasks.TaskStatus.Canceled">Canceled</see>.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="exceptions"/> argument is null.</exception>
        /// <exception cref="T:System.ArgumentException">There are one or more null elements in <paramref name="exceptions"/>.</exception>
        /// <exception cref="T:System.ArgumentException">The <paramref name="exceptions"/> collection is empty.</exception>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="Task"/> was disposed.</exception>
        public bool TrySetException([NotNull] IEnumerable<Exception> exceptions) => _tcs.TrySetException(exceptions);

        /// <summary>
        /// Transitions the underlying
        /// <see cref="T:System.Threading.Tasks.Task"/> into the 
        /// <see cref="System.Threading.Tasks.TaskStatus.Faulted">Faulted</see>
        /// state.
        /// </summary>
        /// <param name="exception">The exception to bind to this <see 
        /// cref="T:System.Threading.Tasks.Task"/>.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="exception"/> argument is null.</exception>
        /// <exception cref="T:System.InvalidOperationException">
        /// The underlying <see cref="T:System.Threading.Tasks.Task"/> is already in one
        /// of the three final states:
        /// <see cref="System.Threading.Tasks.TaskStatus.RanToCompletion">RanToCompletion</see>, 
        /// <see cref="System.Threading.Tasks.TaskStatus.Faulted">Faulted</see>, or
        /// <see cref="System.Threading.Tasks.TaskStatus.Canceled">Canceled</see>.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="Task"/> was disposed.</exception>
        public void SetException([NotNull] Exception exception) => _tcs.SetException(exception);

        /// <summary>
        /// Transitions the underlying
        /// <see cref="T:System.Threading.Tasks.Task"/> into the 
        /// <see cref="System.Threading.Tasks.TaskStatus.Faulted">Faulted</see>
        /// state.
        /// </summary>
        /// <param name="exceptions">The collection of exceptions to bind to this <see 
        /// cref="T:System.Threading.Tasks.Task"/>.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="exceptions"/> argument is null.</exception>
        /// <exception cref="T:System.ArgumentException">There are one or more null elements in <paramref name="exceptions"/>.</exception>
        /// <exception cref="T:System.InvalidOperationException">
        /// The underlying <see cref="T:System.Threading.Tasks.Task"/> is already in one
        /// of the three final states:
        /// <see cref="System.Threading.Tasks.TaskStatus.RanToCompletion">RanToCompletion</see>, 
        /// <see cref="System.Threading.Tasks.TaskStatus.Faulted">Faulted</see>, or
        /// <see cref="System.Threading.Tasks.TaskStatus.Canceled">Canceled</see>.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="Task"/> was disposed.</exception>
        public void SetException([NotNull] IEnumerable<Exception> exceptions) => _tcs.SetException(exceptions);


        /// <summary>
        /// Attempts to transition the underlying
        /// <see cref="T:System.Threading.Tasks.Task"/> into the 
        /// <see cref="System.Threading.Tasks.TaskStatus.RanToCompletion">RanToCompletion</see>
        /// state.
        /// </summary>
        /// <returns>True if the operation was successful; otherwise, false.</returns>
        /// <remarks>This operation will return false if the 
        /// <see cref="T:System.Threading.Tasks.Task"/> is already in one
        /// of the three final states:
        /// <see cref="System.Threading.Tasks.TaskStatus.RanToCompletion">RanToCompletion</see>, 
        /// <see cref="System.Threading.Tasks.TaskStatus.Faulted">Faulted</see>, or
        /// <see cref="System.Threading.Tasks.TaskStatus.Canceled">Canceled</see>.
        /// </remarks>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="Task"/> was disposed.</exception>
        public bool TrySetCompleted() => _tcs.TrySetResult(false);

        /// <summary>
        /// Transitions the underlying
        /// <see cref="T:System.Threading.Tasks.Task"/> into the 
        /// <see cref="System.Threading.Tasks.TaskStatus.RanToCompletion">RanToCompletion</see>
        /// state.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">
        /// The underlying <see cref="T:System.Threading.Tasks.Task"/> is already in one
        /// of the three final states:
        /// <see cref="System.Threading.Tasks.TaskStatus.RanToCompletion">RanToCompletion</see>, 
        /// <see cref="System.Threading.Tasks.TaskStatus.Faulted">Faulted</see>, or
        /// <see cref="System.Threading.Tasks.TaskStatus.Canceled">Canceled</see>.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="Task"/> was disposed.</exception>
        public void SetCompleted() => _tcs.SetResult(false);

        /// <summary>
        /// Attempts to transition the underlying
        /// <see cref="T:System.Threading.Tasks.Task"/> into the 
        /// <see cref="System.Threading.Tasks.TaskStatus.Canceled">Canceled</see>
        /// state.
        /// </summary>
        /// <returns>True if the operation was successful; otherwise, false.</returns>
        /// <remarks>This operation will return false if the 
        /// <see cref="T:System.Threading.Tasks.Task"/> is already in one
        /// of the three final states:
        /// <see cref="System.Threading.Tasks.TaskStatus.RanToCompletion">RanToCompletion</see>, 
        /// <see cref="System.Threading.Tasks.TaskStatus.Faulted">Faulted</see>, or
        /// <see cref="System.Threading.Tasks.TaskStatus.Canceled">Canceled</see>.
        /// </remarks>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="Task"/> was disposed.</exception>
        public bool TrySetCanceled() => _tcs.TrySetCanceled();

#if !NET452
// Enables a token to be stored into the canceled task
        public bool TrySetCanceled(CancellationToken cancellationToken)
        {
            return _tcs.TrySetCanceled(cancellationToken);
        }
#endif

        /// <summary>
        /// Transitions the underlying
        /// <see cref="T:System.Threading.Tasks.Task"/> into the 
        /// <see cref="System.Threading.Tasks.TaskStatus.Canceled">Canceled</see>
        /// state.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">
        /// The underlying <see cref="T:System.Threading.Tasks.Task"/> is already in one
        /// of the three final states:
        /// <see cref="System.Threading.Tasks.TaskStatus.RanToCompletion">RanToCompletion</see>, 
        /// <see cref="System.Threading.Tasks.TaskStatus.Faulted">Faulted</see>, or
        /// <see cref="System.Threading.Tasks.TaskStatus.Canceled">Canceled</see>.
        /// </exception>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="Task"/> was disposed.</exception>
        public void SetCanceled() => _tcs.SetCanceled();
    }
}