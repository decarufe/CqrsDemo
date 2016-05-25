using System.Configuration;
using System.Threading.Tasks;
using NUnit.Framework;
using Pyxis.Messaging.Email;


namespace Pyxis.Messaging.Tests.Email
{
    [TestFixture]
    public class BaseEmailSenderTests
    {
        private TestSender _sender;

        [SetUp]
        public void SetUp()
        {
            _sender = new TestSender();
            ConfigurationManager.AppSettings["smtpFrom"] = "test";
        }

        [Test]
        public void TestSendingWithFromKeepsIt()
        {
            _sender.SendMessage("myfrom", "to", "title","buddy", false);
            Assert.AreEqual("myfrom", _sender.From);
        }
    }

    class TestSender : BaseEmailClient
    {
        public string From;
        public override Task SendMessageAsync(string @from, string to, string title, string body, bool html = true)
        {
            From = from;
            var task =  new Task(delegate {  });
            task.RunSynchronously();
            return task;
        }
    }
}
