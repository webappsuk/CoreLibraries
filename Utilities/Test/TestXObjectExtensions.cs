using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml;
using System.Xml.Linq;

namespace WebApplications.Utilities.Test
{
    [TestClass]
    public class TestXObjectExtensions : UtilitiesTestBase
    {
        private static readonly XDocument _doc1 = XDocument.Parse(@"
<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
<xml>
	<?PI value=""Test""?>
	<?PI value=""Test""?>
	<A a=""1"" b=""2"" />
</xml>
");
        private static readonly XDocument _doc1Clone = XDocument.Parse(@"
<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
<xml>
	<?PI value=""Test""?>
	<?PI value=""Test""?>
	<A a=""1"" b=""2"" />
</xml>
");

        private static readonly XDocument _doc2 = XDocument.Parse(@"
<xml>
<?PI value=""Test""?>
    <!-- Commment -->
	<A b=""2"" a=""1"" />
	<?PI value=""Test""?>
</xml>
");

        [TestMethod]
        public void TestDefaultSame()
        {
            Tuple<XObject, XObject> result = _doc1.DeepEquals(_doc2);
            Assert.IsNull(result, $"{result}");
        }
        [TestMethod]
        public void TestLegacyNotSame()
        {
            Tuple<XObject, XObject> result = _doc1.DeepEquals(_doc2, XObjectComparisonOptions.Legacy);

            // We expect the attribute order to be noticed
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Item1);
            Assert.IsNotNull(result.Item2);
            Assert.AreEqual(XmlNodeType.Attribute, result.Item1.NodeType);
            Assert.AreEqual(XmlNodeType.Attribute, result.Item2.NodeType);
            Assert.AreEqual("a", ((XAttribute)result.Item1).Name);
            Assert.AreEqual("b", ((XAttribute)result.Item2).Name);
        }

        [TestMethod]
        public void TestSemanticSame()
        {
            Tuple<XObject, XObject> result = _doc1.DeepEquals(_doc2, XObjectComparisonOptions.Semantic);
            Assert.IsNull(result, $"{result}");
        }

        [TestMethod]
        public void TestExactNotSame()
        {
            Tuple<XObject, XObject> result = _doc1.DeepEquals(_doc2, XObjectComparisonOptions.Exact);

            // We expect the missing doc type to be detected
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Item1);
            Assert.IsNotNull(result.Item2);
            Assert.AreEqual(XmlNodeType.DocumentType, result.Item1.NodeType);
            Assert.AreEqual(XmlNodeType.Element, result.Item2.NodeType);
        }

        [TestMethod]
        public void TestExactSame()
        {
            Tuple<XObject, XObject> result = _doc1.DeepEquals(_doc1Clone, XObjectComparisonOptions.Exact);
            Assert.IsNull(result, $"{result}");
        }
    }
}
