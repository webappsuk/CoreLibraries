using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Utilities.DataAnnotations.Attributes.Validators;

namespace WebApplications.Utilities.Test.DataAnnotations.Attributes.Validators
{
    [TestClass]
    public class EmailTests
    {
        [TestMethod]
        public void TestEmailValidator()
        {
            EmailAttribute validator = new EmailAttribute();

            // bad object type
            Assert.IsFalse(validator.IsValid(1));

            TestBadEmailAddresses(validator);
            TestGoodEmailAddresses(validator);
        }

        private static void TestGoodEmailAddresses(EmailAttribute validator)
        {
            Assert.IsTrue(validator.IsValid("me@example.com"));
            Assert.IsTrue(validator.IsValid("m@example.com"));
            Assert.IsTrue(validator.IsValid("a.nonymous@example.com"));
            Assert.IsTrue(validator.IsValid("name+tag@example.com"));
            Assert.IsTrue(validator.IsValid("namez@tag@example.com"));
            Assert.IsTrue(validator.IsValid("spaces are allowed@example.com"));
            Assert.IsTrue(validator.IsValid("\"spaces may be quoted\"@example.com"));
            Assert.IsTrue(validator.IsValid("!#$%&\'*+-/=.?^_`{|}~@[1.0.0.127]"));
            Assert.IsTrue(validator.IsValid("me(this is a comment)@example.com"));
            Assert.IsTrue(validator.IsValid("!#$%&\'*+-/=.?^_`{|}~@[IPv6:0123:4567:89AB:CDEF:0123:4567:89AB:CDEF]"));
            Assert.IsTrue(validator.IsValid("试@例子.测试.مثال.آزمایشی"));
        }

        private static void TestBadEmailAddresses(EmailAttribute validator)
        {
            Assert.IsFalse(validator.IsValid("me@"));
            Assert.IsFalse(validator.IsValid("@example.com"));
            Assert.IsFalse(validator.IsValid("me.@example.com"));
            Assert.IsFalse(validator.IsValid(".me@example.com"));
            Assert.IsFalse(validator.IsValid("me@example..com"));
            Assert.IsFalse(validator.IsValid("me.example@com"));
            Assert.IsFalse(validator.IsValid("me\\@example.com"));
        }
    }
}