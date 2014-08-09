using System;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Scheduling
{
    /// <summary>
    /// Schedule options.
    /// </summary>
    /// <remarks></remarks>
    [Flags]
    public enum ScheduleOptions
    {
        /// <summary>
        /// Default options.
        /// </summary>
        None = 0,
        /// <summary>
        /// When set allows executions to run concurrently; otherwise executions can only occur one
        /// at a time.
        /// </summary>
        AllowConcurrent,
        /// <summary>
        /// The value passed into <see cref="ISchedule.Next"/> is the previous due <see cref="DateTime"/>;
        /// otherwise it is the <see cref="DateTime"/> the previous execution completed.
        /// </summary>
        /// <remarks>
        /// In the event there has been no previous scheduled execution then this will be <see cref="DateTime.MinValue"/> or
        /// <see cref="DateTime.Now"/> respectively.
        /// </remarks>
        FromDue,
        /// <summary>
        /// When set, if any scheduled executions were missed they will be executed immediately.
        /// </summary>
        Catchup,
        /// <summary>
        /// Schedules should be persisted between scheduler activations.
        /// </summary>
        Persistent,
        /// <summary>
        /// Action execution is distributed across schedulers.
        /// </summary>
        Distributed
    }
}