using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using Mono.Cecil;

namespace WebApplications.Utilities.Performance.Tools.PerfSetup
{
    /// <summary>
    /// Holds information about performance counter types.
    /// </summary>
    internal class PerformanceType
    {
        [NotNull]
        public static readonly ConcurrentDictionary<TypeReference, PerformanceType> _types = new ConcurrentDictionary<TypeReference, PerformanceType>();

        public readonly CounterCreationData[] CounterCreationData;

        /// <summary>
        /// Initializes a new instance of the <see cref="PerformanceType" /> class.
        /// </summary>
        /// <param name="typeReference">The type reference.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private PerformanceType(TypeReference typeReference)
        {
            TypeDefinition typeDefinition = typeReference.Resolve();

            // Confirm we descend from PerfCounter
            TypeDefinition baseType = typeDefinition;
            do
            {
                baseType = baseType.BaseType != null
                               ? baseType.BaseType.Resolve()
                               : null;

                if (baseType == null)
                    throw new InvalidOperationException(string.Format("The '{0}' type does not descend from 'WebApplications.Utilities.Performance.PerfCounter' and so cannot be used.",
                        typeDefinition.FullName));

                // Check for match
                if ((baseType.FullName == "WebApplications.Utilities.Performance.PerfCounter") &&
                    (baseType.Module.Assembly.Name.Name == "WebApplications.Utilities.Performance"))
                    break;
            } while (true);

            // Find CounterCreationData[] field
            FieldDefinition creationField = typeDefinition
                .Fields
                .SingleOrDefault(
                    fd => fd.IsStatic && fd.FieldType.FullName == "System.Diagnostics.CounterCreationData[]"
                );

            if (creationField == null)
                throw new InvalidOperationException(
                    string.Format(
                        "The '{0}' type does not contains a static field of type 'System.Diagnostics.CounterCreationData[]' so counters cannot be created.",
                        typeDefinition.FullName));

            // TODO Grab static constructor and load creation data!
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the specified type reference.
        /// </summary>
        /// <param name="typeReference">The type reference.</param>
        /// <returns>PerformanceType.</returns>
        [NotNull]
        public static PerformanceType Get(TypeReference typeReference)
        {
            return _types.GetOrAdd(typeReference, t => new PerformanceType(t));
        }
    }
}