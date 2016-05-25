using System;
using Newtonsoft.Json;
using Pyxis.Cqrs.Messages;
using Pyxis.Cqrs.Result;
using Pyxis.Messaging.Command;

namespace Pyxis.Cqrs.Commands
{
    public class DomainMessageTranslator
    {
        public static TrackedCommand TranslateCommand<T>(T cqrsCommand, CommandType type) where T : IDomainMessage
        {
            cqrsCommand.TrackingId = Guid.NewGuid().ToString();
            var command = new TrackedCommand(cqrsCommand.TrackingId, type, JsonConvert.SerializeObject(cqrsCommand, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All }));
            command.Id = cqrsCommand.Id.ToString();
            return command;
        }
        public static IDomainMessage TranslateCommand(ICommand command, Type targetType) 
        {
            var message = (IDomainMessage)JsonConvert.DeserializeObject(command.Content, targetType);
            return message;
        }
    }
}
