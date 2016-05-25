using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Pyxis.Persistance.Container;
using Pyxis.Persistance.Filesystem;

namespace Pyxis.Persistance.Tests
{
    [TestFixture]
    public class FilePersistanceTests : BasePersistanceTests
    {
        [SetUp]
        public void SetUp()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            PersistanceStore = new FilePersistance(tempDirectory);
            PersistanceStore.Purge();
        }

        [Test]
        public void TestPathIsAutoBuiltWhenNotSupplied()
        {
            PersistanceStore = new FilePersistance();
            PersistanceStore.Purge();
            PersistanceStore.Save(new PersistedString {Content = "Test"});
            var saved = PersistanceStore.GetAll<PersistedString>();
            Assert.AreEqual(1, saved.Count());
            Assert.AreEqual("Test", saved.First().Content);
        }
        [Test]
        public void TestReadIsThreadSafe()
        {
            var items = new PersistedString[100];
            try
            {
                for (var count = 0; count < items.Length; count++)
                {
                    items[count] = new PersistedString { Content = Guid.NewGuid().ToString() };
                }
                PersistanceStore.Save<PersistedString>(items);
                Parallel.ForEach(items, item =>
                {
                    PersistanceStore.GetAll<PersistedString>();
                });
            }
            finally
            {
                for (var count = 0; count < items.Length; count++)
                {
                    PersistanceStore.Delete<PersistedString>(items[count].Id);
                }
            }
        }
        [Test]
        public void TestDeleteIsThreadSafe()
        {
            var baseLocation = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var tempDirectory = Path.Combine(baseLocation, "PersistedString", "default");

            Directory.CreateDirectory(tempDirectory);
            PersistanceStore = new FilePersistance(baseLocation);
            var fileInUse = File.Open(Path.Combine(tempDirectory,"1"), FileMode.CreateNew, FileAccess.Write);
            Assert.IsTrue(PersistanceStore.Any<PersistedString>());
            PersistanceStore.Delete<PersistedString>("1");
            fileInUse.Close();
            fileInUse.Dispose();
            Thread.Sleep(100);
            // Wait for deletion
            Assert.IsFalse(PersistanceStore.Any<PersistedString>());
        }
    }
}
