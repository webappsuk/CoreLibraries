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
using System.Collections.Generic;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Reflect
{
    /// <summary>
    /// Interface that allows for signature matching.
    /// </summary>
    /// <remarks></remarks>
    [PublicAPI]
    public interface ISignature
    {
        /// <summary>
        /// Gets the declaring type of the signature, if this is a real signature.
        /// </summary>
        /// <value>The type of the declaring.</value>
        /// <remarks></remarks>
        [CanBeNull]
        Type DeclaringType { get; }

        /// <summary>
        /// Gets the declaring type's generic arguments (if any).
        /// </summary>
        /// <value>The type generic arguments.</value>
        /// <remarks></remarks>
        [CanBeNull]
        IEnumerable<GenericArgument> TypeGenericArguments { get; }

        /// <summary>
        /// Gets the signature generic arguments (if any).
        /// </summary>
        /// <value>The signature generic arguments.</value>
        /// <remarks></remarks>
        [CanBeNull]
        IEnumerable<GenericArgument> SignatureGenericArguments { get; }

        /// <summary>
        /// Gets the parameter types.
        /// </summary>
        /// <value>The parameter types.</value>
        /// <remarks></remarks>
        [CanBeNull]
        IEnumerable<Type> ParameterTypes { get; }

        /// <summary>
        /// Gets the return type of the signature.
        /// </summary>
        /// <value>The type of the return.</value>
        /// <remarks>For constructors this is effectively the same as the <see cref="DeclaringType"/></remarks>
        [NotNull]
        Type ReturnType { get; }

        /// <summary>
        /// Closes the signature with the specified concrete generic types.
        /// </summary>
        /// <param name="typeClosures">The types required to close the current type.</param>
        /// <param name="signatureClosures">The types required to close the signature.</param>
        /// <returns>A closed signature, if possible; otherwise <see langword="null" />.</returns>
        /// <remarks><para>If signature closure is unsupported this method should return <see langword="null" />.</para>
        /// <para>The closure arrays are ordered and contain the same number of elements as their corresponding
        /// generic arguments.  Where elements are <see langword="null"/> a closure is not required.</para></remarks>
        [CanBeNull]
        ISignature Close([NotNull] Type[] typeClosures, [NotNull] Type[] signatureClosures);
    }
}