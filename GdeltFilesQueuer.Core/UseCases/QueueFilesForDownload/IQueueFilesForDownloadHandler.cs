using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GdeltFilesQueuer.Core.UseCases.QueueFilesForDownload
{
    public interface IQueueFilesForDownloadHandler
    {
        Task Handle(QueueFilesForDownloadRequest request);
    }
}
