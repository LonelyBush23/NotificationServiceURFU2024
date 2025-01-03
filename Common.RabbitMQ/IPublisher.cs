using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.RabbitMQ
{
    public interface IPublisher
    {
        Task<bool> PublishMessageAsync<T>(T message, string exchange, string routingKey, CancellationToken cancellationToken = default);
    }
}
