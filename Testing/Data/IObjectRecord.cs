using System.Data;
using WebApplications.Testing.Annotations;

namespace WebApplications.Testing.Data
{
    /// <summary>
    /// Interface that defines data records that implemented by the test system.
    /// </summary>
    public interface IObjectRecord : IDataRecord
    {
        /// <summary>
        /// Gets the record set definition.
        /// </summary>
        /// <value>The record set definition.</value>
        /// <remarks></remarks>
        [NotNull]
        RecordSetDefinition RecordSetDefinition { get; }
    }
}