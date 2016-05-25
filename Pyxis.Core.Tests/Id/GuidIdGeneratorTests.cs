using System;
using NUnit.Framework;
using Pyxis.Core.Id;

namespace Pyxis.Core.Tests.Id
{
    [TestFixture]
    public class GuidIdGeneratorTests
    {
        [Test]
        public void TestThatGenerationIsUnique()
        {
            var generator = new GuidIdGenerator();
            var guid = generator.GenerateId();
            var parsed = Guid.Parse(guid);
            Assert.IsNotNull(parsed);
        }

        [Test]
        public void TestThatSeedHasNoEffet()
        {
            var generator = new GuidIdGenerator();
            var guid = generator.GenerateId("seed");
            generator = new GuidIdGenerator();
            var guid2 = generator.GenerateId("seed");
            Assert.AreNotEqual(guid, guid2);
            Assert.AreNotSame(guid, guid2);
        }
    }
}
