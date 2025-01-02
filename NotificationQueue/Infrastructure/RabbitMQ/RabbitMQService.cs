using System.Text;
using NotificationQueue.Infrastructure.RabbitMQ.Base;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NotificationQueue.Infrastructure.RabbitMQ;

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

  public async Task<IChannel> GetChannel(CancellationToken cancellationToken = default)
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

  public async Task DeclareQueue(IChannel channel, string queue, CancellationToken cancellationToken = default)
  {
    await channel.QueueDeclareAsync(queue: queue,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken);
  }

  public async Task SubscribeToQueue(string queue, Func<string, Task> messageHandler, CancellationToken cancellationToken = default)
  {
    try
    {
      var channel = await GetChannel();
      var consumer = new AsyncEventingBasicConsumer(channel);

      consumer.ReceivedAsync += async (model, ea) =>
      {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);

        try
        {
          await messageHandler(message);
          // сохранение в БД об успехе
          await channel.BasicAckAsync(ea.DeliveryTag, false);
        }
        catch (Exception ex)
        {
          Console.WriteLine($"Error processing message: {ex.Message}");
          // сохранение в БД об неудаче
          // Optionally, you can use BasicNack to requeue or discard the message
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
