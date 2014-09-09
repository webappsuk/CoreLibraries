using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebApplications.Utilities.Logging.Test
{
    [TestClass]
    public class TranslationTest : LoggingTestBase
    {
        [TestInitialize]
        public void TestInitialize()
        {
            Translation.DefaultCulture = Resources.Culture = CultureInfo.InvariantCulture;
        }

        [TestMethod]
        public void TestGenericResourceHelpers()
        {
            Resources.Culture = Translation.DefaultCulture;
            Assert.AreEqual(Resources.TestString, Translation.GetResource<Resources>("TestString"));
            Resources.Culture = CultureInfo.InvariantCulture;
            Assert.AreEqual(Resources.TestString, Translation.GetResource<Resources>("TestString", Resources.Culture));
            Resources.Culture = CultureHelper.GetCultureInfo("fr-FR");
            Assert.AreEqual(Resources.TestString, Translation.GetResource<Resources>("TestString", Resources.Culture));
            Resources.Culture = CultureHelper.GetCultureInfo("de-DE");
            Assert.AreEqual(Resources.TestString, Translation.GetResource<Resources>("TestString", Resources.Culture));
        }

        [TestMethod]
        public void TestTypedResourceHelpers()
        {
            Resources.Culture = Translation.DefaultCulture;
            Assert.AreEqual(Resources.TestString, Translation.GetResource(typeof(Resources), "TestString"));
            Resources.Culture = CultureInfo.InvariantCulture;
            Assert.AreEqual(Resources.TestString, Translation.GetResource(typeof(Resources), "TestString", Resources.Culture));
            Resources.Culture = CultureHelper.GetCultureInfo("fr-FR");
            Assert.AreEqual(Resources.TestString, Translation.GetResource(typeof(Resources), "TestString", Resources.Culture));
            Resources.Culture = CultureHelper.GetCultureInfo("de-DE");
            Assert.AreEqual(Resources.TestString, Translation.GetResource(typeof(Resources), "TestString", Resources.Culture));
        }

        [TestMethod]
        public void TestStringResourceHelpers()
        {
            string tfn = typeof(Resources).FullName;
            // We do this to ensure the resource type has been seen before.
            Assert.IsNotNull(Translation.GetResourceManager<Resources>());

            Resources.Culture = Translation.DefaultCulture;
            Assert.AreEqual(Resources.TestString, Translation.GetResource(tfn, "TestString"));
            Resources.Culture = CultureInfo.InvariantCulture;
            Assert.AreEqual(Resources.TestString, Translation.GetResource(tfn, "TestString", Resources.Culture));
            Resources.Culture = CultureHelper.GetCultureInfo("fr-FR");
            Assert.AreEqual(Resources.TestString, Translation.GetResource(tfn, "TestString", Resources.Culture));
            Resources.Culture = CultureHelper.GetCultureInfo("de-DE");
            Assert.AreEqual(Resources.TestString, Translation.GetResource(tfn, "TestString", Resources.Culture));
        }
    }
}
