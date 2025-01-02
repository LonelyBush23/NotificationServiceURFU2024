using NotificationQueue.Infrastructure.RabbitMQ.Base;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace NotificationQueue.Infrastructure.RabbitMQ
{
    public class RabbitMqConsumer
    {
        private readonly IRabbitMQService _connection;

        public RabbitMqConsumer(IRabbitMQService connection)
        {
            _connection = connection;
        }

        public async Task ConsumeAsync(string queue, Func<string, Task> messageHandler, CancellationToken cancellationToken = default)
        {
            // try
            // {
            //     var connection = await _connection.GetConnection(cancellationToken);
            //     await using var channel = await connection.CreateChannelAsync(null, cancellationToken);

            //     await channel.QueueDeclareAsync(
            //         queue: queue,
            //         durable: false,
            //         exclusive: false,
            //         autoDelete: false,
            //         arguments: null,
            //         cancellationToken: cancellationToken
            //     );

            //     var consumer = new AsyncEventingBasicConsumer(channel);

            //     consumer.ReceivedAsync += async (model, ea) =>
            //     {
            //         var body = ea.Body.ToArray();
            //         var message = Encoding.UTF8.GetString(body);

            //         try
            //         {
            //             await messageHandler(message);
            //             await channel.BasicAckAsync(ea.DeliveryTag, false);
            //         }
            //         catch (Exception ex)
            //         {
            //             Console.WriteLine($"Error processing message: {ex.Message}");
            //             // Optionally, you can use BasicNack to requeue or discard the message
            //             await channel.BasicNackAsync(ea.DeliveryTag, false, true);
            //         }
            //     };

            //     await channel.BasicConsumeAsync(queue: queue, autoAck: false, consumer: consumer);

            //     Console.WriteLine($"Listening on queue: {queue}");

            //     // Keep the channel alive
            //     while (!cancellationToken.IsCancellationRequested)
            //     {
            //         await Task.Delay(100, cancellationToken);
            //     }
            // }
            // catch (Exception ex)
            // {
            //     Console.WriteLine($"Error initializing consumer: {ex.Message}");
            // }
        }
    }
}
