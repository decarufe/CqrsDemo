using System;
using Moq;
using NUnit.Framework;
using Pyxis.Cqrs.Events;
using Pyxis.Cqrs.Messages;
using Pyxis.Cqrs.Result;
using Pyxis.Cqrs.Tests.Commands;

namespace Pyxis.Cqrs.Tests.Messages
{
    [TestFixture]
    public class ResultAwaiterTests
    {
        private ResultAwaiter _awaiter;
        private Mock<IDomainResultStore> _mockResultStore;
        private Guid _id;
        private Guid _trackingId;
        private IDomainMessage _domainCommand;
        private IDomainMessage _domainEvent;

        [SetUp]
        public void SetUp()
        {
            _mockResultStore = new Mock<IDomainResultStore>();
            _id = Guid.NewGuid();
            _trackingId = Guid.NewGuid();
            _domainCommand = new TestDomainCommand() { Id = _id,TrackingId = _trackingId .ToString()};
            _domainEvent = new DomainEvent(Guid.NewGuid()) { TrackingId = _trackingId.ToString() };
        }

        [Test]
        [Ignore("Fail when run in batch")]
        public void WaitUpToTimeout()
        {
            _awaiter = new ResultAwaiter(_mockResultStore.Object,new TimeoutProvider(100) );
            var startWait = DateTime.Now;
            var result = _awaiter.WaitForResults(_domainCommand);
            var endWait = DateTime.Now;
            var delta = (endWait - startWait);
            Assert.AreEqual(ResultCode.Unknown, result.ResultCode);
            // Allow 15 ms buffer
            Assert.IsTrue(delta.TotalMilliseconds <= 115, "delta was " + delta);
        }

        [Test]
        public void ReturnsThePersistedResult()
        {
            _awaiter = new ResultAwaiter(_mockResultStore.Object, new TimeoutProvider(100));
            _mockResultStore.Setup(x => x.Get(_domainCommand.TrackingId)).Returns(new [] { new DomainResult(_domainCommand, ResultCode.OK, string.Empty) });
            var result = _awaiter.WaitForResults(_domainCommand);
            Assert.AreEqual(ResultCode.OK, result.ResultCode);
        }

        [Test]
        public void WaitForAllResultsToBeCompleted()
        {
            _awaiter = new ResultAwaiter(_mockResultStore.Object, new TimeoutProvider(1000));
            var invovationCount = 0;
            var results = new[]
            {
                new DomainResult(_domainCommand, ResultCode.OK, string.Empty),
                new DomainResult(_domainEvent, ResultCode.Unknown, string.Empty)
            };
            _mockResultStore.Setup(x => x.Get(_domainCommand.TrackingId)).Returns(results)
                .Callback(
                () => { 
                    if (invovationCount == 1)
                    {
                        results[0].ResultCode = ResultCode.Failed;
                    }
                    else if (invovationCount == 2)
                    {
                        results[1].ResultCode = ResultCode.OK;
                    }

                    invovationCount++;
                });
                

            _awaiter.WaitForResults(_domainCommand);
            Assert.AreEqual(3, invovationCount);
        }
        [Test]
        public void WorstResultIsReturned()
        {
            _awaiter = new ResultAwaiter(_mockResultStore.Object, new TimeoutProvider(500));

            var results = new[]
            {
                new DomainResult(_domainCommand, ResultCode.OK, string.Empty),
                new DomainResult(_domainEvent, ResultCode.Failed, string.Empty)
            };
            _mockResultStore.Setup(x => x.Get(_domainCommand.TrackingId)).Returns(results);

            var result = _awaiter.WaitForResults(_domainCommand);
            Assert.AreEqual(ResultCode.Failed, result.ResultCode);
        }

        [Test]
        public void CanOnlyAwaitForCommands()
        {
            _awaiter = new ResultAwaiter(_mockResultStore.Object, new TimeoutProvider(500));

            var results = new[]
            {
                new DomainResult(_domainCommand, ResultCode.OK, string.Empty),
                new DomainResult(_domainEvent, ResultCode.Unknown, string.Empty)
            };
            _mockResultStore.Setup(x => x.Get(_domainCommand.TrackingId)).Returns(results);

            var result = _awaiter.WaitForCommand(_domainCommand);
            Assert.AreEqual(ResultCode.OK, result.ResultCode);
        }

        [Test]
        public void RemovedThePersistedResultOnceRead()
        {
            _awaiter = new ResultAwaiter(_mockResultStore.Object, new TimeoutProvider(100));
            _mockResultStore.Setup(x => x.Get(_domainCommand.TrackingId)).Returns(new[] { new DomainResult(_domainCommand, ResultCode.OK, string.Empty)});
            _mockResultStore.Setup(x => x.Delete(_domainCommand.TrackingId)).Verifiable();
            
            var result = _awaiter.WaitForResults(_domainCommand);
            Assert.AreEqual(ResultCode.OK, result.ResultCode);
            _mockResultStore.Verify();
        }
    }
}
