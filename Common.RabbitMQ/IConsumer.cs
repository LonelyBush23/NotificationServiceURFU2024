

namespace Common.RabbitMQ
{
    public interface IConsumer
    {
        Task ListenAsync<T>(Func<T, Task> messageHandler, CancellationToken cancellationToken = default);
        Task ListenAsync<T>(string queue, Func<T, Task> messageHandler, CancellationToken cancellationToken = default);
    }
}
