﻿using System;
using System.Runtime.InteropServices;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Framing.v0_9_1;
using RabbitMQ.Client.MessagePatterns;

namespace RabbitMQ
{
    [ComVisible(true), Guid("8ECF7EBC-BC37-4DD6-BF6E-E6152E543635")]
    [ProgId("UltravioletCatastrophe.RabbitMQ")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class Connection
    {
        private const byte Transient = 1;
        private const byte Persistent = 2;

        private IConnection _connection;
        private IModel _channel;

        // amqp://user:pass@hostName:port/vhost

        public void Connect(string uri)
        {
            if (_connection != null) Disconnect();
            _connection = new ConnectionFactory {Uri = uri}.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public void CreateDirectQueue(string exchange, string routingKey,
            string queue, bool durable, bool exclusive, bool autoDelete)
        {
            CreateQueue(exchange, ExchangeType.Direct, routingKey, queue, durable, exclusive, autoDelete);
        }

        public void CreateFanoutQueue(string exchange, string routingKey,
            string queue, bool durable, bool exclusive, bool autoDelete)
        {
            CreateQueue(exchange, ExchangeType.Fanout, routingKey, queue, durable, exclusive, autoDelete);
        }

        public void CreateTopicQueue(string exchange, string routingKey,
            string queue, bool durable, bool exclusive, bool autoDelete)
        {
            CreateQueue(exchange, ExchangeType.Topic, routingKey, queue, durable, exclusive, autoDelete);
        }

        private void CreateQueue(string exchange, string type, string routingKey,
            string queue, bool durable, bool exclusive, bool autoDelete)
        {
            _channel.ExchangeDeclare(exchange, type);
            _channel.QueueDeclare(queue, durable, exclusive, autoDelete, null);
            _channel.QueueBind(queue, exchange, routingKey);
        }

        public void Publish(string exchange, string routingKey, bool persistent, string message)
        {
            _channel.BasicPublish(exchange, routingKey, new BasicProperties
                    { DeliveryMode = persistent ? Persistent : Transient }, 
                Encoding.UTF8.GetBytes(message));
        }

        public string Call(string queue, string message)
        {
            return Encoding.UTF8.GetString(new SimpleRpcClient(_channel, queue) 
                { TimeoutMilliseconds = 5000 }.Call(Encoding.UTF8.GetBytes(message)));
        }

        public void Disconnect()
        {
            _channel.Dispose();
            _connection.Dispose();
        }
    }
}