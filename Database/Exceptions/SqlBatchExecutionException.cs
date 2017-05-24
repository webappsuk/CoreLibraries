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
using System.Linq.Expressions;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Logging;

namespace WebApplications.Utilities.Database.Exceptions
{
    /// <summary>
    ///   Exceptions thrown during the execution of a <see cref="WebApplications.Utilities.Database.SqlBatch"/>.
    /// </summary>
    [PublicAPI]
    public class SqlBatchExecutionException : LoggingException
    {
        /// <summary>
        /// The batch ID context key
        /// </summary>
        [NotNull]
        public static readonly string BatchIdContextKey = ContextReservations.BatchIdContextKey;

        private Guid _batchId;

        /// <summary>
        /// The name of the program in which the error occurred.
        /// </summary>
        public Guid BatchId
        {
            get
            {
                if (_batchId == System.Guid.Empty)
                    System.Guid.TryParse(this[BatchIdContextKey], out _batchId);
                return _batchId;
            }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="SqlProgramExecutionException"/> class.
        /// </summary>
        /// <param name="sqlBatch">The executing SQL batch.</param>
        /// <param name="innerException">The inner exception.</param>
        internal SqlBatchExecutionException([NotNull] SqlBatch sqlBatch, [NotNull] Exception innerException)
            : base(
                new LogContext()
                    .Set(ContextReservations.PrefixReservation, BatchIdContextKey, sqlBatch.ID.ToString("D")),
                innerException,
                () => Resources.SqlBatchExecutionException_ErrorOccurredDuringExecution,
                sqlBatch.ID.ToString("D"),
                innerException.Message)
        {
            _batchId = sqlBatch.ID;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlBatchExecutionException" /> class.
        /// </summary>
        /// <param name="sqlBatch">The executing SQL batch.</param>
        /// <param name="level">The log level.</param>
        /// <param name="resource">The resource.</param>
        /// <param name="parameters">The parameters.</param>
        internal SqlBatchExecutionException(
            [NotNull] SqlBatch sqlBatch,
            LoggingLevel level,
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
            : base(
                new LogContext()
                    .Set(ContextReservations.PrefixReservation, BatchIdContextKey, sqlBatch.ID.ToString("D")),
                level,
                resource,
                parameters)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlBatchExecutionException"/> class.
        /// </summary>
        /// <param name="sqlBatch">The executing SQL batch.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="resource">The resource.</param>
        /// <param name="parameters">The parameters.</param>
        internal SqlBatchExecutionException(
            [NotNull] SqlBatch sqlBatch,
            [CanBeNull] Exception innerException,
            [CanBeNull] Expression<Func<string>> resource,
            [CanBeNull] params object[] parameters)
            : base(
                new LogContext()
                    .Set(ContextReservations.PrefixReservation, BatchIdContextKey, sqlBatch.ID.ToString("D")),
                innerException,
                resource,
                parameters)
        {
        }
    }
}