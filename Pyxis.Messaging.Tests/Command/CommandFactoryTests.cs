using System.Collections.Generic;
using NUnit.Framework;
using Pyxis.Messaging.Command;

namespace Pyxis.Messaging.Tests.Command
{
    [TestFixture]
    public class CommandFactoryTests
    {
        private readonly CommandType _commandType = new CommandType("mymessage");
        [Test]
        public void TestThatMessageCanBeCreatedWithIdContentAndAttributes()
        {
            var attributes = new Dictionary<string, object> {{"test", "value"}};
            var message = CommandFactory.CreateCommand("id", "content", attributes);
            Assert.AreEqual("id", message.Id);
            Assert.AreEqual("content", message.Content);
            Assert.AreEqual("value", message.Attributes["test"]);
            Assert.IsTrue(message.Async);
        }

        [Test]
        public void TestThatMessageTypeIsProperlyInterpreted()
        {
            var attributes = new Dictionary<string, object> { { "__type", "sometype" } };
            var message = CommandFactory.CreateCommand("id", "content", attributes);
            Assert.AreEqual("sometype", message.Type.Value);
        }

        [Test]
        public void TestThatMessageCanBeCreatedWithTypeAndReferenceMessage()
        {
            var attributes = new Dictionary<string, object> { { "test", "value" } };
            var message = CommandFactory.CreateCommand("id", "content", attributes);
            var newMessage = CommandFactory.CreateCommand(_commandType, "content", true, message);
            Assert.IsTrue(newMessage.Async);
            Assert.AreEqual("content", message.Content);
        }

        [Test]
        public void TestThatSynchronousReferenceMessageProducesASynchronousMessage()
        {
            var referenceMessage = CommandFactory.CreateCommand(_commandType, "content", false);
            var newMessage = CommandFactory.CreateCommand(_commandType, "content", true, referenceMessage);
            Assert.IsFalse(newMessage.Async);
        }

        [Test]
        public void TestThatMessageTypeIsProperlyUsed()
        {
            var message = CommandFactory.CreateCommand(_commandType, "content");
            Assert.AreEqual("content", message.Content);
            Assert.AreEqual(_commandType.Value, message.Type.Value);
        }

        [Test]
        public void TestAsyncFlagCanBeVerified()
        {
            var message = CommandFactory.CreateCommand(_commandType, "content");
            Assert.IsTrue(message.Async);
            message = CommandFactory.CreateCommand(_commandType, "content", false);
            Assert.IsFalse(message.Async);
        }

        [Test]
        public void TestThatTypeIsNotLostWhenResettingAttributes()
        {
            var message = CommandFactory.CreateCommand(_commandType, "content") as Message;
            message.Attributes = new Dictionary<string, object>();
            Assert.AreEqual(_commandType.Value, message.Type.Value);
            Assert.AreEqual(1, message.Attributes .Count);
        }

        [Test]
        public void TestThatEmptyTypeIsLostWhenResettingAttributes()
        {
            var message = CommandFactory.CreateCommand(new CommandType(""), "content") as Message;
            message.Attributes = new Dictionary<string, object>();
            Assert.AreEqual(0, message.Attributes.Count);
            Assert.AreEqual(string.Empty, message.Type.Value);
        }
    }
}
