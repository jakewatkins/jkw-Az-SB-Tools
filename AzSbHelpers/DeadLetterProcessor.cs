// /*
// 	DeadLetterProcessor.cs
// 	jakewatkins
// 	5/18/2020
//
//
// */
using System;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using System.Text;

namespace AzSbHelpers
{
    public class DeadLetterProcessor
    {
        #region private fields
        private ServiceBusHelper _helper;
        private Configuration _config;
        private MessageReceiver _messageReceiver;
        #endregion

        #region Constructor
        public DeadLetterProcessor(Configuration configuration)
        {
            _config = configuration;
            _helper = new ServiceBusHelper(_config);
            _messageReceiver = _helper.GetDeadLetterReceiver();
        }
        #endregion

        #region private implementation
        Task LogMessageHandlerException(ExceptionReceivedEventArgs e)
        {
            Console.WriteLine("Exception: \"{0}\" {0}", e.Exception.Message, e.ExceptionReceivedContext.EntityPath);
            return Task.CompletedTask;
        }
        private void SaveMessage(string messageBody)
        {
            string fileName = string.Format("message_{0}_{1}.json", _config.Topic, DateTime.Now.ToString("yyyyMMddhhmmss"));
            File.WriteAllText(fileName, messageBody);
        }
        private Task MessageHandler(Message message, CancellationToken cancellationToken)
        {
            var doneReceiving = new TaskCompletionSource<bool>();

            SaveMessage(Encoding.UTF8.GetString(message.Body));

            return doneReceiving.Task;
        }
        private async void ProcessMessages()
        {
            try
            {
                /**/
                MessageReceiver receiver = _helper.GetTopicDeadLetterClient();

                bool queueHasMessages = true;
                while (queueHasMessages)
                {
                    Task<Message> task = receiver.ReceiveAsync();
                    task.Wait();

                    Message message = task.Result;
                    SaveMessage(Encoding.UTF8.GetString(message.Body));
                }
            }
            catch (Exception ex)
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendFormat("error: {0}\n", ex.Message);
                if(null != ex.InnerException)
                {
                    builder.AppendFormat("error: {0}\n", ex.InnerException.Message);
                }
                Console.Write(builder.ToString());
            }
        }
        private async void ProcessDeadLetterTopic()
        {

        }
        #endregion

        #region public interface
        public void ProcessDeadLetters()
        {
            //ProcessMessages();
            TopicClient topicClient = _helper.GetTopicClient();
            string topicPath = topicClient.Path;
            Console.WriteLine(topicClient);
        }
        #endregion
    }
}
