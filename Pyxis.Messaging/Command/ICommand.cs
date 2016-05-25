namespace Pyxis.Messaging.Command
{
    public interface ICommand : IMessage
    {
        bool Async { get; }
        new CommandType Type { get; }
    }
}