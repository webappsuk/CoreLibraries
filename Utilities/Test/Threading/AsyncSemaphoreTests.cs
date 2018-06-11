#region © Copyright Web Applications (UK) Ltd, 2018.  All rights reserved.
// Copyright (c) 2018, Web Applications UK Ltd
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
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Utilities.Threading;

namespace WebApplications.Utilities.Test.Threading
{
    [TestClass]
    public class AsyncSemaphoreTests : UtilitiesTestBase
    {
        /// <summary>
        /// When a token is cancelled while waiting, it should not still use up a slot.
        /// </summary>
        [TestMethod]
        [WorkItem(55)]
        public async Task WaiterTimeout()
        {
            bool gotCancelled = false;
            AsyncSemaphore sem = new AsyncSemaphore();
            using (await sem.WaitAsync().ConfigureAwait(false))
            {
                try
                {
                    using (CancellationTokenSource cts = new CancellationTokenSource(5000))
                    {
                        // This will be waiting for the outer wait to be released, but will be cancelled before then
                        await sem.WaitAsync(cts.Token).ConfigureAwait(false);
                    }
                }
                catch (OperationCanceledException)
                {
                    gotCancelled = true;
                }

                if (!gotCancelled)
                    Assert.Inconclusive("WaitAsync didn't get cancelled.");
            }

            // This should run without any issue
            using (CancellationTokenSource cts = new CancellationTokenSource(5000))
            using (await sem.WaitAsync(cts.Token).ConfigureAwait(false))
            {
            }
        }

        /// <summary>
        /// When a token is cancelled while waiting, it should not still use up a slot.
        /// </summary>
        [TestMethod]
        [WorkItem(55)]
        public async Task LockWaiterTimeout()
        {
            bool gotCancelled = false;
            AsyncLock @lock = new AsyncLock();
            using (await @lock.LockAsync().ConfigureAwait(false))
            {
                try
                {
                    using (CancellationTokenSource cts = new CancellationTokenSource(5000))
                    {
                        // This will be waiting for the outer lock to be released, but will be cancelled before then
                        await @lock.LockAsync(cts.Token).ConfigureAwait(false);
                    }
                }
                catch (OperationCanceledException)
                {
                    gotCancelled = true;
                }

                if (!gotCancelled)
                    Assert.Inconclusive("LockAsync didn't get cancelled.");
            }

            // This should run without any issue
            using (CancellationTokenSource cts = new CancellationTokenSource(5000))
            using (await @lock.LockAsync(cts.Token).ConfigureAwait(false))
            {
            }
        }
    }
}
