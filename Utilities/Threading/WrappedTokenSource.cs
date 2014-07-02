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

using System.Diagnostics.Contracts;
using System.Threading;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Threading
{
    /// <summary>
    /// An <see cref="ITokenSource"/> that wraps other sources.
    /// </summary>
    [PublicAPI]
    public class WrappedTokenSource : ITokenSource
    {
        /// <summary>
        /// The cancellation token
        /// </summary>
        private readonly CancellationToken _token;

        /// <summary>
        /// The _sources
        /// </summary>
        [CanBeNull]
        private CancellationTokenSource[] _sources;

        /// <summary>
        /// Gets the <see cref="CancellationToken" /> associated with this <see cref="WrappedTokenSource" />.
        /// </summary>
        /// <value>
        /// The <see cref="CancellationToken" /> associated with this <see cref="WrappedTokenSource" />.
        /// </value>
        public CancellationToken Token
        {
            get { return _token; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WrappedTokenSource"/> class from a single cancellation token source.
        /// </summary>
        /// <param name="source">The source.</param>
        public WrappedTokenSource([NotNull] CancellationTokenSource source)
        {
            Contract.Requires(source != null, "Parameter_Null");

            _token = source.Token;
            _sources = new[] {source};
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WrappedTokenSource" /> class from a token and multiple cancellation token sources.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="sources">The sources.</param>
        public WrappedTokenSource(CancellationToken token, [NotNull] params CancellationTokenSource[] sources)
        {
            Contract.Requires(sources != null, "Parameter_Null");
            Contract.Requires(Contract.ForAll(sources, s => s != null));

            _token = token;
            _sources = sources;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// Will dispose any <see cref="CancellationTokenSource">cancellation token sources</see> that this source was created with.
        /// </summary>
        public void Dispose()
        {
            CancellationTokenSource[] sources = Interlocked.Exchange(ref _sources, null);
            if (sources != null)
                foreach (CancellationTokenSource source in sources)
                    // ReSharper disable once PossibleNullReferenceException
                    source.Dispose();
        }
    }
}