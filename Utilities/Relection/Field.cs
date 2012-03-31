using System.Reflection;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Relection
{
    /// <summary>
    /// Wrap <see cref="FieldInfo"/> with additional information.
    /// </summary>
    /// <remarks></remarks>
    public class Field
    {
        /// <summary>
        /// The extended type.
        /// </summary>
        [NotNull]
        public readonly ExtendedType ExtendedType;

        /// <summary>
        /// The underlying <see cref="FieldInfo"/>.
        /// </summary>
        public readonly FieldInfo Info;

        /// <summary>
        /// Initializes a new instance of the <see cref="Field"/> class.
        /// </summary>
        /// <param name="extendedType">Type of the extended.</param>
        /// <param name="info">The info.</param>
        /// <remarks></remarks>
        public Field([NotNull]ExtendedType extendedType, FieldInfo info)
        {
            ExtendedType = extendedType;
            Info = info;
        }
    }
}