using System;

namespace Workflow.VM.ViewModels
{
    /// <summary>
    /// Проект
    /// </summary>
    public class VmProject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string OwnerId { get; set; }
        public string OwnerFio { get; set; }
        public DateTime CreationDate { get; set; }
        public int? GroupId { get; set; }
        public string GroupName { get; set; }
        public bool IsRemoved { get; set; }
    }
}
