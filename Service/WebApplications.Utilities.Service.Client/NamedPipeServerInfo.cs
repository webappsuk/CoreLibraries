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
using System.Drawing;
using System.IO;
using JetBrains.Annotations;
using WebApplications.Utilities.Formatting;
using WebApplications.Utilities.Service.PipeProtocol;

namespace WebApplications.Utilities.Service.Client
{
    /// <summary>
    /// Class NamedPipeServerInfo.
    /// </summary>
    public class NamedPipeServerInfo : ResolvableWriteable
    {
        /// <summary>
        /// The verbose format
        /// </summary>
        [PublicAPI]
        [NotNull]
        public static readonly FormatBuilder VerboseFormat =
            new FormatBuilder()
                .AppendForegroundColor(ConsoleColor.Red)
                .AppendFormat("{IsValid:{Host}}")
                .AppendForegroundColor(ConsoleColor.Green)
                .AppendFormat("\t{IsValid:{Name}}")
                .AppendForegroundColor(ConsoleColor.White)
                .AppendFormat("\t{Pipe}")
                .AppendResetForegroundColor()
                .AppendLine();

        /// <summary>
        /// Whether this is a valid service pipe name.
        /// </summary>
        public readonly bool IsValid;

        /// <summary>
        /// The unique identifier
        /// </summary>
        public readonly Guid Guid;

        /// <summary>
        /// The host
        /// </summary>
        [PublicAPI]
        [NotNull]
        public readonly string Host;

        /// <summary>
        /// The service name.
        /// </summary>
        [PublicAPI]
        [NotNull]
        public readonly string Name;

        /// <summary>
        /// The full pipe path.
        /// </summary>
        [PublicAPI]
        [NotNull]
        public readonly string Pipe;

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedPipeServerInfo"/> class.
        /// </summary>
        /// <param name="pipe">The pipe.</param>
        internal NamedPipeServerInfo([CanBeNull] string pipe)
        {
            IsValid = false;
            Pipe = pipe;
            Host = ".";
            Name = "Invalid";
            if (string.IsNullOrWhiteSpace(pipe) ||
                !pipe.EndsWith(Common.NameSuffix))
                return;

            string directory = Path.GetDirectoryName(pipe);
            if (string.IsNullOrWhiteSpace(directory))
                return;

            string[] rootParts = directory.Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
            if (rootParts.Length != 2 ||
                !string.Equals(rootParts[1], "pipe", StringComparison.CurrentCultureIgnoreCase))
                return;

            string host = rootParts[0];
            if (string.IsNullOrWhiteSpace(host))
                return;

            Host = host;

            int nsl = Common.NameSuffix.Length;
            string name = Path.GetFileName(pipe);
            if (name.Length < 38 + nsl)
                // Guid is 36 characters, one _ characters, a name of at least 1 character and then the name suffix.
                return;

            Guid guid;
            if (!Guid.TryParseExact(name.Substring(0, 36), "D", out guid))
                return;
            Guid = guid;

            Name = name.Substring(37, name.Length - nsl - 37)
                .Replace('_', ' ');

            IsValid = true;

        }

        /// <summary>
        /// Resolves the specified tag.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="chunk">The chunk.</param>
        /// <returns>An object that will be cached unless it is a <see cref="T:WebApplications.Utilities.Formatting.Resolution" />.</returns>
        // ReSharper disable once CodeAnnotationAnalyzer
        public override object Resolve(FormatWriteContext context, FormatChunk chunk)
        {
            switch (chunk.Tag.ToLowerInvariant())
            {
                case "isvalid":
                    return IsValid ? Resolution.Empty : Resolution.Null;
                case "guid":
                    return Guid;
                case "host":
                    return Host;
                case "name":
                    return Name;
                case "pipe":
                    return Pipe;
                default:
                    return Resolution.Unknown;
            }
        }

        /// <summary>
        /// Gets the default format.
        /// </summary>
        /// <value>The default format.</value>
        public override FormatBuilder DefaultFormat
        {
            get { return VerboseFormat; }
        }
    }
}