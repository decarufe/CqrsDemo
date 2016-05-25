namespace Pyxis.Cqrs.Events
{
    public interface IDomainEventPublisher
    {
        void Publish<T>(T[] @event, string trackingId = null) where T : DomainEvent;
    }
}