using RabbitMQ.Client;

namespace Common.RabbitMQ
{
    public interface IRabbitMQService
    {
        Task<IChannel> GetChannelAsync(CancellationToken cancellationToken = default);
        Task StartUp(string exchange, string[] queueNames, CancellationToken cancellationToken = default);
        // Task SubscribeToQueueAsync(string queue, Func<string, Task> messageHandler, CancellationToken cancellationToken = default);
        Task SubscribeToQueueAsync(string queue, Func<string, Task> messageHandler, CancellationToken cancellationToken = default, int maxRetryCount = 5);
        Task SubsribeToDLX(string dlxQueue);
        Task PublishMessageAsync(string exchange, string routingKey, string queueName, byte[] body, CancellationToken cancellationToken = default);
        Task DisposeAsync();
    }
}
