using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using log4net;
using Microsoft.WindowsAzure.Storage.Blob;
using Pyxis.Core.Id;
using Pyxis.Persistance.Container;

namespace Pyxis.Persistance.Azure
{
    public class BlobPersistance : IPersistanceStore
    {
        private readonly CloudBlobClient _blobClient;
        private readonly ILog _logger = LogManager.GetLogger(typeof(BlobPersistance));

        public BlobPersistance(CloudBlobClient blobClient)
        {
            _blobClient = blobClient;
        }

        private CloudBlobContainer GetContextContainer(string contextName)
        {
            var container = _blobClient.GetContainerReference(contextName.ToLower());
            _logger.Debug("Container create if not exists");
            try
            {
                var exists = container.Exists();
                _logger.DebugFormat("Container exitst: {0}", exists);
                if (!exists)
                {
                    var success = container.CreateIfNotExists(BlobContainerPublicAccessType.Blob);
                    _logger.DebugFormat("Container create returned {0}", success);
                }
            }
            catch (Exception e)
            {
                _logger.Warn("Exception while trying to create the container reference, assuming race condition: " + e);
                throw;
            }

            return container;
        }

        public void Save<T>(T toSave, string context = "default") where T : class, IIdentifiable
        {
            var persistedObject = toSave as PersistedObject ?? new PersistedObject { Content = toSave };
            var container = GetContextContainer(context);
            var blockBlob = container.GetBlockBlobReference(persistedObject.Id);

            var stream = new MemoryStream();
            using (stream)
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, persistedObject);
                stream.Position = 0;
                blockBlob.UploadFromStream(stream);
            }
        }

        public void Save<T>(IEnumerable<T> content, string context = "default") where T : class, IIdentifiable
        {
            foreach (var toSave in content)
            {
                Save(toSave, context);
            }
        }

        public T Get<T>(string key, string context = "default") where T : class, IIdentifiable
        {
            var container = GetContextContainer(context);
            var blockBlob = container.GetBlockBlobReference(key);
            var deserialized = Deserialize<T>(blockBlob);
            return deserialized;
        }

        public void Delete<T>(string[] keys, string context = "default") where T : class, IIdentifiable
        {
            var container = GetContextContainer(context);
            foreach (var key in keys)
            {
                var blockBlob = container.GetBlockBlobReference(key);
                if (blockBlob != null)
                {
                    blockBlob.DeleteIfExists();
                }
            }
        }

        public void Delete<T>(string key, string context = "default") where T : class, IIdentifiable
        {
            Delete<T>(new []{key}, context);
        }

        private static T Deserialize<T>(CloudBlockBlob blockBlob) where T : class, IIdentifiable
        {
            if (!blockBlob.Exists()) return null;
            var stream = new MemoryStream();
            var formatter = new BinaryFormatter();
            T deserialized = null;
            using (stream)
            {
                try
                {
                    blockBlob.DownloadToStream(stream);
                    stream.Position =0;
                    if (stream.Length > 0)
                    {
                        deserialized = formatter.Deserialize(stream) as T;
                        
                    }
                }
                catch (Exception e)
                {
                    Trace.TraceError("Could not download blob for {0}:{1}", typeof(T).Name, e);
                }
            }
            return deserialized;
        }

        public bool Any<T>(string context = "default") where T : class, IIdentifiable
        {
            // Not relevant here
            return true;
        }

        public void Purge<T>(string context = "default") where T : class, IIdentifiable
        {
            throw new NotSupportedException();
        }

        public void Purge(string context = "default")
        {
            var container = _blobClient.GetContainerReference(context.ToLower());
            container.DeleteIfExists();
        }

        public IEnumerable<T> GetAll<T>(string context = "default") where T : class, IIdentifiable
        {
            var toReturn = new List<T>();
            var container = GetContextContainer(context);
            foreach (var item in container.ListBlobs())
            {
                if (item is CloudBlockBlob)
                {
                    var blob = (CloudBlockBlob) item;
                    toReturn.Add(Deserialize<T>(blob));
                }
            }
            return toReturn;
        }

        public IEnumerable<T> GetSome<T>(int count, string context = "default") where T : class, IIdentifiable
        {
            var toReturn = new List<T>();
            var container = GetContextContainer(context);
            var loadCount = 0;
            var itr = container.ListBlobs().GetEnumerator();
            while (loadCount < count && itr.MoveNext())
            {
                var blockBlob = itr.Current as CloudBlockBlob;
                if (blockBlob != null)
                {
                    var blob = blockBlob;
                    toReturn.Add(Deserialize<T>(blob));
                    loadCount++;
                }
            }
            return toReturn;
        }
    }
}
