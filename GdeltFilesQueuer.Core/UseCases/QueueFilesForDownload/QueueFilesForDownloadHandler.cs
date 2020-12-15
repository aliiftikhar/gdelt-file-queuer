using GdeltFilesQueuer.Core.Services.QueueService;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GdeltFilesQueuer.Core.UseCases.QueueFilesForDownload
{
    public class QueueFilesForDownloadHandler : IQueueFilesForDownloadHandler
    {
        private readonly ILogger<QueueFilesForDownloadHandler> logger;
        private readonly IQueueService queueService;

        public QueueFilesForDownloadHandler(ILogger<QueueFilesForDownloadHandler> logger, IQueueService queueService)
        {
            this.logger = logger;
            this.queueService = queueService;
        }

        public async Task Handle(QueueFilesForDownloadRequest request)
        {
            LogMessage("QueueFilesForDownloadRequest started");

            ValidateRequest(request);

            string downloadUrlFormat = "http://data.gdeltproject.org/events/{0}.export.CSV.zip";

            var tasks = new List<Task>();

            for(var date = request.StartDate; date <= request.EndDate; date = date.AddDays(1))
            {
                string downloadUrl = string.Format(downloadUrlFormat, $"{date:yyyyMMdd}");

                var message = new QueueMessage
                {
                    DownloadUrl = downloadUrl
                };

                tasks.Add(this.queueService.Queue(message));
            }

            LogMessage($"Number of Messages Queued: {tasks.Count}");

            await Task.WhenAll(tasks.ToArray());

            LogMessage("QueueFilesForDownloadRequest ended");
        }

        private void LogMessage(string message)
        {
            this.logger.LogInformation($"{DateTime.UtcNow} - {message}");
        }

        private void ValidateRequest(QueueFilesForDownloadRequest request)
        {
            if(request == null)
                throw new ArgumentNullException(nameof(request));

            if(request.StartDate == null || request.StartDate == DateTime.MinValue)
                throw new ArgumentNullException(nameof(request.StartDate));

            if (request.EndDate == null || request.EndDate == DateTime.MinValue)
                throw new ArgumentNullException(nameof(request.EndDate));

            if(request.StartDate < new DateTime(2013, 04, 01))
                throw new ArgumentException("Start Date can not be less than 01APR2013");

            if (request.EndDate >= DateTime.Today)
                throw new ArgumentException("End Date should be less than today");

            if (request.StartDate > request.EndDate)
                throw new ArgumentException("Start Date can not be greater than End Date");
        }
    }
}
