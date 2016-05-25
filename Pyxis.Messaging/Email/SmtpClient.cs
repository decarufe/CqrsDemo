using System;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Pyxis.Messaging.Email
{
    public class SmtpClient : BaseEmailClient, IDisposable
    {
        private readonly string _host;
        private readonly int _port;
        private readonly string _user;
        private readonly string _password;
        private System.Net.Mail.SmtpClient _client;

        public SmtpClient(string host, int port, string user, string password)
        {
            _host = host;
            _port = port;
            _user = user;
            _password = password;
        }

        private void Connect()
        {
            if (_client == null)
            {
                _client = new System.Net.Mail.SmtpClient
                {
                    Host = _host,
                    Port = _port,
                    Credentials = new System.Net.NetworkCredential(_user, _password)
                };
            }
        }
        public override async Task SendMessageAsync(string from, string to, string title, string body, bool html = true)
        {
            Connect();
            // convert IdentityMessage to a MailMessage
            var email =
                new MailMessage(new MailAddress(from),
                    new MailAddress(to))
                {
                    Subject = title,
                    Body = body,
                    IsBodyHtml = html
                };

            await _client.SendMailAsync(email);
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
