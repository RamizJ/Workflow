using System.Threading;
using System.Threading.Tasks;

namespace BackgroundServices
{
    public interface IBackgroundTaskQueue<T>
    {
        void Enqueue(T data);

        Task<T> DequeueAsync(CancellationToken cancellationToken);
    }
}