using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Pyxis.Core.Id;
using Pyxis.Persistance.Azure;

namespace Pyxis.Persistance.Tests.Azure
{
    class DynamicEntityMapper : IDomainEntityMapper
    {
        public TD Map<TS, TD>(TS source) where TD : class
        {
            if (source is DynamicTableEntity)
            {
                var typedSource = source as DynamicTableEntity;
                var toReturn = JsonConvert.DeserializeObject(typedSource.Properties["json"].StringValue, typeof(TD));
                return toReturn as TD;
            }
            if (source is IIdentifiable)
            {
                var typedSource = source as IIdentifiable;
                var toReturn = new DynamicTableEntity();
                toReturn.RowKey = typedSource.Id;
                toReturn.Properties["json"] = new EntityProperty(JsonConvert.SerializeObject(source));
                return toReturn as TD;
            }
            throw new NotSupportedException();
        }

        public IEnumerable<TD> Map<TS, TD>(IEnumerable<TS> entities) where TD : class
        {
            return entities.Select(Map<TS, TD>);
        }
    }
}
