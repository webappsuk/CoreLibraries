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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NodaTime;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Database.Schema;
using WebApplications.Utilities.Threading;

namespace WebApplications.Utilities.Database
{
    /// <summary>
    /// Delegate to a method for handling an exception.
    /// </summary>
    /// <typeparam name="T">The type of the exception.</typeparam>
    /// <param name="exception">The exception to handle.</param>
    /// <param name="suppress">Set to <see langword="true"/> to suppress the exception.</param>
    public delegate void ExceptionHandler<in T>(T exception, ref bool suppress)
        where T : Exception;

    /// <summary>
    /// Allows multiple <see cref="SqlProgram">SqlPrograms</see> to be executed in a single database call.
    /// </summary>
    public partial class SqlBatch : IEnumerable<SqlBatchCommand>, IBatchItem
    {
        /// <summary>
        /// The type of a transaction.
        /// </summary>
        [Flags]
        private enum TransactionType : byte
        {
            /// <summary>
            /// No transaction
            /// </summary>
            None,
            
            /// <summary>
            /// A transaction which commits if successfully executed
            /// </summary>
            Commit,

            /// <summary>
            /// A transaction which always rolls back
            /// </summary>
            Rollback
        }

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
            /// The command count.
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
        
        [NotNull]
        private State _state;

        private SqlBatch _parent;

        [NotNull]
        [ItemNotNull]
        private readonly List<IBatchItem> _items = new List<IBatchItem>();

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
        /// The command that failed will still throw an exception.
        /// </summary>
        private readonly bool _suppressErrors;

        /// <summary>
        /// The exception handler for any errors that occur in the database.
        /// Only used if there is a transaction or errors are suppressed.
        /// </summary>
        private readonly ExceptionHandler<DbException> _exceptionHandler;

#if DEBUG
        /// <summary>
        /// The SQL for the batch. For debugging purposes.
        /// </summary>
        private string _sql;
#endif

        /// <summary>
        /// Creates a new batch.
        /// </summary>
        /// <param name="batchTimeout">The batch timeout. Defaults to 30 seconds.</param>
        /// <returns>The new <see cref="SqlBatch"/>.</returns>
        [NotNull]
        public static SqlBatch Create(Duration? batchTimeout = null)
        {
            return new SqlBatch(batchTimeout);
        }

        /// <summary>
        /// Creates a new batch which is wrapped in a try ... catch block. 
        /// Any errors that occur within this batch wont cause an exception to be thrown for the whole batch, 
        /// unless an <paramref name="exceptionHandler"/> is specified and doesn't suppress the error.
        /// The command that failed will still throw an exception.
        /// </summary>
        /// <param name="exceptionHandler">The optional exception handler.</param>
        /// <param name="batchTimeout">The batch timeout. Defaults to 30 seconds.</param>
        /// <returns>The new <see cref="SqlBatch"/>.</returns>
        [NotNull]
        public static SqlBatch CreateTryCatch(
            ExceptionHandler<DbException> exceptionHandler = null,
            Duration? batchTimeout = null)
        {
            return new SqlBatch(batchTimeout, suppressErrors: true, exceptionHandler: exceptionHandler);
        }

        /// <summary>
        /// Creates a new batch which is wrapped in a transaction.
        /// If an error occurs within the batch, the transaction will rollback.
        /// </summary>
        /// <param name="isolationLevel">The isolation level of the transaction.</param>
        /// <param name="rollback">if set to <see langword="true" /> the transaction will always be rolled back.</param>
        /// <param name="suppressErrors">if set to <see langword="true" /> any errors that occur within this batch
        /// wont cause an exception to be thrown for the whole batch. See <see cref="CreateTryCatch" />.</param>
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
            ExceptionHandler<DbException> exceptionHandler = null,
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
            ExceptionHandler<DbException> exceptionHandler = null)
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
            ExceptionHandler<DbException> exceptionHandler = null)
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
            Stack<List<IBatchItem>.Enumerator> stack = new Stack<List<IBatchItem>.Enumerator>();
            stack.Push(_items.GetEnumerator());

            while (stack.TryPop(out List<IBatchItem>.Enumerator enumerator))
            {
                while (enumerator.MoveNext())
                {
                    IBatchItem item = enumerator.Current;

                    if (item is SqlBatchCommand command) yield return command;
                    else
                    {
                        Debug.Assert(item is SqlBatch);

                        stack.Push(enumerator);
                        stack.Push(((SqlBatch)item)._items.GetEnumerator());
                        break;
                    }
                }
            }
        }
        
        /// <summary>
        /// Adds the command given to the batch.
        /// </summary>
        /// <param name="command">The command.</param>
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
        /// <returns>This <see cref="SqlBatch"/> instance.</returns>
        [NotNull]
        public SqlBatch AddExecuteScalar<TOut>(
            [NotNull] SqlProgram program,
            [NotNull] out SqlBatchResult<TOut> result,
            [CanBeNull] SetBatchParametersDelegate setParameters = null)
        {
            if (program == null) throw new ArgumentNullException(nameof(program));
            CheckBuildingStateQuick();

            SqlBatchCommand.Scalar<TOut> command = new SqlBatchCommand.Scalar<TOut>(this, program, setParameters);
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
        /// <returns>This <see cref="SqlBatch"/> instance.</returns>
        [NotNull]
        public SqlBatch AddExecuteNonQuery(
            [NotNull] SqlProgram program,
            [NotNull] out SqlBatchResult<int> result,
            [CanBeNull] SetBatchParametersDelegate setParameters = null)
        {
            if (program == null) throw new ArgumentNullException(nameof(program));
            CheckBuildingStateQuick();

            SqlBatchCommand.NonQuery command = new SqlBatchCommand.NonQuery(this, program, setParameters);
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
        /// <returns>This <see cref="SqlBatch"/> instance.</returns>
        [NotNull]
        public SqlBatch AddExecuteReader(
            [NotNull] SqlProgram program,
            [NotNull] ResultDelegateAsync resultAction,
            [NotNull] out SqlBatchResult result,
            CommandBehavior behavior = CommandBehavior.Default,
            [CanBeNull] SetBatchParametersDelegate setParameters = null)
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
                setParameters);
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
        /// <returns>This <see cref="SqlBatch"/> instance.</returns>
        [NotNull]
        public SqlBatch AddExecuteReader<TOut>(
            [NotNull] SqlProgram program,
            [NotNull] ResultDelegateAsync<TOut> resultFunc,
            [NotNull] out SqlBatchResult<TOut> result,
            CommandBehavior behavior = CommandBehavior.Default,
            [CanBeNull] SetBatchParametersDelegate setParameters = null)
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
                setParameters);
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
        /// <returns>This <see cref="SqlBatch"/> instance.</returns>
        [NotNull]
        public SqlBatch AddExecuteXmlReader(
            [NotNull] SqlProgram program,
            [NotNull] XmlResultDelegateAsync resultAction,
            [NotNull] out SqlBatchResult result,
            [CanBeNull] SetBatchParametersDelegate setParameters = null)
        {
            if (program == null) throw new ArgumentNullException(nameof(program));
            if (resultAction == null) throw new ArgumentNullException(nameof(resultAction));
            CheckBuildingStateQuick();

            SqlBatchCommand.XmlReader command = new SqlBatchCommand.XmlReader(
                this,
                program,
                resultAction,
                setParameters);
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
        /// <param name="setParameters">An optional method for setting the parameters to pass to the program.</param>
        /// <param name="result">A <see cref="SqlBatchResult{T}" /> which can be used to wait for the program to finish executing
        /// and get the value returned by <paramref name="resultFunc"/>.</param>
        /// <returns>This <see cref="SqlBatch"/> instance.</returns>
        [NotNull]
        public SqlBatch AddExecuteXmlReader<TOut>(
            [NotNull] SqlProgram program,
            [NotNull] XmlResultDelegateAsync<TOut> resultFunc,
            [NotNull] out SqlBatchResult<TOut> result,
            [CanBeNull] SetBatchParametersDelegate setParameters = null)
        {
            if (program == null) throw new ArgumentNullException(nameof(program));
            if (resultFunc == null) throw new ArgumentNullException(nameof(resultFunc));
            CheckBuildingStateQuick();

            SqlBatchCommand.XmlReader<TOut> command = new SqlBatchCommand.XmlReader<TOut>(
                this,
                program,
                resultFunc,
                setParameters);
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
        /// <returns>This <see cref="SqlBatch"/> instance.</returns>
        [NotNull]
        public SqlBatch AddBatch([NotNull] Action<SqlBatch> addToBatch)
        {
            if (addToBatch == null) throw new ArgumentNullException(nameof(addToBatch));
            CheckBuildingStateQuick();

            SqlBatch newBatch = new SqlBatch(this);

            AddBatchQuick(newBatch);

            addToBatch(newBatch);

            return this;
        }

        /// <summary>
        /// Adds a new batch to this batch.
        /// Any errors that occur within the new batch wont cause an exception to be thrown for the whole batch, 
        /// unless an <paramref name="exceptionHandler"/> is specified and doesn't suppress the error.
        /// The command that failed will still throw an exception.
        /// </summary>
        /// <param name="addToBatch">A delegate to the method to use to add commands to the new batch.</param>
        /// <param name="exceptionHandler">The optional exception handler.</param>
        /// <returns>This <see cref="SqlBatch"/> instance.</returns>
        [NotNull]
        public SqlBatch AddTryCatch(Action<SqlBatch> addToBatch, ExceptionHandler<DbException> exceptionHandler = null)
        {
            if (addToBatch == null) throw new ArgumentNullException(nameof(addToBatch));
            CheckBuildingStateQuick();

            SqlBatch newBatch = new SqlBatch(this, suppressErrors: true, exceptionHandler: exceptionHandler);

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
        /// wont cause an exception to be thrown for the whole batch. See <see cref="CreateTryCatch" />.</param>
        /// <param name="exceptionHandler">The optional exception handler.</param>
        /// <returns>This <see cref="SqlBatch"/> instance.</returns>
        [NotNull]
        public SqlBatch AddTransaction(
            Action<SqlBatch> addToBatch,
            IsolationLevel isolationLevel,
            bool rollback = false,
            bool suppressErrors = false,
            ExceptionHandler<DbException> exceptionHandler = null)
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
            // If this isnt the root batch, need to begin executing the root then wait for this batch to complete
            if (!IsRoot)
            {
                throw new NotImplementedException();
            }

            // If we're already completed, just return
            if (_state.Value == Constants.BatchState.Completed) return;

            State state;
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

                    // Set the result count for each command to the number of connections
                    foreach (SqlBatchCommand command in this)
                        command.Result.SetResultCount(1);

                    await ExecuteInternal(connection, 0, cancellationToken).ConfigureAwait(false);
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
            if (!IsRoot)
            {
                throw new NotImplementedException();
            }

            // If we're already completed, just return
            if (_state.Value == Constants.BatchState.Completed) return;

            State state;
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
                    foreach (SqlBatchCommand command in this)
                        command.Result.SetResultCount(commonConnections.Count);

                    Task[] tasks = commonConnections
                        .Select((con, i) => Task.Run(() => ExecuteInternal(con, i, cancellationToken)))
                        .ToArray();

                    await Task.WhenAll(tasks).ConfigureAwait(false);
                }
                finally
                {
                    state.Value = Constants.BatchState.Completed;
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

            // Execute the batch
            if (all)
                Task.Run(() => ExecuteAllAsync());
            else
                Task.Run(() => ExecuteAsync());
        }

        /// <summary>
        /// Executes the batch.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="connectionIndex">Index of the connection.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        [NotNull]
        private async Task ExecuteInternal(
            [NotNull] Connection connection,
            int connectionIndex,
            CancellationToken cancellationToken)
        {
            string uid = $"{ID:B} @ {DateTime.UtcNow:O}:";

            DatabaseSchema schema = connection.CachedSchema;
            // TODO Do we care enough? // ?? await connection.GetSchema(false, cancellationToken).ConfigureAwait(false);

            BatchProcessArgs args =
                new BatchProcessArgs(schema?.ServerVersion ?? DatabaseSchema.MinimumSupportedServerVersion);

            // Build the batch SQL and get the parameters to the commands
            PreProcess(
                uid,
                connection,
                args,
                out CommandBehavior allBehavior);

            AsyncSemaphore[] semaphores = args.GetSemaphores();

            string state = null;
            int index = -1;
            string stateArgs = null;
            int actualIndex = 0;
            DbBatchDataReader commandReader = null;

            void MessageHandler(string message)
            {
                if (!TryParseInfoMessage(message, ref state, ref index, ref stateArgs)) return;

                if (commandReader != null)
                    commandReader.State = BatchReaderState.Finished;
            }

            // Wait the semaphores and setup the connection, command and reader
            using (await AsyncSemaphore.WaitAllAsync(cancellationToken, semaphores).ConfigureAwait(false))
            using (DbConnection dbConnection = await CreateOpenConnectionAsync(connection, uid, MessageHandler, cancellationToken)
                    .ConfigureAwait(false))
            using (DbCommand dbCommand = CreateCommand(args.SqlBuilder.ToString(), dbConnection, args.AllParameters.ToArray()))
            using (DbDataReader reader = await dbCommand.ExecuteReaderAsync(allBehavior, cancellationToken)
                .ConfigureAwait(false))
            {
                Debug.Assert(reader != null, "reader != null");

                object[] values = null;
                
                IEnumerator<SqlBatchCommand> commandEnumerator = GetEnumerator();
                while (commandEnumerator.MoveNext())
                {
                    SqlBatchCommand command = commandEnumerator.Current;
                    Debug.Assert(command != null, "command != null");
                    
                    try
                    {
                        using (commandReader = CreateReader(reader, command.CommandBehavior))
                        {
                            if (index >= actualIndex)
                                commandReader.State = BatchReaderState.Finished;

                            await command.HandleCommandAsync(
                                    commandReader,
                                    dbCommand,
                                    connectionIndex,
                                    cancellationToken)
                                .ConfigureAwait(false);

                            if (commandReader.IsOpen)
                            {
                                // Read the rest of the result sets until an info message finishes the reader
                                await commandReader.ReadTillClosedAsync(cancellationToken).ConfigureAwait(false);
                            }
                        }
                        commandReader = null;

                        // Check the end states
                        while (state != Constants.ExecuteState.End && index == actualIndex)
                        {
                            switch (state)
                            {
                                case Constants.ExecuteState.Output:
                                {
                                    // Get the expected output parameters for the command
                                    if (!args.CommandOutParams.TryGetValue(command, out var outs))
                                        throw new NotImplementedException(
                                            "Proper exception, unexpected output parameters");

                                    // No longer expect the output parameters
                                    args.CommandOutParams.Remove(command);

                                    if (!await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                                        throw new NotImplementedException("Proper exception, missing data");

                                    if (outs.Count != reader.VisibleFieldCount)
                                        throw new NotImplementedException("Proper exception, field count mismatch");

                                    // Expand the values buffer if needed
                                    if (values == null)
                                        values = new object[reader.VisibleFieldCount];
                                    else if (values.Length < reader.VisibleFieldCount)
                                        Array.Resize(ref values, reader.VisibleFieldCount);

                                    // Get the output values record
                                    reader.GetValues(values);

                                    // Set the output values
                                    for (int i = 0; i < outs.Count; i++)
                                    {
                                        Debug.Assert(outs[i].output != null, "outs[i].output != null");
                                        Debug.Assert(outs[i].param != null, "outs[i].param != null");

                                        outs[i].output.SetOutputValue(values[i], outs[i].param.BaseParameter);
                                    }

                                    if (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                                        throw new NotImplementedException("Proper exception, unexpected data");

                                    // TODO Do something with this?
                                    bool hasNext = await reader.NextResultAsync(cancellationToken)
                                        .ConfigureAwait(false);
                                    break;
                                }
                                case Constants.ExecuteState.Error:
                                    throw new NotImplementedException();
                                default:
                                    throw new NotImplementedException("Proper exception, unexpected state");
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        Debug.Assert(cancellationToken.IsCancellationRequested);
                        command.Result.SetCanceled(connectionIndex);

                        while (commandEnumerator.MoveNext())
                            commandEnumerator.Current.Result.SetCanceled(connectionIndex);
                        commandEnumerator.Dispose();

                        throw;
                    }
                    catch (Exception e)
                    {
                        command.Result.SetException(connectionIndex, e);

                        while (commandEnumerator.MoveNext())
                            commandEnumerator.Current.Result.SetCanceled(connectionIndex);
                        commandEnumerator.Dispose();

                        throw;
                    }
                    finally
                    {
                        command.Result.SetCompleted(connectionIndex);
                    }

                    actualIndex++;
                }
            }
        }

        /// <summary>
        /// Pre-processes the commands to be executed
        /// </summary>
        /// <param name="uid">The uid.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="allBehavior">All behavior.</param>
        private void PreProcess(
            [NotNull] string uid,
            [NotNull] string connectionString,
            [NotNull] BatchProcessArgs args,
            out CommandBehavior allBehavior)
        {
            ((IBatchItem)this).Process(
                uid,
                connectionString,
                args);

            allBehavior = args.Behavior;
        }

        /// <summary>
        /// Processes the batch to be executed.
        /// </summary>
        /// <param name="uid">The uid.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="args">The arguments.</param>
        void IBatchItem.Process(
            string uid,
            string connectionString,
            BatchProcessArgs args)
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

            bool hasTransaction = _transaction != TransactionType.None;
            bool hasTryCatch = _suppressErrors || hasTransaction;

            string tranName = null;
            int startIndex = 0;
            if (hasTryCatch)
            {
                if (hasTransaction)
                {
                    string isoLevel = GetIsolationLevelStr(_isolationLevel);
                    if (isoLevel == null)
                        throw new ArgumentOutOfRangeException(
                            nameof(IsolationLevel),
                            _isolationLevel,
                            string.Format(Resources.SqlBatch_Process_IsolationLevelNotSupported, _isolationLevel));
                    
                    // Set the isolation level and begin or save a transaction for the batch
                    tranName = "[" + ID.ToString("N") + "]";
                    args.SqlBuilder
                        .AppendLine()
                        .Append("SET TRANSACTION ISOLATION LEVEL ")
                        .Append(isoLevel)
                        .AppendLine(";")

                        .Append(args.InTransaction ? "SAVE" : "BEGIN")
                        .Append(" TRANSACTION ")
                        .Append(tranName)
                        .AppendLine(";");
                    args.TransactionStack.Push(tranName, isoLevel);
                }

                // Wrap the contents of the batch in a TRY ... CATCH block
                args.SqlBuilder
                    .AppendLine()
                    .AppendLine("BEGIN TRY")
                    .AppendLine()
                    .GetLength(out startIndex);
            }

            // Process the items in this batch
            foreach (IBatchItem item in _items)
                item.Process(uid, connectionString, args);

            if (hasTryCatch)
            {
                if (hasTransaction)
                {
                    args.TransactionStack.Pop(out string name, out _);
                    Debug.Assert(name == tranName);
                }

                // If the transaction type is Commit and this is a root transaction, commit it
                if (_transaction == TransactionType.Commit && !args.InTransaction)
                {
                    args.SqlBuilder
                        .AppendLine()
                        .Append("COMMIT TRANSACTION ")
                        .Append(tranName)
                        .AppendLine(";");
                }
                // If the transaction is Rollback, always roll it back
                else if (_transaction == TransactionType.Rollback)
                    args.SqlBuilder
                        .Append("ROLLBACK TRANSACTION ")
                        .Append(tranName)
                        .AppendLine(";");
                
                // End the TRY block and start the CATCH block
                args.SqlBuilder
                    .IndentRegion(startIndex)
                    .AppendLine()
                    .AppendLine("END TRY")
                    .AppendLine("BEGIN CATCH")
                    .GetLength(out startIndex);

                // If there is a transaction, roll it back if possible
                if (hasTransaction)
                {
                    if (args.InTransaction)
                        args.SqlBuilder
                            .AppendLine("IF XACT_STATE() <> -1 ")
                            .Append("\t");
                    
                    args.SqlBuilder
                            .Append("ROLLBACK TRANSACTION ")
                            .Append(tranName)
                            .AppendLine(";");
                }
                
                // Output an Error info message then select the error information
                AppendInfo(args.SqlBuilder, uid, Constants.ExecuteState.Error, "%d", null, "@CmdIndex")
                    .AppendLine(
                        "SELECT\tERROR_NUMBER(),\r\n\tERROR_SEVERITY(),\r\n\tERROR_STATE(),\r\n\tERROR_LINE(),\r\n\tISNULL(QUOTENAME(ERROR_PROCEDURE()),'NULL'),\r\n\tERROR_MESSAGE();");

                // If the error isnt being suppressed, rethrow it for any outer catches to handle it
                if (!_suppressErrors)
                {
                    if (args.ServerVersion.Major < 11)
                    {
                        // Cant rethrow the actual error, so raise a special error message
                        args.SqlBuilder
                            .Append("RAISERROR(")
                            .AppendVarChar($"{uid}{Constants.ExecuteState.ReThrow}:%d:")
                            .AppendLine(",16,0,@CmdIndex);");
                    }
                    else
                    {
                        args.SqlBuilder.AppendLine("THROW;");
                    }
                }

                // End the CATCH block
                args.SqlBuilder
                    .IndentRegion(startIndex)
                    .AppendLine()
                    .AppendLine("END CATCH")
                    .AppendLine();

                // Reset the isolation level
                if (!IsRoot && hasTransaction)
                {
                    if (!args.TransactionStack.TryPeek(out _, out string isoLevel))
                        isoLevel = GetIsolationLevelStr(IsolationLevel.Unspecified);

                    if (isoLevel != null)
                        args.SqlBuilder
                            .AppendLine()
                            .Append("SET TRANSACTION ISOLATION LEVEL ")
                            .Append(isoLevel)
                            .AppendLine(";");
                }
            }
            
            args.SqlBuilder
                .AppendLine()
                .AppendLine("/*")
                .Append(" * Ending batch ")
                .AppendLine(ID.ToString("D"))
                .AppendLine(" */");
        }

        private static string GetIsolationLevelStr(IsolationLevel isoLevel)
        {
            switch (isoLevel)
            {
                case IsolationLevel.ReadUncommitted:
                    return "READ UNCOMMITTED";
                case IsolationLevel.ReadCommitted:
                case IsolationLevel.Unspecified:
                    return "READ COMMITTED";
                case IsolationLevel.RepeatableRead:
                    return "REPEATABLE READ";
                case IsolationLevel.Serializable:
                    return "SERIALIZABLE";
                case IsolationLevel.Snapshot:
                    return "SNAPSHOT";
                case IsolationLevel.Chaos:
                default:
                    return null;
            }
        }

        /// <summary>
        /// Appends an info message to the SQL builder.
        /// </summary>
        /// <param name="sqlBuilder">The SQL builder.</param>
        /// <param name="uid">The uid.</param>
        /// <param name="state">The state.</param>
        /// <param name="index">The index.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="formatArgs">The format arguments.</param>
        /// <returns>
        /// The <paramref name="sqlBuilder" />
        /// </returns>
        [NotNull]
        internal static SqlStringBuilder AppendInfo(
            [NotNull] SqlStringBuilder sqlBuilder,
            [NotNull] string uid,
            [NotNull] string state,
            int index,
            string args = null,
            string formatArgs = null)
        {
            return AppendInfo(
                sqlBuilder,
                uid,
                state,
                index.ToString(),
                args,
                formatArgs);
        }

        /// <summary>
        /// Appends an info message to the SQL builder.
        /// </summary>
        /// <param name="sqlBuilder">The SQL builder.</param>
        /// <param name="uid">The uid.</param>
        /// <param name="state">The state.</param>
        /// <param name="index">The index.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="formatArgs">The format arguments.</param>
        /// <returns>
        /// The <paramref name="sqlBuilder" />
        /// </returns>
        [NotNull]
        internal static SqlStringBuilder AppendInfo(
            [NotNull] SqlStringBuilder sqlBuilder,
            [NotNull] string uid,
            [NotNull] string state,
            string index,
            string args = null,
            string formatArgs = null)
        {
            // TODO this would be provider specific

            Debug.Assert(!state.Contains(":"));
            sqlBuilder
                .Append("RAISERROR(")
                .AppendVarChar($"{uid}{state}:{index}:{args ?? string.Empty}")
                .Append(",4,0");
            if (formatArgs != null)
                sqlBuilder
                    .Append(',')
                    .Append(formatArgs);
            return sqlBuilder
                .AppendLine(");");
        }

        /// <summary>
        /// Attempts to parse an information message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="state">The state.</param>
        /// <param name="index">The index.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        private static bool TryParseInfoMessage([NotNull] string message, ref string state, ref int index, ref string args)
        {
            int ind1 = message.IndexOf(':');
            int ind1p1 = ind1 + 1;
            int ind2 = message.IndexOf(':', ind1p1);
            if (ind1 < 0 || ind2 < 0 || !ushort.TryParse(message.Substring(ind1p1, ind2 - ind1p1), out ushort parsedIndex))
                return false;

            state = message.Substring(0, ind1);
            index = parsedIndex;
            args = (ind2 + 1 >= message.Length) ? null : message.Substring(ind2 + 1);
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
            string connectionString,
            string uid,
            Action<string> messageHandler,
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
                dbConnection.Dispose();
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
            [NotNull] DbParameter[] parameters)
        {
            DbCommand command = connection.CreateCommand();
            try
            {
                Debug.Assert(command.Connection == connection);

#if DEBUG
                _sql = text;
#endif

                command.CommandText = text;
                command.CommandType = CommandType.Text;
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
        /// Creates the <see cref="DbBatchDataReader" /> for the underlying reader given.
        /// </summary>
        /// <param name="reader">The base reader.</param>
        /// <param name="commandBehavior">The command behavior.</param>
        /// <returns></returns>
        [NotNull]
        private DbBatchDataReader CreateReader([NotNull] DbDataReader reader, CommandBehavior commandBehavior)
        {
            switch (reader)
            {
                case SqlDataReader sqlReader:
                    return new SqlBatchDataReader(sqlReader, commandBehavior);
                default:
                    // Eventually will support multiple db providers
                    throw new NotSupportedException();
            }
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
                            throw new InvalidOperationException(Resources.SqlBatch_AddCommand_NoCommonConnections);
                        continue;
                    }

                    commonConnections.IntersectWith(
                        command.Program.Mappings
                            .Where(m => m.Connection.Weight > 0)
                            .Select(m => m.Connection));
                }

                if (commonConnections.Count < 1)
                    throw new InvalidOperationException(Resources.SqlBatch_AddCommand_NoCommonConnections);

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