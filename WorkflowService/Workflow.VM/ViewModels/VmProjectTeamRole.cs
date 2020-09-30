namespace Workflow.VM.ViewModels
{
    public class VmProjectTeamRole
    {
        public int ProjectId { get; set; }
        public int TeamId { get; set; }

        public bool CanEditUsers { get; set; }
        public bool CanEditGoals { get; set; }
        public bool CanCloseGoals { get; set; }
    }
}