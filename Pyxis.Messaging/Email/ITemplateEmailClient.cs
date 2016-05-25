using System.Collections.Generic;

namespace Pyxis.Messaging.Email
{
    public interface ITemplateEmailClient
    {
        void SendEmail(string to, string templateName, IDictionary<string, string> variables = null);
        void SendEmail(string to, string bcc, string templateName, IDictionary<string, string> variables = null);
    }
}
