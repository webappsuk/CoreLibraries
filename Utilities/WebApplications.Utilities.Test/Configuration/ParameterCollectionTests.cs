using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing;
using WebApplications.Utilities.Configuration;

namespace WebApplications.Utilities.Test.Configuration
{
    [TestClass]
    public class ParameterCollectionTests : ConfigurationTestBase
    {

        [TestMethod]
        public void ParameterCollection_Extends_ConfigurationElementCollection()
        {
            Assert.IsInstanceOfType(new ParameterCollection(), typeof(System.Configuration.ConfigurationElementCollection), "The ParameterCollection class should extend System.Configuration.ConfigurationElementCollection.");
        }
    }
}
