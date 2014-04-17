using System.ComponentModel;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Formatting
{
    /// <summary>
    /// Describes <see cref="Layout"/> alignments.
    /// </summary>
    [PublicAPI]
    public enum Alignment
    {
        /// <summary>
        /// The none alignment preserves spaces at the start of a line.
        /// </summary>
        [Description("The none alignment preserves spaces at the start of a line.")]
        [PublicAPI]
        None,
        /// <summary>
        /// The left alignment removes spaces at the start of a line.
        /// </summary>
        [Description("The left alignment removes spaces at the start of a line.")]
        [PublicAPI]
        Left,
        /// <summary>
        /// The centre alignment centres the text on each line.
        /// </summary>
        [Description("The centre alignment centres the text on each line.")]
        [PublicAPI]
        Centre,
        /// <summary>
        /// The right alignment removes spaces at the end of a line.
        /// </summary>
        [Description("The right alignment removes spaces at the end of a line.")]
        [PublicAPI]
        Right,
        /// <summary>
        /// The justify alignment removes spaces at the start and end of each line, and expands space inside the line.
        /// </summary>
        [Description("The justify alignment removes spaces at the start and end of each line, and expands space inside the line.")]
        [PublicAPI]
        Justify
    }
}