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
using System.IO;
using JetBrains.Annotations;
using ProtoBuf;
using WebApplications.Utilities.Logging;

namespace WebApplications.Utilities.Service.PipeProtocol
{
    [ProtoContract(SkipConstructor = true)]
    [ProtoInclude(100, typeof (Request))]
    [ProtoInclude(200, typeof (Response))]
    [ProtoInclude(300, typeof (LogResponse))]
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
            using (MemoryStream ms = new MemoryStream(data))
                return Serializer.Deserialize<Message>(ms);
        }
    }

    [ProtoContract(SkipConstructor = true)]
    [ProtoInclude(100, typeof (CommandRequest))]
    [ProtoInclude(200, typeof (ConnectRequest))]
    [ProtoInclude(300, typeof (DisconnectRequest))]
    public abstract class Request : Message
    {
        [ProtoMember(1)]
        public readonly Guid ID;

        protected Request()
        {
            ID = Guid.NewGuid();
        }
    }

    [ProtoContract(SkipConstructor = true)]
    [ProtoInclude(100, typeof (CommandResponse))]
    [ProtoInclude(200, typeof (ConnectResponse))]
    [ProtoInclude(300, typeof (DisconnectResponse))]
    public abstract class Response : Message
    {
        [ProtoMember(1)]
        public readonly Guid ID;

        protected Response(Guid id)
        {
            ID = id;
        }
    }

    [ProtoContract(SkipConstructor = true)]
    public class LogResponse : Message
    {
        [ProtoMember(1, OverwriteList = true)]
        public readonly IEnumerable<Log> Logs;

        public LogResponse(IEnumerable<Log> logs)
        {
            Logs = logs;
        }
    }

    [ProtoContract(SkipConstructor = true)]
    public class ConnectRequest : Request
    {
        [ProtoMember(1)]
        public readonly string Description;

        public ConnectRequest(string description)
        {
            Description = description;
        }
    }

    [ProtoContract(SkipConstructor = true)]
    public class ConnectResponse : Response
    {
        [ProtoMember(1)]
        public readonly string ServiceName;

        public ConnectResponse(Guid id, [CanBeNull] string serviceName)
            : base(id)
        {
            ServiceName = serviceName;
        }
    }

    [ProtoContract(SkipConstructor = true)]
    public class DisconnectRequest : Request
    {
    }

    [ProtoContract(SkipConstructor = true)]
    public class DisconnectResponse : Response
    {
        public DisconnectResponse(Guid id)
            : base(id)
        {
        }
    }

    [ProtoContract(SkipConstructor = true)]
    public class CommandRequest : Request
    {
        [ProtoMember(1)]
        public readonly string CommandLine;

        public CommandRequest(string commandLine)
        {
            CommandLine = commandLine;
        }
    }

    [ProtoContract(SkipConstructor = true)]
    public class CommandResponse : Response
    {
        /// <summary>
        /// The sequence starts at 0 and continues until the final chunk which is set at -1 for completed, or -2 for error.
        /// </summary>
        [ProtoMember(1)]
        public readonly int Sequence;

        [ProtoMember(2)]
        public readonly string Chunk;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandResponse" /> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="sequence">The sequence.</param>
        /// <param name="chunk">The chunk.</param>
        public CommandResponse(Guid id, int sequence, string chunk)
            : base(id)
        {
            Sequence = sequence;
            Chunk = chunk;
        }
    }
}