#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities.Logging 
// Project: WebApplications.Utilities.Logging.Test
// File: LoggingTest.cs
// 
// This software, its object code and source code and all modifications made to
// the same (the “Software”) are, and shall at all times remain, the proprietary
// information and intellectual property rights of Web Applications (UK) Limited. 
// You are only entitled to use the Software as expressly permitted by Web
// Applications (UK) Limited within the Software Customisation and
// Licence Agreement (the “Agreement”).  Any copying, modification, decompiling,
// distribution, licensing, sale, transfer or other use of the Software other than
// as expressly permitted in the Agreement is expressly forbidden.  Web
// Applications (UK) Limited reserves its rights to take action against you and
// your employer in accordance with its contractual and common law rights
// (including injunctive relief) should you breach the terms of the Agreement or
// otherwise infringe its copyright or other intellectual property rights in the
// Software.
// 
// © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing;
using WebApplications.Utilities.Logging;
using WebApplications.Utilities.Logging.Configuration;
using WebApplications.Utilities.Logging.Interfaces;
using WebApplications.Utilities.Serialization;

namespace Utilities.Logging.Test
{
    [TestClass]
    public class LoggingTest
    {
        public const int Loops = 10000;
        public const int ChangeConfigAt = -500;

        [TestMethod]
        public void TestMemoryCache()
        {
            DateTime startDate = DateTime.Now;
            string message = "Test message " + Guid.NewGuid();
            Thread.Sleep(10);
            Log.Add(message);
            Log.Flush();
            IEnumerable<Log> logs = Log.Get(DateTime.Now, startDate);
            Assert.IsNotNull(logs);
            Assert.IsTrue(logs.Count() > 0, "No logs found!");
            Assert.IsTrue(logs.Any(l => l.Message == message), "No log with the message '{0}' found.", message);
            Log.Flush();
        }

        [TestMethod]
        public void TestPerformance()
        {
            Log.Add("Initialising Logs");
            ILogger traceLogger;
            if (Log.TryGetLogger("Trace Logger", out traceLogger))
                traceLogger.ValidLevels = LogLevels.AtLeastInformation;
            Stopwatch s = new Stopwatch();
            s.Start();
            Parallel.For(0, Loops, i =>
                                       {
                                           Log.Add("New log item {0}.", LogLevel.Debugging, i);
                                           if (ChangeConfigAt != i) return;

                                           // Change the configuration
                                           LoggingConfiguration newConfiguration = new LoggingConfiguration();
                                           newConfiguration.Loggers.Remove("Trace Logger");
                                           LoggingConfiguration.Active = newConfiguration;
                                       });

            Trace.WriteLine(s.ToString("{0} Loops", Loops));
            Log.Flush();

            s.Stop();

            Trace.WriteLine(s.ToString("Entire thread to Flush"));
        }

        [TestMethod]
        public void TestSerialization()
        {
            // Example log with context.
            Log.Add(new Dictionary<string, string> { { "A", "1" }, { "B", "2" } }, "Test log item {0}.", "Test");
            Log.Flush();
            List<Log> logs = Log.Get(DateTime.Now, 100).ToList();
            string serialized = logs.SerializeToString();
            Trace.WriteLine(serialized);
            List<Log> deserializedLogs = serialized.Deserialize<List<Log>>();

            Assert.AreEqual(logs.Count, deserializedLogs.Count);

            List<Log>.Enumerator le = logs.GetEnumerator();
            List<Log>.Enumerator de = deserializedLogs.GetEnumerator();
            while (le.MoveNext())
            {
                Assert.IsTrue(de.MoveNext());
                Assert.AreEqual(le.Current.ToString(), de.Current.ToString());
                Assert.AreEqual(le.Current.Xml.ToString(), de.Current.Xml.ToString());
                Trace.WriteLine(le.Current.Xml);
            }

            byte[] serializedBytes = logs.First().SerializeToByteArray();
            Trace.WriteLine(string.Format("First log took {0} bytes.", serializedBytes.Length));
        }

        [TestMethod]
        public void TestOperations()
        {
            Operation.Wrap(
                o =>
                    {
                        Assert.IsNotNull(Operation.Current);
                        Assert.AreEqual(o, Operation.Current);

                        Log.Add(OperationFunction("Test value", 1));

                        Log.Flush();
                    }, "TestOperations", instance: this);
        }

        [NotNull]
        public string OperationFunction([NotNull]string aValue, int anotherValue, TimeSpan timeSpan = default(TimeSpan))
        {
            return Operation.Wrap(
                () =>
                    {
                        Log.Add("Inside OperationFunction");
                        return aValue;
                    }, "OperationFunction",
                instance: this,
                arguments: new Dictionary<string, object>
                               {
                                   {"aValue", aValue},
                                   {"anotherValue", anotherValue},
                                   {"timeSpan", timeSpan}
                               }) ?? String.Empty;
        }

        [TestMethod]
        public void TestContext()
        {
            Operation.Wrap(
                () =>
                    {
                        using (new LogContext("First Value", "A").Region)
                        {
                            using (LogContext.CreateRegion(new Dictionary<string, string> { { "Second Value", "B" } }))
                            {
                                Log.Add("Test Context 1");
                                Log.Add(new LogContext("Third Value", "C", "First Value", null, "Forth Value"), "Test Context 2",
                                        1,
                                        2, null, "A string");
                                new LoggingException(new InvalidOperationException("A test invalid operation"),
                                                     "Test logging exception", 1, 2, null, "A string");
                            }
                            Log.Add("Test Context 3");
                        }
                        Log.Add("Test Context 4");
                        Log.Flush();
                    },
                "TestContext", instance: this);
        }

        [TestMethod]
        public void TestExcpetions()
        {
            new TestException();
        }

        public class TestException :LoggingException
        {
            public TestException() :this("Test {0}.", "message")
            { 
                
            }
            public TestException([NotNull] string message, [NotNull] params object[] parameters) : base(message, parameters)
            {
            }
        }
    }
}