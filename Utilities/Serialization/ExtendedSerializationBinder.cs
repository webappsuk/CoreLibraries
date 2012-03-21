#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities
// File: ExtendedSerializationBinder.cs
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
using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Serialization
{
    /// <summary>
    ///   Extends serialization binder to improve performance and to also allow custom type mappings to be added easily
    ///   using <see cref="WebApplications.Utilities.Serialization.ExtendedSerializationBinder.MapType"/>.
    ///   Also simplifies type and assembly names making binding across multiple versions far more reliable.
    ///   (Includes fix for simplification of generic type names).
    /// </summary>
    public class ExtendedSerializationBinder : SerializationBinder
    {
        /// <summary>
        ///   The default extended serialization binder.
        /// </summary>
        public static readonly ExtendedSerializationBinder Default = new ExtendedSerializationBinder();

        /// <summary>
        ///   Maps old types to new types. Stored by the
        ///   <see cref="M:WebApplications.Utilities.Reflection.SimpleAssemblyQualifiedName(System.String,System.String)">simple
        ///   assembly qualified name</see> as the key.
        /// </summary>
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
            return _typeMap.GetOrAdd(Reflection.SimpleAssemblyQualifiedName(assemblyName, typeName), sn => Type.GetType(sn));
        }

        /// <summary>
        ///   Maps an old type to a new type during serialization.
        /// </summary>
        /// <typeparam name="TNew">The new type.</typeparam>
        /// <param name="assemblyName">The assembly name of the old type.</param>
        /// <param name="typeName">The name of the old type.</param>
        /// <seealso cref="System.Reflection.Assembly"/>
        /// <seealso cref="System.Reflection.AssemblyName"/>
        [UsedImplicitly]
        public static void MapType<TNew>([NotNull] string assemblyName, [NotNull] string typeName)
        {
            MapType(assemblyName, typeName, typeof (TNew));
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
            _typeMap.AddOrUpdate(Reflection.SimpleAssemblyQualifiedName(assemblyName, typeName), newType, (n, t) => newType);
        }
    }
}