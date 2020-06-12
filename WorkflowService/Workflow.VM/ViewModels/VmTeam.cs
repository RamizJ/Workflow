namespace Workflow.VM.ViewModels
{
    public class VmTeam
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? GroupId { get; set; }
        public string GroupName { get; set; }
        public bool IsRemoved { get; set; }

    }
}
