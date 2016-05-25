using System;
using System.Linq;
using NUnit.Framework;
using Pyxis.Cqrs.Result;
using Pyxis.Cqrs.Tests.Commands;
using Pyxis.Persistance;

namespace Pyxis.Cqrs.Tests.Messages
{
    [TestFixture]
    public class CqrsResultPersistanceStoreTests
    {
        private IPersistanceStore _persistanceStore;
        private DomainResultPersistanceStore _resultPersistance;
        
        private Guid _id;
        private TestDomainCommand _testCommand;

        [SetUp]
        public void SetUp()
        {
            _persistanceStore = new MemoryPersistance();
            _resultPersistance = new DomainResultPersistanceStore(_persistanceStore, new InProcDomainResultStoreQuery(_persistanceStore));
            _id = Guid.NewGuid();
            _testCommand = new TestDomainCommand { Id = _id, TrackingId = Guid.NewGuid().ToString()};
        }

        [Test]
        public void GettingUnknownReturnsEmpty()
        {
            var result = _resultPersistance.Get("123");
            Assert.AreEqual(0, result.Count());
        }
        [Test]
        public void GettingKnownReturnsResult()
        {
            var source = new DomainResult(_testCommand, ResultCode.OK, string.Empty);
            _persistanceStore.Save(source);
            var results = _resultPersistance.Get(_testCommand.TrackingId);
            Assert.AreEqual(1, results.Count());
            Assert.AreEqual(source.Id, results.First().Id);
        }
        [Test]
        public void CanSaveResult()
        {
            var source = new DomainResult(_testCommand, ResultCode.OK, string.Empty);
            _resultPersistance.Save(source);
            var results = _resultPersistance.Get(_testCommand.TrackingId);
            Assert.AreEqual(1, results.Count());
            Assert.AreEqual(source.Id, results.First().Id);
        }
        [Test]
        public void CanDeleteResult()
        {
            var source = new DomainResult(_testCommand, ResultCode.OK, string.Empty);
            _resultPersistance.Save(source);
            _resultPersistance.Delete(_testCommand.TrackingId);
            var result = _resultPersistance.Get(_testCommand.TrackingId);
            Assert.AreEqual(0, result.Count());
        }

        [Test]
        public void DeleteUnknownIsOK()
        {
            _resultPersistance.Delete("1");
            var results = _resultPersistance.Get("1");
            Assert.AreEqual(0, results.Count());
        }
    }
}
