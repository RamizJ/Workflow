namespace Workflow.VM.ViewModels
{
    public class VmProjectUserRole
    {
        public int ProjectId { get; set; }
        public string UserId { get; set; }

        public bool CanEditUsers { get; set; }
        public bool CanEditGoals { get; set; }
        public bool CanCloseGoals { get; set; }
    }
}