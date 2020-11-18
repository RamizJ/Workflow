using System;

namespace Workflow.VM.ViewModels
{
    public class ByDateStatistic
    {
        public DateTime Date { get; set; }
        public int[] GoalCountForState { get; set; }


        public ByDateStatistic(DateTime date, int[] goalCountForState)
        {
            Date = date;
            GoalCountForState = goalCountForState;
        }
    }
}