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
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using WebApplications.Utilities.Annotations;

namespace WebApplications.Utilities.Threading
{
    /// <summary>
    /// Allows awaiting for a synchronization context, and runs that the continuation on the context.
    /// </summary>
    /// <remarks>Do not use with ConfigureAwait(false), as this can result in the continuation running outside the context.</remarks>
    public struct SynchronizationContextAwaiter : INotifyCompletion
    {
        [NotNull]
        private readonly SynchronizationContext _context;

        [NotNull]
        private readonly SendOrPostCallback _executor;

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizationContextAwaiter"/> struct.
        /// </summary>
        /// <param name="context">The context.</param>
        public SynchronizationContextAwaiter([NotNull] SynchronizationContext context)
        {
            Contract.Requires(context != null);
            _context = context;
            // ReSharper disable once PossibleNullReferenceException
            _executor = a => ((Action)a)();
        }

        /// <summary>
        /// Gets a value indicating whether this instance is completed.
        /// </summary>
        /// <value><see langword="true" /> if this instance is completed; otherwise, <see langword="false" />.</value>
        [UsedImplicitly]
        public bool IsCompleted
        {
            get { return false; }
        }

        /// <summary>
        /// Called when the action is completed..
        /// </summary>
        /// <param name="action">The action.</param>
        public void OnCompleted([NotNull] Action action)
        {
            _context.Post(_executor, action);
        }

        /// <summary>
        /// Gets the result.
        /// </summary>
        [UsedImplicitly]
        public void GetResult()
        {
        }
    }
}