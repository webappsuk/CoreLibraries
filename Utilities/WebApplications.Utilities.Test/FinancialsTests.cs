#region © Copyright Web Applications (UK) Ltd, 2014.  All rights reserved.
// Copyright (c) 2014, Web Applications UK Ltd
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of Web Applications UK Ltd nor the
//       names of its contributors may be used to endorse or promote products
//       derived from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL WEB APPLICATIONS UK LTD BE LIABLE FOR ANY
// DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Testing;
using WebApplications.Utilities.Financials;

namespace WebApplications.Utilities.Test
{
    [TestClass]
    public class FinancialsTests : UtilitiesTestBase
    {
        private readonly decimal _amount = Random.RandomDecimal();
        private static readonly CurrencyInfo _gbp = CurrencyInfoProvider.Current.Get("GBP");

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            if (_gbp == null)
                Assert.Inconclusive("The test GBP currency info is null");
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
            string expectedFormat = String.Format("{0:C}", _amount);
            Assert.AreEqual(expectedFormat, financial.ToString());
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
        [ExpectedException(typeof (InvalidOperationException))]
        public void TestAddOperatorThrowsExceptionIfTheCurrenciesAreDifferent()
        {
            Financial financialA = new Financial(CurrencyInfoProvider.Current.Get("GBP"), 0);
            Financial financialB = new Financial(CurrencyInfoProvider.Current.Get("USD"), 0);
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
        [ExpectedException(typeof (InvalidOperationException))]
        public void TestSubtractOperatorThrowsExceptionIfTheCurrenciesAreDifferent()
        {
            Financial financialA = new Financial(CurrencyInfoProvider.Current.Get("GBP"), 0);
            Financial financialB = new Financial(CurrencyInfoProvider.Current.Get("USD"), 0);
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
        [ExpectedException(typeof (InvalidOperationException))]
        public void TestLessThanOperatorThrowsExceptionIfTheCurrenciesAreDifferent()
        {
            Financial financialA = new Financial(CurrencyInfoProvider.Current.Get("GBP"), 0);
            Financial financialB = new Financial(CurrencyInfoProvider.Current.Get("USD"), 0);
            bool result = financialA < financialB;
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void TestMoreThanOperatorThrowsExceptionIfTheCurrenciesAreDifferent()
        {
            Financial financialA = new Financial(CurrencyInfoProvider.Current.Get("GBP"), 0);
            Financial financialB = new Financial(CurrencyInfoProvider.Current.Get("USD"), 0);
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
        [ExpectedException(typeof (InvalidOperationException))]
        public void TestLessThanOrEqualToOperatorThrowsExceptionIfTheCurrenciesAreDifferent()
        {
            Financial financialA = new Financial(CurrencyInfoProvider.Current.Get("GBP"), 0);
            Financial financialB = new Financial(CurrencyInfoProvider.Current.Get("USD"), 0);
            bool result = financialA <= financialB;
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void TestMoreThanOrEqualToOperatorThrowsExceptionIfTheCurrenciesAreDifferent()
        {
            Financial financialA = new Financial(CurrencyInfoProvider.Current.Get("GBP"), 0);
            Financial financialB = new Financial(CurrencyInfoProvider.Current.Get("USD"), 0);
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
            Assert.AreEqual(amountA * amountB, result.Amount);
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
            CurrencyInfo currency = CurrencyInfoProvider.Current.Get("GBP");

            Financial exchangedFinancial = financialA.Exchange(currency);
            Assert.AreSame(financialA, exchangedFinancial);
        }

        [TestMethod]
        public void TestExchangeWithDefaultExchangeRateReturnsEqualAmount()
        {
            Financial financialA = new Financial(_gbp, 10);
            CurrencyInfo currency = CurrencyInfoProvider.Current.Get("USD");

            Financial exchangedFinancial = financialA.Exchange(currency);
            Assert.AreEqual(financialA.Amount, exchangedFinancial.Amount);
        }

        [TestMethod]
        public void TestExchageWithSpecifiedExchangeRateReturnsExpectedAmount()
        {
            Financial financialA = new Financial(_gbp, 10);
            CurrencyInfo currency = CurrencyInfoProvider.Current.Get("USD");

            Financial exchangedFinancial = financialA.Exchange(currency, 1.5M);
            Assert.AreEqual(15, exchangedFinancial.Amount);
        }

        [TestMethod]
        public void TestExchangeWithDefaultExchangeRateAndInputChargeReturnsExpectedAmount()
        {
            Financial financialA = new Financial(_gbp, 10);
            CurrencyInfo currency = CurrencyInfoProvider.Current.Get("USD");

            Financial exchangedFinancial = financialA.Exchange(currency, 1.0M, 20);
            Assert.AreEqual(30, exchangedFinancial.Amount);
        }

        [TestMethod]
        public void TestExchangeWithSpecificExchangeRateAndInputChargeReturnsExpectedAmount()
        {
            Financial financialA = new Financial(_gbp, 10);
            CurrencyInfo currency = CurrencyInfoProvider.Current.Get("USD");

            Financial exchangedFinancial = financialA.Exchange(currency, 1.25M, 0.75M);
            Assert.AreEqual(13.4375M, exchangedFinancial.Amount);
        }

        [TestMethod]
        public void TestExchangeWithDefaultRateAndInputChargeReturnsTheSameAmount()
        {
            Financial financialA = new Financial(_gbp, 10);
            CurrencyInfo currency = CurrencyInfoProvider.Current.Get("USD");

            Financial exchangedFinancial = financialA.Exchange(currency);
            Assert.AreEqual(financialA.Amount, exchangedFinancial.Amount);
        }

        [TestMethod]
        public void TestExchangeWithDefaultRateAndOutputChargeReturnsExpectedAmount()
        {
            Financial financialA = new Financial(_gbp, 10);
            CurrencyInfo currency = CurrencyInfoProvider.Current.Get("USD");

            Financial exchangedFinancial = financialA.Exchange(currency, 1, 0, 8);
            Assert.AreEqual(18, exchangedFinancial.Amount);
        }

        [TestMethod]
        public void TestExchangeWithSpecificRateAndOutputChargeReturnsExpectedResult()
        {
            Financial financialA = new Financial(_gbp, 10);
            CurrencyInfo currency = CurrencyInfoProvider.Current.Get("USD");

            Financial exchangedFinancial = financialA.Exchange(currency, 1.25M, 0, 6.5M);
            Assert.AreEqual(19, exchangedFinancial.Amount);
        }

        [TestMethod]
        public void TestSumCorrectlySumsTheAmountsInTheProvidedFinancials()
        {
            Financial financialA = new Financial(_gbp, 10);
            Financial financialB = new Financial(_gbp, 2036.54M);
            Financial financialC = new Financial(_gbp, -153);

            Assert.AreEqual(1893.54M, Financial.Sum(new[] {financialA, financialB, financialC}).Amount);
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
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

            Assert.AreEqual(631.18M, Financial.Average(new[] {financialA, financialB, financialC}).Amount);
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void TestAverageWithNoFinancialsInEnumerableThrowsNoFinancialsException()
        {
            Financial.Average(Enumerable.Empty<Financial>());
        }

        [TestMethod]
        public void TestToStringFormatsUsingCurrentCultureWhenPossible()
        {
            Decimal amount = Random.RandomDecimal();
            CurrencyInfo currencyInfo = CurrencyInfoProvider.Current.Get(CultureInfo.CurrentUICulture);
            Financial financial = new Financial(currencyInfo, amount);
            String expectedFormat = String.Format(CultureInfo.CurrentUICulture, "{0:C}", amount);
            Assert.AreEqual(expectedFormat, financial.ToString());
        }

        [TestMethod]
        public void TestToStringFormatsUsingCurrentLanguageWhenPossible()
        {
            Decimal amount = Random.RandomDecimal();
            CurrencyInfo currencyInfo = CurrencyInfoProvider.Current.Get("EUR");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-GB");
            // No Euro in GB, but Ireland has and also at least speaks English (en).
            Financial financial = new Financial(currencyInfo, amount);
            String currentLanguage = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            List<String> expectedFormats =
                currencyInfo.Cultures.Where(c => c.TwoLetterISOLanguageName == currentLanguage).Select(
                    mc =>
                        String.Format(
                            mc,
                            "{0:C}",
                            amount)).
                    ToList();
            CollectionAssert.Contains(expectedFormats, financial.ToString());
        }

        [TestMethod]
        public void TestToStringFormatsUsingCorrectCurrencyWhenCurrencyOfCurrentUICultureDoesNotMatch()
        {
            Decimal amount = Random.RandomDecimal();
            CurrencyInfo currencyInfo = CurrencyInfoProvider.Current.Get("GBP");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("De-de");
            // No pound in Germany, nor any German speaking countries.
            Financial financial = new Financial(currencyInfo, amount);
            List<String> expectedFormats = currencyInfo.Cultures.Select(
                mc =>
                    String.Format(mc, "{0:C}", amount)).ToList();
            CollectionAssert.Contains(expectedFormats, financial.ToString());
        }

        [TestMethod]
        public void TestToStringFormatsUsingCurrentCultureIfCorrectCurrencyIsNotPossible()
        {
            Decimal amount = Random.RandomDecimal();
            CurrencyInfo currencyInfo = CurrencyInfoProvider.Current.Get("XXX"); // Noone uses this dubiously named currency
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("De-de");
            Financial financial = new Financial(currencyInfo, amount);
            String expectedFormat = String.Format(CultureInfo.CurrentUICulture, "{0:C}", amount);
            Assert.AreEqual(expectedFormat, financial.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof (FormatException))]
        public void TestFormatWithInvalidFormatThrowsFormatException()
        {
            Decimal amount = Random.RandomDecimal();
            CurrencyInfo currencyInfo = CurrencyInfoProvider.Current.Get("GBP");
            Financial financial = new Financial(currencyInfo, amount);
            string s = String.Format("{0:NOTVALID}", financial);
        }

        [TestMethod]
        [ExpectedException(typeof (FormatException))]
        public void TestFormatWithFormatProviderAndInvalidFormatThrowsFormatException()
        {
            Decimal amount = Random.RandomDecimal();
            CurrencyInfo currencyInfo = CurrencyInfoProvider.Current.Get("GBP");
            Financial financial = new Financial(currencyInfo, amount);
            string s = String.Format(CultureInfo.CurrentUICulture, "{0:NOTVALID}", financial);
        }

        [TestMethod]
        public void TestFormatWithFormatIReturnsAmountAndIsoCode()
        {
            Decimal amount = Random.RandomDecimal();
            CurrencyInfo currencyInfo = CurrencyInfoProvider.Current.Get("EUR");
            Financial financial = new Financial(currencyInfo, amount);
            Trace.WriteLine(CultureInfo.CurrentUICulture);
            String expectedFormat = String.Format("{0} {1}", amount, currencyInfo.Code);
            Assert.AreEqual(expectedFormat, String.Format("{0:I}", financial));
        }

        [TestMethod]
        public void TestFormatWithFormatIAndFormatProviderReturnsAmountAndIsoCode()
        {
            Decimal amount = Random.RandomDecimal();
            CurrencyInfo currencyInfo = CurrencyInfoProvider.Current.Get("EUR");
            Financial financial = new Financial(currencyInfo, amount);
            IFormatProvider provider = new CultureInfo("fr-ca").NumberFormat;
            String expectedFormat = String.Format(provider, "{0} {1}", amount, currencyInfo.Code);
            Assert.AreEqual(expectedFormat, String.Format(provider, "{0:I}", financial));
        }

        [TestMethod]
        public void TestFormatWithNoFormatUsesFormatI()
        {
            Decimal amount = Random.RandomDecimal();
            CurrencyInfo currencyInfo = CurrencyInfoProvider.Current.Get("EUR");
            Financial financial = new Financial(currencyInfo, amount);
            String expectedFormat = String.Format("{0:I}", financial);
            Assert.AreEqual(expectedFormat, String.Format("{0}", financial));
        }

        [TestMethod]
        public void TestFormatWithFormatCReturnsSameAsToString()
        {
            Decimal amount = Random.RandomDecimal();
            CurrencyInfo currencyInfo = CurrencyInfoProvider.Current.Get("EUR");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-ca");
            Financial financial = new Financial(currencyInfo, amount);
            String expectedFormat = financial.ToString();
            Assert.AreEqual(expectedFormat, String.Format("{0:C}", financial));
        }

        [TestMethod]
        public void TestFormatWithFormatCAndCurrentCultureAsProviderReturnsSameAsToString()
        {
            Decimal amount = Random.RandomDecimal();
            CurrencyInfo currencyInfo = CurrencyInfoProvider.Current.Get("EUR");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-ca");
            Financial financial = new Financial(currencyInfo, amount);
            IFormatProvider provider = CultureInfo.CurrentUICulture;
            String expectedFormat = financial.ToString();
            Assert.AreEqual(expectedFormat, String.Format(provider, "{0:C}", financial));
        }

        [TestMethod]
        public void TestFormatWithFormatCAndWhenProviderIsNotCultureInfoReturnsSameAsToString()
        {
            Decimal amount = Random.RandomDecimal();
            CurrencyInfo currencyInfo = CurrencyInfoProvider.Current.Get("EUR");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-ca");
            Financial financial = new Financial(currencyInfo, amount);
            IFormatProvider provider = NumberFormatInfo.InvariantInfo;
            String expectedFormat = financial.ToString();
            Assert.AreEqual(expectedFormat, String.Format(provider, "{0:C}", financial));
        }

        [TestMethod]
        public void TestFormatWithFormatCFormatsUsingSpecifiedCultureWhenPossible()
        {
            Decimal amount = Random.RandomDecimal();
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-ca");
            CultureInfo provider = new CultureInfo("en-GB");
            CurrencyInfo currencyInfo = CurrencyInfoProvider.Current.Get(provider);
            Financial financial = new Financial(currencyInfo, amount);
            String expectedFormat = String.Format(provider, "{0:C}", amount);
            Assert.AreEqual(expectedFormat, String.Format(provider, "{0:C}", financial));
        }

        [TestMethod]
        public void TestFormatWithFormatCFormatsUsingCorrectLanguageWhenPossible()
        {
            Decimal amount = Random.RandomDecimal();
            CurrencyInfo currencyInfo = CurrencyInfoProvider.Current.Get("EUR");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-fr");
            // Now if CurrentUICulture is used instead, we get French formatting.
            CultureInfo provider = new CultureInfo("en-GB");
            // No Euro in GB, but Ireland has and also at least speaks English (en).
            Financial financial = new Financial(currencyInfo, amount);
            String correctLanguage = provider.TwoLetterISOLanguageName;
            List<String> expectedFormats =
                currencyInfo.Cultures.Where(c => c.TwoLetterISOLanguageName == correctLanguage).Select(
                    mc =>
                        String.Format(
                            mc,
                            "{0:C}",
                            amount)).
                    ToList();
            String actualFormat = String.Format(provider, "{0:C}", financial);
            CollectionAssert.Contains(expectedFormats, actualFormat);
        }

        [TestMethod]
        public void TestFormatWithFormatCFormatsUsingCurrentCurrencyWhenSpecifiedCurrencyDoesNotMatch()
        {
            Decimal amount = Random.RandomDecimal();
            CurrencyInfo currencyInfo = CurrencyInfoProvider.Current.Get("GBP");
            CultureInfo provider = new CultureInfo("De-de"); // No pound in Germany, nor any German speaking countries.
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-ca");
            Financial financial = new Financial(currencyInfo, amount);
            List<String> expectedFormats = currencyInfo.Cultures.Select(
                mc =>
                    String.Format(mc, "{0:C}", amount)).ToList();
            String actualFormat = String.Format(provider, "{0:C}", financial);
            CollectionAssert.Contains(expectedFormats, actualFormat);
        }

        [TestMethod]
        public void TestFormatWithFormatCFormatsUsingSpecifiedCultureIfCorrectCurrencyIsNotPossible()
        {
            Decimal amount = Random.RandomDecimal();
            CurrencyInfo currencyInfo = CurrencyInfoProvider.Current.Get("XXX"); // Noone uses this dubiously named currency
            CultureInfo provider = new CultureInfo("De-de");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-GB");
            Financial financial = new Financial(currencyInfo, amount);
            String expectedFormat = String.Format(provider, "{0:C}", amount);
            Assert.AreEqual(expectedFormat, String.Format(provider, "{0:C}", financial));
        }

        [TestMethod]
        public void TestFinancialEqualityComparerReturnsTrueForEqualFinancials()
        {
            Financial financialA = new Financial(CurrencyInfoProvider.Current.Get("GBP"), 10);
            Financial financialB = new Financial(CurrencyInfoProvider.Current.Get("GBP"), 10);

            Assert.IsTrue(financialA == financialB);
        }

        [TestMethod]
        public void TestFinancialEqualityComparerReturnsFalseForNonEqualFinancialAmounts()
        {
            Financial financialA = new Financial(CurrencyInfoProvider.Current.Get("GBP"), 10);
            Financial financialB = new Financial(CurrencyInfoProvider.Current.Get("GBP"), 11);

            Assert.IsFalse(financialA == financialB);
        }

        [TestMethod]
        public void TestFinancialEqualityComparerReturnsFalseForNonEqualFinancialCurrencies()
        {
            Financial financialA = new Financial(CurrencyInfoProvider.Current.Get("GBP"), 10);
            Financial financialB = new Financial(CurrencyInfoProvider.Current.Get("EUR"), 10);

            Assert.IsFalse(financialA == financialB);
        }
    }
}