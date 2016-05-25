using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Pyxis.Messaging.Command;

namespace Pyxis.Messaging.Events
{
    public class EventDispatcher : IMessageDispatcher<IEvent>
    {
        private readonly IEnumerable<IMessageHandler<IEvent>> _eventListeners;

        public EventDispatcher(IEnumerable<IMessageHandler<IEvent>> eventListeners)
        {
            _eventListeners = eventListeners;
        }

        public bool Dispatch(IEvent eventObject)
        {
            Trace.WriteLine("Dispatching event " + eventObject.Type.Value);
            var handlers = _eventListeners.Where(x => x.CanHandle(eventObject)).ToArray();
            foreach (var eventListener in handlers)
            {
                Trace.WriteLine("Dispatching message to " + eventListener.GetType().Name);
                var listener = eventListener;
                var wcb = new WaitCallback(x => listener.Handle(x as IEvent));
                ThreadPool.QueueUserWorkItem(wcb, eventObject);
            }

            return true;
        }
    }
}