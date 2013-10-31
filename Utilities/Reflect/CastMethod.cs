#region © Copyright Web Applications (UK) Ltd, 2013.  All rights reserved.
// Copyright (c) 2013, Web Applications UK Ltd
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
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Reflect
{
    /// <summary>
    /// A method that implements a cast operator.
    /// </summary>
    /// <remarks></remarks>
    public class CastMethod : Method
    {
        /// <summary>
        /// The type this cast casts from.
        /// </summary>
        public readonly Type FromType;

        /// <summary>
        /// Whether the cast is an explicit cast.
        /// </summary>
        public readonly bool IsExplicit;

        /// <summary>
        /// The type this cast casts to.
        /// </summary>
        public readonly Type ToType;

        /// <summary>
        /// Initializes a new instance of the <see cref="CastMethod" /> class.
        /// </summary>
        /// <param name="extendedType">Type of the extended.</param>
        /// <param name="info">The info.</param>
        /// <param name="isExplicit">if set to <see langword="true" /> [is explicit].</param>
        /// <remarks></remarks>
        internal CastMethod([NotNull] ExtendedType extendedType, [NotNull] MethodInfo info, bool isExplicit)
            : base(extendedType, info)
        {
            Contract.Requires(info.Name == "op_Explicit" || info.Name == "op_Implicit");
            IsExplicit = isExplicit;
            FromType = ParameterTypes.First();
            ToType = ReturnType;
            Contract.Assert(FromType == extendedType.Type || ToType == extendedType.Type);
        }
    }
}