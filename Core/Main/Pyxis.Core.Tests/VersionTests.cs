using System;
using NUnit.Framework;

namespace Pyxis.Core.Tests
{
    [TestFixture]
    public class VersionTests
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void TestDefaultConstructorDefaultsToToday()
        {
            var version = new Version();
            Assert.IsTrue(Convert.ToInt64(DateTime.Now.ToString("yyyyMMddhhmmss")) >=  version.Value);
            Assert.IsTrue(Convert.ToInt64(DateTime.Now.AddSeconds(-1).ToString("yyyyMMddhhmmss")) < version.Value);
        }

        [Test]
        public void TestConstructorWithDateCanBeSupplied()
        {
            var baseDate = DateTime.Today.AddDays(-43);
            var version = new Version(baseDate);
            Assert.AreEqual(Convert.ToInt64(baseDate.ToString("yyyyMMddhhmmss")), version.Value);
        }

        [Test]
        public void TestThatVersionCanBeCompared()
        {
            var version1 = new Version(1);
            var version2 = new Version(2);
            var version2a = new Version("2");
            Assert.IsTrue(version1 < version2);
            Assert.IsFalse(version1 > version2);
            Assert.AreEqual(version1, version1);
            Assert.AreEqual(version2, version2);
            Assert.AreEqual(version2, version2a);
        }
    }
}

