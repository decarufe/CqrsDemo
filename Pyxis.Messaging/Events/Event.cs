namespace Pyxis.Messaging.Events
{
    public class Event : Message, IEvent
    {
        internal Event()
        {
        }
        public Event(EventType type, string content = null)
        {
            base.Type = type;
            Content = content;
        }
        public new EventType Type
        {
            get { return new EventType(base.Type.Value); }
        }
    }
}