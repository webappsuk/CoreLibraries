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
using System.Data;
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

            long maxTics = DateTime.MaxValue.Ticks;

            SqlDbType a = SqlDbType.SmallDateTime;
            long smallTicks = (new DateTime(2079, 6, 7) - new DateTime(1900, 1, 1)).Ticks - 1;
            long minuteTicks = TimeSpan.TicksPerMinute;
            long minutes = smallTicks/minuteTicks;

            long timeSpanTicks = TimeSpan.FromHours(28).Ticks;

            ObjectRecord record = new ObjectRecord(recordSetDefinition, 1, DateTime.Now, "MyName");
        }

        [TestMethod]
        public void TestRandomiser()
        {
            // We test the Variant type as it randomly selects one of the other types under the hood.
            Parallel.For(
                0,
                99999,
                i => Tester.RandomGenerator.RandomSqlValue(SqlDbType.Variant, -1, 0.05));
        }
    }
}