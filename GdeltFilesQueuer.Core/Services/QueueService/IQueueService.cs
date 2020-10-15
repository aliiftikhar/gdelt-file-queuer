using System.Threading.Tasks;

namespace GdeltFilesQueuer.Core.Services.QueueService
{
    public interface IQueueService
    {
        Task Queue(Message message);
    }
}
