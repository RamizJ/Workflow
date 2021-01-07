using System;
using System.Threading;
using System.Threading.Tasks;
using BackgroundServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Workflow.Services.Abstract;
using Workflow.VM.ViewModels;

namespace Workflow.Services.HostedServices
{
    /// <inheritdoc />
    public class EntityStateHostedService : QueuedHostedService<VmEntityStateMessage>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public EntityStateHostedService(
            IBackgroundTaskQueue<VmEntityStateMessage> queue,
            IServiceProvider serviceProvider, 
            ILogger<QueuedHostedService<VmEntityStateMessage>> logger)
            : base(queue, serviceProvider, logger)
        {
        }

        /// <inheritdoc />
        protected override async Task Process(IServiceScope scope, 
            VmEntityStateMessage data, 
            CancellationToken cancellationToken)
        {
            var notifier = scope.ServiceProvider.GetRequiredService<IEntityStateNotifierService>();
            await notifier.Notify(data);
        }
    }
}
