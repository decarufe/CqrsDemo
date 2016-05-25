using System;
using System.Threading.Tasks;

namespace Pyxis.Messaging.Command
{
    public class InProcCommandQueue : ICommandQueue
    {
        private readonly IMessageDispatcher<ICommand> _commandDispatcher;

        public InProcCommandQueue(IMessageDispatcher<ICommand> commandDispatcher)
        {
            _commandDispatcher = commandDispatcher;
        }

        public void QueueCommand(params ICommand[] commands)
        {
            foreach (var command in commands)
            {
                _commandDispatcher.Dispatch(command);
            }
        }

        public void QueueCommand(DateTime time, params ICommand[] commands)
        {
            if (time <= DateTime.Now)
            {
                QueueCommand(commands);
            }
            else
            {
                TimedCommand(time, () => QueueCommand(commands));
            }
        }

        public void QueueCommand(params CommandType[] types)
        {
            foreach (var commandType in types)
            {
                var command = CommandFactory.CreateCommand(commandType);
                QueueCommand(command);
            }
        }

        private async void TimedCommand(DateTime time, Action queueCommand)
        {
            if (time > DateTime.Now)
            {
                var timeoutInMilliseconds = (time - DateTime.Now).Milliseconds;
                await Task.Delay(timeoutInMilliseconds);
            }
            queueCommand.Invoke();
        }

        public void QueueCommand(DateTime time, params CommandType[] types)
        {
            if (time <= DateTime.Now)
            {
                QueueCommand(types);
            }
            else
            {
                TimedCommand(time, () => QueueCommand(types));
            }
        }

        public void QueueFollowupCommand(CommandType type, ICommand sourceCommand)
        {
            QueueCommand(type);
        }
    }
}