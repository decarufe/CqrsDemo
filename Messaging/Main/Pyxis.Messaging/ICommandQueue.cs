using System;
using Pyxis.Messaging.Command;

namespace Pyxis.Messaging
{
    public interface ICommandQueue
    {
        void QueueCommand(params ICommand[] commands);
        void QueueCommand(DateTime time, params ICommand[] commands);
        void QueueCommand(params CommandType[] types);
        void QueueCommand(DateTime time, params CommandType[] types);
        void QueueFollowupCommand(CommandType type, ICommand sourceCommand);
    }
}