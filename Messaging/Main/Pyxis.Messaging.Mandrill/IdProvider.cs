using System;
using System.Globalization;
using System.Linq;
using Mandrill;

namespace Pyxis.Messaging.Mandrill
{
    public class IdProvider : ITemplateEmailIdProvider
    {
        private readonly string _apiKey;

        public IdProvider(string apiKey)
        {
            _apiKey = apiKey;
        }

        public string GetTemplateNameFor(string situation, CultureInfo culture)
        {
            var localizedTemplateName = string.Format("{0}-{1}", situation, culture.TwoLetterISOLanguageName).ToLower();
            return GetTemplateName(localizedTemplateName, situation);
        }

        private string GetTemplateName(params string[] templateNames)
        {
            var mandrill = new MandrillApi(_apiKey);
            var templates = mandrill.Templates.List();

            foreach (var templateName in templateNames)
            {
                if (templates.Any(x => x.Name.ToLower() == templateName.ToLower()))
                    return templateName;
            }

            throw new ArgumentException("No mandrill template was found");
        }
    }
}
