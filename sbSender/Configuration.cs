using System;

namespace sbSender
{
    public class Configuration
    {
        public Configuration()
        {
        }

        public string Connection { get; set; }
        public string Topic { get; set; }
        public string Message { get; set; }
    }
}
