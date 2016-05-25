using System.Collections.Generic;

namespace Pyxis.Messaging
{
    public interface IMessage
    {
        string Id { get; }
        MessageType Type { get; }
        string Content { get; }
        IDictionary<string, object> Attributes { get; }
    }
}