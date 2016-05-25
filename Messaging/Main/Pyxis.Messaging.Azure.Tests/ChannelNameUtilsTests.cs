using Microsoft.WindowsAzure.ServiceRuntime;
using NUnit.Framework;

namespace Pyxis.Messaging.Azure.Tests
{
    [TestFixture]
    public class ChannelNameUtilsTests
    {
        [Test]
        public void TestNameIsPrefixedWithUnnamedWhenNoInstallNameIsProvided()
        {
            var name = ChannelNameUtils.GenerateName(null,"myTopic");
            Assert.AreEqual("unnamed_myTopic", name);
        }

        [Test]
        public void TestNameIsPrefixedWithNameIsProvided()
        {
            var name = ChannelNameUtils.GenerateName("test", "myTopic");
            Assert.AreEqual("test_myTopic", name);
        }
    
        [Test]
        public void TestRoleInstanceIsOnlyAddedWhenAvailable()
        {
            var name = ChannelNameUtils.GenerateName("test", "myTopic", true);
            Assert.AreEqual("test_myTopic", name);
        }
    }
}
