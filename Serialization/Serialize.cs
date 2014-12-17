#region © Copyright Web Applications (UK) Ltd, 2012.  All rights reserved.
// Copyright (c) 2012, Web Applications UK Ltd
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Serialization
{
    /// <summary>
    ///   A class used to assist in implementing serialization code, adds transparent support for Xml serialization
    ///   (both <see cref="XmlElement"/> and <see cref="XElement"/>). Also adds easy support for mapping old types to new
    ///   types, using <see cref="WebApplications.Utilities.Serialization.ExtendedSerializationBinder.MapType"/>.
    /// </summary>
    /// <remarks>
    ///   A good article on surrogates can be found at (http://msdn.microsoft.com/en-gb/magazine/cc188950.aspx).
    /// </remarks>
    public static class Serialize
    {
        /// <summary>
        ///   Holds all of the serialization surrogates.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, Func<ISerializationSurrogate>> _surrogates =
            new ConcurrentDictionary<Type, Func<ISerializationSurrogate>>();

        /// <summary>
        ///   Loads standard serialization surrogates.
        /// </summary>
        /// <seealso cref="WebApplications.Utilities.Serialization.XElementSurrogate"/>
        /// <seealso cref="WebApplications.Utilities.Serialization.XmlElementSurrogate"/>
        static Serialize()
        {
            AddOrUpdateSurrogate<XmlElement, XmlElementSurrogate>();
            AddOrUpdateSurrogate<XElement, XElementSurrogate>();
        }

        /// <summary>
        ///   Gets the standard <see cref="System.Runtime.Serialization.SurrogateSelector">surrogate selector</see>,
        ///   which includes support for additional types. Additional types can be registered to the surrogate
        ///   selector with <see cref="WebApplications.Utilities.Serialization.Serialize.AddOrUpdateSurrogate"/>.
        /// </summary>
        /// <value>
        ///   A <see cref="System.Runtime.Serialization.SurrogateSelector"/> object which contains both the standard surrogates
        ///   and any additional surrogates added with the
        ///   <see cref="WebApplications.Utilities.Serialization.Serialize.AddOrUpdateSurrogate"/> in its list of checked surrogates.
        /// </value>
        [NotNull]
        [UsedImplicitly]
        public static SurrogateSelector SurrogateSelector
        {
            get
            {
                SurrogateSelector surrogateSelector = new SurrogateSelector();
                foreach (KeyValuePair<Type, Func<ISerializationSurrogate>> kvp in _surrogates)
                    surrogateSelector.AddSurrogate(kvp.Key, new StreamingContext(StreamingContextStates.All),
                                                   kvp.Value());
                return surrogateSelector;
            }
        }

        /// <summary>
        ///   Adds a new serialization surrogate.
        ///   If the surrogate already exists then it is instead updated.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <param name="surrogateType">
        ///   The type of the surrogate, which will be used to control the serialization and deserialization
        ///   for objects of type <typeparamref name="TInput"/>.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <para><paramref name="surrogateType"/> was not a concrete type.</para>
        ///   <para>-or-</para>
        ///   <para><paramref name="surrogateType"/> must implement
        ///   <see cref="System.Runtime.Serialization.ISerializationSurrogate"/>.</para>
        /// </exception>
        [UsedImplicitly]
        public static void AddOrUpdateSurrogate<TInput>(Type surrogateType)
        {
            AddOrUpdateSurrogate(typeof (TInput), surrogateType);
        }

        /// <summary>
        ///   Adds a new serialization surrogate.
        ///   If the surrogate already exists then it is instead updated.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <typeparam name="TSurrogate">
        ///   The type of the surrogate to call when serializing/deserializing objects of type <typeparamref name="TInput"/>.
        /// </typeparam>
        [UsedImplicitly]
        public static void AddOrUpdateSurrogate<TInput, TSurrogate>() where TSurrogate : ISerializationSurrogate
        {
            AddOrUpdateSurrogate(typeof (TInput), typeof (TSurrogate));
        }

        /// <summary>
        ///   Adds a new serialization surrogate.
        ///   If the surrogate already exists then it is instead updated.
        /// </summary>
        /// <param name="inputType">The type of the input.</param>
        /// <param name="surrogateType">
        ///   The type of the surrogate to call when serializing/deserializing objects of type <paramref name="inputType"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   <para><paramref name="inputType"/> is a <see langword="null"/>.</para>
        ///   <para>-or-</para>
        ///   <para><paramref name="surrogateType"/> is a <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <para><paramref name="surrogateType"/> was not a concrete type.</para>
        ///   <para>-or-</para>
        ///   <para><paramref name="surrogateType"/> must implement
        ///   <see cref="System.Runtime.Serialization.ISerializationSurrogate"/>.</para>
        /// </exception>
        [UsedImplicitly]
        public static void AddOrUpdateSurrogate([NotNull] Type inputType, [NotNull] Type surrogateType)
        {
            if (inputType == null)
                throw new ArgumentNullException("inputType");

            if (surrogateType == null)
                throw new ArgumentNullException("surrogateType");

            if (surrogateType.IsInterface)
                throw new ArgumentOutOfRangeException("surrogateType",
                                                      Resources.Serialize_AddOrUpdateSurrogate_MustBeConcrete);
            if (surrogateType.GetInterface(typeof (ISerializationSurrogate).FullName) == null)
                throw new ArgumentOutOfRangeException("surrogateType",
                                                      Resources.
                                                          Serialize_AddOrUpdateSurrogate_MustImplementISerializationSurrogate);

            // Create type constructor
            Func<ISerializationSurrogate> constructor = surrogateType.ConstructorFunc<ISerializationSurrogate>();
            _surrogates.AddOrUpdate(inputType, constructor, (t, f) => constructor);
        }

        /// <summary>
        ///   Maps an old type to a new type during serialization.
        /// </summary>
        /// <typeparam name="TNew">The new type.</typeparam>
        /// <param name="assemblyName">The name of the assembly.</param>
        /// <param name="typeName">The name of the type.</param>
        /// <exception cref="OverflowException">
        ///   The type map contains the <see cref="int.MaxValue">maximum</see> number of elements.
        /// </exception>
        /// <seealso cref="System.Reflection.Assembly"/>
        /// <seealso cref="System.Reflection.AssemblyName"/>
        [UsedImplicitly]
        public static void MapType<TNew>([NotNull] string assemblyName, [NotNull] string typeName)
        {
            ExtendedSerializationBinder.MapType(assemblyName, typeName, typeof (TNew));
        }

        /// <summary>
        ///   Maps an old type to a new type during serialization.
        /// </summary>
        /// <param name="assemblyName">The assembly name of the old type.</param>
        /// <param name="typeName">The name of the old type.</param>
        /// <param name="newType">The new type.</param>
        /// <exception cref="OverflowException">
        ///   The type map contains the <see cref="int.MaxValue">maximum</see> number of elements.
        /// </exception>
        /// <seealso cref="System.Reflection.Assembly"/>
        /// <seealso cref="System.Reflection.AssemblyName"/>
        [UsedImplicitly]
        public static void MapType([NotNull] string assemblyName, [NotNull] string typeName, [NotNull] Type newType)
        {
            ExtendedSerializationBinder.MapType(assemblyName, typeName, newType);
        }

        /// <summary>
        ///   Returns an extended <see cref="BinaryFormatter">binary formatter</see>
        ///   (can serialize XML correctly).
        /// </summary>
        /// <param name="context">The serialization context.</param>
        /// <param name="contextState">The state of the serialization context.</param>
        /// <returns>
        ///   A <see cref="BinaryFormatter">binary formatter</see> for serialization.
        /// </returns>
        [NotNull]
        [UsedImplicitly]
        public static BinaryFormatter GetFormatter([CanBeNull] object context = null,
                                                   StreamingContextStates contextState = StreamingContextStates.Other)
        {
            return new BinaryFormatter
                       {
                           SurrogateSelector = SurrogateSelector,
                           AssemblyFormat = FormatterAssemblyStyle.Simple,
                           Binder = ExtendedSerializationBinder.Default,
                           Context = new StreamingContext(contextState, context)
                       };
        }

        /// <summary>
        ///   Returns an extended <see cref="SoapFormatter">SOAP formatter</see>
        ///   (can serialize XML correctly).
        /// </summary>
        /// <param name="context">The serialization context.</param>
        /// <param name="contextState">The state of the serialization context.</param>
        /// <returns>
        ///   A <see cref="SoapFormatter">SOAP formatter</see> for serialization.
        /// </returns>
        [UsedImplicitly]
        [NotNull]
        [Obsolete(
            "See http://social.msdn.microsoft.com/forums/en-US/netfxremoting/thread/c4d67e93-fa11-46d9-aa4f-b63960f6af91/"
            )]
        public static SoapFormatter GetSoapFormatter([CanBeNull] object context = null,
                                                     StreamingContextStates contextState = StreamingContextStates.Other)
        {
            return new SoapFormatter
                       {
                           SurrogateSelector = SurrogateSelector,
                           AssemblyFormat = FormatterAssemblyStyle.Simple,
                           Binder = ExtendedSerializationBinder.Default,
                           Context = new StreamingContext(contextState, context)
                       };
        }

        /// <summary>
        /// Returns an extended <see cref="XmlFormatter">XML formatter</see>.
        /// </summary>
        /// <param name="context">The serialization context.</param>
        /// <param name="contextState">The state of the serialization context.</param>
        /// <returns>A <see cref="XmlFormatter">XML formatter</see>.</returns>
        [UsedImplicitly]
        [NotNull]
        public static XmlFormatter GetXmlFormatter([CanBeNull] object context = null,
                                                   StreamingContextStates contextState = StreamingContextStates.Other)
        {
            return new XmlFormatter
                       {
                           SurrogateSelector = SurrogateSelector,
                           Binder = ExtendedSerializationBinder.Default,
                           Context = new StreamingContext(contextState, context)
                       };
        }

        /// <summary>
        ///   Serializes the specified object tree to base64 encoded <see cref="string"/>.
        /// </summary>
        /// <param name="obj">The object tree to serialize.</param>
        /// <param name="context">The serialization context.</param>
        /// <param name="contextState">The state of the serialization context.</param>
        /// <returns><paramref name="obj"/> serialized to a <see cref="string"/>.</returns>
        [UsedImplicitly]
        [NotNull]
        public static string SerializeToString([NotNull] this object obj, [CanBeNull] object context = null,
                                               StreamingContextStates contextState = StreamingContextStates.Other)
        {
            StringBuilder s = new StringBuilder();
            s.AppendSerialization(obj, GetFormatter(context, contextState));
            return s.ToString();
        }

        /// <summary>
        ///   Serializes object tree to base64 encoded <see cref="string"/>.
        /// </summary>
        /// <param name="obj">The object tree to serialize.</param>
        /// <param name="formatter">The formatter for the serialized object.</param>
        /// <returns><paramref name="obj"/> serialized to a <see cref="string"/>.</returns>
        [UsedImplicitly]
        [NotNull]
        public static string SerializeToString([NotNull] this object obj, [NotNull] IFormatter formatter)
        {
            StringBuilder s = new StringBuilder();
            s.AppendSerialization(obj, formatter);
            return s.ToString();
        }

        /// <summary>
        ///   Serializes object tree to base64 encoded <see cref="string"/>.
        /// </summary>
        /// <typeparam name="T">The type of the objects in the enumeration.</typeparam>
        /// <param name="enumerable">The enumerable to serialize.</param>
        /// <param name="context">The serialization context.</param>
        /// <param name="contextState">The state of the serialization context.</param>
        /// <returns>
        ///   <paramref name="enumerable"/> serialized to a <see cref="string"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="enumerable"/> is a <see langword="null"/>
        /// </exception>
        [UsedImplicitly]
        [NotNull]
        public static string SerializeToString<T>([NotNull] this IEnumerable<T> enumerable,
                                                  [CanBeNull] object context = null,
                                                  StreamingContextStates contextState = StreamingContextStates.Other)
        {
            StringBuilder s = new StringBuilder();
            s.AppendSerialization(enumerable.ToList(), GetFormatter(context, contextState));
            return s.ToString();
        }

        /// <summary>
        /// Serializes object tree to base64 encoded <see cref="string"/>.
        /// </summary>
        /// <typeparam name="T">The type of the objects in the enumeration.</typeparam>
        /// <param name="enumerable">The enumerable to serialize.</param>
        /// <param name="formatter">The formatter for the serialized object.</param>
        /// <returns>
        ///   <paramref name="enumerable"/> serialized to a <see cref="string"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="enumerable"/> is a <see langword="null"/>
        /// </exception>
        [UsedImplicitly]
        [NotNull]
        public static string SerializeToString<T>([NotNull] this IEnumerable<T> enumerable,
                                                  [NotNull] IFormatter formatter)
        {
            StringBuilder s = new StringBuilder();
            s.AppendSerialization(enumerable.ToList(), formatter);
            return s.ToString();
        }

        /// <summary>
        ///   Serializes object tree to base64 encoded <see cref="System.Text.StringBuilder"/>.
        /// </summary>
        /// <param name="obj">The object tree to serialize.</param>
        /// <param name="formatter">The formatter for the serialized object.</param>
        /// <returns>
        ///   Base64 encoded <see cref="System.Text.StringBuilder"/>.
        /// </returns>
        [UsedImplicitly]
        [NotNull]
        public static StringBuilder SerializeToStringBuilder(
            [NotNull] this object obj,
            [NotNull] IFormatter formatter)
        {
            StringBuilder s = new StringBuilder();
            s.AppendSerialization(obj, formatter);
            return s;
        }

        /// <summary>
        ///   Serializes object tree to base64 encoded <see cref="System.Text.StringBuilder"/>.
        /// </summary>
        /// <param name="obj">The object tree to serialize.</param>
        /// <param name="context">The serialization context.</param>
        /// <param name="contextState">The state of the serialization context.</param>
        /// <returns>
        ///   Base64 encoded <see cref="System.Text.StringBuilder"/>.
        /// </returns>
        [UsedImplicitly]
        [NotNull]
        public static StringBuilder SerializeToStringBuilder(
            [NotNull] this object obj,
            [CanBeNull] object context = null,
            StreamingContextStates contextState = StreamingContextStates.Other)
        {
            StringBuilder s = new StringBuilder();
            s.AppendSerialization(obj, GetFormatter(context, contextState));
            return s;
        }

        /// <summary>
        ///   Serializes object tree to base64 encoded <see cref="System.Text.StringBuilder"/>.
        /// </summary>
        /// <typeparam name="T">The type of the objects in the enumeration.</typeparam>
        /// <param name="enumerable">The enumerable to serialize.</param>
        /// <param name="formatter">The formatter for the serialized object.</param>
        /// <returns>
        ///   Base64 encoded <see cref="System.Text.StringBuilder"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="enumerable"/> is a <see langword="null"/>
        /// </exception>
        [UsedImplicitly]
        [NotNull]
        public static StringBuilder SerializeToStringBuilder<T>(
            [NotNull] this IEnumerable<T> enumerable,
            [NotNull] IFormatter formatter)
        {
            StringBuilder s = new StringBuilder();
            s.AppendSerialization(enumerable.ToList(), formatter);
            return s;
        }

        /// <summary>
        ///   Serializes object tree to base64 encoded <see cref="System.Text.StringBuilder"/>.
        /// </summary>
        /// <typeparam name="T">The type of the objects in the enumeration.</typeparam>
        /// <param name="enumerable">The enumerable to serialize.</param>
        /// <param name="context">The serialization context.</param>
        /// <param name="contextState">The serialization context state.</param>
        /// <returns>
        ///   Base64 encoded <see cref="System.Text.StringBuilder"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="enumerable"/> is a <see langword="null"/>
        /// </exception>
        [UsedImplicitly]
        [NotNull]
        public static StringBuilder SerializeToStringBuilder<T>(
            [NotNull] this IEnumerable<T> enumerable,
            [CanBeNull] object context = null,
            StreamingContextStates contextState = StreamingContextStates.Other)
        {
            StringBuilder s = new StringBuilder();
            s.AppendSerialization(enumerable.ToList(), GetFormatter(context, contextState));
            return s;
        }

        /// <summary>
        ///   Serializes object tree to base64 encoded <see cref="System.Text.StringBuilder"/>.
        /// </summary>
        /// <param name="stringBuilder">The string builder.</param>
        /// <param name="obj">The object tree to serialize.</param>
        /// <param name="context">The serialization context.</param>
        /// <param name="contextState">The serialization context state.</param>
        /// <returns>
        ///   Base64 encoded <see cref="System.Text.StringBuilder"/>.
        /// </returns>
        [UsedImplicitly]
        [NotNull]
        public static StringBuilder AppendSerialization([NotNull] this StringBuilder stringBuilder,
                                                        [NotNull] Object obj,
                                                        [CanBeNull] object context = null,
                                                        StreamingContextStates contextState =
                                                            StreamingContextStates.Other)
        {
            return AppendSerialization(stringBuilder, obj, GetFormatter(context, contextState));
        }

        /// <summary>
        ///   Serializes object tree to base64 encoded <see cref="System.Text.StringBuilder"/>.
        /// </summary>
        /// <typeparam name="T">The type of the objects in the enumeration.</typeparam>
        /// <param name="stringBuilder">The string builder.</param>
        /// <param name="enumerable">The enumerable to serialize.</param>
        /// <param name="context">The serialization context.</param>
        /// <param name="contextState">The serialization context state.</param>
        /// <returns>
        ///   Base64 encoded <see cref="System.Text.StringBuilder"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="enumerable"/> is a <see langword="null"/>
        /// </exception>
        [UsedImplicitly]
        [NotNull]
        public static StringBuilder AppendSerialization<T>([NotNull] this StringBuilder stringBuilder,
                                                           [NotNull] IEnumerable<T> enumerable,
                                                           [CanBeNull] object context = null,
                                                           StreamingContextStates contextState =
                                                               StreamingContextStates.Other)
        {
            return AppendSerialization(stringBuilder, (object) enumerable.ToList(), GetFormatter(context, contextState));
        }

        /// <summary>
        ///   Serializes object tree to base64 encoded <see cref="StringBuilder"/>.
        /// </summary>
        /// <typeparam name="T">The type of the objects in the enumeration.</typeparam>
        /// <param name="stringBuilder">The string builder.</param>
        /// <param name="enumerable">The enumerable to serialize.</param>
        /// <param name="formatter">The formatter for the serialized object.</param>
        /// <returns>
        ///   Base64 encoded <see cref="System.Text.StringBuilder"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="enumerable"/> is a <see langword="null"/>
        /// </exception>
        [UsedImplicitly]
        [NotNull]
        public static StringBuilder AppendSerialization<T>([NotNull] this StringBuilder stringBuilder,
                                                           [NotNull] IEnumerable<T> enumerable,
                                                           [NotNull] IFormatter formatter)
        {
            return AppendSerialization(stringBuilder, (object) enumerable.ToList(), formatter);
        }

        /// <summary>
        ///   Serializes object tree to base64 encoded <see cref="System.Text.StringBuilder"/>.
        /// </summary>
        /// <param name="stringBuilder">The string builder.</param>
        /// <param name="obj">The object tree to serialize.</param>
        /// <param name="formatter">The formatter for the serialized object.</param>
        /// <returns>
        ///   Base64 encoded <see cref="System.Text.StringBuilder"/>.
        /// </returns>
        [UsedImplicitly]
        [NotNull]
        public static StringBuilder AppendSerialization([NotNull] this StringBuilder stringBuilder, object obj,
                                                        [NotNull] IFormatter formatter)
        {
            using (Stream serializationStream = new MemoryStream())
            {
                formatter.Serialize(serializationStream, obj);

                long count = 0;
                long length = serializationStream.Length;
                serializationStream.Seek(0, SeekOrigin.Begin);
                byte[] buffer = new byte[3];

                // Read into byte[]
                while (count < length)
                {
                    if (length - count < 3)
                        buffer = new byte[length - count];
                    serializationStream.Read(buffer, 0, buffer.Length);
                    stringBuilder.Append(Convert.ToBase64String(buffer));
                    count += 3;
                }
            }

            return stringBuilder;
        }

        /// <summary>
        ///   Serializes object tree to <see cref="byte"/> <see cref="Array"/>.
        /// </summary>
        /// <typeparam name="T">The type of the objects in the enumeration.</typeparam>
        /// <param name="enumerable">The enumerable to serialize.</param>
        /// <param name="context">The serialization context.</param>
        /// <param name="contextState">The serialization context state.</param>
        /// <returns>
        ///   <paramref name="enumerable"/> serialized to a <see cref="byte"/> <see cref="Array"/>.
        /// </returns>
        [UsedImplicitly]
        [NotNull]
        public static byte[] SerializeToByteArray<T>(
            [NotNull] this IEnumerable<T> enumerable,
            [CanBeNull] object context = null,
            StreamingContextStates contextState = StreamingContextStates.Other)
        {
            return SerializeToByteArray((object) enumerable.ToList(), context, contextState);
        }

        /// <summary>
        ///   Serializes object tree to <see cref="byte"/> <see cref="Array"/>.
        /// </summary>
        /// <param name="obj">The object tree to serialize.</param>
        /// <param name="context">The serialization context.</param>
        /// <param name="contextState">The serialization context state.</param>
        /// <returns>
        ///   <paramref name="obj"/> serialized to a <see cref="byte" /> <see cref="Array"/>.
        /// </returns>
        [UsedImplicitly]
        [NotNull]
        public static byte[] SerializeToByteArray(
            [NotNull] this object obj,
            [CanBeNull] object context = null,
            StreamingContextStates contextState = StreamingContextStates.Other)
        {
            return SerializeToByteArray(obj, GetFormatter(context, contextState));
        }

        /// <summary>
        ///   Serializes object tree to <see cref="byte"/> <see cref="Array"/>.
        /// </summary>
        /// <typeparam name="T">The type of the objects in the enumeration.</typeparam>
        /// <param name="enumerable">The enumerable to serialize.</param>
        /// <param name="formatter">The formatter for the serialized object.</param>
        /// <returns>
        ///   <paramref name="enumerable"/> serialized to a <see cref="byte"/> <see cref="Array"/>.
        /// </returns>
        [UsedImplicitly]
        [NotNull]
        public static byte[] SerializeToByteArray<T>(
            [NotNull] this IEnumerable<T> enumerable,
            [NotNull] IFormatter formatter)
        {
            return SerializeToByteArray((object) enumerable.ToList(), formatter);
        }


        /// <summary>
        ///   Serializes object tree to <see cref="byte"/> <see cref="Array"/>.
        /// </summary>
        /// <param name="obj">The object tree to serialize.</param>
        /// <param name="formatter">The formatter for the serialized object.</param>
        /// <returns>
        ///   <paramref name="obj"/> serialized to a <see cref="byte"/> <see cref="Array"/>.
        /// </returns>
        [UsedImplicitly]
        [NotNull]
        public static byte[] SerializeToByteArray(
            [NotNull] this object obj,
            [NotNull] IFormatter formatter)
        {
            byte[] output;
            using (Stream serializationStream = new MemoryStream())
            {
                formatter.Serialize(serializationStream, obj);
                output = new byte[serializationStream.Length];

                long count = 0;
                serializationStream.Seek(0, SeekOrigin.Begin);

                // Read into byte[]
                while (count < serializationStream.Length)
                    output[count++] = Convert.ToByte(serializationStream.ReadByte());
            }

            return output;
        }

        /// <summary>
        ///   Deserializes the data to an object tree.
        /// </summary>
        /// <typeparam name="T">The type of the deserialized objects.</typeparam>
        /// <param name="data">The data to deserialize.</param>
        /// <param name="context">The de-serialization context.</param>
        /// <param name="contextState">The de-serialization context state.</param>
        /// <returns>The deserialized object tree of type <typeparamref name="T"/>.</returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="data"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="FormatException">
        ///   <para>The length of <paramref name="data"/> is not zero or a multiple of 4 (ignoring whitespace).</para>
        ///   <para>-or-</para>
        ///   <para>The format of the <paramref name="data"/> is invalid, contains either a non-base-64 character,
        ///   more than two padding characters, or a non-white space-character among the padding characters.</para>
        /// </exception>
        [UsedImplicitly]
        [CanBeNull]
        public static T Deserialize<T>([NotNull] this string data, [CanBeNull] object context = null,
                                       StreamingContextStates contextState = StreamingContextStates.Other)
        {
            return Deserialize<T>(Convert.FromBase64String(data), GetFormatter(context, contextState));
        }

        /// <summary>
        ///   Deserializes the data to an object tree.
        /// </summary>
        /// <typeparam name="T">The type of the deserialized objects.</typeparam>
        /// <param name="data">The data to deserialize.</param>
        /// <param name="formatter">The formatter.</param>
        /// <returns>The deserialized object tree of type <typeparamref name="T"/>.</returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="data"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="FormatException">
        ///   <para>The length of <paramref name="data"/> is not zero or a multiple of 4 (ignoring whitespace).</para>
        ///   <para>-or-</para>
        ///   <para>The format of the <paramref name="data"/> is invalid, contains either a non-base-64 character,
        ///   more than two padding characters, or a non-white space-character among the padding characters.</para>
        /// </exception>
        [UsedImplicitly]
        [CanBeNull]
        public static T Deserialize<T>([NotNull] this string data, [NotNull] IFormatter formatter)
        {
            return Deserialize<T>(Convert.FromBase64String(data), formatter);
        }

        /// <summary>
        ///   Deserializes the data to an object tree.
        /// </summary>
        /// <typeparam name="T">The type of the deserialized objects.</typeparam>
        /// <param name="data">The data to deserialize.</param>
        /// <param name="context">The de-serialization context.</param>
        /// <param name="contextState">The de-serialization context state.</param>
        /// <returns>The deserialized object tree of type <typeparamref name="T"/>.</returns>
        [UsedImplicitly]
        [CanBeNull]
        public static T Deserialize<T>([NotNull] this byte[] data, [CanBeNull] object context = null,
                                       StreamingContextStates contextState = StreamingContextStates.Other)
        {
            return Deserialize<T>(data, GetFormatter(context, contextState));
        }

        /// <summary>
        ///   Deserializes the data to an object tree.
        /// </summary>
        /// <typeparam name="T">The type of the deserialized object.</typeparam>
        /// <param name="data">The data to deserialize.</param>
        /// <param name="formatter">The formatter.</param>
        /// <returns>The deserialized object tree of type <typeparamref name="T"/>.</returns>
        [UsedImplicitly]
        [CanBeNull]
        public static T Deserialize<T>([NotNull] this byte[] data, [NotNull] IFormatter formatter)
        {
            using (Stream serializationStream = new MemoryStream(data))
            {
                return (T) formatter.Deserialize(serializationStream);
            }
        }
    }
}