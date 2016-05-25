using System;
using FluentAssertions;
using NUnit.Framework;
using Pyxis.Core.Id;

namespace Pyxis.Persistance.Tests
{
    [TestFixture]
    public class MemoryPersistanceTests : BasePersistanceTests
    {
        [SetUp]
        public void SetUp()
        {
            PersistanceStore = new MemoryPersistance();
        }

        [Test]
        public void UpdateToLiveObjectShouldNotAffectPersisted()
        {
            var idGenerator = new GuidIdGenerator();
            var testObject1 = new TestObject(idGenerator.GenerateId(), "Name1");

            PersistanceStore.Save(testObject1);

            testObject1.Name = "New Name";

            var testObject2 = PersistanceStore.Get<TestObject>(testObject1.Id);

            testObject2.Should().NotBe(testObject1);
        }

        public class TestObject : IIdentifiable
        {
            private string _id;
            private string _name;

            public TestObject(string id, string name)
            {
                _id = id;
                _name = name;
            }

            public string Id
            {
                get { return _id; }
                set { _id = value; }
            }

            public string Name
            {
                get { return _name; }
                set { _name = value; }
            }
        }
    }
}
