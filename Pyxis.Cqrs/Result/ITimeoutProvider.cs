namespace Pyxis.Cqrs.Result
{
    public interface ITimeoutProvider
    {
        long Timeout { get; }
    }
}
