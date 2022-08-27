namespace App.Services;

public interface IAzureQueueService
{
    Task RunAsync(CancellationToken cancellationToken = default);
}