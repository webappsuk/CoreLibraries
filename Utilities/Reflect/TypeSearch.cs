#region © Copyright Web Applications (UK) Ltd, 2014.  All rights reserved.
// Copyright (c) 2014, Web Applications UK Ltd
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
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Reflect
{
    /// <summary>
    /// Used to match types in searches.
    /// </summary>
    /// <remarks></remarks>
    [DebuggerDisplay(
        "{Type} [Location: {GenericArgumentLocation}; Name: {GenericArgumentName}; Position: {GenericArgumentPosition}]"
        )]
    public class TypeSearch
    {
        #region Defaults
        /// <summary>
        /// The void type.
        /// </summary>
        public static readonly TypeSearch Void = new TypeSearch(typeof (void));

        /// <summary>
        /// The first generic argument on the type.
        /// </summary>
        public static readonly TypeSearch T1 = new TypeSearch(GenericArgumentLocation.Type, 0);

        /// <summary>
        /// The second generic argument on the type.
        /// </summary>
        public static readonly TypeSearch T2 = new TypeSearch(GenericArgumentLocation.Type, 1);

        /// <summary>
        /// The third generic argument on the type.
        /// </summary>
        public static readonly TypeSearch T3 = new TypeSearch(GenericArgumentLocation.Type, 2);

        /// <summary>
        /// The fourth generic argument on the type.
        /// </summary>
        public static readonly TypeSearch T4 = new TypeSearch(GenericArgumentLocation.Type, 3);

        /// <summary>
        /// The fifth generic argument on the type.
        /// </summary>
        public static readonly TypeSearch T5 = new TypeSearch(GenericArgumentLocation.Type, 4);

        /// <summary>
        /// The sixth generic argument on the type.
        /// </summary>
        public static readonly TypeSearch T6 = new TypeSearch(GenericArgumentLocation.Type, 5);

        /// <summary>
        /// The seventh generic argument on the type.
        /// </summary>
        public static readonly TypeSearch T7 = new TypeSearch(GenericArgumentLocation.Type, 6);

        /// <summary>
        /// The eighth generic argument on the type.
        /// </summary>
        public static readonly TypeSearch T8 = new TypeSearch(GenericArgumentLocation.Type, 7);

        /// <summary>
        /// The first generic argument on the signature.
        /// </summary>
        public static readonly TypeSearch S1 = new TypeSearch(GenericArgumentLocation.Signature, 0);

        /// <summary>
        /// The second generic argument on the signature.
        /// </summary>
        public static readonly TypeSearch S2 = new TypeSearch(GenericArgumentLocation.Signature, 1);

        /// <summary>
        /// The third generic argument on the signature.
        /// </summary>
        public static readonly TypeSearch S3 = new TypeSearch(GenericArgumentLocation.Signature, 2);

        /// <summary>
        /// The forth generic argument on the signature.
        /// </summary>
        public static readonly TypeSearch S4 = new TypeSearch(GenericArgumentLocation.Signature, 3);

        /// <summary>
        /// The fifth generic argument on the signature.
        /// </summary>
        public static readonly TypeSearch S5 = new TypeSearch(GenericArgumentLocation.Signature, 4);

        /// <summary>
        /// The sixth generic argument on the signature.
        /// </summary>
        public static readonly TypeSearch S6 = new TypeSearch(GenericArgumentLocation.Signature, 5);

        /// <summary>
        /// The seventh generic argument on the signature.
        /// </summary>
        public static readonly TypeSearch S7 = new TypeSearch(GenericArgumentLocation.Signature, 6);

        /// <summary>
        /// The eighth generic argument on the signature.
        /// </summary>
        public static readonly TypeSearch S8 = new TypeSearch(GenericArgumentLocation.Signature, 7);
        #endregion

        /// <summary>
        /// The generic argument location to search for (if any).
        /// </summary>
        public readonly GenericArgumentLocation GenericArgumentLocation;

        /// <summary>
        /// The generic argument name to search for (if any).
        /// </summary>
        [CanBeNull]
        public readonly string GenericArgumentName;

        /// <summary>
        /// The generic argument position to search for (if any; otherwise -1).
        /// </summary>
        public readonly int GenericArgumentPosition;

        /// <summary>
        /// Whether the type is by reference.
        /// </summary>
        public readonly bool IsByReference;

        /// <summary>
        /// Whether the type is a pointer.
        /// </summary>
        public readonly bool IsPointer;

        /// <summary>
        /// A concrete type to search for (if any).
        /// </summary>
        [CanBeNull]
        public readonly Type Type;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The name.</param>
        /// <param name="location">The location.</param>
        /// <param name="position">The position.</param>
        /// <param name="isByReference">if set to <see langword="true"/> [is by reference].</param>
        /// <param name="isPointer">if set to <see langword="true"/> [is pointer].</param>
        /// <remarks></remarks>
        private TypeSearch(
            Type type = null,
            string name = null,
            GenericArgumentLocation location = GenericArgumentLocation.None,
            int position = -1,
            bool isByReference = false,
            bool isPointer = false)
        {
            Type = type;
            GenericArgumentName = name;
            GenericArgumentLocation = location;
            GenericArgumentPosition = position;
            IsByReference = isByReference;
            IsPointer = isPointer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeSearch"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <remarks></remarks>
        public TypeSearch([NotNull] Type type)
            : this(type, isByReference: type.IsByRef, isPointer: type.IsPointer)
        {
            if (type.IsGenericParameter)
            {
                GenericArgumentLocation = type.DeclaringMethod != null
                    ? GenericArgumentLocation.Signature
                    : GenericArgumentLocation.Type;
                GenericArgumentPosition = type.GenericParameterPosition;
                GenericArgumentName = type.Name;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeSearch"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="position">The position.</param>
        /// <param name="isByReference">if set to <see langword="true"/> is by reference.</param>
        /// <param name="isPointer">if set to <see langword="true"/> is pointer.</param>
        /// <remarks></remarks>
        public TypeSearch(
            GenericArgumentLocation location,
            int position,
            bool isByReference = false,
            bool isPointer = false)
            : this(null, null, location, position, isByReference, isPointer)
        {
            // Validate location
            switch (GenericArgumentLocation)
            {
                case GenericArgumentLocation.Signature:
                case GenericArgumentLocation.Type:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("location");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeSearch"/> class.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="name">The name.</param>
        /// <param name="isByReference">if set to <see langword="true"/> is by reference.</param>
        /// <param name="isPointer">if set to <see langword="true"/> is pointer.</param>
        /// <remarks></remarks>
        public TypeSearch(
            GenericArgumentLocation location,
            [NotNull] string name,
            bool isByReference = false,
            bool isPointer = false)
            : this(null, name, location, -1, isByReference, isPointer)
        {
            // Validate location
            switch (GenericArgumentLocation)
            {
                case GenericArgumentLocation.Signature:
                case GenericArgumentLocation.Type:
                case GenericArgumentLocation.Any:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("location");
            }
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="TypeSearch"/> to <see cref="System.Type"/>.
        /// </summary>
        /// <param name="typeSearch">The type search.</param>
        /// <returns>The result of the conversion.</returns>
        /// <remarks></remarks>
        public static implicit operator Type(TypeSearch typeSearch)
        {
            return typeSearch != null ? typeSearch.Type : null;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="TypeSearch"/> to <see cref="System.Type"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The result of the conversion.</returns>
        /// <remarks></remarks>
        public static implicit operator TypeSearch(Type type)
        {
            return type != null ? new TypeSearch(type) : null;
        }
    }
}