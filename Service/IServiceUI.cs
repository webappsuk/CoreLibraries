using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WebApplications.Utilities.Formatting;

namespace WebApplications.Utilities.Service
{
    /// <summary>
    /// Interface IServiceUI defines an interface for a user interface that a service can interact with.
    /// </summary>
    public interface IServiceUI
    {
        /// <summary>
        /// Gets the writer for outputting information from the service.
        /// </summary>
        /// <value>The writer.</value>
        [NotNull]
        IColoredTextWriter Writer { get; }

        /// <summary>
        /// An observable of input commands.
        /// </summary>
        /// <returns>An observable of command lines.</returns>
        IObservable<string> GetNextLine();
    }
}