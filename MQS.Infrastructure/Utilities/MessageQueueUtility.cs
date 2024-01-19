using log4net;
using MQS.Application.Utilities;
using MQS.Infrastructure.Data;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQS.Infrastructure.Utilities
{
    public class MessageQueueUtility : IMessageQueueUtility
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(MessageQueueUtility));
        public void SendMessage(IModel channel, string exchange, string routingKey, string message)
        {
            byte[] messageBodyBytes = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange, routingKey, null, messageBodyBytes);
            LogMessages.AddMessage($"{message}");
        }

        public void ConsumeMessages(IModel channel, string queueName, string prefixMessage)
        {
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, args) =>
            {
                Task.Delay(TimeSpan.FromSeconds(2)).Wait();
                var body = args.Body.ToArray();
                string message = Encoding.UTF8.GetString(body);
                LogMessages.AddMessage($"{prefixMessage}. Received Message: {message}");
                channel.BasicAck(args.DeliveryTag, false);
            };

            channel.BasicQos(0, 1, false);
            string consumerTag = channel.BasicConsume(queueName, false, consumer);
        }

        public void SharedConsumeMessages(IModel orderChannel, string orderQueue, IModel inventoryChannel,
            string inventoryQueue, int sharedConsumerNumber)
        {
            var orderConsumer = new EventingBasicConsumer(orderChannel);
            var inventoryConsumer = new EventingBasicConsumer(inventoryChannel);

            int orderMessageCount = GetMessageCount(orderChannel, orderQueue);
            int inventoryMessageCount = GetMessageCount(inventoryChannel, inventoryQueue);

            orderConsumer.Received += (sender, args) =>
            {
                Task.Delay(TimeSpan.FromSeconds(1)).Wait();
                var body = args.Body.ToArray();
                string message = Encoding.UTF8.GetString(body);

                if (orderMessageCount >= inventoryMessageCount)
                {
                    LogMessages.AddMessage($"📦🛍️ Order has been received on shared consumer. Received Message: {message}");
                    orderChannel.BasicAck(args.DeliveryTag, false);
                }
                else
                {
                    orderChannel.BasicNack(args.DeliveryTag, false, true);
                }

                orderMessageCount = GetMessageCount(orderChannel, orderQueue);
                inventoryMessageCount = GetMessageCount(inventoryChannel, inventoryQueue);
            };

            inventoryConsumer.Received += (sender, args) =>
            {
                Task.Delay(TimeSpan.FromSeconds(1)).Wait();
                var body = args.Body.ToArray();
                string message = Encoding.UTF8.GetString(body);

                if (inventoryMessageCount > orderMessageCount)
                {
                    LogMessages.AddMessage($"📦🛍️ Inventory update message has been received on shared " +
                        $"consumer {sharedConsumerNumber}. Received Message: {message}");
                    inventoryChannel.BasicAck(args.DeliveryTag, false);
                }
                else
                {
                    inventoryChannel.BasicNack(args.DeliveryTag, false, true);
                }

                orderMessageCount = GetMessageCount(orderChannel, orderQueue);
                inventoryMessageCount = GetMessageCount(inventoryChannel, inventoryQueue);
            };

            orderChannel.BasicQos(0, 1, false);
            inventoryChannel.BasicQos(0, 1, false);

            orderChannel.BasicConsume(orderQueue, false, orderConsumer);
            inventoryChannel.BasicConsume(inventoryQueue, false, inventoryConsumer);
        }

        public int GetMessageCount(IModel channel, string queueName)
        {
            try
            {
                var queueDeclare = channel.QueueDeclarePassive(queueName);
                return (int)queueDeclare.MessageCount;
            }
            catch (RabbitMQ.Client.Exceptions.OperationInterruptedException ex)
            {
                _logger.Error("Error: ", ex);
                return 0;
            }
        }
    }
}
