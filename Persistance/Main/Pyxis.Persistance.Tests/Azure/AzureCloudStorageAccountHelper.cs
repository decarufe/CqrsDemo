using Microsoft.WindowsAzure.Storage;

namespace Pyxis.Persistance.Tests.Azure
{
    class AzureCloudStorageAccountHelper
    {
        private static string _accessKey = "HeQWm7Gld6sBb8E9Q3xAJOC4lHpdS/5ZkEwfYWvXpcW/R2WmEzKiM/ysL24ezzt4el4kOtGnSQxOpsC1Be3cUg==";
        private static string _storageUrl = @"pyxispersistence";
        private readonly CloudStorageAccount _storageAccount;

        public static CloudStorageAccount Create()
        {
            return CloudStorageAccount.Parse(
                string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", _storageUrl, _accessKey)); 
        }
    }
}
