using JetBrains.Annotations;

namespace WebApplications.Testing.Data
{
    public class ColumnDefinition
    {
        [NotNull]
        public readonly RecordSetDefinition RecordSetDefinition;
        
        [NotNull]
        public readonly string Name;
        
        [NotNull]
        public readonly MetaType MetaType;
    }
}