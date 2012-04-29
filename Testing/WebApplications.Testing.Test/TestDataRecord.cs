using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing.Data;

namespace WebApplications.Testing.Test
{
    [TestClass]
    public class TestDataRecord
    {
        [TestMethod]
        public void TestMethod1()
        {
            /*
            using (SqlConnection connection = new SqlConnection(@"Data Source=developmentsvr\SQL2008R2;Initial Catalog=THG_Test;Integrated Security=true;"))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("Select TOP 10 * FROM Booking", connection))
                {
                    using (IDataReader dataReader = command.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        int fieldCount = dataReader.FieldCount;
                        object[] values = new object[fieldCount];
                        while (dataReader.Read())
                        {
                            dataReader.GetValues(values);
                            Trace.WriteLine(string.Join(", ", values));
                        }
                    }
                }
            }
             */

            RecordSetDefinition recordSetDefinition =
                new RecordSetDefinition(new ColumnDefinition("Id", SqlDbType.Int),
                                        new ColumnDefinition("Date", SqlDbType.DateTime),
                                        new ColumnDefinition("Name", SqlDbType.NVarChar),
                                        new ColumnDefinition("Value", SqlDbType.Variant));

            var maxTics = DateTime.MaxValue.Ticks;

            var a = SqlDbType.SmallDateTime;
            var smallTicks = (new DateTime(2079, 6, 7) - new DateTime(1900, 1, 1)).Ticks - 1;
            var minuteTicks = TimeSpan.TicksPerMinute;
            var minutes = smallTicks/minuteTicks;

            var timeSpanTicks = TimeSpan.FromHours(28).Ticks;

            ObjectRecord record = new ObjectRecord(recordSetDefinition, 1, DateTime.Now, "MyName");
        }

        [TestMethod]
        public void TestRandomiser()
        {
            // We test the Variant type as it randomly selects one of the other types under the hood.
            Parallel.For(
                0,
                99999,
                i => Tester.RandomGenerator.GenerateRandomSqlValue(SqlDbType.Variant, -1, 0.05));

        }
    }
}
