using Microsoft.WindowsAzure.Storage.Table;
using NUnit.Framework;
using Pyxis.Persistance.Azure;

namespace Pyxis.Persistance.Tests.Azure
{
    [TestFixture]
    public class TableStorePersistanceTests : BasePersistanceTests
    {
        private CloudTableClient _client;

        [SetUp]
        public void SetUp()
        {
            _client = _client = AzureCloudStorageAccountHelper.Create().CreateCloudTableClient();
            PersistanceStore = new TableStorePersistance(_client, new DynamicEntityMapper());
            PersistanceStore.Purge();
        }
    }
}

