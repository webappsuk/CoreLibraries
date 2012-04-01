using System.Diagnostics;
using System.Reflection;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Relection
{
    /// <summary>
    /// Wrap <see cref="FieldInfo"/> with additional information.
    /// </summary>
    /// <remarks></remarks>
    [DebuggerDisplay("{Info} [Extended]")]
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
        [NotNull]
        public readonly FieldInfo Info;

        /// <summary>
        /// Initializes a new instance of the <see cref="Field"/> class.
        /// </summary>
        /// <param name="extendedType">Type of the extended.</param>
        /// <param name="info">The info.</param>
        /// <remarks></remarks>
        public Field([NotNull]ExtendedType extendedType, [NotNull]FieldInfo info)
        {
            ExtendedType = extendedType;
            Info = info;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="WebApplications.Utilities.Relection.Field"/> to <see cref="System.Reflection.FieldInfo"/>.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns>The result of the conversion.</returns>
        /// <remarks></remarks>
        public static implicit operator FieldInfo(Field field)
        {
            return field == null ? null : field.Info;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Reflection.FieldInfo"/> to <see cref="WebApplications.Utilities.Relection.Field"/>.
        /// </summary>
        /// <param name="fieldInfo">The field info.</param>
        /// <returns>The result of the conversion.</returns>
        /// <remarks></remarks>
        public static implicit operator Field(FieldInfo fieldInfo)
        {
            return fieldInfo == null
                       ? null
                       : ((ExtendedType) fieldInfo.DeclaringType).GetField(fieldInfo);
        }
    }
}