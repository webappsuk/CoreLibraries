using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Utilities.DataAnnotations.Attributes.Validators;

namespace WebApplications.Utilities.Test.DataAnnotations.Attributes
{
    [TestClass]
    public class EmailTests
    {
        [Email(ErrorMessage = "ErrorTagTest")]
        public string Email { get; set; }

        [TestMethod]
        public void TestMethod1()
        {
            EmailAttribute compareValidator = typeof(EmailTests)
                .GetProperties()
                .First(property => property.Name == "Email")
                .GetCustomAttributes()
                .OfType<EmailAttribute>()
                .First();

            Base testValidator = compareValidator.GetExt4Validator();

            Assert.IsNull(testValidator.Field);
            Assert.AreEqual("ErrorTagTest", testValidator.ErrorTag);
            Assert.AreEqual("email", testValidator.Type);
        }
    }
}