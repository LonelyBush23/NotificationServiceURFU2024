using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Common.RabbitMQ
{
    public class Consumer : IConsumer
    {
        private readonly IRabbitMQService _rabbitMQService;
        private readonly string[] _queues;

        public Consumer(IRabbitMQService rabbitMQService, string[] queues)
        {
            _rabbitMQService = rabbitMQService;
            _queues = queues;
        }

        public async Task ListenAsync<T>(Func<T, Task> messageHandler, CancellationToken cancellationToken = default)
        {
            var listeningTasks = _queues.Select(queue => ListenAsync(queue, messageHandler, cancellationToken));
            await Task.WhenAll(listeningTasks);
        }

        private async Task ListenAsync<T>(string queue, Func<T, Task> messageHandler, CancellationToken cancellationToken = default)
        {
            try
            {
                var channel = await _rabbitMQService.GetChannelAsync();
                var consumer = new AsyncEventingBasicConsumer(channel);

                consumer.ReceivedAsync += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var tMessage = JsonSerializer.Deserialize<T>(message)
                        ?? throw new ArgumentNullException(nameof(T), $"Failed to deserialize message {message}");

                    Console.WriteLine(message);

                    try
                    {
                        await messageHandler(tMessage);
                        await channel.BasicAckAsync(ea.DeliveryTag, false);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing message: {ex.Message}");

                        await channel.BasicNackAsync(ea.DeliveryTag, false, false);  
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

        Task IConsumer.ListenAsync<T>(string queue, Func<T, Task> messageHandler, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
