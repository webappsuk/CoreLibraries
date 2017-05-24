using System;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Database.Exceptions
{
    /// <summary>
    /// An exception thrown when a batch has not been run due to an unhandled exception in the batch that was executing the batch.
    /// </summary>
    /// <seealso cref="WebApplications.Utilities.Database.Exceptions.SqlProgramExecutionException" />
    public class SqlBatchNotRunException : SqlBatchExecutionException
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="SqlProgramExecutionException"/> class.
        /// </summary>
        /// <param name="sqlBatch">The executing SQL batch.</param>
        /// <param name="innerException">The inner exception.</param>
        internal SqlBatchNotRunException([NotNull] SqlBatch sqlBatch, [CanBeNull] Exception innerException)
            : base(
                sqlBatch,
                innerException,
                () => Resources.SqlBatchNotRunException_BatchNotRun,
                sqlBatch.ID.ToString("D"))
        {
        }
    }
}