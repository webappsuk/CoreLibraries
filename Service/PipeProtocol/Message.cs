using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using ProtoBuf;
using WebApplications.Utilities.Logging;

namespace WebApplications.Utilities.Service.PipeProtocol
{
    [ProtoContract(SkipConstructor = true)]
    [ProtoInclude(100, typeof(Request))]
    [ProtoInclude(200, typeof(Response))]
    [ProtoInclude(300, typeof(LogResponse))]
    [ProtoInclude(400, typeof(DisconnectResponse))]
    public abstract class Message
    {
    }

    [ProtoContract(SkipConstructor = true)]
    [ProtoInclude(100, typeof(CommandRequest))]
    [ProtoInclude(200, typeof(ConnectRequest))]
    [ProtoInclude(300, typeof(DisconnectRequest))]
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
    [ProtoInclude(100, typeof(CommandResponse))]
    [ProtoInclude(200, typeof(ConnectResponse))]
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
        [ProtoMember(1)]
        public readonly Log Log;

        public LogResponse(Log log)
        {
            Log = log;
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
    public class DisconnectResponse : Message
    {
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
        [ProtoMember(1)]
        public readonly string Result;

        public CommandResponse(Guid id, string result)
            : base(id)
        {
            Result = result;
        }
    }
}
