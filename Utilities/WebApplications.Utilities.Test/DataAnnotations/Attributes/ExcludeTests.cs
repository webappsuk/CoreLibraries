using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Utilities.DataAnnotations.Attributes.Validators;
using WebApplications.Utilities.DataAnnotations.ExtJsValidators;

namespace WebApplications.Utilities.Test.DataAnnotations.Attributes
{
    [TestClass]
    public class ExcludeTests
    {
        [Exclude("one", "two", ErrorMessage = "ErrorTagTest")]
        public string Exclude { get; set; }

        [TestMethod]
        public void TestMethod1()
        {
            ExcludeAttribute compareValidator = typeof (ExcludeTests)
                .GetProperties()
                .First(property => property.Name == "Exclude")
                .GetCustomAttributes()
                .OfType<ExcludeAttribute>()
                .First();

            Exclusion testValidator = (Exclusion)compareValidator.GetExt4Validator();

            Assert.IsNotNull(testValidator);
            Assert.IsNull(testValidator.Field);
            Assert.AreEqual("ErrorTagTest", testValidator.ErrorTag);
            Assert.AreEqual("exclusion", testValidator.Type);
            Assert.IsNotNull(testValidator.List);
            Assert.IsTrue(testValidator.List.Contains("one"));
            Assert.IsTrue(testValidator.List.Contains("two"));
            Assert.IsFalse(testValidator.List.Contains("three"));
        }
    }
}