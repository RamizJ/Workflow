using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace BackgroundServices
{
    public class BackgroundQueue<T> : IBackgroundTaskQueue<T>
    {
        public void Enqueue(T data)
        {
            _queue.Enqueue(data);
            _signal.Release();
        }

        public async Task<T> DequeueAsync(
            CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _queue.TryDequeue(out var data);

            return data;
        }


        private readonly ConcurrentQueue<T> _queue = new();
        private readonly SemaphoreSlim _signal = new(0);
    }
}