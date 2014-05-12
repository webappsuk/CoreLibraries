using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Service
{
    /// <summary>
    /// Creates a server for a bi-directional pipe.
    /// </summary>
    public class NamedPipeServer : IDisposable
    {
        /// <summary>
        /// The name of the pipe.
        /// </summary>
        [NotNull]
        public readonly string Name;

        private NamedPipeServerStream _pipeServer;
        private TextReader _reader;
        private TextWriter _writer;

        /// <summary>
        /// Gets the reader.
        /// </summary>
        /// <value>The reader.</value>
        public TextReader Reader
        {
            get { return _reader; }
        }

        /// <summary>
        /// Gets the writer.
        /// </summary>
        /// <value>The writer.</value>
        public TextWriter Writer
        {
            get { return _writer; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedPipeServer"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public NamedPipeServer([NotNull] string name)
        {
            Contract.Requires<RequiredContractException>(name != null, "Parameter_Null");
            Name = name;

            // Create the new async pipe 
            _pipeServer = new NamedPipeServerStream(
                name,
                PipeDirection.InOut,
                1,
                PipeTransmissionMode.Byte,
                PipeOptions.Asynchronous);

            _reader = new StreamReader(_pipeServer);
            _writer = new StreamWriter(_pipeServer);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _reader = null;
            _writer = null;
            var ps = Interlocked.Exchange(ref _pipeServer, null);
            if (ps != null)
                ps.Dispose();
        }
    }
}
