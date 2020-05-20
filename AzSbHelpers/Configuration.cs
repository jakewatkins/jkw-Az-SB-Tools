using System;

namespace AzSbHelpers
{
    public class Configuration
    {
        public Configuration()
        {
        }

        public string Connection { get; set; }
        public string Topic { get; set; }
        public string Message { get; set; }
        public string OutputPath { get; set; }
        public string Subscription { get; set; }
    }
}
