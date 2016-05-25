using System;
using System.Collections.Generic;
using System.Linq;
using Pyxis.Cqrs.Messages;
using Pyxis.Messaging.Command;

namespace Pyxis.Cqrs.Commands
{
    public class MessagingCommandHandler : MessagingHandler<ICqrsCommandHandler, ICommand>
    {
        public MessagingCommandHandler(IEnumerable<ICqrsCommandHandler> handlers) : base(handlers, typeof(ICqrsCommandHandler<>), true)
        {
        }

        protected override ICqrsMessage Translate(ICommand message, Type targetType)
        {
            var cqrsEvent = CqrsMessageTranslator.TranslateCommand(message, targetType);
            return (ICqrsMessage)cqrsEvent;
        }
    }


}
