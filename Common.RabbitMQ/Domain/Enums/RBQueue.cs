using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.RabbitMQ.Domain.Enums
{
    public enum RBQueue
    {
        ProcessedQueue,
        DeadLetterQueue,
        TelegramQueue,
        EmailQueue
    }
}
