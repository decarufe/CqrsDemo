using System.Configuration;
using NUnit.Framework;
using Pyxis.Core.Config;

namespace Pyxis.Core.Tests.Config
{
    [TestFixture]
    public class ConfigProviderTests
    {
        private AppConfigProvider _manager;
        
        [Test]
        public void TestUnexistingKeyReturnsNull()
        {
            _manager = new AppConfigProvider();

            var value = _manager.Get("nonExisting");
            Assert.IsNull(value);
        }
        
        [Test]
        public void TestNoProviderReturnsEmptyString()
        {
            try
            {
                _manager = new AppConfigProvider();
                ConfigurationManager.AppSettings["existingKey"] = "myValue";
                var value = _manager.Get("existingKey");
                Assert.AreEqual("myValue", value);
            }
            finally
            {
                ConfigurationManager.AppSettings["existingKey"] = null;
            }

        }
    }
}
