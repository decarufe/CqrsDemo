namespace Pyxis.Cqrs.Messages
{
    public interface IBaseHandler<T>  where T : IDomainMessage
    {
        void Handle(T message);
    }
    public interface IBaseHandler
    {
    }

}