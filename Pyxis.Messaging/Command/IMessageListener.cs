namespace Pyxis.Messaging.Command
{
    public interface IMessageListener
    {
        void Listen();
        void Close();
    }
}
