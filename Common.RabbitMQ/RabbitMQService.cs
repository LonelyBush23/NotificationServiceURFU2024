﻿using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Common.RabbitMQ
{
    public class RabbitMQService : IRabbitMQService
    {
        private ConnectionFactory _factory;
        private IConnection? _connection;
        private IChannel? _channel;

        public RabbitMQService()
        {
            _factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "admin",
                Password = "admin"
            };
        }

        public async Task DeclareQueueAsync(string queue, IChannel channel, CancellationToken cancellationToken = default)
        {
            await channel.QueueDeclareAsync(queue: queue,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken);
        }

        public async Task<IChannel> GetChannelAsync(CancellationToken cancellationToken = default)
        {
            if (_connection == null || !_connection.IsOpen)
            {
                _connection = await _factory.CreateConnectionAsync(cancellationToken);
            }

            if (_channel == null || _channel.IsClosed)
            {
                _channel = await _connection.CreateChannelAsync();
            }
            return _channel;
        }


        public async Task SubscribeToQueueAsync(string queue, Func<string, Task> messageHandler, CancellationToken cancellationToken = default)
        {
            try
            {
                var channel = await GetChannelAsync();
                var consumer = new AsyncEventingBasicConsumer(channel);

                consumer.ReceivedAsync += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(message);
                    try
                    {
                        await messageHandler(message);
                        await channel.BasicAckAsync(ea.DeliveryTag, false);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing message: {ex.Message}");
                        await channel.BasicNackAsync(ea.DeliveryTag, false, true);
                    }
                };

                await channel.BasicConsumeAsync(queue: queue, autoAck: false, consumer: consumer);

                Console.WriteLine($"Listening on queue: {queue}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing consumer: {ex.Message}");
            }
        }

        public async Task PublishMessageAsync(string queue, byte[] body, CancellationToken cancellationToken = default) {
            var channel = await GetChannelAsync(cancellationToken);
            await DeclareQueueAsync(queue, channel, cancellationToken);
            await channel.BasicPublishAsync("", queue, false, body, cancellationToken);
        }


        public async Task PublishMessageAsync(string exchange, string routingKey, byte[] body, CancellationToken cancellationToken = default)
        {
            var channel = await GetChannelAsync(cancellationToken);
            // Объявляем Exchange (если он не существует)
            await channel.ExchangeDeclareAsync(exchange: exchange, type: "direct", durable: true);
            // Создаем очередь (временное решение)
            await DeclareQueueAsync(routingKey, channel, cancellationToken);
            // Магия перенаправления
            await channel.QueueBindAsync(queue: routingKey, exchange: exchange, routingKey: routingKey);

            await channel.BasicPublishAsync(exchange, routingKey, body, cancellationToken);

            await Task.CompletedTask; // Условный асинхронный вызов
        }


        public async Task DisposeAsync()
        {
            if (_channel != null && _channel.IsOpen)
            {
                await _channel.CloseAsync();
                _channel.Dispose();
                _channel = null;
            }

            if (_connection != null && _connection.IsOpen)
            {
                await _connection.CloseAsync();
                _connection.Dispose();
                _connection = null;
            }
        }
    }
}