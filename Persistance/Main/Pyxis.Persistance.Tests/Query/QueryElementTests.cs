using NUnit.Framework;
using Pyxis.Persistance.Query;

namespace Pyxis.Persistance.Tests.Query
{
    [TestFixture]
    public class QueryElementTests
    {
        private QueryElement _element;
        
        [Test]
        public void TestElementIsBuiltCorrectly()
        {
            _element = new QueryElement("field", FieldComparison.Equals, "value");
            Assert.AreEqual("field", _element.Field);
            Assert.AreEqual(FieldComparison.Equals, _element.Comparison);
            Assert.AreEqual("value", _element.Value);
        }

        [Test]
        public void TestFieldEqualsBuildAProperElement()
        {
            _element = QueryElement.FieldEquals("field", "value");
            Assert.AreEqual("field", _element.Field);
            Assert.AreEqual(FieldComparison.Equals, _element.Comparison);
            Assert.AreEqual("value", _element.Value);
        }
    }
}
