using Common.RabbitMQ.Domain.Enums;
using RabbitMQ.Client;

namespace Common.RabbitMQ
{
    public class RabbitMQSetUp
    {
        private readonly IRabbitMQService _rabbitMQService;

        public RabbitMQSetUp(IRabbitMQService rabbitMQService) 
        {
            _rabbitMQService = rabbitMQService;
        }

        public async Task Configure(CancellationToken cancellationToken = default) 
        {
            var channel = await _rabbitMQService.GetChannelAsync(cancellationToken);
            await CreateBindDlx(channel, Exchange.DeadLetterExchange.ToString(), RBQueue.DeadLetterQueue.ToString());
            await CreateBind(channel, Exchange.ProcessedExchange.ToString(), [RBQueue.ProcessedQueue.ToString()]);
            await CreateBind(channel, Exchange.NotificationExchange.ToString(), [RBQueue.TelegramQueue.ToString(), RBQueue.EmailQueue.ToString()]);
        }

        private async Task CreateBindDlx(IChannel channel, string exchange, string queue, CancellationToken cancellationToken = default) 
        {
            await channel.ExchangeDeclareAsync(exchange: exchange, type: ExchangeType.Fanout);
            await channel.QueueDeclareAsync(queue: queue, durable: true, exclusive: false, autoDelete: false);
            await channel.QueueBindAsync(queue: queue, exchange: exchange, routingKey: "");
        }

        private async Task CreateBind(IChannel channel, string exchange, string[] queues, CancellationToken cancellationToken = default)
        {
            await channel.ExchangeDeclareAsync(exchange: exchange, type: ExchangeType.Direct, durable: true, cancellationToken: cancellationToken);

            foreach (string queue in queues)
            {
                await channel.QueueDeclareAsync(queue: queue,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: new Dictionary<string, object?>
                {
                    { "x-dead-letter-exchange", Exchange.DeadLetterExchange.ToString() },
                    { "x-message-ttl", 30000 }
                },
                cancellationToken: cancellationToken);
                await channel.QueueBindAsync(queue: queue, exchange: exchange, routingKey: queue, cancellationToken: cancellationToken);
            }
        }
    }
}
