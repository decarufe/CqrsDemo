using Pyxis.Cqrs.Bus;

namespace Pyxis.Cqrs.Result
{
    public interface IResultPublisher
    {
        void Publish(DomainResult result);
    }
}
