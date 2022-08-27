namespace App.Services;

public class AzureQueueService : IAzureQueueService
{
    private readonly IAzureQueueClient _client;

    public AzureQueueService(IAzureQueueClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        await _client.CreateQueueAsync(cancellationToken);
        await _client.EnqueueMessageAsync($"Message {Random.Shared.NextInt64()}", cancellationToken);
        await _client.DequeueMessageAsync(cancellationToken);
        await _client.DeleteQueueAsync(cancellationToken);
    }
}