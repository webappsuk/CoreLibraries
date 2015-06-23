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

using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Serialization
{
    /// <summary>
    ///   Allows serialization of delegates.
    /// </summary>
    /// <remarks>
    ///   Anonymous method serialization: (http://www.codeproject.com/KB/cs/AnonymousSerialization.aspx ).
    /// </remarks>
    [Serializable]
    [PublicAPI]
    public sealed class DelegateSerializer : ISerializable
    {
        private const string DelegateTypeName = "delegateType";
        private const string IsCombinedName = "isCombined";
        private const string DelegatesName = "delegates";
        private const string IsSerializableName = "isSerializable";
        private const string DelegateName = "delegate";
        private const string MethodName = "method";
        private const string ClassName = "class";

        [NotNull]
        private Delegate _delegate;

        private DelegateSerializer[] _delegateSerializers;

        /// <summary>
        ///   Creates a new instance of <see cref="WebApplications.Utilities.Serialization.DelegateSerializer"/>
        /// </summary>
        /// <param name="delegate">The delegate to serialize.</param>
        /// <example>
        ///   <code>formatter.Serialize(stream, new DelegateSerializer(del));</code>
        /// </example>
        public DelegateSerializer([NotNull] Delegate @delegate)
        {
            if (@delegate == null) throw new ArgumentNullException("delegate");
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
        // ReSharper disable once NotNullMemberIsNotInitialized - In the case of combined delegates, _delegate will be set in OnDeserialize
        internal DelegateSerializer([NotNull] SerializationInfo info, StreamingContext context)
        {
            if (info == null) throw new ArgumentNullException("info");

            Type delType = (Type)info.GetValue(DelegateTypeName, typeof(Type));
            Debug.Assert(delType != null);

            // If the delegate is a combined delegate, we need to read the delegates it contains
            if (info.GetBoolean(IsCombinedName))
            {
                _delegateSerializers = (DelegateSerializer[])info.GetValue(DelegatesName, typeof(DelegateSerializer[]));
                Debug.Assert(_delegateSerializers != null);
            }
            else
            {
                // If it's a "simple" delegate we just read it straight off
                if (info.GetBoolean(IsSerializableName))
                {
                    Delegate @delegate = (Delegate)info.GetValue(DelegateName, delType);
                    Debug.Assert(@delegate != null);
                    _delegate = @delegate;
                }

                // Otherwise, we need to read its anonymous class
                else
                {
                    MethodInfo method = (MethodInfo)info.GetValue(MethodName, typeof(MethodInfo));
                    Debug.Assert(method != null);

                    AnonymousClassWrapper w = (AnonymousClassWrapper)info.GetValue(ClassName, typeof(AnonymousClassWrapper));
                    Debug.Assert(w != null);

                    _delegate = Delegate.CreateDelegate(delType, w.Obj, method);
                }
            }
        }

        /// <summary>
        /// Called when the class has been deserialized.
        /// </summary>
        /// <param name="context">The context.</param>
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (_delegateSerializers != null)
            {
                // ReSharper disable once AssignNullToNotNullAttribute, PossibleNullReferenceException
                _delegate = Delegate.Combine(_delegateSerializers.Select(ds => ds._delegate).ToArray());
                _delegateSerializers = null;
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
            info.AddValue(DelegateTypeName, _delegate.GetType());

            // If the delegate is a combined delegate, serialise the delegates separately
            Delegate[] allDelegates = _delegate.GetInvocationList();
            if (allDelegates.Length > 1)
            {
                info.AddValue(IsCombinedName, true);
                // ReSharper disable once AssignNullToNotNullAttribute
                info.AddValue(DelegatesName, allDelegates.Select(d => new DelegateSerializer(d)).ToArray(), typeof(DelegateSerializer[]));
            }
            else
            {
                Debug.Assert(_delegate.Method != null);
                Debug.Assert(_delegate.Method.DeclaringType != null);

                info.AddValue(IsCombinedName, false);

                // If it's a "simple" delegate we can serialize it directly
                if (_delegate.Target == null ||
                    (_delegate.Method.DeclaringType.GetCustomAttributes(typeof(SerializableAttribute), false).Length > 0))
                {
                    info.AddValue(IsSerializableName, true);
                    info.AddValue(DelegateName, _delegate);
                }
                // Otherwise, serialize the anonymous class
                else
                {
                    info.AddValue(IsSerializableName, false);
                    info.AddValue(MethodName, _delegate.Method);
                    info.AddValue(
                        ClassName,
                        new AnonymousClassWrapper(_delegate.Method.DeclaringType, _delegate.Target));
                }
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
            /// <summary>
            /// The class type info name. Note: the space at the start is to prevent conflicts with field names.
            /// </summary>
            private const string ClassTypeName = " classType";

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
            internal AnonymousClassWrapper([NotNull] SerializationInfo info, StreamingContext context)
            {
                if (info == null) throw new ArgumentNullException("info");

                _type = (Type)info.GetValue(ClassTypeName, typeof(Type));
                if (_type == null)
                    return;

                Obj = Activator.CreateInstance(_type);

                foreach (FieldInfo field in _type.GetFields())
                {
                    Debug.Assert(field != null);

                    // If the field is a delegate
                    if (typeof(Delegate).IsAssignableFrom(field.FieldType))
                        field.SetValue(
                            Obj,
                            // ReSharper disable once PossibleNullReferenceException
                            ((DelegateSerializer)info.GetValue(field.Name, typeof(DelegateSerializer)))._delegate);
                    // If the field is an anonymous class
                    else if (!field.FieldType.IsSerializable)
                        field.SetValue(
                            Obj,
                            // ReSharper disable once PossibleNullReferenceException
                            ((AnonymousClassWrapper)info.GetValue(field.Name, typeof(AnonymousClassWrapper))).Obj);
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
                info.AddValue(ClassTypeName, _type);

                if (_type == null)
                    return;

                foreach (
                    FieldInfo field in
                        _type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    Debug.Assert(field != null);

                    // If the field is a delegate
                    if (typeof(Delegate).IsAssignableFrom(field.FieldType))
                        // ReSharper disable once AssignNullToNotNullAttribute
                        info.AddValue(field.Name, new DelegateSerializer((Delegate)field.GetValue(Obj)));
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