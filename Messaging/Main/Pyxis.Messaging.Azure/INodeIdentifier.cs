namespace Pyxis.Messaging.Azure
{
    public interface INodeIdentifier
    {
        string InstallName { get; }
        string NodeName { get; }
    }
}
