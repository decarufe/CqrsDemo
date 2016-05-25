using System;
using System.Linq;
using NUnit.Framework;
using Pyxis.Persistance.Query;

namespace Pyxis.Persistance.Tests.Query
{
    public abstract class BaseQueryTests
    {
        protected IPersistanceStore PersistanceStore;
        protected IPersistanceQuery QueryStore;

        [Test]
        public void TestNothingWhenNotFound()
        {
            var query = QueryElement.FieldEquals("Version", 1);
            var result = QueryStore.Query<QueryableTestObject>(query);
            Assert.AreEqual(0, result.Count());
        }

        [Test]
        public void TestAsInt()
        {
            PersistanceStore.Save(new QueryableTestObject {IntVersion = 1});
            var query = QueryElement.FieldEquals("IntVersion", 1);
            var result = QueryStore.Query<QueryableTestObject>(query);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(1, result.First().IntVersion);

            query = QueryElement.FieldCompare("IntVersion",FieldComparison.Greater,  0);
            result = QueryStore.Query<QueryableTestObject>(query);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(1, result.First().IntVersion);

            query = QueryElement.FieldCompare("IntVersion", FieldComparison.GreaterEquals, 1);
            result = QueryStore.Query<QueryableTestObject>(query);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(1, result.First().IntVersion);


            query = QueryElement.FieldCompare("IntVersion", FieldComparison.Less, 2);
            result = QueryStore.Query<QueryableTestObject>(query);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(1, result.First().IntVersion);

            query = QueryElement.FieldCompare("IntVersion", FieldComparison.LessOrEquals, 1);
            result = QueryStore.Query<QueryableTestObject>(query);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(1, result.First().IntVersion);

            query = QueryElement.FieldCompare("IntVersion", FieldComparison.Different, 1);
            result = QueryStore.Query<QueryableTestObject>(query);
            Assert.AreEqual(0, result.Count());
            
            query = QueryElement.FieldCompare("IntVersion", FieldComparison.Different, 0);
            result = QueryStore.Query<QueryableTestObject>(query);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(1, result.First().IntVersion);
        }
        [Test]
        public void TestAsLong()
        {
            PersistanceStore.Save(new QueryableTestObject { LongVersion = 1 });
            var query = QueryElement.FieldEquals("LongVersion", 1);
            var result = QueryStore.Query<QueryableTestObject>(query);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(1, result.First().LongVersion);

            query = QueryElement.FieldCompare("LongVersion", FieldComparison.Greater, 0);
            result = QueryStore.Query<QueryableTestObject>(query);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(1, result.First().LongVersion);

            query = QueryElement.FieldCompare("LongVersion", FieldComparison.GreaterEquals, 1);
            result = QueryStore.Query<QueryableTestObject>(query);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(1, result.First().LongVersion);

            query = QueryElement.FieldCompare("LongVersion", FieldComparison.Less, 2);
            result = QueryStore.Query<QueryableTestObject>(query);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(1, result.First().LongVersion);

            query = QueryElement.FieldCompare("LongVersion", FieldComparison.LessOrEquals, 1);
            result = QueryStore.Query<QueryableTestObject>(query);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(1, result.First().LongVersion);

            query = QueryElement.FieldCompare("LongVersion", FieldComparison.Different, 1);
            result = QueryStore.Query<QueryableTestObject>(query);
            Assert.AreEqual(0, result.Count());

            query = QueryElement.FieldCompare("LongVersion", FieldComparison.Different, 0);
            result = QueryStore.Query<QueryableTestObject>(query);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(1, result.First().LongVersion);
        }
        [Test]
        public void TestAsDouble()
        {
            PersistanceStore.Save(new QueryableTestObject { DoubleVersion = 1.123 });
            var query = QueryElement.FieldEquals("DoubleVersion", 1.123d);
            var result = QueryStore.Query<QueryableTestObject>(query);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(1.123, result.First().DoubleVersion);

            query = QueryElement.FieldCompare("DoubleVersion", FieldComparison.Greater, 1.0d);
            result = QueryStore.Query<QueryableTestObject>(query);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(1.123, result.First().DoubleVersion);

            query = QueryElement.FieldCompare("DoubleVersion", FieldComparison.GreaterEquals, 1.123d);
            result = QueryStore.Query<QueryableTestObject>(query);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(1.123, result.First().DoubleVersion);

            query = QueryElement.FieldCompare("DoubleVersion", FieldComparison.Less, 2d);
            result = QueryStore.Query<QueryableTestObject>(query);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(1.123, result.First().DoubleVersion);

            query = QueryElement.FieldCompare("DoubleVersion", FieldComparison.LessOrEquals, 1.123d);
            result = QueryStore.Query<QueryableTestObject>(query);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(1.123, result.First().DoubleVersion);

            query = QueryElement.FieldCompare("DoubleVersion", FieldComparison.Different, 1.123d);
            result = QueryStore.Query<QueryableTestObject>(query);
            Assert.AreEqual(0, result.Count());

            query = QueryElement.FieldCompare("DoubleVersion", FieldComparison.Different, 0);
            result = QueryStore.Query<QueryableTestObject>(query);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(1.123, result.First().DoubleVersion);
        }
        [Test]
        public void TestAsString()
        {
            PersistanceStore.Save(new QueryableTestObject { StringVersion = "1" });
            var query = QueryElement.FieldEquals("StringVersion", "1");
            var result = QueryStore.Query<QueryableTestObject>(query);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("1", result.First().StringVersion);

            query = QueryElement.FieldCompare("StringVersion", FieldComparison.Greater, "0");
            result = QueryStore.Query<QueryableTestObject>(query);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("1", result.First().StringVersion);

            query = QueryElement.FieldCompare("StringVersion", FieldComparison.GreaterEquals, "1");
            result = QueryStore.Query<QueryableTestObject>(query);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("1", result.First().StringVersion);

            query = QueryElement.FieldCompare("StringVersion", FieldComparison.Less, "2");
            result = QueryStore.Query<QueryableTestObject>(query);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("1", result.First().StringVersion);

            query = QueryElement.FieldCompare("StringVersion", FieldComparison.LessOrEquals, "1");
            result = QueryStore.Query<QueryableTestObject>(query);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("1", result.First().StringVersion);

            query = QueryElement.FieldCompare("StringVersion", FieldComparison.Different, "1");
            result = QueryStore.Query<QueryableTestObject>(query);
            Assert.AreEqual(0, result.Count());

            query = QueryElement.FieldCompare("StringVersion", FieldComparison.Different, "0");
            result = QueryStore.Query<QueryableTestObject>(query);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("1", result.First().StringVersion);
        }

        [Test]
        public void TestAsDate()
        {
            var toSave = new QueryableTestObject {TimeStamp = DateTime.Today};
            PersistanceStore.Save(toSave);
            var query = QueryElement.FieldEquals("TimeStamp", DateTime.Today);
            var result = QueryStore.Query<QueryableTestObject>(query);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(DateTime.Today, result.First().TimeStamp);

            query = QueryElement.FieldCompare("TimeStamp", FieldComparison.Greater, DateTime.Today.AddMinutes(-1));
            result = QueryStore.Query<QueryableTestObject>(query);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(DateTime.Today, result.First().TimeStamp);

            query = QueryElement.FieldCompare("TimeStamp", FieldComparison.GreaterEquals, DateTime.Today);
            result = QueryStore.Query<QueryableTestObject>(query);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(DateTime.Today, result.First().TimeStamp);

            query = QueryElement.FieldCompare("TimeStamp", FieldComparison.Less, DateTime.Today.AddMinutes(1));
            result = QueryStore.Query<QueryableTestObject>(query);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(DateTime.Today, result.First().TimeStamp);

            query = QueryElement.FieldCompare("TimeStamp", FieldComparison.LessOrEquals, DateTime.Today);
            result = QueryStore.Query<QueryableTestObject>(query);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(DateTime.Today, result.First().TimeStamp);

            query = QueryElement.FieldCompare("TimeStamp", FieldComparison.Different, DateTime.Today);
            result = QueryStore.Query<QueryableTestObject>(query);
            Assert.AreEqual(0, result.Count());

            query = QueryElement.FieldCompare("TimeStamp", FieldComparison.Different, DateTime.Today.AddMinutes(-1));
            result = QueryStore.Query<QueryableTestObject>(query);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(DateTime.Today, result.First().TimeStamp);
        }
    }
}
