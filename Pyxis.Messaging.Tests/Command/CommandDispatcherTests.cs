using System;
using Moq;
using NUnit.Framework;
using Pyxis.Messaging.Command;

namespace Pyxis.Messaging.Tests.Command
{
    [TestFixture]
    public class CommandDispatcherTests
    {
        private Mock<ICommand> _message;
        private CommandDispatcher _dispatcher;
        private Mock<IMessageHandler<ICommand>> _handler;

        [SetUp]
        public void SetUp()
        {
            _message = new Mock<ICommand>();
            _message.Setup(x => x.Type).Returns(new CommandType("test"));
            _handler = new Mock<IMessageHandler<ICommand>>();
            _dispatcher = new CommandDispatcher(new[] { _handler.Object }); 
        }

        [Test]
        public void TestThatMessageIsNotDispatchedToHandlerThatCannotHandleIt()
        {
            _handler.Setup(x => x.CanHandle(It.IsAny<ICommand>())).Returns(false);
            Assert.IsFalse(_dispatcher.Dispatch(_message.Object));
            _handler.Verify();
        }

        [Test]
        public void TestThatMessageIsDispatchedToHandlerThatCanHandleIt()
        {
            _handler.Setup(x => x.CanHandle(It.IsAny<ICommand>())).Returns(true);
            _handler.Setup(x=> x.Handle(It.IsAny<ICommand>())).Verifiable();
            _dispatcher.Dispatch(_message.Object);
            _handler.Verify();
        }

        [Test]
        public void TestThatAHandlerCrashReturnsFalse()
        {
            _handler.Setup(x => x.CanHandle(It.IsAny<ICommand>())).Returns(true);
            _handler.Setup(x => x.Handle(It.IsAny<ICommand>())).Throws(new NullReferenceException());
            var result = _dispatcher.Dispatch(_message.Object);
            Assert.IsFalse(result);
        }

        [Test]
        public void TestThatMessageCannotBeDispatchedToMoreThanOneHandler()
        {
            var handler2 = new Mock<IMessageHandler<ICommand>>();
            _dispatcher = new CommandDispatcher(new[] { _handler.Object, handler2.Object });
            _handler.Setup(x => x.CanHandle(It.IsAny<ICommand>())).Returns(true);
            handler2.Setup(x => x.CanHandle(It.IsAny<ICommand>())).Returns(true);
            _handler.Setup(x => x.Handle(It.IsAny<ICommand>())).Verifiable();
            _dispatcher.Dispatch(_message.Object);
            _handler.Verify();
            handler2.Verify();
        }

        [Test]
        public void TestDispatcherReturnsFalseIfTheMessageCannotBeHandled()
        {
            _handler.Setup(x => x.CanHandle(It.IsAny<ICommand>())).Returns(false);
            var result = _dispatcher.Dispatch(_message.Object);
            Assert.IsFalse(result);
        }

        [Test]
        public void TestDispatcherReturnsHandlerResultIfTheMessageCanBeHandled()
        {
            _handler.Setup(x => x.CanHandle(It.IsAny<ICommand>())).Returns(true);
            _handler.Setup(x => x.Handle(It.IsAny<ICommand>())).Returns(true);
            var result = _dispatcher.Dispatch(_message.Object);
            Assert.IsTrue(result);

            _handler.Setup(x => x.Handle(It.IsAny<ICommand>())).Returns(false);
            result = _dispatcher.Dispatch(_message.Object);
            Assert.IsFalse(result);
        }
    }
}
