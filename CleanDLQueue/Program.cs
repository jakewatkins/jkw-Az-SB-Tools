using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using AzSbHelpers;
using CommandLine;
using Microsoft.Azure.ServiceBus;

namespace CleanDLQueue
{
    class Program
    {
        #region private fields
        private Configuration _config;
        private ServiceBusHelper _helper;
        private TopicClient _topicClient;
        #endregion

        #region private implementation
        private Configuration LoadConfiguration(Options options)
        {
            Configuration configuration = new Configuration();

            if (true == File.Exists(options.ConnectionPath))
            {
                configuration.Connection = File.ReadAllText(options.ConnectionPath);
            }

            configuration.Topic = options.Topic;
            configuration.Subscription = options.Subscription;
            configuration.OutputPath = options.OutputPath;

            return configuration;
        }

        private bool ProcessDeadLetter(Message message)
        {
            try
            {
                var msg = new Message(message.Body);
                Task result = _topicClient.SendAsync(msg);
                result.Wait();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion


        public int main(string[] args)
        {
            bool error = false;
            Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(o =>
            {
                try
                {
                    _config = LoadConfiguration(o);
                    _helper = new ServiceBusHelper(_config);
                    _topicClient = _helper.GetTopicClient();

                    DeadLetterProcessor deadLetterProcessor = new DeadLetterProcessor(_config);

                    deadLetterProcessor.ProcessDeadLetters(this.ProcessDeadLetter);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: {0}", e.Message);
                    if (null != e.InnerException)
                    {
                        Console.WriteLine("Inner error: {0}", e.InnerException.Message);
                        error = true;
                    }
                }
            });
            if (true == error)
            {
                return 1;
            }
            return 0;
        }

        static int Main(string[] args)
        {
            Program program = new Program();
            return program.main(args);
        }
    }
}
