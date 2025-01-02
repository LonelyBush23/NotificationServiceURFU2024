using RabbitMQ.Client;

namespace NotificationQueue.Infrastructure.RabbitMQ.Base;

public interface IRabbitMQConnection
{
  Task<IConnection> GetConnection(CancellationToken cancellationToken = default);
}
