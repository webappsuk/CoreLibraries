using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace WebApplications.Utilities.Initializer.Test
{
    /// <summary>
    /// Implements a logger for use by a project builder during testing.
    /// </summary>
    /// <remarks></remarks>
    public class UnitTestBuildLogger : Logger
    {
        /// <summary>
        /// The current build output.
        /// </summary>
        private readonly StringBuilder _output = new StringBuilder();

        /// <summary>
        /// Any errors
        /// </summary>
        private readonly StringBuilder _errors = new StringBuilder();

        /// <summary>
        /// Gets the output so far.
        /// </summary>
        /// <remarks></remarks>
        public string Output { get { return _output.ToString(); } }

        /// <summary>
        /// Gets the output so far.
        /// </summary>
        /// <remarks></remarks>
        public string Errors { get { return _errors.ToString(); } }

        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="parameters">The parameters.</param>
        /// <remarks></remarks>
        private void Log(string message, params object[] parameters)
        {
            if (String.IsNullOrWhiteSpace(message))
                return;

            string m = SafeFormat(message, parameters);
            Trace.WriteLine(m);
            _output.AppendLine(m);
        }
        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <param name="parameters">The parameters.</param>
        /// <remarks></remarks>
        private void LogError(string error, params object[] parameters)
        {
            if (String.IsNullOrWhiteSpace(error))
                error = "EMPTY ERROR REPORTED!";

            string m = SafeFormat(error, parameters);
            Log(m);
            _errors.AppendLine(m);
        }

        /// <summary>
        /// Safe format method.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private string SafeFormat(string message, params object[] parameters)
        {
            if ((parameters == null) || (parameters.Length < 1))
                return message;
            try
            {
                return String.Format(message, parameters);
            }
            catch
            {
                return message;
            }
        }

        /// <summary>
        /// Initialize is guaranteed to be called by MSBuild at the start of the build
        /// before any events are raised.
        /// </summary>
        public override void Initialize(IEventSource eventSource)
        {
            _output.Clear();
            _errors.Clear();

            // For brevity, we'll only register for certain event types. Loggers can also
            // register to handle TargetStarted/Finished and other events.
            eventSource.ProjectStarted += (s, e) => Log("Started building project {0}.", e.ProjectFile);
            eventSource.TaskStarted += (s, e) => Log("Started task {0}.", e.TaskName);
            eventSource.MessageRaised += (s, e) => Log(e.Message);
            eventSource.WarningRaised += (s, e) => Log(FormatWarningEvent(e));
            eventSource.ErrorRaised += (s, e) => LogError(FormatErrorEvent(e));
            eventSource.ProjectFinished += (s, e) => Log("Finished building project {0}.", e.ProjectFile);
        }
        
        /// <summary>
        /// Shutdown() is guaranteed to be called by MSBuild at the end of the build, after all 
        /// events have been raised.
        /// </summary>
        public override void Shutdown()
        {
        }
    }
}
