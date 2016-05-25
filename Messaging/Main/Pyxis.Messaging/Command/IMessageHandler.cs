namespace Pyxis.Messaging.Command
{
    public interface IMessageHandler<in T> where T: IMessage
    {
        bool CanHandle(T message);
        bool Handle(T command);
    }
}