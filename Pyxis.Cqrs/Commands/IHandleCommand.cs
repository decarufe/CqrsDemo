using Pyxis.Cqrs.Messages;

namespace Pyxis.Cqrs.Commands
{
    public interface IHandleCommand<T> : IHandleCommand, IBaseHandler<T> where T :IDomainCommand
    {
    }

    public interface IHandleCommand : IBaseHandler
    {
    }

}