#region © Copyright Web Applications (UK) Ltd, 2015.  All rights reserved.
// Copyright (c) 2015, Web Applications UK Ltd
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

using System.Runtime.Serialization;
using System.Xml.Linq;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Serialization
{
    /// <summary>
    ///   Serialization surrogate used to serialize/deserialize <see cref="XElement"/>s.
    /// </summary>
    public class XElementSurrogate : ISerializationSurrogate
    {
        #region ISerializationSurrogate Members
        /// <summary>
        ///   Populates the provided <see cref="System.Runtime.Serialization.SerializationInfo"/>
        ///   with the data needed to serialize the object.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="info">
        ///   <para>The <see cref="System.Runtime.Serialization.SerializationInfo"/> to populate with data.</para>
        ///   <para>This object stores all the required information for serializing/deserializing the object.</para>
        /// </param>
        /// <param name="context">
        ///   The destination (see <see cref="System.Runtime.Serialization.StreamingContext"/>) for this serialization.
        /// </param>
        /// <exception cref="System.Security.SecurityException">
        ///   The caller does not have the required permission.
        /// </exception>
        void ISerializationSurrogate.GetObjectData(
            object obj,
            [NotNull] SerializationInfo info,
            StreamingContext context)
        {
            info.AddValue("data", obj == null ? null : ((XElement)obj).ToString(SaveOptions.DisableFormatting));
        }

        /// <summary>
        ///   Populates the object using the information in the <see cref="System.Runtime.Serialization.SerializationInfo"/>.
        /// </summary>
        /// <param name="obj">The object to populate.</param>
        /// <param name="info">
        ///   <para>The information to populate the object.</para>
        ///   <para>This object stores all the required information for serializing/deserializing the object.</para>
        /// </param>
        /// <param name="context">
        ///   The source from which the object is deserialized (see <see cref="System.Runtime.Serialization.StreamingContext"/>).
        /// </param>
        /// <param name="selector">The surrogate selector where the search for a compatible surrogate begins.</param>
        /// <returns>
        ///   The populated deserialized object.
        ///   If the serialization info cannot be found then a <see langword="null"/> is returned.
        /// </returns>
        /// <exception cref="System.Security.SecurityException">
        ///   The caller does not have the required permission.
        /// </exception>
        object ISerializationSurrogate.SetObjectData(
            object obj,
            [NotNull] SerializationInfo info,
            StreamingContext context,
            ISurrogateSelector selector)
        {
            string o = info.GetString("data");

            return o == null ? null : XElement.Parse(o, LoadOptions.PreserveWhitespace);
        }
        #endregion
    }
}