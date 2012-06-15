using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Utilities.DataAnnotations.Attributes.Validators;

namespace WebApplications.Utilities.Test.DataAnnotations.Attributes
{
    [TestClass]
    public class BaseTests
    {
        [Required(ErrorMessage = "ErrorTagTest")]
        public string Required { get; set; }

        [TestMethod]
        public void TestBlankConstructorSetsEverythingToNull()
        {
            Base b = new Base();

            Assert.IsNull(b.ErrorTag);
            Assert.IsNull(b.Field);
            Assert.IsNull(b.Type);
        }

        [TestMethod]
        public void AttributeConstructorTest()
        {
            // if we make a new with the attribute constructor we should get some information
            RequiredAttribute requiredValidator = typeof(BaseTests)
                .GetProperties()
                .First(property => property.Name == "Required")
                .GetCustomAttributes()
                .OfType<RequiredAttribute>()
                .First();

            Base b = new Base(requiredValidator);

            Assert.IsNull(b.Field);
            Assert.AreEqual("ErrorTagTest", b.ErrorTag);
        }
    }
}