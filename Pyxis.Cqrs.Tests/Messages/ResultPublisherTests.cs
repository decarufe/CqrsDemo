using System;
using System.Collections.Generic;
using System.Threading;
using Moq;
using NUnit.Framework;
using Pyxis.Cqrs.Messages;
using Pyxis.Cqrs.Result;

namespace Pyxis.Cqrs.Tests.Messages
{
    [TestFixture]
    public class ResultPublisherTests
    {
        private ResultPublisher _publisher;
        private Mock<IDomainResultStore> _mockResultStore;
        private Mock<IDomainMessage> _mockTrackedMessage;
        private Guid _id;
        private Guid _trackingId;

        [SetUp]
        public void SetUp()
        {
            _mockResultStore = new Mock<IDomainResultStore>();
            _mockTrackedMessage= new Mock<IDomainMessage>();
            _id = Guid.NewGuid();
            _trackingId = Guid.NewGuid();
            _mockTrackedMessage.Setup(x => x.Id).Returns(_id);
            _mockTrackedMessage.Setup(x => x.TrackingId).Returns(_trackingId.ToString());
        }

        [Test]
        public void ResultCanBePublished()
        {
            var result = new DomainResult(_mockTrackedMessage.Object, ResultCode.OK, string.Empty);
            _mockResultStore.Setup(x=> x.Save(It.IsAny<DomainResult>())).Verifiable();
            _publisher = new ResultPublisher(_mockResultStore.Object, new TimeoutProvider(200));
            _publisher.Publish(result);
            
            Assert.AreEqual(ResultCode.OK, result.ResultCode);
            Assert.AreEqual(_id.ToString(), result.Id);
            Assert.AreEqual(string.Empty, result.ResultMessage);
            _mockResultStore.Verify();
            _mockResultStore.Verify(x => x.Delete(_id.ToString()), Times.Never);
        }

        [Test]
        public void ResultAreCleanupUpAfterTwiceAGivenTime()
        {
            var result = new DomainResult(_mockTrackedMessage.Object, ResultCode.OK, string.Empty);
            _mockResultStore.Setup(x => x.Save(It.IsAny<DomainResult>())).Verifiable();
            _mockResultStore.Setup(x => x.Delete(_id.ToString()));
            _publisher = new ResultPublisher(_mockResultStore.Object, new TimeoutProvider(200));
            _publisher.Publish(result);
            Thread.Sleep(250);
            _mockResultStore.Verify(x => x.Delete(_trackingId.ToString()), Times.Exactly(0));
            Thread.Sleep(200);
            _mockResultStore.Verify(x => x.Delete(_trackingId.ToString()), Times.Exactly(1));
        }

        [Test]
        public void ResultCanBeUpdated()
        {
            var intercepted = new List<DomainResult>();
            var result = new DomainResult(_mockTrackedMessage.Object, ResultCode.Unknown, string.Empty);
            _mockResultStore.Setup(x => x.Save(It.IsAny<DomainResult>())).Callback<DomainResult>(x=> intercepted.Add(x));
            _publisher = new ResultPublisher(_mockResultStore.Object, new TimeoutProvider(200));
            _publisher.Publish(result);
            result = new DomainResult(_mockTrackedMessage.Object, ResultCode.OK, "finished");
            _publisher.Publish(result);
            Assert.AreEqual(2, intercepted.Count);
            Assert.AreEqual(intercepted[0].Id, intercepted[1].Id);
            Assert.AreEqual(intercepted[0].TrackingId, intercepted[1].TrackingId);
            Assert.AreEqual(intercepted[0].ResultCode, ResultCode.Unknown);
            Assert.AreEqual(intercepted[1].ResultCode, ResultCode.OK);
            Assert.AreEqual(string.Empty, intercepted[0].ResultMessage);
            Assert.AreEqual("finished",intercepted[1].ResultMessage);
        }
    }
}
