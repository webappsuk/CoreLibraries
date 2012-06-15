using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Utilities.DataAnnotations.Attributes.Validators;
using WebApplications.Utilities.DataAnnotations.ExtJsValidators;

namespace WebApplications.Utilities.Test.DataAnnotations.Attributes
{
    [TestClass]
    public class CompareTests
    {
        [Compare("Compare2", ErrorMessage = "ErrorTagTest")]
        public string Compare1 { get; set; }

        public string Compare2 { get; set; }

        [TestMethod]
        public void CompareConstructorTests()
        {
            CompareAttribute compareValidator = typeof(CompareTests)
                .GetProperties()
                .First(property => property.Name == "Compare1")
                .GetCustomAttributes()
                .OfType<CompareAttribute>()
                .First();

            Compare testCompare = new Compare(compareValidator);

            Assert.AreEqual("compare", testCompare.Type);
            Assert.AreEqual("Compare2", testCompare.CompareTo);
            Assert.AreEqual("ErrorTagTest", testCompare.ErrorTag);

            Assert.AreEqual(null, testCompare.Field);

            Compare testAutoCompare = (Compare)compareValidator.GetExt4Validator();

            Assert.AreEqual(testAutoCompare.ErrorTag, testCompare.ErrorTag);
            Assert.AreEqual(testAutoCompare.Field, testCompare.Field);
            Assert.AreEqual(testAutoCompare.CompareTo, testCompare.CompareTo);
            Assert.AreEqual(testAutoCompare.Type, testCompare.Type);
        }
    }
}