namespace Pyxis.Core.Config
{
    public interface IConfigProvider
    {
        string Get(string key);
    }
}