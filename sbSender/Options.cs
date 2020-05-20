using System;
using CommandLine;

namespace sbSender
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
        [Option('m', "message", Required = true, HelpText = "path to file containing the message")]
        public string MessagePath { get; set; }
    }
}
