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
using System.Collections.Concurrent;
using System.Runtime.Serialization;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Serialization
{
    /// <summary>
    ///   Extends serialization binder to improve performance and to also allow custom type mappings to be added easily
    ///   using <see cref="WebApplications.Utilities.Serialization.ExtendedSerializationBinder.MapType"/>.
    ///   Also simplifies type and assembly names making binding across multiple versions far more reliable.
    ///   (Includes fix for simplification of generic type names).
    /// </summary>
    [PublicAPI]
    public class ExtendedSerializationBinder : SerializationBinder
    {
        /// <summary>
        ///   The default extended serialization binder.
        /// </summary>
        [NotNull]
        public static readonly ExtendedSerializationBinder Default = new ExtendedSerializationBinder();

        /// <summary>
        ///   Maps old types to new types.
        /// </summary>
        [NotNull]
        private static readonly ConcurrentDictionary<string, Type> _typeMap = new ConcurrentDictionary<string, Type>();

        /// <summary>
        ///   Initializes a new instance of the <see cref="System.Runtime.Serialization.SerializationBinder"/> class.
        /// </summary>
        private ExtendedSerializationBinder()
        {
        }

        /// <summary>
        ///   Overrides the default binder, firstly by using a cache of <see cref="Type"/>s based on the assembly name and type name,
        ///   speeding up type binding, but also allowing easy overrides using
        ///   <see cref="WebApplications.Utilities.Serialization.ExtendedSerializationBinder.MapType"/>.
        /// </summary>
        /// <param name="assemblyName">
        ///   Specifies the <see cref="System.Reflection.Assembly"/> name of the serialized object.
        /// </param>
        /// <param name="typeName">
        ///   Specifies the <see cref="System.Type"/> name of the serialized object.
        /// </param>
        /// <returns>The mapped type.</returns>
        /// <exception cref="OverflowException">
        ///   The type map contains the <see cref="int.MaxValue">maximum</see> number of elements.
        /// </exception>
        /// <seealso cref="System.Reflection.Assembly"/>
        /// <seealso cref="System.Reflection.AssemblyName"/>
        public override Type BindToType(string assemblyName, string typeName)
        {
            if (assemblyName == null) throw new ArgumentNullException("assemblyName");
            if (typeName == null) throw new ArgumentNullException("typeName");
            return _typeMap.GetOrAdd(
                Reflection.SimplifiedTypeFullName(new[] { typeName, assemblyName }.JoinNotNullOrWhiteSpace(",")),
                Type.GetType);
        }

        /// <summary>
        ///   Maps an old type to a new type during serialization.
        /// </summary>
        /// <typeparam name="TNew">The new type.</typeparam>
        /// <param name="assemblyName">The assembly name of the old type.</param>
        /// <param name="typeName">The name of the old type.</param>
        /// <seealso cref="System.Reflection.Assembly"/>
        /// <seealso cref="System.Reflection.AssemblyName"/>
        public static void MapType<TNew>([NotNull] string assemblyName, [NotNull] string typeName)
        {
            MapType(assemblyName, typeName, typeof(TNew));
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
        public static void MapType([NotNull] string assemblyName, [NotNull] string typeName, [NotNull] Type newType)
        {
            if (assemblyName == null) throw new ArgumentNullException("assemblyName");
            if (typeName == null) throw new ArgumentNullException("typeName");
            _typeMap.AddOrUpdate(
                Reflection.SimplifiedTypeFullName(new[] { typeName, assemblyName }.JoinNotNullOrWhiteSpace(",")),
                newType,
                (n, t) => newType);
        }
    }
}