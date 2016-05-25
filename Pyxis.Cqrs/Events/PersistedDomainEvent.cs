using System;
using Pyxis.Core.Id;

namespace Pyxis.Cqrs.Events
{
    [Serializable]
    public class PersistedDomainEvent : IIdentifiable 
    {
        public PersistedDomainEvent()
        {

        }
        public PersistedDomainEvent(DomainEvent domainEvent) 
        {
            EventId = domainEvent.Id.ToString();
            Version = domainEvent.Version;
            Source = domainEvent;
        }

        public DomainEvent Source { get; set; }
        public int Version { get; set; }

        public string Id { get { return EventId + Version; }
            set { }
        }

        public string EventId { get; set; }
    }
}
