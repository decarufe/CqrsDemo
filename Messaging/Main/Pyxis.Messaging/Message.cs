using System.Collections.Generic;

namespace Pyxis.Messaging
{
    public abstract class Message : IMessage
    {
        private const string MessageTypeAttribute = "__type";
        internal IDictionary<string, object> _attributes = new Dictionary<string, object>();

        public MessageType Type
        {
            get
            {
                return _attributes.ContainsKey(MessageTypeAttribute)
                    ? new MessageType (_attributes[MessageTypeAttribute] as string)
                    : new MessageType (string.Empty);
            }
            set { _attributes[MessageTypeAttribute] = value.Value; }
        }

        public string Id { get;  set; }
        public string Content { get; protected internal set; }

        public T TryGetAttribute<T>(string name) 
        {
            object value;
            _attributes.TryGetValue(name, out value);
            return (T)value ;
        }

        public string TryGetAttribute(string name) 
        {
            return TryGetAttribute<string>(name);
        }

        public IDictionary<string, object> Attributes
        {
            get { return _attributes; }
            set
            {
                var type = Type;
                _attributes = value;
                if (!string.IsNullOrEmpty(type.Value))
                {
                    _attributes[MessageTypeAttribute] = type.Value;
                }
            }
        }
    }
}