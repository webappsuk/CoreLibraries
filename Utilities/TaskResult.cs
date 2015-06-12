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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities
{
    /// <summary>
    /// Usefull completed Tasks.
    /// </summary>
    public static class TaskResult
    {
        /// <summary>
        /// The completed result
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly Task Completed;

        /// <summary>
        /// A task that returns a <see langword="true"/>
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly Task<bool> True;

        /// <summary>
        /// A task that returns a <see langword="false"/>
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly Task<bool> False;

        /// <summary>
        /// A task that returns a <c>0</c>
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly Task<int> Zero;

        /// <summary>
        /// A task that returns a <c>-1</c>
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly Task<int> MinusOne;

        /// <summary>
        /// A task that returns a <c>1</c>
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly Task<int> One;

        /// <summary>
        /// A task that returns <see cref="System.Int32.MinValue"/>
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly Task<int> MinInt;

        /// <summary>
        /// A task that returns <see cref="System.Int32.MaxValue"/>
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly Task<int> MaxInt;

        /// <summary>
        /// The cancelled result
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly Task Cancelled;

        /// <summary>
        /// The cancelled token.
        /// </summary>
        [PublicAPI]
        public static readonly CancellationToken CancelledToken;

        /// <summary>
        /// Initializes static members of the <see cref="TaskResult"/> class.
        /// </summary>
        static TaskResult()
        {
            // ReSharper disable AssignNullToNotNullAttribute
            True = Task.FromResult(true);
            Completed = True;
            False = Task.FromResult(false);
            Zero = Task.FromResult(0);
            One = Task.FromResult(1);
            MinusOne = Task.FromResult(-1);
            MinInt = Task.FromResult(int.MinValue);
            MaxInt = Task.FromResult(int.MaxValue);
            Cancelled = TaskResult<object>.Cancelled;
            // ReSharper restore AssignNullToNotNullAttribute

            CancelledToken = new CancellationToken(true);
        }

        /// <summary>
        /// Creates a <see cref="Task"/> from the <paramref name="exception"/>.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns></returns>
        [NotNull]
        [PublicAPI]
        public static Task FromException([NotNull] Exception exception)
        {
            if (exception == null) throw new ArgumentNullException("exception");
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            tcs.SetException(exception);
            // ReSharper disable once AssignNullToNotNullAttribute
            return tcs.Task;
        }

        /// <summary>
        /// Creates a <see cref="Task"/> from the <paramref name="exceptions"/>.
        /// </summary>
        /// <param name="exceptions">The exceptions.</param>
        /// <returns></returns>
        [NotNull]
        [PublicAPI]
        public static Task FromException([NotNull] IEnumerable<Exception> exceptions)
        {
            if (exceptions == null) throw new ArgumentNullException("exceptions");
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            tcs.SetException(exceptions);
            // ReSharper disable once AssignNullToNotNullAttribute
            return tcs.Task;
        }
    }

    /// <summary>
    /// Usefull completed Tasks.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class TaskResult<T>
    {
        /// <summary>
        /// A task that returns the <see langword="default"/> value for the type <typeparamref name="T"/>.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly Task<T> Default;

        /// <summary>
        /// The cancelled result
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static readonly Task<T> Cancelled;

        /// <summary>
        /// Initializes static members of the <see cref="TaskResult{T}"/> class.
        /// </summary>
        static TaskResult()
        {
            TaskCompletionSource<T> cancelledSource = new TaskCompletionSource<T>();
            cancelledSource.SetCanceled();

            // ReSharper disable AssignNullToNotNullAttribute
            Default = Task.FromResult(default(T));
            Cancelled = cancelledSource.Task;
            // ReSharper restore AssignNullToNotNullAttribute
        }

        /// <summary>
        /// Creates a <see cref="Task"/> from the <paramref name="exception"/>.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns></returns>
        [NotNull]
        [PublicAPI]
        public static Task<T> FromException([NotNull] Exception exception)
        {
            if (exception == null) throw new ArgumentNullException("exception");
            TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();
            tcs.SetException(exception);
            // ReSharper disable once AssignNullToNotNullAttribute
            return tcs.Task;
        }

        /// <summary>
        /// Creates a <see cref="Task"/> from the <paramref name="exceptions"/>.
        /// </summary>
        /// <param name="exceptions">The exceptions.</param>
        /// <returns></returns>
        [NotNull]
        [PublicAPI]
        public static Task<T> FromException([NotNull] IEnumerable<Exception> exceptions)
        {
            if (exceptions == null) throw new ArgumentNullException("exceptions");
            TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();
            tcs.SetException(exceptions);
            // ReSharper disable once AssignNullToNotNullAttribute
            return tcs.Task;
        }
    }
}