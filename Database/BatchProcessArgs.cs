using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Threading;

namespace WebApplications.Utilities.Database
{
    /// <summary>
    /// Arguments for processing a <see cref="SqlBatch"/>.
    /// </summary>
    internal class BatchProcessArgs
    {
        [NotNull]
        public readonly Version ServerVersion;
        
        [NotNull]
        public readonly string InfoMessagePrefix;

        [NotNull]
        public readonly string ConnectionString;

        [NotNull]
        public readonly SqlStringBuilder SqlBuilder = new SqlStringBuilder();

        [NotNull]
        public readonly List<DbParameter> AllParameters = new List<DbParameter>();

        [NotNull]
        public readonly Dictionary<IOut, DbParameter> OutParameters = new Dictionary<IOut, DbParameter>();

        [NotNull]
        public readonly Dictionary<IOut, SqlBatchCommand> OutParameterCommands = new Dictionary<IOut, SqlBatchCommand>();

        [NotNull]
        public readonly Dictionary<SqlBatchCommand, IReadOnlyList<(DbBatchParameter param, IOut output)>> CommandOutParams =
            new Dictionary<SqlBatchCommand, IReadOnlyList<(DbBatchParameter, IOut)>>();

        [NotNull]
        public readonly HashSet<AsyncSemaphore> ConnectionSemaphores = new HashSet<AsyncSemaphore>();

        [NotNull]
        public readonly HashSet<AsyncSemaphore> LoadBalConnectionSemaphores = new HashSet<AsyncSemaphore>();

        [NotNull]
        public readonly HashSet<AsyncSemaphore> DatabaseSemaphores = new HashSet<AsyncSemaphore>();

        [NotNull]
        public readonly Stack<string, string, List<ushort>> TransactionStack = new Stack<string, string, List<ushort>>();

        public bool InTransaction => TransactionStack.Count > 0;

        public CommandBehavior Behavior = CommandBehavior.SequentialAccess;

        public ushort CommandIndex;

        public BatchProcessArgs([NotNull] Version serverVersion, [NotNull] string infoMessagePrefix, string connectionString)
        {
            ServerVersion = serverVersion;
            InfoMessagePrefix = infoMessagePrefix;
            ConnectionString = connectionString;
        }

        [NotNull]
        public AsyncSemaphore[] GetSemaphores()
        {
            AsyncSemaphore[] semaphores;
            // Concat the semaphores to a single array
            int semaphoreCount =
                ConnectionSemaphores.Count +
                LoadBalConnectionSemaphores.Count +
                DatabaseSemaphores.Count;
            if (semaphoreCount < 1)
                semaphores = Array<AsyncSemaphore>.Empty;
            else
            {
                semaphores = new AsyncSemaphore[semaphoreCount];
                int i = 0;

                // NOTE! Do NOT reorder these without also reordering the semaphores in SqlProgramCommand.WaitSemaphoresAsync
                foreach (AsyncSemaphore semaphore in ConnectionSemaphores)
                    semaphores[i++] = semaphore;
                foreach (AsyncSemaphore semaphore in LoadBalConnectionSemaphores)
                    semaphores[i++] = semaphore;
                foreach (AsyncSemaphore semaphore in DatabaseSemaphores)
                    semaphores[i++] = semaphore;
            }
            return semaphores;
        }
    }
}