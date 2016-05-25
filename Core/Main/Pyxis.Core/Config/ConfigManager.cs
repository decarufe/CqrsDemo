using System;
using System.Collections.Generic;

namespace Pyxis.Core.Config
{
    public class ConfigManager : IConfigManager
    {
        private readonly IEnumerable<IConfigProvider> _providers;

        public ConfigManager(IEnumerable<IConfigProvider> providers)
        {
            _providers = providers;
        }

        public string Get(string key)
        {
            return Get(key, false);
        }

        private string Get(string key, bool allowNull)
        {
            foreach (var provider in _providers)
            {
                var value = provider.Get(key);

                if (value != null)
                    return value;
            }

            return allowNull ? null : string.Empty;
        }


        public T Get<T>(string key)
        {
            return Get<T>(key, default(T));
        }

        public T Get<T>(string key, T defaultValue)
        {
            var value = Get(key,true);
            T converted = defaultValue;
            if (value != null)
            {
                try
                {
                    converted = (T) Convert.ChangeType(value, typeof (T));
                }
                catch (Exception)
                {
                }
            }
            return converted;
        }
    }
}
