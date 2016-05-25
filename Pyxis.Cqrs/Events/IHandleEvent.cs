using Pyxis.Cqrs.Messages;

namespace Pyxis.Cqrs.Events
{
    public interface IHandleEvent<T> : IHandleEvent, IBaseHandler<T> where T : DomainEvent
    {
    }
    public interface IHandleEvent : IBaseHandler
    {
    }
}