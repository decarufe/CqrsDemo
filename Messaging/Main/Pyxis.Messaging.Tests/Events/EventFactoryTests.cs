using System.Collections.Generic;
using NUnit.Framework;
using Pyxis.Messaging.Events;

namespace Pyxis.Messaging.Tests.Events
{
    [TestFixture]
    public class EventFactoryTests
    {
        [Test]
        public void TestThatEventCanBeCreatedWithContentAndAttributes()
        {
            var attributes = new Dictionary<string, object> {{"test", "value"}};
            var eventObj = EventFactory.CreateEvent( "content", attributes);
            Assert.AreEqual("content", eventObj.Content);
            Assert.AreEqual("value", eventObj.Attributes["test"]);
        }

        [Test]
        public void TestThatEventTypeIsProperlyInterpreted()
        {
            var attributes = new Dictionary<string, object> { { "__type", "sometype" } };
            var eventObj = EventFactory.CreateEvent("content", attributes);
            Assert.AreEqual("sometype", eventObj.Type.Value);
        }

        [Test]
        public void TestThatEventTypeIsProperlyUsed()
        {
            var eventType = new EventType("type");
            var eventObj = EventFactory.CreateEvent(eventType, "content");
            Assert.AreEqual("content", eventObj.Content);
            Assert.AreEqual(eventType.Value, eventObj.Type.Value);
        }

  
    }
}
