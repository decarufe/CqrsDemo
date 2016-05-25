using log4net;
using Pyxis.Messaging.Command;

namespace Pyxis.Messaging.Azure.Worker
{
    public class MessageListenerWorker : IWorker
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(MessageListenerWorker));
        private readonly IMessageListener _messageListener;

        public MessageListenerWorker(IMessageListener messageListener)
        {
            _messageListener = messageListener;
        }

        public bool Run()
        {
            _logger.Debug("Running");
            _messageListener.Listen();
            return true;
        }

        public bool OnStart()
        {
            _logger.Debug("On start");
            return true;
        }

        public bool OnStop()
        {
            _logger.Debug("On stop");
            _messageListener.Close();
            return true;
        }
    }
}
