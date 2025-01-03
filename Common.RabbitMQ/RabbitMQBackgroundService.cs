using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.RabbitMQ
{
    public class RabbitMQBackgroundService<T> : BackgroundService
    {
        private readonly IConsumer _consumer;
        private readonly Func<T, Task> _messageHandler;

        public RabbitMQBackgroundService(IConsumer consumer, Func<T, Task> messageHandler)
        {
            _consumer = consumer;
            _messageHandler = messageHandler;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _consumer.ListenAsync(_messageHandler, stoppingToken);
        }
    }
}
