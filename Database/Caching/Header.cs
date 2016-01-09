#region © Copyright Web Applications (UK) Ltd, 2016.  All rights reserved.
// Copyright (c) 2016, Web Applications UK Ltd
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

using System.Data.Common;
using System.IO;
using WebApplications.Utilities.Annotations;
using WebApplications.Utilities.Database.Exceptions;

namespace WebApplications.Utilities.Database.Caching
{
    /// <summary>
    /// Holds header information.
    /// </summary>
    [PublicAPI]
    public class Header
    {
        /// <summary>
        /// The protocol version currently implemented by this code-base.
        /// </summary>
        public static ProtocolVersion CurrentProtocolVersion = ProtocolVersion.Initial;

        /// <summary>
        /// The current protocol version.
        /// </summary>
        public readonly ProtocolVersion ProtocolVersion;

        /// <summary>
        /// The current depth.
        /// </summary>
        public readonly int Depth;

        /// <summary>
        /// The number of records affected.
        /// </summary>
        public readonly int RecordsAffected;

        /// <summary>
        /// Initializes a new instance of the <see cref="Header" /> class.
        /// </summary>
        /// <param name="protocolVersion">The protocol version.</param>
        /// <param name="recordsAffected">The records affected.</param>
        /// <param name="depth">The depth.</param>
        private Header(ProtocolVersion protocolVersion, int recordsAffected, int depth)
        {
            ProtocolVersion = protocolVersion;
            RecordsAffected = recordsAffected;
            Depth = depth;
        }

        /// <summary>
        /// Reads a <see cref="Header" /> from the <paramref name="dataReader">specified dataReader</paramref>.
        /// </summary>
        /// <param name="dataReader">The dataReader.</param>
        /// <returns>A <see cref="TableDefinition" />.</returns>
        [NotNull]
        internal static Header Read([NotNull] DbDataReader dataReader)
            => new Header(CurrentProtocolVersion, dataReader.RecordsAffected, dataReader.Depth);

        /// <summary>
        /// Reads a <see cref="Header" /> from the <paramref name="stream">specified stream</paramref>.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>A <see cref="TableDefinition" />.</returns>
        [NotNull]
        internal static Header Read([NotNull] Stream stream)
        {
            ProtocolVersion protocolVersion = (ProtocolVersion)stream.ReadByte();

            // Sanity check, in future we can decide now to decode based on protocol version, for now we only have one!
            if (protocolVersion != CurrentProtocolVersion)
                throw new SqlCachingException(() => Resources.Header_Invalid_Protocol, protocolVersion);

            int recordsAffected = VariableLengthEncoding.DecodeInt(stream);
            int depth = VariableLengthEncoding.DecodeInt(stream);
            return new Header(protocolVersion, recordsAffected, depth);
        }

        /// <summary>
        /// Serializes this instance to the <paramref name="stream">specified stream</paramref>.
        /// </summary>
        /// <param name="stream">The stream.</param>
        internal void Serialize([NotNull] Stream stream)
        {
            stream.WriteByte((byte)ProtocolVersion);
            VariableLengthEncoding.Encode(RecordsAffected, stream);
            VariableLengthEncoding.Encode(Depth, stream);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString() => $"Cache protocol {ProtocolVersion} - {RecordsAffected} records affected, Depth = {Depth}.";
    }
}