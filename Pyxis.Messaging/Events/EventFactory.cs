using System.Collections.Generic;

namespace Pyxis.Messaging.Events
{
    public class EventFactory
    {
        public static IEvent CreateEvent(string content, IDictionary<string, object> properties)
        {
            return new Event
            {
                Content = content,
                Attributes = new Dictionary<string, object>(properties)
            };
        }
        public static IEvent CreateEvent(EventType type, string content= "")
        {
            var eventObj = new Event(type, content);
            return eventObj;
        }
    }
}