using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Utilities.DataAnnotations.Attributes.Validators;

namespace WebApplications.Utilities.Test.DataAnnotations.Attributes.Validators
{
    [TestClass]
    public class IncludeTests
    {
        [TestMethod]
        public void IsValidStrings()
        {
            IncludeAttribute validator = new IncludeAttribute("one", "two");

            Assert.IsInstanceOfType(validator.IsValid("one"), typeof(bool));
            Assert.IsTrue(validator.IsValid("one"));
            Assert.IsTrue(validator.IsValid("two"));
            Assert.IsFalse(validator.IsValid("three"));
        }

        [TestMethod]
        public void TestIntegersWorkWithCompareValidator()
        {
            IncludeAttribute validator = new IncludeAttribute(1, 2, 3);

            Assert.IsTrue(validator.IsValid(1));
            Assert.IsTrue(validator.IsValid(2));
            Assert.IsFalse(validator.IsValid(5));
        }

        [TestMethod]
        public void TestObjectsWorkWithCompareValidator()
        {
            var obj1 = new { value = 1, value2 = 2 };

            var obj2 = new { value = 1, value2 = 2 };

            IncludeAttribute validator = new IncludeAttribute(obj1);

            Assert.IsTrue(validator.IsValid(obj1));
            Assert.IsTrue(validator.IsValid(obj2));
            Assert.IsTrue(validator.IsValid(new { value = 1, value2 = 2 }));
            Assert.IsFalse(validator.IsValid(new { value3 = 3 }));
        }
    }
}