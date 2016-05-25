using System;

namespace Pyxis.Messaging.Command
{
    public class Command : Message, ICommand
    {

        internal Command()
        {
        }

        public Command(ICommand source)
        {
            _attributes = source.Attributes;
            Content = source.Content;
        }

        public Command(CommandType type, string content = null, bool async = true)
        {
            base.Type = type;
            Async = async;
            Content = content;
        }

        public new CommandType Type
        {
            get { return new CommandType(base.Type.Value); }
        }

        public bool Async { get; set; }
    }
}