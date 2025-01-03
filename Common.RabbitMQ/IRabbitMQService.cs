using RabbitMQ.Client;

namespace Common.RabbitMQ
{
    public interface IRabbitMQService
    {
        Task<IChannel> GetChannelAsync(CancellationToken cancellationToken = default);

        Task DeclareQueueAsync(string queue, IChannel channel, CancellationToken cancellationToken = default);

        Task SubscribeToQueueAsync(string queue, Func<string, Task> messageHandler, CancellationToken cancellationToken = default);

        Task PublishMessageAsync(string queue, byte[] body, CancellationToken cancellationToken = default);

        Task PublishMessageAsync(string exchange, string routingKey, byte[] body, CancellationToken cancellationToken = default);

        Task DisposeAsync();
    }
}
