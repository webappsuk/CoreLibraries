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
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Threading
{
    /// <summary>
    ///   Represents the <see cref="IAsyncResult">status</see> of an asynchronous operation that has no return type.
    /// </summary>
    /// <remarks>
    ///   Based on Jeff Ritcher's wintellect threading libraries.
    /// </remarks>
    [DebuggerStepThrough]
    [PublicAPI]
    [Obsolete("Consider using TPL or Async.")]
    public class AsyncResult : IAsyncResult
    {
        private const int StateCancelled = 3;
        private const int StateCompletedAsynchronously = 2;
        private const int StateCompletedSynchronously = 1;
        private const int StatePending = 0;

        [NotNull]
        private static readonly AsyncCallback _asyncCallbackHelper =
            AsyncCallbackCompleteOpHelperNoReturnValue;

        [NotNull]
        private static readonly WaitCallback _waitCallbackHelper = WaitCallbackCompleteOpHelperNoReturnValue;

        private readonly AsyncCallback _asyncCallback;
        private readonly object _asyncState;
        private readonly object _initiatingObject;
        private volatile ManualResetEvent _asyncWaitHandle;
        private int _completedState;
        private int _eventSet;
        private Exception _exception;

        /// <summary>
        ///   Initializes a new instance of the <see cref="AsyncResult"/> class.
        /// </summary>
        /// <param name="asyncCallback">The method to execute once the operation completes.</param>
        /// <param name="state">
        ///   The object that can be obtained via the <see cref="AsyncResult.AsyncState"/> property.
        /// </param>
        public AsyncResult(AsyncCallback asyncCallback, object state)
        {
            _asyncCallback = asyncCallback;
            _asyncState = state;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="AsyncResult"/> class.
        /// </summary>
        /// <param name="asyncCallback">The method that should be executed when the operation completes.</param>
        /// <param name="state">The object that can be obtained via the AsyncState property.</param>
        /// <param name="initiatingObject">
        ///   <para>The object that is initiating the asynchronous operation.</para>
        ///   <para>This is stored in the <see cref="WebApplications.Utilities.Threading.AsyncResult.InitiatingObject"/> property.</para>
        /// </param>
        public AsyncResult(AsyncCallback asyncCallback, object state, object initiatingObject)
            : this(asyncCallback, state)
        {
            _initiatingObject = initiatingObject;
        }

        /// <summary>
        ///   Gets the <see cref="object"/> that was used to initiate the asynchronous operation.
        /// </summary>
        public object InitiatingObject
        {
            get { return _initiatingObject; }
        }

        /// <summary>
        ///   Gets a <see cref="bool"/> value that indicates whether the asynchronous operation has cancelled.
        /// </summary>
        /// <value>
        ///   Returns <see langword="true"/> if the operation is cancelled; otherwise returns <see langword="false"/>.
        /// </value>
        public bool IsCancelled
        {
            get { return (Thread.VolatileRead(ref _completedState) == StateCancelled); }
        }

        #region IAsyncResult Members
        /// <summary>
        ///   Gets a user-defined object that qualifies or contains information about an asynchronous operation.
        /// </summary>
        public object AsyncState
        {
            get { return _asyncState; }
        }

        /// <summary>
        ///   Gets a WaitHandle that is used to wait for an asynchronous operation to complete.
        /// </summary>
        /// <returns>A wait handle that is used to wait for an asynchronous operation to complete.</returns>
        [NotNull]
        public WaitHandle AsyncWaitHandle
        {
            get
            {
                if (_asyncWaitHandle == null)
                {
                    ManualResetEvent mre = new ManualResetEvent(false);
#pragma warning disable 420
                    if (Interlocked.CompareExchange(ref _asyncWaitHandle, mre, null) != null)
#pragma warning restore 420
                        mre.Close();
                    else if (IsCompleted && CallingThreadShouldSetTheEvent())
                    {
                        Debug.Assert(_asyncWaitHandle != null);
                        _asyncWaitHandle.Set();
                    }
                }
                Debug.Assert(_asyncWaitHandle != null);
                return _asyncWaitHandle;
            }
        }

        /// <summary>
        ///   Gets a <see cref="bool"/> value that indicates whether the asynchronous operation completed synchronously.
        /// </summary>
        /// <value>
        ///   Returns <see langword="true"/> if the asynchronous operation completed synchronously; otherwise returns <see langword="false"/>.
        /// </value>
        public bool CompletedSynchronously
        {
            get { return (Thread.VolatileRead(ref _completedState) == StateCompletedSynchronously); }
        }

        /// <summary>
        ///   Gets a <see cref="bool"/> value that indicates whether the asynchronous operation has completed.
        /// </summary>
        /// <value>
        ///   Returns <see langword="true"/> if the operation is complete; otherwise returns <see langword="false"/>.
        /// </value>
        /// <remarks>This includes the cancelled state which will return <see langword="true"/>.</remarks>
        public bool IsCompleted
        {
            get { return (Thread.VolatileRead(ref _completedState) != StatePending); }
        }
        #endregion

        private static void AsyncCallbackCompleteOpHelperNoReturnValue([NotNull] IAsyncResult otherAsyncResult)
        {
            // ReSharper disable once PossibleNullReferenceException
            ((AsyncResult)otherAsyncResult.AsyncState).CompleteOpHelper(otherAsyncResult);
        }

        /// <summary>
        ///   Returns the <see cref="IAsyncResult">status</see> of an operation that was queued to the thread pool.
        /// </summary>
        /// <returns>The IAsyncResult.</returns>
        protected IAsyncResult BeginInvokeOnWorkerThread()
        {
            ThreadPool.QueueUserWorkItem(_waitCallbackHelper, this);
            return this;
        }

        private bool CallingThreadShouldSetTheEvent()
        {
            return (Interlocked.Exchange(ref _eventSet, 1) == 0);
        }

        /// <summary>
        ///   A helper function used to run the callback on the completed asynchronous operation.
        /// </summary>
        /// <param name="ar">
        ///   The <see cref="IAsyncResult"/> object that represents the asynchronous operation.
        /// </param>
        /// <seealso cref="WebApplications.Utilities.Threading.AsyncResult.OnCompleteOperation"/>
        private void CompleteOpHelper(IAsyncResult ar)
        {
            Exception exception = null;
            try
            {
                OnCompleteOperation(ar);
            }
            catch (TargetInvocationException e)
            {
                exception = e.InnerException;
            }
            catch (Exception e)
            {
                exception = e;
            }
            finally
            {
                SetAsCompleted(exception);
            }
        }

        /// <summary>
        ///   Frees up the resources used by the asynchronous operation.
        ///   If the asynchronous operation failed then this method throws the exception.
        /// </summary>
        public void EndInvoke()
        {
            if (!IsCompleted ||
                (_asyncWaitHandle != null))
                AsyncWaitHandle.WaitOne();
#pragma warning disable 420
            ManualResetEvent mre = Interlocked.Exchange(ref _asyncWaitHandle, null);
#pragma warning restore 420
            if (mre != null)
                mre.Close();
            if (_exception != null)
                throw _exception;
        }

        /// <summary>
        ///   Returns a single <see langword="static"/> delegate to a <see langword="static"/> method that will invoke
        ///   the desired <see cref="AsyncCallback"/>
        /// </summary>
        /// <returns>The <see langword="static"/> delegate.</returns>
        protected static AsyncCallback GetAsyncCallbackHelper()
        {
            return _asyncCallbackHelper;
        }

        /// <summary>
        ///   Invokes the callback method when the asynchronous operations completes.
        /// </summary>
        /// <param name="result">
        ///   The <see cref="IAsyncResult"/> object identifying that the asynchronous operation that has completed.
        /// </param>

        // ReSharper disable once VirtualMemberNeverOverriden.Global
        protected virtual void OnCompleteOperation(IAsyncResult result)
        {
        }

        /// <summary>
        ///   Set the status of the asynchronous operation to completed.
        /// </summary>
        /// <param name="exception">
        ///   If non-null then this identifies the exception that occurred while processing the asynchronous operation.
        /// </param>
        /// <param name="completedSynchronously">Indicates whether the operation completed synchronously or asynchronously.</param>
        /// <exception cref="InvalidOperationException">The operation result has already been set previously.</exception>
        public void SetAsCompleted(Exception exception = null, bool completedSynchronously = false)
        {
            ExceptionDispatchInfo exceptionInfo = exception != null
                ? ExceptionDispatchInfo.Capture(exception)
                : null;

            _exception = exceptionInfo != null
                ? exceptionInfo.SourceException
                : exception;

            if (Interlocked.Exchange(
                ref _completedState,
                completedSynchronously ? StateCompletedSynchronously : StateCompletedAsynchronously) !=
                StatePending)
                throw new InvalidOperationException(Resources.AsyncResult_SetAsCompleted_CanOnlySetResultOnce);
            ManualResetEvent mre = _asyncWaitHandle;
            if ((mre != null) &&
                CallingThreadShouldSetTheEvent())
                mre.Set();
            if (_asyncCallback != null)
                _asyncCallback(this);
        }

        /// <summary>
        ///   Set the status of the asynchronous operation to cancelled.
        /// </summary>
        public void SetAsCancelled()
        {
            // Set the state to completed asynchronously, you can do this even if already completed as you
            // can cancel at any time.
            bool alreadyCompleted = Interlocked.Exchange(ref _completedState, StateCancelled) !=
                                    StatePending;
            // Set the exception
            _exception = new OperationCanceledException();
            if (alreadyCompleted)
                return;

            ManualResetEvent mre = _asyncWaitHandle;
            if ((mre != null) &&
                CallingThreadShouldSetTheEvent())
                mre.Set();
            if (_asyncCallback != null)
                _asyncCallback(this);
        }

        private static void WaitCallbackCompleteOpHelperNoReturnValue([NotNull] object o)
        {
            ((AsyncResult)o).CompleteOpHelper(null);
        }
    }

    /// <summary>
    ///   Represents the status of an asynchronous operation that has a return type of <typeparamref name="TResult" />.
    /// </summary>
    /// <typeparam name="TResult">The type of the value returned.</typeparam>
    [DebuggerStepThrough]
    [PublicAPI]
    [Obsolete("Consider using TPL or Async.")]
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class AsyncResult<TResult> : AsyncResult
    {
        // ReSharper disable StaticFieldInGenericType
        [NotNull]
        private static readonly AsyncCallback _asyncCallbackHelper =
            AsyncCallbackCompleteOpHelperWithReturnValue;

        [NotNull]
        private static readonly WaitCallback _waitCallbackHelper = WaitCallbackCompleteOpHelperWithReturnValue;
        // ReSharper restore StaticFieldInGenericType

        private TResult _result;

        /// <summary>
        ///   Initializes a new instance of the <see cref="AsyncResult&lt;TResult&gt;"/> class.
        /// </summary>
        /// <param name="asyncCallback">The method to execute once the operation completes.</param>
        /// <param name="state">The object that can be obtained via the AsyncState property.</param>
        public AsyncResult(AsyncCallback asyncCallback, object state)
            : base(asyncCallback, state)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="AsyncResult&lt;TResult&gt;"/> class.
        /// </summary>
        /// <param name="asyncCallback">The method that should be executed when the operation completes.</param>
        /// <param name="state">The object that can be obtained via the AsyncState property.</param>
        /// <param name="initiatingObject">
        ///   <para>The object that is initiating the asynchronous operation.</para>
        ///   <para>This is stored in the <see cref="WebApplications.Utilities.Threading.AsyncResult.InitiatingObject"/>property.</para>
        /// </param>
        public AsyncResult(AsyncCallback asyncCallback, object state, object initiatingObject)
            : base(asyncCallback, state, initiatingObject)
        {
        }

        private static void AsyncCallbackCompleteOpHelperWithReturnValue([NotNull] IAsyncResult otherAsyncResult)
        {
            // ReSharper disable once PossibleNullReferenceException
            ((AsyncResult<TResult>)otherAsyncResult.AsyncState).CompleteOpHelper(otherAsyncResult);
        }

        /// <summary>
        ///   Returns an <see cref="IAsyncResult"/> for an operation that was queued to the thread pool.
        /// </summary>
        /// <returns>The <see cref="IAsyncResult"/>.</returns>
        protected new IAsyncResult BeginInvokeOnWorkerThread()
        {
            ThreadPool.QueueUserWorkItem(_waitCallbackHelper, this);
            return this;
        }

        /// <summary>
        ///   A helper function used to run the callback on the completed asynchronous operation.
        /// </summary>
        /// <param name="ar">
        ///   The <see cref="IAsyncResult"/> object that represents the asynchronous operation.
        /// </param>
        /// <seealso cref="WebApplications.Utilities.Threading.AsyncResult.OnCompleteOperation"/>
        private void CompleteOpHelper(IAsyncResult ar)
        {
            TResult result = default(TResult);
            Exception exception = null;
            try
            {
                result = OnCompleteOperation(ar);
            }
            catch (Exception e)
            {
                exception = (e is TargetInvocationException) ? e.InnerException : e;
            }
            if (exception == null)
                SetAsCompleted(result);
            else
                SetAsCompleted(exception);
        }

        /// <summary>
        ///   Frees up the resources used by the asynchronous operation.
        ///   If the asynchronous operation failed then this method throws the exception.
        /// </summary>
        /// <returns>The value calculated by the asynchronous operation.</returns>
        public new TResult EndInvoke()
        {
            base.EndInvoke();
            return _result;
        }

        /// <summary>
        ///   Returns a single <see langword="static"/> delegate to a <see langword="static"/> method that will invoke
        ///   the desired <see cref="AsyncCallback"/>.
        /// </summary>
        /// <returns>The single static delegate.</returns>
        protected new static AsyncCallback GetAsyncCallbackHelper()
        {
            return _asyncCallbackHelper;
        }

        /// <summary>
        ///   Invokes the callback method when the asynchronous operations completes.
        /// </summary>
        /// <param name="result">
        ///   The object identifying the asynchronous operation that has completed.
        /// </param>
        /// <returns>The value computed by the asynchronous operation.</returns>
        protected new virtual TResult OnCompleteOperation(IAsyncResult result)
        {
            return default(TResult);
        }

        /// <summary>
        ///   Call this method to indicate that the asynchronous operation has completed.
        /// </summary>
        /// <param name="result">The value calculated by the asynchronous operation.</param>
        /// <param name="completedSynchronously">
        ///   Indicates whether the operation completed synchronously or asynchronously.
        /// </param>
        public void SetAsCompleted(TResult result, bool completedSynchronously = false)
        {
            _result = result;
            SetAsCompleted(null, completedSynchronously);
        }

        private static void WaitCallbackCompleteOpHelperWithReturnValue([NotNull] object o)
        {
            ((AsyncResult<TResult>)o).CompleteOpHelper(null);
        }
    }
}