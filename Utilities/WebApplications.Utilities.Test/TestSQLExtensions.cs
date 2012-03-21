#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: WebApplications.Utilities 
// Project: WebApplications.Utilities.Test
// File: TestSQLExtensions.cs
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
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebApplications.Utilities.Test
{
    [TestClass]
    public class TestSQLExtensions
    {
        public const string ConnectionString =
            @"Data Source=server2008r2test\SQL2008R2;Initial Catalog=THG_Test;Integrated Security=true;";

        public const string TVPTestStoredProcedureName = "spThgTVPTest";
        public const string TVP2TestStoredProcedureName = "spThgTVPTest2";
        public const string TVP3TestStoredProcedureName = "spThgTVPTest3";

        public int Loops = 10;
        public int Size = 10000;

        [TestMethod]
        [Ignore]
        public void TestTVP()
        {
            IEnumerable<short> durations = new short[] {1, 2, 3, 4};
            IEnumerable<int> services = new[] {5, 6, 7, 8};
            IEnumerable<KeyValuePair<short, char>> commissions = new[]
                                                                     {
                                                                         new KeyValuePair<short, char>(1, 'A'),
                                                                         new KeyValuePair<short, char>(2, 'F'),
                                                                         new KeyValuePair<short, char>(5, 'G'),
                                                                         new KeyValuePair<short, char>(6, 'H'),
                                                                         new KeyValuePair<short, char>(8, 'J'),
                                                                         new KeyValuePair<short, char>(9, 'K')
                                                                     };


            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                // Open the connection
                sqlConnection.Open();
                using (
                    SqlCommand sqlCommand = new SqlCommand(TVPTestStoredProcedureName, sqlConnection)
                                                {CommandType = CommandType.StoredProcedure})
                {
                    sqlCommand.Parameters.AddWithValue("@Durations", durations, SqlDbType.SmallInt);
                    sqlCommand.Parameters.AddWithValue("@Services", services, SqlDbType.Int);
                    sqlCommand.Parameters.AddWithValue("@Commission",
                                                       commissions,
                                                       (kvp, record) =>
                                                           {
                                                               record.SetInt16(0, kvp.Key);
                                                               record.SetString(1, kvp.Value.ToString());
                                                               return true;
                                                           },
                                                       new SqlMetaData("Transaction_Type", SqlDbType.SmallInt),
                                                       new SqlMetaData("Bracket", SqlDbType.Char, 1));

                    // Execute command
                    using (SqlDataReader dataReader = sqlCommand.ExecuteReader())
                    {
                        using (IEnumerator<short> durationsEnumerator = durations.GetEnumerator())
                            while (dataReader.Read())
                            {
                                Assert.IsTrue(durationsEnumerator.MoveNext());
                                Assert.AreEqual(durationsEnumerator.Current, dataReader.GetInt16(0));
                            }

                        dataReader.NextResult();
                        using (IEnumerator<int> servicesEnumerator = services.GetEnumerator())
                            while (dataReader.Read())
                            {
                                Assert.IsTrue(servicesEnumerator.MoveNext());
                                Assert.AreEqual(servicesEnumerator.Current, dataReader.GetInt32(0));
                            }

                        dataReader.NextResult();
                        using (
                            IEnumerator<KeyValuePair<short, char>> commissionsEnumerator = commissions.GetEnumerator())
                            while (dataReader.Read())
                            {
                                Assert.IsTrue(commissionsEnumerator.MoveNext());
                                Assert.AreEqual(commissionsEnumerator.Current.Key, dataReader.GetInt16(0));
                                Assert.AreEqual(commissionsEnumerator.Current.Value, dataReader.GetString(1)[0]);
                            }
                    }
                }
            }
        }


        [TestMethod]
        public void TestTVP2()
        {
            IEnumerable<int> services = CreateEnumeration(Size);

            Stopwatch stopwatch = new Stopwatch();

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                // Open the connection
                sqlConnection.Open();

                for (int loop = 0; loop <= Loops; loop++)
                {
                    if (loop == 1)
                        stopwatch.Start();

                    using (
                        SqlCommand sqlCommand = new SqlCommand(TVP2TestStoredProcedureName, sqlConnection)
                                                    {CommandType = CommandType.StoredProcedure})
                    {
                        sqlCommand.Parameters.AddWithValue("@Services", services, SqlDbType.Int);

                        // Execute command
                        using (SqlDataReader dataReader = sqlCommand.ExecuteReader())
                        {
                            dataReader.NextResult();
                            using (IEnumerator<int> servicesEnumerator = services.GetEnumerator())
                                while (dataReader.Read())
                                {
                                    Assert.IsTrue(servicesEnumerator.MoveNext());
                                    Assert.AreEqual(servicesEnumerator.Current, dataReader.GetInt32(0));
                                }
                        }
                    }
                }
            }

            stopwatch.Stop();

            Trace.WriteLine(
                string.Format("SqlDataRecord{0}============={0}Size:{1}{0}Loops:{2}{0}Elapsed milliseconds: {3}{0}",
                              Environment.NewLine,
                              Size,
                              Loops,
                              stopwatch.ElapsedMilliseconds));
        }


        [TestMethod]
        public void TestTVP3()
        {
            // Create enumeration
            IEnumerable<int> services = CreateEnumeration(Size);

            Stopwatch stopwatch = new Stopwatch();

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                // Open the connection
                sqlConnection.Open();

                for (int loop = 0; loop <= Loops; loop++)
                {
                    if (loop == 1)
                        stopwatch.Start();

                    // Build a string representation of enumeration.
                    bool first = true;
                    string asString = services.Aggregate(
                        new StringBuilder(Size*5),
                        (builder, current) =>
                            {
                                if (first)
                                    first = false;
                                else
                                    builder.Append(",");
                                builder.Append(current);
                                return builder;
                            }).ToString();

                    using (
                        SqlCommand sqlCommand = new SqlCommand(TVP3TestStoredProcedureName, sqlConnection)
                                                    {CommandType = CommandType.StoredProcedure, CommandTimeout = 120})
                    {
                        sqlCommand.Parameters.AddWithValue("@Services", asString);

                        // Execute command
                        using (SqlDataReader dataReader = sqlCommand.ExecuteReader())
                        {
                            dataReader.NextResult();
                            using (IEnumerator<int> servicesEnumerator = services.GetEnumerator())
                                while (dataReader.Read())
                                {
                                    Assert.IsTrue(servicesEnumerator.MoveNext());
                                    Assert.AreEqual(servicesEnumerator.Current, dataReader.GetInt32(0));
                                }
                        }
                    }
                }
            }

            stopwatch.Stop();

            Trace.WriteLine(string.Format("MakeTable{0}========={0}Size:{1}{0}Loops:{2}{0}Elapsed milliseconds: {3}{0}",
                                          Environment.NewLine,
                                          Size,
                                          Loops,
                                          stopwatch.ElapsedMilliseconds));
        }

        [TestMethod]
        public void TestAll()
        {
            Loops = 10000;
            Size = 10;

            do
            {
                TestTVP2();
                TestTVP3();

                Size *= 10;
                Loops /= 10;
            } while (Loops > 1);
        }

        private IEnumerable<int> CreateEnumeration(int size)
        {
            Random random = new Random();
            List<int> enumeration = new List<int>(size);
            for (int a = 0; a < size; a++)
                enumeration.Add(random.Next());
            return enumeration;
        }
    }
}