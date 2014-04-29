using System.IO;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Formatting
{
    /// <summary>
    /// Indicates the object supports writing to a <see cref="TextWriter"/> using an optional <see cref="FormatBuilder"/> to specify the format.
    /// </summary>
    [PublicAPI]
    public interface IFormattableWrite : IWriteable
    {
        /// <summary>
        /// Writes this instance to the <see paramref="writer"/>, using the optional <see paramref="format"/>.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="format">The format.</param>
        void WriteTo([NotNull]TextWriter writer, [CanBeNull]FormatBuilder format);
    }
}