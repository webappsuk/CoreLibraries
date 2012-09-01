using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplications.Utilities.Financials;
using WebApplications.Utilities.Ranges;

namespace WebApplications.Utilities.Test.Ranges
{
    [TestClass]
    public class FinancialRangeTests
    {
        [TestMethod]
        public void TestEqualFinancialRangesReturnTrue()
        {
            Financial financialA = new Financial(CurrencyInfo.Get("GBP"), 10);
            Financial financialB = new Financial(CurrencyInfo.Get("GBP"), 10);

            Range<Financial> rangeA = new Range<Financial>(financialA, financialB);
            Range<Financial> rangeB = new Range<Financial>(financialA, financialB);

            Assert.IsTrue(rangeA.Equals(rangeB));
        }

        [TestMethod]
        public void TestEqualFinancialRangesWithStepReturnTrue()
        {
            Financial financialA = new Financial(CurrencyInfo.Get("GBP"), 10);
            Financial financialB = new Financial(CurrencyInfo.Get("GBP"), 10);
            Financial financialC = new Financial(CurrencyInfo.Get("GBP"), 10);

            Range<Financial> rangeA = new Range<Financial>(financialA, financialB, financialC);
            Range<Financial> rangeB = new Range<Financial>(financialA, financialB, financialC);

            Assert.IsTrue(rangeA.Equals(rangeB));
        }

        [TestMethod]
        public void TestNonEqualFinancialRangesReferencesWithEqualValuesReturnsTrue()
        {
            Financial financialA = new Financial(CurrencyInfo.Get("GBP"), 10);
            Financial financialB = new Financial(CurrencyInfo.Get("GBP"), 10);
            Financial financialC = new Financial(CurrencyInfo.Get("GBP"), 10);

            Range<Financial> rangeA = new Range<Financial>(financialA, financialB);
            Range<Financial> rangeB = new Range<Financial>(financialA, financialC);

            Assert.IsTrue(rangeA.Equals(rangeB));
        }
    }
}
