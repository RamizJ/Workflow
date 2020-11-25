namespace Workflow.VM.ViewModels
{
    public class VmGoalMessage
    {
        public int Id { get; set; }
        public string Text { get; set; }

        public int GoalId { get; set; }
        public string GoalTitle { get; set; }

        public string OwnerId { get; set; }
        public string OwnerOwnerFio { get; set; }
    }
}