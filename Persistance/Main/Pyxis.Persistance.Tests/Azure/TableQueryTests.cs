using Microsoft.WindowsAzure.Storage.Table;
using NUnit.Framework;
using Pyxis.Persistance.Azure;
using Pyxis.Persistance.Tests.Query;

namespace Pyxis.Persistance.Tests.Azure
{
    [TestFixture]
    public class TableQueryTests : BaseQueryTests
    {
        private CloudTableClient _client;

        [SetUp]
        public void SetUp()
        {
            _client = AzureCloudStorageAccountHelper.Create().CreateCloudTableClient();
            var store = new TableStorePersistance(_client, new QueryableEntityMapper());
            QueryStore = store;
            PersistanceStore = store;
            store.Purge();
        }
    }
}


