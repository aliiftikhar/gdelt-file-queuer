using GdeltFilesQueuer.Core.Services.QueueService;
using GdeltFilesQueuer.Core.UseCases.QueueFilesForDownload;
using GdeltFilesQueuer.Providers.Services.QueueServices.AzureStorageQueue;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GdeltFilesQueuer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var services = new ServiceCollection();
            ConfigureServices(services, configuration);

            var serviceProvider = services.BuildServiceProvider();

            await QueueFilesForDownload(configuration, serviceProvider.GetService<IQueueFilesForDownloadHandler>());
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddLogging(builder =>
            {
                builder.AddConsole();
            });

            services.Configure<AzureStorageQueueConfiguration>(configuration.GetSection("AzureStorageQueue"));

            services.AddTransient<IQueueService, AzureStorageQueueService>();
            services.AddTransient<IQueueFilesForDownloadHandler, QueueFilesForDownloadHandler>();
        }

        private static async Task QueueFilesForDownload(IConfiguration configuration, IQueueFilesForDownloadHandler queueFilesForDownloadHandler)
        {
            DateTime startDate;
            DateTime.TryParse(configuration.GetValue<string>("QueueFilesConfig:StartDate"), out startDate);

            DateTime endDate;
            DateTime.TryParse(configuration.GetValue<string>("QueueFilesConfig:EndDate"), out endDate);

            var request = new QueueFilesForDownloadRequest
            {
                StartDate = startDate,
                EndDate = endDate,
            };

            await queueFilesForDownloadHandler.Handle(request);
        }
    }
}