using System;
using Workflow.DAL.Models;

namespace Workflow.VM.ViewModels.Statistic
{
    public class VmGoalStatistic : GoalStatisticResult
    {
        public ByDateGoalStatistic[] ByDateStatistics { get; set; }

        public VmGoalStatistic()
        {
            var statesCount = Enum.GetValues(typeof(GoalState)).Length;
            GoalsCountForState = new int[statesCount];
        }
    }
}