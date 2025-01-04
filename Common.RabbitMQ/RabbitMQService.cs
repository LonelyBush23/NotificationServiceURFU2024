using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Runtime.InteropServices;
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
                HostName = "rabbitmq",
                Port = 5672,
                UserName = "admin",
                Password = "admin"
            };
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

        public async Task StartUp(string exchange, string[] queueNames, CancellationToken cancellationToken = default)
        {
            var channel = await GetChannelAsync(cancellationToken);

            await channel.ExchangeDeclareAsync(exchange: "dlx", type: ExchangeType.Fanout);
            await channel.QueueDeclareAsync(queue: "DeadLetterQueue", durable: true, exclusive: false, autoDelete: false);
            await channel.QueueBindAsync(queue: "DeadLetterQueue", exchange: "dlx", routingKey: "");

            await channel.ExchangeDeclareAsync(exchange: exchange, type: ExchangeType.Direct, durable: true, cancellationToken: cancellationToken);

            foreach (string queue in queueNames)
            {
                await channel.QueueDeclareAsync(queue: queue,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: new Dictionary<string, object?>
            {
                { "x-dead-letter-exchange", "dlx" },
                { "x-message-ttl", 30000 }
            },
                cancellationToken: cancellationToken);
                await channel.QueueBindAsync(queue: queue, exchange: exchange, routingKey: queue, cancellationToken: cancellationToken);
            }
        }

        public async Task SubsribeToDLX(string dlxQueue)
        {
            try
            {
                var channel = await GetChannelAsync();
                var consumer = new AsyncEventingBasicConsumer(channel);

                consumer.ReceivedAsync += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var headers = ea.BasicProperties.Headers;
                    
                    var exchange = headers["exchange"].ToString();
                    var routingKey = headers["routing-keys"].ToString();
                    var queue = headers["queue"].ToString();

                    // Console.WriteLine($"Error processing message: {ex.Message}");

                    Console.WriteLine($"Количество попыток: {GetRetryCound(ea.BasicProperties)}");

                    Thread.Sleep(1000);

                    try
                    {
                        await PublishMessageAsync(exchange: exchange, routingKey: routingKey, queueName: queue, body: body);
                        await channel.BasicAckAsync(ea.DeliveryTag, false);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing message");
                    }
                };

                await channel.BasicConsumeAsync(queue: dlxQueue, autoAck: false, consumer: consumer);

                Console.WriteLine($"Listening on queue: {dlxQueue}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing consumer: {ex.Message}");
            }
        }

        public async Task SubscribeToQueueAsync(string queue, Func<string, Task> messageHandler, CancellationToken cancellationToken = default, int maxRetryCount = 5)
        {
            try
            {
                var channel = await GetChannelAsync();
                var consumer = new AsyncEventingBasicConsumer(channel);

                consumer.ReceivedAsync += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var header = ea.BasicProperties;
                    Console.WriteLine(message);
                    try
                    {
                        await messageHandler(message);
                        await channel.BasicAckAsync(ea.DeliveryTag, false);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing message: {ex.Message}");

                        Console.WriteLine($"Количество попыток: {GetRetryCound(ea.BasicProperties)}");

                        if (GetRetryCound(ea.BasicProperties) <= maxRetryCount)
                        {
                            await channel.BasicNackAsync(ea.DeliveryTag, false, false);
                        }
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

        static int GetRetryCound(IReadOnlyBasicProperties properties)
        {
            var headers = properties.Headers;
            int retryCount = headers != null && headers.ContainsKey("x-death")
                ? Convert.ToInt32(headers["x-death"])
                : 0;

            retryCount++;

            return retryCount;
        }


        public async Task PublishMessageAsync(string exchange, string routingKey, string queueName, byte[] body, CancellationToken cancellationToken = default)
        {
            var channel = await GetChannelAsync(cancellationToken);
            await channel.BasicPublishAsync(exchange, routingKey, body, cancellationToken);
        }

        public async Task PublishMessageAsync(string exchange, string routingKey, byte[] body, CancellationToken cancellationToken = default)
        {
            var channel = await GetChannelAsync(cancellationToken);
            await channel.ExchangeDeclarePassiveAsync(exchange);
            await channel.QueueDeclarePassiveAsync(routingKey);
            await channel.BasicPublishAsync(exchange, routingKey, body, cancellationToken);
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