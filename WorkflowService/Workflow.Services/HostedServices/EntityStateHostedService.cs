using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BackgroundServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Workflow.Services.HostedServices
{
    /// <inheritdoc />
    public class EntityStateHostedService : QueuedHostedService<EntityStateMessage>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public EntityStateHostedService(
            IBackgroundTaskQueue<EntityStateMessage> queue, 
            IServiceProvider serviceProvider, 
            ILogger<QueuedHostedService<EntityStateMessage>> logger)
            : base(queue, serviceProvider, logger)
        {
        }

        /// <inheritdoc />
        protected override async Task Process(IServiceScope scope, 
            EntityStateMessage data, 
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
    

    public class EntityStateMessage
    {
        public string SenderId { get; set; }
        public IEnumerable<object> EntityIds { get; set; }

        public string EntityType { get; set; }
        public EntityOperation Operation { get; set; }
    }

    public enum EntityOperation
    {
        Create,
        Update,
        Delete
    }
}
