using System.Threading;
using NUnit.Framework;
using Pyxis.Messaging.Command;
using Pyxis.Messaging.Events;

namespace Pyxis.Messaging.Tests.Events
{
    [TestFixture]
    public class InProcEventBusTests
    {
        private InProcEventBus _eventBus;
        private TestDispatcher _dispatcher;

        [SetUp]
        public void SetUp()
        {
            _dispatcher = new TestDispatcher();
            _eventBus = new InProcEventBus(_dispatcher);
            
        }

        [Test]
        public void TestThatEventIsDispactchedRightAwayForCommandType()
        {
            _eventBus.Broadcast(new EventType("myevent"));
            Thread.Sleep(20);
            Assert.IsTrue(_dispatcher.Called);
        }
    }

    class TestDispatcher : IMessageDispatcher<IEvent>
    {
        public bool Called { get; set; }
        public bool Dispatch(IEvent command)
        {
            Called = true;
            return true;
        }
    }
}
