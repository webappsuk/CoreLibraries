using System.Data.SqlTypes;
using JetBrains.Annotations;

namespace WebApplications.Utilities
{
    /// <summary>
    /// Interface for an object that has an optional value.
    /// </summary>
    /// <remarks>
    /// This interface should only ever be implemented by <see cref="Optional{T}" />, hence it is internal.
    /// </remarks>
    internal interface IOptional : INullable
    {
        /// <summary>
        /// Gets a value indicating whether this instance is assigned.
        /// </summary>
        /// <value>
        ///     <see langword="true" /> if this instance is assigned; otherwise, <see langword="false" />.
        /// </value>
        [UsedImplicitly]
        bool IsAssigned { get; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        [UsedImplicitly]
        [CanBeNull]
        object Value { get; }
    }

    /// <summary>
    /// Interface for an object that has an optional value.
    /// </summary>
    /// <remarks>
    /// This interface should only ever be implemented by <see cref="Optional{T}" />, hence it is internal.
    /// </remarks>
    internal interface IOptional<out T> :
        IOptional
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        [UsedImplicitly]
        [CanBeNull]
        new T Value { get; }
    }
}
