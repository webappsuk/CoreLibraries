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

using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Threading
{
    /// <summary>
    /// Allows the creation of <see cref="PauseToken">pause tokens</see> which support pausing.
    /// </summary>
    /// <remarks>See http://blogs.msdn.com/b/pfxteam/archive/2013/01/13/cooperatively-pausing-async-methods.aspx.
    /// </remarks>
    [PublicAPI]
    public class PauseTokenSource
    {
        /// <summary>
        /// The paused completion source.
        /// </summary>
        private TaskCompletionSource<bool> _paused;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is paused.
        /// </summary>
        /// <value><see langword="true" /> if this instance is paused; otherwise, <see langword="false" />.</value>
        public bool IsPaused
        {
            [PublicAPI] get { return _paused != null; }
            [PublicAPI]
            set
            {
                if (value)
                    Interlocked.CompareExchange(ref _paused, new TaskCompletionSource<bool>(), null);
                else
                {
                    while (true)
                    {
                        TaskCompletionSource<bool> tcs = _paused;
                        if (tcs == null) return;
                        if (Interlocked.CompareExchange(ref _paused, null, tcs) != tcs) continue;
                        tcs.SetResult(true);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the token.
        /// </summary>
        /// <value>The token.</value>
        [PublicAPI]
        public PauseToken Token
        {
            get { return new PauseToken(this); }
        }

        /// <summary>
        /// Used by <see cref="PauseToken"/> to paused.
        /// </summary>
        /// <returns>Task.</returns>
        [PublicAPI]
        [NotNull]
        internal Task WaitWhilePausedAsync()
        {
            TaskCompletionSource<bool> cur = _paused;
            // ReSharper disable once AssignNullToNotNullAttribute
            return cur != null ? cur.Task : TaskResult.Completed;
        }
    }
}