namespace Analytics.Core.Domain.Contracts.Infrastructure
{
    public interface IMessageBroker
    {
        Task PublishAsync<T>(T message);
        void StartConsuming(string queueName, Func<string, Task<bool>> messageHandler);
    }
}
