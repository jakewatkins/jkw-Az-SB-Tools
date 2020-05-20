// /*
// 	ServiceBusHelper.cs
// 	jakewatkins
// 	5/18/2020
//
//
// */
using System;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;

namespace AzSbHelpers
{
    public class ServiceBusHelper
    {
        #region Private fields
        private Configuration _config;
        #endregion

        #region Constructors
        private ServiceBusHelper()
        {
        }
        public ServiceBusHelper(Configuration configuration) 
        {
            _config = configuration;
        }
        #endregion

        #region Private implementation
        #endregion

        #region Public interface
        public TopicClient  GetTopicClient()
        {
             return new TopicClient(_config.Connection, _config.Topic);
        }
        public MessageReceiver GetDeadLetterReceiver()
        {
            return new MessageReceiver(_config.Connection, EntityNameHelper.FormatDeadLetterPath(_config.Topic), ReceiveMode.PeekLock);
        }
        public MessageReceiver GetTopicDeadLetterClient()
        {
            string deadLetterPath = EntityNameHelper.FormatDeadLetterPath("consumerdb.profile.update/ConsumerDB.Profile.Update.CRM");
            return new MessageReceiver(_config.Connection, deadLetterPath,ReceiveMode.PeekLock, RetryPolicy.Default, 0);
        }
       
        #endregion

    }
}
