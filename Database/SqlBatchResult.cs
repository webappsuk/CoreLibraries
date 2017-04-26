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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Database
{
    /// <summary>
    /// Represents the result of a batched <see cref="SqlProgram"/>
    /// </summary>
    public class SqlBatchResult
    {
        /// <summary>
        /// The command that this is the result of.
        /// </summary>
        private readonly SqlBatchCommand _command;

        private List<Task> _resultTasks;

        protected IReadOnlyList<Task> ResultTasks => _resultTasks;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlBatchResult"/> class.
        /// </summary>
        /// <param name="command">The command that this is the result of.</param>
        internal SqlBatchResult([NotNull] SqlBatchCommand command)
        {
            _command = command ?? throw new ArgumentNullException(nameof(command));
        }

        /// <summary>
        /// Adds the specified result to this result. 
        /// </summary>
        /// <param name="task">The result to add.</param>
        internal virtual void AddResultTask([NotNull] Task task)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));

            if (_resultTasks == null)
                _resultTasks = new List<Task>();
            _resultTasks.Add(task);
        }

        public Task GetResultAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            // TODO Execute the command if not already running

            Debug.Assert(_resultTasks != null);
            return _resultTasks.Count == 1 ? _resultTasks[0] : Task.WhenAll(_resultTasks);
        }
    }

    /// <summary>
    /// Represents the result of a batched <see cref="SqlProgram"/>
    /// </summary>
    /// <typeparam name="T">The type of the value returned by the program.</typeparam>
    public class SqlBatchResult<T> : SqlBatchResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlBatchResult"/> class.
        /// </summary>
        /// <param name="command">The command that this is the result of.</param>
        internal SqlBatchResult([NotNull] SqlBatchCommand command)
            : base(command)
        {
        }

        /// <summary>
        /// Adds the specified result to this result. 
        /// </summary>
        /// <param name="task">The result to add.</param>
        internal override void AddResultTask(Task task)
        {
            if (task is Task<T>)
                base.AddResultTask(task);
            else
                throw new ArgumentException($"Task should be of type {typeof(Task<T>)}", nameof(task));
        }

        // Use with ExecuteAsync
        public new Task<T> GetResultAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            // TODO Execute the command if not already running

            Debug.Assert(ResultTasks != null);
            if (ResultTasks.Count != 1)
                throw new InvalidOperationException();

            return (Task<T>)ResultTasks[0];
        }

        // Use with ExecuteAllAsync?
        public Task<IEnumerable<T>> GetResultsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            // TODO Execute the command if not already running

            Debug.Assert(ResultTasks != null);

            return Task.WhenAll(ResultTasks.Cast<Task<T>>())
                .ContinueWith(
                    t => (IEnumerable<T>)t.GetAwaiter().GetResult(),
                    TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.NotOnCanceled);
        }
    }
}