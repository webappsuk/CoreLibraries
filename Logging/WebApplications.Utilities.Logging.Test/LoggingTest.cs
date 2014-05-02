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
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProtoBuf;
using ProtoBuf.Meta;
using WebApplications.Testing;
using WebApplications.Testing.Data;
using WebApplications.Testing.Data.Exceptions;
using WebApplications.Utilities.Logging.Loggers;

namespace WebApplications.Utilities.Logging.Test
{
    [TestClass]
    public class LoggingTests : TestBase
    {
        [NotNull]
        private static readonly Dictionary<string, string> _logDictionary;

        static LoggingTests()
        {
            Exception ex;
            try
            { throw new Exception(); }
            catch (Exception e)
            { ex = e; }

            _logDictionary = new Dictionary<string, string>
            {
                {Log.GuidKey, CombGuid.NewCombGuid().ToString()},
                {Log.LevelKey, LoggingLevel.Emergency.ToString()},
                {Log.ExceptionTypeFullNameKey, "TestException"},
                {Log.InnerExceptionGuidsPrefix + 0, CombGuid.NewCombGuid().ToString()},
                {Log.InnerExceptionGuidsPrefix + 1, CombGuid.NewCombGuid().ToString()},
                {Log.InnerExceptionGuidsPrefix + 2, CombGuid.NewCombGuid().ToString()},
                {Log.MessageFormatKey, "A test log: {0}, {1}, {2}"},
                {Log.ParameterPrefix + 0, "Parameter 1"},
                {Log.ParameterPrefix + 1, "Parameter 2"},
                {Log.ParameterPrefix + 2, "Parameter 3"},
                {Log.StackTraceKey, ex.StackTrace},
                {Log.StoredProcedureKey, "spTestLog"},
                {Log.StoredProcedureLineKey, "123"},
                {Log.ThreadIDKey, "343"},
                {Log.ThreadNameKey, "Log Test Thread"},
                {"Some key", "Some value"},
                {"Some other key", "Some other value"},
            };
        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Log.Flush().Wait();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            Log.ValidLevels = LoggingLevels.All;
            Translation.DefaultCulture = Resources.Culture = CultureInfo.InvariantCulture;
        }

        [TestMethod]
        public async Task TestMemoryLogger()
        {
            FileLogger fileLogger = Log.GetLoggers<FileLogger>().First();
            Assert.IsNotNull(fileLogger);
            fileLogger.Format = "Verbose,Xml";
            string message = "Test message " + Guid.NewGuid();
            LogContext context = new LogContext();
            context.Set("My data", 1);
            context.Set("Some more", "Test");
            Log.Add(context, LoggingLevel.Notification, message);
            await Log.Flush();

            List<Log> logs = Log.AllCached.ToList();
            Assert.IsNotNull(logs);
            Assert.IsTrue(logs.Any(), "No logs found!");
            Assert.IsTrue(logs.Any(l => l.Message == message), "No log with the message '{0}' found.", message);
        }

        [TestMethod]
        public void TestPartialLogMinimum()
        {
            var loggers = Log.Loggers.ToArray();
            foreach (var logger in loggers)
                Log.RemoveLogger(logger);

            Log.SetTrace();
            Log.Flush();

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
        public void TestToFromDictionary()
        {
            Log initialLog = new Log(_logDictionary);
            Trace.WriteLine(initialLog.ToString(Log.AllFormat));
            Trace.WriteLine(string.Empty);

            Dictionary<string, string> resultDictionary = initialLog.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            Log resultLog = new Log(initialLog);
            Trace.WriteLine(resultLog.ToString(Log.AllFormat));
            Trace.WriteLine(string.Empty);

            Log.Flush().Wait();
            CollectionAssert.AreEquivalent(_logDictionary, resultDictionary);
        }

        [TestMethod]
        public void TestGet()
        {
            Log log = new Log(_logDictionary);
            Log.Flush().Wait();

            foreach (KeyValuePair<string, string> kvp in _logDictionary)
            {
                Contract.Assert(kvp.Key != null);
                Assert.AreEqual(kvp.Value, log.Get(kvp.Key), "The value for the key {0} did not match the expected", kvp.Key);
                Assert.AreEqual(kvp.Value, log[kvp.Key], "The value for the key {0} did not match the expected", kvp.Key);
            }
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
                new LogContext().Set("My data", "Was here"),
                (new SqlInvalidSyntaxException()).SqlException,
                LoggingLevel.Error,
                message,
                parameters)
            {
            }
        }
        #endregion

        [TestMethod]
        public async Task TestInnerExceptions()
        {
            try
            {
                try
                {
                    try
                    {
                        throw new AggregateException("Exception 3",
                            new object[3].Select((o, i) =>
                            {
                                try { throw new Exception("Exception 4." + (i + 1)); }
                                catch (Exception e) { return e; }
                            }));
                    }
                    catch (Exception e)
                    { throw new InvalidOperationException("Exception 2", e); }
                }
                catch (Exception e)
                { throw new Exception("Exception 1", e); }
            }
            catch (Exception e)
            { Log.Add(e, LoggingLevel.Information, "An exception occured!"); }

            await Log.Flush();
        }

        [TestMethod]
        public async Task TestDataContractSerialization()
        {
            const int testCount = 5;

            Log[] logs = new Log[testCount];
            for (int m = 0; m < testCount; m++)
                logs[m] = new Log(new LogContext().Set("Test No", m), LoggingLevel.Information,"Test Message {0} - {1}", m, Guid.NewGuid());

            await Log.Flush();

            CollectionAssert.AllItemsAreNotNull(logs);
            Assert.IsTrue(logs.All(l => l.MessageFormat == "Test Message {0} - {1}"), "Logs contain incorrect message format.");

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
            Assert.AreEqual(logs.Length, result.Count);
            for (int i = 0; i < logs.Length; i++)
                Assert.AreEqual(logs[i].ToString(), result[i].ToString());
        }

        [TestMethod]
        public async Task TestProtoBufSerialization()
        {
            Trace.WriteLine(RuntimeTypeModel.Default.GetSchema(typeof(Log)));
            const int testCount = 5;

            Log[] logs = new Log[testCount];
            for (int m = 0; m < testCount; m++)
                logs[m] = new Log(new LogContext().Set("Test No", m), null, LoggingLevel.Information, () => Resources.TestString, m, Guid.NewGuid());

            await Log.Flush();

            CollectionAssert.AllItemsAreNotNull(logs);
            Assert.IsTrue(logs.All(l => l.MessageFormat == Resources.TestString), "Logs contain incorrect message format.");

            List<Log> logs2 = new List<Log>();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                foreach (Log log in logs)
                    Serializer.SerializeWithLengthPrefix(memoryStream, log, PrefixStyle.Base128);

                Trace.WriteLine(string.Format("Serialized logs took up {0} bytes.", memoryStream.Position));
                memoryStream.Seek(0, SeekOrigin.Begin);

                while (memoryStream.Position < memoryStream.Length)
                    logs2.Add(Serializer.DeserializeWithLengthPrefix<Log>(memoryStream, PrefixStyle.Base128));
            }
            Assert.IsNotNull(logs2);
            List<Log> result = logs2.ToList();
            Assert.AreEqual(logs.Length, result.Count);
            for (int i = 0; i < logs.Length; i++)
                Assert.AreEqual(logs[i].ToString(), result[i].ToString());
        }

        [TestMethod]
        public void TestResource()
        {
            Contract.Assert(Resources.TestString != null);

            Log log = new Log(() => Resources.TestString, "p0");
            Assert.AreEqual(typeof(Resources).FullName +".TestString", log.ResourceProperty);
            Assert.AreEqual(string.Format(Resources.TestString, "p0"), log.Message);
        }

        [TestMethod]
        public void TestTranslations()
        {
            Contract.Assert(Resources.TestString != null);
            Log log = new Log(() => Resources.TestString, "p0");

            var culture = Resources.Culture;

            Resources.Culture = Translation.DefaultCulture;
            Trace.WriteLine(log.ToString(Log.VerboseFormat) + Environment.NewLine);
            Assert.AreEqual(string.Format(Resources.TestString, "p0"), log.Message);

            Resources.Culture = CultureInfo.InvariantCulture;
            Trace.WriteLine(log.ToString(Log.VerboseFormat, Resources.Culture) + Environment.NewLine);
            Assert.AreEqual(string.Format(Resources.TestString, "p0"), log.GetMessage(Resources.Culture));

            Resources.Culture = CultureHelper.GetCultureInfo("fr-FR");
            Trace.WriteLine(log.ToString(Log.VerboseFormat, Resources.Culture) + Environment.NewLine);
            Assert.AreEqual(string.Format(Resources.TestString, "p0"), log.GetMessage(Resources.Culture));

            Resources.Culture = CultureHelper.GetCultureInfo("de-DE");
            Trace.WriteLine(log.ToString(Log.VerboseFormat, Resources.Culture) + Environment.NewLine);
            Assert.AreEqual(string.Format(Resources.TestString, "p0"), log.GetMessage(Resources.Culture));

            Resources.Culture = culture;
        }

        private class ExecutionCounter
        {
            public int Count;

            public string Increment()
            {
                Count++;
                return "Incremented counter";
            }
        }

        [TestMethod]
        public void TestDeferredExecution()
        {
            Contract.Assert(Resources.TestString != null);
            ExecutionCounter ec = new ExecutionCounter();
            Assert.AreEqual(0, ec.Count, "Execution Counter incorrectly initialized.");
            Log.ValidLevels = LoggingLevels.AtLeastError;
            Log.Add(LoggingLevel.Error, () => ec.Increment());
            Assert.AreEqual(1, ec.Count, "The resource lambda was not executed.");
            Log.Add(LoggingLevel.Debugging, () => ec.Increment());
            Assert.AreEqual(1, ec.Count, "The resource lambda was executed when the logging level was too low.");
        }

        [TestCleanup]
        public void Cleanup()
        {
            Log.Flush().Wait();
        }
    }
}