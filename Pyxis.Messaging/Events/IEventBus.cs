namespace Pyxis.Messaging.Events
{
    public interface IEventBus
    {
        void Broadcast(EventType eventType, string data = null);
        void Broadcast(IEvent @event);
    }
}