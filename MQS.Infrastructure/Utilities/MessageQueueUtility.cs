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
        public void SendMessage(IModel channel, string exchange, string routingKey, string message)
        {
            byte[] messageBodyBytes = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange, routingKey, null, messageBodyBytes);
            LogMessages.AddMessage($"{message}");
        }

        public void ConsumeMessages(IModel channel, string queueName, string prefix)
        {
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, args) =>
            {
                Task.Delay(TimeSpan.FromSeconds(2)).Wait();
                var body = args.Body.ToArray();
                string message = Encoding.UTF8.GetString(body);
                LogMessages.AddMessage($"{prefix}. Received Message: { message }");
                channel.BasicAck(args.DeliveryTag, false);
            };

            channel.BasicQos(0, 1, false);
            string consumerTag = channel.BasicConsume(queueName, false, consumer);
        }

        public void SharedConsumeMessages(IModel orderChannel, string orderQueue, IModel inventoryChannel,
            string inventoryQueue, int sharedConsumerNumber)
        {
            var orderConsumer = new EventingBasicConsumer(orderChannel);
            orderConsumer.Received += (sender, args) =>
            {
                Task.Delay(TimeSpan.FromSeconds(1)).Wait();
                var body = args.Body.ToArray();
                string message = Encoding.UTF8.GetString(body);
                LogMessages.AddMessage($"📦🛍️ Order has been received on channel 1, consumer {sharedConsumerNumber}. Received Message: {message}");
                orderChannel.BasicAck(args.DeliveryTag, false);
            };

            var inventoryConsumer = new EventingBasicConsumer(inventoryChannel);
            inventoryConsumer.Received += (sender, args) =>
            {
                Task.Delay(TimeSpan.FromSeconds(1)).Wait();
                var body = args.Body.ToArray();
                string message = Encoding.UTF8.GetString(body);
                LogMessages.AddMessage($"📦🛍️ Order has been received on channel 2 consumer {sharedConsumerNumber}. Received Message: {message}");
                inventoryChannel.BasicAck(args.DeliveryTag, false);
            };

            orderChannel.BasicQos(0, 1, false);
            inventoryChannel.BasicQos(0, 1, false);

            orderChannel.BasicConsume(orderQueue, false, orderConsumer);
            inventoryChannel.BasicConsume(inventoryQueue, false, inventoryConsumer);
        }
    }
}
