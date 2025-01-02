using RabbitMQ.Client;

namespace NotificationQueue.Infrastructure.RabbitMQ.Base;

public interface IRabbitMQService
{
  Task<IChannel> GetChannel(CancellationToken cancellationToken = default);

  Task DeclareQueue(IChannel channel, string queue, CancellationToken cancellationToken = default);

  Task SubscribeToQueue(string queue, Func<string, Task> messageHandler, CancellationToken cancellationToken = default);
}
