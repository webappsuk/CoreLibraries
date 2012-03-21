using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Threading
{
    /// <summary>
    ///   Represents the <see cref="IAsyncResult">status</see> of an asynchronous operation that has no return type.
    /// </summary>
    /// <remarks>
    ///   Based on Jeff Ritcher's wintellect threading libraries.
    /// </remarks>
    [DebuggerStepThrough]
    [UsedImplicitly]
    public class AsyncResult : IAsyncResult
    {
        private const int StateCancelled = 3;
        private const int StateCompletedAsynchronously = 2;
        private const int StateCompletedSynchronously = 1;
        private const int StatePending = 0;
        private readonly AsyncCallback _asyncCallback;
        private readonly object _asyncState;
        private volatile ManualResetEvent _asyncWaitHandle;
        private int _completedState;
        private int _eventSet;
        private Exception _exception;
        private readonly object _initiatingObject;
        [NotNull]
        private static readonly AsyncCallback _asyncCallbackHelper = AsyncCallbackCompleteOpHelperNoReturnValue;
        [NotNull]
        private static readonly WaitCallback _waitCallbackHelper = WaitCallbackCompleteOpHelperNoReturnValue;

        /// <summary>
        ///   Initializes a new instance of the <see cref="AsyncResult"/> class.
        /// </summary>
        /// <param name="asyncCallback">The method to execute once the operation completes.</param>
        /// <param name="state">
        ///   The object that can be obtained via the <see cref="AsyncResult.AsyncState"/> property.
        /// </param>
        [UsedImplicitly]
        public AsyncResult(AsyncCallback asyncCallback, object state)
        {
            this._asyncCallback = asyncCallback;
            this._asyncState = state;
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
        [UsedImplicitly]
        public AsyncResult(AsyncCallback asyncCallback, object state, object initiatingObject)
            : this(asyncCallback, state)
        {
            this._initiatingObject = initiatingObject;
        }

        private static void AsyncCallbackCompleteOpHelperNoReturnValue([NotNull]IAsyncResult otherAsyncResult)
        {
            ((AsyncResult)otherAsyncResult.AsyncState).CompleteOpHelper(otherAsyncResult);
        }

        /// <summary>
        ///   Returns the <see cref="IAsyncResult">status</see> of an operation that was queued to the thread pool.
        /// </summary>
        /// <returns>The IAsyncResult.</returns>
        [UsedImplicitly]
        protected IAsyncResult BeginInvokeOnWorkerThread()
        {
            ThreadPool.QueueUserWorkItem(_waitCallbackHelper, this);
            return this;
        }

        private bool CallingThreadShouldSetTheEvent()
        {
            return (Interlocked.Exchange(ref this._eventSet, 1) == 0);
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
                this.OnCompleteOperation(ar);
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
                this.SetAsCompleted(exception, false);
            }
        }

        /// <summary>
        ///   Frees up the resources used by the asynchronous operation.
        ///   If the asynchronous operation failed then this method throws the exception.
        /// </summary>
        [UsedImplicitly]
        public void EndInvoke()
        {
            if (!this.IsCompleted || (this._asyncWaitHandle != null))
            {
                this.AsyncWaitHandle.WaitOne();
            }
#pragma warning disable 420
            ManualResetEvent mre = Interlocked.Exchange(ref this._asyncWaitHandle, null);
#pragma warning restore 420
            if (mre != null)
            {
                mre.Close();
            }
            if (this._exception != null)
            {
                throw this._exception;
            }
        }

        /// <summary>
        ///   Returns a single <see langword="static"/> delegate to a <see langword="static"/> method that will invoke
        ///   the desired <see cref="AsyncCallback"/>
        /// </summary>
        /// <returns>The <see langword="static"/> delegate.</returns>
        [UsedImplicitly]
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
        [UsedImplicitly]
        public void SetAsCompleted(Exception exception = null, bool completedSynchronously = false)
        {
            this._exception = exception.PreserveStackTrace();
            if (Interlocked.Exchange(ref this._completedState, completedSynchronously ? StateCompletedSynchronously : StateCompletedAsynchronously) != StatePending)
            {
                throw new InvalidOperationException("You can set a result only once");
            }
            ManualResetEvent mre = this._asyncWaitHandle;
            if ((mre != null) && this.CallingThreadShouldSetTheEvent())
            {
                mre.Set();
            }
            if (this._asyncCallback != null)
            {
                this._asyncCallback(this);
            }
        }

        /// <summary>
        ///   Set the status of the asynchronous operation to cancelled.
        /// </summary>
        [UsedImplicitly]
        public void SetAsCancelled()
        {
            // Set the state to completed asynchronously, you can do this even if already completed as you
            // can cancel at any time.
            bool alreadyCompleted = Interlocked.Exchange(ref this._completedState, StateCancelled) !=
                                    StatePending;
            // Set the exception
            this._exception = new OperationCanceledException();
            if (alreadyCompleted)
                return;

            ManualResetEvent mre = this._asyncWaitHandle;
            if ((mre != null) && this.CallingThreadShouldSetTheEvent())
            {
                mre.Set();
            }
            if (this._asyncCallback != null)
            {
                this._asyncCallback(this);
            }
        }

        private static void WaitCallbackCompleteOpHelperNoReturnValue([NotNull]object o)
        {
            ((AsyncResult)o).CompleteOpHelper(null);
        }

        /// <summary>
        ///   Gets a user-defined object that qualifies or contains information about an asynchronous operation.
        /// </summary>
        public object AsyncState
        {
            get
            {
                return this._asyncState;
            }
        }

        /// <summary>
        ///   Gets a WaitHandle that is used to wait for an asynchronous operation to complete.
        /// </summary>
        /// <returns>A wait handle that is used to wait for an asynchronous operation to complete.</returns>
        public WaitHandle AsyncWaitHandle
        {
            get
            {
                if (this._asyncWaitHandle == null)
                {
                    ManualResetEvent mre = new ManualResetEvent(false);
#pragma warning disable 420
                    if (Interlocked.CompareExchange(ref this._asyncWaitHandle, mre, null) != null)
#pragma warning restore 420
                    {
                        mre.Close();
                    }
                    else if (this.IsCompleted && this.CallingThreadShouldSetTheEvent())
                    {
                        this._asyncWaitHandle.Set();
                    }
                }
                return this._asyncWaitHandle;
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
            get
            {
                return (Thread.VolatileRead(ref this._completedState) == StateCompletedSynchronously);
            }
        }

        /// <summary>
        ///   Gets the <see cref="object"/> that was used to initiate the asynchronous operation.
        /// </summary>
        [UsedImplicitly]
        public object InitiatingObject
        {
            get
            {
                return this._initiatingObject;
            }
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
            get
            {
                return (Thread.VolatileRead(ref this._completedState) != StatePending);
            }
        }

        /// <summary>
        ///   Gets a <see cref="bool"/> value that indicates whether the asynchronous operation has cancelled.
        /// </summary>
        /// <value>
        ///   Returns <see langword="true"/> if the operation is cancelled; otherwise returns <see langword="false"/>.
        /// </value>
        public bool IsCancelled
        {
            get
            {
                return (Thread.VolatileRead(ref this._completedState) == StateCancelled);
            }
        }
    }

    /// <summary>
    ///   Represents the status of an asynchronous operation that has a return type of <typeparamref name="TResult" />.
    /// </summary>
    /// <typeparam name="TResult">The type of the value returned.</typeparam>
    [DebuggerStepThrough]
    [UsedImplicitly]
    public class AsyncResult<TResult> : AsyncResult
    {
        private TResult _result;
        // ReSharper disable StaticFieldInGenericType
        [NotNull]
        private static readonly AsyncCallback _asyncCallbackHelper = AsyncCallbackCompleteOpHelperWithReturnValue;
        [NotNull]
        private static readonly WaitCallback _waitCallbackHelper = WaitCallbackCompleteOpHelperWithReturnValue;
        // ReSharper restore StaticFieldInGenericType

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

        private static void AsyncCallbackCompleteOpHelperWithReturnValue([NotNull]IAsyncResult otherAsyncResult)
        {
            ((AsyncResult<TResult>)otherAsyncResult.AsyncState).CompleteOpHelper(otherAsyncResult);
        }

        /// <summary>
        ///   Returns an <see cref="IAsyncResult"/> for an operation that was queued to the thread pool.
        /// </summary>
        /// <returns>The <see cref="IAsyncResult"/>.</returns>
        [UsedImplicitly]protected new IAsyncResult BeginInvokeOnWorkerThread()
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
                result = this.OnCompleteOperation(ar);
            }
            catch (Exception e)
            {
                exception = (e is TargetInvocationException) ? e.InnerException : e;
            }
            if (exception == null)
            {
                this.SetAsCompleted(result, false);
            }
            else
            {
                SetAsCompleted(exception, false);
            }
        }

        /// <summary>
        ///   Frees up the resources used by the asynchronous operation.
        ///   If the asynchronous operation failed then this method throws the exception.
        /// </summary>
        /// <returns>The value calculated by the asynchronous operation.</returns>
        [UsedImplicitly]
        public new TResult EndInvoke()
        {
            base.EndInvoke();
            return this._result;
        }

        /// <summary>
        ///   Returns a single <see langword="static"/> delegate to a <see langword="static"/> method that will invoke
        ///   the desired <see cref="AsyncCallback"/>.
        /// </summary>
        /// <returns>The single static delegate.</returns>
        [UsedImplicitly]
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
        [UsedImplicitly]
        public void SetAsCompleted(TResult result, bool completedSynchronously = false)
        {
            this._result = result;
            SetAsCompleted(null, completedSynchronously);
        }

        private static void WaitCallbackCompleteOpHelperWithReturnValue([NotNull]object o)
        {
            ((AsyncResult<TResult>)o).CompleteOpHelper(null);
        }
    }
}
