using System;
using System.Collections.Generic;
using Pyxis.Cqrs.Domain.Exception;
using Pyxis.Cqrs.Events;
using Pyxis.Cqrs.Infrastructure;

namespace Pyxis.Cqrs.Domain
{
    public abstract class Entity
    {
        private readonly List<DomainEvent> _changes = new List<DomainEvent>();

        public AggregateRoot AggregateRoot { get; set; }
        public Guid Id { get; protected set; }
        public int Version { get; protected set; }

        public IEnumerable<DomainEvent> GetUncommittedChanges()
        {
            lock (_changes)
            {
                return _changes.ToArray();
            }
        }

        public IEnumerable<DomainEvent> FlushUncommitedChanges()
        {
            lock (_changes)
            {
                var changes = _changes.ToArray();
                var i = 0;
                foreach (var @event in changes)
                {
                    if (@event.Id == Guid.Empty && Id == Guid.Empty)
                        throw new AggregateOrEventMissingIdException(GetType(), @event.GetType());
                    if (@event.Id == Guid.Empty)
                        @event.Id = Id;
                    i++;
                    @event.Version = Version + i;
                    @event.TimeStamp = DateTimeOffset.UtcNow;
                }
                Version = Version + _changes.Count;
                _changes.Clear();
                return changes;
            }
        }

        public void LoadFromHistory(IEnumerable<DomainEvent> history)
        {
            foreach (var e in history)
            {
                if (e.Version != Version + 1)
                    throw new EventsOutOfOrderException(e.Id);
                ApplyChange(e, false);
            }
        }

        protected void ApplyChange(DomainEvent domainEvent)
        {
            ApplyChange(domainEvent, true);
        }

        private void ApplyChange(DomainEvent domainEvent, bool isNew)
        {
            lock (_changes)
            {
                this.AsDynamic().Apply(domainEvent);
                if (isNew)
                {
                    AggregateRoot.TrackEntityEvent(domainEvent);
                    _changes.Add(domainEvent);
                }
                else
                {
                    Id = domainEvent.Id;
                    Version++;
                }
            }
        }
    }
}