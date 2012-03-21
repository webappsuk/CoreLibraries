#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities
// File: DelegateSerializer.cs
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

using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace WebApplications.Utilities.Serialization
{
    /// <summary>
    ///   Allows serialization of delegates.
    /// </summary>
    /// <remarks>
    ///   Anonymous method serialization: (http://www.codeproject.com/KB/cs/AnonymousSerialization.aspx ).
    /// </remarks>
    [Serializable]
    public sealed class DelegateSerializer : ISerializable
    {
        private readonly Delegate _delegate;

        /// <summary>
        ///   Creates a new instance of <see cref="WebApplications.Utilities.Serialization.DelegateSerializer"/>
        /// </summary>
        /// <param name="delegate">The delegate to serialize.</param>
        /// <example>
        ///   <code>formatter.Serialize(stream, new DelegateSerializer(del));</code>
        /// </example>
        internal DelegateSerializer(Delegate @delegate)
        {
            _delegate = @delegate;
        }

        /// <summary>
        ///   Creates a new instance of <see cref="WebApplications.Utilities.Serialization.DelegateSerializer"/>
        /// </summary>
        /// <param name="info">
        ///   <para>The <see cref="System.Runtime.Serialization.SerializationInfo"/>.</para>
        ///   <para>This object stores all the required information for serializing/deserializing the delegate.</para>
        /// </param>
        /// <param name="context">
        ///   The <see cref="System.Runtime.Serialization.StreamingContext"/> for this serialization.
        /// </param>
        /// <example>
        ///   <code>formatter.Serialize(stream, new DelegateSerializer(info, context));</code>
        /// </example>
        internal DelegateSerializer(SerializationInfo info, StreamingContext context)
        {
            Type delType = (Type) info.GetValue("delegateType", typeof (Type));

            // If it's a "simple" delegate we just read it straight off
            if (info.GetBoolean("isSerializable"))
                _delegate = (Delegate) info.GetValue("delegate", delType);

            // Otherwise, we need to read its anonymous class
            else
            {
                MethodInfo method = (MethodInfo) info.GetValue("method", typeof (MethodInfo));

                AnonymousClassWrapper w = (AnonymousClassWrapper) info.GetValue("class", typeof (AnonymousClassWrapper));

                _delegate = Delegate.CreateDelegate(delType, w.Obj, method);
            }
        }

        #region ISerializable Members
        /// <summary>
        ///   Populates the provided <see cref="System.Runtime.Serialization.SerializationInfo"/> with the data needed to
        ///   serialize the object.
        /// </summary>
        /// <param name="info">
        ///   <para>The <see cref="System.Runtime.Serialization.SerializationInfo"/>.</para>
        ///   <para>This object stores all the required information for serializing/deserializing the delegate.</para>
        /// </param>
        /// <param name="context">
        ///   The <see cref="System.Runtime.Serialization.StreamingContext"/> for this serialization.
        /// </param>
        /// <exception cref="System.Security.SecurityException">The caller does not have the required permission.</exception>
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("delegateType", _delegate.GetType());

            // If it's a "simple" delegate we can serialize it directly
            if (_delegate == null || _delegate.Target == null ||
                (_delegate.Method.DeclaringType.GetCustomAttributes(typeof (SerializableAttribute), false).Length > 0))
            {
                info.AddValue("isSerializable", true);
                info.AddValue("delegate", _delegate);
            }
            // Otherwise, serialize the anonymous class
            else
            {
                info.AddValue("isSerializable", false);
                info.AddValue("method", _delegate.Method);
                info.AddValue(
                    "class", new AnonymousClassWrapper(_delegate.Method.DeclaringType, _delegate.Target));
            }
        }
        #endregion

        #region Nested type: AnonymousClassWrapper
        /// <summary>
        ///   A wrapper class used internally for serializing anonymous delegates.
        /// </summary>
        [Serializable]
        private class AnonymousClassWrapper : ISerializable
        {
            public readonly object Obj;
            private readonly Type _type;

            /// <summary>
            ///   Initializes a new instance of the <see cref="AnonymousClassWrapper"/> class.
            /// </summary>
            /// <param name="bclass">The declaring type.</param>
            /// <param name="bobject">The target object instance.</param>
            internal AnonymousClassWrapper(Type bclass, object bobject)
            {
                _type = bclass;
                Obj = bobject;
            }

            /// <summary>
            ///   Initializes a new instance of the <see cref="AnonymousClassWrapper"/> class.
            /// </summary>
            /// <param name="info">
            ///   <para>The <see cref="System.Runtime.Serialization.SerializationInfo"/>.</para>
            ///   <para>This object stores all the required information for serializing/deserializing the delegate.</para>
            /// </param>
            /// <param name="context">
            ///   The <see cref="System.Runtime.Serialization.StreamingContext"/> for this serialization.
            /// </param>
            internal AnonymousClassWrapper(SerializationInfo info, StreamingContext context)
            {
                _type = (Type) info.GetValue("classType", typeof (Type));
                if (_type == null)
                    return;

                Obj = Activator.CreateInstance(_type);

                foreach (FieldInfo field in _type.GetFields())
                {
                    // If the field is a delegate
                    if (typeof (Delegate).IsAssignableFrom(field.FieldType))
                        field.SetValue(
                            Obj,
                            ((DelegateSerializer) info.GetValue(field.Name, typeof (DelegateSerializer)))._delegate);
                    // If the field is an anonymous class
                    else if (!field.FieldType.IsSerializable)
                        field.SetValue(
                            Obj,
                            ((AnonymousClassWrapper) info.GetValue(field.Name, typeof (AnonymousClassWrapper))).Obj);
                    else
                        field.SetValue(Obj, info.GetValue(field.Name, field.FieldType));
                }
            }

            #region ISerializable Members
            /// <summary>
            ///   Populates the provided <see cref="System.Runtime.Serialization.SerializationInfo"/> with the data needed
            ///   to serialize the object.
            /// </summary>
            /// <param name="info">
            ///   <para>The <see cref="System.Runtime.Serialization.SerializationInfo"/>.</para>
            ///   <para>This object stores all the required information for serializing/deserializing the delegate.</para>
            /// </param>
            /// <param name="context">
            ///   The <see cref="System.Runtime.Serialization.StreamingContext"/> for this serialization.
            /// </param>
            /// <exception cref="System.Security.SecurityException">
            ///   The caller does not have the required permission.
            /// </exception>
            void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("classType", _type);

                if (_type == null)
                    return;

                foreach (FieldInfo field in _type.GetFields())
                {
                    // If the field is a delegate
                    if (typeof (Delegate).IsAssignableFrom(field.FieldType))
                        info.AddValue(field.Name, new DelegateSerializer((Delegate) field.GetValue(Obj)));
                    // If the field is an anonymous class
                    else if (!field.FieldType.IsSerializable)
                        info.AddValue(field.Name, new AnonymousClassWrapper(field.FieldType, field.GetValue(Obj)));
                    else
                        info.AddValue(field.Name, field.GetValue(Obj));
                }
            }
            #endregion
        }
        #endregion
    }
}