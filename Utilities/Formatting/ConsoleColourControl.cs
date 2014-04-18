using JetBrains.Annotations;
using WebApplications.Utilities.Enumerations;

namespace WebApplications.Utilities.Formatting
{
    /// <summary>
    /// Used for controlling console colour during formatting.
    /// </summary>
    internal class ConsoleColourControl
    {

        /// <summary>
        /// Used to indicate we need to reset the foreground and background.
        /// </summary>
        [NotNull]
        public static readonly ConsoleColourControl Reset = new ConsoleColourControl(TriState.Equal, null);

        /// <summary>
        /// Used to indicate we need to reset the foreground.
        /// </summary>
        [NotNull]
        public static readonly ConsoleColourControl ResetForeground = new ConsoleColourControl(TriState.Yes, null);

        /// <summary>
        /// Used to indicate we need to reset the foreground.
        /// </summary>
        [NotNull]
        public static readonly ConsoleColourControl ResetBackground = new ConsoleColourControl(TriState.No, null);

        /// <summary>
        /// The colour is for the foreground.
        /// </summary>
        [PublicAPI]
        public readonly TriState IsForeground;

        /// <summary>
        /// The new colour.
        /// </summary>
        [PublicAPI]
        [CanBeNull]
        public readonly string Colour;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleColourControl" /> class.
        /// </summary>
        /// <param name="isForeground">if set to <see langword="true" /> applies to the foreground..</param>
        /// <param name="colour">The colour.</param>
        internal ConsoleColourControl(TriState isForeground, [CanBeNull] string colour)
        {
            IsForeground = isForeground;
            Colour = colour;
        }
    }
}