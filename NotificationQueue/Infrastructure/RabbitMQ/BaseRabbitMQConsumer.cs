using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NotificationQueue.Infrastructure.RabbitMQ;

public class BaseRabbitMQConsumer(
    string rabbitUri,
    string queueName
  ) : IDisposable
{
  private IConnection connection;
  private IChannel channel;

  public string RabbitUri { get; } = rabbitUri;
  public string QueueName { get; } = queueName;

  public async Task ExecuteAsync(CancellationToken cancellationToken = default)
  {
    cancellationToken.ThrowIfCancellationRequested();

    var factory = new ConnectionFactory { Uri = new Uri(RabbitUri) };
    connection = await factory.CreateConnectionAsync();
    channel = await connection.CreateChannelAsync();

    await channel.QueueDeclareAsync(queue: QueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

    Console.WriteLine("[*] Waiting for messages.");

    var consumer = new AsyncEventingBasicConsumer(channel);
    consumer.ReceivedAsync += Handler;

    await channel.BasicConsumeAsync(QueueName, autoAck: true, consumer: consumer);
  }

  public virtual Task Handler(object model, BasicDeliverEventArgs args)
  {
    throw new NotImplementedException("");
  }

  public void Dispose()
  {
    channel.Dispose();
    connection.Dispose();
  }
}
