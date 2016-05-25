using System.Threading.Tasks;

namespace Pyxis.Messaging.Email
{
    public abstract class BaseEmailClient : IEmailClient
    {
        public void SendMessage(string @from, string to, string title, string body, bool html = true)
        {
            Task.Run(() => SendMessageAsync(from, to, title, body, html)).Wait();
        }

        public abstract Task SendMessageAsync(string @from, string to, string title, string body, bool html = true);
    }
}
