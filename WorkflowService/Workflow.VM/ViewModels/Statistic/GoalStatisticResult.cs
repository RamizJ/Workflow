namespace Workflow.VM.ViewModels.Statistic
{
    public class GoalStatisticResult
    {
        public int GoalsCompletedInTimeCount { get; set; }
        public int GoalsCompletedLateCount { get; set; }
        public int GoalsInProcessCount { get; set; }
        public int GoalsNotCompletedCount { get; set; }
        public int[] GoalsCountForState { get; set; }
    }
}