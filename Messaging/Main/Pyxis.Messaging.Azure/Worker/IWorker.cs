namespace Pyxis.Messaging.Azure.Worker
{
    public interface IWorker
    {
        bool Run();
        bool OnStart();
        bool OnStop();
    }
}