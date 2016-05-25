using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Pyxis.Core.Id;
using Pyxis.Persistance.Serialization;

namespace Pyxis.Persistance
{
    public class MemoryPersistance : IPersistanceStore
    {
        private readonly IDictionary<string, IDictionary<Type, IDictionary<string, object>>> _allEntities =
            new ConcurrentDictionary<string, IDictionary<Type, IDictionary<string, object>>>();

        private readonly IIdGenerator _idGenerator;

        public MemoryPersistance() : this(new GuidIdGenerator())
        {
        }
        public MemoryPersistance(IIdGenerator idGenerator)
        {
            _idGenerator = idGenerator;
        }

        public void Save<T>(T content, string context = "default") where T : class, IIdentifiable
        {
            SetContentId(content);
            var storage = GetTypeStorage<T>(context);
            var internalId = GetInternalId(content.Id);
            storage[internalId] = DeepCopy(content);
        }

        public void Save<T>(IEnumerable<T> items, string context = "default") where T : class, IIdentifiable
        {
            var storage = GetTypeStorage<T>(context);
            foreach (var item in items.Where(x => x != null).ToArray())
            {
                SetContentId(item);
                var internalId = GetInternalId(item.Id);
                storage[internalId] = DeepCopy(item);
            }
        }

        private static T DeepCopy<T>(T objectToCopy) where T : class
        {
            var serializer = SerializerFactory.GetSerializer(objectToCopy);
            return serializer.Deserialize<T>(serializer.Serialize(objectToCopy));
        }

        public T Get<T>(string key, string context = "default") where T : class, IIdentifiable
        {
            var storage = GetTypeStorage<T>(context);
            key = GetInternalId(key);
            return storage.ContainsKey(key) ? storage[key] as T : default(T);
        }

        public void Delete<T>(string[] keys, string context = "default") where T : class, IIdentifiable
        {
            var storage = GetTypeStorage<T>(context);
            foreach (var key in keys)
            {
                var internalKey = GetInternalId(key);
                storage.Remove(internalKey);
            }
        }

        public void Delete<T>(string key, string context = "default") where T : class, IIdentifiable
        {
            Delete<T>(new []{key}, context);
        }

        public bool Any<T>(string context = "default") where T : class, IIdentifiable
        {
            var contextStore = GetContextStore(context);
            return contextStore.ContainsKey(typeof(T));
        }

        public void Purge<T>(string context = "default") where T : class, IIdentifiable
        {
            var contextStore = GetContextStore(context);
            contextStore.Remove(typeof(T));
        }

        public void Purge(string context = "default")
        {
            var internalContext = GetInternalContextName(context);
            _allEntities.Remove(internalContext);
        }

        public IEnumerable<T> GetAll<T>(string context = "default") where T : class, IIdentifiable
        {
            var storage = GetTypeStorage<T>(context);
            return storage.Values.ToArray().Cast<T>();
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

        private IDictionary<string, object> GetTypeStorage<T>(string context) where T : class
        {
            var contextStore = GetContextStore(context);
            var type = typeof (T);
            if (!contextStore.ContainsKey(type))
            {
                contextStore[type] = new ConcurrentDictionary<string, object>();
            }
            return contextStore[type];
        }

        private IDictionary<Type, IDictionary<string, object>> GetContextStore(string context)
        {
            var internalContext = GetInternalContextName(context);
            if (!_allEntities.ContainsKey(internalContext))
            {
                _allEntities[internalContext] = new ConcurrentDictionary<Type, IDictionary<string, object>>();
            }
            return _allEntities[internalContext];
        }

        private static string GetInternalContextName(string context)
        {
            if (context == null) context = string.Empty;
            var internalContext = context.ToLower();
            return internalContext;
        }

        private string GetInternalId(string id)
        {
            return id != null ? id.ToLowerInvariant() : string.Empty;
        }
    }
}