using System.Threading.Tasks;
using Mandrill;
using Mandrill.Model;
using Pyxis.Messaging.Email;

namespace Pyxis.Messaging.Mandrill
{
    public class MandrillApiClient : BaseEmailClient
    {
        private readonly MandrillApi _mandrillConnector;

        public MandrillApiClient(string apiKey)
        {
            _mandrillConnector = new MandrillApi(apiKey);
        }
        public override Task SendMessageAsync(string @from, string to, string title, string body, bool html = true)
        {
            var message = new MandrillMessage(@from, to, title, body) {TrackClicks = true, TrackOpens = true};
            return _mandrillConnector.Messages.SendAsync(message);
        }
    }
}
