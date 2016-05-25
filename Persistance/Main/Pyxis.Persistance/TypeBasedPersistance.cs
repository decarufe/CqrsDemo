using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using Pyxis.Core.Id;
using Pyxis.Persistance.Query;

namespace Pyxis.Persistance
{
    public class TypeBasedPersistance : IPersistanceStore, IPersistanceQuery
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(TypeBasedPersistance));
        private readonly IIdGenerator _idGenerator;
        private readonly IStoreSelector _storeSelector;

        public TypeBasedPersistance(IStoreSelector storeSelector, IIdGenerator idGenerator = null)
        {
            _idGenerator = idGenerator ?? new GuidIdGenerator();
            _storeSelector = storeSelector;
        }

        public void Save<T>(T content, string context = "default") where T : class, IIdentifiable
        {
            SafeWrap(()=>
            {
                _logger.DebugFormat("Saving {0} with id {1} in context {2}", typeof (T).Name, content.Id, context);
                var persistanceStore = _storeSelector.GetStoreFor<T>();
                EnsureIdExists(content);
                persistanceStore.Save(content, context);
            });
        }

        private void EnsureIdExists<T>(params T[] contents) where T : IIdentifiable
        {
            foreach (var content in contents)
            {
                if (string.IsNullOrWhiteSpace(content.Id))
                {
                    content.Id = _idGenerator.GenerateId();
                }
            }
        }

        public void Save<T>(IEnumerable<T> content, string context = "default") where T : class, IIdentifiable
        {
            SafeWrap(() =>
            {
                var persistanceStore = _storeSelector.GetStoreFor<T>();
                var fixedContent = content.ToArray();
                EnsureIdExists(fixedContent);
                persistanceStore.Save<T>(fixedContent, context);
            });
        }

        public T Get<T>(string key, string context = "default") where T : class, IIdentifiable
        {
            var persistanceStore = _storeSelector.GetStoreFor<T>();
            return persistanceStore.Get<T>(key, context);
        }


        public void Delete<T>(string[] keys, string context = "default") where T : class, IIdentifiable
        {
            SafeWrap(() =>
            {
                var persistanceStore = _storeSelector.GetStoreFor<T>();
                persistanceStore.Delete<T>(keys, context);
            });
        }

        public void Delete<T>(string key, string context = "default") where T : class, IIdentifiable
        {
            SafeWrap(() =>
            {
                Delete<T>(new[] {key}, context);
            });
        }

        public bool Any<T>(string context = "default") where T : class, IIdentifiable
        {
            var persistanceStore = _storeSelector.GetStoreFor<T>();
            return persistanceStore.Any<T>(context);
        }

        public void Purge<T>(string context = "default") where T : class, IIdentifiable
        {
            SafeWrap(() =>
            {
                var persistanceStore = _storeSelector.GetStoreFor<T>();
                persistanceStore.Purge<T>(context);
            });
        }

        public void Purge(string context = "default")
        {
            SafeWrap(() =>
            {
                var stores = _storeSelector.GetAllStores();
                Parallel.ForEach(stores, store =>
                {
                    store.Purge(context);
                });
            });
        }

        public IEnumerable<T> GetAll<T>(string context = "default") where T : class, IIdentifiable
        {
            var persistanceStore = _storeSelector.GetStoreFor<T>();
            return persistanceStore.GetAll<T>(context);
        }

        public IEnumerable<T> GetSome<T>(int count, string context = "default") where T : class, IIdentifiable
        {
            var persistanceStore = _storeSelector.GetStoreFor<T>();
            return persistanceStore.GetSome<T>(count, context);
        }

        public IEnumerable<T> Query<T>(QueryElement condition, int maxResults = int.MaxValue, string context = "default") where T : class, IIdentifiable
        {
            return Query<T>(new [] {condition}, maxResults, context);
        }

        public IEnumerable<T> Query<T>(IEnumerable<QueryElement> conditions, int maxResults = int.MaxValue, string context = "default") where T : class, IIdentifiable
        {
            var persistanceStore = _storeSelector.GetStoreFor<T>();

            if (persistanceStore is IPersistanceQuery)
            {
                return ((IPersistanceQuery)persistanceStore).Query<T>(conditions, maxResults, context);
            }
            throw new NotSupportedException(); 
        }

        private T SafeReturn<T>(Action<T> method)
        {
            return (T) SafeWrap(method as Action);
        }
        private object SafeWrap(Action method)
        {
            try
            {
                _logger.DebugFormat("Invoking {0}", method.Method.Name);
                var result= method.DynamicInvoke();
                _logger.DebugFormat("Done Invoking {0}", method.Method.Name);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error("Error invoking persistance: " , ex);
                throw;
            }
        }
    }
}