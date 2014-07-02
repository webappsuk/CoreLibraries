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
using System.Threading;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Threading
{
    /// <summary>
    /// An <see cref="ITokenSource"/> implementation that is based on <see cref="CancellationTokenSource"/>.
    /// </summary>
    [PublicAPI]
    public class TokenSource : CancellationTokenSource, ITokenSource
    {
        /// <summary>
        /// A cancelled <see cref="ITokenSource"/>
        /// </summary>
        [NotNull]
        public static readonly ITokenSource Cancelled;

        /// <summary>
        /// A <see cref="ITokenSource"/> that is never cancelled.
        /// </summary>
        [NotNull]
        public static readonly ITokenSource None;

        /// <summary>
        /// Initializes the <see cref="TokenSource"/> class.
        /// </summary>
        static TokenSource()
        {
            TokenSource c = new TokenSource();
            c.Cancel();
            Cancelled = c;

            None = new WrappedTokenSource(CancellationToken.None);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenSource"/> class.
        /// </summary>
        public TokenSource()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenSource"/> class.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        public TokenSource(TimeSpan timeout)
            : base(timeout)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenSource"/> class.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        public TokenSource(int timeout)
            : base(timeout)
        {
        }
    }
}