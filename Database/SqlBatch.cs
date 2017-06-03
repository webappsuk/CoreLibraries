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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using NodaTime;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Database.Exceptions;
using WebApplications.Utilities.Database.Schema;
using WebApplications.Utilities.Logging;
using WebApplications.Utilities.Threading;

namespace WebApplications.Utilities.Database
{
    /// <summary>
    /// Delegate to a method for handling an exception.
    /// </summary>
    /// <param name="exception">The exception to handle.</param>
    /// <returns><see langword="true"/> if the exception is handled, <see langword="false"/> to propagate to parent handlers.</returns>
    public delegate bool ExceptionHandler(Exception exception);

    /// <summary>
    /// Allows multiple <see cref="SqlProgram">SqlPrograms</see> to be executed in a single database call.
    /// </summary>
    public partial class SqlBatch : IEnumerable<SqlBatchCommand>, IBatchItem
    {
        /// <summary>
        /// The additional time given to a command before a cancellation is triggered.
        /// </summary>
        /// <remarks>
        /// <para>All batches will be cleaned up if the <see cref="BatchTimeout"/> is greater
        /// than <see cref="Duration.Zero"/> and the <see cref="BatchTimeout"/> plus the
        /// <see cref="AdditionalCancellationTime"/> has elapsed.</para>
        /// <para>This ensures that resources don't leak over time, by badly written consumers.</para>
        /// </remarks>
        public static readonly Duration AdditionalCancellationTime = Duration.FromSeconds(1);

        /// <summary>
        /// Holds the state of a batch
        /// </summary>
        /// <seealso cref="System.IDisposable" />
        private sealed class State : IDisposable
        {
            /// <summary>
            /// The batch the state is for.
            /// </summary>
            [NotNull]
            public readonly SqlBatch Batch;
            /// <summary>
            /// The execute lock
            /// </summary>
            [NotNull]
            public readonly AsyncLock ExecuteLock = new AsyncLock();
            /// <summary>
            /// The value of the state.
            /// </summary>
            public int Value = Constants.BatchState.Building;
            /// <summary>
            /// The program count.
            /// </summary>
            public int CommandCount;

            /// <summary>
            /// Initializes a new instance of the <see cref="State"/> class.
            /// </summary>
            /// <param name="batch">The batch.</param>
            public State([NotNull] SqlBatch batch)
            {
                Batch = batch;
            }

            /// <summary>
            /// Checks the state is valid for adding to the batch.
            /// </summary>
            public void CheckBuildingState()
            {
                if (Value != Constants.BatchState.Building)
                {
                    throw new InvalidOperationException(
                        Value == Constants.BatchState.Executing
                            ? Resources.SqlBatch_CheckState_Executing
                            : Resources.SqlBatch_CheckState_Completed);
                }
            }

            /// <summary>Releases the lock held on this state.</summary>
            public void Dispose() => Monitor.Exit(this);
        }

        /// <summary>
        /// The states of two batches.
        /// </summary>
        /// <seealso cref="System.IDisposable" />
        private struct States : IDisposable
        {
            public State StateA;
            public State StateB;

            /// <summary>
            /// Releases the locks held on the states
            /// </summary>
            public void Dispose()
            {
                try
                {
                    try
                    {
                    }
                    finally
                    {
                        State stateB = Interlocked.Exchange(ref StateB, null);
                        if (stateB != null) Monitor.Exit(stateB);
                    }
                }
                finally
                {
                    State stateA = Interlocked.Exchange(ref StateA, null);
                    if (stateA != null) Monitor.Exit(StateA);
                }
            }
        }

        /// <summary>
        /// Gets the state.
        /// </summary>
        [NotNull]
        private State GetState()
        {
            bool hasLock = false;
            State state = _state;
            Monitor.Enter(state, ref hasLock);
            try
            {
                while (state.Batch._parent != null)
                {
                    Monitor.Exit(Interlocked.Exchange(ref state, state.Batch._parent._state));
                    hasLock = false;
                    Monitor.Enter(state, ref hasLock);
                }
                Debug.Assert(hasLock);
                _state = state;
                return state;
            }
            catch
            {
                if (hasLock)
                    Monitor.Exit(state);
                throw;
            }
        }

        /// <summary>
        /// Gets the states of two batches.
        /// </summary>
        /// <param name="batchA">The batch a.</param>
        /// <param name="batchB">The batch b.</param>
        /// <returns></returns>
        private static States GetStates([NotNull] SqlBatch batchA, [NotNull] SqlBatch batchB)
        {
            States states = new States();
            try
            {
                if (batchA.ID.CompareTo(batchB.ID) < 0)
                {
                    states.StateA = batchA.GetState();
                    states.StateB = batchB.GetState();
                }
                else
                {
                    states.StateB = batchB.GetState();
                    states.StateA = batchA.GetState();
                }
                return states;
            }
            catch
            {
                states.Dispose();
                throw;
            }
        }

        /// <summary>
        /// The state of the batch.
        /// </summary>
        [NotNull]
        private State _state;

        /// <summary>
        /// The parent batch.
        /// </summary>
        private SqlBatch _parent;

        /// <summary>
        /// The items within this batch.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        private readonly List<IBatchItem> _items = new List<IBatchItem>();

        /// <summary>
        /// The batch timeout
        /// </summary>
        private Duration _batchTimeout;

        /// <summary>
        /// The type of transaction to use.
        /// </summary>
        private readonly TransactionType _transaction;

        /// <summary>
        /// The isolation level for the transaction.
        /// </summary>
        private readonly IsolationLevel _isolationLevel;

        /// <summary>
        /// If <see langword="true" /> then any errors that occur within this batch wont cause an exception to be thrown for the whole batch.
        /// The program that failed will still throw an exception.
        /// </summary>
        private readonly bool _suppressErrors;

        /// <summary>
        /// The exception handler for any errors that occur in the database.
        /// Only used if there is a transaction or errors are suppressed.
        /// </summary>
        private readonly ExceptionHandler _exceptionHandler;

        /// <summary>
        /// The batch result. Used when this is not the root batch to know when this batch is completed.
        /// </summary>
        [NotNull]
        private readonly SqlBatchResult<bool> _batchResult = new SqlBatchResult<bool>();
        
        /// <summary>
        /// Gets the result for the batch item.
        /// </summary>
        SqlBatchResult IBatchItem.Result => _batchResult;

        /// <summary>
        /// Gets the transaction for this item.
        /// </summary>
        TransactionType IBatchItem.Transaction => _transaction;

        /// <summary>
        /// Gets the isolation level of the transaction for this item.
        /// </summary>
        IsolationLevel IBatchItem.IsolationLevel => _isolationLevel;

        /// <summary>
        /// Gets the name of the transaction, if there is one.
        /// </summary>
        string IBatchItem.TransactionName => ID.ToString("N");

        /// <summary>
        /// Gets a value indicating whether errors are suppressed in the batch for this item.
        /// </summary>
        /// <value>
        ///   <see langword="true" /> if errors should be suppressed; otherwise, <see langword="false" />.
        /// </value>
        bool IBatchItem.SuppressErrors => _suppressErrors;

        /// <summary>
        /// Gets the exception handler for this item.
        /// </summary>
        /// <value>
        /// The exception handler.
        /// </value>
        ExceptionHandler IBatchItem.ExceptionHandler => _exceptionHandler;

#if DEBUG
        /// <summary>
        /// The SQL for the batch. For debugging purposes.
        /// </summary>
        [UsedImplicitly]
        private string _sql;
#endif

        /// <summary>
        /// Creates a new batch.
        /// </summary>
        /// <param name="suppressErrors">if set to <see langword="true" /> any errors that occur within this batch
        /// wont cause an exception to be thrown for the whole batch, unless an <paramref name="exceptionHandler"/> 
        /// is specified and doesn't suppress the error. The program that failed will still throw an exception.</param>
        /// <param name="exceptionHandler">The optional exception handler.</param>
        /// <param name="batchTimeout">The batch timeout. Defaults to 30 seconds.</param>
        /// <returns>The new <see cref="SqlBatch"/>.</returns>
        [NotNull]
        public static SqlBatch Create(
            bool suppressErrors = false,
            ExceptionHandler exceptionHandler = null,
            Duration? batchTimeout = null)
        {
            return new SqlBatch(batchTimeout, suppressErrors: suppressErrors, exceptionHandler: exceptionHandler);
        }

        /// <summary>
        /// Creates a new batch which is wrapped in a transaction.
        /// If an error occurs within the batch, the transaction will rollback.
        /// </summary>
        /// <param name="isolationLevel">The isolation level of the transaction.</param>
        /// <param name="rollback">if set to <see langword="true" /> the transaction will always be rolled back.</param>
        /// <param name="suppressErrors">if set to <see langword="true" /> any errors that occur within this batch
        /// wont cause an exception to be thrown for the whole batch, unless an <paramref name="exceptionHandler"/> 
        /// is specified and doesn't suppress the error. The program that failed will still throw an exception.</param>
        /// <param name="exceptionHandler">The optional exception handler.</param>
        /// <param name="batchTimeout">The batch timeout. Defaults to 30 seconds.</param>
        /// <returns>
        /// The new <see cref="SqlBatch" />.
        /// </returns>
        [NotNull]
        public static SqlBatch CreateTransaction(
            IsolationLevel isolationLevel,
            bool rollback = false,
            bool suppressErrors = false,
            ExceptionHandler exceptionHandler = null,
            Duration? batchTimeout = null)
        {
            return new SqlBatch(
                batchTimeout,
                rollback ? TransactionType.Rollback : TransactionType.Commit,
                isolationLevel,
                suppressErrors,
                exceptionHandler);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlBatch" /> class.
        /// </summary>
        /// <param name="batchTimeout">The batch timeout. Defaults to 30 seconds.</param>
        /// <param name="transaction">The transaction.</param>
        /// <param name="isolationLevel">The isolation level.</param>
        /// <param name="suppressErrors">if set to <see langword="true" /> errors are suppressed.</param>
        /// <param name="exceptionHandler">The exception handler.</param>
        private SqlBatch(
            Duration? batchTimeout,
            TransactionType transaction = TransactionType.None,
            IsolationLevel isolationLevel = IsolationLevel.Unspecified,
            bool suppressErrors = false,
            ExceptionHandler exceptionHandler = null)
        {
            BatchTimeout = batchTimeout ?? Duration.FromSeconds(30);
            _state = new State(this);
            _transaction = transaction;
            _isolationLevel = isolationLevel;
            _suppressErrors = suppressErrors;
            _exceptionHandler = exceptionHandler;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlBatch" /> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="transaction">The transaction.</param>
        /// <param name="isolationLevel">The isolation level.</param>
        /// <param name="suppressErrors">if set to <see langword="true" /> errors are suppressed.</param>
        /// <param name="exceptionHandler">The exception handler.</param>
        private SqlBatch(
            [NotNull] SqlBatch parent,
            TransactionType transaction = TransactionType.None,
            IsolationLevel isolationLevel = IsolationLevel.Unspecified,
            bool suppressErrors = false,
            ExceptionHandler exceptionHandler = null)
        {
            BatchTimeout = parent._batchTimeout;
            _state = parent._state;
            _parent = parent;
            _transaction = transaction;
            _isolationLevel = isolationLevel;
            _suppressErrors = suppressErrors;
            _exceptionHandler = exceptionHandler;
        }

        /// <summary>
        /// Checks the state is valid for adding to the batch, without requiring a lock.
        /// </summary>
        private void CheckBuildingStateQuick()
        {
            State state = _state;
            if (state.Batch._parent == null)
                state.CheckBuildingState();
        }

        /// <summary>
        /// Gets the identifier of the batch.
        /// </summary>
        /// <value>The identifier.</value>
        public Guid ID { get; } = Guid.NewGuid();

        /// <summary>
        /// Gets a value indicating whether this is a root batch.
        /// </summary>
        /// <value>
        ///   <see langword="true" /> if this instance is root; otherwise, <see langword="false" />.
        /// </value>
        public bool IsRoot => _parent == null;

        /// <summary>
        /// Gets the batch that owns this batch.
        /// </summary>
        public SqlBatch Owner => _parent;

        /// <summary>
        ///   Gets or sets the batch timeout.
        ///   This is the time to wait for the batch to execute.
        /// </summary>
        /// <remarks>
        /// Set to <see cref="Duration.Zero"/> or <see cref="TimeHelpers.InfiniteDuration"/> to indicate no limit.
        /// </remarks>
        /// <value>
        ///   The time to wait for the batch to execute.
        /// </value>
        public Duration BatchTimeout
        {
            get => _batchTimeout;
            set
            {
                if (_batchTimeout == value)
                    return;
                if (value == TimeHelpers.InfiniteDuration)
                    _batchTimeout = Duration.Zero;
                else if (value < Duration.Zero)
                    throw new ArgumentOutOfRangeException(nameof(value));
                else
                    _batchTimeout = value;
            }
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        public IEnumerator<SqlBatchCommand> GetEnumerator()
        {
            foreach (IBatchItem item in EnumerateItems())
                if (item is SqlBatchCommand command)
                    yield return command;
        }

        /// <summary>
        /// Enumerates the items in this batch and all child batches.
        /// </summary>
        /// <returns></returns>
        [NotNull]
        [ItemNotNull]
        private IEnumerable<IBatchItem> EnumerateItems()
        {
            yield return this;

            Stack<List<IBatchItem>.Enumerator> stack = new Stack<List<IBatchItem>.Enumerator>();
            stack.Push(_items.GetEnumerator());

            while (stack.TryPop(out List<IBatchItem>.Enumerator enumerator))
            {
                while (enumerator.MoveNext())
                {
                    IBatchItem item = enumerator.Current;

                    yield return item;

                    if (item is SqlBatch batch)
                    {
                        stack.Push(enumerator);
                        stack.Push(batch._items.GetEnumerator());
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Adds the command given to the batch.
        /// </summary>
        /// <param name="command">The program.</param>
        private void AddCommand([NotNull] SqlBatchCommand command)
        {
            using (State state = GetState())
            {
                state.CheckBuildingState();

                if (state.CommandCount >= ushort.MaxValue || 
                    Interlocked.Increment(ref state.CommandCount) > ushort.MaxValue)
                    throw new InvalidOperationException(Resources.SqlBatch_AddCommand_OnlyAllowed65536);
                
                _items.Add(command);
            }
        }

        /// <summary>
        /// Adds the specified program to the batch.
        /// The first column of the first row in the result set returned by the query will be returned from the <see cref="SqlBatchResult{T}" />.
        /// Additional columns or rows are ignored.
        /// </summary>
        /// <typeparam name="TOut">The output type expected.</typeparam>
        /// <param name="program">The program to add to the batch.</param>
        /// <param name="result">A <see cref="SqlBatchResult{T}"/> which can be used to get the scalar value returned by the program.</param>
        /// <param name="setParameters">An optional method for setting the parameters to pass to the program.</param>
        /// <param name="suppressErrors">if set to <see langword="true" /> any errors that occur within this program
        /// wont cause an exception to be thrown for the whole batch, unless an <paramref name="exceptionHandler"/> 
        /// is specified and doesn't suppress the error. The program itself will still throw an exception.</param>
        /// <param name="exceptionHandler">The optional exception handler.</param>
        /// <returns>This <see cref="SqlBatch"/> instance.</returns>
        [NotNull]
        public SqlBatch AddExecuteScalar<TOut>(
            [NotNull] SqlProgram program,
            [NotNull] out SqlBatchResult<TOut> result,
            [CanBeNull] SetParametersDelegate setParameters = null,
            bool suppressErrors = false,
            ExceptionHandler exceptionHandler = null)
        {
            if (program == null) throw new ArgumentNullException(nameof(program));
            CheckBuildingStateQuick();

            SqlBatchCommand.Scalar<TOut> command = new SqlBatchCommand.Scalar<TOut>(
                this,
                program,
                setParameters,
                exceptionHandler,
                suppressErrors);
            AddCommand(command);
            result = command.Result;

            return this;
        }

        /// <summary>
        /// Adds the specified program to the batch. 
        /// The number of records affected by the program will be returned from the <see cref="SqlBatchResult{T}"/>.
        /// </summary>
        /// <param name="program">The program to add to the batch.</param>
        /// <param name="setParameters">An optional method for setting the parameters to pass to the program.</param>
        /// <param name="result">A <see cref="SqlBatchResult{T}" /> which can be used to get the number of records affected by the program.</param>
        /// <param name="suppressErrors">if set to <see langword="true" /> any errors that occur within this program
        /// wont cause an exception to be thrown for the whole batch, unless an <paramref name="exceptionHandler"/> 
        /// is specified and doesn't suppress the error. The program itself will still throw an exception.</param>
        /// <param name="exceptionHandler">The optional exception handler.</param>
        /// <returns>This <see cref="SqlBatch"/> instance.</returns>
        [NotNull]
        public SqlBatch AddExecuteNonQuery(
            [NotNull] SqlProgram program,
            [NotNull] out SqlBatchResult<int> result,
            [CanBeNull] SetParametersDelegate setParameters = null,
            bool suppressErrors = false,
            ExceptionHandler exceptionHandler = null)
        {
            if (program == null) throw new ArgumentNullException(nameof(program));
            CheckBuildingStateQuick();

            SqlBatchCommand.NonQuery command = new SqlBatchCommand.NonQuery(
                this,
                program,
                setParameters,
                exceptionHandler,
                suppressErrors);
            AddCommand(command);
            result = command.Result;

            return this;
        }

        /// <summary>
        /// Adds the specified program to the batch.
        /// </summary>
        /// <param name="program">The program to add to the batch.</param>
        /// <param name="resultAction">The action used to process the result.</param>
        /// <param name="behavior">The query's effect on the database.</param>
        /// <param name="setParameters">An optional method for setting the parameters to pass to the program.</param>
        /// <param name="result">A <see cref="SqlBatchResult" /> which can be used to wait for the program to finish executing.</param>
        /// <param name="suppressErrors">if set to <see langword="true" /> any errors that occur within this program
        /// wont cause an exception to be thrown for the whole batch, unless an <paramref name="exceptionHandler"/> 
        /// is specified and doesn't suppress the error. The program itself will still throw an exception.</param>
        /// <param name="exceptionHandler">The optional exception handler.</param>
        /// <returns>This <see cref="SqlBatch"/> instance.</returns>
        [NotNull]
        public SqlBatch AddExecuteReader(
            [NotNull] SqlProgram program,
            [NotNull] ResultDelegateAsync resultAction,
            [NotNull] out SqlBatchResult result,
            CommandBehavior behavior = CommandBehavior.Default,
            [CanBeNull] SetParametersDelegate setParameters = null,
            bool suppressErrors = false,
            ExceptionHandler exceptionHandler = null)
        {
            if (program == null) throw new ArgumentNullException(nameof(program));
            if (resultAction == null) throw new ArgumentNullException(nameof(resultAction));
            if ((behavior & CommandBehavior.CloseConnection) != 0)
                throw new ArgumentOutOfRangeException(
                    nameof(behavior),
                    "CommandBehavior.CloseConnection is not supported");
            CheckBuildingStateQuick();

            SqlBatchCommand.Reader command = new SqlBatchCommand.Reader(
                this,
                program,
                resultAction,
                behavior,
                setParameters,
                exceptionHandler,
                suppressErrors);
            AddCommand(command);
            result = command.Result;

            return this;
        }

        /// <summary>
        /// Adds the specified program to the batch. 
        /// The value returned by the <paramref name="resultFunc"/> will be returned by the <see cref="SqlBatchResult{T}"/>.
        /// </summary>
        /// <typeparam name="TOut">The type of the result.</typeparam>
        /// <param name="program">The program to add to the batch.</param>
        /// <param name="resultFunc">The function used to process the result.</param>
        /// <param name="behavior">The query's effect on the database.</param>
        /// <param name="setParameters">An optional method for setting the parameters to pass to the program.</param>
        /// <param name="result">A <see cref="SqlBatchResult{T}" /> which can be used to wait for the program to finish executing
        /// and get the value returned by <paramref name="resultFunc"/>.</param>
        /// <param name="suppressErrors">if set to <see langword="true" /> any errors that occur within this program
        /// wont cause an exception to be thrown for the whole batch, unless an <paramref name="exceptionHandler"/> 
        /// is specified and doesn't suppress the error. The program itself will still throw an exception.</param>
        /// <param name="exceptionHandler">The optional exception handler.</param>
        /// <returns>This <see cref="SqlBatch"/> instance.</returns>
        [NotNull]
        public SqlBatch AddExecuteReader<TOut>(
            [NotNull] SqlProgram program,
            [NotNull] ResultDelegateAsync<TOut> resultFunc,
            [NotNull] out SqlBatchResult<TOut> result,
            CommandBehavior behavior = CommandBehavior.Default,
            [CanBeNull] SetParametersDelegate setParameters = null,
            bool suppressErrors = false,
            ExceptionHandler exceptionHandler = null)
        {
            if (program == null) throw new ArgumentNullException(nameof(program));
            if (resultFunc == null) throw new ArgumentNullException(nameof(resultFunc));
            if ((behavior & CommandBehavior.CloseConnection) != 0)
                throw new ArgumentOutOfRangeException(
                    nameof(behavior),
                    "CommandBehavior.CloseConnection is not supported");
            CheckBuildingStateQuick();

            SqlBatchCommand.Reader<TOut> command = new SqlBatchCommand.Reader<TOut>(
                this,
                program,
                resultFunc,
                behavior,
                setParameters,
                exceptionHandler,
                suppressErrors);
            AddCommand(command);
            result = command.Result;

            return this;
        }

        /// <summary>
        /// Adds the specified program to the batch.
        /// </summary>
        /// <param name="program">The program to add to the batch.</param>
        /// <param name="resultAction">The action used to process the result.</param>
        /// <param name="behavior">The query's effect on the database.</param>
        /// <param name="setParameters">An optional method for setting the parameters to pass to the program.</param>
        /// <param name="result">A <see cref="SqlBatchResult" /> which can be used to wait for the program to finish executing.</param>
        /// <param name="suppressErrors">if set to <see langword="true" /> any errors that occur within this program
        /// wont cause an exception to be thrown for the whole batch, unless an <paramref name="exceptionHandler"/> 
        /// is specified and doesn't suppress the error. The program itself will still throw an exception.</param>
        /// <param name="exceptionHandler">The optional exception handler.</param>
        /// <returns>This <see cref="SqlBatch"/> instance.</returns>
        [NotNull]
        public SqlBatch AddExecuteReader(
            [NotNull] SqlProgram program,
            [NotNull] ResultDisposableDelegateAsync resultAction,
            [NotNull] out SqlBatchResult result,
            CommandBehavior behavior = CommandBehavior.Default,
            [CanBeNull] SetParametersDelegate setParameters = null,
            bool suppressErrors = false,
            ExceptionHandler exceptionHandler = null)
        {
            if (program == null) throw new ArgumentNullException(nameof(program));
            if (resultAction == null) throw new ArgumentNullException(nameof(resultAction));
            if ((behavior & CommandBehavior.CloseConnection) != 0)
                throw new ArgumentOutOfRangeException(
                    nameof(behavior),
                    "CommandBehavior.CloseConnection is not supported");
            CheckBuildingStateQuick();

            SqlBatchCommand.ReaderDisposable command = new SqlBatchCommand.ReaderDisposable(
                this,
                program,
                resultAction,
                behavior,
                setParameters,
                exceptionHandler,
                suppressErrors);
            AddCommand(command);
            result = command.Result;

            return this;
        }

        /// <summary>
        /// Adds the specified program to the batch. 
        /// The value returned by the <paramref name="resultFunc"/> will be returned by the <see cref="SqlBatchResult{T}"/>.
        /// </summary>
        /// <typeparam name="TOut">The type of the result.</typeparam>
        /// <param name="program">The program to add to the batch.</param>
        /// <param name="resultFunc">The function used to process the result.</param>
        /// <param name="behavior">The query's effect on the database.</param>
        /// <param name="setParameters">An optional method for setting the parameters to pass to the program.</param>
        /// <param name="result">A <see cref="SqlBatchResult{T}" /> which can be used to wait for the program to finish executing
        /// and get the value returned by <paramref name="resultFunc"/>.</param>
        /// <param name="suppressErrors">if set to <see langword="true" /> any errors that occur within this program
        /// wont cause an exception to be thrown for the whole batch, unless an <paramref name="exceptionHandler"/> 
        /// is specified and doesn't suppress the error. The program itself will still throw an exception.</param>
        /// <param name="exceptionHandler">The optional exception handler.</param>
        /// <returns>This <see cref="SqlBatch"/> instance.</returns>
        [NotNull]
        public SqlBatch AddExecuteReader<TOut>(
            [NotNull] SqlProgram program,
            [NotNull] ResultDisposableDelegateAsync<TOut> resultFunc,
            [NotNull] out SqlBatchResult<TOut> result,
            CommandBehavior behavior = CommandBehavior.Default,
            [CanBeNull] SetParametersDelegate setParameters = null,
            bool suppressErrors = false,
            ExceptionHandler exceptionHandler = null)
        {
            if (program == null) throw new ArgumentNullException(nameof(program));
            if (resultFunc == null) throw new ArgumentNullException(nameof(resultFunc));
            if ((behavior & CommandBehavior.CloseConnection) != 0)
                throw new ArgumentOutOfRangeException(
                    nameof(behavior),
                    "CommandBehavior.CloseConnection is not supported");
            CheckBuildingStateQuick();

            SqlBatchCommand.ReaderDisposable<TOut> command = new SqlBatchCommand.ReaderDisposable<TOut>(
                this,
                program,
                resultFunc,
                behavior,
                setParameters,
                exceptionHandler,
                suppressErrors);
            AddCommand(command);
            result = command.Result;

            return this;
        }

        /// <summary>
        /// Adds the specified program to the batch.
        /// </summary>
        /// <param name="program">The program to add to the batch.</param>
        /// <param name="resultAction">The action used to process the result.</param>
        /// <param name="setParameters">An optional method for setting the parameters to pass to the program.</param>
        /// <param name="result">A <see cref="SqlBatchResult" /> which can be used to wait for the program to finish executing.</param>
        /// <param name="suppressErrors">if set to <see langword="true" /> any errors that occur within this program
        /// wont cause an exception to be thrown for the whole batch, unless an <paramref name="exceptionHandler"/> 
        /// is specified and doesn't suppress the error. The program itself will still throw an exception.</param>
        /// <param name="exceptionHandler">The optional exception handler.</param>
        /// <returns>This <see cref="SqlBatch"/> instance.</returns>
        [NotNull]
        public SqlBatch AddExecuteXmlReader(
            [NotNull] SqlProgram program,
            [NotNull] XmlResultDelegateAsync resultAction,
            [NotNull] out SqlBatchResult result,
            [CanBeNull] SetParametersDelegate setParameters = null,
            bool suppressErrors = false,
            ExceptionHandler exceptionHandler = null)
        {
            if (program == null) throw new ArgumentNullException(nameof(program));
            if (resultAction == null) throw new ArgumentNullException(nameof(resultAction));
            CheckBuildingStateQuick();

            SqlBatchCommand.XmlReader command = new SqlBatchCommand.XmlReader(
                this,
                program,
                resultAction,
                setParameters,
                exceptionHandler,
                suppressErrors);
            AddCommand(command);
            result = command.Result;

            return this;
        }

        /// <summary>
        /// Adds the specified program to the batch.
        /// The value returned by the <paramref name="resultFunc" /> will be returned by the <see cref="SqlBatchResult{T}" />.
        /// </summary>
        /// <typeparam name="TOut">The type of the result.</typeparam>
        /// <param name="program">The program to add to the batch.</param>
        /// <param name="resultFunc">The function used to process the result.</param>
        /// <param name="result">A <see cref="SqlBatchResult{T}" /> which can be used to wait for the program to finish executing
        /// and get the value returned by <paramref name="resultFunc" />.</param>
        /// <param name="setParameters">An optional method for setting the parameters to pass to the program.</param>
        /// <param name="suppressErrors">if set to <see langword="true" /> any errors that occur within this program
        /// wont cause an exception to be thrown for the whole batch, unless an <paramref name="exceptionHandler" />
        /// is specified and doesn't suppress the error. The program itself will still throw an exception.</param>
        /// <param name="exceptionHandler">The optional exception handler.</param>
        /// <returns>
        /// This <see cref="SqlBatch" /> instance.
        /// </returns>
        [NotNull]
        public SqlBatch AddExecuteXmlReader<TOut>(
            [NotNull] SqlProgram program,
            [NotNull] XmlResultDelegateAsync<TOut> resultFunc,
            [NotNull] out SqlBatchResult<TOut> result,
            [CanBeNull] SetParametersDelegate setParameters = null,
            bool suppressErrors = false,
            ExceptionHandler exceptionHandler = null)
        {
            if (program == null) throw new ArgumentNullException(nameof(program));
            if (resultFunc == null) throw new ArgumentNullException(nameof(resultFunc));
            CheckBuildingStateQuick();

            SqlBatchCommand.XmlReader<TOut> command = new SqlBatchCommand.XmlReader<TOut>(
                this,
                program,
                resultFunc,
                setParameters,
                exceptionHandler,
                suppressErrors);
            AddCommand(command);
            result = command.Result;

            return this;
        }

        /// <summary>
        /// Adds the specified program to the batch.
        /// </summary>
        /// <param name="program">The program to add to the batch.</param>
        /// <param name="resultAction">The action used to process the result.</param>
        /// <param name="setParameters">An optional method for setting the parameters to pass to the program.</param>
        /// <param name="result">A <see cref="SqlBatchResult" /> which can be used to wait for the program to finish executing.</param>
        /// <param name="suppressErrors">if set to <see langword="true" /> any errors that occur within this program
        /// wont cause an exception to be thrown for the whole batch, unless an <paramref name="exceptionHandler"/> 
        /// is specified and doesn't suppress the error. The program itself will still throw an exception.</param>
        /// <param name="exceptionHandler">The optional exception handler.</param>
        /// <returns>This <see cref="SqlBatch"/> instance.</returns>
        [NotNull]
        public SqlBatch AddExecuteXmlReader(
            [NotNull] SqlProgram program,
            [NotNull] XmlResultDisposableDelegateAsync resultAction,
            [NotNull] out SqlBatchResult result,
            [CanBeNull] SetParametersDelegate setParameters = null,
            bool suppressErrors = false,
            ExceptionHandler exceptionHandler = null)
        {
            if (program == null) throw new ArgumentNullException(nameof(program));
            if (resultAction == null) throw new ArgumentNullException(nameof(resultAction));
            CheckBuildingStateQuick();

            SqlBatchCommand.XmlReaderDisposable command = new SqlBatchCommand.XmlReaderDisposable(
                this,
                program,
                resultAction,
                setParameters,
                exceptionHandler,
                suppressErrors);
            AddCommand(command);
            result = command.Result;

            return this;
        }

        /// <summary>
        /// Adds the specified program to the batch.
        /// The value returned by the <paramref name="resultFunc" /> will be returned by the <see cref="SqlBatchResult{T}" />.
        /// </summary>
        /// <typeparam name="TOut">The type of the result.</typeparam>
        /// <param name="program">The program to add to the batch.</param>
        /// <param name="resultFunc">The function used to process the result.</param>
        /// <param name="result">A <see cref="SqlBatchResult{T}" /> which can be used to wait for the program to finish executing
        /// and get the value returned by <paramref name="resultFunc" />.</param>
        /// <param name="setParameters">An optional method for setting the parameters to pass to the program.</param>
        /// <param name="suppressErrors">if set to <see langword="true" /> any errors that occur within this program
        /// wont cause an exception to be thrown for the whole batch, unless an <paramref name="exceptionHandler" />
        /// is specified and doesn't suppress the error. The program itself will still throw an exception.</param>
        /// <param name="exceptionHandler">The optional exception handler.</param>
        /// <returns>
        /// This <see cref="SqlBatch" /> instance.
        /// </returns>
        [NotNull]
        public SqlBatch AddExecuteXmlReader<TOut>(
            [NotNull] SqlProgram program,
            [NotNull] XmlResultDisposableDelegateAsync<TOut> resultFunc,
            [NotNull] out SqlBatchResult<TOut> result,
            [CanBeNull] SetParametersDelegate setParameters = null,
            bool suppressErrors = false,
            ExceptionHandler exceptionHandler = null)
        {
            if (program == null) throw new ArgumentNullException(nameof(program));
            if (resultFunc == null) throw new ArgumentNullException(nameof(resultFunc));
            CheckBuildingStateQuick();

            SqlBatchCommand.XmlReaderDisposable<TOut> command = new SqlBatchCommand.XmlReaderDisposable<TOut>(
                this,
                program,
                resultFunc,
                setParameters,
                exceptionHandler,
                suppressErrors);
            AddCommand(command);
            result = command.Result;

            return this;
        }

        /// <summary>
        /// Adds the specified batch to this batch.
        /// </summary>
        /// <param name="batch">The batch to add to this batch.</param>
        /// <returns>This <see cref="SqlBatch"/> instance.</returns>
        [NotNull]
        public SqlBatch AddBatch([NotNull] SqlBatch batch)
        {
            if (batch == null) throw new ArgumentNullException(nameof(batch));

            // Can't add a batch which already has a parent
            if (batch._parent != null)
                throw new InvalidOperationException(Resources.SqlBatch_RootBatch_AlreadyAdded);

            // Check the states before taking the locks
            State myState = _state, otherState = batch._state;
            if (otherState.Value != Constants.BatchState.Building)
            {
                throw new InvalidOperationException(
                    otherState.Value == Constants.BatchState.Executing
                        ? Resources.SqlBatch_RootBatch_Executing
                        : Resources.SqlBatch_RootBatch_Completed);
            }

            // If the state is for the root, check its in the building state and has enough command capacity
            if (myState.Batch._parent == null)
            {
                myState.CheckBuildingState();
                if (myState.CommandCount + otherState.CommandCount > ushort.MaxValue)
                    throw new InvalidOperationException(Resources.SqlBatch_AddCommand_OnlyAllowed65536);
            }

            // Get the states of both batches
            using (States states = GetStates(this, batch))
            {
                myState = states.StateA;
                otherState = states.StateB;

                // Can't add a batch which already has a parent
                if (batch._parent != null)
                    throw new InvalidOperationException(Resources.SqlBatch_RootBatch_AlreadyAdded);
                Debug.Assert(otherState.Batch == batch);

                // Make sure the batches are in the Building state
                myState.CheckBuildingState();
                if (otherState.Value != Constants.BatchState.Building)
                {
                    throw new InvalidOperationException(
                        otherState.Value == Constants.BatchState.Executing
                            ? Resources.SqlBatch_RootBatch_Executing
                            : Resources.SqlBatch_RootBatch_Completed);
                }

                // Increment the command counter
                if (myState.CommandCount + otherState.CommandCount > ushort.MaxValue ||
                    Interlocked.Add(ref myState.CommandCount, otherState.CommandCount) > ushort.MaxValue)
                    throw new InvalidOperationException(Resources.SqlBatch_AddCommand_OnlyAllowed65536);

                batch._parent = this;
                batch._state = myState;

                _items.Add(batch);
            }

            return this;
        }

        /// <summary>
        /// Adds the batch to this batch, only checking the state of this batch.
        /// </summary>
        /// <param name="batch">The batch.</param>
        /// <exception cref="System.InvalidOperationException"></exception>
        private void AddBatchQuick(SqlBatch batch)
        {
            using (State state = GetState())
            {
                state.CheckBuildingState();
                
                _items.Add(batch);
            }
        }

        /// <summary>
        /// Adds a new batch to this batch.
        /// </summary>
        /// <param name="addToBatch">A delegate to the method to use to add commands to the new batch.</param>
        /// <param name="suppressErrors">if set to <see langword="true" /> any errors that occur within this batch
        /// wont cause an exception to be thrown for the whole batch, unless an <paramref name="exceptionHandler"/> 
        /// is specified and doesn't suppress the error. The program that failed will still throw an exception.</param>
        /// <param name="exceptionHandler">The optional exception handler.</param>
        /// <returns>This <see cref="SqlBatch"/> instance.</returns>
        [NotNull]
        public SqlBatch AddBatch(
            [NotNull] Action<SqlBatch> addToBatch,
            bool suppressErrors = false,
            ExceptionHandler exceptionHandler = null)
        {
            if (addToBatch == null) throw new ArgumentNullException(nameof(addToBatch));
            CheckBuildingStateQuick();

            SqlBatch newBatch = new SqlBatch(this, suppressErrors: suppressErrors, exceptionHandler: exceptionHandler);

            AddBatchQuick(newBatch);

            addToBatch(newBatch);

            return this;
        }

        /// <summary>
        /// Adds a new batch to this batch.
        /// If an error occurs within the batch, the transaction will rollback.
        /// </summary>
        /// <param name="addToBatch">A delegate to the method to use to add commands to the new batch.</param>
        /// <param name="isolationLevel">The isolation level of the transaction.</param>
        /// <param name="rollback">if set to <see langword="true" /> the transaction will always be rolled back.</param>
        /// <param name="suppressErrors">if set to <see langword="true" /> any errors that occur within this batch
        /// wont cause an exception to be thrown for the whole batch, unless an <paramref name="exceptionHandler"/> 
        /// is specified and doesn't suppress the error. The program that failed will still throw an exception.</param>
        /// <param name="exceptionHandler">The optional exception handler.</param>
        /// <returns>This <see cref="SqlBatch"/> instance.</returns>
        [NotNull]
        public SqlBatch AddTransactionBatch(
            Action<SqlBatch> addToBatch,
            IsolationLevel isolationLevel,
            bool rollback = false,
            bool suppressErrors = false,
            ExceptionHandler exceptionHandler = null)
        {
            if (addToBatch == null) throw new ArgumentNullException(nameof(addToBatch));
            CheckBuildingStateQuick();

            SqlBatch newBatch = new SqlBatch(
                this,
                rollback ? TransactionType.Rollback : TransactionType.Commit,
                isolationLevel,
                suppressErrors,
                exceptionHandler);

            AddBatchQuick(newBatch);

            addToBatch(newBatch);

            return this;
        }

        /// <summary>
        /// Executes the batch on a single connection, asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token which can be used to cancel the entire batch operation.</param>
        /// <returns>An awaitable task which completes when the batch is complete.</returns>
        [NotNull]
        public async Task ExecuteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (BatchTimeout > Duration.Zero)
                cancellationToken = cancellationToken
                    .WithTimeout(BatchTimeout.Plus(AdditionalCancellationTime))
                    .Token;

            State state;

            // If this isnt the root batch, need to begin executing the root then wait for this batch to complete
            if (!IsRoot)
            {
                using (state = GetState())
                {
                    Debug.Assert(state.Batch.IsRoot);
#pragma warning disable 4014
                    // Start but don't await it
                    state.Batch.ExecuteAsync(cancellationToken);
#pragma warning restore 4014
                }

                await ((IBatchItem)this).Result.GetResultAsync(cancellationToken).ConfigureAwait(false);
                return;
            }

            // If we're already completed, just return
            if (_state.Value == Constants.BatchState.Completed) return;

            using (state = GetState())
            {
                if (state.CommandCount < 1)
                    throw new InvalidOperationException(Resources.SqlBatch_ExecuteAsync_Empty);

                // Change the state to Executing, or return if the state has already been set to completed.
                if (Interlocked.CompareExchange(
                        ref state.Value,
                        Constants.BatchState.Executing,
                        Constants.BatchState.Building) == Constants.BatchState.Completed) return;
            }

            using (await state.ExecuteLock.LockAsync(cancellationToken).ConfigureAwait(false))
            {
                if (state.Value == Constants.BatchState.Completed) return;

                try
                {
                    Connection connection = DetermineConnection();

                    // Set the result count for each item to the number of connections
                    foreach (IBatchItem item in EnumerateItems())
                        item.Result.SetResultCount(1);

                    await ExecuteInternal(connection, 0, cancellationToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException oce)
                {
                    // Ensures all results get completed
                    foreach (IBatchItem item in EnumerateItems())
                        item.Result.SetCanceledIfNotComplete(oce.CancellationToken);
                    throw;
                }
                catch (Exception e)
                {
                    // Ensures all results get completed
                    foreach (IBatchItem item in EnumerateItems())
                        item.Result.SetExceptionIfNotComplete(() => item.GetNotRunException(e));

                    if (e is LoggingException) throw;
                    throw new SqlBatchExecutionException(this, e);
                }
                finally
                {
                    state.Value = Constants.BatchState.Completed;
                }
            }
        }

        /// <summary>
        /// Executes the batch on all supported connections, asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token which can be used to cancel the entire batch operation.</param>
        /// <returns>An awaitable task which completes when the batch is complete.</returns>
        [NotNull]
        public async Task ExecuteAllAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (BatchTimeout > Duration.Zero)
                cancellationToken = cancellationToken
                    .WithTimeout(BatchTimeout.Plus(AdditionalCancellationTime))
                    .Token;

            State state;
            if (!IsRoot)
            {
                using (state = GetState())
                {
                    Debug.Assert(state.Batch.IsRoot);
#pragma warning disable 4014
                    // Start but dont await it
                    state.Batch.ExecuteAllAsync(cancellationToken);
#pragma warning restore 4014
                }

                await ((IBatchItem)this).Result.GetResultAsync(cancellationToken).ConfigureAwait(false);
                return;
            }

            // If we're already completed, just return
            if (_state.Value == Constants.BatchState.Completed) return;

            using (state = GetState())
            {
                if (state.CommandCount < 1)
                    throw new InvalidOperationException(Resources.SqlBatch_ExecuteAsync_Empty);

                // Change the state to Executing, or return if the state has already been set to completed.
                if (Interlocked.CompareExchange(
                        ref state.Value,
                        Constants.BatchState.Executing,
                        Constants.BatchState.Building) == Constants.BatchState.Completed) return;
            }

            using (await state.ExecuteLock.LockAsync(cancellationToken).ConfigureAwait(false))
            {
                if (state.Value == Constants.BatchState.Completed) return;

                try
                {
                    // Get the connection strings which are common to each program
                    HashSet<Connection> commonConnections = GetCommonConnections();
                    Debug.Assert(commonConnections != null, "commonConnections != null");

                    // Set the result count for each command to the number of connections
                    foreach (IBatchItem item in EnumerateItems())
                        item.Result.SetResultCount(commonConnections.Count);

                    // Execute the batch on the connections
                    Task[] tasks = commonConnections
                        .Select((con, i) => Task.Run(() => ExecuteInternal(con, i, cancellationToken)))
                        .ToArray();

                    await Task.WhenAll(tasks).ConfigureAwait(false);
                }
                catch (OperationCanceledException oce)
                {
                    // Ensures all results get completed
                    foreach (IBatchItem item in EnumerateItems())
                        item.Result.SetCanceledIfNotComplete(oce.CancellationToken);
                    throw;
                }
                catch (Exception e)
                {
                    // Ensures all results get completed
                    foreach (IBatchItem item in EnumerateItems())
                        item.Result.SetExceptionIfNotComplete(() => item.GetNotRunException(e));

                    if (e is LoggingException) throw;
                    throw new SqlBatchExecutionException(this, e);
                }
                finally
                {
                    state.Value = Constants.BatchState.Completed;
                    Debug.Assert(EnumerateItems().All(i => i.Result.IsCompleted()));
                }
            }
        }

        /// <summary>
        /// Begins executing the batch if it is not already executing.
        /// </summary>
        /// <param name="all">if set to <see langword="true" /> execute the batch on all connections.</param>
        internal void BeginExecute(bool all)
        {
            // If the state is Executing or Completed, don't need to do anything.
            if (_state.Value != Constants.BatchState.Building)
                return;

#pragma warning disable 4014
            // Execute the batch
            if (all)
                ExecuteAllAsync();
            else
                ExecuteAsync();
#pragma warning restore 4014
        }

        /// <summary>
        /// Executes this batch for the result given asynchronously.
        /// </summary>
        /// <param name="result">The result to get.</param>
        /// <param name="cancellationToken">A cancellation token which can be used to cancel the entire batch operation.</param>
        /// <returns></returns>
        [NotNull]
        internal Task ExecuteAsync([NotNull] SqlBatchResult result, CancellationToken cancellationToken)
        {
            Task t = ExecuteAsync(cancellationToken);
            return result.GetResultAsync(cancellationToken);
        }

        /// <summary>
        /// Executes this batch for the result given asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="result">The result to get.</param>
        /// <param name="cancellationToken">A cancellation token which can be used to cancel the entire batch operation.</param>
        /// <returns></returns>
        [NotNull]
        internal Task<T> ExecuteAsync<T>([NotNull] SqlBatchResult<T> result, CancellationToken cancellationToken)
        {
            Task t = ExecuteAsync(cancellationToken);
            return result.GetResultAsync(cancellationToken);
        }

        /// <summary>
        /// Executes this batch for the result given asynchronously.
        /// </summary>
        /// <param name="result">The result to get.</param>
        /// <param name="cancellationToken">A cancellation token which can be used to cancel the entire batch operation.</param>
        /// <returns></returns>
        [NotNull]
        internal Task ExecuteAllAsync([NotNull] SqlBatchResult result, CancellationToken cancellationToken)
        {
            Task t = ExecuteAllAsync(cancellationToken);
            return result.GetResultAsync(cancellationToken);
        }

        /// <summary>
        /// Executes this batch for the result given asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="result">The result to get.</param>
        /// <param name="cancellationToken">A cancellation token which can be used to cancel the entire batch operation.</param>
        /// <returns></returns>
        [NotNull]
        internal Task<IEnumerable<T>> ExecuteAllAsync<T>([NotNull] SqlBatchResult<T> result, CancellationToken cancellationToken)
        {
            Task t = ExecuteAllAsync(cancellationToken);
            return result.GetResultsAsync(cancellationToken);
        }

        /// <summary>
        /// Executes the batch.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="connectionIndex">Index of the connection.</param>
        /// <param name="cancellationToken">A cancellation token which can be used to cancel the entire batch operation.</param>
        [NotNull]
        private async Task ExecuteInternal(
            [NotNull] Connection connection,
            int connectionIndex,
            CancellationToken cancellationToken)
        {
            string infoMessagePrefix = $"{ID:B} @ {DateTime.UtcNow:O}:";

            DatabaseSchema schema = connection.CachedSchema;

            BatchProcessArgs args =
                new BatchProcessArgs(
                    schema?.ServerVersion ?? DatabaseSchema.MinimumSupportedServerVersion,
                    infoMessagePrefix,
                    connection.ConnectionString);

            // Build the batch SQL and get the parameters to the commands
            PreProcess(args);

            CommandBehavior allBehavior = args.Behavior;
            AsyncSemaphore[] semaphores = args.GetSemaphores();

            string state = null;
            int index = -1;
            int actualIndex = 0;
            DbBatchDataReader commandReader = null;

            // ReSharper disable AccessToModifiedClosure
            void MessageHandler(string message)
            {
                Debug.Assert(message != null, "message != null");

                if (!TryParseInfoMessage(message, ref state, ref index))
                    return;

                if (commandReader != null && (index > actualIndex || state != Constants.ExecuteState.Start))
                    commandReader.SetFinished();
            }
            // ReSharper restore AccessToModifiedClosure

            // Wait the semaphores and setup the connection, command and reader
            using (await AsyncSemaphore.WaitAllAsync(cancellationToken, semaphores).ConfigureAwait(false))
            using (DbConnection dbConnection = await CreateOpenConnectionAsync(connection, infoMessagePrefix, MessageHandler, cancellationToken)
                .ConfigureAwait(false))
            using (DbCommand dbCommand = CreateCommand(args.SqlBuilder.ToString(), dbConnection, args.AllParameters.ToArray()))
            using (cancellationToken.Register(dbCommand.Cancel))
            using (DbDataReader reader = await dbCommand.ExecuteReaderAsync(allBehavior, cancellationToken)
                .ConfigureAwait(false))
            {
                Debug.Assert(reader != null, "reader != null");

                cancellationToken.ThrowIfCancellationRequested();

                object[] values = null;

                // Ensures the 'values' buffer is big enough
                void EnsureValuesCapacity(int count)
                {
                    // Expand the values buffer if needed
                    if (values == null)
                        values = new object[count];
                    else if (values.Length < count)
                        Array.Resize(ref values, count);
                }

                SqlBatchCommand command = null;
                SqlBatch batch = this;

                var enumeratorStack = new Stack<List<IBatchItem>.Enumerator, SqlBatch, List<Exception>>();
                List<IBatchItem>.Enumerator currentEnumerator = _items.GetEnumerator();
                List<Exception> exceptions = new List<Exception>();

                // The batch for the current root transaction
                SqlBatch transactionBatch = _transaction != TransactionType.None ? this : null;
                // The items within the current root tranasction that need to be completed
                List<IBatchItem> transactionItems = transactionBatch == null ? null : new List<IBatchItem>();

                ValueReference<bool> hasResultSet = new ValueReference<bool>(true);

                bool resetConnection = false;

                // Method for enumerates the batches and commands within this batch
                bool NextCommand()
                {
                    if (enumeratorStack == null)
                    {
                        // All commands have finished executing, don't need to do anything
                        return false;
                    }

                    do
                    {
                        // Attempt to get the next item from the enumerator, popping a new enumerator from the stack
                        // if this one is empty
                        while (!currentEnumerator.MoveNext())
                        {
                            Debug.Assert(batch != null);

                            // No items in the enumerator means we have reached the end of a batch. 
                            // We might need to set it to complete, depending on the transaction

                            if (batch._suppressErrors)
                                Debug.Assert(exceptions.Count < 1);

                            if (exceptions.Count > 0)
                            {
                                batch.SetException(connectionIndex, exceptions.ToArray());

                                if (batch == transactionBatch)
                                {
                                    foreach (IBatchItem item in transactionItems)
                                        item.SetException(connectionIndex, item.GetNotRunException());
                                }
                            }

                            // Not inside a transaction, just complete it
                            if (transactionBatch == null)
                                batch._batchResult.SetCompleted(connectionIndex);

                            // This is the batch that started the root transaction, so need to complete it and all child batches
                            else if (batch == transactionBatch)
                            {
                                transactionBatch = null;
                                Debug.Assert(transactionItems != null);

                                batch._batchResult.SetCompleted(connectionIndex);
                                foreach (IBatchItem item in transactionItems)
                                    item.Result.SetCompleted(connectionIndex);

                                transactionItems.Clear();
                            }

                            List<Exception> childEx = exceptions;

                            // Attempt to pop the parent batches enumerator from the stack
                            if (!enumeratorStack.TryPop(out currentEnumerator, out batch, out exceptions))
                            {
                                // Finished processing the last command.

                                // If there are any errors, throw them
                                if (!_suppressErrors && childEx.Count > 0)
                                {
                                    Debug.Assert(!_suppressErrors);
                                    if (childEx.Count < 2) childEx[0].ReThrow();
                                    else throw new AggregateException(
                                        $"Multiple exceptions occurred for the program on connection #{connectionIndex}",
                                        childEx);
                                }

                                enumeratorStack = null;
                                return false;
                            }

                            if (!batch._suppressErrors)
                                exceptions.AddRange(childEx);
                        }

                        // If transactionBatch is assigned, the item is within a transaction
                        if (transactionBatch != null)
                        {
                            Debug.Assert(transactionItems != null);
                            transactionItems.Add(currentEnumerator.Current);
                        }

                        // If the current item is a batch, need to push the parent batch to the stack
                        if (currentEnumerator.Current is SqlBatch currBatch)
                        {
                            enumeratorStack.Push(currentEnumerator, batch, exceptions);
                            currentEnumerator = currBatch._items.GetEnumerator();
                            batch = currBatch;
                            exceptions = new List<Exception>();

                            // If we arent currently in a transaction, and the new batch has a transaction, set it as the 
                            // transaction batch
                            if (transactionBatch == null && currBatch._transaction != TransactionType.None)
                            {
                                transactionBatch = currBatch;
                                if (transactionItems == null)
                                    transactionItems = new List<IBatchItem>();
                            }
                        }
                        else
                        {
                            command = (SqlBatchCommand)currentEnumerator.Current;
                            return true;
                        }
                    } while (true);
                }

                try
                {
                    // Enumerate the commands in the batch
                    while (NextCommand())
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            command.Result.SetCanceled(connectionIndex);

                            // If we're inside a transaction, the connection should be reset to ensure it gets rolled back
                            if (transactionBatch != null)
                                resetConnection = true;

                            cancellationToken.ThrowIfCancellationRequested();
                        }

                        Debug.Assert(command != null, "command != null");
                        actualIndex = command.Index;

                        if (index > actualIndex || !hasResultSet)
                        {
                            command.SetException(connectionIndex, new SqlProgramNotRunException(command.Program));
                            continue;
                        }
                        Debug.Assert(index == actualIndex);

                        bool gotOutput = false;

                        // Enumerate the states returned from the database for this command
                        bool cont = true;
                        while (index == actualIndex && hasResultSet && cont)
                        {
                            if (cancellationToken.IsCancellationRequested)
                            {
                                command.Result.SetCanceled(connectionIndex);

                                // If we're inside a transaction, the connection should be reset to ensure it gets rolled back
                                if (transactionBatch != null)
                                    resetConnection = true;

                                cancellationToken.ThrowIfCancellationRequested();
                            }

                            try
                            {
                                string currState = state;
                                // Null out the state, so if no more info messages set it we can detect it
                                state = null;
                                switch (currState)
                                {
                                    // If the state is null, then there were no more info messages to set it
                                    case null:
                                        cont = false;
                                        break;

                                    // Indicates the start of a command
                                    case Constants.ExecuteState.Start:
                                        // Handle the commands response from the database
                                        using (commandReader = CreateReader(reader, command.CommandBehavior, hasResultSet))
                                        {
                                            await command.HandleCommandAsync(
                                                    commandReader,
                                                    dbCommand,
                                                    connectionIndex,
                                                    cancellationToken)
                                                .ConfigureAwait(false);

                                            if (!commandReader.IsFinishedReading)
                                            {
                                                // Read the rest of the result sets until an info message finishes the reader
                                                await commandReader.ReadTillFinishedAsync(cancellationToken)
                                                    .ConfigureAwait(false);
                                            }
                                        }
                                        commandReader = null;
                                        break;

                                    // Indicates the values of output parameters will be returned in the next record set
                                    case Constants.ExecuteState.Output:
                                        // Get the expected output parameters for the command
                                        if (!args.CommandOutParams.TryGetValue(command, out var outs))
                                        {
                                            throw new SqlBatchExecutionException(
                                                this,
                                                LoggingLevel.Critical,
                                                () => Resources.SqlBatch_ExecuteInternal_UnexpectedOutputParameters,
                                                command.Program.Name);
                                        }

                                        gotOutput = true;

                                        // No longer expect the output parameters
                                        args.CommandOutParams.Remove(command);

                                        // Make sure the output parameter values were actually returned
                                        if (!await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                                        {
                                            throw new SqlBatchExecutionException(
                                                this,
                                                LoggingLevel.Critical,
                                                () => Resources.SqlBatch_ExecuteInternal_MissingOutputParameters,
                                                command.Program.Name);
                                        }

                                        // Make sure the correct number of values were returned
                                        if (outs.Count != reader.VisibleFieldCount)
                                        {
                                            throw new SqlBatchExecutionException(
                                                this,
                                                LoggingLevel.Critical,
                                                () => Resources.SqlBatch_ExecuteInternal_WrongOutputParameters,
                                                command.Program.Name);
                                        }

                                        // Expand the values buffer if needed
                                        EnsureValuesCapacity(reader.VisibleFieldCount);

                                        // Get the output values record
                                        reader.GetValues(values);

                                        // Set the output values
                                        for (int i = 0; i < outs.Count; i++)
                                        {
                                            IOut output = outs[i].output;
                                            DbBatchParameter param = outs[i].param;

                                            Debug.Assert(output != null, "outs[i].output != null");
                                            Debug.Assert(param != null, "outs[i].param != null");

                                            try
                                            {
                                                object outValue = param.ProgramParameter.CastSQLValue(values[i], output.Type);
                                                output.SetOutputValue(outValue, param.BaseParameter);
                                            }
                                            catch (Exception e)
                                            {
                                                output.SetOutputError(e, param.BaseParameter);
                                            }
                                        }

                                        // Make sure theres only one record
                                        if (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                                        {
                                            throw new SqlBatchExecutionException(
                                                this,
                                                LoggingLevel.Critical,
                                                () => Resources.SqlBatch_ExecuteInternal_WrongOutputParameters,
                                                command.Program.Name);
                                        }

                                        hasResultSet.Value =
                                            await reader.NextResultAsync(cancellationToken).ConfigureAwait(false);
                                        break;

                                    // Indicates error information will be returned in the next record set
                                    case Constants.ExecuteState.Error:
                                        // Make sure error data was returned
                                        if (!await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                                        {
                                            throw new SqlBatchExecutionException(
                                                this,
                                                LoggingLevel.Critical,
                                                () => Resources.SqlBatch_ExecuteInternal_MissingErrorData,
                                                command.Program.Name);
                                        }

                                        // Expand the values buffer if needed
                                        EnsureValuesCapacity(reader.VisibleFieldCount);

                                        // Get the error data record
                                        reader.GetValues(values);

                                        // Create a DbException from the error data
                                        DbException dbException = GetDbException(values, dbConnection);
                                        SqlProgramExecutionException progExecException =
                                            new SqlProgramExecutionException(command.Program, dbException);

                                        command.SetException(connectionIndex, progExecException);

                                        // TODO Not currently needed, but could support multiple rows of error data

                                        hasResultSet.Value =
                                            await reader.NextResultAsync(cancellationToken).ConfigureAwait(false);

                                        // Add it to the current exception collection if not suppressed
                                        if (!batch._suppressErrors && !command.SuppressErrors)
                                            exceptions.Add(progExecException);
                                        break;

                                    // Indicates the end of a command
                                    case Constants.ExecuteState.End:
                                        // If no output parameters were returned, but the command has output parameters, something has gone wrong
                                        if (!gotOutput && args.CommandOutParams.ContainsKey(command))
                                        {
                                            throw new SqlBatchExecutionException(
                                                this,
                                                LoggingLevel.Critical,
                                                () => Resources.SqlBatch_ExecuteInternal_MissingOutputParameters,
                                                command.Program.Name);
                                        }

                                        // Make sure the end record set has data
                                        if (!await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                                        {
                                            throw new SqlBatchExecutionException(
                                                this,
                                                LoggingLevel.Critical,
                                                () => Resources.SqlBatch_ExecuteInternal_MissingEnd,
                                                command.Program.Name);
                                        }

                                        // Expects a single row and column with the string "End"
                                        if (reader.FieldCount != 1
                                            || await reader.IsDBNullAsync(0, cancellationToken).ConfigureAwait(false)
                                            || !"End".Equals(reader.GetValue(0))
                                            || await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                                        {
                                            throw new SqlBatchExecutionException(
                                                this,
                                                LoggingLevel.Critical,
                                                () => Resources.SqlBatch_ExecuteInternal_EndWrongFormat,
                                                command.Program.Name);
                                        }

                                        // Move on to the next record set, if any
                                        hasResultSet.Value =
                                            await reader.NextResultAsync(cancellationToken).ConfigureAwait(false);
                                        break;

                                    default:
                                        throw new SqlBatchExecutionException(
                                            this,
                                            LoggingLevel.Critical,
                                            () => Resources.SqlBatch_ExecuteInternal_UnexpectedState,
                                            state,
                                            command.Program.Name);
                                }
                            }

                            catch (OperationCanceledException)
                            {
                                Debug.Assert(cancellationToken.IsCancellationRequested);
                                command.Result.SetCanceled(connectionIndex);

                                // If we're inside a transaction, the connection should be reset to ensure it gets rolled back
                                if (transactionBatch != null)
                                    resetConnection = true;

                                throw;
                            }
                            catch (Exception e)
                            {
                                // If the exception is at least Critical and it was thrown by this batch or command,
                                // rethrow it right away
                                bool rethrow = false;
                                if (e is SqlBatchExecutionException bee)
                                    rethrow = bee.Level >= LoggingLevel.Critical && (bee.BatchId == ID || bee.BatchId == batch.ID);
                                else if (e is SqlProgramExecutionException pee)
                                    rethrow = pee.Level >= LoggingLevel.Critical && pee.ProgramName == command.Program.Name;
                                else
                                    e = new SqlProgramExecutionException(command.Program, e);
                                
                                command.SetException(connectionIndex, e);

                                // If the token has cancellation requested, this might be a cancelled/timeout exception
                                if (cancellationToken.IsCancellationRequested)
                                {
                                    // If we're inside a transaction, the connection should be reset to ensure it gets rolled back
                                    if (transactionBatch != null)
                                        resetConnection = true;
                                }
                                
                                // Rethrow if needed
                                if (rethrow)
                                    throw;

                                // Add it to the current exception collection if not suppressed
                                if (!batch._suppressErrors && !command.SuppressErrors)
                                    exceptions.Add(e);
                            }
                        }
                        if (transactionBatch == null)
                            command.Result.SetCompleted(connectionIndex);
                    }
                }
                finally
                {
                    try
                    {
                        // If the batch was cancelled while inside a transaction, we need to explicitly reset the connection
                        // to ensure the transaction gets rolled back. Just closing the connection doesn't always do this due to pooling.
                        if (resetConnection && dbConnection.State == ConnectionState.Open)
                        {
                            reader.Close();

                            Debug.WriteLine("Resetting connection");
                            using (DbCommand cmd = CreateCommand(
                                "sp_reset_connection",
                                dbConnection,
                                Array<DbParameter>.Empty,
                                CommandType.StoredProcedure))
                            {

                                // ReSharper disable once MethodSupportsCancellation
                                await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Add(e, LoggingLevel.Error, () => "An error occurred while resetting the connection.");
                    }
                }
            }
        }

        /// <summary>
        /// Pre-processes the commands to be executed
        /// </summary>
        /// <param name="args">The arguments.</param>
        private void PreProcess([NotNull] BatchProcessArgs args) => ((IBatchItem)this).Process(args);

        /// <summary>
        /// Processes the batch to be executed.
        /// </summary>
        /// <param name="args">The arguments.</param>
        void IBatchItem.Process(BatchProcessArgs args)
        {
            args.SqlBuilder
                .AppendLine()
                .AppendLine("/*")
                .Append(" * Starting batch ")
                .AppendLine(ID.ToString("D"))
                .AppendLine(" */");

            if (IsRoot)
            {
                // Declare the variable for storing the index of the currently executing command
                args.SqlBuilder
                    .AppendLine("DECLARE @CmdIndex int;");
            }

            args.SqlBuilder.AppendLine();

            this.BeginTry(args, out int startIndex);

            // Process the items in this batch
            foreach (IBatchItem item in _items)
                item.Process(args);

            this.EndTry(args, startIndex);
            
            args.SqlBuilder
                .AppendLine("/*")
                .Append(" * Ending batch ")
                .AppendLine(ID.ToString("D"))
                .AppendLine(" */")
                .AppendLine();
        }

        /// <summary>
        /// Appends an info message to the SQL builder.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="state">The state.</param>
        /// <param name="commandIndex">Override the index.</param>
        /// <param name="formatArgs">The format arguments.</param>
        /// <returns>
        /// The <see cref="BatchProcessArgs.SqlBuilder" />
        /// </returns>
        [NotNull]
        internal static SqlStringBuilder AppendInfo(
            [NotNull] BatchProcessArgs args,
            [NotNull] string state,
            [CanBeNull] string commandIndex = null,
            [CanBeNull] string formatArgs = null,
            bool error = false)
        {
            // TODO this would be provider specific

            Debug.Assert(!state.Contains(":"));
            if (commandIndex == null) commandIndex = args.CommandIndex.ToString();
            args.SqlBuilder
                .Append("RAISERROR(")
                .AppendVarChar($"{args.InfoMessagePrefix}{state}:{commandIndex}")
                .Append(error ? ",16,0" : ",4,0");
            if (formatArgs != null)
                args.SqlBuilder
                    .Append(',')
                    .Append(formatArgs);
            return args.SqlBuilder
                .AppendLine(");");
        }

        /// <summary>
        /// Attempts to parse an information message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="state">The state.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private static bool TryParseInfoMessage([NotNull] string message, ref string state, ref int index)
        {
            int ind1 = message.IndexOf(':');
            if (ind1 < 0 || !ushort.TryParse(message.Substring(ind1 + 1), out ushort parsedIndex))
                return false;

            state = message.Substring(0, ind1);
            index = parsedIndex;
            return true;
        }

        /// <summary>
        /// Registers an information message handler.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="uid">The uid.</param>
        /// <param name="handler">The handler.</param>
        /// <exception cref="System.NotSupportedException"></exception>
        private void RegisterInfoMessageHandler(
            [NotNull] DbConnection connection,
            [NotNull] string uid,
            [NotNull] Action<string> handler)
        {
            switch (connection)
            {
                case SqlConnection sqlConnection:
                    sqlConnection.InfoMessage += (sender, args) =>
                    {
                        foreach (SqlError error in args.Errors)
                        {
                            // Check if the error is one we care about
                            // The RAISERROR will return specific values to avoid conflicting with user RAISERRORs
                            // Class == 4 was chosen as the built in messages use 0 or 10+, so 4 should rarely happen
                            // Number == 50000 is the message ID returned when a string is passed to RAISERROR
                            // The message also starts with a specific string
                            if (error.Class == 4 &&
                                error.Number == 50000 &&
                                error.Message.StartsWith(uid))
                            {
                                handler(error.Message.Substring(uid.Length));
                            }
                        }
                    };
                    break;
                default:
                    // Eventually will support multiple db providers
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Creates and opens a connection asynchronously.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="uid">The uid.</param>
        /// <param name="messageHandler">The message handler.</param>
        /// <param name="cancellationToken">A cancellation token which can be used to cancel the entire batch operation.</param>
        [NotNull]
        [ItemNotNull]
        private async Task<DbConnection> CreateOpenConnectionAsync(
            [NotNull] string connectionString,
            [NotNull] string uid,
            [NotNull] Action<string> messageHandler,
            CancellationToken cancellationToken)
        {
            DbConnection dbConnection = null;
            try
            {
                dbConnection = new SqlConnection(connectionString);

                RegisterInfoMessageHandler(dbConnection, uid, messageHandler);

                await dbConnection.OpenAsync(cancellationToken).ConfigureAwait(false);

                return dbConnection;
            }
            catch
            {
                dbConnection?.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Creates a command for the batch.
        /// </summary>
        /// <param name="text">The text to execute.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        [NotNull]
        private DbCommand CreateCommand(
            [NotNull] string text,
            [NotNull] DbConnection connection,
            [NotNull] DbParameter[] parameters,
            CommandType commandType = CommandType.Text)
        {
            DbCommand command = connection.CreateCommand();
            try
            {
                Debug.Assert(command.Connection == connection);

#if DEBUG
                _sql = text;
#endif

                command.CommandText = text;
                command.CommandType = commandType;
                command.CommandTimeout = (int)BatchTimeout.TotalSeconds();
                command.Parameters.AddRange(parameters);

                return command;
            }
            catch
            {
                command.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Gets a "not run" exception for this item.
        /// </summary>
        /// <param name="innerException">The inner exception.</param>
        /// <returns>The exception.</returns>
        Exception IBatchItem.GetNotRunException(Exception innerException) 
            => new SqlBatchNotRunException(this, innerException);

        /// <summary>
        /// Gets an execution exception for this item.
        /// </summary>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="resource">The resource.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The exception.</returns>
        Exception IBatchItem.GetExecutionException(
            Exception innerException,
            Expression<Func<string>> resource,
            params object[] parameters)
            => new SqlBatchExecutionException(this, innerException, resource, parameters);

        /// <summary>
        /// Creates the <see cref="DbBatchDataReader" /> for the underlying reader given.
        /// </summary>
        /// <param name="reader">The base reader.</param>
        /// <param name="commandBehavior">The program behavior.</param>
        /// <param name="hasResultSet">The reference to the flag indicating if there are any more result sets.</param>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException"></exception>
        [NotNull]
        private DbBatchDataReader CreateReader(
            [NotNull] DbDataReader reader,
            CommandBehavior commandBehavior,
            [NotNull] ValueReference<bool> hasResultSet)
        {
            switch (reader)
            {
                case SqlDataReader sqlReader:
                    return new SqlBatchDataReader(this, sqlReader, commandBehavior, hasResultSet);
                default:
                    // Eventually will support multiple db providers
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Gets an exception from error data.
        /// </summary>
        /// <param name="errorData">The error data.</param>
        /// <param name="dbConnection">The database connection.</param>
        /// <returns></returns>
        private static DbException GetDbException([NotNull] object[] errorData, [NotNull] DbConnection dbConnection)
        {
            Debug.Assert(dbConnection is SqlConnection);
            SqlConnection sqlConnection = (SqlConnection)dbConnection;
            return new SqlExceptionPrototype(
                new SqlErrorCollectionPrototype
                {
                    new SqlErrorPrototype(
                        (int)errorData[0],
                        (byte)errorData[1],
                        (byte)errorData[2],
                        errorData[3] as string,
                        errorData[4] as string,
                        errorData[5] as string,
                        (int)errorData[6])
                },
                sqlConnection.ServerVersion,
                sqlConnection.ClientConnectionId);
        }

        /// <summary>
        /// An equality comparer that compares only the connection string for a connection.
        /// </summary>
        [NotNull]
        private static readonly EqualityBuilder<Connection> _connectionStringEquality =
            new EqualityBuilder<Connection>(
                (a, b) => a.ConnectionString == b.ConnectionString,
                c => c.ConnectionString.GetHashCode());

        /// <summary>
        /// Determines the connection string to use.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">
        /// </exception>
        [NotNull]
        private Connection DetermineConnection()
        {
            Debug.Assert(IsRoot);
            Debug.Assert(_state.CommandCount > 0);

            // Get the connection strings which are common to each program
            HashSet<Connection> commonConnections = GetCommonConnections();

            // If there is a single common connection string, just use that
            if (commonConnections.Count == 1)
                return commonConnections.First();

            Debug.Assert(commonConnections != null);

            Dictionary<Connection, WeightCounter> connWeightCounts =
                new Dictionary<Connection, WeightCounter>(_connectionStringEquality);

            foreach (SqlBatchCommand command in this)
            {
                SqlProgramMapping[] mappings = command.Program.Mappings
                    .Where(m => commonConnections.Contains(m.Connection))
                    .ToArray();

                double totWeight = mappings.Sum(m => m.Connection.Weight);

                foreach (SqlProgramMapping mapping in mappings)
                {
                    if (!connWeightCounts.TryGetValue(mapping.Connection, out var counter))
                    {
                        counter = new WeightCounter();
                        connWeightCounts.Add(mapping.Connection, counter);
                    }

                    counter.Increment(mapping.Connection.Weight / totWeight);
                }
            }

            return connWeightCounts.Choose(kvp => kvp.Value.GetWeight()).Key;
        }

        /// <summary>
        /// Gets the connection strings common to all commands.
        /// </summary>
        /// <returns>The set of common connections.</returns>
        /// <exception cref="System.InvalidOperationException">
        /// </exception>
        [NotNull]
        [ItemNotNull]
        private HashSet<Connection> GetCommonConnections()
        {
            Connection commonConnection = null;
            HashSet<Connection> commonConnections = null;
            foreach (SqlBatchCommand command in this)
            {
                if (commonConnections == null)
                {
                    commonConnections = new HashSet<Connection>(
                        command.Program.Mappings
                            .Where(m => m.Connection.Weight > 0)
                            .Select(m => m.Connection),
                        _connectionStringEquality);
                }
                else
                {
                    // If there's a single common connection, just check if any mapping has that connection.
                    if (commonConnection != null)
                    {
                        bool contains = false;
                        foreach (SqlProgramMapping mapping in command.Program.Mappings)
                        {
                            if (mapping.Connection.Weight > 0 &&
                                mapping.Connection.ConnectionString == commonConnection.ConnectionString)
                            {
                                contains = true;
                                break;
                            }
                        }

                        if (!contains)
                            throw new SqlBatchExecutionException(
                                this,
                                LoggingLevel.Error,
                                () => Resources.SqlBatch_AddCommand_NoCommonConnections);
                        continue;
                    }

                    commonConnections.IntersectWith(
                        command.Program.Mappings
                            .Where(m => m.Connection.Weight > 0)
                            .Select(m => m.Connection));
                }

                if (commonConnections.Count < 1)
                    throw new SqlBatchExecutionException(
                        this,
                        LoggingLevel.Error,
                        () => Resources.SqlBatch_AddCommand_NoCommonConnections);

                if (commonConnections.Count == 1)
                    commonConnection = commonConnections.Single();
            }
            Debug.Assert(commonConnections != null, "commonConnections != null");
            return commonConnections;
        }

        /// <summary>
        /// Used to aggregate weights.
        /// </summary>
        private class WeightCounter
        {
            /// <summary>
            /// The total count, equal to the sum of the values of <see cref="_weightedCounts"/>.
            /// </summary>
            private int _totalCount;

            /// <summary>
            /// The the count for each weight.
            /// </summary>
            [NotNull]
            private readonly Dictionary<double, int> _weightedCounts = new Dictionary<double, int>();

            /// <summary>
            /// Increments the count for the specified weight.
            /// </summary>
            /// <param name="weight">The weight.</param>
            public void Increment(double weight)
            {
                _totalCount++;
                _weightedCounts.TryGetValue(weight, out var count);
                _weightedCounts[weight] = count + 1;
            }

            /// <summary>
            /// Gets the aggregated weight.
            /// </summary>
            /// <returns></returns>
            public double GetWeight()
            {
                if (_weightedCounts.Count == 1)
                {
                    Dictionary<double, int>.KeyCollection.Enumerator enumerator = _weightedCounts.Keys.GetEnumerator();
                    enumerator.MoveNext();
                    return enumerator.Current;
                }

                double totCount = _totalCount;
                double weight = 0;
                bool first = true;
                foreach (KeyValuePair<double, int> kvp in _weightedCounts)
                {
                    double currWeight = Math.Pow(kvp.Key, kvp.Value / totCount);

                    if (first)
                    {
                        first = false;
                        weight = currWeight;
                    }
                    else
                    {
                        weight *= currWeight;
                    }
                }
                return weight;
            }
        }
    }
}