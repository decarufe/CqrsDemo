namespace Pyxis.Messaging.Email
{
    public interface ISubscriptionManager
    {
        void SubscribeToList(string listName, string email);
    }
}
