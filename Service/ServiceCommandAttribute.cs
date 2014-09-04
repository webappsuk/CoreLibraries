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
using System.Diagnostics.Contracts;
using JetBrains.Annotations;
using WebApplications.Utilities.Service.Common;

namespace WebApplications.Utilities.Service
{
    /// <summary>
    /// Add to an instance method to indicate it implements a <see cref="BaseService">service</see> command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    [Serializable]
    [PublicAPI]
    [MeansImplicitUse]
    public class ServiceCommandAttribute : Attribute
    {
        /// <summary>
        /// The resource type.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public readonly Type ResourceType;

        /// <summary>
        /// The resource property for a comma-separated list of names.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public readonly string NamesProperty;

        /// <summary>
        /// The resource property for the description.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public readonly string DescriptionProperty;

        /// <summary>
        /// If <see langword="true"/> then the last parameter should accept the entire line; otherwise <see langword="false"/>.
        /// </summary>
        [PublicAPI]
        public readonly bool ConsumeLine;

        /// <summary>
        /// The minimum arguments.
        /// </summary>
        [PublicAPI]
        public readonly int MinimumArguments;

        /// <summary>
        /// The identifier parameter. If specified, this parameter must be of type <see cref="Guid"/>, and it will be passed the current connection ID.
        /// </summary>
        [CanBeNull]
        [PublicAPI]
        public readonly string IDParameter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceCommandAttribute" /> class.
        /// </summary>
        /// <param name="resourceType">Type of the resource.</param>
        /// <param name="namesProperty">The names property.</param>
        /// <param name="descriptionProperty">The description property.</param>
        /// <param name="consumeLine">if set to <see langword="true" /> [consume line].</param>
        /// <param name="minimumArguments">The minimum arguments.</param>
        /// <param name="idParameter">The identifier parameter. if specified, this parameter must be of type 
        /// <see cref="Guid" />,
        /// and it will be passed the current connection ID.</param>
        public ServiceCommandAttribute(
            [NotNull] Type resourceType,
            [NotNull] string namesProperty,
            [NotNull] string descriptionProperty,
            bool consumeLine = false,
            int minimumArguments = 0,
            [CanBeNull] string idParameter = null)
        {
            Contract.Requires<RequiredContractException>(resourceType != null, "Parameter_Null");
            Contract.Requires<RequiredContractException>(namesProperty != null, "Parameter_Null");
            Contract.Requires<RequiredContractException>(descriptionProperty != null, "Parameter_Null");
            ResourceType = resourceType;
            NamesProperty = namesProperty;
            DescriptionProperty = descriptionProperty;
            MinimumArguments = minimumArguments;
            ConsumeLine = consumeLine;
            IDParameter = idParameter;
        }
    }
}