using System.Collections.Generic;
using Pyxis.Core.Id;

namespace Pyxis.Persistance
{
    public interface IPersistanceStore
    {
        void Save<T>(T content, string context = "default") where T : class, IIdentifiable;
        void Save<T>(IEnumerable<T> content, string context = "default") where T : class, IIdentifiable;
        T Get<T>(string key, string context = "default") where T : class, IIdentifiable;
        void Delete<T>(string key, string context = "default") where T : class, IIdentifiable;
        void Delete<T>(string[] keys, string context = "default") where T : class, IIdentifiable;
        bool Any<T>(string context = "default") where T : class, IIdentifiable;
        void Purge<T>(string context = "default") where T : class, IIdentifiable;
        IEnumerable<T> GetSome<T>(int count, string context = "default") where T : class, IIdentifiable;
        // Context specific
        void Purge(string context = "default");
        IEnumerable<T> GetAll<T>(string context = "default") where T : class, IIdentifiable;
        
    }
}