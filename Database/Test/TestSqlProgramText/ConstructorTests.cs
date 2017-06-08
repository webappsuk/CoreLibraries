#region © Copyright Web Applications (UK) Ltd, 2017.  All rights reserved.
// Copyright (c) 2017, Web Applications UK Ltd
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

using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Utilities.Logging;

// ReSharper disable ConsiderUsingConfigureAwait
// ReSharper disable InconsistentNaming
#pragma warning disable 1591 // Missing XML commend
#pragma warning disable 0618 // Type or member is obsolete

namespace WebApplications.Utilities.Database.Test
{
    [TestClass]
    public partial class SqlProgramTextTests : DatabaseTestBase
    {
        private const string AString = "John Dough";
        private const int AInt = 30;
        private const decimal ADecimal = -200.15M;
        private const bool ABool = true;

        [TestMethod]
        public async Task Constructor_WithUsedParameter_Succeeds()
        {
            SqlProgram<int> timeoutTest = await SqlProgram<int>.Create(
                LocalDatabaseConnection,
                "ProgramName",
                "@param",
                "SELECT @param",
                CommandType.Text);
            Assert.IsNotNull(timeoutTest);
        }

        [TestMethod]
        [ExpectedException(typeof(LoggingException))]
        public async Task Constructor_WithoutUsedParameter_ThrowsLoggingException()
        {
            Trace.WriteLine(Local_spNonQueryText);

            SqlProgram<int> timeoutTest = await SqlProgram<int>.Create(
                LocalDatabaseConnection,
                "ProgramName",
                "SELECT @param",
                CommandType.Text);
            Assert.IsNotNull(timeoutTest);
        }

        [TestMethod]
        [ExpectedException(typeof(LoggingException))]
        public async Task Constructor_WithoutKnownUsedParameter_ThrowsLoggingException()
        {
            Trace.WriteLine(Local_spNonQueryText);

            SqlProgram<int> timeoutTest = await SqlProgram<int>.Create(
                LocalDatabaseConnection,
                "ProgramName",
                "@param",
                "SELECT @param, @param2",
                CommandType.Text);
            Assert.IsNotNull(timeoutTest);
        }

        [TestMethod]
        [ExpectedException(typeof(LoggingException))]
        public async Task Constructor_WithInvalidProgramText_ThrowsLoggingException()
        {
            SqlProgram<int> timeoutTest = await SqlProgram<int>.Create(
                DifferentLocalDatabaseConnection,
                "ProgramName",
                "Invalid text",
                CommandType.Text);
            Assert.IsNotNull(timeoutTest);
        }
    }
}