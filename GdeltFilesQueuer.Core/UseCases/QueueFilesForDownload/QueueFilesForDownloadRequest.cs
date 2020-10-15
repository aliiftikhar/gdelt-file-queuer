using System;

namespace GdeltFilesQueuer.Core.UseCases.QueueFilesForDownload
{
    public class QueueFilesForDownloadRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
