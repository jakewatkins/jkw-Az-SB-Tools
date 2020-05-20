using System;
using System.IO;

using CommandLine;

namespace sbSender
{
    class Program
    {

        static Configuration LoadConfiguration(Options options)
        {
            Configuration configuration = new Configuration();

            if(true == File.Exists(options.ConnectionPath))
            {
                configuration.Connection = File.ReadAllText(options.ConnectionPath);
            }

            if(true == File.Exists(options.MessagePath))
            {
                configuration.Message = File.ReadAllText(options.MessagePath);
            }

            configuration.Topic = options.Topic;

            return configuration;
        }

        static int Main(string[] args)
        {
            bool error = false;
            Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(o =>
            {
                MessageSender sender = new MessageSender(LoadConfiguration(o));
                try
                {
                    sender.SendMessage();
                }
                catch(Exception e)
                {
                    Console.WriteLine("Error: {0}", e.Message);
                    if(null != e.InnerException)
                    {
                        Console.WriteLine("Inner error: {0}", e.InnerException.Message);
                        error = true;
                    }

                }
            });
            if(true == error)
            {
                return 1;
            }
            return 0;
        }
    }
}
