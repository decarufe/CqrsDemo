using System;
using System.Collections.Generic;
using Pyxis.Cqrs.Commands;
using Pyxis.Cqrs.Messages;
using Pyxis.Messaging.Events;

namespace Pyxis.Cqrs.Events
{
    public class MessagingEventHandler : MessagingHandler<ICqrsEventHandler, IEvent>
    {
        public MessagingEventHandler(IEnumerable<ICqrsEventHandler> handlers) : base(handlers, typeof(ICqrsEventHandler<>), false)
        {
        }

        protected override ICqrsMessage Translate(IEvent message, Type targetType)
        {
            var cqrsEvent = CqrsMessageTranslator.TranslateEvent(message, targetType);
            return (ICqrsMessage)cqrsEvent;
        }
    }
}
