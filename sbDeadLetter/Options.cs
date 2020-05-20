using System;
using CommandLine;

namespace sbDeadLetter
{
    public class Options
    {
        public Options()
        {
        }

        [Option('c', "connection", Required = true, HelpText = "path to file containing the connection string")]
        public string ConnectionPath { get; set; }
        [Option('t', "topic", Required = true, HelpText = "name of the topic for the message")]
        public string Topic { get; set; }
        [Option('s', "subscription", Required =true, HelpText = "subscription name")]
        public string Subscription { get; set; }
        [Option('o', "output", Required = true, HelpText = "output directory path for messages")]
        public string OutputPath { get; set; }
        
    }
}
