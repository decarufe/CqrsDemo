using System;
using Moq;
using NUnit.Framework;
using Pyxis.Cqrs.Bus;
using Pyxis.Cqrs.Commands;
using Pyxis.Cqrs.Result;
using Pyxis.Cqrs.Tests.Commands;
using Pyxis.Cqrs.Tests.Events;
using Pyxis.Messaging;
using Pyxis.Messaging.Command;

namespace Pyxis.Cqrs.Tests.Bus
{
    [TestFixture]
    public class MessagingBusTests
    {
        private MessagingBus _bus;
        private Mock<ICommandQueue> _commandQueueBus;
        private Mock<IResultPublisher> _resultPublisher;
        private Mock<IResultAwaiter> _resultAwaiter;
        [SetUp]
        public void SetUp()
        {
            _commandQueueBus = new Mock<ICommandQueue>();
            _resultAwaiter = new Mock<IResultAwaiter>();
            _resultPublisher = new Mock<IResultPublisher>();
            _bus = new MessagingBus(_commandQueueBus.Object, _resultPublisher.Object, _resultAwaiter.Object);
        }
        [Test]
        public void TestCommandsAreDispatchedOnCommandQueue()
        {
            DomainResult runResult = null;
            _resultPublisher.Setup(x => x.Publish(It.IsAny<DomainResult>())).Callback<DomainResult>(r => runResult = r);
            _commandQueueBus.Setup(x => x.QueueCommand(It.IsAny<ICommand>())).Verifiable();
            var command = new TestDomainCommand();
            _bus.Send(command);
            Assert.IsNotNull(runResult);
            Assert.IsFalse(string.IsNullOrEmpty(runResult.TrackingId));
            _commandQueueBus.Verify();
        }
        [Test]
        public void TestCommandsTypeAreFromSourceObjectType()
        {
            CommandType commandType = null;
            _commandQueueBus.Setup(x => x.QueueCommand(It.IsAny<ICommand[]>()))
                .Callback<ICommand[]>(x => commandType = x[0].Type);
            _bus.Send(new TestDomainCommand());
            Assert.IsNotNull(commandType);
            Assert.AreEqual("TestDomainCommand", commandType.Value);
        }

        [Test]
        public void TestCommandsAreDispatchedOnEventBus()
        {
            _commandQueueBus.Setup(x => x.QueueCommand(It.IsAny<ICommand>())).Verifiable();
            _bus.Publish(new []{ new TestDomainEvent(Guid.NewGuid())});
            _commandQueueBus.Verify();
        }

        [Test]
        public void TestCommandTypeIsFromSourceObjectType()
        {
            CommandType commandType = null;
            _commandQueueBus.Setup(x => x.QueueCommand(It.IsAny<ICommand>()))
                .Callback<ICommand[]>(x => commandType = x[0].Type);
            _bus.Publish(new[] { new TestDomainEvent(Guid.NewGuid()) });
            Assert.IsNotNull(commandType);
            Assert.AreEqual("TestDomainEvent", commandType.Value);
        }

        [Test]
        public void TestCommandResultIsAwaitedFor()
        {
            var id = Guid.NewGuid();
            ITrackable sent = null;
            var command = new TestDomainCommand();
            command.Id = id;
            _commandQueueBus.Setup(x => x.QueueCommand(It.IsAny<ICommand[]>())).Callback<ICommand[]>(bm=> sent = bm[0] as ITrackable);
            var result = _bus.Send(new TestDomainCommand {Id = id });
            _resultAwaiter.Verify(x => x.WaitForCommand(It.IsAny<ITrackable>()),Times.Exactly(1) );
            Assert.IsNotNull(sent);
            Assert.IsFalse(string.IsNullOrEmpty(sent.TrackingId));
        }

        [Test]
        public void TestEventResultIsAwaitedFor()
        {
            var id = Guid.NewGuid();
            ITrackable sent = null;
            var command = new TestDomainCommand();
            command.Id = id;
            _commandQueueBus.Setup(x => x.QueueCommand(It.IsAny<ICommand[]>())).Callback<ICommand[]>(bm => sent = bm[0] as ITrackable);
            _bus.SendAndWait(new TestDomainCommand { Id = id });
            Assert.IsNotNull(sent);
            Assert.IsFalse(string.IsNullOrEmpty(sent.TrackingId));
            _resultAwaiter.Verify(x => x.WaitForResults(It.IsAny<ITrackable>()), Times.Exactly(1));
        }
    }
}
