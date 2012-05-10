using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Reflect
{
    /// <summary>
    /// Holds information about a generic argument.
    /// </summary>
    /// <remarks></remarks>
    [DebuggerDisplay("{Type} [{Location} position {Position}]")]
    public struct GenericArgument
    {
        /// <summary>
        /// The arguments location.
        /// </summary>
        public readonly GenericArgumentLocation Location;

        /// <summary>
        /// The arguments position.
        /// </summary>
        public readonly int Position;

        /// <summary>
        /// The argument type.
        /// </summary>
        [NotNull]
        public readonly Type Type;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericArgument"/> struct.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="position">The position.</param>
        /// <param name="type">The type.</param>
        /// <remarks></remarks>
        public GenericArgument(GenericArgumentLocation location, int position, [NotNull]Type type) : this()
        {
            Location = location;
            Position = position;
            Type = type;
        }
    }
}