using System;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Scheduling
{
    /// <summary>
    /// The result of a scheduled action.
    /// </summary>
    public interface IScheduledActionResult
    {
        /// <summary>
        /// When the execution was due.
        /// </summary>
        DateTime Due { get; }

        /// <summary>
        /// When the execution duration.
        /// </summary>
        TimeSpan Duration { get; }

        /// <summary>
        /// Any exception that was thrown by the function.
        /// </summary>
        [CanBeNull]
        Exception Exception { get; }

        /// <summary>
        /// When the execution actually started.
        /// </summary>
        DateTime Started { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="IScheduledActionResult"/> was cancelled.
        /// </summary>
        /// <remarks></remarks>
        bool Cancelled { get; }
    }
}