using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQS.Application.Utilities
{
    public interface IMessageQueueUtility
    {
        void SendMessage(IModel channel, string exchange, string routingKey, string message);
        void ConsumeMessages(IModel channel, string queueName, string prefix);
        void SharedConsumeMessages(IModel orderChannel, string orderQueue, IModel inventoryChannel,
            string inventoryQueue, int sharedConsumerNumber);
    }
}
