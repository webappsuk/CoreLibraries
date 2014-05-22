#region © Copyright Web Applications (UK) Ltd, 2014.  All rights reserved.
// Copyright (c) 2014, Web Applications UK Ltd
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of Web Applications UK Ltd nor the
//       names of its contributors may be used to endorse or promote products
//       derived from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL WEB APPLICATIONS UK LTD BE LIABLE FOR ANY
// DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using JetBrains.Annotations;
using ProtoBuf;
using WebApplications.Utilities.Logging;

namespace WebApplications.Utilities.Service.PipeProtocol
{
    /// <summary>
    /// Base message class, used for communication between named pipe client and server.
    /// </summary>
    [ProtoContract(SkipConstructor = true)]
    [ProtoInclude(100, typeof(Request))]
    [ProtoInclude(200, typeof(Response))]
    [ProtoInclude(300, typeof(LogResponse))]
    public abstract class Message
    {
        /// <summary>
        /// Gets the serialized form of a message.
        /// </summary>
        /// <returns>System.Byte[].</returns>
        [NotNull]
        public byte[] Serialize()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Serializer.Serialize(ms, this);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Deserializes the specified data to a message.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>Message.</returns>
        [NotNull]
        public static Message Deserialize([NotNull] byte[] data)
        {
            Contract.Requires(data != null);
            using (MemoryStream ms = new MemoryStream(data))
                // ReSharper disable once AssignNullToNotNullAttribute
                return Serializer.Deserialize<Message>(ms);
        }
    }

    /// <summary>
    /// Base request message, sent by a client to request something from the server.
    /// </summary>
    [ProtoContract(SkipConstructor = true)]
    [ProtoInclude(100, typeof(CommandRequest))]
    [ProtoInclude(200, typeof(ConnectRequest))]
    [ProtoInclude(300, typeof(DisconnectRequest))]
    public abstract class Request : Message
    {
        /// <summary>
        /// The request identifier.
        /// </summary>
        [ProtoMember(1)]
        public readonly Guid ID;

        /// <summary>
        /// Initializes a new instance of the <see cref="Request"/> class.
        /// </summary>
        protected Request()
        {
            ID = Guid.NewGuid();
        }
    }

    /// <summary>
    /// Base response message, sent by the server in response to requests from the client.
    /// </summary>
    [ProtoContract(SkipConstructor = true)]
    [ProtoInclude(100, typeof(CommandResponse))]
    [ProtoInclude(200, typeof(ConnectResponse))]
    [ProtoInclude(300, typeof(DisconnectResponse))]
    public abstract class Response : Message
    {
        /// <summary>
        /// The identifier of the <see cref="Request"/> this is a response to.
        /// </summary>
        [ProtoMember(1)]
        public readonly Guid ID;

        /// <summary>
        /// Initializes a new instance of the <see cref="Response"/> class.
        /// </summary>
        /// <param name="id">The identifier of the <see cref="Request"/> this is a response to.</param>
        protected Response(Guid id)
        {
            ID = id;
        }
    }

    /// <summary>
    /// Log response message, sent by the server when <see cref="Log">logs</see> are added.
    /// </summary>
    [ProtoContract(SkipConstructor = true)]
    public class LogResponse : Message
    {
        /// <summary>
        /// The logs.
        /// </summary>
        [ProtoMember(1, OverwriteList = true)]
        public readonly IEnumerable<Log> Logs;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogResponse"/> class.
        /// </summary>
        /// <param name="logs">The logs.</param>
        public LogResponse([NotNull] IEnumerable<Log> logs)
        {
            Contract.Requires(logs != null);
            Logs = logs;
        }
    }

    /// <summary>
    /// Connection request message, sent by the client when it wants to connect to the server.
    /// </summary>
    [ProtoContract(SkipConstructor = true)]
    public class ConnectRequest : Request
    {
        /// <summary>
        /// The description of the client.
        /// </summary>
        [ProtoMember(1)]
        public readonly string Description;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectRequest"/> class.
        /// </summary>
        /// <param name="description">The description.</param>
        public ConnectRequest([NotNull] string description)
        {
            Contract.Requires(description != null);
            Description = description;
        }
    }

    /// <summary>
    /// Connection response message, sent by the server when a client has connected.
    /// </summary>
    [ProtoContract(SkipConstructor = true)]
    public class ConnectResponse : Response
    {
        /// <summary>
        /// The service name.
        /// </summary>
        [ProtoMember(1)]
        public readonly string ServiceName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectResponse"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="serviceName">Name of the service.</param>
        public ConnectResponse(Guid id, [CanBeNull] string serviceName)
            : base(id)
        {
            ServiceName = serviceName;
        }
    }

    /// <summary>
    /// Disconnect request message, sent by a client when it wants to disconnect from the server.
    /// </summary>
    [ProtoContract(SkipConstructor = true)]
    public class DisconnectRequest : Request
    {
    }

    /// <summary>
    /// Disconnect response message, sent by the server when a client is being disconnected.
    /// </summary>
    [ProtoContract(SkipConstructor = true)]
    public class DisconnectResponse : Response
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisconnectResponse"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public DisconnectResponse(Guid id)
            : base(id)
        {
        }
    }

    /// <summary>
    /// Command request message, sent by a client.
    /// </summary>
    [ProtoContract(SkipConstructor = true)]
    public class CommandRequest : Request
    {
        /// <summary>
        /// The command line to execute.
        /// </summary>
        [ProtoMember(1)]
        public readonly string CommandLine;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandRequest"/> class.
        /// </summary>
        /// <param name="commandLine">The command line to execute.</param>
        public CommandRequest([NotNull] string commandLine)
        {
            Contract.Requires(commandLine != null);
            CommandLine = commandLine;
        }
    }

    /// <summary>
    /// Command response message, sent by the server in response to a <see cref="CommandRequest"/>.
    /// </summary>
    [ProtoContract(SkipConstructor = true)]
    public class CommandResponse : Response
    {
        /// <summary>
        /// The sequence starts at 0 and continues until the final chunk which is set at -1 for completed, or -2 for error.
        /// </summary>
        [ProtoMember(1)]
        public readonly int Sequence;

        /// <summary>
        /// The response chunk.
        /// </summary>
        [ProtoMember(2)]
        public readonly string Chunk;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandResponse" /> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="sequence">The sequence.</param>
        /// <param name="chunk">The chunk.</param>
        public CommandResponse(Guid id, int sequence, [NotNull] string chunk)
            : base(id)
        {
            Contract.Requires(chunk != null);
            Sequence = sequence;
            Chunk = chunk;
        }
    }
}