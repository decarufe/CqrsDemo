using System.Configuration;
using System.Linq;
using log4net;
using MailChimp;
using MailChimp.Helper;
using MailChimp.Lists;
using Pyxis.Messaging.Email;

namespace Pyxis.Messaging.Mailchimp
{
    public class SubscriptionManager : ISubscriptionManager
    {
        private readonly MailChimpManager _mailChimpManager;
        private readonly ILog _logger = LogManager.GetLogger(typeof (SubscriptionManager));

        public SubscriptionManager(string apiKey)
        {
            _logger.DebugFormat("Will be using api {0}", apiKey);
            _mailChimpManager =  new MailChimpManager(apiKey);
        }

        public void SubscribeToList(string listName, string email)
        {
            _logger.DebugFormat("Subscribing email {0} to list {1}", email, listName);
            var listId = GetGlobalListId(listName);
            _logger.DebugFormat("List id is {0} ", listId);
            _mailChimpManager.Subscribe(listId, new EmailParameter { Email = email }, updateExisting: true, doubleOptIn: false);
            _logger.Debug("Done!");
        }

        private string GetGlobalListId(string listName)
        {
            var lists = _mailChimpManager.GetLists(new ListFilter { ListName = listName });
            if (!lists.Data.Any())
            { 
                _logger.DebugFormat("Cannot find list with name \"{0}\"", listName);
                throw new ConfigurationErrorsException(string.Format("Please configure the {0} list in MailChimp!", listName));
            }
            var id = lists.Data.First().Id;
            return id;
        }
    }
}
