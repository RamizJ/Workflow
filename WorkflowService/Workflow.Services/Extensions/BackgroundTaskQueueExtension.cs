using System.Collections.Generic;
using System.Linq;
using BackgroundServices;
using Workflow.VM.ViewModels;

namespace Workflow.Services.Extensions
{
    public static class BackgroundTaskQueueExtension
    {
        public static void EnqueueId<T>(this IBackgroundTaskQueue<VmEntityStateMessage> queue,
            string senderId, T entityId, string entityType, EntityOperation operation,
            RequireClass<T> ignore = null) 
            where T : class
        {
            queue.Enqueue(new VmEntityStateMessage
            {
                SenderId = senderId,
                EntityIds = new object[] { entityId },
                EntityType = entityType,
                Operation = operation
            });
        }

        public static void EnqueueId<T>(this IBackgroundTaskQueue<VmEntityStateMessage> queue,
            string senderId, T entityId, string entityType, EntityOperation operation,
            RequireStruct<T> ignore = null)
            where T : struct
        {
            queue.Enqueue(new VmEntityStateMessage
            {
                SenderId = senderId,
                EntityIds = new object[] {entityId},
                EntityType = entityType,
                Operation = operation
            });
        }

        public static void EnqueueIds<T>(this IBackgroundTaskQueue<VmEntityStateMessage> queue,
            string senderId, IEnumerable<T> entityIds, string entityType, EntityOperation operation)
        {
            queue.Enqueue(new VmEntityStateMessage
            {
                SenderId = senderId,
                EntityIds = entityIds.Cast<object>(),
                EntityType = entityType,
                Operation = operation
            });
        }
    }


    public abstract class RequireStruct<T> where T : struct { }
    public abstract class RequireClass<T> where T : class { }
}
