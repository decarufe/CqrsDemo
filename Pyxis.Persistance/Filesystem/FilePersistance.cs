using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Pyxis.Core.Id;
using Pyxis.Persistance.Serialization;

namespace Pyxis.Persistance.Filesystem
{
    public class FilePersistance : IPersistanceStore
    {
        private readonly string _baseLocation;

        private readonly IIdGenerator _idGenerator;
        private readonly int _inUseRetryInterval;
        private readonly int _inUseRetryCount;

        public FilePersistance(string baseLocation = ""): this(new GuidIdGenerator(), baseLocation)
        {
        }

        public FilePersistance(IIdGenerator idGenerator, string baseLocation = "")
        {
            _inUseRetryInterval = 50;
            _inUseRetryCount = 1000;
            _idGenerator = idGenerator;
            _baseLocation = string.IsNullOrWhiteSpace(baseLocation) ? "./fspersistance" : baseLocation;
            Directory.CreateDirectory(_baseLocation);
        }

        public void Save<T>(T content, string context = "default") where T : class, IIdentifiable
        {
            SetContentId(content);
            var storage = GetContextStorage<T>(context);
            WriteFile(storage, content);
        }

        private void WriteFile<T>(DirectoryInfo storage, T content) where T : class, IIdentifiable
        {
            var serializer = SerializerFactory.GetSerializer(content);
            var fileName = Path.Combine(storage.FullName, content.Id.ToLower());

            using (var serializedContent = serializer.Serialize(content))
            {
                FileUtils.Write(fileName, serializedContent);
            }
        }

        public void Save<T>(IEnumerable<T> items, string context = "default") where T : class, IIdentifiable
        {
            var storage = GetContextStorage<T>(context);
            foreach (var item in items.Where(x=> x != null).ToArray())
            {
                SetContentId(item);
                WriteFile(storage, item);
            }
        }

        public T Get<T>(string key, string context = "default") where T : class, IIdentifiable
        {
            var storage = GetContextStorage<T>(context);
            return ReadFile<T>(storage, key);
        }

        private T ReadFile<T>(DirectoryInfo storage, string id) where T : class, IIdentifiable
        {
            if (id == null) return null;
            var files = storage.GetFiles(id.ToLower() + "*");
            if (files.Any())
            {
                return ReadFile<T>(files[0]);
            }
            return null;
        }

        private T ReadFile<T>(FileInfo file) where T : class, IIdentifiable
        {
            var serializer = SerializerFactory.GetSerializer(typeof(T));
            T content = null;
            var stream = FileUtils.Read(file.FullName);
            using (stream)
            {
                content = serializer.Deserialize<T>(stream);
                stream.Close();
            }
            return content;
        }

        public void Delete<T>(string key, string context = "default") where T : class, IIdentifiable
        {
            Delete<T>(new []{key}, context);
        }

        public void Delete<T>(string[] keys, string context = "default") where T : class, IIdentifiable
        {
            var storage = GetContextStorage<T>(context);
            foreach (var key in keys)
            {
                DeleteFile(storage.FullName, key);
            }
        }

        private void DeleteFile(string containerName, string key)
        {
            Retry(()=> File.Delete(Path.Combine(containerName, key.ToLower())));
        }

        private async void Retry(Action action)
        {
            for (var count = 0; count < _inUseRetryCount; count++)
            {
                try
                {
                    action.Invoke();
                    return;
                }
                catch (Exception)
                {
                    // In user, retry
                    
                }
                await Task.Delay(_inUseRetryInterval);
            }
        }

        public bool Any<T>(string context = "default") where T : class, IIdentifiable
        {
            var storage = GetContextStorage<T>(context);
            return storage.GetFiles().Any();
        }

        public void Purge<T>(string context = "default") where T : class, IIdentifiable
        {
            var storage = GetContextStorage<T>(context);
            Directory.Delete(storage.FullName, true); 
        }

        public void Purge(string context = "default")
        {
            var basePath = CreatePath(_baseLocation, string.Empty);
            foreach (var typeStoragePath in basePath.EnumerateDirectories())
            {
                var contextPath = CreatePath(typeStoragePath.FullName, context.ToLower());
                Directory.Delete(contextPath.FullName, true);
            }
        }

        public IEnumerable<T> GetAll<T>(string context = "default") where T : class, IIdentifiable
        {
            var toReturn = new List<T>();
            var storage = GetContextStorage<T>(context);
            foreach (var fileInfo in storage.GetFiles().ToArray())
            {
                var read = ReadFile<T>(fileInfo);
                toReturn.Add(read);
            }
            return toReturn;
        }

        public IEnumerable<T> GetSome<T>(int count, string context = "default") where T : class, IIdentifiable
        {
            return GetAll<T>(context).Take(count);
        }

        private void SetContentId<T>(T content) where T : class, IIdentifiable
        {
            if (string.IsNullOrEmpty(content.Id))
            {
                content.Id = _idGenerator.GenerateId();
            }
        }

        private DirectoryInfo GetContextStorage<T>(string context) where T : class
        {
            if (context == null)
            {
                context = string.Empty;
            }
             var typeStoragePath = CreatePath(_baseLocation, typeof(T).Name);
            var contextPath  =  CreatePath(typeStoragePath.FullName, context.ToLower());
            return contextPath;
        }
    
        private DirectoryInfo CreatePath(string basePath, string subPath)
        {
            var storagePath = Path.Combine(basePath, subPath);
            var dirInfo = Directory.CreateDirectory(storagePath);
            return dirInfo;
        }
    }
}