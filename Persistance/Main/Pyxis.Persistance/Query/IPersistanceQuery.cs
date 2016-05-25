using System.Collections.Generic;
using Pyxis.Core.Id;

namespace Pyxis.Persistance.Query
{
    public interface IPersistanceQuery 
    {
        IEnumerable<T> Query<T>(QueryElement condition, int resultLimit = int.MaxValue, string context = "default") where T : class, IIdentifiable;
        IEnumerable<T> Query<T>(IEnumerable<QueryElement> conditions, int resultLimit = int.MaxValue, string context = "default") where T : class, IIdentifiable;
    }
}