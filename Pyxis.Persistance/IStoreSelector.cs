using System.Collections.Generic;

namespace Pyxis.Persistance
{
    public interface IStoreSelector
    {
        IPersistanceStore GetStoreFor<T>();
        IEnumerable<IPersistanceStore> GetAllStores();
    }
}
