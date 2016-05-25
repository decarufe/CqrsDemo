using System;
using Pyxis.Cqrs.Messages;

namespace Pyxis.Cqrs.Events
{
    [Serializable]
    public class DomainEvent : IDomainMessage
    {
        public Guid Id { get; set; }
        public SessionInfo SessionInfo { get; set; }
        public int Version { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
        public string TrackingId { get; set; }

        public DomainEvent(Guid id)
        {
            Id = id;
        }
    }
}