#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: Utilities.Database 
// Project: Utilities.Database.Test
// File: TestBase.cs
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
using System.Diagnostics;
using System.IO;
using System.Linq;
using WebApplications.Utilities.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing;
using WebApplications.Utilities.Logging;

namespace WebApplications.Utilities.Database.Test
{
    [DeploymentItem("Data", "Data")]
    [DeploymentItem("Resources\\", "Resources")]
    public abstract class DatabaseTestBase : TestBase
    {
        protected static readonly Random Random = new Random();

        protected readonly string LocalDatabaseConnectionString = CreateConnectionString("LocalData");
        protected readonly string LocalDatabaseCopyConnectionString = CreateConnectionString("LocalDataCopy");
        protected readonly string DifferentLocalDatabaseConnectionString = CreateConnectionString("DifferentLocalData");

        private double _testStartTicks, _testEndTicks;

        /// <summary>
        /// Lazy loader for database connection
        /// </summary>
        [NotNull]
        private readonly Lazy<LoadBalancedConnection> _conn;

        /// <summary>
        /// Static constructor of the <see cref="T:System.Object"/> class, used to initialize the locatoin of the data directory for all tests.
        /// </summary>
        /// <remarks></remarks>
        static DatabaseTestBase()
        {
            // Find the data directory
            string path = Path.GetDirectoryName(typeof (DatabaseTestBase).Assembly.Location);
            string root = Path.GetPathRoot(path);
            string dataDirectory;
            do
            {
                // Look recursively for directory called Data containing mdf files.
                dataDirectory = Directory.GetDirectories(path, "Data", SearchOption.AllDirectories)
                    .SingleOrDefault(d => Directory.GetFiles(d, "*.mdf", SearchOption.TopDirectoryOnly).Any());

                // Move up a directory
                path = Path.GetDirectoryName(path);
            } while ((dataDirectory == null) &&
                     !String.IsNullOrWhiteSpace(path) &&
                     !path.Equals(root, StringComparison.CurrentCultureIgnoreCase));

            if (dataDirectory == null)
                throw new InvalidOperationException("Could not find the data directory.");

            // Set the DataDirectory data in the current AppDomain for use in connection strings.
            AppDomain.CurrentDomain.SetData("DataDirectory", dataDirectory);
        }

        protected DatabaseTestBase()
        {
            _conn = new Lazy<LoadBalancedConnection>(() => new LoadBalancedConnection(CreateConnectionString("LocalData")));
        }

        /// <summary>
        /// Returns a database connection
        /// </summary>
        /// <value>A database connection.</value>
        protected LoadBalancedConnection Connection
        {
            get { return _conn.Value; }
        }

        /// <summary>
        /// Creates the connection string.
        /// </summary>
        /// <param name="databaseName">Name of the database.</param>
        /// <param name="isAsync">if set to <see langword="true"/> allows asynchronous processing.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        [NotNull]
        protected static string CreateConnectionString(string databaseName)
        {
            return
                String.Format(@"Data Source=(localdb)\v11.0;AttachDbFilename=|DataDirectory|\{0}.mdf;Integrated Security=True;Connect Timeout=30;", databaseName);
        }

        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            Trace.WriteLine(String.Format("Begin test: {0}", TestContext.TestName));
            GC.Collect();
            GC.WaitForPendingFinalizers();
            _testStartTicks = Stopwatch.GetTimestamp();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _testEndTicks = Stopwatch.GetTimestamp();
            Trace.WriteLine(String.Format("Ending test: {0}, time taken {1}ms", TestContext.TestName,
                (_testEndTicks - _testStartTicks) / TimeSpan.TicksPerMillisecond));
            Log.Flush().Wait();
        }
    }
}