#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities
// File: XElementSurrogate.cs
// 
// This software, its object code and source code and all modifications made to
// the same (the “Software”) are, and shall at all times remain, the proprietary
// information and intellectual property rights of Web Applications (UK) Limited. 
// You are only entitled to use the Software as expressly permitted by Web
// Applications (UK) Limited within the Software Customisation and
// Licence Agreement (the “Agreement”).  Any copying, modification, decompiling,
// distribution, licensing, sale, transfer or other use of the Software other than
// as expressly permitted in the Agreement is expressly forbidden.  Web
// Applications (UK) Limited reserves its rights to take action against you and
// your employer in accordance with its contractual and common law rights
// (including injunctive relief) should you breach the terms of the Agreement or
// otherwise infringe its copyright or other intellectual property rights in the
// Software.
// 
// © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
#endregion

using System.Runtime.Serialization;
using System.Xml.Linq;

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
        void ISerializationSurrogate.GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            info.AddValue("data", obj == null ? null : ((XElement) obj).ToString(SaveOptions.DisableFormatting));
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
            object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            string o = info.GetString("data");

            return o == null ? null : XElement.Parse(o, LoadOptions.PreserveWhitespace);
        }
        #endregion
    }
}