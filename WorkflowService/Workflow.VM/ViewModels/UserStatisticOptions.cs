using System;
using Workflow.DAL.Models;

namespace Workflow.VM.ViewModels
{
    public class UserStatistic
    {
        public int[] GoalsCountForState { get; set; }
        public ByDateStatistic[] ByDateStatistics { get; set; }

        public UserStatistic()
        {
            var statesCount = Enum.GetValues(typeof(GoalState)).Length;
            GoalsCountForState = new int[statesCount];
        }
    }
}