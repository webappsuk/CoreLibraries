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
using System.Data.SqlClient;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing.Data;

namespace WebApplications.Testing.Test
{
    [TestClass]
    public class Examples
    {
        [TestMethod]
        public void RecordExample()
        {
            // To create a record that implement IDataRecord we start with a record set definition.
            RecordSetDefinition recordSetDefinition = new RecordSetDefinition(
                new ColumnDefinition("ID", SqlDbType.Int),
                new ColumnDefinition("Name", SqlDbType.Char, 50),
                new ColumnDefinition("Description", SqlDbType.NVarChar),
                // This column is not nullable so defaults to true
                new ColumnDefinition("Active", SqlDbType.Bit, isNullable:false, defaultValue:true)
                );

            // Now we can create a record
            IObjectRecord dataRecord = new ObjectRecord(recordSetDefinition, 1, "Test", "This is my test record");

            // Or we can create one with random values
            IObjectRecord randomRecord = new ObjectRecord(recordSetDefinition, true);

            // To create a record that throws an exception we first create a SqlException
            // We can't do this directly, but we can use our prototypes to construct one.

            // SqlExceptions are made from a collection of SqlErrors - which can make like this :
            SqlErrorCollection errorCollection = new SqlErrorCollectionPrototype
                                                {
                                                    new SqlErrorPrototype(1000, 80, 17, "MyFakeServer",
                                                                          "Connection Timeout.", "spMySproc", 54)
                                                };

            SqlException sqlException = new SqlExceptionPrototype(errorCollection, "9.0.0.0", Guid.NewGuid());
            IObjectRecord exceptionRecord = new ExceptionRecord(sqlException);

            // We can stick these records into a recordset
            // Note the records must have the same RecordSetDefinition (unless it's an exception record)
            // The final record will through an exception when reached!
            ObjectSet recordSet = new ObjectSet(recordSetDefinition)
                                      {
                                          dataRecord,
                                          randomRecord,
                                          //exceptionRecord
                                      };

            // We can add recordsets to an ObjectReader
            ObjectReader reader = new ObjectReader
                                      {
                                          recordSet
                                      };

            // We can also add random record sets - this one has the same definition as the first.
            reader.Add(new RandomSet(recordSetDefinition));

            // We can also fix certain rows values using the column generators arry, a null indicates
            // that the column should us a random value, otherwise a lambda can be supplied - in this case
            // it sets the row to the row number (1 - indexed).
            reader.Add(new RandomSet(recordSetDefinition,
                                     columnGenerators: new Func<int, object>[] {null, row => "Row #" + row}));

            // Whereas this one has a random set of columns (with random types).
            reader.Add(new RandomSet(10));

            // Now that we have a reader we can use it like a normal reader - it even simulates disposal.
            using (IDataReader dataReader = reader)
            {
                int recordset = 1;
                do
                {
                    Trace.Write("Recordset #" + recordset);
                    int rows = 0;
                    while (dataReader.Read())
                        rows++;
                    Trace.WriteLine(" - " + rows + " rows.");
                    recordset++;
                } while (dataReader.NextResult());
            }
        }
    }
}