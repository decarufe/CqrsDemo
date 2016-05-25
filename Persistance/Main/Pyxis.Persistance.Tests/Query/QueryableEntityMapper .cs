using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Pyxis.Core.Id;
using Pyxis.Persistance.Azure;

namespace Pyxis.Persistance.Tests.Query
{
    class QueryableEntityMapper : IDomainEntityMapper
    {
        public TD Map<TS, TD>(TS source) where TD : class
        {
            if (source is DynamicTableEntity)
            {
                var typedSource = source as DynamicTableEntity;
                var toReturn = JsonConvert.DeserializeObject(typedSource.Properties["json"].StringValue, typeof(TD));
                return toReturn as TD;
            }
            else
            {
                var typedSource = source as IIdentifiable;
                var toReturn = new DynamicTableEntity();
                toReturn.RowKey = typedSource.Id;
                toReturn.Properties["json"] = new EntityProperty(JsonConvert.SerializeObject(source));
                if (source is QueryableTestObject)
                {
                    var willQuery = source as QueryableTestObject;
                    toReturn.Properties["IntVersion"] = new EntityProperty(willQuery.IntVersion);
                    toReturn.Properties["TimeStamp"] = new EntityProperty(willQuery.TimeStamp);
                    toReturn.Properties["DoubleVersion"] = new EntityProperty(willQuery.DoubleVersion);
                    toReturn.Properties["LongVersion"] = new EntityProperty(willQuery.LongVersion);
                    toReturn.Properties["StringVersion"] = new EntityProperty(willQuery.StringVersion);
                }
                return toReturn as TD;
            }
        }

        public IEnumerable<TD> Map<TS, TD>(IEnumerable<TS> entities) where TD : class
        {
            return entities.Select(Map<TS, TD>);
        }
    }
}
