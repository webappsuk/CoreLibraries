using System;
using System.Data;
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
                new ColumnDefinition("Description", SqlDbType.NVarChar)
                );

            // Now we can create a record
            IObjectRecord dataRecord = new ObjectRecord(recordSetDefinition, 1, "Test", "This is my test record");

            // Or we can create one with random values
            IObjectRecord randomRecord = new ObjectRecord(recordSetDefinition, true);

            // We can stick these records into a recordset
            // Note the records must have the same RecordSetDefinition
            ObjectSet recordSet = new ObjectSet(recordSetDefinition)
                                      {
                                          dataRecord,
                                          randomRecord
                                      };

            // We can add recordsets to an ObjectReader
            ObjectReader reader = new ObjectReader
                                      {
                                          recordSet
                                      };

            // We can also add random record sets - this one has the same definition as the first.
            reader.Add(new RandomSet(recordSetDefinition));

            // Whereas this one has a random set of columns (with random types).
            reader.Add(new RandomSet(10));
        }
    }
}
