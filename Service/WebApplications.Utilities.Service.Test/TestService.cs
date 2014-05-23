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
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace WebApplications.Utilities.Service.Test
{
    /// <summary>
    /// A test service
    /// </summary>
    public class TestService : BaseService<TestService>
    {
        /// <summary>
        /// A test command.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="optional">The optional.</param>
        [PublicAPI]
        [ServiceCommand(typeof (TestServiceResources), "Cmd_TestCommand_Names", "Cmd_TestCommand_Description",
            idParameter: "id")]
        public void TestCommand(
            [NotNull] TextWriter writer,
            [NotNull] string parameter,
            Guid id = default(Guid),
            [NotNull] string optional = null)
        {
            writer.WriteLine("Connection: {0}", id);
            writer.WriteLine("Parameter: {0}", parameter);
            if (optional != null)
                writer.WriteLine("Optional: {0}", optional);
        }


        /// <summary>
        /// Stops this instance.
        /// </summary>
        [PublicAPI]
        [ServiceCommand(typeof (TestResource), "Cmd_LongRun", "Cmd_LongRun_Description")]
        public async Task<bool> StopService(
            [NotNull] TextWriter writer,
            [ServiceCommandParameter(typeof (TestResource), "Cmd_LongRun_Loops_Description")] int loops = 10,
            [ServiceCommandParameter(typeof (TestResource), "Cmd_LongRun_ThrowError_Description")] bool throwError =
                false,
            CancellationToken token = default(CancellationToken))
        {
            writer.WriteLine("Running long running operation:");
            for (int l = 0; l < loops; l++)
            {
                writer.WriteLine("Loop {0} completed", l);
                await Task.Delay(1000, token);
                token.ThrowIfCancellationRequested();
            }
            if (throwError)
                throw new ApplicationException("Test exception");
            writer.WriteLine("Completed");
            return true;
        }
    }
}