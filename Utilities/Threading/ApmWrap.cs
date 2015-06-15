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
using System.Runtime.InteropServices;
using System.Threading;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Threading
{
    /// <summary>
    ///   A light-weight struct with the ability to associate an arbitrary piece of data of type T with any
    ///   <see cref="IAsyncResult"/> object. When the asynchronous operation completes this data can be retrieved
    ///   to complete processing.
    /// </summary>
    /// <typeparam name="T">The type of the data to embed.</typeparam>
    /// <remarks>
    ///   <para>This is typically used when you are implementing code that wraps an asynchronous operation and you
    ///   wish to add some context or state of your own to complete the wrapping.</para>
    ///   <para>Based on Jeff Ritcher's wintellect threading libraries.</para>
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    [DebuggerStepThrough]
    [PublicAPI]
    [Obsolete("Consider using TPL or Async.")]
    public struct ApmWrap<T>
    {
        /// <summary>
        ///   Returns a <see cref="bool"/> value indicating whether this instance and the specified
        ///   <see cref="T:WebApplications.Utilities.Threading.ApmWrap`1">ApmWrap</see> object are equal.
        /// </summary>
        /// <param name="value">The ApmWrap object to compare to this instance.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if <paramref name="value"/> is equal to this instance; otherwise returns <see langword="false"/>.
        /// </returns>
        [PublicAPI]
        public bool Equals(ApmWrap<T> value)
        {
            return Equals(SyncContext, value.SyncContext);
        }

        /// <summary>
        ///   Returns a <see cref="bool"/> value indicating whether this instance and the specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare to this instance.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if the value of the <paramref name="obj"/> is equal to this instance; otherwise returns <see langword="false"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return ((obj is ApmWrap<T>) && Equals((ApmWrap<T>)obj));
        }

        /// <summary>
        ///   Returns a hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code for this instance.</returns>
        /// <remarks>Suitable for use in hashing algorithms and also for data structures like a hash table.</remarks>
        /// <seealso cref="System.Object.GetHashCode"/>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        ///   <para>Implements the operator ==.</para>
        ///   <para>Returns a <see cref="bool"/> indicating whether two instances of
        ///   <see cref="T:WebApplications.Utilities.Threading.ApmWrap`1">ApmWrap</see> are equal.</para>
        /// </summary>
        /// <param name="obj1">The first ApmWrap object to compare.</param>
        /// <param name="obj2">The second ApmWrap object to compare.</param>
        public static bool operator ==(ApmWrap<T> obj1, ApmWrap<T> obj2)
        {
            return obj1.Equals(obj2);
        }

        /// <summary>
        ///   <para>Implements the operator !=.</para>
        ///   <para>Returns a <see cref="bool"/> indicating whether two instances of
        ///   <see cref="T:WebApplications.Utilities.Threading.ApmWrap`1">ApmWrap</see> are <b>not</b> equal.</para>
        /// </summary>
        /// <param name="obj1">The first ApmWrap object to compare.</param>
        /// <param name="obj2">The second ApmWrap object to compare.</param>
        public static bool operator !=(ApmWrap<T> obj1, ApmWrap<T> obj2)
        {
            return !obj1.Equals(obj2);
        }

        /// <summary>
        ///   If the SyncContext is a non-null value when creating an ApmWrap object then the ApmWrap object will force the
        ///   operation to complete using the specified <see cref="SynchronizationContext"/>.
        /// </summary>
        [PublicAPI]
        private SynchronizationContext SyncContext { get; set; }

        /// <summary>
        ///   Creates an ApmWrap object around a callback method.
        /// </summary>
        /// <param name="data">The data to embed in the ApmWrap object.</param>
        /// <param name="callback">The callback method that should be invoked when the operation completes.</param>
        /// <returns>An ApmWrap object's completion method.</returns>
        [PublicAPI]
        [NotNull]
        public AsyncCallback CreateCallback([NotNull] AsyncCallback callback, T data)
        {
            return WrapCallback(callback, data, SyncContext);
        }

        /// <summary>
        ///   Creates an ApmWrap object around a callback method.
        /// </summary>
        /// <param name="data">The data to embed.</param>
        /// <param name="callback">
        ///   <para>The callback method.</para>
        ///   <para>This is called once the asynchronous operation completes.</para>
        /// </param>
        /// <param name="syncContext">The <see cref="SynchronizationContext"/>.</param>
        /// <returns>
        ///   The internal callback stored within the created ApmWrap object.
        ///   A <see langword="null"/> is returned if the <paramref name="callback"/> is null.
        /// </returns>
        [PublicAPI]
        [NotNull]
        public static AsyncCallback WrapCallback(
            [NotNull] AsyncCallback callback,
            T data,
            SynchronizationContext syncContext = null)
        {
            if (callback == null) throw new ArgumentNullException("callback");

            ApmWrapper wrapper = new ApmWrapper
            {
                Data = data,
                AsyncCallback = callback,
                SyncContext = syncContext
            };
            return wrapper.AsyncCallbackInternal;
        }

        /// <summary>
        ///   Creates an ApmWrap object around an asynchronous operation.
        /// </summary>
        /// <param name="data">The data to embed in the ApmWrap object.</param>
        /// <param name="result">The original IAsyncResult object returned from the BeginXXX method.</param>
        /// <returns>
        ///   An ApmWrap object that contains the originally-returned <see cref="IAsyncResult"/> object.
        /// </returns>
        [PublicAPI]
        [NotNull]
        public static IAsyncResult Wrap([NotNull] IAsyncResult result, T data)
        {
            return new ApmWrapper { Data = data, AsyncResult = result };
        }

        /// <summary>
        ///   Unwraps an ApmWrap object and also retrieves the embedded data.
        /// </summary>
        /// <param name="result">
        ///   The <see langword="ref">reference</see> to the wrapped IAsyncResult object.
        /// </param>
        /// <returns>The embedded data.</returns>
        [PublicAPI]
        public static T Unwrap([NotNull] ref IAsyncResult result)
        {
            ApmWrapper apmWrap = result as ApmWrapper;
            if (apmWrap == null) throw new ArgumentException(Resources.ApmWrap_Unwrap_ResultWrongType, "result");

            Debug.Assert(apmWrap.AsyncResult != null);
            result = apmWrap.AsyncResult;
            return apmWrap.Data;
        }

        /// <summary>
        ///   Represents the actual wrapper.
        /// </summary>
        [DebuggerStepThrough]
        private sealed class ApmWrapper : IAsyncResult
        {
            /// <summary>
            ///   Initializes a new instance of the <see cref="ApmWrapper"/> class.
            /// </summary>
            [PublicAPI]
            internal ApmWrapper()
            {
            }

            /// <summary>
            ///   Gets or sets the <see cref="AsyncCallback">async callback</see>.
            /// </summary>
            /// <remarks>This is the method to call when the asynchronous operation has completed.</remarks>
            [PublicAPI]
            internal AsyncCallback AsyncCallback { get; set; }

            /// <summary>
            ///   Gets or sets the <see cref="IAsyncResult">status</see> of the asynchronous operation.
            /// </summary>
            internal IAsyncResult AsyncResult { get; set; }

            /// <summary>
            ///   Gets or sets the data to embed.
            /// </summary>
            /// <value>The data embedded in the result object.</value>
            internal T Data { get; set; }

            /// <summary>
            ///   Gets or sets the <see cref="SynchronizationContext">synchronization context</see>.
            /// </summary>
            /// <value>The synchronization context.</value>
            /// <remarks>
            ///   The synchronization context allows you to queue a unit of work to a specific context.
            /// </remarks>
            [PublicAPI]
            internal SynchronizationContext SyncContext { get; set; }

            #region IAsyncResult Members
            /// <summary>
            ///   Gets a user-defined object that qualifies or contains information about an asynchronous operation.
            /// </summary>
            /// <seealso cref="IAsyncResult.AsyncState"/>
            public object AsyncState
            {
                get { return AsyncResult == null ? null : AsyncResult.AsyncState; }
            }

            /// <summary>
            ///   Gets a <see cref="System.Threading.WaitHandle"/> which is used to wait for an asynchronous operation to complete.
            /// </summary>
            /// <returns>A wait handle that is used to wait for an asynchronous operation to complete.</returns>
            public WaitHandle AsyncWaitHandle
            {
                get { return AsyncResult == null ? null : AsyncResult.AsyncWaitHandle; }
            }

            /// <summary>
            ///   Gets a <see cref="bool"/> value indicating whether the asynchronous operation completed synchronously.
            /// </summary>
            /// <returns>
            ///   Returns <see langword="true"/> if the asynchronous operation completed synchronously; otherwise returns <see langword="false"/>.
            /// </returns>
            public bool CompletedSynchronously
            {
                get { return AsyncResult != null && AsyncResult.CompletedSynchronously; }
            }

            /// <summary>
            ///   Gets a <see cref="bool"/> value that indicates whether the asynchronous operation has completed.
            /// </summary>
            /// <value>Returns <see langword="true"/> if the operation is complete; otherwise returns <see langword="false"/>.</value>
            public bool IsCompleted
            {
                get { return AsyncResult != null && AsyncResult.IsCompleted; }
            }
            #endregion

            /// <summary>
            ///   The internal callback.
            /// </summary>
            /// <param name="result">The status of the asynchronous operation.</param>
            internal void AsyncCallbackInternal(IAsyncResult result)
            {
                AsyncResult = result;
                if (SyncContext == null)
                {
                    Debug.Assert(AsyncCallback != null);
                    AsyncCallback(this);
                }
                else
                    SyncContext.Post(PostCallback, this);
            }

            /// <summary>
            ///   Determines whether the specified <see cref="object"/> is equal to this instance.
            /// </summary>
            /// <param name="obj">The object to compare with the current instance.</param>
            /// <returns>
            ///   Returns <see langword="true"/> if the specified <see cref="object"/> is equal to this instance; otherwise returns <see langword="false"/>.
            /// </returns>
            public override bool Equals(object obj)
            {
                return Equals(AsyncResult, obj);
            }

            /// <summary>
            ///   Returns a hash code for this instance.
            /// </summary>
            /// <returns>A 32-bit signed integer hash code for this instance.</returns>
            /// <remarks>Suitable for use in hashing algorithms and also for data structures like a hash table.</remarks>
            /// <seealso cref="System.Object.GetHashCode"/>
            public override int GetHashCode()
            {
                return AsyncResult == null ? 0 : AsyncResult.GetHashCode();
            }

            /// <summary>
            ///   Posts the callback.
            /// </summary>
            /// <param name="state">The object passed to the delegate.</param>
            private static void PostCallback(object state)
            {
                ApmWrapper apmWrap = (ApmWrapper)state;
                if (apmWrap != null)
                {
                    Debug.Assert(apmWrap.AsyncCallback != null);
                    apmWrap.AsyncCallback(apmWrap);
                }
            }

            /// <summary>
            ///   Returns a <see cref="string"/> that represents this instance.
            /// </summary>
            /// <returns>A <see cref="string"/> representation of the instance.</returns>
            public override string ToString()
            {
                return AsyncResult == null ? string.Empty : AsyncResult.ToString();
            }
        }
    }
}