using NUnit.Framework;
using Pyxis.Core.Config;

namespace Pyxis.Core.Tests.Config
{
    [TestFixture]
    public class ConfigManagerTests
    {
        private ConfigManager _manager;
        private IConfigProvider _configProvider;

        [SetUp]
        public void SetUp()
        {
            _configProvider = new TestConfigProvider();
            _manager = new ConfigManager(new[] {_configProvider});
        }

        [Test]
        public void TestUnexistingKeyReturnsEmptyString()
        {
            var value = _manager.Get("nonExisting");
            Assert.AreEqual(string.Empty, value);
        }

        [Test]
        public void TestExistingKeyReturnsValue()
        {
            var value = _manager.Get("existingKey");
            Assert.AreEqual("goodValue", value);
        }
        [Test]
        public void TestValueCanBeTyped()
        {
            var value = _manager.Get<int>("existingInt");
            Assert.AreEqual(4, value);
        }
        [Test]
        public void TestCanHaveDefault()
        {
            var value = _manager.Get("nonExisting", "default");
            Assert.AreEqual("default", value);
        }
        [Test]
        public void TestTypingIsCastSafe()
        {
            var value = _manager.Get("existingKey", 4);
            Assert.AreEqual(4, value);
            value = _manager.Get<int>("nonExisting");
            Assert.AreEqual(0, value);
        }
        [Test]
        public void TestNoProviderReturnsEmptyString()
        {
            _manager = new ConfigManager(new IConfigProvider[0]);
            var value = _manager.Get("nonExisting");
            Assert.AreEqual(string.Empty, value);
        }

        [Test]
        public void TestValueReturnedComesFromFirstProviderContainingKey()
        {
            _manager = new ConfigManager(new IConfigProvider[] {new EmptyConfigProvider(), new TestConfigProvider()});

            var value = _manager.Get("existingKey");
            Assert.AreEqual("goodValue", value);
        }
    }

    internal class TestConfigProvider : IConfigProvider
    {
        public string Get(string key)
        {
            if ("existingKey" == key)
            {
                return "goodValue";
            }
            if ("existingInt" == key)
            {
                return "4";
            }
            return null;
        }
    }
    internal class EmptyConfigProvider : IConfigProvider
    {
        public string Get(string key)
        {
            return null;
        }
    }

}