using NUnit.Framework;

namespace Pyxis.Messaging.Tests
{
    [TestFixture]
    public class MessageTests
    {
        [Test]
        public void TestGettingUnknownAttributeReturnsNull()
        {
            var message = new TestMessage();
            message.Attributes["here"] = true;
            Assert.AreEqual(true, message.TryGetAttribute<bool>("here"));
            Assert.IsNull(message.TryGetAttribute("nothere"));
        }
    }

    class TestMessage : Message
    {
        
    }
}
