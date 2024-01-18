using MQS.Application.Services;
using MQS.Application.Utilities;
using MQS.Infrastructure.Data;
using MQS.Infrastructure.Utilities;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MQS.Infrastructure.Services
{
    public class MessageQueueService : IMessageQueueService
    {
        private static int orderNumber = 1;
        private static int inventoryNumber = 1;
        MessageQueueUtility _messageQueueUtility = new MessageQueueUtility();
        //private readonly IMessageQueueUtility _messageQueueUtility;

        //public MessageQueueService() { }

        //public MessageQueueService(IMessageQueueUtility messageQueueUtility)
        //{
        //    _messageQueueUtility = messageQueueUtility;
        //}

        public void ProcessMessages()
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri("amqp://guest:guest@localhost:5672"),
                ClientProvidedName = "Render App"
            };

            using (var connection = factory.CreateConnection())
            {
                using (var orderChannel = connection.CreateModel())
                using (var inventoryChannel = connection.CreateModel())
                {
                    string orderExchangeName = "OrderExchange";
                    string orderRoutingKey1 = "order-routing-key-1";
                    string orderQueueName1 = "OrderQueue1";
                    string orderRoutingKey2 = "order-routing-key-2";
                    string orderQueueName2 = "OrderQueue2";

                    string inventoryExchangeName = "InventoryExchange";
                    string inventoryRoutingKey1 = "inventory-routing-key-1";
                    string inventoryQueueName1 = "InventoryQueue1";
                    string inventoryRoutingKey2 = "inventory-routing-key-2";
                    string inventoryQueueName2 = "InventoryQueue2";

                    orderChannel.ExchangeDeclare(orderExchangeName, ExchangeType.Direct);
                    orderChannel.QueueDeclare(orderQueueName1, false, false, false, null);
                    orderChannel.QueueBind(orderQueueName1, orderExchangeName, orderRoutingKey1, null);
                    orderChannel.QueueDeclare(orderQueueName2, false, false, false, null);
                    orderChannel.QueueBind(orderQueueName2, orderExchangeName, orderRoutingKey2, null);

                    inventoryChannel.ExchangeDeclare(inventoryExchangeName, ExchangeType.Direct);
                    inventoryChannel.QueueDeclare(inventoryQueueName1, false, false, false, null);
                    inventoryChannel.QueueBind(inventoryQueueName1, inventoryExchangeName, inventoryRoutingKey1, null);
                    inventoryChannel.QueueDeclare(inventoryQueueName2, false, false, false, null);
                    inventoryChannel.QueueBind(inventoryQueueName2, inventoryExchangeName, inventoryRoutingKey2, null);

                    Task.Run(() =>
                    {
                        while (true)
                        {
                            string orderMessage = $"C1P1. Order #{orderNumber}";
                            _messageQueueUtility.SendMessage(orderChannel, orderExchangeName, orderRoutingKey1, orderMessage);
                            LogMessages.AddMessage($"Sent: {orderMessage}");
                            orderNumber++;
                            Thread.Sleep(2000);
                        }
                    });

                    Task.Run(() =>
                    {
                        while (true)
                        {
                            string orderMessage = $"C1P2. Order #{orderNumber}";
                            _messageQueueUtility.SendMessage(orderChannel, orderExchangeName, orderRoutingKey2, orderMessage);
                            LogMessages.AddMessage($"Sent: {orderMessage}");
                            orderNumber++;
                            Thread.Sleep(3000);
                        }
                    });

                    Task.Run(() =>
                    {
                        while (true)
                        {
                            string inventoryMessage = $"C2P1. Order #{inventoryNumber}";
                            _messageQueueUtility.SendMessage(inventoryChannel, inventoryExchangeName, inventoryRoutingKey1,
                                inventoryMessage);
                            LogMessages.AddMessage($"Sent: {inventoryMessage}");
                            inventoryNumber++;
                            Thread.Sleep(4000);
                        }
                    });

                    Task.Run(() =>
                    {
                        while (true)
                        {
                            string inventoryMessage = $"C2P2. Order #{inventoryNumber}";
                            _messageQueueUtility.SendMessage(inventoryChannel, inventoryExchangeName, inventoryRoutingKey2,
                                inventoryMessage);
                            LogMessages.AddMessage($"Sent: {inventoryMessage}");
                            inventoryNumber++;
                            Thread.Sleep(5000);
                        }
                    });

                    Task.Run(() =>
                    {
                        _messageQueueUtility.ConsumeMessages(orderChannel, orderQueueName1, "C1C1");
                    });

                    Task.Run(() =>
                    {
                        _messageQueueUtility.ConsumeMessages(orderChannel, orderQueueName2, "C1C2");
                    });

                    Task.Run(() =>
                    {
                        _messageQueueUtility.ConsumeMessages(inventoryChannel, inventoryQueueName1, "C2C1");
                    });

                    Task.Run(() =>
                    {
                        _messageQueueUtility.ConsumeMessages(inventoryChannel, inventoryQueueName2, "C2C2");
                    });

                    Task.Run(() =>
                    {
                        _messageQueueUtility.SharedConsumeMessages(orderChannel, orderQueueName2, inventoryChannel,
                            inventoryQueueName2, 2);
                    });

                    Console.ReadLine();
                }
            }
        }
    }
}
