namespace WebApplications.Utilities.Service.PipeProtocol
{
    /// <summary>
    /// The PipeState indicates whether the connection
    /// </summary>
    public enum PipeState
    {
        /// <summary>
        /// The connection is starting up.
        /// </summary>
        Starting,

        /// <summary>
        /// The connection is open.
        /// </summary>
        Open,

        /// <summary>
        /// The connection is connected but we need a connect message.
        /// </summary>
        AwaitingConnect,

        /// <summary>
        /// The connection is connected.
        /// </summary>
        Connected,

        /// <summary>
        /// The connection is closed.
        /// </summary>
        Closed
    }
}