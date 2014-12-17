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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Serialization
{
    /// <summary>
    ///   The <see cref="XmlFormatter"/> class implements a custom XML Formatter,
    ///   which uses the <see cref="System.Runtime.Serialization.ISerializable"/> interface.
    ///   The class implements the <see cref="System.Runtime.Serialization.IFormatter"/>
    ///   interface to serialize and deserialize the object to an XML representation.
    /// </summary>
    /// <remarks>
    ///   The class calls the methods of <see cref="System.Runtime.Serialization.ISerializable"/>
    ///   on the object if the object supports this interface. If not, the class will use Reflection
    ///   to examine the public fields and properties of the object.
    ///   When adding objects that inherit or implement <see cref="System.Collections.IList"/>,
    ///   <see cref="System.Collections.ICollection"/>, the element of the list should be passed
    ///   as an <see cref="Array">array</see> to <see cref="System.Runtime.Serialization.SerializationInfo"/>.
    /// </remarks>
    [UsedImplicitly]
    public sealed class XmlFormatter : IFormatter
    {
        /// <summary>
        ///   The serialization binder used to map object types to names.
        /// </summary>
        private SerializationBinder _binder;

        /// <summary>
        ///   Contains a list with objects that implement the
        ///   <see cref="System.Runtime.Serialization.IDeserializationCallback"/> interface.
        /// </summary>
        private List<object> _deserializationCallbackList;

        /// <summary>
        ///   Initializes a new instance of the <see cref="XmlFormatter"/> class. The serialization context is
        ///   set be a <see cref="System.Runtime.Serialization.StreamingContextStates">persisted store</see>.
        /// </summary>
        public XmlFormatter()
        {
            Context = new StreamingContext(StreamingContextStates.Persistence);
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="XmlFormatter"/> class.
        /// </summary>
        /// <param name="surrogateSelector">
        ///   <para>The surrogate selector.</para>
        ///   <para>This selects which surrogate to use in the serialization/deserialization process.</para>
        /// </param>
        /// <param name="streamingContext">The streaming context.</param>
        public XmlFormatter(ISurrogateSelector surrogateSelector, StreamingContext streamingContext)
        {
            SurrogateSelector = surrogateSelector;
            Context = streamingContext;
        }

        /// <summary>
        ///   A property which gets the deserialization call back list.
        ///   An empty list is returned if the deserialization callback list is <see langword="null"/>.
        /// </summary>
        private List<object> DeserializationCallBackList
        {
            get { return _deserializationCallbackList ?? (_deserializationCallbackList = new List<object>()); }
        }

        #region IFormatter Members
        /// <summary>
        ///   Gets or sets the type binder.
        /// </summary>
        public SerializationBinder Binder
        {
            get { return _binder ?? (_binder = ExtendedSerializationBinder.Default); }
            set { _binder = value; }
        }

        /// <summary>
        ///   Gets or sets the <see cref="StreamingContext"/>.
        /// </summary>
        public StreamingContext Context { get; set; }

        /// <summary>
        ///   Gets or sets the Surrogate Selector.
        /// </summary>
        public ISurrogateSelector SurrogateSelector { get; set; }

        /// <summary>
        ///   Serializes the specified <see cref="object"/> to the provided <see cref="Stream"/>.
        /// </summary>
        /// <param name="serializationStream">The stream to serialize to.</param>
        /// <param name="objectToSerialize">The object to serialize.</param>
        /// <exception cref="ArgumentException">
        ///   <paramref name="serializationStream"/> is <see langword="null"/>.
        /// </exception>
        public void Serialize(Stream serializationStream, object objectToSerialize)
        {
            if (objectToSerialize == null)
                return;

            if (serializationStream == null)
                throw new ArgumentException(Resources.XmlFormatter_Serialize_EmptyStream);

            XmlTextWriter writer = new XmlTextWriter(serializationStream, Encoding.UTF8);

            Serialize(writer, new FormatterConverter(), objectToSerialize.GetType().Name, objectToSerialize,
                      objectToSerialize.GetType());
        }

        /// <summary>
        ///   Deserializes an object from the passed stream.
        /// </summary>
        /// <param name="serializationStream">The stream to deserialize the object from.</param>
        /// <returns>The deserialized object.</returns>
        public object Deserialize(Stream serializationStream)
        {
            return Deserialize(serializationStream, null);
        }
        #endregion

        /// <summary>
        ///   Serializes the <see cref="object"/> using the passed <see cref="System.Xml.XmlTextWriter"/>.
        /// </summary>
        /// <param name="writer">The writer to use.</param>
        /// <param name="converter">
        ///   The <see cref="System.Runtime.Serialization.FormatterConverter">converter</see> to use when converting simple types.
        /// </param>
        /// <param name="elementName">The name of the element in the XML.</param>
        /// <param name="objectToSerialize">The object to serialize.</param>
        /// <param name="objectType">The type of the object to serialize.</param>
        private void Serialize(XmlTextWriter writer, FormatterConverter converter, string elementName,
                               object objectToSerialize, Type objectType)
        {
            // Include type information when using ISerializable.
            bool includeTypeInfo = (objectToSerialize is ISerializable);

            // Write the opening tag.
            writer.WriteStartElement(elementName);

            // If the name of the tag differs from the type name, include type information.
            if (elementName != objectType.Name || includeTypeInfo)
            {
                WriteAttributes(writer, objectType, false);
            }

            // for each serializable item in this object
            foreach (CustomSerializationEntry entry in GetMemberInfo(objectToSerialize, objectType, converter))
            {
                if (entry.ObjectType.IsPrimitive || entry.ObjectType == typeof (string) || entry.ObjectType.IsEnum ||
                    entry.ObjectType == typeof (DateTime))
                {
                    // simple type, directly write the value.
                    WriteValueElement(writer, entry);
                }
                else if (entry.ObjectType == typeof (Array) || entry.ObjectType.IsSubclassOf(typeof (Array)))
                {
                    // the type is an array type. iterate through the members
                    // get the type of the elements in the array
                    Type enumeratedType = entry.ObjectType.GetElementType();
                    // write the opening tag.
                    writer.WriteStartElement(entry.Name);
                    // write the attributes.
                    WriteAttributes(writer, enumeratedType, true);

                    // if the array is an null value, skip loop
                    if (entry.Value != null)
                    {
                        foreach (object item in (entry.Value as Array))
                        {
                            if (enumeratedType.IsPrimitive || enumeratedType == typeof (string))
                            {
                                // write the element
                                WriteValueElement("Value", writer, entry);
                            }
                            else
                            {
                                // serialize the object (recursive call)
                                Serialize(writer, converter, enumeratedType.Name, item, item.GetType());
                            }
                        }
                    }
                    // write closing tag
                    writer.WriteEndElement();
                }
                else if (entry.IsList)
                {
                    // write the opening tag
                    writer.WriteStartElement(entry.Name);

                    if (includeTypeInfo)
                    {
                        // write the attributes
                        WriteAttributes(writer, entry.ObjectType, false);
                    }

                    IList items = (IList) entry.Value;

                    // loop through the list
                    foreach (object item in items)
                    {
                        // serialize the object (recursive call)
                        Serialize(writer, converter, entry.ObjectType.GetElementType().Name, item, entry.ObjectType);
                    }
                    // write the closing tag
                    writer.WriteEndElement();
                }
                else
                {
                    // serialize the object (recursive call)
                    Serialize(writer, converter, entry.Name, entry.Value, entry.ObjectType);
                }
            }
            // write the closing tag
            writer.WriteEndElement();
            // flush the contents of the writer to the stream
            writer.Flush();
        }

        /// <summary>
        ///   Writes the Type and includeArrayAttribute attributes to the element
        /// </summary>
        /// <param name="writer">The XMLTextWriter to write to.</param>
        /// <param name="objectType">The type of the object.</param>
        /// <param name="includeArrayAttribute">Indicates whether to write the isArray attribute.</param>
        private void WriteAttributes(XmlTextWriter writer, Type objectType, bool includeArrayAttribute)
        {
            writer.WriteStartAttribute("type", "http://www.w3.org/2001/XMLSchema-instance");

            if (objectType.IsPrimitive || objectType == typeof (string))
                writer.WriteString(objectType.FullName);
            else
                writer.WriteString(objectType.Name);

            writer.WriteEndAttribute();

            if (includeArrayAttribute)
            {
                writer.WriteStartAttribute("includeArrayAttribute", "http://www.w3.org/2001/XMLSchema-instance");
                writer.WriteString("true");
                writer.WriteEndAttribute();
            }
        }

        /// <summary>
        ///   Writes a simple element to the writer.
        ///   The name of the element is the name of the object <see cref="Type"/>.
        /// </summary>
        /// <param name="writer">The XMLTextWriter to write to.</param>
        /// <param name="entry">
        ///   The <see cref="CustomSerializationEntry">entry</see> to write to the element.
        /// </param>
        private void WriteValueElement(XmlTextWriter writer, CustomSerializationEntry entry)
        {
            WriteValueElement(entry.Name, writer, entry);
        }

        /// <summary>
        ///   Writes a simple element to the writer. 
        /// </summary>
        /// <param name="tagName">The name of the tag to write.</param>
        /// <param name="writer">The <see cref="System.Xml.XmlTextWriter">XMLTextWriter</see> to write to.</param>
        /// <param name="entry">The entry to write to the element.</param>
        private void WriteValueElement(string tagName, XmlTextWriter writer, CustomSerializationEntry entry)
        {
            // write opening tag
            writer.WriteStartElement(tagName);

            if (entry.ObjectType.IsEnum)
            {
                writer.WriteValue(((int) entry.Value).ToString());
            }
            else
            {
                writer.WriteValue(entry.Value);
            }

            // write closing tag
            writer.WriteEndElement();
        }

        /// <summary>
        ///   Gets all the serializable members of an <see cref="object"/> and returns an enumerable collection.
        /// </summary>
        /// <param name="objectToSerialize">The object to get the members from.</param>
        /// <param name="objectToSerializeType">The type of the object.</param>
        /// <param name="converter">
        ///   The <see cref="System.Runtime.Serialization.FormatterConverter">converter</see> to use when converting simple types.</param>
        /// <returns>
        ///   An IEnumerable list of <see cref="CustomSerializationEntry"/> entries.
        /// </returns>
        /// <exception cref="SerializationException">
        ///   <paramref name="objectToSerializeType"/> is not serializable.
        /// </exception>
        private IEnumerable<CustomSerializationEntry> GetMemberInfo(object objectToSerialize, Type objectToSerializeType,
                                                                    FormatterConverter converter)
        {
            ISurrogateSelector selector1 = null;
            ISerializationSurrogate serializationSurrogate = null;
            SerializationInfo info = null;
            Type objectType = objectToSerializeType;

            // if the passed object is null, break the iteration.
            if (objectToSerialize == null)
                yield break;

            if ((SurrogateSelector != null) &&
                ((serializationSurrogate = SurrogateSelector.GetSurrogate(objectType, Context, out selector1)) != null))
            {
                // use a surrogate to get the members.
                info = new SerializationInfo(objectType, converter);

                if (!objectType.IsPrimitive)
                {
                    // get the data from the surrogate.
                    serializationSurrogate.GetObjectData(objectToSerialize, info, Context);
                }
            }
            else if (objectToSerialize is ISerializable)
            {
                // object implements ISerializable
                if (!objectType.IsSerializable)
                {
                    throw new SerializationException(
                        string.Format(Resources.XmlFormatter_GetMemberInfo_TypeNotSerializable,
                                      objectType.FullName));
                }

                info = new SerializationInfo(objectType, converter);
                // get the data using ISerializable.
                ((ISerializable) objectToSerialize).GetObjectData(info, Context);
            }

            if (info != null)
            {
                // either the surrogate provided the members, or the members were retrieved 
                // using ISerializable.
                // create the custom entries collection by copying all the members
                foreach (SerializationEntry member in info)
                {
                    CustomSerializationEntry entry = new CustomSerializationEntry(member.Name, member.ObjectType, false,
                                                                                  member.Value);
                    // yield return will return the entry now and return to this point when
                    // the enclosing loop (the one that contains the call to this method)
                    // request the next item.
                    yield return entry;
                }
            }
            else
            {
                // The item does not have a surrogate, nor does it implement ISerializable.
                // We use reflection to get the objects state.
                if (!objectType.IsSerializable)
                {
                    throw new SerializationException(
                        string.Format(Resources.XmlFormatter_GetMemberInfo_TypeNotSerializable,
                                      objectType.FullName));
                }
                // Get all serializable members
                MemberInfo[] members = FormatterServices.GetSerializableMembers(objectType, Context);

                foreach (MemberInfo member in members)
                {
                    if (CanSerialize(member))
                    {
                        // create the entry
                        CustomSerializationEntry entry = new CustomSerializationEntry(member.Name, member.ReflectedType,
                                                                                      false);

                        if (typeof (IList).IsAssignableFrom(entry.ObjectType))
                        {
                            entry.IsList = true;
                        }
                        // get the value of the member
                        entry.Value = GetMemberValue(objectToSerialize, member);
                        // yield return will return the entry now and return to this point when
                        // the enclosing loop (the one that contains the call to this method)
                        // request the next item.
                        yield return entry;
                    }
                }
            }
        }

        /// <summary>
        ///   Determines whether the passed member is <see cref="System.Reflection.FieldInfo.IsPublic">public</see> and writable.
        /// </summary>
        /// <param name="member">The member to investigate.</param>
        /// <returns>
        ///   Returns <see langword="true"/> if public and writable; otherwise returns <see langword="false"/>.
        /// </returns>
        private bool CanSerialize(MemberInfo member)
        {
            if (member.MemberType == MemberTypes.Field)
            {
                // if the member is a field, the member is writable when public.
                FieldInfo field = member as FieldInfo;

                if (field.IsPublic && !field.IsStatic)
                {
                    return true;
                }
            }
            else if (member.MemberType == MemberTypes.Property)
            {
                // if the member is a property, the member is writable when public set methods exist.
                PropertyInfo property = member as PropertyInfo;

                if (property.CanRead && property.CanWrite && property.GetGetMethod().IsPublic &&
                    !property.GetGetMethod().IsStatic && property.GetSetMethod().IsPublic &&
                    !property.GetSetMethod().IsStatic)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///   Gets the value of a member from the provided <see cref="object"/>.
        /// </summary>
        /// <param name="item">The object to get the member from.</param>
        /// <param name="member">The member to get the value from.</param>
        /// <returns>
        ///   The value of the member.
        /// </returns>
        /// <exception cref="NotSupportedException">
        ///   The <see cref="System.Reflection.MemberTypes">member type</see> cannot be serialized.
        ///   The type should be a Field or Property.
        /// </exception>
        private object GetMemberValue(object item, MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Property:
                    {
                        return item.GetType().GetProperty(member.Name).GetValue(item, null);
                    }
                case MemberTypes.Field:
                    {
                        return item.GetType().GetField(member.Name).GetValue(item);
                    }

                default:
                    {
                        throw new NotSupportedException(
                            string.Format(Resources.XmlFormatter_GetMemberValue_MemberCannotBeSerialized,
                                          member.MemberType.ToString()));
                    }
            }
        }

        /// <summary>
        ///   Deserializes an <see cref="object"/> from the given <see cref="Stream"/> for the given <see cref="Type"/>.
        /// </summary>
        /// <param name="serializationStream">The stream to read the object from.</param>
        /// <param name="objectType">The type of object to create.</param>
        /// <returns>The deserialized object.</returns>
        private object Deserialize(Stream serializationStream, Type objectType)
        {
            object deserialized;

            // create xml reader from stream
            using (XmlTextReader reader = new XmlTextReader(serializationStream))
            {
                DeserializationCallBackList.Clear();
                deserialized = InitializeObject(reader, new FormatterConverter(), objectType);
            }

            foreach (IDeserializationCallback callBack in DeserializationCallBackList)
            {
                callBack.OnDeserialization(null);
            }

            return deserialized;
        }

        /// <summary>
        ///   Reads an <see cref="object"/> from the XML and initializes it.
        /// </summary>
        /// <param name="reader">The XMLTextReader to read from.</param>
        /// <param name="converter">
        ///   The <see cref="System.Runtime.Serialization.FormatterConverter">converter</see> used to parse the values from the XML.
        /// </param>
        /// <param name="objectType">The type of the object to create.</param>
        /// <returns>The recreated object.</returns>
        /// <exception cref="SerializationException">
        ///   <para>The stream contained an element not found in <paramref name="objectType"/>.</para>
        ///   <para>-or</para>
        ///   <para>A List member in the object is a <see langword="null"/>.</para>
        ///   <para>-or-</para>
        ///   <para>More than one member was found for an XML tag.</para>
        /// </exception>
        private object InitializeObject(XmlTextReader reader, FormatterConverter converter, Type objectType)
        {
            Type actualType;
            ISurrogateSelector selector1 = null;
            ISerializationSurrogate serializationSurrogate = null;
            SerializationInfo info = null;
            object initializedObject = null;

            // check if a type attribute is present
            if (reader.HasAttributes)
            {
                // if so, get the type
                string actualTypeName = reader.GetAttribute("type", "http://www.w3.org/2001/XMLSchema-instance");
                actualType = Binder.BindToType("", actualTypeName);
            }
            else
            {
                // passed type is actual type.
                actualType = objectType;
            }

            // check whether a surrogate should be used, ISerializable is implemented or reflection is needed.
            if ((SurrogateSelector != null) &&
                ((serializationSurrogate = SurrogateSelector.GetSurrogate(actualType, Context, out selector1)) != null))
            {
                // use surrogate
                info = new SerializationInfo(actualType, converter);

                if (!actualType.IsPrimitive)
                {
                    // create a instance of the type.
                    initializedObject = FormatterServices.GetUninitializedObject(actualType);

                    // read the first element
                    reader.ReadStartElement();

                    while (reader.IsStartElement())
                    {
                        // determine type
                        string typeName = reader.GetAttribute("type", "http://www.w3.org/2001/XMLSchema-instance");
                        Type type = Binder.BindToType("", typeName);
                        // using ISerializable
                        info.AddValue(reader.Name, DetermineValue(reader, converter, type));
                        reader.ReadEndElement();
                    }
                    // use the surrogate to populate the instance
                    initializedObject = serializationSurrogate.SetObjectData(initializedObject, info, Context,
                                                                             SurrogateSelector);
                }
            }
            else if (typeof (ISerializable).IsAssignableFrom(actualType))
            {
                // The item implements ISerializable. Create a SerializationInfo object
                info = new SerializationInfo(actualType, converter);
                // Populate the collection
                PopulateSerializationInfo(reader, converter, actualType, info);
                // Get the specialized Serialization Constructor
                ConstructorInfo ctor =
                    actualType.GetConstructor(new[] {typeof (SerializationInfo), typeof (StreamingContext)});
                // Create the object
                initializedObject = ctor.Invoke(new object[] {info, Context});
            }
            else
            {
                // The item does not implement ISerializable. Use reflection to get public 
                // fields and properties.
                initializedObject = FormatterServices.GetUninitializedObject(actualType);

                List<MemberInfo> memberList = new List<MemberInfo>();
                List<object> valuesList = new List<object>();
                // read the first element.
                reader.ReadStartElement();

                while (reader.IsStartElement())
                {
                    // Get public fields and members of this type.
                    MemberInfo[] possibleMembers = actualType.GetMember(reader.Name,
                                                                        MemberTypes.Property | MemberTypes.Field,
                                                                        BindingFlags.Public | BindingFlags.DeclaredOnly |
                                                                        BindingFlags.Instance);

                    if (possibleMembers.Length < 1)
                        throw new SerializationException(
                            string.Format(Resources.XmlFormatter_InitializeObject_ElementNotFound, reader.Name));

                    if (possibleMembers.Length > 1)
                        throw new SerializationException(
                            string.Format(Resources.XmlFormatter_InitializeObject_MoreThanOneMemberFound,
                                          reader.Name));

                    if (typeof (IList).IsAssignableFrom(possibleMembers[0].ReflectedType))
                    {
                        // the type is a list, get the list from the initialized object.
                        IList list = GetMemberValue(initializedObject, possibleMembers[0]) as IList;

                        if (list == null)
                            throw new SerializationException(
                                string.Format(Resources.XmlFormatter_InitializeObject_MemberListIsNull,
                                              possibleMembers[0].DeclaringType.FullName,
                                              possibleMembers[0].Name));

                        // read the next element
                        reader.ReadStartElement();

                        while (reader.IsStartElement())
                        {
                            if (!reader.IsEmptyElement)
                            {
                                // Initialize the object (recursive call)
                                object listItem = InitializeObject(reader, converter,
                                                                   possibleMembers[0].ReflectedType.GetElementType());
                                list.Add(listItem);
                                reader.ReadEndElement();
                            }
                            else
                            {
                                reader.ReadStartElement();
                            }
                        }
                    }
                    else
                    {
                        // determine the value.
                        object value = DetermineValue(reader, converter, possibleMembers[0].ReflectedType);
                        memberList.Add(possibleMembers[0]);
                        valuesList.Add(value);
                    }
                }
                if (memberList.Count > 0)
                {
                    initializedObject = FormatterServices.PopulateObjectMembers(initializedObject, memberList.ToArray(),
                                                                                valuesList.ToArray());
                }
                reader.ReadEndElement();
            }

            if ((initializedObject as IDeserializationCallback) != null)
            {
                DeserializationCallBackList.Add(initializedObject);
            }
            return initializedObject;
        }

        /// <summary>
        ///   Populates the serialized members in the <see cref="System.Runtime.Serialization.SerializationInfo"/>.
        /// </summary>
        /// <param name="reader">The XMLReader to read from.</param>
        /// <param name="converter">
        ///   The <see cref="System.Runtime.Serialization.FormatterConverter">converter</see> used to parse the values from the XML.
        /// </param>
        /// <param name="actualType">The type of the object to create.</param>
        /// <param name="info">The <see cref="System.Runtime.Serialization.SerializationInfo"/> to populate.</param>
        private void PopulateSerializationInfo(XmlTextReader reader, FormatterConverter converter, Type actualType,
                                               SerializationInfo info)
        {
            // read the next element.
            reader.ReadStartElement();

            while (reader.IsStartElement())
            {
                Type type = null;

                // determine type
                string typeName = reader.GetAttribute("type", "http://www.w3.org/2001/XMLSchema-instance");
                bool isArray =
                    (reader.GetAttribute("includeArrayAttribute", "http://www.w3.org/2001/XMLSchema-instance") == "true")
                        ? true
                        : false;

                // If type is found in attribute, get the System.Type by using the Binder.
                if (typeName != null)
                    type = Binder.BindToType("", typeName);
                else
                    type = typeof (object);

                if (reader.IsEmptyElement)
                {
                    // if the type is an array type, place a empty array in the collection.
                    if (isArray)
                        info.AddValue(reader.Name,
                                      type.MakeArrayType().GetConstructor(new[] {typeof (Int32)}).Invoke(new object[]
                                                                                                             {0}));
                    else
                        info.AddValue(reader.Name, null);

                    reader.ReadStartElement();
                }
                else
                {
                    if (isArray)
                    {
                        // Item found is an array type.
                        string name = reader.Name;
                        // create a list of the type.
                        IList list =
                            (IList)
                            typeof (List<>).MakeGenericType(new[] {type}).GetConstructor(Type.EmptyTypes).Invoke(null);
                        // read the next element.
                        reader.ReadStartElement();

                        while (reader.IsStartElement())
                        {
                            if (!reader.IsEmptyElement)
                            {
                                // determine value
                                list.Add(DetermineValue(reader, converter, type));
                                reader.ReadEndElement();
                            }
                            else
                            {
                                reader.ReadStartElement();
                            }
                        }
                        // create an array of the element type and copy the list to the array.
                        Array array =
                            (Array)
                            type.MakeArrayType().GetConstructor(new[] {typeof (Int32)}).Invoke(new object[] {list.Count});
                        list.CopyTo(array, 0);
                        // add the array to the collection.
                        info.AddValue(name, array, type.MakeArrayType());

                        if (!reader.IsEmptyElement)
                            reader.ReadEndElement();
                        else
                            reader.ReadStartElement();
                    }
                    else
                    {
                        // type if not an array type.
                        // determine value and add it to the collection
                        info.AddValue(reader.Name, DetermineValue(reader, converter, type));
                        reader.ReadEndElement();
                    }
                }
            }
        }

        /// <summary>
        ///   Determines the value of an object.
        /// </summary>
        /// <param name="reader">The XMLTextReader the read from.</param>
        /// <param name="converter">
        ///   The <see cref="System.Runtime.Serialization.FormatterConverter">converter</see> used to parse the values from the XML.
        /// </param>
        /// <param name="objectType">The type of the object to create.</param>
        /// <returns>The value of the object.</returns>
        private object DetermineValue(XmlTextReader reader, FormatterConverter converter, Type objectType)
        {
            object parsedObject;

            // check if the value can be directly determined or that the type is a complex type.
            if (objectType.IsPrimitive || objectType == typeof (string) || objectType.IsEnum ||
                objectType == typeof (DateTime) || objectType == typeof (object))
            {
                // directly parse
                parsedObject = converter.Convert(reader.ReadString(), objectType);
            }
            else
            {
                // Initialize the object (recursive call)
                parsedObject = InitializeObject(reader, converter, objectType);
            }

            return parsedObject;
        }

        #region Nested type: CustomSerializationEntry
        /// <summary>
        ///   The <see cref="CustomSerializationEntry"/> mimics the <see cref="System.Runtime.Serialization.SerializationEntry"/>
        ///   class to make it possible to create our own entries. The class acts as a placeholder for a type, it's name and it's value.
        ///   This class is used in the <see cref="XmlFormatter"/> class to serialize objects.
        /// </summary>
        public struct CustomSerializationEntry
        {
            /// <summary>
            ///   Indicates whether the object is a list.
            /// </summary>
            private bool _isList;

            /// <summary>
            ///   The name of the object.
            /// </summary>
            private string _name;

            /// <summary>
            ///   The type of the object.
            /// </summary>
            private Type _objectType;

            /// <summary>
            ///   The value of the object.
            /// </summary>
            private object _value;

            /// <summary>
            ///   A constructor for creating a <see cref="CustomSerializationEntry"/> without a value. 
            ///   Value is set to <see langword="null"/>.
            /// </summary>
            /// <param name="name">The name of the object.</param>
            /// <param name="objectType">The <see cref="Type"/> of the object.</param>
            /// <param name="isList">Indicates whether the object is a list type.</param>
            public CustomSerializationEntry(string name, Type objectType, bool isList)
                : this(name, objectType, isList, null)
            {
            }

            /// <summary>
            ///   A constructor to create a <see cref="CustomSerializationEntry"/>. 
            /// </summary>
            /// <param name="name">The name of the object.</param>
            /// <param name="objectType">The <see cref="System.Type"/> of the object.</param>
            /// <param name="isList">Indicates whether the object is a list type.</param>
            /// <param name="value">The value of the object.</param>
            public CustomSerializationEntry(string name, Type objectType, bool isList, object value)
            {
                _name = name;
                _objectType = objectType;
                _isList = isList;
                _value = value;
            }

            /// <summary>
            ///   Gets the name of the object.
            /// </summary>
            public string Name
            {
                get { return _name; }
            }

            /// <summary>
            ///   Gets the <see cref="Type"/> of the object.
            /// </summary>
            public Type ObjectType
            {
                get { return _objectType; }
            }

            /// <summary>
            ///   Gets or sets the value contained in the object.
            /// </summary>
            public object Value
            {
                get { return _value; }
                set { _value = value; }
            }

            /// <summary>
            ///   Gets or sets whether the object is a list type.
            /// </summary>
            public bool IsList
            {
                get { return _isList; }
                set { _isList = value; }
            }
        }
        #endregion
    }
}