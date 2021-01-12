using System.Collections.Generic;

namespace Workflow.VM.ViewModels
{
    public class VmEntityStateMessage
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
        Delete,
        Restore
    }
}