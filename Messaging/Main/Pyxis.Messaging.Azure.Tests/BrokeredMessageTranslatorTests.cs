using System.Collections.Generic;
using NUnit.Framework;
using Pyxis.Messaging.Command;
using Pyxis.Messaging.Events;

namespace Pyxis.Messaging.Azure.Tests
{
    [TestFixture]
    public class BrokeredMessageTranslatorTests
    {
        [Test]
        public void TestCommandCanBeTranslated()
        {
            var command = new TestCommand();
            var message = BrokeredMessageTranslator.CreateMessage(command);
            var commandReCreated = BrokeredMessageTranslator.CreateCommand(message);
            Assert.IsNotNull(message);
            Assert.AreEqual(command.Id, commandReCreated.Id);
            Assert.AreEqual(command.Async, commandReCreated.Async);
            Assert.AreEqual(command.Attributes.Count, commandReCreated.Attributes.Count);
            foreach (var key in command.Attributes.Keys)
            {
                Assert.AreEqual(command.Attributes[key], commandReCreated.Attributes[key]);
            }
        }

        [Test]
        public void TestEventCanBeTranslated()
        {
            var myEvent = new TestEvent();
            var message = BrokeredMessageTranslator.CreateMessage(myEvent);
            var eventReCreated = BrokeredMessageTranslator.CreateEvent(message);
            Assert.IsNotNull(message);
            Assert.AreEqual(myEvent.Attributes.Count, eventReCreated.Attributes.Count);
            foreach (var key in myEvent.Attributes.Keys)
            {
                Assert.AreEqual(myEvent.Attributes[key], eventReCreated.Attributes[key]);
            }
        }
    }

    internal class TestCommand : ICommand
    {
        public TestCommand()
        {
            Attributes = new Dictionary<string, object>();
            Attributes["customAttr"] = "customValue";
        }
        public string Id { get { return "MyId"; } }
        public CommandType Type { get{ return new CommandType("TestCommand");} }
        public bool Async { get { return true; }}

        MessageType IMessage.Type
        {
            get { return Type; }
        }

        public string Content { get { return "MyContent"; } }
        public IDictionary<string, object> Attributes { get; private set; }
    }

    class TestEvent : IEvent
    {
        public TestEvent()
        {
            Attributes = new Dictionary<string, object>();
            Attributes["customAttr"] = "customValue";
        }
        public string Id { get { return "MyId"; } }
        public EventType Type { get {return new EventType("MyEvent");} }

        MessageType IMessage.Type
        {
            get { return Type; }
        }

        public string Content { get { return "MyContent"; } }
        public IDictionary<string, object> Attributes { get; private set; }
    }
}
