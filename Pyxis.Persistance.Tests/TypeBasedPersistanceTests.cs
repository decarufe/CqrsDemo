using System;
using System.Collections.Generic;
using NUnit.Framework;
using Pyxis.Core.Id;
using Pyxis.Persistance.Container;
using Pyxis.Persistance.Query;
using FluentAssertions;

namespace Pyxis.Persistance.Tests
{
    [TestFixture]
    public class TypeBasedPersistanceTests : BasePersistanceTests
    {
        [SetUp]
        public void SetUp()
        {
            PersistanceStore = new TypeBasedPersistance(new TestStoreSelector(new MemoryPersistance()));
        }

        [Test]
        public void TestQueryingNonQueriableStoreThrows()
        {
            var queriable = (TypeBasedPersistance) PersistanceStore;
            Assert.That(() => queriable.Query<PersistedString>(new[] {QueryElement.FieldEquals("id", "1")}), 
                Throws.TypeOf<NotSupportedException>());
        }

        [Test]
        public void TestQueryingQueriableStoreProxies()
        {
            var query = new QueryPersistance();
            PersistanceStore = new TypeBasedPersistance(new TestStoreSelector(query));
            var queriable = (TypeBasedPersistance)PersistanceStore;
            Assert.IsFalse(query.Queried);
            queriable.Query<PersistedString>(new[] { QueryElement.FieldEquals("id", "1") });
            Assert.IsTrue(query.Queried);
        }

        [Test]
        public void TestIdGeneratorCanBeSupplied()
        {
            var query = new QueryPersistance();
            PersistanceStore = new TypeBasedPersistance(new TestStoreSelector(query), new TestGenerator());
            var toSave = new PersistedString();
            PersistanceStore.Save(toSave);
            Assert.AreEqual("123", toSave.Id);
        }

    }
  
    class TestStoreSelector : IStoreSelector
    {
        private readonly IPersistanceStore _persistanceStore;

        public TestStoreSelector(IPersistanceStore store)
        {
            _persistanceStore = store;
        }

        public IPersistanceStore GetStoreFor<T>()
        {
            return _persistanceStore;
        }

        public IEnumerable<IPersistanceStore> GetAllStores()
        {
            return new[] { _persistanceStore };
        }
    }

    class QueryPersistance : MemoryPersistance, IPersistanceQuery
    {
        public bool Queried;

        public IEnumerable<T> Query<T>(QueryElement condition, int maxResults = int.MaxValue, string context = "default") where T : class, IIdentifiable
        {
            return Query<T>(new [] {condition}, maxResults, context);
        }

        public IEnumerable<T> Query<T>(IEnumerable<QueryElement> conditions, int maxResults, string context) where T : class, IIdentifiable
        {
            Queried = true;
            return new T[0];
        }
    }

    public class TestGenerator : IIdGenerator
    {
        public string GenerateId(string seed = "")
        {
            return "123";
        }

    }
    
}
