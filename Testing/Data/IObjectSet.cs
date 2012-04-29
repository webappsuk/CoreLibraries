using System.Collections.Generic;
using System.Data;
using JetBrains.Annotations;

namespace WebApplications.Testing.Data
{
    /// <summary>
    /// Defines a collection of records.
    /// </summary>
    /// <remarks></remarks>
    public interface IObjectSet : IEnumerable<IObjectRecord>
    {
        /// <summary>
        /// Gets the definition.
        /// </summary>
        /// <value>The definition.</value>
        /// <remarks></remarks>
        [NotNull]
        RecordSetDefinition Definition { get; }
    }
}