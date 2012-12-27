#region © Copyright Web Applications (UK) Ltd, 2012.  All rights reserved.
// Copyright (c) 2012, Web Applications UK Ltd
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
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing;
using WebApplications.Testing.Data;
using WebApplications.Testing.Data.Exceptions;
using WebApplications.Utilities.Logging.Loggers;

namespace WebApplications.Utilities.Logging.Test
{
    [TestClass]
    public class LoggingTests : TestBase
    {
        [TestMethod]
        public async Task TestMemoryLogger()
        {
            FileLogger fileLogger = Log.Loggers.OfType<FileLogger>().First();
            Assert.IsNotNull(fileLogger);
            fileLogger.Format = "Verbose,Xml";

            string message = "Test message " + Guid.NewGuid();
            Log.Add(new LogContext("My data", 1, "Some more", "Test"), message);
            await Log.Flush();

            List<Log> logs = Log.AllCached.ToList();
            Assert.IsNotNull(logs);
            Assert.IsTrue(logs.Any(), "No logs found!");
            Assert.IsTrue(logs.Any(l => l.Message == message), "No log with the message '{0}' found.", message);

            logs[0].ToString("{Verbose}\r\n");
        }

        [TestMethod]
        public void TestPartialLogMinimum()
        {
            Log partialLog = new Log(new[]
                {
                    new KeyValuePair<string, string>(Log.GuidKey, CombGuid.NewCombGuid().ToString())
                });
            Assert.IsNotNull(partialLog);

            partialLog.ReLog();
        }

        [TestMethod]
        public void TestPartialLog()
        {
            Log partialLog = new Log(new[]
                {
                    new KeyValuePair<string, string>(Log.GuidKey, CombGuid.NewCombGuid().ToString()),
                    new KeyValuePair<string, string>(Log.GroupKey, CombGuid.NewCombGuid().ToString()),
                    new KeyValuePair<string, string>(Log.LevelKey, LoggingLevel.SystemNotification.ToString()),
                    new KeyValuePair<string, string>(Log.ThreadIDKey, "1"),
                    new KeyValuePair<string, string>(Log.ExceptionTypeFullNameKey, "System.Data.SqlException"),
                    new KeyValuePair<string, string>(Log.StoredProcedureKey, "spTest"),
                    new KeyValuePair<string, string>(Log.StoredProcedureLineKey, "2")
                });
            Assert.IsNotNull(partialLog);

            partialLog.ReLog();
        }

        [TestMethod]
        [ExpectedException(typeof(LoggingException))]
        public void TestPartialLogRequiresGUID()
        {
            Log partialLog = new Log(new List<KeyValuePair<string, string>>());
        }

        [TestMethod]
        public void TestExceptions()
        {
            var t = new TestException();
        }

        #region Nested type: TestException
        private class TestException : LoggingException
        {
            public TestException()
                : this("Test {0}.", "message")
            {
            }

            private TestException(string message, params object[] parameters)
                : base(
                new LogContext("My data", "Was here"),
                (new SqlInvalidSyntaxException()).SqlException,
                LoggingLevel.Error,
                message,
                parameters)
            {
            }
        }
        #endregion

        [TestMethod]
        public async Task TestDataContractSerialization()
        {
            for (int m = 0; m < 5; m++)
                Log.Add("Test Message {0} - {1}", m, Guid.NewGuid());

            await Log.Flush();
            List<Log> logs = Log.AllCached.ToList();
            Assert.IsNotNull(logs);
            Assert.IsTrue(logs.Any(), "No logs found!");
            Assert.IsTrue(logs.Any(l => l.MessageFormat == "Test Message {0} - {1}"), "No log with the message format found");
            await Log.Flush();

            DataContractSerializer serializer = new DataContractSerializer(typeof(IEnumerable<Log>));
            IEnumerable<Log> logs2;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                serializer.WriteObject(memoryStream, logs);

                memoryStream.Seek(0, SeekOrigin.Begin);

                logs2 = serializer.ReadObject(memoryStream) as IEnumerable<Log>;

                memoryStream.Seek(0, SeekOrigin.Begin);
                XDocument doc = XDocument.Load(memoryStream);
                Trace.WriteLine(doc.ToString());
            }
            Assert.IsNotNull(logs2);
            List<Log> result = logs2.ToList();
            Assert.AreEqual(logs.Count, result.Count);
            for (int i = 0; i < logs.Count; i++)
                Assert.AreEqual(logs[i].ToString(), result[i].ToString());
        }

        [TestCleanup]
        public void Cleanup()
        {
            Log.Flush().Wait();
        }
    }
}