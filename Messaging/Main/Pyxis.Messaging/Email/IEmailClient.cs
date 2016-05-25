using System.Threading.Tasks;

namespace Pyxis.Messaging.Email
{
    public interface IEmailClient
    {
        void SendMessage(string from, string to, string title, string body, bool html = true);
        Task SendMessageAsync(string from, string to, string title, string body, bool html = true);
    }
}
