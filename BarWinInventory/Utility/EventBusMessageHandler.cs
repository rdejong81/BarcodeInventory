using BarWinInventory.Hubs;
using BarWinInventory.Models;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BarWinInventory.Utility
{
    public class EventBusMessageHandler
    {
        private readonly IModel channel;
        private SerialControl serialControl;

        public EventBusMessageHandler(IModel channel, SerialControl serialControl)
        {
            this.serialControl = serialControl;
            this.channel = channel;
        }

        public void StartListening()
        {
            Subscribe("scanner-response", HandleScanResult);
        }

        public delegate void OnMessage(string str);

        private void Subscribe(string exchange, OnMessage callback)
        {
            channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Fanout);
            var queueName = exchange + ".inventory";
            channel.QueueDeclare(queueName);
            Console.WriteLine(queueName);

            // Messages published to exchange, should be directed to our queue
            channel.QueueBind(queue: queueName,
                exchange: exchange,
                routingKey: "");

            Console.WriteLine(" [*] Waiting for logs.");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, eventArgs) =>
            {
                byte[] body = eventArgs.Body.ToArray();
                string message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [string] {0}", message);
                callback(message);
            };
            channel.BasicConsume(
                queue: queueName,
                autoAck: true,
                consumer: consumer);
        }

        private void HandleScanResult(string message)
        {
            ScannerResultMessage scannerMessage = JsonConvert.DeserializeObject<ScannerResultMessage>(message);
            char[] s = new char[5];
            switch (scannerMessage.ScannerResult)
            {
                case ScannerResult.UnknownSku:
                    {            
                        s[0] = 'c';
                        s[1] = (char)255;
                        s[2] = (char)0;
                        s[3] = (char)0;
                        s[4] = '\r';
                        serialControl.SendSerial(s);
                    }
                    break;
                case ScannerResult.AddedToStock:
                    {
                        s[0] = 'c';
                        s[1] = (char)0;
                        s[2] = (char)255;
                        s[3] = (char)0;
                        s[4] = '\r';
                        serialControl.SendSerial(s);
                    }
                    break;
                case ScannerResult.UnknownError:
                    {
                        s[0] = 'c';
                        s[1] = (char)255;
                        s[2] = (char)165;
                        s[3] = (char)0;
                        s[4] = '\r';
                        serialControl.SendSerial(s);
                    }
                    break;
            }
            serialControl.WriteLog(scannerMessage.Barcode + " : " + scannerMessage.ScannerResult.ToString());

            Thread.Sleep(4000);
            // clear leds after 4 seconds.
            s[0] = 'c';
            s[1] = (char)0;
            s[2] = (char)0;
            s[3] = (char)0;
            s[4] = '\r';
            serialControl.SendSerial(s);
        }
    }
}
