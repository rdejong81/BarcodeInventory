using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace BarWinInventory.Utility
{
    public class EventBusService
    {
        private readonly EventBusPublisher publisher;
        private readonly EventBusMessageHandler messageHandler;

        private readonly IConnection connection;
        private readonly IModel channel;

        public EventBusService(SerialControl serialControl)
        {
            var rabbitHostName = Environment.GetEnvironmentVariable("RABBIT_HOSTNAME");

            var connectionFactory = new ConnectionFactory
            {
                HostName = rabbitHostName ?? "localhost",
                Port = 5672
                // UserName = "ops0",
                // Password = "ops0"
            };
            this.connection = connectionFactory.CreateConnection();
            this.channel = connection.CreateModel();
            this.messageHandler = new EventBusMessageHandler(channel, serialControl);
            this.publisher = new EventBusPublisher(channel);
            messageHandler.StartListening();
        }

        public IModel GetChannel()
        {
            return channel;
        }

        public void Publish(string exchange, string payload)
        {
            publisher.Publish(exchange, payload);
        }
    }
}
