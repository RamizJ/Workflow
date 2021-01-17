using Workflow.DAL.Models;

namespace Workflow.VM.ViewModels
{
    public class VmGoalState
    {
        public int GoalId { get; set; }
        public GoalState GoalState { get; set; }
        public string Comment { get; set; }
    }
}