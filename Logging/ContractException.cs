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
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Logging
{
    /// <summary>
    /// Exception for a <see cref="System.Diagnostics.Contracts.Contract.Requires{ContractException}(bool, string)" /> failure.
    /// </summary>
    [Serializable]
    [PublicAPI]
    public class ContractException : LoggingException
    {
        [NonSerialized]
        private static readonly Guid _reservation = System.Guid.NewGuid();

        /// <summary>
        /// The condition context key
        /// </summary>
        [NotNull]
        [NonSerialized]
        public static readonly string ConditionKey = LogContext.ReserveKey("Contract Condition", _reservation);

        /// <summary>
        /// Gets the condition that failed.
        /// </summary>
        /// <value>
        /// The condition.
        /// </value>
        [CanBeNull]
        public string Condition
        {
            get { return Log.Get(ConditionKey); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContractException"/> class.
        /// </summary>
        /// <param name="resourceType">Type of the resource.</param>
        /// <param name="resourceProperty">The resource property.</param>
        /// <param name="conditions">The conditions.</param>
        protected ContractException(
            [NotNull] Type resourceType,
            [NotNull] string resourceProperty,
            [CanBeNull] string conditions)
            : base(
                new LogContext().Set(_reservation, ConditionKey, conditions),
                LoggingLevel.Critical,
                resourceType,
                resourceProperty,
                conditions)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContractException" /> class.  This is used by <see cref="System.Diagnostics.Contracts.Contract.Requires{ContractException}(bool)" />.
        /// </summary>
        /// <param name="conditions">The conditions.</param>
        public ContractException([CanBeNull] string conditions)
            : base(
                new LogContext().Set(_reservation, ConditionKey, conditions),
                LoggingLevel.Critical,
                () => Resources.Contract_Failed,
                conditions)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContractException" /> class.  This is used by <see cref="System.Diagnostics.Contracts.Contract.Requires{ContractException}(bool, string)" />.
        /// </summary>
        /// <param name="conditions">The conditions.</param>
        /// <param name="message">The tag.</param>
        public ContractException([CanBeNull] string conditions, [NotNull] string message)
            : base(
                new LogContext().Set(_reservation, ConditionKey, conditions),
                LoggingLevel.Critical,
                () => Resources.Contract_Failed_Message,
                conditions,
                message)
        {
        }
    }

    /// <summary>
    /// Exception for a <see cref="System.Diagnostics.Contracts.Contract.Requires{ContractException}(bool, string)" /> failure.
    /// </summary>
    /// <typeparam name="TResource">The type of the resource.</typeparam>
    [Serializable]
    [PublicAPI]
    public class ContractException<TResource> : ContractException
        where TResource : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContractException" /> class.  This is used by <see cref="System.Diagnostics.Contracts.Contract.Requires{ContractException}(bool)" />.
        /// </summary>
        /// <param name="conditions">The conditions.</param>
        public ContractException([CanBeNull] string conditions)
            : base(typeof(TResource), "Contract_Failed", conditions)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContractException" /> class.  This is used by <see cref="System.Diagnostics.Contracts.Contract.Requires{ContractException}(bool, string)" />.
        /// </summary>
        /// <param name="conditions">The conditions.</param>
        /// <param name="resourceProperty">The tag.</param>
        public ContractException([CanBeNull] string conditions, [NotNull] string resourceProperty)
            : base(typeof(TResource), resourceProperty, conditions)
        {
        }
    }
}