using System;

namespace Workflow.VM.ViewModels
{
    /// <summary>
    /// Область задач (например проект содержащий задачи)
    /// </summary>
    public class VmScope
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string OwnerId { get; set; }
        public string OwnerFio { get; set; }
        public int? TeamId { get; set; }
        public string TeamName { get; set; }
        public DateTime CreationDate { get; set; }
        public int? GroupId { get; set; }
        public string GroupName { get; set; }
        public bool IsRemoved { get; set; }
    }
}
