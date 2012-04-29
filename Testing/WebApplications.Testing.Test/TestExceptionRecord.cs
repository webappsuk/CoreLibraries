using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing.Data;

namespace WebApplications.Testing.Test
{
    [TestClass]
    public class TestExceptionRecord
    {
        [TestMethod]
        public void TestSqlExceptionPrototype()
        {
            Parallel.For(0, 100,
                         i =>
                             {
                                 Random random = Tester.RandomGenerator;
                                 SqlErrorCollectionPrototype errorCollectionPrototype =
                                     new SqlErrorCollectionPrototype();

                                 int loops = Tester.RandomGenerator.Next(10) + 1;
                                 for (int loop = 0; loop < loops; loop++)
                                 {
                                     // Generate random values.
                                     int infoNumber = random.GenerateRandomInt32();
                                     byte errorState = random.GenerateRandomByte();
                                     byte errorClass = (byte) random.Next(1, 26);
                                     string server = random.GenerateRandomString();
                                     string errorMessage = random.GenerateRandomString();
                                     string procedure = random.GenerateRandomString();
                                     int lineNumber = random.GenerateRandomInt32();
                                     uint wind32ErrorCode = (uint) Math.Abs(random.GenerateRandomInt32());

                                     // Create prototype.
                                     SqlErrorPrototype sqlErrorPrototype = new SqlErrorPrototype(infoNumber, errorState,
                                                                                                 errorClass, server,
                                                                                                 errorMessage, procedure,
                                                                                                 lineNumber,
                                                                                                 wind32ErrorCode);

                                     // Test implicit cast
                                     SqlError sqlError = sqlErrorPrototype;
                                     Assert.IsNotNull(sqlError);

                                     // Check SqlError created properly
                                     Assert.AreEqual(infoNumber, sqlError.Number);
                                     Assert.AreEqual(errorState, sqlError.State);
                                     Assert.AreEqual(errorClass, sqlError.Class);
                                     Assert.AreEqual(server, sqlError.Server);
                                     Assert.AreEqual(errorMessage, sqlError.Message);
                                     Assert.AreEqual(procedure, sqlError.Procedure);
                                     Assert.AreEqual(lineNumber, sqlError.LineNumber);
                                     Assert.AreEqual(sqlErrorPrototype.ToString(), sqlError.ToString());

                                     errorCollectionPrototype.Add(sqlError);
                                 }

                                 Assert.AreEqual(loops, errorCollectionPrototype.Count);

                                 // Test implicit cast
                                 SqlErrorCollection collection = errorCollectionPrototype;

                                 Assert.AreSame(errorCollectionPrototype.SqlErrorCollection, collection);

                                 // Now create a SqlException
                                 Guid connectionId = Guid.NewGuid();
                                 SqlExceptionPrototype sqlExceptionPrototype = new SqlExceptionPrototype(collection,
                                                                                                         "9.0.0.0",
                                                                                                         connectionId);

                                 // Test implicit conversion
                                 SqlException sqlException = sqlExceptionPrototype;
                                 Assert.IsNotNull(sqlException);

                                 // Check SqlException created properly - it uses the first error from the collection.
                                 SqlError first = collection[0];
                                 Assert.AreEqual(first.Number, sqlException.Number);
                                 Assert.AreEqual(first.State, sqlException.State);
                                 Assert.AreEqual(first.Class, sqlException.Class);
                                 Assert.AreEqual(first.Server, sqlException.Server);
                                 //Assert.AreEqual(first.Message, sqlException.Message);
                                 Assert.AreEqual(first.Procedure, sqlException.Procedure);
                                 Assert.AreEqual(first.LineNumber, sqlException.LineNumber);
                             });

        }
    }
}
