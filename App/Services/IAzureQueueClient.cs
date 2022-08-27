namespace App.Services
{
    public interface IAzureQueueClient
    {
        Task CreateQueueAsync(CancellationToken cancellationToken = default);
        Task EnqueueMessageAsync(string message, CancellationToken cancellationToken = default);
        Task DequeueMessageAsync(CancellationToken cancellationToken = default);
        Task DeleteQueueAsync(CancellationToken cancellationToken = default);
    }
}
