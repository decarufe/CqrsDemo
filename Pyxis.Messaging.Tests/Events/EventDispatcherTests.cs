using System.Threading;
using Moq;
using NUnit.Framework;
using Pyxis.Messaging.Command;
using Pyxis.Messaging.Events;

namespace Pyxis.Messaging.Tests.Events
{
    [TestFixture]
    public class EventDispatcherTests
    {
        private Mock<IEvent> _event;
        private EventDispatcher _dispatcher;
        private Mock<IMessageHandler<IEvent>> _handler;

        [SetUp]
        public void SetUp()
        {
            _event = new Mock<IEvent>();
            _event.Setup(x => x.Type).Returns(new EventType("test"));
            _handler = new Mock<IMessageHandler<IEvent>>(MockBehavior.Strict);
            _dispatcher = new EventDispatcher(new[] { _handler.Object }); 
        }

        [Test]
        public void TestThatEventIsNotDispatchedHandlerThatCannotHandleIt()
        {
            _handler.Setup(x => x.CanHandle(It.IsAny<IEvent>())).Returns(false);
            _dispatcher.Dispatch(_event.Object);
            _handler.Verify();
        }

        [Test]
        public void TestThatMessageIsDispatchedToHandlerThatCanHandleIt()
        {
            var secondHandler = new Mock<IMessageHandler<IEvent>>();
            _dispatcher = new EventDispatcher(new[] { _handler.Object, secondHandler.Object }); 
            var completedEvent = new ManualResetEvent(false);
            _handler.Setup(x => x.CanHandle(It.IsAny<IEvent>())).Returns(true).Verifiable();
            secondHandler.Setup(x => x.CanHandle(It.IsAny<IEvent>())).Returns(false).Verifiable();
            _handler.Setup(x => x.Handle(It.IsAny<IEvent>())).Callback(() =>  completedEvent.Set());
            _dispatcher.Dispatch(_event.Object);
            completedEvent.WaitOne(1000);
            _handler.Verify();
            secondHandler.Verify();
        }

        [Test]
        public void TestThatMessageCanBeDispatchedToMoreThanOneHandler()
        {
            var completedEvent = new ManualResetEvent(false);
            var completedEvent2 = new ManualResetEvent(false);
            var handler2 = new Mock<IMessageHandler<IEvent>>();
            _dispatcher = new EventDispatcher(new[] { _handler.Object, handler2.Object });
            _handler.Setup(x => x.CanHandle(It.IsAny<IEvent>())).Returns(true);
            handler2.Setup(x => x.CanHandle(It.IsAny<IEvent>())).Returns(true);
            _handler.Setup(x => x.Handle(It.IsAny<IEvent>())).Callback(() => completedEvent.Set());
            handler2.Setup(x => x.Handle(It.IsAny<IEvent>())).Callback(() => completedEvent2.Set());
            _dispatcher.Dispatch(_event.Object);
            completedEvent.WaitOne(1000);
            completedEvent2.WaitOne(1000);
            _handler.Verify();
            handler2.Verify();
        }
    }
}
