namespace Pyxis.Messaging.Events
{
    public interface IEvent : IMessage
    {
        new EventType Type { get; }
    }
}