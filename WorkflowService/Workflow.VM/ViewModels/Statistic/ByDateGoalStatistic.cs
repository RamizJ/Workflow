using System;

namespace Workflow.VM.ViewModels.Statistic
{
    public class ByDateGoalStatistic : GoalStatisticResult
    {
        public DateTime Date { get; set; }

        public ByDateGoalStatistic(DateTime date, int[] goalsCountForState)
        {
            Date = date;
            GoalsCountForState = goalsCountForState;
        }
    }
}