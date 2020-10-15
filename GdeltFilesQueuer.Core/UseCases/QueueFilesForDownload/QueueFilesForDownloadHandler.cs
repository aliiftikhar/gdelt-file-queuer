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
            ValidateRequest(request);

            string downloadUrlFormat = "http://data.gdeltproject.org/events/{0}.export.CSV.zip";

            var tasks = new List<Task>();

            for(var date = request.StartDate; date <= request.EndDate; date = date.AddDays(1))
            {
                string downloadUrl = string.Format(downloadUrlFormat, $"{GenerateFileName(date)}");

                var message = new Message
                {
                    DownloadUrl = downloadUrl
                };

                tasks.Add(this.queueService.Queue(message));

                this.logger.LogInformation($"Message Queued: {downloadUrl}");
            }

            await Task.WhenAll(tasks.ToArray());
        }

        private void ValidateRequest(QueueFilesForDownloadRequest request)
        {
            if(request == null)
                throw new ArgumentNullException(nameof(request));

            if(request.StartDate == null || request.StartDate == DateTime.MinValue)
                throw new ArgumentNullException(nameof(request.StartDate));

            if (request.EndDate == null || request.EndDate == DateTime.MinValue)
                throw new ArgumentNullException(nameof(request.EndDate));

            if (request.StartDate > request.EndDate)
                throw new ArgumentException("Start Date caan not be greater than End Date");
        }

        private string GenerateFileName(DateTime date)
        {
            if (date > new DateTime(2013, 3, 31))
            {
                return date.ToString("yyyyMMdd");
            }
            else if (date > new DateTime(2005, 12, 31))
            {
                return date.ToString("yyyyMM");
            }
            else
            {
                return date.ToString("yyyy");
            }
        }
    }
}
