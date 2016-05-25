using System.Collections.Generic;

namespace Pyxis.Persistance.Azure
{
    public interface IDomainEntityMapper
    {
        TD Map<TS,TD>(TS source) where TD: class;
        IEnumerable<TD> Map<TS, TD>(IEnumerable<TS> entities) where TD : class;
    }
}


