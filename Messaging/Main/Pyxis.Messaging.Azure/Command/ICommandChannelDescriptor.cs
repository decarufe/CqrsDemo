namespace Pyxis.Messaging.Azure.Command
{
    public interface ICommandChannelDescriptor : IChannelDescriptor
    {
        bool DetectDuplicateMessages { get; }
    }
}
