using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing;
using WebApplications.Utilities.Financials;

namespace WebApplications.Utilities.Test
{
    [TestClass]
    public class FinancialsTests : UtilitiesTestBase
    {
        private readonly CurrencyInfo _currency = CurrencyInfo.Get("GBP");
        private readonly decimal _amount = Random.RandomDecimal();

        [TestMethod]
        public void TestCanCreateInstance()
        {
            Financial financial = new Financial(null, 0);
            Assert.IsNotNull(financial);
        }

        [TestMethod]
        public void TestConstructorSetsCurrencyInfoPropertyToExpectedValue()
        {
            Financial financial = new Financial(_currency, 0);
            Assert.IsNotNull(financial.Currency);
            Assert.AreEqual("GBP", financial.Currency.Code);
        }

        [TestMethod]
        public void TestConstructorSetsAmountPropertyToExpectedValue()
        {
            Financial financial = new Financial(_currency, _amount);
            Assert.IsNotNull(financial.Currency);
            Assert.AreEqual(_amount, financial.Amount);
        }

        [TestMethod]
        public void TestToStringReturnsExpectedValue()
        {
            Financial financial = new Financial(_currency, _amount);
            Assert.IsNotNull(financial.Currency);
            Assert.AreEqual(string.Format("Financial {0}{1}", _amount, _currency.Code), financial.ToString());
        }

        [TestMethod]
        public void TestAddOperatorAddsTheAmountsInTheFinancials()
        {
            decimal amountA = Random.RandomDecimal();
            decimal amountB = Random.RandomDecimal();

            Financial financialA = new Financial(_currency, amountA);
            Financial financialB = new Financial(_currency, amountB);

            Financial result = financialA + financialB;
            Assert.IsNotNull(result);
            Assert.AreEqual(amountA + amountB, result.Amount);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestAddOperatorThrowsExceptionIfTheCurrenciesAreDifferent()
        {
            Financial financialA = new Financial(CurrencyInfo.Get("GBP"), 0);
            Financial financialB = new Financial(CurrencyInfo.Get("USD"), 0);
            Financial result = financialA + financialB;
        }

        [TestMethod]
        public void TestSubtractOperatorSubtractsTheAmountsInTheFinancials()
        {
            decimal amountA = Random.RandomDecimal();
            decimal amountB = Random.RandomDecimal();

            Financial financialA = new Financial(_currency, amountA);
            Financial financialB = new Financial(_currency, amountB);

            Financial result = financialA - financialB;
            Assert.IsNotNull(result);
            Assert.AreEqual(amountA - amountB, result.Amount);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestSubtractOperatorThrowsExceptionIfTheCurrenciesAreDifferent()
        {
            Financial financialA = new Financial(CurrencyInfo.Get("GBP"), 0);
            Financial financialB = new Financial(CurrencyInfo.Get("USD"), 0);
            Financial result = financialA - financialB;
        }

        [TestMethod]
        public void TestSubtractOperatorForDecimalSubtractionsReturnsCloneWithCorrectAmount()
        {
            decimal amountA = Random.RandomDecimal();
            decimal amountB = Random.RandomDecimal();

            Financial financialA = new Financial(_currency, amountA);
            Financial result = financialA - amountB;
            Assert.AreEqual(amountA - amountB, result.Amount);
            Assert.AreNotSame(financialA, result);
        }

        [TestMethod]
        public void TestAddOperatorForDecimalAdditionsReturnsCloneWithCorrectAmount()
        {
            decimal amountA = Random.RandomDecimal();
            decimal amountB = Random.RandomDecimal();

            Financial financialA = new Financial(_currency, amountA);
            Financial result = financialA + amountB;
            Assert.AreEqual(amountA + amountB, result.Amount);
            Assert.AreNotSame(financialA, result);
        }

        [TestMethod]
        public void TestLessThanOperatorReturnsTheExpectedValue()
        {
            decimal amountA = Random.RandomDecimal();
            decimal amountB = Random.RandomDecimal();
            bool calcResult = amountA < amountB;

            Financial financialA = new Financial(_currency, amountA);
            Financial financialB = new Financial(_currency, amountB);

            bool result = financialA < financialB;
            Assert.AreEqual(calcResult, result);
        }

        [TestMethod]
        public void TestMoreThanOperatorReturnsTheExpectedValue()
        {
            decimal amountA = Random.RandomDecimal();
            decimal amountB = Random.RandomDecimal();
            bool calcResult = amountA > amountB;

            Financial financialA = new Financial(_currency, amountA);
            Financial financialB = new Financial(_currency, amountB);

            bool result = financialA > financialB;
            Assert.AreEqual(calcResult, result);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestLessThanOperatorThrowsExceptionIfTheCurrenciesAreDifferent()
        {
            Financial financialA = new Financial(CurrencyInfo.Get("GBP"), 0);
            Financial financialB = new Financial(CurrencyInfo.Get("USD"), 0);
            bool result = financialA < financialB;
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestMoreThanOperatorThrowsExceptionIfTheCurrenciesAreDifferent()
        {
            Financial financialA = new Financial(CurrencyInfo.Get("GBP"), 0);
            Financial financialB = new Financial(CurrencyInfo.Get("USD"), 0);
            bool result = financialA > financialB;
        }

        [TestMethod]
        public void TestLessThanOrEqualToOperatorReturnsTheExpectedValue()
        {
            decimal amountA = Random.RandomDecimal();
            decimal amountB = Random.RandomDecimal();
            bool calcResult = amountA <= amountB;

            Financial financialA = new Financial(_currency, amountA);
            Financial financialB = new Financial(_currency, amountB);

            bool result = financialA <= financialB;
            Assert.AreEqual(calcResult, result);
        }

        [TestMethod]
        public void TestMoreThanOrEqualToOperatorReturnsTheExpectedValue()
        {
            decimal amountA = Random.RandomDecimal();
            decimal amountB = Random.RandomDecimal();
            bool calcResult = amountA >= amountB;

            Financial financialA = new Financial(_currency, amountA);
            Financial financialB = new Financial(_currency, amountB);

            bool result = financialA >= financialB;
            Assert.AreEqual(calcResult, result);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestLessThanOrEqualToOperatorThrowsExceptionIfTheCurrenciesAreDifferent()
        {
            Financial financialA = new Financial(CurrencyInfo.Get("GBP"), 0);
            Financial financialB = new Financial(CurrencyInfo.Get("USD"), 0);
            bool result = financialA <= financialB;
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestMoreThanOrEqualToOperatorThrowsExceptionIfTheCurrenciesAreDifferent()
        {
            Financial financialA = new Financial(CurrencyInfo.Get("GBP"), 0);
            Financial financialB = new Financial(CurrencyInfo.Get("USD"), 0);
            bool result = financialA >= financialB;
        }

        [TestMethod]
        public void TestMultiplyOperatorReturnsTheExpectedValue()
        {
            decimal amountA = Random.Next();
            decimal amountB = Random.Next();

            Financial financialA = new Financial(_currency, amountA);
            Financial financialB = new Financial(_currency, amountB);

            Financial result = financialA * financialB;
            Assert.AreEqual(amountA*amountB, result.Amount);
        }

        [TestMethod]
        public void TestEqualsComparerReturnsTrueWhenFinancialsAreEqual()
        {
            Financial financialA = new Financial(_currency, 10);
            Financial financialB = new Financial(_currency, 10);

            bool result = financialA.Equals(financialB);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestEqualityComparerReturnsFalseWhenFinancialsAreNotEqual()
        {
            Financial financialA = new Financial(_currency, 10);
            Financial financialB = new Financial(_currency, 15);

            bool result = financialA == financialB;
            Assert.IsFalse(result);
        }
    }
}
