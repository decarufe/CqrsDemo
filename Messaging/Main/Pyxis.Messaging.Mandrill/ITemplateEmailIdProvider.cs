using System.Globalization;

namespace Pyxis.Messaging.Mandrill
{
    public interface ITemplateEmailIdProvider
    {
        string GetTemplateNameFor(string situation, CultureInfo culture);
    }
}
