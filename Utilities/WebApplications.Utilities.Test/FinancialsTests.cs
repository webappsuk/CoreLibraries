using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing;
using WebApplications.Utilities.Financials;

namespace WebApplications.Utilities.Test
{
    [TestClass]
    public class FinancialsTests : UtilitiesTestBase
    {
        private readonly CurrencyInfo _gbp = CurrencyInfo.Get("GBP");
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
            Financial financial = new Financial(_gbp, 0);
            Assert.IsNotNull(financial.Currency);
            Assert.AreEqual("GBP", financial.Currency.Code);
        }

        [TestMethod]
        public void TestConstructorSetsAmountPropertyToExpectedValue()
        {
            Financial financial = new Financial(_gbp, _amount);
            Assert.IsNotNull(financial.Currency);
            Assert.AreEqual(_amount, financial.Amount);
        }

        [TestMethod]
        public void TestToStringReturnsExpectedValue()
        {
            Financial financial = new Financial(_gbp, _amount);
            Assert.IsNotNull(financial.Currency);
            Assert.AreEqual(string.Format("Financial {0}{1}", _amount, _gbp.Code), financial.ToString());
        }

        [TestMethod]
        public void TestAddOperatorAddsTheAmountsInTheFinancials()
        {
            decimal amountA = Random.RandomDecimal();
            decimal amountB = Random.RandomDecimal();

            Financial financialA = new Financial(_gbp, amountA);
            Financial financialB = new Financial(_gbp, amountB);

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

            Financial financialA = new Financial(_gbp, amountA);
            Financial financialB = new Financial(_gbp, amountB);

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

            Financial financialA = new Financial(_gbp, amountA);
            Financial result = financialA - amountB;
            Assert.AreEqual(amountA - amountB, result.Amount);
            Assert.AreNotSame(financialA, result);
        }

        [TestMethod]
        public void TestAddOperatorForDecimalAdditionsReturnsCloneWithCorrectAmount()
        {
            decimal amountA = Random.RandomDecimal();
            decimal amountB = Random.RandomDecimal();

            Financial financialA = new Financial(_gbp, amountA);
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

            Financial financialA = new Financial(_gbp, amountA);
            Financial financialB = new Financial(_gbp, amountB);

            bool result = financialA < financialB;
            Assert.AreEqual(calcResult, result);
        }

        [TestMethod]
        public void TestMoreThanOperatorReturnsTheExpectedValue()
        {
            decimal amountA = Random.RandomDecimal();
            decimal amountB = Random.RandomDecimal();
            bool calcResult = amountA > amountB;

            Financial financialA = new Financial(_gbp, amountA);
            Financial financialB = new Financial(_gbp, amountB);

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

            Financial financialA = new Financial(_gbp, amountA);
            Financial financialB = new Financial(_gbp, amountB);

            bool result = financialA <= financialB;
            Assert.AreEqual(calcResult, result);
        }

        [TestMethod]
        public void TestMoreThanOrEqualToOperatorReturnsTheExpectedValue()
        {
            decimal amountA = Random.RandomDecimal();
            decimal amountB = Random.RandomDecimal();
            bool calcResult = amountA >= amountB;

            Financial financialA = new Financial(_gbp, amountA);
            Financial financialB = new Financial(_gbp, amountB);

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

            Financial financialA = new Financial(_gbp, amountA);
            Financial financialB = new Financial(_gbp, amountB);

            Financial result = financialA * financialB;
            Assert.AreEqual(amountA*amountB, result.Amount);
        }

        [TestMethod]
        public void TestEqualsComparerReturnsTrueWhenFinancialsAreEqual()
        {
            Financial financialA = new Financial(_gbp, 10);
            Financial financialB = new Financial(_gbp, 10);

            bool result = financialA.Equals(financialB);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestEqualityComparerReturnsFalseWhenFinancialsAreNotEqual()
        {
            Financial financialA = new Financial(_gbp, 10);
            Financial financialB = new Financial(_gbp, 15);

            bool result = financialA == financialB;
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestExchangeReturnsTheCurrentInstanceIfTheCurrencyParamIsTheSame()
        {
            Financial financialA = new Financial(_gbp, 10);
            CurrencyInfo currency = CurrencyInfo.Get("GBP");

            Financial exchangedFinancial = financialA.Exchange(currency);
            Assert.AreSame(financialA, exchangedFinancial);
        }

        [TestMethod]
        public void TestExchangeWithDefaultExchangeRateReturnsEqualAmount()
        {
            Financial financialA = new Financial(_gbp, 10);
            CurrencyInfo currency = CurrencyInfo.Get("USD");

            Financial exchangedFinancial = financialA.Exchange(currency);
            Assert.AreEqual(financialA.Amount, exchangedFinancial.Amount);
        }

        [TestMethod]
        public void TestExchageWithSpecifiedExchangeRateReturnsExpectedAmount()
        {
            Financial financialA = new Financial(_gbp, 10);
            CurrencyInfo currency = CurrencyInfo.Get("USD");

            Financial exchangedFinancial = financialA.Exchange(currency, 1.5M);
            Assert.AreEqual(15, exchangedFinancial.Amount);
        }

        [TestMethod]
        public void TestExchangeWithDefaultExchangeRateAndInputChargeReturnsExpectedAmount()
        {
            Financial financialA = new Financial(_gbp, 10);
            CurrencyInfo currency = CurrencyInfo.Get("USD");

            Financial exchangedFinancial = financialA.Exchange(currency, 1.0M, 20);
            Assert.AreEqual(30, exchangedFinancial.Amount);
        }

        [TestMethod]
        public void TestExchangeWithSpecificExchangeRateAndInputChargeReturnsExpectedAmount()
        {
            Financial financialA = new Financial(_gbp, 10);
            CurrencyInfo currency = CurrencyInfo.Get("USD");

            Financial exchangedFinancial = financialA.Exchange(currency, 1.25M, 0.75M);
            Assert.AreEqual(13.4375M, exchangedFinancial.Amount);
        }

        [TestMethod]
        public void TestExchangeWithDefaultRateAndInputChargeReturnsTheSameAmount()
        {
            Financial financialA = new Financial(_gbp, 10);
            CurrencyInfo currency = CurrencyInfo.Get("USD");

            Financial exchangedFinancial = financialA.Exchange(currency);
            Assert.AreEqual(financialA.Amount, exchangedFinancial.Amount);
        }

        [TestMethod]
        public void TestExchangeWithDefaultRateAndOutputChargeReturnsExpectedAmount()
        {
            Financial financialA = new Financial(_gbp, 10);
            CurrencyInfo currency = CurrencyInfo.Get("USD");

            Financial exchangedFinancial = financialA.Exchange(currency, 1, 0, 8);
            Assert.AreEqual(18, exchangedFinancial.Amount);
        }

        [TestMethod]
        public void TestExchangeWithSpecificRateAndOutputChargeReturnsExpectedResult()
        {
            Financial financialA = new Financial(_gbp, 10);
            CurrencyInfo currency = CurrencyInfo.Get("USD");

            Financial exchangedFinancial = financialA.Exchange(currency, 1.25M, 0, 6.5M);
            Assert.AreEqual(19, exchangedFinancial.Amount);
        }

        [TestMethod]
        public void TestSumCorrectlySumsTheAmountsInTheProvidedFinancials()
        {
            Financial financialA = new Financial(_gbp, 10);
            Financial financialB = new Financial(_gbp, 2036.54M);
            Financial financialC = new Financial(_gbp, -153);

            Assert.AreEqual(1893.54M, Financial.Sum(new[] { financialA, financialB, financialC }).Amount);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestSumWithNoFinancialsInEnumerableThrowsNoFinancialsException()
        {
            Financial.Sum(Enumerable.Empty<Financial>());
        }

        [TestMethod]
        public void TestAverageReturnsTheAverageAmountOfTheProvidedFinancials()
        {
            Financial financialA = new Financial(_gbp, 10);
            Financial financialB = new Financial(_gbp, 2036.54M);
            Financial financialC = new Financial(_gbp, -153);

            Assert.AreEqual(631.18M, Financial.Average(new[] { financialA, financialB, financialC }).Amount);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestAverageWithNoFinancialsInEnumerableThrowsNoFinancialsException()
        {
            Financial.Average(Enumerable.Empty<Financial>());
        }
    }
}
