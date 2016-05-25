using System.Collections.Generic;

namespace Pyxis.Messaging.Command
{
    public class CommandFactory
    {
        public static ICommand CreateCommand(string id, string content, IDictionary<string, object> properties, bool async = true)
        {
            var  command = new Command
            {
                Content = content,
                Id = id,
                Attributes = new Dictionary<string, object>(properties)
            };
            command.Async = async;
            return command;
        }

        public static ICommand CreateCommand(CommandType type, bool async = true, ICommand sourceCommand = null)
        {
            return CreateCommand(type, string.Empty, async, sourceCommand);
        }

        public static ICommand CreateCommand(CommandType type, string content, bool async = true,
            ICommand sourceCommand = null)
        {
            var message = new Command(type, content, async);
            if (sourceCommand != null)
            {
                message.Async = sourceCommand.Async;
            }
            return message;
        }
    }
}