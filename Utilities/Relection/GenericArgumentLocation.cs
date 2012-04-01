using System;

namespace WebApplications.Utilities.Relection
{
    /// <summary>
    /// The location of a generic argument.
    /// </summary>
    /// <remarks></remarks>
    [Flags]
    public enum GenericArgumentLocation
    {
        /// <summary>
        /// This is not a generic argument.
        /// </summary>
        None = 0,
        /// <summary>
        /// Generic argument is found on method.
        /// </summary>
        Method = 1,
        /// <summary>
        /// Generic argument is found on type.
        /// </summary>
        Type = 2,
        /// <summary>
        /// Generic argument is found on type or method (should only be used with a name).
        /// </summary>
        Any = 3
    }
}