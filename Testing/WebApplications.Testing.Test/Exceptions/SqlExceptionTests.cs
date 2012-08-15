using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing.Data;
using WebApplications.Testing.Data.Exceptions;

namespace WebApplications.Testing.Test.Exceptions
{
    [TestClass]
    public class SqlExceptionTests
    {
        [TestMethod]
        public void TestCanCreateInstanceOfSqlEventSessionException()
        {
            SqlEventSessionStoppedException sessionStoppedException =
                new SqlEventSessionStoppedException();

            Assert.IsNotNull(sessionStoppedException);
            Assert.IsInstanceOfType(sessionStoppedException, typeof(SqlExceptionPrototype));
        }

        [TestMethod]
        public void TestCanCreateInstanceOfSqlInvalidConnectionException()
        {
            SqlInvalidConnectionException invalidConnectionException = 
                new SqlInvalidConnectionException();

            Assert.IsNotNull(invalidConnectionException);
            Assert.IsInstanceOfType(invalidConnectionException, typeof(SqlExceptionPrototype));
        }
    }
}
