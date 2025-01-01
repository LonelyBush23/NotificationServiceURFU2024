using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NotificationQueue.Application.Features.Notification;
using NotificationQueue.Application.Infrastructure.Cqs;
using NotificationQueue.Domain.Enums;
using RabbitMQ.Client;

namespace NotificationQueue.Infrastructure.RabbitMQ;

public class RabbitMqService : IRabbitMqService
{
    private readonly ConnectionFactory _factory;

    public RabbitMqService()
    {
        _factory = new ConnectionFactory()
        {
            HostName = "localhost",
            UserName = "admin",
            Password = "admin"
        };
    }

    public async Task<bool> SendMessageAsync(SendNotificationCommand command, CancellationToken cancellationToken = default)
    {
        string queue;
        Console.WriteLine(command.ToString());
        switch (command.Channel)
        {
            case NotificationChannel.Telegram:
                queue = "Telegram";
                break;
            case NotificationChannel.SMS:
                queue = "SMS";
                break;
            case NotificationChannel.Email:
                queue = "Email";
                break;
            default:
                throw new NotSupportedException("Channel type not supported");
        }
        var message = JsonSerializer.Serialize(command);
        Console.WriteLine(message);
        return await SendMessageAsync(message, queue, cancellationToken);
    }

    public async Task<bool> SendMessageAsync(string message, string queue, CancellationToken cancellationToken = default)
    {
        try
        {
            var connection = await _factory.CreateConnectionAsync(cancellationToken);
            await using var channel = await connection.CreateChannelAsync(null, cancellationToken);
            await channel.QueueDeclareAsync(queue: queue,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken);

            var body = Encoding.UTF8.GetBytes(message);
            await channel.BasicPublishAsync("", queue, false, body, cancellationToken);
            return true;
        }
        catch (Exception exception)
        {
            return false;
        }
    }
}