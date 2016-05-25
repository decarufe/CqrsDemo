using System.Collections.Generic;
using System.Linq;
using Mandrill;
using Mandrill.Model;
using Pyxis.Messaging.Email;

namespace Pyxis.Messaging.Mandrill
{
    public class TemplateEmailClient : ITemplateEmailClient
    {
        private readonly MandrillApi _mandrillConnector;

        public TemplateEmailClient(string apiKey)
        {
            _mandrillConnector = new MandrillApi(apiKey);
        }
               
        public void SendEmail(string to, string templateName, IDictionary<string, string> variables = null)
        {
            SendEmail(to, string.Empty, templateName, variables);
        }

        public void SendEmail(string to, string bcc, string templateName, IDictionary<string, string> variables = null)
        {
            var content = new List<MandrillTemplateContent> { new MandrillTemplateContent { Name = templateName } };
            var message = new MandrillMessage { TrackClicks = true, TrackOpens = true };
            message.AddTo(to);
            if (!string.IsNullOrEmpty(bcc))
            {
                message.BccAddress = bcc;
            }
            if (variables != null)
            {
                foreach (var variable in variables.Keys.ToArray())
                {
                    message.AddGlobalMergeVars(variable, variables[variable]);
                }
            }
            _mandrillConnector.Messages.SendTemplateAsync(message, templateName, content);
        }
    }
}
