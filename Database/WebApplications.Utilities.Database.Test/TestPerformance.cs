using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;
using Microsoft.SqlServer.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebApplications.Utilities.Database.Test
{
    [TestClass]
    [Ignore]
    public class TestPerformance
    {
        private const string ConnectionString =
            @"Data Source=.;Initial Catalog=BabelConfiguration41;Integrated Security=true;Asynchronous Processing=false;";
            //@"Data Source=Babel01\Sql2005;Initial Catalog=BabelConfiguration40;Integrated Security=true;Asynchronous Processing=true;";

        private const string ConnectionStringAsync =
            @"Data Source=.;Initial Catalog=BabelConfiguration41;Integrated Security=true;Asynchronous Processing=true;";
            //@"Data Source=Babel01\Sql2005;Initial Catalog=BabelConfiguration40;Integrated Security=true;Asynchronous Processing=true;";

        private const string ProcedureName = "spOperationModeGet";

        private const int Loops = 10;

        private const int TestLoops = 10;

        [TestMethod]
        public void TestNormal()
        {
            int rowCount = 0;

            Stopwatch stopwatch = new Stopwatch();

            Task[] tasks = new Task[Loops];

            stopwatch.Start();
            for (int i = 0; i < Loops; i++)
            {
                tasks[i] = Task.Factory.StartNew(() =>
                                          {
                                              using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                                              {
                                                  // Open the connection
                                                  sqlConnection.Open();

                                                  using (
                                                      SqlCommand sqlCommand = new SqlCommand(ProcedureName,
                                                                                             sqlConnection)
                                                                                  {
                                                                                      CommandType =
                                                                                          CommandType.StoredProcedure
                                                                                  })
                                                  {
                                                      // To be absolutely fair, this needs improving to accurately set the SqlParameters (not all the sizes are correct)
                                                      sqlCommand.Parameters.AddWithValue("@SystemProvider", "System");
                                                      sqlCommand.Parameters.AddWithValue("@OperationModeVersion", -1);
                                                      sqlCommand.Parameters.AddWithValue("@ConfigurationVersion", -1);

                                                      // Execute command
                                                      using (SqlDataReader dataReader = sqlCommand.ExecuteReader())
                                                      {
                                                          while (dataReader.Read())
                                                          {
                                                              // Operation modes
                                                              Interlocked.Increment(ref rowCount);
                                                          }
                                                          dataReader.NextResult();

                                                          while (dataReader.Read())
                                                          {
                                                              // Guids
                                                              Interlocked.Increment(ref rowCount);
                                                          }
                                                      }
                                                  }
                                              }
                                          });
            }

            Task.WaitAll(tasks);
            stopwatch.Stop();

            Trace.WriteLine(string.Format(
                "Normal{0}============={0}Loops:{1}{0}Rows:{2}{0}Elapsed milliseconds: {3}{0}",
                Environment.NewLine,
                Loops,
                rowCount,
                stopwatch.ElapsedMilliseconds));
        }

        [TestMethod]
        public void TestSqlProgram()
        {
            LoadBalancedConnection connection = new LoadBalancedConnection(ConnectionString);
            SqlProgram<string, int, int> getOpModes = new SqlProgram<string, int, int>(connection, ProcedureName, 
                "@SystemProvider",
                "@OperationModeVersion",
                "@ConfigurationVersion");
            int rowCount = 0;

            Stopwatch stopwatch = new Stopwatch();

            Task[] tasks = new Task[Loops];

            stopwatch.Start();
            for (int i = 0; i < Loops; i++)
            {
                tasks[i] = Task.Factory.StartNew(() =>
                {
                    getOpModes.ExecuteReader("System",
                                                                    -1,
                                                                    -1,
                                                                    reader =>
                                                                    {
                                                                        while (reader.Read())
                                                                        {
                                                                            // Operation modes
                                                                            Interlocked.Increment(ref rowCount);
                                                                        }
                                                                        reader.NextResult();

                                                                        while (reader.Read())
                                                                        {
                                                                            // Guids
                                                                            Interlocked.Increment(ref rowCount);
                                                                        }
                                                                    });
                });
            }

            Task.WaitAll(tasks);
            stopwatch.Stop();

            Trace.WriteLine(string.Format(
                "Program{0}============={0}Loops:{1}{0}Rows:{2}{0}Elapsed milliseconds: {3}{0}",
                Environment.NewLine,
                Loops,
                rowCount,
                stopwatch.ElapsedMilliseconds));
        }

        [TestMethod]
        public void TestSqlProgramAsync()
        {
            LoadBalancedConnection connection = new LoadBalancedConnection(ConnectionStringAsync);
            SqlProgram<string, int, int> getOpModes = new SqlProgram<string, int, int>(connection, ProcedureName,
                "@SystemProvider",
                "@OperationModeVersion",
                "@ConfigurationVersion");
            int rowCount = 0;

            Stopwatch stopwatch = new Stopwatch();

            Task[] tasks = new Task[Loops];

            stopwatch.Start();
            for (int i = 0; i < Loops; i++)
            {
                tasks[i] = getOpModes.ExecuteReaderAsync("System",
                                                         -1,
                                                         -1,
                                                         reader =>
                                                             {
                                                                 while (reader.Read())
                                                                 {
                                                                     // Operation modes
                                                                     Interlocked.Increment(ref rowCount);
                                                                 }
                                                                 reader.NextResult();

                                                                 while (reader.Read())
                                                                 {
                                                                     // Guids
                                                                     Interlocked.Increment(ref rowCount);
                                                                 }
                                                             });
            }

            Task.WaitAll(tasks);
            stopwatch.Stop();

            Trace.WriteLine(string.Format(
                "Async{0}============={0}Loops:{1}{0}Rows:{2}{0}Elapsed milliseconds: {3}{0}",
                Environment.NewLine,
                Loops,
                rowCount,
                stopwatch.ElapsedMilliseconds));
        }

        [TestMethod]
        public void TestAll()
        {
            Stopwatch stopwatch = new Stopwatch();
            for (int a = 0; a < TestLoops; a++)
            {
                if (a == 1)
                    stopwatch.Start();

                TestNormal();
                TestSqlProgram();
                TestSqlProgramAsync();
            }
            stopwatch.Stop();

            Trace.WriteLine(string.Empty);
            Trace.WriteLine(string.Format(
                "Total{0}============={0}Loops:{1}{0}{0}Elapsed milliseconds: {2}{0}",
                Environment.NewLine,
                TestLoops - 1,
                stopwatch.ElapsedMilliseconds));
        }

        [TestMethod]
        public void TestAPM()
        {
            Guid state = Guid.NewGuid();
            Trace.WriteLine("State: "+state);

            IAsyncResult ar = BeginAPM("System", -1, -1, APMCallback, state);
            Thread.Sleep(5000);
        }

        public static void APMCallback(IAsyncResult result)
        {
            Trace.WriteLine("APMCallback");
            Trace.WriteLine("State: "+result.AsyncState);
            int count = EndAPM(result);
            Trace.WriteLine("Count: "+count);
        }

        public static IAsyncResult BeginAPM(string systemProvider, int ov, int cv, AsyncCallback callback, object state)
        {
            Trace.WriteLine("BeginAPM Started");
            LoadBalancedConnection connection = new LoadBalancedConnection(ConnectionStringAsync);
            SqlProgram<string, int, int> getOpModes = new SqlProgram<string, int, int>(connection, ProcedureName,
                "@SystemProvider",
                "@OperationModeVersion",
                "@ConfigurationVersion");
            Task<int> t = getOpModes.ExecuteReaderAsync(systemProvider,
                                                   ov,
                                                   cv,
                                                   reader =>
                                                       {
                                                           int count = 0;
                                                           while (reader.Read())
                                                           {
                                                               // Operation modes
                                                               count++;
                                                           }
                                                           reader.NextResult();

                                                           while (reader.Read())
                                                           {
                                                               // Guids
                                                               count++;
                                                           }
                                                           Trace.WriteLine("Reader count: " + count);
                                                           return count;
                                                       }, state: state);
            Trace.WriteLine("Task state: "+t.AsyncState);
            if (callback != null) t.ContinueWith(_ => callback(t));

            Trace.WriteLine("BeginAPM Ended");
            return t;
        }

        public static  int EndAPM(IAsyncResult result)
        {
            Trace.WriteLine("EndAPM Started");
            return ((Task<int>) result).Result;
        }
    }
}
