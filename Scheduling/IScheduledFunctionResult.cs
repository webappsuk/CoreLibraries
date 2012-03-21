namespace WebApplications.Utilities.Scheduling
{
    /// <summary>
    /// The result of a scheduled function.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <remarks></remarks>
    public interface IScheduledFunctionResult<out T> : IScheduledActionResult
    {
        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <remarks></remarks>
        T Result { get; }
    }
}