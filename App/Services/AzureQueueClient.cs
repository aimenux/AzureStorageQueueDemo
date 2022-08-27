using Azure.Storage.Queues;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace App.Services;

public class AzureQueueClient : IAzureQueueClient
{
    private readonly IOptions<Settings> _options;
    private readonly QueueClient _client;
    private readonly ILogger _logger;

    private static readonly TimeSpan NeverExpireTimeToLive = TimeSpan.FromSeconds(-1);

    public AzureQueueClient(IOptions<Settings> options, ILogger logger)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _client = new QueueClient(_options.Value.ConnectionString, _options.Value.QueueName);
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task CreateQueueAsync(CancellationToken cancellationToken = default)
    {
        var response = await _client.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
        if (response.IsError)
        {
            throw new Exception($"Failed to create queue '{_options.Value.QueueName}'");
        }

        _logger.LogInformation("Queue '{name}' created", _options.Value.QueueName);
    }

    public async Task EnqueueMessageAsync(string message, CancellationToken cancellationToken = default)
    {
        var response = await _client.SendMessageAsync(message, timeToLive: NeverExpireTimeToLive, cancellationToken: cancellationToken);
        _logger.LogInformation("Message '{id}' added to queue '{name}'", response.Value.MessageId, _options.Value.QueueName);
    }

    public async Task DequeueMessageAsync(CancellationToken cancellationToken = default)
    {
        var receiveResponse = await _client.ReceiveMessageAsync(cancellationToken: cancellationToken);
        if (receiveResponse.Value == null)
        {
            _logger.LogInformation($"No message(s) found in queue '{_options.Value.QueueName}'");
        }
        else
        {
            var deleteResponse = await _client.DeleteMessageAsync(receiveResponse.Value.MessageId, receiveResponse.Value.PopReceipt, cancellationToken);
            if (deleteResponse.IsError)
            {
                throw new Exception($"Failed to delete message '{receiveResponse.Value.MessageId}' from queue '{_options.Value.QueueName}'");
            }

            _logger.LogInformation("Message '{id}' deleted from queue '{name}'", receiveResponse.Value.MessageId, _options.Value.QueueName);
        }
    }

    public async Task DeleteQueueAsync(CancellationToken cancellationToken = default)
    {
        var response = await _client.DeleteIfExistsAsync(cancellationToken: cancellationToken);
        if (!response.Value)
        {
            throw new Exception($"Failed to delete queue '{_options.Value.QueueName}'");
        }

        _logger.LogInformation("Queue '{name}' deleted", _options.Value.QueueName);
    }
}