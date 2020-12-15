using System;
using Workflow.DAL.Models;

namespace Workflow.VM.ViewModels.Statistic
{
    public class VmProjectStatistic
    {
        public int[] GoalsCountForState { get; set; }
        public ByDateGoalStatistic[] ByDateStatistics { get; set; }

        public VmProjectStatistic()
        {
            var statesCount = Enum.GetValues(typeof(GoalState)).Length;
            GoalsCountForState = new int[statesCount];
        }
    }
}
