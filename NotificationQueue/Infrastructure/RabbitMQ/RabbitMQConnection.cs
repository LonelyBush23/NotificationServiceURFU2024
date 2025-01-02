using NotificationQueue.Infrastructure.RabbitMQ.Base;
using RabbitMQ.Client;

namespace NotificationQueue.Infrastructure.RabbitMQ;

public class RabbitMQConnection : IRabbitMQConnection
{
  private readonly ConnectionFactory _factory;

public RabbitMQConnection()
  {
    _factory = new ConnectionFactory()
    {
      HostName = "localhost",
      UserName = "admin",
      Password = "admin"
    };
  }

  public async Task<IConnection> GetConnection(CancellationToken cancellationToken = default) => await _factory.CreateConnectionAsync(cancellationToken);
}
