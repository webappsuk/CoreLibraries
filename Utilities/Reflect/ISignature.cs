#region © Copyright Web Applications (UK) Ltd, 2012.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities
// File: ISignature.cs
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
// © Copyright Web Applications (UK) Ltd, 2012.  All rights reserved.
#endregion

using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Reflect
{
    /// <summary>
    /// Interface that allows for signature matching.
    /// </summary>
    /// <remarks></remarks>
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
        ISignature Close([NotNull]Type[] typeClosures, [NotNull]Type[] signatureClosures);
    }
}