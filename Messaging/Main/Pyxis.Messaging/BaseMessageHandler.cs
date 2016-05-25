using System.Collections.Generic;
using System.Linq;

namespace Pyxis.Messaging
{
    public abstract class BaseMessageHandler<T> where T : IMessage
    {
        private readonly List<MessageType> _handledMessageType = new List<MessageType>();

        protected BaseMessageHandler(params MessageType[] handledMessageType)
        {
            _handledMessageType.AddRange(handledMessageType);
        }

        protected void AddMessageHandlers(params MessageType[] handledMessageType)
        {
            _handledMessageType.AddRange(handledMessageType);
        }

        public bool CanHandle(T message)
        {
            return _handledMessageType.Any(x=> x.Value.Equals(message.Type.Value));
        }
    }
}