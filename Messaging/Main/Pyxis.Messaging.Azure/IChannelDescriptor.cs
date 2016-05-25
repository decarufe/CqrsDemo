namespace Pyxis.Messaging.Azure
{
    public interface IChannelDescriptor
    {
        string ConnectionString { get; }
        string ChannelName { get; }
    }
}
