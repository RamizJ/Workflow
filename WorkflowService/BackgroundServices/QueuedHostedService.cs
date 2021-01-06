using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BackgroundServices
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class QueuedHostedService<T> : BackgroundService
    {
        public IBackgroundTaskQueue<T> Queue { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        protected QueuedHostedService(
            IBackgroundTaskQueue<T> queue,
            IServiceProvider serviceProvider,
            ILogger<QueuedHostedService<T>> logger)
        {
            Queue = queue;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            await base.StopAsync(stoppingToken);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await BackgroundProcessing(stoppingToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="data"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected abstract Task Process(IServiceScope scope, T data, CancellationToken cancellationToken);
        
        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                T data = await Queue.DequeueAsync(stoppingToken);

                try
                {
                    await Process(_serviceProvider.CreateScope(), data, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                }
            }
        }


        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<QueuedHostedService<T>> _logger;
    }
}