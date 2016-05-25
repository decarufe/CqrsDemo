using Pyxis.Messaging.Command;

namespace Pyxis.Messaging.Events
{
    public class InProcEventBus : IEventBus
    {
        private readonly IMessageDispatcher<IEvent> _eventDispatcher;
        public InProcEventBus(IMessageDispatcher<IEvent> dispatcher)
        {
            _eventDispatcher = dispatcher;
        }

        public void Broadcast(EventType eventType, string data = null)
        {
            var eventObj = EventFactory.CreateEvent(eventType, data);
            Broadcast(eventObj);
        }

        public void Broadcast(IEvent eventObj)
        {
            _eventDispatcher.Dispatch(eventObj);
        }
    }
}