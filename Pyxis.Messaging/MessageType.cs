namespace Pyxis.Messaging
{
    public class MessageType
    {
        public MessageType(string value)
        {
            Value = value;
        }
        public string Value { get; private set; }
    }
}