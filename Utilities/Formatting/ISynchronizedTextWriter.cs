using System.IO;
using System.Threading;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Formatting
{
    /// <summary>
    /// Indicates the <see cref="TextWriter"/> is synchronized with a <see cref="SynchronizationContext"/>.
    /// </summary>
    public interface ISynchronizedTextWriter
    {
        /// <summary>
        /// The synchronization context.
        /// </summary>
        [NotNull]
        SynchronizationContext Context { get; }
    }
}