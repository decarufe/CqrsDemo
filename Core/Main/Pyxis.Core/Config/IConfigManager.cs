namespace Pyxis.Core.Config
{
    public interface IConfigManager : IConfigProvider
    {
        T Get<T>(string key);
        T Get<T>(string key, T defaultValue);
    }
}