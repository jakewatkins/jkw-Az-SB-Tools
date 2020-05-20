using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace sbSender
{
    public class MessageSender
    {
        #region Private fields
        private Configuration _config;
        private TopicClient _topicClient;
        #endregion

        #region Constructors
        public MessageSender(Configuration configuration)
        {
            _config = configuration;
        }
        #endregion

        #region Private implementation
        private void ConnectToServiceBus()
        {
            _topicClient = new TopicClient(_config.Connection, _config.Topic);
        }
        #endregion

        #region Public interface
        public async void SendMessage()
        {
            try
            {

                ConnectToServiceBus();
                var message = new Message(Encoding.UTF8.GetBytes(_config.Message));

                Task result = _topicClient.SendAsync(message);
                result.Wait();
                await _topicClient.CloseAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine("error: {0}", ex.Message);
                if(null != ex.InnerException)
                {
                    Console.WriteLine("error: {0}", ex.InnerException.Message);
                }
            }
        }
        #endregion

    }
}
