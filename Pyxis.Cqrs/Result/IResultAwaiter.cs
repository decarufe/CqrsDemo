using Pyxis.Cqrs.Commands;

namespace Pyxis.Cqrs.Result
{
    public interface IResultAwaiter
    {
        DomainResult WaitForCommand(ITrackable command);
        DomainResult WaitForResults(ITrackable command);
    }
}
