using Pyxis.Cqrs.Result;

namespace Pyxis.Cqrs.Commands
{
    public interface ICommandSender
    {
        DomainResult Send<T>(T command) where T : IDomainCommand;
        DomainResult SendAndWait<T>(T command) where T : IDomainCommand;
    }
}