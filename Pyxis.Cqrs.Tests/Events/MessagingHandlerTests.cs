using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Pyxis.Cqrs.Commands;
using Pyxis.Cqrs.Events;
using Pyxis.Cqrs.Messages;
using Pyxis.Cqrs.Result;
using Pyxis.Cqrs.Tests.Commands;
using Pyxis.Messaging.Command;

namespace Pyxis.Cqrs.Tests.Events
{
    [TestFixture]
    public class MessagingHandlerTests
    {
        private CqrsMessagingHandler _eventHandler;
        private TestHandleEvent _eventInstance;
        private TestHandleCommand _handleCommandInstance;
        private Mock<IResultPublisher> _resultPublisherMock;

        [SetUp]
        public void SetUp()
        {
            _eventInstance = new TestHandleEvent();
            _handleCommandInstance = new TestHandleCommand();
            _resultPublisherMock = new Mock<IResultPublisher>();
            _eventHandler = new CqrsMessagingHandler(_resultPublisherMock.Object, new [] { _handleCommandInstance },
                new[] { _eventInstance });
        }

        [Test]
        public void EventHandlerIsCalled()
        {
            var testEvent = DomainMessageTranslator.TranslateCommand(new TestDomainEvent(Guid.NewGuid()), new CommandType("TestDomainEvent"));
            Assert.IsTrue(_eventHandler.Dispatch(testEvent));
            Assert.AreEqual(1,_eventInstance.Handled.Count);
            Assert.AreEqual("TestDomainEvent", _eventInstance.Handled.First());
        }
        [Test]
        public void CommandHandlerIsCalled()
        {
            var testCommand = DomainMessageTranslator.TranslateCommand(new TestDomainCommand(), new CommandType("TestDomainCommand"));
            Assert.IsTrue(_eventHandler.Dispatch(testCommand));
            Assert.AreEqual(1, _handleCommandInstance.Handled.Count);
            Assert.AreEqual("TestDomainCommand", _handleCommandInstance.Handled.First());
        }
        [Test]
        public void SupportsMultipleHandlers()
        {
            var testEvent = DomainMessageTranslator.TranslateCommand(new TestDomainEvent2(Guid.NewGuid()),
                new CommandType("TestDomainEvent2"));
            Assert.IsTrue(_eventHandler.Dispatch(testEvent));
            Assert.AreEqual(1,_eventInstance.Handled.Count);
            Assert.AreEqual("TestDomainEvent2", _eventInstance.Handled.First());
        }

        [Test]
        public void ProperlyHandleUnknownCommands()
        {
            DomainResult result = null;
            _resultPublisherMock.Setup(x => x.Publish(It.IsAny<DomainResult>())).Callback<DomainResult>(r => result = r);
            var testCommand = DomainMessageTranslator.TranslateCommand(new TestDomainEvent3(Guid.NewGuid()),
                new CommandType("TestDomainEvent3"));
            Assert.IsTrue(_eventHandler.Dispatch(testCommand));
            Assert.IsFalse(_handleCommandInstance.Handled.Any());
            Assert.AreEqual(ResultCode.OK, result.ResultCode);
        }

        [Test]
        public void ProperlyHandleCommandThrowingException()
        {
            DomainResult result = null;
            _resultPublisherMock.Setup(x=>x.Publish(It.IsAny<DomainResult>())).Callback<DomainResult>(r=> result = r);
            var testCommand = DomainMessageTranslator.TranslateCommand(new TestDomainEvent4(Guid.NewGuid()),
                new CommandType("TestDomainEvent4"));
            Assert.IsTrue(_eventHandler.Dispatch(testCommand));
            Assert.IsFalse(_handleCommandInstance.Handled.Any());
            Assert.AreEqual(ResultCode.Failed, result.ResultCode);
        }

        [Test]
        [Ignore("This is unstable and not the final solution, does not support multiple instances")]
        public void CommandsWithSameIdAreQueued()
        {
            var concurrentCommands = 100;
            var commands = new List<TrackedCommand>();
            for (var count = 0; count < concurrentCommands; count++)
            {
                commands.Add(DomainMessageTranslator.TranslateCommand(new TestDomainCommand(),
                    new CommandType("TestDomainCommand")));
            }
            Parallel.For(0, concurrentCommands, count =>
            {
                _eventHandler.Dispatch(commands[count]);
            });
            Thread.Sleep(100);
            Assert.AreEqual(concurrentCommands, _handleCommandInstance.Handled.Count);
            Assert.AreEqual("TestDomainCommand", _handleCommandInstance.Handled.First());
            var times = _handleCommandInstance.Times.OrderBy(x => x.Item1.Ticks).ToArray();

            for (var count = 0; count < concurrentCommands - 1; count++)
            {
                Assert.IsTrue(times[count].Item1.Ticks < times[count].Item2.Ticks);
                Assert.IsTrue(times[count + 1].Item1.Ticks < times[count + 1].Item2.Ticks);
                Assert.IsTrue(times[count].Item2.Ticks <= times[count+1].Item1.Ticks);
            }
        }
    }

    [ExcludeFromCodeCoverage]
    class TestDomainEvent2 : DomainEvent
    {
        public TestDomainEvent2(Guid id) : base(id)
        {
        }
    }
    [ExcludeFromCodeCoverage]
    class TestDomainEvent3 : DomainEvent
    {
        public TestDomainEvent3(Guid id) : base(id)
        {
        }
    }
    [ExcludeFromCodeCoverage]
    class TestDomainEvent4 : DomainEvent
    {
        public TestDomainEvent4(Guid id) : base(id)
        {
        }
    }
    class TestHandleEvent : IHandleEvent<TestDomainEvent>, IHandleEvent<TestDomainEvent2>, IHandleEvent<TestDomainEvent4>
    {
        public List<string> Handled = new List<string>();
        public void Handle(TestDomainEvent message)
        {
            Handled.Add("TestDomainEvent");
        }
        public void Handle(TestDomainEvent2 message)
        {
            Handled.Add("TestDomainEvent2");
        }
        public void Handle(TestDomainEvent4 message)
        {
            throw new Exception("Should not cause any problem!");
        }
    }
    class TestHandleCommand : IHandleCommand<TestDomainCommand>
    {
        public ConcurrentStack<Tuple<DateTime, DateTime>> Times = new ConcurrentStack<Tuple<DateTime, DateTime>>();
        public ConcurrentStack<string> Handled = new ConcurrentStack<string>();
        public void Handle(TestDomainCommand message)
        {
            // Avoid duplicate time
            Thread.Sleep(20);
            var start = DateTime.Now;
            Handled.Push("TestDomainCommand");
            // Simulate process time
            Thread.Sleep(20);
            Times.Push(Tuple.Create(start, DateTime.Now));
            
        }
    }

}
