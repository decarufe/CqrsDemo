using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Pyxis.Messaging.Command
{
    public class CommandDispatcher : IMessageDispatcher<ICommand>
    {
        private readonly IEnumerable<IMessageHandler<ICommand>> _messageHandlers;

        public CommandDispatcher(IEnumerable<IMessageHandler<ICommand>> messageHandlers)
        {
            _messageHandlers = messageHandlers;
        }

        public bool Dispatch(ICommand receivedCommand)
        {
            var result = false;
            Trace.WriteLine("Dispatching message " + receivedCommand.Type.Value);
            try
            {
                var handler = _messageHandlers.FirstOrDefault(x => x.CanHandle(receivedCommand));
                if (handler != null)
                {
                    Trace.WriteLine("Handling message in " + handler.GetType().Name);
                    result = handler.Handle(receivedCommand);
                    Trace.WriteLine(handler.GetType().Name + " is done with success: " + result);
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error processing message: {0}", e.Message);
            }
            return result;
        }
    }
}