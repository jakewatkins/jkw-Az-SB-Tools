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
using System.Collections.Generic;

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
        private void SaveMessage(string messageId, string messageBody)
        {
            string fileName = string.Format("{0}/message_{1}_{2}.json", _config.OutputPath, messageId, DateTime.Now.ToString("yyyyMMddhhmmss"));
            File.WriteAllText(fileName, messageBody);
        }
        private Task MessageHandler(Message message, CancellationToken cancellationToken)
        {
            var doneReceiving = new TaskCompletionSource<bool>();

            SaveMessage(message.MessageId, Encoding.UTF8.GetString(message.Body));

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
                    Task<IList<Message>> task = receiver.ReceiveAsync(1000);
                    task.Wait();

                    if (null != task.Result && 0 != task.Result.Count)
                    {
                        IList<Message> messages = task.Result;
                        foreach(Message message in messages)
                        {
                            SaveMessage(message.MessageId, Encoding.UTF8.GetString(message.Body));
                        }
                        
                    }
                    else
                    {
                        queueHasMessages = false;
                    }
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
        #endregion

        #region public interface
        public delegate bool HandleDeadLetter(Message message);

        public void ProcessDeadLetters()
        {
            ProcessMessages();
        }
        public void ProcessDeadLetters(HandleDeadLetter handleDeadLetter)
        {
            try
            {
                /**/
                MessageReceiver receiver = _helper.GetTopicDeadLetterClient();

                bool queueHasMessages = true;
                while (queueHasMessages)
                {
                    Task<IList<Message>> task = receiver.ReceiveAsync(1000);
                    task.Wait();

                    if (null != task.Result && 0 != task.Result.Count)
                    {
                        IList<Message> messages = task.Result;
                        foreach (Message message in messages)
                        {
                            SaveMessage(message.MessageId, Encoding.UTF8.GetString(message.Body));
                            if(true == handleDeadLetter(message))
                            {
                                receiver.CompleteAsync(message.SystemProperties.LockToken);                                
                            }
                        }

                    }
                    else
                    {
                        queueHasMessages = false;
                    }
                }
            }
            catch (Exception ex)
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendFormat("error: {0}\n", ex.Message);
                if (null != ex.InnerException)
                {
                    builder.AppendFormat("error: {0}\n", ex.InnerException.Message);
                }
                Console.Write(builder.ToString());
            }
        }
        #endregion
    }
}
