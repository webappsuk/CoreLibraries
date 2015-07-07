#region © Copyright Web Applications (UK) Ltd, 2010.  All rights reserved.
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
// ©  Copyright Web Applications (UK) Ltd, 2010.  All rights reserved.
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Utilities.Logging;
using WebApplications.Utilities.Logging.Loggers;
using WebApplications.Utilities.Logging.Operations;

namespace WebApplications.Utilities.Test
{
    /// <summary>
    ///   Summary description for TestLogging
    /// </summary>
    [TestClass]
    public class TestLogging
    {
        private const string ConnectionString =
                @"Data Source=server2008r2tes\SQL2008R2;Initial Catalog=Log;uid=Babel;pwd=Babel;";

        private const string ConnectionStringInvalid =
                @"Data Source=server2008r2tesWRONG\SQL2008R2;Initial Catalog=Log;uid=Babel;pwd=Babel;";
        private const string OPERATION_NAME = "Test Operation";
        private const string OPERATION_NAME_WRAPPER = "Outer Test Operation";
        private const string OPERATION_CATEGORY_NAME = "Testing";

        private static void RemoveCounter(string categoryName)
        {
            if (PerformanceCounterCategory.Exists(categoryName))
            {
                PerformanceCounterCategory.Delete(categoryName);
            }
        }

        [TestMethod]
        public void RemoveTestingCounters()
        {
            RemoveCounter("WebApplications.Utilities.Logging: Creating Log");
            RemoveCounter("WebApplications.Utilities.Logging: Creating Logging Exception");
            RemoveCounter("WebApplications.Utilities.Logging: Garbage collecting memory logs");
            RemoveCounter("WebApplications.Utilities.Logging: Logging thread");
            RemoveCounter("WebApplications.Utilities.Test: Complex method signature");
            RemoveCounter("WebApplications.Utilities.Test: Test operation");
            RemoveCounter("Testing: Outer Test Operation");
            RemoveCounter("Testing: Test Operation");
        }

        [TestMethod]
        public void TestMemoryLogger()
        {
            DateTime testStart = DateTime.Now;

            Log.DefaultMemoryLogger.ValidLevels = LogLevels.All;

            IEnumerable<LogItem> testLogItems = AddTestLogItems(testStart);

            Thread.Sleep(5000);

            // Now retrieve the logs.
            IEnumerable<Log> logs = Log.GetForward(testStart, 10);

            foreach (string expectedOutput in testLogItems.Select(test => test.ExpectedOutput))
            {
                string output = expectedOutput;
                Assert.IsTrue(logs.Any(l => l.Message.Equals(output)));
            }
        }

        [TestMethod]
        public void SqlLoggerInvalidConnectionStringProcedure()
        {
            DateTime testStart = DateTime.Now;

            Log.DefaultMemoryLogger.ValidLevels = LogLevels.All;

            // Create a new SQL logger
            SqlLogger sqlLogger = new SqlLogger("Test SQL Logger", Guid.NewGuid(), ConnectionStringInvalid);

            // Add the SQL logger
            Log.GetOrAddLogger(sqlLogger);

            AddTestLogItems(testStart);

            Thread.Sleep(15000);

            // Now retrieve the logs.
            IEnumerable<Log> logs = Log.GetForward(testStart, 10);

            Assert.IsTrue(
                    logs.Any(l => l.Level == LogLevel.Critical) &&
                    logs.Any(
                            l =>
                            l.Level == LogLevel.Information && l.ExceptionType != null &&
                            l.ExceptionType.Equals("System.Data.SqlClient.SqlException")));
        }

        [TestMethod]
        public void SqlLoggerInvalidAddStoredProcedure()
        {
            DateTime testStart = DateTime.Now;

            Log.DefaultMemoryLogger.ValidLevels = LogLevels.All;

            string invalidStoredProcName = Guid.NewGuid().ToString();

            // Create a new SQL logger
            SqlLogger sqlLogger = new SqlLogger(
                    "Test SQL Logger", Guid.NewGuid(), ConnectionString, invalidStoredProcName);

            // Add the SQL logger
            Log.GetOrAddLogger(sqlLogger);

            AddTestLogItems(testStart);

            Thread.Sleep(15000);

            // Now retrieve the logs.
            IEnumerable<Log> logs = Log.GetForward(testStart, 10);

            // Ensire that there is a critical log level item, and that there is a information log level of type SQL.
            Assert.IsTrue(
                    logs.Any(l => l.Level == LogLevel.Critical) &&
                    logs.Any(
                            l =>
                            l.Level == LogLevel.Information && l.ExceptionType != null &&
                            l.ExceptionType.Equals("System.Data.SqlClient.SqlException")));
        }

        [TestMethod]
        public void SqlLoggerInvalidGetStoredProcedure()
        {
            DateTime testStart = DateTime.Now;

            Log.DefaultMemoryLogger.ValidLevels = LogLevels.All;

            // Create a new SQL logger
            SqlLogger sqlLogger = new SqlLogger(
                    "Test SQL Logger", Guid.NewGuid(), ConnectionString, getStoredProcedure: Guid.NewGuid().ToString());

            // Add the SQL logger
            Log.GetOrAddLogger(sqlLogger);

            AddTestLogItems(testStart);
        }

        [TestMethod]
        public void TestSqlLogger()
        {
            DateTime testStart = DateTime.Now;

            Log.DefaultMemoryLogger.ValidLevels = LogLevels.All;

            // Create a new SQL logger
            SqlLogger sqlLogger = new SqlLogger("Test SQL Logger", 
                new Guid("E12A1F4A-6F69-4F35-B2A0-96B112C495D5"), 
                ConnectionString);

            // Add the SQL logger
            Log.GetOrAddLogger(sqlLogger);

            // Every 500ms log some items.
            int x = 0;
            while (x < 50)
            {
                AddTestLogItemsWrapper(testStart);
                x++;

                Thread.Sleep(500);
            }

            // TODO: Retrieve and check we havent added the latest every time.
            
            //foreach (string expectedOutput in testLogItems.Select(test => test.ExpectedOutput))
            //{
            //    string output = expectedOutput;
            //    Assert.IsTrue(logs.Any(l => l.Operation.Name == OPERATION_NAME
            //        && l.Operation.CategoryName == OPERATION_CATEGORY_NAME
            //        && l.Operation.Parent != null
            //        && l.Operation.Parent.Name == OPERATION_NAME_WRAPPER
            //        && l.Message.Equals(output)));
            //}
        }

        [TestMethod]
        public void TestSqlLoggerMemoryCacheExpired()
        {
            DateTime testStart = DateTime.Now;

            Log.DefaultMemoryLogger.ValidLevels = LogLevels.All;

            // Create a new sql logger
            SqlLogger sqlLogger = new SqlLogger("Test SQL Logger",
                new Guid("E12A1F4A-6F69-4F35-B2A0-96B112C495D5"),
                ConnectionString);

            // Add the sql logger
            Log.GetOrAddLogger(sqlLogger);

            // Add the test log items
            var testLogItems = AddTestLogItemsWrapper(testStart);

            Thread.Sleep(2100000);

            // Force an immediate GC of all generations.
            GC.Collect(0, GCCollectionMode.Forced);
            GC.Collect(1, GCCollectionMode.Forced);
            GC.Collect(2, GCCollectionMode.Forced);

            // Give the GC 120s econds (just to be safe).
            Thread.Sleep(120000);

            IEnumerable<Log> logs = Log.GetForward(sqlLogger, testStart, 10);

            //foreach (string expectedOutput in testLogItems.Select(test => test.ExpectedOutput))
            //{
            //    string output = expectedOutput;
            //    Assert.IsTrue(logs.Any(l => l.Operation.Name == OPERATION_NAME
            //        && l.Operation.CategoryName == OPERATION_CATEGORY_NAME
            //        && l.Operation.Parent != null
            //        && l.Operation.Parent.Name == OPERATION_NAME_WRAPPER
            //        && l.Message.Equals(output)));
            //}
        }

        [Operation(OPERATION_NAME_WRAPPER, OPERATION_CATEGORY_NAME)]
        private static IEnumerable<LogItem> AddTestLogItemsWrapper(DateTime start)
        {
            return AddTestLogItems(start);
        }

        [Operation(OPERATION_NAME, OPERATION_CATEGORY_NAME)]
        private static IEnumerable<LogItem> AddTestLogItems(DateTime testStart)
        {
            IEnumerable<LogItem> testLogItems = new List<LogItem>
                                                    {
                                                            new LogItem("Test Log Entry One", LogLevel.Information),
                                                            new LogItem("Test Log Entry Two", LogLevel.Error),
                                                            new LogItem(
                                                                    "Test Log Entry Three, Start of test: {0}",
                                                                    LogLevel.Error,
                                                                    new List<Object> {testStart}.ToArray()),
                                                    };

            foreach (LogItem l in testLogItems)
                Log.Add(l.StringFormat, l.LogLevel, l.Parameters);

            return testLogItems;
        }

        #region Nested type: LogItem
        private class LogItem
        {
            public readonly LogLevel LogLevel;

            public readonly object[] Parameters;
            public readonly string StringFormat;

            public LogItem(string stringFormat, LogLevel logLevel, object[] parameters = null)
            {
                this.StringFormat = stringFormat;
                this.LogLevel = logLevel;
                this.Parameters = parameters ?? new object[0];
            }

            public string ExpectedOutput
            {
                get
                {
                    if (this.Parameters == null ||
                        this.Parameters.Length < 1)
                        return this.StringFormat;

                    return string.Format(this.StringFormat, this.Parameters);
                }
            }
        }
        #endregion
    }
}