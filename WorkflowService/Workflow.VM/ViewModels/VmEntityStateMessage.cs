using System.Collections.Generic;

namespace Workflow.VM.ViewModels
{
    public class VmEntityStateMessage
    {
        public string SenderId { get; set; }
        public IEnumerable<object> EntityIds { get; set; }

        public string EntityType { get; set; }
        public EntityOperation Operation { get; set; }


        public VmEntityStateMessage(string senderId, 
            IEnumerable<object> entityIds, 
            string entityType, 
            EntityOperation operation)
        {
            SenderId = senderId;
            EntityIds = entityIds;
            EntityType = entityType;
            Operation = operation;
        }
    }

    public enum EntityOperation
    {
        Create,
        Update,
        Delete
    }
}