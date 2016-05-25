using System.Configuration;

namespace Pyxis.Core.Config
{
    public class AppConfigProvider : IConfigProvider
    {
        public string Get(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}
