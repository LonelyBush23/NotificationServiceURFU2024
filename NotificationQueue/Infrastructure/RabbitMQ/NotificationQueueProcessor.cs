namespace NotificationQueue.Infrastructure.RabbitMQ
{
    public class NotificationQueueProcessor : IHostedService
    {
        private readonly RabbitMqConsumer _consumer;

        public NotificationQueueProcessor(RabbitMqConsumer consumer)
        {
            _consumer = consumer;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            //    _consumer.Consume("sms_queue", async message =>
            //    {
            //        Console.WriteLine($"Processing SMS: {message}");
            //        // Save status to database here
            //        await Task.CompletedTask;
            //    });

            //    _consumer.Consume("telegram_queue", async message =>
            //    {
            //        Console.WriteLine($"Processing Telegram: {message}");
            //        // Save status to database here
            //        await Task.CompletedTask;
            //    });

            //    _consumer.Consume("email_queue", async message =>
            //    {
            //        Console.WriteLine($"Processing Email: {message}");
            //        // Save status to database here
            //        await Task.CompletedTask;
            //    });

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

    }
}
