namespace WebApplications.Utilities.Initializer
{
    /// <summary>
    /// Holds output.
    /// </summary>
    /// <remarks></remarks>
    internal struct Output
    {
        /// <summary>
        /// The message.
        /// </summary>
        public readonly string Message;

        /// <summary>
        /// The importance.
        /// </summary>
        public readonly OutputImportance Importance;

        /// <summary>
        /// Initializes a new instance of the <see cref="Output"/> struct.
        /// </summary>
        /// <param name="importance">The importance.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        /// <remarks></remarks>
        public Output(OutputImportance importance, string format, params object[] args)
        {
            Importance = importance;
            Message = string.Format(format, args);
        }
    }
}