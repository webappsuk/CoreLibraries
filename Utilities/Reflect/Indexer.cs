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
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Threading;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Reflect
{
    /// <summary>
    ///   Wraps a <see cref="System.Reflection.PropertyInfo"/> that actually refers to an indexer with accessors.
    /// </summary>
    /// <remarks></remarks>
    [DebuggerDisplay("{Info} [Extended]")]
    public class Indexer : Property, ISignature
    {
        /// <summary>
        /// Creates index parameters on demand.
        /// </summary>
        [NotNull] private readonly Lazy<ParameterInfo[]> _indexParameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="Property"/> class.
        /// </summary>
        /// <param name="extendedType">The extended type.</param>
        /// <param name="info">The <see cref="System.Reflection.PropertyInfo">property info</see>.</param>
        /// <remarks></remarks>
        internal Indexer([NotNull] ExtendedType extendedType, [NotNull] PropertyInfo info) : base(extendedType, info)
        {
            Contract.Assert(extendedType.DefaultMember == info.Name);
            _indexParameters = new Lazy<ParameterInfo[]>(info.GetIndexParameters, LazyThreadSafetyMode.PublicationOnly);
        }

        /// <summary>
        /// Gets the index parameters (if this is an index);
        /// </summary>
        /// <remarks></remarks>
        [NotNull]
        public IEnumerable<ParameterInfo> IndexParameters
        {
            get { return _indexParameters.Value; }
        }

        #region ISignature Members
        /// <inheritdoc/>
        public Type DeclaringType
        {
            get { return ExtendedType.Type; }
        }

        /// <inheritdoc/>
        public IEnumerable<GenericArgument> TypeGenericArguments
        {
            get { return ExtendedType.GenericArguments; }
        }

        /// <inheritdoc/>
        public IEnumerable<GenericArgument> SignatureGenericArguments
        {
            get { return Enumerable.Empty<GenericArgument>(); }
        }

        /// <inheritdoc/>
        public IEnumerable<Type> ParameterTypes
        {
            get
            {
                Contract.Assert(_indexParameters.Value != null);
                return _indexParameters.Value.Select(p => p.ParameterType);
            }
        }

        /// <inheritdoc/>
        public Type ReturnType
        {
            get { return Info.PropertyType; }
        }

        /// <inheritdoc/>
        ISignature ISignature.Close(Type[] typeClosures, Type[] signatureClosures)
        {
            // Indexers don't support signature closures.
            return signatureClosures.Length != 0 ? null : Close(typeClosures);
        }
        #endregion

        /// <summary>
        /// Performs an implicit conversion from <see cref="WebApplications.Utilities.Relection.Property"/> to <see cref="System.Reflection.PropertyInfo"/>.
        /// </summary>
        /// <param name="indexer">The indexer.</param>
        /// <returns>The result of the conversion.</returns>
        /// <remarks></remarks>
        public static implicit operator PropertyInfo(Indexer indexer)
        {
            return indexer == null ? null : indexer.Info;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Reflection.PropertyInfo"/> to <see cref="WebApplications.Utilities.Relection.Property"/>.
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        /// <returns>The result of the conversion.</returns>
        /// <remarks></remarks>
        public static implicit operator Indexer(PropertyInfo propertyInfo)
        {
            return propertyInfo == null
                       ? null
                       : ((ExtendedType) propertyInfo.DeclaringType).GetIndexer(propertyInfo);
        }

        /// <summary>
        /// Closes the constructor with the specified concrete generic types.
        /// </summary>
        /// <param name="typeClosures">The types required to close the current type.</param>
        /// <returns>A closed signature, if possible; otherwise <see langword="null" />.</returns>
        /// <remarks><para>If signature closure is unsupported this method should return <see langword="null" />.</para>
        /// <para>The closure arrays are ordered and contain the same number of elements as their corresponding
        /// generic arguments.  Where elements are <see langword="null"/> a closure is not required.</para></remarks>
        [CanBeNull]
        public Indexer Close([NotNull] Type[] typeClosures)
        {
            // Check input arrays are valid.
            if (typeClosures.Length != ExtendedType.GenericArguments.Count())
                return null;

            // If we haven't got any type closures, we can return this indexer.
            if (!typeClosures.Any(t => t != null))
                return this;

            // Close type
            ExtendedType et = ExtendedType.CloseType(typeClosures);

            // Check closure succeeded.
            if (et == null)
                return null;

            // Create new search.
            Contract.Assert(_indexParameters.Value != null);
            int pCount = _indexParameters.Value.Length;
            TypeSearch[] searchTypes = new TypeSearch[pCount + 1];

            Type[] typeGenericArguments = et.GenericArguments.Select(g => g.Type).ToArray();

            // Search for closed 
            for (int i = 0; i < pCount; i++)
            {
                Contract.Assert(_indexParameters.Value[i] != null);
                Type pType = _indexParameters.Value[i].ParameterType;
                searchTypes[i] = Reflection.ExpandParameterType(pType, Reflection.EmptyTypes, typeGenericArguments);
            }

            // Add return type
            searchTypes[pCount] = Reflection.ExpandParameterType(Info.PropertyType, Reflection.EmptyTypes,
                                                                 typeGenericArguments);

            // Search for indexer on new type.
            return et.GetIndexer(searchTypes);
        }
    }
}