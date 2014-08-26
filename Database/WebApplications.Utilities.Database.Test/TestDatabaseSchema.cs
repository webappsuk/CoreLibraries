#region © Copyright Web Applications (UK) Ltd, 2011.  All rights reserved.
// Solution: Utilities.Database 
// Project: Utilities.Database.Test
// File: TestDatabaseSchema.cs
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
using System.Text;
using System.Xml;
using JetBrains.Annotations;
using Microsoft.SqlServer.Server;
using Microsoft.SqlServer.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Utilities.Database.Schema;
using System.Threading.Tasks;

namespace WebApplications.Utilities.Database.Test
{
    [TestClass]
    [Ignore]
    public class TestDatabaseSchema
    {
        [NotNull]
        public const string ConnectionString =
            @"Data Source=server2008r2test\SQL2008R2;Initial Catalog=THG_Test;Integrated Security=true;";

        public const string SProcName = "spThgWebPriceAvailability";
        public const string TVP2TestStoredProcedureName = "spThgTVPTest2";
        public const string TestTypesProgramName = "spTestTypes";

        public int Loops = 10;
        public int Size = 10000;

        public static readonly Connection Connection = new Connection(ConnectionString);

        [TestMethod]
        [NotNull]
        public async Task TestSchemaEquality()
        {
            DatabaseSchema databaseSchema = await DatabaseSchema.GetOrAdd((Connection)ConnectionString);
            DatabaseSchema databaseSchema2 = await DatabaseSchema.GetOrAdd((Connection)(ConnectionString + " "));
            Assert.AreEqual(databaseSchema, databaseSchema2);
        }

        [TestMethod]
        public void TestNoParams()
        {
            SqlProgram program = Create(Connection, "spThgWebData");
            program.ExecuteReader(
                reader =>
                    {
                        while (reader.Read())
                        {
                        }
                    });
        }

        [TestMethod]
        public void TestPrograms()
        {
            LoadBalancedConnection connection = new LoadBalancedConnection(ConnectionString, ConnectionString + " ");
            SqlProgram program = SqlProgram.Create(connection, SProcName);
            /* 
             * Example validation
             * 
            Dictionary<string, SqlDbType?> paramTypes = new Dictionary<string, SqlDbType?>
                                                            {
                                                                    {"@Brand_Id", SqlDbType.Int},
                                                                    {"@StartDate", SqlDbType.SmallDateTime},
                                                                    {"@Durations", SqlDbType.Structured}
                                                            };
            Assert.IsTrue(program.Definition.ValidateParameters(paramTypes));


             *
             * Example short hand
             * 
            Dictionary<string, object> parameters = new Dictionary<string, object>
                                                        {
                                                                {"@Brand_Id", 1575},
                                                                {"@StartDate", DateTime.Today.AddDays(7)},
                                                                {"@FinishDate", DateTime.Today.AddDays(10)},
                                                                {"@Durations",new Int16[] { 7 }.ToSqlParameterValue(new SqlMetaData("Value", SqlDbType.SmallInt))},
                                                                {"@Adults", 2}
                                                        };
            
            // Execute program.
            program.ExecuteReader(parameters,
                    reader =>
                        {
                            while (reader.Read())
                            {

                            }
                        });
             */

            IEnumerable<KeyValuePair<int, int>> units = new[]
                                                            {
                                                                new KeyValuePair<int, int>(4562, 6217),
                                                                new KeyValuePair<int, int>(4562, 56217),
                                                                new KeyValuePair<int, int>(4563, 6218),
                                                                new KeyValuePair<int, int>(4563, 56218),
                                                                new KeyValuePair<int, int>(4564, 6219),
                                                                new KeyValuePair<int, int>(4564, 6220),
                                                                new KeyValuePair<int, int>(4564, 6221),
                                                                new KeyValuePair<int, int>(4564, 6222),
                                                                new KeyValuePair<int, int>(4564, 56219),
                                                                new KeyValuePair<int, int>(4564, 56220),
                                                                new KeyValuePair<int, int>(4564, 56221),
                                                                new KeyValuePair<int, int>(4564, 56222),
                                                                new KeyValuePair<int, int>(4565, 6223),
                                                                new KeyValuePair<int, int>(4565, 6224),
                                                                new KeyValuePair<int, int>(4565, 56223),
                                                                new KeyValuePair<int, int>(4565, 56224),
                                                                new KeyValuePair<int, int>(4566, 6226),
                                                                new KeyValuePair<int, int>(4566, 56226),
                                                                new KeyValuePair<int, int>(4567, 6227),
                                                                new KeyValuePair<int, int>(4567, 56227),
                                                                new KeyValuePair<int, int>(4568, 6229),
                                                                new KeyValuePair<int, int>(4568, 6230),
                                                                new KeyValuePair<int, int>(4568, 6232),
                                                                new KeyValuePair<int, int>(4568, 6233),
                                                                new KeyValuePair<int, int>(4568, 6235),
                                                                new KeyValuePair<int, int>(4568, 6236),
                                                                new KeyValuePair<int, int>(4568, 24763),
                                                                new KeyValuePair<int, int>(4568, 56229),
                                                                new KeyValuePair<int, int>(4568, 56230),
                                                                new KeyValuePair<int, int>(4568, 56232),
                                                                new KeyValuePair<int, int>(4568, 56233),
                                                                new KeyValuePair<int, int>(4568, 56235),
                                                                new KeyValuePair<int, int>(4568, 56236),
                                                                new KeyValuePair<int, int>(4568, 74763),
                                                                new KeyValuePair<int, int>(4573, 6273),
                                                                new KeyValuePair<int, int>(4573, 56273),
                                                                new KeyValuePair<int, int>(4575, 6278),
                                                                new KeyValuePair<int, int>(4575, 56278),
                                                                new KeyValuePair<int, int>(4576, 6281),
                                                                new KeyValuePair<int, int>(4576, 6282),
                                                                new KeyValuePair<int, int>(4576, 6283),
                                                                new KeyValuePair<int, int>(4576, 6284),
                                                                new KeyValuePair<int, int>(4576, 6287),
                                                                new KeyValuePair<int, int>(4576, 6288),
                                                                new KeyValuePair<int, int>(4576, 56281),
                                                                new KeyValuePair<int, int>(4576, 56282),
                                                                new KeyValuePair<int, int>(4576, 56283),
                                                                new KeyValuePair<int, int>(4576, 56284),
                                                                new KeyValuePair<int, int>(4576, 56287),
                                                                new KeyValuePair<int, int>(4576, 56288),
                                                                new KeyValuePair<int, int>(4579, 6303),
                                                                new KeyValuePair<int, int>(4579, 6304),
                                                                new KeyValuePair<int, int>(4579, 26912),
                                                                new KeyValuePair<int, int>(4579, 56303),
                                                                new KeyValuePair<int, int>(4579, 56304),
                                                                new KeyValuePair<int, int>(4579, 76912),
                                                                new KeyValuePair<int, int>(4582, 6328),
                                                                new KeyValuePair<int, int>(4582, 56328),
                                                                new KeyValuePair<int, int>(4584, 6331),
                                                                new KeyValuePair<int, int>(4584, 6332),
                                                                new KeyValuePair<int, int>(4584, 56331),
                                                                new KeyValuePair<int, int>(4584, 56332),
                                                                new KeyValuePair<int, int>(4590, 6365),
                                                                new KeyValuePair<int, int>(4590, 6366),
                                                                new KeyValuePair<int, int>(4590, 56365),
                                                                new KeyValuePair<int, int>(4590, 56366),
                                                                new KeyValuePair<int, int>(4602, 6430),
                                                                new KeyValuePair<int, int>(4602, 23081),
                                                                new KeyValuePair<int, int>(4602, 56430),
                                                                new KeyValuePair<int, int>(4602, 73081),
                                                                new KeyValuePair<int, int>(4606, 6463),
                                                                new KeyValuePair<int, int>(4606, 6464),
                                                                new KeyValuePair<int, int>(4606, 56463),
                                                                new KeyValuePair<int, int>(4606, 56464),
                                                                new KeyValuePair<int, int>(4611, 6489),
                                                                new KeyValuePair<int, int>(4611, 6490),
                                                                new KeyValuePair<int, int>(4611, 6491),
                                                                new KeyValuePair<int, int>(4611, 56489),
                                                                new KeyValuePair<int, int>(4611, 56490),
                                                                new KeyValuePair<int, int>(4611, 56491),
                                                                new KeyValuePair<int, int>(4612, 6494),
                                                                new KeyValuePair<int, int>(4612, 6495),
                                                                new KeyValuePair<int, int>(4612, 6496),
                                                                new KeyValuePair<int, int>(4612, 6497),
                                                                new KeyValuePair<int, int>(4612, 6498),
                                                                new KeyValuePair<int, int>(4612, 6499),
                                                                new KeyValuePair<int, int>(4612, 6500),
                                                                new KeyValuePair<int, int>(4612, 6501),
                                                                new KeyValuePair<int, int>(4612, 6502),
                                                                new KeyValuePair<int, int>(4612, 6503),
                                                                new KeyValuePair<int, int>(4612, 6504),
                                                                new KeyValuePair<int, int>(4612, 6505),
                                                                new KeyValuePair<int, int>(4612, 6506),
                                                                new KeyValuePair<int, int>(4612, 6507),
                                                                new KeyValuePair<int, int>(4612, 6508),
                                                                new KeyValuePair<int, int>(4612, 6509),
                                                                new KeyValuePair<int, int>(4612, 6510),
                                                                new KeyValuePair<int, int>(4612, 6511),
                                                                new KeyValuePair<int, int>(4612, 6512),
                                                                new KeyValuePair<int, int>(4612, 6514)
                                                            };

            SqlProgram<int, DateTime, DateTime, IEnumerable<Int16>, IEnumerable<KeyValuePair<int, int>>, int> p =
                new SqlProgram<int, DateTime, DateTime, IEnumerable<short>, IEnumerable<KeyValuePair<int, int>>, int>(
                    program,
                    "@Brand_Id", "@StartDate", "@FinishDate", "@Durations", "@Units", "@Adults");

            p.ExecuteReader(
                reader =>
                    {
                        while (reader.Read())
                        {
                        }
                    },
                1575,
                DateTime.Today.AddDays(7),
                DateTime.Today.AddDays(10),
                new Int16[] {7},
                units,
                2);
        }

        [TestMethod]
        public void NormalPerformance()
        {
            Random random = new Random();
            IEnumerable<int> services = CreateEnumeration(Size);

            Stopwatch stopwatch = new Stopwatch();

            for (int loop = 0; loop <= Loops; loop++)
            {
                if (loop == 1)
                    stopwatch.Start();

                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    // Open the connection
                    sqlConnection.Open();

                    using (
                        SqlCommand sqlCommand = new SqlCommand(TestTypesProgramName, sqlConnection)
                                                    {CommandType = CommandType.StoredProcedure})
                    {
                        byte[] bytes = new byte[10];
                        random.NextBytes(bytes);
                        // To be absolutely fair, this needs improving to accurately set the SqlParameters (not all the sizes are correct)
                        sqlCommand.Parameters.AddWithValue("@Code", "Test");
                        sqlCommand.Parameters.AddWithValue("@BigInt", random.Next());
                        sqlCommand.Parameters.AddWithValue("@Binary", bytes);
                        sqlCommand.Parameters.AddWithValue("@Bit", true);
                        sqlCommand.Parameters.AddWithValue("@Date", DateTime.Now);
                        sqlCommand.Parameters.AddWithValue("@DateTime", DateTime.Now);
                        sqlCommand.Parameters.AddWithValue("@DateTime2", DateTime.Now);
                        sqlCommand.Parameters.AddWithValue("@DateTimeOffset", DateTime.Now);
                        sqlCommand.Parameters.AddWithValue("@Decimal", 1M);
                        sqlCommand.Parameters.AddWithValue("@Float", random.NextDouble());
                        SqlParameter parameter = sqlCommand.Parameters.AddWithValue("@Geography", new SqlGeography());
                        parameter.UdtTypeName = "geography";
                        parameter = sqlCommand.Parameters.AddWithValue("@Geometry", new SqlGeometry());
                        parameter.UdtTypeName = "geometry";
                        parameter = sqlCommand.Parameters.AddWithValue("@HierarchyId", new SqlHierarchyId());
                        parameter.UdtTypeName = "hierarchyid";
                        sqlCommand.Parameters.AddWithValue("@Image", null);
                        sqlCommand.Parameters.AddWithValue("@Int", random.Next());
                        sqlCommand.Parameters.AddWithValue("@Money", 1M);
                        sqlCommand.Parameters.AddWithValue("@NChar", string.Empty);
                        sqlCommand.Parameters.AddWithValue("@NText", string.Empty);
                        sqlCommand.Parameters.AddWithValue("@Numeric", 1M);
                        sqlCommand.Parameters.AddWithValue("@NVarChar", string.Empty);
                        sqlCommand.Parameters.AddWithValue("@Real", 1F);
                        sqlCommand.Parameters.AddWithValue("@SmallInt", (short) random.Next(0, 128));
                        sqlCommand.Parameters.AddWithValue("@SmallMoney", 1M);
                        sqlCommand.Parameters.AddWithValue("@Variant", null);
                        sqlCommand.Parameters.AddWithValue("@Time", new TimeSpan(1, 0, 0));
                        sqlCommand.Parameters.AddWithValue("@TimeStamp", null);
                        sqlCommand.Parameters.AddWithValue("@UniqueIdentifier", Guid.NewGuid());
                        sqlCommand.Parameters.AddWithValue("@VarBinary", bytes);
                        sqlCommand.Parameters.AddWithValue("@VarChar", "Hello");
                        sqlCommand.Parameters.AddWithValue("@XML", null);
                        sqlCommand.Parameters.Add(
                            new SqlParameter("@PriceData", SqlDbType.Structured)
                                {
                                    Value =
                                        new[]
                                            {
                                                new Tuple<DateTime, short, short, decimal>(
                                                    DateTime.Now,
                                                    (short) random.Next(0, 128),
                                                    (short) random.Next(0, 128),
                                                    (decimal) random.NextDouble()),
                                                new Tuple<DateTime, short, short, decimal>(
                                                    DateTime.Now.AddMinutes(5),
                                                    (short) random.Next(0, 128),
                                                    (short) random.Next(0, 128),
                                                    (decimal) random.NextDouble())
                                            }.
                                        ToSqlParameterValue(
                                            (t, record) =>
                                                {
                                                    record.SetDateTime(0, t.Item1);
                                                    record.SetInt16(1, t.Item2);
                                                    record.SetInt16(2, t.Item3);
                                                    record.SetDecimal(3, t.Item4);
                                                    return true;
                                                },
                                            new SqlMetaData("Date", SqlDbType.DateTime),
                                            new SqlMetaData("Nights", SqlDbType.SmallInt),
                                            new SqlMetaData("Item", SqlDbType.SmallInt),
                                            new SqlMetaData("Price", SqlDbType.Decimal, 9, 2))
                                });

                        // Execute command
                        using (SqlDataReader dataReader = sqlCommand.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                // Validate return types
                            }
                            dataReader.NextResult();

                            while (dataReader.Read())
                            {
                                // Validate return types
                            }
                        }
                    }
                }
            }

            stopwatch.Stop();

            Trace.WriteLine(string.Format(
                "Normal{0}============={0}Size:{1}{0}Loops:{2}{0}Elapsed milliseconds: {3}{0}",
                Environment.NewLine,
                Size,
                Loops,
                stopwatch.ElapsedMilliseconds));
        }


        [TestMethod]
        public void SqlProgramPerformance()
        {
            Random random = new Random();
            LoadBalancedConnection connection = new LoadBalancedConnection(ConnectionString);
            // Ridiculously long parameter list
            SqlProgram
                <string, long, byte[], bool, DateTime, DateTime, DateTime, DateTime, decimal, double, SqlGeography,
                    SqlGeometry, SqlHierarchyId, byte[], int, decimal, string, string, decimal, string, float, short,
                    decimal, object, TimeSpan, byte[], Guid, byte[], string, XmlReader, IEnumerable<SqlDataRecord>>
                program =
                    new SqlProgram
                        <string, long, byte[], bool, DateTime, DateTime, DateTime, DateTime, decimal, double,
                            SqlGeography, SqlGeometry, SqlHierarchyId, byte[], int, decimal, string, string,
                            decimal, string, Single, Int16, decimal, object, TimeSpan, byte[], Guid, byte[],
                            string, XmlReader, IEnumerable<SqlDataRecord>>(
                        connection,
                        TestTypesProgramName,
                        "@Code",
                        "@BigInt",
                        "@Binary",
                        "@Bit",
                        "@Date",
                        "@DateTime",
                        "@DateTime2",
                        "@DateTimeOffset",
                        "@Decimal",
                        "@Float",
                        "@Geography",
                        "@Geometry",
                        "@HierarchyId",
                        "@Image",
                        "@Int",
                        "@Money",
                        "@NChar",
                        "@NText",
                        "@Numeric",
                        "@NVarChar",
                        "@Real",
                        "@SmallInt",
                        "@SmallMoney",
                        "@Variant",
                        "@Time",
                        "@TimeStamp",
                        "@UniqueIdentifier",
                        "@VarBinary",
                        "@VarChar",
                        "@XML",
                        "@PriceData");

            IEnumerable<int> services = CreateEnumeration(Size);

            Stopwatch stopwatch = new Stopwatch();

            for (int loop = 0; loop <= Loops; loop++)
            {
                if (loop == 1)
                    stopwatch.Start();

                byte[] bytes = new byte[10];
                random.NextBytes(bytes);
                program.ExecuteReader(
                    dataReader =>
                        {
                            while (dataReader.Read())
                            {
                                // Validate return types
                            }
                            dataReader.NextResult();

                            while (dataReader.Read())
                            {
                                // Validate return types
                            }
                        },
                    "Test",
                    random.Next(),
                    bytes,
                    true,
                    DateTime.Now,
                    DateTime.Now,
                    DateTime.Now,
                    DateTime.Now,
                    1M,
                    random.NextDouble(),
                    new SqlGeography(),
                    new SqlGeometry(),
                    new SqlHierarchyId(),
                    null,
                    random.Next(),
                    1M,
                    string.Empty,
                    string.Empty,
                    1M,
                    string.Empty,
                    1F,
                    (short) random.Next(0, 128),
                    1M,
                    null,
                    new TimeSpan(1, 0, 0),
                    null,
                    Guid.NewGuid(),
                    bytes,
                    "Hello",
                    null,
                    new[]
                        {
                            new Tuple<DateTime, short, short, decimal>(
                                DateTime.Now,
                                (short) random.Next(0, 128),
                                (short) random.Next(0, 128),
                                (decimal) random.NextDouble()),
                            new Tuple<DateTime, short, short, decimal>(
                                DateTime.Now.AddMinutes(5),
                                (short) random.Next(0, 128),
                                (short) random.Next(0, 128),
                                (decimal) random.NextDouble())
                        }.ToSqlParameterValue(
                            (t, record) =>
                                {
                                    record.SetDateTime(0, t.Item1);
                                    record.SetInt16(1, t.Item2);
                                    record.SetInt16(2, t.Item3);
                                    record.SetDecimal(3, t.Item4);
                                    return true;
                                },
                            new SqlMetaData("Date", SqlDbType.DateTime),
                            new SqlMetaData("Nights", SqlDbType.SmallInt),
                            new SqlMetaData("Item", SqlDbType.SmallInt),
                            new SqlMetaData("Price", SqlDbType.Decimal, 9, 2)));
            }

            stopwatch.Stop();

            Trace.WriteLine(
                string.Format("Program{0}============={0}Size:{1}{0}Loops:{2}{0}Elapsed milliseconds: {3}{0}",
                              Environment.NewLine,
                              Size,
                              Loops,
                              stopwatch.ElapsedMilliseconds));

            SqlProgram p = Create(ConnectionString, "spTest");

            DatabaseSchema schema = DatabaseSchema.GetOrAdd(ConnectionString);


            SqlProgram<int, string> prog2 = new SqlProgram<int, string>(p, "@Int", "@Str");
            prog2.ExecuteNonQuery(1, "Hello");
            prog2.ExecuteReader(
                reader =>
                {
                    while (reader.Read())
                    {
                    }
                },
                1,
                "Hello");
        }

        /// <summary>
        /// Tests both performance mode.
        /// 
        /// This shows that SqlProgram is ~1% slower over a large number of iterations.  (up to ~8% slower with only 10 loops).
        /// 
        /// This is an acceptable degradation in performance considering the benefits.
        /// </summary>
        [TestMethod]
        public void TestAll()
        {
            Loops = 10000;
            Size = 10;

            do
            {
                NormalPerformance();
                SqlProgramPerformance();

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

        [TestMethod]
        public void TestTypes()
        {
            Random random = new Random();
            LoadBalancedConnection connection = new LoadBalancedConnection(ConnectionString);
            SqlProgram
                <string, long, byte[], bool, DateTime, DateTime, DateTime, DateTime, decimal, double, SqlGeography,
                    SqlGeometry, SqlHierarchyId, byte[], int, decimal, string, string, decimal, string, float, short,
                    decimal, object, TimeSpan, byte[], Guid, byte[], string, XmlReader, IEnumerable<SqlDataRecord>>
                program =
                    new SqlProgram
                        <string, long, byte[], bool, DateTime, DateTime, DateTime, DateTime, decimal, double,
                            SqlGeography, SqlGeometry, SqlHierarchyId, byte[], int, decimal, string, string,
                            decimal, string, Single, Int16, decimal, object, TimeSpan, byte[], Guid, byte[],
                            string, XmlReader, IEnumerable<SqlDataRecord>>(
                        connection,
                        TestTypesProgramName,
                        "@Code",
                        "@BigInt",
                        "@Binary",
                        "@Bit",
                        "@Date",
                        "@DateTime",
                        "@DateTime2",
                        "@DateTimeOffset",
                        "@Decimal",
                        "@Float",
                        "@Geography",
                        "@Geometry",
                        "@HierarchyId",
                        "@Image",
                        "@Int",
                        "@Money",
                        "@NChar",
                        "@NText",
                        "@Numeric",
                        "@NVarChar",
                        "@Real",
                        "@SmallInt",
                        "@SmallMoney",
                        "@Variant",
                        "@Time",
                        "@TimeStamp",
                        "@UniqueIdentifier",
                        "@VarBinary",
                        "@VarChar",
                        "@XML",
                        "@PriceData");

            /*
             * Old Skool check
            program.Definition.ValidateParameters(
                    typeof(string),
                    typeof(Int64),
                    typeof(byte[]),
                    typeof(bool),
                    typeof(DateTime),
                    typeof(DateTime),
                    typeof(DateTime),
                    typeof(DateTime),
                    typeof(decimal),
                    typeof(double),
                    typeof(SqlGeography),
                    typeof(SqlGeometry),
                    typeof(SqlHierarchyId),
                    typeof(byte[]),
                    typeof(int),
                    typeof(decimal),
                    typeof(string),
                    typeof(string),
                    typeof(decimal),
                    typeof(string),
                    typeof(Single),
                    typeof(Int16),
                    typeof(decimal),
                    typeof(object),
                    typeof(TimeSpan),
                    typeof(byte[]),
                    typeof(Guid),
                    typeof(byte[]),
                    typeof(string),
                    typeof(XmlReader),
                    typeof(IEnumerable<SqlDataRecord>));
             */

            byte[] bytes = new byte[10];
            random.NextBytes(bytes);
            program.ExecuteReader(
                dataReader =>
                    {
                        while (dataReader.Read())
                        {
                            // Validate return types
                        }
                        dataReader.NextResult();

                        while (dataReader.Read())
                        {
                            // Validate return types
                        }
                    },
                "Test",
                random.Next(),
                bytes,
                true,
                DateTime.Now,
                DateTime.Now,
                DateTime.Now,
                DateTime.Now,
                1M,
                random.NextDouble(),
                new SqlGeography(),
                new SqlGeometry(),
                new SqlHierarchyId(),
                null,
                random.Next(),
                1M,
                string.Empty,
                string.Empty,
                1M,
                string.Empty,
                1F,
                (short) random.Next(0, 128),
                1M,
                null,
                new TimeSpan(1, 0, 0),
                null,
                Guid.NewGuid(),
                bytes,
                "Hello",
                null,
                new[]
                    {
                        new Tuple<DateTime, short, short, decimal>(
                            DateTime.Now,
                            (short) random.Next(0, 128),
                            (short) random.Next(0, 128),
                            (decimal) random.NextDouble()),
                        new Tuple<DateTime, short, short, decimal>(
                            DateTime.Now.AddMinutes(5),
                            (short) random.Next(0, 128),
                            (short) random.Next(0, 128),
                            (decimal) random.NextDouble())
                    }.ToSqlParameterValue(
                        (t, record) =>
                            {
                                record.SetDateTime(0, t.Item1);
                                record.SetInt16(1, t.Item2);
                                record.SetInt16(2, t.Item3);
                                record.SetDecimal(3, t.Item4);
                                return true;
                            },
                        new SqlMetaData("Date", SqlDbType.DateTime),
                        new SqlMetaData("Nights", SqlDbType.SmallInt),
                        new SqlMetaData("Item", SqlDbType.SmallInt),
                        new SqlMetaData("Price", SqlDbType.Decimal, 9, 2)));
        }

        /// <summary>
        /// Creates the code for the generic overload of SqlProgram.
        /// </summary>
        [TestMethod]
        public void CreateCode()
        {
            for (int count = 2; count < 33; count++)
            {
                string type = "T" + count;
                StringBuilder types = new StringBuilder();
                StringBuilder typeofs = new StringBuilder();
                StringBuilder code = new StringBuilder();
                StringBuilder comments = new StringBuilder();
                StringBuilder constComments = new StringBuilder();
                StringBuilder parameters = new StringBuilder();
                StringBuilder parameters2 = new StringBuilder();
                StringBuilder constParameters = new StringBuilder();
                StringBuilder constParameters2 = new StringBuilder();
                bool first = true;
                for (int loop = 1; loop <= count; loop++)
                {
                    string join = first ? string.Empty : ", ";

                    if (loop < count)
                        types.Append(string.Format("{0}T{1}", join, loop));
                    typeofs.Append(string.Format("{0}typeof(T{1})", join, loop));
                    if (!first)
                    {
                        comments.Append(Environment.NewLine);
                        constComments.Append(Environment.NewLine);
                    }
                    comments.Append(string.Format(Resources.CommentTemplate, loop, loop.ToEnglish().ToLower()));
                    constComments.Append(string.Format(Resources.CommentTemplate2, loop, loop.ToEnglish().ToLower()));
                    parameters.Append(string.Format("{0}T{1} p{1}", join, loop));
                    parameters2.Append(string.Format("{0}p{1}", join, loop));

                    constParameters.Append(string.Format("{0}string p{1}Name", join, loop));
                    constParameters2.Append(string.Format("{0}p{1}Name", join, loop));
                    code.Append(
                        string.Format(
                            "{1}{0}{2}{0}{3}{0}{4}",
                            Environment.NewLine,
                            Resources.ExecuteScalar.Replace("{0}", comments.ToString()).Replace(
                                "{1}", parameters.ToString()).Replace("{2}", parameters2.ToString()),
                            Resources.ExecuteNonQuery.Replace("{0}", comments.ToString()).Replace(
                                "{1}", parameters.ToString()).Replace("{2}", parameters2.ToString()),
                            Resources.ExecuteReader.Replace("{0}", comments.ToString()).Replace(
                                "{1}", parameters.ToString()).Replace("{2}", parameters2.ToString()),
                            Resources.ExecuteXmlReader.Replace("{0}", comments.ToString()).Replace(
                                "{1}", parameters.ToString()).Replace("{2}", parameters2.ToString())));

                    first = false;
                }
                Trace.WriteLine(
                    Resources.RootCodeTemplate.Replace("{0}", types.ToString()).Replace("{1}", type).Replace(
                        "{2}", typeofs.ToString()).Replace("{3}", constComments.ToString()).Replace(
                            "{4}", constParameters.ToString()).Replace("{5}", constParameters2.ToString()).
                        Replace("{6}", code.ToString()));
                Trace.WriteLine(String.Empty);
            }
        }
    }
}