using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing;

namespace WebApplications.Utilities.Test
{
    [TestClass]
    public class TestOptional : TestBase
    {
        [TestMethod]
        [Description("Tests the constructor sets all the object properties correctly")]
        public void ConstructorSetsPropertiesCorrectly()
        {
            // Generate random value
            string value = Tester.RandomGenerator.RandomString(30);

            // Create an optional string
            Optional<string> optional = new Optional<string>(value);

            // Validate the properties where set correctly
            Assert.AreEqual(value, optional.Value);
            Assert.IsTrue(optional.IsAssigned);
        }

        [TestMethod]
        [Description("Tests implicit casts set option values correctly.")]
        public void CheckImplicitCast()
        {
            // Generate random value
            string value = Tester.RandomGenerator.RandomString(30);

            // Cast string top optional.
            Optional<string> optional = value;

            // Validate properties were set correctly
            Assert.AreEqual(value, optional.Value);
            Assert.AreEqual(true, optional.IsAssigned);
        }

        [TestMethod]
        [Description("Tests the not operator flips the IsAssigned flag.")]
        public void Unassigned()
        {
            // Validate the properties were set correctly
            Assert.AreEqual(default(string), Optional<string>.Unassigned.Value);
            Assert.AreEqual(false, Optional<string>.Unassigned.IsAssigned);

            Assert.AreEqual(default(string), new Optional<string>().Value);
            Assert.AreEqual(false, new Optional<string>().IsAssigned);
        }

        [TestMethod]
        public void TestEquality()
        {
            Optional<int> one = new Optional<int>(1);
            Optional<int> two = new Optional<int>(2);
            Optional<int> anotherOne = new Optional<int>(1);
            Optional<int> unassignedOne = new Optional<int>();
            Optional<int> unassignedTwo = Optional<int>.Unassigned;

            Assert.AreEqual(one, one);
            Assert.AreEqual(one, anotherOne);
            Assert.AreEqual(one.GetHashCode(), anotherOne.GetHashCode(),
                "Two equal assigned objects must return the same hash code.");
            Assert.IsTrue(one.Equals(1));

            Assert.AreNotEqual(one, two);
            Assert.IsFalse(one.Equals(2));

            Assert.AreNotEqual(one, unassignedOne);
            Assert.IsFalse(unassignedOne.Equals(0));
            Assert.IsFalse(unassignedOne.Equals(1));

            Assert.IsFalse(one.Equals(null));
            Assert.IsFalse(unassignedOne.Equals(null));

            Assert.AreEqual(unassignedOne, unassignedTwo);
            Assert.AreEqual(unassignedOne.GetHashCode(), unassignedTwo.GetHashCode(),
                "Two equal unassigned objects must return the same hash code.");

            Assert.IsTrue(one == anotherOne);
            Assert.IsTrue(unassignedOne == unassignedTwo);
            Assert.IsFalse(one == unassignedOne);
            Assert.IsFalse(one == two);

            Assert.IsFalse(one != anotherOne);
            Assert.IsFalse(unassignedOne != unassignedTwo);
            Assert.IsTrue(one != unassignedOne);
            Assert.IsTrue(one != two);
        }

        [TestMethod]
        public void TestIsNull()
        {
            Assert.IsTrue(new Optional<string>(null).IsNull);
            Assert.IsFalse(new Optional<string>(string.Empty).IsNull);

            Assert.IsFalse(Optional<string>.Unassigned.IsNull);
            Assert.IsFalse(Optional<int>.Unassigned.IsNull);
        }

        [TestMethod]
        public void TestToString()
        {
            string value = Tester.RandomGenerator.RandomString(30);

            Assert.AreEqual(value, new Optional<string>(value).ToString());
            Assert.AreEqual("null", new Optional<string>(null).ToString());
            Assert.AreEqual("Unassigned", Optional<string>.Unassigned.ToString());
            Assert.AreEqual("Unassigned", new Optional<string>().ToString());
        }

        [TestMethod]
        public void TestUnassignedOnError()
        {
            Assert.AreEqual(string.Empty,
                Optional<string>.UnassignedOnError(() => string.Empty));
            Assert.AreEqual(null,
                Optional<string>.UnassignedOnError(() => null));
            Assert.AreEqual(Optional<string>.Unassigned,
                Optional<string>.UnassignedOnError(() => null, true));
            Assert.AreEqual(Optional<string>.Unassigned,
                Optional<string>.UnassignedOnError(() => { throw new Exception(); }));
            Assert.AreEqual(Optional<string>.Unassigned,
                Optional<string>.UnassignedOnError(() => { throw new Exception(); }, true));
        }

        private static bool TryFail<T>(T input, out T output)
        {
            output = default(T);
            return false;
        }
        private static bool TrySucceed<T>(T input, out T output)
        {
            output = input;
            return true;
        }

        [TestMethod]
        public void TestUnassignedOnFailure()
        {
            string value = Tester.RandomGenerator.RandomString(30);

            Assert.AreEqual(Optional<string>.Unassigned,
                Optional<string>.UnassignedOnFailure<string, string>(TryFail, value));
            Assert.AreEqual(value,
                Optional<string>.UnassignedOnFailure<string, string>(TrySucceed, value));
            Assert.AreEqual(null,
                Optional<string>.UnassignedOnFailure<string, string>(TrySucceed, null));
            Assert.AreEqual(Optional<string>.Unassigned,
                Optional<string>.UnassignedOnFailure<string, string>(TryFail, null, true));
            Assert.AreEqual(Optional<string>.Unassigned,
                Optional<string>.UnassignedOnFailure<string, string>(TrySucceed, null, true));
        }

        [TestMethod]
        public void TestAssignIfNotNull()
        {
            string value = Tester.RandomGenerator.RandomString(30);

            Assert.AreEqual(value, Optional<string>.AssignIfNotNull(value));
            Assert.AreEqual(Optional<string>.Unassigned, Optional<string>.AssignIfNotNull(null));
        }

        [TestMethod]
        public void TestCompareTo()
        {
            Optional<int> one = new Optional<int>(1);
            Optional<int> two = new Optional<int>(2);
            Optional<int> anotherOne = new Optional<int>(1);
            Optional<int> unassignedOne = new Optional<int>();
            Optional<int> unassignedTwo = Optional<int>.Unassigned;

            Assert.IsTrue(one.CompareTo(two) < 0);
            Assert.IsTrue(two.CompareTo(one) > 0);
            Assert.IsTrue(one.CompareTo(one) == 0);
            Assert.IsTrue(one.CompareTo(anotherOne) == 0);
            Assert.IsTrue(one.CompareTo(unassignedTwo) > 0);
            Assert.IsTrue(unassignedOne.CompareTo(unassignedTwo) == 0);
            Assert.IsTrue(unassignedOne.CompareTo(one) < 0);

            Assert.IsTrue(one.CompareTo(2) < 0);
            Assert.IsTrue(one.CompareTo(1) == 0);
            Assert.IsTrue(two.CompareTo(1) > 0);
            Assert.IsTrue(unassignedOne.CompareTo(1) < 0);
        }
    }

}
