namespace WebApplications.Utilities.Database.Schema
{
    /// <summary>
    ///   Determines what should happen in the event the data being passed into a type cannot be fully represented by the type.
    /// </summary>
    public enum TypeConstraintMode
    {
        /// <summary>
        ///   Will truncate data to fit type without a warning/error.
        /// </summary>
        Silent,

        /// <summary>
        ///   Will log a warning when truncation or loss of precision occurs.
        /// </summary>
        Warn,

        /// <summary>
        ///   Will throw an error if truncation or loss of precision occurs.
        /// </summary>
        Error
    }
}
