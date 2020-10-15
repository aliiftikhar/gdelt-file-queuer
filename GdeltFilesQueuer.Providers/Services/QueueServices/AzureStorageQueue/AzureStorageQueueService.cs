using Azure.Storage.Queues;
using GdeltFilesQueuer.Core.Services.QueueService;
using Microsoft.Extensions.Options;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace GdeltFilesQueuer.Providers.Services.QueueServices.AzureStorageQueue
{
    public class AzureStorageQueueService : IQueueService
    {
        private readonly AzureStorageQueueConfiguration azureStorageQueueConfiguration;
        private readonly QueueClient queueClient;

        public AzureStorageQueueService(IOptions<AzureStorageQueueConfiguration> azureStorageQueueConfiguration)
        {
            this.azureStorageQueueConfiguration = azureStorageQueueConfiguration?.Value ?? throw new ArgumentNullException(nameof(azureStorageQueueConfiguration));

            if (string.IsNullOrWhiteSpace(this.azureStorageQueueConfiguration.ConnectionString))
                throw new ArgumentNullException(nameof(this.azureStorageQueueConfiguration.ConnectionString));

            if (string.IsNullOrWhiteSpace(this.azureStorageQueueConfiguration.QueueName))
                throw new ArgumentNullException(nameof(this.azureStorageQueueConfiguration.QueueName));

            queueClient = new QueueClient(this.azureStorageQueueConfiguration.ConnectionString, this.azureStorageQueueConfiguration.QueueName);
        }

        public async Task Queue(Message message)
        {
            await queueClient.SendMessageAsync(JsonSerializer.Serialize(message));
        }
    }
}
