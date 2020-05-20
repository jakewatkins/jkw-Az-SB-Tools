using System;
using System.IO;
using AzSbHelpers;
using CommandLine;

namespace sbDeadLetter
{
    class Program
    {

        static Configuration LoadConfiguration(Options options)
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

        static int Main(string[] args)
        {
            bool error = false;
            Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(o =>
            {
                try
                {
                    Configuration config = LoadConfiguration(o);
                    DeadLetterProcessor deadLetterProcessor = new DeadLetterProcessor(config);
                    deadLetterProcessor.ProcessDeadLetters();
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
    }
}
