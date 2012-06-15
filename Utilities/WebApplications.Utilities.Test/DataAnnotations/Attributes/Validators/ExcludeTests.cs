using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Utilities.DataAnnotations.Attributes.Validators;

namespace WebApplications.Utilities.Test.DataAnnotations.Attributes.Validators
{
    [TestClass]
    public class ExcludeTests
    {
        [TestMethod]
        public void IsValidStrings()
        {
            ExcludeAttribute validator = new ExcludeAttribute("one", "two");

            Assert.IsTrue(validator.IsValid("three"));
            Assert.IsFalse(validator.IsValid("one"));
            Assert.IsFalse(validator.IsValid("two"));
        }

        [TestMethod]
        public void IsValidInts()
        {
            ExcludeAttribute validator = new ExcludeAttribute(1, 2);

            Assert.IsTrue(validator.IsValid(3));
            Assert.IsFalse(validator.IsValid(1));
            Assert.IsFalse(validator.IsValid(2));
        }

        [TestMethod]
        public void IsValidObjects()
        {
            var obj1 = new { value = 1, value2 = 2 };

            var obj2 = new { value = 1, value2 = 2 };

            ExcludeAttribute validator = new ExcludeAttribute(obj1);
            Assert.IsFalse(validator.IsValid(obj1));
            Assert.IsFalse(validator.IsValid(obj2));
            Assert.IsFalse(validator.IsValid(new { value = 1, value2 = 2 }));
            Assert.IsTrue(validator.IsValid(new { value3 = 3 }));
        }
    }
}